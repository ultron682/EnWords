using Windows.Storage;
using System;
using Windows.UI.Xaml.Controls;
using EnWords.Helpers;
using EnWords.Views;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using System.Collections.Generic;
using System.Diagnostics;

namespace EnWords.Views_ContentDialogs
{
    public sealed partial class SettingsContentDialog : ContentDialog
    {
        private SqLiteHelper MySqLiteHelper = new SqLiteHelper();
        private ApplicationDataContainer MyApplicationDataContainerSettings = ApplicationData.Current.LocalSettings;

        public SettingsContentDialog()
        {
            this.InitializeComponent();
            try
            {
                SortComboBox.SelectedIndex = (int)MyApplicationDataContainerSettings.Values["SortMethod"];
                ExamComboBox.SelectedIndex = (int)MyApplicationDataContainerSettings.Values["ExamAnswerMethod"];
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            MyApplicationDataContainerSettings.Values["SortMethod"] = SortComboBox.SelectedIndex;
            MyApplicationDataContainerSettings.Values["ExamAnswerMethod"] = ExamComboBox.SelectedIndex;
            if (DeleteCheckBox.IsChecked == true)
            {
                MySqLiteHelper.DeleteAll();
                MainPage.MainPageObject.ShowNewChange("Wymazano dane");
                MainPage.MainPageObject.LoadDatabase();
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Hide();
        }

        private async void CreateBackupButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await CreateBackup();
        }

        private async void ReadBackupButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await ReadBackup();
        }

        private async Task CreateBackup()
        {
            try
            {
                StorageFile MyStorageFile = await ApplicationData.Current.LocalFolder.GetFileAsync("MyDatabase.db");

                FileSavePicker MyFileSavePicker = new FileSavePicker();
                MyFileSavePicker.SuggestedStartLocation = PickerLocationId.Desktop;
                MyFileSavePicker.SuggestedFileName = "EnWords " + DateTime.Now.Day + "." + DateTime.Now.Month + "." + DateTime.Now.Year;
                MyFileSavePicker.FileTypeChoices.Add("Plik EnWords", new List<string> { ".enwords" });
                StorageFile MySF = await MyFileSavePicker.PickSaveFileAsync();
                if (MySF != null)
                {
                    await MyStorageFile.CopyAndReplaceAsync(MySF);
                    MainPage.MainPageObject.ShowNewChange("Utworzono kopię zapasową");
                    MainPage.MainPageObject.LoadDatabase();
                    Hide();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private async Task ReadBackup()
        {
            try
            {
                FileOpenPicker MyFileOpenPicker = new FileOpenPicker();
                MyFileOpenPicker.FileTypeFilter.Add(".enwords");
                MyFileOpenPicker.SuggestedStartLocation = PickerLocationId.Desktop;
                StorageFile MyStorageFile = await MyFileOpenPicker.PickSingleFileAsync();
                if (MyStorageFile != null)
                {
                    StorageFile MySF = await ApplicationData.Current.LocalFolder.CreateFileAsync("MyDatabase.db", CreationCollisionOption.OpenIfExists);
                    await MyStorageFile.CopyAndReplaceAsync(MySF);
                    MainPage.MainPageObject.ShowNewChange("Wczytano kopię zapasową");
                    MainPage.MainPageObject.LoadDatabase();
                }
                Hide();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }
    }
}
