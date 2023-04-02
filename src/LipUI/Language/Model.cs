using CommunityToolkit.Mvvm.ComponentModel;
namespace LipUI.Language;
/// <summary>
/// 多语言类
/// 添加规则：
///     1.必须添加[ObservableProperty]注解，以便自动生成属性
///     2.必须string类型，以便前端绑定
///     3.下划线开头，自动生成属性会去掉下划线并让首个字母大写
/// 前端绑定方法：
///      对于 [ObservableProperty] string _textA = "A";
///     则有 Text="{Binding TextA,Source={StaticResource I18N}}"
/// 后端获取方法：
///     string v = Global.I18N.TextA;
/// </summary>
internal partial class Model : ObservableObject
{
    [ObservableProperty] private string _HomeStartTip = "Click the left navigation bar to get started!";
    [ObservableProperty] private string _HomeWorkingDirectoryTitle = "Working directory:";
    [ObservableProperty] private string _LipIntallerGlobalExe = "[Global] Download and install globally";
    [ObservableProperty] private string _LipIntallerPortableExe = "[Portable] Download and place in current directory";
    [ObservableProperty] private string _LipIntallerManualExe = "[Manually] Configure manually";
    //[ObservableProperty] string _HomeStartTip = "点击左边的导航栏开始吧！";
    //[ObservableProperty] string _HomeWorkingDirectoryTitle = "工作目录：";
    //[ObservableProperty] string _LipIntallerGlobalExe = "下载并安装到全局";
    //[ObservableProperty] string _LipIntallerPortableExe = "下载并放置到当前目录";
    //[ObservableProperty] string _LipIntallerManualExe = "手动配置路径";
} 
