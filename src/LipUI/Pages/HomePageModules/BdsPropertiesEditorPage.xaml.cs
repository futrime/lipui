using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LipUI.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Devices.Radios;
using Windows.UI;
using Microsoft.UI.Xaml.Input;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LipUI.Pages.HomePageModules;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
internal sealed partial class BdsPropertiesEditorPage : Page
{
    private static readonly Dictionary<string, string[]> enums = new()
    {
        { "gamemode", new string[]{ "survival", "creative", "adventure" } },
        { "difficulty", new string[]{ "peaceful", "easy", "normal", "hard" } },
        { "default-player-permission-level", new string[]{ "visitor", "member", "operator" } },
        { "compression-algorithm", new string[]{ "zlib", "easy", "snappy" } },
        { "server-authoritative-movement", new string[]{ "client-auth", "server-auth", "server-auth-with-rewind" } },
        { "chat-restriction", new string[]{ "None", "Dropped", "Disabled" } },
    };

    public ServerInstance? Server { get; private set; }

    public Dictionary<string, string> BindingSettings { get; private set; } = new();


    public BdsPropertiesEditorPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        Server = e.Parameter as ServerInstance;

        base.OnNavigatedTo(e);
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        if (Server is null)
        {
            var dialog = new ContentDialog()
            {
                XamlRoot = XamlRoot,
                Content = "i18n.nullServerPath",
                CloseButtonText = "OK"
            };
            await dialog.ShowAsync();
            Frame.GoBack();
        }

        Viewer.Content = new ProgressRing();
        DispatcherQueue.TryEnqueue(LoadPropertiesAndCreateUI);
    }

    private async ValueTask SaveAsync()
    {
        if (BindingSettings.Count is 0)
            return;

        var path = Path.Combine(Server!.WorkingDirectory, "server.properties");
        var lines = await File.ReadAllLinesAsync(path);

        for (int i = 0; i < lines.Length; i++)
        {
            string? line = lines[i];
            if (line.StartsWith('#') || string.IsNullOrWhiteSpace(line))
                continue;

            var key = line[..line.IndexOf('=')];
            if (BindingSettings.TryGetValue(key, out var value))
                lines[i] = $"{key}={value}";
        }

        await File.WriteAllLinesAsync(path, lines);
    }

    private async void LoadPropertiesAndCreateUI()
    {
        var path = Path.Combine(Server!.WorkingDirectory, "server.properties");
        var lines = await File.ReadAllLinesAsync(path);

        string? currentPropertiesLine, nextPropertiesLine = null;
        var notes = new List<string>();
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            if (line.StartsWith('#'))
            {
                notes.Add(line[1..].Trim());
                continue;
            }
            else
            {
                currentPropertiesLine = nextPropertiesLine;
                nextPropertiesLine = line;
            }

            if (currentPropertiesLine is not null)
            {
                var index = currentPropertiesLine.IndexOf('=');
                var key = currentPropertiesLine[..index];
                var value = currentPropertiesLine[(index + 1)..];

                if (bool.TryParse(value, out _))
                    PropertiesView.Items.Add(BoolTypeView(key, value, notes.ToArray(), BindingSettings));
                else if (enums.TryGetValue(key, out var _enums))
                    PropertiesView.Items.Add(EnumTypeView(key, value, notes.ToArray(), BindingSettings, _enums));
                else
                    PropertiesView.Items.Add(StringTypeView(key, value, notes.ToArray(), BindingSettings));

                notes.Clear();
            }
        }

        Viewer.Content = PropertiesView;
    }

    private static UIElement BoolTypeView(
        string key,
        string? currentValue,
        string[] notes,
        IDictionary<string, string> bindingSettings)
    {
        var res = Application.Current.Resources;

        var rlt = new StackPanel() { Padding = new(4) };
        var val = currentValue is not null && bool.Parse(currentValue);
        var @switch = new ToggleSwitch()
        {
            IsOn = val,
            Height = 32
        };

        var properties = new StackPanel()
        {
            Spacing = 8,
            Orientation = Orientation.Horizontal,
            Children =
            {
                new TextBlock()
                {
                    Style = res["BodyStrongTextBlockStyle"] as Style,
                    Foreground = res["TextFillColorPrimaryBrush"] as Brush,
                    Text = key,
                    VerticalAlignment = VerticalAlignment.Center,
                },
                @switch
            }
        };

        @switch.Toggled += (sender, e) =>
        {
            lock (bindingSettings)
                bindingSettings[key] = ((ToggleSwitch)sender).IsOn.ToString().ToLower();
        };

        var builder = new StringBuilder();
        foreach (var item in notes)
            builder.AppendLine(item);

        rlt.Children.Add(properties);
        rlt.Children.Add(new TextBlock() { Text = builder.ToString() });

        return rlt;
    }

    private static UIElement EnumTypeView(
        string key,
        string? currentValue,
        string[] notes,
        IDictionary<string, string> bindingSettings,
        string[] enums)
    {
        var res = Application.Current.Resources;
        var rlt = new StackPanel() { Padding = new(4) };
        var flyout = new MenuFlyout();
        var button = new DropDownButton()
        {
            Flyout = flyout,
            Content = new TextBlock { Text = currentValue },
            Height = 32
        };

        foreach (var @enum in enums)
        {
            var item = new MenuFlyoutItem() { Text = @enum };
            item.Click += (sender, e) =>
            {
                var item = sender as MenuFlyoutItem;

                lock (bindingSettings)
                    bindingSettings[key] = item!.Text;

                ((TextBlock)button.Content).Text = item!.Text;
            };
            flyout.Items.Add(item);
        }

        var properties = new StackPanel()
        {
            Spacing = 8,
            Orientation = Orientation.Horizontal,
            Children =
            {
                new TextBlock()
                {
                    Style = res["BodyStrongTextBlockStyle"] as Style,
                    Foreground = res["TextFillColorPrimaryBrush"] as Brush,
                    Text = key,
                    VerticalAlignment = VerticalAlignment.Center,
                },
                button
            }
        };

        var builder = new StringBuilder();
        foreach (var item in notes)
            builder.AppendLine(item);

        rlt.Children.Add(properties);
        rlt.Children.Add(new TextBlock() { Text = builder.ToString() });

        return rlt;
    }

    private static UIElement StringTypeView(
        string key,
        string? currentValue,
        string[] notes,
        IDictionary<string, string> bindingSettings)
    {
        var res = Application.Current.Resources;

        var rlt = new StackPanel() { Padding = new(4) };
        var brush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        var input = new TextBox()
        {
            Text = currentValue,
            Background = brush,
            BorderBrush = brush,
            Height = 32
        };

        var properties = new StackPanel()
        {
            Spacing = 8,
            Orientation = Orientation.Horizontal,
            Children =
            {
                new TextBlock()
                {
                    Style = res["BodyStrongTextBlockStyle"] as Style,
                    Foreground = res["TextFillColorPrimaryBrush"] as Brush,
                    Text = key,
                    VerticalAlignment = VerticalAlignment.Center,
                },
                input
            }
        };

        input.TextChanged += (sender, e) =>
        {
            lock (bindingSettings)
                bindingSettings[key] = ((TextBox)sender).Text;
        };

        var builder = new StringBuilder();
        foreach (var item in notes)
            builder.AppendLine(item);

        rlt.Children.Add(properties);
        rlt.Children.Add(new TextBlock() { Text = builder.ToString() });

        return rlt;
    }

    private async void SaveButton_Click(object sender, RoutedEventArgs e)
        => await SaveAsync();



    private bool ctrlPressed = false;

    private async void Page_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key is VirtualKey.Control)
        {
            ctrlPressed = true;
            return;
        }

        if (ctrlPressed && e.Key is VirtualKey.S)
            await SaveAsync();
    }

    private void Page_KeyUp(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key is VirtualKey.Control)
            ctrlPressed = false;
    }
}
