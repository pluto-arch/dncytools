using Lucene.Net.Analysis;

namespace Dncy.Tools.LuceneNet
{
    public class LuceneSearchEngineOptions
    {
        /// <summary>
        /// 索引文件目录
        /// </summary>
        public string IndexDir { get; set; }

        /// <summary>
        /// 分析器
        /// </summary>
        public Analyzer Analyzer { get; set; }
    }
}



