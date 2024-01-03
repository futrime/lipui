using LipUI.Models;
using LipUI.VIews;
using Microsoft.UI.Xaml.Controls;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LipUI.Pages.HomePageModules
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ModulesPage : Page
    {

        public ModulesPage()
        {
            InitializeComponent();

            DispatcherQueue.TryEnqueue(async () =>
            {
                await Task.Delay(500);
                ModulesView.Items.Add(new ModuleIcon(this, new SymbolIcon(Symbol.Setting), typeof(BdsPropertiesEditorPage), Main.Config.SelectedServer));
            });
        }

        private void ModulesView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as ModuleIcon;
            item!.Navigate();

        }
    }
}
