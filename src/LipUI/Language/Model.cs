using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using o = CommunityToolkit.Mvvm.ComponentModel.ObservablePropertyAttribute;
namespace LipUI.Language
{
    /// <summary>
    /// 多语言类
    /// 添加规则：
    ///     1.必须添加[o]注解，以便自动生成属性
    ///     2.必须string类型，以便前端绑定
    ///     3.下划线开头，自动生成属性会去掉下划线并让首个字母大写
    /// 前端绑定方法：
    ///      对于 [o] string _textA = "A";
    ///     则有 Text="{Binding TextA,Source={StaticResource I18N}}"
    /// 后端获取方法：
    ///     string v = Global.I18N.TextA;
    /// </summary>
    internal partial class Model : ObservableObject
    {
        [o] string _homeStartTip = "点击左边的导航栏开始吧！";
    }
}
