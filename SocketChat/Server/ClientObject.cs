using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ServerUtils.Interfaces;

namespace Server
{
    internal class ClientObject : IDisposable
    {
        #region Fields

        private readonly ICompressor _compressor;

        private readonly ServerObject _server;
        private readonly TcpClient _tcpClient;
        private readonly NetworkStream _stream;
        private string _nickname;

        #endregion

        #region Constructors

        public ClientObject(TcpClient tcpClient, ServerObject server, ICompressor compressor)
        {
            _tcpClient = tcpClient;
            _stream = tcpClient.GetStream();
            _server = server;

            _compressor = compressor;
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            _stream.Close();
            _tcpClient.Close();
        }

        #endregion

        #region Methods

        public async void Send(string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            await _stream.WriteAsync(bytes, 0, bytes.Length);
        }

        public async Task StartAsync()
        {
            _nickname = await GetMessageAsync();
            _server.BroadcastMessage($"{_nickname} connected to the chat", this);
            
            while (true)
            {
                var message = await GetMessageAsync();
                if (string.IsNullOrEmpty(message))
                {
                    _server.BroadcastMessage($"{_nickname} leaves the chat", this);
                    break;
                }

                _server.BroadcastMessage($"{_nickname}: {message}", this);
            }
        }

        private async Task<string> GetMessageAsync()
        {
            var messageBytes = new List<byte>();

            try
            {
                var bytes = new byte[1024];
                do
                {
                    var count = await _stream.ReadAsync(bytes, 0, bytes.Length);
                    for (int i = 0; i < count; i++)
                        messageBytes.Add(bytes[i]);
                } while (_stream.DataAvailable);
            }
            catch (IOException)
            {
                // Клиент принудительно закрыли
                return null;
            }

            var decompressedBytes = await _compressor.DecompressAsync(messageBytes.ToArray());
            return Encoding.UTF8.GetString(decompressedBytes, 0, decompressedBytes.Length);
        }

        #endregion
    }
}
