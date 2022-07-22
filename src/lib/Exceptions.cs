﻿using System;

namespace MJBLogger {
    public class InvalidLogLevelException : Exception
    {
        internal InvalidLogLevelException(String message) : base(message) { }
    }

    public class InvalidLogPathException : Exception
    {
        internal InvalidLogPathException(String expression) : base($"The indicated log path element \"{expression}\" contains invalid characters. Avoid: <>:\"/\\|? *") { }
    }

    public class InvalidDateTimePatternException : Exception
    {
        internal InvalidDateTimePatternException(String expression) : base($"The expression \"{expression}\" cannot be used to define a DateTime string") { }
    }
}
