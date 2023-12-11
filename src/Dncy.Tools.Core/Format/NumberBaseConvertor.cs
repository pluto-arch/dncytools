using System;

namespace Dotnetydd.Tools.Core.Format
{
    /// <summary>
    /// 数制转换器
    /// </summary>
    public class NumberBaseConvertor
    {
        /// <summary>
        /// 进制长度
        /// </summary>
        public int Length => Digits.Length;

        /// <summary>
        /// 进制字符集
        /// </summary>
        readonly string Digits = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        private byte radix = 10;

        /// <summary>
        /// 数制转换器
        /// init with 10 hex
        /// </summary>
        public NumberBaseConvertor()
        {
            Digits = "0123456789";
        }

        /// <summary>
        /// 数制格式化器
        /// </summary>
        /// <param name="base">进制</param>
        public NumberBaseConvertor(byte @base)
        {
            if (@base < 2)
            {
                @base = 2;
            }

            if (@base > 62)
            {
                throw new ArgumentException("默认进制最大支持62进制");
            }

            radix = @base;
        }


        /// <summary>
        /// 数值转对应进制
        /// </summary>
        /// <param name="decimalNumber"></param>
        /// <param name="radix">基数 【2-62】</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public string ToString(long decimalNumber)
        {
            const int BitsInLong = 64;

            if (radix < 2 || radix > Digits.Length)
                throw new ArgumentException($"The radix must be >= 2 and <= {Digits.Length}");

            if (decimalNumber == 0)
                return "0";

            int index = BitsInLong - 1;
            long currentNumber = Math.Abs(decimalNumber);
            char[] charArray = new char[BitsInLong];

            while (currentNumber != 0)
            {
                int remainder = (int)(currentNumber % radix);
                charArray[index--] = Digits[remainder];
                currentNumber = currentNumber / radix;
            }

            string result = new string(charArray, index + 1, BitsInLong - index - 1);
            if (decimalNumber < 0)
            {
                result = "-" + result;
            }

            return result;
        }


        /// <summary>
        /// 字符串转对应基数进制
        /// </summary>
        /// <param name="number"></param>
        /// <param name="radix">基数 【2-62】</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public long ToNumber(string number)
        {
            if (radix < 2 || radix > Digits.Length)
                throw new ArgumentException($"The radix must be >= 2 and <= {Digits.Length}");

            if (string.IsNullOrEmpty(number))
                return 0;

            long result = 0;
            long multiplier = 1;
            for (int i = number.Length - 1; i >= 0; i--)
            {
                char c = number[i];
                if (i == 0 && c == '-')
                {
                    result = -result;
                    break;
                }

                int digit = Digits.IndexOf(c);
                if (digit == -1)
                    throw new ArgumentException("Invalid character in the arbitrary numeral system number", nameof(number));

                result += digit * multiplier;
                multiplier *= radix;
            }

            return result;
        }


        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return radix + "进制模式，进制符：" + Digits;
        }
    }
}
