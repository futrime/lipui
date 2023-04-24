using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace LipUI.ViewModels;
public partial class SourceSelectorViewModel : ObservableObject
{//todo 换源自定义的页面
    [RelayCommand]
    void Copy(string v)
    {
        Clipboard.SetText(v);
        Global.PopupSnackbar(Global.I18N.CopyToClipboard, v);
    }
    [RelayCommand]
    void Select(string v)
    {
        //Config.WorkingDirectory = v;
        //Global.CheckWorkDir();
    }
    [RelayCommand]
    void Delete(string dir)
    {
        //Config.AllWorkingDirectory.Remove(dir);
        //Global.CheckWorkDir();
    }
}