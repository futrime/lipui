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
using Wpf.Ui.Common;
using Wpf.Ui.Controls.Interfaces;

namespace LipUI.Views.Controls
{
    /// <summary>
    /// Interaction logic for ToothItem.xaml
    /// </summary>
    public partial class ToothItem 
    {
        public ViewModels.ToothItemViewModel ViewModel => DataContext as ViewModels.ToothItemViewModel;
        public ToothItem()
        { 
            InitializeComponent(); 
        } 
    }
}
