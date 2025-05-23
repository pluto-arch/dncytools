﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DotnetGeek.Tools
{
    public static partial class IEnumerableExtension
    {

#if NETFRAMEWORK || NET5_0
        /// <summary>
        /// 自定义比较器去重
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> DistinctBy<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            var seen = new HashSet<TSource>(comparer);
            foreach (var item in source)
            {
                if (seen.Add(item))
                    yield return item;
            }
        }

        /// <summary>
        /// 按key去重
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
#endif

        /// <summary>
        /// 按字段属性判等取交集
        /// </summary>
        /// <typeparam name="TFirst"></typeparam>
        /// <typeparam name="TSecond"></typeparam>
        /// <param name="second"></param>
        /// <param name="condition"></param>
        /// <param name="first"></param>
        /// <returns></returns>
        public static IEnumerable<TFirst> IntersectBy<TFirst, TSecond>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, bool> condition)
        {
            return first.Where(f => second.Any(s => condition(f, s)));
        }

        /// <summary>
        /// 按字段属性判等取差集
        /// </summary>
        /// <typeparam name="TFirst"></typeparam>
        /// <typeparam name="TSecond"></typeparam>
        /// <param name="second"></param>
        /// <param name="condition"></param>
        /// <param name="first"></param>
        /// <returns></returns>
        public static IEnumerable<TFirst> ExceptBy<TFirst, TSecond>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, bool> condition)
        {
            return first.Where(f => !second.Any(s => condition(f, s)));
        }



        /// <summary>
        /// 按字段属性判等取交集
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> IntersectBy<TSource, TKey>(this IEnumerable<TSource> first, IEnumerable<TKey> second, Func<TSource, TKey> keySelector)
        {
            return first.IntersectBy(second, keySelector, null);
        }

        /// <summary>
        /// 按字段属性判等取交集
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> IntersectBy<TSource, TKey>(this IEnumerable<TSource> first, IEnumerable<TKey> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            if (first == null)
                throw new ArgumentNullException(nameof(first));
            if (second == null)
                throw new ArgumentNullException(nameof(second));
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));
            return IntersectByIterator(first, second, keySelector, comparer);
        }

        private static IEnumerable<TSource> IntersectByIterator<TSource, TKey>(IEnumerable<TSource> first, IEnumerable<TKey> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            var set = new HashSet<TKey>(second, comparer);
            foreach (var source in first)
            {
                if (set.Remove(keySelector(source)))
                    yield return source;
            }
        }

        /// <summary>
        /// 按字段属性判等取差集
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> ExceptBy<TSource, TKey>(this IEnumerable<TSource> first, IEnumerable<TKey> second, Func<TSource, TKey> keySelector)
        {
            return first.ExceptBy(second, keySelector, null);
        }

        /// <summary>
        /// 按字段属性判等取差集
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="keySelector"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerable<TSource> ExceptBy<TSource, TKey>(this IEnumerable<TSource> first, IEnumerable<TKey> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            if (first == null)
                throw new ArgumentNullException(nameof(first));
            if (second == null)
                throw new ArgumentNullException(nameof(second));
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));
            return ExceptByIterator(first, second, keySelector, comparer);
        }

        private static IEnumerable<TSource> ExceptByIterator<TSource, TKey>(IEnumerable<TSource> first, IEnumerable<TKey> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            var set = new HashSet<TKey>(second, comparer);
            foreach (var source in first)
            {
                if (set.Add(keySelector(source)))
                    yield return source;
            }
        }



        /// <summary>
        /// 添加多个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="values"></param>
        public static void AddRange<T>(this ICollection<T> @this, params T[] values)
        {
            foreach (var obj in values)
            {
                @this.Add(obj);
            }
        }

        /// <summary>
        /// 添加多个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="values"></param>
        public static void AddRange<T>(this ConcurrentBag<T> @this, params T[] values)
        {
            foreach (var obj in values)
            {
                @this.Add(obj);
            }
        }

        /// <summary>
        /// 添加多个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="values"></param>
        public static void AddRange<T>(this ConcurrentQueue<T> @this, params T[] values)
        {
            foreach (var obj in values)
            {
                @this.Enqueue(obj);
            }
        }

        /// <summary>
        /// 添加符合条件的多个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="predicate"></param>
        /// <param name="values"></param>
        public static void AddRangeIf<T>(this ICollection<T> @this, Func<T, bool> predicate, params T[] values)
        {
            foreach (var obj in values)
            {
                if (predicate(obj))
                {
                    @this.Add(obj);
                }
            }
        }

        /// <summary>
        /// 添加符合条件的多个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="predicate"></param>
        /// <param name="values"></param>
        public static void AddRangeIf<T>(this ConcurrentBag<T> @this, Func<T, bool> predicate, params T[] values)
        {
            foreach (var obj in values)
            {
                if (predicate(obj))
                {
                    @this.Add(obj);
                }
            }
        }

        /// <summary>
        /// 添加符合条件的多个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="predicate"></param>
        /// <param name="values"></param>
        public static void AddRangeIf<T>(this ConcurrentQueue<T> @this, Func<T, bool> predicate, params T[] values)
        {
            foreach (var obj in values)
            {
                if (predicate(obj))
                {
                    @this.Enqueue(obj);
                }
            }
        }

        /// <summary>
        /// 添加不重复的元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="values"></param>
        public static void AddRangeIfNotContains<T>(this ICollection<T> @this, params T[] values)
        {
            foreach (T obj in values)
            {
                if (!@this.Contains(obj))
                {
                    @this.Add(obj);
                }
            }
        }

        /// <summary>
        /// 移除符合条件的元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="where"></param>
        public static void RemoveWhere<T>(this ICollection<T> @this, Func<T, bool> @where)
        {
            foreach (var obj in @this.Where(where).ToList())
            {
                @this.Remove(obj);
            }
        }

        /// <summary>
        /// 在元素之后添加元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="condition">条件</param>
        /// <param name="value">值</param>
        public static void InsertAfter<T>(this IList<T> list, Func<T, bool> condition, T value)
        {
            foreach (var item in list.Select((item, index) => new
            {
                item,
                index
            }).Where(p => condition(p.item)).OrderByDescending(p => p.index))
            {
                if (item.index + 1 == list.Count)
                {
                    list.Add(value);
                }
                else
                {
                    list.Insert(item.index + 1, value);
                }
            }
        }

        /// <summary>
        /// 在元素之后添加元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="index">索引位置</param>
        /// <param name="value">值</param>
        public static void InsertAfter<T>(this IList<T> list, int index, T value)
        {
            foreach (var item in list.Select((v, i) => new
            {
                Value = v,
                Index = i
            }).Where(p => p.Index == index).OrderByDescending(p => p.Index))
            {
                if (item.Index + 1 == list.Count)
                {
                    list.Add(value);
                }
                else
                {
                    list.Insert(item.Index + 1, value);
                }
            }
        }

        /// <summary>
        /// 转HashSet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static HashSet<TResult> ToHashSet<T, TResult>(this IEnumerable<T> source, Func<T, TResult> selector)
        {
            var set = new HashSet<TResult>();
            set.UnionWith(source.Select(selector));
            return set;
        }

        /// <summary>
        /// 遍历IEnumerable
        /// </summary>
        /// <param name="objs"></param>
        /// <param name="action">回调方法</param>
        /// <typeparam name="T"></typeparam>
        public static void ForEach<T>(this IEnumerable<T> objs, Action<T> action)
        {
            foreach (var o in objs)
            {
                action(o);
            }
        }

#if !NET40
        /// <summary>
        /// 异步foreach
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="maxParallelCount">最大并行数</param>
        /// <param name="action"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task ForeachAsync<T>(this IEnumerable<T> source, Func<T, Task> action, int maxParallelCount, CancellationToken cancellationToken = default)
        {
            if (Debugger.IsAttached)
            {
                foreach (var item in source)
                {
                    await action(item);
                }

                return;
            }

            var list = new List<Task>();
            foreach (var item in source)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                list.Add(action(item));
                if (list.Count >= maxParallelCount)
                {
                    await Task.WhenAll(list);
                    list.Clear();
                }
            }

            await Task.WhenAll(list);
        }

        /// <summary>
        /// 异步foreach
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static Task ForeachAsync<T>(this IEnumerable<T> source, Func<T, Task> action, CancellationToken cancellationToken = default)
        {
            return source.ForeachAsync(action, source.Count(), cancellationToken);
        }

        /// <summary>
        /// 异步Select
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static Task<TResult[]> SelectAsync<T, TResult>(this IEnumerable<T> source, Func<T, Task<TResult>> selector)
        {
            return Task.WhenAll(source.Select(selector));
        }

        /// <summary>
        /// 异步Select
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static Task<TResult[]> SelectAsync<T, TResult>(this IEnumerable<T> source, Func<T, int, Task<TResult>> selector)
        {
            return Task.WhenAll(source.Select(selector));
        }

        /// <summary>
        /// 异步For
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <param name="maxParallelCount">最大并行数</param>
        /// <param name="cancellationToken">取消口令</param>
        /// <returns></returns>
        public static async Task ForAsync<T>(this IEnumerable<T> source, Func<T, int, Task> selector, int maxParallelCount, CancellationToken cancellationToken = default)
        {
            int index = 0;
            if (Debugger.IsAttached)
            {
                foreach (var item in source)
                {
                    await selector(item, index);
                    index++;
                }

                return;
            }

            var list = new List<Task>();
            foreach (var item in source)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                list.Add(selector(item, index));
                Interlocked.Add(ref index, 1);
                if (list.Count >= maxParallelCount)
                {
                    await Task.WhenAll(list);
                    list.Clear();
                }
            }

            await Task.WhenAll(list);
        }

        /// <summary>
        /// 异步For
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <param name="cancellationToken">取消口令</param>
        /// <returns></returns>
        public static Task ForAsync<T>(this IEnumerable<T> source, Func<T, int, Task> selector, CancellationToken cancellationToken = default)
        {
            return source.ForAsync(selector, source.Count(), cancellationToken);
        }
#endif
        /// <summary>
        /// 取最大值
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static TResult MaxOrDefault<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector) => source.Select(selector).DefaultIfEmpty().Max();

        /// <summary>
        /// 取最大值
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static TResult MaxOrDefault<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector, TResult defaultValue) => source.Select(selector).DefaultIfEmpty(defaultValue).Max();

        /// <summary>
        /// 取最大值
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TSource MaxOrDefault<TSource>(this IQueryable<TSource> source) => source.DefaultIfEmpty().Max();

        /// <summary>
        /// 取最大值
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static TSource MaxOrDefault<TSource>(this IQueryable<TSource> source, TSource defaultValue) => source.DefaultIfEmpty(defaultValue).Max();

        /// <summary>
        /// 取最大值
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static TResult MaxOrDefault<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector, TResult defaultValue) => source.Select(selector).DefaultIfEmpty(defaultValue).Max();

        /// <summary>
        /// 取最大值
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static TResult MaxOrDefault<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector) => source.Select(selector).DefaultIfEmpty().Max();

        /// <summary>
        /// 取最大值
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TSource MaxOrDefault<TSource>(this IEnumerable<TSource> source) => source.DefaultIfEmpty().Max();

        /// <summary>
        /// 取最大值
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static TSource MaxOrDefault<TSource>(this IEnumerable<TSource> source, TSource defaultValue) => source.DefaultIfEmpty(defaultValue).Max();

        /// <summary>
        /// 取最小值
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static TResult MinOrDefault<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector) => source.Select(selector).DefaultIfEmpty().Min();

        /// <summary>
        /// 取最小值
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static TResult MinOrDefault<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector, TResult defaultValue) => source.Select(selector).DefaultIfEmpty(defaultValue).Min();

        /// <summary>
        /// 取最小值
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TSource MinOrDefault<TSource>(this IQueryable<TSource> source) => source.DefaultIfEmpty().Min();

        /// <summary>
        /// 取最小值
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static TSource MinOrDefault<TSource>(this IQueryable<TSource> source, TSource defaultValue) => source.DefaultIfEmpty(defaultValue).Min();

        /// <summary>
        /// 取最小值
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static TResult MinOrDefault<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector) => source.Select(selector).DefaultIfEmpty().Min();

        /// <summary>
        /// 取最小值
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static TResult MinOrDefault<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector, TResult defaultValue) => source.Select(selector).DefaultIfEmpty(defaultValue).Min();

        /// <summary>
        /// 取最小值
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TSource MinOrDefault<TSource>(this IEnumerable<TSource> source) => source.DefaultIfEmpty().Min();

        /// <summary>
        /// 取最小值
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static TSource MinOrDefault<TSource>(this IEnumerable<TSource> source, TSource defaultValue) => source.DefaultIfEmpty(defaultValue).Min();

        /// <summary>
        /// 标准差
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static TResult StandardDeviation<T, TResult>(this IEnumerable<T> source, Func<T, TResult> selector) where TResult : IConvertible
        {
            return source.Select(t => selector(t).ConvertTo<double>()).StandardDeviation().ConvertTo<TResult>();
        }

        /// <summary>
        /// 标准差
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T StandardDeviation<T>(this IEnumerable<T> source) where T : IConvertible
        {
            return source.Select(t => t.ConvertTo<double>()).StandardDeviation().ConvertTo<T>();
        }

        /// <summary>
        /// 标准差
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static double StandardDeviation(this IEnumerable<double> source)
        {
            double result = 0;
            int count = source.Count();
            if (count > 1)
            {
                double avg = source.Average();
                double sum = source.Sum(d => (d - avg) * (d - avg));
                result = Math.Sqrt(sum / count);
            }

            return result;
        }

        /// <summary>
        /// 随机排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<T> OrderByRandom<T>(this IEnumerable<T> source)
        {
            return source.OrderBy(_ => Guid.NewGuid());
        }

        /// <summary>
        /// 序列相等
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static bool SequenceEqual<T>(this IEnumerable<T> first, IEnumerable<T> second, Func<T, T, bool> condition)
        {
            if (first is ICollection<T> source1 && second is ICollection<T> source2)
            {
                if (source1.Count != source2.Count)
                {
                    return false;
                }

                if (source1 is IList<T> list1 && source2 is IList<T> list2)
                {
                    int count = source1.Count;
                    for (int index = 0; index < count; ++index)
                    {
                        if (!condition(list1[index], list2[index]))
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }

            using IEnumerator<T> enumerator1 = first.GetEnumerator();
            using IEnumerator<T> enumerator2 = second.GetEnumerator();
            while (enumerator1.MoveNext())
            {
                if (!enumerator2.MoveNext() || !condition(enumerator1.Current, enumerator2.Current))
                {
                    return false;
                }
            }

            return !enumerator2.MoveNext();
        }

        /// <summary>
        /// 序列相等
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static bool SequenceEqual<T1, T2>(this IEnumerable<T1> first, IEnumerable<T2> second, Func<T1, T2, bool> condition)
        {
            if (first is ICollection<T1> source1 && second is ICollection<T2> source2)
            {
                if (source1.Count != source2.Count)
                {
                    return false;
                }

                if (source1 is IList<T1> list1 && source2 is IList<T2> list2)
                {
                    int count = source1.Count;
                    for (int index = 0; index < count; ++index)
                    {
                        if (!condition(list1[index], list2[index]))
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }

            using IEnumerator<T1> enumerator1 = first.GetEnumerator();
            using IEnumerator<T2> enumerator2 = second.GetEnumerator();
            while (enumerator1.MoveNext())
            {
                if (!enumerator2.MoveNext() || !condition(enumerator1.Current, enumerator2.Current))
                {
                    return false;
                }
            }

            return !enumerator2.MoveNext();
        }

        /// <summary>
        /// 对比两个集合哪些是新增的、删除的、修改的
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="condition">对比因素条件</param>
        /// <returns></returns>
        public static (List<T1> adds, List<T2> remove, List<T1> updates) CompareChanges<T1, T2>(this IEnumerable<T1> first, IEnumerable<T2> second, Func<T1, T2, bool> condition)
        {
            var add = first.ExceptBy(second, condition).ToList();
            var remove = second.ExceptBy(first, (s, f) => condition(f, s)).ToList();
            var update = first.IntersectBy(second, condition).ToList();
            return (add, remove, update);
        }

        /// <summary>
        /// 对比两个集合哪些是新增的、删除的、修改的
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="condition">对比因素条件</param>
        /// <returns></returns>
        public static (List<T1> adds, List<T2> remove, List<(T1 first, T2 second)> updates) CompareChangesPlus<T1, T2>(this IEnumerable<T1> first, IEnumerable<T2> second, Func<T1, T2, bool> condition)
        {
            var add = first.ExceptBy(second, condition).ToList();
            var remove = second.ExceptBy(first, (s, f) => condition(f, s)).ToList();
            var updates = first.IntersectBy(second, condition).Select(t1 => (t1, second.FirstOrDefault(t2 => condition(t1, t2)))).ToList();
            return (add, remove, updates);
        }

        public static List<T> AsNotNull<T>(this List<T> list)
        {
            return list ?? new List<T>();
        }

        public static IEnumerable<T> AsNotNull<T>(this IEnumerable<T> list)
        {
            return list ?? new List<T>();
        }
    }
}

