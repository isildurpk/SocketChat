using System;
using ServerUtils;

namespace Server
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var server = new ServerObject(new Compressor());
            server.Start(56000);

            while (Console.ReadKey().Key != ConsoleKey.Escape)
            {
            }

            server.Stop();
        }
    }
}