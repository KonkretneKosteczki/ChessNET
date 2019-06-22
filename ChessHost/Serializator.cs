using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace ChessHost
{
    public class Serializator
    {
        public ChessBoard ReadToObject(string json)
        {
            ChessBoard deserializedChessBoard = null;
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ChessBoard));
            deserializedChessBoard = ser.ReadObject(ms) as ChessBoard;
            ms.Close();
            return deserializedChessBoard;
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
    }
}