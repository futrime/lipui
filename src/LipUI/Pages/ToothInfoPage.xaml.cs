#pragma warning disable CS8632

using CommunityToolkit.WinUI.UI.Controls;
using LipUI.Models;
using LipUI.Pages.LipExecutionPanel;
using LipUI.Protocol;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Net.Http;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LipUI.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ToothInfoPage : Page
{

    private LipIndex.LipIndexData.LipToothItem? toothItem;

    private LipTooth? tooth;

    private LipIndex.LipIndexData.LipToothItem ToothItem
    {
        get => toothItem!;
        set
        {
            toothItem = value;

            Title.Text = toothItem.Name;
            RepoPath.Text = toothItem.RepoPath;
            Description.Text = toothItem.Description;
            Author.Text = toothItem.Author;
            LatestVersion.Text = toothItem.LatestVersion;
            LatestReleaseTime.Text
                = DateTimeOffset
                .FromUnixTimeSeconds(toothItem.LatestVersionReleaseTime)
                .LocalDateTime
                .ToString();
            DownloadCount.Text = toothItem.DownloadCount.ToString();

        }
    }

    private string ToothRepoUri => $"https://{ToothItem.RepoPath}";

    public ToothInfoPage()
    {
        this.InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        ToothItem = (e.Parameter as LipIndex.LipIndexData.LipToothItem)!;
        base.OnNavigatedTo(e);
    }

    private void CopyRepoPathButton_Click(object sender, RoutedEventArgs e)
    {
        DataPackage package = new()
        {
            RequestedOperation = DataPackageOperation.Copy
        };
        package.SetText(ToothItem.RepoPath);
        Clipboard.SetContent(package);
    }

    private void MarkdownViewer_Loaded(object sender, RoutedEventArgs e)
    {
        MarkdownViewer.Content = new ProgressRing();

        DispatcherQueue.TryEnqueue(async () =>
        {
            try
            {
                var client = new Octokit.GitHubClient(new Octokit.ProductHeaderValue("lippkg"));
                var readme = await client.Repository.Content.GetReadme(ToothItem.RepoOwner, ToothItem.RepoName);
                MarkdownViewer.Content = new MarkdownTextBlock()
                {
                    Margin = new(24, 8, 24, 8),
                    Background = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255)),
                    Text = readme.Content
                };
            }
            catch (Octokit.NotFoundException)
            {
                MarkdownViewer.Content = new TextBlock()
                {
                    Text = "readme$notfound".GetLocalized(),
                    Foreground = Application.Current.Resources["TextFillColorSecondary"] as SolidColorBrush,
                    Style = Application.Current.Resources["SubtitleTextBlockStyle"] as Style
                };
            }
        });
    }

    private void ToothVersionSelectButton_Loaded(object sender, RoutedEventArgs e)
    {
        ToothVersionSelectButton.Content = new ProgressRing()
        {
            Height = 16,
            Width = 16
        };

        DispatcherQueue.TryEnqueue(async () =>
        {
            using var client = new HttpClient();
            var dataStr = await client.GetStringAsync(
                $"https://{Main.Config.GeneralSettings.LipIndexApiKey}/teeth/{ToothItem.RepoOwner}/{ToothItem.RepoName}/{ToothItem.LatestVersion}");

            tooth = LipTooth.Deserialize(dataStr);

            var flyout = new MenuFlyout();
            ToothVersionSelectButton.Flyout = flyout;

            foreach (var version in tooth.Data.Versions)
            {
                var item = new MenuFlyoutItem() { Text = version.Version };
                item.Click += Item_Click;

                flyout.Items.Add(item);
            }

            ToothVersionSelectButton.Content = new TextBlock()
            {
                Text = ToothItem.LatestVersion
            };
        });
    }

    private void Item_Click(object sender, RoutedEventArgs e)
    {
        if (ToothVersionSelectButton.Content is TextBlock textBlock)
        {
            var item = sender as MenuFlyoutItem;
            textBlock.Text = item!.Text;
        }
    }

    private void InstallButton_Click(object sender, RoutedEventArgs e)
    {
        if (ToothVersionSelectButton.Content is TextBlock text)
            Frame.Navigate(
                typeof(LipExecutionPanelPage),
                new LipExecutionPanelPage.InitArguments(
                    LipExecutionPanelPage.ExecutionMode.Install,
                    (tooth, toothItem, text.Text)));
    }
}
