#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace MJBLogger;

public class LogLevel {
    static Int32 padding = 0;
    static Int32 chibiPadding = 0;
    static Boolean gotPadding = false;

    public Int32 Criticality { get; private set; }
    public String Name { get; private set; }
    public String ChibiName { get; private set; }
    public ConsoleColor Color { get; private set; }

    internal String Label => $"<{Name.PadRight(Padding)}> ";

    internal String ChibiLabel => $"<{ChibiName.PadRight(ChibiPadding)}> ";

    internal static Int32 Padding {
        get {
            if (!gotPadding) {
                GetPadding();
            }
            return padding;
        }
    }

    internal static Int32 ChibiPadding {
        get {
            if (!gotPadding) {
                GetPadding();
            }
            return chibiPadding;
        }
    }

    /// <summary>
    /// LogLevel Constructor
    /// </summary>
    /// <param name="Label">Text label you'd like to appear on log entries of this type</param>
    /// <param name="Criticality">A value representing the importance of log entries of this type. The lower the value, the more critical</param>
    /// <param name="ChibiLabel">A short-hand identifier for Label. If not defined, will be assigned the value of Label truncated to 4 characters.</param>
    /// <param name="Color">The text color you'd like log entries of this type to appear with on-screen</param>
    public LogLevel(String Label, Int32 Criticality, String ChibiLabel = null, ConsoleColor Color = ConsoleColor.Gray) {
        Name = Label;
        ChibiName = ChibiLabel ?? Label.Substring(0, 4);
        this.Criticality = Criticality;
        this.Color = Color;

        if (Supported.Any(p => String.Equals(p.Name, Label, StringComparison.OrdinalIgnoreCase))) {
            throw new InvalidLogLevelException($"Value entered for {nameof(Label)} ({Label}) is already in use.");
        }

        if (Supported.Any(p => String.Equals(p.ChibiName, ChibiLabel, StringComparison.OrdinalIgnoreCase))) {
            throw new InvalidLogLevelException($"Value entered for {nameof(ChibiLabel)} ({ChibiLabel}) is already in use.");
        }

        Supported.Add(this);
        gotPadding = false;
    }

    LogLevel() { }

    /// <summary>
    /// Disables all logging
    /// </summary>
    public static readonly LogLevel None = new() {
        Criticality = -1
    };

    /// <summary>
    /// Built-in logging level for most critical log messages.
    /// </summary>
    public static readonly LogLevel Critical = new() {
        Criticality = 0,
        Name = nameof(Critical),
        ChibiName = @"Crit",
        Color = ConsoleColor.Red
    };

    /// <summary>
    /// Built-in logging level for exceptions. Equivalent criticality to "Error"
    /// </summary>
    public static readonly LogLevel Exception = new() {
        Criticality = 10,
        Name = nameof(Exception),
        ChibiName = @"Excep",
        Color = ConsoleColor.Red
    };

    /// <summary>
    /// Built-in logging level for errors. Equivalent criticality to "Exception"
    /// </summary>
    public static readonly LogLevel Error = new() {
        Criticality = 10,
        Name = nameof(Error),
        ChibiName = @"Err",
        Color = ConsoleColor.Red
    };

    /// <summary>
    /// Built-in logging level for warning messages. 
    /// </summary>
    public static readonly LogLevel Warning = new() {
        Criticality = 20,
        Name = nameof(Warning),
        ChibiName = @"Warn",
        Color = ConsoleColor.Yellow
    };

    /// <summary>
    /// Built-in logging level for informational messages.
    /// </summary>
    public static readonly LogLevel Info = new() {
        Criticality = 30,
        Name = nameof(Info),
        ChibiName = nameof(Info),
        Color = ConsoleColor.Gray
    };

    /// <summary>
    /// Built-in logging level for verbose messages
    /// </summary>
    public static readonly LogLevel Verbose = new() {
        Criticality = 40,
        Name = nameof(Verbose),
        ChibiName = @"Verb",
        Color = ConsoleColor.Green
    };

    /// <summary>
    /// Built-in logging level for diagnostic messages
    /// </summary>
    public static readonly LogLevel Diagnostic = new() {
        Criticality = 50,
        Name = nameof(Diagnostic),
        ChibiName = @"Diag",
        Color = ConsoleColor.Blue
    };

    internal static List<LogLevel> Supported = new() { Critical, Error, Warning, Info, Verbose, Diagnostic };

    /// <summary>
    /// Indicates whether this LogLevel is of greater or equal criticality than the specified LogLevel object
    /// </summary>
    /// <param name="compareLevel">LogLevel object to </param>
    /// <returns>true or false</returns>
    public Boolean GE(LogLevel? compareLevel) {
        if (compareLevel is null) {
            return true;
        }
        return Criticality >= compareLevel.Criticality;
    }

    /// <summary>
    /// Indicates whether this LogLevel is of lesser or equal criticality than the specified LogLevel object
    /// </summary>
    /// <param name="compareLevel">LogLevel object to </param>
    /// <returns>true or false</returns>
    public Boolean LE(LogLevel? compareLevel) {
        if (compareLevel is null) {
            return true;
        }
        return Criticality <= compareLevel.Criticality;
    }

    /// <summary>
    /// Returns a LogLevel object whose <see cref="Name"/> matches the indicated expression.
    /// </summary>
    /// <param name="expression"></param>
    /// <returns>A LogLevel object whose <see cref="Name"/> matches the indicated expression or <see cref="Default.LogLevel"/> if no match is found.</returns>
    public static LogLevel Select(String expression) {
        LogLevel? logLevel = Supported.FirstOrDefault(p => String.Equals(p.Name, expression, StringComparison.InvariantCultureIgnoreCase));
        if (logLevel != null) {
            return logLevel;
        }
        logLevel = Supported.FirstOrDefault(p => String.Equals(p.ChibiName, expression, StringComparison.InvariantCultureIgnoreCase));
        
        return logLevel ?? Default.LogLevel;

    }

    static void GetPadding() {
        padding = Supported.Select(p => p.Name).LongestStringLength() + 3;
        chibiPadding = Supported.Select(p => p.ChibiName).LongestStringLength() + 3;
        gotPadding = true;
    }
}