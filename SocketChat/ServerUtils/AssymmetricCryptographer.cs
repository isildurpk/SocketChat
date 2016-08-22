using System;
using System.Security.Cryptography;

namespace ServerUtils
{
    public sealed class AssymmetricCryptographer : IDisposable
    {
        #region Fields

        private readonly RSACryptoServiceProvider _rsa;

        #endregion

        #region Constructors

        public AssymmetricCryptographer()
        {
            _rsa = new RSACryptoServiceProvider();
            PublicKeyBlob = _rsa.ExportCspBlob(false);
        }

        #endregion

        #region Properties
        
        public byte[] PublicKeyBlob { get; }

        #endregion

        #region Methods

        public byte[] Encrypt(byte[] bytes, byte[] externalPublicKey)
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportCspBlob(externalPublicKey);
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

        #region Implementation of IDisposable

        public void Dispose()
        {
            _rsa.Dispose();
        }

        #endregion
    }
}
