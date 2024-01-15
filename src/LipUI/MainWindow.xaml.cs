using LipUI.Models;
using LipUI.Models.Plugin;
using LipUI.Pages.Settings;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading;
using System.Threading.Tasks;
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

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);

        SubClassing();

        AppWindow.Resize(new(1600, 900));

        InternalServices.MainWindow = this;

        PersonalizationSettingsView.Initialize();

        enabled = PluginEnabled;
        disabled = PluginDisabled;

        Task.Run(PluginSystem.LoadAsync);
    }

    private async void MainWindow_Closed(object sender, WindowEventArgs args)
    {
        InternalServices.OnWindowClosed();
        await Main.SaveConfigAsync();
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
}
