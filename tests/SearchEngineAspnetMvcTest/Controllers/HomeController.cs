using System.Diagnostics;
using Dncy.SnowFlake;
using Dncy.Tools.LuceneNet;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Util;

using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SearchEngineAspnetMvcTest.Models;

namespace SearchEngineAspnetMvcTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly LuceneSearchEngine _luceneSearchEngine;

        public HomeController(ILogger<HomeController> logger, LuceneSearchEngine luceneSearchEngine)
        {
            _logger = logger;
            _luceneSearchEngine = luceneSearchEngine;
            InitData();
        }


        private static readonly List<Person> _users = new List<Person>();

        private void InitData()
        {
            _users.Clear();
            var contentPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "content.txt");
            foreach (var item in System.IO.File.ReadLines(contentPath))
            {
                if (string.IsNullOrEmpty(item))
                {
                    continue;
                }
                _users.Add(new Person
                {
                    Id = SnowFlake.NewLongId,
                    Name = item.Substring(item.Length>10?10:0, item.Length > 40 ? 40 : item.Length),
                    Remarks = item
                });
            }
        }



        public IActionResult Index()
        {
            var idx = _luceneSearchEngine.CreateIndex(_users);
            ViewBag.Success = idx;
            return View();
        }

       
        public IActionResult Privacy([FromForm]string keyWords,[FromForm]string highlight)
        {
            if (string.IsNullOrEmpty(keyWords))
            {
                return View(nameof(Index));
            }
            var parser = new QueryParser(LuceneSearchEngine.LuceneVersion, nameof(Person.Remarks), _luceneSearchEngine.Analyzer);
            var query = parser.Parse(keyWords);
            var searchResult = _luceneSearchEngine.Search<Person>(new SearchModel(query, 100)
            {
                OrderBy = new SortField[] { SortField.FIELD_SCORE },
                Skip = 0,
                Take = 20,
                Score = 0,
                OnlyTyped = true,
                HighlightTag = ($"<a style='color:{highlight}'>", "</a>")
            });
            ViewBag.keyWords = keyWords;
            return View(searchResult);
        }


        public IActionResult Delete([FromForm]string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return View(nameof(Index));
            }

            _luceneSearchEngine.DeleteDocumentByDataId(nameof(Person.Id), id);

            return View(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}