using LipUI.Pages.Home.Modules;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace LipUI.Models.Plugin;

public interface IHomePageModule : IPlugin
{
    public Type PageType { get; }

    public FrameworkElement? IconContent { get => null; }

    public Brush? IconBackground { get => null; }

    public void OnIconInitialze(ModuleIcon icon) { }

    public void OnExit() { }
}
