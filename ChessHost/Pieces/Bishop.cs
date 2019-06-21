﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessHost.Pieces
{
    public class Bishop : ChessPiece
    {
        public Bishop(Tuple<int, int> position, Player player)
            : base(position, player, ChessPieceType.Bishop)
        {
        }

        protected override List<Tuple<int, int>> GetPossibleMoves(ChessBoard cb)
        {
            Tuple<int, int> currentPosition = GetPosition();
            List<Tuple<int, int>> allMoves = new List<Tuple<int, int>>();

            for (int i = 1; i <= currentPosition.Item1 && i <= currentPosition.Item2; i++) // top-left diagonal
                if (AddMoveIfAvailable(new Tuple<int, int>(currentPosition.Item1 - i, currentPosition.Item2 - i),
                    ref cb, ref allMoves))
                    break;

            for (int i = 1; i <= currentPosition.Item1 && i + currentPosition.Item2 <= 8; i++) // top-right diagonal
                if (AddMoveIfAvailable(new Tuple<int, int>(currentPosition.Item1 - i, currentPosition.Item2 + i),
                    ref cb, ref allMoves))
                    break;

            for (int i = 1; i + currentPosition.Item1 <= 8 && i <= currentPosition.Item2; i++) // bottom-left diagonal
                if (AddMoveIfAvailable(new Tuple<int, int>(currentPosition.Item1 + i, currentPosition.Item2 - i),
                    ref cb, ref allMoves))
                    break;

            for (int i = 1;
                i + currentPosition.Item1 <= 8 && i + currentPosition.Item2 <= 8;
                i++) // bottom-right diagonal
                if (AddMoveIfAvailable(new Tuple<int, int>(currentPosition.Item1 + i, currentPosition.Item2 + i),
                    ref cb, ref allMoves))
                    break;

            return allMoves;
        }
    }
}