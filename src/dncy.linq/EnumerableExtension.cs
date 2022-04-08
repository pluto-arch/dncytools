
namespace System.Collections.Generic
{
    using System.Linq;
    using ComponentModel;
    using Data;
    using Reflection;
    public static class EnumerableExtension
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
        /// 转化为datatable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <remarks>only property value with display、description or property name</remarks>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this IEnumerable<T> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            var props = typeof(T).GetProperties();
            DataTable table = new DataTable();
            var displayProps = new List<PropertyInfo>();
            foreach (PropertyInfo item in props)
            {
#if NET40 
                var desc = item.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault() as DescriptionAttribute;
                var desplay = item.GetCustomAttributes(typeof(DisplayNameAttribute), true).FirstOrDefault() as DisplayNameAttribute;
#else
                var desc = item.GetCustomAttribute<DescriptionAttribute>();
                var desplay = item.GetCustomAttribute<DisplayNameAttribute>();
#endif
                if (desc != null)
                {
                    displayProps.Add(item);
                    table.Columns.Add(desc.Description, item.PropertyType);
                    continue;
                }

                if (desplay != null)
                {
                    displayProps.Add(item);
                    table.Columns.Add(desplay.DisplayName, item.PropertyType);
                    continue;
                }

                displayProps.Add(item);
                table.Columns.Add(item.Name, item.PropertyType);
            }

            object[] values = new object[displayProps.Count];
            foreach (T item in source)
            {
                for (int i = 0; i < values.Length; i++)
                {
#if NET40
                    values[i] = displayProps[i].GetValue(item, null);
#else
                    values[i] = displayProps[i].GetValue(item);
#endif
                }

                table.Rows.Add(values);
            }

            return table;
        }

    }
}
