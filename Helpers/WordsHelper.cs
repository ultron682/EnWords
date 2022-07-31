using EnWords.Models;

namespace EnWords.Helpers
{
    public static class WordsHelper
    {
        /// <summary>
        /// Koduje tłumaczenia
        /// </summary>
        public static Words EnCodeWordsObject(Words words)
        {
            if (words.PolishWord != null)
                words.PolishWord = EnCodeString(words.PolishWord);
            if (words.Description != null)
                words.Description = EnCodeString(words.Description);

            return words;
        }

        /// <summary>
        /// Dekoduje tłumaczenia
        /// </summary>
        public static Words DeCodeWordsObject(Words words)
        {
            if (words.PolishWord != null)
                words.PolishWord = DeCodeString(words.PolishWord);
            if (words.Description != null)
                words.Description = DeCodeString(words.Description);

            return words;
        }

        /// <summary>
        /// Oczyszcza tłumaczenia z nieporządanych efektów
        /// </summary>
        public static Words TrimEndAndStart_ToLower(Words words)
        {
            words.EnglishWord = words.EnglishWord.ToLower();
            words.PolishWord = words.PolishWord.ToLower();
            words.EnglishWord = words.EnglishWord.TrimEnd();
            words.PolishWord = words.PolishWord.TrimEnd();
            words.EnglishWord = words.EnglishWord.TrimStart();
            words.PolishWord = words.PolishWord.TrimStart();
            return words;
        }

        public static string EnCodeString(string word)
        {
            if (word != null)
            {
                word = word.Replace(" ", "_");
                word = word.Replace("ą", "ghf");
                word = word.Replace("ę", "cfg");
                word = word.Replace("ć", "gfl");
                word = word.Replace("ś", "lgh");
                word = word.Replace("ź", "cgc");
                word = word.Replace("ż", "clc");
                word = word.Replace("ó", "lcl");
                word = word.Replace("ł", "lrp");
                word = word.Replace("ń", "lep");
            }
            return word;
        }

        public static string DeCodeString(string word)
        {
            if (word != null)
            {
                word = word.Replace("_", " ");
                word = word.Replace("ghf", "ą");
                word = word.Replace("cfg", "ę");
                word = word.Replace("gfl", "ć");
                word = word.Replace("lgh", "ś");
                word = word.Replace("cgc", "ź");
                word = word.Replace("clc", "ż");
                word = word.Replace("lcl", "ó");
                word = word.Replace("lrp", "ł");
                word = word.Replace("lep", "ń");
            }
            return word;
        }
    }
}
