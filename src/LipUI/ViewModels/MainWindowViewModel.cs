using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace LipUI.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private bool _isInitialized = false;

        [ObservableProperty]
        private string _applicationTitle = String.Empty;

        [ObservableProperty]
        private ObservableCollection<INavigationControl> _navigationItems = new();

        [ObservableProperty]
        private ObservableCollection<INavigationControl> _navigationFooter = new();

        [ObservableProperty]
        private ObservableCollection<MenuItem> _trayMenuItems = new();

        public MainWindowViewModel(INavigationService navigationService)
        {
            if (!_isInitialized)
                InitializeViewModel();
        }

        private void InitializeViewModel()
        {
            ApplicationTitle = "LipUI - 齿包管理器"; 
            NavigationItems = new ObservableCollection<INavigationControl>
            {
                new NavigationItem()
                {
                    Content = "主页",
                    PageTag = "dashboard",
                    Icon = SymbolRegular.Home24,
                    PageType = typeof(Views.Pages.DashboardPage)
                },
                new NavigationItem()
                {
                    Content = "本地包",
                    PageTag = "local",
                    Icon = SymbolRegular.Box24,
                    PageType = typeof(Views.Pages.ToothLocalPage)
                },
                new NavigationItem()
                {
                    Content = "安装齿包",
                    PageTag = "add",
                    Icon = SymbolRegular.Add24,
                    PageType = typeof(Views.Pages.InstallPage)
                },
                new NavigationItem()
                {
                    Content = "卸载齿包",
                    PageTag = "remove",
                    Icon = SymbolRegular.BoxDismiss24,
                    PageType = typeof(Views.Pages.UninstallPage)
                },
                new NavigationItem()
                {
                    Content = "包市场",
                    PageTag = "registry",
                    Icon = SymbolRegular.BoxSearch24,
                    PageType = typeof(Views.Pages.LipRegistryPage)
                }
            };

            NavigationFooter = new ObservableCollection<INavigationControl>
            {
                new NavigationItem()
                {
                    Content = "设置",
                    PageTag = "settings",
                    Icon = SymbolRegular.Settings24,
                    PageType = typeof(Views.Pages.SettingsPage)
                } 
            };

            TrayMenuItems = new ObservableCollection<MenuItem>
            {
                new MenuItem
                {
                    Header = "Home",
                    Tag = "tray_home"
                }
            };

            _isInitialized = true;
        }
    }
}
