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
    public string UninstallTip1 { get; set; } = "请在";
    public string UninstallTip2 { get; set; } = "页面点击详细信息，然后点击删除";
    public string UninstallLocalLink { get; set; } = "本地包";
    public string UninstallConfirm { get; set; } = "确认删除 {0}";
    public string InstallInvoking { get; set; } = "安装中...";
    public string InstallTerminate { get; set; } = "终止安装（危险）";
    public string InstallInputToothAddress { get; set; } = "输入Tooth包的地址...";
    public string RegistryItemVersionButton { get; set; } = "版本：";
    public string RegistryItemInstallButton { get; set; } = "安装 {0}";
    public string RegistryFeaturedFilter { get; set; } = "精华内容";
    public string RegistrySearchPlaceholder { get; set; } = "查找...";
    public string AppConfigWorkingDirectoryUnnamed { get; set; } = "未命名";
    public string DeveloperEnterTip { get; set; } = "请到设置页面连续点击5次版本号开启开发者模式";
    public string RegistryTipFetchAll { get; set; } = "正在获取包列表";
    public string RegistryTipFetchFailed { get; set; } = "获取失败";
    public string RegistryTipRefreshFailed { get; set; } = "刷新出错";
    public string RegistrySearchTagFailed { get; set; } = "查询Tag失败";
    public string DeveloperSnackbarTitle { get; set; } = "开发者模式";
    public string DeveloperTitle { get; set; } = "开发者模式";
    public string DeveloperSnackbarSubtitle { get; set; } = "再点 {0} 次进入开发者模式。";
    public string DeveloperDialog { get; set; } = "是否进入开发者模式？\n开发者模式下部分页面会有一些额外的选项，请谨慎使用。";
    public string DeveloperDialogConfirm { get; set; } = "确认";
    public string DeveloperDialogCancel { get; set; } = "取消";
    public string DeveloperAlreadyIn { get; set; } = "您已经在开发者模式了。";
    public string DeveloperEnterSuccess { get; set; } = "已进入开发者模式。";
    public string InstallYNDialog { get; set; } = "提示";
    public string InstallYNDialogGrant { get; set; } = "好的";
    public string InstallYNDialogDeny { get; set; } = "取消";
    public string InstallYNCanceledTitle { get; set; } = "取消";
    public string InstallYNCanceled { get; set; } = "安装已取消";
    public string InstallSuccessTitle { get; set; } = "安装完成";
    public string InstallSuccess { get; set; } = "Successfully installed all tooth files.";
    public string InstallGeneratingBDSLib { get; set; } = "生成BDS...";
    public string InstallErrorTitle { get; set; } = "小错误";
    public string InstallFetchFailed { get; set; } = "获取失败";
    public string SettingsSuccessTitle { get; set; } = "设置成功";
    public string SettingsFailedTitle { get; set; } = "路径不存在";
    public string DeveloperDialogExit { get; set; } = "是否退出开发者模式？";
    public string DeveloperDialogExited { get; set; } = "已退出开发者模式";
    public string CopyToClipboard { get; set; } = "已复制到剪切板";
    public string WorkingPathSelectorTitle { get; set; } = "选择工作目录";
    public string WorkingPathSelectorAdded { get; set; } = "添加成功";
    public string WorkingPathSelectorAlreadyAdded { get; set; } = "目录已添加";
    public string WorkingPathSelectorNotExist { get; set; } = "目录不存在";
    public string LocalFetchRetry { get; set; } = "小错误，请尝试重新获取";
    public string LocalFetchFailed { get; set; } = "获取失败";
    public string WorkingPathSelectorAddButton { get; set; } = "添加";
    public string WorkingPathSelectorSelectButton { get; set; } = "选择";
    public string WorkingPathSelectorCurrent { get; set; } = "当前目录";
    public string WorkingPathSelectorItemCopy { get; set; } = "复制";
    public string WorkingPathSelectorItemOpen { get; set; } = "打开文件夹";
    public string WorkingPathSelectorItemDelete { get; set; } = "删除当前项";
    public string HomeWorkingDirectoryEditToggle { get; set; } = "开启编辑";
    public string Eula { get; set; } = "条款";
    public string EulaDeny { get; set; } = "不同意";
    public string EulaAccept { get; set; } = "同意";
    public string EulaDeniedTitle { get; set; } = "您拒绝了条款";
    public string EulaDeniedContent { get; set; } = "程序即将退出";
    public string LipInstallerDialog { get; set; } = "需要配置 lip.exe";
    public string LipInstallerDialogComplete { get; set; } = "完成";
    public string LipInstallerSnackbarLipNotFound { get; set; } = "未找到lip.exe";
    public string LipInstallerSnackbarLipNotFoundTip { get; set; } = "如已安装请重新启动LipUI";
    public string LipInstallerDialogDownload { get; set; } = "下载";
    public string LipInstallerLipDownloading { get; set; } = "正在下载，请等待...";
    public string LipInstallerLipDownloadingProgress { get; set; } = "下载中...{0}/{1}";
    public string LipInstallerLipDownloadedSuccess { get; set; } = "下载完成.";
    public string LipInstallerDownloadFailed { get; set; } = "失败：{0}";
    public string LipInstallerLipNotFound { get; set; } = "压缩包中未找到 lip.exe";
    public string LipInstallerInvalidOperation { get; set; } = "无效操作";
    public string LipInstallerInvalidOperationTip { get; set; } = "请选中安装到'全局'或者'当前目录'以下载 lip.exe";
    public string WorkingPathSelectorInitDialog { get; set; } = "需要指定有效的工作路径";
    public string WorkingPathSelectorInitDialogComplete { get; set; } = "完成";
    public string WorkingPathSelectorInitErr { get; set; } = "请选择有效的工作路径";
    public string SettingsClearCache { get; set; } = "清除Lip的缓存";
    public string SettingsClearCacheTitle { get; set; } = "清除Lip的缓存";
    public string SettingsClearCacheContent { get; set; } = "确认清除缓存";
    public string SettingsClearCacheConfirm { get; set; } = "确定";
    public string SettingsClearCacheCancel { get; set; } = "取消";
    public string SettingsClearCacheCompleted { get; set; } = "清除缓存执行完成";
}