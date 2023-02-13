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
    [JsonProperty("index")] public IReadOnlyDictionary<string, LipRegistryItem> Index;
    public class LipRegistryItem
    {
        [JsonProperty("author")] public string Author { get; set; }
        [JsonProperty("description")] public string Description { get; set; }
        [JsonProperty("homepage")] public string Homepage { get; set; }
        [JsonProperty("license")] public string License { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("repository")] public string Repository { get; set; }
        [JsonProperty("tooth")] public string Tooth { get; set; }
    }
}