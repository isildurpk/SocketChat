using System;
using System.IO;
using System.Security.Cryptography;
using SecurityDriven.Inferno;

namespace ServerUtils
{
    public static class Cryptographer
    {
        private static readonly CryptoRandom Random = new CryptoRandom();

        public static byte[] Encrypt(byte[] data, byte[] keyBlob)
        {
            using (var input = new MemoryStream(data))
            using (var output = new MemoryStream())
            using (var transform = new EtM_EncryptTransform(keyBlob))
            {
                using (var cryptoStream = new CryptoStream(output, transform, CryptoStreamMode.Write))
                    input.CopyTo(cryptoStream);
                return output.ToArray();
            }
        }

        public static byte[] Decrypt(byte[] data, byte[] keyBlob)
        {
            using (var input = new MemoryStream(data))
            using (var output = new MemoryStream())
            using (var transform = new EtM_DecryptTransform(keyBlob))
            {
                using (var cryptoStream = new CryptoStream(input, transform, CryptoStreamMode.Read))
                    cryptoStream.CopyTo(output);

                if (!transform.IsComplete)
                    throw new Exception("Не все блоки расшифрованы!");

                return output.ToArray();
            }
        }

        public static byte[] GenerateKey()
        {
            return Random.NextBytes(32);
        }
    }
}
