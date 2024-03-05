using LipUI.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

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
                await InternalServices.ShowInfoBarAsync(
                    severity: InfoBarSeverity.Error,
                    title: "infoBar$error".GetLocalized(),
                    message: "lipExecution$nullLipPath".GetLocalized());

            Process.Start(
                Path.Combine(Main.WorkingDirectory, "AutoUpdate.exe"),
                @$"--type lipui_autoupdate --lip-path ""{path}"" --working-dir ""{Main.WorkingDirectory}""");

            Environment.Exit(0);
        }
    }
}
