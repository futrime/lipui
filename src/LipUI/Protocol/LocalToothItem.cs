using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LipUI.Protocol;

internal class ToothPackage
{
    public class ToothPackageInfo
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("author")]
        public string Author { get; set; } = string.Empty;

        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; } = new();
    }

    public class ToothPackageFiles
    {
        [JsonPropertyName("place")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<Place>? Place { get; set; }

        [JsonPropertyName("preserve")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? Preserve { get; set; }

        [JsonPropertyName("remove")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? Remove { get; set; }
    }

    public class Place
    {
        [JsonPropertyName("src")]
        public string Src { get; set; } = string.Empty;

        [JsonPropertyName("dest")]
        public string Dest { get; set; } = string.Empty;
    }

    public class ToothPackageCommands
    {
        [JsonPropertyName("pre_install")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? PreInstall { get; set; }

        [JsonPropertyName("post_install")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? PostInstall { get; set; }

        [JsonPropertyName("pre_uninstall")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? PreUninstall { get; set; }

        [JsonPropertyName("post_uninstall")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? PostUninstall { get; set; }
    }


    [JsonPropertyName("format_version")]
    public int FormatVersion { get; set; }

    [JsonPropertyName("tooth")]
    public string Tooth { get; set; } = string.Empty;

    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    [JsonPropertyName("info")]
    public ToothPackageInfo Info { get; set; } = new();

    [JsonPropertyName("commands")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ToothPackageCommands? Commands { get; set; }

    [JsonPropertyName("dependencies")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, string>? Dependencies { get; set; }

    [JsonPropertyName("prerequisites")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, string>? Prerequisites { get; set; }

    [JsonPropertyName("files")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ToothPackageFiles? Files { get; set; }
}
