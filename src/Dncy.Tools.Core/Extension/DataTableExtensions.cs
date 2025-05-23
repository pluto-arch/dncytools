using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;

namespace DotnetGeek.Tools
{
    public static class DataTableExtensions
    {
        /// <summary>
        /// 给DataTable增加一个自增列
        /// 如果DataTable 存在 identityid 字段  则 直接返回DataTable 不做任何处理
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <returns>返回Datatable 增加字段 identityid </returns>
        public static DataTable AddIdentityColumn(this DataTable dt)
        {
            if (!dt.Columns.Contains("identityid"))
            {
                dt.Columns.Add("identityid");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dt.Rows[i]["identityid"] = (i + 1).ToString();
                }
            }

            return dt;
        }

        /// <summary>
        /// 检查DataTable 是否有数据行
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <returns>是否有数据行</returns>
        public static bool HasRows(this DataTable dt)
        {
            return dt.Rows.Count > 0;
        }

        /// <summary>
        /// 转化为datatable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static DataTable ToDataTable<T>(this IEnumerable<T> source, string tableName = null)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            var props = typeof(T).GetProperties();
            DataTable table = new DataTable(tableName);
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


        /// <summary>
        /// 根据nameList里面的字段创建一个表格,返回该表格的DataTable
        /// </summary>
        /// <param name="nameList">包含字段信息的列表</param>
        /// <returns>DataTable</returns>
        public static DataTable CreateTable(this List<string> nameList)
        {
            if (nameList.Count <= 0)
            {
                return null;
            }

            var myDataTable = new DataTable();
            foreach (string columnName in nameList)
            {
                myDataTable.Columns.Add(columnName, typeof(string));
            }

            return myDataTable;
        }

        /// <summary>
        /// 获得从DataRowCollection转换成的DataRow数组
        /// </summary>
        /// <param name="drc">DataRowCollection</param>
        /// <returns>DataRow数组</returns>
        public static DataRow[] GetDataRowArray(this DataRowCollection drc)
        {
            int count = drc.Count;
            var drs = new DataRow[count];
            for (int i = 0; i < count; i++)
            {
                drs[i] = drc[i];
            }

            return drs;
        }

    }
}

