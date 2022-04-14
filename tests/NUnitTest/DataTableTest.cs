using Dncy.Tools;
using NUnit.Framework;
using NUnitTest.TestModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NUnitTest
{
    public class DataTableTest
    {
        [SetUp]
        public void Setup()
        {

        }
        private readonly List<User> _users = new List<User>();

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
        public void ToList_Test()
        {
            InitData();
            var dt = _users.ToDataTable();

        }
    }
}

