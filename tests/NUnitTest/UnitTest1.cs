using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Dncy.SnowFlake;
using Dncy.Tools;

using NUnit.Framework;
using NUnitTest.TestModels;

namespace NUnitTest
{
    public class UnitTest1
    {
        private readonly List<User> _users = new List<User>();

        [SetUp]
        public void Setup()
        {
            
        }

        private void InitData()
        {
            _users.Clear();
            foreach (var item in Enumerable.Range(1, 9999))
            {
                _users.Add(new User
                {
                    Id = item,
                    Name = $"User {item}",
                    Age = item % 2,
                    CreateTime = DateTime.Now,
                });
            }
        }


        [Test]
        public void EnumerableExtension_Test()
        {
            InitData();
            var a = _users.ToDataTable();

#if NETFRAMEWORK || NET5_0
            var b = _users.DistinctBy(x => x.Age);
            Assert.IsTrue(b.Count() == 2);
#endif
        }

        [Test]
        public void LinqExtension_Test()
        {
            InitData();
            Expression<Func<User, bool>> predicate = x => x.Id > 0;
            predicate = predicate.And(x => x.Id == 1);

            var c = _users.Where(predicate.Compile()).ToList();
            Assert.IsTrue(c.Count() == 1);

            predicate = predicate.Or(x => x.Id == 2);
            var d = _users.Where(predicate.Compile()).ToList();
            Assert.IsTrue(d.Count() == 2);
        }

        [Test]
        public void ChineseCalendar_Test()
        {
            var dd = new ChineseCalendar(DateTime.Now);
            var sd = dd.ChineseCalendarHoliday;
            Console.WriteLine(sd);

            sd = dd.ChineseDateString;
            Console.WriteLine(sd);


            sd = dd.ChineseHour;
            Console.WriteLine(sd);
        }

        private static SnowFlake snowFlake_in_datacenter1 = new SnowFlake(1);
        [Test]
        public void SnowFlake_Test()
        {

            var id = SnowFlake.NewLongId;

            var shortId2 = id.ToNewBase(62);
            var or2 = shortId2.ToNumber(62);
            
            var shortId = (-id).ToNewBase(62);
            var or = shortId.ToNumber(62);

#if !NET40
            
            ConcurrentHashSet<long> longIds = new ConcurrentHashSet<long>();
            ConcurrentHashSet<string> uniqueIds = new ConcurrentHashSet<string>();

            Parallel.ForEach(Enumerable.Range(0, 900000), (i) =>
            {
                var id = SnowFlake.NewLongId;
                longIds.Add(id);
                var shortId = id.ToNewBase(32);
                uniqueIds.Add(shortId);
                var or = shortId.ToNumber(32);
                Assert.IsTrue(or == id);
            });
            Assert.IsTrue(longIds.Count==900000);
            Assert.IsTrue(longIds.Count==uniqueIds.Count);
            Assert.IsTrue(longIds.Count == 900000);
            Assert.IsTrue(uniqueIds.Count == 900000);
#endif
        }

        [Test]
        public void StringExt_Text()
        {
            var ss = "123".ToChineseTenThousandInt();
            Console.WriteLine(ss);
            Assert.IsTrue("一百二十三" == ss);

            ss = 123.22.ToChineseNumber();
            Console.WriteLine(ss);
            Assert.IsTrue("一百二十三点二二" == ss);

            ss = 123.22.ToChineseMoney();
            Console.WriteLine(ss);
            Assert.IsTrue("壹佰贰拾叁元贰角贰分" == ss);

            var isMatch = "18530064437".MatchPhoneNumber(out var res);
            Console.WriteLine(res);
            Assert.IsTrue(isMatch);


            var ipNumber = "127.0.0.1".ToIPAddress().ToNumber(); ;
            Console.WriteLine(ipNumber);


            var encryptStr = "1231qwe".Crc32();
            Console.WriteLine(encryptStr);
            encryptStr = "1231qwe222".Crc64();
            Console.WriteLine(encryptStr);
        }


        public enum Demo
        {
            A=1,
            B=2,
            C=3
        }
        [Test]
        public void EnumExt_Text()
        {
            var ddd = typeof(Demo).GetNameValueDictionary();
            Assert.IsTrue(ddd.Count == 3);
        }


        [Test]
        public void NumberFormat_Text()
        {
            //var number = SnowFlake.NewLongId;
            //Console.WriteLine("10 : " + number.ToString());
            //Console.WriteLine("2  : " + DecimalToArbitrarySystem(number,  2));
            //Console.WriteLine("8  : " + DecimalToArbitrarySystem(number,  8));
            //Console.WriteLine("16 : " + DecimalToArbitrarySystem(number, 16));
            //Console.WriteLine("26 : " + DecimalToArbitrarySystem(number, 26));
            //Console.WriteLine("32 : " + DecimalToArbitrarySystem(number, 32));
            //Console.WriteLine("36 : " + DecimalToArbitrarySystem(number, 36));
            //Console.WriteLine("52 : " + DecimalToArbitrarySystem(number, 52));
            //Console.WriteLine("58 : " + DecimalToArbitrarySystem(number, 58));
            //Console.WriteLine("62 : " + DecimalToArbitrarySystem(number, 62));
            //Console.WriteLine("------");
            //number = -number;
            //Console.WriteLine("10 : " + number.ToString());
            //Console.WriteLine("2  : " + DecimalToArbitrarySystem(number,  2));
            //Console.WriteLine("8  : " + DecimalToArbitrarySystem(number,  8));
            //Console.WriteLine("16 : " + DecimalToArbitrarySystem(number, 16));
            //Console.WriteLine("26 : " + DecimalToArbitrarySystem(number, 26));
            //Console.WriteLine("32 : " + DecimalToArbitrarySystem(number, 32));
            //Console.WriteLine("36 : " + DecimalToArbitrarySystem(number, 36));
            //Console.WriteLine("52 : " + DecimalToArbitrarySystem(number, 52));
            //Console.WriteLine("58 : " + DecimalToArbitrarySystem(number, 58));
            //Console.WriteLine("62 : " + DecimalToArbitrarySystem(number, 62));


            
            Console.WriteLine("from 2  : " + ArbitraryToDecimalSystem("11100000000101101000100010110101111001100000000000000000000000",  2));
            Console.WriteLine("from 8  : " + ArbitraryToDecimalSystem("340055042657140000000",  8));
            Console.WriteLine("from 16 : " + ArbitraryToDecimalSystem("3805a22d79800000", 16));
            Console.WriteLine("from 26 : " + ArbitraryToDecimalSystem("1g7lpa3mk6iaik", 26));
            Console.WriteLine("from 32 : " + ArbitraryToDecimalSystem("3g1d25lso0000", 32));
            Console.WriteLine("from 36 : " + ArbitraryToDecimalSystem("uo42209wj474", 36));
            Console.WriteLine("from 52 : " + ArbitraryToDecimalSystem("rM7d1tJLFzk", 52));
            Console.WriteLine("from 58 : " + ArbitraryToDecimalSystem("9ls2SzUk9pi", 58));
            Console.WriteLine("from 62 : " + ArbitraryToDecimalSystem("4OcDuFFnDQk", 62));


            Console.WriteLine("from 2  : " + ArbitraryToDecimalSystem("-11100000000101101000100010110101111001100000000000000000000000",  2));
            Console.WriteLine("from 8  : " + ArbitraryToDecimalSystem("-340055042657140000000",  8));
            Console.WriteLine("from 16 : " + ArbitraryToDecimalSystem("-3805a22d79800000", 16));
            Console.WriteLine("from 26 : " + ArbitraryToDecimalSystem("-1g7lpa3mk6iaik", 26));
            Console.WriteLine("from 32 : " + ArbitraryToDecimalSystem("-3g1d25lso0000", 32));
            Console.WriteLine("from 36 : " + ArbitraryToDecimalSystem("-uo42209wj474", 36));
            Console.WriteLine("from 52 : " + ArbitraryToDecimalSystem("-rM7d1tJLFzk", 52));
            Console.WriteLine("from 58 : " + ArbitraryToDecimalSystem("-9ls2SzUk9pi", 58));
            Console.WriteLine("from 62 : " + ArbitraryToDecimalSystem("-4OcDuFFnDQk", 62));

        }


        /// <summary>
        /// Converts the given decimal number to the numeral system with the
        /// specified radix (in the range [2, 62]).
        /// </summary>
        /// <param name="decimalNumber">The number to convert.</param>
        /// <param name="radix">The radix of the destination numeral system
        /// (in the range [2, 62]).</param>
        /// <returns></returns>
        public static string DecimalToArbitrarySystem(long decimalNumber, int radix)
        {
            const int BitsInLong = 64;
            const string Digits = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

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

            string result = new String(charArray, index + 1, BitsInLong - index - 1);
            if (decimalNumber < 0)
            {
                result = "-" + result;
            }

            return result;
        }


        /// <summary>
        /// Converts the given number from the numeral system with the specified
        /// radix (in the range [2, 62]) to decimal numeral system.
        /// </summary>
        /// <param name="number">The arbitrary numeral system number to convert.</param>
        /// <param name="radix">The radix of the numeral system the given number
        /// is in (in the range [2, 62]).</param>
        /// <returns></returns>
        public static long ArbitraryToDecimalSystem(string number, int radix)
        {
            const string Digits = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

            if (radix < 2 || radix > Digits.Length)
                throw new ArgumentException($"The radix must be >= 2 and <= {Digits.Length}");

            if (String.IsNullOrEmpty(number))
                return 0;

            long result = 0;
            long multiplier = 1;
            for (int i = number.Length - 1; i >= 0; i--)
            {
                char c = number[i];
                if (i == 0 && c == '-')
                {
                    // This is the negative sign symbol
                    result = -result;
                    break;
                }

                int digit = Digits.IndexOf(c);
                if (digit == -1)
                    throw new ArgumentException("Invalid character in the arbitrary numeral system number", "number");

                result += digit * multiplier;
                multiplier *= radix;
            }

            return result;
        }

    }
}