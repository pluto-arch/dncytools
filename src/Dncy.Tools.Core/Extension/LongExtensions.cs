using System;
using System.Collections.Generic;
using System.Text;

namespace Dncy.Tools.Core.Extension
{
    public static class LongExtensions
    {
        /// <summary>
        /// 十进制转任意进制
        /// </summary>
        /// <param name="num"></param>
        /// <param name="newBase">进制，最大64</param>
        /// <param name="offset">偏移量，不能超过newBase</param>
        /// <returns></returns>
        public static string ToBinary(this long num, byte newBase,byte offset=0)
        {
            var nf = new NumberFormater(newBase, offset);
            return nf.ToString(num);
        }


        /// <summary>
        /// 任意进制转十进制
        /// </summary>
        /// <param name="num"></param>
        /// <param name="newBase">进制，最大64</param>
        /// <param name="offset">偏移量，不能超过newBase</param>
        /// <returns></returns>
        public static long ToLong(this string value, byte newBase, byte offset = 0)
        {
            var nf = new NumberFormater(newBase, offset);
            return nf.FromString(value);
        }


        /// <summary>
        /// 转换成字节数组
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetBytes(this long value)
        {
            return BitConverter.GetBytes(value);
        }
    }
}
