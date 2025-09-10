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
        private MessageBoxImage _warningIcon = MessageBoxImage.Warning;
        private MessageBoxResult _result;
        public ServerConnectWindow()
        {
            InitializeComponent();
        }

        private async void ConnectBtn_Click(object sender, RoutedEventArgs e)
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
                await TryConnection(host, port);
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

            _result = MessageBox.Show(messageBoxText, captionText, _button, _warningIcon, MessageBoxResult.Yes);
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

            _result = MessageBox.Show(messageBoxText, captionText, _button, _warningIcon, MessageBoxResult.Yes);
            return false;
        }

        private async Task TryConnection(string host, int port)
        {
            try
            {

                MainWindow mainWindow = this.Owner as MainWindow;
                ClientData clientData = mainWindow._clientData;
                MessageHandler messageHandler = clientData.MessageHandler;

                if (mainWindow != null) {
                    await clientData.ConnectClient(host, port);
                    MessageData message = await messageHandler.ReceiveMessage();

                    //Message will be either a UserMessage or a ServerMessage
                    //at this stage if it is a ServerMessage it can only be a MaxCapacity Message
                    if (messageHandler.CheckServerMessage(message))
                    {
                        //User will not be able to connect to the server so remove it from the stream and allow the user to try to connect rto a different server
                        clientData.DisconnectClient();

                        string messageBoxText = "Could not connect because server is full!";
                        string captionText = "Server if full!";
                        _result = MessageBox.Show(messageBoxText, captionText, _button, _warningIcon, MessageBoxResult.Yes);
                    }
                    else
                    {
                        UsernameSelectWindow userSelectWindow = new UsernameSelectWindow();
                        userSelectWindow.Owner = this.Owner;
                        userSelectWindow.ShowDialog();
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }
    }

}
