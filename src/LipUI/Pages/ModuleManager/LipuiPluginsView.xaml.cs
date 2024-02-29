using LipUI.Models;
using LipUI.Models.Plugin;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System.Reflection;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LipUI.Pages.ModuleManager;

public sealed partial class LipuiPluginsView : UserControl
{
    private readonly Style CaptionTextBlockStyle = (Style)Application.Current.Resources["CaptionTextBlockStyle"];
    private readonly Brush TextFillColorPrimaryBrush = (Brush)Application.Current.Resources["TextFillColorPrimaryBrush"];
    private readonly Brush TextFillColorSecondaryBrush = (Brush)Application.Current.Resources["TextFillColorSecondaryBrush"];

    private readonly IEnumerable<IPlugin> plugins;

    public LipuiPluginsView(IEnumerable<IPlugin> plugins)
    {
        InitializeComponent();

        this.plugins = plugins;
    }

    public async ValueTask InitializeUIAsync()
    {

        Dictionary<Assembly, List<IPlugin>> assemblyAndPlugins = [];

        foreach (var plugin in plugins)
        {
            try
            {
                var assembly = plugin.GetType().Assembly;
                if (assemblyAndPlugins.TryGetValue(assembly, out var list) is false)
                    assemblyAndPlugins[assembly] = list = [];

                list.Add(plugin);
            }
            catch (Exception ex)
            {
                await InternalServices.ShowInfoBarAsync(ex);
                continue;
            }
        }

        foreach (var (asm, pluginList) in assemblyAndPlugins)
        {
            foreach (var plugin in pluginList)
            {
                try
                {
                    var pluginNameTextBlock = new TextBlock
                    {
                        Style = CaptionTextBlockStyle,
                        Foreground = TextFillColorPrimaryBrush,
                        Text = plugin.PluginName,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    var assemblyNameTextBlock = new TextBlock
                    {
                        Style = CaptionTextBlockStyle,
                        Foreground = TextFillColorSecondaryBrush,
                        Text = Path.GetFileName(asm.Location),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    var assemblyPathTextBlock = new TextBlock
                    {
                        Style = CaptionTextBlockStyle,
                        Foreground = TextFillColorSecondaryBrush,
                        Text = asm.Location,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    var guidTextBlock = new TextBlock
                    {
                        Style = CaptionTextBlockStyle,
                        Foreground = TextFillColorSecondaryBrush,
                        Text = plugin.Guid.ToString(),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    var swich = new ToggleSwitch()
                    {
                        IsOn = PluginSystem.IsPluginEnabled(plugin),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    swich.Toggled += (sender, e) =>
                    {
                        if (((ToggleSwitch)sender).IsOn)
                            PluginSystem.EnablePlugin(plugin);
                        else
                            PluginSystem.DisablePlugin(plugin);
                    };
                    Grid.SetColumn(pluginNameTextBlock, 0);
                    Grid.SetRow(pluginNameTextBlock, 0);
                    Grid.SetRowSpan(pluginNameTextBlock, 2);

                    Grid.SetColumn(assemblyNameTextBlock, 1);
                    Grid.SetRow(assemblyNameTextBlock, 0);
                    Grid.SetRowSpan(assemblyNameTextBlock, 2);

                    Grid.SetColumn(assemblyPathTextBlock, 2);
                    Grid.SetRow(assemblyNameTextBlock, 0);

                    Grid.SetColumn(guidTextBlock, 2);
                    Grid.SetRow(guidTextBlock, 1);

                    Grid.SetColumn(swich, 3);
                    Grid.SetRow(swich, 0);
                    Grid.SetRowSpan(swich, 2);

                    var grid = new Grid()
                    {
                        Margin = new(4),
                        RowSpacing = 2,
                        ColumnSpacing = 2,
                        RowDefinitions =
                        {
                            new() { Height = new(1, GridUnitType.Star) },
                            new() { Height = new(1, GridUnitType.Star) }
                        },
                        ColumnDefinitions =
                        {
                            new() { Width = new(1, GridUnitType.Star) },
                            new() { Width = new(1, GridUnitType.Star) },
                            new() { Width = new(4, GridUnitType.Star) },
                            new() { Width = new(2, GridUnitType.Star) },
                        },
                        Children =
                        {
                            pluginNameTextBlock,
                            assemblyNameTextBlock,
                            assemblyPathTextBlock,
                            guidTextBlock,
                            swich
                        }
                    };
                    PluginsListView.Items.Add(grid);
                }
                catch (Exception ex)
                {
                    await InternalServices.ShowInfoBarAsync(ex);
                    continue;
                }
            }
        }
    }
}
