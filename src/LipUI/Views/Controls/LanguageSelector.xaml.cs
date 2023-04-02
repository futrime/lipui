using LipUI.ViewModels;
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

namespace LipUI.Views.Controls
{
    /// <summary>
    /// Interaction logic for LanguageSelector.xaml
    /// </summary>
    public partial class LanguageSelector : UserControl
    {
        public LanguageSelector(LanguageSelectorViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
        public LanguageSelector() : this(new()) { }
    }
}
