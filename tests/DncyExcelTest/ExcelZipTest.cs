using System.IO.Compression;
using Dncy.Tools.Excel.Zip;

namespace DncyExcelTest
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetExcelEntries_Test()
        {
            var file = Path.Combine(Environment.CurrentDirectory, "docs", "demo.xlsx");
            var fs=File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var archive = new OpenOfficeExcelXmlZip(fs);
            Assert.IsTrue(archive.entries.Count>0);
            foreach (var item in archive.entries)
            {
                Console.WriteLine(item.FullName);
            }
        }

    }
}