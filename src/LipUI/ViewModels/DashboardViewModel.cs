using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Wpf.Ui.Common.Interfaces;

namespace LipUI.ViewModels;
public partial class DashboardViewModel : ObservableObject, INavigationAware
{
    [ObservableProperty]
    WorkingPathSelectorViewModel _selector = new() { NoEdit = true };
    public void OnNavigatedTo()
    {
        LocalRef.OnNavigatedTo();
        Global.Config.PropertyChanged += OnConfigOnPropertyChanged;
    }
    void OnConfigOnPropertyChanged(object s, PropertyChangedEventArgs e)
    {
        Debug.WriteLine(e.PropertyName);
        if (e.PropertyName == nameof(Global.Config.WorkingDirectory))
        {
            LocalRef.OnNavigatedTo();
        }
    }

    public void OnNavigatedFrom()
    {
        Global.Config.PropertyChanged -= OnConfigOnPropertyChanged;
    }
    [ObservableProperty] ToothLocalModel _localRef = new();
}