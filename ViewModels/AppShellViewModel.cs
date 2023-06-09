using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text;
using IImage = Microsoft.Maui.Graphics.IImage;

using Microsoft.Maui.Graphics.Skia;
using VietOCR.Utilities;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Maui.Alerts;
using System.Collections.ObjectModel;
using VietOCR.NET.Utilities;

namespace VietOCR.ViewModels
{
    public partial class AppShellViewModel : ObservableObject
    {
        [ObservableProperty]
        string text = string.Empty;

        [ObservableProperty]
        string inputfilename;

        [ObservableProperty]
        string textFilename;

        [ObservableProperty]
        ImageSource imageSource;

        [ObservableProperty]
        IImage currentImage;

        [ObservableProperty]
        float imageWidth;

        [ObservableProperty]
        float imageHeight;

        [ObservableProperty]
        bool isBusy;

        public ObservableCollection<string> MruList { get; } = new();

        public Options Options { get; set; }

        private int filterIndex;
        private string strClearRecentFiles;
        const string strFilterIndex = "FilterIndex";
        const string strMruList = "MruList";
        bool textModified;

        protected int imageIndex;

        protected IList<IImage> imageList = new List<IImage>();

        //public AppShellViewModel()
        //{
        //}

        [RelayCommand]
        public async Task OpenFileAsync()
        {
            var customFileType = new FilePickerFileType(
                new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.WinUI, new[] { ".jpg", ".png", ".tif", ".pdf", ".txt" } }, // file extension
                });

            PickOptions options = new()
            {
                PickerTitle = "Please select an image file",
                FileTypes = customFileType,
            };

            var result = await FilePicker.Default.PickAsync(options);
            if (result != null)
            {
                if (result.FileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                    result.FileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                    result.FileName.EndsWith(".tif", StringComparison.OrdinalIgnoreCase) ||
                    result.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) ||
                    result.FileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                {
                    await OpenFile(result.FullPath);
                }
            }
        }

        /// <summary>
        /// Opens image or text file.
        /// </summary>
        /// <param name="selectedFile"></param>
        public async Task OpenFile(string selectedFile)
        {
            if (!File.Exists(selectedFile))
            {
                //MessageBox.Show(this, Properties.Resources.File_not_exist, strProgName, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // if text file, load it into textbox
            if (selectedFile.EndsWith(".txt"))
            {
                //if (!OkToTrash())
                //    return;

                try
                {
                    using (StreamReader sr = new(selectedFile, Encoding.UTF8, true))
                    {
                        //textModified = false;
                        Text = sr.ReadToEnd();
                        UpdateMRUList(selectedFile);
                        TextFilename = selectedFile;
                    }
                }
                catch (Exception e)
                {
                    await Application.Current.MainPage.DisplayAlert("Alert", e.Message, "OK");
                }
                return;
            }

            List<string> imageFiles = new List<string>();

            if (selectedFile.EndsWith(".tif"))
            {
                List<string> pngFiles = TiffHelper.TiffToBitmap(selectedFile);
                imageFiles.AddRange(pngFiles);
            }
            else if (selectedFile.EndsWith(".pdf"))
            {
                string workingTiffFileName = PdfUtilities.ConvertPdf2TiffGS(selectedFile);
                List<string> pngFiles = TiffHelper.TiffToBitmap(workingTiffFileName);
                imageFiles.AddRange(pngFiles);
            }
            else
            {
                imageFiles.Add(selectedFile);
            }

            //this.statusLabel.Content = Properties.Resources.Loading_image;
            //this.Cursor = Cursors.Wait;
            ////this.pictureBox1.UseWaitCursor = true;
            //this.textBox1.Cursor = Cursors.Wait;
            //this.buttonOCR.IsEnabled = false;
            //this.oCRToolStripMenuItem.IsEnabled = false;
            //this.oCRAllPagesToolStripMenuItem.IsEnabled = false;
            //this.toolStripProgressBar1.IsEnabled = true;
            //this.toolStripProgressBar1.Visibility = Visibility.Visible;
            //this.toolStripProgressBar1.Style = ProgressBarStyle.Marquee;

            //this.backgroundWorkerLoad.RunWorkerAsync(selectedFile);
            imageList.Clear();
            imageIndex = 0;

            imageFiles.ForEach(filename => {
                using Stream stream = File.OpenRead(filename);
                imageList.Add(SkiaImage.FromStream(stream));
                Inputfilename = filename;
            });

            LoadImage();

            UpdateMRUList(selectedFile);
        }

        protected void LoadImage()
        {
            CurrentImage = imageList[imageIndex];
            ImageWidth = CurrentImage.Width;
            ImageHeight = CurrentImage.Height;
            ImageSource = ImageSource.FromStream(() => CurrentImage.AsStream());
        }

        /// <summary>
        /// Update MRU List.
        /// </summary>
        /// <param name="fileName"></param>
        private void UpdateMRUList(string fileName)
        {
            if (MruList.Contains(fileName))
            {
                MruList.Remove(fileName);
            }

            MruList.Insert(0, fileName);

            if (MruList.Count > 10)
            {
                MruList.RemoveAt(10);
            }

            //updateMRUMenu();
        }

        [RelayCommand]
        protected async Task<bool> Save(CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(TextFilename))
            {
                return await SaveFileDlg(cancellationToken);
            }
            else
            {
                return await SaveTextFile(TextFilename);
            }
        }

        [RelayCommand]
        protected async Task<bool> SaveAs(CancellationToken cancellationToken)
        {
            return await SaveFileDlg(cancellationToken);
        }

        async Task<bool> SaveFileDlg(CancellationToken cancellationToken)
        {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(Text));
            var fileSaverResult = await FileSaver.Default.SaveAsync(".txt", stream, cancellationToken);
            if (fileSaverResult.IsSuccessful)
            {
                await Toast.Make($"File is saved: {fileSaverResult.FilePath}").Show(cancellationToken);
                TextFilename = fileSaverResult.FilePath;
            }
            else
            {
                await Toast.Make($"The file was not saved successfully with error: {fileSaverResult.Exception.Message}").Show(cancellationToken);
            }

            return fileSaverResult.IsSuccessful;
        }

        async Task<bool> SaveTextFile(string fileName)
        {
            //this.Cursor = Cursors.Wait;

            try
            {
                using StreamWriter sw = new(fileName, false, new UTF8Encoding());
                sw.Write(Text);
                UpdateMRUList(fileName);
            }
            catch (Exception exc)
            {
                await Application.Current.MainPage.DisplayAlert("Alert", exc.Message, "OK");
            }

            textModified = false;
            //this.textBox1.Modified = false;
            //this.Cursor = null;

            return true;
        }
       
        [RelayCommand]
        public async void ImageProperties()
        {
            await Application.Current.MainPage.DisplayAlert("Alert", "ImageProperties", "OK");
        }

        [RelayCommand]
        public void Exit()
        {
            Application.Current.Quit();
        }
    }
}
