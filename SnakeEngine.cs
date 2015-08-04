using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SnakeWars.SampleBot
{
    internal class SnakeEngine
    {
        private readonly string _mySnakeId;
        private readonly StreamWriter _writer;
        private readonly Random _random = new Random();

        public SnakeEngine(string mySnakeId, StreamWriter writer)
        {
            _mySnakeId = mySnakeId;
            _writer = writer;
        }

        public void NextMove(GameBoardState gameBoardState)
        {
            //===========================
            // Your snake logic goes here
            //===========================

            var mySnake = gameBoardState.GetSnake(_mySnakeId);
            if (mySnake.IsAlive)
            {
                var occupiedCells = gameBoardState.GetOccupiedCells();

                // Check possible moves in random order.
                var moves = new List<MoveDirection>
                {
                    MoveDirection.Left,
                    MoveDirection.Right,
                    MoveDirection.Straight
                };
                
                var foundValidMove = false;
                while (!foundValidMove && moves.Any())
                {
                    // Select random move.
                    var move = moves[_random.Next(moves.Count)];
                    moves.Remove(move);

                    var newHead = gameBoardState.GetSnakeNewHeadPosition(_mySnakeId, move);
                    if (!occupiedCells.Contains(newHead))
                    {
                        foundValidMove = true;
                        var command = move.Command;
                        _writer.WriteLine(command);
                        Console.WriteLine("Sending command {0}", command);
                    }
                }
            }
        }
    }
}