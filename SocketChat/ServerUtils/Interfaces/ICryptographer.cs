namespace ServerUtils.Interfaces
{
    public interface ICryptographer
    {
        byte[] Encrypt(byte[] data);

        byte[] Decrypt(byte[] data);
    }
}
