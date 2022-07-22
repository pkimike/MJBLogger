using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MJBLogger {
    /// <summary>
    /// Defines an MJBLog object
    /// </summary>
    public partial class MJBLog {
        StreamWriter stream;

        static DateTime CurDate,
            OldestDate;

        static Int64 maxFileSize = Default.MAX_FILE_SIZE;
        static Int32 maxTypeLength = Default.MAX_TYPE_LENGTH;
        static LogLevel lastMessageLevel = Default.LogLevel;

        String datePattern = Default.DATE_STAMP_PATTERN;
        String timePattern = Default.TIME_STAMP_PATTERN;

        String logName = Context.CallingAssembly;
        String logDirectory = String.Empty;
        String fileExtension = Default.FILE_EXTENSION;
        String storeByDatePattern = Default.STORE_BY_DATE_PATTERN;
        String logPath;

        Boolean useUtcTimestamps = false;
        Boolean storeByDate = false;
        Boolean disabled = false;
        Boolean cachedMode;

        Int32 fileIndex = 0;
        Int32 daysToKeep = 0;
        Int32 bannerLength = Default.BANNER_LENGTH;
        Int32 echoIndentLength = Default.ECHO_INDENT_LENGTH;
        Int32 exceptionIndentLength = Default.EXCEPTION_INDENT_LENGTH;

        LogLevel level = Default.LogLevel;

        MJBLog() {
            _cache = new Queue<String>();
        }

        public MJBLog(Boolean disabled) : base() {
            this.disabled = disabled;
        }

        public MJBLog(String LogName = null, String LogDirectory = null) : base() {
            fileIndex = 1;
            CurDate = Now;

            if (!String.IsNullOrEmpty(LogDirectory)) {
                this.LogDirectory = LogDirectory;
            } else {
                logDirectory = $"{Context.AssemblyDirectory}\\logs\\";
            }

            if (!String.IsNullOrEmpty(LogName)) {
                this.LogName = LogName;
            }

            SetLogFilePath();
        }

        public Boolean CachedMode {
            get => cachedMode;
            set {
                cachedMode = value;
                if (!cachedMode) {
                    writeCache();
                }
            }
        }

        /// <summary>
        /// Dictates the main element of the log file name. If not specified, the executing assembly name is used.
        /// </summary>
        public String LogName {
            get => logName;
            set {
                logName = value;
                SetLogFilePath();
            }
        }

        /// <summary>
        /// The directory where log files are to be stored. If not specified, the directory where the executing assembly is located will be used.
        /// </summary>
        public String LogDirectory {
            get => logDirectory;
            set {
                logDirectory = value;
                SetLogFilePath();
            }
        }

        /// <summary>
        /// Indicates the file extension that log files will be named with. Default value is defined by <see cref="Default.FILE_EXTENSION" />
        /// </summary>
        public String FileExtension {
            get => fileExtension;
            set => fileExtension = value[0] == '.' ? value : $".{value}";
        }

        /// <summary>
        /// Indicates the number of space characters that will proceed Echo messages in the log. Default value is defined by <see cref="Default.ECHO_INDENT_LENGTH" />
        /// </summary>
        public Int32 EchoIndentLength {
            get => echoIndentLength;
            set => echoIndentLength = Math.Abs(value);
        }

        /// <summary>
        /// A string of n space characters whose length is defined by <see cref="EchoIndentLength"/>
        /// </summary>
        public String EchoIndent => new String(' ', EchoIndentLength);

        /// <summary>
        /// Indicates the number of space characters with which to indent an inner exception stack trace. Default value is defined by <see cref="Default.EXCEPTION_INDENT_LENGTH" />
        /// </summary>
        public Int32 ExceptionIndentLength {
            get => exceptionIndentLength;
            set => exceptionIndentLength = Math.Abs(value);
        }

        /// <summary>
        /// Indicates whether log messages should also be written to the console window (only applicable to console applications). Disabled by default.
        /// </summary>
        public Boolean ConsoleEcho { get; set; } = false;

        /// <summary>
        /// Indicates at what criticality threshold log messages should be written to the console window. Log messages of lesser criticality are repressed. Only applicable to console applications and when <see cref="ConsoleEcho"/> is enabled. Default value is defined by <see cref="Default.LogLevel" />
        /// </summary>
        public LogLevel ConsoleEchoLevel { get; set; } = Default.LogLevel;


        /// <summary>
        /// When enabled, the calling class & method will be included with each log message. Default is <see cref="true"/>
        /// </summary>
        public Boolean IncludeCaller { get; set; } = true;

        /// <summary>
        /// When enabled, timestamps will use Coordinated Universal Time. Otherwise, timestamps will use the current timezone. Default is <see cref="false"/>
        /// </summary>
        public Boolean UseUtcTimestamps {
            get => useUtcTimestamps;
            set {
                useUtcTimestamps = value;
                SetLogFilePath();
            }
        }

        /// <summary>
        /// When enabled, log files will be stored in directories named by date. Default is <see cref="false"/>
        /// </summary>
        public Boolean StoreByDate {
            get => storeByDate;
            set {
                storeByDate = value;
                SetLogFilePath();
            }
        }

        /// <summary>
        /// Dictates the format of date stamps included with log entries. The default value is defined by <see cref="Default.DATE_STAMP_PATTERN"/>
        /// </summary>
        public String DatePattern {
            get => datePattern;
            set {
                try {
                    _ = DateTime.Now.ToString(value);
                    datePattern = value;
                } catch (Exception) {
                    throw new InvalidDateTimePatternException(value);
                }
            }
        }

        /// <summary>
        /// Dictates the format of time stamps included with log entries. The default value is defined by <see cref="Default.TIME_STAMP_PATTERN"/>
        /// </summary>
        public String TimePattern {
            get => timePattern;
            set {
                try {
                    _ = DateTime.Now.ToString(value);
                    timePattern = value;
                } catch (Exception) {
                    throw new InvalidDateTimePatternException(value);
                }
            }
        }

        /// <summary>
        /// Dictates the format of log directory names when <see cref="StoreByDate"/> is enabled. The default value is defined by <see cref="Default.STORE_BY_DATE_PATTERN"/>
        /// </summary>
        public String StoreByDatePattern {
            get => storeByDatePattern;
            set {
                try {
                    _ = DateTime.Now.ToString(value);
                    storeByDatePattern = value;
                } catch (Exception) {
                    throw new InvalidDateTimePatternException(value);
                }
            }
        }

        /// <summary>
        /// Indicates the number of days worth of log files to be retained. If set to 0, all log files will be retained. The default value is 0.
        /// </summary>
        public Int32 DaysToKeep {
            get => daysToKeep;
            set {
                daysToKeep = Math.Abs(value);
                if (daysToKeep > 0) {
                    DateTime now = UseUtcTimestamps ? DateTime.UtcNow : DateTime.Now;
                    OldestDate = now.AddDays(0 - daysToKeep);
                    CheckForOldDirectories();
                }
            }
        }

        /// <summary>
        /// Indicates the size, in megabytes each log file can grow to before a new log file is generated. The default value is 5 MB.
        /// </summary>
        public Int64 MaxSizeInMB {
            get => maxFileSize / 1000000;
            set => maxFileSize = Math.Abs(value) * 1000000;
        }

        /// <summary>
        /// Indicates the number of characters the entry type descriptor can be on each log entry. The default value is defined by <see cref="Default.MAX_TYPE_LENGTH"/>
        /// </summary>
        public Int32 MaxTypeLength {
            get => maxTypeLength;
            set => maxTypeLength = Math.Abs(value);
        }

        /// <summary>
        /// If enabled, a date stamp will be included with each log entry. Enabled by default.
        /// </summary>
        public Boolean IncludeDateStamps { get; set; } = true;

        /// <summary>
        /// If enabled, a time stamp will be included with each log entry. Enabled by default.
        /// </summary>
        public Boolean IncludeTimeStamps { get; set; } = true;

        /// <summary>
        /// Indicates the character which will be repeated before and after a log message written using the <see cref="Banner(String)"/> method. The default value is defined by <see cref="Default.BANNER_CHAR"/>
        /// </summary>
        public Char BannerChar { get; set; } = Default.BANNER_CHAR;

        /// <summary>
        /// Indicates the number of times <see cref="BannerChar"/> will be repeated before and after a log message written using the <see cref="Banner(String)"/> method. The default value is defined by <see cref="Default.BANNER_LENGTH"/>
        /// </summary>
        public Int32 BannerLength {
            get => bannerLength;
            set => bannerLength = Math.Abs(value);
        }

        /// <summary>
        /// Indicates the criticality threshold of messages which will be written to the log. Log messages which are of lower criticality will be omitted. The default value is defined by <see cref="Default.LogLevel"/>
        /// </summary>
        public LogLevel Level {
            get => level;
            set {
                level = value;
                disabled = level == LogLevel.None;
            }
        }

        /// <summary>
        /// If enabled, criticality labels written to each log entry will be shorter. Disabled by default.
        /// </summary>
        public Boolean UseChibiLevelLabels { get; set; } = false;

        String BannerBoarder => new String(BannerChar, BannerLength);

        DateTime Now => UseUtcTimestamps ? DateTime.UtcNow : DateTime.Now;

        Boolean OutTOConsole => ConsoleEcho && ConsoleEchoLevel.GE(lastMessageLevel);

        public void SetLevel(String sLevel) {
            level = LogLevel.Select(sLevel);
        }

        public String GetLevel(Boolean getChibiName = false) => getChibiName ? Level.ChibiName : Level.Name;

        String GetTimeStamp() {
            if (!(IncludeDateStamps || IncludeTimeStamps)) {
                return String.Empty;
            }

            StringBuilder expression = new StringBuilder(String.Empty);
            if (IncludeDateStamps) {
                expression.Append($"{Now.ToString(DatePattern)}");
            }

            if (IncludeTimeStamps) {
                expression.Append($"{(IncludeDateStamps ? " " : String.Empty)}{Now.ToString(TimePattern)}");
            }

            return $"{expression} ";
        }

        void SetLogFilePath() {
            String fullName,
                logDate,
                directoryPath,
                fullPath;

            fullName = $"{LogName}_{fileIndex}{FileExtension}";

            if (StoreByDate) {
                logDate = CurDate.ToString(StoreByDatePattern);
                directoryPath = Path.Combine(LogDirectory, logDate);
            } else {
                directoryPath = LogDirectory;
            }

            if (!CachedMode && !Directory.Exists(directoryPath)) {
                Directory.CreateDirectory(directoryPath);
            }

            fullPath = Path.Combine(directoryPath, fullName);
            if (File.Exists(fullPath)) {
                if (maxFileSize == 0 || new FileInfo(fullPath).Length < maxFileSize) {
                    logPath = fullPath;
                } else {
                    fileIndex++;
                    SetLogFilePath();
                }
            } else {
                logPath = fullPath;
            }
        }

        String ExceptionIndent(Int32 innerExceptions) => innerExceptions == 0
            ? String.Empty
            : new String(' ', ExceptionIndentLength * innerExceptions);
    }
}