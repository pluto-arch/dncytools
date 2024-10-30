using Dotnetydd.Tools;
using Dotnetydd.Tools.Date;
using Dotnetydd.Tools.Format;

namespace Net5678Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var a = DateTools.IsRuYear(2023);
            Assert.IsFalse(a);
            var b = DateTools.IsRuYear(2024);
            Assert.IsTrue(b);


            var convert = new NumberBaseConvertor(18);
            var res = convert.ToString(123);
            Assert.AreEqual("6f", res);
        }


        [Test]
        public void SnowFlakeTest()
        {
            HashSet<long> set = new HashSet<long>();

            foreach (var _ in Enumerable.Range(1,50000))
            {
                set.Add(SnowFlake.NewLongId);
            }

            Assert.That(set.Count, Is.EqualTo(50000));
        }
    }
}