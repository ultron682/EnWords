using EnWords.Helpers;
using EnWords.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace EnWords.Views
{
    public sealed partial class ExamPage : Page
    {
        private ApplicationDataContainer MyApplicationDataContainer = ApplicationData.Current.LocalSettings;
        private SqLiteHelper MySqLiteHelper;
        private List<Words> ListOfAllElements;
        private List<Words> ListUsedElements;
        private Words ActiveWords;
        private int ScoreOfExam = 0;

        public ExamPage()
        {
            this.InitializeComponent();
            MySqLiteHelper = new SqLiteHelper();
            ListUsedElements = new List<Words>();
            ListOfAllElements = MySqLiteHelper.GetAllList();
            List<Words> TempList = new List<Words>();
            foreach (Words words in ListOfAllElements)
            {
                TempList.Add(WordsHelper.DeCodeWordsObject(words));
            }
            ListOfAllElements.Clear();
            ListOfAllElements = TempList;

            Loaded += ExamPage_Loaded;
        }



        private async void ExamPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (await CheckOfCountTranslatesIsGreatherThan10())
            {
                SetNewTranslate();
            }
        }

        private Words GetOneElementFromAllElements()
        {
            do
            {
                Words TempWords = ListOfAllElements[new Random().Next(0, ListOfAllElements.Count)];
                if (!ListUsedElements.Contains(TempWords))
                {
                    ListUsedElements.Add(TempWords);
                    return TempWords;
                }
            } while (true);
        }

        private void SetNewTranslate()
        {
            object examAnswerMethod = MyApplicationDataContainer.Values["ExamAnswerMethod"];
            if ((int)examAnswerMethod == 0)
            {
                WrittenMethodAnswerGrid.Visibility = Visibility.Visible;

                Words words = GetOneElementFromAllElements();
                words = WordsHelper.DeCodeWordsObject(words);
                ActiveWords = words;
                ButtonNavigateToMainPage.IsEnabled = false;
                CheckButton.IsEnabled = false;
                PolishTranslateField.IsEnabled = false;
                EnglishTranslateField.IsEnabled = false;
                PolishTranslateField.Text = "";
                EnglishTranslateField.Text = "";

                int RandInt = new Random().Next(1, 3);

                if (RandInt == 1)
                {
                    PolishTranslateField.Text = words.PolishWord;
                    EnglishTranslateField.IsEnabled = true;
                    CheckButton.IsEnabled = true;
                    ButtonNavigateToMainPage.IsEnabled = true;
                }
                else if (RandInt == 2)
                {
                    EnglishTranslateField.Text = words.EnglishWord;
                    PolishTranslateField.IsEnabled = true;
                    CheckButton.IsEnabled = true;
                    ButtonNavigateToMainPage.IsEnabled = true;
                }
            }
            else
            {
                SelectableMethodAnswerGrid.Visibility = Visibility.Visible;

                Words words = GetOneElementFromAllElements();
                words = WordsHelper.DeCodeWordsObject(words);
                ActiveWords = words;
                Question.Text = "Jak jest \"" + words.PolishWord + "\" po angielsku?";
                List<Words> TempList = new List<Words>();
                TempList.Clear();
                while (true)
                {
                    int result = new Random().Next(1, ListOfAllElements.Count);
                    if (!TempList.Contains(ListOfAllElements[result]) && ListOfAllElements[result] != words)
                    {
                        TempList.Add(ListOfAllElements[result]);
                    }
                    if(TempList.Count == 3)
                    {
                        break;
                    }
                }
                TempList.Add(words);
                int n = TempList.Count;
                while(n>1)
                {
                    n--;
                    int k = new Random().Next(n+1);
                    Words value = TempList[k];
                    TempList[k] = TempList[n];
                    TempList[n] = value;

                }

                AnswerAButton.Content = TempList[0].EnglishWord;
                AnswerBButton.Content = TempList[1].EnglishWord;
                AnswerCButton.Content = TempList[2].EnglishWord;
                AnswerDButton.Content = TempList[3].EnglishWord;
            }
        }

        private async Task<bool> CheckOfCountTranslatesIsGreatherThan10()
        {
            if (ListOfAllElements.Count >= 10)
            {
                return true;
            }
            MessageDialog MyContentDialog = new MessageDialog("Żeby przeprowadzić test wymagane jest conajmniej 10 tłumaczeń.");
            MyContentDialog.Title = "Nastąpi powrót";
            await MyContentDialog.ShowAsync();
            Frame.Navigate(typeof(MainPage));
            return false;
        }

        private void ButtonNavigateToMainPage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void TranslateField_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                CheckButton_Click(sender, new RoutedEventArgs());
            }
        }

        private void CheckButton_Click(object sender, RoutedEventArgs e)
        {
            object examAnswerMethod = MyApplicationDataContainer.Values["ExamAnswerMethod"];
            if ((int)examAnswerMethod == 0)
            {
                if (ActiveWords.PolishWord == PolishTranslateField.Text.ToLower() && ActiveWords.EnglishWord == EnglishTranslateField.Text.ToLower())
                {
                    ScoreOfExam++;
                }
                else
                {
                    ScoreOfExam--;
                }
                SetScore();
                SetNewTranslate();
            }
        }

        private async void SetScore()
        {
            Points.Text = ScoreOfExam + "/10";

            if (ScoreOfExam < 0)
            {
                await new MessageDialog("Nie zdałeś!!!").ShowAsync();
                Frame.Navigate(typeof(MainPage));
                return;
            }
            if (ScoreOfExam >= 10)
            {
                await new MessageDialog("Zdałeś!!!").ShowAsync();
                Frame.Navigate(typeof(MainPage));
                return;
            }
        }

        private void AnswerButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if ((string)button.Content == ActiveWords.EnglishWord || (string)button.Content == ActiveWords.PolishWord)
            {
                ScoreOfExam++;
            }
            else
            {
                ScoreOfExam--;
            }

            SetScore();
            SetNewTranslate();
        }
    }
}
