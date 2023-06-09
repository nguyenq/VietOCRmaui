using CommunityToolkit.Mvvm.Input;


namespace VietOCR.ViewModels
{
    public partial class HelpViewModel : PostProcessingViewModel
    {
        [RelayCommand]
        public async void Help()
        {
            await Application.Current.MainPage.DisplayAlert("Alert", "Help", "OK");
        }

        [RelayCommand]
        public async void About()
        {
            await Application.Current.MainPage.DisplayAlert("Alert", "About VietOCR .NET Maui", "OK");
        }
    }
}
