using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MJBLogger
{
    public interface IMJBLog
    {
        char BannerChar { get; set; }
        int BannerLength { get; set; }
        bool CachedMode { get; set; }
        int CacheTimeToLiveInMinutes { get; set; }
        bool ConsoleEcho { get; set; }
        LogLevel ConsoleEchoLevel { get; set; }
        string DatePattern { get; set; }
        int DaysToKeep { get; set; }
        string EchoIndent { get; }
        int EchoIndentLength { get; set; }
        int ExceptionIndentLength { get; set; }
        string FileExtension { get; set; }
        bool IncludeCaller { get; set; }
        bool IncludeDateStamps { get; set; }
        bool IncludeTimeStamps { get; set; }
        LogLevel Level { get; set; }
        string LogDirectory { get; set; }
        string LogName { get; set; }
        long MaxSizeInMB { get; set; }
        int MaxTypeLength { get; set; }
        bool StoreByDate { get; set; }
        string StoreByDatePattern { get; set; }
        string TimePattern { get; set; }
        bool UseChibiLevelLabels { get; set; }
        bool UseUtcTimestamps { get; set; }

        void Banner(string message = null);
        void Clear();
        void Critical(string text, string callingMethod = "");
        void Diagnostic(string text, string callingMethod = "");
        void Echo(Dictionary<string, string> tuples, bool indent = true, LogLevel level = null);
        void Echo(string text, bool indent = true, LogLevel level = null);
        void Error(string text, string callingMethod = "");
        void Exception(Exception ex, string message = null, bool includeInnerExceptions = true, [CallerMemberName] string callingMethod = "");
        string GetLevel(bool getChibiName = false);
        void Info(string text, string callingMethod = "");
        void LineFeed(LogLevel level = null);
        void PropertyReport(object obj, string message = null, bool indent = true, LogLevel level = null, [CallerMemberName] string callingMethod = "");
        void SetLevel(string sLevel);
        void Verbose(string text, string callingMethod = "");
        void Warning(string text, string callingMethod = "");
    }
}