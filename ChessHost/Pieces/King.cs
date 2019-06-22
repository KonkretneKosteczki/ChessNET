using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessHost.Pieces
{
    public class King : ChessPiece
    {
        public Tuple<bool, bool> Castling = new Tuple<bool, bool>(true, true);

        public King(Tuple<int, int> position, Player player)
            : base(position, player, ChessPieceType.King)
        {
        }

        protected override List<Tuple<int, int>> GetPossibleMoves(ChessBoard cb)
        {
            Tuple<int, int> currentPosition = GetPosition();
            List<Tuple<int, int>> allMoves = new List<Tuple<int, int>>();

            for (int i = -1; i <= 1; i++) // square 3x3 with king in the center
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0 || // ignore kings current position
                        currentPosition.Item1 + i > 7 || currentPosition.Item1 + i < 0 || // ignore y out of bounds
                        currentPosition.Item2 + j > 7 || currentPosition.Item2 + j < 0) // ignore x out of bounds
                        continue; 

                    else AddMoveIfAvailable(new Tuple<int, int>(currentPosition.Item1 + i, currentPosition.Item2 + j), ref cb,
                        ref allMoves);
                }
            }



            //TODO: castling

            return allMoves;
        }
    }
}
