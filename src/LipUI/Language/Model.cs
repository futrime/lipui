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
    public string NavigationWebUi { get; set; } = "WebUI";
    public string NavigationDeveloper { get; set; } = "开发者工具";
    public string NavigationSettings { get; set; } = "设置 & 关于";
    public string LocalBottomInstall { get; set; } = "已安装 {0} 个Tooth包 ";
    public string LocalBottomReload { get; set; } = "重新加载";
    public string RegistryBottomInstall { get; set; } = "获取到 {0} / {1} 个Tooth包 ";
    public string UninstallBackToLocal { get; set; } = "返回本地包";
    public string RegistryBottomReload { get; set; } = "重新加载";
    public string LocalButtonUpgrade { get; set; } = "升级";
    public string LocalButtonUninstall { get; set; } = "卸载";
    public string ToothItemDetailButton { get; set; } = "详细信息";
    public string ToothItemAuthor { get; set; } = "作者：";
    public string ToothItemLicense { get; set; } = "协议：";
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
    public string LipInstallerConfigMode { get; set; } = "配置模式：";
    public string LipInstallerAssumedPath { get; set; } = "当前拟定位置：";
    public string LipInstallerGlobalExe { get; set; } = "下载并安装到全局";
    public string LipInstallerPortableExe { get; set; } = "下载并放置到当前目录";
    public string LipInstallerManualExe { get; set; } = "手动配置路径";
    public string RegistryTagFeatured { get; set; } = "精华";
    public string RegistryTagIntegration { get; set; } = "整合";
    public string RegistryTagModule { get; set; } = "模块";
    public string RegistryTagPlugin { get; set; } = "插件";
    public string RegistryTagAntiCheat { get; set; } = "反作弊";
    public string InstallSkipDependency { get; set; } = "跳过依赖";
    public string InstallButton { get; set; } = "安装 {0}";
    public string InstallFetchButton { get; set; } = "获取";
    public string InstallFetchCancelButton { get; set; } = "取消";
    public string ToothInfoInstalled { get; set; } = "[已安装]";
    public string ToothInfoPackName { get; set; } = "包名：";
    public string ToothInfoPackVersion { get; set; } = "版本：";
    public string ToothInfoName { get; set; } = "名称：";
    public string ToothInfoAuthor { get; set; } = "作者：";
    public string ToothInfoLicense { get; set; } = "协议：";
    public string ToothInfoDescription { get; set; } = "描述：";
    public string ToothInfoHomePage { get; set; } = "主页：";
    public string ToothInfoHomePageOpenLink { get; set; } = "打开";
    //public string ToothInfoPackT { get; set; } = "版本：";
    //public string ToothInfoPackT { get; set; } = "版本：";
}