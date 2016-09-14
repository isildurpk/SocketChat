using System.Windows;
using SocketChat.ViewModels;

namespace SocketChat.Views
{
    public partial class TextViewerWindow : Window
    {
        public TextViewerWindow()
        {
            InitializeComponent();
            DataContext = new TextViewerVm();
        }
    }
}
