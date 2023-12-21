using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LipUI.Protocol;

public class LipTooth
{
    [JsonPropertyName("apiVersion")]
    public string ApiVersion { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public LipToothData Data { get; set; } = new();

    public class LipToothData
    {
        [JsonPropertyName("toothRepoOwner")]
        public string ToothRepoOwner { get; set; } = string.Empty;

        [JsonPropertyName("toothRepoName")]
        public string ToothRepoName { get; set; } = string.Empty;

        [JsonPropertyName("version")]
        public string Version { get; set; } = string.Empty;

        [JsonPropertyName("releaseTime")]
        public long ReleaseTime { get; set; }

        [JsonPropertyName("versions")]
        public IReadOnlyList<LipToothVersion> Versions { get; set; } = new List<LipToothVersion>();

        [JsonPropertyName("downloadCount")]
        public int DownloadCount { get; set; }

        public struct LipToothVersion
        {
            [JsonPropertyName("version")]
            public string Version { get; set; }

            [JsonPropertyName("releaseTime")]
            public long ReleaseTime { get; set; }
        }
    }

    public static LipTooth Deserialize(string json)
        => JsonSerializer.Deserialize<LipTooth>(json) ?? throw new NullReferenceException();
}
