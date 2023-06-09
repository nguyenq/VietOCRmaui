using CommunityToolkit.Mvvm.Input;
using VietOCR.NET.Postprocessing;

namespace VietOCR.ViewModels
{
    public partial class PostProcessingViewModel : OcrViewModel
    {
        [RelayCommand]
        public void PostProcess()
        {
            Text = Processor.PostProcess(Text, Options.CurLangCode, ProcessingOptions.DangAmbigsPath, ProcessingOptions.DangAmbigsEnabled, ProcessingOptions.ReplaceHyphens);
        }
    }
}
