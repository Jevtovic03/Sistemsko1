using System;

namespace sistemsko_1._18681
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
