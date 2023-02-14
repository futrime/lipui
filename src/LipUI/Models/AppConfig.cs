using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using Wpf.Ui.Appearance;

namespace LipUI.Models
{
    [Serializable]
    public partial class AppConfig : ObservableObject
    {
        [ObservableProperty] string _lipPath = "lip.exe";
        [ObservableProperty] string _workingDirectory;
        [ObservableProperty] ThemeType _theme;
        [ObservableProperty] private bool _autoLipPath = true;

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
