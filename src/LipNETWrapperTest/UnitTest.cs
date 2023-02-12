using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using NUnit.Framework.Internal;

namespace LipNETWrapperTest
{
    public class Tests
    {
        private void Sep()
        {
            Console.WriteLine("---");
        }
        private void OutPut(object obj, [CallerArgumentExpression(nameof(obj))] string? expression = null)
        {
            Console.WriteLine(expression + " = " + obj);
        }
        [SetUp]
        public void Setup()
        {
            foreach (var exePath in new[]
            {
                "A:\\Documents\\GitHub\\BDS\\Latest\\lip.exe",
                //put your path here
            })
            {
                if (!File.Exists(exePath)) continue;
                Loader = new LipNETWrapper.LipConsoleWrapper(exePath);
                break;
            }
            if (Loader == null)
                Assert.Fail("please put your lip.exe path");
        }
        LipNETWrapper.LipConsoleWrapper Loader = null;
        [Test]
        public async Task TestLipVersion()
        {
            var result = await Loader.GetLipVersion();
            Assert.Pass(result);
        }
        [Test]
        public async Task TestLipList()
        {
            var (packages, message) = await Loader.GetAllPackagesAsync();
            OutPut(packages.Length.ToString());
            foreach (var item in packages)
            {
                OutPut("Tooth = " + item.Tooth);
                OutPut("Version = " + item.Version);
                OutPut("-----");
            }
            OutPut(message);
        }

        [Test]
        public async Task TestGetPackageInfo()
        {
            //github.com/tooth-hub/liteloaderbds
            var (success, package, message) = await Loader.GetPackageInfoAsync("github.com/tooth-hub/liteloaderbds");
            OutPut(success.ToString());
            OutPut(package?.Name);
            OutPut(package?.Version);
            OutPut("----------");
            OutPut(message);
        }

        [Test]
        public async Task TestGetLocalPackageInfo()
        {
            //github.com/tooth-hub/liteloaderbds
            var (success, package, message) = await Loader.GetLocalPackageInfoAsync("github.com/tooth-hub/liteloaderbds");
            OutPut(success.ToString());
            OutPut(package?.Name);
            OutPut(package?.Version);
            OutPut("----------");
            OutPut(message);
        }
        [Test]
        public async Task TestInstallPackage()
        {
            var result = await Loader.InstallPackageAsync("github.com/tooth-hub/liteloaderbds", onOutput: s =>
            {
                OutPut(s);
            });
            OutPut("exit code : " + result);
        }

        [Test]
        public async Task TestGetLipRegistry()
        {
            var registry = await Loader.GetLipRegistryAsync("https://registry.litebds.com/index.json");
            //output all info
            OutPut(registry.FormatVersion);
            foreach (var x in registry.Index)
            {
                Sep();
                OutPut(x.Key);
                OutPut(x.Value.Tooth);
            }
        }
    }
}