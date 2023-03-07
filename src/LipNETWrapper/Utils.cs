using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LipNETWrapper
{
    public static class Utils
    {
        /// <summary>
        /// Get lip.exe Path
        /// </summary>
        /// <returns></returns>
        public static (bool success, string? path) TryGetLipFromPath()
        {
            var lipPath = Environment.GetEnvironmentVariable("PATH");
            if (lipPath != null)
            {
                var lipPaths = lipPath.Split(';');
                foreach (var lip in lipPaths)
                {
                    var file = Path.Combine(lip, "lip.exe");
                    if (File.Exists(file))
                    {
                        return (true, file);
                    }
                }
            }
            return (false, null);
        }
        public static async Task<byte[]> DownloadLipPortable(Action<DownloadProgressChangedEventArgs> progress)
        {
            using var client = new WebClient();
            void ClientDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) { progress(e); }
            client.DownloadProgressChanged += ClientDownloadProgressChanged;
            // Fetch latest release from GitHub API
            var apiUrl = "https://api.github.com/repos/LiteLDev/Lip/releases/latest";
            client.Headers.Add("Accept", "application/vnd.github.v3+json");
            client.Headers.Add("User-Agent", "request"); // GitHub requires a User-Agent header
            var response = await client.DownloadStringTaskAsync(apiUrl);
            var data = JObject.Parse(response);
            // Find asset with filename ending with "-windows-amd64-setup.exe"
            var targetFilename = "-windows-amd64.zip";
            var assetUrl = "";
            foreach (var asset in data["assets"]!.Value<JArray>()!)
            {
                var assetFilename = asset["name"]!.Value<string>()!;
                if (assetFilename.EndsWith(targetFilename))
                {
                    assetUrl = asset["browser_download_url"]!.Value<string>();
                    break;
                }
            }
            // Download release asset
            try
            {
                return await client.DownloadDataTaskAsync(assetUrl!);
            }
            catch
            {
                //try to download via mirror
                var mirrorUrl = "https://ghproxy.com/" + assetUrl!;
                return await client.DownloadDataTaskAsync(mirrorUrl);
            }
        }
        public static async Task<byte[]> DownloadLipInstaller(Action<DownloadProgressChangedEventArgs> progress)
        {
            using var client = new WebClient();
            void ClientDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) { progress(e); }
            client.DownloadProgressChanged += ClientDownloadProgressChanged;
            // Fetch latest release from GitHub API
            var apiUrl = "https://api.github.com/repos/LiteLDev/Lip/releases/latest";
            client.Headers.Add("Accept", "application/vnd.github.v3+json");
            client.Headers.Add("User-Agent", "request"); // GitHub requires a User-Agent header
            var response = await client.DownloadStringTaskAsync(apiUrl);
            var data = JObject.Parse(response);
            // Find asset with filename ending with "-windows-amd64-setup.exe"
            var targetFilename = "-windows-amd64-setup.exe";
            var assetUrl = "";
            foreach (var asset in data["assets"]!.Value<JArray>()!)
            {
                var assetFilename = asset["name"]!.Value<string>()!;
                if (assetFilename.EndsWith(targetFilename))
                {
                    assetUrl = asset["browser_download_url"]!.Value<string>();
                    break;
                }
            }
            // Download release asset
            try
            {
                return await client.DownloadDataTaskAsync(assetUrl!);
            }
            catch
            {
                //try to download via mirror
                var mirrorUrl = "https://ghproxy.com/" + assetUrl!;
                return await client.DownloadDataTaskAsync(mirrorUrl);
            }
        }
    }
}
