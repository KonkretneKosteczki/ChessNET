using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using ChessHost.Pieces;

namespace ChessHost
{
    public class Serializator
    {
        public ChessBoard DeepCopy(ChessBoard cb)
        {
            return ReadToObject(WriteFromObject(cb));
        }
        public ChessPiece DeepCopy(ChessPiece cp)
        {
            return ReadPieceToObject(WriteFromObject(cp));
        }


        public ChessBoard ReadToObject(string json)
        {
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ChessBoard));
            var deserializedChessBoard = ser.ReadObject(ms) as ChessBoard;
            ms.Close();
            return deserializedChessBoard;
        }
        public ChessPiece ReadPieceToObject(string json)
        {
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ChessPiece));
            var deserializedChessPiece = ser.ReadObject(ms) as ChessPiece;
            ms.Close();
            return deserializedChessPiece;
        }

        public string WriteFromObject(ChessBoard chessBoard)
        {
            MemoryStream ms = new MemoryStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ChessBoard));
            ser.WriteObject(ms, chessBoard);
            byte[] json = ms.ToArray();
            ms.Close();
            return Encoding.UTF8.GetString(json, 0, json.Length);
        }

        public string WriteFromObject(ChessPiece chessPiece)
        {
            MemoryStream ms = new MemoryStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ChessPiece));
            ser.WriteObject(ms, chessPiece);
            byte[] json = ms.ToArray();
            ms.Close();
            return Encoding.UTF8.GetString(json, 0, json.Length);
        }
    }
}