using System;

namespace ServerUtils
{
    public static class Cryptographer
    {
        public static byte[] Encrypt(byte[] data, byte[] keyBlob)
        {
            return data;
        }

        public static byte[] Decrypt(byte[] data, byte[] keyBlob)
        {
            return data;
        }

        public static byte[] GenerateKey()
        {
            return Guid.NewGuid().ToByteArray();
        }
    }
}
