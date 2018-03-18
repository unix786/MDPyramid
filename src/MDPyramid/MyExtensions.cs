using System;
using System.Collections.Generic;
using System.Linq;

namespace MDPyramid
{
    internal static class MyExtensions
    {
        public static string ToStringAggregate<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Aggregate((string)null, (s, x) => s == null ? x.ToString() : s + ", " + x.ToString())
                ?? String.Empty;
        }
    }
}
