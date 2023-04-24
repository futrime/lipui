using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
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
        /*  public static class HttpClientExtensions
        {
            public static async Task DownloadAsync(this HttpClient client, string requestUri, Stream destination, IProgress<float> progress = null, CancellationToken cancellationToken = default)
            {
                // Get the http headers first to examine the content length
                using (var response = await client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead))
                {
                    var contentLength = response.Content.Headers.ContentLength;

                    using (var download = await response.Content.ReadAsStreamAsync())
                    {

                        // Ignore progress reporting when no progress reporter was 
                        // passed or when the content length is unknown
                        if (progress == null || !contentLength.HasValue)
                        {
                            await download.CopyToAsync(destination);
                            return;
                        }

                        // Convert absolute progress (bytes downloaded) into relative progress (0% - 100%)
                        var relativeProgress = new Progress<long>(totalBytes => progress.Report((float)totalBytes / contentLength.Value));
                        // Use extension method to report progress while downloading
                        await download.CopyToAsync(destination, 81920, relativeProgress, cancellationToken);
                        progress.Report(1);
                    }
                }
            }
        }*/
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
