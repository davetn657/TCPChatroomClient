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

        private MessageBoxButton _button = MessageBoxButton.OK;
        private MessageBoxImage _warningIcon = MessageBoxImage.Warning;
        private MessageBoxResult _result;

        ServerConnectWindow serverConnectWindow = new ServerConnectWindow();

        public MainWindow()
        {
            InitializeComponent();
            _clientData = new ClientData("temp");
            _handler = _clientData.messageHandler;
        }

        private void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            serverConnectWindow.Owner = this;
            serverConnectWindow.ShowDialog();

            //Change ConnectBtn to DisconnectBtn
        }

        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        public async Task StartConnection(IPAddress host, int port)
        {
            await _clientData.ConnectClient(host, port);

            if(await CheckServerCapacity())
            {
                //start loop to recieve messages
                await WaitMessageLoop();
            }


        }
        public async Task WaitMessageLoop()
        {
            while (true)
            {
                MessageData messageData = await _handler.ReceiveMessage();
                if (_handler.CheckMessageType(messageData))
                {
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
                case ServerCommands.disconnectMessage:
                    break;

            }
        }

        public async Task<bool> CheckServerCapacity()
        {
            MessageData message = await _handler.ReceiveMessage();

            //Message will be either a UserMessage or a ServerMessage
            //at this stage if it is a ServerMessage it can only be a MaxCapacity Message
            if (message.message == ServerCommands.joinedServerMessage)
            {
                UsernameSelectWindow userSelectWindow = new UsernameSelectWindow();
                userSelectWindow.Owner = this;
                userSelectWindow.ShowDialog();
                serverConnectWindow.Close();
                return true;
            }
            else
            {
                //User will not be able to connect to the server so remove it from the stream and allow the user to try to connect rto a different server
                _clientData.DisconnectClient();

                string messageBoxText = "Could not connect because server is full!";
                string captionText = "Server if full!";
                _result = MessageBox.Show(messageBoxText, captionText, _button, _warningIcon, MessageBoxResult.Yes);
                return false;
            }
        }

    }
}