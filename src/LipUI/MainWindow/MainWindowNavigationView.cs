using LipUI.Models;
using LipUI.Models.Plugin;
using LipUI.Pages.Home;
using LipUI.Pages.Index;
using LipUI.Pages.LocalPackage;
using LipUI.Pages.Settings;
using LipUI.Pages.ToothPack;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System.ComponentModel;

namespace LipUI;

internal partial class MainWindow
{
    private record NavigationPages(string HomePage, string IndexPage, string ModuleManagerPage, string LocalPackagePage, string SettingsPage);


    private readonly NavigationPages NavigationPage = new(
        typeof(HomePage).FullName!,
        typeof(IndexPage).FullName!,
        typeof(ModuleManagerPage).FullName!,
        typeof(LocalPackagePage).FullName!,
        typeof(SettingsPage).FullName!);

    private readonly HashSet<Type> NavigationPageTypes = [
        typeof(HomePage),
        typeof(IndexPage),
        typeof(ModuleManagerPage),
        typeof(LocalPackagePage),
        typeof(SettingsPage)
    ];



    private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
    {
        throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
    }

    private void NavView_Loading(FrameworkElement sender, object args)
    {
        sender.Resources["NavigationViewContentBackground"]
            = PersonalizationSettingsView.MyRes.ApplicationNavigationViewContentBackground;
        sender.Resources["NavigationViewContentGridBorderBrush"]
            = PersonalizationSettingsView.MyRes.ApplicationNavigationViewContentBorder;

        PersonalizationSettingsView.MyRes.PropertyChanged += (object? _sender, PropertyChangedEventArgs e) =>
        {
            switch (e.PropertyName)
            {
                case nameof(GlobalResources.ApplicationNavigationViewContentBackground):
                    sender.Resources["NavigationViewContentBackground"]
                    = PersonalizationSettingsView.MyRes.ApplicationNavigationViewContentBackground;
                    break;

                case nameof(GlobalResources.ApplicationNavigationViewContentBorder):
                    sender.Resources["NavigationViewContentGridBorderBrush"]
                    = PersonalizationSettingsView.MyRes.ApplicationNavigationViewContentBorder;
                    break;

                case nameof(GlobalResources.ApplicationBackground):
                    RootBorder.Background = PersonalizationSettingsView.MyRes.ApplicationBackground;
                    break;
            }
        };
    }

    private void NavView_Loaded(object sender, RoutedEventArgs e)
    {
        // You can also add items in code.

        // Add handler for ContentFrame navigation.
        ContentFrame.Navigated += On_Navigated;

        // NavView doesn't load any page by default, so load home page.
        NavView.SelectedItem = NavView.MenuItems[0];
        // If navigation occurs on SelectionChanged, this isn't needed.
        // Because we use ItemInvoked to navigate, we need to call Navigate
        // here to load the home page.
        NavView_Navigate(typeof(HomePage), null, new EntranceNavigationTransitionInfo());
    }

    private async void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {

        if (args.IsSettingsInvoked == true)
        {
            NavView_Navigate(typeof(SettingsPage), null, args.RecommendedNavigationTransitionInfo);
        }
        else if (args.InvokedItemContainer != null)
        {
            if (args.InvokedItemContainer.Tag is null)
                return;

            if (args.InvokedItemContainer.Tag is IUIPlugin plugin)
            {
                try
                {
                    NavView_Navigate(
                    plugin.PageType, plugin.PageParameterRequsted(), args.RecommendedNavigationTransitionInfo);
                }
                catch (Exception ex) { await InternalServices.ShowInfoBarAsync(ex); }
            }
            else
            {
                NavView_Navigate(
                    Type.GetType(args.InvokedItemContainer.Tag.ToString()!)!,
                    null,
                    args.RecommendedNavigationTransitionInfo);
            }
        }
    }

    private void NavView_Navigate(
        Type navPageType,
        object? parameter,
        NavigationTransitionInfo transitionInfo)
    {
        Type preNavPageType = ContentFrame.CurrentSourcePageType;

        if (navPageType is not null && !Equals(preNavPageType, navPageType))
        {
            ContentFrame.Navigate(navPageType, parameter, transitionInfo);
        }
    }

    private void NavView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
    {
        TryGoBack();
    }

    private bool TryGoBack()
    {
        if (!ContentFrame.CanGoBack)
            return false;

        if (NavView.IsPaneOpen &&
            (NavView.DisplayMode is NavigationViewDisplayMode.Compact ||
             NavView.DisplayMode is NavigationViewDisplayMode.Minimal))
            return false;

        ContentFrame.TryGoBack();
        return true;
    }

    private void On_Navigated(object sender, NavigationEventArgs e)
    {
        NavView.IsBackEnabled = ContentFrame.CanGoBack;

        if (ContentFrame.SourcePageType == typeof(SettingsPage))
        {
            NavView.SelectedItem = (NavigationViewItem)NavView.SettingsItem;
        }
        else if (ContentFrame.SourcePageType != null)
        {

            if (NavigationPageTypes.Contains(ContentFrame.SourcePageType))
                NavView.SelectedItem = NavView.MenuItems
                    .OfType<NavigationViewItem>()
                    .First(i => i.Tag.Equals(ContentFrame.SourcePageType.FullName!.ToString()));
        }
    }

}
