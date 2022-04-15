using System;
using Lucene.Net.Documents;

namespace Dncy.Tools.LuceneNet
{
    [AttributeUsage(AttributeTargets.Property)]
    public class LuceneIndexAttribute:Attribute
    {

        public LuceneIndexAttribute(string name)
        {
            Name = name;
            Store = Field.Store.YES;
            IsHtml = false;
        }


        /// <summary>
        /// 索引字段名
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// 是否被存储到索引库
        /// </summary>
        public Field.Store Store { get; set; }

        /// <summary>
        /// 是否是html
        /// </summary>
        public bool IsHtml { get; set; }
    }
}

