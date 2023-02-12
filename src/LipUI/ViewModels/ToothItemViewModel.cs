using CommunityToolkit.Mvvm.ComponentModel;
using System;
namespace LipUI.ViewModels;
public partial class ToothItemViewModel : ObservableObject
{
    [ObservableProperty]
    private string _tooth = string.Empty;
    [ObservableProperty]
    private string _version = string.Empty;
}