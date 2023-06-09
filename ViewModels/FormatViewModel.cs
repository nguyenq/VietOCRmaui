using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VietOCR.Utilities;
using VietOCR.Views;

namespace VietOCR.ViewModels
{
    public partial class FormatViewModel : AppShellViewModel
    {
        [ObservableProperty]
        int cursorPosition;

        [ObservableProperty]
        int selectionLength;


        public ProcessingOptions ProcessingOptions { get; set; } = new();

        [RelayCommand]
        public async void Font()
        {
            await Application.Current.MainPage.DisplayAlert("Alert", "Font", "OK");
        }
        [RelayCommand]
        public async void ChangeCase()
        {
            var popup = new ChangCasePopup(new RadioButtonViewModel
            {
                GroupName = "lettercases",
                Selection = Options.SelectedLetterCase
            });

            string selectedCase = (string) await Application.Current.MainPage.ShowPopupAsync(popup);
            if (selectedCase != null)
            {
                Options.SelectedLetterCase = selectedCase;
                changeCase(selectedCase);
            }            
        }

        /// <summary>
        /// Changes letter case.
        /// </summary>
        /// <param name="typeOfCase"></param>
        void changeCase(string typeOfCase)
        {
            if (SelectionLength == 0)
            {
                SelectionLength = Text.Length;
                //return;
            }

            string selectedText = Text.Substring(CursorPosition, SelectionLength);
            string result = TextUtilities.ChangeCase(selectedText, typeOfCase);
            Text = Text.Remove(CursorPosition, SelectionLength).Insert(CursorPosition, result);
        }

        [RelayCommand]
        void RemoveLineBreaks()
        {
            if (SelectionLength == 0)
            {
                CursorPosition = 0;
                SelectionLength = Text.Length;
                if (SelectionLength == 0) return;
            }

            string selectedText = Text.Substring(CursorPosition, SelectionLength);
            string result = TextUtilities.RemoveLineBreaks(selectedText, ProcessingOptions.RemoveHyphens);
            Text = result;
        }
    }
}
