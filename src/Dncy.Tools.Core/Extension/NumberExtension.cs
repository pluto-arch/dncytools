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

    }
}
