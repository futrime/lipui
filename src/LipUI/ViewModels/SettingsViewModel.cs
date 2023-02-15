using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.IO;
using System.Threading.Tasks;
using Wpf.Ui.Common.Interfaces;

namespace LipUI.ViewModels
{
    public partial class SettingsViewModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;
        [ObservableProperty]
        private string _appVersion = string.Empty;
        [ObservableProperty]
        private string _lipVersion = string.Empty;
        [ObservableProperty]
        private Wpf.Ui.Appearance.ThemeType _currentTheme = Global.Config.Theme;
        partial void OnCurrentThemeChanged(Wpf.Ui.Appearance.ThemeType value)
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
                Global.PopupSnackbar("设置成功", value);
            }
            else if (File.Exists(Path.Combine(value, "lip.exe")))
            {
                Global.Config.LipPath = Path.Combine(value, "lip.exe");
                Global.PopupSnackbar("设置成功", Global.Config.LipPath);
            }
            else
            {
                Global.PopupSnackbarWarn("路径不存在", value);
            }
        }
        [ObservableProperty]
        public string _workingDir = Global.Config.WorkingDirectory;

        public string CurrentLipPath => Global.Lip.ExecutablePath;

        partial void OnWorkingDirChanged(string value)
        {
            if (Directory.Exists(value))
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
            CurrentTheme = Wpf.Ui.Appearance.Theme.GetAppTheme();
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
            System.Diagnostics.Process.Start("https://github.com/LiteLDev/Lip");
        }
        private string GetAssemblyVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? String.Empty;
        }

        [RelayCommand]
        private void OnChangeTheme(string parameter)
        {
            switch (parameter)
            {
                case "theme_light":
                    if (CurrentTheme == Wpf.Ui.Appearance.ThemeType.Light)
                        break;

                    Wpf.Ui.Appearance.Theme.Apply(Wpf.Ui.Appearance.ThemeType.Light);
                    CurrentTheme = Wpf.Ui.Appearance.ThemeType.Light;

                    break;

                default:
                    if (CurrentTheme == Wpf.Ui.Appearance.ThemeType.Dark)
                        break;

                    Wpf.Ui.Appearance.Theme.Apply(Wpf.Ui.Appearance.ThemeType.Dark);
                    CurrentTheme = Wpf.Ui.Appearance.ThemeType.Dark;

                    break;
            }
        }
    }
}
