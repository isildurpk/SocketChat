using System;
using System.Text;
using System.Windows.Input;
using SocketChat.Insfrastructure;

namespace SocketChat.ViewModels
{
    public class MainVm : NotifyPropertyChangedBase
    {
        private readonly StringBuilder _outputSb = new StringBuilder();

        #region Constructors

        public MainVm()
        {
            SendCommand = new RelayCommand(Send, CanSend);
        }

        #endregion

        #region Commands

        public ICommand SendCommand { get; private set; }

        private void Send()
        {
            _outputSb.AppendFormat("{0:t} Me: {1}{2}", DateTime.Now, Input, Environment.NewLine);
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

        #endregion

        #region Methods

        #endregion
    }
}