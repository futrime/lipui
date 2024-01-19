using CommunityToolkit.WinUI.Helpers;
using LipUI.Models;
using Microsoft.UI;
using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Windows.AppLifecycle;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.UI;
using WinRT;
using WinRT.Interop;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LipUI.Pages.Settings;

internal class GlobalResources : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    internal readonly SolidColorBrush appBackgroundBrush;
    internal readonly SolidColorBrush appNavigationViewContentBackgroundBrush;
    internal readonly SolidColorBrush appNavigationViewContentBorderBrush;
    internal readonly SolidColorBrush appBackgroundSecondaryColorBrush;
    internal readonly SolidColorBrush appBackgroundImageBrush;

#pragma warning disable CS8618
    public GlobalResources()
#pragma warning restore CS8618 
    {
        appBackgroundBrush = new(Colors.Transparent);
        appNavigationViewContentBackgroundBrush = new((Color)Application.Current.Resources["LayerFillColorDefault"]);
        appNavigationViewContentBorderBrush = new((Color)Application.Current.Resources["CardStrokeColorDefault"]);
        appBackgroundSecondaryColorBrush = new((Color)Application.Current.Resources["SolidBackgroundFillColorBaseAlt"]);
        appBackgroundImageBrush = new(Colors.Transparent);

        ApplicationBackground = appBackgroundBrush;
        ApplicationNavigationViewContentBackground = appNavigationViewContentBackgroundBrush;
        ApplicationNavigationViewContentBorder = appNavigationViewContentBorderBrush;
        ApplicationBackgroundSecondary = appBackgroundSecondaryColorBrush;
        ApplicationBackgroundImage = appBackgroundImageBrush;
    }


    private Brush _applicationBackground;

    public Brush ApplicationBackground
    {
        get => _applicationBackground;
        set
        {
            _applicationBackground = value;
            OnPropertyChanged();
        }
    }

    private Brush _applicationNavigationViewContentBackground;

    public Brush ApplicationNavigationViewContentBackground
    {
        get => _applicationNavigationViewContentBackground;
        set
        {
            _applicationNavigationViewContentBackground = value;
            OnPropertyChanged();
        }
    }

    private Brush _applicationNavigationViewContentBorder;

    public Brush ApplicationNavigationViewContentBorder
    {
        get => _applicationNavigationViewContentBorder;
        set
        {
            _applicationNavigationViewContentBorder = value;
            OnPropertyChanged();
        }
    }

    private Brush _applicationBackgroundSecondaryColor;

    public Brush ApplicationBackgroundSecondary
    {
        get => _applicationBackgroundSecondaryColor;
        set
        {
            _applicationBackgroundSecondaryColor = value;
            OnPropertyChanged();
        }
    }

    private Brush _applicationBackgroundImage;

    public Brush ApplicationBackgroundImage
    {
        get => _applicationBackgroundImage;
        set
        {
            _applicationBackgroundImage = value;
            OnPropertyChanged();
        }
    }

    public void ResetBrush()
    {
        ApplicationBackground = appBackgroundBrush;
        ApplicationNavigationViewContentBackground = appNavigationViewContentBackgroundBrush;
        ApplicationNavigationViewContentBorder = appNavigationViewContentBorderBrush;
        ApplicationBackgroundSecondary = appBackgroundSecondaryColorBrush;
        ApplicationBackgroundImage = appBackgroundImageBrush;
    }

    public void ResetColors()
    {
        appBackgroundBrush.Color = Colors.Transparent;
        appNavigationViewContentBackgroundBrush.Color = (Color)Application.Current.Resources["LayerFillColorDefault"];
        appNavigationViewContentBorderBrush.Color = (Color)Application.Current.Resources["CardStrokeColorDefault"];
        appBackgroundSecondaryColorBrush.Color = (Color)Application.Current.Resources["SolidBackgroundFillColorBaseAlt"];
        appBackgroundImageBrush.Color = Colors.Transparent;
    }

    public void ResetBrush(string name)
    {
        switch (name)
        {
            case nameof(ApplicationBackground):
                ApplicationBackground = appBackgroundBrush;
                break;

            case nameof(ApplicationNavigationViewContentBackground):
                ApplicationNavigationViewContentBackground = appNavigationViewContentBackgroundBrush;
                break;

            case nameof(ApplicationNavigationViewContentBorder):
                ApplicationNavigationViewContentBorder = appNavigationViewContentBorderBrush;
                break;

            case nameof(ApplicationBackgroundSecondary):
                ApplicationBackgroundSecondary = appBackgroundSecondaryColorBrush;
                break;

            case nameof(ApplicationBackgroundImage):
                ApplicationBackgroundImage = appBackgroundImageBrush;
                break;
        }
    }
}

internal sealed partial class PersonalizationSettingsView : UserControl
{
    public PersonalizationSettingsView()
    {
        InitializeComponent();
    }

    public static GlobalResources MyRes = new();

    public static void Initialize()
    {
        try
        {
            if (initialized) return;

            if (DesktopAcrylicController.IsSupported() || MicaController.IsSupported())
            {
                wsdqHelper = new WindowsSystemDispatcherQueueHelper();
                wsdqHelper.EnsureWindowsSystemDispatcherQueueController();

                configurationSource = new SystemBackdropConfiguration()
                {
                    IsInputActive = true
                };
            }

            var settings = Main.Config.PersonalizationSettings;

            if (settings.ResetColors)
            {
                MyRes.ResetColors();
                settings.BackgroundColor = MyRes.appBackgroundBrush.Color.ToHex();
                settings.NavigationViewContentBackgroundColor = MyRes.appNavigationViewContentBackgroundBrush.Color.ToHex();
                settings.NavigationViewContentBorderColor = MyRes.appNavigationViewContentBorderBrush.Color.ToHex();
                settings.BackgroundSecondaryColor = MyRes.appBackgroundSecondaryColorBrush.Color.ToHex();
                settings.ResetColors = false;
            }
            else
            {
                MyRes.appBackgroundBrush.Color = settings.BackgroundColor.ToColor();
                MyRes.appNavigationViewContentBackgroundBrush.Color = settings.NavigationViewContentBackgroundColor.ToColor();
                MyRes.appNavigationViewContentBorderBrush.Color = settings.NavigationViewContentBorderColor.ToColor();
                MyRes.appBackgroundSecondaryColorBrush.Color = settings.BackgroundSecondaryColor.ToColor();
            }

            var selection = settings.BackdropType switch
            {
                BackdropControllerType.Mica => MicaController.IsSupported() ? Mica_Selection : None_Selection,
                BackdropControllerType.Acrylic => DesktopAcrylicController.IsSupported() ? Acrylic_Selection : None_Selection,
                BackdropControllerType.Transparent => Transparent_Selection,
                BackdropControllerType.None or _ => None_Selection
            };

            BackdropChanged(selection);

            if (backdropMicaController is not null)
                backdropMicaController.LuminosityOpacity = (float)(settings.BackdropLuminosity ?? 0 / 100);
            if (backdropAcrylicController is not null)
                backdropAcrylicController.LuminosityOpacity = (float)(settings.BackdropLuminosity ?? 0 / 100);

            if (settings.EnableImageBackground && settings.BackgroundImagePath is not null)
            {
                var image = InternalServices.CreateImageFromBytes(File.ReadAllBytes(settings.BackgroundImagePath));
                MyRes.ApplicationBackgroundImage = new ImageBrush() { ImageSource = image, Stretch = Stretch.UniformToFill };
            }

            var window = InternalServices.MainWindow;
            if (window is null) return;

            window.Closed += Window_Closed;
            window.Activated += Window_Activated;
            ((FrameworkElement)window.Content).ActualThemeChanged += Window_ThemeChanged;

            initialized = true;
        }
        catch (Exception ex) { Task.Run(async () => await InternalServices.ShowInfoBarAsync(ex)); }
    }

    private void InitilizeUIFromConfig(Config config)
    {
        try
        {
            var settings = config.PersonalizationSettings;
            ColorThemeSelectionButton.Content = settings.ColorTheme switch
            {
                ElementTheme.Light => "personalizationSettings$light/Text".GetLocalized(),
                ElementTheme.Dark => "personalizationSettings$dark/Text".GetLocalized(),
                ElementTheme.Default or _ => "personalizationSettings$default/Text".GetLocalized()
            };

            var selection = settings.BackdropType switch
            {
                BackdropControllerType.Mica => MicaController.IsSupported() ? Mica_Selection : None_Selection,
                BackdropControllerType.Acrylic => DesktopAcrylicController.IsSupported() ? Acrylic_Selection : None_Selection,
                BackdropControllerType.Transparent => Transparent_Selection,
                BackdropControllerType.None or _ => None_Selection
            };
            BackgroundStyleRadioButtons.SelectedItem = selection;
            LuminosityOpacitySlider.Value = settings.BackdropLuminosity ?? 0;

            BackgroundImageEnableCheckBox.IsChecked = settings.EnableImageBackground;
            if (settings.EnableImageBackground && settings.BackgroundImagePath is not null)
            {
                var image = InternalServices.CreateImageFromBytes(File.ReadAllBytes(settings.BackgroundImagePath));
                image.DecodePixelType = DecodePixelType.Logical;
                image.DecodePixelWidth = 256;

                PreviewImage.Source = image;
            }
        }
        catch (Exception ex) { Task.Run(async () => await InternalServices.ShowInfoBarAsync(ex)); }
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
        => InitilizeUIFromConfig(Main.Config);

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

    private static readonly string None_Selection = "personalizationSettings$backdrop$none".GetLocalized();
    private static readonly string Transparent_Selection = "personalizationSettings$backdrop$transparent".GetLocalized();
    private static readonly string Mica_Selection = "personalizationSettings$backdrop$mica".GetLocalized();
    private static readonly string Acrylic_Selection = "personalizationSettings$backdrop$acrylic".GetLocalized();

    private void MyColorPickerFlyout_Opening(object sender, object e)
    {
        try
        {
            var flyout = (Flyout)sender;
            ColorPicker.Color = ((SolidColorBrush)((Button)flyout.Target).Background).Color;
        }
        catch (Exception ex) { Task.Run(() => InternalServices.ShowInfoBarAsync(ex)); }
    }

    private async void ConfirmColorButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (MyColorPickerFlyout.Target is not Button button)
                return;

            ((SolidColorBrush)button.Background).Color = ColorPicker.Color;
            ColorChanged?.Invoke(button, ColorPicker.Color);
        }
        catch (Exception ex) { await InternalServices.ShowInfoBarAsync(ex, containsStacktrace: true); }
        BackgroundBrushSettingsPanel_ColorPickerButton.Flyout.Hide();
    }

    private void CancelColorButton_Click(object sender, RoutedEventArgs e)
        => BackgroundBrushSettingsPanel_ColorPickerButton.Flyout.Hide();

    private event Action<Button, Color>? ColorChanged;

    #region BackdropController

    private static WindowsSystemDispatcherQueueHelper? wsdqHelper; // See below for implementation.
    private static SystemBackdropConfiguration? configurationSource;
    private static bool initialized = false;

    private static DesktopAcrylicController? backdropAcrylicController;
    private static MicaController? backdropMicaController;
    private static TransparentTintBackdrop? transparentTintBackdrop;

    private static void ResetController()
    {

        InternalServices.MainWindow!.SystemBackdrop = null;

        if (backdropAcrylicController is not null)
        {
            //backdropAcrylicController.ResetProperties();
            backdropAcrylicController.RemoveAllSystemBackdropTargets();
        }

        if (backdropMicaController is not null)
        {
            //backdropMicaController.ResetProperties();
            backdropMicaController.RemoveAllSystemBackdropTargets();
        }
    }

    private static bool TryEnableAcrylicController()
    {
        try
        {

            if (DesktopAcrylicController.IsSupported() is false)
                return false;

            if (InternalServices.MainWindow is null)
                return false;

            backdropAcrylicController ??= new DesktopAcrylicController();
            backdropAcrylicController.LuminosityOpacity = (float)Main.Config.PersonalizationSettings.BackdropLuminosity! / 100;


            backdropAcrylicController.AddSystemBackdropTarget(
                InternalServices.MainWindow.As<ICompositionSupportsSystemBackdrop>());

            backdropAcrylicController.SetSystemBackdropConfiguration(configurationSource);

            return true;
        }
        catch (Exception ex)
        {
            Task.Run(() => InternalServices.ShowInfoBarAsync(ex));
            return false;
        }
    }

    private static bool TryEnableMicaController()
    {
        try
        {

            if (MicaController.IsSupported() is false)
                return false;

            if (InternalServices.MainWindow is null)
                return false;

            backdropMicaController ??= new MicaController();
            backdropMicaController.LuminosityOpacity = (float)Main.Config.PersonalizationSettings.BackdropLuminosity! / 100;


            backdropMicaController.AddSystemBackdropTarget(
                InternalServices.MainWindow.As<ICompositionSupportsSystemBackdrop>());

            backdropMicaController.SetSystemBackdropConfiguration(configurationSource);

            return true;
        }
        catch (Exception ex)
        {
            Task.Run(() => InternalServices.ShowInfoBarAsync(ex));
            return false;
        }
    }

    private static bool TryEnableTransparentBackdrop()
    {
        try
        {
            if (InternalServices.MainWindow is null)
                return false;

            transparentTintBackdrop ??= new TransparentTintBackdrop();

            InternalServices.MainWindow.SystemBackdrop = transparentTintBackdrop;

            return true;
        }
        catch (Exception ex)
        {
            Task.Run(() => InternalServices.ShowInfoBarAsync(ex));
            return false;
        }
    }

    private static void Window_Activated(object sender, WindowActivatedEventArgs args)
    {
    }

    private static void Window_Closed(object sender, WindowEventArgs args)
    {
        if (backdropAcrylicController is not null)
        {
            backdropAcrylicController.Dispose();
            backdropAcrylicController = null;
        }

        if (backdropMicaController is not null)
        {
            backdropMicaController.Dispose();
            backdropMicaController = null;
        }

        configurationSource = null;
    }

    private static void Window_ThemeChanged(FrameworkElement sender, object args)
    {
        if (configurationSource is not null) SetConfigurationSourceTheme(InternalServices.MainWindow!);
    }

    private static void SetConfigurationSourceTheme(Window window)
    {
        if (configurationSource is null)
            return;

        switch (((FrameworkElement)window.Content).ActualTheme)
        {
            case ElementTheme.Dark: configurationSource.Theme = SystemBackdropTheme.Dark; break;
            case ElementTheme.Light: configurationSource.Theme = SystemBackdropTheme.Light; break;
            case ElementTheme.Default: configurationSource.Theme = SystemBackdropTheme.Default; break;
        }
    }

    private static void BackdropChanged(string? selection)
    {
        if (selection == None_Selection)
        {
            ResetController();
            CurrentBackdrop = BackdropControllerType.None;
        }
        else if (selection == Transparent_Selection)
        {
            ResetController();
            TryEnableTransparentBackdrop();
            CurrentBackdrop = BackdropControllerType.Transparent;
        }
        else if (selection == Mica_Selection)
        {
            ResetController();
            TryEnableMicaController();

            CurrentBackdrop = BackdropControllerType.Mica;
        }
        else if (selection == Acrylic_Selection)
        {
            ResetController();
            TryEnableAcrylicController();

            CurrentBackdrop = BackdropControllerType.Acrylic;
        }
    }

    #endregion


    #region BackdropControllerSelection

    public enum BackdropControllerType { Mica, Acrylic, Transparent, None }

    private static BackdropControllerType CurrentBackdrop
    {
        get
        {
            return Main.Config.PersonalizationSettings.BackdropType;
        }

        set
        {
            Main.Config.PersonalizationSettings.BackdropType = value;
            Task.Run(Main.SaveConfigAsync);
        }
    }

    private void BackgroundStyleRadioButtons_Loading(FrameworkElement sender, object args)
    {
        var button = (RadioButtons)sender;
        var items = button.Items;

        items.Add(None_Selection);
        items.Add(Transparent_Selection);

        if (MicaController.IsSupported())
            items.Add(Mica_Selection);

        if (DesktopAcrylicController.IsSupported())
            items.Add(Acrylic_Selection);
    }

    private void BackgroundStyleRadioButtons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        => SelectionChanged(((RadioButtons)sender).SelectedItem as string);

    private void SelectionChanged(string? selection)
    {
        BackdropChanged(selection);

        LuminosityOpacitySlider.Value = Main.Config.PersonalizationSettings.BackdropLuminosity ?? 0;
        ChangeBackdropSettingsPanel(CurrentBackdrop);
    }

    private void ChangeBackdropSettingsPanel(BackdropControllerType backdropType)
    {
        switch (backdropType)
        {
            case BackdropControllerType.Mica:
            case BackdropControllerType.Acrylic:
                BackdropControllerSettingsPanel.Visibility = Visibility.Visible;
                break;

            case BackdropControllerType.None:
            case BackdropControllerType.Transparent:
                BackdropControllerSettingsPanel.Visibility = Visibility.Collapsed;
                break;
        }
    }

    #endregion

    private async void LuminosityOpacitySlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        try
        {
            switch (CurrentBackdrop)
            {
                case BackdropControllerType.Mica:
                    if (backdropMicaController is not null)
                        backdropMicaController.LuminosityOpacity = (float)((Slider)sender).Value / 100;
                    break;
                case BackdropControllerType.Acrylic:
                    if (backdropAcrylicController is not null)
                        backdropAcrylicController.LuminosityOpacity = (float)((Slider)sender).Value / 100;
                    break;
                case BackdropControllerType.None:
                    break;
            }

            Main.Config.PersonalizationSettings.BackdropLuminosity = ((Slider)sender).Value;
            await Main.SaveConfigAsync();
        }
        catch (Exception ex) { await InternalServices.ShowInfoBarAsync(ex, containsStacktrace: true); }
    }

    private void BackgroundBrushSettingsPanel_ColorPickerButton_Loading(FrameworkElement sender, object args)
    {
        ((Button)sender).Background = MyRes.ApplicationBackground;
        ColorChanged += (button, color) =>
        {
            if (button == sender)
            {
                MyRes.appBackgroundBrush.Color = color;
                Main.Config.PersonalizationSettings.BackgroundColor = color.ToHex();
                Task.Run(Main.SaveConfigAsync);
            }
        };
    }

    private void NavigationViewContentBackgroundBrush_ColorPickerButton_Loading(FrameworkElement sender, object args)
    {
        ((Button)sender).Background = MyRes.ApplicationNavigationViewContentBackground;
        ColorChanged += (button, color) =>
        {
            if (button == sender)
            {
                MyRes.appNavigationViewContentBackgroundBrush.Color = color;
                Main.Config.PersonalizationSettings.NavigationViewContentBackgroundColor = color.ToHex();
                Task.Run(Main.SaveConfigAsync);
            }
        };
    }

    private void NavigationViewContentGridBorderBrush_ColorPickerButton_Loading(FrameworkElement sender, object args)
    {
        ((Button)sender).Background = MyRes.ApplicationNavigationViewContentBorder;
        ColorChanged += (button, color) =>
        {
            if (button == sender)
            {
                MyRes.appNavigationViewContentBorderBrush.Color = color;
                Main.Config.PersonalizationSettings.NavigationViewContentBorderColor = color.ToHex();
                Task.Run(Main.SaveConfigAsync);
            }
        };
    }

    private void BackgroundSecondaryColor_ColorPickerButton_Loading(FrameworkElement sender, object args)
    {
        ((Button)sender).Background = MyRes.ApplicationBackgroundSecondary;
        ColorChanged += (button, color) =>
        {
            if (button == sender)
            {
                MyRes.appBackgroundSecondaryColorBrush.Color = color;
                Main.Config.PersonalizationSettings.BackgroundSecondaryColor = color.ToHex();
                Task.Run(Main.SaveConfigAsync);
            }
        };
    }

    private async void CheckBox_Checked(object sender, RoutedEventArgs e)
    {
        try
        {
            ImageBackgroundEnabledPanel.Visibility = Visibility.Visible;
            Main.Config.PersonalizationSettings.EnableImageBackground = true;
            await Main.SaveConfigAsync();

            var path = Main.Config.PersonalizationSettings.BackgroundImagePath;
            if (path is not null)
            {
                var image = InternalServices.CreateImageFromBytes(await File.ReadAllBytesAsync(path));

                image.DecodePixelType = DecodePixelType.Logical;
                image.DecodePixelWidth = 256;
                PreviewImage.Source = image;

                image = InternalServices.CreateImageFromBytes(await File.ReadAllBytesAsync(path));
                MyRes.ApplicationBackgroundImage = new ImageBrush() { ImageSource = image, Stretch = Stretch.UniformToFill };
            }
        }
        catch (Exception ex) { await InternalServices.ShowInfoBarAsync(ex); }
    }

    private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        ImageBackgroundEnabledPanel.Visibility = Visibility.Collapsed;
        Main.Config.PersonalizationSettings.EnableImageBackground = false;
        Task.Run(Main.SaveConfigAsync);

        PreviewImage.Source = null;
        MyRes.ResetBrush(nameof(GlobalResources.ApplicationBackgroundImage));
    }

    private async void SelectImageButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            string imagePath = string.Empty;

            var picker = new FileOpenPicker()
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                FileTypeFilter = { ".jpeg", ".jpg", ".png", ".bmp", ".tiff", ".ico" }
            };
            InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle((Application.Current as App)!.m_window));

            var file = await picker.PickSingleFileAsync();
            if (file is not null)
                imagePath = file.Path;

            var image = InternalServices.CreateImageFromBytes(await File.ReadAllBytesAsync(imagePath));
            image.DecodePixelType = DecodePixelType.Logical;
            image.DecodePixelWidth = 256;
            PreviewImage.Source = image;

            image = InternalServices.CreateImageFromBytes(await File.ReadAllBytesAsync(imagePath));
            MyRes.ApplicationBackgroundImage = new ImageBrush() { ImageSource = image, Stretch = Stretch.UniformToFill };

            Main.Config.PersonalizationSettings.BackgroundImagePath = imagePath;
            await Main.SaveConfigAsync();
        }
        catch (Exception ex) { await InternalServices.ShowInfoBarAsync(ex); }

    }

    private void ClearImageButton_Click(object sender, RoutedEventArgs e)
    {
        PreviewImage.Source = null;
        MyRes.ResetBrush(nameof(GlobalResources.ApplicationBackgroundImage));
        Main.Config.PersonalizationSettings.BackgroundImagePath = null;
    }

    private void BackgroundImageOpacitySlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        MyRes.ApplicationBackgroundImage.Opacity = e.NewValue / 100;
        Main.Config.PersonalizationSettings.BackdropLuminosity = e.NewValue;
        Task.Run(Main.SaveConfigAsync);
    }

    private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
    {
        var option = ((MenuFlyoutItem)sender).Tag.ToString();
        ColorThemeSelectionButton.Content = option;

        Main.Config.PersonalizationSettings.ColorTheme = option switch
        {
            "light" => ElementTheme.Light,
            "dark" => ElementTheme.Dark,
            "default" or _ => ElementTheme.Default,
        };
        Task.Run(Main.SaveConfigAsync);

        Button button = new()
        {
            Content = "application$restart".GetLocalized(),
            HorizontalAlignment = HorizontalAlignment.Left
        };

        button.Click += async (sender, e) =>
        {
            var rlt = AppInstance.Restart(string.Empty);

            await InternalServices.ShowInfoBarAsync(
                title: "infoBar$error".GetLocalized(),
                message: rlt.ToString(),
                severity: InfoBarSeverity.Error);
        };

        Task.Run(() => InternalServices.ShowInfoBarAsync(
            title: "infoBar$warning".GetLocalized(),
            message: "application$restartMessage".GetLocalized(),
            severity: InfoBarSeverity.Warning,
            barContent: button));
    }
}
