using System.Collections.Specialized;
using System.Windows.Controls;
using System.Windows.Input;
using LipUI.ViewModels;
using Wpf.Ui.Common.Interfaces;

namespace LipUI.Views.Pages
{
    /// <summary>
    /// Interaction logic for InstallPage.xaml
    /// </summary>
    public partial class InstallPage : INavigableView<InstallPageViewModel>
    {
        public InstallPageViewModel ViewModel
        {
            get;
        }
        public InstallPage(InstallPageViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            ViewModel.OutPut.CollectionChanged += OutPut_CollectionChanged!;
        }
        private void OutPut_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ScrollViewer.ScrollToEnd();
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
}
