using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LipUI.Models;
namespace LipUI.ViewModels;
public partial class WorkingPathSelectorViewModel : ObservableObject
{
    public AppConfig Config => Global.Config;
    [RelayCommand]
    void Copy(string v)
    {
        Clipboard.SetText(v);
    }
    [RelayCommand]
    void Select(string v)
    {
        Config.WorkingDirectory = v;
    }
    [RelayCommand]
    void Delete(string dir)
    {
        Config.AllWorkingDirectory.Remove(dir);
    }
    [ObservableProperty] string _addingWorkingDir;
    [ObservableProperty] string _tip;
    /// <summary>
    /// 添加目录
    /// </summary>
    [RelayCommand]
    void AddWorkingDir()
    {
        AddingWorkingDir = AddingWorkingDir.Trim();
        if (Directory.Exists(AddingWorkingDir))
        {
            if (Config.AllWorkingDirectory.Contains(AddingWorkingDir))
            {
                Config.WorkingDirectory = AddingWorkingDir;
                Tip = "目录已存在";
            }
            else
            {
                Config.AllWorkingDirectory.Add(AddingWorkingDir);
                Config.WorkingDirectory = AddingWorkingDir;
                AddingWorkingDir = "";
            }
        }
        else
        {
            Tip = "目录不存在";
        }
    }
}