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
        private Thread incoming;
        private string ver = "Twenty Questions 1.0";

        public void connect()
        {
            client = new TcpClient();

            try
            {
                client.Connect(IP, port);
                Console.WriteLine("Connected to Game server ({0}:{1})", IP, port);
                
                // Start a thread in the background that listens for server messages
                incoming = new Thread(new ThreadStart(receiveMessage));
                incoming.IsBackground = true;
                incoming.Start();

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
            byte[] package;

            Console.WriteLine();
            Console.WriteLine("Quitting {0}", ver);
            Console.WriteLine("Ending connection with server...");

            try
            {
                package = packageData("Q:", " ");
                transmitData(package);

                // Stop all threads and end connections
                client.GetStream().Close();
                client.Close();
                incoming.Abort();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.Read();
            }
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
                package = packageData("?:", data);
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
                        package = packageData("A:", "Incorrect");
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
            game = false;
        }


        /**
         * Notice from host client that a game has been started.
         * Accessible by any client - designates them as host of the game.
         */
        private void Start()
        {
            if (!game)
            {
                byte[] package;

                Console.WriteLine("Initiating a new game\n");

                lives = 20;
                game = true;

                package = packageData("S:", "A new game has been started");
                transmitData(package);

                Console.WriteLine("Game started! Lives: {0}", lives);
            }
            else
            {
                Console.WriteLine("Game already in progress!");
            }
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

            // Ensure the packet is the expected size
            raw = raw.PadRight(256);

            package = Encoding.ASCII.GetBytes(raw);

            return package;
        }

        /**
         * Function that actually transmits messages to the server
         */
        private void transmitData(byte[] data)
        {
            try
            {
                client.GetStream().Write(data, 0, 256);
                client.GetStream().Flush();
                Console.WriteLine("Package sent to server");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }

        /**
         * Only call this when the client should expect a response from
         * the server.
         */
        private void receiveMessage()
        {
            byte[] messageRaw = new byte[254];
            string testCase1 = "A new game has been started";
            string testCase2 = "Incorrect";
            string testCase3 = "Game Over - You lose!";
            string testCase4 = "Game Over - You win!";

            testCase1 = testCase1.PadRight(254);
            testCase2 = testCase2.PadRight(254);
            testCase3 = testCase3.PadRight(254);
            testCase4 = testCase4.PadRight(254);

            while (true)
            {
                try
                {
                    client.GetStream().Read(messageRaw, 0, messageRaw.Length);
                    string message = Encoding.ASCII.GetString(messageRaw);

                    Console.WriteLine("SERVER: {0}", message);

                    // Is a game in progress?
                    if (String.Equals(message, testCase1))
                    {
                        game = true;
                    }
                    else if (String.Equals(message, testCase2))
                    {
                        --lives;
                        Console.WriteLine("Lives remaining: {0}", lives);

                        // This should be handled elsewhere... ?
                        if (lives == 0)
                            End(false);
                    }
                    else if (String.Equals(message, testCase3) || String.Equals(message, testCase4))
                    {
                        game = false;
                    }

                    client.GetStream().Flush();
                }
                catch (Exception Exception)
                {
                    Console.WriteLine(Exception.Message);
                }
            }
        }

    }
}
