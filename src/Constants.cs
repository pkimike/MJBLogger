using System;
using System.Collections.Generic;
using System.Text;

namespace MJBLogger
{
    internal static class LogSubject
    {
        internal const string Retention = nameof(Retention);
        internal const string Maintenance = nameof(Maintenance);
    }

    internal static class Defaults
    {
        internal const string FileExtension = @".log";
        internal const string DatePattern = @"MM/dd/yyyy";
        internal const string TimePattern = @"HH:mm:ss.fff";
        internal const string InvalidNTFSChars = @"[<>:""/\|?*]";
        internal static string Bumper = new string('=', 50);

        internal const long MaxFileSize = 5000000;
        internal const int MaxTypeLength = 20;
        internal const int EchoIndent = 10;

        internal static readonly LogLevel MessageLevel = LogLevel.Info;
    }

    internal static class Context
    {
        internal static readonly string CallingAssembly = AppDomain.CurrentDomain.FriendlyName.Split('.')[0];
    }
}
