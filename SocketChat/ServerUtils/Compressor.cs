using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace ServerUtils
{
    public static class Compressor
    {
        public static Task<byte[]> CompressAsync(byte[] bytes)
        {
            return Task.Run(() =>
            {
                using (var input = new MemoryStream(bytes))
                using (var output = new MemoryStream())
                {
                    using (var gs = new GZipStream(output, CompressionMode.Compress))
                        input.CopyTo(gs);
                    return output.ToArray();
                }
            });
        }

        public static Task<byte[]> DecompressAsync(byte[] bytes)
        {
            return Task.Run(() =>
            {
                using (var input = new MemoryStream(bytes))
                using (var output = new MemoryStream())
                {
                    using (var gs = new GZipStream(input, CompressionMode.Decompress))
                        gs.CopyTo(output);
                    return output.ToArray();
                }
            });
        }
    }
}