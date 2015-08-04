using System;
using System.Collections.Generic;
using System.Linq;

namespace SnakeWars.SampleBot
{
    internal class GameBoardState
    {
        private readonly GameStateDTO _gameState;

        public GameBoardState(GameStateDTO gameState)
        {
            _gameState = gameState;
        }

        public PointDTO GetSnakeNewHeadPosition(string snakeId, MoveDirection move)
        {
            var snake = GetSnake(snakeId);
            var newHead = move.GetSnakeNewHead(snake, _gameState.BoardSize);
            return newHead;
        }
        
        public HashSet<PointDTO> GetOccupiedCells()
        {
            return new HashSet<PointDTO>(_gameState.Walls.Concat(_gameState.Snakes.SelectMany(snake => snake.Cells)));
        }

        public SnakeDTO GetSnake(string snakeId)
        {
            return _gameState.Snakes.First(s => s.Id == snakeId);
        }
    }
}