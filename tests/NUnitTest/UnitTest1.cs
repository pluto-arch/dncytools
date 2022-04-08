using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
    }
}