using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LipUI.Pages.Home.Modules;

//[LipUIModule]
//internal class AllowListViewerPage_Module : ILipUIModules<AllowListViewerPage_Module>
//{
//    public string ModuleName => "modules$title$allowListViewer".GetLocalized();

//    public Type PageType => typeof(AllowListViewerPage);

//    public FrameworkElement? IconContent
//        => new SymbolIcon(Symbol.ContactInfo) { Height = 32, Width = 32 };

//    public Brush? IconBackground
//    {
//        get
//        {
//            var color = Colors.AliceBlue;
//            color.A = 200;
//            return new SolidColorBrush(color);
//        }
//    }
//}

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
internal sealed partial class AllowListViewerPage : Page
{
    public AllowListViewerPage()
    {
        InitializeComponent();
    }
}
