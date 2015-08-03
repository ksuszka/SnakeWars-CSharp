using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using Newtonsoft.Json;
using SnakeWars.SampleBot.Properties;

namespace SnakeWars.SampleBot
{
    internal class Program
    {
        private static readonly Random rng = new Random();

        private static readonly List<DirectionOffset> Offsets = new List<DirectionOffset>
        {
            new DirectionOffset {Direction = SnakeDirection.Up, Offset = new Offset {DX = 0, DY = 1}},
            new DirectionOffset {Direction = SnakeDirection.Right, Offset = new Offset {DX = 1, DY = 0}},
            new DirectionOffset {Direction = SnakeDirection.Down, Offset = new Offset {DX = 0, DY = -1}},
            new DirectionOffset {Direction = SnakeDirection.Left, Offset = new Offset {DX = -1, DY = 0}}
        };

        private static PointDTO OffsetModulo(PointDTO p, Offset offset, SizeDTO boardSize)
        {
            return new PointDTO
            {
                X = (p.X + offset.DX + boardSize.Width)%boardSize.Width,
                Y = (p.Y + offset.DY + boardSize.Height)%boardSize.Height
            };
        }

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

                            // Loop till key press.
                            while (!Console.KeyAvailable)
                            {
                                var gameState = JsonConvert.DeserializeObject<GameStateDTO>(reader.ReadLine());

                                var mySnake = gameState.Snakes.First(s => s.Id == mySnakeId);
                                if (mySnake.IsAlive)
                                {
                                    var occupiedCells =
                                        new HashSet<PointDTO>(
                                            gameState.Walls.Concat(gameState.Snakes.SelectMany(snake => snake.Cells)));

                                    // Check possible moves in random order.
                                    var moves = new List<Tuple<string, int>>
                                    {
                                        Tuple.Create("LEFT", -1),
                                        Tuple.Create("RIGHT", 1),
                                        Tuple.Create("STRAIGHT", 0)
                                    };

                                    var foundValidMove = false;
                                    while (!foundValidMove && moves.Any())
                                    {
                                        // Select random move.
                                        var move = moves[rng.Next(moves.Count)];
                                        moves.Remove(move);

                                        var index = Offsets.FindIndex(t => t.Direction == mySnake.Direction);
                                        var offset = Offsets[(index + move.Item2 + 4)%4];
                                        var newHead = OffsetModulo(mySnake.Head, offset.Offset, gameState.BoardSize);
                                        if (!occupiedCells.Contains(newHead))
                                        {
                                            foundValidMove = true;
                                            var command = move.Item1.ToUpper();
                                            writer.WriteLine(command);
                                            Console.WriteLine("Sending command {0}", command);
                                        }
                                    }
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

        private struct Offset
        {
            public int DX { get; set; }
            public int DY { get; set; }
        }

        private struct DirectionOffset
        {
            public SnakeDirection Direction { get; set; }
            public Offset Offset { get; set; }
        }
    }
}