using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace sistemsko_1._18681
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
                ThreadPool.QueueUserWorkItem(ProcessRequest, context);
            }
        }

        private static void ProcessRequest(object state)
        {
            Console.WriteLine("Zahtev primljen, obradjivanje...");
            var context = (HttpListenerContext)state;
            int brojstavki = 0;
            FileProcessor.CountOccurrences(context, brojstavki);
        }

        }
    }
