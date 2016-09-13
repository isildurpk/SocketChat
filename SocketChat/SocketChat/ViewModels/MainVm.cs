using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using ServerUtils;
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
        private byte[] _cryptoKey;
        private bool _isConnecting;

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
            IsConnecting = true;
            InfoMessage = string.Empty;

            try
            {
                var random = new Random();
                var port = random.Next(LocalPortFrom, LocalPortTo);
                var localIep = new IPEndPoint(IPAddress.Any, port);
                _tcpClient = new TcpClient(localIep);
            }
            catch (SocketException e)
            {
                InfoMessage = e.Message;
                IsConnecting = false;
                return;
            }

            var tcpClient = _tcpClient;
            try
            {
                var serverIp = IPAddress.Parse(ServerIp);
                await tcpClient.ConnectAsync(serverIp, ServerPort.Value);
                _stream = _tcpClient.GetStream();
            }
            catch (SocketException)
            {
                InfoMessage = $"Can`t connect to the server {ServerIp}:{ServerPort}";
                tcpClient.Close();
                IsConnecting = false;
                return;
            }

            try
            {
                using (var ac = new AssymmetricCryptographer())
                {
                    await ac.PublicKeyBlob.SendToStream(_stream);
                    _cryptoKey = ac.Decrypt(await GetMessageBytesAsync());
                }
                
                await SendMesage(Nickname);
            }
            catch (ObjectDisposedException)
            {
                IsConnecting = false;
                return;
            }

            IsConnecting = false;
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
            await SendMesage(Input);

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

        public bool IsConnected { get; private set; }

        public bool IsConnecting
        {
            get { return _isConnecting; }
            private set
            {
                if (_isConnecting == value)
                    return;

                _isConnecting = value;
                OnPropertyChanged();
            }
        }

        public string Input { get; set; }

        public string InfoMessage
        {
            get { return _infoMessage; }
            private set
            {
                if (_infoMessage == value)
                    return;
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
                while (IsConnected)
                {
                    var message = await GetMessageBytesAsync();
                    message = await message.Decrypt(_cryptoKey).DecompressAsync();

                    _outputSb.AppendLine($"{DateTime.Now:t} {Encoding.UTF8.GetString(message)}");
                    Output = _outputSb.ToString();
                    OnPropertyChanged(nameof(Output));
                }
            }
            catch (ObjectDisposedException)
            {
            }
        }

        private async Task<byte[]> GetMessageBytesAsync()
        {
            var messageContainer = new List<byte>();
            var buffer = new byte[1024];
            do
            {
                var count = await _stream.ReadAsync(buffer, 0, buffer.Length);
                for (int i = 0; i < count; i++)
                    messageContainer.Add(buffer[i]);
            } while (_stream.DataAvailable);

            return messageContainer.ToArray();
        }

        private async Task SendMesage(string message)
        {
            var compressedBytes = await message.ToBytes().CompressAsync();
            await compressedBytes.Encrypt(_cryptoKey).SendToStream(_stream);
        }

        #endregion
    }
}