using System;
using System.IO;
using System.Reflection;

namespace MJBLogger {
    internal static class LogSubject
    {
        internal const String Retention = nameof(Retention);
        internal const String Maintenance = nameof(Maintenance);
    }

    /// <summary>
    /// Defines various default settings
    /// </summary>
    static class Default
    {
        /// <summary>
        /// The default file extension for log files (".log")
        /// </summary>
        public const String FILE_EXTENSION = @".log";

        /// <summary>
        /// The default date stamp pattern used to generate log entry date stamps
        /// </summary>
        public const String DATE_STAMP_PATTERN = @"MM/dd/yyyy";

        /// <summary>
        /// The default time stamp pattern used to generate log entry time stamps
        /// </summary>
        public const String TIME_STAMP_PATTERN = @"HH:mm:ss.fff";

        /// <summary>
        /// The default date stamp pattern used to name log directories when <see cref="MJBLog.storeByDate"/> is enabled.
        /// </summary>
        public const String STORE_BY_DATE_PATTERN = @"dd-MMM-yyyy";

        /// <summary>
        /// The default character which is used to preceed and follow messages written using the <see cref="MJBLog.Banner(String)"/> method.
        /// </summary>
        public const Char BANNER_CHAR = '=';

        /// <summary>
        /// The default number of characters a type descriptor can be on log messages. (20)
        /// </summary>
        public const Int32 MAX_TYPE_LENGTH = 20;

        /// <summary>
        /// The default number of characters to preceed messages written using the <see cref="MJBLog.Echo(String, Boolean, MJBLogger.LogLevel)"/> method.
        /// </summary>
        public const Int32 ECHO_INDENT_LENGTH = 10;

        /// <summary>
        /// The default number of characters each inner exception will be indented by when messages are written using the <see cref="MJBLog.Exception(Exception, String, Boolean, String)"/> method.
        /// </summary>
        public const Int32 EXCEPTION_INDENT_LENGTH = 3;

        /// <summary>
        /// The number of times <see cref="MJBLog.BannerChar"/> will be repeated before and after messages written using the <see cref="MJBLog.Banner(String)"/> method.
        /// </summary>
        public const Int32 BANNER_LENGTH = 50;

        /// <summary>
        /// The default criticality threshold of new <see cref="MJBLog"/> objects if no value for <see cref="MJBLog.Level"/> is specified. (<see cref="MJBLogger.LogLevel.Info"/>)
        /// </summary>
        public static readonly LogLevel LogLevel = LogLevel.Info;


        internal const String EXCEPTION_MESSAGE = @"An exception occurred";
        internal const Int64 MAX_FILE_SIZE = 5000000;
    }

    static class Context {
        static Boolean gotAssemblyDirectory;
        static String assemblyDirectory;

        internal static readonly String CallingAssembly = AppDomain.CurrentDomain.FriendlyName.Split('.')[0];
        internal static String AssemblyDirectory
        {
            get {
                if (!gotAssemblyDirectory) {

                    try {
                        assemblyDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                    } catch {
                        assemblyDirectory = Environment.CurrentDirectory;
                    }

                    gotAssemblyDirectory = true;
                }

                return assemblyDirectory;
            }
        }
    }
}
