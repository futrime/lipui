using LipUI.Models;
using LipUI.Pages;
using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using WinRT;
using static PInvoke.User32;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LipUI;

internal static class NavigationViewHelper
{
    public static DependencyObject GetChild(this DependencyObject obj, int index)
        => VisualTreeHelper.GetChild(obj, index);
}

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    WindowsSystemDispatcherQueueHelper? m_wsdqHelper; // See below for implementation.
    DesktopAcrylicController? m_backdropController;
    SystemBackdropConfiguration? m_configurationSource;

    private readonly int MinWidth = 800;
    private readonly int MinHeight = 450;

    public MainWindow()
    {
        InitializeComponent();

        SubClassing();

        TrySetSystemBackdrop();

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);

        AppWindow.Resize(new(1600, 900));

        Closed += MainWindow_Closed;
    }

    private async void MainWindow_Closed(object sender, WindowEventArgs args)
        => await Main.SaveConfigAsync();

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
        oldWndProc = NativeMethods.SetWindowLong(hwnd, PInvoke.User32.WindowLongIndexFlags.GWL_WNDPROC, newWndProc);
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

    bool TrySetSystemBackdrop()
    {
        if (DesktopAcrylicController.IsSupported())
        {
            m_wsdqHelper = new WindowsSystemDispatcherQueueHelper();
            m_wsdqHelper.EnsureWindowsSystemDispatcherQueueController();

            // Create the policy object.
            m_configurationSource = new SystemBackdropConfiguration();
            //Activated += WindowActivated;
            Closed += WindowClosed;
            ((FrameworkElement)Content).ActualThemeChanged += WindowThemeChanged;

            // Initial configuration state.
            m_configurationSource.IsInputActive = true;
            SetConfigurationSourceTheme();

            m_backdropController = new DesktopAcrylicController()
            {
                TintOpacity = 0.2f,
                LuminosityOpacity = 0.2f
            };

            // Enable the system backdrop.
            // Note: Be sure to have "using WinRT;" to support the Window.As<...>() call.
            m_backdropController.AddSystemBackdropTarget(this.As<ICompositionSupportsSystemBackdrop>());
            m_backdropController.SetSystemBackdropConfiguration(m_configurationSource);
            return true; // succeeded
        }

        return false; // Mica is not supported on this system
    }

    private void WindowActivated(object sender, WindowActivatedEventArgs args)
    {
        m_configurationSource!.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
    }

    private void WindowClosed(object sender, WindowEventArgs args)
    {
        // Make sure any Mica/Acrylic controller is disposed
        // so it doesn't try to use this closed window.
        if (m_backdropController != null)
        {
            m_backdropController.Dispose();
            m_backdropController = null;
        }
        //Activated -= WindowActivated;
        m_configurationSource = null;
    }

    private void WindowThemeChanged(FrameworkElement sender, object args)
    {
        if (m_configurationSource != null)
        {
            SetConfigurationSourceTheme();
        }
    }

    private void SetConfigurationSourceTheme()
    {
        switch (((FrameworkElement)this.Content).ActualTheme)
        {
            case ElementTheme.Dark: m_configurationSource!.Theme = SystemBackdropTheme.Dark; break;
            case ElementTheme.Light: m_configurationSource!.Theme = SystemBackdropTheme.Light; break;
            case ElementTheme.Default: m_configurationSource!.Theme = SystemBackdropTheme.Default; break;
        }
    }

    private record NavigationPages(string HomePage, string IndexPage, string PackToothPage, string LocalPackagePage, string SettingsPage);


    private readonly NavigationPages NavigationPage = new(
        typeof(HomePage).FullName!,
        typeof(IndexPage).FullName!,
        typeof(PackToothPage).FullName!,
        typeof(LocalPackagePage).FullName!,
        typeof(SettingsPage).FullName!);

    private readonly HashSet<Type> NavigationPageTypes = new()
    {
        typeof(HomePage),
        typeof(IndexPage),
        typeof(PackToothPage),
        typeof(LocalPackagePage),
        typeof(SettingsPage)
    };



    private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
    {
        throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
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
        if (navPageType is not null && !Type.Equals(preNavPageType, navPageType))
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

        ContentFrame.GoBack();
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

}

class WindowsSystemDispatcherQueueHelper
{
    [StructLayout(LayoutKind.Sequential)]
    struct DispatcherQueueOptions
    {
        internal int dwSize;
        internal int threadType;
        internal int apartmentType;
    }

    [DllImport("CoreMessaging.dll")]
    private static extern int CreateDispatcherQueueController([In] DispatcherQueueOptions options, [In, Out, MarshalAs(UnmanagedType.IUnknown)] ref object dispatcherQueueController);

    object? m_dispatcherQueueController = null;
    public void EnsureWindowsSystemDispatcherQueueController()
    {
        if (Windows.System.DispatcherQueue.GetForCurrentThread() != null)
        {
            // one already exists, so we'll just use it.
            return;
        }

        if (m_dispatcherQueueController == null)
        {
            DispatcherQueueOptions options;
            options.dwSize = Marshal.SizeOf(typeof(DispatcherQueueOptions));
            options.threadType = 2;    // DQTYPE_THREAD_CURRENT
            options.apartmentType = 2; // DQTAT_COM_STA

            CreateDispatcherQueueController(options, ref m_dispatcherQueueController!);
        }
    }
}
