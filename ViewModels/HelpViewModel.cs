using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Input;
using VietOCR.Views;

namespace VietOCR.ViewModels
{
    public partial class HelpViewModel : PostProcessingViewModel
    {
        [RelayCommand]
        public async void Help()
        {
            var popup = new HelpPopup();

            await Application.Current.MainPage.ShowPopupAsync(popup);
        }

        [RelayCommand]
        public async void About()
        {
            await Application.Current.MainPage.DisplayAlert("Alert", "About VietOCR .NET Maui", "OK");
        }
    }
}
