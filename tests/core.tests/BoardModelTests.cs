namespace Core.Tests;

public partial class BoardModelTests
{
    private readonly BoardModel b = new();
    private readonly Type[] backRank =
        [
            typeof(Rook),
            typeof(Knight),
            typeof(Bishop),
            typeof(Queen),
            typeof(King),
            typeof(Bishop),
            typeof(Knight),
            typeof(Rook)
        ];

    [Fact]
    public void AssertStartingPosition()
    {
        AssertStartingPieces();
        AssertEmptySquares();
    }

    [Fact]
    public void Assert64Squares()
    {
        Assert.Equal(64, b.Board.Length);
    }

    [Fact]
    public void Assert32Pieces()
    {
        Assert.Equal(32, b.Board.Count(s => s.Piece != null));
    }
}