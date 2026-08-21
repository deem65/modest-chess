namespace ChessGameLogic
{
    /// <summary>
    /// A class which holds all infomation about a board
    /// </summary>
    public class BoardModel
    {
        public Square[] Board { get; set; } = new Square[64];
        private static int Color { get; set; } = 1;
        public BoardModel()
        {
            ConstructDefaultBoard();
        }
        public BoardModel(string boardString)
        {
            LinkedList<string> squares = new ([.. boardString.Split('/')]);
            LinkedListNode<string> currentNode = squares.First;
            Position pos = new(1, 0);            
            int i = 0;
            string square;
            while (currentNode != null)
            {
                pos = GameUtil.NextPosition(pos);
                square = currentNode.Value;
                char piece = square.ToUpper()[0];
                if (piece >= 'A' && piece <= 'Z')
                {
                    ProcessPieceString(ref pos, ref i, square);
                }
                else
                {
                    ProcessNumberString(ref pos, ref i, square);
                }
                i++;
                currentNode = currentNode.Next;            
            }

            

            
        }
        void ProcessNumberString(ref Position pos, ref int i, string square)
        {
            int skippedLoops = Convert.ToInt32(square);
            for (int ח = GameUtil.CoordsToIndex(pos.X, pos.Y); ח < GameUtil.CoordsToIndex(pos.X, pos.Y) + skippedLoops- 1; ח++)
            {
                if (i % 8 != 0)
                {
                    Color = Color == 0 ? 1 : 0;
                }               
                Board[i] = new Square(pos, null, Color);
                pos = GameUtil.NextPosition(pos);
            }
            i += skippedLoops - 1;
        }
        void ProcessPieceString(ref Position pos, ref int i, string square)
        {
            if (i % 8 != 0)
            {
                Color = Color == 0 ? 1 : 0;
            }
            Board[i] = new Square(pos, FindPieceType(square), Color);
        }
        private static Piece? FindPieceType(string s)
        {
            int c = s[1] - 48;
            return s[0].ToString().ToUpper() switch
            {
                "K" => new King(c),
                "Q" => new Queen(c),
                "R" => new Rook(c),
                "B" => new Bishop(c),
                "N" => new Knight(c),
                "P" => new Pawn(c),
                _ => null,
            };
        }        
        private void ConstructDefaultBoard()
        {
            Type[] piecesObjects = [typeof(Rook), typeof(Knight), typeof(Bishop), typeof(Queen), typeof(King), typeof(Bishop), typeof(Knight), typeof(Rook)];

            Position pos = new(1, 0);

            pos = AddRow(pos, 0, 0, piecesObjects);
            pos = AddPawns(pos, 8, 16, 0);
            pos = AddEmptySpace(pos, 16, 48);
            pos = AddPawns(pos, 48, 56, 1);
            AddRow(pos, 56, 1, piecesObjects);
        }


        private Position AddEmptySpace(Position pos, int startI, int endI)
        {            
            for (int i = startI; i < endI; i++)
            {
                if (i % 8 != 0)
                {
                    Color = Color == 0 ? 1 : 0;
                }
                pos = GameUtil.NextPosition(pos);
                Board[i] = new Square(pos, null, Color);
            }
            return pos;
        }
        private Position AddPawns(Position pos, int startI, int endI, int colorId)
        {
            for (int i = startI; i < endI; i++)
            {
                if (i % 8 != 0)
                {
                    Color = Color == 0 ? 1 : 0;
                }
                pos = GameUtil.NextPosition(pos);
                Board[i] = new Square(pos, new Pawn(colorId), Color);
            }
            return pos;
        }
        private Position AddRow(Position pos, int i, int colorId, params Type[] pieceTypes)
        {
            foreach (Type type in pieceTypes)
            {
                if (i % 8 != 0)
                {
                    Color = Color == 0 ? 1 : 0;
                }
                pos = GameUtil.NextPosition(pos);
                Board[i++] = new Square(pos, (Piece)Activator.CreateInstance(type, colorId), Color);
            }
            return pos;
        }
        /// <summary>
        /// temporary, just for console.
        /// </summary>
        public override string ToString()
        {
            string final;
            final = "  ";
            for (int i = 0; i < 8; i++)
            {
                final += "-" + (i + 1) + " ";
            }
            for (int i = 0; i < Board.Length; i++)
            {
                if (i % 8 == 0)
                {
                    final += "\n" + (i / 8 + 1) + " ";
                }
                final += Board[i].ToString() + " ";
            }
            return final;
        }

    }
}