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
        private ClientData ServerData;
        private ClientData UserData;
        private CancellationTokenSource CancelTokenSource;
        private CancellationToken CancelToken;

        //Message Identification
        private const string ServerMessage = "SERVERMESSAGE";
        private const string UserMessage = "USERMESSAGE";

        private const string DisconnectMessage = "DISCONNECTED";
        private const string NameTakenMessage = "NAME TAKEN";
        private const string UserConnectedMessage = "CONNECTED";
        private const string ServerCapacityMessage = "SERVER AT CAPACITY";
        private const string MessageFailedMessage = "MESSAGE FAILED TO SEND";


        public MessageHandler(ClientData userData)
        {

            this.UserData = userData;
            this.ServerData = new ClientData("Server");
            CancelTokenSource = new CancellationTokenSource();
            CancelToken = CancelTokenSource.Token;
        }

        //MESSAGES
        public async Task WaitUserMessage()
        {
            while (!CancelTokenSource.IsCancellationRequested)
            {
                try
                {
                    MessageData receivedMessage = await ReceiveMessage();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        public async Task<MessageData> ReceiveMessage()
        {
            try
            {
                byte[] data = new byte[1024];
                Int32 bytes = await UserData.ClientStream.ReadAsync(data, 0, data.Length, CancelToken);
                MessageData message = new MessageData();

                message = message.Deserialize(data, bytes);

                return message;
            }
            catch (IOException ex)
            {
                Debug.WriteLine($"Error: {ex.Message} \n");
                UserData.DisconnectClient();
                return null;
            }
        }

        public async Task SendMessage(string message)
        {
            MessageData data;

            data = new MessageData(UserMessage, UserData, message);

            byte[] messageToSend = data.Serialize();

            await UserData.ClientStream.WriteAsync(messageToSend, 0, messageToSend.Length);
        }

        public async Task SendDisconnect()
        {
            await this.SendMessage(DisconnectMessage);
        }

        public bool CheckServerMessage(MessageData message)
        {
            if(message.MessageType == ServerMessage)
            {
                return true;
            }

            return false;
        }

        public void StopWaitUserMessage()
        {
            CancelTokenSource.Cancel();
            CancelTokenSource.Dispose();
        }
    }
}
