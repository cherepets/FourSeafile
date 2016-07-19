using System;
using System.Collections.Generic;

namespace FourSeafile.Extensions
{
    public static class IEnumerableExt
    {
        public static IEnumerable<TSource> Distinct<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var knownKeys = new HashSet<TKey>();
            foreach (var element in source)
                if (knownKeys.Add(keySelector(element)))
                    yield return element;
        }
    }
}
