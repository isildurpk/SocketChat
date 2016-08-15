using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Xml;
using ServerUtils.Interfaces;

namespace ServerUtils
{
    public class Compressor : ICompressor
    {
        #region Implementation of ICompressor

        public Task<byte[]> CompressAsync(byte[] bytes)
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

        public Task<byte[]> DecompressAsync(byte[] bytes)
        {
            return Task.Run(() =>
            {
                using (var input = new MemoryStream(bytes))
                using (var output = new MemoryStream())
                {
                    using (var gs = new GZipStream(input, CompressionMode.Decompress))
                        input.CopyTo(gs);
                    return output.ToArray();
                }
            });
        }

        #endregion
    }
}