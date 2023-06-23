#nullable enable
using System;
using System.Diagnostics;
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
        public async Task<(LipPackage[] packages, string message)> GetAllPackagesAsync(CancellationToken tk = default)
        {
            async Task<(LipPackage[] packages, string message)> GetInternal()
            {
                var json = await new LipConsoleLoader(ExecutablePath, WorkingPath)
                    .RunString(LipCommand.Create("list", true).WithJson(), tk: tk);
                Debug.WriteLine(json);
                var arr = JsonConvert.DeserializeObject<LipPackage[]>(json);
                return (arr ?? Array.Empty<LipPackage>(), json);
            }
            try
            {
                return await GetInternal();
            }
            catch
            { //retry once
                return await GetInternal();
            }
        }
        public async Task<(bool success, LipPackageVersions? package, string message)> GetPackageInfoAsync(string packageId, CancellationToken tk = default, Action<string>? onOutput = null)
        {
            var json = await new LipConsoleLoader(ExecutablePath, WorkingPath)
                .RunString(LipCommand.Create("show", onOutput is null).WithJson() + "--available" + packageId, onOutput, tk);
            var obj = JsonConvert.DeserializeObject<LipPackageItem>(json)?.AvailableVersions;
            return (obj is not null, obj, json);
        }
        public async Task<(bool success, LipPackage? package, string message)> GetLocalPackageInfoAsync(string packageId, CancellationToken tk = default)
        {
            var json = await new LipConsoleLoader(ExecutablePath, WorkingPath)
                .RunString(LipCommand.Create("show", true).WithJson() + packageId, tk: tk);
            var obj = JsonConvert.DeserializeObject<LipPackageItem>(json)?.Package;
            return (obj is not null, obj, json);
        }
        public Task<int> InstallPackageAsync(string packageId, bool upgrade = false, bool skipDependency = false, CancellationToken tk = default, Action<string, Action<string>>? onOutput = null)
        {
            var cmd = LipCommand.Create("install") + "-y" /*+ "--numeric-progress"*/;
            if (upgrade)
                cmd += "--upgrade";
            if (skipDependency)
                cmd += "--no-dependencies";
            //todo 强制重装
            //if (forceReinstall)
            //    cmd += "--force-reinstall";
            return new LipConsoleLoader(ExecutablePath, WorkingPath)
                .RunWithInput(cmd + packageId, onOutput, tk);
        }

        public Task<int> UninstallPackageAsync(string packageId, CancellationToken tk = default, Action<string>? onOutput = null)
        {
            return new LipConsoleLoader(ExecutablePath, WorkingPath)
                .Run(LipCommand.Create("uninstall") + "-y" + packageId, onOutput, tk);
        }
        public async Task<LipRegistry> GetLipRegistryAsync(string registry, CancellationToken tk = default)
        {
#if NET7_0 || NETCOREAPP
            var client = new System.Net.Http.HttpClient() { };
            var response = await client.GetAsync(registry, tk);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to get registry: " + response.StatusCode);
            }
            var content = await response.Content.ReadAsStringAsync(tk);
            return JsonConvert.DeserializeObject<LipRegistry>(content)!;
#else
            using var client = new WebClient() { Encoding = Encoding.UTF8 };
            var text = await client.DownloadStringTaskAsync(registry);
            if (text is null)
            {
                throw new NullReferenceException("Failed to get registry : " + registry);
            }
            return JsonConvert.DeserializeObject<LipRegistry>(text)!;
#endif
        }
        public Task  CachePurge()
        {
            return new LipConsoleLoader(ExecutablePath, WorkingPath)
                .Run(LipCommand.Create("cache") + "purge", null, CancellationToken.None);
        }
    }
}
