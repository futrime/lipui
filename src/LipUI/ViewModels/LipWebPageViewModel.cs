using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LipWebApi;
using Wpf.Ui.Common.Interfaces;
using System.Collections.ObjectModel;
using System.Linq;

namespace LipUI.ViewModels;
public partial class LipWebPageViewModel : ObservableRecipient, INavigationAware
{
    private LipWebApi.Launcher _launcher = new(wd =>
    {
        var lip = Global.Lip.Clone();
        lip.WorkingPath = wd;
        return lip;
    }, $"LipUI - {Global.GetAssemblyVersion()}");
    [ObservableProperty] private string _host = "localhost";
    //partial void OnHostChanged(string value)
    //{
    //}
    [ObservableProperty] private ushort _port = 0;
    //partial void OnPortChanged(string value)
    //{
    //}
    public async Task ApplyHostPort()
    {
        await _launcher.Load(v =>
        {
            foreach (var x in v.ToString().Replace("\r", "").Split('\n'))
            {
                OutPut.Add(x);
            }
#if DEBUG
            Debug.WriteLine(v);
#endif
        });
    }
    public record AuthDataItem(string Username, string PasswordMd5);
    public IEnumerable<AuthDataItem> AuthData => Launcher.Auth.Select(x =>
        new AuthDataItem(x.Key, x.Value.Remove(8) + new string('*', x.Value.Length - 8)));
    public Models.AppConfig GlobalConfig => Global.Config;
    [ObservableProperty] private string _username = "";
    [ObservableProperty] private string _password = "";
    [ObservableProperty] private bool _isRunning = false;
    async Task RefreshServerStatus()
    {
        if (Global.Config.WebUIEnabled)
        {
            if (IsRunning)
            {
                await _launcher.Stop();
            }
            Launcher.LoadAuth();
            Launcher.LoadConfig();
            Launcher.LoadUsersData();
            _launcher.UpdateWorkingDir(from x in GlobalConfig.AllWorkingDirectory
                                       select new LipWebApi.Models.UsersData.DirectoryInfo
                                       {
                                           Directory = x.Directory,
                                           Name = x.Name
                                       }); 
            Launcher.Config.Host = Host;
            Launcher.Config.Port = Port;
            await _launcher.Load(v =>
                {
                    foreach (var x in v.ToString().Replace("\r", "").Split('\n'))
                    {
                        Global.DispatcherInvoke(() =>
                        {
                            OutPut.Add(x);
                        });
                    }
#if DEBUG
                    Debug.WriteLine(v);
#endif
                });
            IsRunning = true;
            Global.PopupSnackbar("WebUI", "已开启");
        }
        else
        {
            await _launcher.Stop();
            IsRunning = false;
            Global.PopupSnackbar("WebUI", "已关闭");
        }
    }
    [ObservableProperty]
    private ObservableCollection<string> _outPut = new();
    [RelayCommand]
    void AddUser()
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            Global.PopupSnackbar("参数错误", "用户名或密码不能为空");
            return;
        }

        RemoveUser(Username);
        if (Launcher.AddUser(Username, Password))
        {
            OnPropertyChanged(nameof(AuthData));
        }
    }
    [RelayCommand]
    void RemoveUser(string? user)
    {
        if (Launcher.DeleteUser(user))
        {
            OnPropertyChanged(nameof(AuthData));
        }
    }
    [RelayCommand]
    void ClearOutPut() => OutPut.Clear();
    [RelayCommand]
    async Task Toggle()
    {
        await RefreshServerStatus();
        OnPropertyChanged(nameof(AuthData));
    }/// <summary>
     /// 进入
     /// </summary>
    public void OnNavigatedTo()
    {
        if (Port == 0)
        {
            Host = Launcher.Config.Host;
            Port = Launcher.Config.Port;
        }
        _ = RefreshServerStatus();
    }
    /// <summary>
    /// 离开
    /// </summary>
    public void OnNavigatedFrom()
    {
        _launcher.Stop();
        IsRunning = false;
    }
}
