using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace MJBLogger;

public partial class MJBLog {
    const Int32 MAX_CACHED_MESSAGES = 500;

    readonly Queue<String> _cache;

    #region Core Writing Methods

    void WriteMessage(String message) {
        if (CachedMode) {
            cacheEntry(message);
        } else {
            checkSize();
            checkDate();

            stream = File.Exists(logPath) ? File.AppendText(logPath) : File.CreateText(logPath);
            stream.WriteLine(message);
            stream.Close();
        }

        if (OutTOConsole) {
            Console.ForegroundColor = lastMessageLevel.Color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }

    void cacheEntry(String message) {
        if (_cache.Count >= MAX_CACHED_MESSAGES) {
            _cache.Dequeue();
        }
        _cache.Enqueue(message);
    }

    void writeCache() {
        if (!_cache.Any()) {
            return;
        }

        var sb = new StringBuilder();
        while (_cache.Any()) {
            sb.AppendLine(_cache.Dequeue());
        }
        WriteMessage(sb.ToString());
    }

    void standardEntry(String text, String callingMethod, LogLevel EntryLevel) {
        if (disabled) {
            return;
        }

        var callerInfo = String.Empty;

        if (level.GE(EntryLevel)) {
            lastMessageLevel = EntryLevel;
            callerInfo =
                $" [{$"{(IncludeCaller ? CallingClass(1) : String.Empty)}{callingMethod}".Truncate(MaxTypeLength - 1)}] ";
            WriteMessage(
                $"{GetTimeStamp()}{(UseChibiLevelLabels ? EntryLevel.ChibiLabel : EntryLevel.Label)}{callerInfo}{text}");
        }
    }

    #endregion

    #region Level Entries

    /// <summary>
    /// Writes a message to the log of <see cref="LogLevel.Critical"/> criticality
    /// </summary>
    /// <param name="text">Expression to write to the log</param>
    /// <param name="callingMethod">If not specified, the calling method name</param>
    public void Critical(String text, [CallerMemberName] String callingMethod = "") {
        standardEntry(text, callingMethod, LogLevel.Critical);
    }

    /// <summary>
    /// Writes a message to the log of <see cref="LogLevel.Error"/> criticality
    /// </summary>
    /// <param name="text">Expression to write to the log</param>
    /// <param name="callingMethod">If not specified, the calling method name</param>
    public void Error(String text, [CallerMemberName] String callingMethod = "") {
        standardEntry(text, callingMethod, LogLevel.Error);
    }

    /// <summary>
    /// Writes a message to the log of <see cref="LogLevel.Warning"/> criticality
    /// </summary>
    /// <param name="text">Expression to write to the log</param>
    /// <param name="callingMethod">If not specified, the calling method name</param>
    public void Warning(String text, [CallerMemberName] String callingMethod = "") {
        standardEntry(text, callingMethod, LogLevel.Warning);
    }

    /// <summary>
    /// Writes a message to the log of <see cref="LogLevel.Info"/> criticality
    /// </summary>
    /// <param name="text">Expression to write to the log</param>
    /// <param name="callingMethod">If not specified, the calling method name</param>
    public void Info(String text, [CallerMemberName] String callingMethod = "") {
        standardEntry(text, callingMethod, LogLevel.Info);
    }

    /// <summary>
    /// Writes a message to the log of <see cref="LogLevel.Verbose"/> criticality
    /// </summary>
    /// <param name="text">Expression to write to the log</param>
    /// <param name="callingMethod">If not specified, the calling method name</param>
    public void Verbose(String text, [CallerMemberName] String callingMethod = "") {
        standardEntry(text, callingMethod, LogLevel.Verbose);
    }

    /// <summary>
    /// Writes a message to the log of <see cref="LogLevel.Diagnostic"/> criticality
    /// </summary>
    /// <param name="text">Expression to write to the log</param>
    /// <param name="callingMethod">If not specified, the calling method name</param>
    public void Diagnostic(String text, [CallerMemberName] String callingMethod = "") {
        standardEntry(text, callingMethod, LogLevel.Diagnostic);
    }

    #endregion

    #region Echo Entries

    /// <summary>
    /// Writes the specified expression to the log.
    /// </summary>
    /// <param name="text">The expression to write to the log</param>
    /// <param name="indent">If true the log entry will be indented.</param>
    /// <param name="level">The criticality threshold of the log message. If set to a lower criticality than the value of <see cref="Level"/> the message will be omitted. Default is <see cref="Default.LogLevel"/></param>
    public void Echo(String text, Boolean indent = true, LogLevel level = null) {
        if (disabled) {
            return;
        }

        lastMessageLevel = level == null ? Default.LogLevel : level;

        if (Level.GE(level)) {
            WriteMessage($"{(indent ? EchoIndent : String.Empty)}{text}");
        }
    }

    /// <summary>
    /// Writes a collection of key/value pairs to the log in a line break-separated, justified format.
    /// </summary>
    /// <param name="tuples">The collection of key/value pairs to be written to the log</param>
    /// <param name="indent">If true each written line will be indented.</param>
    /// <param name="level">The criticality threshold of the log message. If set to a lower criticality than the value of <see cref="Level"/> the message will be omitted. Default is <see cref="Default.LogLevel"/></param>
    public void Echo(Dictionary<String, String> tuples, Boolean indent = true, LogLevel level = null) {
        if (disabled) {
            return;
        }

        if (level == null) {
            level = Default.LogLevel;
        }
        if (Level.GE(level)) {
            var longestLabel = tuples.Keys.LongestStringLength() + 1;
            foreach (var element in tuples) {
                Echo($"{element.Key.PadRight(longestLabel)}: {element.Value}", indent, level);
            }
        }
    }

    /// <summary>
    /// Writes the names and values of all string, int and bool properties of the specified object in a line break-separated, justified format
    /// </summary>
    /// <param name="obj">The object for which to list all in-scope properties</param>
    /// <param name="message">A message to preceed the property list.</param>
    /// <param name="indent">If true each written line will be indented.</param>
    /// <param name="level">The criticality threshold of the log message. If set to a lower criticality than the value of <see cref="Level"/> the message will be omitted. Default is <see cref="Default.LogLevel"/></param>
    /// <param name="callingMethod">If not specified, the calling method name</param>
    public void PropertyReport(Object obj, String message = null, Boolean indent = true, LogLevel level = null,
        [CallerMemberName] String callingMethod = "") {
        if (disabled) {
            return;
        }

        if (level == null) {
            level = Default.LogLevel;
        }
        if (Level.GE(level)) {
            var tuples = new Dictionary<String, String>();
            var type = obj.GetType();
            var properties = type.GetProperties();

            foreach (var property in properties) {
                if (property.PropertyType == typeof(String)) {
                    tuples.Add(property.Name, property.GetValue(obj) as String);
                }

                if (property.PropertyType == typeof(Int32) || property.PropertyType == typeof(Boolean)) {
                    tuples.Add(property.Name, property.GetValue(obj).ToString());
                }
            }

            standardEntry(message ?? $"{type} properties:", callingMethod, level);
            Echo(tuples, indent, level);
        }
    }

    #endregion

    #region Exception Entries

    /// <summary>
    /// Writes the description and stack trace of the specified exception to the log
    /// </summary>
    /// <param name="ex">The exception for which details should be written to the log</param>
    /// <param name="message">A message to preceed the exception details</param>
    /// <param name="includeInnerExceptions">If true, details for each inner exception will also be written to the log. Each additional inner exception found in the stack trace will be indented by <see cref="ExceptionIndentLength"/> more space characters</param>
    /// <param name="callingMethod">If not specified, the calling method name</param>
    public void Exception(Exception ex, String message = null, Boolean includeInnerExceptions = true,
        [CallerMemberName] String callingMethod = "") {
        if (disabled || !Level.GE(LogLevel.Exception)) {
            return;
        }

        lastMessageLevel = LogLevel.Exception;
        var Expression = new StringBuilder($"{message ?? Default.EXCEPTION_MESSAGE}:\r\n");
        Expression.AppendLine(appendException(ex, includeInnerExceptions));
        standardEntry(Expression.ToString(), callingMethod, LogLevel.Exception);
    }

    String appendException(Exception ex, Boolean includeInnerExceptions, Int32 innerExceptions = 0) {
        var Expression = new StringBuilder();
        var indent = ExceptionIndent(innerExceptions);

        if (innerExceptions > 0) {
            Expression.AppendLine($"\r\n{indent}Inner Exception #{innerExceptions}:\r\n");
        }

        ex.ToString().SplitOnNewLine().ForEach(p => Expression.AppendLine($"{indent}{p}"));

        if (includeInnerExceptions && ex.InnerException != null) {
            Expression.AppendLine(appendException(ex.InnerException, true, innerExceptions + 1));
        }

        return Expression.ToString();
    }

    #endregion

    #region Misc

    /// <summary>
    /// Writes a banner message to the log preceeded and followed by <see cref="BannerLength"/> instances of the character specified by <see cref="BannerChar"/>
    /// </summary>
    /// <param name="message">A message to be written to the log. If not specified, the assembly name, invoker and timestamp will be written instead.</param>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public void Banner(String message = null) {
        if (disabled) {
            return;
        }

        if (String.IsNullOrEmpty(message)) {
            message = $"{Context.CallingAssembly} -- invoked by {Environment.UserDomainName}\\{Environment.UserName} -- {GetTimeStamp()}";
        }

        WriteMessage($"\r\n{BannerBoarder}\r\n{message}\r\n{BannerBoarder}\r\n");
    }

    /// <summary>
    /// Writes a blank line to the log.
    /// </summary>
    /// <param name="level">The criticality threshold of the log message. If set to a lower criticality than the value of <see cref="Level"/> the message will be omitted. Default is <see cref="Default.LogLevel"/></param>
    public void LineFeed(LogLevel level = null) {
        if (disabled) {
            return;
        }

        level ??= Default.LogLevel;
        if (Level.GE(level)) {
            lastMessageLevel = level;
            WriteMessage(String.Empty);
        }
    }

    /// <summary>
    /// Clears the contents from the current log file
    /// </summary>
    public void Clear() {
        if (disabled) {
            return;
        }

        if (File.Exists(logPath)) {
            File.WriteAllText(logPath, String.Empty);
        }
    }

    #endregion
}