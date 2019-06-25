using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ChessHost.Pieces
{
    [DataContract]
    public class Queen : ChessPiece
    {
        public Queen(Tuple<int, int> position, Player player)
            : base(position, player, ChessPieceType.Queen)
        {
        }

        public override List<Tuple<int, int>> GetPossibleMoves(ChessBoard cb)
        {
            Tuple<int, int> currentPosition = Position;
            List<Tuple<int, int>> allMoves = new List<Tuple<int, int>>();

            for (int i = 1; i <= Math.Min(currentPosition.Item1, currentPosition.Item2); i++) // top-left diagonal
                if (AddMoveIfAvailable(new Tuple<int, int>(currentPosition.Item1 - i, currentPosition.Item2 - i),
                    cb, ref allMoves))
                    break;

            for (int i = 1; i <= Math.Min(currentPosition.Item1, 8 - currentPosition.Item2); i++) // top-right diagonal
                if (AddMoveIfAvailable(new Tuple<int, int>(currentPosition.Item1 - i, currentPosition.Item2 + i),
                    cb, ref allMoves))
                    break;

            for (int i = 1; i <= Math.Min(8 - currentPosition.Item1, currentPosition.Item2); i++) // bottom-left diagonal
                if (AddMoveIfAvailable(new Tuple<int, int>(currentPosition.Item1 + i, currentPosition.Item2 - i),
                    cb, ref allMoves))
                    break;

            for (int i = 1; i <= Math.Min(8 - currentPosition.Item1, 8 - currentPosition.Item2); i++) // bottom-right diagonal
                if (AddMoveIfAvailable(new Tuple<int, int>(currentPosition.Item1 + i, currentPosition.Item2 + i),
                    cb, ref allMoves))
                    break;

            for (int i = currentPosition.Item1 + 1; i < 8; i++) // moves down
                if (AddMoveIfAvailable(new Tuple<int, int>(i, currentPosition.Item2), cb, ref allMoves)) break;
            for (int i = currentPosition.Item1 - 1; i > 0; i--) // moves up
                if (AddMoveIfAvailable(new Tuple<int, int>(i, currentPosition.Item2), cb, ref allMoves)) break;
            for (int i = currentPosition.Item2 + 1; i < 8; i++) // moves right
                if (AddMoveIfAvailable(new Tuple<int, int>(currentPosition.Item1, i), cb, ref allMoves)) break;
            for (int i = currentPosition.Item2 - 1; i > 0; i--) // moves left
                if (AddMoveIfAvailable(new Tuple<int, int>(currentPosition.Item1, i), cb, ref allMoves)) break;

            return allMoves;
        }
    }
}
