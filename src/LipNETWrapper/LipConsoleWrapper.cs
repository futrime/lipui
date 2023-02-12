#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
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
                .Run(LipCommand.Create("-V"), tk)).Trim();
        }
        public async Task<(LipPackageSimple[] packages, string message)> GetAllPackagesAsync(CancellationToken tk = default)
        {
            var text = (await new LipConsoleLoader(ExecutablePath)
                .Run(LipCommand.Create("list").WithJson(), tk));
            var json = text.Split('\n').First(x => x.StartsWith("[")).Trim();
            return (JsonConvert.DeserializeObject<LipPackageSimple[]>(json)!, text);
        }
        public async Task<(bool success, LipPackage? package, string message)> GetPackageInfoAsync(string packageId, CancellationToken tk = default)
        {
            var text = await new LipConsoleLoader(ExecutablePath)
                .Run(LipCommand.Create("show").WithJson() + "--available" + packageId, tk);
            var json = text.Split('\n').FirstOrDefault(x => x.StartsWith("{"))?.Trim();
            return (json is not null, json is null ? null : JsonConvert.DeserializeObject<LipPackage>(json)!, text);
        }
        public async Task<(bool success, LipPackage? package, string message)> GetLocalPackageInfoAsync(string packageId, CancellationToken tk = default)
        {
            var text = await new LipConsoleLoader(ExecutablePath)
                .Run(LipCommand.Create("show").WithJson() +  packageId, tk);
            var json = text.Split('\n').FirstOrDefault(x => x.StartsWith("{"))?.Trim();
            return (json is not null, json is null ? null : JsonConvert.DeserializeObject<LipPackage>(json)!, text);
        }
    }
}
