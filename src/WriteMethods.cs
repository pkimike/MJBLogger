using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace MJBLogger
{
    public partial class MJBLog
    {
        #region Core Writing Methods

        private void WriteMessage(string message)
        {
            if (CachedMode)
            {
                cache.AppendLine(message);
            }
            else
            {
                contents.AppendLine(message);

                CheckSize();
                CheckDate();

                Stream = File.Exists(logPath) ? File.AppendText(logPath) : File.CreateText(logPath);
                Stream.WriteLine(message);
                Stream.Close();
            }

            if (OutTOConsole)
            {
                Console.ForegroundColor = MessageLevel.Color;
                Console.WriteLine(message);
                Console.ResetColor();
            }
        }

        private void StandardEntry(string text, string callingMethod, LogLevel EntryLevel)
        {
            if (disabled)
            {
                return;
            }

            string callerInfo = string.Empty;

            if (level.GE(EntryLevel))
            {
                MessageLevel = EntryLevel;
                callerInfo = $" [{$"{(IncludeCaller ? CallingClass(1) : string.Empty)}{callingMethod}".Truncate(MaxTypeLength - 1)}] ";
                WriteMessage($"{TimeStamp}{(UseChibiLevelLabels ? EntryLevel.ChibiLabel : EntryLevel.Label)}{callerInfo}{text}");
            }
        }

        #endregion

        #region Level Entries

        public void Critical(string text, [CallerMemberName] string callingMethod = "")
        {
            StandardEntry(text, callingMethod, LogLevel.Critical);
        }

        public void Error(string text, [CallerMemberName] string callingMethod = "")
        {
            StandardEntry(text, callingMethod, LogLevel.Error);
        }

        public void Warning(string text, [CallerMemberName] string callingMethod = "")
        {
            StandardEntry(text, callingMethod, LogLevel.Warning);
        }

        public void Info(string text, [CallerMemberName] string callingMethod = "")
        {
            StandardEntry(text, callingMethod, LogLevel.Info);
        }

        public void Verbose(string text, [CallerMemberName] string callingMethod = "")
        {
            StandardEntry(text, callingMethod, LogLevel.Verbose);
        }

        public void Diagnostic(string text, [CallerMemberName] string callingMethod = "")
        {
            StandardEntry(text, callingMethod, LogLevel.Diagnostic);
        }

        #endregion

        #region Echo Entries

        public void Echo(string text, bool indent = true, LogLevel level = null)
        {
            if (disabled)
            {
                return;
            }

            MessageLevel = level == null ? Defaults.Level : level;

            if (Level.GE(level))
            {
                WriteMessage($"{(indent ? EchoIndent : string.Empty)}{text}");
            }
        }

        public void Echo(Dictionary<string,string> tuples, bool indent = true, LogLevel level = null)
        {
            if (disabled)
            {
                return;
            }

            level ??= Defaults.Level;
            if (Level.GE(level ?? Defaults.Level))
            {
                int longestLabel = tuples.Keys.LongestStringLength() + 1;
                foreach(KeyValuePair<string,string> element in tuples)
                {
                    Echo($"{element.Key.PadRight(longestLabel)}: {element.Value}", indent, level);
                }
            }
        }

        public void PropertyReport(object obj, string message = null, bool indent=true, LogLevel level = null, [CallerMemberName] string callingMethod = "")
        {
            if (disabled)
            {
                return;
            }

            level ??= Defaults.Level;
            if (Level.GE(level))
            {
                Dictionary<string, string> tuples = new Dictionary<string, string>();
                Type type = obj.GetType();
                PropertyInfo[] properties = type.GetProperties();

                foreach(PropertyInfo property in properties)
                {
                    if (property.PropertyType == typeof(string))
                    {
                        tuples.Add(property.Name, property.GetValue(obj) as string);
                    }

                    if (property.PropertyType == typeof(int) || property.PropertyType == typeof(bool))
                    {
                        tuples.Add(property.Name, property.GetValue(obj).ToString());
                    }
                }

                StandardEntry(message ?? $"{type} properties:", callingMethod, level);
                Echo(tuples, indent, level);
            }
        }

        #endregion

        #region Exception Entries

        public void Exception(Exception ex, string message = null, bool includeInnerExceptions = true, [CallerMemberName] string callingMethod = "")
        {
            if (disabled || !Level.GE(LogLevel.Exception))
            {
                return;
            }

            MessageLevel = LogLevel.Exception;
            StringBuilder Expression = new StringBuilder($"{message ?? Defaults.ExceptionMessage}:\r\n");
            Expression.AppendLine(AppendException(ex, includeInnerExceptions));
            StandardEntry(Expression.ToString(), callingMethod, LogLevel.Exception);
        }

        private string AppendException(Exception ex, bool includeInnerExceptions, int innerExceptions = 0)
        {
            StringBuilder Expression = new StringBuilder();
            string indent = ExceptionIndent(innerExceptions);

            if (innerExceptions > 0)
            {
                Expression.AppendLine($"\r\n{indent}Inner Exception #{innerExceptions}:\r\n");
            }

            ex.ToString().SplitOnNewLine().ForEach(p => Expression.AppendLine($"{indent}{p}"));

            if (includeInnerExceptions && ex.InnerException != null)
            {
                Expression.AppendLine(AppendException(ex.InnerException, true, innerExceptions + 1));
            }

            return Expression.ToString();
        }

        #endregion

        #region Misc

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Banner(string message = null)
        {
            if (disabled)
            {
                return;
            }

            if (string.IsNullOrEmpty(message))
            {
                message = $"{Context.CallingAssembly} invoked by {Environment.UserDomainName}\\{Environment.UserName}";
            }

            WriteMessage($"\r\n{Divider}{message}{Divider}\r\n");
        }

        public void LineFeed(LogLevel level = null)
        {
            if (disabled)
            {
                return;
            }

            level ??= Defaults.Level;
            if (Level.GE(level))
            {
                MessageLevel = level;
                WriteMessage(string.Empty);
            }
        }

        public void Clear()
        {
            if (disabled)
            {
                return;
            }

            if (File.Exists(logPath))
            {
                File.WriteAllText(logPath, string.Empty);
            }
            contents = new StringBuilder(string.Empty);
        }

        #endregion
    }
}
