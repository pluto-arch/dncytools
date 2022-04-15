using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Lucene.Net.Analysis.Cn.Smart;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Microsoft.Extensions.Options;
using Directory = System.IO.Directory;

namespace Dncy.Tools.LuceneNet
{
    public class LuceneSearchEngine
    {

        private readonly LuceneSearchEngineOptions _options;
        public const LuceneVersion _luceneVersion =LuceneVersion.LUCENE_48;

#if NETCOREAPP
        public LuceneSearchEngine(IOptions<LuceneSearchEngineOptions> options)
        {
            _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        }
#else
        public LuceneSearchEngine(LuceneSearchEngineOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _ = _options.IndexDir ?? throw new ArgumentNullException(nameof(options.IndexDir));
            _ = _options.Analyzer ?? throw new ArgumentNullException(nameof(options.Analyzer));
        }
#endif
       
        

        /// <summary>
        /// 创建索引
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="datas"></param>
        /// <param name="recreate"></param>
        /// <returns></returns>
        public bool CreateIndex<T>(IEnumerable<T> datas,bool recreate=false)
        {
            var directory = OpenDirectory<T>();

            if (IndexWriter.IsLocked(directory))
            {
                //  若是索引目录被锁定（好比索引过程当中程序异常退出），则首先解锁
                //  Lucene.Net在写索引库以前会自动加锁，在close的时候会自动解锁
                IndexWriter.Unlock(directory);
            }

            //Lucene的index模块主要负责索引的建立
            using var writer = new IndexWriter(directory,new IndexWriterConfig(_luceneVersion,_options.Analyzer));
            // 删除重建
            if (recreate)
            {
                writer.DeleteAll();
                writer.Commit();
            }

            // 遍历实体集，添加到索引库
            foreach (var entity in datas)
            {
                writer.AddDocument(ToDocument(entity));
            }
            writer.Flush(true, true);
            return true;
        }

        
        /// <summary>
        /// 搜索
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public (int total,List<dynamic> datas) Search<T>(SearchModel search)
        {
            var directory = OpenDirectory<T>();
            using var reader = DirectoryReader.Open(directory);
            var searcher = new IndexSearcher(reader);
            Query quer = null;
            if (search.Fields.Count == 1)
            {
                var query = new QueryBuilder(_options.Analyzer).CreatePhraseQuery(search.Fields[0], search.Keywords);
                quer = query;
            }
            else
            {
                var finalQuery = new BooleanQuery();
                foreach (var field in search.Fields)
                {
                    var query = new QueryBuilder(_options.Analyzer).CreatePhraseQuery(field, search.Keywords);
                    finalQuery.Add(query, Occur.SHOULD);
                }
                quer = finalQuery;
            }
            // 排序规则处理
            var sort = new Sort(search.OrderBy.ToArray());
            Expression<Func<ScoreDoc, bool>> where = m => m.Score >= search.Score;
            // 过滤掉已经设置了类型的对象
            where = where.And(m => typeof(T).AssemblyQualifiedName == searcher.Doc(m.Doc).Get("Type"));
            var matches = searcher.Search(quer, search.Filter, search.MaximumNumberOfHits, sort, true, true).ScoreDocs.Where(where.Compile());
            var total = matches.Count();
            if (total<=0)
            {
                return (total, new List<dynamic>());
            }
            // 分页处理
            if (search.Skip.HasValue)
            {
                matches = matches.Skip(search.Skip.Value);
            }
            if (search.Take.HasValue)
            {
                matches = matches.Take(search.Take.Value);
            }
            var docs = matches.ToList();
            var results = new List<dynamic>();
            
            foreach (var match in docs)
            {
                var doc = searcher.Doc(match.Doc);
                results.Add(new 
                {
                    Score = match.Score,
                    Document = doc
                });
            }

            return (total, results);
        }



        /// <summary>
        /// 删除索引
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool DeleteIndex<T>(IEnumerable<T> datas)
        {
            var directory = OpenDirectory<T>();
            using var writer = new IndexWriter(directory, new IndexWriterConfig(_luceneVersion,_options.Analyzer));
            foreach (var entity in datas)
            {
                if (entity==null)
                {
                    continue;
                }
                writer.DeleteDocuments(new Term("Id", entity.ToString()));
            }
            
            writer.Flush(true, true);
            writer.Commit();
            return true;
        }
        

        /// <summary>
        /// 索引库数量
        /// </summary>
        /// <returns></returns>
        public int Count<T>()
        {
            var directory = OpenDirectory<T>();
            IndexReader reader = DirectoryReader.Open(directory);
            return reader.NumDocs;
        }



        public static Document ToDocument<T>(T data)
        {
            var doc = new Document();
            var type = typeof(T);
            
            doc.Add(new StringField("Type", type.AssemblyQualifiedName, Field.Store.YES));

            var classProperties = type.GetProperties();

            foreach (PropertyInfo propertyInfo in classProperties)
            {
                var attr = propertyInfo.GetCustomAttributes<LuceneIndexAttribute>().FirstOrDefault() as LuceneIndexAttribute;
                if (attr==null)
                {
                    continue;
                }
                var value = propertyInfo.GetValue(data);
                if (value == null)
                {
                    continue;
                }
                string name = !string.IsNullOrEmpty(attr.Name) ? attr.Name : propertyInfo.Name;
                doc.Add(GetField(value,name, attr));
            }
            return doc;
        }


        

        private static Field GetField(object value, string name, LuceneIndexAttribute attr)
        {
            switch (value)
            {
                case DateTime time:
                    return new StringField(name, time.ToString("yyyy-MM-dd HH:mm:ss"), attr.Store);
                case int num:
                    return new Int32Field(name, num, attr.Store);
                case long num:
                    return new Int64Field(name, num, attr.Store);
                case float num:
                    return new SingleField(name, num, attr.Store);
                case double num:
                    return new DoubleField(name, num, attr.Store);
                default:
                    var htmlValue = attr.IsHtml ? value.ToString().RemoveHtmlTag() : value.ToString();
                    return new TextField(name, htmlValue, attr.Store);
            }
        }

        
        private FSDirectory OpenDirectory<T>()
        {
            var typeName = typeof(T).Name;
            var indexPath = $"{_options.IndexDir}/LunceneIndex/{typeName}";
            if (!Directory.Exists(indexPath)) Directory.CreateDirectory(indexPath);
            var directory = FSDirectory.Open(new DirectoryInfo(indexPath), new NativeFSLockFactory());
            return directory;
        }
    }
}