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
                Console.WriteLine("Connected to Game server ({0}:{0})", IP, port);
                playTQ();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error connecting to game server:");
                Console.WriteLine(ex.Message);
            }
        }

        public void playTQ()
        {
            Console.WriteLine("Welcome to Twenty Questions 1.0");
            Console.WriteLine("Type 'h' for a list of commands");

            while(true)
            {
                Console.WriteLine("Enter a command...");
                string command = Console.ReadLine();

                switch(command)
                {
                    case "q":
                    case "Q":
                        Console.WriteLine("Received command: {0}", command);
                        Quit();
                        return;
                    case "a":
                    case "A":
                        Console.WriteLine("Received command: {0}", command);
                        Answer();
                        break;
                    case "?":
                        Console.WriteLine("Received command: {0}", command);
                        Question();
                        break;
                    case "e":
                    case "E":
                        Console.WriteLine("Received command: {0}", command);
                        End();
                        break;
                    case "s":
                    case "S":
                        Console.WriteLine("Received command: {0}", command);
                        Start();
                        break;
                    case "h":
                    case "H":
                        Help();
                        break;
                    default:
                        Console.WriteLine("Invalid command: {0}", command);
                        Invalid();
                        break;
                }
            
            }
        }

        /**
         * Tell the server that the client wants to exit the game and
         * close the connection.
         */ 
        public void Quit()
        {
            Console.WriteLine("Ending connection with server");
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
            Console.WriteLine("QUESTION");
        }

        /**
         * Host clients use this function to respond to player client's
         * question messages. Simple string input with yes/no answer.
         * Should only be accessible by the host clients.
         */ 
        public void Answer()
        {
            Console.WriteLine("ANSWER");
        }

        /**
         * Notice from host client that the game has either been won or lost.
         * Should only be accessible by the host client.
         */ 
        public void End()
        {
            Console.WriteLine("END");
        }

        /**
         * Notice from host client that a game has been started.
         * Accessible by any client - designates them as host of the game.
         */
        public void Start()
        {
            Console.WriteLine("START");
        }

        /**
         * Client has entered an invalid command - remind them of the valid
         * inputs.
         */
        public void Invalid()
        {
            Console.WriteLine();
            Console.WriteLine("Invalid command received!");
            Console.WriteLine("Type 'h' for a list of commands");
            Console.WriteLine();
        }

        public void Help()
        {
            Console.WriteLine();
            Console.WriteLine("Valid Commands:");
            Console.WriteLine("S : Start a new game");
            Console.WriteLine("? : Ask a new question");
            Console.WriteLine("A : Answer a question");
            Console.WriteLine("E : End the current game");
            Console.WriteLine("Q : Quit the program");
            Console.WriteLine();
        }
    }
}
