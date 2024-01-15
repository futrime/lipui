using LipUI.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LipUI.Pages.Settings;

internal sealed partial class GeneralSettingsView : UserControl
{
    public GeneralSettingsView()
    {
        InitializeComponent();
    }

    private void GithubProxyInput_TextChanged(object sender, TextChangedEventArgs e)
    {
        Main.Config.GeneralSettings.GithubProxy = GithubProxyInput.Text;
    }

    private void LipIndexApiInput_TextChanged(object sender, TextChangedEventArgs e)
    {
        Main.Config.GeneralSettings.LipIndexApiKey = LipIndexApiInput.Text;
    }

    private void LipPathInput_TextChanged(object sender, TextChangedEventArgs e)
    {
        Main.Config.GeneralSettings.LipPath = LipPathInput.Text;
    }

    private void GithubApiInput_TextChanged(object sender, TextChangedEventArgs e)
    {
        Main.Config.GeneralSettings.GithubApiKey = GithubApiInput.Text;
    }

    private void GithubProxyInput_Loading(FrameworkElement sender, object args)
    {
        GithubProxyInput.Text = Main.Config.GeneralSettings.GithubProxy;
    }

    private void LipIndexApiInput_Loading(FrameworkElement sender, object args)
    {
        LipIndexApiInput.Text = Main.Config.GeneralSettings.LipIndexApiKey;
    }

    private void GithubApiInput_Loading(FrameworkElement sender, object args)
    {
        GithubApiInput.Text = Main.Config.GeneralSettings.GithubApiKey;
    }

    private void LipPathInput_Loading(FrameworkElement sender, object args)
    {
        LipPathInput.Text = Main.Config.GeneralSettings.LipPath;
    }
}
