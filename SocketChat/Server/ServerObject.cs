using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Server.Interfaces;

namespace Server
{
    internal class ServerObject : IServer
    {
        #region Fields

        private readonly IList<ClientObject> _clients = new List<ClientObject>();
        private TcpListener _tcpListener;

        #endregion

        #region Constructors

        public ServerObject(IPEndPoint localIpEndPoint)
        {
            LocalIpEndPoint = localIpEndPoint;
        }

        #endregion
        
        #region Implementation of IServer

        public IPEndPoint LocalIpEndPoint { get; }

        public void Start()
        {
            _tcpListener = new TcpListener(LocalIpEndPoint);
        }

        #endregion

        #region Methods

        #endregion
    }
}
