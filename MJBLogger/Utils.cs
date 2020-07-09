using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;

namespace MJBLogger
{
    partial class MJBLog
    {
        private static Regex InvalidNTFSChars = new Regex(Defaults.InvalidNTFSChars);

        private static void CheckForInvalidNTFSChars(string expression)
        {
            if (InvalidNTFSChars.IsMatch(expression))
            {
                throw new InvalidLogPathException(expression);
            }
        }
    }
}
