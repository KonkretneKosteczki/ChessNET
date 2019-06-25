using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ChessHost.Pieces
{
    [DataContract]
    public class Knight : ChessPiece
    {
        public Knight(Tuple<int, int> position, Player player)
            : base(position, player, ChessPieceType.Knight)
        {
        }

        private void AddPositionValidatedMove(Tuple<int, int> move, ref ChessBoard cb, ref List<Tuple<int, int>> allMoves)
        {
            if (ValidatePosition(move)) AddMoveIfAvailable(move, cb, ref allMoves);
        }

        public override List<Tuple<int, int>> GetPossibleMoves(ChessBoard cb)
        {
            Tuple<int, int> currentPosition = Position;
            List<Tuple<int, int>> allMoves = new List<Tuple<int, int>>();

            AddPositionValidatedMove(new Tuple<int, int>(currentPosition.Item1 + 1, currentPosition.Item2 + 2), ref cb,
                ref allMoves);
            AddPositionValidatedMove(new Tuple<int, int>(currentPosition.Item1 + 1, currentPosition.Item2 - 2), ref cb,
                ref allMoves);
            AddPositionValidatedMove(new Tuple<int, int>(currentPosition.Item1 - 1, currentPosition.Item2 + 2), ref cb,
                ref allMoves);
            AddPositionValidatedMove(new Tuple<int, int>(currentPosition.Item1 - 1, currentPosition.Item2 - 2), ref cb,
                ref allMoves);
            AddPositionValidatedMove(new Tuple<int, int>(currentPosition.Item1 + 2, currentPosition.Item2 + 1), ref cb,
                ref allMoves);
            AddPositionValidatedMove(new Tuple<int, int>(currentPosition.Item1 + 2, currentPosition.Item2 - 1), ref cb,
                ref allMoves);
            AddPositionValidatedMove(new Tuple<int, int>(currentPosition.Item1 - 2, currentPosition.Item2 + 1), ref cb,
                ref allMoves);
            AddPositionValidatedMove(new Tuple<int, int>(currentPosition.Item1 - 2, currentPosition.Item2 - 1), ref cb,
                ref allMoves);

            return allMoves;
        }
    }
}