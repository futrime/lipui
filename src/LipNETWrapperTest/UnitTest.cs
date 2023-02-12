using System.Text;
using NUnit.Framework.Internal;

namespace LipNETWrapperTest
{
    public class Tests
    {
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
            var sb = new StringBuilder();
            sb.AppendLine(packages.Length.ToString());
            foreach (var item in packages)
            {
                sb.AppendLine("Tooth = " + item.Tooth);
                sb.AppendLine("Version = " + item.Version);
                sb.AppendLine("-----");
            }
            sb.AppendLine(message);
            Assert.Pass(sb.ToString());
        }

        [Test]
        public async Task TestGetPackageInfo()
        {
            //github.com/tooth-hub/liteloaderbds
            var (success, package, message) = await Loader.GetPackageInfoAsync("github.com/tooth-hub/liteloaderbds");
            var sb = new StringBuilder();
            sb.AppendLine(success.ToString());
            sb.AppendLine(package?.Name);
            sb.AppendLine(package?.Version);
            sb.AppendLine("----------");
            sb.AppendLine(message);
            Assert.Pass(sb.ToString());
        }

        [Test]
        public async Task TestGetLocalPackageInfo()
        {
            //github.com/tooth-hub/liteloaderbds
            var (success, package, message) = await Loader.GetLocalPackageInfoAsync("github.com/tooth-hub/liteloaderbds");
            var sb = new StringBuilder();
            sb.AppendLine(success.ToString());
            sb.AppendLine(package?.Name);
            sb.AppendLine(package?.Version);
            sb.AppendLine("----------");
            sb.AppendLine(message);
            Assert.Pass(sb.ToString());
        }

    }
}