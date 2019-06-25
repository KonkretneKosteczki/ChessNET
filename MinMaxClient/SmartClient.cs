using System;
using System.Collections.Generic;
using ChessClient;
using ChessHost;
using ChessHost.Pieces;
using MovePair = System.Tuple<System.Tuple<int, int>, System.Tuple<int, int>>;
using ScorePair = System.Tuple<int, System.Tuple<System.Tuple<int, int>, System.Tuple<int, int>>>;


namespace MinMaxClient
{
    public class SmartClient : Client
    {
        private static readonly Dictionary<ChessPieceType, int> PieceValues = new Dictionary<ChessPieceType, int>
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

        private static int GetPieceScore(ChessPiece chessPiece, Player currentPlayer)
        {
            if (chessPiece.GetPlayer() == currentPlayer)
                return PieceValues[chessPiece.GetPieceType()];
            return -PieceValues[chessPiece.GetPieceType()];
        }

        private static int CountScore(ChessBoard cb, Player player = Player.White)
        {
            int i = 0;
            foreach (var piece in cb.Pieces)
                i += GetPieceScore(piece.Value, player);

            return i;
        }

        private static ScorePair AlphaBetaMax(ChessBoard chessBoard, int alpha, int beta, int depth)
        {
            if (depth == 0) return new ScorePair(CountScore(chessBoard, chessBoard.CurrentPlayer), null);
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
                    var score = AlphaBetaMin(chessBoardCopy, alpha, beta, depth - 1).Item1;
                    if (score >= beta)
                        return new ScorePair(beta, null); // fail hard beta-cutoff
                    if (score > alpha)
                    {
                        alpha = score; // alpha acts like max in MiniMax
                        bestMove = thisMove;
                    }
                }
            }

            return new ScorePair(alpha, bestMove);
        }

        private static ScorePair AlphaBetaMin(ChessBoard chessBoard, int alpha, int beta, int depth)
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
                    var score = AlphaBetaMax(chessBoardCopy, alpha, beta, depth - 1).Item1;
                    if (score <= alpha)
                        return new ScorePair(alpha, null); // fail hard alpha-cutoff
                    if (score < beta)
                    {
                        beta = score;
                        bestMove = thisMove;
                    }
                }
            }

            return new ScorePair(beta, bestMove);
        }


        private static ScorePair NegaMax(ChessBoard chessBoard, int depth,
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

        private enum EvaluationAlgorithms
        {
            NegaMax,
            AlphaBetaMinMax
        }

        private static string GetBestMove(ChessBoard chessBoard, int depth,
            EvaluationAlgorithms alg = EvaluationAlgorithms.AlphaBetaMinMax)
        {
            MovePair bestMove = (alg == EvaluationAlgorithms.AlphaBetaMinMax)
                ? AlphaBetaMax(chessBoard, int.MinValue, int.MaxValue, depth).Item2
                : NegaMax(chessBoard, depth).Item2;

            string move = TransformMove(chessBoard, bestMove);
            Console.WriteLine("Best move found using {0}, for depth {1} is: {2}", alg, depth, move);
            return move;
        }

        public SmartClient(string host, int port, int depth) : base(host, port,
            (cb) => GetBestMove(cb, depth))
        {
        }
    }
}