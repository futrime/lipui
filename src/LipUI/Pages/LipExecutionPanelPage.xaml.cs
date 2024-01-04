using LipUI.Models;
using LipUI.Models.Lip;
using LipUI.Protocol;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Windows.UI;
using static LipUI.Models.Lip.LipCommand;
using static LipUI.Models.Lip.LipCommandOption;
using static LipUI.Protocol.LipIndex.LipIndexData;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LipUI.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
internal sealed partial class LipExecutionPanelPage : Page
{
    private partial class LipConsoleHandler
    {
        private LipExecutionPanelPage page;
        private LipConsole? lip;
        private Process? process;
        private readonly DispatcherQueue dispatcherQueue;

        private readonly List<(Regex, Func<Regex, Match, string, bool>)> regexsAndMethods;

        public void Output(string line)
            => dispatcherQueue.TryEnqueue(() => page.output.Add(line));

        public void Input(string line, bool output = true)
        {
            if (output) dispatcherQueue.TryEnqueue(() => page.output.Add(line));
            process?.StandardInput.WriteLine(line);
        }

        private void OutputHandler(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return;

            bool output = true;

            foreach (var (k, v) in regexsAndMethods)
            {
                var match = k.Match(line);
                if (match.Success)
                {
                    output = v(k, match, line);
                    break;
                }
            }

            if (output) Output(line);
        }

        public LipConsoleHandler(LipExecutionPanelPage page)
        {
            this.page = page;
            dispatcherQueue = page.DispatcherQueue;
            regexsAndMethods = new()
            {
                (Y_or_N_Regex(), Y_or_N_Matched),
                (BDSDownloader_ProgressBar_Regex(), BDSDownloader_ProgressBar_Matched),
                (BDSDownloader_StartDownloadOrUnziping_Regex(), BDSDownloader_StartDownloadOrUnziping_Matched),
                (BDSDownloader_DownloadComplete_Regex(), BDSDownloader_DownloadComplete_Matched),
                (Done_Regex(), Done_Matched),
                (Lip_ProgressBar_Regex(), Lip_ProgressBar_Matched),
                (Lip_StartDownload_Regex(), Lip_StartDownload_Matched)
            };
        }

        public void Run()
        {
            dispatcherQueue.TryEnqueue(async () =>
            {
                lip = await Main.CreateLipConsole(page.XamlRoot);
                if (lip is null)
                {
                    var bar = page.TaskProgressBar;
                    bar.IsIndeterminate = false;
                    bar.Value = 0;

                    page.LipWorkingInfoText.Text = string.Empty;
                    page.ProgressRateText.Text = string.Empty;

                    await Helpers.ShowInfoBarAsync(
                        "infobar$error".GetLocalized(),
                        "lipExecution$nullServerPath".GetLocalized(),
                         InfoBarSeverity.Error);

                    return;
                }


                lip.Output += OutputHandler;

                switch (page.Mode)
                {
                    case ExecutionMode.Install:

                        await lip.Run(CreateCommand() + Install + page.ToothItem!.RepoPath, ins => process = ins.Process);

                        break;

                    case ExecutionMode.Delete:

                        await lip.Run(CreateCommand() + Uninstall + page.Package!.Tooth, ins => process = ins.Process);

                        break;

                    case ExecutionMode.Update:

                        await lip.Run(CreateCommand() + Install + Upgrade + page.Package!.Tooth, ins => process = ins.Process);

                        break;
                }

            });
        }


        [GeneratedRegex(@"(?<string>.*)\[y/N\]")]
        private static partial Regex Y_or_N_Regex();

        [GeneratedRegex(@"(Downloading|Unziping)\.+\s+(?<percent>[0-9]+%)\s+\[(?<progress>[\s]+|[=>\s])+\]\s+\((?<rate>.*?)\)\s+\[(?<elapsed>[0-9a-z]+):(?<estimated>[0-9a-z]+)\]")]
        private static partial Regex BDSDownloader_ProgressBar_Regex();

        [GeneratedRegex(@"Downloading BDS (?<version>.*?)\.\.\.|Unziping downloaded files\.\.\.")]
        private static partial Regex BDSDownloader_StartDownloadOrUnziping_Regex();

        [GeneratedRegex("Download complete!|Unzip complete!")]
        private static partial Regex BDSDownloader_DownloadComplete_Regex();

        [GeneratedRegex(@"Done\.|ERROR: aborted")]
        private static partial Regex Done_Regex();

        [GeneratedRegex(@"\s+(?<percent>[0-9]+%)\s+\|[¨€|\s]+\|\s+\((?<progress>.*?),(?<rate>.*?)\)\s+\[(?<elapsed>[0-9a-z]+):(?<estimated>[0-9a-z]+)\]")]
        private static partial Regex Lip_ProgressBar_Regex();

        [GeneratedRegex(@"Downloading (.*?)\.\.\.")]
        private static partial Regex Lip_StartDownload_Regex();


        private bool Y_or_N_Matched(Regex regex, Match match, string str)
        {
            dispatcherQueue.TryEnqueue(async () =>
            {
                ContentDialog contentDialog = new()
                {
                    XamlRoot = page.XamlRoot,
                    Content = new TextBlock()
                    {
                        Text = match.Groups["string"].Value
                    },
                    CloseButtonText = "eula$deny".GetLocalized(),
                    PrimaryButtonText = "eula$accept".GetLocalized()
                };

                var operation = await contentDialog.ShowAsync();
                switch (operation)
                {
                    case ContentDialogResult.None: Input("N"); break;
                    case ContentDialogResult.Primary: Input("y"); break;
                }
            });
            return true;
        }

        private bool BDSDownloader_ProgressBar_Matched(Regex regex, Match match, string str)
        {
            dispatcherQueue.TryEnqueue(() =>
            {
                var percent = int.Parse(match.Groups["percent"].Value[..^1]);
                page.TaskProgressBar.Value = percent;
                page.ProgressRateText.Text = $"{match.Groups["rate"].Value}  {match.Groups["elapsed"].Value} {match.Groups["estimated"].Value}";
            });

            return false;
        }

        private bool BDSDownloader_StartDownloadOrUnziping_Matched(Regex regex, Match match, string str)
        {
            dispatcherQueue.TryEnqueue(() =>
            {
                var bar = page.TaskProgressBar;
                bar.IsIndeterminate = false;
                bar.Minimum = 0;
                bar.Maximum = 100;
                bar.Value = 0;

                page.LipWorkingInfoText.Text = str;
            });
            return true;
        }

        private bool BDSDownloader_DownloadComplete_Matched(Regex regex, Match match, string str)
        {
            dispatcherQueue.TryEnqueue(() =>
            {
                var bar = page.TaskProgressBar;
                bar.IsIndeterminate = true;
                bar.Value = 0;

                page.LipWorkingInfoText.Text = string.Empty;
                page.ProgressRateText.Text = string.Empty;
            });
            return true;
        }

        private bool Done_Matched(Regex regex, Match match, string str)
        {
            dispatcherQueue.TryEnqueue(() =>
            {
                var bar = page.TaskProgressBar;
                bar.IsIndeterminate = false;
                bar.Value = 0;

                page.LipWorkingInfoText.Text = string.Empty;
                page.ProgressRateText.Text = string.Empty;
            });
            return true;
        }

        private bool Lip_ProgressBar_Matched(Regex regex, Match match, string str)
        {
            dispatcherQueue.TryEnqueue(() =>
            {
                var percent = int.Parse(match.Groups["percent"].Value[..^1]);
                page.TaskProgressBar.Value = percent;
                page.ProgressRateText.Text = $"{match.Groups["progress"].Value} {match.Groups["rate"].Value}  {match.Groups["elapsed"].Value} {match.Groups["estimated"].Value}";
            });

            return false;
        }

        private bool Lip_StartDownload_Matched(Regex regex, Match match, string str)
        {
            dispatcherQueue.TryEnqueue(() =>
            {
                var bar = page.TaskProgressBar;
                bar.IsIndeterminate = false;
                bar.Minimum = 0;
                bar.Maximum = 100;
                bar.Value = 0;

                page.LipWorkingInfoText.Text = str;
            });
            return true;
        }

    }

    public enum ExecutionMode { Install, Delete, Update }

    public record InitArguments(ExecutionMode Mode, object Parameter);


    private LipTooth? Tooth { get; set; }

    private LipToothItem? ToothItem { get; set; }

    private string? SelectedVersion { get; set; }

    private ToothPackage? Package { get; set; }

    private ExecutionMode Mode { get; set; }




    private readonly ObservableCollection<string> output = new();
    private LipConsoleHandler? handler;

    public LipExecutionPanelPage()
    {
        InitializeComponent();
        output.CollectionChanged += Output_CollectionChanged;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        var args = (InitArguments)e.Parameter;
        Mode = args.Mode;

        var style = (Style)Application.Current.Resources["CaptionTextBlockStyle"];
        var brush = new SolidColorBrush((Color)Application.Current.Resources["TextFillColorSecondary"]);

        switch (args.Mode)
        {
            case ExecutionMode.Install:
                {
                    var (tooth, toothItem, selectedVersion) = ((LipTooth, LipToothItem, string))args.Parameter;
                    Tooth = tooth;
                    ToothItem = toothItem;
                    SelectedVersion = selectedVersion;

                    ToothInfoPanel.Children.Add(new TextBlock()
                    {
                        Text = ToothItem.RepoName,
                        Style = style,
                        Foreground = brush
                    });
                    ToothInfoPanel.Children.Add(new TextBlock()
                    {
                        Text = SelectedVersion,
                        Style = style,
                        Foreground = brush
                    });
                }
                break;
            case ExecutionMode.Delete:
            case ExecutionMode.Update:
                {
                    Package = (ToothPackage)args.Parameter;

                    ToothInfoPanel.Children.Add(new TextBlock()
                    {
                        Text = Package.Tooth,
                        Style = style,
                        Foreground = brush
                    });
                    ToothInfoPanel.Children.Add(new TextBlock()
                    {
                        Text = Package.Version,
                        Style = style,
                        Foreground = brush
                    });
                }
                break;
        }

        base.OnNavigatedTo(e);
    }

    private void Output_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OutputViewer.ChangeView(default, OutputViewer.ScrollableHeight, default);
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        handler = new(this);
        Title.Text = Mode switch
        {
            ExecutionMode.Install => ToothItem!.RepoPath,
            ExecutionMode.Delete or ExecutionMode.Update => Package!.Tooth,
            _ => throw new Exception()
        };

        handler.Run();
    }

    private void TextBox_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key is Windows.System.VirtualKey.Enter)
        {
            var box = ((TextBox)sender);
            DispatcherQueue.TryEnqueue(() =>
            {
                handler!.Input(box.Text);
                box.Text = string.Empty;
            });
        }
    }
}
