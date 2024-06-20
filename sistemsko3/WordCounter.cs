using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace sistemsko3
{
    public static class WordCounter
    {
        public static Dictionary<string, int> GetWordCounts(string text)
        {
            var wordCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var words = Regex.Split(text, @"\W+");

            foreach (var word in words)
            {
                if (string.IsNullOrWhiteSpace(word)) continue;

                if (wordCounts.ContainsKey(word))
                {
                    wordCounts[word]++;
                }
                else
                {
                    wordCounts[word] = 1;
                }
            }

            return wordCounts;
        }
    }
}
