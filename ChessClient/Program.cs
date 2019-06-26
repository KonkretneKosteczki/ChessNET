using ChessHost;
using ChessHost.Pieces;

namespace ChessClient
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    public class Client
    {
        public delegate string MoveSource(ChessBoard cb);

        private readonly MoveSource _moveSource;

        public Player Player;
        public ChessBoard ChessBoard;

        private readonly Socket _sender;

        public Client(string host, int port, MoveSource move)
        {
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, port);
            _moveSource = move;

            try
            {
                _sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                _sender.Connect(localEndPoint);

                Console.WriteLine("Socket connected to Chess Server");


                if (!ReceiveBoardState())return;
                SendRequest("Received Board<EOF>");

                ReceivePlayer();
                SendRequest("Received Player<EOF>");
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se);
                _sender.Close();
            }
        }

        public void PrintBoard()
        {
            ChessBoard.PrintBoard(Player);
        }

        public string ReceiveResponse()
        {
            byte[] messageReceived = new byte[256];

            string msg = null;
            while (true)
            {
                int numByte = _sender.Receive(messageReceived);
                msg += Encoding.ASCII.GetString(messageReceived, 0, numByte);
                if (msg.IndexOf("<EOF>", StringComparison.Ordinal) > -1)
                    break;
            }

            msg = msg.Substring(0, msg.LastIndexOf("<EOF>", StringComparison.Ordinal));

            if (msg.StartsWith("INFO:"))
            {
                Console.WriteLine(msg);
                return null;
            }

            return msg;
        }

        public void SendRequest(string msg)
        {
            if (!msg.EndsWith("<EOF>")) msg += "<EOF>";
            byte[] messageSent = Encoding.ASCII.GetBytes(msg);
            _sender.Send(messageSent);
        }

        public bool ReceiveBoardState()
        {
            Serializator ser = new Serializator();
            String response = ReceiveResponse();
            if (response == null) return false;

            ChessBoard = ser.ReadToObject(response);
            return true;
        }


        public void GameLoop(bool printLogs = false)
        {
            try
            {
                if (Player == Player.Black)
                {
                    // waiting for additional response with white's move before starting game loop
                    if (!ReceiveBoardState()) return;
                    if (printLogs) PrintBoard();
                }

                while (true)
                {
                    string move = _moveSource(ChessBoard);
                    if (ChessBoard.MovePiece(move))
                    {
                        // try to make a move on local copy of the board for client side move validation
                        SendRequest(move); // send move over to server only if valid move
                        if (printLogs) PrintBoard(); // print board with your changes
                        if (!ReceiveBoardState()) return; // get other players changes
                        if (printLogs) PrintBoard(); // print board with other players changes
                    }
                    else Console.WriteLine("Invalid Move");
                }
            }
            catch (SocketException)
            {
                Console.WriteLine("Lost connection to the server");
                _sender.Close();
                Console.ReadLine();
            }
        }

        private void ReceivePlayer()
        {
            string response = ReceiveResponse();
            if (response!=null)
            Player = (Player) Enum.Parse(typeof(Player), response);
        }
    }

    internal class Program
    {
        private static void Main()
        {
            Client client = new Client("localhost", 3000, (cb) => Console.ReadLine());
            client.PrintBoard();
            client.GameLoop(true);
            Console.ReadLine();
        }
    }
}