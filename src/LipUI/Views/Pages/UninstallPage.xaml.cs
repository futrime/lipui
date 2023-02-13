using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LipUI.ViewModels;
using Wpf.Ui.Common.Interfaces;

namespace LipUI.Views.Pages
{
    /// <summary>
    /// Interaction logic for UninstallPage.xaml
    /// </summary>
    public partial class UninstallPage : INavigableView<ViewModels.UninstallPageViewModel>
    {
        public ViewModels.UninstallPageViewModel ViewModel
        {
            get;
        }
        public UninstallPage(ViewModels.UninstallPageViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
        }

        private void Hyperlink_OnClick(object sender, RoutedEventArgs e)
        {
            Global.Navigate<ToothLocalPage, ToothLocalModel>();
        }
    }
}
