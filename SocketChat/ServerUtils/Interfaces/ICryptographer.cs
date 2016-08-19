using System.Security.Cryptography;

namespace ServerUtils.Interfaces
{
    public interface ICryptographer
    {
        RSAParameters PublicKey { get; }

        byte[] Encrypt(byte[] bytes, RSAParameters externalPublicKey);

        byte[] Decrypt(byte[] bytes);
    }
}
