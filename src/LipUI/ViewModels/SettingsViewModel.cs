using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LipUI.Models;
using Wpf.Ui.Appearance;
using Wpf.Ui.Common.Interfaces;
using static LipUI.Models.AppConfig;

namespace LipUI.ViewModels
{
    public partial class SettingsViewModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized;
        [ObservableProperty]
        private string _appVersion = string.Empty;
        [ObservableProperty]
        private string _lipVersion = string.Empty;
        [ObservableProperty]
        private ThemeType _currentTheme = Global.Config.Theme;
        partial void OnCurrentThemeChanged(ThemeType value)
        {
            Global.Config.Theme = value;
        }
        [ObservableProperty]
        bool _autoLipPath = Global.Config.AutoLipPath;
        partial void OnAutoLipPathChanged(bool value)
        {
            Global.Config.AutoLipPath = value;
        }
        [ObservableProperty]
        string _lipPath = Global.Config.LipPath;
        partial void OnLipPathChanged(string value)
        {
            if (File.Exists(value))
            {
                Global.Config.LipPath = value;
                Global.PopupSnackbar(Global.I18N.SettingsSuccessTitle, value);
            }
            else if (File.Exists(Path.Combine(value, "lip.exe")))
            {
                Global.Config.LipPath = Path.Combine(value, "lip.exe");
                Global.PopupSnackbar(Global.I18N.SettingsSuccessTitle, Global.Config.LipPath);
            }
            else
            {
                Global.PopupSnackbarWarn(Global.I18N.SettingsFailedTitle,value);
            }
        }
        [ObservableProperty]
        public AppConfigWorkingDirectory _workingDir = Global.Config.WorkingDirectory ?? new();

        public string CurrentLipPath => Global.Lip.ExecutablePath;

        partial void OnWorkingDirChanged(AppConfigWorkingDirectory value)
        {
            if (Directory.Exists(value.Directory))
            {
                Global.Config.WorkingDirectory = value;
            }
        }
        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();
        }
        public void OnNavigatedFrom()
        {
        }
        private void InitializeViewModel()
        {
            CurrentTheme = Theme.GetAppTheme();
            AppVersion = $"LipUI - {GetAssemblyVersion()}";
            LipVersion = "loading ... ";
            Task.Run(async () =>
            {
                var version = await Global.Lip.GetLipVersion();
                LipVersion = version;
            }).ConfigureAwait(false);
            _isInitialized = true;
        }
        [RelayCommand]
        private void OnOpenLipUrl()
        {
            //https://github.com/LiteLDev/Lip
            Process.Start("https://github.com/LiteLDev/Lip");
        }
        private string GetAssemblyVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? String.Empty;
        }

        [RelayCommand]
        private void OnChangeTheme(string parameter)
        {
            switch (parameter)
            {
                case "theme_light":
                    if (CurrentTheme == ThemeType.Light)
                        break;
                    Theme.Apply(ThemeType.Light);
                    CurrentTheme = ThemeType.Light;
                    break;
                default:
                    if (CurrentTheme == ThemeType.Dark)
                        break;
                    Theme.Apply(ThemeType.Dark);
                    CurrentTheme = ThemeType.Dark;
                    break;
            }
        }
        public AppConfig GlobalConfig => Global.Config;
        public string DMConfigPath => Global.ConfigPath;

        [RelayCommand]
        private async Task ExitDeveloperMode()
        {
            await Global.ShowDialog(Global.I18N.DeveloperDialog, Global.I18N.DeveloperDialogExit , (Global.I18N.DeveloperDialogCancel, hide => hide()), (Global.I18N.DeveloperDialogConfirm, hide =>
             {
                 Global.Config.DeveloperMode = false;
                 Global.PopupSnackbar(Global.I18N.DeveloperDialog, Global.I18N.DeveloperDialogExited);
                 hide();
             }
            ));
        }
    }
}
