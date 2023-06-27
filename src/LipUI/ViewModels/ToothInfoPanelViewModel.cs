using System;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LipNETWrapper.Class;

namespace LipUI.ViewModels
{
    public partial class ToothInfoPanelViewModel : ObservableObject
    {
        private LipPackageVersions? _ver;
        public ToothInfoPanelViewModel(LipPackage info)  
        {
            Tooth = info.Tooth;
            Author = info.Info.Author;
            Description = info.Info.Description;
            Homepage = "undefined";
            Name = info.Info.Name;
            License = "";
            Version = info.Version;
        }
        public ToothInfoPanelViewModel(LipPackageVersions info, LipRegistry.LipRegistryItem item) : this(info)
        {
            Tooth = item.Tooth;
            Author = item.Author;
            Description = item.Description;
            Homepage = item.Homepage;
            Name = item.Name;
            License = item.License;
            //Version = item.Version;
            Tags = item.Tags.ToArray();
        }
        public ToothInfoPanelViewModel(LipPackageVersions ver)
        {
            _ver = ver;
            if (Versions?.FirstOrDefault() is not null and var v)
            {
                SelectedVersion = v;
            }
        }
        [ObservableProperty] string _author = string.Empty;
        [ObservableProperty] string _description = string.Empty;
        [ObservableProperty] string _homepage = string.Empty;
        [ObservableProperty] string _name = string.Empty;
        [ObservableProperty] string _license = string.Empty;
        [ObservableProperty] string _version = string.Empty;
        [ObservableProperty] string _tooth = string.Empty;
        [ObservableProperty] string[] _tags = Array.Empty<string>();
        public string[]? Versions => _ver?.ToArray();
        public void RefreshVersion(LipPackageVersions ver)
        {
            _ver = ver;
            OnPropertyChanged(nameof(Versions));
        }
        [RelayCommand]
        void CopyToothButton()
        {
            Clipboard.SetText(Tooth);
            Global.PopupSnackbar(Global.I18N.CopyToClipboard, Tooth);
        }
        [ObservableProperty] private string? _selectedVersion;
    }
}
