using SocketChat.Insfrastructure;

namespace SocketChat.ViewModels
{
    public class TextViewerVm : NotifyPropertyChangedBase
    {
        #region Properties

        public string Text { get; private set; }

        #endregion

        #region Methods

        public void Initialize(string text)
        {
            Text = text;
            OnPropertyChanged(nameof(Text));
        }

        #endregion
    }
}
