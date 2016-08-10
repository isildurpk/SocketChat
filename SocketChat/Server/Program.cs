using System;

namespace Server
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var server = new ServerObject();
            server.Start(55555);

            while (Console.ReadKey().Key != ConsoleKey.Escape)
            {
            }

            server.Stop();
            Console.Write("Press any key...");
            Console.ReadKey(true);
        }
    }
}