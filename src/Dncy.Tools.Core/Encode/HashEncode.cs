using Dotnetydd.Tools.Core.Extension;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Dotnetydd.Tools.Core.Encode
{
    /// <summary>
    /// 得到随机安全码（哈希加密）。
    /// </summary>
    public static class HashEncode
    {
        /// <summary>
        /// 得到随机哈希加密字符串
        /// </summary>
        /// <returns>随机哈希加密字符串</returns>
        public static string GetSecurity(this Random r) => r.StrictNext().ToString().HashEncoding();

        /// <summary>
        /// 哈希加密一个字符串
        /// </summary>
        /// <param name="security">需要加密的字符串</param>
        /// <returns>加密后的数据</returns>
        public static string HashEncoding(this string security)
        {
            var code = new UnicodeEncoding();
            byte[] message = code.GetBytes(security);
            using var arithmetic = new SHA512Managed();
            var value = arithmetic.ComputeHash(message);
            var sb = new StringBuilder();
            foreach (byte o in value)
            {
                sb.Append((int)o + "O");
            }

            return sb.ToString();
        }
    }
}
