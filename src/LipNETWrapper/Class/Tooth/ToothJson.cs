﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace LipNETWrapper.Class.Tooth;
public class ToothJson
{
    [JsonProperty("format_version")] public int FormatVersion { get; set; }

    [JsonProperty("tooth")] public string Tooth { get; set; } = "";
    [JsonProperty("version")] public string Version { get; set; } = "";
    [JsonProperty("dependencies")] public Dictionary<string, ObservableCollection<ObservableCollection<string>>> Dependencies { get; set; } = new();
    [JsonProperty("information")] public Information Information { get; set; } = new();
    [JsonProperty("placement")] public ObservableCollection<Placement> Placement { get; set; } = new();
    [JsonProperty("possession")] public ObservableCollection<string> Possession { get; set; } = new();
    [JsonProperty("commands")] public ObservableCollection<Command> Commands { get; set; } = new();
    [JsonProperty("confirmation")] public ObservableCollection<Confirmation> Confirmation { get; set; } = new();
}

public class Information
{
    [JsonProperty("name")] public string Name { get; set; } = "";
    [JsonProperty("description")] public string Description { get; set; } = "";
    [JsonProperty("author")] public string Author { get; set; } = "";
    [JsonProperty("license")] public string License { get; set; } = "";
    [JsonProperty("homepage")] public string Homepage { get; set; } = "";
}

public class Placement
{
    [JsonProperty("source")] public string Source { get; set; } = "";
    [JsonProperty("destination")] public string Destination { get; set; } = "";
}
public class Command
{
    [JsonProperty("type")] public string Type { get; set; } = "";
    [JsonProperty("commands")] public ObservableCollection<string> Commands { get; set; } = new();
    [JsonProperty("GOOS")] public string GOOS { get; set; } = "";
    [JsonProperty("GOARCH")] public string GOARCH { get; set; } = "";
}

public class Confirmation
{
    [JsonProperty("type")] public string Type { get; set; } = "";
    [JsonProperty("message")] public string Message { get; set; } = "";
    [JsonProperty("GOOS")] public string GOOS { get; set; } = "";

    [JsonProperty("GOARCH")] public string GOARCH { get; set; } = "";
}