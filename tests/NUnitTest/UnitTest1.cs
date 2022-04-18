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
            var shortId = id.ToBinary(32);

#if !NET40
            
            ConcurrentHashSet<long> longIds = new ConcurrentHashSet<long>();
            ConcurrentHashSet<string> uniqueIds = new ConcurrentHashSet<string>();

            Parallel.ForEach(Enumerable.Range(0, 20000), (i) =>
            {
                var id = SnowFlake.NewLongId;
                var id2 = snowFlake_in_datacenter1.GetLongId();
                if (longIds.Contains(id)||longIds.Contains(id2))
                {
                    Assert.Fail();
                }
                else
                {
                    longIds.Add(id);
                    longIds.Add(id2);
                }

                var uniqueId = SnowFlake.NewUniqueId;
                var id3 = snowFlake_in_datacenter1.GetUniqueId();
                if (uniqueIds.Contains(uniqueId)||uniqueIds.Contains(id3))
                {
                    Assert.Fail();
                }
                else
                {
                    uniqueIds.Add(uniqueId);
                    uniqueIds.Add(id3);
                }
            });

            
            Assert.IsTrue(longIds.Count == 40000);
            Assert.IsTrue(uniqueIds.Count == 40000);
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



    }
}