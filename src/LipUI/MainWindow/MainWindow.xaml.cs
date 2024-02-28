using LipUI.Models;
using LipUI.Models.Lip;
using LipUI.Models.Plugin;
using LipUI.Pages.LipExecutionPanel;
using LipUI.Pages.Settings;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Octokit;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LipUI;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
internal sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        AppWindow.Closing += (_, _) => InternalServices.OnWindowClosed();

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);

        SubClassing();

        AppWindow.Resize(new(1600, 900));

        InternalServices.MainWindow = this;

        PersonalizationSettingsView.Initialize(Main.Config.PersonalizationSettings);

        enabled = PluginEnabled;
        disabled = PluginDisabled;
    }

    internal void ShowInfoBar(
        string? title,
        string? message,
        InfoBarSeverity severity,
        TimeSpan interval = default,
        UIElement? barContent = null,
        Action? completed = null)
    {
        Task.Run(async () =>
        {
            var mre = new ManualResetEvent(false);

            DispatcherQueue.TryEnqueue(() =>
            {
                GlobalInfoBar.Title = title;
                GlobalInfoBar.Message = message;
                GlobalInfoBar.Severity = severity;
                GlobalInfoBar.IsClosable = false;
                GlobalInfoBar.Content = barContent;
                GlobalInfoBar.IsOpen = true;

                void task(object? sender, object e)
                {
                    InfoBarPopInStoryboard.Completed -= task;
                    mre.Set();
                }
                InfoBarPopInStoryboard.Completed += task;

                InfoBarPopInStoryboard.Begin();
            });
            mre.WaitOne();

            await Task.Delay(interval);

            mre.Reset();
            DispatcherQueue.TryEnqueue(() =>
            {
                InfoBarPopOutStoryboard.Begin();

                void task(object? sender, object e)
                {
                    InfoBarPopOutStoryboard.Completed -= task;
                    GlobalInfoBar.IsOpen = false;
                    mre.Set();
                }
                InfoBarPopOutStoryboard.Completed += task;
            });
            mre.WaitOne();
            mre.Dispose();
            completed?.Invoke();
        });
    }

    internal async ValueTask ShowInfoBarAsync(string? title,
        string? message,
        InfoBarSeverity severity,
        TimeSpan interval = default,
        UIElement? barContent = null)
    {
        await Task.Run(async () =>
        {
            var mre = new ManualResetEvent(false);

            DispatcherQueue.TryEnqueue(() =>
            {
                GlobalInfoBar.Title = title;
                GlobalInfoBar.Message = message;
                GlobalInfoBar.Severity = severity;
                GlobalInfoBar.IsClosable = false;
                GlobalInfoBar.Content = barContent;
                GlobalInfoBar.IsOpen = true;

                void task(object? sender, object e)
                {
                    InfoBarPopInStoryboard.Completed -= task;
                    mre.Set();
                }
                InfoBarPopInStoryboard.Completed += task;

                InfoBarPopInStoryboard.Begin();
            });
            mre.WaitOne();

            await Task.Delay(interval);

            mre.Reset();
            DispatcherQueue.TryEnqueue(() =>
            {
                InfoBarPopOutStoryboard.Begin();

                void task(object? sender, object e)
                {
                    InfoBarPopOutStoryboard.Completed -= task;
                    GlobalInfoBar.IsOpen = false;
                    mre.Set();
                }
                InfoBarPopOutStoryboard.Completed += task;
            });
            mre.WaitOne();
            mre.Dispose();
        });
    }

    private async void RootBorder_Drop(object sender, DragEventArgs e)
    {
        if (e.DataView.Contains(StandardDataFormats.StorageItems) is false)
            return;

        foreach (var item in await e.DataView.GetStorageItemsAsync())
        {
            if (item is StorageFile file && Path.GetExtension(file.Path) is ".tth")
            {
                var lip = await Main.CreateLipConsole(RootBorder.XamlRoot);
                if (lip is null)
                {
                    InternalServices.ShowInfoBar(
                        "infobar$error".GetLocalized(),
                        Main.Config.SelectedServer is null ?
                        "lipExecution$nullServerPath".GetLocalized() :
                        "lipExecution$nullLipPath".GetLocalized(),
                        InfoBarSeverity.Error);

                    return;
                }
                var cmd = LipCommand.CreateCommand() + LipCommand.Install + file.Path;
                var info = new List<string>();

                ContentFrame.Navigate(
                    typeof(LipExecutionPanelPage),
                    new LipExecutionPanelPage.NavigationArgs(file.Path, info, lip, cmd));
            }
            else
            {
                await InternalServices.ShowInfoBarAsync(
                    "infobar$error".GetLocalized(),
                    @$"{item.Name} is not a '.tth' file.",
                    InfoBarSeverity.Error);
            }
        }
    }

    private void RootBorder_DragOver(object sender, DragEventArgs e)
    {
        e.AcceptedOperation = DataPackageOperation.Move;
        e.DragUIOverride.IsCaptionVisible = false;
        e.DragUIOverride.IsGlyphVisible = false;
    }

    private async void RootBorder_Loaded(object sender, RoutedEventArgs e)
    {
        await PluginSystem.LoadAsync();

        var timer = DispatcherQueue.CreateTimer();
        timer.Interval = TimeSpan.FromSeconds(60);
        timer.IsRepeating = true;
        timer.Tick += (_sender, e) => Task.Run(Main.SaveConfig);
        timer.Start();
    }
}
