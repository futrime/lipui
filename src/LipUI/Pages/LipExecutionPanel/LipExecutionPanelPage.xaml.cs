using LipUI.Models;
using LipUI.Models.Lip;
using LipUI.Protocol;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
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

namespace LipUI.Pages.LipExecutionPanel;

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

        private static Style CaptionTextBlockStyle = (Style)Application.Current.Resources["CaptionTextBlockStyle"];
        private static Brush TextFillColorPrimaryBrush = (Brush)Application.Current.Resources["TextFillColorPrimaryBrush"];

        private readonly List<(Regex, Func<Regex, Match, string, bool>)> regexsAndMethods;

        public void Output(string line)
            => dispatcherQueue.TryEnqueue(() => page.output.Add(new()
            {
                Text = line,
                Style = CaptionTextBlockStyle,
                Foreground = TextFillColorPrimaryBrush
            }));

        public void Input(string line, bool output = true)
        {
            if (output) dispatcherQueue.TryEnqueue(() => page.output.Add(new()
            {
                Text = line,
                Style = CaptionTextBlockStyle,
                Foreground = TextFillColorPrimaryBrush
            }));
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
            regexsAndMethods =
            [
                (Y_or_N_Regex(), Y_or_N_Matched),
                (BDSDownloader_ProgressBar_Regex(), BDSDownloader_ProgressBar_Matched),
                (BDSDownloader_StartDownloadOrUnziping_Regex(), BDSDownloader_StartDownloadOrUnziping_Matched),
                (BDSDownloader_DownloadComplete_Regex(), BDSDownloader_DownloadComplete_Matched),
                (Done_Regex(), Done_Matched),
                (Lip_ProgressBar_Regex(), Lip_ProgressBar_Matched),
                (Lip_StartDownload_Regex(), Lip_StartDownload_Matched)
            ];
        }

        public void Run()
        {
            Action closed = null!;
            closed = () =>
            {
                process?.Kill();
                InternalServices.WindowClosed -= closed;
            };
            InternalServices.WindowClosed += closed;

            dispatcherQueue.TryEnqueue(async () =>
            {
                lip = page.args!.Lip;

                lip.Output += OutputHandler;
                lip.OutputError += OutputHandler;

                await lip.Run(page.args!.LipCommand, ins => process = ins.Process);

                InternalServices.WindowClosed -= closed;
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

    private readonly ObservableCollection<TextBlock> output = [];
    private LipConsoleHandler? handler;
    private NavigationArgs? args;

    public LipExecutionPanelPage()
    {
        InitializeComponent();
        output.CollectionChanged += Output_CollectionChanged;
    }

    public record NavigationArgs(string Title, IReadOnlyList<string> Info, LipConsole Lip, LipCommandContext LipCommand);

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        args = (NavigationArgs)e.Parameter;

        var style = (Style)Application.Current.Resources["CaptionTextBlockStyle"];
        var brush = new SolidColorBrush((Color)Application.Current.Resources["TextFillColorSecondary"]);

        foreach (var item in args.Info)
        {
            ToothInfoPanel.Children.Add(new TextBlock()
            {
                Text = item,
                Style = style,
                Foreground = brush
            });
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
        Title.Text = args!.Title;
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
