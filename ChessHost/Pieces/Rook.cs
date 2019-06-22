using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ChessHost.Pieces
{
    [DataContract]
    public class Rook : ChessPiece
    {
        public Rook(Tuple<int, int> position, Player player)
            : base(position, player, ChessPieceType.Rook)
        {
        }

        protected override List<Tuple<int, int>> GetPossibleMoves(ChessBoard cb)
        {
            Tuple<int, int> currentPosition = GetPosition();
            List<Tuple<int, int>> allMoves = new List<Tuple<int, int>>();

            for (int i = currentPosition.Item1 + 1; i < 8; i++) // moves down
                if (AddMoveIfAvailable(new Tuple<int, int>(i, currentPosition.Item2), ref cb, ref allMoves)) break;
            for (int i = currentPosition.Item1 - 1; i > 0; i--) // moves up
                if (AddMoveIfAvailable(new Tuple<int, int>(i, currentPosition.Item2), ref cb, ref allMoves)) break;
            for (int i = currentPosition.Item2 + 1; i < 8; i++) // moves right
                if (AddMoveIfAvailable(new Tuple<int, int>(currentPosition.Item1, i), ref cb, ref allMoves)) break;
            for (int i = currentPosition.Item2 - 1; i > 0; i--) // moves left
                if (AddMoveIfAvailable(new Tuple<int, int>(currentPosition.Item1, i), ref cb, ref allMoves)) break;

            return allMoves;
        }
    }
}
