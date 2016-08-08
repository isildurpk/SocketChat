using System.Windows.Input;
using SocketChat.Insfrastructure;

namespace SocketChat.ViewModels
{
    public class MainVm : NotifyPropertyChangedBase
    {
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
        }

        private bool CanSend()
        {
            return !string.IsNullOrEmpty(Input);
        }

        #endregion

        #region Properties

        public string Input { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}