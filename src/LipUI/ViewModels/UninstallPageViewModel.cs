using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wpf.Ui.Common.Interfaces;

namespace LipUI.ViewModels;
public record UninstallItem(string Tooth);
public partial class UninstallPageViewModel : ObservableObject, INavigationAware
{
    [ObservableProperty][NotifyPropertyChangedFor(nameof(HasTooth))] string  _tooth=string.Empty;
    [ObservableProperty][NotifyPropertyChangedFor(nameof(HasTooth))] bool _uninstallComplete;
    public bool HasTooth => !string.IsNullOrWhiteSpace(Tooth);
    [ObservableProperty] ObservableCollection<string> _outPut = new();
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
    [RelayCommand]
    async Task DoUninstall()
    {
        OutPut.Clear();
        try
        {
            var exitCode = await Global.Lip.UninstallPackageAsync(Tooth, default, x =>
            {
                Global.DispatcherInvoke(() => OutPut.Add(x));
            });
            OutPut.Add($"ExitCode：{exitCode}");
            UninstallComplete = true;
        }
        catch (Exception ex)
        {
            OutPut.Add(ex.ToString());
        }
    }
}
