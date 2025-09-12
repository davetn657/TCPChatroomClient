using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
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

        public UsernameSelectWindow()
        {
            InitializeComponent();
        }

        private async void JoinBtn_Click(object sender, RoutedEventArgs e)
        {
            //Send the username to the server and wait for response.
            MainWindow? mainWindow = Owner as MainWindow;
            string username = UsernameText.Text;

            if(await mainWindow.TryUsername(username))
            {
                Close();
                await mainWindow.StartMessageLoop();
            }
            else
            {
                TryDifferentNamePopUp();
            }
        }

        public void TryDifferentNamePopUp()
        {
            string messageBoxText = "Username not valid. Try a different one";
            string captionText = "Username taken!";

            _result = MessageBox.Show(messageBoxText, captionText, _button, _warningIcon, MessageBoxResult.OK);
        }
    }
}
