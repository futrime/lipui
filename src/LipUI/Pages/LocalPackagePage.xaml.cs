using LipUI.Models;
using LipUI.Models.Lip;
using LipUI.Protocol;
using LipUI.VIews;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Text.Json;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LipUI.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class LocalPackagePage : Page
{
    public LocalPackagePage()
    {
        InitializeComponent();
    }

    private void ReloadPackage()
    {
        TeethScrollViewer.Content = new ProgressRing();
        ToothListView.Items.Clear();

        DispatcherQueue.TryEnqueue(async () =>
        {
            var lip = await Main.CreateLipConsole(XamlRoot);

            if (lip is null)
            {
                ((ProgressRing)TeethScrollViewer.Content).IsActive = false;
                return;
            }

            var cmd = LipCommand.CreateCommand();

            var json = await lip.RunAndGetString(cmd + LipCommand.List + LipCommandOption.Json);

            ToothPackage[]? arr = null;
            try
            {
                arr = JsonSerializer.Deserialize<ToothPackage[]>(json);
            }
            catch (Exception ex)
            {
                Helpers.ShowInfoBar(ex);
            }

            if (arr is not null)
            {
                foreach (var item in arr!)
                {
                    ToothListView.Items.Add(new LocalToothView(this, item));
                }
                RefreshUpgradableTeeth();
            }
            TeethScrollViewer.Content = ToothListView;
        });
    }

    private void RefreshUpgradableTeeth()
    {
        DispatcherQueue.TryEnqueue(async () =>
        {
            var lip = await Main.CreateLipConsole(XamlRoot);
            if (lip is null) return;

            var cmd = LipCommand.CreateCommand();
            var json = await lip.RunAndGetString(cmd + LipCommand.List + LipCommandOption.Upgradable + LipCommandOption.Json);

            ToothPackage[] arr;
            try
            {
                arr = JsonSerializer.Deserialize<ToothPackage[]>(json)!;
            }
            catch (Exception ex)
            {
                await Helpers.ShowInfoBarAsync(ex);
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

    private void ReloadPackageButton_Click(object sender, RoutedEventArgs e)
        => ReloadPackage();
}
