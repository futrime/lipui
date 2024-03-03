using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI.Helpers;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using System.ComponentModel;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LipUI.Models;

public enum BackdropControllerType { Mica, Acrylic, Transparent, None }

internal static class DefaultSettings
{
    public const string ConfigFileName = "config.json";

    public const string PluginsDir = "plugins";

    public const string DataDirectory = ".lipui";

    public const string ConfigsDirectory = DataDirectory + "/configs";

    public const string LipRepoOwner = "lippkg";

    public const string LipRepoName = "lip";

    public const string LipPortableUrl = "https://api.github.com/repos/lippkg/lip/releases/latest";




    public const string LipIndexApiKey = "api.lippkg.com";

    public const string GitHubApiKey = "api.github.com";

    public const string LipExecutableFileName = "lip.exe";
}

public partial class Config : ObservableObject
{
    public partial class GeneralSetting : ObservableObject
    {
        [ObservableProperty]
        [property: JsonPropertyName("lip_path")]
        private string lipPath = string.Empty;

        [ObservableProperty]
        [property: JsonPropertyName("lip_index_api")]
        private string lipIndexApiKey = string.Empty;

        [ObservableProperty]
        [property: JsonPropertyName("github_api")]
        private string githubApiKey = string.Empty;

        [ObservableProperty]
        [property: JsonPropertyName("github_proxy")]
        private string githubProxy = string.Empty;
    }

    public partial class PersonalizationSetting : ObservableObject
    {
        [ObservableProperty]
        [property: JsonPropertyName("color_theme")]
        private ElementTheme colorTheme = ElementTheme.Default;

        [ObservableProperty]
        [property: JsonPropertyName("backdrop_type")]
        private BackdropControllerType backdropType = BackdropControllerType.None;

        [ObservableProperty]
        [property: JsonPropertyName("backdrop_luminosity")]
        private double? backdropLuminosity = 20;

        [ObservableProperty]
        [property: JsonPropertyName("background_color")]
        private string? backgroundColor;

        [ObservableProperty]
        [property: JsonPropertyName("navigation_view_content_background_color")]
        private string? navigationViewContentBackgroundColor;

        [ObservableProperty]
        [property: JsonPropertyName("navigation_view_content_border_color")]
        private string? navigationViewContentBorderColor;

        [ObservableProperty]
        [property: JsonPropertyName("background_secondary_color")]
        private string? backgroundSecondaryColor;

        [ObservableProperty]
        [property: JsonPropertyName("enable_image_background")]
        private bool enableImageBackground;

        [ObservableProperty]
        [property: JsonPropertyName("background_image_path")]
        private string? backgroundImagePath;

        [ObservableProperty]
        [property: JsonPropertyName("reload_colors")]
        private bool resetColors;
    }

    [ObservableProperty]
    [property: JsonPropertyName("general_settings")]
    private GeneralSetting generalSettings = new();

    [ObservableProperty]
    [property: JsonPropertyName("personalization_settings")]
    private PersonalizationSetting personalizationSettings = new();

    [ObservableProperty]
    [property: JsonPropertyName("selected_server")]
    private ServerInstance? selectedServer;


    [JsonInclude]
    [JsonPropertyName("plugin_enable_info")]
    private Dictionary<string, bool> pluginEanbleInfo = [];

    [JsonInclude]
    [JsonPropertyName("servers")]
    private readonly List<ServerInstance> serverInstances = [];

    [JsonIgnore]
    public IReadOnlyDictionary<string, bool> PluginEanbleInfo => pluginEanbleInfo;

    [JsonIgnore]
    public IReadOnlyList<ServerInstance> ServerInstances => serverInstances;

    public void AddServerInstance(ServerInstance instance)
    {
        serverInstances.Add(instance);
        OnPropertyChanged(nameof(ServerInstances));
    }

    public void RemoveServerInstance(ServerInstance instance)
    {
        serverInstances.Remove(instance);
        OnPropertyChanged(nameof(ServerInstances));
    }

    public void SetPluginEnabled(string key)
    {
        pluginEanbleInfo[key] = true;
        OnPropertyChanged(nameof(PluginEanbleInfo));
    }

    public void SetPluginDisabled(string key)
    {
        pluginEanbleInfo[key] = false;
        OnPropertyChanged(nameof(PluginEanbleInfo));
    }

    public void AddPluginEnableInfo(string key, bool val)
    {
        pluginEanbleInfo.Add(key, val);
        OnPropertyChanged(nameof(PluginEanbleInfo));
    }

    public void RemovePluginEnableInfo(string key)
    {
        pluginEanbleInfo.Remove(key);
        OnPropertyChanged(nameof(PluginEanbleInfo));
    }

    public void ClearPluginEnableInfo()
    {
        pluginEanbleInfo.Clear();
        OnPropertyChanged(nameof(PluginEanbleInfo));
    }

    public Config()
    {
        ResetGeneralSettings(Main.WorkingDirectory);
        ResetPersonalizationSettings();

        void GeneralSettingsPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(GeneralSettings));
        }
        void PersonalizationSettingsPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(PersonalizationSettings));
        }

        GeneralSettings.PropertyChanged += GeneralSettingsPropertyChanged;
        PersonalizationSettings.PropertyChanged += PersonalizationSettingsPropertyChanged;
    }

    public void ResetGeneralSettings(string workingDir)
    {
        GeneralSettings.LipPath = Path.Combine(workingDir, DefaultSettings.LipExecutableFileName);
        GeneralSettings.LipIndexApiKey = DefaultSettings.LipIndexApiKey;
        GeneralSettings.GithubApiKey = DefaultSettings.GitHubApiKey;
    }

    public void ResetPersonalizationSettings()
    {
        PersonalizationSettings.BackdropType = BackdropControllerType.None;
        PersonalizationSettings.BackdropLuminosity = 0;
        PersonalizationSettings.BackgroundColor = Colors.Transparent.ToHex();
        PersonalizationSettings.NavigationViewContentBackgroundColor = Colors.Transparent.ToHex();
        PersonalizationSettings.NavigationViewContentBorderColor = Colors.Transparent.ToHex();
        PersonalizationSettings.BackgroundSecondaryColor = Colors.Transparent.ToHex();
        PersonalizationSettings.EnableImageBackground = false;
        PersonalizationSettings.BackgroundImagePath = null;
        PersonalizationSettings.ResetColors = true;
    }

    internal static readonly JsonSerializerOptions options = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public static Config Deserialize(string json) => JsonSerializer.Deserialize<Config>(json)!;

    public string Serialize()
    {
        lock (this)
        {
            return JsonSerializer.Serialize(this, options);
        }
    }
}
