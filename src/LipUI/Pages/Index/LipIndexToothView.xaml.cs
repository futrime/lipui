using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using static LipUI.Protocol.LipIndex.LipIndexData;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LipUI.Pages.Index;

public sealed partial class LipIndexToothView : UserControl
{

    public LipToothItem Tooth { get; private set; }

    private readonly Action<string> onClick;

    public LipIndexToothView(LipToothItem tooth, Action<string> authorButtonClickHandler)
    {
        Tooth = tooth;

        InitializeComponent();

        ToothName.Text = tooth.Name;
        ToothDescription.Text = tooth.Description;
        AuthorButtonText.Text = tooth.Author;
        onClick = authorButtonClickHandler;
    }

    private void AuthorButton_Click(object sender, RoutedEventArgs e)
        => onClick(Tooth.Author);
}
