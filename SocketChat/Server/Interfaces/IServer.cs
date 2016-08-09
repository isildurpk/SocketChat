using System.Net;

namespace Server.Interfaces
{
    internal interface IServer
    {
        IPEndPoint LocalIpEndPoint { get; }

        void Start();
    }
}
