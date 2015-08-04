using System;
using System.IO;
using System.Net.Sockets;
using Newtonsoft.Json;
using SnakeWars.SampleBot.Properties;

namespace SnakeWars.SampleBot
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                using (var tcpClient = new TcpClient())
                {
                    tcpClient.NoDelay = true;
                    tcpClient.Connect(Settings.Default.ServerHost, Settings.Default.ServerPort);
                    using (var reader = new StreamReader(tcpClient.GetStream()))
                    {
                        using (var writer = new StreamWriter(tcpClient.GetStream()))
                        {
                            writer.AutoFlush = true;

                            // Server should start with "ID" line.
                            if (reader.ReadLine() != "ID")
                                throw new InvalidDataException("Server didn't ask for my identity.");

                            // Send our login id.
                            writer.WriteLine(Settings.Default.LoginId);
                            // Read our snake id.
                            var mySnakeId = reader.ReadLine();
                            Console.WriteLine("My snake id: {0}", mySnakeId);

                            var snakeEngine = new SnakeEngine(mySnakeId);

                            // Loop till key press.
                            while (!Console.KeyAvailable)
                            {
                                var gameBoardState =
                                    new GameBoardState(JsonConvert.DeserializeObject<GameStateDTO>(reader.ReadLine()));
                                var move = snakeEngine.GetNextMove(gameBoardState);
                                if (move)
                                {
                                    Console.WriteLine("Sending command {0}", move.Command);
                                    writer.WriteLine(move.Command);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex);
            }
        }
    }
}