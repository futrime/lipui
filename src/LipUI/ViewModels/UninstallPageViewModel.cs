using CommunityToolkit.Mvvm.ComponentModel;
using Wpf.Ui.Common.Interfaces;

namespace LipUI.ViewModels;
public record UninstallItem(string Tooth);
public partial class UninstallPageViewModel : ObservableObject, INavigationAware
{
    [ObservableProperty] string _tooth;
    public void OnNavigatedTo()
    {
        if (Global.TryDequeueItem<UninstallItem>(out var data))
        {
            Tooth = data.Tooth;
        }
    }
    public void OnNavigatedFrom()
    {
    }
}
