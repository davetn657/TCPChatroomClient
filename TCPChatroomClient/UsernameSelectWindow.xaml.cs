using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// Interaction logic for UsernameSelectWindow.xaml
    /// </summary>
    public partial class UsernameSelectWindow : Window
    {

        private MessageBoxButton _button = MessageBoxButton.OK;
        private MessageBoxImage _warningIcon = MessageBoxImage.Warning;
        private MessageBoxResult _result;
        MainWindow _mainWindow;
        ClientData _clientData;
        MessageHandler _messageHandler;

        public UsernameSelectWindow()
        {
            InitializeComponent();
            _mainWindow = this.Owner as MainWindow;
            _clientData = _mainWindow._clientData;
            _messageHandler = _clientData.messageHandler;
        }

        private void JoinBtn_Click(object sender, RoutedEventArgs e)
        {
            //Send the username to the server and wait for response.
            //if username is good close window
            //if username is not good display messagebox telling them to try a different username

            string username = UsernameText.Text;
            TryUsername(username);
        }

        private async void TryUsername(string username)
        {
            

            await _messageHandler.SendMessage(username);
            MessageData confirmMessage = await _messageHandler.ReceiveMessage();

            if (IsValidUsername(confirmMessage))
            {
                //change clientdata name & display list of all users in MainWindow
                _mainWindow._clientData.name = username;
                DisplayAllUsers();
                _mainWindow._connected = true;
                this.Close();
            }
            else
            {
                //try different username
                TryDifferentNamePopUp();
            }
        }

        private bool IsValidUsername(MessageData confirmMessage)
        {
            if (_messageHandler.CheckMessageType(confirmMessage) && confirmMessage.message == ServerCommands.nameConfirmMessage)
            {
                return true;
            }
            return false;
        }

        private void TryDifferentNamePopUp()
        {
            string messageBoxText = "Username not valid. Try a different one";
            string captionText = "Username taken!";

            _result = MessageBox.Show(messageBoxText, captionText, _button, _warningIcon, MessageBoxResult.Yes);
        }

        private async void DisplayAllUsers()
        {
            MessageData message = await _messageHandler.ReceiveMessage();
            string allUser = message.message;
            string[] users = allUser.Split(',');

            foreach (string user in users)
            {
                _mainWindow.ConnectedUsers.Text += $"{user}\n";
            }
        }
    }
}
