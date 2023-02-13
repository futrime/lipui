using System.Windows.Input;
using Wpf.Ui.Common.Interfaces;

namespace LipUI.Views.Pages
{
     /// <summary>
    /// Interaction logic for ToothLocalPage.xaml
    /// </summary>
    public partial class ToothLocalPage : INavigableView<ViewModels.ToothLocalModel>
    {
        public ViewModels.ToothLocalModel ViewModel
        {
            get;
        }
        public ToothLocalPage(ViewModels.ToothLocalModel viewModel)
        {
            ViewModel = viewModel;

            InitializeComponent();
        }
        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            ViewModel.IsShowingDetail = false;
        }
        private void UIElement_OnTouchDown(object sender, TouchEventArgs e)
        {
            ViewModel.IsShowingDetail = false;
        }  
    }
}
