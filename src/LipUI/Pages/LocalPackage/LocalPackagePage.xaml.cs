using LipUI.Models;
using LipUI.Models.Lip;
using LipUI.Protocol;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LipUI.Pages.LocalPackage;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class LocalPackagePage : Page
{
    public LocalPackagePage()
    {
        InitializeComponent();
    }

    private ToothPackage[]? teeth;
    private object _lock = new();

    private void ReloadPackage(IEnumerable<ToothPackage>? items = null)
    {
        DispatcherQueue.TryEnqueue(async () =>
        {
            TeethScrollView.Content = new ProgressRing();
            ToothListView.Items.Clear();

            var lip = await Main.CreateLipConsole(XamlRoot);

            if (lip is null)
            {
                ((ProgressRing)TeethScrollView.Content).IsActive = false;
                return;
            }

            var cmd = LipCommand.CreateCommand();



            ToothPackage[]? arr = null;

            if (items is not null)
                arr = items.ToArray();
            else
                try
                {
                    var json = await lip.RunAndGetString(cmd + LipCommand.List + LipCommandOption.Json);
                    teeth = arr = JsonSerializer.Deserialize<ToothPackage[]>(json);
                }
                catch (Exception ex)
                {
                    await Services.ShowInfoBarAsync(ex);
                }

            if (arr is not null)
            {
                foreach (var item in arr!)
                {
                    ToothListView.Items.Add(new LocalToothView(this, item));
                }
                RefreshUpgradableTeeth();
            }
            TeethScrollView.Content = ToothListView;
        });
    }

    private void RefreshUpgradableTeeth()
    {
        DispatcherQueue.TryEnqueue(async () =>
        {
            var lip = await Main.CreateLipConsole(XamlRoot);
            if (lip is null) return;

            var cmd = LipCommand.CreateCommand();

            ToothPackage[] arr;
            try
            {
                var json = await lip.RunAndGetString(cmd + LipCommand.List + LipCommandOption.Upgradable + LipCommandOption.Json);
                arr = JsonSerializer.Deserialize<ToothPackage[]>(json)!;
            }
            catch (Exception ex)
            {
                await Services.ShowInfoBarAsync(ex);
                return;
            }

            HashSet<string> strings = new();

            foreach (var tooth in arr)
                strings.Add(tooth.Info.Name + tooth.Info.Author + tooth.Version);

            foreach (var i in ToothListView.Items)
            {
                var item = (LocalToothView)i;

                var tooth = item.Tooth;

                item.RefreshUpdateButton(strings.Contains(tooth.Info.Name + tooth.Info.Author + tooth.Version));
            }
        });
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
        => ReloadPackage();

    private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason is not AutoSuggestionBoxTextChangeReason.UserInput)
            return;

        var str = sender.Text.ToLower();

        Task.Run(async () =>
        {
            try
            {
                lock (_lock)
                {
                    var dataset = from tooth in teeth
                                  where tooth.Info.Name.ToLower().Contains(str)
                                  || tooth.Info.Author.ToLower().Contains(str)
                                  select tooth.Info.Name;

                    DispatcherQueue.TryEnqueue(() => sender.ItemsSource = dataset);
                }
            }
            catch (Exception ex)
            {
                await Services.ShowInfoBarAsync(ex);
            }
        });
    }

    private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        var query = args.QueryText.ToLower();
        Task.Run(async () =>
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    ReloadPackage();
                    return;
                }
                lock (_lock)
                {
                    var selectedTeeth = from tooth in teeth
                                        where tooth.Info.Name.ToLower().Contains(query)
                                        || tooth.Info.Author.ToLower().Contains(query)
                                        select tooth;

                    ReloadPackage(selectedTeeth);
                }
            }
            catch (Exception ex)
            {
                await Services.ShowInfoBarAsync(ex);
            }
        });
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

    private void ReloadPackageButton_Click(object sender, RoutedEventArgs e)
        => ReloadPackage();
}
