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
        partial void OnCurrentThemeChanged(Wpf.Ui.Appearance.ThemeType theme)
        {
            Global.Config.Theme = theme;
        }
        [ObservableProperty]
        public bool _autoLipPath = Global.Config.AutoLipPath;
        partial void OnAutoLipPathChanged(bool auto)
        {
            Global.Config.AutoLipPath = auto;
        }
        [ObservableProperty]
        public string _lipPath = Global.Config.LipPath;
        partial void OnLipPathChanged(string path)
        {
            if (File.Exists(path))
            {
                Global.Config.LipPath = path;
            }
            else if (File.Exists(Path.Combine(path, "lip.exe")))
            {
                Global.Config.LipPath = Path.Combine(path, "lip.exe");
            }
        }
        [ObservableProperty]
        public string _workingDir = Global.Config.WorkingDirectory;
        partial void OnWorkingDirChanged(string path)
        {
            if (Directory.Exists(path))
            {
                Global.Config.WorkingDirectory = path;
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
