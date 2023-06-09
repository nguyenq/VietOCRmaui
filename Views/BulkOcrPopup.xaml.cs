using CommunityToolkit.Maui.Views;
using VietOCR.ViewModels;

namespace VietOCR.Views;

public partial class BulkOcrPopup : Popup
{
    public BulkOcrPopup(BulkOCRViewModel vm)
    {
		InitializeComponent();
        combobox.ItemsSource = Enum.GetNames(typeof(Tesseract.RenderedFormat));
        BindingContext = vm;
    }

    private void ButtonRun_Clicked(object sender, EventArgs e)
    {
        Close((BindingContext as BulkOCRViewModel).Options);
    }

    private void ButtonClose_Clicked(object sender, EventArgs e) => Close();
}