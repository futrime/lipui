using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace LipUI.ViewModels
{
    public partial class InstallPageViewModel : ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<string> _outPut;
        [ObservableProperty]
        string _toothName;
        [ObservableProperty]
        ToothInfoPanelViewModel _toothInfoPanel;
        [RelayCommand]
        public async Task Install()
        {

        }
        [RelayCommand]
        public async Task FetchInfo()
        {
            OutPut.Clear();
            var (success, package, message) = await Global.Lip.GetPackageInfoAsync(ToothName);
            if (success)
            {
                ToothInfoPanel = new ToothInfoPanelViewModel(package!);
            }
            else
            {
                //todo 获取失败
            }
        }
    }
}
