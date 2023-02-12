using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
namespace LipUI.ViewModels;
public partial class LipRegistryPageViewModel : ToothLocalModel
{
    [ObservableProperty] private string _searchText;
    [ObservableProperty] private string _registryHub = "https://registry.litebds.com/index.json";

    partial void OnSearchTextChanged(string _)
    {

    }
    protected override async Task LoadAllPackages()
    {
        ToothItems.Clear();
        var registry = await Global.Lip.GetLipRegistryAsync(RegistryHub);
        foreach (var item in registry.Index)
        {
            ToothItems.Add(new ToothItemViewModel(ShowInfo, item.Value));
            await Task.Delay(100);//100毫秒显示一个，假装很丝滑
        }
    }

    protected override void InitializeViewModel()
    {
        _ = LoadAllPackages();//初始化加载所以包
        _isInitialized = true;
    }
}