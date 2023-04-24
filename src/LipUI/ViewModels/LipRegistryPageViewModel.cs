using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LipUI.Views.Pages;
using Wpf.Ui.Common.Interfaces;

namespace LipUI.ViewModels;
public partial class LipRegistryPageViewModel : ObservableRecipient, INavigationAware
{
    public LipRegistryPageViewModel()
    {
        ToothItems.CollectionChanged += (_, _) =>
        {
            RefreshQuery();
            //OnPropertyChanged(nameof(VisibleToothItems));
        };
    }
    private bool _isInitialized;
    [ObservableProperty]
    ToothInfoPanelViewModel? _currentInfo;
    [ObservableProperty]
    ToothItemViewModel? _currentSelected;
    [ObservableProperty]
    bool _isShowingDetail;
    [ObservableProperty]
    //[NotifyPropertyChangedFor(nameof(VisibleToothItems))]
    ObservableCollection<ToothItemViewModel> _toothItems = new();
    partial void OnToothItemsChanged(ObservableCollection<ToothItemViewModel> value)
    {
        RefreshQuery();
    }
    private bool waitingRefreshQuery;
    [ObservableProperty] bool _isSearching;
    int _searchingCount;

    async void RefreshQuery()
    {
        IEnumerable<ToothItemViewModel> RefreshQueryInternal()
        {
            if (string.IsNullOrWhiteSpace(SearchText) && !OnlyFeatured)
                return ToothItems;
            var searchLower = Regex.Replace(SearchText.ToLower(), @"\[[\w-_]+?\]", "");
            var tags = Array.Empty<string>();
            try
            {
                tags = (
                        from x in Regex.Matches(SearchText, @"\[([\w-_]+?)\]").Cast<Match>()
                        select x.Groups[1].Value).Concat(OnlyFeatured ? new[] { "featured" } : Array.Empty<string>())
                    .ToArray();
            }
            catch (Exception e)
            {
                Global.PopupSnackbarWarn(Global.I18N.RegistrySearchTagFailed, e.Message);
            }

            var items = ToothItems.Where(x =>
                x.Tooth.ToLower().Contains(searchLower) ||
                (x.RegistryItem != null && (
                    x.RegistryItem.Description.ToLower().Contains(searchLower) ||
                    x.RegistryItem.License.ToLower().Contains(searchLower) ||
                    x.RegistryItem.Homepage.ToLower().Contains(searchLower) ||
                    x.RegistryItem.Name.ToLower().Contains(searchLower) ||
                    x.RegistryItem.Repository.ToLower().Contains(searchLower) ||
                    x.RegistryItem.Tooth.ToLower().Contains(searchLower) ||
                    x.RegistryItem.Author.ToLower().Contains(searchLower)))
            );
            if (tags.Length > 0)
                items = from x in items
                        where tags.All(t => x.Author.Tag == t || x.Tags.Any(y => y.Tag == t))
                        select x;
            var result = items.ToArray();
            return result;
        }
        try
        {
            IsSearching = true;
            _searchingCount++;
            do
            {
                await Task.Delay(5);
            } while (waitingRefreshQuery);

            waitingRefreshQuery = true;
            var all = RefreshQueryInternal();
            //remove the items VisibleToothItems don't have
            var allItems = all as ToothItemViewModel[] ?? all.ToArray();
            var toRemove = VisibleToothItems.Except(allItems).ToArray();
            foreach (var item in toRemove)
            {
                item.Actived = false;
                //await Task.Delay(50);
            }
            //add the items VisibleToothItems don't have
            foreach (var item in allItems.Except(VisibleToothItems).ToArray())
            {
                VisibleToothItems.Add(item);
                item.Actived = true;
                await Task.Delay(10);
            }
            if (toRemove.Any())
            {
                await Task.Delay(2000);
                foreach (var item in toRemove)
                {
                    VisibleToothItems.Remove(item);
                    await Task.Delay(10);
                }
            }
        }
        catch (Exception e)
        {
            Global.PopupSnackbarWarn(Global.I18N.RegistryTipRefreshFailed, e.Message);
        }
        finally
        {
            waitingRefreshQuery = false;
            _searchingCount--;
            if (_searchingCount == 0)
            {
                IsSearching = false;
            }
        }
    }

    [ObservableProperty] ObservableCollection<ToothItemViewModel> _visibleToothItems = new();
    [RelayCommand]
    void InvokeSearch()
    {
        //OnPropertyChanged(nameof(VisibleToothItems));
        RefreshQuery();
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
    [ObservableProperty] string _searchText = string.Empty;
    DateTime _lastSearch = DateTime.Now;//上次执行搜索
    partial void OnSearchTextChanged(string value)//输入变动
    {//1秒的时间间隔是为了避免频繁刷新导致界面卡顿
        void TrySearch()//执行搜索
        {
            //do not invoke if the last search is in 1 second 
            var now = DateTime.Now;
            if (now - _lastSearch < TimeSpan.FromSeconds(1))
            {
                //delay for 1 second to invoke search
                Task.Run(async () =>
                {
                    await Task.Delay(1000);
                    await Global.DispatcherInvokeAsync(() =>
                    {
                        var now = DateTime.Now;
                        //这1秒内没搜索，执行一次搜索
                        if (now - _lastSearch > TimeSpan.FromSeconds(1))
                        {
                            InvokeSearch();
                            _lastSearch = now;
                        }
                    });
                });
                return;
            }
            _lastSearch = now;
            InvokeSearch();
        }

        TrySearch();
    }

    public string RegistryHub
    {
        get
        {
            var args = Environment.GetCommandLineArgs().ToList();
            //using LipUI.exe -r http://xxx.xxx.xxx/registry.json
            //or --registry
            var index = args.FindIndex(x => x is "-r" or "--registry");
            if (index != -1 && args.Count > index + 1)
            {
                return args[index + 1];
            }
            return "https://registry.litebds.com/index.json";
        }
    }
    //[NotifyPropertyChangedFor(nameof(VisibleToothItems))]
    [ObservableProperty] bool _onlyFeatured;
    partial void OnOnlyFeaturedChanged(bool value)
    {
        //RefreshQuery();
        AddTag(new object[] { "featured", value });
    }
    [RelayCommand]
    protected async Task LoadAllPackages()
    {
        try
        {
            await Global.DispatcherInvokeAsync(() =>
            {
                IsLoading = true;
                LoadingOutPut.Clear();
                LoadingOutPut.Add(Global.I18N.RegistryTipFetchAll);
                LoadingOutPut.Add(RegistryHub);
                ToothItems.Clear();
            });
            var registry = await Global.Lip.GetLipRegistryAsync(RegistryHub);

            await Global.DispatcherInvokeAsync(() =>
            {
                IsLoading = false;
            });
            foreach (var item in registry.Index)
            {
                await Global.DispatcherInvokeAsync(() =>
                    ToothItems.Add(new ToothItemViewModel(ShowInfo, item.Value)));
            }
        }
        catch (Exception e)
        {
            Global.PopupSnackbarWarn(Global.I18N.RegistryTipFetchFailed, e.Message);
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
            if (toothItem.RegistryItem is not null)
                CurrentInfo = new ToothInfoPanelViewModel(versions!, toothItem.RegistryItem);
        }
        else
        {
            Global.PopupSnackbarWarn(Global.I18N.RegistryTipFetchFailed, message);
        }
        IsShowingDetail = true;
    }
    [ObservableProperty] bool _isLoading;
    [ObservableProperty] ObservableCollection<string> _loadingOutPut = new();
    [RelayCommand]
    void GotoInstall()
    {
        if (CurrentInfo != null)
            Global.EnqueueItem(new InstallInfo(CurrentInfo.Tooth, CurrentInfo, CurrentInfo.SelectedVersion));
        Global.Navigate<InstallPage, InstallPageViewModel>();
    }

    [RelayCommand]
    void AddTag(object[] value)
    {
        if (value is [string v, bool isChecked])
        {
            if (v == "featured")
            {
                OnlyFeatured = isChecked;
            }
            var item = "[" + v + "]";
            if (isChecked)
            {
                SearchText = item + SearchText.Replace(item, "");
                foreach (var element in ToothItems)
                    foreach (var tag in element.Tags.Concat(new[] { element.Author }))
                        if (tag.Tag == v)
                            tag.IsSelected = true;
            }
            else
            {
                SearchText = SearchText.Replace(item, "");
                foreach (var element in ToothItems)
                    foreach (var tag in element.Tags.Concat(new[] { element.Author }))
                        if (tag.Tag == v)
                            tag.IsSelected = false;
            }
        }
    }
}