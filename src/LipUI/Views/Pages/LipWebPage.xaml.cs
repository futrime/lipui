using LipUI.ViewModels;
using Wpf.Ui.Common.Interfaces;

namespace LipUI.Views.Pages
{
    /// <summary>
    /// Interaction logic for LipWebPage.xaml
    /// </summary>
    public partial class LipWebPage : INavigableView<LipWebPageViewModel>
    {
        public LipWebPage(LipWebPageViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
        }

        public LipWebPageViewModel ViewModel { get; }
    }
}
