#nullable enable
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LipNETWrapper.Class;
namespace LipNETWrapper;
public interface ILipWrapper
{
    Task<string> GetLipVersion(CancellationToken tk = default);
    Task<(LipPackageSimple[] packages, string message)> GetAllPackagesAsync(CancellationToken tk = default);

    Task<(bool success, LipPackage? package, string message)> GetPackageInfoAsync(string packageId,
        CancellationToken tk = default);

    Task<(bool success, LipPackage? package, string message)> GetLocalPackageInfoAsync(string packageId,
        CancellationToken tk = default);
}
