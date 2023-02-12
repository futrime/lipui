using System.Windows.Input;
using Wpf.Ui.Common.Interfaces;

namespace LipUI.Views.Pages;
/// <summary>
/// Interaction logic for LipRegistryPage.xaml
/// </summary>
public partial class LipRegistryPage : INavigableView<ViewModels.LipRegistryPageViewModel>
{
    public ViewModels.LipRegistryPageViewModel ViewModel
    {
        get;
    }
    public LipRegistryPage(ViewModels.LipRegistryPageViewModel viewModel)
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
