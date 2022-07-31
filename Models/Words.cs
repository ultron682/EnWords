using SQLite.Net.Attributes;

namespace EnWords.Models
{
    [Table("Words")]
    public class Words
    {
        [PrimaryKey, NotNull, AutoIncrement]
        public int IDWord { get; set; }

        public string EnglishWord { get; set; }
        public string PolishWord { get; set; }
        public string Description { get; set; }

        public Words()
        {

        }

        public Words(string EnglishWord, string PolishWord)
        {
            this.EnglishWord = EnglishWord;
            this.PolishWord = PolishWord;
        }

        public override string ToString()
        {
            return "       " + IDWord + ":  " + "'" + EnglishWord + "'" + "  :  " + "'" + PolishWord + "'" + "   :   " + "'" + Description + "'";
        }

        public void ToLower()
        {
            EnglishWord = EnglishWord.ToLower();
            PolishWord = PolishWord.ToLower();
        }
    }
}
