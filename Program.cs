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
            string IP;
            Client client = new Client();

            Console.WriteLine("Please enter server IP: ");
            IP = Console.ReadLine();

            client.IP = IP;
            client.port = 14000;
            client.connect();
        }
    }
}
