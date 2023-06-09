using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace VietOCR.ViewModels
{
    public partial class BulkOCRViewModel : ObservableObject
    {
        [ObservableProperty]
        string inputDir;

        [ObservableProperty]
        string outputDir;

        [ObservableProperty]
        string outputFormat;

        public Options Options { 
            get 
            {
                Options.InputFolder = InputDir;
                Options.OutputFolder = OutputDir;
                Options.OutputFormat = OutputFormat;
                return Options;
            } 
            set 
            {
                InputDir = value.InputFolder;
                OutputDir = value.OutputFolder;
                OutputFormat = value.OutputFormat;
            } 
        }

        [RelayCommand]
        public async Task SetInputDir()
        {
            var result = await FolderPicker.Default.PickAsync(InputDir, new CancellationToken());
            if (result.Folder != null)
            {
                InputDir = result.Folder.Path;
            }
        }

        [RelayCommand]
        public async Task SetOutputDir()
        {
            var result = await FolderPicker.Default.PickAsync(OutputDir, new CancellationToken());
            if (result.Folder != null)
            {
                OutputDir = result.Folder.Path;
            }
        }
    }
}
