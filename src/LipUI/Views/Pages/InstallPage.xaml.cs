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
        }
    }
}
