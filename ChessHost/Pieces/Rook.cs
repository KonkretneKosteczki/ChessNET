using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ChessHost.Pieces
{
    [DataContract]
    public class Rook : ChessPiece
    {

        public Rook(Tuple<int, int> position, Player player)
            : base(position, player, ChessPieceType.Rook)
        {
        }

        [DataMember]
        private bool _canCastle = true;

        public bool CanCastle
        {
            get => _canCastle;
            set => _canCastle = value;
        }

        public override List<Tuple<int, int>> GetPossibleMoves(ChessBoard cb)
        {
            Tuple<int, int> currentPosition = Position;
            List<Tuple<int, int>> allMoves = new List<Tuple<int, int>>();

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
