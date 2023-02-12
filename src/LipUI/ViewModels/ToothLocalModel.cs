using CommunityToolkit.Mvvm.ComponentModel;
using LipUI.Models;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using Wpf.Ui.Common.Interfaces;

namespace LipUI.ViewModels
{
    public partial class ToothLocalModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;
        [ObservableProperty]
        private IEnumerable<ToothItemViewModel> _toothItems;

        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();
        }

        public void OnNavigatedFrom()
        {
        }

        private void InitializeViewModel()
        {
            var itemCollection = new List<ToothItemViewModel>();
            for (int i = 1; i <= 32; i++)
            {
                itemCollection.Add(new ToothItemViewModel
                {
                    PackageName = "测试 " + i,
                    PackageDescription = "描述"
                });
            }
            ToothItems = itemCollection;
            _isInitialized = true;
        }
    }
}
