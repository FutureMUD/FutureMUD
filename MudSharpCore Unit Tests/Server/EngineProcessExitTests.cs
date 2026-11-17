#nullable enable

#if !DEBUG
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MudSharp_Unit_Tests.Server;

[TestClass]
[TestCategory("Integration")]
public class EngineProcessExitTests
{
	[TestMethod]
	[DataRow("invalid-connection", 1)]
	[DataRow("startup-file-failure", 1)]
	[DataRow("documentation-export", 0)]
	public async Task ReleaseProcess_ReportsItsActualCompletionStatus(string scenario, int expectedExitCode)
	{
		var directory = Path.Combine(Path.GetTempPath(), "futuremud-exit-" + Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(directory);
		try
		{
			var info = new ProcessStartInfo(Environment.GetEnvironmentVariable("DOTNET_HOST_PATH") ?? "dotnet")
			{
				WorkingDirectory = directory,
				UseShellExecute = false,
				CreateNoWindow = true,
				RedirectStandardOutput = true,
				RedirectStandardError = true
			};
			var testAssembly = typeof(EngineProcessExitTests).Assembly.Location;
			info.ArgumentList.Add("exec");
			info.ArgumentList.Add("--runtimeconfig");
			info.ArgumentList.Add(Path.ChangeExtension(testAssembly, ".runtimeconfig.json"));
			info.ArgumentList.Add("--depsfile");
			info.ArgumentList.Add(Path.ChangeExtension(testAssembly, ".deps.json"));
			info.ArgumentList.Add(typeof(global::MudSharp.Server.MudSharp).Assembly.Location);
			switch (scenario)
			{
				case "invalid-connection":
					await File.WriteAllTextAsync(Path.Combine(directory, "Connection.config"), "invalid-address\n4000\n");
					break;
				case "startup-file-failure":
					Directory.CreateDirectory(Path.Combine(directory, "BOOTING"));
					break;
				case "documentation-export":
					info.ArgumentList.Add("--export-documentation");
					info.ArgumentList.Add(Path.Combine(directory, "catalogue.json"));
					info.ArgumentList.Add("--source-revision");
					info.ArgumentList.Add(new string('a', 40));
					break;
			}

			using var process = Process.Start(info)!;
			var stdout = process.StandardOutput.ReadToEndAsync();
			var stderr = process.StandardError.ReadToEndAsync();
			try
			{
				await process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(30));
			}
			finally
			{
				if (!process.HasExited)
				{
					process.Kill(entireProcessTree: true);
					await process.WaitForExitAsync();
				}
			}

			Assert.AreEqual(expectedExitCode, process.ExitCode, await stdout + "\n" + await stderr);
			if (expectedExitCode != 0)
			{
				Assert.IsTrue(Directory.EnumerateFiles(directory, "Fatal Exception*").Any(),
					"The failed startup must retain its crash report.");
			}
			else
			{
				Assert.IsTrue(File.Exists(Path.Combine(directory, "catalogue.json")));
			}
		}
		finally
		{
			Directory.Delete(directory, recursive: true);
		}
	}
}
#endif
