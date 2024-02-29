using LipUI.Models;
using LipUI.Models.Plugin;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LipUI.Pages.Home.Modules;

internal class ModulesPage_Module : IHomePageModule
{

    public FrameworkElement IconContent => new Viewbox();

    public Brush IconBackground => new SolidColorBrush(Colors.Transparent);

    public Type PageType => typeof(ModulesPage);

    public string PluginName => "modules$title$modulePage".GetLocalized();

    public bool DefaultEnabled => false;

    public Guid Guid => default;
}

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ModulesPage : Page
{
    public static void InitEventHandlers()
    {
        PluginSystem.PluginEnabled += PluginSystem_PluginEnabled;
        PluginSystem.PluginDisabled += PluginSystem_PluginDisabled;
    }

    private static readonly HashSet<IHomePageModule> enabledModules = new();
    private static void PluginSystem_PluginEnabled(IPlugin obj)
    {
        lock (enabledModules)
        {
            if (obj is IHomePageModule module)
                enabledModules.Add(module);
        }
    }

    private static void PluginSystem_PluginDisabled(IPlugin obj)
    {
        lock (enabledModules)
        {
            if (obj is IHomePageModule module)
                enabledModules.Remove(module);
        }
    }

    public ModulesPage()
    {
        InitializeComponent();

        DispatcherQueue.TryEnqueue(async () =>
        {
            await Task.Delay(500);

            foreach (var module in enabledModules)
            {
                ModulesView.Items.Add(new ModuleIcon(module));
            }
        });
    }

    private void ModulesView_ItemClick(object sender, ItemClickEventArgs e)
    {
        var item = e.ClickedItem as ModuleIcon;
        Frame.Navigate(item!.PageType);
    }
}
