using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Xml.Linq;

namespace TCPChatroomClient
{
    public class ClientData
    {
        public string name { get; set; }
        public TcpClient client { get; set; }
        public NetworkStream clientStream { get; set; }
        public MessageHandler messageHandler { get; set; }

        public ClientData()
        {
            this.name = string.Empty;
            client = new TcpClient();
            this.messageHandler = new MessageHandler(this);
        }

        public ClientData(string name, TcpClient client, NetworkStream stream)
        {
            this.name = name;
            this.client = client;
            this.clientStream = stream;
            this.messageHandler = new MessageHandler(this);
        }

        public ClientData(string name) 
        {
            this.name = name;
            client = new TcpClient();
            this.messageHandler = new MessageHandler(this);
        }

        public async Task ConnectClient(IPAddress host, int port)
        {
            try
            {
                Debug.WriteLine($"HOST:{host} PORT:{port}");
                await client.ConnectAsync(host, port);
                using NetworkStream stream = client.GetStream();
                clientStream = stream;

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Could not connect! Error: {ex}");
            }
        }
        //CLIENT DISCONNECTION
        public void DisconnectClient()
        {
            try
            {
                this.messageHandler.StopWaitUserMessage();

                this.clientStream?.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error {ex.Message} \nUnexpected Disconnection");
            }
        }
    }
}
