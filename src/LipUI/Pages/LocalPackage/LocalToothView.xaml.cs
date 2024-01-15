using LipUI.Pages.LipExecutionPanel;
using LipUI.Protocol;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LipUI.Pages.LocalPackage;

internal sealed partial class LocalToothView : UserControl
{
    public ToothPackage Tooth { get; private set; }

    private LocalPackagePage page;

    public LocalToothView(LocalPackagePage page, ToothPackage tooth)
    {
        this.page = page;
        Tooth = tooth;

        InitializeComponent();

        ToothName.Text = tooth.Info.Name;
        ToothDescription.Text = tooth.Info.Description;
        UpdateButtonText.Text = "localTooth$update".GetLocalized();
        DeleteButtonText.Text = "localTooth$uninstall".GetLocalized();

        UpdateButton.Content = new ProgressRing()
        {
            Height = 16,
            Width = 16
        };
    }

    public void RefreshUpdateButton(bool enable) =>
        DispatcherQueue.TryEnqueue(() =>
        {
            UpdateButton.IsEnabled = enable;
            UpdateButton.Content = UpdateButtonText;
        });

    private void UpdateButton_Click(object sender, RoutedEventArgs e)
    {
        page.Frame.Navigate(
            typeof(LipExecutionPanelPage),
            new LipExecutionPanelPage.InitArguments(
                 LipExecutionPanelPage.ExecutionMode.Update,
                 Tooth));
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        page.Frame.Navigate(
            typeof(LipExecutionPanelPage),
            new LipExecutionPanelPage.InitArguments(
                 LipExecutionPanelPage.ExecutionMode.Delete,
                 Tooth));
    }
}
