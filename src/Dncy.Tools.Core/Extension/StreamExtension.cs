using System.IO;
using System.Threading.Tasks;

namespace Dotnetydd.Tools.Extension
{
    public static class StreamExtension
    {
        public static byte[] ToByteArray(this Stream stream, int bufferSize = 10240)
        {
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream, bufferSize);
                stream.Position = 0;
                return memoryStream.ToArray();
            }
        }

#if !NET40
        public static async Task<byte[]> ToByteArrayAsync(this Stream stream, int bufferSize = 10240)
        {
            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream, bufferSize);
                stream.Position = 0;
                return await memoryStream.ToByteArrayAsync();
            }
        }
#endif

    }
}
