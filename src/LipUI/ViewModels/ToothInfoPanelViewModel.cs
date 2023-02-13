using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LipNETWrapper.Class;

namespace LipUI.ViewModels
{
    public partial class ToothInfoPanelViewModel : ObservableObject
    {
        private LipPackage? _info;
        private LipRegistry.LipRegistryItem? _registryItem;
        private LipPackageVersions _ver;
        public ToothInfoPanelViewModel(LipPackage info)
        {
            _info = info;
            _ver = info;
        }
        public ToothInfoPanelViewModel(LipPackageVersions info, LipRegistry.LipRegistryItem item)
        {
            _ver = info;
            _registryItem = item;
        }
        public ToothInfoPanelViewModel(LipPackageVersions info)
        {
            _ver = info;
        }
        public string Author => _info?.Author ?? _registryItem?.Author ?? string.Empty;
        public string Description => _info?.Description ?? _registryItem?.Description ?? string.Empty;
        public string Homepage => _info?.Homepage ?? _registryItem?.Homepage ?? string.Empty;
        public string Name => _info?.Name ?? _registryItem?.Name ?? string.Empty;
        public string Tooth => _info?.Tooth ?? _registryItem?.Tooth ?? string.Empty;
        public string License => _info?.License ?? _registryItem?.License ?? string.Empty;
        public string Version => _info?.Version ?? "";
        public string[] Versions => _ver.Versions;
        [RelayCommand]
        void CopyToothButton()
        {
            System.Windows.Clipboard.SetText(Tooth);
        }
    }
}
