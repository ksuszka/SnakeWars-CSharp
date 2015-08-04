using System;
using System.Collections.Generic;
using System.Linq;

namespace SnakeWars.SampleBot
{
    internal class SnakeEngine
    {
        private readonly string _mySnakeId;
        private readonly Random _random = new Random();

        public SnakeEngine(string mySnakeId)
        {
            _mySnakeId = mySnakeId;
        }

        public Move GetNextMove(GameBoardState gameBoardState)
        {
            //===========================
            // Your snake logic goes here
            //===========================

            var mySnake = gameBoardState.GetSnake(_mySnakeId);
            if (mySnake.IsAlive)
            {
                var occupiedCells = gameBoardState.GetOccupiedCells();

                // Check possible moves in random order.
                var moves = new List<Move>
                {
                    Move.Left,
                    Move.Right,
                    Move.Straight
                };

                while (moves.Any())
                {
                    // Select random move.
                    var move = moves[_random.Next(moves.Count)];
                    moves.Remove(move);

                    var newHead = gameBoardState.GetSnakeNewHeadPosition(_mySnakeId, move);
                    if (!occupiedCells.Contains(newHead))
                    {
                        return move;
                    }
                }
            }
            return Move.None;
        }
    }
}