using System.Diagnostics.CodeAnalysis;

namespace CoreLogic
{
    /// <summary>
    /// A struct to hold infomation about a Position for example (1, 1). Board[0] would be (1, 1), and Board[63] would be (8, 8)
    ///
    /// NOTE - I made a mistake and the X axis number shows up as the Y axis in Console and vice versa, and this needs work in the UI
    /// </summary>
    public struct Position(int x, int y)
    {
        public int X { get; set; } = x;
        public int Y { get; set; } = y;

        /// <summary>
        /// checks if position is out of bounds
        /// </summary>
        public static bool IsValid(Position position)
        {
            return !(position.X > 8 || position.X < 1 || position.Y > 8 || position.Y < 1);
        }
        public override readonly string ToString()
        {
            char YChar = 'n';
            switch (Y)
            {
                case 1:
                    YChar = 'a';
                    break;
                case 2:
                    YChar = 'b';
                    break;
                case 3:
                    YChar = 'c';
                    break;
                case 4:
                    YChar = 'd';
                    break;
                case 5:
                    YChar = 'e';
                    break;
                case 6:
                    YChar = 'f';
                    break;
                case 7:
                    YChar = 'g';
                    break;
                case 8:
                    YChar = 'h';
                    break;
                default:
                    break;
            }
            return $"({YChar}, {X})"; //I made a mistake dw about it
        }
    }
}
