using System.Text.Json;
using System.Text.Json.Serialization;

namespace LipUI.Protocol;

public class LipIndex
{
    [JsonPropertyName("apiVersion")]
    public string ApiVersion { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public LipIndexData Data { get; set; } = new();

    public class LipIndexData
    {
        [JsonPropertyName("pageIndex")]
        public int PageIndex { get; set; }

        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("items")]
        public List<LipToothItem> Items { get; set; } = new List<LipToothItem>();

        public class LipToothItem
        {
            [JsonPropertyName("repoPath")]
            public string RepoPath { get; set; } = string.Empty;

            [JsonPropertyName("repoOwner")]
            public string RepoOwner { get; set; } = string.Empty;

            [JsonPropertyName("repoName")]
            public string RepoName { get; set; } = string.Empty;

            [JsonPropertyName("latestVersion")]
            public string LatestVersion { get; set; } = string.Empty;

            [JsonPropertyName("latestVersionReleasedAt")]
            public string LatestVersionReleasedAt { get; set; } = string.Empty;

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

            [JsonPropertyName("source")]
            public string Source { get; set; } = string.Empty;

            [JsonPropertyName("sourceRepoCreatedAt")]
            public string SourceRepoCreatedAt { get; set; } = string.Empty;

            [JsonPropertyName("sourceRepoStarCount")]
            public int SourceRepoStarCount { get; set; }
        }
    }

    public static LipIndex Deserialize(string json)
        => JsonSerializer.Deserialize<LipIndex>(json) ?? throw new NullReferenceException();
}