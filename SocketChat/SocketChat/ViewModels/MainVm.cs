using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
            var random = new Random();
            var localIep = new IPEndPoint(IPAddress.Loopback, random.Next(LocalPortFrom, LocalPortTo));
            _tcpClient = new TcpClient(localIep);

            var ip = IPAddress.Parse(ServerIp);
            await _tcpClient.ConnectAsync(ip, ServerPort.Value);

            IsConnected = true;
        }

        private bool CanConnect()
        {
            return !IsConnected && !string.IsNullOrEmpty(ServerIp) && ServerPort > 0;
        }

        public ICommand DisconnectCommand { get; private set; }

        private void Disconnect()
        {
            _tcpClient.Close();
            IsConnected = false;
        }

        private bool CanDisconnect()
        {
            return IsConnected;
        }

        public ICommand SendCommand { get; private set; }

        private void Send()
        {
            _outputSb.AppendLine($"{DateTime.Now:t} Me: {Input}");
            Output = _outputSb.ToString();
            Input = string.Empty;

            OnPropertyChanged(nameof(Output));
            OnPropertyChanged(nameof(Input));
        }

        private bool CanSend()
        {
            return !string.IsNullOrEmpty(Input);
        }

        #endregion

        #region Properties

        public bool IsConnected { get; set; }

        public string Input { get; set; }

        public string Output { get; private set; }

        public string ServerIp { get; set; } = "127.0.0.1";

        public ushort? ServerPort { get; set; } = 56000;

        #endregion

        #region Methods

        #endregion
    }
}