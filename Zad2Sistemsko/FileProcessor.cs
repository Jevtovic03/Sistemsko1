using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Zad2Sistemsko;

namespace Zad2Sistemsko
{
    internal class FileProcessor
    {
        static int izbroji(string content, string word)
        {
            if (string.IsNullOrEmpty(word))
                return 0;
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

        public static async Task CountOccurrencesAsync(HttpListenerContext context, int brojstavki)
        {
            var request = context.Request;
            var searchWords = request.RawUrl.TrimStart('/').Split('&');
            var result = new StringBuilder();
            if (searchWords == null || searchWords.Length == 0)
            {
                Console.WriteLine("Nema dostupnih clanaka ili nisu adekvatno pribavljeni.");
                return;
            }

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

            var fileTasks = Directory.GetFiles(rootDirectory, "*.txt", SearchOption.AllDirectories)
                .Select(async file =>
                {
                    var fileName = Path.GetFileName(file);
                    var fileContent = await File.ReadAllTextAsync(file);

                    var fileResult = new StringBuilder();

                    foreach (var word in searchWords)
                    {
                        var wordCount = izbroji(fileContent, word);
                        if (wordCount > 0)
                        {
                            fileResult.AppendLine($"<p>U fajlu : {fileName}, za rec {word} postoji: {wordCount} pojavljivanja</p>");
                            kes.WriteToCache(word.GetHashCode(), $"<p>U fajlu : {fileName}, za rec {word} postoji: {wordCount} pojavljivanja</p>");
                        }
                        else
                        {
                            fileResult.AppendLine($"<p>U fajlu : {fileName}, za rec {word} ne postoje pojavljivanja</p>");
                            kes.WriteToCache(word.GetHashCode(), $"<p>U fajlu : {fileName}, za rec {word} ne postoje pojavljivanja</p>");
                        }
                    }
                    if (fileResult.Length == 0)
                        fileResult.AppendLine($"<p>Nema pojavljivanja</p>");
                    return fileResult.ToString();
                });

            var fileResults = await Task.WhenAll(fileTasks);
            foreach (var fileResult in fileResults)
            {
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
            await output.WriteAsync(buffer, 0, buffer.Length);
            output.Close();
        }
    }
}
