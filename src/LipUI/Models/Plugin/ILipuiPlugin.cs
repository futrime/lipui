namespace LipUI.Models.Plugin;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class LipUIModuleAttribute : Attribute
{
}

public interface ILipuiPlugin
{
    public string PluginName { get; }

    public bool DefaultEnabled { get; }

    public Guid Guid { get; }

    public void OnInitlalize(LipuiRuntimeInfo info) { }

    public void OnEnable(LipuiServices services) { }

    public void OnDisable(LipuiServices services) { }

    public void OnApplicationExit() { }
}
