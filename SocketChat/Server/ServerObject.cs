﻿using System;
using System.Collections.Generic;
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

        #region Implementation of IServer

        public bool IsListenning { get; private set; }

        public ushort Port { get; private set; }

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

        #endregion

        #region Methods

        private async void Listen()
        {
            Console.WriteLine("Waiting for connections...");
            IsListenning = true;

            while (IsListenning)
            {
                try
                {
                    var client = await _tcpListener.AcceptTcpClientAsync();
                    _clients.Add(new ClientObject(client));
                    Console.WriteLine("New client connected");
                }
                catch (ObjectDisposedException)
                {
                }
            }
        }

        #endregion
    }
}