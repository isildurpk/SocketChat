using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class ClientObject : IDisposable
    {
        #region Fields

        private readonly TcpClient _tcpClient;
        private readonly NetworkStream _stream;

        #endregion

        #region Constructors

        public ClientObject(TcpClient tcpClient)
        {
            _tcpClient = tcpClient;
            _stream = tcpClient.GetStream();
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

        public async void Start()
        {
            var nickname = await GetMessageAsync();
            Console.WriteLine($"{nickname} connected to chat");
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
