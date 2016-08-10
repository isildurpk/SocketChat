using System;
using System.Net.Sockets;

namespace Server
{
    internal class ClientObject : IDisposable
    {
        #region Fields

        private readonly TcpClient _tcpClient;

        #endregion

        #region Constructors

        public ClientObject(TcpClient tcpClient)
        {
            _tcpClient = tcpClient;
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            _tcpClient.Close();
        }

        #endregion
    }
}
