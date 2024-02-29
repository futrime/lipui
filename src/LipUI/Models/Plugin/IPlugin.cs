namespace LipUI.Models.Plugin;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class LipUIModuleAttribute : Attribute
{
}

public interface IPlugin
{
    public string PluginName { get; }

    public bool DefaultEnabled { get; }

    public Guid Guid { get; }

    public void OnInitlalize(LipuiServices services) { }

    public void OnEnable(LipuiServices services) { }

    public void OnDisable(LipuiServices services) { }

    public void OnApplicationExit() { }
}
