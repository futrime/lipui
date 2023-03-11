using System.Windows.Controls;
using LipUI.ViewModels;

namespace LipUI.Views.Controls
{
    /// <summary>
    /// Interaction logic for LipInstaller.xaml
    /// </summary>
    public partial class LipInstaller : UserControl
    {
        public LipInstaller(LipInstallerViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
