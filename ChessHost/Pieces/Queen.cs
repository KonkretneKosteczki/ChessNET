using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
                    ref cb, ref allMoves))
                    break;

            for (int i = 1; i <= Math.Min(currentPosition.Item1, 8 - currentPosition.Item2); i++) // top-right diagonal
                if (AddMoveIfAvailable(new Tuple<int, int>(currentPosition.Item1 - i, currentPosition.Item2 + i),
                    ref cb, ref allMoves))
                    break;

            for (int i = 1; i <= Math.Min(8 - currentPosition.Item1, currentPosition.Item2); i++) // bottom-left diagonal
                if (AddMoveIfAvailable(new Tuple<int, int>(currentPosition.Item1 + i, currentPosition.Item2 - i),
                    ref cb, ref allMoves))
                    break;

            for (int i = 1; i <= Math.Min(8 - currentPosition.Item1, 8 - currentPosition.Item2); i++) // bottom-right diagonal
                if (AddMoveIfAvailable(new Tuple<int, int>(currentPosition.Item1 + i, currentPosition.Item2 + i),
                    ref cb, ref allMoves))
                    break;

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
