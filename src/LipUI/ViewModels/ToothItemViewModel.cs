using CommunityToolkit.Mvvm.ComponentModel;
using System;
namespace LipUI.ViewModels;
public partial class ToothItemViewModel : ObservableObject
{
    [ObservableProperty]
    private string _packageName = string.Empty;
    [ObservableProperty]
    private string _packageDescription = string.Empty;

}