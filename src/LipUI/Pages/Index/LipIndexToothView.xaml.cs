using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.UI;
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

        var style = Application.Current.Resources["CaptionTextBlockStyle"] as Style;
        var foreground = new SolidColorBrush((Color)Application.Current.Resources["TextFillColorTertiary"]);

        foreach (var tag in tooth.Tags)
            ToothTags.Children.Add(new TextBlock()
            {
                Text = tag,
                Style = style,
                Foreground = foreground
            });
    }

    private void AuthorButton_Click(object sender, RoutedEventArgs e)
        => onClick(Tooth.Author);
}
