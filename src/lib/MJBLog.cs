using System;
using System.IO;
using System.Text;

namespace MJBLogger
{
    /// <summary>
    /// Defines an MJBLog object
    /// </summary>
    public partial class MJBLog
    {
        private static StreamWriter Stream;
        private static DateTime CurDate,
                                OldestDate;

        private static long maxFileSize = Defaults.MaxFileSize;
        private static int maxTypeLength = Defaults.MaxTypeLength;
        private static LogLevel MessageLevel = Defaults.Level;

        private string datePattern = Defaults.DateStampPattern;
        private string timePattern = Defaults.TimeStampPattern;

        private string logName = Context.CallingAssembly;
        private string logDirectory = string.Empty;
        private string fileExtension = Defaults.FileExtension;
        private string storeByDatePattern = Defaults.StoreByDatePattern;
        private string logPath;

        private bool useUtcTimestamps = false;
        private bool cachedMode = false;
        private bool storeByDate = false;
        private bool disabled = false;

        private int fileIndex = 0;
        private int daysToKeep = 0;
        private int bannerLength = Defaults.BannerLength;
        private int echoIndentLength = Defaults.EchoIndentLength;
        private int exceptionIndentLength = Defaults.ExceptionIndentLength;

        private LogLevel level = Defaults.Level;

        private StringBuilder cache = new StringBuilder(string.Empty);
        private StringBuilder contents = new StringBuilder(string.Empty);

        /// <summary>
        /// Dictates the main element of the log file name. If not specified, the executing assembly name is used.
        /// </summary>
        public string LogName
        {
            get
            {
                return logName;
            }
            set
            {
                CheckForInvalidNTFSChars(value);
                logName = value;
                SetLogFilePath();
            }
        }

        /// <summary>
        /// The directory where log files are to be stored. If not specified, the directory where the executing assembly is located will be used.
        /// </summary>
        public string LogDirectory
        {
            get
            {
                return logDirectory;
            }
            set
            {
                CheckForInvalidNTFSChars(value);
                logDirectory = value;
                SetLogFilePath();
            }
        }

        /// <summary>
        /// Indicates the file extension that log files will be named with. Default value is defined by <see cref="Defaults.FileExtension" />
        /// </summary>
        public string FileExtension
        {
            get
            {
                return fileExtension;
            }
            set
            {
                CheckForInvalidNTFSChars(value);
                fileExtension = value[0] == '.' ? value : $".{value}";
            }
        }

        /// <summary>
        /// Indicates the number of space characters that will proceed Echo messages in the log. Default value is defined by <see cref="Defaults.EchoIndentLength" />
        /// </summary>
        public int EchoIndentLength
        {
            get => echoIndentLength;
            set => echoIndentLength = Math.Abs(value);
        }

        /// <summary>
        /// A string of n space characters whose length is defined by <see cref="EchoIndentLength"/>
        /// </summary>
        public string EchoIndent
        {
            get
            {
                return new string(' ', EchoIndentLength);
            }
        }

        /// <summary>
        /// Indicates the number of space characters with which to indent an inner exception stack trace. Default value is defined by <see cref="Defaults.ExceptionIndentLength" />
        /// </summary>
        public int ExceptionIndentLength
        {
            get => exceptionIndentLength;
            set => exceptionIndentLength = Math.Abs(value);
        }

        /// <summary>
        /// Indicates whether log messages should also be written to the console window (only applicable to console applications). Disabled by default.
        /// </summary>
        public bool ConsoleEcho { get; set; } = false;

        /// <summary>
        /// Indicates at what criticality threshold log messages should be written to the console window. Log messages of lesser criticality are repressed. Only applicable to console applications and when <see cref="ConsoleEcho"/> is enabled. Default value is defined by <see cref="Defaults.Level" />
        /// </summary>
        public LogLevel ConsoleEchoLevel { get; set; } = Defaults.Level;

        /// <summary>
        /// When enabled, log messages will be stored in a StringBuilder object until <see cref="CachedMode"/> is disabled, at which time, all collected log messages will be written to the current log file. Enabling this property is useful if you would like to start logging but don't have the means to write to the file system
        /// </summary>
        public bool CachedMode
        {
            get
            {
                return cachedMode;
            }
            set
            {
                switch(cachedMode)
                {
                    case true:
                        if (!value)
                        {
                            ClearCache();
                        }
                        break;
                    default:
                        if (value)
                        {
                            cache = new StringBuilder(string.Empty);
                        }
                        break;
                }
                cachedMode = value;
            }
        }

        /// <summary>
        /// Contains all log messages which have not yet been written to the log file.
        /// </summary>
        public string Cache
        {
            get
            {
                return cache.ToString();
            }
        }

        /// <summary>
        /// When enabled, the calling class & method will be included with each log message. Default is <see cref="true"/>
        /// </summary>
        public bool IncludeCaller { get; set; } = true;

        /// <summary>
        /// When enabled, timestamps will use Coordinated Universal Time. Otherwise, timestamps will use the current timezone. Default is <see cref="false"/>
        /// </summary>
        public bool UseUtcTimestamps
        {
            get
            {
                return useUtcTimestamps;
            }
            set
            {
                useUtcTimestamps = value;
                SetLogFilePath();
            }
        }

        /// <summary>
        /// When enabled, log files will be stored in directories named by date. Default is <see cref="false"/>
        /// </summary>
        public bool StoreByDate
        {
            get
            {
                return storeByDate;
            }
            set
            {
                storeByDate = value;
                SetLogFilePath();
            }
        }

        /// <summary>
        /// Dictates the format of date stamps included with log entries. The default value is defined by <see cref="Defaults.DateStampPattern"/>
        /// </summary>
        public string DatePattern
        {
            get
            {
                return datePattern;
            }
            set
            {
                try
                {
                    _ = DateTime.Now.ToString(value);
                    datePattern = value;
                }
                catch (Exception)
                {
                    throw new InvalidDateTimePatternException(value);
                }
            }
        }

        /// <summary>
        /// Dictates the format of time stamps included with log entries. The default value is defined by <see cref="Defaults.TimeStampPattern"/>
        /// </summary>
        public string TimePattern
        {
            get
            {
                return timePattern;
            }
            set
            {
                try
                {
                    _ = DateTime.Now.ToString(value);
                    timePattern = value;
                }
                catch (Exception)
                {
                    throw new InvalidDateTimePatternException(value);
                }
            }
        }

        /// <summary>
        /// Dictates the format of log directory names when <see cref="StoreByDate"/> is enabled. The default value is defined by <see cref="Defaults.StoreByDatePattern"/>
        /// </summary>
        public string StoreByDatePattern
        {
            get
            {
                return storeByDatePattern;
            }
            set
            {
                try
                {
                    _ = DateTime.Now.ToString(value);
                    CheckForInvalidNTFSChars(value);
                    storeByDatePattern = value;
                }
                catch (Exception)
                {
                    throw new InvalidDateTimePatternException(value);
                }
            }
        }

        /// <summary>
        /// Indicates the number of days worth of log files to be retained. If set to 0, all log files will be retained. The default value is 0.
        /// </summary>
        public int DaysToKeep
        {
            get
            {
                return daysToKeep;
            }
            set
            {
                daysToKeep = Math.Abs(value);
                if (daysToKeep > 0)
                {
                    DateTime now = UseUtcTimestamps ? DateTime.UtcNow : DateTime.Now;
                    OldestDate = now.AddDays(0 - daysToKeep);
                    CheckForOldDirectories();
                }
            }
        }

        /// <summary>
        /// Indicates the size, in megabytes each log file can grow to before a new log file is generated. The default value is 5 MB.
        /// </summary>
        public long MaxSizeInMB
        {
            get
            {
                return maxFileSize / 1000000;
            }
            set
            {
                maxFileSize = Math.Abs(value) * 1000000;
            }
        }

        /// <summary>
        /// Indicates the number of characters the entry type descriptor can be on each log entry. The default value is defined by <see cref="Defaults.MaxTypeLength"/>
        /// </summary>
        public int MaxTypeLength
        {
            get
            {
                return maxTypeLength;
            }
            set
            {
                maxTypeLength = Math.Abs(value);
            }
        }

        /// <summary>
        /// If enabled, a date stamp will be included with each log entry. Enabled by default.
        /// </summary>
        public bool IncludeDateStamps { get; set; } = true;

        /// <summary>
        /// If enabled, a time stamp will be included with each log entry. Enabled by default.
        /// </summary>
        public bool IncludeTimeStamps { get; set; } = true;

        /// <summary>
        /// Indicates the character which will be repeated before and after a log message written using the <see cref="Banner(string)"/> method. The default value is defined by <see cref="Defaults.BannerChar"/>
        /// </summary>
        public char BannerChar { get; set; } = Defaults.BannerChar;

        /// <summary>
        /// Indicates the number of times <see cref="BannerChar"/> will be repeated before and after a log message written using the <see cref="Banner(string)"/> method. The default value is defined by <see cref="Defaults.BannerLength"/>
        /// </summary>
        public int BannerLength
        {
            get
            {
                return bannerLength;
            }
            set
            {
                bannerLength = Math.Abs(value);
            }
        }

        /// <summary>
        /// Indicates the criticality threshold of messages which will be written to the log. Log messages which are of lower criticality will be omitted. The default value is defined by <see cref="Defaults.Level"/>
        /// </summary>
        public LogLevel Level
        {
            get
            {
                return level;
            }
            set
            {
                level = value;
                disabled = level == LogLevel.None;
            }
        }

        /// <summary>
        /// If enabled, criticality labels written to each log entry will be shorter. Disabled by default.
        /// </summary>
        public bool UseChibiLevelLabels { get; set; } = false;

        private string Divider
        {
            get
            {
                return new string(BannerChar, BannerLength);
            }
        }

        private string TimeStamp
        {
            get
            {
                if (!(IncludeDateStamps || IncludeTimeStamps))
                {
                    return string.Empty;
                }
                else
                {
                    StringBuilder Expression = new StringBuilder(string.Empty);
                    if (IncludeDateStamps)
                    {
                        Expression.Append($"{Now.ToString(DatePattern)}");
                    }
                    if (IncludeTimeStamps)
                    {
                        Expression.Append($"{(IncludeDateStamps ? " " : string.Empty)}{Now.ToString(TimePattern)}");
                    }
                    return $"{Expression.ToString()} ";
                }
            }
        }

        private DateTime Now
        {
            get
            {
                return UseUtcTimestamps ? DateTime.UtcNow : DateTime.Now;
            }
        }

        private bool OutTOConsole
        {
            get
            {
                return ConsoleEcho && ConsoleEchoLevel.GE(MessageLevel);
            }
        }

        public MJBLog(bool disabled)
        {
            this.disabled = disabled;
        }

        public MJBLog(string LogName = null, string LogDirectory = null)
        {
            fileIndex = 1;
            CurDate = Now;

            if (!string.IsNullOrEmpty(LogDirectory))
            {
                this.LogDirectory = LogDirectory;
            }
            else
            {
                logDirectory = $"{Context.AssemblyDirectory}\\logs\\";
            }

            if (!string.IsNullOrEmpty(LogName))
            {
                this.LogName = LogName;
            }

            SetLogFilePath();
        }

        public void SetLevel(string sLevel)
        {
            level = LogLevel.Select(sLevel);
        }

        public string GetLevel(bool getChibiName = false) => getChibiName ? Level.ChibiName : Level.Name;

        public override string ToString() => contents.ToString();

        private void ClearCache()
        {
            WriteMessage(Cache.ToString());
            cache = new StringBuilder(string.Empty);
        }

        private void SetLogFilePath()
        {
            string fullName,
                   logDate,
                   directoryPath,
                   fullPath;

            fullName = $"{LogName}_{fileIndex}{FileExtension}";

            if (StoreByDate)
            {
                logDate = CurDate.ToString(StoreByDatePattern);
                directoryPath = Path.Combine(LogDirectory, logDate);
            }
            else
            {
                directoryPath = LogDirectory;
            }

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            fullPath = Path.Combine(directoryPath, fullName);
            if (File.Exists(fullPath))
            {
                if (maxFileSize == 0 || new FileInfo(fullPath).Length < maxFileSize)
                {
                    logPath = fullPath;
                }
                else
                {
                    fileIndex++;
                    SetLogFilePath();
                }
            }
            else
            {
                logPath = fullPath;
            }
        }

        private string ExceptionIndent(int innerExceptions)
        {
            return innerExceptions == 0 ? string.Empty : new string(' ', ExceptionIndentLength * innerExceptions);
        }
    }
}
