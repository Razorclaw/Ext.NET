﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Ext.NET
{
    /// <summary>
    /// Contain every extensions for Enumerable.
    /// </summary>
    public static class EnumerableExtension
    {
        /// <summary>
        /// Inline foreach statement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="action">Action applied on each element from the source</param>
        public static void Each<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (action == null) throw new ArgumentNullException("action");

            foreach (var item in source)
            {
                action(item);
            }
        }

        /// <summary>
        /// Keep only element containing the given pattern. Similar to unix grep.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pattern">A simple string</param>
        /// <returns></returns>
        public static IEnumerable<T> Grep<T>(this IEnumerable<T> source, string pattern)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (pattern == null) throw new ArgumentNullException("pattern");

            foreach (var item in source)
            {
                if (item.ToString().Contains(pattern))
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Keep only element matching the given pattern. Similar to unix egrep.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pattern">A regex string</param>
        /// <param name="option">Regex options, default None</param>
        /// <returns></returns>
        public static IEnumerable<T> EGrep<T>(this IEnumerable<T> source, string pattern, RegexOptions option = RegexOptions.None)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (pattern == null) throw new ArgumentNullException("pattern");
            if (option == null) throw new ArgumentNullException("option");

            var regex = new Regex(pattern, option);
            foreach (var item in source)
            {
                if (regex.IsMatch(item.ToString()))
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Rotates the elements in the source by the specified distance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static IEnumerable<T> Rotate<T>(this IEnumerable<T> source, int distance = 1)
        {
            if (source == null) throw new ArgumentNullException("source");

            int size = source.Size();
            distance = distance > 0 ? distance % size : size - (distance - size);
            return distance == 0 ? source : source.RotateIterator(distance);
        }

        private static IEnumerable<T> RotateIterator<T>(this IEnumerable<T> source, int distance)
        {
            var t = new T[distance]; // Itering over an array is more than 2 times faster
            using (var it = source.GetEnumerator())
            {
                for (int i = 0; i < distance && it.MoveNext(); ++i)
                    t[i] = it.Current;
                while (it.MoveNext())
                    yield return it.Current;
            }
            for (int i = 0; i < distance; ++i)
                yield return t[i];
        }

        /// <summary>
        /// Randomly permutes the source using the Fisher–Yates algorithm and a default source of randomness.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            var random = new Random();
            return source.ShuffleIterator(random);
        }

        /// <summary>
        /// Randomly permute the source using the Fisher–Yates algorithm and the specified source of randomness.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="random">A random source</param>
        /// <returns></returns>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random random)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (random == null) throw new ArgumentNullException("random");

            return source.ShuffleIterator(random);
        }

        private static IEnumerable<T> ShuffleIterator<T>(this IEnumerable<T> source, Random random)
        {
            var buffer = new List<T>(source);

            for (int i = 0; i < buffer.Count; ++i)
            {
                int j = random.Next(i, buffer.Count);
                yield return buffer[j];

                buffer[j] = buffer[i];
            }
        }

        public static int Size<T>(this IEnumerable<T> source)
        {
            var col = source as ICollection<T>;
            if (col != null)
                return col.Count;

            int c = 0;
            using (var e = source.GetEnumerator())
            {
                while (e.MoveNext())
                    c++;
            }

            return c;
        }
    }
}
