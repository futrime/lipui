using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;

namespace LipUI.ViewModels;
public partial class ToothItemViewModel : ObservableObject
{
    public ToothItemViewModel(
        Func<ToothItemViewModel, Task> showInfo)
    {
        _showInfo = showInfo;
    }
    private Func<ToothItemViewModel, Task> _showInfo;
    [RelayCommand(CanExecute = nameof(ExecutingShowInfo))]
    private async Task ShowInfo()
    {
        await _showInfo(this);
    }
    [ObservableProperty]
    private bool executingShowInfo = true;
    [ObservableProperty]
    private string _tooth = string.Empty;
    [ObservableProperty]
    private string _version = string.Empty;
}