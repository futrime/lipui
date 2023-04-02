using CommunityToolkit.Mvvm.ComponentModel;
namespace LipUI.Language;
/// <summary>
/// 多语言类
/// 添加规则：
///     1.必须添加[ObservableProperty]注解，以便自动生成属性
///     2.必须string类型，以便前端绑定
///     3.下划线开头，自动生成属性会去掉下划线并让首个字母大写
/// 前端绑定方法：
///      对于 string textA {get;set;} = "A";
///     则有 Text="{Binding TextA,Source={StaticResource I18N}}"
/// 后端获取方法：
///     string v = Global.I18N.TextA;
/// </summary>
public class Model : ObservableObject
{
    public new void OnPropertyChanged(string key) => base.OnPropertyChanged(key);
    public string ApplicationTitle { get; set; } = "LipUI - Tooth包管理器";
    public string NavigationHome { get; set; } = "主页";
    public string NavigationLocal { get; set; } = "本地Tooth包";
    public string NavigationInstall { get; set; } = "安装Tooth包";
    public string NavigationRemove { get; set; } = "卸载Tooth包";
    public string NavigationRegistry { get; set; } = "包市场";
    public string NavigationWebUI { get; set; } = "WebUI";
    public string NavigationDeveloper { get; set; } = "开发者工具";
    public string NavigationSettings { get; set; } = "设置 & 关于";
    public string SettingsMainTitle { get; set; } = "主设置";
    public string SettingsTheme { get; set; } = "主题";
    public string SettingsLipPath { get; set; } = "Lip路径：";
    public string SettingsLanguage { get; set; } = "语言";
    public string SettingsLipPathAuto { get; set; } = "自动获取";
    public string SettingsDeveloperTitle { get; set; } = "开发者选项";
    public string SettingsDeveloperConfigPath { get; set; } = "配置文件位置：";
    public string SettingsDeveloperConfigOpen { get; set; } = "打开";
    public string SettingsDeveloperSkipDependency { get; set; } = "安装界面显示\"跳过依赖\"选项";
    public string SettingsDeveloperExit { get; set; } = "退出开发者模式";
    public string AboutTitle { get; set; } = "关于";
    public string AboutLipUIVersion { get; set; } = "LipUI版本：";
    public string AboutLipVersion { get; set; } = "Lip版本：";
    public string AboutLipUILink { get; set; } = "LipUI 的 GitHub 项目地址";
    public string AboutLipLink { get; set; } = "Lip 的 GitHub 项目地址";
    public string HomeStartTip { get; set; } = "点击左边的导航栏开始吧！";
    public string HomeWorkingDirectoryTitle { get; set; } = "工作目录：";
    public string LipInstallerGlobalExe { get; set; } = "下载并安装到全局";
    public string LipInstallerPortableExe { get; set; } = "下载并放置到当前目录";
    public string LipInstallerManualExe { get; set; } = "手动配置路径";
}
