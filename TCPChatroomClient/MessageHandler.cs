using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPChatroomClient
{
    public class MessageHandler
    {
        private ClientData userData;
        private CancellationTokenSource cancelTokenSource;
        private CancellationToken cancelToken;

        public MessageHandler(ClientData userData)
        {
            this.userData = userData;
            cancelTokenSource = new CancellationTokenSource();
            cancelToken = cancelTokenSource.Token;
        }

        //MESSAGES
        public async Task<MessageData> ReceiveMessage()
        {
            try
            {
                byte[] data = new byte[1024];
                Int32 bytes = await userData.clientStream.ReadAsync(data, 0, data.Length, cancelToken);
                MessageData message = new MessageData();

                message = message.Deserialize(data, bytes);

                return message;
            }
            catch (IOException ex)
            {
                Debug.WriteLine($"Error: {ex.Message} \n");
                userData.DisconnectClient();
                return null;
            }
        }

        public async Task SendMessage(string message)
        {
            MessageData data = new MessageData(ServerCommands.userMessage, userData.name, message);
            byte[] messageToSend = data.Serialize();

            await userData.clientStream.WriteAsync(messageToSend, 0, messageToSend.Length);
        }

        public async Task SendUserCommand(string messageType, string message)
        {
            Debug.WriteLine("SENDING USER COMMAND: " + messageType);
            MessageData data = new MessageData(messageType, userData.name, message);
            byte[] messageToSend = data.Serialize();

            await userData.clientStream.WriteAsync(messageToSend, 0, messageToSend.Length);
        }

        public async Task SendDisconnect()
        {
            await this.SendMessage(ServerCommands.disconnectMessage);
        }

        public bool CheckIfServerMessage(MessageData message)
        {
            if(ServerCommands.commandMessages.Contains(message.messageType))
            {
                return true;
            }

            return false;
        }

        public void StopWaitUserMessage()
        {
            cancelTokenSource.Cancel();
            cancelTokenSource.Dispose();
        }
    }
}
