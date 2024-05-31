using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace sistemsko_1._18681
{
    internal class FileProcessor
    {
        static int izbroji(string content, string word)
        {
            if (word.Length == 0 || word == null)
                return 1;
            var count = 0;
            var brojac = 0;

            for (int i = 0; i < content.Length; i++)
            {
                if (content[i] == word[0])
                {
                    brojac = 0;
                    for (int j = 0; j < word.Length; j++)
                    {
                        if (content.Length - i < word.Length)
                        {
                            return count;
                        }
                        if (content[i + j] == word[j])
                        {
                            brojac++;
                        }
                    }
                    if (brojac == word.Length)
                    {
                        count++;
                        i += word.Length - 1;
                    }

                }
            }
            return count;
        }
        public static void CountOccurrences(HttpListenerContext context, int brojstavki)
        {
            var request = context.Request;
            var searchWords = request.RawUrl.TrimStart('/').Split('&');
            var files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.txt");
            var result = new StringBuilder();
            if (searchWords == null || searchWords.Length == 0)
            {
                Console.WriteLine("Nema dostupnih clanaka ili nisu adekvatno pribavljeni.");
                return;
            }
            int i;
            foreach (var word0 in searchWords)
            {
                int keywordHash = word0.GetHashCode();
                var cachedResult = kes.ReadFromCache(keywordHash);
                if (!string.IsNullOrEmpty(cachedResult))
                {
                    Console.WriteLine("Rezultat pronađen u kešu.");
                    result.AppendLine($"<p>{cachedResult}</p>");
                }
            }

            var rootDirectory = Directory.GetCurrentDirectory(); // putanja

            foreach (var file in Directory.GetFiles(rootDirectory, "*.txt", SearchOption.AllDirectories))
            {
                var fileName = Path.GetFileName(file);

                var fileContent = File.ReadAllText(file);

                var fileResult = new StringBuilder();

                foreach (var word in searchWords)
                {
                    var wordCount = izbroji(fileContent, word);
                    if (wordCount > 0)
                    {
                        fileResult.AppendLine($"<p>U fajlu : {fileName}, za rec {word} postoji: {wordCount} pojavljivanja</p>");
                        kes.WriteToCache(word.GetHashCode(), $"<p>U fajlu : {fileName}, za rec {word} postoji: {wordCount} pojavljivanja</p>");
                    }
                    else {
                        fileResult.AppendLine($"<p>U fajlu : {fileName}, za rec {word} ne postoje pojavljivanja</p>");
                        kes.WriteToCache(word.GetHashCode(), $"<p>U fajlu : {fileName}, za rec {word} ne postoje pojavljivanja</p>");
                    }
                }
                if(fileResult.Length == 0)
                    fileResult.AppendLine($"<p>Nema pojavljivanja</p>");
                result.Append(fileResult);
            }
            var response = context.Response;
            var responseString = $@"
            <html>
            <head><title>Rezultati pretrage</title></head>
            <body>
                <h1>Rezultati pretrage</h1>
                {result}
            </body>
            </html>";
            var buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            var output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }
    }
}
