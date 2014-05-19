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

        private int lives;  // Guesses/Questions left in current game
        private bool game;  // Game in progress flag
        private TcpClient client;
        private string ver = "Twenty Questions 1.0";

        public void connect()
        {
            client = new TcpClient();

            try
            {
                client.Connect(IP, port);
                Console.WriteLine("Connected to Game server ({0}:{1})", IP, port);
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
            Console.WriteLine("Welcome to {0}", ver);
            Console.WriteLine("Type 'h' for a list of commands");

            while(true)
            {
                Console.WriteLine("Enter a command...");
                string command = Console.ReadLine();

                switch(command)
                {
                    case "q":
                    case "Q":
                        Quit();
                        return;
                    case "a":
                    case "A":
                        Answer();
                        break;
                    case "?":
                        Question();
                        break;
                    case "e":
                    case "E":
                        End(false);
                        break;
                    case "s":
                    case "S":
                        Start();
                        break;
                    case "h":
                    case "H":
                        Help();
                        break;
                    default:
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
            Console.WriteLine();
            Console.WriteLine("Quitting {0}", ver);
            Console.WriteLine("Ending connection with server...");
            
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
            string data;
            byte[] package;

            // Need space for carriage return and new line char
            Console.WriteLine("Type out your question: (252 max char)");
            Console.WriteLine();

            data = Console.ReadLine();

            if(data.Length > 254)
            {
                Console.WriteLine("Input is too long");
                return;
            }
            else
            {
                Console.WriteLine("Transmitting question...");
                package = packageData("Q:", data);
                transmitData(package);
            }

        }

        /**
         * Host clients use this function to respond to player client's
         * question messages. Simple string input with yes/no answer.
         * Should only be accessible by the host clients.
         */ 
        public void Answer()
        {
            string answer;
            byte[] package;
            bool flag = true;

            Console.WriteLine("Did the other player guess the answer? (y/n)");
            Console.WriteLine();

            // Waiting for the client to tell us if the answer was correct
            while (flag)
            {
                answer = Console.ReadLine();

                switch(answer)
                {
                    case "y":
                    case "Y":
                        Console.WriteLine("Sending Answer...");
                        package = packageData("A:", answer);
                        transmitData(package);
                        End(true);
                        flag = false;
                        break;
                    case "n":
                    case "N":
                        Console.WriteLine("Sending Answer...");
                        package = packageData("A:", answer);
                        transmitData(package);
                        flag = false;
                        break;
                    default:
                        Console.WriteLine("Please type 'y' or 'n'");
                        Console.WriteLine();
                        break;
                }
            }
        }

        /**
         * Notice from host client that the game has either been won or lost.
         * Should only be accessible by the host client.
         */ 
        private void End(Boolean win)
        {
            byte[] package;

            // Did the game end in a win or loss for the players?
            if(win)
            {
                Console.WriteLine("They guess the answer!");
                package = packageData("E:", "Game Over - You Win!");
                transmitData(package);
            }
            else
            {
                Console.WriteLine("They didn't guess the answer!");
                package = packageData("E:", "Game Over - You lose!");
                transmitData(package);
            }

            Console.WriteLine("Game Over - Let's play again!\n");
        }

        /**
         * Notice from host client that a game has been started.
         * Accessible by any client - designates them as host of the game.
         */
        private void Start()
        {
            byte[] package;

            Console.WriteLine("Initiating a new game\n");

            lives = 20;
            game = true;

            package = packageData("S:", "A new game has been started");
            transmitData(package);

            Console.WriteLine("Game started! Lives: {0}", lives);
        }

        /**
         * Client has entered an invalid command - remind them of the valid
         * inputs.
         */
        private void Invalid()
        {
            Console.WriteLine("\nInvalid command received!");
            Console.WriteLine("Type 'h' for a list of commands\n");
        }

        /**
         * Client has asked for the command list
         */
        private void Help()
        {
            Console.WriteLine("\nValid Commands:");
            Console.WriteLine("S : Start a new game");
            Console.WriteLine("? : Ask a new question");
            Console.WriteLine("A : Answer a question");
            Console.WriteLine("E : End the current game");
            Console.WriteLine("Q : Quit the program\n");
        }

        /**
         * Function to help the client package commands and data into
         * the proper format that the server is expecting
         */
        private byte[] packageData(string command, string data)
        {
            byte[] package = new byte[256];
            string raw = command + data;

            package = Encoding.ASCII.GetBytes(raw);

            return package;
        }

        /**
         * Function to help unpackage the information received from the
         * server.
         */
        private string unpackageData(byte[] raw)
        {
            string unpackaged = " ";

            return unpackaged;
        }

        /**
         * Function that actually transmits messages to the server
         */
        private void transmitData(byte[] data)
        {

        }

    }
}
