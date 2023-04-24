using CommunityToolkit.Mvvm.ComponentModel;
using LipUI.Models;
using Wpf.Ui.Common.Interfaces;

namespace LipUI.ViewModels
{
    public partial class DeveloperPageViewModel : ObservableObject, INavigationAware
    {
        public void OnNavigatedTo()
        {
        }
        public void OnNavigatedFrom()
        {
        }

        [ObservableProperty]
        private ToothJsonViewModel _toothJson = new(new()
        {
            
        });
        public AppConfig GlobalConfig => Global.Config;
    }
}
