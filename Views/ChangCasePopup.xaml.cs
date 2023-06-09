using CommunityToolkit.Maui.Views;
using VietOCR.ViewModels;

namespace VietOCR.Views;

public partial class ChangCasePopup : Popup
{
	public ChangCasePopup(RadioButtonViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }

    private void ButtonChange_Clicked(object sender, EventArgs e)
    {
        Close((BindingContext as RadioButtonViewModel).Selection);
    }

    private void ButtonClose_Clicked(object sender, EventArgs e) => Close();
}