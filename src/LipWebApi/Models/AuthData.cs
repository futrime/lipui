using System;
using System.Collections.Concurrent;
using System.IO;
using Newtonsoft.Json;

namespace LipWebApi.Models
{
    public class AuthData : ConcurrentDictionary<string, string>
    {
        public static readonly Lazy<AuthData> _instance = new(() =>
        {
            var authConfig = Path.Combine(Global.ConfigFolder, "auth.json");
            return JsonConvert.DeserializeObject<AuthData>(File.ReadAllText(authConfig))!;
        });
        public static AuthData Instance => _instance.Value;
    }
}
