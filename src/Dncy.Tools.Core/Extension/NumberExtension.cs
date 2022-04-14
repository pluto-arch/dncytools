using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dncy.Tools
{     
    public static class NumberExtension
    {
        /// <summary>
        /// 转换为中文数字格式
        /// </summary>
        /// <param name="num">123.45</param>
        /// <returns></returns>
        public static string ToChineseNumber(this double num)
        {
            var x = num.ToString(CultureInfo.CurrentCulture);
            if (x.Length == 0)
            {
                return "";
            }

            string result = "";
            if (x[0] == '-')
            {
                result = "负";
                x = x.Remove(0, 1);
            }
            if (x[0].ToString() == ".")
            {
                x = "0" + x;
            }

            if (x[x.Length - 1].ToString() == ".")
            {
                x = x.Remove(x.Length - 1, 1);
            }

            if (x.IndexOf(".") > -1)
            {
                result += x.Substring(0, x.IndexOf(".")).ToChineseNumberInt() + "点" + x.Substring(x.IndexOf(".") + 1).Aggregate("", (current, t) => current + t.ToNum());
            }
            else
            {
                result += x.ToChineseNumberInt();
            }

            return result;
        }

        /// <summary>
        /// 数字转中文金额大写
        /// </summary>
        /// <param name="number">22.22</param>
        /// <returns></returns>
        public static string ToChineseMoney(this double number)
        {
            string s = number.ToString("#L#E#D#C#K#E#D#C#J#E#D#C#I#E#D#C#H#E#D#C#G#E#D#C#F#E#D#C#.0B0A");
            string d = Regex.Replace(s, @"((?<=-|^)[^1-9]*)|((?'z'0)[0A-E]*((?=[1-9])|(?'-z'(?=[F-L\.]|$))))|((?'b'[F-L])(?'z'0)[0A-L]*((?=[1-9])|(?'-z'(?=[\　　.]|$))))", "${b}${z}");
            return Regex.Replace(d, ".", m => "负元空零壹贰叁肆伍陆柒捌玖空空空空空空空分角拾佰仟萬億兆京垓秭穰"[m.Value[0] - '-'].ToString());
        }


        /// <summary>
        /// 转换成字节数组
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetBytes(this int value)
        {
            return BitConverter.GetBytes(value);
        }

        /// <summary>
        /// 十进制转任意进制
        /// </summary>
        /// <param name="num"></param>
        /// <param name="newBase">进制</param>
        /// <returns></returns>
        public static string ToBinary(this int num, byte newBase)
        {
            var nf = new NumberFormater(newBase);
            return nf.ToString(num);
        }

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


        /// <summary>
        /// 转换成字节数组
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetBytes(this short value)
        {
            return BitConverter.GetBytes(value);
        }


        /// <summary>
        /// 转double
        /// </summary>
        /// <param name="s"></param>
        /// <returns>double类型的数据</returns>
        public static double ToDouble(this decimal s)
        {
            return (double)s;
        }

        /// <summary>
        /// 将double转换成int
        /// </summary>
        /// <param name="num">double类型</param>
        /// <returns>int类型</returns>
        public static int ToInt32(this double num)
        {
            return (int)Math.Floor(num);
        }

        /// <summary>
        /// 将double转换成int
        /// </summary>
        /// <param name="num">double类型</param>
        /// <returns>int类型</returns>
        public static int ToInt32(this decimal num)
        {
            return (int)Math.Floor(num);
        }

        /// <summary>
        /// 将int转换成double
        /// </summary>
        /// <param name="num">int类型</param>
        /// <returns>int类型</returns>
        public static double ToDouble(this int num)
        {
            return num * 1.0;
        }

        /// <summary>
        /// 将int转换成decimal
        /// </summary>
        /// <param name="num">int类型</param>
        /// <returns>int类型</returns>
        public static decimal ToDecimal(this int num)
        {
            return new decimal(num);
        }
    }
}
