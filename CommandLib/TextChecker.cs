using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectLib
{
    public class TextChecker
    {
        private static TextChecker instance = null!;

        private static HashSet<string> bannedWords = null!;

        private TextChecker() 
        {
            if (bannedWords is null)
            {
                bannedWords = LoadBannedWords();
            }
        }

        public static TextChecker Instance() 
        {
            if (instance is null)
            {
                instance = new TextChecker();
            }
            return instance;
        }

        public bool ContainsBannedWord(string text)
        {
            foreach (string word in bannedWords)
            {
                if (text.IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
            }
            return false;
        }

        private HashSet<string> LoadBannedWords(string filename = "")
        {
            // Загружает банворды из текстового файла
            var bannedWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            try
            {
                foreach (var line in File.ReadLines(filename))
                {
                    var trimmedLine = line.Trim();
                    if (!string.IsNullOrEmpty(trimmedLine))
                    {
                        bannedWords.Add(trimmedLine);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке файла: {ex.Message}");
            }

            return bannedWords;
        }
    }
}
