using System.Windows.Controls;
namespace LipUI.Views.Controls
{
    /// <summary>
    /// Interaction logic for LipInstaller.xaml
    /// </summary>
    public partial class LipInstaller : UserControl
    {
        public LipInstaller(ViewModels.LipInstallerViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
