using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerUtils
{
    public static class Extensions
    {
        public static byte[] ToBytes(this string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        public static byte[] Encrypt(this byte[] data, byte[] key)
        {
            return Cryptographer.Encrypt(data, key);
        }

        public static byte[] Decrypt(this byte[] data, byte[] key)
        {
            return Cryptographer.Decrypt(data, key);
        }

        public static Task SendToStream(this byte[] data, NetworkStream stream)
        {
            return stream.WriteAsync(data, 0, data.Length);
        }

        public static Task<byte[]> CompressAsync(this byte[] data)
        {
            return Compressor.CompressAsync(data);
        }

        public static Task<byte[]> DecompressAsync(this byte[] data)
        {
            return Compressor.DecompressAsync(data);
        }
    }
}
