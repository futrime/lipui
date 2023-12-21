using LipUI.Protocol;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LipUI.VIews;

public sealed partial class LipIndexToothView : UserControl
{

    public LipIndex.LipIndexData.LipToothItem Tooth { get; private set; }

    public LipIndexToothView(LipIndex.LipIndexData.LipToothItem tooth)
    {
        Tooth = tooth;

        this.InitializeComponent();

        ToothName.Text = tooth.Name;
        ToothDescription.Text = tooth.Description;
        AuthorButtonText.Text = tooth.Author;
    }
}
