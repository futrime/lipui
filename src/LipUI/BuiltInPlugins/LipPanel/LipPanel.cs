using LipUI.Models.Plugin;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace LipUI.BuiltInPlugins.LipPanel;

//[LipUIModule]
internal class LipPanel : IHomePageModule
{
    public Type PageType => typeof(LipPanelPage);

    public string PluginName => "LipPanel";

    public bool DefaultEnabled => true;

    public Guid Guid => new("21A13C74-BEC2-41BE-782F-1EF4BD7701AB");

    FrameworkElement IHomePageModule.IconContent => new SymbolIcon(Symbol.Remote)
    {
        Height = 32,
        Width = 32,
    };

    Brush IHomePageModule.IconBackground =>
        LipuiServices.ApplicationTheme is ApplicationTheme.Light ?
        new SolidColorBrush(Colors.LightGray) :
        new SolidColorBrush(Colors.DarkGray);
}
