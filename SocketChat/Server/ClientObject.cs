using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ServerUtils;
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
        private byte[] _externalPublicKeyBlob;

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
            var data = await _compressor.CompressAsync(message.ToBytes());
            data = _server.Cryptographer.Encrypt(data, _externalPublicKeyBlob);
            await _stream.Send(data);
        }

        public async Task StartAsync()
        {
            _externalPublicKeyBlob = await GetMessageBytesAsync();
            await _stream.Send(_server.Cryptographer.PublicKeyBlob);

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

            messageBytes = _server.Cryptographer.Decrypt(messageBytes);
            messageBytes = await _compressor.DecompressAsync(messageBytes);
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
                // Клиент принудительно закрыли
                return null;
            }

            return messageBytesList.ToArray();
        }

        #endregion
    }
}