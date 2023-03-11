using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using LipNETWrapper.Class;
using Wpf.Ui.Controls;

namespace LipUI.ViewModels;
public partial class ToothItemViewModel : ObservableObject
{
    [ObservableProperty] bool _actived = true;//等待移除，用于淡出动画
    private Func<ToothItemViewModel, Task> _showInfo;
    public ToothItemViewModel(Func<ToothItemViewModel, Task> showInfo, LipPackageSimple package)
    {
        _showInfo = showInfo;
        Version = package.Version;
        Tooth = package.Tooth;
        Information = package.Information;
        Author.Tag = Information.Author;
    }
    public ToothItemViewModel(
        Func<ToothItemViewModel, Task> showInfo, LipRegistry.LipRegistryItem item)
    {
        _showInfo = showInfo;
        Detailed = true;
        Tooth = item.Tooth;
        RegistryItem = item;
        Tags = (from x in item.Tags
                select new ToothItemTagViewModel() { Tag = x })
            //.Concat(new[] { new ToothItemTagViewModel() { Tag = item.Author } })
            .ToArray();
        Author.Tag = item.Author;
    }
    #region Detailed
    [ObservableProperty] bool _detailed = false;//是否有具体细节 
    [ObservableProperty] LipRegistry.LipRegistryItem? _registryItem;
    [ObservableProperty] LipPackageSimple.LipPackageSimpleInformation? _Information;
    #endregion
    [RelayCommand(CanExecute = nameof(ExecutingShowInfo))]
    async Task ShowInfo() => await _showInfo(this);
    [ObservableProperty] private bool _executingShowInfo = true;
    [ObservableProperty] string _tooth = string.Empty;
    [ObservableProperty] string _version = string.Empty;
    [ObservableProperty] ToothItemTagViewModel[] _tags = Array.Empty<ToothItemTagViewModel>();
    [ObservableProperty] ToothItemTagViewModel _author = new();

    public partial class ToothItemTagViewModel : ObservableObject
    {
        [ObservableProperty] string _tag = string.Empty;
        [ObservableProperty] bool _isSelected = false;
    }
}
