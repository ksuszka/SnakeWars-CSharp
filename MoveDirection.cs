using System.Collections.Generic;

namespace SnakeWars.SampleBot
{
    public class MoveDirection
    {
        public static MoveDirection Left { get { return new MoveDirection { Command = "LEFT", MoveOffset = -1 }; } }
        public static MoveDirection Right { get { return new MoveDirection { Command = "RIGHT", MoveOffset = 1 }; } }
        public static MoveDirection Straight { get { return new MoveDirection { Command = "STRAIGHT", MoveOffset = 0 }; } }

        public string Command { get; private set; }

        private int MoveOffset { get; set; }

        public PointDTO GetSnakeNewHead(SnakeDTO snake, SizeDTO boardSize)
        {
            var index = Offsets.FindIndex(t => t.Direction == snake.Direction);
            var offset = Offsets[(index + MoveOffset + 4) % 4];
            var newHead = OffsetModulo(snake.Head, offset.Offset, boardSize);
            return newHead;
        }
        
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
                X = (p.X + offset.DX + boardSize.Width) % boardSize.Width,
                Y = (p.Y + offset.DY + boardSize.Height) % boardSize.Height
            };
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