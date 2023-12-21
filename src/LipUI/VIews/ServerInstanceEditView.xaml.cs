using LipUI.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LipUI.VIews;

internal sealed partial class ServerInstanceEditView : UserControl
{


    public ServerInstance Server { get; private set; }

    private string? iconPath = null;

    public BitmapImage? CustomIcon { get; private set; }

    public ServerInstanceEditView(ServerInstance server)
    {
        this.Server = server;
        InitializeComponent();
    }

    private async ValueTask RefreshIcon(string? version, string? iconPath)
    {
        var image = await ServerIcon.GetIcon(version, iconPath);
        IconImage.Source = image;
        CustomIcon = image;
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        NameInput.Text = Server.Name;
        DescriptionInput.Text = Server.Description;
        VersionInput.Text = Server.Version;
        WorkingDirectoryInput.Text = Server.WorkingDirectory;

        DispatcherQueue.TryEnqueue(async () => await RefreshIcon(Server.Version, Server.Icon));
    }

    private async void VersionInput_TextChanged(object sender, TextChangedEventArgs e)
        => await RefreshIcon(VersionInput.Text, iconPath);

    private async void EditIconButton_Click(object sender, RoutedEventArgs e)
    {
        var picker = new FileOpenPicker()
        {
            SuggestedStartLocation = PickerLocationId.PicturesLibrary,
            FileTypeFilter = { ".jpeg", ".jpg", ".png", ".bmp", ".tiff", ".ico" }
        };
        InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle((Application.Current as App)!.m_window));

        var file = await picker.PickSingleFileAsync();
        if (file is not null)
            iconPath = file.Path;

        await RefreshIcon(VersionInput.Text, iconPath);
    }

    public void CommitServerProperies()
    {
        Server.Name = NameInput.Text;
        Server.Description = DescriptionInput.Text;
        Server.Version = VersionInput.Text;
        Server.WorkingDirectory = WorkingDirectoryInput.Text;
        Server.Icon = iconPath;
    }
}
