using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LipNETWrapper.Class;
using LipUI.Views.Pages;
using Wpf.Ui.Common.Interfaces;

namespace LipUI.ViewModels
{
    public partial class ToothLocalModel : ObservableObject, INavigationAware
    {
        //protected bool _isInitialized = false;
        [ObservableProperty]
        ToothInfoPanelViewModel? _currentInfo;
        [ObservableProperty]
        ToothItemViewModel? _currentSelected;
        [ObservableProperty]
        bool _isShowingDetail;
        [ObservableProperty]
        ObservableCollection<ToothItemViewModel> _toothItems
            = new();
        [ObservableProperty]
        bool _loading = true;
        public void OnNavigatedTo()
        {
            Task.Run(LoadAllPackages);//初始化加载所以包
            //if (!_isInitialized)
            //    InitializeViewModel();
        }
        public void OnNavigatedFrom() { }
        [RelayCommand(CanExecute = nameof(Loading))]
        protected async Task LoadAllPackages()
        {
            try
            {
                await Global.DispatcherInvokeAsync(() => ToothItems.Clear());
                var (packages, message) = await Global.Lip.GetAllPackagesAsync();
                foreach (var package in packages)
                {
                    await Global.DispatcherInvokeAsync(() => ToothItems.Add(new ToothItemViewModel(ShowInfo, package)));
                    await Task.Delay(100);//100毫秒显示一个，假装很丝滑
                }
            }
            catch (Exception e)
            {
                Global.PopupSnackbarWarn(Global.I18N.LocalFetchRetry, e.Message);
#if DEBUG
                throw;
#endif
            }
        }

        protected Task<(bool success, LipPackage? package, string message)> FetchPackageInfo(string tooth)
        {
            return Global.Lip.GetLocalPackageInfoAsync(tooth);
        }
        protected async Task ShowInfo(ToothItemViewModel toothItem)
        {
            CurrentSelected = toothItem;
            var (success, package, message) = await FetchPackageInfo(toothItem.Tooth);
            if (success)
            {
                CurrentInfo = new ToothInfoPanelViewModel(package!);
            }
            else
            {
                Global.PopupSnackbarWarn(Global.I18N.LocalFetchFailed, message);
            }
            IsShowingDetail = true;
        }

        [RelayCommand]
        void Uninstall(string target)
        {
            IsShowingDetail = false;
            Global.EnqueueItem(new UninstallItem(target));
            Global.Navigate<UninstallPage, UninstallPageViewModel>();
        }
        [RelayCommand]
        void Upgrade(ToothInfoPanelViewModel vm)
        {
            IsShowingDetail = false;
            Global.EnqueueItem(new InstallInfo(vm.Tooth, vm, vm.Version));
            Global.Navigate<InstallPage, InstallPageViewModel>();
        }
    }
}
