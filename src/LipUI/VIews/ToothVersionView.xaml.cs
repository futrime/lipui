using LipUI.Protocol;
using Microsoft.UI.Xaml.Controls;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LipUI.VIews;

public sealed partial class ToothVersionView : UserControl
{
    public ToothVersionView(in LipTooth.LipToothData.LipToothVersion version)
    {
        this.InitializeComponent();

        Version.Text = version.Version;
        ReleaseTime.Text
            = DateTimeOffset
            .FromUnixTimeSeconds(version.ReleaseTime)
            .LocalDateTime
            .ToString();
    }
}
