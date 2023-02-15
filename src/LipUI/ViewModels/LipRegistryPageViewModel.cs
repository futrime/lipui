using System;
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
            var searchLower = SearchText.ToLower();
            return new ObservableCollection<ToothItemViewModel>(ToothItems.Where(x =>
                x.Tooth.ToLower().Contains(searchLower) ||
                (x.RegistryItem != null && (
                    x.RegistryItem.Description.ToLower().Contains(searchLower) ||
                    x.RegistryItem.License.ToLower().Contains(searchLower) ||
                    x.RegistryItem.Homepage.ToLower().Contains(searchLower) ||
                    x.RegistryItem.Name.ToLower().Contains(searchLower) ||
                    x.RegistryItem.Repository.ToLower().Contains(searchLower) ||
                    x.RegistryItem.Tooth.ToLower().Contains(searchLower) ||
                    x.RegistryItem.Author.ToLower().Contains(searchLower)))
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
    //[NotifyPropertyChangedFor(nameof(VisibleToothItems))]
    [ObservableProperty] string _searchText;
    DateTime lastSearch = DateTime.Now;//上次执行搜索
    partial void OnSearchTextChanged(string _)//输入变动
    {//1秒的时间间隔是为了避免频繁刷新导致界面卡顿
        void TrySearch()//执行搜索
        {
            //do not invoke if the last search is in 1 second 
            var now = DateTime.Now;
            if (now - lastSearch < TimeSpan.FromSeconds(1))
            {
                //delay for 1 second to invoke search
                Task.Run(async () =>
                {
                    await Task.Delay(1000);
                    await Global.DispatcherInvokeAsync(() =>
                    {
                        var now = DateTime.Now;
                        //这1秒内没搜索，执行一次搜索
                        if (now - lastSearch > TimeSpan.FromSeconds(1))
                        {
                            InvokeSearch();
                            lastSearch = now;
                        }
                    });
                });
                return;
            }
            lastSearch = now;
            InvokeSearch();
        }

        TrySearch();
    }
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