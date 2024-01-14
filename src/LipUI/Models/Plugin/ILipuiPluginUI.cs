using Microsoft.UI.Xaml.Controls;
using System;

namespace LipUI.Models.Plugin;

public interface ILipuiPluginUI : ILipuiPlugin
{
    public IconElement NavigatonBarIcon { get; }

    public object NavigationBarContent { get; }

    public Type PageType { get; }

    public object? PageParameterRequsted() { return null; }
}
