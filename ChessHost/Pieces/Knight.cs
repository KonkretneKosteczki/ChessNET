using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessHost.Pieces
{
    public class Knight : ChessPiece
    {
        public Knight(Tuple<int, int> position, Player player)
            : base(position, player, ChessPieceType.Knight)
        {
        }

        protected override List<Tuple<int, int>> GetPossibleMoves(ChessBoard cb)
        {
            Tuple<int, int> currentPosition = GetPosition();
            List<Tuple<int, int>> allMoves = new List<Tuple<int, int>>();

            AddMoveIfAvailable(new Tuple<int, int>(currentPosition.Item1 + 1, currentPosition.Item2 + 2), ref cb,
                ref allMoves);
            AddMoveIfAvailable(new Tuple<int, int>(currentPosition.Item1 + 1, currentPosition.Item2 - 2), ref cb,
                ref allMoves);
            AddMoveIfAvailable(new Tuple<int, int>(currentPosition.Item1 - 1, currentPosition.Item2 + 2), ref cb,
                ref allMoves);
            AddMoveIfAvailable(new Tuple<int, int>(currentPosition.Item1 - 1, currentPosition.Item2 - 2), ref cb,
                ref allMoves);
            AddMoveIfAvailable(new Tuple<int, int>(currentPosition.Item1 + 2, currentPosition.Item2 + 1), ref cb,
                ref allMoves);
            AddMoveIfAvailable(new Tuple<int, int>(currentPosition.Item1 + 2, currentPosition.Item2 - 1), ref cb,
                ref allMoves);
            AddMoveIfAvailable(new Tuple<int, int>(currentPosition.Item1 - 2, currentPosition.Item2 + 1), ref cb,
                ref allMoves);
            AddMoveIfAvailable(new Tuple<int, int>(currentPosition.Item1 - 2, currentPosition.Item2 - 1), ref cb,
                ref allMoves);
            
            return allMoves;
        }
    }
}
