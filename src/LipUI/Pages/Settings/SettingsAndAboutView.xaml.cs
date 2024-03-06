using LipUI.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Diagnostics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LipUI.Pages.Settings
{
    public sealed partial class SettingsAndAboutView : UserControl
    {
        public SettingsAndAboutView()
        {
            this.InitializeComponent();
        }

        private void UpdateButton_Loading(FrameworkElement sender, object args)
        {
            var button = (Button)sender;
            button.Content = "localTooth$update".GetLocalized();
        }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            var (success, path) = await Main.TryGetLipConsolePathAsync(XamlRoot);

            if (success is false)
            {
                await InternalServices.ShowInfoBarAsync(
                    severity: InfoBarSeverity.Error,
                    title: "infoBar$error".GetLocalized(),
                    message: "lipExecution$nullLipPath".GetLocalized());

                return;
            }

            var args = "install --yes --force-reinstall --no-dependencies github.com/lippkg/LipUI";

            InternalServices.ShowInfoBar(
                interval: TimeSpan.FromSeconds(3),
                message: $"Running {args}",
                completed: () =>
                {
                    var process = new Process()
                    {
                        StartInfo = new(path)
                        {
                            WorkingDirectory = Main.ProgramDirectory,
                            Arguments = args,
                        }
                    };
                    
                    Main.SaveConfig();
                    process.Start();
                    Environment.Exit(0);
                });
        }
    }
}
