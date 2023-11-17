using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Dncy.Tools.Excel.Zip
{
    public class DncyExcelZipArchive: ZipArchive
    {
        public DncyExcelZipArchive(Stream stream, ZipArchiveMode mode, bool leaveOpen, Encoding entryNameEncoding)
            : base(stream, mode, leaveOpen, entryNameEncoding)
        {
        }

        public new void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

