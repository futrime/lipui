using LipUI.Pages.Home.Modules;
using System.Reflection;
using System.Runtime.Loader;
using GuidString = System.String;

namespace LipUI.Models.Plugin;

internal static class PluginSystem
{
    static PluginSystem()
    {
        PluginConfigManager.Load();
        ModulesPage.InitEventHandlers();
        MainWindow.InitEventHandlers();
    }

    public static IEnumerable<IPlugin>? Plugins { get; private set; }

    public static IEnumerable<IUIPlugin>? UIPlugins { get; private set; }

    public static IEnumerable<IHomePageModule>? Modules { get; private set; }

    private static readonly HashSet<IPlugin> enabledPlugins = [];

    private static readonly Dictionary<GuidString, IPlugin> guidWithPlugins = [];


    /// <summary>
    /// noexcept
    /// </summary>
    public static async ValueTask LoadAsync()
    {
        try
        {
            var types = await GetPluginTypes();
            if (types is null)
                return;

            var (plugins, uiPlugins, modules) = await CreateInstances(types);

            (Plugins, UIPlugins, Modules) = (plugins, uiPlugins, modules);

            await InitializePlugins(plugins);
            await EnablePlugins(plugins);
        }
        catch (Exception ex) { await InternalServices.ShowInfoBarAsync(ex); }
    }

    private static async ValueTask<IEnumerable<Type>?> GetPluginTypes()
    {
        var dir = Path.Combine(Main.WorkingDirectory, DefaultSettings.PluginsDir);

        if (Directory.Exists(dir) is false)
            Directory.CreateDirectory(dir);

        List<Type> pluginTypes = [];
        var ctx = AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly())!;

        var dirInfo = new DirectoryInfo(dir);

        var types = from type in Assembly.GetExecutingAssembly().GetTypes()
                    where type.IsAssignableTo(typeof(IPlugin)) && type.GetCustomAttribute<LipUIModuleAttribute>() is not null
                    select type;

        pluginTypes.AddRange(types);

        foreach (var file in dirInfo.EnumerateFiles())
        {
            types = await TryLoadAssemblyAndGetPluginTypes(ctx, file.FullName);
            if (types is not null)
                pluginTypes.AddRange(types);
        }

        return pluginTypes;
    }

    private static async ValueTask<IEnumerable<Type>?> TryLoadAssemblyAndGetPluginTypes(AssemblyLoadContext ctx, string path)
    {
        try
        {
            var assembly = ctx.LoadFromAssemblyPath(path);
            if (assembly is null)
                return null;

            var types = from type in assembly.DefinedTypes
                        where type.IsAssignableTo(typeof(IPlugin)) && type.GetCustomAttribute<LipUIModuleAttribute>() is not null
                        select type;
            return types;
        }
        catch (Exception ex)
        {
            await InternalServices.ShowInfoBarAsync(ex);
            return null;
        }
    }

    private static async ValueTask<(IEnumerable<IPlugin>, IEnumerable<IUIPlugin>, IEnumerable<IHomePageModule>)> CreateInstances(IEnumerable<Type> types)
    {
        List<IPlugin> instances = new(types.Count());
        List<IUIPlugin> uiInstances = [];
        List<IHomePageModule> modules = [];

        foreach (var type in types)
        {
            try
            {
                var obj = Activator.CreateInstance(type);

                if (obj is IPlugin plugin)
                {
                    var guidStr = plugin.Guid.ToString();
                    if (guidWithPlugins.ContainsKey(guidStr))
                        continue;

                    guidWithPlugins.Add(guidStr, plugin);

                    instances.Add(plugin);
                }

                if (obj is IUIPlugin ui)
                    uiInstances.Add(ui);

                if (obj is IHomePageModule module)
                    modules.Add(module);
            }
            catch (Exception ex)
            {
                await InternalServices.ShowInfoBarAsync(ex);
                continue;
            }
        }
        return (instances, uiInstances, modules);
    }

    private static async ValueTask InitializePlugins(IEnumerable<IPlugin> plugins)
    {
        foreach (var plugin in plugins)
        {
            try
            {
                await Task.Run(() => plugin.OnInitlalize(LipuiServices.Default));
            }
            catch (Exception ex)
            {
                await InternalServices.ShowInfoBarAsync(ex);
                continue;
            }
        }
    }

    public static string GetPluginKey(IPlugin plugin)
        => $"{plugin.GetType().Assembly.GetName().Name}+{plugin.Guid}";

    private static async ValueTask EnablePlugins(IEnumerable<IPlugin> plugins)
    {
        var enableInfo = Main.Config.PluginEanbleInfo;
        List<(string, bool)> temp = new(8);

        foreach (var plugin in plugins)
        {
            var key = GetPluginKey(plugin);
            try
            {
                if (enableInfo.TryGetValue(key, out bool enable) is false)
                {
                    enable = plugin.DefaultEnabled;
                }

                if (enable)
                {
                    EnablePlugin(plugin);
                }

                temp.Add((key, enable));
            }
            catch (Exception ex)
            {
                await InternalServices.ShowInfoBarAsync(ex);
                continue;
            }
        }

        Main.Config.ClearPluginEnableInfo();
        foreach (var (key, val) in temp)
            Main.Config.AddPluginEnableInfo(key, val);
    }

    public static void EnablePlugin(IPlugin plugin)
    {
        try
        {
            if (enabledPlugins.Contains(plugin))
                return;

            Main.Config.SetPluginEnabled(GetPluginKey(plugin));

            enabledPlugins.Add(plugin);

            Task.Run(() => plugin.OnEnable(LipuiServices.Default));
            PluginEnabled?.Invoke(plugin);
        }
        catch (Exception ex)
        {
            Task.Run(() => InternalServices.ShowInfoBarAsync(ex));
        }
    }

    public static void DisablePlugin(IPlugin plugin)
    {
        try
        {
            if (enabledPlugins.Contains(plugin) is false)
                return;

            Main.Config.SetPluginDisabled(GetPluginKey(plugin));

            enabledPlugins.Remove(plugin);

            Task.Run(() => plugin.OnDisable(LipuiServices.Default));
            PluginDisabled?.Invoke(plugin);
        }
        catch (Exception ex)
        {
            Task.Run(() => InternalServices.ShowInfoBarAsync(ex));
        }
    }

    public static bool IsPluginEnabled(IPlugin plugin)
        => enabledPlugins.Contains(plugin);

    public static event Action<IPlugin>? PluginEnabled;
    public static event Action<IPlugin>? PluginDisabled;
}
