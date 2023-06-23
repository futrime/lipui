#nullable disable
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LipNETWrapper.Class;

public partial class LipPackage
{
    [JsonProperty("format_version")]
    public long FormatVersion { get; set; }

    [JsonProperty("tooth")]
    public string Tooth { get; set; }

    [JsonProperty("version")]
    public string Version { get; set; }

    [JsonProperty("info")]
    public Info Info { get; set; }

    [JsonProperty("commands")]
    public Commands Commands { get; set; }

    [JsonProperty("dependencies")]
    public Dictionary<string,string> Dependencies { get; set; }

    [JsonProperty("files")]
    public Files Files { get; set; }
}

public partial class Commands
{
    [JsonProperty("post_install")]
    public string[] PostInstall { get; set; }
} 
public partial class Files
{
    [JsonProperty("place")]
    public Place[] Place { get; set; }

    [JsonProperty("remove")]
    public string[] Remove { get; set; }
}

public partial class Place
{
    [JsonProperty("src")]
    public string Src { get; set; }

    [JsonProperty("dest")]
    public string Dest { get; set; }
}

public partial class Info
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("author")]
    public string Author { get; set; }
}