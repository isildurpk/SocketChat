using System.Security.Cryptography;
using ServerUtils.Interfaces;

namespace ServerUtils
{
    public class Cryptographer : ICryptographer
    {
        #region Fields

        private CngKey _privateKey;

        #endregion

        #region Constructors

        public Cryptographer()
        {
        }

        #endregion

        #region Properties

        public byte[] PublicKey { get; }

        #endregion

        #region Methods

        #endregion
    }
}
