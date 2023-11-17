using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml;

namespace Dncy.Tools.Excel.Zip
{
    public class OpenOfficeExcelXmlZip : IDisposable
    {
        private readonly Dictionary<string, ZipArchiveEntry> _entries;
        internal DncyExcelZipArchive zipFile;
        public ReadOnlyCollection<ZipArchiveEntry> entries => _entries.Select(x=>x.Value).ToList().AsReadOnly();
        private bool _disposed;
        private static readonly XmlReaderSettings XmlSettings = new XmlReaderSettings
        {
            IgnoreComments = true,
            IgnoreWhitespace = true,
            XmlResolver = null,
        };

        public OpenOfficeExcelXmlZip(Stream fileStream, ZipArchiveMode mode = ZipArchiveMode.Read, bool leaveOpen = false, Encoding entryNameEncoding = null)
        {
            zipFile = new DncyExcelZipArchive(fileStream, mode, leaveOpen, entryNameEncoding);
            _entries = new Dictionary<string, ZipArchiveEntry>(StringComparer.OrdinalIgnoreCase);
            foreach (var entry in zipFile.Entries)
            {
                _entries.Add(entry.FullName.Replace('\\', '/'), entry);
            }
        }

        public ZipArchiveEntry GetEntry(string path)
        {
            if (_entries.TryGetValue(path, out var entry))
                return entry;
            return null;
        }

        public XmlReader GetXmlReader(string path)
        {
            var entry = GetEntry(path);
            if (entry != null)
                return XmlReader.Create(entry.Open(), XmlSettings);
            return null;
        }





        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                    zipFile?.Dispose();
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                zipFile = null;
                _disposed = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~OpenOfficeExcelXmlZip()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

