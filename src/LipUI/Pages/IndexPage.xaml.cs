using LipUI.Models;
using LipUI.Protocol;
using LipUI.VIews;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Net.Http;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LipUI.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class IndexPage : Page
{

    private LipIndex? LipIndex { get; set; }

    public IndexPage()
    {
        InitializeComponent();
    }

    private void ReloadLipIndex()
    {
        TeethScrollViewer.Content = new ProgressRing();
        ToothListView.Items.Clear();

        DispatcherQueue.TryEnqueue(async () =>
        {
            LipIndex = await RequestLipIndexAsync(Main.Config.Settings.LipIndexApiKey);

            foreach (var item in LipIndex.Data.Items)
            {
                ToothListView.Items.Add(new LipIndexToothView(item));
            }
            TeethScrollViewer.Content = ToothListView;
        });
    }

    private void IndexPage_Loaded(object sender, RoutedEventArgs e)
        => ReloadLipIndex();

    private static async ValueTask<LipIndex> RequestLipIndexAsync(string lipApiUrl)
    {
        static void ThrowException(string api) => throw new NullReferenceException($"Failed to get index : {api}");

        using var client = new HttpClient();

        //foreach all lip index pages

        var text = await client.GetStringAsync($"https://{lipApiUrl}/search/teeth?page=1");
        if (string.IsNullOrWhiteSpace(text))
            ThrowException(lipApiUrl);

        var index = LipIndex.Deserialize(text)!;
        var pageCount = index.Data.TotalPages;
        for (int i = 2; i <= pageCount; ++i)
        {
            text = await client.GetStringAsync($"https://{lipApiUrl}/search/teeth?&page={i}");
            if (string.IsNullOrWhiteSpace(text))
                ThrowException(lipApiUrl);

            var temp = LipIndex.Deserialize(text)!;

            index.Data.Items.AddRange(temp.Data.Items);
        }

        return index;
    }

    private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {

    }

    private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {

    }

    private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {

    }

    private void FeaturedFilterButton_Checked(object sender, RoutedEventArgs e)
    {

    }

    private void FeaturedFilterButton_Unchecked(object sender, RoutedEventArgs e)
    {

    }

    private void ToothListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        var view = e.ClickedItem as LipIndexToothView;
        Frame.Navigate(typeof(ToothInfoPage), view!.Tooth);
    }

    private void ReloadPackageButton_Click(object sender, RoutedEventArgs e)
        => ReloadLipIndex();
}
