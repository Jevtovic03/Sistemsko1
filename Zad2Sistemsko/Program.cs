using System;

namespace Zad2Sistemsko
{
    class Program
    {
        static void Main()
        {
            var server = new HttpServer("http://localhost:8080/");
            server.Start();
            Console.ReadLine();
        }
    }
}
