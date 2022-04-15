
#if NETCOREAPP || NETSTANDARD || (NET46_OR_GREATER)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Dncy.Tools;
using Dncy.Tools.LuceneNet;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Search;
using NUnit.Framework;
using NUnitTest.TestModels;

namespace NUnitTest
{
    public class LunceneSearchEngineTest
    {
        [SetUp]
        public void Setup()
        {

        }
        private readonly List<Person> _users = new List<Person>();

        private void InitData()
        {
            _users.Clear();
            foreach (var item in Enumerable.Range(1, 9999))
            {
                _users.Add(new Person
                {
                    Id = item,
                    Name = GenerateChineseWord(2),
                    Remarks = "今天(18日)，由中国社会科学院主办、中国社会科学院考古研究所和考古杂志社承办的“中国社会科学院考古学论坛·2021年中国考古新发现” 在北京举行。与此同时，正式公布2021年中国考古新发现最终入选的6个田野考古发掘项目。"
                });
            }
        }

        [Test]
        public void LuceneSearch_Test()
        {
            InitData();
#if !NETCOREAPP
            var searchEngine = new LuceneSearchEngine(new LuceneSearchEngineOptions
            {
                IndexDir = AppDomain.CurrentDomain.BaseDirectory,
                Analyzer = new SimpleAnalyzer(LuceneSearchEngine._luceneVersion)
            });
            var idx = searchEngine.CreateIndex(_users);
            Assert.IsTrue(idx);


            var res = searchEngine.Search<Person>(new SearchModel
            {
                Keywords = "中国社会科学院",
                MaximumNumberOfHits = 3000,
                OrderBy = new List<SortField>
                {
                    SortField.FIELD_SCORE
                },
                Skip = 0,
                Take = 100,
                Score = 3,
                Fields = new List<string>{nameof(Person.Name),nameof(Person.Remarks)}
            });


            Console.WriteLine(res.total);
#endif

        }




        /// <summary>
        /// 随机产生常用汉字
        /// </summary>
        /// <param name="count">要产生汉字的个数</param>
        /// <returns>常用汉字</returns>
        public static string GenerateChineseWord(int count)
        {
            string chineseWords = "";
            System.Random rm = new System.Random();
            var gb = Encoding.GetEncoding("gb2312");
            for (int i = 0; i < count; i++)
            {
                int regionCode = rm.Next(16, 56);

                int positionCode;
                if (regionCode == 55)
                {
                    positionCode = rm.Next(1, 90);
                }
                else
                {
                    positionCode = rm.Next(1, 95);
                }
                int regionCode_Machine = regionCode + 160;
                int positionCode_Machine = positionCode + 160;
                byte[] bytes = new byte[] { (byte)regionCode_Machine, (byte)positionCode_Machine };
                chineseWords += gb.GetString(bytes);
            }
            return chineseWords;
        }
    }


    public class Person
    {
        [LuceneIndex("Id")]
        public int Id { get; set; }

        [LuceneIndex("Name")]
        public string Name { get; set; }

        [LuceneIndex("Remarks")]
        public string Remarks { get; set; }
    }
}
#endif
