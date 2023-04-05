using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using LipUI.Language;
using Newtonsoft.Json;
using Wpf.Ui.Appearance;

namespace LipUI.Models
{
    [Serializable]
    public partial class AppConfig : ObservableObject
    {
        public AppConfig()
        {
            _allWorkingDirectory.CollectionChanged += (_, _) =>
            {
                OnPropertyChanged(nameof(WorkingDirectory));
            };
        }
        public partial class AppConfigWorkingDirectory : ObservableObject, IEquatable<AppConfigWorkingDirectory>
        {
            [ObservableProperty] string _directory = string.Empty;
            [ObservableProperty] string _name = Global.I18N.AppConfigWorkingDirectoryUnnamed;
            public bool Equals(AppConfigWorkingDirectory other)
            {
                return Directory == other.Directory;
            }
            public static implicit operator AppConfigWorkingDirectory(string v) => new() { Directory = v };
        }
        [ObservableProperty] string _lipPath = "lip.exe";
        [JsonIgnore]
        public AppConfigWorkingDirectory? WorkingDirectory
        {
            get
            {
                return AllWorkingDirectory.FirstOrDefault(x => x.Directory == _workingDirectoryPath) ?? new();
            }
            set
            {
                if (value is not null)
                {
                    _workingDirectoryPath = value.Directory;
                }
                OnPropertyChanged();
            }
        }
        [ObservableProperty] private Utils.LangId _language = Utils.LangId.Default;
        [JsonProperty("WorkingDirectory")] string? _workingDirectoryPath = string.Empty;
        [NotifyPropertyChangedFor(nameof(WorkingDirectory))][ObservableProperty] ObservableCollection<AppConfigWorkingDirectory> _allWorkingDirectory = new();
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
        [ObservableProperty] private bool _webUIEnabled;

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