using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
