using System.Windows.Controls;
using System.Windows.Input;
using LipUI.ViewModels;
using Wpf.Ui.Common.Interfaces;

namespace LipUI.Views.Pages;
/// <summary>
/// Interaction logic for LipRegistryPage.xaml
/// </summary>
public partial class LipRegistryPage : INavigableView<LipRegistryPageViewModel>
{
    public LipRegistryPageViewModel ViewModel
    {
        get;
    }
    public LipRegistryPage(LipRegistryPageViewModel viewModel)
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
    private void UIElement_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        var s = (ComboBox)sender;
        var length = s.Items.Count;
        if (e.Delta > 0)//向下滚
        {
            if (s.SelectedIndex == 0)
                s.SelectedIndex = length - 1;
            else
                s.SelectedIndex--;
        }
        else
        {
            if (s.SelectedIndex == length - 1)
                s.SelectedIndex = 0;
            else
                s.SelectedIndex++;
        }
        e.Handled = true;
    }
}
