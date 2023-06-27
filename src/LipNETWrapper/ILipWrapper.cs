#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;
using LipNETWrapper.Class;
namespace LipNETWrapper;
public interface ILipWrapper
{
    string ExecutablePath { get; set; }
    string? WorkingPath { get; set; }
    Task<string> GetLipVersion(CancellationToken tk = default);
    Task<(LipPackage[] packages, string message)> GetAllPackagesAsync(CancellationToken tk = default);
    Task<(bool success, LipPackageVersions? package, string message)> GetPackageInfoAsync(string packageId,
        CancellationToken tk = default, Action<string>? onOutput = null);
    Task<(bool success, LipPackage? package, string message)> GetLocalPackageInfoAsync(string packageId,
        CancellationToken tk = default);
    Task<int> InstallPackageAsync(string packageId, bool upgrade = false, bool skipDependency = false,
        CancellationToken tk = default, Action<string, Action<string>>? onOutput = null);
    Task<int> UninstallPackageAsync(string packageId,
        CancellationToken tk = default, Action<string>? onOutput = null);
    Task<LipRegistry> GetLipRegistryAsync(string registry, CancellationToken tk = default);
    Task CachePurge();
}
