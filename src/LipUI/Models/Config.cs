using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;

namespace LipUI.Models;

internal static class DefaultSettings
{
    public const string ConfigFileName = "config.json";

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

    [JsonPropertyName("settings")]
    public Setting Settings { get; set; }

    [JsonPropertyName("servers")]
    public List<ServerInstance> ServerInstances { get; set; }

    [JsonPropertyName("selected_server")]
#nullable enable
    public ServerInstance? SelectedServer { get; set; }
#nullable disable

    public class Setting
    {
        [JsonPropertyName("lip_path")]
        public string LipPath { get; set; }

        [JsonPropertyName("lip_index_api")]
        public string LipIndexApiKey { get; set; }

        [JsonPropertyName("github_api")]
        public string GithubApiKey { get; set; }
    }

    public Config()
    {
        ServerInstances = new();
        ResetSettings(Main.WorkingDirectory);
    }

    public void ResetSettings(string workingDir)
    {
        Settings = new()
        {
            LipPath = Path.Combine(workingDir, DefaultSettings.LipExecutableFileName),
            LipIndexApiKey = DefaultSettings.LipIndexApiKey,
            GithubApiKey = DefaultSettings.GitHubApiKey,
        };
    }
}
