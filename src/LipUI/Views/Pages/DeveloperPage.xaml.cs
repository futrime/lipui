using LipUI.ViewModels;
using Wpf.Ui.Common.Interfaces;

namespace LipUI.Views.Pages
{
    /// <summary>
    /// Interaction logic for DeveloperPage.xaml
    /// </summary>
    public partial class DeveloperPage : INavigableView<DeveloperPageViewModel>
    {
        public DeveloperPage(DeveloperPageViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
        }
        public DeveloperPageViewModel ViewModel { get; }
    }
}
