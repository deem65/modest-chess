using System.Linq;

namespace Core
{
    /// <summary>
    /// static class which has all methods to work with a board, it can check for checkmate, stalemate, checks, draws, move pieces, check is move is valid, find possible moves, and more.
    /// </summary>
    public static class GameUtil
    {
        public static readonly Dictionary<int, int> pawnlessMoves = new()
        {
            { 0, 0 }, // for white pieces
            { 1, 0 }  // for black pieces
        };

        //Add these values to X and Y to move: Up, Down, Right, Left, UpRight, UpLeft, DownRight, DownLeft, and all 8 knight moves...
        public static readonly int[,] moveSet = {
                { 1, 0 }, { -1, 0 }, { 0, 1 },
                { 0, -1 }, { 1, 1 }, { 1, -1 },
                { -1, 1 }, { -1, -1 }, { 2, 1 },
                { 1, 2 }, { -1, 2 }, { -2, 1 },
                { -2, -1 }, { -1, -2 }, { 1, -2 },
                { 2, -1 } };
        /*public static bool IsDraw(BoardModel board)
        {
            //WIP          
        }*/
        public static bool IsInCheckmate(int colorId, BoardModel testModel)
        {
            return IsInStalemate(colorId, testModel) && IsInCheck(colorId, testModel);
        }
        public static bool IsInStalemate(int colorId, BoardModel testModel)
        {
            return testModel.Board
                .Where(sq => sq.Piece != null && sq.Piece.ColorId == colorId)
                .All(sq => !FindMoves(testModel, sq)
                .Any(m => IsLegalMove(testModel, new Move(sq.Position, m))));
        }
        public static bool IsInCheck(int colorId, BoardModel testModel) //bug- findmoves returns movement squares not attackable squares, problem is pawns. 
        {
            Position kingLocation = testModel.Board
                .Where(sq => sq.Piece != null && sq.Piece.ColorId == colorId && sq.Piece.GetType() == typeof(King))
                .Select(sq => sq.Position)
                .FirstOrDefault();

            List<Position> opponentPossibleMoves = testModel.Board
                .Where(sq => sq.Piece != null && sq.Piece.ColorId != colorId)
                .SelectMany(sq => FindMoves(testModel, sq)).ToList();

            return opponentPossibleMoves.Contains(kingLocation);
        }
        public static bool IsLegalMove(BoardModel b, Move m)
        {
            Piece? p = b.GetSquare(m.From).Piece;

            if (p == null)
                return false;

            if (!FindMoves(b, b.GetSquare(m.From)).Contains(m.To))
                return false;

            BoardModel boardCopy = DeepCopy(b);

            ApplyMove(m, boardCopy);

            return !IsInCheck(p.ColorId, boardCopy);
        }
        public static void ApplyMove(Move m, BoardModel b)
        {
            b.GetSquare(m.To).Piece = b.GetSquare(m.From).Piece;
            b.GetSquare(m.From).Piece = null;
        }
        public static BoardModel DeepCopy(BoardModel toCopy)
        {
            return new BoardModel
            {
                Board = toCopy.Board.Select(square => new Square(square)).ToArray()
            };
        }
        public static Position NextPosition(Position current)
        {
            if (current.Y > 7)
            {
                return new Position(current.X + 1, 1);
            }
            return new Position(current.X, current.Y + 1);

        }
        public static List<Position> FindMoves(BoardModel board, Square square)
        {
            var piece = square.Piece;
            if (piece == null)
                return [];

            List<Position> results = [];
            List<int> moves = [];

            if (square.Piece == null) 
                return [];

            int startMoveSetIndex = square.Piece.StartIndex;
            int endMoveSetIndex = square.Piece.EndIndex;

            if (piece.GetType() == typeof(Pawn))
            {
                bool isPawnWhite = piece.ColorId == 0;

                int t = isPawnWhite ? 8 : -8;

                if (board.Board[CTI(square.Position.X, square.Position.Y) + t - 1].Piece != null)
                {
                    moves.Add(isPawnWhite ? 5 : 7);
                }
                if (board.Board[CTI(square.Position.X, square.Position.Y) + t + 1].Piece != null)
                {
                    moves.Add(isPawnWhite ? 4 : 6);
                }
            }

            for (int i = 0; i < endMoveSetIndex - startMoveSetIndex; i++)
            {
                moves.Add(startMoveSetIndex + i);
            }

            foreach (int move in moves)
            {
                var currentPosition = square.Position;
                int pieceReach = piece.FindReach(currentPosition, piece.ColorId);
                for (int i = 0; i < pieceReach; i++)
                {
                    int newX = currentPosition.X + moveSet[move, 0],
                        newY = currentPosition.Y + moveSet[move, 1];

                    currentPosition = new Position(newX, newY);
                    if (!Position.IsValid(currentPosition))
                    {
                        break;
                    }
                    int moveToIndex = CTI(currentPosition.X, currentPosition.Y);

                    Piece? currectPositionPiece = board.Board[moveToIndex].Piece;
                    if (currectPositionPiece != null && currectPositionPiece.ColorId == piece.ColorId)
                    {
                        break;
                    }

                    if (!(currectPositionPiece != null && piece.GetType() == typeof(Pawn) && move < 2))
                        results.Add(currentPosition);

                    if (currectPositionPiece != null)
                    {
                        break;
                    }
                }
            }
            return results;
        }
        public static List<List<Piece>> GetDrawableCombinations()
        {
            return [
                [new Knight(1), new Knight(1), new King(1), new King(0)],
                [new Knight(0), new Knight(0), new King(0), new King(1)],
                [new King(0), new King(1)],
                [new Knight(0), new King(0), new King(1)],
                [new Knight(1), new King(0), new King(1)]];
        }
        public static int CTI(int x, int y)
        {
            return 8 * (x - 1) + y - 1;
        }
        public static (int, int) ITC(int index)
        {
            return ((index / 8) + 1, (index % 8) + 1); 
        }
    }
}