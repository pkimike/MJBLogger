using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MJBLogger
{
    public class MJBLog
    {
        private static StreamWriter Stream;
        private static DateTime CurDate,
                                OldestDate;

        private static long maxFileSize = Defaults.MaxFileSize;
        private static int maxTypeLength = Defaults.MaxTypeLength;
        private static LogLevel MessageLevel = Defaults.MessageLevel;

        private string logName = Context.CallingAssembly;
        private string logDirectory = string.Empty;

        private bool useUtcTimestamps = false;
        private bool cachedMode = false;
        private bool storeByDate = false;
        private bool disabled = false;

        private int fileIndex = 0;
        private int daysToKeep = 0;

        private StringBuilder cache = new StringBuilder(string.Empty);
        private StringBuilder contents = new StringBuilder(string.Empty);

        public string LogPath { get; private set; }

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

        public int EchoIndent { get; set; } = 10;
        public bool ConsoleEcho { get; set; } = false;
        public LogLevel ConsoleEchoLevel { get; set; } = LogLevel.Info;

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

        public string Cache
        {
            get
            {
                return cache.ToString();
            }
        }

        private void ClearCache()
        {
            throw new NotImplementedException();
        }

        private void SetLogFilePath()
        {
            throw new NotImplementedException();
        }
    }
}
