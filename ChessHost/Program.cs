using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessHost
{
    class Program
    {
        static void Main(string[] args)
        {
//            Console.OutputEncoding = System.Text.Encoding.UTF8;
            ChessBoard cb = new ChessBoard();

            while (true)
            {
//                Console.Clear();
                cb.PrintBoard();
                cb.MovePiece(Console.ReadLine());
            }
        }
    }
}
