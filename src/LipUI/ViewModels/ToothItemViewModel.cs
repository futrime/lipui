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
    public ToothItemViewModel(
        Func<ToothItemViewModel, Task> showInfo)
    {
        _showInfo = showInfo;
    }
    public ToothItemViewModel(
        Func<ToothItemViewModel, Task> showInfo, LipRegistry.LipRegistryItem item)
    {
        _showInfo = showInfo;
        Detailed = true;
        Tooth = item.Tooth;
        Description = item.Description;
        Name = item.Name;
    }
    #region Detailed
    [ObservableProperty] bool _detailed = false;//是否有具体细节
    [ObservableProperty] string _description = string.Empty;
    [ObservableProperty] string _author = string.Empty;
    [ObservableProperty] string _name = string.Empty;
    #endregion
    [RelayCommand(CanExecute = nameof(ExecutingShowInfo))]
    async Task ShowInfo() => await _showInfo(this);
    [ObservableProperty] bool executingShowInfo = true;
    [ObservableProperty] string _tooth = string.Empty;
    [ObservableProperty] string _version = string.Empty;
}