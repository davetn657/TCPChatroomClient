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
        MessageBoxButton _button = MessageBoxButton.OK;
        MessageBoxImage _icon = MessageBoxImage.Warning;
        MessageBoxResult _result;
        
        string _messageBoxText = string.Empty;
        string _captionText = string.Empty;

        public ServerConnectWindow()
        {
            InitializeComponent();

        }

        private void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            string host = IpText.Text;
            string port = PortText.Text;

            if (CheckValidIP(host) && CheckValidPort(port))
            {

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

        private bool CheckValidPort(string port)
        {
            int portInt;
            Int32.TryParse(port, out portInt);

            if(portInt > 40000 &&  portInt < 45000)
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
