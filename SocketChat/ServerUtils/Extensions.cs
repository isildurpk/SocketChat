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

        public static Task Send(this NetworkStream stream, byte[] data)
        {
            return stream.WriteAsync(data, 0, data.Length);
        }
    }
}
