using LipUI.Models.Plugin;
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LipUI.Pages.Home.Modules;

public sealed partial class ModuleIcon : UserControl
{

    internal static Dictionary<Type, ILipuiPluginModule> Modules { get; private set; } = new();

    public Type? PageType { get; private set; }

    public ModuleIcon(ILipuiPluginModule module)
    {
        InitializeComponent();

        try
        {

            if (module is not null)
            {
                PageType = module.PageType;

                ModuleName.Text = module.PluginName;

                var icon = module.IconContent;
                IconView.Child = icon;

                ElementBorder.Background = module.IconBackground;

                IconView.Child ??= (icon = new SymbolIcon(Symbol.More));
                IconView.Height = icon!.Height;
                IconView.Width = icon!.Width;

                ElementBorder.Background ??= new SolidColorBrush(Colors.Transparent);

                Modules.TryAdd(PageType, module);

                module.OnIconInitialze(this);
            }

        }
        catch (Exception ex)
        {
            Task.Run(() => InternalServices.ShowInfoBarAsync(ex));
            return;
        }
    }

    internal static void OnExit(Type type)
    {
        if (Modules.TryGetValue(type, out var module))
        {
            module.OnExit();
        }
    }
}
