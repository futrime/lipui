using LipUI.Models;
using LipUI.VIews;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LipUI.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class HomePage : Page
{
    public HomePage()
    {
        InitializeComponent();
    }

    private void ServerIconImage_Loading(FrameworkElement sender, object args)
        => RefreshIcon();

    private async void RefreshIcon()
    {
        ServerInstance? instance = Main.Config.SelectedServer;
        ServerIconImage.Source = await ServerIcon.GetIcon(instance);
    }

    private void SelectServerButton_Click(object sender, RoutedEventArgs e)
        => Frame.Navigate(typeof(SelectServerPage), () => { DispatcherQueue.TryEnqueue(() => { RefreshIcon(); }); });

    private void Page_Loaded(object sender, RoutedEventArgs e)
        => ShowLipInstallerPageIfNotExist();

    private void ShowLipInstallerPageIfNotExist()
    {
        if (File.Exists(Main.Config.Settings.LipPath)) 
        {
            return;
        }

        var dialog = new ContentDialog()
        {
            XamlRoot = XamlRoot,
            Content = new LipInstallerView()
        };

        DispatcherQueue.TryEnqueue(async () =>
        {
            await dialog.ShowAsync();
        });
    }
}
