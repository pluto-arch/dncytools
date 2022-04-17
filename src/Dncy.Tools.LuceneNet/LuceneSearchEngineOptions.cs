using Lucene.Net.Analysis;

namespace Dncy.Tools.LuceneNet
{
    public class LuceneSearchEngineOptions
    {
        /// <summary>
        /// 索引文件目录
        /// 默认会拼接LuceneIndexs
        /// </summary>
        public string IndexDir { get; set; }

        /// <summary>
        /// 分析器
        /// </summary>
        public Analyzer Analyzer { get; set; }
    }
}



