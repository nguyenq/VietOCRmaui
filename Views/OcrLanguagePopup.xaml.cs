using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Xml;
using Tesseract;
using VietOCR.Models;
using VietOCR.ViewModels;

namespace VietOCR.Views;

public partial class OcrLanguagePopup : Popup
{ 
    public OcrLanguagePopup(OcrLanguageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }    

    private void ButtonChange_Clicked(object sender, EventArgs e)
    {
        //IList<string> selected = this.languageSelector.SelectedItems.Cast<OcrLanguage>().Select(x => x.Code).ToList();
        var vm = BindingContext as OcrLanguageViewModel;
        ObservableCollection<object> selected = vm.SelectedLanguages;
        Close(selected.Select(x => ((OcrLanguage)x).Code).ToList());
    }

    private void ButtonClose_Clicked(object sender, EventArgs e) => Close();
}