using Wpf.Ui.Common.Interfaces;

namespace LipUI.Views.Pages
{
    /// <summary>
    /// Interaction logic for InstallPage.xaml
    /// </summary>
    public partial class InstallPage : INavigableView<ViewModels.InstallPageViewModel>
    {
        public ViewModels.InstallPageViewModel ViewModel
        {
            get;
        }
        public InstallPage(ViewModels.InstallPageViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            ViewModel.OutPut.CollectionChanged += OutPut_CollectionChanged!;
        }
        private void OutPut_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ScrollViewer.ScrollToEnd();
        }
    }
}
