using System;
using System.Collections.Generic;
using System.Linq;

namespace NetCoreSample.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    internal static class IEnumerableExtensions
    {
        /// <summary>
        /// Perform a depth-first flatten of the given collection.
        /// </summary>
        /// <typeparam name="T">Type of elements in the collection</typeparam>
        /// <param name="collection">The collection contains the top level elements</param>
        /// <param name="getChildren">A delegate to retrieve children of a given element</param>
        /// <returns></returns>
        public static IEnumerable<T> DepthFirstFlatten<T>(this IEnumerable<T> collection, Func<T, IEnumerable<T>> getChildren)
        {
            return collection.SelectMany(elem => new[] { elem }.Concat(getChildren(elem).DepthFirstFlatten(getChildren)));
        }

        /// <summary>
        /// Execute the given action on each of the element in the collection
        /// </summary>
        /// <remarks>
        /// This preserves the lazy evaluation of the given collection has it.
        /// </remarks>
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (T element in collection)
            {
                action(element);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T FirstOrValue<T>(this IEnumerable<T> source, Func<T, bool> predicate, T value)
        {
            var filtered = source.Where(predicate);
            return filtered.Any() ? filtered.First() : value;
        }
    }
}
