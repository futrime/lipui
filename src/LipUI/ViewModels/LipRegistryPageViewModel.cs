using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LipNETWrapper.Class;
using LipUI.Views.Pages;
using Wpf.Ui.Common.Interfaces;

namespace LipUI.ViewModels;
public partial class LipRegistryPageViewModel : ObservableRecipient, INavigationAware
{
    public LipRegistryPageViewModel()
    {
        ToothItems.CollectionChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(VisibleToothItems));
        };
    }
    protected bool _isInitialized = false;
    [ObservableProperty]
    ToothInfoPanelViewModel _currentInfo = null;
    [ObservableProperty]
    ToothItemViewModel _currentSelected = null;
    [ObservableProperty]
    bool _isShowingDetail = false;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(VisibleToothItems))]
    ObservableCollection<ToothItemViewModel> _toothItems = new();
    public ObservableCollection<ToothItemViewModel> VisibleToothItems
    {
        get
        {
            if (string.IsNullOrWhiteSpace(SearchText))
                return ToothItems;
            return new ObservableCollection<ToothItemViewModel>(ToothItems.Where(x =>
                x.Tooth.Contains(SearchText) ||
                (x.RegistryItem != null && (
                    x.RegistryItem.Description.Contains(SearchText) ||
                    x.RegistryItem.License.Contains(SearchText) ||
                    x.RegistryItem.Homepage.Contains(SearchText) ||
                    x.RegistryItem.Name.Contains(SearchText) ||
                    x.RegistryItem.Repository.Contains(SearchText) ||
                    x.RegistryItem.Tooth.Contains(SearchText) ||
                    x.RegistryItem.Author.Contains(SearchText)))
                ));
        }
    }
    [RelayCommand]
    void InvokeSearch()
    {
        OnPropertyChanged(nameof(VisibleToothItems));
    }
    [ObservableProperty]
    bool _loading = true;
    public void OnNavigatedTo()
    {
        if (!_isInitialized)
            InitializeViewModel();
    }
    public void OnNavigatedFrom()
    {
    }
    [NotifyPropertyChangedFor(nameof(VisibleToothItems))]
    [ObservableProperty] private string _searchText;
    [ObservableProperty] private string _registryHub = "https://registry.litebds.com/index.json";
    [RelayCommand]
    protected async Task LoadAllPackages()
    {
        await Global.DispatcherInvokeAsync(() =>
        {
            IsLoading = true;
            LoadingOutPut.Clear();
            LoadingOutPut.Add("正在加载所有包");
            LoadingOutPut.Add(RegistryHub);
            ToothItems.Clear();
        });
        var registry = await Global.Lip.GetLipRegistryAsync(_registryHub);

        await Global.DispatcherInvokeAsync(() =>
        {
            IsLoading = false;
        }); foreach (var item in registry.Index)
        {
            await Global.DispatcherInvokeAsync(() =>
                ToothItems.Add(new ToothItemViewModel(ShowInfo, item.Value)));
            await Task.Delay(100);//100毫秒显示一个，假装很丝滑
        }
    }
    protected void InitializeViewModel()
    {
        Task.Run(LoadAllPackages);    //初始化加载所有包
        _isInitialized = true;
    }
    protected async Task ShowInfo(ToothItemViewModel toothItem)
    {
        CurrentSelected = toothItem;
        var (success, versions, message) = await Global.Lip.GetPackageInfoAsync(toothItem.Tooth);
        if (success)
        {
            CurrentInfo = new ToothInfoPanelViewModel(versions!, toothItem.RegistryItem);
        }
        else
        {
            //todo 读取失败
        }
        IsShowingDetail = true;
    }
    [ObservableProperty] bool _isLoading = new();
    [ObservableProperty] ObservableCollection<string> _loadingOutPut = new();
    [RelayCommand]
    void GotoInstall()
    {
        Global.EnqueueItem(new InstallInfo(CurrentInfo.Tooth, CurrentInfo, CurrentInfo.SelectedVersion));
        Global.Navigate<InstallPage, ViewModels.InstallPageViewModel>();
    }
}