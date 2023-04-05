using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LipWebApi;
using Wpf.Ui.Common.Interfaces;
using System.Collections.ObjectModel;

namespace LipUI.ViewModels;
public partial class LipWebPageViewModel : ObservableRecipient, INavigationAware
{
    private LipWebApi.Launcher _launcher = new();
    [ObservableProperty] private string _host = "localhost";
    //partial void OnHostChanged(string value)
    //{
    //}
    [ObservableProperty] private short _port = 9000;
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
    public partial class AuthDataModel : ObservableObject
    {
        [ObservableProperty] private string _username;
        [ObservableProperty] private string _password;
        [ObservableProperty] private bool _isMd5 = false;
    }
    [ObservableProperty]
    ObservableCollection<AuthDataModel> _authData = new();
    public Models.AppConfig GlobalConfig => Global.Config;
    public void OnNavigatedTo()
    {
        if (Global.Config.WebUIEnabled)
        {
            _launcher.LoadAuth();
            _launcher.LoadConfig();
            Launcher.Config.Host = Host;
            Launcher.Config.Port = Port;
            _launcher.Load(v =>
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
    }
    [ObservableProperty]
    private ObservableCollection<string> _outPut = new();
    [RelayCommand]
    void AddUser(string userName)
    {

    }
    public void OnNavigatedFrom()
    {
    }
}
