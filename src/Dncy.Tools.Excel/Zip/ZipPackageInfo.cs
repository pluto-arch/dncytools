using System.IO.Compression;

namespace Dncy.Tools.Excel.Zip
{
    public class ZipPackageInfo
    {
        public ZipArchiveEntry ZipArchiveEntry { get; set; }
        public string ContentType { get; set; }
        public ZipPackageInfo(ZipArchiveEntry zipArchiveEntry, string contentType)
        {
            this.ZipArchiveEntry = zipArchiveEntry;
            ContentType = contentType;
        }
    }
}

