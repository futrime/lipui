using CommunityToolkit.Mvvm.ComponentModel;
using LipUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Input;
using Wpf.Ui.Common.Interfaces;

namespace LipUI.ViewModels
{
    public partial class ToothLocalModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;
        [ObservableProperty]
        IEnumerable<ToothItemViewModel> _toothItems
            = Array.Empty<ToothItemViewModel>();

        [ObservableProperty]
        private bool _loading = true;

        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();
        }

        public void OnNavigatedFrom()
        {
        }
        [RelayCommand(CanExecute = nameof(Loading))]
        private async Task LoadAllPackages()
        {
            ToothItems = Array.Empty<ToothItemViewModel>();
            var (packages, message) = await Global.Lip.GetAllPackagesAsync();
            ToothItems = (from x in packages
                select new ToothItemViewModel()
                {
                    Version = x.Version,
                    Tooth = x.Tooth
                }).ToArray();
        }
        private void InitializeViewModel()
        {
            _ = LoadAllPackages();//初始化加载所以包
            _isInitialized = true;
        }
    }
}
