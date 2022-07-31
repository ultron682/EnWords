using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using EnWords.Models;
using EnWords.Helpers;
using System.Diagnostics;
using System;
using EnWords.Views;
using System.Threading.Tasks;

namespace EnWords.Views_ContentDialogs
{
    public sealed partial class EditContentDialog : ContentDialog
    {
        private SqLiteHelper MySqLiteHelper;
        private Words words;
        private ContentDialogResult MyContentDialogResult = ContentDialogResult.Error;

        public EditContentDialog(Words words)
        {
            this.InitializeComponent();
            this.MySqLiteHelper = new SqLiteHelper();
            words.ToLower();
            words.EnglishWord = words.EnglishWord.Trim(char.Parse("_"), char.Parse(" "));
            this.words = words;
            LoadTextBoxs();
        }

        private void LoadTextBoxs()
        {
            Words tempwords = WordsHelper.DeCodeWordsObject(words);
            
            tempwords.EnglishWord = tempwords.EnglishWord.ToLower();
            tempwords.PolishWord = tempwords.PolishWord.ToLower();
            EnglishWordField.Text = tempwords.EnglishWord;
            PolishWordField.Text = tempwords.PolishWord;
            DescriptionField.Text = tempwords.Description;
        }


        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                words = WordsHelper.EnCodeWordsObject(words);
                MySqLiteHelper.ExecuteCommandNonQuery(string.Format("DELETE FROM Words WHERE PolishWord='{0}' AND EnglishWord='{1}'", words.PolishWord, words.EnglishWord));
                MainPage.MainPageObject.LoadDatabase();
                MainPage.MainPageObject.ShowNewChange("Usunięto tłumaczenie");
                Hide();
                MyContentDialogResult = ContentDialogResult.Delete;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                MainPage.MainPageObject.ShowNewChange("Błąd: " + ex.Message);
            }
        }

        private void ButtonSubmit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Words TempWords = new Words(EnglishWordField.Text, PolishWordField.Text);
                TempWords.Description = WordsHelper.EnCodeString( DescriptionField.Text);
                TempWords = WordsHelper.TrimEndAndStart_ToLower(TempWords);
                TempWords = WordsHelper.EnCodeWordsObject(TempWords);
                TempWords.IDWord = words.IDWord;
                words = WordsHelper.EnCodeWordsObject(words);
                words = WordsHelper.TrimEndAndStart_ToLower(words);
                MySqLiteHelper.ExecuteCommandNonQuery(string.Format("UPDATE Words SET PolishWord='{0}', EnglishWord='{1}', Description='{3}' WHERE IDWord='{2}'", TempWords.PolishWord, TempWords.EnglishWord, words.IDWord, TempWords.Description));
                MainPage.MainPageObject.ShowNewChange("Wprowadzono zmiany");
                MyContentDialogResult = ContentDialogResult.Submit;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                MainPage.MainPageObject.ShowNewChange("Błąd: " + ex.Message);
            }
            finally
            {
                Hide();
                MainPage.MainPageObject.LoadDatabase();
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            MainPage.MainPageObject.ShowNewChange("Anulowano zmiany");
            MyContentDialogResult = ContentDialogResult.Cancel;
        }

        private void EnglishWordField_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            if(sender.Text == "")
            {
                ButtonSubmit.IsEnabled = false;
                ButtonDelete.IsEnabled = false;
            }
            else
            {
                ButtonSubmit.IsEnabled = true;
                ButtonDelete.IsEnabled = true;
            }
        }

        public async new Task<ContentDialogResult> ShowAsync()
        {
            await base.ShowAsync();
            return MyContentDialogResult;
        }



        public enum ContentDialogResult
        {
            Submit,
            Cancel,
            Delete,
            Error
        }
    }
}