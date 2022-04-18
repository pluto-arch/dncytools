using System.Linq;
using System.Text;

namespace Dncy.Tools
{
    public static partial class StringExtension
    {
        /// <summary>
        /// 获取字符串crc32签名
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Crc32(this string s)
        {
            return string.Join(string.Empty, new Security.Crc32().ComputeHash(Encoding.UTF8.GetBytes(s)).Select(b => b.ToString("x2")));
        }


        /// <summary>
        /// 获取字符串crc64签名
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Crc64(this string s)
        {
            return string.Join(string.Empty, new Security.Crc64().ComputeHash(Encoding.UTF8.GetBytes(s)).Select(b => b.ToString("x2")));
        }
    }
}
