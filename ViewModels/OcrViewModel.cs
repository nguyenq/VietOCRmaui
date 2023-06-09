using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text;
using Tesseract;
using VietOCR.Views;

namespace VietOCR.ViewModels
{
    public partial class OcrViewModel : PsmViewModel
    {
        [ObservableProperty]
        public string curLangCode = "vie";

        [RelayCommand]
        void OCR()
        {
            IsBusy = true;
            OCR<Microsoft.Maui.Graphics.IImage> ocrEngine = new OCRImages();
            ocrEngine.PageSegMode = PageSegMode.Auto.ToString();
            ocrEngine.OcrEngineMode = EngineMode.Default.ToString();
            ocrEngine.Language = Options.CurLangCode;

            // Assign the result of the computation to the Result property of the DoWorkEventArgs
            // object. This is will be available to the RunWorkerCompleted eventhandler.
            //e.Result = ocrEngine.RecognizeText(entity.ClonedImages, entity.Lang, entity.Rect, worker, e);

            if (imageList == null || !imageList.Any())
            {
                IsBusy = false;
                return;
            }

            string str = ocrEngine.RecognizeText(((List<Microsoft.Maui.Graphics.IImage>)imageList).GetRange(imageIndex, 1), TextFilename);
            Text += str;
            IsBusy = false;
        }

        [RelayCommand]
        void OCRAll()
        {
            IsBusy = true;
            OCR<Microsoft.Maui.Graphics.IImage> ocrEngine = new OCRImages();
            ocrEngine.PageSegMode = PageSegMode.Auto.ToString();
            ocrEngine.OcrEngineMode = EngineMode.Default.ToString();
            ocrEngine.Language = Options.CurLangCode;

            // Assign the result of the computation to the Result property of the DoWorkEventArgs
            // object. This is will be available to the RunWorkerCompleted eventhandler.
            //e.Result = ocrEngine.RecognizeText(entity.ClonedImages, entity.Lang, entity.Rect, worker, e);
            if (imageList == null || !imageList.Any())
            {
                IsBusy = false;
                return;
            }

            StringBuilder strB = new();

            for (int i = 0; i < imageList.Count; i++)
            {
                strB.Append(ocrEngine.RecognizeText(((List<Microsoft.Maui.Graphics.IImage>)imageList).GetRange(i, 1), TextFilename));
            }
            Text += strB.ToString();
            IsBusy = false;
        }

        string bulkOCRValue = "";

        [RelayCommand]
        async void BulkOCR()
        {
            var popup = new BulkOcrPopup(new BulkOCRViewModel { Options = Options });

            Options = (Options)await Application.Current.MainPage.ShowPopupAsync(popup) ?? Options;
        }

        [RelayCommand]
        void Clear()
        {
            Text = string.Empty;
        }

        [RelayCommand]
        async Task ChangeOCRLanguageAsync()
        {
            var popup = new OcrLanguagePopup(new OcrLanguageViewModel
            {
                SelectedLanguageCodes = Options.CurLangCodes
            });

            var results = (IList<string>) await Application.Current.MainPage.ShowPopupAsync(popup);
            if (results != null && results.Any())
            {
                Options.CurLangCodes = results;
            }
        }
    }
}
