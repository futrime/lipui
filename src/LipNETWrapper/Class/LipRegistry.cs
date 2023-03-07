using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LipNETWrapper.Class;
public class LipRegistry
{
    /*
         * {
        "format_version": 1,
        "index": {
            "adaptationprotocols": {
                "author": "QingYu",
                "description": "Let 1.19.60/62 client into 1.19.63 server or 1.19.63 client into 1.19.61/62 server",
                "homepage": "",
                "license": "",
                "name": "AdaptationProtocols",
                "repository": "github.com/Tooth-Hub/AdaptationProtocols",
                "tags": ["plugin", "ll"],
                "tooth": "github.com/Tooth-Hub/AdaptationProtocols"
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
        [JsonProperty("tags")] public IReadOnlyList<string> Tags { get; set; } = Array.Empty<string>();
        [JsonProperty("tooth")] public string Tooth { get; set; } = string.Empty;
    }
}