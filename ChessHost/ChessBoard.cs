using System;
using System.Collections.Generic;
using ChessHost.Pieces;

namespace ChessHost
{
    public class ChessBoard
    {
        private Player _CurrentPlayer = Player.White;

        public Dictionary<Tuple<int, int>, ChessPiece> Pieces = new Dictionary<Tuple<int, int>, ChessPiece>();

        public ChessBoard()
        {
            for (int i = 0; i < 8; i++)
            {
                Pieces.Add(new Tuple<int, int>(6, i), new Pawn(new Tuple<int, int>(6, i), Player.White));
                Pieces.Add(new Tuple<int, int>(1, i), new Pawn(new Tuple<int, int>(1, i), Player.Black));
            }

            Pieces.Add(new Tuple<int, int>(7, 0), new Rook(new Tuple<int, int>(7, 0), Player.White));
            Pieces.Add(new Tuple<int, int>(7, 1), new Knight(new Tuple<int, int>(7, 1), Player.White));
            Pieces.Add(new Tuple<int, int>(7, 2), new Bishop(new Tuple<int, int>(7, 2), Player.White));
            Pieces.Add(new Tuple<int, int>(7, 3), new Queen(new Tuple<int, int>(7, 3), Player.White));
            Pieces.Add(new Tuple<int, int>(7, 4), new King(new Tuple<int, int>(7, 4), Player.White));
            Pieces.Add(new Tuple<int, int>(7, 5), new Bishop(new Tuple<int, int>(7, 5), Player.White));
            Pieces.Add(new Tuple<int, int>(7, 6), new Knight(new Tuple<int, int>(7, 6), Player.White));
            Pieces.Add(new Tuple<int, int>(7, 7), new Rook(new Tuple<int, int>(7, 7), Player.White));
//
            Pieces.Add(new Tuple<int, int>(0, 0), new Rook(new Tuple<int, int>(0, 0), Player.Black));
            Pieces.Add(new Tuple<int, int>(0, 1), new Knight(new Tuple<int, int>(0, 1), Player.Black));
            Pieces.Add(new Tuple<int, int>(0, 2), new Bishop(new Tuple<int, int>(0, 2), Player.Black));
            Pieces.Add(new Tuple<int, int>(0, 3), new Queen(new Tuple<int, int>(0, 3), Player.Black));
            Pieces.Add(new Tuple<int, int>(0, 4), new King(new Tuple<int, int>(0, 4), Player.Black));
            Pieces.Add(new Tuple<int, int>(0, 5), new Bishop(new Tuple<int, int>(0, 5), Player.Black));
            Pieces.Add(new Tuple<int, int>(0, 6), new Knight(new Tuple<int, int>(0, 6), Player.Black));
            Pieces.Add(new Tuple<int, int>(0, 7), new Rook(new Tuple<int, int>(0, 7), Player.Black));
        }

        private readonly Dictionary<char, int> _positionCodes = new Dictionary<char, int>
        {
            ['A'] = 0,
            ['B'] = 1,
            ['C'] = 2,
            ['D'] = 3,
            ['E'] = 4,
            ['F'] = 5,
            ['G'] = 6,
            ['H'] = 7,
        };
        private readonly List<char> _positionValues = new List<char>{'A', 'B', 'C', 'D', 'E', 'F', 'G','H'};

        public string TransformPosition(Tuple<int, int> pos)
        {
            return _positionValues[pos.Item2] + (8 - pos.Item1).ToString();
        }

        public Tuple<int, int> TransformPosition(string pos)
        {
            int x, y;
            if (int.TryParse(pos[1].ToString(), out y))
                x = _positionCodes[pos[0]];
            else if (int.TryParse(pos[0].ToString(), out y))
                x = _positionCodes[pos[1]];
            else return null;

            y = 8 - y;
            return new Tuple<int, int>(y, x);
        }

        public void MovePiece(string move)
        {
            var positions = move.Split(' ');
            if (positions.Length == 2 && positions[0].Length == 2 && positions[1].Length == 2)
            {
                Tuple<int, int> pStart = TransformPosition(positions[0]);
                Tuple<int, int> pEnd = TransformPosition(positions[1]);

                MovePiece(pStart, pEnd);
            }
        }

        public void MovePiece(Tuple<int, int> pStart, Tuple<int, int> pEnd)
        {
            if (Pieces.TryGetValue(pStart, out ChessPiece piece)) // check if piece even exists
            {
                if (piece.GetPlayer() == _CurrentPlayer) // compare the piece with player
                {
                    // TODO: add check if check
                    if (piece.MovePiece(pEnd, this)) // validates move, updates piece parameters if possible
                    {
//                        if (piece.GetType() == typeof(King)) // castling
//                            ((King)piece).Castling = new Tuple<bool, bool>(false, false);

                        Pieces[pEnd] = Pieces[pStart];
                        Pieces.Remove(pStart);
                        _CurrentPlayer = (_CurrentPlayer == Player.White) ? Player.Black : Player.White;
                    }
                }
            }
        }

        public ChessPiece GetPieceInPosition(Tuple<int, int> pos)
        {
            Pieces.TryGetValue(pos, out ChessPiece piece);
            return piece;
        }

        private void PrintPiece(Tuple<int, int> pos)
        {
            if (Pieces.TryGetValue(pos, out ChessPiece piece))
            {
                ChessPieceType type = piece.GetPieceType();

                Console.Write("| {0}", piece.GetPlayer() == Player.Black ? "B" : "W");

                switch (type)
                {
                    case ChessPieceType.Bishop:
                        Console.Write("B ");
                        break;
                    case ChessPieceType.King:
                        Console.Write("K ");
                        break;
                    case ChessPieceType.Queen:
                        Console.Write("Q ");
                        break;
                    case ChessPieceType.Rook:
                        Console.Write("R ");
                        break;
                    case ChessPieceType.Knight:
                        Console.Write("N ");
                        break;
                    case ChessPieceType.Pawn:
                        Console.Write("P ");
                        break;
                }
            }
            else Console.Write("|    ");
        }

        public void PrintBoard(Player player = Player.White)
        {
            bool playerIsWhite = (player == Player.White);

            if (playerIsWhite)
            {
                foreach (char label in _positionValues) Console.Write("    {0}", label);
                Console.WriteLine("");
                for (int i = 0; i < 8; i++)
                {
                    Console.WriteLine("  -----------------------------------------");
                    Console.Write("{0} ", 8 - i);
                    for (int j = 0; j < 8; j++) PrintPiece(new Tuple<int, int>(i, j));
                    Console.WriteLine("|");
                }
            }
            else
            {
                for (int i = _positionValues.Count - 1; i >= 0; i--)
                    Console.Write("    {0}", _positionValues[i]);

                Console.WriteLine("");
                for (int i = 7; i >= 0; i--)
                {
                    Console.WriteLine("  -----------------------------------------");
                    Console.Write("{0} ", 8 - i);
                    for (int j = 7; j >= 0; j--) PrintPiece(new Tuple<int, int>(i, j));
                    Console.WriteLine("|");
                }
            }

            Console.WriteLine("  -----------------------------------------");
        }
    }
}