using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using LipUI.Views.Pages;
using Microsoft.Win32;
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
            NavigationItem Create<T>(string tag, SymbolRegular icon, string i18nKey)
            {
                var item = new NavigationItem
                {
                    Content = "",
                    PageTag = tag,
                    Icon = icon,
                    PageType = typeof(T)
                };
                item.SetBinding(ContentControl.ContentProperty, new Binding
                {
                    Path = new PropertyPath(i18nKey),
                    Source = Global.I18N,
                    Mode = BindingMode.OneWay
                });
                return item;
            }
            NavigationItems = new ObservableCollection<INavigationControl>
            {
                Create<DashboardPage>("dashboard", SymbolRegular.Home24, nameof(Global.I18N.NavigationHome)),
                Create<ToothLocalPage>("local", SymbolRegular.Box24, nameof(Global.I18N.NavigationLocal)),
                Create<InstallPage>("install", SymbolRegular.Add24, nameof(Global.I18N.NavigationInstall)),
                Create<UninstallPage>("remove", SymbolRegular.BoxDismiss24, nameof(Global.I18N.NavigationRemove)),
                Create<LipRegistryPage>("registry", SymbolRegular.BoxSearch24, nameof(Global.I18N.NavigationRegistry)),
                //Create<LipWebPage>("web", SymbolRegular.WebAsset24, nameof(Global.I18N.NavigationWebUi)),
                //Create<DeveloperPage>("developer", SymbolRegular.DeveloperBoard24, nameof(Global.I18N.NavigationDeveloper)),
            };

            NavigationFooter = new ObservableCollection<INavigationControl>
            {
                Create<SettingsPage>("settings", SymbolRegular.Settings24, nameof(Global.I18N.NavigationSettings)),
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
