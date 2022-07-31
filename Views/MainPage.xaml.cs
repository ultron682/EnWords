using EnWords.Models;
using EnWords.Views_ContentDialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.UI.Xaml.Controls;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.UI.Xaml;
using EnWords.Helpers;
using System.Threading.Tasks;
using Windows.Storage;

// Ostatnie oznaczenie błędu: 2
// Klucze ustawień: SortMethod, ExamAnswerMethod, 

namespace EnWords.Views
{
    public partial class MainPage : Page
    {
        ApplicationDataContainer MyApplicationDataContainer = ApplicationData.Current.LocalSettings;
        public static MainPage MainPageObject;
        private SqLiteHelper MySqLiteHelper = new SqLiteHelper();
        private DispatcherTimer MyDispatcherTimer;
        private int TimeOfWork = 0;

        public MainPage()
        {
            this.InitializeComponent();
            SetTitleBarColor();
            MyDispatcherTimer = new DispatcherTimer();
            MyDispatcherTimer.Tick += MyDispatcherTimer_Tick;
            MyDispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Inicjalizacja ustawienień aplikacji
            if (!MyApplicationDataContainer.Values.ContainsKey("SortMethod"))
            {
                MyApplicationDataContainer.Values["SortMethod"] = 0;
                MyApplicationDataContainer.Values["ExamAnswerMethod"] = 1;
                // inne tu
            }

            //Wyłączenie focusu na kontrolke
            MyListView.Focus(FocusState.Unfocused);

            //Ładowanie bazy danych
            LoadDatabase();           

            // Ustawienie referencji do obiektu ze względu na zewnętrzny dostęp
            MainPageObject = this;
        }

        private static void SetTitleBarColor()
        {
            var PasekTytułu = ApplicationView.GetForCurrentView().TitleBar;
            PasekTytułu.BackgroundColor = Color.FromArgb(255, 21, 101, 192);// Kolor: Niebieski
            PasekTytułu.ForegroundColor = Color.FromArgb(254, 1, 1, 1);// Kolor: Prawie Czarny
            PasekTytułu.ButtonBackgroundColor = Color.FromArgb(255, 21, 101, 192);// Kolor: Niebieski
            PasekTytułu.ButtonForegroundColor = Color.FromArgb(254, 1, 1, 1);// Kolor: Prawie Czarny
            ApplicationView.GetForCurrentView().TryResizeView(new Windows.Foundation.Size(500, 750));
        }

        public void LoadDatabase()
        {
            try
            {
                SearchBox.Text = "";
                MyListView.Items.Clear();
                object sortMethod = MyApplicationDataContainer.Values["SortMethod"];
                foreach (Words words in MySqLiteHelper.GetAllList((SqLiteHelper.SortMethodEnums)sortMethod))
                {
                    Words tempwords = WordsHelper.DeCodeWordsObject(words);
                    words.EnglishWord = tempwords.EnglishWord + " ________________________________";
                    MyListView.Items.Add(tempwords);
                }

                //Ustawianie tekstu dla SearchBox'u
                if (MySqLiteHelper.GetCountTranslate() == 0)
                {
                    SearchBox.PlaceholderText = "Dodaj tłumaczenie klikając na plus";
                }
                else
                {
                    SearchBox.PlaceholderText = "Szukaj spośród " + MySqLiteHelper.GetCountTranslate() + " tłumaczeń";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                ShowNewChange("Błąd 1 przepraszamy");
            }
        }

        public async Task InsertData(string PolishWord, string EnglishWord)
        {
            try
            {
                foreach (Words words in MySqLiteHelper.GetAllList())
                {
                    if (words.PolishWord == PolishWord && words.EnglishWord == EnglishWord)
                    {
                        await new MessageDialog("Nie możesz dodać istniejącego tłumaczenia") { Title = "Błąd" }.ShowAsync();
                        return;
                    }
                }
                await MySqLiteHelper.Insert(new Words(EnglishWord, PolishWord));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                ShowNewChange("Błąd 2 przepraszamy");
            }
            finally
            {
                LoadDatabase();
            }
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            AddContentDialog MyAddPane = new AddContentDialog();
            ContentDialogResult MyContentDialogResult = await MyAddPane.ShowAsync();
            if (MyContentDialogResult == ContentDialogResult.Primary)
            {
                LoadDatabase();
            }
        }

        private async void MyListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                Words words = e.ClickedItem as Words; //zdekodowane są   ą ę ł
                if (words.Description == null)
                {
                    words.Description = "";
                }
                Debug.WriteLine(words);
                EditContentDialog MyEditContentDialog = new EditContentDialog(words);
                EditContentDialog.ContentDialogResult MyEditContentDialogResult = await MyEditContentDialog.ShowAsync();
                if(MyEditContentDialogResult == EditContentDialog.ContentDialogResult.Error)
                {
                    throw new Exception("Jest błąd przy że zwrócilo enum ContentDialogResult.Error");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                ShowNewChange("Błąd: " + ex.Message);
            }
        }

        private void SearchBox_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            if (sender.Text == "")
            {
                LoadDatabase();
                return;
            }

            try
            {
                MyListView.Items.Clear();
                foreach (Words words in MySqLiteHelper.Query(string.Format("SELECT * FROM Words WHERE PolishWord LIKE '%{0}%' OR EnglishWord LIKE '%{1}%'", WordsHelper.EnCodeWordsObject(new Words("", sender.Text.ToLower())).PolishWord, sender.Text.ToLower())))
                {
                    try
                    {
                        string EnWord = words.EnglishWord.Replace(sender.Text.ToLower(), sender.Text.ToUpper());
                        string PlWord = WordsHelper.DeCodeString(words.PolishWord);
                        PlWord = PlWord.Replace(sender.Text.ToLower(), sender.Text.ToUpper());
                        EnWord = EnWord + " ____________________________";
                        Words tempwords = new Words(EnWord, PlWord);
                        tempwords.IDWord = words.IDWord;
                        MyListView.Items.Add(tempwords);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public void RefreshDatabase(object sender, RoutedEventArgs e)
        {
            ShowNewChange("Odświerzono");
            LoadDatabase();
        }

        public void ShowNewChange(string Message)
        {
            MyDispatcherTimer.Stop();
            MyDispatcherTimer.Start();
            InfoChange.Text = Message;
        }

        private void MyDispatcherTimer_Tick(object sender, object e)
        {
            TimeOfWork++;
            if (TimeOfWork >= 3)
            {
                InfoChange.Text = "";
                TimeOfWork = 0;
                MyDispatcherTimer.Stop();
            }
        }

        private void ExamYourself(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ExamPage));
        }

        public List<Words> GetElementsFromListView()
        {
            List<Words> TempList = new List<Words>();
            foreach (Words words in MyListView.Items)
            {
                Words dasdf = new Words(words.EnglishWord, words.PolishWord);
                dasdf = WordsHelper.TrimEndAndStart_ToLower(dasdf);
                dasdf = WordsHelper.EnCodeWordsObject(dasdf);
                dasdf.EnglishWord = dasdf.EnglishWord.Trim(char.Parse("_"), char.Parse(" "));
                TempList.Add(dasdf);
            }
            return TempList;
        }

        private async void Settings_Click(object sender, RoutedEventArgs e)
        {
            SettingsContentDialog MySettingsContentDialog = new SettingsContentDialog();
            await MySettingsContentDialog.ShowAsync();
            LoadDatabase();
        }
    }
}
