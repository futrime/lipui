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
        [JsonProperty("author")] public string Author;
        [JsonProperty("description")] public string Description;
        [JsonProperty("homepage")] public string Homepage;
        [JsonProperty("license")] public string License;
        [JsonProperty("name")] public string Name;
        [JsonProperty("repository")] public string Repository;
        [JsonProperty("tooth")] public string Tooth;
    }
}