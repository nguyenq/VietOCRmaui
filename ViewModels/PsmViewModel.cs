using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Input;
using VietOCR.Views;

namespace VietOCR.ViewModels
{
    public partial class PsmViewModel : ImageViewModel
    {
        string psmValue = "Auto"; // 3 - Fully automatic page segmentation, but no OSD (default);
        string oemValue = "Default";

        [RelayCommand]
        public async void Psm()
        {
            var popup = new PsmPopup(new RadioButtonViewModel
            {
                GroupName = "psm",
                Selection = psmValue
            });

            psmValue = (string) await Application.Current.MainPage.ShowPopupAsync(popup) ?? psmValue;
        }

        [RelayCommand]
        public async void Oem()
        {
            var popup = new OemPopup(new RadioButtonViewModel
            {
                GroupName = "oem",
                Selection = oemValue
            });

            oemValue = (string) await Application.Current.MainPage.ShowPopupAsync(popup) ?? oemValue;
        }
    }
}
