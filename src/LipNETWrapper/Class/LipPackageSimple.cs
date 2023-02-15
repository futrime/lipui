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
        [JsonProperty("tooth")] public string Tooth { get; set; } = string.Empty;
        [JsonProperty("version")] public string Version { get; set; } = string.Empty;
    } 
}
