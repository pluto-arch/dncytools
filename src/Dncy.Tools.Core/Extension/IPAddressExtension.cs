using System;
/* 项目“Dotnetydd.Tools.Core (net46)”的未合并的更改
在此之前:
using System.Net;
在此之后:
using System.Net;
using Dncy;
using Dncy.Tools;
using Dotnetydd.Tools.Core.Extension;
*/
using System.Net;
using System.Net.Sockets;

namespace Dotnetydd.Tools.Core.Extension
{
    public static class IPAddressExtension
    {
        /// <summary>
        /// 判断IP是否是私有地址
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsPrivateIP(this IPAddress ip)
        {
            if (IPAddress.IsLoopback(ip)) return true;
#if NET40
            ip = ip.IsIPv4MappedToIPv6() ? ip.MapToIPv4() : ip;
#else
            ip = ip.IsIPv4MappedToIPv6 ? ip.MapToIPv4() : ip;
#endif

            byte[] bytes = ip.GetAddressBytes();
            return ip.AddressFamily switch
            {
                AddressFamily.InterNetwork when bytes[0] == 10 => true,
                AddressFamily.InterNetwork when bytes[0] == 100 && bytes[1] >= 64 && bytes[1] <= 127 => true,
                AddressFamily.InterNetwork when bytes[0] == 169 && bytes[1] == 254 => true,
                AddressFamily.InterNetwork when bytes[0] == 172 && bytes[1] == 16 => true,
                AddressFamily.InterNetwork when bytes[0] == 192 && bytes[1] == 88 && bytes[2] == 99 => true,
                AddressFamily.InterNetwork when bytes[0] == 192 && bytes[1] == 168 => true,
                AddressFamily.InterNetwork when bytes[0] == 198 && bytes[1] == 18 => true,
                AddressFamily.InterNetwork when bytes[0] == 198 && bytes[1] == 51 && bytes[2] == 100 => true,
                AddressFamily.InterNetwork when bytes[0] == 203 && bytes[1] == 0 && bytes[2] == 113 => true,
                AddressFamily.InterNetwork when bytes[0] >= 233 => true,
                AddressFamily.InterNetworkV6 when ip.IsIPv6Teredo || ip.IsIPv6LinkLocal || ip.IsIPv6Multicast || ip.IsIPv6SiteLocal || bytes[0] == 0 || bytes[0] >= 252 => true,
                _ => false
            };
        }

        /// <summary>
        /// IP地址转换成数字
        /// </summary>
        /// <param name="addr">IP地址</param>
        /// <returns>数字</returns>
        /// <exception cref="ArgumentNullException">输入的IP字符串不合法时抛出异常</exception>
        public static uint ToNumber(this IPAddress addr)
        {
            if (addr == null)
            {
                throw new ArgumentNullException(nameof(addr));
            }
            byte[] bInt = addr.GetAddressBytes();
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bInt);
            }

            return BitConverter.ToUInt32(bInt, 0);
        }

        /// <summary>
        /// 判断IP地址在不在某个IP地址段
        /// </summary>
        /// <param name="input">需要判断的IP地址</param>
        /// <param name="begin">起始地址</param>
        /// <param name="ends">结束地址</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">输入的IP字符串不合法时抛出异常</exception>
        public static bool IpAddressInRange(this IPAddress input, IPAddress begin, IPAddress ends)
        {
            if (input == null || begin == null || ends == null)
            {
                throw new InvalidOperationException("输入的IP地址不合法");
            }
            uint current = input.ToNumber();
            return current >= begin.ToNumber() && current <= ends.ToNumber();
        }


#if NET40
        private static ushort[] m_Numbers = new ushort[8];
        public static IPAddress MapToIPv4(this IPAddress ip)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip;
            }

            long newAddress = (uint)((int)((uint)(m_Numbers[6] & 0xFF00) >> 8) | (m_Numbers[6] & 0xFF) << 8 | ((int)((uint)(m_Numbers[7] & 0xFF00) >> 8) | (m_Numbers[7] & 0xFF) << 8) << 16);
            return new IPAddress(newAddress);
        }

        public static bool IsIPv4MappedToIPv6(this IPAddress ip)
        {
            if (ip.AddressFamily != AddressFamily.InterNetworkV6)
            {
                return false;
            }

            for (int i = 0; i < 5; i++)
            {
                if (m_Numbers[i] != 0)
                {
                    return false;
                }
            }

            return m_Numbers[5] == ushort.MaxValue;
        }

#endif
    }
}
