using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ChessHost.Pieces;

namespace ChessHost
{
    [DataContract]
    public enum Player
    {
        [EnumMember]
        Black,
        [EnumMember]
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

    [DataContract]
    [KnownType(typeof(Pawn))]
    [KnownType(typeof(Rook))]
    [KnownType(typeof(King))]
    [KnownType(typeof(Queen))]
    [KnownType(typeof(Bishop))]
    [KnownType(typeof(Knight))]
    public abstract class ChessPiece
    {
        protected ChessPiece(Tuple<int, int> position, Player player, ChessPieceType type)
        {
            _startingPosition = position;
            _position = position;
            _player = player;
            _type = type;
        }

        [DataMember]
        private Tuple<int, int> _startingPosition;
        [DataMember]
        private Tuple<int, int> _position; // Item1 of the tuple is Y direction, Item2 is X direction
        [DataMember]
        private readonly Player _player;
        [DataMember]
        private readonly ChessPieceType _type;

        private bool ValidateMove(Tuple<int, int> whereTo, ChessBoard cb)
        {
            if (!ValidatePosition(whereTo)) return false;
            return GetPossibleMoves(cb).Contains(whereTo);
        }

        public abstract List<Tuple<int, int>> GetPossibleMoves(ChessBoard cb);

        public bool ValidatePosition(Tuple<int, int> pos)
        {
            // checks if given position is not out of range of the board
            return pos.Item1 >= 0 && pos.Item1 <= 7 && pos.Item2 >= 0 && pos.Item2 <= 7;
        }

        public Tuple<int, int> Position
        {
            get => _position;
            set => _position = value;
        }

        public Tuple<int, int> StartingPosition
        {
            get => _startingPosition;
            set => _startingPosition = value;
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
}