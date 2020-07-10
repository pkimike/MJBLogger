using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace MJBLogger
{
    internal static class LogSubject
    {
        internal const string Retention = nameof(Retention);
        internal const string Maintenance = nameof(Maintenance);
    }

    /// <summary>
    /// Defines various default settings
    /// </summary>
    public static class Defaults
    {
        /// <summary>
        /// The default file extension for log files (".log")
        /// </summary>
        public const string FileExtension = @".log";

        /// <summary>
        /// The default date stamp pattern used to generate log entry date stamps
        /// </summary>
        public const string DateStampPattern = @"MM/dd/yyyy";

        /// <summary>
        /// The default time stamp pattern used to generate log entry time stamps
        /// </summary>
        public const string TimeStampPattern = @"HH:mm:ss.fff";

        /// <summary>
        /// The default date stamp pattern used to name log directories when <see cref="MJBLog.storeByDate"/> is enabled.
        /// </summary>
        public const string StoreByDatePattern = @"dd-MMM-yyyy";

        /// <summary>
        /// The default character which is used to preceed and follow messages written using the <see cref="MJBLog.Banner(string)"/> method.
        /// </summary>
        public const char BannerChar = '=';

        /// <summary>
        /// The default number of characters a type descriptor can be on log messages. (20)
        /// </summary>
        public const int MaxTypeLength = 20;
        
        /// <summary>
        /// The default number of characters to preceed messages written using the <see cref="MJBLog.Echo(string, bool, LogLevel)"/> method.
        /// </summary>
        public const int EchoIndentLength = 10;

        /// <summary>
        /// The default number of characters each inner exception will be indented by when messages are written using the <see cref="MJBLog.Exception(Exception, string, bool, string)"/> method.
        /// </summary>
        public const int ExceptionIndentLength = 3;

        /// <summary>
        /// The number of times <see cref="MJBLog.BannerChar"/> will be repeated before and after messages written using the <see cref="MJBLog.Banner(string)"/> method.
        /// </summary>
        public const int BannerLength = 50;

        /// <summary>
        /// The default criticality threshold of new <see cref="MJBLog"/> objects if no value for <see cref="MJBLog.Level"/> is specified. (<see cref="LogLevel.Info"/>)
        /// </summary>
        public static readonly LogLevel Level = LogLevel.Info;


        internal const string InvalidNTFSChars = @"[<>:""/\|?*]";
        internal const string ExceptionMessage = @"An exception occurred";
        internal const long MaxFileSize = 5000000;
    }

    internal static class Context
    {
        internal static readonly string CallingAssembly = AppDomain.CurrentDomain.FriendlyName.Split('.')[0];
        internal static readonly string AssemblyDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
    }
}
