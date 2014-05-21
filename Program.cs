using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace twentyQclient
{
    class Program
    {
        static void Main(string[] args)
        {

            Client[] clients = new Client[1];
            for(int i = 0; i < clients.Length; i++)
            {
                clients[i] = new Client();
                clients[i].port = 14000;
                clients[i].IP = "127.0.0.1";
                clients[i].connect();
            }
        }
    }
}
