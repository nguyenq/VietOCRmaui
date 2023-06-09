using CommunityToolkit.Mvvm.ComponentModel;

namespace VietOCR.ViewModels
{
    public partial class RadioButtonViewModel : ObservableObject
    {
        [ObservableProperty]
        public string groupName;

        [ObservableProperty]
        public string selection;
    }
}
