using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ChessHost.Pieces
{
    [DataContract]
    public class King : ChessPiece
    {
        [DataMember]
        private Tuple<bool, bool> _castling = new Tuple<bool, bool>(true,true);

        public Tuple<bool, bool> Castling
        {
            get => _castling;
            set => _castling = value;
        }

        public King(Tuple<int, int> position, Player player)
            : base(position, player, ChessPieceType.King)
        {
        }

        public override List<Tuple<int, int>> GetPossibleMoves(ChessBoard cb)
        {
            Tuple<int, int> currentPosition = Position;
            List<Tuple<int, int>> allMoves = new List<Tuple<int, int>>();

            for (int i = -1; i <= 1; i++) // square 3x3 with king in the center
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0 || // ignore kings current position
                        currentPosition.Item1 + i > 7 || currentPosition.Item1 + i < 0 || // ignore y out of bounds
                        currentPosition.Item2 + j > 7 || currentPosition.Item2 + j < 0) // ignore x out of bounds
                        continue;

                    else
                        AddMoveIfAvailable(new Tuple<int, int>(currentPosition.Item1 + i, currentPosition.Item2 + j),
                            ref cb,
                            ref allMoves);
                }
            }

            // Castling
            if (Castling.Item2 &&
                cb.GetPieceInPosition(new Tuple<int, int>(currentPosition.Item1 + 1, currentPosition.Item2)) == null)
                //// K _ _ R => _ R K _
                // manually checking K+1, K+2 will be checked by AddMoveIfAvailable
                // Castling.Item2 has information on castled rook/king being moved in the past
                AddMoveIfAvailable(new Tuple<int, int>(currentPosition.Item1, currentPosition.Item2 + 2), ref cb,
                    ref allMoves);
            if (Castling.Item1 &&
                cb.GetPieceInPosition(new Tuple<int, int>(currentPosition.Item1 - 1, currentPosition.Item2)) == null &&
                cb.GetPieceInPosition(new Tuple<int, int>(currentPosition.Item1 - 3, currentPosition.Item2)) == null)
                //// R _ _ _ K => _ _ K R _
                // manually checking K+1, K+2 will be checked by AddMoveIfAvailable
                // Castling.Item2 has information on castled rook/king being moved in the past
                AddMoveIfAvailable(new Tuple<int, int>(currentPosition.Item1, currentPosition.Item2 - 2), ref cb,
                    ref allMoves);

            return allMoves;
        }
    }
}