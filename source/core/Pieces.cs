using CoreLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLogic
{
    /// <summary>
    /// an abtract class which holds all infomation about a piece. 
    /// </summary>
    public abstract class Piece
    {
        /// <summary>
        /// StartIndex is the start index of the moveSet double dimensional array in the GameUtil.FindMoves() method.
        /// </summary>
        public int StartIndex { get; set; }
        /// <summary>
        /// EndIndex is the end index of the moveSet double dimensional array in the GameUtil.FindMoves() method.
        /// </summary>
        public int EndIndex { get; set; }
        /// <summary>
        /// ColorId = 0 for white, 1 for black
        /// </summary>
        public int ColorId { get; set; }
        /// <summary>
        /// Id from 0 to 5 inc. - King, Queen, Rook, Bishop, Knight, Pawn
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Returns reach of a piece
        /// </summary>
        public int FindReach(Position position, int colorId)
        {
            return GetType() switch
            {
                Type t when t == typeof(King) || t == typeof(Knight) => 1,
                Type t when t == typeof(Queen) || t == typeof(Rook) || t == typeof(Bishop) => 7,
                Type t when t == typeof(Pawn) => (colorId == 0 && position.X == 2) || (colorId == 1 && position.X == 7) ? 2 : 1,
                _ => 0,
            };
        }

    }
    public class King : Piece
    {
        public King(int colorId)
        {
            Id = 0;
            StartIndex = 0;
            EndIndex = 8;
            ColorId = colorId;
        }
    }
    public class Queen : Piece
    {
        public Queen(int colorId)
        {
            Id = 1;
            StartIndex = 0;
            EndIndex = 8;
            ColorId = colorId;
        }
    }
    public class Rook : Piece
    {
        public Rook(int colorId)
        {
            Id = 2;
            StartIndex = 0;
            EndIndex = 4;
            ColorId = colorId;
        }
    }
    public class Bishop : Piece
    {
        public Bishop(int colorId)
        {
            Id = 3;
            StartIndex = 4;
            EndIndex = 8;
            ColorId = colorId;
        }
    }
    public class Knight : Piece
    {
        public Knight(int colorId)
        {
            Id = 4;
            StartIndex = 8;
            EndIndex = 16;
            ColorId = colorId;
        }
    }
    public class Pawn : Piece
    {
        public Pawn(int colorId)
        {
            Id = 5;
            StartIndex = colorId == 0 ? 0 : 1;
            EndIndex = StartIndex + 1;
            ColorId = colorId;
        }
    }
}
