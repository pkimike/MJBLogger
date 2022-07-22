using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace MJBLogger;

partial class MJBLog {
    [MethodImpl(MethodImplOptions.NoInlining)]
    String CallingClass(Int32 framesIn = 0) {
        if (IncludeCaller) {
            try {
                return $"{new StackTrace().GetFrame(2 + framesIn).GetMethod().ReflectedType.Name}.";
            } catch {
                return String.Empty;
            }
        }

        return String.Empty;
    }
}