using LipUI.Models.Plugin;

namespace LipUI.BuiltInPlugins.LipuiTips;

//[LipUIModule]
internal class LipuiTips : IPlugin
{
    public string PluginName => "Tips";

    public bool DefaultEnabled => true;

    public Guid Guid => new("9116AABC-6448-9933-DE77-D477EB10B857");

    void IPlugin.OnEnable(LipuiServices services)
    {
    }
}