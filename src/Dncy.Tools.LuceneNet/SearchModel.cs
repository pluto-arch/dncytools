using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Search;

namespace Dncy.Tools.LuceneNet
{
    public class SearchModel
    {
        /// <summary>
        /// 关键词
        /// </summary>
        public string Keywords { get; set; }

        /// <summary>
        /// 限定搜索字段
        /// </summary>
        public List<string> Fields { get; set; }

        /// <summary>
        /// 最大检索量
        /// </summary>
        public int MaximumNumberOfHits { get; set; }


        /// <summary>
        /// 多字段搜索时，给字段设定搜索权重
        /// </summary>
        private readonly Dictionary<string, float> _boosts;

        /// <summary>
        /// 多字段搜索时，给字段设定搜索权重
        /// </summary>
        internal Dictionary<string, float> Boosts
        {
            get
            {
                foreach (var field in Fields.Where(field => _boosts.All(x => x.Key.ToUpper() != field.ToUpper())))
                {
                    _boosts.Add(field, 2.0f);
                }

                return _boosts;
            }
        }


        /// <summary>
        /// 排序字段
        /// </summary>
        public List<SortField> OrderBy { get; set; }

        /// <summary>
        /// 跳过多少条
        /// </summary>
        public int? Skip { get; set; }

        /// <summary>
        /// 取多少条
        /// </summary>
        public int? Take { get; set; }


        /// <summary>
        /// 匹配度，0-1，数值越大结果越精确
        /// </summary>
        public float Score { get; set; } = 0.5f;

        /// <summary>
        /// 过滤条件
        /// </summary>
        public Filter Filter { get; set; }
    }
}

