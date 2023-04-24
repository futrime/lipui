using System.Windows.Input;
using LipUI.ViewModels;
using Wpf.Ui.Common.Interfaces;

namespace LipUI.Views.Pages
{
    /// <summary>
    /// Interaction logic for DashboardPage.xaml
    /// </summary>
    public partial class DashboardPage : INavigableView<DashboardViewModel>
    {
        public DashboardViewModel ViewModel
        {
            get;
        }
        public DashboardPage(DashboardViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
        } 
    }
}