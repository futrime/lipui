using LipUI.Models;
using LipUI.VIews;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Reflection;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LipUI.Pages.HomePageModules;

internal class ModulesPage_Module : ILipUIModules<ModulesPage_Module>
{
    public string ModuleName => "modules$title$modulePage".GetLocalized();

    public FrameworkElement IconContent => new Viewbox();

    public Brush IconBackground => new SolidColorBrush(Colors.Transparent);

    public Type PageType => typeof(ModulesPage);

    public double IconHeight => 32;

    public double IconWidth => 32;
}

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ModulesPage : Page
{

    public ModulesPage()
    {
        InitializeComponent();

        DispatcherQueue.TryEnqueue(async () =>
        {
            await Task.Delay(500);
            foreach (var moduleType in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (
                moduleType.IsAssignableTo(typeof(ILipUIModulesNonGeneric))
                && moduleType.IsClass
                && moduleType != typeof(ModulesPage_Module)
                && moduleType.GetCustomAttribute<LipUIModuleAttribute>() is not null)
                {
                    ModulesView.Items.Add(new ModuleIcon(moduleType));
                }
            }
        });
    }

    private void ModulesView_ItemClick(object sender, ItemClickEventArgs e)
    {
        var item = e.ClickedItem as ModuleIcon;
        Frame.Navigate(item!.PageType);
    }
}
