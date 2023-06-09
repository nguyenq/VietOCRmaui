using CommunityToolkit.Mvvm.Input;

namespace VietOCR.ViewModels
{
    public partial class ImageViewModel : FormatViewModel
    {
        [RelayCommand]
        public void PreviousImage()
        {
            if (--imageIndex < 0)
            {
                imageIndex = 0;
            }
            else
            {
                LoadImage();
            }
        }

        [RelayCommand]
        public void NextImage()
        {
            if (++imageIndex > imageList.Count - 1)
            {
                --imageIndex;
            }
            else
            {
                LoadImage();
            }
        }
    }
}
