using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
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

        [MethodImpl(MethodImplOptions.NoInlining)]
        private string CallingClass(int framesIn = 0)
        {
            if (IncludeCaller)
            {
                try
                {
                    return $"{new StackTrace().GetFrame(2 + framesIn).GetMethod().ReflectedType.Name}.";
                }
                catch
                {
                    return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
