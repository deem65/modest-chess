using CoreLogic;
using Client.Properties;
using static CoreLogic.GameUtil;
using static Microsoft.VisualBasic.Interaction;

namespace Client
{
    public partial class Chess : Form
    {
        private BoardModel BoardModel { get; set; }
        private Button[]? Squares { get; set; } = null;
        private bool IsSelecting { get; set; }
        private bool IsFirst { get; set; } = true;
        private Position MovingFrom { get; set; }
        private List<Position>? TPossibleMoves { get; set; }
        public Chess()
        {
            InitializeComponent();
        }
        private static int GetTypeIndex(Type pieceType)
        {
            return pieceType switch
            {
                Type t when t == typeof(King) => 0,
                Type t when t == typeof(Queen) => 1,
                Type t when t == typeof(Rook) => 2,
                Type t when t == typeof(Bishop) => 3,
                Type t when t == typeof(Knight) => 4,
                Type t when t == typeof(Pawn) => 5,
                _ => -1,
            };
        }

        private void Sq_Click(object sender, EventArgs e)
        {
            if (sender is not Button clickedButton) return;
            int squareIndex = int.Parse(clickedButton.Name[2..]);
            (int x, int y) coordinates = IndexToCoords(squareIndex);
            int x = coordinates.x, y = coordinates.y;
            if (IsSelecting)
            {
                PossibleMoves.Text = string.Empty;

                string? pieceChar = BoardModel.Board[squareIndex].Piece?.GetType().Name;

                PossibleMoves.Text += $"Possible moves for {Environment.NewLine}{pieceChar}: {new Position(x, y)}:{Environment.NewLine}{Environment.NewLine}";
                if (BoardModel.Board[squareIndex].Piece != null)
                {
                    TPossibleMoves = FindMoves(BoardModel, BoardModel.Board[squareIndex]);
                    foreach (Position move in TPossibleMoves)
                    {
                        PossibleMoves.AppendText(move.ToString() + Environment.NewLine);
                    }
                    IsSelecting = false;
                    MovingFrom = new(x, y);
                }
            }
            else
            { 
                Position movingTo = new(x, y);
                bool succeeded = TryMove(BoardModel, MovingFrom, movingTo, false);
                string colorString = BoardModel.Board[CoordsToIndex(x: MovingFrom.X, y: MovingFrom.Y)].Piece.ColorId == 0 ? "White" : "Black";
                if (!succeeded)
                {
                    Log.AppendText(Environment.NewLine + "Could not move, try again");
                    //throw new Exception();
                }
                else
                {
                    TryMove(BoardModel, MovingFrom, movingTo);
                    Log.AppendText($"{Environment.NewLine}{colorString} {MovingFrom} to {new Position(x, y)}");
                    RefreshBoard();
                }
                IsSelecting = true;
            }

        }

        private void RefreshBoard()
        {
            if (Squares != null)
            {
                for (int i = 0; i < Squares.Length; i++)
                {
                    Square? tSquare = BoardModel.Board[i];
                    if (tSquare.Piece != null)
                    {
                        int pieceTypeIndex = GetTypeIndex(tSquare.Piece.GetType());
                        string resourceName = string.Format("_{0}{1}{2}", pieceTypeIndex, tSquare.Piece.ColorId, tSquare.Color);
                        Squares[i].Image = Resources.ResourceManager.GetObject(resourceName) as Bitmap;
                    }
                    else if (tSquare.Color == 0)
                    {
                        Squares[i].Image = Resources.WhiteEmpty;
                    }
                    else if (tSquare.Color == 1)
                    {
                        Squares[i].Image = Resources.BlackEmpty;
                    }
                }
            }
            string[] players = ["White", "Black"];

            for (int player = 0; player <= 1; player++)
            {
                if (GameUtil.IsInCheckmate(player, BoardModel))
                {
                    MessageBox.Show($"{players[player]} in checkmate");
                }
                else if (GameUtil.IsInStalemate(player, BoardModel))
                {
                    MessageBox.Show($"{players[player]} in stalemate");
                }
                else if (GameUtil.IsInCheck(player, BoardModel))
                {
                    MessageBox.Show($"{players[player]} in check");
                }
                else if (GameUtil.IsDraw(BoardModel))
                {
                    MessageBox.Show($"Draw");
                }

            }
        }

        private void ToggleLightButton_Click(object sender, EventArgs e)
        {
            Label[] allIndexLabels =
                [I1Label, I2Label, I3Label, I4Label, I5Label, I6Label, I7Label, I8Label,
                IALabel, IBLabel, ICLabel, IDLabel, IELabel, IFLabel, IGLabel, IHLabel];

            if (BackColor == Color.DarkSlateGray)
            {
                BackColor = Color.LightSeaGreen;
                PossibleMoves.BackColor = Color.LightSeaGreen;
                PossibleMoves.ForeColor = Color.Black;
                Log.BackColor = Color.LightSeaGreen;
                Log.ForeColor = Color.Black;
                foreach (var label in allIndexLabels)
                {
                    label.ForeColor = Color.Black;
                }
            }
            else
            {
                BackColor = Color.DarkSlateGray;
                PossibleMoves.BackColor = Color.DarkSlateGray;
                PossibleMoves.ForeColor = Color.White;
                Log.BackColor = Color.DarkSlateGray;
                Log.ForeColor = Color.White;
                foreach (var label in allIndexLabels)
                {
                    label.ForeColor = Color.White;
                }
            }
        }

        private void NewGameButton_Click(object sender, EventArgs e)
        {

            BoardModel = new();
            IsSelecting = true;
            TPossibleMoves = [];

            Squares = Enumerable.Range(0, 64).Select(selector: i => Controls[$"Sq{i}"] as Button).ToArray();
            if (IsFirst)
            {
                Array.ForEach(Squares, s => s.Click += Sq_Click);
                IsFirst = false;
            }
            RefreshBoard();
            RefreshLogs();
        }
        private void RefreshLogs()
        {
            Log.Clear();
            Log.Text += "Logs:";
            PossibleMoves.Clear();
            PossibleMoves.Text += "Possible moves for:";
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Chess_Load(object sender, EventArgs e)
        {
            NewGameButton_Click(sender, e);
        }

        private void CustomBoardButton_Click(object sender, EventArgs e)
        {
            //try
            {
                string boardInput = InputBox("Enter the board string prompt (no exception handling):\nRULES:\n" +
                "K, Q, R, B, N, P, for all pieces respectfully\n" +
                "0 for white and 1 for black\n" +
                "to input a square, first put the piece char, and then piece color id.\ndivide each piece with '/'.\nif square is empty, enter a number of the consecutive empty squares\n" +
                "example for a white queen then 5 empty squares then a black bishop: Q0/05/B1\nensure the board contains a king from each side.",
                "Custom board creation");
                if (boardInput != null || boardInput == string.Empty)
                {
                    BoardModel = new(boardInput);
                    RefreshBoard();
                }                
            }
            //catch
            //{
            //    MessageBox.Show("i lied, there is exception handling. just not a good one. try again");
            //}
        }
    }
}

