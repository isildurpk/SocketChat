using System.Windows.Input;

namespace SocketChat.ViewModels
{
    public class MainVm
    {
        #region Constructors

        public MainVm()
        {
            
        }

        #endregion

        #region Properties

        public ICommand SendCommand { get; private set; }

        #endregion
    }
}
