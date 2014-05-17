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

            try
            {
                client.Connect(IP, port);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error connecting to game server:");
                Console.WriteLine(ex.Message);
            }
        }

        /**
         * Tell the server that the client wants to exit the game and
         * close the connection.
         */ 
        public void Quit()
        {
            client.GetStream().Close();
            client.Close();
        }

        /**
         * Player clients use this function to send a question to the 
         * host client. Simple string input in the form of a question.
         * Should only be accessible by the player clients.
         */ 
        public void Question()
        {

        }

        /**
         * Host clients use this function to respond to player client's
         * question messages. Simple string input with yes/no answer.
         * Should only be accessible by the host clients.
         */ 
        public void Answer()
        {

        }

        /**
         * Notice from host client that the game has either been won or lost.
         * Should only be accessible by the host client.
         */ 
        public void End()
        {

        }

        /**
         * Notice from host client that a game has been started.
         * Accessible by any client - designates them as host of the game.
         */
        public void Start()
        {

        }
    }
}
