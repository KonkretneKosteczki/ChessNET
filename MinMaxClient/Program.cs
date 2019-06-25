using ChessClient;

namespace MinMaxClient
{
    internal class Program
    {
        private static void Main()
        {
            Client smartClient = new SmartClient("localhost", port: 3000, depth: 3);
            smartClient.GameLoop();
        }
    }
}