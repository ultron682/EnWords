using System;
using System.Diagnostics;
using Windows.UI.Xaml.Controls;
using EnWords.Models;
using System.Collections.Generic;
using EnWords.Helpers;

namespace EnWords.Views_ContentDialogs
{
    public sealed partial class AddContentDialog : ContentDialog
    {
        private SqLiteHelper MySqLiteHelper;

        public AddContentDialog()
        {
            this.InitializeComponent();
            MySqLiteHelper = new SqLiteHelper();
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (PolishField.Text != "" && EnglishField.Text != "")
            {
                try
                {
                    Words words = new Words(EnglishField.Text, PolishField.Text);
                    words.Description = WordsHelper.EnCodeString(DescriptionField.Text);
                    words = WordsHelper.TrimEndAndStart_ToLower(words);
                    words = WordsHelper.EnCodeWordsObject(words);

                    await MySqLiteHelper.Insert(words);
                    Debug.WriteLine("Dodano: " + words);
                    Views.MainPage.MainPageObject.LoadDatabase();
                    Hide();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Hide();
        }

        private void PolishField_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
                ContentDialog_PrimaryButtonClick(null, null);
        }
    }
}
