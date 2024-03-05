using LipUI.Models;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LipUI.Pages.Settings;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class SettingsPage : Page
{
    public SettingsPage()
    {
        InitializeComponent();

        SettingsBorder.Child = new GeneralSettingsView();
        SettingsTitleText.Text = "generalSettings$title/Text".GetLocalized();
    }

    private void ListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is TextBlock text && text.Tag is string tag)
        {
            SettingsTitleText.Text = text.Text;

            switch (tag)
            {
                case "generalSettings":
                    SettingsBorder.Child = new GeneralSettingsView();
                    break;

                case "personalizationSettings":
                    SettingsBorder.Child = new PersonalizationSettingsView();
                    break;

                case "settingsAndAbout":
                    SettingsBorder.Child = new SettingsAndAboutView();
                    break;

                case "lipSettings":
                    SettingsBorder.Child = new LipSettingsView();
                    break;
            }
        }
    }
}
