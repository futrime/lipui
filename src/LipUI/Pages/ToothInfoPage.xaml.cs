#pragma warning disable CS8632

using CommunityToolkit.WinUI.UI.Controls;
using LipUI.Models;
using LipUI.Models.Lip;
using LipUI.Pages.LipExecutionPanel;
using LipUI.Protocol;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
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
            LatestVersionReleasedAt.Text = toothItem.LatestVersionReleasedAt;
            SourceRepoCreatedAt.Text = toothItem.RepoCreatedAt;
            SourceRepoStarCount.Text = toothItem.StarCount.ToString();

        }
    }

    private string ToothRepoUri => $"https://{ToothItem.RepoPath}";

    public ToothInfoPage()
    {
        InitializeComponent();
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
            try
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
            }
            catch (Exception ex)
            {
                await InternalServices.ShowInfoBarAsync(ex);
            }
            finally
            {
                ToothVersionSelectButton.Content = new TextBlock()
                {
                    Text = ToothItem.LatestVersion
                };
            }
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

    private async void InstallButton_Click(object sender, RoutedEventArgs e)
    {
        if (ToothVersionSelectButton.Content is TextBlock text)
        {
            var lip = await Main.CreateLipConsole(XamlRoot);
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

            var cmd = LipCommand.CreateCommand() + LipCommand.Install + toothItem!.RepoPath;
            var info = new List<string>()
            {
                toothItem!.RepoPath,
                toothItem!.RepoOwner,
                toothItem!.RepoName,
                toothItem!.LatestVersion,
                toothItem!.LatestVersionReleasedAt,
                toothItem!.Name,
                toothItem!.Description,
                toothItem!.Author,
                toothItem!.AvatarUrl,
                toothItem!.RepoCreatedAt,
            };

            Frame.Navigate(
                typeof(LipExecutionPanelPage),
                new LipExecutionPanelPage.NavigationArgs(toothItem.RepoPath, info, lip, cmd));

        }
    }
}
