using Dncy.Tools.LuceneNet;
using Lucene.Net.Documents;

namespace SearchEngineAspnetMvcTest.Models
{
    public class Person
    {
        [LuceneIndexed("Id", true)]
        public long Id { get; set; }

        [LuceneIndexed("Name", false)]
        public string Name { get; set; }

        [LuceneIndexed("Remarks", false, IsTextField = true, IsHighLight = true, HightLightMaxNumber = 2)]
        public string Remarks { get; set; }
    }
}

