using LipUI.Models;
using LipUI.VIews;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LipUI.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class SelectServerPage : Page
{
    private ToggleButton DeleteButton;
    private Button ApplyDeleteButton;

    private Action? refreshIcon;

    [Flags] private enum Mode { Select, Edit }

    private Mode ItemClickMode { get; set; }

    private readonly HashSet<ServerInstanceView> selectedViews = new();

    public SelectServerPage()
    {
        InitializeComponent();
        InitButtons();
    }

    [MemberNotNull(nameof(DeleteButton), nameof(ApplyDeleteButton))]
    private void InitButtons()
    {
        DeleteButton = new()
        {
            Height = 48,
            Width = 48,
            Background = new SolidColorBrush(Colors.Transparent),
            BorderBrush = new SolidColorBrush(Colors.Transparent),
            Content = new SymbolIcon()
            {
                Symbol = Symbol.Delete,
                Height = 24,
                Width = 24
            },
        };
        DeleteButton.Click += DeleteButton_Click;

        ApplyDeleteButton = new()
        {
            Height = 48,
            Width = 48,
            Background = new SolidColorBrush(Colors.Transparent),
            BorderBrush = new SolidColorBrush(Colors.Transparent),
            Shadow = null,
            Content = new SymbolIcon()
            {
                Symbol = Symbol.Accept,
                Height = 24,
                Width = 24
            }
        };
        ApplyDeleteButton.Click += DeleteApplyButton_Click;
    }


    private void ServerScrollViewer_Loaded(object sender, RoutedEventArgs e)
    {
        var progressRing = new ProgressRing()
        {
            IsIndeterminate = false,
            Minimum = 0,
            Maximum = Main.Config.ServerInstances.Count,
            IsActive = true
        };
        ServerScrollViewer.Content = progressRing;

        DispatcherQueue.TryEnqueue(async () =>
        {
            await Task.Yield();

            for (int i = 0; i < Main.Config.ServerInstances.Count; i++)
            {
                ServerGridView.Items.Add(new ServerInstanceView(Main.Config.ServerInstances[i]));
                progressRing.Value = i;
            }

            ServerScrollViewer.Content = ServerGridView;
        });
    }


    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        refreshIcon = (Action)e.Parameter;
        base.OnNavigatedTo(e);
    }



    private void SwitchMode(Mode clickMode)
    {
        lock (this)
        {
            switch (clickMode)
            {
                case Mode.Select:
                    if (ItemClickMode is Mode.Edit)
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            ButtonsPanel.Children.Remove(ApplyDeleteButton);
                            DeleteButton.IsChecked = false;
                            ButtonsPanel.Children.Remove(DeleteButton);
                        });
                        foreach (var view in selectedViews)
                        {
                            view.UnselectStoryboard.Begin();
                        }
                        selectedViews.Clear();
                    }
                    break;
                case Mode.Edit:
                    {
                        if (ItemClickMode is Mode.Select)
                        {
                            DispatcherQueue.TryEnqueue(() =>
                            {
                                ButtonsPanel.Children.Add(DeleteButton);
                            });
                        }
                    }
                    break;
            }
            ItemClickMode = clickMode;
        }
    }

    private async void ServerGridView_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is Border border && border.Tag as string is "Add")
        {
            await ItemClick_AddServerViewClicked(sender, e);
        }
        else
        {
            switch (ItemClickMode)
            {
                case Mode.Select: ItemClick_SelectMode(sender, e); return;
                case Mode.Edit: ItemClick_EditMode(sender, e); return;
            }
        }
    }

    private async ValueTask ItemClick_AddServerViewClicked(object sender, ItemClickEventArgs e)
    {
        if (ItemClickMode is not Mode.Select)
        {
            AddServerCardViewStoryboard.Begin();
            return;
        }

        var picker = new FolderPicker()
        {
            SuggestedStartLocation = PickerLocationId.Desktop,
            FileTypeFilter = { "*" }
        };
        InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle((Application.Current as App)!.m_window));

        var folder = await picker.PickSingleFolderAsync();
        if (folder is not null)
        {
            var server = new ServerInstance()
            {
                WorkingDirectory = folder.Path
            };

            foreach (var instance in Main.Config.ServerInstances)
            {
                if (instance == server) return;
            }

            Main.Config.ServerInstances.Add(server);

            ServerGridView.DispatcherQueue.TryEnqueue(async () =>
            {
                ServerGridView.Items.Add(new ServerInstanceView(server));
                await Main.SaveConfigAsync();
            });
        }
    }

    private async void ItemClick_SelectMode(object sender, ItemClickEventArgs e)
    {
        Main.Config.SelectedServer = (e.ClickedItem as ServerInstanceView)!.ServerInstance;
        refreshIcon!();
        await Main.SaveConfigAsync();
        Frame.GoBack();
    }

    private void ItemClick_EditMode(object sender, ItemClickEventArgs e)
    {
        lock (selectedViews)
        {
            var view = (ServerInstanceView)e.ClickedItem;
            if (DeleteButton.IsChecked!.Value)
            {
                if (selectedViews.Contains(view))
                {
                    selectedViews.Remove(view);
                    view.UnselectStoryboard.Begin();
                }
                else if (selectedViews.Add(view))
                {
                    view.SelectStoryboard.Begin();
                }
            }
            else
            {
                var editView = new ServerInstanceEditView(view.ServerInstance);
                var dialog = new ContentDialog()
                {
                    XamlRoot = XamlRoot,
                    Content = editView,
                    CloseButtonText = "server$editor$cancel".GetLocalized(),
                    PrimaryButtonText = "server$editor$confirm".GetLocalized()
                };
                DispatcherQueue.TryEnqueue(async () =>
                {
                    var operation = await dialog.ShowAsync();
                    switch (operation)
                    {
                        case ContentDialogResult.Primary:

                            editView.CommitServerProperies();

                            if (editView.CustomIcon is not null)
                                view.SetIconImageSource(editView.CustomIcon);

                            view.RefreshUI();
                            await Main.SaveConfigAsync();

                            return;
                        case ContentDialogResult.None:
                        case ContentDialogResult.Secondary:
                            return;
                    }
                });
            }
        }
    }

    private void EditButton_Click(object sender, RoutedEventArgs e)
        => SwitchMode(EditButton.IsChecked ?? false ? Mode.Edit : Mode.Select);

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (DeleteButton.IsChecked!.Value)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                ButtonsPanel.Children.Add(ApplyDeleteButton);
            });
        }
        else
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                ButtonsPanel.Children.Remove(ApplyDeleteButton);
            });
        }
    }

    private async void DeleteApplyButton_Click(object sender, RoutedEventArgs e)
    {
        lock (Main.Config)
        {
            foreach (var view in selectedViews)
            {
                ServerGridView.Items.Remove(view);
                Main.Config.ServerInstances.Remove(view.ServerInstance);
                if (Main.Config.SelectedServer == view.ServerInstance)
                    Main.Config.SelectedServer = null;
            }

            refreshIcon!();
        }
        await Main.SaveConfigAsync();
    }
}
