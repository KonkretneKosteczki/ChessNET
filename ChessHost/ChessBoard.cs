using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using ChessHost.Pieces;

namespace ChessHost
{
    [DataContract]
    public class ChessBoard
    {
        [DataMember] private Player _currentPlayer = Player.White;

        [DataMember]
        public Dictionary<Tuple<int, int>, ChessPiece> Pieces = new Dictionary<Tuple<int, int>, ChessPiece>();

        public List<Tuple<ChessPiece, Tuple<int, int>>> GetAllPossibleMoves()
        {
            List<Tuple<ChessPiece, Tuple<int, int>>> allMoves =
                new List<Tuple<ChessPiece, Tuple<int, int>>>();

            foreach (KeyValuePair<Tuple<int, int>, ChessPiece> pieceObject in Pieces)
            {
                foreach (var possibleMove in pieceObject.Value.GetPossibleMoves(this))
                {
                    allMoves.Add(new Tuple<ChessPiece, Tuple<int, int>>(
                        pieceObject.Value, possibleMove
                    ));
                }
            }

            return allMoves;
        }

        public Player CurrentPlayer
        {
            get => _currentPlayer;
            set => _currentPlayer = value;
        }

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
            Pieces.Add(new Tuple<int, int>(7, 5), new Bishop(new Tuple<int, int>(7, 5), Player.White));
            Pieces.Add(new Tuple<int, int>(7, 6), new Knight(new Tuple<int, int>(7, 6), Player.White));
            Pieces.Add(new Tuple<int, int>(7, 7), new Rook(new Tuple<int, int>(7, 7), Player.White));
            Pieces.Add(new Tuple<int, int>(7, 4), new King(new Tuple<int, int>(7, 4), Player.White));

            Pieces.Add(new Tuple<int, int>(0, 0), new Rook(new Tuple<int, int>(0, 0), Player.Black));
            Pieces.Add(new Tuple<int, int>(0, 1), new Knight(new Tuple<int, int>(0, 1), Player.Black));
            Pieces.Add(new Tuple<int, int>(0, 2), new Bishop(new Tuple<int, int>(0, 2), Player.Black));
            Pieces.Add(new Tuple<int, int>(0, 3), new Queen(new Tuple<int, int>(0, 3), Player.Black));
            Pieces.Add(new Tuple<int, int>(0, 5), new Bishop(new Tuple<int, int>(0, 5), Player.Black));
            Pieces.Add(new Tuple<int, int>(0, 6), new Knight(new Tuple<int, int>(0, 6), Player.Black));
            Pieces.Add(new Tuple<int, int>(0, 7), new Rook(new Tuple<int, int>(0, 7), Player.Black));
            Pieces.Add(new Tuple<int, int>(0, 4), new King(new Tuple<int, int>(0, 4), Player.Black));
        }

        [DataMember] private readonly Dictionary<char, int> _positionCodes = new Dictionary<char, int>
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

        [DataMember]
        private readonly List<char> _positionValues = new List<char> {'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H'};

        public Tuple<int, int> TransformPosition(string pos)
        {
            int positionX;
            if (int.TryParse(pos[1].ToString(), out int positionY))
                positionX = _positionCodes[pos[0]];
            else return null;

            positionY = 8 - positionY;
            return new Tuple<int, int>(positionY, positionX);
        }

        public string TransformPosition(Tuple<int, int> pos)
        {
            return _positionValues[pos.Item2] + (8 - pos.Item1).ToString();
        }


        public bool MovePiece(string move)
        {
            Regex rx = new Regex(@"[A-H][1-8]");
            MatchCollection positions = rx.Matches(move);

            if (positions.Count == 2)
            {
                Tuple<int, int> pStart = TransformPosition(positions[0].Value);
                Tuple<int, int> pEnd = TransformPosition(positions[1].Value);

                return MovePiece(pStart, pEnd);
            }

            return false;
        }

        public bool MovePiece(Tuple<int, int> pStart, Tuple<int, int> pEnd, bool validateKingExposure = true)
        {
            if (pStart != null && Pieces.TryGetValue(pStart, out ChessPiece piece)) // check if piece even exists
            {
                if (piece.GetPlayer() == _currentPlayer) // compare the piece with player
                {
                    if (validateKingExposure)
                    {
                        // create and perform operations on deep copy for validation purposes

                        Serializator ser = new Serializator();
                        ChessPiece chessPieceCopy = ser.DeepCopy(piece);
                        ChessBoard chessBoardCopy = ser.DeepCopy(this);

                        if (chessPieceCopy.MovePiece(pEnd, chessBoardCopy))
                        {
                            chessBoardCopy.Pieces[pEnd] = Pieces[pStart];
                            chessBoardCopy.Pieces.Remove(pStart);

                            foreach (var movablePiece in chessBoardCopy.GetAllPossibleMoves())
                                if (movablePiece.Item1.GetPlayer() != _currentPlayer) //only enemy moves
                                {
                                    ChessPiece cpInPosition = chessBoardCopy.GetPieceInPosition(movablePiece.Item2);
                                    if (cpInPosition != null && cpInPosition.GetType() == typeof(King))
                                        // if any piece would end on king ban the move
                                        return false;
                                }
                        }
                        else return false;
                    }

                    if (piece.MovePiece(pEnd, this)) // validates move, updates piece parameters if possible
                    {
                        Pieces[pEnd] = Pieces[pStart];
                        Pieces.Remove(pStart);
                        _currentPlayer = (_currentPlayer == Player.White) ? Player.Black : Player.White;

                        // PAWN PROMOTION
                        if (piece.GetType() == typeof(Pawn) && (pEnd.Item1 == 7 || pEnd.Item1 == 0))
                            Pieces[pEnd] = new Queen(pEnd, piece.GetPlayer());

                        // CASTLING
                        if (piece.GetPieceType() == ChessPieceType.Rook)
                        {
                            // try to get king and update its castling
                            // no need for action if king not in default position (moved before)
                            Rook rook = (Rook) piece;
                            if (rook.CanCastle)
                            {
                                ChessPiece originalKingPos = GetPieceInPosition(new Tuple<int, int>(pStart.Item1, 4));
                                if (originalKingPos != null && originalKingPos.GetPieceType() == ChessPieceType.King)
                                {
                                    King king = (King) originalKingPos;

                                    // can use rook's starting position to define if its left or right because
                                    // if it moved in the past (ex. right rook moved to leftmost space)
                                    // still rook.CanCastle == false so would not even enter here
                                    king.Castling = (pStart.Item2 == 0)
                                        ? new Tuple<bool, bool>(false, king.Castling.Item2)
                                        : new Tuple<bool, bool>(king.Castling.Item1, false);
                                    rook.CanCastle = false;
                                }
                            }
                        }

                        if (piece.GetType() == typeof(King))
                        {
                            ((King) piece).Castling = new Tuple<bool, bool>(false, false);
                            // king moved by 2 spaces so castling
                            if (pStart.Item2 - pEnd.Item2 == -2) // move to the right
                            {
                                Tuple<int, int> rookStart = new Tuple<int, int>(pEnd.Item1, 7),
                                    rookEnd = new Tuple<int, int>(pEnd.Item1, 5);
                                GetPieceInPosition(rookStart).Position = rookEnd;

                                Pieces[rookEnd] = Pieces[rookStart];
                                Pieces.Remove(rookStart);
                            }
                            else if (pStart.Item2 - pEnd.Item2 == 2)
                            {
                                Tuple<int, int> rookStart = new Tuple<int, int>(pEnd.Item1, 0),
                                    rookEnd = new Tuple<int, int>(pEnd.Item1, 3);
                                GetPieceInPosition(rookStart).Position = rookEnd;

                                Pieces[rookEnd] = Pieces[rookStart];
                                Pieces.Remove(rookStart);
                            }
                        }

                        return true;
                    }
                }
            }

            return false;
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
            Console.Clear();
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