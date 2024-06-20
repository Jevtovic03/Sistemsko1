using System;
using System.Net;
using System.Threading;
using Zad2Sistemsko;

namespace Zad2Sistemsko
{
    internal class HttpServer
    {
        private readonly HttpListener listener;
        public HttpServer(string prefix)
        {
            listener = new HttpListener();
            listener.Prefixes.Add(prefix);
            listener.Start();
        }

        public void Start()
        {
            Console.WriteLine("Web server pokrenut. Osluškujem zahteve...");
            while (true)
            {
                var context = listener.GetContext();
                Task.Run(async () => {
                    Console.WriteLine("Zahtev primljen, obradjivanje...");
                    await FileProcessor.CountOccurrencesAsync(context);
                });
            }
        }
    }
}
