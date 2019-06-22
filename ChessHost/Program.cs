using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChessHost
{
    class Program
    {
        static void DecideStartingPlayer(ref Server server)
        {
            // random 0/1 deciding which players starts the game
            // if 1 starting player is the one that first connected to the server Player1
            // if 0 starting player is the second one (achieved by swapping values of server instance)
            if (new Random().Next(0, 2) == 0)
            {
                var temp = server.Player1;
                server.Player1 = server.Player2;
                server.Player2 = temp;
            }
        }

        static void Main(string[] args)
        {
            ChessBoard cb = new ChessBoard();
            cb.PrintBoard();

            List<Tuple<ChessPiece, List<Tuple<int, int>>>> moves = cb.GetAllPossibleMoves();
            foreach (var move in moves)
            {
                Console.WriteLine("{0},{1}",
                    move.Item1.GetPosition().Item1, move.Item1.GetPosition().Item2);
                foreach (var m in move.Item2)
                    Console.Write("{0},{1}; ",m.Item1, m.Item2);
                Console.WriteLine("\n");
            }

            Serializator ser = new Serializator();
            string BoardState = ser.WriteFromObject(cb);

            Server server = new Server("localhost", 3000);
//            DecideStartingPlayer(ref server);

            server.sendResponse(server.Player1, BoardState + "<EOF>");
            server.ReceiveRequest(server.Player1); // wait for response confirming received data
            server.sendResponse(server.Player1, Player.White.ToString() + "<EOF>");
            server.ReceiveRequest(server.Player1);

            server.sendResponse(server.Player2, BoardState + "<EOF>");
            server.ReceiveRequest(server.Player2);
            server.sendResponse(server.Player2, Player.Black.ToString() + "<EOF>");
            server.ReceiveRequest(server.Player2);

            while (true)
            {
                try
                {
                    string dataFromClient1 = server.ReceiveRequest(server.Player1);
                    Console.WriteLine("RECEIVED: {0} from WHITE", dataFromClient1);
                    cb.MovePiece(dataFromClient1);
                    server.sendResponse(server.Player2, ser.WriteFromObject(cb));

                    string dataFromClient2 = server.ReceiveRequest(server.Player2);
                    Console.WriteLine("RECEIVED {0} from BLACK", dataFromClient2);
                    cb.MovePiece(dataFromClient2);
                    server.sendResponse(server.Player1, ser.WriteFromObject(cb));
                }
                catch (SocketException e)
                {
                    // connection closed by client
                    // TODO: send message about winning by walk over to other client
                }
            }
        }
    }

    public class Server
    {
        public Socket Player1;
        public Socket Player2;

        private Socket _listener;

        public string ReceiveRequest(Socket clientSocket)
        {
            byte[] messageReceived = new byte[8192];

            string msg = null;
            while (true)
            {
                int numByte = clientSocket.Receive(messageReceived);
                msg += Encoding.ASCII.GetString(messageReceived, 0, numByte);
                if (msg.IndexOf("<EOF>", StringComparison.Ordinal) > -1)
                    break;
            }

            return msg.Substring(0, msg.LastIndexOf("<EOF>", StringComparison.Ordinal));
        }

        public void sendResponse(Socket clientSocket, string msg)
        {
            if (!msg.EndsWith("<EOF>")) msg += "<EOF>";
            byte[] message = Encoding.ASCII.GetBytes(msg);
            clientSocket.Send(message);
        }

        public Server(string hostAddress, int port)
        {
            IPHostEntry host = Dns.GetHostEntry(hostAddress);
            IPAddress ipAddress = host.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

            try
            {
                _listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                _listener.Bind(localEndPoint);
                _listener.Listen(2);

                Console.WriteLine("Waiting for connections... ");

                Player1 = _listener.Accept();
                Console.WriteLine("Player 1: connected.");
                Player2 = _listener.Accept();
                Console.WriteLine("Player 2: connected.");
            }

            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void SocketShutDown(Socket clientSocket)
        {
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }
    }
}