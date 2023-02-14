using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LipUI.Models;
using Wpf.Ui.Common.Interfaces;

namespace LipUI.ViewModels;
public partial class DashboardViewModel : ObservableObject, INavigationAware
{
    public void OnNavigatedTo()
    {
    }
    public void OnNavigatedFrom() { }
}