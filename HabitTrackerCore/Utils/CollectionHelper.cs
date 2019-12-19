using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HabitTrackerCore.Utils
{
    public static class CollectionHelper
    {
        /// <summary>
        /// Checks if an item is in a specified array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool In<T>(this T item, params T[] list)
        {
            return list.Contains(item);
        }

        /// <summary>
        /// Checks if an item is in a specified list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool In<T>(this T item, IEnumerable<T> _list)
        {
            return _list.Contains(item);
        }

        /// <summary>
        /// Checks if an item is not in a specified array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool NotIn<T>(this T item, params T[] list)
        {
            return !list.Contains(item);
        }

        /// <summary>
        /// Checks if an item is not in a specified array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool NotIn<T>(this T item, params IEnumerable<T>[] listArray)
        {
            return !listArray.Any(p => p.Contains(item));
        }

        /// <summary>
        /// Checks if an item is not in a specified array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool In<T>(this T item, params IEnumerable<T>[] listArray)
        {
            return listArray.Any(p => p.Contains(item));
        }

        /// <summary>
        /// Checks if an item is not in a specified list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool NotIn<T>(this T item, IEnumerable<T> _list)
        {
            return !_list.Contains(item);
        }

        public static void AddIfNotIn<T>(this List<T> _list, T item)
        {
            if (!_list.Contains(item))
                _list.Add(item);
        }
    }
}
