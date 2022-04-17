using System.Diagnostics;
using Dncy.SnowFlake;
using Dncy.Tools.LuceneNet;
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
            var contentPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "content.txt");//$"{AppDomain.CurrentDomain.BaseDirectory}/content.txt";
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
            // var idx = _luceneSearchEngine.CreateIndex(_users);
            ViewBag.Success = true;
            return View();
        }

        public IActionResult Privacy([FromForm]string keyWords,[FromForm]string highlight)
        {
            var parser = new QueryParser(LuceneVersion.LUCENE_48, nameof(Person.Remarks), _luceneSearchEngine.Analyzer);
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}