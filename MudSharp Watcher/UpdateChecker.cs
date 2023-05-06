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
			using (var fs = new FileStream("Engine-Version.info", FileMode.Open))
			{
				var reader = new StreamReader(fs);
				var versionText = await reader.ReadToEndAsync();
				if (Version.TryParse(versionText, out var version))
				{
					return version;
				}
			}

			return new Version(0, 0, 0, 0);
		}

		public async Task WriteCurrentVersion(Version version)
		{
			using (var fs = new FileStream("Engine-Version.info", FileMode.Create))
			{
				var writer = new StreamWriter(fs);
				await writer.WriteLineAsync(version.ToString());
				await writer.FlushAsync();
			}
		}

		// https://www.labmud.com/downloads/Engine-Version.info
		public async Task CheckForUpdates()
		{
			var client = new HttpClient();
			client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
			{
				NoCache = true,
				MaxAge = TimeSpan.FromSeconds(10),
				MustRevalidate = true
			};
			var response = await client.GetAsync("https://www.labmud.com/downloads/Engine-Version.info");
			if (!response.IsSuccessStatusCode)
			{
				return;
			}

			var content = await response.Content.ReadAsStringAsync();
			if (!Version.TryParse(content, out var version))
			{
				return;
			}

			var current = await CurrentVersion();
			if (version > current)
			{
				response = await client.GetAsync("http://www.labmud.com/downloads/FutureMUD-Windows.zip");
				if (!response.IsSuccessStatusCode)
				{
					return;
				}

				using (var fs = new FileStream("FutureMUD-Windows.zip", FileMode.Create))
				{
					var ms = await response.Content.ReadAsStreamAsync();
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
