using CommunityToolkit.Maui.Views;
using Tesseract;
using VietOCR.ViewModels;

namespace VietOCR.Views;

public partial class HelpPopup : Popup
{

    public HelpPopup()
	{
		InitializeComponent();
        this.webView.Reload();
    }

    private void ButtonClose_Clicked(object sender, EventArgs e) => Close();
}