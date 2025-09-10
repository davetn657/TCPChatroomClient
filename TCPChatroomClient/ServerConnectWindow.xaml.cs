using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TCPChatroomClient
{
    /// <summary>
    /// Interaction logic for ServerConnectWindow.xaml
    /// </summary>
    public partial class ServerConnectWindow : Window
    {
        private MessageBoxButton _button = MessageBoxButton.OK;
        private MessageBoxImage _icon = MessageBoxImage.Warning;
        private MessageBoxResult _result;
        private string _messageBoxText = string.Empty;
        private string _captionText = string.Empty;

        public ServerConnectWindow()
        {
            InitializeComponent();
        }

        private void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            string host = IpText.Text;
            int port;

            try
            {
                Int32.TryParse(PortText.Text, out port);
            }
            catch (FormatException ex)
            {
                throw new FormatException("Could not convert port to int");
            }


            if (CheckValidIP(host) && CheckValidPort(port))
            {
                MainWindow mainWindow = this.Owner as MainWindow; 
                mainWindow._connection.SetHost(host);
                mainWindow._connection.SetPort(port);
                mainWindow._connection.StartConnection();
                this.Close();
            }
            
        }

        private bool CheckValidIP(string host)
        {
            if(IPAddress.TryParse(host, out IPAddress ip))
            {
                return true;
            }

            string messageBoxText = "Host is not valid! Try this format (127.0.0.1)";
            string captionText = "Invalid Host";

            _result = MessageBox.Show(messageBoxText, captionText, _button, _icon, MessageBoxResult.Yes);
            return false;
        }

        private bool CheckValidPort(int port)
        {
            if(port > 40000 &&  port < 45000)
            {
                return true;
            }

            string messageBoxText = "Port is invalid! Try a number between 40000 - 45000";
            string captionText = "Invalid Port";

            _result = MessageBox.Show(messageBoxText, captionText, _button, _icon, MessageBoxResult.Yes);

            return false;
        }
    }
}
