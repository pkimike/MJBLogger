using System;
using System.Collections.Generic;
using System.Linq;

namespace MJBLogger;

static class Extensions {
    internal static String Truncate(this String expression, Int32 length, Boolean padRight = false) {
        if (expression.Length > length) {
            return expression.Substring(0, length);
        }

        return padRight ? expression.PadRight(length) : expression;
    }

    internal static Int32 LongestStringLength(this IEnumerable<String> elements) {
        return elements.Aggregate(String.Empty, (max, cur) => max.Length > cur.Length ? max : cur).Length;
    }

    internal static List<String> SplitOnNewLine(this String expression) {
        return expression.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)
            .Where(p => !String.IsNullOrWhiteSpace(p))
            .ToList();
    }
}