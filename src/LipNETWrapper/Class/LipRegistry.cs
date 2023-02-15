using System.Collections.Generic;
using Newtonsoft.Json;

namespace LipNETWrapper.Class;
public class LipRegistry
{
    /*
         * {
        "format_version": 1,
        "index": {
            "7zip": {
                "author": "Futrime",
                "description": "A Lip tool adaptation of 7-Zip",
                "homepage": "https://www.7-zip.org/",
                "license": "LGPL-2.1-or-later",
                "name": "7-Zip Lip Tool",
                "repository": "github.com/Tooth-Hub/7zip",
                "tooth": "github.com/Tooth-Hub/7zip"
            }
        }
    }   */
    [JsonProperty("format_version")] public int FormatVersion;
#pragma warning disable CS8618
    [JsonProperty("index")] public IReadOnlyDictionary<string, LipRegistryItem> Index;
#pragma warning restore CS8618
    public class LipRegistryItem
    {
        [JsonProperty("author")] public string Author { get; set; } = string.Empty;
        [JsonProperty("description")] public string Description { get; set; } = string.Empty;
        [JsonProperty("homepage")] public string Homepage { get; set; } = string.Empty;
        [JsonProperty("license")] public string License { get; set; } = string.Empty;
        [JsonProperty("name")] public string Name { get; set; } = string.Empty;
        [JsonProperty("repository")] public string Repository { get; set; } = string.Empty;
        [JsonProperty("tooth")] public string Tooth { get; set; } = string.Empty;
    }
}