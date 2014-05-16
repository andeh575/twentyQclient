using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace twentyQclient
{
    public class Client
    {
        public int port;
        public string IP;
        private TcpClient client;

        public void connect()
        {
            client = new TcpClient();
            client.Connect(IP, port);
        } 
    }
}
