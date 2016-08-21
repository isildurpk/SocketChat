namespace ServerUtils.Interfaces
{
    public interface ICryptographer
    {
        byte[] PublicKeyBlob { get; }

        byte[] Encrypt(byte[] bytes, byte[] externalPublicKey);

        byte[] Decrypt(byte[] bytes);
    }
}
