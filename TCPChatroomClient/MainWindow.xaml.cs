using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace TCPChatroomClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ClientData _clientData;
        private MessageHandler _handler;
        public bool _connected = false;

        ServerConnectWindow serverConnectWindow = new ServerConnectWindow();
        UsernameSelectWindow userSelectWindow = new UsernameSelectWindow();

        private MessageBoxButton _button = MessageBoxButton.OK;
        private MessageBoxImage _warningIcon = MessageBoxImage.Warning;
        private MessageBoxResult _result;

        public MainWindow()
        {
            InitializeComponent();
            _clientData = new ClientData("Temp");
            _handler = _clientData.messageHandler;
        }

        private void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {

            serverConnectWindow.Owner = this;
            serverConnectWindow.Show();

            //Change ConnectBtn to DisconnectBtn
        }

        private void DisconnectBtn_Click(object sender, RoutedEventArgs e)
        {
            ConnectBtn.Visibility = Visibility.Visible;
            ConnectBtn.IsEnabled = true;
            DisconnectBtn.Visibility = Visibility.Hidden;
            DisconnectBtn.IsEnabled = false;

            _clientData.DisconnectClient();
        }

        private async void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_clientData.clientStream != null)
            {
                await _handler.SendMessage($"{_clientData.name}: {MessageText.Text}");
            }
            else
            {
                //throw a messagebox that the user is not connected to a server
            }
        }

        public async Task<bool> StartConnection(IPAddress host, int port)
        {
            await _clientData.ConnectClient(host, port);
            await _handler.SendUserCommand(ServerCommands.userConnectedMessage, "Connected");

            MessageData messageData = await _handler.ReceiveMessage();

            if (messageData.messageType == ServerCommands.joinedServerMessage)
            {
                await CheckServerMessage(messageData);
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task WaitMessageLoop()
        {
            while (true)
            {
                Debug.WriteLine("WAITING FOR MESSAGE");
                MessageData messageData = await _handler.ReceiveMessage();

                if (_handler.CheckIfServerMessage(messageData))
                {
                    Debug.WriteLine($"SERVER MESSAGE: {messageData.message}");
                    //check what type of message it is and respond accordingly
                    await CheckServerMessage(messageData);
                }
                else
                {
                    //display message on chatlog as it will be a user message
                    ChatLog.Text += messageData.message + "\n";
                }

            }
        }

        private async Task CheckServerMessage(MessageData messageData)
        {
            switch (messageData.messageType)
            {
                case ServerCommands.joinedServerMessage:
                    userSelectWindow.Owner = this;
                    userSelectWindow.Show();
                    break;

                case ServerCommands.serverCapacityMessage:
                    //User will not be able to connect to the server so remove it from the stream and allow the user to try to connect rto a different server
                    _clientData.DisconnectClient();
                    break;

                case ServerCommands.disconnectMessage:
                    _clientData.DisconnectClient();
                    break;

                case ServerCommands.messageFailedMessage:
                    //throw messagebox that tells user could not send their message
                    string messageBoxText = "Message could not send! Try again";
                    string captionText = "Message Send Failed!";
                    _result = MessageBox.Show(messageBoxText, captionText, _button, _warningIcon, MessageBoxResult.OK);
                    break;

                case ServerCommands.sendingAllConnectedMessage:
                    await _handler.SendUserCommand(ServerCommands.acceptAllConnectedMessage, "Accepting Usernames");
                    DisplayAllUsers(messageData.message);
                    break;

                case ServerCommands.userConnectedMessage:
                    if (messageData.message != _clientData.name)
                    {
                        ConnectedUsers.Text += $"{messageData.message}\n";
                    }
                    break;
                default:
                    break;
            }
        }
        public async Task<bool> TryUsername(string username)
        {
            await _handler.SendUserCommand(ServerCommands.userMessage, username);
            MessageData messageData = await _handler.ReceiveMessage();

            if (messageData.messageType == ServerCommands.nameConfirmMessage)
            {
                _clientData.name = username;
                return true;
            }
            else
            {
                return false;
            }
        }

        private void DisplayAllUsers(string message)
        {
            string allUser = message;
            string[] users = allUser.Split(',');

            foreach (string user in users)
            {
                ConnectedUsers.Text += $"{user}\n";
            }
        }

        
    }
}