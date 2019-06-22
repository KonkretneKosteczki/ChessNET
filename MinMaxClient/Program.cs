using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessHost;

namespace MinMaxClient
{
    //            _value = _pieceValues[type];

    class Program
    {

        //            List<Tuple<ChessPiece, List<Tuple<int, int>>>> moves = cb.GetAllPossibleMoves();
        //            foreach (var move in moves)
        //            {
        //                Console.WriteLine("{0},{1}",
        //                    move.Item1.GetPosition().Item1, move.Item1.GetPosition().Item2);
        //                foreach (var m in move.Item2)
        //                    Console.Write("{0},{1}; ",m.Item1, m.Item2);
        //                Console.WriteLine("\n");
        //            }
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
        static void Main(string[] args)
        {
            // The code provided will print ‘Hello World’ to the console.
            // Press Ctrl+F5 (or go to Debug > Start Without Debugging) to run your app.
            Console.WriteLine("Hello World!");
            Console.ReadKey();

            // Go to http://aka.ms/dotnet-get-started-console to continue learning how to build a console app! 
        }
    }
}
