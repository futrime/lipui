using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LipNETWrapper.Class;
using Wpf.Ui.Common.Interfaces;

namespace LipUI.ViewModels;
public partial class LipRegistryPageViewModel : ObservableObject, INavigationAware
{
    protected bool _isInitialized = false;
    [ObservableProperty]
    ToothInfoPanelViewModel _currentInfo = null;
    [ObservableProperty]
    ToothItemViewModel _currentSelected = null;
    [ObservableProperty]
    bool _isShowingDetail = false;
    [ObservableProperty]
    ObservableCollection<ToothItemViewModel> _toothItems
        = new();
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
    [ObservableProperty] private string _searchText;
    [ObservableProperty] private string _registryHub = "https://registry.litebds.com/index.json";

    partial void OnSearchTextChanged(string _)
    {

    }
    [RelayCommand]
    protected async Task LoadAllPackages()
    {
        ToothItems.Clear();
        var registry = await Global.Lip.GetLipRegistryAsync(_registryHub);
        foreach (var item in registry.Index)
        {
            ToothItems.Add(new ToothItemViewModel(ShowInfo, item.Value));
            await Task.Delay(100);//100毫秒显示一个，假装很丝滑
        }
    }
    protected void InitializeViewModel()
    {
        _ = Global.DispatcherInvokeAsync(async () =>
           {
               await LoadAllPackages();//初始化加载所以包
           });
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
}