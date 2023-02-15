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
        public LipConsoleWrapper(string executablePath = "lip.exe", string? workingDir = null)
        {
            ExecutablePath = executablePath;
            WorkingPath = workingDir;
        }
        public string ExecutablePath { get; set; }
        public string? WorkingPath { get; set; }
        public async Task<string> GetLipVersion(CancellationToken tk = default)
        {
            return (await new LipConsoleLoader(ExecutablePath, WorkingPath)
                .RunString(LipCommand.Create("-V"), tk: tk)).Trim();
        }
        public async Task<(LipPackageSimple[] packages, string message)> GetAllPackagesAsync(CancellationToken tk = default)
        {
            try
            {
                var text = (await new LipConsoleLoader(ExecutablePath, WorkingPath)
                    .RunString(LipCommand.Create("list", true).WithJson(), tk: tk));
                var json = text.Split('\n').First(x => x.StartsWith("[")).Trim();
                var arr = JsonConvert.DeserializeObject<LipPackageSimple[]>(json);
                return (arr ?? Array.Empty<LipPackageSimple>(), text);
            }
            catch 
            { //retry once
                var text = (await new LipConsoleLoader(ExecutablePath, WorkingPath)
                    .RunString(LipCommand.Create("list", true).WithJson(), tk: tk));
                var json = text.Split('\n').First(x => x.StartsWith("[")).Trim();
                var arr = JsonConvert.DeserializeObject<LipPackageSimple[]>(json);
                return (arr ?? Array.Empty<LipPackageSimple>(), text);
            }
        }
        public async Task<(bool success, LipPackageVersions? package, string message)> GetPackageInfoAsync(string packageId, CancellationToken tk = default, Action<string>? onOutput = null)
        {
            var text = await new LipConsoleLoader(ExecutablePath, WorkingPath)
                .RunString(LipCommand.Create("show", onOutput is null).WithJson() + "--available" + packageId, onOutput, tk);
            var json = text.Split('\n').FirstOrDefault(x => x.StartsWith("{"))?.Trim();
            var obj = json is null ? null : JsonConvert.DeserializeObject<LipPackage>(json);
            return (obj is not null, obj, text);
        }
        public async Task<(bool success, LipPackage? package, string message)> GetLocalPackageInfoAsync(string packageId, CancellationToken tk = default)
        {
            var text = await new LipConsoleLoader(ExecutablePath, WorkingPath)
                .RunString(LipCommand.Create("show", true).WithJson() + packageId, tk: tk);
            var json = text.Split('\n').FirstOrDefault(x => x.StartsWith("{"))?.Trim();
            var obj = json is null ? null : JsonConvert.DeserializeObject<LipPackage>(json);
            return (obj is not null, obj, text);
        }
        public Task<int> InstallPackageAsync(string packageId, CancellationToken tk = default, Action<string, Action<string>>? onOutput = null)
        {
            return new LipConsoleLoader(ExecutablePath, WorkingPath)
                .RunWithInput(LipCommand.Create("install") + "-y" + "--numeric-progress" + packageId, onOutput, tk);
        }

        public Task<int> UninstallPackageAsync(string packageId, CancellationToken tk = default, Action<string>? onOutput = null)
        {
            return new LipConsoleLoader(ExecutablePath, WorkingPath)
                .Run(LipCommand.Create("uninstall") + packageId, onOutput, tk);
        }
        public async Task<LipRegistry> GetLipRegistryAsync(string registry, CancellationToken tk = default)
        {
#if NET7_0 || NETCOREAPP
            var client = new System.Net.Http.HttpClient();
            var response = await client.GetAsync(registry);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to get registry: " + response.StatusCode);
            }
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<LipRegistry>(content)!;
#else
            using var client = new WebClient();
            var text = await client.DownloadStringTaskAsync(registry);
            if (text is null)
            {
                throw new NullReferenceException("Failed to get registry : " + registry);
            }
            return JsonConvert.DeserializeObject<LipRegistry>(text)!;
#endif
        }
        public Task CachePurge()
        {
            return new LipConsoleLoader(ExecutablePath, WorkingPath)
                .Run(LipCommand.Create("cache") + "purge", null, CancellationToken.None);
        }
    }
}
