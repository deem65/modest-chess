namespace ChessGameLogic
{
    /// <summary>
    /// a square object holds the position it's at and which piece is holds. if it's empty then piece = null, therefore Piece is nullable.
    /// </summary>
    public class Square
    {
        public Position Position { get; set; }
        public Piece? Piece { get; set; }
        public int Color { get; set; }
        public Square(Position position, Piece? piece, int color)
        {
            Position = position;
            Piece = piece;
            Color = color;
        }
        /// <summary>
        /// used to deep copy a board
        /// </summary>
        public Square(Square square)
        {
            Position = new Position(square.Position.X, square.Position.Y);
            Color = square.Color;
            if (square.Piece != null)
            {
                int colorId = square.Piece.ColorId;
                Piece = square.Piece switch
                {
                    King => new King(colorId),
                    Queen => new Queen(colorId),
                    Rook => new Rook(colorId),
                    Bishop => new Bishop(colorId),
                    Knight => new Knight(colorId),
                    Pawn => new Pawn(colorId),
                    _ => null
                };
            }
            else
                Piece = null;           
        }
        /// <summary>
        /// a white queen would be white id (0) plus queen id (1) so 01. very temporary for console testing
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string id, pcColor;
            if (Piece != null)
            {
                id = Piece.Id.ToString();
                pcColor = Piece.ColorId.ToString();
            }
            else
            {
                id = "-";
                pcColor = "-";
            }
            string sqColor = Color.ToString();
            return $"{id}{pcColor}{sqColor}";
        }        
    }
}
