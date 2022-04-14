using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Dncy.Tools.Files
{

    /// <summary>
    /// 文件工具类
    /// </summary>
    public static class FilesTools
    {
        /// <summary>
        /// 以文件流的形式复制大文件
        /// </summary>
        /// <param name="fs">源</param>
        /// <param name="dest">目标地址</param>
        /// <param name="bufferSize">缓冲区大小，默认8MB</param>
        public static void CopyToFile(this Stream fs, string dest, int bufferSize = 1024 * 8 * 1024)
        {
            using var fsWrite = new FileStream(dest, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            byte[] buf = new byte[bufferSize];
            int len;
            while ((len = fs.Read(buf, 0, buf.Length)) != 0)
            {
                fsWrite.Write(buf, 0, len);
            }
        }

#if NET45_OR_GREATER
        /// <summary>
        /// 以文件流的形式复制大文件
        /// </summary>
        /// <param name="fs">源</param>
        /// <param name="dest">目标地址</param>
        /// <param name="bufferSize">缓冲区大小，默认8MB</param>
        /// <param name="cancellationToken"></param>
        public static async void CopyToFileAsync(this Stream fs, string dest, int bufferSize = 1024 * 1024 * 8, CancellationToken cancellationToken = default)
        {
            using var fsWrite = new FileStream(dest, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            byte[] buf = new byte[bufferSize];
            int len;
            while ((len = await fs.ReadAsync(buf, 0, buf.Length, cancellationToken)) != 0)
            {
                await fsWrite.WriteAsync(buf, 0, len, cancellationToken);
            };
        }
#endif


        /// <summary>
        /// 将内存流转储成文件
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="filename"></param>
        public static void SaveFile(this MemoryStream ms, string filename)
        {
            using var fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
            byte[] buffer = ms.ToArray();
            fs.Write(buffer, 0, buffer.Length);
            fs.Flush();
        }


#if NET45_OR_GREATER
        /// <summary>
        /// 将内存流转储成文件
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="filename"></param>
        /// <param name="cancellationToken"></param>
        public static async void SaveFileAsync(this MemoryStream ms, string filename, CancellationToken cancellationToken = default)
        {
            using var fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
            byte[] buffer = ms.ToArray();
            await fs.WriteAsync(buffer, 0, buffer.Length, cancellationToken);
            await fs.FlushAsync(cancellationToken);
        }
#endif


        /// <summary>
        /// 将文件转储成内存流
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static MemoryStream LoadFile(this string filename)
        {
            using var fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[fs.Length];
            var _ = fs.Read(buffer, 0, buffer.Length);
            return new MemoryStream(buffer);
        }
        


#if NET45_OR_GREATER
        /// <summary>
        /// 将文件转储成内存流
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static MemoryStream LoadFileAsync(this string filePath, CancellationToken cancellationToken = default)
        {
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[fs.Length];
            var _ = fs.ReadAsync(buffer, 0, buffer.Length,cancellationToken);
            return new MemoryStream(buffer);
        }
#endif


        /// <summary>
        /// 计算文件的 MD5 值
        /// </summary>
        /// <param name="fs">源文件流</param>
        /// <returns>MD5 值16进制字符串</returns>
        public static string GetFileMD5(this FileStream fs) => HashFile(fs, "md5");

        /// <summary>
        /// 计算文件的 sha1 值
        /// </summary>
        /// <param name="fs">源文件流</param>
        /// <returns>sha1 值16进制字符串</returns>
        public static string GetFileSha1(this Stream fs) => HashFile(fs, "sha1");


        /// <summary>
        /// 计算文件的哈希值
        /// </summary>
        /// <param name="fs">被操作的源数据流</param>
        /// <param name="algo">加密算法</param>
        /// <returns>哈希值16进制字符串</returns>
        private static string HashFile(Stream fs, string algo)
        {
            HashAlgorithm crypto = algo switch
            {
                "sha1" => new SHA1CryptoServiceProvider(),
                _ => new MD5CryptoServiceProvider(),
            };
            byte[] retVal = crypto.ComputeHash(fs);

            StringBuilder sb = new StringBuilder();
            foreach (var t in retVal)
            {
                sb.Append(t.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}