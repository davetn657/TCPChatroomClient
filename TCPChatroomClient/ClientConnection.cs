using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPChatroomClient
{
    public class ClientConnection
    {
        public string Host;
        public int Port;
        public TcpClient Client;
        public NetworkStream Stream;
        
        public ClientConnection()
        {
            this.Host = "localhost";
            this.Port = 0; 
        }

        public ClientConnection(string host, int port)
        {
            this.Host = host;
            this.Port = port;
        }

        public void StartConnection()
        {
            try
            {
                Client = new TcpClient(Host, Port);
                Stream = Client.GetStream();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Could not connect! Error: {ex}");
            }
        }

        public void StopConnection()
        {

        }

    }
}
