using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    internal class ServerObject
    {
        #region Fields

        private readonly IList<ClientObject> _clients = new List<ClientObject>();
        private TcpListener _tcpListener;

        #endregion

        #region Properties

        public bool IsListenning { get; private set; }

        public ushort Port { get; private set; }

        #endregion

        #region Methods

        public void Start(ushort port)
        {
            Port = port;

            _tcpListener = new TcpListener(new IPEndPoint(IPAddress.Any, Port));
            _tcpListener.Start();
            Console.WriteLine($"Server has been started on port {Port}");

            ThreadPool.QueueUserWorkItem(state => Listen());
        }

        public void Stop()
        {
            Console.WriteLine("Stopping the server");
            _tcpListener.Stop();
            IsListenning = false;

            foreach (var client in _clients)
            {
                client.Dispose();
            }
        }

        public void BroadcastMessage(string message, ClientObject exceptClient = null)
        {
            Console.WriteLine($"{DateTime.Now:t} {message}");
            foreach (var client in _clients.Where(x => x != exceptClient))
            {
                client.Send(message);
            }
        }

        private async void Listen()
        {
            Console.WriteLine("Waiting for connections...");
            IsListenning = true;

            while (IsListenning)
            {
                try
                {
                    var tcpClient = await _tcpListener.AcceptTcpClientAsync();
                    var client = new ClientObject(tcpClient, this);
                    _clients.Add(client);
                    ThreadPool.QueueUserWorkItem(async state =>
                    {
                        await client.StartAsync();
                        _clients.Remove(client);
                        client.Dispose();
                    });
                }
                catch (ObjectDisposedException)
                {
                }
            }
        }

        #endregion
    }
}