namespace Core.Tests;

public partial class BoardModelTests
{
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

    private void AssertEmptySquares()
    {
        for (int i = 16; i < 48; i++)
        {
            Assert.Null(b.Board[i].Piece);
        }
    }

    private void AssertPiece(int index, Type type, int colorId)
    {
        Piece? piece = b.Board[index].Piece;

        Assert.NotNull(piece);
        Assert.Equal(type, piece.GetType());
        Assert.Equal(colorId, piece.ColorId);
    }

    private void AssertPiece<T>(int index, int colorId) where T : Piece
    {
        Piece? piece = b.Board[index].Piece;

        Assert.NotNull(piece);
        Assert.IsType<T>(piece);
        Assert.Equal(colorId, piece.ColorId);
    }
}