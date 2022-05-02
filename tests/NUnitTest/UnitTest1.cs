using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Xml.Linq;

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

#if  NET6_0
        public enum Demo
        {
            [System.ComponentModel.Description("123123")]
            A = 1,
            [System.ComponentModel.DataAnnotations.Display(Name = "已退回")]
            B = 2,
            C = 3
        }
        [Test]
        public void EnumExt_Text()
        {
            var ddd = typeof(Demo).GetNameValueDictionary();

            var sdsd = Demo.A.GetDescription();

            var sdsd22 = Demo.A.GetDisplay();

            Assert.IsTrue(ddd.Count == 3);
        }
#endif



        [Test]
        public void NumberFormat_Text()
        {
            var number = SnowFlake.NewLongId;
            Console.WriteLine("10 : " + number.ToString());
            Console.WriteLine("2  : " + number.ToNewBase(2));
            Console.WriteLine("8  : " + number.ToNewBase(8) );
            Console.WriteLine("16 : " + number.ToNewBase(16));
            Console.WriteLine("26 : " + number.ToNewBase(26));
            Console.WriteLine("32 : " + number.ToNewBase(32));
            Console.WriteLine("36 : " + number.ToNewBase(36));
            Console.WriteLine("52 : " + number.ToNewBase(52));
            Console.WriteLine("58 : " + number.ToNewBase(58));
            Console.WriteLine("62 : " + number.ToNewBase(62));
            Console.WriteLine("------");
            number = 0-number;
            Console.WriteLine("10 : " + number.ToString());
            Console.WriteLine("2  : " + number.ToNewBase(2 ));
            Console.WriteLine("8  : " + number.ToNewBase(8 ));
            Console.WriteLine("16 : " + number.ToNewBase(16));
            Console.WriteLine("26 : " + number.ToNewBase(26));
            Console.WriteLine("32 : " + number.ToNewBase(32));
            Console.WriteLine("36 : " + number.ToNewBase(36));
            Console.WriteLine("52 : " + number.ToNewBase(52));
            Console.WriteLine("58 : " + number.ToNewBase(58));
            Console.WriteLine("62 : " + number.ToNewBase(62));


            number = 0-number;
            Console.WriteLine("from 2  : " + number.ToNewBase(2).ToNumber( 2 ) );
            Console.WriteLine("from 8  : " + number.ToNewBase(8).ToNumber( 8 ) );
            Console.WriteLine("from 16 : " + number.ToNewBase(16).ToNumber(16));
            Console.WriteLine("from 26 : " + number.ToNewBase(26).ToNumber(26));
            Console.WriteLine("from 32 : " + number.ToNewBase(32).ToNumber(32));
            Console.WriteLine("from 36 : " + number.ToNewBase(36).ToNumber(36));
            Console.WriteLine("from 52 : " + number.ToNewBase(52).ToNumber(52));
            Console.WriteLine("from 58 : " + number.ToNewBase(58).ToNumber(58));
            Console.WriteLine("from 62 : " + number.ToNewBase(62).ToNumber(62));

            number = 0-number;
            Console.WriteLine("from 2  : " + number.ToNewBase(2).ToNumber( 2 ) );
            Console.WriteLine("from 8  : " + number.ToNewBase(8).ToNumber( 8 ) );
            Console.WriteLine("from 16 : " + number.ToNewBase(16).ToNumber(16));
            Console.WriteLine("from 26 : " + number.ToNewBase(26).ToNumber(26));
            Console.WriteLine("from 32 : " + number.ToNewBase(32).ToNumber(32));
            Console.WriteLine("from 36 : " + number.ToNewBase(36).ToNumber(36));
            Console.WriteLine("from 52 : " + number.ToNewBase(52).ToNumber(52));
            Console.WriteLine("from 58 : " + number.ToNewBase(58).ToNumber(58));
            Console.WriteLine("from 62 : " + number.ToNewBase(62).ToNumber(62));

        }

    }
}