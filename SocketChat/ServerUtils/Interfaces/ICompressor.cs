using System.Threading.Tasks;

namespace ServerUtils.Interfaces
{
    public interface ICompressor
    {
        Task<byte[]> CompressAsync(byte[] bytes);

        Task<byte[]> DecompressAsync(byte[] bytes);
    }
}