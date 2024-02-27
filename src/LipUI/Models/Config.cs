using CommunityToolkit.WinUI.Helpers;
using LipUI.Pages.Settings;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace LipUI.Models;

internal static class DefaultSettings
{
    public const string ConfigFileName = "config.json";

    public const string PluginsDir = "plugins";

    public const string DataDirectory = ".lipui";

    public const string LipRepoOwner = "lippkg";

    public const string LipRepoName = "lip";

    public const string LipPortableUrl = "https://api.github.com/repos/lippkg/lip/releases/latest";




    public const string LipIndexApiKey = "api.lippkg.com";

    public const string GitHubApiKey = "api.github.com";

    public const string LipExecutableFileName = "lip.exe";
}

internal class Config
{
    public class GeneralSetting
    {
        [JsonPropertyName("lip_path")]
        public string LipPath { get; set; } = string.Empty;

        [JsonPropertyName("lip_index_api")]
        public string LipIndexApiKey { get; set; } = string.Empty;

        [JsonPropertyName("github_api")]
        public string GithubApiKey { get; set; } = string.Empty;

        [JsonPropertyName("github_proxy")]
        public string GithubProxy { get; set; } = string.Empty;
    }

    public class PersonalizationSetting
    {

        [JsonPropertyName("color_theme")]
        public ElementTheme ColorTheme { get; set; } = ElementTheme.Default;

        [JsonPropertyName("backdrop_type")]
        public PersonalizationSettingsView.BackdropControllerType BackdropType { get; set; } = PersonalizationSettingsView.BackdropControllerType.None;

        [JsonPropertyName("backdrop_luminosity")]
        public double? BackdropLuminosity { get; set; } = 20;

        [JsonPropertyName("background_color")]
        public string? BackgroundColor { get; set; }

        [JsonPropertyName("navigation_view_content_background_color")]
        public string? NavigationViewContentBackgroundColor { get; set; }

        [JsonPropertyName("navigation_view_content_border_color")]
        public string? NavigationViewContentBorderColor { get; set; }

        [JsonPropertyName("background_secondary_color")]
        public string? BackgroundSecondaryColor { get; set; }

        [JsonPropertyName("enable_image_background")]
        public bool EnableImageBackground { get; set; }

        [JsonPropertyName("background_image_path")]
        public string? BackgroundImagePath { get; set; }

        [JsonPropertyName("reload_colors")]
        public bool ResetColors { get; set; }
    }


    [JsonPropertyName("general_settings")]
    public GeneralSetting GeneralSettings { get; set; }

    [JsonPropertyName("personalization_settings")]
    public PersonalizationSetting PersonalizationSettings { get; set; }

    [JsonPropertyName("plugin_enable_info")]
    public Dictionary<string, bool> PluginEanbleInfo { get; set; }

    [JsonPropertyName("servers")]
    public List<ServerInstance> ServerInstances { get; set; }

    [JsonPropertyName("selected_server")]
    public ServerInstance? SelectedServer { get; set; }

    public Config()
    {
        ServerInstances = [];
        ResetGeneralSettings(Main.WorkingDirectory);
        ResetPersonalizationSettings();
        PluginEanbleInfo = [];
    }

    [MemberNotNull(nameof(GeneralSettings))]
    public void ResetGeneralSettings(string workingDir)
    {
        GeneralSettings = new()
        {
            LipPath = Path.Combine(workingDir, DefaultSettings.LipExecutableFileName),
            LipIndexApiKey = DefaultSettings.LipIndexApiKey,
            GithubApiKey = DefaultSettings.GitHubApiKey,
        };
    }

    [MemberNotNull(nameof(PersonalizationSettings))]
    public void ResetPersonalizationSettings()
    {
        PersonalizationSettings = new()
        {
            BackdropType = PersonalizationSettingsView.BackdropControllerType.None,
            BackdropLuminosity = 0,
            BackgroundColor = Colors.Transparent.ToHex(),
            NavigationViewContentBackgroundColor = Colors.Transparent.ToHex(),
            NavigationViewContentBorderColor = Colors.Transparent.ToHex(),
            BackgroundSecondaryColor = Colors.Transparent.ToHex(),
            EnableImageBackground = false,
            BackgroundImagePath = null,
            ResetColors = true
        };
    }
}
