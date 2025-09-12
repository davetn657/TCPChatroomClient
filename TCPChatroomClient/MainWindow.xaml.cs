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
            _clientData = new ClientData("temp");
            _handler = _clientData.messageHandler;
        }

        private void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {

            serverConnectWindow.Owner = this;
            serverConnectWindow.Show();

            //Change ConnectBtn to DisconnectBtn
        }

        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        public async Task<bool> StartConnection(IPAddress host, int port)
        {
            await _clientData.ConnectClient(host, port);
            await _handler.SendUserCommand(ServerCommands.userConnectedMessage);

            MessageData messageData = await _handler.ReceiveMessage();

            if (messageData.message == ServerCommands.joinedServerMessage)
            {
                CheckServerMessage(messageData.message);
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task StartMessageLoop()
        {
            await WaitMessageLoop();
        }

        private async Task WaitMessageLoop()
        {
            while (true)
            {
                Debug.WriteLine("WAITING FOR MESSAGE");
                MessageData messageData = await _handler.ReceiveMessage();
                if (_handler.CheckMessageType(messageData))
                {
                    Debug.WriteLine($"SERVER MESSAGE: {messageData.message}");
                    //check what type of message it is and respond accordingly
                    CheckServerMessage(messageData.message);
                }
                else
                {
                    //display message on chatlog as it will be a user message
                }

            }
        }

        private void CheckServerMessage(string message)
        {
            switch (message)
            {
                case ServerCommands.joinedServerMessage:
                    Debug.WriteLine("CLOSING SERVER CONNECTION::OPENING USERNAME SELECTION");
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
                    DisplayAllUsers();
                    Task.Run(() => _handler.SendUserCommand(ServerCommands.acceptAllConnectedMessage));
                    break;
                default:
                    Debug.WriteLine("Something failed server side");
                    break;
            }
        }
        public async Task<bool> TryUsername(string username)
        {
            await _handler.SendUserCommand(username);
            MessageData messageData = await _handler.ReceiveMessage();

            if (messageData.message == ServerCommands.nameConfirmMessage)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private async void DisplayAllUsers()
        {
            MessageData message = await _handler.ReceiveMessage();
            string allUser = message.message;
            string[] users = allUser.Split(',');

            foreach (string user in users)
            {
                ConnectedUsers.Text += $"{user}\n";
            }
        }
    }
}