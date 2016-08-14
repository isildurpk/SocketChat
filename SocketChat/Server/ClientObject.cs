using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class ClientObject : IDisposable
    {
        #region Fields

        private readonly ServerObject _server;
        private readonly TcpClient _tcpClient;
        private readonly NetworkStream _stream;

        #endregion

        #region Constructors

        public ClientObject(TcpClient tcpClient, ServerObject server)
        {
            _tcpClient = tcpClient;
            _stream = tcpClient.GetStream();
            _server = server;
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

        public async void Start()
        {
            var nickname = await GetMessageAsync();
            _server.BroadcastMessage($"{nickname} connected to chat", this);

            while (true)
            {
                var message = await GetMessageAsync();
                _server.BroadcastMessage(message, this);
            }
        }

        private async Task<string> GetMessageAsync()
        {
            var sb = new StringBuilder();

            var bytes = new byte[128];
            do
            {
                var count = await _stream.ReadAsync(bytes, 0, bytes.Length);
                sb.Append(Encoding.UTF8.GetString(bytes, 0, count));
            } while (_stream.DataAvailable);

            return sb.ToString();
        }

        #endregion
    }
}
