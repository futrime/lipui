using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using LipNETWrapper.Class;
using Wpf.Ui.Controls;

namespace LipUI.ViewModels;
public partial class ToothItemViewModel : ObservableObject
{
    private Func<ToothItemViewModel, Task> _showInfo;
    public ToothItemViewModel(Func<ToothItemViewModel, Task> showInfo, LipPackageSimple package)
    {
        _showInfo = showInfo;
        Version = package.Version;
        Tooth = package.Tooth;
        Information=package.Information;
    }
    public ToothItemViewModel(
        Func<ToothItemViewModel, Task> showInfo, LipRegistry.LipRegistryItem item)
    {
        _showInfo = showInfo;
        Detailed = true;
        Tooth = item.Tooth;
        RegistryItem = item;
    }
    #region Detailed
    [ObservableProperty] bool _detailed = false;//是否有具体细节 
    [ObservableProperty] LipRegistry.LipRegistryItem? _registryItem;
    [ObservableProperty] LipPackageSimple.LipPackageSimpleInformation? _Information;
    #endregion
    [RelayCommand(CanExecute = nameof(ExecutingShowInfo))]
    async Task ShowInfo() => await _showInfo(this);
    [ObservableProperty] private bool _executingShowInfo = true;
    [ObservableProperty] string _tooth = string.Empty;
    [ObservableProperty] string _version = string.Empty;
}