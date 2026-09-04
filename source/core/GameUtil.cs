using System.Linq;

namespace ChessGameLogic
{
    /// <summary>
    /// static class which has all methods to work with a board, it can check for checkmate, stalemate, checks, draws, move pieces, check is move is valid, find possible moves, and more.
    /// </summary>
    public static class GameUtil
    {
        private static readonly Dictionary<int, int> pawnlessMoves = new()
        {
            { 0, 0 }, // for white pieces
            { 1, 0 }  // for black pieces
        };

        //Add these values to X and Y to move: Up, Down, Right, Left, UpRight, UpLeft, DownRight, DownLeft, and all 8 knight moves...
        private static readonly int[,] moveSet = {
                { 1, 0 }, { -1, 0 }, { 0, 1 },
                { 0, -1 }, { 1, 1 }, { 1, -1 },
                { -1, 1 }, { -1, -1 }, { 2, 1 },
                { 1, 2 }, { -1, 2 }, { -2, 1 },
                { -2, -1 }, { -1, -2 }, { 1, -2 },
                { 2, -1 } };
        public static bool IsDraw(BoardModel board)
        {
            List<List<Piece>> draws = GetDrawableCombinations();
            List<Piece> activePieces = board.Board.Where(sq => sq.Piece != null)
                           .Select(sq => sq.Piece).ToList();
            _ = activePieces.OrderBy(p => p.Id);
            bool res = false;
            foreach (var draw in draws) 
            {
                bool any = true;
                _ = draw.OrderBy(p => p.Id);
                if (draw.Count != activePieces.Count)
                {
                    any = false;
                }
                else
                {
                    int l = draw.Count;
                    for (int i = 0; i < l; i++)
                    {
                        if (draw[i].GetType() != activePieces[i].GetType() && draw[i].ColorId != activePieces[i].ColorId)
                        {
                            any = false;
                        }
                    }
                }
                if (any)
                    res = true;
            }
            return res;            
        }
        public static bool IsInCheckmate(int colorId, BoardModel testModel)
        {
            return IsInStalemate(colorId, testModel) && IsInCheck(colorId, testModel);
        }
        public static bool IsInStalemate(int colorId, BoardModel testModel)
        {
            return testModel.Board
                .Where(sq => sq.Piece != null && sq.Piece.ColorId == colorId)
                .All(sq => !FindMoves(testModel, sq)
                .Any(move => TryMove(testModel, sq.Position, move, false)));
        }
        public static bool IsInCheck(int colorId, BoardModel testModel)
        {
            Position kingLocation = testModel.Board
                .Where(sq => sq.Piece != null && sq.Piece.ColorId == colorId && sq.Piece.GetType() == typeof(King))
                .Select(sq => sq.Position)
                .FirstOrDefault();

            List<Position> allEnemyPossibleMoves = testModel.Board
                .Where(sq => sq.Piece != null && sq.Piece.ColorId != colorId)
                .SelectMany(sq => FindMoves(testModel, sq)).ToList();

            return allEnemyPossibleMoves.Contains(kingLocation);
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
        /// <summary>
        /// tries and moves two pieces and returns if it succeeded
        /// </summary>
        /// <param name="toMove">optional parameter - set false if you want for it to only try. by default it's true</param>
        public static bool TryMove(BoardModel board, Position moveFrom, Position moveTo, bool toMove = true)
        {
            int moveFromIndex = CoordsToIndex(moveFrom.X, moveFrom.Y);
            int moveToIndex = CoordsToIndex(moveTo.X, moveTo.Y);

            var possibleMoves = FindMoves(board, board.Board[moveFromIndex]);

            if (!possibleMoves.Contains(moveTo))
            {
                return false;
            }
            int turn = board.Board[moveFromIndex].Piece.ColorId;

            var boardCopy = DeepCopy(board); 
            boardCopy.Board[moveToIndex].Piece = board.Board[moveFromIndex].Piece;
            boardCopy.Board[moveFromIndex].Piece = null;

            if (IsInCheck(turn, boardCopy))
            {
                return false;
            }
            if (toMove)
            {
                if (board.Board[moveToIndex].Piece is Pawn pawn)
                {
                    pawnlessMoves[pawn.ColorId]++;
                }
                board.Board[moveToIndex].Piece = board.Board[moveFromIndex].Piece;
                board.Board[moveFromIndex].Piece = null;
            }
            return true;
        }
        /// <summary>
        /// finds all possible moves for a piece on a chessboard. 
        /// </summary>
        public static List<Position> FindMoves(BoardModel board, Square square)
        {
            var piece = square.Piece;
            if (piece == null)
            {
                return [];
            }
            List<Position> results = [];
            List<int> moves = [];
            

            int startMoveSetIndex = square.Piece.StartIndex,
                endMoveSetIndex = square.Piece.EndIndex;
            if (piece.GetType() == typeof(Pawn))
            {
                bool isPawnWhite = piece.ColorId == 0;

                int t = isPawnWhite ? 8 : -8;

                if (board.Board[CoordsToIndex(square.Position.X, square.Position.Y) + t - 1].Piece != null)
                {
                    moves.Add(isPawnWhite ? 5 : 7);
                }
                if (board.Board[CoordsToIndex(square.Position.X, square.Position.Y) + t + 1].Piece != null)
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
                    int moveToIndex = CoordsToIndex(currentPosition.X, currentPosition.Y);

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
        public static int CoordsToIndex(int x, int y)
        {
            return 8 * (x - 1) + y - 1;
        }
        public static (int, int) IndexToCoords(int index)
        {
            return ((index / 8) + 1, (index % 8) + 1); 
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
    }
}