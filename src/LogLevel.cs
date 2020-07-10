using System;
using System.Collections.Generic;
using System.Linq;

namespace MJBLogger
{
    public class LogLevel
    {
        private static int padding = 0;
        private static int chibiPadding = 0;
        private static bool gotPadding = false;

        public int Criticality { get; private set; }
        public string Name { get; private set; }
        public string ChibiName { get; private set; }
        public ConsoleColor Color { get; private set; }

        internal string Label
        {
            get
            {
                return $"<{Name.PadRight(Padding)}> ";
            }
        }

        internal string ChibiLabel
        {
            get
            {
                return $"<{ChibiName.PadRight(ChibiPadding)}> ";
            }
        }

        internal static int Padding
        {
            get
            {
                if (!gotPadding)
                {
                    GetPadding();
                }
                return padding;
            }
        }

        internal static int ChibiPadding
        {
            get
            {
                if (!gotPadding)
                {
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
        public LogLevel(string Label, int Criticality, string ChibiLabel = null, ConsoleColor Color = ConsoleColor.Gray)
        {
            this.Name = Label;
            this.ChibiName = ChibiLabel ?? Label.Substring(0, 4);
            this.Criticality = Criticality;
            this.Color = Color;

            if (Supported.Any(p => string.Equals(p.Name, Label, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidLogLevelException($"Value entered for {nameof(Label)} ({Label}) is already in use.");
            }

            if (Supported.Any(p => string.Equals(p.ChibiName, ChibiLabel, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidLogLevelException($"Value entered for {nameof(ChibiLabel)} ({ChibiLabel}) is already in use.");
            }

            Supported.Add(this);
            gotPadding = false;
        }

        private LogLevel() { }

        /// <summary>
        /// Disables all logging
        /// </summary>
        public static readonly LogLevel None = new LogLevel()
        {
            Criticality = -1
        };

        /// <summary>
        /// Built-in logging level for most critical log messages.
        /// </summary>
        public static readonly LogLevel Critical = new LogLevel()
        {
            Criticality = 0,
            Name = nameof(Critical),
            ChibiName = @"Crit",
            Color = ConsoleColor.Red
        };

        /// <summary>
        /// Built-in logging level for exceptions. Equivalent criticality to "Error"
        /// </summary>
        public static readonly LogLevel Exception = new LogLevel()
        {
            Criticality = 10,
            Name = nameof(Exception),
            ChibiName = @"Excep",
            Color = ConsoleColor.Red
        };

        /// <summary>
        /// Built-in logging level for errors. Equivalent criticality to "Exception"
        /// </summary>
        public static readonly LogLevel Error = new LogLevel()
        {
            Criticality = 10,
            Name = nameof(Error),
            ChibiName = @"Err",
            Color = ConsoleColor.Red
        };

        /// <summary>
        /// Built-in logging level for warning messages. 
        /// </summary>
        public static readonly LogLevel Warning = new LogLevel()
        {
            Criticality = 20,
            Name = nameof(Warning),
            ChibiName = @"Warn",
            Color = ConsoleColor.Yellow
        };

        /// <summary>
        /// Built-in logging level for informational messages.
        /// </summary>
        public static readonly LogLevel Info = new LogLevel()
        {
            Criticality = 30,
            Name = nameof(Info),
            ChibiName = nameof(Info),
            Color = ConsoleColor.Gray
        };

        /// <summary>
        /// Built-in logging level for verbose messages
        /// </summary>
        public static readonly LogLevel Verbose = new LogLevel()
        {
            Criticality = 40,
            Name = nameof(Verbose),
            ChibiName = @"Verb",
            Color = ConsoleColor.Green
        };

        /// <summary>
        /// Built-in logging level for diagnostic messages
        /// </summary>
        public static readonly LogLevel Diagnostic = new LogLevel()
        {
            Criticality = 50,
            Name = nameof(Diagnostic),
            ChibiName = @"Diag",
            Color = ConsoleColor.Blue
        };

        internal static List<LogLevel> Supported = new List<LogLevel> { Critical, Error, Warning, Info, Verbose, Diagnostic };

        /// <summary>
        /// Indicates whether this LogLevel is of creater or equal criticality than the specified LogLevel object
        /// </summary>
        /// <param name="compareLevel">LogLevel object to </param>
        /// <returns>true or false</returns>
        public bool GE(LogLevel compareLevel)
        {
            return Criticality >= compareLevel.Criticality;
        }

        /// <summary>
        /// Indicates whether this LogLevel is of lesser or equal criticality than the specified LogLevel object
        /// </summary>
        /// <param name="compareLevel">LogLevel object to </param>
        /// <returns>true or false</returns>
        public bool LE(LogLevel compareLevel)
        {
            return Criticality <= compareLevel.Criticality;
        }

        /// <summary>
        /// Returns a LogLevel object whose <see cref="Name"/> matches the indicated expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns>A LogLevel object whose <see cref="Name"/> matches the indicated expression or <see cref="Defaults.Level"/> if no match is found.</returns>
        public static LogLevel Select(string expression)
        {
            LogLevel ll = Supported.FirstOrDefault(p => string.Equals(p.Name, expression, StringComparison.OrdinalIgnoreCase));
            if (ll == null)
            {
                ll = Supported.FirstOrDefault(p => string.Equals(p.ChibiName, expression, StringComparison.OrdinalIgnoreCase));
                return ll ?? Defaults.Level;
            }
            else
            {
                return ll;
            }
        }

        private static void GetPadding()
        {
            padding = Supported.Select(p => p.Name).LongestStringLength() + 3;
            chibiPadding = Supported.Select(p => p.ChibiName).LongestStringLength() + 3;
            gotPadding = true;
        }

    }
}
