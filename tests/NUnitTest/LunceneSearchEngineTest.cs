
#if NETCOREAPP || NETSTANDARD || (NET46_OR_GREATER)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Dncy.SnowFlake;
using Dncy.Tools;
using Dncy.Tools.LuceneNet;

using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Cn.Smart;
using Lucene.Net.Analysis.Cn.Smart.Hhmm;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;

using Lucene.Net.Util;
using Newtonsoft.Json;
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
            var contentPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestModels", "content.txt");//$"{AppDomain.CurrentDomain.BaseDirectory}/content.txt";
            foreach (var item in File.ReadLines(contentPath))
            {
                if (string.IsNullOrEmpty(item))
                {
                    continue;
                }
                _users.Add(new Person
                {
                    Id = SnowFlake.NewLongId,
                    Name = item.Substring(0, item.Length > 20 ? 20 : item.Length),
                    Remarks = item
                });
            }
        }

        [Test]
        public void LuceneCreateIndex_Test()
        {
            InitData();
#if !NETCOREAPP
            using var searchEngine = new LuceneSearchEngine(new SmartChineseAnalyzer(LuceneSearchEngine.LuceneVersion),new LuceneSearchEngineOptions
            {
                IndexDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,  "luceneIndexs"),
            },new NewtonsoftMessageSerializeProvider());
            var idx = searchEngine.CreateIndex(_users);
            Assert.IsTrue(idx);
#endif

        }


        [Test]
        public void LuceneSearch_Test()
        {
#if !NETCOREAPP
            using var searchEngine = new LuceneSearchEngine(new SmartChineseAnalyzer(LuceneSearchEngine.LuceneVersion),new LuceneSearchEngineOptions
            {
                IndexDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,  "luceneIndexs"),
            },new NewtonsoftMessageSerializeProvider());

            var parser = new QueryParser(LuceneVersion.LUCENE_48, nameof(Person.Remarks), searchEngine.Analyzer);
            var query = parser.Parse("掉多少根头发");

            var res2 = searchEngine.Search<Person>(new SearchModel(query,100)
            {
                OrderBy = new SortField[] { SortField.FIELD_SCORE },
                Skip = 0,
                Take = 20,
                Score = 0,
                OnlyTyped = true,
                HighlightTag = ("<a style='color:green'>", "</a>")
            });

            Assert.IsTrue(res2.TotalHits > 0);


#endif

        }



        [Test]
        public void IndexInfo_Test()
        {
#if !NETCOREAPP

            var sds=  LuceneSearchEngine.GetKeyWords("asp.net core中托管SPA应用",true,new List<string>{"的","中",","});

            using var searchEngine = new LuceneSearchEngine(new SmartChineseAnalyzer(LuceneSearchEngine.LuceneVersion),new LuceneSearchEngineOptions
            {
                IndexDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,  "luceneIndexs"),
            },new NewtonsoftMessageSerializeProvider());

            var indexInfo = searchEngine.CurrentIndexInfo();
#endif

        }



        [Test]
        public void DeleteDocument_Test()
        {
#if !NETCOREAPP
            using var searchEngine = new LuceneSearchEngine(new SmartChineseAnalyzer(LuceneSearchEngine.LuceneVersion),new LuceneSearchEngineOptions
            {
                IndexDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,  "luceneIndexs"),
            },new NewtonsoftMessageSerializeProvider());

            //var parser = new QueryParser(LuceneVersion.LUCENE_48, nameof(Person.Remarks), searchEngine.Analyzer);
            //var query = parser.Parse("掉多少根头发");


            var query = new TermQuery(new Term(nameof(Person.Id), 4036255564626395142.ToString()));
            //query = new QueryBuilder(searchEngine.Analyzer).CreateBooleanQuery(nameof(Person.Name), "屠格涅夫曾经提到过，你想成为幸福的人吗？", Occur.MUST);

            var res2 = searchEngine.Search<Person>(new SearchModel(query, 100)
            {
                OrderBy = new SortField[] { SortField.FIELD_SCORE },
                Skip = 0,
                Take = 20,
                Score = 0,
                OnlyTyped = true,
                HighlightTag = ("<a style='color:green'>", "</a>")
            });


            Assert.IsTrue(res2.TotalHits > 0);

            var id = res2.Results.First().Data.Id;

            var res = searchEngine.DeleteDocumentByDataId(nameof(Person.Id),id.ToString());

            Assert.IsTrue(res);
#endif

        }        

    }


    public class Person
    {
        [LuceneIndexed("Id",true)]
        public long Id { get; set; }

        [LuceneIndexed("Name",false)]
        public string Name { get; set; }

        [LuceneIndexed("Remarks", false, IsTextField = true,IsHighLight = true,HightLightMaxNumber = 100)]
        public string Remarks { get; set; }
    }


    public class NewtonsoftMessageSerializeProvider : IFieldSerializeProvider
    {

        /// <inheritdoc />
        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        /// <inheritdoc />
        public T? Deserialize<T>(string objStr)
        {
            return JsonConvert.DeserializeObject<T>(objStr);
        }

        /// <inheritdoc />
        public object? Deserialize(string objStr, Type type)
        {
            return JsonConvert.DeserializeObject(objStr, type);
        }

        public void Dispose()
        {
        }
    }
}
#endif
