using LipUI.Models;
using LipUI.Protocol;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static LipUI.Protocol.LipIndex.LipIndexData;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LipUI.Pages.Index;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class IndexPage : Page
{

    private LipIndex? lipIndex;

    public IndexPage()
    {
        InitializeComponent();
    }

    private void ReloadLipIndex(IEnumerable<LipToothItem>? items = null)
    {
        DispatcherQueue.TryEnqueue(async () =>
        {
            try
            {
                TeethScrollView.Content = new ProgressRing();
                ToothListView.Items.Clear();

                if (items is null)
                {
                    lipIndex = await RequestLipIndexAsync(Main.Config.GeneralSettings.LipIndexApiKey);
                    items = lipIndex.Data.Items;
                }

                var handler = AuthorButton_Click;
                foreach (var item in items)
                {
                    ToothListView.Items.Add(new LipIndexToothView(item, handler));
                }
                TeethScrollView.Content = ToothListView;
            }
            catch (Exception ex)
            {
                TeethScrollView.Content = ToothListView;
                await InternalServices.ShowInfoBarAsync(ex);
            }
        });
    }

    private void AuthorButton_Click(string author)
    {
        DispatcherQueue.TryEnqueue(() => SuggestBox.Text = author);
        QuerySubmitted(author);
    }

    private void IndexPage_Loaded(object sender, RoutedEventArgs e)
        => ReloadLipIndex();

    private static async ValueTask<LipIndex> RequestLipIndexAsync(string lipApiUrl)
    {
        static void ThrowException(string api) => throw new NullReferenceException($"Failed to get index : {api}");

        using var client = new HttpClient();

        //foreach all lip index pages

        var text = await client.GetStringAsync($"https://{lipApiUrl}/search/teeth");
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
        if (args.Reason is not AutoSuggestionBoxTextChangeReason.UserInput)
            return;

        var str = sender.Text.ToLower();

        Task.Run(async () =>
        {
            try
            {
                lock (lipIndex!.Data.Items)
                {
                    var dataset = from tooth in lipIndex.Data.Items
                                  where tooth.Name.ToLower().Contains(str)
                                  || tooth.Author.ToLower().Contains(str)
                                  select tooth.Name;

                    DispatcherQueue.TryEnqueue(() => sender.ItemsSource = dataset);
                }
            }
            catch (Exception ex)
            {
                await InternalServices.ShowInfoBarAsync(ex);
            }
        });
    }

    private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        => QuerySubmitted(args.QueryText);

    private void QuerySubmitted(string queryText)
        => Task.Run(async () =>
        {
            try
            {
                var query = queryText.ToLower();

                if (string.IsNullOrWhiteSpace(query))
                {
                    ReloadLipIndex();
                    return;
                }
                lock (lipIndex!.Data.Items)
                {
                    var selectedTeeth = from tooth in lipIndex.Data.Items
                                        where tooth.Name.ToLower().Contains(query)
                                        || tooth.Author.ToLower().Contains(query)
                                        select tooth;

                    ReloadLipIndex(selectedTeeth);
                }
            }
            catch (Exception ex)
            {
                await InternalServices.ShowInfoBarAsync(ex);
            }
        });

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
    {
        SuggestBox.Text = null;
        ReloadLipIndex();
    }
}
