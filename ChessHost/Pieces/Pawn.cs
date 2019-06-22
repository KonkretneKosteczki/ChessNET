using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ChessHost.Pieces
{
    [DataContract]
    public class Pawn : ChessPiece
    {
        public Pawn(Tuple<int, int> position, Player player)
            : base(position, player, ChessPieceType.Pawn)
        {
        }

        public override List<Tuple<int, int>> GetPossibleMoves(ChessBoard cb)
        {
            Tuple<int, int> position = Position;
            List<Tuple<int, int>>
                nonCaptureMoves = new List<Tuple<int, int>>(),
                captureMoves = new List<Tuple<int, int>>(),
                allMoves = new List<Tuple<int, int>>();
            bool isPlayerBlack = (GetPlayer() == Player.Black);

            nonCaptureMoves.Add(new Tuple<int, int>(position.Item1 + (isPlayerBlack ? 1 : -1), position.Item2)); // moveForward
            nonCaptureMoves.Add(new Tuple<int, int>(position.Item1 + (isPlayerBlack ? 2 : -2), position.Item2)); // moveForwardDouble

            captureMoves.Add(new Tuple<int, int>(position.Item1 + (isPlayerBlack ? 1 : -1), position.Item2 + 1)); // moveCaptureRight
            captureMoves.Add(new Tuple<int, int>(position.Item1 + (isPlayerBlack ? 1 : -1), position.Item2 - 1)); // moveCaptureLeft

            foreach (Tuple<int, int> move in nonCaptureMoves)
                if (cb.GetPieceInPosition(move) == null)
                    allMoves.Add(move);

            foreach (Tuple<int, int> move in captureMoves)
            {
                ChessPiece CapturedPiece = cb.GetPieceInPosition(move);
                if (CapturedPiece != null && CapturedPiece.GetPlayer() != GetPlayer())
                    allMoves.Add(move);
            }

            return allMoves;
        }
    }
}
