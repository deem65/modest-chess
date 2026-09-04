using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Tests
{
    public partial class MoveTests
    {
        private readonly BoardModel b = new();

        [Theory]
        [InlineData(1, 3, 1, 3, 3)]
        [InlineData(6, 3, 6, 3, 8)]
        [InlineData(57, 6, 1, 6, 3)] 
        [InlineData(62, 6, 6, 6, 8)]
        public void KnightStartingMoves(int knightIndex, int x1, int y1, int x2, int y2)
        {
            List<Position> moves = GameUtil.FindMoves(b, b.Board[knightIndex]);

            Assert.Equal(2, moves.Count);
            Assert.Contains(new Position(x1, y1), moves);
            Assert.Contains(new Position(x2, y2), moves);
        }
    }
}
