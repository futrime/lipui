using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LipUI.Views.Pages;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace LipUI.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private bool _isInitialized;

        [ObservableProperty]
        private string _applicationTitle = String.Empty;

        [ObservableProperty]
        private ObservableCollection<INavigationControl> _navigationItems = new();

        [ObservableProperty]
        private ObservableCollection<INavigationControl> _navigationFooter = new();

        //[ObservableProperty]
        //private ObservableCollection<MenuItem> _trayMenuItems = new();

        public MainWindowViewModel(INavigationService navigationService)
        {
            if (!_isInitialized)
                InitializeViewModel();
        }
        private void InitializeViewModel()
        {
            ApplicationTitle = "LipUI - Tooth包管理器";
            NavigationItems = new ObservableCollection<INavigationControl>
            {
                new NavigationItem
                {
                    Content = "主页",
                    PageTag = "dashboard",
                    Icon = SymbolRegular.Home24,
                    PageType = typeof(DashboardPage)
                },
                new NavigationItem
                {
                    Content = "本地Tooth包",
                    PageTag = "local",
                    Icon = SymbolRegular.Box24,
                    PageType = typeof(ToothLocalPage)
                },
                new NavigationItem
                {
                    Content = "安装Tooth包",
                    PageTag = "add",
                    Icon = SymbolRegular.Add24,
                    PageType = typeof(InstallPage)
                },
                new NavigationItem
                {
                    Content = "卸载Tooth包",
                    PageTag = "remove",
                    Icon = SymbolRegular.BoxDismiss24,
                    PageType = typeof(UninstallPage)
                },
                new NavigationItem
                {
                    Content = "包市场",
                    PageTag = "registry",
                    Icon = SymbolRegular.BoxSearch24,
                    PageType = typeof(LipRegistryPage)
                },   new NavigationItem
                {
                    Content = "WebUI",
                    PageTag = "web",
                    Icon = SymbolRegular.WebAsset24,
                    PageType = typeof(LipWebPage)
                },
                new NavigationItem
                {
                    Icon = SymbolRegular.DeveloperBoard24,
                    Content = "开发者工具",
                    PageTag = "developer",
                    PageType = typeof(DeveloperPage)
                }
            };

            NavigationFooter = new ObservableCollection<INavigationControl>
            {
                new NavigationItem
                {
                    Content = "设置 & 关于",
                    PageTag = "settings",
                    Icon = SymbolRegular.Settings24,
                    PageType = typeof(SettingsPage)
                }
            };

            //TrayMenuItems = new ObservableCollection<MenuItem>
            //{
            //    new MenuItem
            //    {
            //        Header = "Home",
            //        Tag = "tray_home"
            //    }
            //};

            _isInitialized = true;
        }
    }
}
