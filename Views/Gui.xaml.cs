/**
 * Copyright @ 2022 Quan Nguyen
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *  http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/

using IImage = Microsoft.Maui.Graphics.IImage;
using VietOCR.ViewModels;
using System.Collections.Specialized;
using System.Text;
using System.Collections.ObjectModel;

namespace VietOCR.Views;

public partial class Gui : ContentPage
{
    public const string strProgName = "VietOCR";
    public const string TO_BE_IMPLEMENTED = "To be implemented";

    protected string textFilename;
    protected string inputfilename;
    protected IList<IImage> imageList = new List<IImage>();
    protected int imageIndex;
    protected int imageTotal;
    protected bool textModified;
    const string strClearRecentFiles = "Clear_Recent_Files";
    const string strNoRecentFiles = "No_Recent_Files";
    const string strFilterIndex = "FilterIndex";
    const string strMruList = "MruList";
    const string strInputFolder = "InputFolder";
    const string strBulkOutputFolder = "BulkOutputFolder";
    const string strBulkOutputFormat = "BulkOutputFormat";
    const string strBulkDeskewEnable = "BulkDeskewEnable";
    const string strSelectedCase = "SelectedCase";
    const string strOcrLanguage = "OcrLanguage";

    private string outputFormat;
    protected bool bulkDeskewEnabled;

    Options options;

    protected float scaleX = 1f;
    protected float scaleY = 1f;

    private int filterIndex;
    readonly ObservableCollection<string> MruList;

    protected Editor Editor => editor;
    protected MenuFlyoutSubItem RecentFilesMenuFlyoutSubItem => recentFilesMenuFlyoutSubItem;

    public Gui(HelpViewModel vm)
	{
		InitializeComponent();
        options = new Options();
        BindingContext = vm;
        vm.Options = options;
        MruList = vm.MruList;
        vm.MruList.CollectionChanged += OnCollectionChanged;

        //image.Source = ImageSource.FromResource("VietOCR.Resources.Images.dotnet_bot.png");
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        LoadRegistryInfo();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        SaveRegistryInfo();
    }

    private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        updateMRUMenu();
    }

    protected bool OkToTrash()
    {
        if (!textModified)
        {
            return true;
        }

        string action = DisplayActionSheet("Do_you_want_to_save_the_changes_to_?", "Cancel", null, "Yes", "No").Result;

        switch (action)
        {
            case "Yes":
                //return saveAction().Result;
            case "No":
                return true;
            case "Cancel":
                return false;
        }
        return false;
    }

    protected string FileTitle()
    {
        return (this.textFilename != null && this.textFilename.Length > 1) ?
            Path.GetFileName(this.textFilename) : "Untitled";
    }

    /// <summary>
    /// Update MRU Submenu.
    /// </summary>
    private void updateMRUMenu()
    {
        RecentFilesMenuFlyoutSubItem.Clear();

        if (!MruList.Any())
        {
            RecentFilesMenuFlyoutSubItem.Add(new MenuFlyoutItem { Text = strNoRecentFiles });
        }
        else
        {
            EventHandler eh = new EventHandler(MenuRecentFilesOnClick);

            foreach (string fileName in MruList)
            {
                MenuFlyoutItem item = new MenuFlyoutItem { Text = fileName };
                RecentFilesMenuFlyoutSubItem.Add(item);
                item.Clicked += eh;
            }
            RecentFilesMenuFlyoutSubItem.Add(new MenuFlyoutSeparator());
            MenuFlyoutItem clearItem = new() { Text = strClearRecentFiles };
            RecentFilesMenuFlyoutSubItem.Add(clearItem);
            clearItem.Clicked += eh;
        }
    }

    async void MenuRecentFilesOnClick(object obj, EventArgs ea)
    {
        MenuFlyoutItem item = (MenuFlyoutItem)obj;
        string fileName = item.Text;

        if (fileName == strClearRecentFiles)
        {
            MruList.Clear();
            RecentFilesMenuFlyoutSubItem.Clear();
            RecentFilesMenuFlyoutSubItem.Add(new MenuFlyoutItem { Text = strNoRecentFiles });
        }
        else
        {
            await (BindingContext as AppShellViewModel).OpenFile(fileName);
            if (!File.Exists(fileName))
            {
                MruList.Remove(fileName);
                RecentFilesMenuFlyoutSubItem.Remove(item);
            }
        }
    }

    protected virtual void LoadRegistryInfo()
    {
        //base.LoadRegistryInfo();
        filterIndex = Preferences.Default.Get(strFilterIndex, 1);

        string[] fileNames = Preferences.Default.Get(strMruList, string.Empty).Split(new[] { Path.PathSeparator }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string fileName in fileNames)
        {
            MruList.Add(fileName);
        }
        updateMRUMenu();

        options.InputFolder = Preferences.Default.Get(strInputFolder, Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
        if (!Directory.Exists(options.InputFolder)) options.InputFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        options.OutputFolder = Preferences.Default.Get(strBulkOutputFolder, Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
        if (!Directory.Exists(options.OutputFolder)) options.OutputFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        outputFormat = Preferences.Default.Get(strBulkOutputFormat, "text");
        options.SelectedLetterCase = Preferences.Default.Get(strSelectedCase, string.Empty);

        options.OutputFormat = outputFormat;

        string selectedLangCode = Preferences.Default.Get(strOcrLanguage, "eng");
        options.CurLangCodes = new ObservableCollection<string>(selectedLangCode.Split('+'));
    }

    protected virtual void SaveRegistryInfo()
    {
        //base.SaveRegistryInfo();
        Preferences.Default.Set(strFilterIndex, filterIndex);
        StringBuilder strB = new();
        foreach (string name in MruList)
        {
            strB.Append(name).Append(Path.PathSeparator);
        }
        Preferences.Default.Set(strMruList, strB.ToString());
        Preferences.Default.Set(strInputFolder, options.InputFolder);
        Preferences.Default.Set(strBulkOutputFolder, options.OutputFolder);
        Preferences.Default.Set(strBulkOutputFormat, outputFormat);
        Preferences.Default.Set(strBulkDeskewEnable, Convert.ToInt32(bulkDeskewEnabled));
        Preferences.Default.Set(strSelectedCase, options.SelectedLetterCase);

        Preferences.Default.Set(strOcrLanguage, String.Join("+", options.CurLangCodes));
    }
}