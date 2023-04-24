using Newtonsoft.Json;
using System;

namespace LipNETWrapper.Class
{
    //    {
    //    "author": "Mojang",
    //    "description": "Bedrock Dedicated Servers allow Minecraft players on Windows and Linux computers to set up their own server at home, or host their server using a cloud-based service.",
    //    "files": [],
    //    "homepage": "https://www.minecraft.net",
    //    "license": "",
    //    "name": "Minecraft Bedrock Dedicated Server",
    //    "tooth": "github.com/tooth-hub/bds",
    //    "version": "1.19.61",
    //    "versions": [
    //    "1.19.61",
    //    "1.7.0",
    //    "1.6.1"
    //        ]
    //}
    [Serializable]
    public class LipPackageVersions
    { 
        [JsonProperty("versions")] public string[]? Versions { get; set; }
    }
    [Serializable]
    public class LipPackage: LipPackageVersions
    {
        [JsonProperty("author")] public string Author { get; set; } = string.Empty;
        [JsonProperty("description")] public string Description { get; set; } = string.Empty;
        [JsonProperty("files")] public LipFile[] Files { get; set; } = Array.Empty<LipFile>();
        [JsonProperty("homepage")] public string Homepage { get; set; } = string.Empty;
        [JsonProperty("license")] public string License { get; set; } = string.Empty;
        [JsonProperty("name")] public string Name { get; set; } = string.Empty;
        [JsonProperty("tooth")] public string Tooth { get; set; } = string.Empty;
        [JsonProperty("version")] public string Version { get; set; } = string.Empty;
    }
    [Serializable]
    public class LipFile//todo 这玩意长啥样
    {
        //[JsonProperty("name")] public string Name { get; set; }
        //[JsonProperty("sha256")] public string Sha256 { get; set; }
        //[JsonProperty("size")] public long Size { get; set; }
        //[JsonProperty("url")] public string Url { get; set; }
    }
}
