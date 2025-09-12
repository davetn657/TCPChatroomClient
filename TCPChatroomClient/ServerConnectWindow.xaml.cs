using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private MessageBoxImage _warningIcon = MessageBoxImage.Warning;
        private MessageBoxResult _result;

        public ServerConnectWindow()
        {
            InitializeComponent();
        }

        private async void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            string host = IpText.Text;
            string portText = PortText.Text;

            if (CheckValidIP(host) && CheckValidPort(portText))
            {
                try
                {
                    TryConnection(IPAddress.Parse(host), Int32.Parse(portText));
                }
                catch (FormatException ex)
                {
                    Console.WriteLine($"Error: Could not convert port to int\n{ex.Message}");
                }
            }
        }

        private bool CheckValidIP(string host)
        {

            if (IPAddress.TryParse(host, out IPAddress ip))
            {
                return true;
            }

            string messageBoxText = "Host is not valid! Try this format (127.0.0.1)";
            string captionText = "Invalid Host";

            _result = MessageBox.Show(messageBoxText, captionText, _button, _warningIcon, MessageBoxResult.OK);
            return false;
        }

        private bool CheckValidPort(string portText)
        {
            int port = 0;

            if(Int32.TryParse(portText, out port) && (port > 40000 && port < 45000))
            {
                return true;
            }

            string messageBoxText = "Port is invalid! Try a number between 40000 - 45000";
            string captionText = "Invalid Port";

            _result = MessageBox.Show(messageBoxText, captionText, _button, _warningIcon, MessageBoxResult.OK);
            return false;
        }

        private void TryConnection(IPAddress host, int port)
        {
            try
            {
                MainWindow? mainWindow = Owner as MainWindow;

                if (mainWindow != null)
                {
                    mainWindow.StartConnection(host, port);
                    //Task.Run(() => mainWindow.CheckServerCapacity());
                }
                else
                {
                    Debug.WriteLine("main is null");
                }
                
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }
    }

}
