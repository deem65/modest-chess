using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Tests
{
    public partial class NewGameTests
    {
        private readonly GameState gs = new();

        [Fact]
        public void WhiteStarts()
        {
            Assert.Equal(0, gs.SideToMove);
        }
        [Fact]
        public void IsOngoing()
        {
            Assert.Equal(GameResult.Ongoing, gs.Result);
        }
        [Fact]
        public void IsStandardBoard()
        {
            Assert.Equal(32, gs.Board.Board.Count(sq => sq.Piece != null)); 
        }
    }
}
