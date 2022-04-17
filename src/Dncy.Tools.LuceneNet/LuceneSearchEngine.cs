using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

using Dncy.Tools.LuceneNet.Models;

using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Cn.Smart;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Index.Extensions;
using Lucene.Net.Queries;
using Lucene.Net.Search;
using Lucene.Net.Search.Highlight;
using Lucene.Net.Store;
using Lucene.Net.Util;

#if NETCOREAPP
using Microsoft.Extensions.Options;
#endif

using Directory = System.IO.Directory;

namespace Dncy.Tools.LuceneNet
{
    public class LuceneSearchEngine
    {

        private readonly LuceneSearchEngineOptions _options;
        private readonly IFieldSerializeProvider _fieldSerializeProvider;
        public const LuceneVersion LuceneVersion = Lucene.Net.Util.LuceneVersion.LUCENE_48;
        public Analyzer Analyzer { get; private set; }

#if NETCOREAPP
        public LuceneSearchEngine(IOptions<LuceneSearchEngineOptions> options, IFieldSerializeProvider fieldSerializeProvider)
        {
            _options = options.Value ?? throw new ArgumentNullException(nameof(options));
            _fieldSerializeProvider = fieldSerializeProvider ?? throw new ArgumentNullException(nameof(fieldSerializeProvider));

            _ = _options.IndexDir ?? throw new ArgumentNullException(nameof(LuceneSearchEngineOptions.IndexDir));
            _ = _options.Analyzer ?? throw new ArgumentNullException(nameof(LuceneSearchEngineOptions.Analyzer));

            Analyzer = _options.Analyzer;

            IndexDirCache.Add(_options.IndexDir);
            
        }
#else
        public LuceneSearchEngine(LuceneSearchEngineOptions options, IFieldSerializeProvider serializeProvider)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _ = _options.IndexDir ?? throw new ArgumentNullException(nameof(options.IndexDir));
            _ = _options.Analyzer ?? throw new ArgumentNullException(nameof(options.Analyzer));
            Analyzer = _options.Analyzer;
            _fieldSerializeProvider = serializeProvider;
            IndexDirCache.Add(_options.IndexDir);
        }
#endif


        static readonly ConcurrentDictionary<Type, Dictionary<PropertyInfo, LuceneIndexedAttribute>> TypeFieldCache = new();
        static readonly ConcurrentBag<string> IndexDirCache = new();

        /// <summary>
        /// 创建索引
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="datas"></param>
        /// <param name="recreate"></param>
        /// <returns></returns>
        public bool CreateIndex<T>(IEnumerable<T> datas,bool recreate=false)
        {
            var directory = OpenDirectory();
            if (IndexWriter.IsLocked(directory))
            {
                //  若是索引目录被锁定（好比索引过程当中程序异常退出），则首先解锁
                //  Lucene.Net在写索引库以前会自动加锁，在close的时候会自动解锁
                IndexWriter.Unlock(directory);
            }

            //Lucene的index模块主要负责索引的建立
            using var writer = new IndexWriter(directory,new IndexWriterConfig(LuceneVersion,_options.Analyzer));
            // 删除重建
            if (recreate)
            {
                writer.DeleteAll();
                writer.Commit();
            }
            // 遍历实体集，添加到索引库
            foreach (var entity in datas)
            {
                var doc = ToDocument(entity);
                writer.AddDocument(doc);
            }
            writer.Flush(true, true);
            return true;
        }
        

        /// <summary>
        /// 搜索
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="search">查询对象</param>
        /// <returns></returns>
        public SearchResultCollection<ScoredSearchResult<T>> Search<T>(SearchModel search) where T: class, new()
        {
            _ = search.MaxHits <= 0 ? throw new ArgumentOutOfRangeException(nameof(search.MaxHits)) : search.MaxHits;
            var directory = OpenDirectory();
            using var reader = DirectoryReader.Open(directory);
            var searcher = new SearcherFactory().NewSearcher(reader);
            var sort = new Sort(search.OrderBy.ToArray());
            Expression<Func<ScoreDoc, bool>> where = m => m.Score >= search.Score;
            Filter filter = null;
            if (search.OnlyTyped)
            {
                filter = new BooleanFilter { { new QueryWrapperFilter(new TermQuery(new Term("Type", typeof(T).AssemblyQualifiedName))), Occur.MUST } };
            }
            var matches = searcher.Search(search.Query, filter, search.MaxHits, sort, true, true);
            var resultSet = new SearchResultCollection<ScoredSearchResult<T>>();
            if (matches.TotalHits <= 0)
            {
                return resultSet;
            }

            resultSet.TotalHits = matches.TotalHits;

            var hits = matches.ScoreDocs.Where(where.Compile());
            if (search.Skip.HasValue)
            {
                hits = hits.Skip(search.Skip.Value);
            }
            if (search.Take.HasValue)
            {
                hits = hits.Take(search.Take.Value);
            }
            var docs = hits.ToList();
            var scorer = new QueryScorer(search.Query);
            var highlighter = new Highlighter(new SimpleHTMLFormatter(search.HighlightTag.preTag, search.HighlightTag.postTag), scorer)
            {
                TextFragmenter = new SimpleFragmenter()
            };
            foreach (var match in docs)
            {
                var doc = searcher.Doc(match.Doc);
                var entity = GetConcreteFromDocument<T>(doc, highlighter);
                entity.DocId = match.Doc;
                entity.Score = match.Score;
                resultSet.Results.Add(entity);
            }
            return resultSet;
        }


        ///// <summary>
        ///// 根据索引文档id删除文档
        ///// </summary>
        ///// <param name="docId">文档id</param>
        ///// <returns></returns>
        //public bool DeleteDocumentByDocId(int docId)
        //{
        //    var directory = OpenDirectory();
        //    if (IndexWriter.IsLocked(directory))
        //    {
        //        //  若是索引目录被锁定（好比索引过程当中程序异常退出），则首先解锁
        //        //  Lucene.Net在写索引库以前会自动加锁，在close的时候会自动解锁
        //        IndexWriter.Unlock(directory);
        //    }
        //    using var writer = new IndexWriter(directory, new IndexWriterConfig(LuceneVersion, _options.Analyzer));
        //    writer.DeleteDocuments(new Term("IndexId", docId.ToString()));
        //    writer.Flush(true, true);
        //    writer.Commit();
        //    return true;
        //}

        ///// <summary>
        ///// 根据文档id集合删除文档
        ///// </summary>
        ///// <param name="docIds">文档id集合</param>
        ///// <returns></returns>
        //public bool DeleteDocumentsByDocId(IEnumerable<int> docIds)
        //{
        //    var directory = OpenDirectory();
        //    if (IndexWriter.IsLocked(directory))
        //    {
        //        //  若是索引目录被锁定（好比索引过程当中程序异常退出），则首先解锁
        //        //  Lucene.Net在写索引库以前会自动加锁，在close的时候会自动解锁
        //        IndexWriter.Unlock(directory);
        //    }
        //    using var writer = new IndexWriter(directory, new IndexWriterConfig(LuceneVersion, _options.Analyzer));
        //    foreach (var entity in docIds)
        //    {
        //        var tempId = entity.ToString();
        //        if (string.IsNullOrEmpty(tempId))
        //        {
        //            continue;
        //        }
        //        writer.DeleteDocuments(new Term("IndexId", tempId));
        //    }
        //    writer.Flush(true, true);
        //    writer.Commit();
        //    return true;
        //}


        /// <summary>
        /// 根据数据id删除文档
        /// </summary>
        /// <param name="idFieldName">数据id字段名称</param>
        /// <param name="dataId">数据id值</param>
        /// <returns></returns>
        public bool DeleteDocumentByDataId(string idFieldName,string dataId)
        {
            var directory = OpenDirectory();
            if (IndexWriter.IsLocked(directory))
            {
                //  若是索引目录被锁定（好比索引过程当中程序异常退出），则首先解锁
                //  Lucene.Net在写索引库以前会自动加锁，在close的时候会自动解锁
                IndexWriter.Unlock(directory);
            }
            using var writer = new IndexWriter(directory, new IndexWriterConfig(LuceneVersion, _options.Analyzer));
            var query = new QueryBuilder(_options.Analyzer).CreateBooleanQuery(idFieldName, dataId,Occur.MUST);
            writer.DeleteDocuments(query);
            writer.Flush(true, true);
            writer.Commit();
            return true;
        }


        /// <summary>
        /// 根据文档id集合删除文档
        /// </summary>
        /// <param name="idFieldName">数据id字段名称</param>
        /// <param name="dataIds">数据id集合</param>
        /// <returns></returns>
        public bool DeleteDocumentsByDataId(string idFieldName, IEnumerable<string> dataIds)
        {
            var directory = OpenDirectory();
            if (IndexWriter.IsLocked(directory))
            {
                //  若是索引目录被锁定（好比索引过程当中程序异常退出），则首先解锁
                //  Lucene.Net在写索引库以前会自动加锁，在close的时候会自动解锁
                IndexWriter.Unlock(directory);
            }            
            using var writer = new IndexWriter(directory, new IndexWriterConfig(LuceneVersion,_options.Analyzer));
            foreach (var entity in dataIds)
            {
                if (string.IsNullOrEmpty(entity))
                {
                    continue;
                }
                writer.DeleteDocuments(new Term(idFieldName, entity));
            }
            writer.Flush(true, true);
            writer.Commit();
            var dd = writer.HasDeletions();
            return true;
        }
        
        /// <summary>
        /// 当前索引信息
        /// </summary>
        /// <returns></returns>
        public IndexInfo CurrentIndexInfo()
        {
            var directory = OpenDirectory();
            if (directory==null)
            {
                throw new DirectoryNotFoundException($"{_options.IndexDir} is not found");
            }

            var size = (from strFile in directory.ListAll() select directory.FileLength(strFile)).Sum();
            using IndexReader reader = DirectoryReader.Open(directory);
            var di = new System.IO.DriveInfo(_options.IndexDir);
            var model = new IndexInfo
            {
                Dir = _options.IndexDir,
                DocumentCount = reader.NumDocs,
                LastAccessTimeUtc = directory.Directory.LastAccessTimeUtc,
                CreateTimeUtc = directory.Directory.CreationTimeUtc,
                LastWriteTimeUtc = directory.Directory.LastWriteTimeUtc,
                DriveTotalFreeSpace = di.TotalFreeSpace,
                DriveTotalSize = di.TotalSize,
                DriveAvailableFreeSpace = di.AvailableFreeSpace,
                IndexSize = size
            };
            return model;
        }


        /// <summary>
        /// 获取索引信息
        /// </summary>
        /// <returns></returns>
        public List<IndexInfo> IndexInfos()
        {
            var res = new List<IndexInfo>();
            foreach (var dir in IndexDirCache)
            {
               
                var di = new System.IO.DriveInfo(_options.IndexDir);
                var model = new IndexInfo
                {
                    Dir = dir,
                    DriveTotalFreeSpace = di.TotalFreeSpace,
                    DriveTotalSize = di.TotalSize,
                    DriveAvailableFreeSpace = di.AvailableFreeSpace,
                    
                };
                var directory = OpenDirectory(dir);
                if (directory==null)
                {
                    continue;
                }
                var size = ( from strFile in directory.ListAll() select directory.FileLength(strFile) ).Sum();
                model.IndexSize = size;
                model.LastAccessTimeUtc = directory.Directory.LastAccessTimeUtc;
                model.CreateTimeUtc = directory.Directory.CreationTimeUtc;
                using IndexReader reader = DirectoryReader.Open(directory);
                model.DocumentCount = reader.NumDocs;
                res.Add(model);
            }
            return res;
        }



        /// <summary>
        /// 切换索引目录
        /// </summary>
        /// <returns></returns>
        public IDisposable ChangeIndexDir(string dir)
        {
            if (!Directory.Exists(dir))
            {
                throw new DirectoryNotFoundException($"索引目录不存在：{dir}");
            }
            var parentScope = _options.IndexDir;
            _options.IndexDir = dir;
            IndexDirCache.Add(dir);
            return new DisposeAction(() =>
            {
                _options.IndexDir = parentScope;
            });
        }

        
        public Document ToDocument<T>(T data)
        {
            var doc = new Document();
            var type = typeof(T);
            
            doc.Add(new StringField("Type", type.AssemblyQualifiedName, Field.Store.YES));

            var propertys = TypeFieldCache.GetOrAdd(type, m =>
             {
                 var properties = m.GetProperties();
                 var dic = new Dictionary<PropertyInfo, LuceneIndexedAttribute>();
                 foreach (var property in properties)
                 {
                     var attr = property.GetCustomAttribute<LuceneIndexedAttribute>();
                     if (attr == null)
                     {
                         continue;
                     }
                     dic.Add(property, attr);
                 }
                 return dic;
             });

            if (propertys != null && propertys.Any())
            {
                foreach (var item in propertys)
                {
                    var property = item.Key;
                    var attr = item.Value;
                    var value = property.GetValue(data);
                    if (value == null)
                    {
                        continue;
                    }
                    string name = !string.IsNullOrEmpty(attr.Name) ? attr.Name : property.Name;
                    var field = GetField(value, name, attr);
                    doc.Add(field);
                }

                return doc;
            }

            var classProperties = type.GetProperties();
            foreach (PropertyInfo propertyInfo in classProperties)
            {
                if (propertyInfo.GetCustomAttributes<LuceneIndexedAttribute>().FirstOrDefault() is not LuceneIndexedAttribute attr)
                {
                    continue;
                }
                TypeFieldCache.AddOrUpdate(type, new Dictionary<PropertyInfo, LuceneIndexedAttribute> {{propertyInfo, attr } }, (_, _) => new Dictionary<PropertyInfo, LuceneIndexedAttribute> { { propertyInfo, attr } });
                var value = propertyInfo.GetValue(data);
                if (value == null)
                {
                    continue;
                }
                string name = !string.IsNullOrEmpty(attr.Name) ? attr.Name : propertyInfo.Name;
                var field = GetField(value, name, attr);
                doc.Add(field);
            }
            return doc;
        }

        public static List<string> GetKeyWords(string q)
        {
            var keyworkds = new List<string>();
            var analyzer = new SmartChineseAnalyzer(LuceneVersion.LUCENE_48);
            using var ts = analyzer.GetTokenStream(null, q);
            ts.Reset();
            var ct = ts.GetAttribute<Lucene.Net.Analysis.TokenAttributes.ICharTermAttribute>();
            while (ts.IncrementToken())
            {
                StringBuilder keyword = new StringBuilder();
                for (int i = 0; i < ct.Length; i++)
                {
                    keyword.Append(ct.Buffer[i]);
                }
                string item = keyword.ToString();
                if (!keyworkds.Contains(item))
                {
                    keyworkds.Add(item);
                }
            }

            return keyworkds;
        }



        #region private members

        private Field GetField(object value, string name, LuceneIndexedAttribute attr)
        {
            if (attr.IsIdentityField)
            {
                return new StringField(name, value.ToString(), attr.Store);
            }
            switch (value)
            {
                case DateTime time:
                    return new StringField(name, time.ToString("yyyy-MM-dd HH:mm:ss"), attr.Store);
                case int num:
                    return new Int32Field(name, num, attr.Store);
                case uint num:
                    return new Int32Field(name, (int)num, attr.Store);
                case long num:
                    return new Int64Field(name, num, attr.Store);
                case ulong num:
                    return new Int64Field(name, (long)num, attr.Store);
                case float num:
                    return new SingleField(name, num, attr.Store);
                case short num:
                    return new SingleField(name, num, attr.Store);
                case double num:
                    return new DoubleField(name, num, attr.Store);
                case decimal num:
                    return new DoubleField(name, (double)num, attr.Store);
                case string _:
                    var htmlValue = attr.IsHtml ? value.ToString().RemoveHtmlTag() : value.ToString();
                    if (attr.IsTextField)
                    {
                        return new TextField(name, htmlValue, attr.Store);
                    }
                    else
                    {
                        return new StringField(name, htmlValue, attr.Store);
                    }
                default:
                    var serValue = _fieldSerializeProvider.Serialize(value);
                    return new StringField(name, serValue, attr.Store);
            }
        }

        private FSDirectory OpenDirectory(string dir=null)
        {
            dir ??= _options.IndexDir;
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            var directory = FSDirectory.Open(new DirectoryInfo(dir));
            return directory;
        }

        private ScoredSearchResult<T> GetConcreteFromDocument<T>(Document doc, Highlighter highlighter = null) where T : class, new()
        {
            var t = typeof(T);
            var type = typeof(ScoredSearchResult<>).MakeGenericType(t);
            var obj = (ScoredSearchResult<T>)Activator.CreateInstance(type);
            var dataInstance = Activator.CreateInstance(t);
            var dic = new Dictionary<string, string>();
            if (TypeFieldCache.ContainsKey(t))
            {
                var properties = TypeFieldCache[t].Select(x => x.Key);
                foreach (var p in properties.Where(p => p.GetCustomAttributes<LuceneIndexedAttribute>().Any()))
                {
                    p.SetValue(dataInstance, doc.Get(p.Name, p.PropertyType, _fieldSerializeProvider));
                    var attr = p.GetCustomAttribute<LuceneIndexedAttribute>();
                    if (attr.IsHighLight)
                    {
                        var value = doc.GetField(p.Name).GetStringValue();
                        value = GetHightLightPreviewStr(value, p.Name, attr.HightLightMaxNumber, highlighter);
                        dic.Add(p.Name, value);
                    }
                }
            }
            else
            {
                foreach (var p in t.GetProperties().Where(p => p.GetCustomAttributes<LuceneIndexedAttribute>().Any()))
                {
                    p.SetValue(dataInstance, doc.Get(p.Name, p.PropertyType, _fieldSerializeProvider));
                    var attr = p.GetCustomAttribute<LuceneIndexedAttribute>();
                    if (attr.IsHighLight)
                    {
                        var value = doc.GetField(p.Name).GetStringValue();
                        value = GetHightLightPreviewStr(value, p.Name, attr.HightLightMaxNumber, highlighter);
                        dic.Add(p.Name, value);
                    }
                    TypeFieldCache.AddOrUpdate(t, new Dictionary<PropertyInfo, LuceneIndexedAttribute> { { p, attr } }, (_, _) => new Dictionary<PropertyInfo, LuceneIndexedAttribute> { { p, attr } });
                }
            }

            //var hightProperty = type.GetProperty("HightLightValue");
            //hightProperty?.SetValue(obj, dic);
            //var dataProperty = type.GetProperty("Data");
            //dataProperty?.SetValue(obj, dataInstance);
            obj.Data = (T)dataInstance;
            obj.HightLightValue = dic;
            return obj;
        }

        private string GetHightLightPreviewStr(string value, string field, int maxNumFragments, Highlighter highlighter)
        {
            using var stream = Analyzer.GetTokenStream(field, new StringReader(value));
            var previews = highlighter.GetBestFragments(stream, value, maxNumFragments);
            var preview = string.Join("\n", previews.Select(html => html.Trim()));
            return preview;
        }
        #endregion

    }
}