#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LipNETWrapper.Class;
using Newtonsoft.Json;

namespace LipNETWrapper
{
    public class LipConsoleWrapper : ILipWrapper
    {
        public LipConsoleWrapper(string executablePath = "lip.exe")
        {
            ExecutablePath = executablePath;
        }
        public string ExecutablePath { get; }
        public async Task<string> GetLipVersion(CancellationToken tk = default)
        {
            return (await new LipConsoleLoader(ExecutablePath)
                .RunString(LipCommand.Create("-V"), tk: tk)).Trim();
        }
        public async Task<(LipPackageSimple[] packages, string message)> GetAllPackagesAsync(CancellationToken tk = default)
        {
            var text = (await new LipConsoleLoader(ExecutablePath)
                .RunString(LipCommand.Create("list").WithJson(), tk: tk));
            var json = text.Split('\n').First(x => x.StartsWith("[")).Trim();
            return (JsonConvert.DeserializeObject<LipPackageSimple[]>(json)!, text);
        }
        public async Task<(bool success, LipPackage? package, string message)> GetPackageInfoAsync(string packageId, CancellationToken tk = default, Action<string>? onOutput = null)
        {
            var text = await new LipConsoleLoader(ExecutablePath)
                .RunString(LipCommand.Create("show").WithJson() + "--available" + packageId, onOutput, tk);
            var json = text.Split('\n').FirstOrDefault(x => x.StartsWith("{"))?.Trim();
            return (json is not null, json is null ? null : JsonConvert.DeserializeObject<LipPackage>(json)!, text);
        }
        public async Task<(bool success, LipPackage? package, string message)> GetLocalPackageInfoAsync(string packageId, CancellationToken tk = default)
        {
            var text = await new LipConsoleLoader(ExecutablePath)
                .RunString(LipCommand.Create("show").WithJson() + packageId, tk: tk);
            var json = text.Split('\n').FirstOrDefault(x => x.StartsWith("{"))?.Trim();
            return (json is not null, json is null ? null : JsonConvert.DeserializeObject<LipPackage>(json)!, text);
        }
        public Task<int> InstallPackageAsync(string packageId, CancellationToken tk = default, Action<string>? onOutput = null)
        {
            return new LipConsoleLoader(ExecutablePath)
                .Run(LipCommand.Create("install") + packageId, onOutput, tk);
        }
        public async Task<LipRegistry> GetLipRegistryAsync(CancellationToken tk = default)
        {
            //https://registry.litebds.com/index.json
            using var client = new WebClient();
            var text = await client.DownloadStringTaskAsync("https://registry.litebds.com/index.json");
            if (text is null)
            {
                throw new NullReferenceException("Failed to get registry");
            }
            return JsonConvert.DeserializeObject<LipRegistry>(text)!;
        }
    }
}
