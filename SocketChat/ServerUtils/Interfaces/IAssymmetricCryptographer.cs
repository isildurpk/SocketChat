namespace ServerUtils.Interfaces
{
    public interface IAssymmetricCryptographer
    {
        byte[] PublicKeyBlob { get; }

        byte[] Encrypt(byte[] bytes, byte[] externalPublicKey);

        byte[] Decrypt(byte[] bytes);
    }
}
