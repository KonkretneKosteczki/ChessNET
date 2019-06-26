using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ChessHost.Pieces;

namespace ChessHost
{
    internal class Program
    {
        private static void DecideStartingPlayer(Server server)
        {
            // random 0/1 deciding which players starts the game
            // if 1 starting player is the one that first connected to the server Player1
            // if 0 starting player is the second one (achieved by swapping values of server instance)
            if (new Random().Next(0, 2) != 0) return;

            Socket temp = server.Player1;
            server.Player1 = server.Player2;
            server.Player2 = temp;
        }

        private static void Main()
        {
            ChessBoard cb = new ChessBoard();
            cb.PrintBoard();

            Serializator ser = new Serializator();
            string boardState = ser.WriteFromObject(cb);
            Server server = new Server("localhost", 3000);
            try
            {
                DecideStartingPlayer(server);

                server.SendResponse(server.Player1, boardState + "<EOF>");
                server.ReceiveRequest(server.Player1); // wait for response confirming received data
                server.SendResponse(server.Player1, Player.White.ToString() + "<EOF>");
                server.ReceiveRequest(server.Player1);

                server.SendResponse(server.Player2, boardState + "<EOF>");
                server.ReceiveRequest(server.Player2);
                server.SendResponse(server.Player2, Player.Black.ToString() + "<EOF>");
                server.ReceiveRequest(server.Player2);
                while (true)
                {
                    string dataFromClient1 = server.ReceiveRequest(server.Player1);
                    Console.WriteLine("RECEIVED: {0} from WHITE", dataFromClient1);
                    cb.MovePiece(dataFromClient1);
                    server.SendResponse(server.Player2, ser.WriteFromObject(cb));

                    string dataFromClient2 = server.ReceiveRequest(server.Player2);
                    Console.WriteLine("RECEIVED {0} from BLACK", dataFromClient2);
                    cb.MovePiece(dataFromClient2);
                    server.SendResponse(server.Player1, ser.WriteFromObject(cb));
                }
            }
            catch (SocketException)
            {
                if (!server.SocketConnected(server.Player1))
                {
                    Console.WriteLine("Player WHITE forcefully closed connection.");
                    server.SendResponse(server.Player2, "INFO: OPPONENT DISCONNECTED YOU WIN!");
                }
                if (!server.SocketConnected(server.Player2))
                {
                    Console.WriteLine("Player BLACK forcefully closed connection.");
                    server.SendResponse(server.Player1, "INFO: OPPONENT DISCONNECTED YOU WIN!");
                }
                server.Player1.Close();
                server.Player2.Close();
            }
        }
    }

    public class Server
    {
        public Socket Player1;
        public Socket Player2;

        public bool SocketConnected(Socket s)
        {
            bool part1 = s.Poll(1000, SelectMode.SelectRead);
            bool part2 = (s.Available == 0);
            return !part1 || !part2;
        }

        private readonly Socket _listener;

        public string ReceiveRequest(Socket clientSocket)
        {
            byte[] messageReceived = new byte[256];

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

        public void SendResponse(Socket clientSocket, string msg)
        {
            if (!SocketConnected(clientSocket)) return;
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

            catch (SocketException e)
            {
                Console.WriteLine(e.ToString());
                SocketShutDown(_listener);
            }
        }

        private void SocketShutDown(Socket clientSocket)
        {
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }
    }
}