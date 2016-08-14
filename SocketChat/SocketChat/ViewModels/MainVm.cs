using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Input;
using SocketChat.Insfrastructure;

namespace SocketChat.ViewModels
{
    public class MainVm : NotifyPropertyChangedBase
    {
        #region Fields

        private const ushort LocalPortFrom = 55000;
        private const ushort LocalPortTo = 55999;

        private readonly StringBuilder _outputSb = new StringBuilder();
        private TcpClient _tcpClient;
        private NetworkStream _stream;
        private string _infoMessage;

        #endregion

        #region Constructors

        public MainVm()
        {
            ConnectCommand = new RelayCommand(Connect, CanConnect);
            DisconnectCommand = new RelayCommand(Disconnect, CanDisconnect);
            SendCommand = new RelayCommand(Send, CanSend);
        }

        #endregion

        #region Commands

        public ICommand ConnectCommand { get; private set; }

        [SuppressMessage("ReSharper", "PossibleInvalidOperationException")]
        private async void Connect()
        {
            InfoMessage = string.Empty;

            try
            {
                var random = new Random();
                var port = random.Next(LocalPortFrom, LocalPortTo);
                var localIep = new IPEndPoint(IPAddress.Loopback, port);
                _tcpClient = new TcpClient(localIep);
            }
            catch (SocketException e)
            {
                InfoMessage = e.Message;
                return;
            }

            try
            {
                var serverIp = IPAddress.Parse(ServerIp);
                await _tcpClient.ConnectAsync(serverIp, ServerPort.Value);
                _stream = _tcpClient.GetStream();
            }
            catch (SocketException)
            {
                InfoMessage = $"Can`t connect to the server {ServerIp}:{ServerPort}";
                _tcpClient.Close();
                return;
            }

            var bytes = Encoding.UTF8.GetBytes(Nickname);
            await _stream.WriteAsync(bytes, 0, bytes.Length);

            IsConnected = true;
            OnPropertyChanged(nameof(IsConnected));

            ThreadPool.QueueUserWorkItem(state => ReceiveMessages());
        }

        private bool CanConnect()
        {
            return !IsConnected && !string.IsNullOrEmpty(ServerIp) && ServerPort > 0 &&
                   !string.IsNullOrEmpty(Nickname);
        }

        public ICommand DisconnectCommand { get; private set; }

        private void Disconnect()
        {
            _tcpClient.Close();
            IsConnected = false;
            OnPropertyChanged(nameof(IsConnected));
        }

        private bool CanDisconnect()
        {
            return IsConnected;
        }

        public ICommand SendCommand { get; private set; }

        private async void Send()
        {
            var bytes = Encoding.UTF8.GetBytes(Input);
            await _stream.WriteAsync(bytes, 0, bytes.Length);

            _outputSb.AppendLine($"{DateTime.Now:t} Me: {Input}");
            Output = _outputSb.ToString();
            Input = string.Empty;

            OnPropertyChanged(nameof(Output));
            OnPropertyChanged(nameof(Input));
        }

        private bool CanSend()
        {
            return !string.IsNullOrEmpty(Input) && IsConnected;
        }

        #endregion

        #region Properties

        public bool IsConnected { get; set; }

        public string Input { get; set; }

        public string InfoMessage
        {
            get { return _infoMessage; }
            private set
            {
                _infoMessage = value;
                OnPropertyChanged();
            }
        }

        public string Output { get; private set; }

        public string Nickname { get; set; } = "Isildur";

        public string ServerIp { get; set; } = "127.0.0.1";

        public ushort? ServerPort { get; set; } = 56000;

        #endregion

        #region Methods

        private async void ReceiveMessages()
        {
            try
            {
                var sb = new StringBuilder();
                var bytes = new byte[128];

                while (IsConnected)
                {
                    do
                    {
                        var count = await _stream.ReadAsync(bytes, 0, bytes.Length);
                        sb.Append(Encoding.UTF8.GetString(bytes, 0, count));
                    } while (_stream.DataAvailable);

                    _outputSb.AppendLine($"{DateTime.Now:t} {sb}");
                    sb.Clear();
                    Output = _outputSb.ToString();
                    OnPropertyChanged(nameof(Output));
                }
            }
            catch (ObjectDisposedException) // Отключились
            {
            }
        }

        #endregion
    }
}