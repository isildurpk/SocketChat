using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ServerUtils;

namespace Server
{
    internal class ClientObject : IDisposable
    {
        #region Fields
        
        private readonly ServerObject _server;
        private readonly TcpClient _tcpClient;
        private readonly NetworkStream _stream;
        private string _nickname;
        private readonly byte[] _cryptoKey;

        #endregion

        #region Constructors

        public ClientObject(TcpClient tcpClient, ServerObject server, byte[] cryptoKey)
        {
            _tcpClient = tcpClient;
            _stream = tcpClient.GetStream();
            _server = server;
            _cryptoKey = cryptoKey;
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
            var data = await Compressor.CompressAsync(message.ToBytes());
            await data.Encrypt(_cryptoKey).SendToStream(_stream);
        }

        public async Task StartAsync()
        {
            using (var ac = new AssymmetricCryptographer())
            {
                var clientPublicKeyBlob = await GetMessageBytesAsync();
                await ac.Encrypt(_cryptoKey, clientPublicKeyBlob).SendToStream(_stream);
            }

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
            var messageBytes = await GetMessageBytesAsync();
            if (messageBytes == null || messageBytes.Length == 0)
                return null;
            
            messageBytes = await Compressor.DecompressAsync(messageBytes.Decrypt(_cryptoKey));
            return Encoding.UTF8.GetString(messageBytes, 0, messageBytes.Length);
        }

        private async Task<byte[]> GetMessageBytesAsync()
        {
            var messageBytesList = new List<byte>();

            try
            {
                var buffer = new byte[1024];
                do
                {
                    var count = await _stream.ReadAsync(buffer, 0, buffer.Length);
                    for (int i = 0; i < count; i++)
                        messageBytesList.Add(buffer[i]);
                } while (_stream.DataAvailable);
            }
            catch (IOException)
            {
                return null; // Client's app was stopped
            }

            return messageBytesList.ToArray();
        }

        #endregion
    }
}