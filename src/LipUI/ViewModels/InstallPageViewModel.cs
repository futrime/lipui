using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace LipUI.ViewModels
{
    public partial class InstallPageViewModel : ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<string> _outPut = new();

        [ObservableProperty]
        string _toothName;
        partial void OnToothNameChanged(string _)
        {
            ToothInfoPanel = null;
        }
        [NotifyCanExecuteChangedFor(nameof(InstallCommand))]
        [NotifyPropertyChangedFor(nameof(InfoLoaded))]
        [ObservableProperty]
        ToothInfoPanelViewModel? _toothInfoPanel;
        private CancellationTokenSource _ctk = new();
        public bool InfoLoaded => _toothInfoPanel is not null;
        [ObservableProperty] private bool _installing = false;
        [RelayCommand(CanExecute = nameof(InfoLoaded))]
        public async Task Install()
        {
            OutPut.Clear();
            _ctk = new CancellationTokenSource();
            Installing = true;
            try
            {
                var exitCode = await Global.Lip.InstallPackageAsync(ToothName, _ctk.Token, x =>
                {
                    if (x is not null)
                    {
                        if (x.StartsWith("{"))
                        {
                            //todo json进度
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
            Installing = false;
        }
        [RelayCommand]
        public async Task FetchInfo()
        {
            OutPut.Clear();
            _ctk = new CancellationTokenSource();
            try
            {
                var (success, package, message) = await Global.Lip.GetPackageInfoAsync(ToothName, _ctk.Token, x =>
                {
                    if (x is not null && !x.StartsWith("{"))
                    {
                        Global.DispatcherInvoke(() => OutPut.Add(x));
                    }
                });
                if (success)
                {
                    ToothInfoPanel = new ToothInfoPanelViewModel(package!);
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
        }
    }
}
