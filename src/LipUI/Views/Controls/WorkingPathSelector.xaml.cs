using System.Windows.Controls;
using LipUI.ViewModels;

namespace LipUI.Views.Controls
{
    /// <summary>
    /// Interaction logic for WorkingPathSelector.xaml
    /// </summary>
    public partial class WorkingPathSelector : UserControl
    {
        public WorkingPathSelectorViewModel ViewModel => (WorkingPathSelectorViewModel)DataContext;
        public bool NoEdit
        {
            set => ViewModel.NoEdit = value;
        }
        public WorkingPathSelector()
        {
            InitializeComponent();
            DataContext ??= new WorkingPathSelectorViewModel();
        }
    }
}
