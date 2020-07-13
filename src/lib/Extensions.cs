using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MJBLogger
{
    internal static class Extensions
    {
        internal static string Truncate(this string expression, int length, bool padRight = false)
        {
            if (expression.Length > length)
            {
                return expression.Substring(0, length);
            }
            else
            {
                return padRight ? expression.PadRight(length) : expression;
            }
        }

        internal static int LongestStringLength(this IEnumerable<string> Elements)
        {
            return Elements.Aggregate(string.Empty, (max, cur) => max.Length > cur.Length ? max : cur).Length;
        }

        internal static List<string> SplitOnNewLine(this string expression)
        {
            return expression.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)
                             .Where(p => !string.IsNullOrWhiteSpace(p))
                             .ToList();
        }
    }
}
