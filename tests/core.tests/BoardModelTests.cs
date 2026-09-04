using CoreLogic;

namespace Core.Tests;

public class BoardModelTests
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
    public void TestStartingPosition()
    {
        AssertStartingPieces();
        AssertEmptySquares();
    }

    private void AssertEmptySquares()
    {
        for (int i = 16; i < 48; i++)
        {
            Assert.Null(b.Board[i].Piece);
        }
    }

    private void AssertStartingPieces()
    {
        for (int i = 0; i < 8; i++)
        {
            AssertPiece(i, backRank[i], 0);
            AssertPiece<Pawn>(i + 8, 0);
            AssertPiece<Pawn>(i + 48, 1);
            AssertPiece(i + 56, backRank[i], 1);
        }
    }

    private void AssertPiece(int i, Type t, int colorId)
    {
        Piece? p = b.Board[i].Piece;

        Assert.NotNull(p);
        Assert.Equal(t, p.GetType());
        Assert.Equal(colorId, p.ColorId);
    }

    private void AssertPiece<T>(int index, int colorId) where T : Piece
    {
        Piece? p = b.Board[index].Piece;

        Assert.NotNull(p);
        Assert.IsType<T>(p);
        Assert.Equal(colorId, p.ColorId);
    }
}