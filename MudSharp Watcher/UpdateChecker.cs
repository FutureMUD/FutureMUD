using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp_Watcher
{
	internal class UpdateChecker
	{
		public void StartWatchingForUpdates()
		{
			
		}

		// https://www.labmud.com/downloads/Engine-Version.info
		public async Task CheckForUpdates()
		{
			var client = new HttpClient();
			var response = await client.GetAsync("https://www.labmud.com/downloads/Engine-Version.info");
			if (response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync();
				if (!Version.TryParse(content, out var version))
				{
					return;
				}


			}
		}
	}
}
