using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ChessHost
{
    public enum Player
    {
        Black,
        White
    }

    public enum ChessPieceType
    {
        King,
        Queen,
        Rook,
        Bishop,
        Knight,
        Pawn
    }


    public abstract class ChessPiece
    {
        private readonly Dictionary<ChessPieceType, int> _pieceValues = new Dictionary<ChessPieceType, int>
        {
            //The following table is the most common assignment of point values
            //(Capablanca & de Firmian 2006:24–25), (Seirawan & Silman 1990:40),
            //(Soltis 2004:6), (Silman 1998:340), (Polgar & Truong 2005:11).
            [ChessPieceType.King] = 1000,
            [ChessPieceType.Queen] = 9,
            [ChessPieceType.Rook] = 5,
            [ChessPieceType.Bishop] = 3,
            [ChessPieceType.Knight] = 3,
            [ChessPieceType.Pawn] = 1,
        };

        protected ChessPiece(Tuple<int, int> position, Player player, ChessPieceType type)
        {
            _position = position;
            _player = player;
            _type = type;
            _value = _pieceValues[type];
        }

        // Item1 of the tuple is Y direction, Item2 is X direction
        private Tuple<int, int> _position;
        private int _value; // for min-max ai algorithm

        private readonly Player _player;
        private readonly ChessPieceType _type;

        private bool ValidateMove(Tuple<int, int> whereTo, ChessBoard cb)
        {
            if (!ValidatePosition(whereTo)) return false;
            return GetPossibleMoves(cb).Contains(whereTo);
        }

        protected abstract List<Tuple<int, int>> GetPossibleMoves(ChessBoard cb);

        public bool ValidatePosition(Tuple<int, int> pos)
        {
            // checks if given position is not out of range of the board
            return pos.Item1 >= 0 && pos.Item1 <= 7 && pos.Item2 >= 0 && pos.Item2 <= 7;
        }

        public Tuple<int, int> GetPosition()
        {
            return _position;
        }

        public ChessPieceType GetPieceType()
        {
            return _type;
        }

        public Player GetPlayer()
        {
            return _player;
        }

        public bool MovePiece(Tuple<int, int> whereTo, ChessBoard cb)
        {
            bool canMove = ValidateMove(whereTo, cb);
            if (canMove) _position = whereTo;
            return canMove;
        }

        protected bool AddMoveIfAvailable(Tuple<int, int> pos, ref ChessBoard cb, ref List<Tuple<int, int>> allMoves)
        {
            ChessPiece piece = cb.GetPieceInPosition(pos);

            if (piece == null)
            {
                allMoves.Add(pos); // add position if no piece
                return false;
            }
            if (piece.GetPlayer() != GetPlayer()) allMoves.Add(pos); // add position if capturing move
            return true; // stop adding pieces as bishop cant jump over other pieces
        }

    }


    public class Queen : ChessPiece
    {
        public Queen(Tuple<int, int> position, Player player)
            : base(position, player, ChessPieceType.Queen)
        {
        }

        protected override List<Tuple<int, int>> GetPossibleMoves(ChessBoard cb)
        {
            throw new NotImplementedException();
        }
    }

    
    public class King : ChessPiece
    {
        public King(Tuple<int, int> position, Player player)
            : base(position, player, ChessPieceType.King)
        {
        }

        protected override List<Tuple<int, int>> GetPossibleMoves(ChessBoard cb)
        {
            throw new NotImplementedException();
        }
    }

    
}