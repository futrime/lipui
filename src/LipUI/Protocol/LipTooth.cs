using System.Text.Json;
using System.Text.Json.Serialization;

namespace LipUI.Protocol;

public struct LipToothVersion
{
    public LipToothVersion()
    {
    }

    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    [JsonPropertyName("releasedAt")]
    public string ReleasedAt { get; set; } = string.Empty;
}

public class LipTooth
{
    [JsonPropertyName("apiVersion")]
    public string ApiVersion { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public LipToothData Data { get; set; } = new();

    public class LipToothData
    {
        [JsonPropertyName("repoPath")]
        public string RepoPath { get; set; } = string.Empty;

        [JsonPropertyName("repoOwner")]
        public string RepoOwner { get; set; } = string.Empty;

        [JsonPropertyName("repoName")]
        public string RepoName { get; set; } = string.Empty;

        [JsonPropertyName("version")]
        public string Version { get; set; } = string.Empty;

        [JsonPropertyName("releasedAt")]
        public string ReleasedAt { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("author")]
        public string Author { get; set; } = string.Empty;

        [JsonPropertyName("tags")]
        public IReadOnlyList<string> Tags { get; set; } = new List<string>();

        [JsonPropertyName("avatarUrl")]
        public string AvatarUrl { get; set; } = string.Empty;

        [JsonPropertyName("Source")]
        public string Source { get; set; } = string.Empty;

        [JsonPropertyName("sourceRepoCreatedAt")]
        public string SourceRepoCreatedAt { get; set; } = string.Empty;

        [JsonPropertyName("sourceRepoStarCount")]
        public int SourceRepoStarCount { get; set; }

        [JsonPropertyName("versions")]
        public IReadOnlyList<LipToothVersion> Versions { get; set; } = new List<LipToothVersion>();

        [JsonPropertyName("downloadCount")]
        public int DownloadCount { get; set; }
    }

    public static LipTooth Deserialize(string json)
        => JsonSerializer.Deserialize<LipTooth>(json) ?? throw new NullReferenceException();
}
