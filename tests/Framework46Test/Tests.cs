using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dotnetydd.Tools;
using Dotnetydd.Tools.Encode;
using NUnit.Framework;

namespace Framework46Test
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void Test_Random_GetSecurity()
        {
            var random = new Random();
            var res = random.GetSecurity();
            TestContext.WriteLine($"Random.GetSecurity : {res}");
            Assert.True(!string.IsNullOrEmpty(res));
        }
        
        [Test]
        public void Test_HashEncry()
        {
            var res = "abcdef".HashEncoding();
            TestContext.WriteLine(res);
        }

        [Test]
        public async Task Test_SnowFlake()
        {
            var sn = new SnowFlake();
            var l = new HashSet<long>();
            var t1 = Task.Run(async () =>
            {
                for (int i = 0; i < 1000; i++)
                {
                    l.Add(sn.GetLongId());
                }
            });
            var t2= Task.Run(() =>
            {
                for (int i = 0; i < 1000; i++)
                {
                    l.Add(sn.GetLongId());
                }
            });

            await Task.WhenAll(t1, t2);
            
            TestContext.WriteLine(l.Count);
            
            Assert.IsTrue(l.Count==2000);
        }
    }
}