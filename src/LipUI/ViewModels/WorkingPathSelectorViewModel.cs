using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LipUI.Models;
using Ookii.Dialogs.Wpf;
using Wpf.Ui.Common;
using static LipUI.Models.AppConfig;
using Clipboard = System.Windows.Clipboard;

namespace LipUI.ViewModels;

public partial class WorkingPathSelectorViewModel : ObservableObject
{
    [ObservableProperty] public bool _noEdit = false;
    public AppConfig Config => Global.Config;
    [RelayCommand]
    void Copy(AppConfigWorkingDirectory v)
    {
        Clipboard.SetText(v.Directory);
        Global.PopupSnackbar(Global.I18N.CopyToClipboard, v.Directory);
    }
    [RelayCommand]
    internal void Select(AppConfigWorkingDirectory v)
    {
        Config.WorkingDirectory = v;
        Global.CheckWorkDir();
    }
    [RelayCommand]
    internal async Task Open(AppConfigWorkingDirectory v)
    {
        await Task.Run(() =>
        {
             Process.Start("explorer.exe", v.Directory)?.WaitForExit(1000);
        });
    }
    [RelayCommand]
    void Delete(AppConfigWorkingDirectory dir)
    {
        Config.AllWorkingDirectory.Remove(dir);
        Global.CheckWorkDir();
    }
    [ObservableProperty] string _addingWorkingDir = string.Empty;
    //[ObservableProperty] string _tip;
    /// <summary>
    /// 选择目录
    /// </summary>
    [RelayCommand]
    void SelectWorkingDir()
    {
        var dialog = new VistaFolderBrowserDialog
        {
            SelectedPath = AddingWorkingDir,
            ShowNewFolderButton = true,
            Description = Global.I18N.WorkingPathSelectorTitle
        };
        if (dialog.ShowDialog() == true)
        {
            AddingWorkingDir = dialog.SelectedPath;
            //自动添加
            AddWorkingDir();
        }
    }
    public bool ShowGetCurrentButton => !Global.Config.AllWorkingDirectory.Contains(Directory.GetCurrentDirectory());
    /// <summary>
    /// 获取当前目录
    /// </summary>
    [RelayCommand]
    void GetCurrentDir()
    {
        AddingWorkingDir = Directory.GetCurrentDirectory();
        //自动添加
        AddWorkingDir();
    }
    /// <summary>
    /// 添加目录
    /// </summary>
    [RelayCommand]
    void AddWorkingDir()
    {
        if (string.IsNullOrWhiteSpace(AddingWorkingDir)) return;
        AddingWorkingDir = AddingWorkingDir.Trim();
        if (Directory.Exists(AddingWorkingDir))
        {
            if (Config.AllWorkingDirectory.Contains(AddingWorkingDir))
            {
                Config.WorkingDirectory = AddingWorkingDir;
                Global.PopupSnackbar(Global.I18N.WorkingPathSelectorAlreadyAdded, AddingWorkingDir, SymbolRegular.Warning16, ControlAppearance.Caution);
            }
            else
            {
                Config.AllWorkingDirectory.Add(AddingWorkingDir);
                Config.WorkingDirectory = AddingWorkingDir;
                AddingWorkingDir = "";
                Global.PopupSnackbar(Global.I18N.WorkingPathSelectorAdded, AddingWorkingDir);
            }
        }
        else
        {
            Global.PopupSnackbar(Global.I18N.WorkingPathSelectorNotExist, AddingWorkingDir, SymbolRegular.Warning16, ControlAppearance.Caution);
        }
    }
}