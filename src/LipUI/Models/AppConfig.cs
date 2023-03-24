using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using Wpf.Ui.Appearance;

namespace LipUI.Models
{
    [Serializable]
    public partial class AppConfig : ObservableObject
    {
        [ObservableProperty] string _lipPath = "lip.exe";
        [ObservableProperty] string _workingDirectory = string.Empty;
        [ObservableProperty] ObservableCollection<string> _allWorkingDirectory = new();
        [ObservableProperty] ThemeType _theme;
        [ObservableProperty] private bool _autoLipPath = true;
        [ObservableProperty] private bool _developerMode = false;
        partial void OnDeveloperModeChanged(bool value)
        {
            if (!value)//退出开发者模式时恢复所有选项到默认
            {
                DevShowSkipDependency = false;
            }
        }
        [ObservableProperty] private bool _devShowSkipDependency;
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
        public static AppConfig FromString(string json)
        {
            return JsonConvert.DeserializeObject<AppConfig>(json) ?? new AppConfig();
        }
    }
}
