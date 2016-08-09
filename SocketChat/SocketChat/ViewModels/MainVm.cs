using System;
using System.Text;
using System.Windows.Input;
using SocketChat.Insfrastructure;

namespace SocketChat.ViewModels
{
    public class MainVm : NotifyPropertyChangedBase
    {
        #region Fields

        private readonly StringBuilder _outputSb = new StringBuilder();

        #endregion

        #region Constructors

        public MainVm()
        {
            ConnectCommand = new RelayCommand(Connect, CanConnect);
            SendCommand = new RelayCommand(Send, CanSend);
        }

        #endregion

        #region Commands

        public ICommand ConnectCommand { get; private set; }

        private void Connect()
        {
            
        }

        private bool CanConnect()
        {
            return !string.IsNullOrEmpty(ServerIp) && ServerPort > 0;
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

        public string Input { get; set; }

        public string Output { get; private set; }

        public string ServerIp { get; set; }

        public ushort? ServerPort { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}