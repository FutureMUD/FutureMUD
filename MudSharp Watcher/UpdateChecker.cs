using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MudSharp_Watcher
{
    internal class UpdateChecker
    {
        public event EventHandler? UpdateAvailable;

        public void StartWatchingForUpdates()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    await CheckForUpdates();
                    Thread.Sleep(TimeSpan.FromSeconds(30));
                }
            });
        }

        public async Task<Version> CurrentVersion()
        {
            if (!File.Exists("Engine-Version.info"))
            {
                return new Version(0, 0, 0, 0);
            }
            using (FileStream fs = new("Engine-Version.info", FileMode.Open))
            {
                StreamReader reader = new(fs);
                string versionText = await reader.ReadToEndAsync();
                if (Version.TryParse(versionText, out Version? version))
                {
                    return version;
                }
            }

            return new Version(0, 0, 0, 0);
        }

        public async Task WriteCurrentVersion(Version version)
        {
            using (FileStream fs = new("Engine-Version.info", FileMode.Create))
            {
                StreamWriter writer = new(fs);
                await writer.WriteLineAsync(version.ToString());
                await writer.FlushAsync();
            }
        }

        // https://www.labmud.com/downloads/Engine-Version.info
        public async Task CheckForUpdates()
        {
            HttpClient client = new();
            client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
            {
                NoCache = true,
                MaxAge = TimeSpan.FromSeconds(10),
                MustRevalidate = true
            };
            HttpResponseMessage response = await client.GetAsync("https://www.labmud.com/downloads/Engine-Version.info");
            if (!response.IsSuccessStatusCode)
            {
                return;
            }

            string content = await response.Content.ReadAsStringAsync();
            if (!Version.TryParse(content, out Version? version))
            {
                return;
            }

            Version current = await CurrentVersion();
            if (version > current)
            {
                response = await client.GetAsync("http://www.labmud.com/downloads/FutureMUD-Windows.zip");
                if (!response.IsSuccessStatusCode)
                {
                    return;
                }

                using (FileStream fs = new("FutureMUD-Windows.zip", FileMode.Create))
                {
                    Stream ms = await response.Content.ReadAsStreamAsync();
                    ms.Seek(0, SeekOrigin.Begin);
                    await ms.CopyToAsync(fs);
                }

                System.IO.Compression.ZipFile.ExtractToDirectory("FutureMUD-Windows.zip", "Binaries", true);
                UpdateAvailable?.Invoke(this, EventArgs.Empty);
                await WriteCurrentVersion(version);
            }
        }
    }
}
