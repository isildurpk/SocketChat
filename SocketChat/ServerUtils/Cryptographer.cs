using System.Security.Cryptography;
using ServerUtils.Interfaces;

namespace ServerUtils
{
    public sealed class Cryptographer : ICryptographer
    {
        private readonly RSACryptoServiceProvider _rsa;

        #region Constructors

        public Cryptographer()
        {
            _rsa = new RSACryptoServiceProvider();
            PublicKey = _rsa.ExportParameters(false);
        }

        #endregion

        #region Properties

        public RSAParameters PublicKey { get; }

        #endregion

        #region Methods

        public byte[] Encrypt(byte[] bytes, RSAParameters externalPublicKey)
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(externalPublicKey);
                return rsa.Encrypt(bytes, false);
            }
        }

        public byte[] Decrypt(byte[] bytes)
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(_rsa.ExportParameters(true));
                return rsa.Decrypt(bytes, false);
            }
        }

        #endregion
    }
}
