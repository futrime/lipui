using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using PluginKey = System.String;

namespace LipUI.Models.Plugin;

internal record struct PluginConfigFileInfo(string AssemblyName, string Name, Guid Guid);

internal static partial class PluginConfigManager
{
    private static readonly Dictionary<PluginKey, (PluginConfigFileInfo Info, ConfigCollection Config)> pluginConfigs = [];

    [GeneratedRegex(@"(?<assembly_name>.+?)\+(?<name>.+)\+(?<guid>[a-z0-9]{8}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{12})$")]
    private static partial Regex PluginConfigInfoRegex();

    private static bool TryParse(string str, [NotNullWhen(true)] out PluginConfigFileInfo info)
    {
        info = default;

        var match = PluginConfigInfoRegex().Match(str);
        if (match.Success is false)
            return false;

        info = new(match.Groups["assembly_name"].Value, match.Groups["name"].Value, Guid.Parse(match.Groups["guid"].Value));
        return true;
    }

    public static void Load()
    {
        lock (pluginConfigs)
        {
            var dir = new DirectoryInfo(DefaultSettings.ConfigsDirectory);
            if (dir.Exists is false)
                dir.Create();

            foreach (var file in dir.EnumerateFiles())
            {
                var key = Path.GetFileNameWithoutExtension(file.Name);

                if (TryParse(key, out var info) is false)
                    continue;

                pluginConfigs.Add(key, (info, ConfigCollection.Deserialize(File.ReadAllText(file.FullName))));
            }
        }
    }

    public static void Save(PluginKey key, ConfigCollection config)
    {
        lock (config)
        {
            if (config.Changed is false)
                return;

            var path = Path.Combine(DefaultSettings.ConfigsDirectory, $"{key}.json");
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
            foreach (var (key, (_, config)) in pluginConfigs)
                Save(key, config);
        }
    }

    public static ConfigCollection? InitPluginConfig(PluginKey key)
    {
        if (TryParse(key, out var info) is false)
            return null;

        lock (pluginConfigs)
        {
            if (pluginConfigs.ContainsKey(key))
                return null;

            var ret = new ConfigCollection();

            pluginConfigs.Add(key, (info, ret));

            return ret;
        }
    }

    public static ConfigCollection? InitPluginConfig(IPlugin plugin)
        => InitPluginConfig(PluginSystem.GetPluginKey(plugin));

    public static bool TryGetPluginConfig(PluginKey key, [NotNullWhen(true)] out ConfigCollection? config)
    {
        config = null;

        if (TryParse(key, out var _) && pluginConfigs.TryGetValue(key, out var temp) is true)
        {
            config = temp.Config;
            return true;
        }

        return false;
    }

    public static bool TryGetPluginConfig(IPlugin plugin, [NotNullWhen(true)] out ConfigCollection? config)
        => TryGetPluginConfig(PluginSystem.GetPluginKey(plugin), out config);

    public static ConfigCollection GetOrCreatePluginConfig(PluginKey key)
    {
        if (TryGetPluginConfig(key, out var config))
            return config;

        return InitPluginConfig(key) ?? throw new NullReferenceException();
    }

    public static ConfigCollection GetOrCreatePluginConfig(IPlugin plugin)
        => GetOrCreatePluginConfig(PluginSystem.GetPluginKey(plugin));

    public static bool Contains(IPlugin plugin) => pluginConfigs.ContainsKey(PluginSystem.GetPluginKey(plugin));
}
