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
        private string Host;
        private int Port;
        private TcpClient Client;
        private NetworkStream Stream;
        
        public ClientConnection()
        {
            this.Host = "localhost";
            this.Port = 0; 
        }

        public void SetHost(string host)
        {
            this.Host = host;
        }

        public void SetPort(int port)
        {
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
            try
            {

            }
            catch (Exception ex ) 
            {

            }
        }
    }
}
