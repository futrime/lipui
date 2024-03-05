using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using PluginKey = System.String;

namespace LipUI.Models.Plugin;

internal record struct PluginConfigFileInfo(string AssemblyName, string Name, Guid Guid);
internal record struct PluginKeyInfo(string AssemblyName, Guid Guid);

internal static partial class PluginConfigManager
{
    private static readonly Dictionary<PluginKey, (PluginConfigFileInfo Info, ConfigCollection Config)> pluginConfigs = [];

    [GeneratedRegex(@"(?<assembly_name>.+?)\+(?<name>.+)\+(?<guid>[a-z0-9]{8}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{12})$")]
    private static partial Regex PluginConfigFileNameRegex();

    [GeneratedRegex(@"(?<assembly_name>.+?)\+(?<guid>[a-z0-9]{8}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{12})$")]
    private static partial Regex PluginKeyRegex();

    private static string GetPluginKey(string assemblyName, in Guid guid) => $"{assemblyName}+{guid}";

    private static bool TryParseFileInfo(string str, [NotNullWhen(true)] out PluginConfigFileInfo info)
    {
        info = default;

        var match = PluginConfigFileNameRegex().Match(str);
        if (match.Success is false)
            return false;

        info = new(match.Groups["assembly_name"].Value, match.Groups["name"].Value, Guid.Parse(match.Groups["guid"].Value));
        return true;
    }

    private static bool TryParseKeyInfo(string str, [NotNullWhen(true)] out PluginKeyInfo info)
    {
        info = default;

        var match = PluginKeyRegex().Match(str);
        if (match.Success is false)
            return false;

        info = new(match.Groups["assembly_name"].Value, Guid.Parse(match.Groups["guid"].Value));
        return true;
    }

    public static void Load()
    {
        lock (pluginConfigs)
        {
            var dir = new DirectoryInfo(Path.Combine(Main.WorkingDirectory, DefaultSettings.ConfigsDirectory));
            if (dir.Exists is false)
                dir.Create();

            foreach (var file in dir.EnumerateFiles())
            {
                var key = Path.GetFileNameWithoutExtension(file.Name);

                if (TryParseFileInfo(key, out var info) is false)
                    continue;

                pluginConfigs.Add($"{info.AssemblyName}+{info.Guid}", (info, ConfigCollection.Deserialize(File.ReadAllText(file.FullName))));
            }
        }
    }

    public static void Save(in PluginConfigFileInfo info, ConfigCollection config)
    {
        lock (config)
        {
            if (config.Changed is false)
                return;

            var path = Path.Combine(Main.WorkingDirectory, DefaultSettings.ConfigsDirectory, $"{info.AssemblyName}+{info.Name}+{info.Guid}.json");
            if (File.Exists(path))
                File.Delete(path);
            File.WriteAllText(path, config.Serialize());

            config.Changed = false;
        }
    }

    public static void Save()
    {
        lock (pluginConfigs)
        {
            foreach (var (_, (info, config)) in pluginConfigs)
                Save(info, config);
        }
    }

    public static ConfigCollection? InitPluginConfig(PluginKey key, string pluginDisplayName)
    {
        if (TryParseKeyInfo(key, out var info) is false)
            return null;

        lock (pluginConfigs)
        {
            if (pluginConfigs.ContainsKey(key))
                return null;

            var ret = new ConfigCollection();

            pluginConfigs.Add(key, (new(info.AssemblyName, pluginDisplayName, info.Guid), ret));

            return ret;
        }
    }

    public static ConfigCollection? InitPluginConfig(IPlugin plugin)
        => InitPluginConfig(PluginSystem.GetPluginKey(plugin), plugin.PluginName);

    public static bool TryGetPluginConfig(PluginKey key, [NotNullWhen(true)] out ConfigCollection? config)
    {
        config = null;

        if (TryParseKeyInfo(key, out var _) && pluginConfigs.TryGetValue(key, out var temp) is true)
        {
            config = temp.Config;
            return true;
        }

        return false;
    }

    public static bool TryGetPluginConfig(IPlugin plugin, [NotNullWhen(true)] out ConfigCollection? config)
        => TryGetPluginConfig(PluginSystem.GetPluginKey(plugin), out config);

    public static ConfigCollection GetOrCreatePluginConfig(PluginKey key, string pluginDisplayName)
    {
        if (TryGetPluginConfig(key, out var config))
            return config;

        return InitPluginConfig(key, pluginDisplayName) ?? throw new NullReferenceException();
    }

    public static ConfigCollection GetOrCreatePluginConfig(IPlugin plugin)
        => GetOrCreatePluginConfig(PluginSystem.GetPluginKey(plugin), plugin.PluginName);

    public static bool Contains(IPlugin plugin) => pluginConfigs.ContainsKey(PluginSystem.GetPluginKey(plugin));
}
