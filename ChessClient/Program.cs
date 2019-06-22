using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessHost;

namespace ChessClient
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    class Client
    {
        private Socket _sender;

        public Client(string host, int port)
        {
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, port);

            try
            {
                _sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                _sender.Connect(localEndPoint);

                Console.WriteLine("Socket connected to Chess Server");
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se.ToString());
            }
        }

        public string ReceiveResponse()
        {
            byte[] messageReceived = new byte[8192];

            string msg = null;
            while (true)
            {
                int numByte = _sender.Receive(messageReceived);
                msg += Encoding.ASCII.GetString(messageReceived, 0, numByte);
                if (msg.IndexOf("<EOF>", StringComparison.Ordinal) > -1)
                    break;
            }

//            Console.WriteLine("RECEIVED: {0}", msg);
            return msg.Substring(0, msg.LastIndexOf("<EOF>", StringComparison.Ordinal));
        }

        public void SendRequest(string msg)
        {
            if (!msg.EndsWith("<EOF>")) msg += "<EOF>";
            byte[] messageSent = Encoding.ASCII.GetBytes(msg);
            int byteSent = _sender.Send(messageSent);
        }

        public ChessBoard ReceiveBoardState()
        {
            Serializator ser = new Serializator();
            return ser.ReadToObject(ReceiveResponse());
        }

        public Player ReceivePlayer()
        {
            return (Player) System.Enum.Parse(typeof(Player), ReceiveResponse());
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Client client = new Client("localhost", 3000);
            ChessBoard chessBoard = client.ReceiveBoardState();
            client.SendRequest("Received Board<EOF>");

            Player player = client.ReceivePlayer();
            client.SendRequest("Received Player<EOF>");

            chessBoard.PrintBoard(player);
            Console.WriteLine("Playing as: {0}", player);

            if (player == Player.Black) // waiting for additional response with white's move before starting game loop
            {
                chessBoard = client.ReceiveBoardState();
                chessBoard.PrintBoard(player);
            }

            while (true)
            {
                string move = Console.ReadLine();
                if (chessBoard.MovePiece(move)) // try to make a move on local copy of the board for client side move validation
                {
                    client.SendRequest(move); // send move over to server only if valid move
                    chessBoard.PrintBoard(player); // print board with your changes
                    chessBoard = client.ReceiveBoardState(); // get other players changes
                    chessBoard.PrintBoard(player); // print board with other players changes
                }
                else Console.WriteLine("Invalid Move");
            }
        }
    }
}