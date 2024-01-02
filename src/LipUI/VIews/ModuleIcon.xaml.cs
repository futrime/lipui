using LipUI.Pages.HomePageModules;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Net.Security;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LipUI.VIews
{
    public sealed partial class ModuleIcon : UserControl
    {
        private readonly UIElement iconElement;
        private readonly Type pageType;
        private readonly object? parameter;
        private readonly Page modulesPage;


        public ModuleIcon(ModulesPage page, UIElement element, Type pageType, object? parameter = null)
        {
            InitializeComponent();

            modulesPage = page;
            iconElement = element;
            this.pageType = pageType;
            this.parameter = parameter;
        }

        private void ModuleIcon_Loading(FrameworkElement sender, object args)
        {
            ElementBorder.Child = iconElement;
        }

        public void Navigate()
            => modulesPage.Frame.Navigate(pageType, parameter);
    }
}
