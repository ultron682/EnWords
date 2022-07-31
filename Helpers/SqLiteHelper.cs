using EnWords.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite.Net.Platform.WinRT;
using SQLite.Net;
using Windows.UI.Popups;
using System.Diagnostics;

namespace EnWords.Helpers
{
    public class SqLiteHelper
    {
        private SQLiteConnection MySqLiteConnection = new SQLiteConnection(new SQLitePlatformWinRT(), App.PathOfDatabase);

        public SqLiteHelper()
        {
            MySqLiteConnection.CreateTable<Words>();
        }

        public List<Words> GetAllList()
        {
            return MySqLiteConnection.Query<Words>("SELECT * FROM Words ORDER BY EnglishWord");
        }

        public List<Words> GetAllList(SortMethodEnums sortMethodEnums)
        {
            if(sortMethodEnums == SortMethodEnums.Angielskiego_słowa_A_Z)
            {
                return MySqLiteConnection.Query<Words>("SELECT * FROM Words ORDER BY EnglishWord ASC");
            }
            if (sortMethodEnums == SortMethodEnums.Angielskiego_słowa_Z_A)
            {
                return MySqLiteConnection.Query<Words>("SELECT * FROM Words ORDER BY EnglishWord DESC");
            }
            if (sortMethodEnums == SortMethodEnums.Polskiego_słowa_A_Z)
            {
                return MySqLiteConnection.Query<Words>("SELECT * FROM Words ORDER BY PolishWord ASC");
            }
            if (sortMethodEnums == SortMethodEnums.Polskiego_słowa_Z_A)
            {
                return MySqLiteConnection.Query<Words>("SELECT * FROM Words ORDER BY PolishWord DESC");
            }
            Debug.WriteLine("Wystąpił błąd metoda  SqLiteHelper.GetAllList()");
            return null;
        }

        public void DeleteAll()
        {
            MySqLiteConnection.DeleteAll<Words>();
        }

        public async Task Insert(Words words)
        {
            foreach (Words w in GetAllList())
            {
                if (w.PolishWord == words.PolishWord && w.EnglishWord == words.EnglishWord)
                {
                    await new MessageDialog("Nie możesz dodać istniejącego tłumaczenia") { Title = "Błąd" }.ShowAsync();
                    return;
                }
            }

            SQLiteCommand TempSQLiteCommand = MySqLiteConnection.CreateCommand(string.Format("INSERT INTO Words (PolishWord, EnglishWord, Description) VALUES ('{0}','{1}', '{2}')", words.PolishWord, words.EnglishWord, words.Description));
            TempSQLiteCommand.ExecuteNonQuery();
        }

        public void ExecuteCommandNonQuery(string command)
        {
            SQLiteCommand MySQLiteCommand = MySqLiteConnection.CreateCommand(command);
            MySQLiteCommand.ExecuteNonQuery();
        }

        public List<Words> Query(string query)
        {
            List<Words> TempList = MySqLiteConnection.Query<Words>(query);

            return TempList;
        }

        public bool ContainElement(Words words)
        {
            words = WordsHelper.EnCodeWordsObject(words);

            foreach (Words w in GetAllList())
            {
                if (w.PolishWord == words.PolishWord && w.EnglishWord == words.EnglishWord)
                {
                    return true;
                }
            }
            return false;
        }

        public int GetCountTranslate()
        {
            return GetAllList().Count;
        }

        public enum SortMethodEnums
        {
            Angielskiego_słowa_A_Z,
            Angielskiego_słowa_Z_A,
            Polskiego_słowa_A_Z,
            Polskiego_słowa_Z_A
        }
    }
}
