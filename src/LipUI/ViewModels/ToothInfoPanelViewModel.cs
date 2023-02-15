using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LipNETWrapper.Class;

namespace LipUI.ViewModels
{
    public partial class ToothInfoPanelViewModel : ObservableObject
    {
        private readonly LipPackage? _info;
        private readonly LipRegistry.LipRegistryItem? _registryItem;
        private readonly LipPackageVersions _ver;
        public ToothInfoPanelViewModel(LipPackage info) : this((LipPackageVersions)info)
        {
            Tooth = info.Tooth;
        }
        public ToothInfoPanelViewModel(LipPackageVersions info, LipRegistry.LipRegistryItem item) : this(info)
        {
            _registryItem = item;
            Tooth = item.Tooth;
        }
        public ToothInfoPanelViewModel(LipPackageVersions ver)
        {
            _ver = ver;
            if (ver is LipPackage info)
            {
                _info = info;
            }
            if (Versions?.FirstOrDefault() is not null and var v)
            {
                SelectedVersion = v;
            }
        }
        public string Author => _info?.Author ?? _registryItem?.Author ?? string.Empty;
        public string Description => _info?.Description ?? _registryItem?.Description ?? string.Empty;
        public string Homepage => _info?.Homepage ?? _registryItem?.Homepage ?? string.Empty;
        public string Name => _info?.Name ?? _registryItem?.Name ?? string.Empty;
        [ObservableProperty] public string _tooth;
        public string License => _info?.License ?? _registryItem?.License ?? string.Empty;
        public string Version => _info?.Version ?? "";
        public string[]? Versions => _ver.Versions;
        [RelayCommand]
        void CopyToothButton()
        {
            System.Windows.Clipboard.SetText(Tooth);
            Global.PopupSnackbar("复制成功", Tooth);
        }
        [ObservableProperty] private string? _selectedVersion;
    }
}
