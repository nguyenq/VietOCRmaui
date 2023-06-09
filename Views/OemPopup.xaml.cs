using CommunityToolkit.Maui.Views;
using Tesseract;
using VietOCR.ViewModels;

namespace VietOCR.Views;

public partial class OemPopup : Popup
{
    public OemPopup(RadioButtonViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
        LoadOemMenu();
    }

    public void LoadOemMenu()
    {
        Dictionary<string, string> oemDict = new Dictionary<string, string>();
        oemDict.Add("TesseractOnly", "0 - Legacy Engine Only");
        oemDict.Add("LstmOnly", "1 - LSTM Engine Only");
        oemDict.Add("TesseractAndLstm", "2 - Legacy & LSTM Engines");
        oemDict.Add("Default", "3 - Default");

        foreach (string mode in Enum.GetNames(typeof(EngineMode)))
        {
            RadioButton psmRadioBtn = new RadioButton { Content = oemDict[mode], Value = mode };
            Application.Current.Dispatcher.Dispatch(() =>
            {
                VLayout.Add(psmRadioBtn);
            });
        }
    }

    private void ButtonChange_Clicked(object sender, EventArgs e)
    {
        Close((BindingContext as RadioButtonViewModel).Selection);
    }

    private void ButtonClose_Clicked(object sender, EventArgs e) => Close();
}