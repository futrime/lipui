using CommunityToolkit.Mvvm.ComponentModel;
using Wpf.Ui.Common.Interfaces;

namespace LipUI.ViewModels;
public partial class DashboardViewModel : ObservableObject, INavigationAware
{
    [ObservableProperty]
    WorkingPathSelectorViewModel _selector = new() { _noEdit = true };
    public void OnNavigatedTo()
    {
    }
    public void OnNavigatedFrom() { }
}