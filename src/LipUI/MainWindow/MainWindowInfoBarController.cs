using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUIEx.Messaging;

namespace LipUI;

internal partial class MainWindow
{
    private record struct InfoBarTask(
        ManualResetEvent Mre,
        string? Title,
        string? Message,
        InfoBarSeverity Severity,
        TimeSpan Interval,
        UIElement? Content,
        Action? Completed,
        CancellationToken CancellationToken);

    private readonly Queue<InfoBarTask> InfoBarTaskQueue = new();
    private bool IsRunning = false;

    private void ShowInfoBar(InfoBarTask task)
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            GlobalInfoBar.Title = task.Title;
            GlobalInfoBar.Message = task.Message;
            GlobalInfoBar.Severity = task.Severity;
            GlobalInfoBar.IsClosable = false;
            GlobalInfoBar.Content = task.Content;
            GlobalInfoBar.IsOpen = true;

            void set(object? sender, object e)
            {
                InfoBarPopInStoryboard.Completed -= set;
                task.Mre.Set();
            }
            InfoBarPopInStoryboard.Completed += set;

            InfoBarPopInStoryboard.Begin();
        });
    }

    private void CloseInfoBar(ManualResetEvent mre)
    {
        InfoBarPopOutStoryboard.Begin();

        void task(object? sender, object e)
        {
            InfoBarPopOutStoryboard.Completed -= task;
            GlobalInfoBar.IsOpen = false;
            mre.Set();
        }
        InfoBarPopOutStoryboard.Completed += task;
    }

    private void StartShowInfoBarLoop()
    => Task.Run(async () =>
        {
            InfoBarTask task;
            bool dequeued;

            IsRunning = true;
        LOOP:
            lock (InfoBarTaskQueue)
            {
                dequeued = InfoBarTaskQueue.TryDequeue(out task);
            }

            if (dequeued)
            {
                DispatcherQueue.TryEnqueue(() => ShowInfoBar(task));

                try
                {
                    await Task.Delay(task.Interval, task.CancellationToken);
                }
                catch (TaskCanceledException) { }

                task.Mre.Reset();
                DispatcherQueue.TryEnqueue(() => CloseInfoBar(task.Mre));
                task.Mre.WaitOne();
                task.Mre.Dispose();
                task.Completed?.Invoke();

                goto LOOP;
            }
            IsRunning = false;
        });

    internal void ShowInfoBar(
        string? title,
        string? message,
        InfoBarSeverity severity,
        TimeSpan interval = default,
        UIElement? barContent = null,
        Action? completed = null,
        CancellationToken cancellationToken = default)
    {
        Task.Run(() =>
        {
            var mre = new ManualResetEvent(false);
            InfoBarTaskQueue.Enqueue(new(mre, title, message, severity,
                interval, barContent, completed, cancellationToken));
            if (IsRunning is false)
                StartShowInfoBarLoop();
            mre.WaitOne();
        }, CancellationToken.None);
    }

    internal async ValueTask ShowInfoBarAsync(string? title,
        string? message,
        InfoBarSeverity severity,
        TimeSpan interval = default,
        UIElement? barContent = null,
        CancellationToken cancellationToken = default)
    {
        await Task.Run(() =>
        {
            var mre = new ManualResetEvent(false);
            InfoBarTaskQueue.Enqueue(new(mre, title, message, severity,
                interval, barContent, null, cancellationToken));
            if (IsRunning is false)
                StartShowInfoBarLoop();
            mre.WaitOne();
        }, CancellationToken.None);
    }

}