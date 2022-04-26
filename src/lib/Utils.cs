using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace MJBLogger
{
    partial class MJBLog
    {

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
