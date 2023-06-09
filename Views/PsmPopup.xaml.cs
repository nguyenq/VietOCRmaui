using CommunityToolkit.Maui.Views;
using Tesseract;
using VietOCR.ViewModels;

namespace VietOCR.Views;

public partial class PsmPopup : Popup
{

    public PsmPopup(RadioButtonViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
        LoadPsmMenu();
    }

    public void LoadPsmMenu()
    {
        Dictionary<string, string> psmDict = new Dictionary<string, string>();
        psmDict.Add("OsdOnly", "0 - Orientation and script detection (OSD) only");
        psmDict.Add("AutoOsd", "1 - Automatic page segmentation with OSD");
        psmDict.Add("AutoOnly", "2 - Automatic page segmentation, but no OSD, or OCR");
        psmDict.Add("Auto", "3 - Fully automatic page segmentation, but no OSD (default)");
        psmDict.Add("SingleColumn", "4 - Assume a single column of text of variable sizes");
        psmDict.Add("SingleBlockVertText", "5 - Assume a single uniform block of vertically aligned text");
        psmDict.Add("SingleBlock", "6 - Assume a single uniform block of text");
        psmDict.Add("SingleLine", "7 - Treat the image as a single text line");
        psmDict.Add("SingleWord", "8 - Treat the image as a single word");
        psmDict.Add("CircleWord", "9 - Treat the image as a single word in a circle");
        psmDict.Add("SingleChar", "10 - Treat the image as a single character");
        psmDict.Add("SparseText", "11 - Find as much text as possible in no particular order");
        psmDict.Add("SparseTextOsd", "12 - Sparse text with orientation and script detection");
        psmDict.Add("RawLine", "13 - Treat the image as a single text line, bypassing hacks that are Tesseract-specific");
        psmDict.Add("Count", "14 - Number of enum entries");

        foreach (string mode in Enum.GetNames(typeof(PageSegMode)))
        {
            if ((PageSegMode)Enum.Parse(typeof(PageSegMode), mode) == PageSegMode.Count)
            {
                continue;
            }
            RadioButton psmRadioBtn = new RadioButton { Content = psmDict[mode], Value = mode };
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