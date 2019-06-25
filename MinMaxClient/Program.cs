using System;
using System.Collections.Generic;
using ChessHost;
using ChessClient;
using ChessHost.Pieces;
using MovePair = System.Tuple<System.Tuple<int, int>, System.Tuple<int, int>>;
using ScorePair = System.Tuple<int, System.Tuple<System.Tuple<int, int>, System.Tuple<int, int>>>;


namespace MinMaxClient
{
    class Program
    {
        public class SmartClient : Client
        {
            private static readonly Dictionary<ChessPieceType, int> _pieceValues = new Dictionary<ChessPieceType, int>
            {
                //The following table is the most common assignment of point values
                //(Capablanca & de Firmian 2006:24–25), (Seirawan & Silman 1990:40),
                //(Soltis 2004:6), (Silman 1998:340), (Polgar & Truong 2005:11).
                [ChessPieceType.King] = 1000,
                [ChessPieceType.Queen] = 9,
                [ChessPieceType.Rook] = 5,
                [ChessPieceType.Bishop] = 3,
                [ChessPieceType.Knight] = 3,
                [ChessPieceType.Pawn] = 1,
            };

            public static int GetPieceScore(ChessPiece chessPiece, Player currentPlayer)
            {
                if (chessPiece.GetPlayer() == currentPlayer)
                    return _pieceValues[chessPiece.GetPieceType()];
                return -_pieceValues[chessPiece.GetPieceType()];
            }

            public static int CountScore(ChessBoard cb, Player player=Player.White)
            {
                int i = 0;
                foreach (var piece in cb.Pieces)
                    i += GetPieceScore(piece.Value, player);

                return i;
            }

            public static ScorePair AlphaBetaNegaMax(ChessBoard chessBoard, int alpha, int beta, int depth)
            {
                Serializator ser = new Serializator();

                if (depth == 0)
                    return new ScorePair(CountScore(chessBoard), null);

                MovePair bestMove=null;

                List<Tuple<ChessPiece, Tuple<int, int>>> allMovesThisTurn = chessBoard
                    .GetAllPossibleMoves()
                    .FindAll(chessPieceNMoves => chessPieceNMoves.Item1.GetPlayer() == chessBoard.CurrentPlayer);

                foreach (var move in allMovesThisTurn)
                {
                    ChessBoard chessBoardCopy = ser.DeepCopy(chessBoard);
                    var thisMove = new MovePair(move.Item1.Position, move.Item2);

                    if (chessBoardCopy.MovePiece(thisMove.Item1, thisMove.Item2))
                    {
                        int score = -AlphaBetaNegaMax(chessBoardCopy, -beta,-alpha, depth - 1).Item1;
                        Console.WriteLine("beta {0} , score {1}, alpha {2}", beta,score,alpha);
                        if (score >= beta)
                            return new ScorePair(beta, bestMove);
                        if (score > alpha)
                            bestMove = thisMove;
                    }
                }

                return new ScorePair(alpha,bestMove);
            }


            public static ScorePair alphaBetaMax(ChessBoard chessBoard, int alpha, int beta, int depth)
            {
                if (depth == 0) return new ScorePair(CountScore(chessBoard, chessBoard.CurrentPlayer),null);
                Serializator ser = new Serializator();
                List<Tuple<ChessPiece, Tuple<int, int>>> allMovesThisTurn = chessBoard
                    .GetAllPossibleMoves()
                    .FindAll(chessPieceNMoves => chessPieceNMoves.Item1.GetPlayer() == chessBoard.CurrentPlayer);

                MovePair bestMove = null;

                foreach (var move in allMovesThisTurn)
                {
                    ChessBoard chessBoardCopy = ser.DeepCopy(chessBoard);
                    var thisMove = new MovePair(move.Item1.Position, move.Item2);
                    if (chessBoardCopy.MovePiece(thisMove.Item1, thisMove.Item2))
                    {
                        var score = alphaBetaMin(chessBoardCopy, alpha, beta, depth - 1).Item1;
                        if (score >= beta)
                            return new ScorePair(beta,null);   // fail hard beta-cutoff
                        if (score > alpha)
                        {
                            alpha = score; // alpha acts like max in MiniMax
                            bestMove = thisMove;
                        }
                    }
                }
                return new ScorePair(alpha, bestMove);
            }

            public static ScorePair alphaBetaMin(ChessBoard chessBoard, int alpha, int beta, int depth)
            {
                if (depth == 0) return new ScorePair(-CountScore(chessBoard, chessBoard.CurrentPlayer), null);
                Serializator ser = new Serializator();
                List<Tuple<ChessPiece, Tuple<int, int>>> allMovesThisTurn = chessBoard
                    .GetAllPossibleMoves()
                    .FindAll(chessPieceNMoves => chessPieceNMoves.Item1.GetPlayer() == chessBoard.CurrentPlayer);

                MovePair bestMove = null;

                foreach (var move in allMovesThisTurn)
                {
                    ChessBoard chessBoardCopy = ser.DeepCopy(chessBoard);
                    var thisMove = new MovePair(move.Item1.Position, move.Item2);
                    if (chessBoardCopy.MovePiece(thisMove.Item1, thisMove.Item2))
                    {
                        var score = alphaBetaMax(chessBoardCopy, alpha, beta, depth - 1).Item1;
                        if (score <= alpha)
                            return new ScorePair(alpha, null);   // fail hard alpha-cutoff
                        if (score < beta)
                        {
                            beta = score;
                            bestMove = thisMove;
                        }
                    }
                }
                return new ScorePair(beta, bestMove);
            }


            public static ScorePair NegaMax(ChessBoard chessBoard, int depth,
                MovePair lastMove = null)
            {
                Serializator ser = new Serializator();

                if (depth == 0)
                    return new Tuple<int, MovePair>(
                        CountScore(chessBoard, chessBoard.CurrentPlayer), lastMove);

                ScorePair maxS = new ScorePair(Int32.MinValue, null);

                List<Tuple<ChessPiece, Tuple<int, int>>> allMovesThisTurn = chessBoard
                    .GetAllPossibleMoves()
                    .FindAll(chessPieceNMoves => chessPieceNMoves.Item1.GetPlayer() == chessBoard.CurrentPlayer);

                foreach (var move in allMovesThisTurn)
                {
                    ChessBoard chessBoardCopy = ser.DeepCopy(chessBoard);
                    var thisMove = new MovePair(move.Item1.Position, move.Item2);

                    if (chessBoardCopy.MovePiece(thisMove.Item1, thisMove.Item2))
                    {
                        int score = -NegaMax(chessBoardCopy, depth - 1, thisMove).Item1;
                        if (score > maxS.Item1)
                            maxS = new ScorePair(score, thisMove);
                    }
                }

                return maxS;
            }

            private static string TransformMove(ChessBoard cb, MovePair bestMove)
            {
                return cb.TransformPosition(bestMove.Item1) + " " + cb.TransformPosition(bestMove.Item2);
            }

            public static string GetBestMove(ChessBoard cb, int depth)
            {
                MovePair bestMove = alphaBetaMax(cb, Int32.MinValue, Int32.MaxValue,  depth).Item2;
//                MovePair bestMove = NegaMax(chessBoard, depth).Item2;

                string move = TransformMove(cb, bestMove);
                Console.WriteLine(move);
                return move;
            }

            public SmartClient(string host, int port, int depth) : base(host, port,
                (cb) => GetBestMove(cb, depth))
            {
            }
        }


        static void Main(string[] args)
        {
            Client smartClient = new SmartClient("localhost", 3000, 3);
            smartClient.GameLoop();
        }
    }
}