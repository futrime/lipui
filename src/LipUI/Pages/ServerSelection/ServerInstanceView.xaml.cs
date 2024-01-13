using LipUI.Models;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LipUI.Pages.ServerSelection;

internal sealed partial class ServerInstanceView : UserControl
{
    public ServerInstance ServerInstance { get; private set; }

    public void SetIconImageSource(ImageSource source)
        => Image.Source = source;

    public ServerInstanceView(ServerInstance instance)
    {
        InitializeComponent();
        ServerInstance = instance;

        RefreshUI();

        DispatcherQueue.TryEnqueue(async () =>
        {
            Image.Source = await ServerIcon.GetIcon(instance);
        });
    }

    public void RefreshUI()
    {
        Text.Text = $"{ServerInstance.Name}{Environment.NewLine}{ServerInstance.Description}{Environment.NewLine}{ServerInstance.Version}";
    }

    public Storyboard SelectStoryboard => _selectStoryboard;
    public Storyboard UnselectStoryboard => _unselectStoryboard;
}
