using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text.RegularExpressions;
using Dncy.Tools;
using System.Date;
using System.Globalization;

namespace Dncy.Tools
{
    public static class StringExtensions
    {
        /// <summary>
        /// 转换万以下整数
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static string ToChineseTenThousandInt(this string x)
        {
            string[] strArrayLevelNames = { "", "十", "百", "千" };
            string ret = "";
            int i;
            for (i = x.Length - 1; i >= 0; i--)
            {
                if (x[i] == '0')
                {
                    ret = ToNum(x[i]) + ret;
                }
                else
                {
                    ret = ToNum(x[i]) + strArrayLevelNames[x.Length - 1 - i] + ret;
                }
            }

            while (( i = ret.IndexOf("零零", StringComparison.Ordinal) ) != -1)
            {
                ret = ret.Remove(i, 1);
            }

            if (ret[ret.Length - 1] == '零' && ret.Length > 1)
            {
                ret = ret.Remove(ret.Length - 1, 1);
            }

            if (ret.Length >= 2 && ret.Substring(0, 2) == "一十")
            {
                ret = ret.Remove(0, 1);
            }

            return ret;
        }

        /// <summary>
        /// 转换整数
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static string ToChineseNumberInt(this string x)
        {
            int len = x.Length;
            string result;
            string temp;
            if (len <= 4)
            {
                result = ToChineseTenThousandInt(x);
            }
            else if (len <= 8)
            {
                result = ToChineseTenThousandInt(x.Substring(0, len - 4)) + "万";
                temp = ToChineseTenThousandInt(x.Substring(len - 4, 4));
                if (temp.IndexOf("千", StringComparison.Ordinal) == -1 && !string.IsNullOrEmpty(temp))
                {
                    result += "零" + temp;
                }
                else
                {
                    result += temp;
                }
            }
            else
            {
                result = ToChineseTenThousandInt(x.Substring(0, len - 8)) + "亿";
                temp = ToChineseTenThousandInt(x.Substring(len - 8, 4));
                if (temp.IndexOf("千", StringComparison.Ordinal) == -1 && !string.IsNullOrEmpty(temp))
                {
                    result += "零" + temp;
                }
                else
                {
                    result += temp;
                }

                result += "万";
                temp = ToChineseTenThousandInt(x.Substring(len - 4, 4));
                if (temp.IndexOf("千", StringComparison.Ordinal) == -1 && !string.IsNullOrEmpty(temp))
                {
                    result += "零" + temp;
                }
                else
                {
                    result += temp;
                }
            }
            int i;
            if (( i = result.IndexOf("零万", StringComparison.Ordinal) ) != -1)
            {
                result = result.Remove(i + 1, 1);
            }

            while (( i = result.IndexOf("零零", StringComparison.Ordinal) ) != -1)
            {
                result = result.Remove(i, 1);
            }

            if (result[result.Length - 1] == '零' && result.Length > 1)
            {
                result = result.Remove(result.Length - 1, 1);
            }

            return result;
        }

        /// <summary>
        /// 0-9转化中文数字
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static char ToNum(this char x)
        {
            string strChnNames = "零一二三四五六七八九";
            string strNumNames = "0123456789";
            return strChnNames[strNumNames.IndexOf(x)];
        }


        /// <summary>
        /// 字符串掩码
        /// </summary>
        /// <param name="s">字符串</param>
        /// <param name="mask">掩码符</param>
        /// <returns></returns>
        public static string Mask(this string s, char mask = '*')
        {
            if (string.IsNullOrWhiteSpace(s?.Trim()))
            {
                return s;
            }
            s = s.Trim();
            string masks = mask.ToString().PadLeft(4, mask);
            return s.Length switch
            {
                >= 11 => Regex.Replace(s, "(.{3}).*(.{4})", $"$1{masks}$2"),
                10 => Regex.Replace(s, "(.{3}).*(.{3})", $"$1{masks}$2"),
                9 => Regex.Replace(s, "(.{2}).*(.{3})", $"$1{masks}$2"),
                8 => Regex.Replace(s, "(.{2}).*(.{2})", $"$1{masks}$2"),
                7 => Regex.Replace(s, "(.{1}).*(.{2})", $"$1{masks}$2"),
                6 => Regex.Replace(s, "(.{1}).*(.{1})", $"$1{masks}$2"),
                _ => Regex.Replace(s, "(.{1}).*", $"$1{masks}")
            };
        }


        /// <summary>
        /// 判断字符串是否为空或""
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s) || s.Equals("null", StringComparison.CurrentCultureIgnoreCase);
        }


        #region 检测字符串中是否包含列表中的关键词

        /// <summary>
        /// 检测字符串中是否包含列表中的关键词
        /// </summary>
        /// <param name="s">源字符串</param>
        /// <param name="keys">关键词列表</param>
        /// <param name="ignoreCase">忽略大小写</param>
        /// <returns></returns>
        public static bool Contains(this string s, IEnumerable<string> keys, bool ignoreCase = true)
        {
            if (!keys.Any() || string.IsNullOrEmpty(s))
            {
                return false;
            }

            if (ignoreCase)
            {
                return Regex.IsMatch(s, string.Join("|", keys.Select(Regex.Escape)), RegexOptions.IgnoreCase);
            }

            return Regex.IsMatch(s, string.Join("|", keys.Select(Regex.Escape)));
        }

        /// <summary>
        /// 检测字符串中是否以列表中的关键词结尾
        /// </summary>
        /// <param name="s">源字符串</param>
        /// <param name="keys">关键词列表</param>
        /// <param name="ignoreCase">忽略大小写</param>
        /// <returns></returns>
        public static bool EndsWith(this string s, string[] keys, bool ignoreCase = true)
        {
            if (keys.Length == 0 || string.IsNullOrEmpty(s))
            {
                return false;
            }

            return ignoreCase ? keys.Any(key => s.EndsWith(key, StringComparison.CurrentCultureIgnoreCase)) : keys.Any(s.EndsWith);
        }

        /// <summary>
        /// 检测字符串中是否包含列表中的关键词
        /// </summary>
        /// <param name="s">源字符串</param>
        /// <param name="regex">关键词列表</param>
        /// <param name="ignoreCase">忽略大小写</param>
        /// <returns></returns>
        public static bool RegexMatch(this string s, string regex, bool ignoreCase = true)
        {
            if (string.IsNullOrEmpty(regex) || string.IsNullOrEmpty(s))
            {
                return false;
            }

            if (ignoreCase)
            {
                return Regex.IsMatch(s, regex, RegexOptions.IgnoreCase);
            }

            return Regex.IsMatch(s, regex);
        }

        /// <summary>
        /// 检测字符串中是否包含列表中的关键词
        /// </summary>
        /// <param name="s">源字符串</param>
        /// <param name="regex">关键词列表</param>
        /// <returns></returns>
        public static bool RegexMatch(this string s, Regex regex) => !string.IsNullOrEmpty(s) && regex.IsMatch(s);

        /// <summary>
        /// 判断是否包含符号
        /// </summary>
        /// <param name="str"></param>
        /// <param name="symbols"></param>
        /// <returns></returns>
        public static bool ContainsSymbol(this string str, params string[] symbols)
        {
            return str switch
            {
                null => false,
                "" => false,
                _ => symbols.Any(str.Contains)
            };
        }

        #endregion 


        /// <summary>
        /// 任意进制转大数十进制
        /// </summary>
        /// <param name="str"></param>
        /// <param name="base">进制</param>
        /// <returns></returns>
        public static BigInteger FromBinaryBig(this string str, byte @base)
        {
            var nf = new NumberFormater(@base);
            return nf.FromStringBig(str);
        }


        /// <summary>
        /// 任意进制转十进制
        /// </summary>
        /// <param name="str"></param>
        /// <param name="base">进制</param>
        /// <returns></returns>
        public static long FromBinary(this string str, byte @base)
        {
            var nf = new NumberFormater(@base);
            return nf.FromString(str);
        }

        /// <summary>
        /// 生成唯一短字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="chars">可用字符数数量，0-9,a-z,A-Z</param>
        /// <returns></returns>
        public static string CreateShortToken(this string _, byte chars = 36)
        {
            var nf = new NumberFormater(chars);
            return nf.ToString(( DateTime.Now.Ticks - 630822816000000000 ) * 100 + Stopwatch.GetTimestamp() % 100);
        }


        public static string Join(this IEnumerable<string> strs, string separate = ", ") => string.Join(separate, strs);


        #region 邮箱
        /// <summary>
        /// 匹配Email
        /// </summary>
        /// <param name="s">源字符串</param>
        /// <returns>匹配对象；是否匹配成功，若返回true，则会得到一个Match对象，否则为null</returns>
        public static (bool isMatch, Match? match) MatchEmail(this string s)
        {
            if (string.IsNullOrEmpty(s) || s.Length < 7)
            {
                return (false, null);
            }

            var match = Regex.Match(s, @"[^@ \t\r\n]+@[^@ \t\r\n]+\.[^@ \t\r\n]+");
            var isMatch = match.Success;
            return (isMatch, match);
        }
        
        /// <summary>
        /// 邮箱掩码
        /// </summary>
        /// <param name="s">邮箱</param>
        /// <param name="mask">掩码</param>
        /// <returns></returns>
        public static string MaskEmail(this string s, char mask = '*')
        {
            var index = s.LastIndexOf("@");
            var oldValue = s.Substring(0, index);
            return !MatchEmail(s).isMatch ? s : s.Replace(oldValue, Mask(oldValue, mask));
        }
        #endregion


        #region 权威校验身份证号码
        /// <summary>
        /// 根据GB11643-1999标准权威校验中国身份证号码的合法性
        /// </summary>
        /// <param name="s">源字符串</param>
        /// <returns>是否匹配成功</returns>
        public static bool MatchIdentifyCard(this string s)
        {
            return s.Length switch
            {
                18 => CheckChinaID18(s),
                15 => CheckChinaID15(s),
                _ => false
            };
        }

        private static readonly string[] ChinaIDProvinceCodes = {
             "11", "12", "13", "14", "15",
             "21", "22", "23",
             "31", "32", "33", "34", "35", "36", "37",
             "41", "42", "43", "44", "45", "46",
             "50", "51", "52", "53", "54",
             "61", "62", "63", "64", "65",
             "71",
             "81", "82",
             "91"
        };

        private static bool CheckChinaID18(string ID)
        {
            ID = ID.ToUpper();
            Match m = Regex.Match(ID, @"\d{17}[\dX]", RegexOptions.IgnoreCase);
            if (!m.Success)
            {
                return false;
            }
            if (!ChinaIDProvinceCodes.Contains(ID.Substring(0, 2)))
            {
                return false;
            }
            CultureInfo zhCN = new CultureInfo("zh-CN", true);
            if (!DateTime.TryParseExact(ID.Substring(6, 8), "yyyyMMdd", zhCN, DateTimeStyles.None, out DateTime birthday))
            {
                return false;
            }
            if (!birthday.In(new DateTime(1800, 1, 1), DateTime.Today))
            {
                return false;
            }
            int[] factors = { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                sum += ( ID[i] - '0' ) * factors[i];
            }
            int n = ( 12 - sum % 11 ) % 11;
            return n < 10 ? ID[17] - '0' == n : ID[17].Equals('X');
        }

        private static bool CheckChinaID15(string ID)
        {
            Match m = Regex.Match(ID, @"\d{15}", RegexOptions.IgnoreCase);
            if (!m.Success)
            {
                return false;
            }
            if (!ChinaIDProvinceCodes.Contains(ID.Substring(0, 2)))
            {
                return false;
            }
            CultureInfo zhCN = new CultureInfo("zh-CN", true);
            if (!DateTime.TryParseExact("19" + ID.Substring(6, 6), "yyyyMMdd", zhCN, DateTimeStyles.None, out DateTime birthday))
            {
                return false;
            }
            return birthday.In(new DateTime(1800, 1, 1), new DateTime(2000, 1, 1));
        }

        #endregion



        #region IP

        /// <summary>
        /// IP地址转换成数字
        /// </summary>
        /// <param name="addr">IP地址</param>
        /// <returns>数字</returns>
        /// <exception cref="InvalidOperationException">输入的IP字符串不合法时抛出异常</exception>
        public static IPAddress ToIPAddress(this string addr)
        {
            if (!IPAddress.TryParse(addr, out var ip))
            {
                throw new InvalidOperationException("输入的IP地址不合法");
            }

            return ip;
        }



        #endregion


        #region 手机号码
        /// <summary>
        /// 匹配手机号码
        /// </summary>
        /// <param name="s">源字符串</param>
        /// <param name="isMatch">是否匹配成功，若返回true，则会得到一个Match对象，否则为null</param>
        /// <returns>匹配对象</returns>
        public static bool MatchPhoneNumber(this string s, out string matchValue)
        {
            if (string.IsNullOrEmpty(s))
            {
                matchValue = string.Empty;
                return false;
            }
            Match match = Regex.Match(s, @"^((1[3,5,6,8][0-9])|(14[5,7])|(17[0,1,3,6,7,8])|(19[8,9]))\d{8}$");
            if (match!=null&&match.Success)
            {
                matchValue = match.Value;
                return true;
            }

            matchValue = string.Empty;
            return false;
        }

        /// <summary>
        /// 匹配手机号码
        /// </summary>
        /// <param name="s">源字符串</param>
        /// <returns>是否匹配成功</returns>
        public static bool IsPhoneNumber(this string s)
        {
            return MatchPhoneNumber(s, out var _);
        }


        #endregion
    }
}
