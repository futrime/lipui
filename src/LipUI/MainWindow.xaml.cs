using LipUI.Models;
using LipUI.Pages.Home;
using LipUI.Pages.Index;
using LipUI.Pages.LocalPackage;
using LipUI.Pages.Settings;
using LipUI.Pages.ToothPack;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.System;
using WinRT;
using static PInvoke.User32;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LipUI;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
internal sealed partial class MainWindow : Window
{

    private readonly int MinWidth = 800;
    private readonly int MinHeight = 450;

    public MainWindow()
    {
        InitializeComponent();

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);

        SubClassing();
        //TrySetSystemBackdrop();
        AppWindow.Resize(new(1600, 900));

        Services.MainWindow = this;

        PersonalizationSettingsView.Initialize();
    }

    private async void MainWindow_Closed(object sender, WindowEventArgs args)
    {
        Services.OnWindowClosed();
        await Main.SaveConfigAsync();
    }

    private NativeMethods.WinProc? newWndProc = null;
    private nint oldWndProc = nint.Zero;

    private void SubClassing()
    {
        var hwnd = this.As<NativeMethods.IWindowNative>().WindowHandle;
        if (hwnd == nint.Zero)
        {
            int error = Marshal.GetLastWin32Error();
            throw new InvalidOperationException($"Failed to get window handler: error code {error}");
        }

        newWndProc = new(NewWindowProc);

        // Here we use the NativeMethods class 👇
        oldWndProc = NativeMethods.SetWindowLong(hwnd, WindowLongIndexFlags.GWL_WNDPROC, newWndProc);
        if (oldWndProc == nint.Zero)
        {
            int error = Marshal.GetLastWin32Error();
            throw new InvalidOperationException($"Failed to set GWL_WNDPROC: error code {error}");
        }
    }

    private nint NewWindowProc(nint hWnd, WindowMessage Msg, nint wParam, nint lParam)
    {
        [DllImport("user32.dll")]
        static extern nint CallWindowProc(nint lpPrevWndFunc, nint hWnd, WindowMessage Msg, nint wParam, nint lParam);

        switch (Msg)
        {
            case WindowMessage.WM_GETMINMAXINFO:
                var dpi = GetDpiForWindow(hWnd);
                float scalingFactor = (float)dpi / 96;

                MINMAXINFO minMaxInfo = Marshal.PtrToStructure<MINMAXINFO>(lParam);
                minMaxInfo.ptMinTrackSize.x = (int)(MinWidth * scalingFactor);
                minMaxInfo.ptMinTrackSize.y = (int)(MinHeight * scalingFactor);
                Marshal.StructureToPtr(minMaxInfo, lParam, true);
                break;

        }
        return CallWindowProc(oldWndProc, hWnd, Msg, wParam, lParam);
    }

    private static class NativeMethods
    {
        [StructLayout(LayoutKind.Sequential)]
        struct MINMAXINFO
        {
            public PInvoke.POINT ptReserved;
            public PInvoke.POINT ptMaxSize;
            public PInvoke.POINT ptMaxPosition;
            public PInvoke.POINT ptMinTrackSize;
            public PInvoke.POINT ptMaxTrackSize;
        }


        public delegate nint WinProc(nint hWnd, WindowMessage Msg, nint wParam, nint lParam);

        // We have to handle the 32-bit and 64-bit functions separately.
        // 'SetWindowLongPtr' is the 64-bit version of 'SetWindowLong', and isn't available in user32.dll for 32-bit processes.
        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern nint SetWindowLong32(nint hWnd, WindowLongIndexFlags nIndex, WinProc newProc);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern nint SetWindowLong64(nint hWnd, WindowLongIndexFlags nIndex, WinProc newProc);

        public static nint SetWindowLong(nint hWnd, WindowLongIndexFlags nIndex, WinProc newProc)
            => nint.Size is 4 ? SetWindowLong32(hWnd, nIndex, newProc) : SetWindowLong64(hWnd, nIndex, newProc);

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("EECDBF0E-BAE9-4CB6-A68E-9598E1CB57BB")]
        internal interface IWindowNative
        {
            nint WindowHandle { get; }
        }
    }

    private record NavigationPages(string HomePage, string IndexPage, string PackToothPage, string LocalPackagePage, string SettingsPage);


    private readonly NavigationPages NavigationPage = new(
        typeof(HomePage).FullName!,
        typeof(IndexPage).FullName!,
        typeof(ToothPackPage).FullName!,
        typeof(LocalPackagePage).FullName!,
        typeof(SettingsPage).FullName!);

    private readonly HashSet<Type> NavigationPageTypes = new()
    {
        typeof(HomePage),
        typeof(IndexPage),
        typeof(ToothPackPage),
        typeof(LocalPackagePage),
        typeof(SettingsPage)
    };



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
        NavView_Navigate(typeof(HomePage), new EntranceNavigationTransitionInfo());
    }

    private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {

        if (args.IsSettingsInvoked == true)
        {
            NavView_Navigate(typeof(SettingsPage), args.RecommendedNavigationTransitionInfo);
        }
        else if (args.InvokedItemContainer != null)
        {
            Type navPageType = Type.GetType(args.InvokedItemContainer.Tag.ToString()!)!;
            NavView_Navigate(navPageType, args.RecommendedNavigationTransitionInfo);
        }
    }

    private void NavView_Navigate(
        Type navPageType,
        NavigationTransitionInfo transitionInfo)
    {
        // Get the page type before navigation so you can prevent duplicate
        // entries in the backstack.
        Type preNavPageType = ContentFrame.CurrentSourcePageType;

        // Only navigate if the selected page isn't currently loaded.
        if (navPageType is not null && !Equals(preNavPageType, navPageType))
        {
            ContentFrame.Navigate(navPageType, null, transitionInfo);
        }
    }

    private void NavView_BackRequested(NavigationView sender,
                                       NavigationViewBackRequestedEventArgs args)
    {
        TryGoBack();
    }

    private bool TryGoBack()
    {
        if (!ContentFrame.CanGoBack)
            return false;

        // Don't go back if the nav pane is overlayed.
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
            // SettingsItem is not part of NavView.MenuItems, and doesn't have a Tag.
            NavView.SelectedItem = (NavigationViewItem)NavView.SettingsItem;
            //NavView.Header = "Settings";
        }
        else if (ContentFrame.SourcePageType != null)
        {
            // Select the nav view item that corresponds to the page being navigated to.

            if (NavigationPageTypes.Contains(ContentFrame.SourcePageType))
                NavView.SelectedItem = NavView.MenuItems
                    .OfType<NavigationViewItem>()
                    .First(i => i.Tag.Equals(ContentFrame.SourcePageType.FullName!.ToString()));

            //NavView.Header =
            //    ((NavigationViewItem)NavView.SelectedItem)?.Content?.ToString();

        }
    }

    internal void ShowInfoBar(
        string? title,
        string? message,
        InfoBarSeverity severity,
        TimeSpan interval = default,
        UIElement? barContent = null,
        Action? completed = null)
    {
        Task.Run(() =>
        {
            var timer = DispatcherQueue.CreateTimer();
            timer.Interval = interval == default ? TimeSpan.FromSeconds(3) : interval;
            timer.Tick += (sender, e) =>
            {
                GlobalInfoBar.IsOpen = false;
                InfoBarPopOutStoryboard.Begin();
                timer.Stop();
                completed?.Invoke();
            };

            DispatcherQueue.TryEnqueue(() =>
            {
                GlobalInfoBar.Title = title;
                GlobalInfoBar.Message = message;
                GlobalInfoBar.Severity = severity;
                GlobalInfoBar.IsClosable = false;
                GlobalInfoBar.IsOpen = true;
                GlobalInfoBar.Content = barContent;
                InfoBarPopInStoryboard.Begin();
            });

            timer.Start();
        });
    }

    internal async ValueTask ShowInfoBarAsync(string? title,
        string? message,
        InfoBarSeverity severity,
        TimeSpan interval = default,
        UIElement? barContent = null)
    {
        await Task.Run(async () =>
        {
            var mre = new ManualResetEvent(false);

            DispatcherQueue.TryEnqueue(() =>
            {
                GlobalInfoBar.Title = title;
                GlobalInfoBar.Message = message;
                GlobalInfoBar.Severity = severity;
                GlobalInfoBar.IsClosable = false;
                GlobalInfoBar.Content = barContent;
                GlobalInfoBar.IsOpen = true;

                void task(object? sender, object e)
                {
                    InfoBarPopInStoryboard.Completed -= task;
                    mre.Set();
                }
                InfoBarPopInStoryboard.Completed += task;

                InfoBarPopInStoryboard.Begin();
            });
            mre.WaitOne();

            await Task.Delay(interval);

            mre.Reset();
            DispatcherQueue.TryEnqueue(() =>
            {
                InfoBarPopOutStoryboard.Begin();

                void task(object? sender, object e)
                {
                    InfoBarPopOutStoryboard.Completed -= task;
                    GlobalInfoBar.IsOpen = false;
                    mre.Set();
                }
                InfoBarPopOutStoryboard.Completed += task;
            });
            mre.WaitOne();
            mre.Dispose();
        });
    }

    //public SolidColorBrush NavViewContentBackground => MyNavigationViewContentBackgroundBrush;

    //public SolidColorBrush NavViewContentGridBorder => MyNavigationViewContentGridBorderBrush;

    public Grid RootGrid => MainWondowRootGrid;
}
