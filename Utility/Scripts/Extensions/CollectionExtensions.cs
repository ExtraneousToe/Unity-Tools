using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UnityTools
{
    /// <summary>
    /// Extension methods.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Shuffle the given enumerable.
        /// </summary>
        /// <param name="list">List.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> list)
        {
            var r = new MOARandom(DateTime.Now.Ticks);
            var shuffledList = list.Select(
                x => new { Number = r.GetRange(0, int.MaxValue), Item = x }
            ).OrderBy(x => x.Number).Select(x => x.Item);
            return shuffledList.ToList();
        }
    }
}