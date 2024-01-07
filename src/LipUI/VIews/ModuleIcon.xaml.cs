using LipUI.Models;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LipUI.VIews;

public sealed partial class ModuleIcon : UserControl
{

    public static Dictionary<Type, ILipUIModulesNonGeneric> Modules { get; private set; } = new();

    public Type? PageType { get; private set; }

    public ModuleIcon(Type moduleType)
    {
        InitializeComponent();

        ILipUIModulesNonGeneric? module;
        try
        {
            module = Modules.TryGetValue(moduleType, out var _module) ? _module : Activator.CreateInstance(moduleType) as ILipUIModulesNonGeneric;

            if (module is not null)
            {
                PageType = module.PageType;

                ModuleName.Text = module.ModuleName;

                var icon = module.IconContent;
                IconView.Child = icon;

                ElementBorder.Background = module.IconBackground;

                IconView.Child ??= (icon = new SymbolIcon(Symbol.More));
                IconView.Height = icon!.Height;
                IconView.Width = icon!.Width;

                ElementBorder.Background ??= new SolidColorBrush(Colors.White);

                Modules.TryAdd(PageType, module);

                module.OnIconInitialze(this);
            }

        }
        catch (Exception ex)
        {
            Task.Run(() => Services.ShowInfoBarAsync(ex));
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
