using LipUI.Models.Plugin;
using LipUI.Pages.ModuleManager;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LipUI.Pages.ToothPack;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ModuleManagerPage : Page
{
    [Flags]
    private enum Selection { Plugins, UIPlugins, Modules }

    private static Selection currentSelection = Selection.Plugins;

    public ModuleManagerPage()
    {
        InitializeComponent();
    }

    private void PluginsButton_Click(object sender, RoutedEventArgs e)
        => Plugins_Selection();

    private void Plugins_Selection()
    {
        currentSelection = Selection.Plugins;

        if (PluginSystem.Plugins is null) return;

        ModulesTreeView.Content = new ProgressRing();
        DispatcherQueue.TryEnqueue(async () =>
        {
            var view = new LipuiPluginsView(PluginSystem.Plugins);
            await view.InitializeUIAsync();
            ModulesTreeView.Content = view;
        });
    }

    private void UIPluginsButton_Click(object sender, RoutedEventArgs e)
        => UIPlugins_Selection();

    private void UIPlugins_Selection()
    {
        currentSelection = Selection.UIPlugins;

        if (PluginSystem.UIPlugins is null) return;

        ModulesTreeView.Content = new ProgressRing();
        DispatcherQueue.TryEnqueue(async () =>
        {
            var view = new LipuiPluginsView(PluginSystem.UIPlugins);
            await view.InitializeUIAsync();
            ModulesTreeView.Content = view;
        });
    }

    private void ModulesButton_Click(object sender, RoutedEventArgs e)
        => Modules_Selection();

    private void Modules_Selection()
    {
        currentSelection = Selection.Modules;

        if (PluginSystem.Modules is null) return;

        ModulesTreeView.Content = new ProgressRing();
        DispatcherQueue.TryEnqueue(async () =>
        {
            var view = new LipuiPluginsView(PluginSystem.Modules);
            await view.InitializeUIAsync();
            ModulesTreeView.Content = view;
        });
    }

    private void Page_Loading(FrameworkElement sender, object args)
    {
        switch (currentSelection)
        {
            case Selection.Plugins:
                Plugins_Selection();
                break;
            case Selection.UIPlugins:
                UIPlugins_Selection();
                break;
            case Selection.Modules:
                Modules_Selection();
                break;
        }
    }
}
