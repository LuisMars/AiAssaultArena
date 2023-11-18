using System;
using System.Collections.Generic;

namespace AiAssaultArena.Web.Helpers;

public static class EnumerableHelpers
{
    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> func)
    {
        foreach (var item in enumerable)
        {
            func(item);
        }
    }
}
