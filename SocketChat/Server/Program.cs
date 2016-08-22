using System;

namespace Server
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var server = new ServerObject();
            server.Start(56000);

            while (Console.ReadKey().Key != ConsoleKey.Escape)
            {
            }

            server.Stop();
        }
    }
}