using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wpf.Ui.Common.Interfaces;

namespace LipUI.ViewModels
{
    public record InstallInfo(string Tooth, ToothInfoPanelViewModel data, string? Version);
    public partial class InstallPageViewModel : ObservableObject, INavigationAware
    {
        [ObservableProperty]
        ObservableCollection<string> _outPut = new();
        [ObservableProperty]
        string _toothName;
        partial void OnToothNameChanged(string _)
        {
            ToothInfoPanel = null;
            OutPut.Clear();
        }
        [NotifyCanExecuteChangedFor(nameof(InstallCommand))]
        [NotifyPropertyChangedFor(nameof(InfoLoaded))]
        [ObservableProperty]
        ToothInfoPanelViewModel? _toothInfoPanel;
        public bool InfoLoaded => _toothInfoPanel is not null;
        [ObservableProperty] private bool _installing = false;
        [RelayCommand(CanExecute = nameof(InfoLoaded))]
        public async Task Install()
        {
            OutPut.Clear();
            Ctk = new CancellationTokenSource();
            Installing = true;
            try
            {
                var fullname = ToothName;
                if (!string.IsNullOrWhiteSpace(SelectedVersion))
                    fullname += "@" + SelectedVersion;
                var exitCode = await Global.Lip.InstallPackageAsync(fullname, Ctk.Token, x =>
                {
                    if (x is not null)
                    {//todo Please enter y if you agree with the above terms
                     //if (x.StartsWith("{"))
                     //{
                        if (x.Trim().EndsWith("|"))
                        {
                            Percentage = x.Replace("|", "").Trim();
                        }
                        else
                        {
                            Global.DispatcherInvoke(() => OutPut.Add(x));
                        }
                    }
                });
                OutPut.Add($"ExitCode：{exitCode}");
            }
            catch (Exception ex)
            {
                OutPut.Add(ex.ToString());
            }
            Ctk = null;
            Installing = false;
        }
        [RelayCommand]
        public async Task FetchInfo()
        {
            OutPut.Clear();
            Ctk = new CancellationTokenSource();
            try
            {
                var (success, package, message) = await Global.Lip.GetPackageInfoAsync(ToothName, _ctk.Token, x =>
                {
                    if (x is not null)
                    {
                        if (!x.StartsWith("{"))
                        {
                            Global.DispatcherInvoke(() => OutPut.Add(x));
                        }
                    }
                });
                if (success)
                {
                    ToothInfoPanel = new ToothInfoPanelViewModel(package!)
                    {
                        Tooth = ToothName
                    };
                }
                else
                {
                    //todo 获取失败
                }
            }
            catch (Exception ex)
            {
                OutPut.Add(ex.ToString());
            }
            Ctk = null;
        }
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanCancel))]
        [NotifyCanExecuteChangedFor(nameof(CancelCommand))]
        CancellationTokenSource? _ctk = null;
        [ObservableProperty]
        string? _selectedVersion;
        [ObservableProperty]
        string _percentage = string.Empty;
        public bool CanCancel => Ctk is not null;
        [RelayCommand(CanExecute = nameof(CanCancel))]
        public void Cancel()
        {
            Ctk?.Cancel();
            Ctk = null;
        }
        public void OnNavigatedTo()
        {
            if (Global.TryDequeueItem<InstallInfo>(out var item))
            {
                ToothName = item.Tooth;
                ToothInfoPanel = item.data;
                SelectedVersion = item.Version ?? item.data.Versions.FirstOrDefault();
            }
        }
        public void OnNavigatedFrom()
        {
        }
    }
}
