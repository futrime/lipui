using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace LipNETWrapper.Class
{
    /*[
      {
          "tooth": "github.com/tooth-hub/bds",
          "version": "1.19.61"
      },
      {
          "tooth": "github.com/tooth-hub/bdsdownloader",
          "version": "0.1.0"
      },
      {
          "tooth": "github.com/tooth-hub/liteloaderbds",
          "version": "2.10.0"
      },
      {
          "tooth": "github.com/tooth-hub/llessentials",
          "version": "2.10.0"
      }
  ]*/
    [Serializable]
    public class LipPackageSimple
    {
        [JsonProperty("tooth")] public string Tooth { get; set; }
        [JsonProperty("version")] public string Version { get; set; }
    }
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
    public class LipPackage
    {
        [JsonProperty("author")] public string Author { get; set; }
        [JsonProperty("description")] public string Description { get; set; }
        [JsonProperty("files")] public LipFile[] Files { get; set; }
        [JsonProperty("homepage")] public string Homepage { get; set; }
        [JsonProperty("license")] public string License { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("tooth")] public string Tooth { get; set; }
        [JsonProperty("version")] public string Version { get; set; }
        [JsonProperty("versions")] public string[] Versions { get; set; }
    }

    [Serializable]
    public class LipFile
    {
        //[JsonProperty("name")] public string Name { get; set; }
        //[JsonProperty("sha256")] public string Sha256 { get; set; }
        //[JsonProperty("size")] public long Size { get; set; }
        //[JsonProperty("url")] public string Url { get; set; }
    }

}
