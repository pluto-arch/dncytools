using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Dncy.Tools
{
    /// <summary>
    /// 数制格式化器
    /// </summary>
    public class NumberFormater
    {
        /// <summary>
        /// 数制表示字符集
        /// </summary>
        private string Characters { get; set; }

        /// <summary>
        /// 进制长度
        /// </summary>
        public int Length => Characters.Length;

        /// <summary>
        /// 起始值偏移
        /// </summary>
        private readonly byte _offset;

        /// <summary>
        /// 数制格式化器
        /// </summary>
        public NumberFormater()
        {
            Characters = "0123456789";
        }

        /// <summary>
        /// 数制格式化器
        /// </summary>
        /// <param name="characters">符号集</param>
        /// <param name="offset">起始值偏移</param>
        public NumberFormater(string characters, byte offset = 0)
        {
            if (string.IsNullOrEmpty(characters))
            {
                throw new ArgumentException("符号集不能为空");
            }

            Characters = characters;
            _offset = offset;
        }

        /// <summary>
        /// 数制格式化器
        /// </summary>
        /// <param name="base">进制</param>
        /// <param name="offset">起始值偏移</param>
        public NumberFormater(byte @base, byte offset = 0)
        {
            if (@base < 2)
            {
                @base = 2;
            }

            if (@base > 64)
            {
                throw new ArgumentException("默认进制最大支持64进制");
            }

            Characters = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ+/".Substring(0, @base);
            if (offset >= @base)
            {
                throw new ArgumentException("偏移量不能超过进制基数" + @base);
            }

            _offset = offset;
        }

        /// <summary>
        /// 数字转换为指定的进制形式字符串
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public string ToString(long number)
        {
            int start = 0;
            int resultOffset = 0;
            if (_offset > 0)
            {
                start = 1;
                resultOffset = _offset - 1;
            }

            number = number - resultOffset;
            List<string> result = new List<string>();
            long t = Math.Abs(number);
            while (t != 0)
            {
                var mod = t % Length;
                t = Math.Abs(t / Length);
                var character = Characters[Convert.ToInt32(mod) - start].ToString();
                result.Insert(0, character);
            }

            if (number < 0)
            {
                result.Insert(0, "-");
            }

            return string.Join("", result);
        }

        /// <summary>
        /// 数字转换为指定的进制形式字符串
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public string ToString(BigInteger number)
        {
            int start = 0;
            int resultOffset = 0;
            if (_offset > 0)
            {
                start = 1;
                resultOffset = _offset - 1;
            }

            number = number - resultOffset;
            List<string> result = new List<string>();
            if (number < 0)
            {
                number = -number;
                result.Add("0");
            }

            BigInteger t = number;

            while (t != 0)
            {
                var mod = t % Length;
                t = BigInteger.Abs(BigInteger.Divide(t, Length));
                var character = Characters[(int)mod - start].ToString();
                result.Insert(0, character);
            }

            return string.Join("", result);
        }

        /// <summary>
        /// 指定字符串转换为指定进制的数字形式
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public long FromString(string str)
        {
            byte start = 0;
            int resultOffset = 0;
            if (_offset > 0)
            {
                start = 1;
                resultOffset = _offset - 1;
            }

            int j = 0;
            return new string(str.ToCharArray().Reverse().ToArray()).Where(ch => Characters.Contains(ch)).Sum(ch => ( Characters.IndexOf(ch) + start ) * (long)Math.Pow(Length, j++)) + resultOffset;
        }

        /// <summary>
        /// 指定字符串转换为指定进制的大数形式
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public BigInteger FromStringBig(string str)
        {
            byte start = 0;
            int resultOffset = 0;
            if (_offset > 0)
            {
                start = 1;
                resultOffset = _offset - 1;
            }
            int j = 0;
            var chars = new string(str.ToCharArray().Reverse().ToArray()).Where(ch => Characters.Contains(ch));
            return chars.Aggregate(BigInteger.Zero, (current, c) => current + ( Characters.IndexOf(c) + start ) * BigInteger.Pow(Length, j++)) + resultOffset;
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return Length + "进制模式，进制符：" + Characters;
        }
    }
}
