using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TCPChatroomClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ClientConnection _connection;

        public MainWindow()
        {
            InitializeComponent();
            _connection = new ClientConnection();
        }

        private void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            ServerConnectWindow serverConnectWindow = new ServerConnectWindow();
            serverConnectWindow.Owner = this;
            serverConnectWindow.ShowDialog();

            //Change ConnectBtn to DisconnectBtn
        }
    }
}