using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPChatroomClient
{
    internal class MessageHandler
    {
        private ClientData ServerData;
        private CancellationTokenSource CancelTokenSource;
        private CancellationToken CancelToken;

        //Message Identification
        public readonly string ServerCommand = "SERVERCOMMAND";
        public readonly string ServerMessage = "SERVERMESSAGE";
        public readonly string UserMessage = "USERMESSAGE";

        public readonly string DisconnectMessage = "DISCONNECT";
        public readonly string NameTakenMessage = "NAME TAKEN";
        public readonly string UserConnectedMessage = "CONNECTED";
        public readonly string ServerCapacityMessage = "SERVER AT CAPACITY";
        public readonly string MessageFailedMessage = "MESSAGE FAILED TO SEND";


        public MessageHandler()
        {
            this.ServerData = new ClientData("Server");
            CancelTokenSource = new CancellationTokenSource();
            CancelToken = CancelTokenSource.Token;
        }

        //MESSAGES
        public async Task WaitUserMessage(ClientData user)
        {
            while (!CancelTokenSource.IsCancellationRequested)
            {
                try
                {
                    MessageData receivedMessage = await ReceiveMessage(user);

                    if (receivedMessage.MessageType == ServerCommand && receivedMessage.Message == DisconnectMessage)
                    {
                        await user.DisconnectClient();
                        break;
                    }
                    else if (receivedMessage.MessageType == UserMessage)
                    {
                        //Display new message in chatlog
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        public async Task<MessageData> ReceiveMessage(ClientData user)
        {
            try
            {
                byte[] data = new byte[1024];
                Int32 bytes = await user.ClientStream.ReadAsync(data, 0, data.Length, CancelToken);
                MessageData message = new MessageData();

                message = message.Deserialize(data, bytes);

                return message;
            }
            catch (IOException ex)
            {
                Debug.WriteLine($"Error: {ex.Message} \n");
                await user.DisconnectClient();
                return null;
            }
        }

        public async Task SendMessage(ClientData user, string message)
        {
            MessageData data;

            if (message == DisconnectMessage)
            {
                data = new MessageData(ServerCommand, user, message);
            }
            else
            {
                data = new MessageData(UserMessage, user, message);
            }

            byte[] messageToSend = data.Serialize();

            await user.ClientStream.WriteAsync(messageToSend, 0, messageToSend.Length);
        }

        public void StopWaitUserMessage()
        {
            CancelTokenSource.Cancel();
            CancelTokenSource.Dispose();
        }
    }
}
