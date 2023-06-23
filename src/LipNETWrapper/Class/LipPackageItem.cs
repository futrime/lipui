using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
#nullable disable 
namespace LipNETWrapper.Class;

public class LipPackageItem
{
    [JsonProperty("available_versions")] public string[] AvailableVersions { get; set; }
    [JsonProperty("metadata")] public LipPackage Package { get; set; }
}