using LipUI.Pages.Home.Modules;
using System.Reflection;
using System.Runtime.Loader;
using guid_string = System.String;

namespace LipUI.Models.Plugin;

internal static class PluginSystem
{
    static PluginSystem()
    {
        ModulesPage.InitEventHandlers();
        MainWindow.InitEventHandlers();
    }

    public static IEnumerable<ILipuiPlugin>? Plugins { get; private set; }

    public static IEnumerable<ILipuiPluginUI>? UIPlugins { get; private set; }

    public static IEnumerable<ILipuiPluginModule>? Modules { get; private set; }

    private static readonly HashSet<ILipuiPlugin> enabledPlugins = [];

    private static readonly Dictionary<guid_string, ILipuiPlugin> guidWithPlugins = [];


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
        {
            Directory.CreateDirectory(dir);
            return null;
        }

        List<Type> pluginTypes = [];
        var ctx = AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly())!;

        var dirInfo = new DirectoryInfo(dir);

        var types = from type in Assembly.GetExecutingAssembly().GetTypes()
                    where type.IsAssignableTo(typeof(ILipuiPlugin)) && type.GetCustomAttribute<LipUIModuleAttribute>() is not null
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
                        where type.IsAssignableTo(typeof(ILipuiPlugin)) && type.GetCustomAttribute<LipUIModuleAttribute>() is not null
                        select type;
            return types;
        }
        catch (Exception ex)
        {
            await InternalServices.ShowInfoBarAsync(ex);
            return null;
        }
    }

    private static async ValueTask<(IEnumerable<ILipuiPlugin>, IEnumerable<ILipuiPluginUI>, IEnumerable<ILipuiPluginModule>)> CreateInstances(IEnumerable<Type> types)
    {
        List<ILipuiPlugin> instances = new(types.Count());
        List<ILipuiPluginUI> uiInstances = [];
        List<ILipuiPluginModule> modules = [];

        foreach (var type in types)
        {
            try
            {
                var obj = Activator.CreateInstance(type);

                if (obj is ILipuiPlugin plugin)
                {
                    var guidStr = plugin.Guid.ToString();
                    if (guidWithPlugins.ContainsKey(guidStr))
                        continue;

                    guidWithPlugins.Add(guidStr, plugin);

                    instances.Add(plugin);
                }

                if (obj is ILipuiPluginUI ui)
                    uiInstances.Add(ui);

                if (obj is ILipuiPluginModule module)
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

    private static async ValueTask InitializePlugins(IEnumerable<ILipuiPlugin> plugins)
    {
        var info = new LipuiRuntimeInfo()
        {
            Theme = Main.Config.PersonalizationSettings.ColorTheme,
            ApplicationWindow = InternalServices.MainWindow ?? throw new NullReferenceException()
        };
        foreach (var plugin in plugins)
        {
            try
            {
                await Task.Run(() => plugin.OnInitlalize(info));
            }
            catch (Exception ex)
            {
                await InternalServices.ShowInfoBarAsync(ex);
                continue;
            }
        }
    }

    public static string GetPluginKey(ILipuiPlugin plugin)
        => $"{plugin.GetType().Assembly.GetName().Name}+{plugin.PluginName}+{plugin.Guid}";

    private static async ValueTask EnablePlugins(IEnumerable<ILipuiPlugin> plugins)
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

    public static void EnablePlugin(ILipuiPlugin plugin)
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

    public static void DisablePlugin(ILipuiPlugin plugin)
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

    public static bool IsPluginEnabled(ILipuiPlugin plugin)
        => enabledPlugins.Contains(plugin);

    public static event Action<ILipuiPlugin>? PluginEnabled;
    public static event Action<ILipuiPlugin>? PluginDisabled;
}
