using System.Text;

namespace ServerUtils
{
    public static class Extensions
    {
        public static byte[] ToBytes(this string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }
    }
}
