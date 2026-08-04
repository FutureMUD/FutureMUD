#nullable enable

using DatabaseSeeder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;

namespace MudSharp_Unit_Tests;

[TestClass]
public class StartupScriptGeneratorTests
{
	[TestMethod]
	public void BuildWindowsScript_QuotesPathsAppliesStagedPayloadAndBacksOff()
	{
		string script = StartupScriptGenerator.BuildWindowsScript(
			@"C:\Program Files\FutureMUD",
			"server=localhost;database=futuremud;");

		StringAssert.Contains(script, "set \"MUDDIR=C:\\Program Files\\FutureMUD\"");
		StringAssert.Contains(script, "xcopy \"%CODEDIR%\\*\" \"%MUDDIR%\\\" /E /I /H /Y /Q >nul");
		StringAssert.Contains(script, "\"%MUDDIR%\\MudSharp.exe\"");
		StringAssert.Contains(script, "timeout /t 5 /nobreak >nul");
		Assert.IsFalse(script.Contains("%loopcount%0", StringComparison.Ordinal));
	}

	[TestMethod]
	public void BuildLinuxScript_UsesItsOwnDirectoryAndPosixRestartCounter()
	{
		string script = StartupScriptGenerator.BuildLinuxScript("server=localhost;database=futuremud;");

		StringAssert.StartsWith(script, "#!/bin/sh");
		StringAssert.Contains(script, "SCRIPT_DIR=$(CDPATH= cd \"$(dirname \"$0\")\" && pwd)");
		StringAssert.Contains(script, "while [ \"$attempt\" -lt \"$max_attempts\" ]");
		StringAssert.Contains(script, "attempt=$((attempt + 1))");
		StringAssert.Contains(script, "sleep 5");
		StringAssert.Contains(script, "chmod +x \"$SCRIPT_DIR/MudSharp\"");
		Assert.IsFalse(script.Contains("for i in 'seq 1 100'", StringComparison.Ordinal));
	}

	[TestMethod]
	public void EnsureStartScript_ReplacesRecognisedLegacyScript()
	{
		using TemporaryDirectoryHarness harness = new();
		string scriptPath = Path.Combine(harness.DirectoryPath, "Start-MUD.sh");
		File.WriteAllText(scriptPath, """
#!/bin/sh
SERVER_PORT_BASEDIR="."
for i in 'seq 1 100'
$SERVER_PORT_BASEDIR/MudSharp
""");

		StartupScriptGenerationResult result = StartupScriptGenerator.EnsureStartScript(
			harness.DirectoryPath,
			"server=localhost;database=futuremud;",
			isWindows: false);

		Assert.AreEqual(StartupScriptGenerationResult.Updated, result);
		StringAssert.Contains(File.ReadAllText(scriptPath), "FutureMUD generated startup script v2");
	}

	[TestMethod]
	public void EnsureStartScript_PreservesCustomScript()
	{
		using TemporaryDirectoryHarness harness = new();
		string scriptPath = Path.Combine(harness.DirectoryPath, "Start-MUD.bat");
		const string customScript = "@echo off\necho custom launcher\n";
		File.WriteAllText(scriptPath, customScript);

		StartupScriptGenerationResult result = StartupScriptGenerator.EnsureStartScript(
			harness.DirectoryPath,
			"server=localhost;database=futuremud;",
			isWindows: true);

		Assert.AreEqual(StartupScriptGenerationResult.PreservedCustom, result);
		Assert.AreEqual(customScript, File.ReadAllText(scriptPath));
	}

	[TestMethod]
	public void EnsureStartScript_WindowsLauncherHandlesAnInstallationDirectoryWithSpaces()
	{
		if (!OperatingSystem.IsWindows())
		{
			return;
		}

		using TemporaryDirectoryHarness harness = new();
		string installationDirectory = Path.Combine(harness.DirectoryPath, "FutureMUD Installation With Spaces");
		Directory.CreateDirectory(installationDirectory);
		StartupScriptGenerator.EnsureStartScript(
			installationDirectory,
			"server=localhost;database=futuremud;",
			isWindows: true);

		using Process process = Process.Start(new ProcessStartInfo
		{
			FileName = Environment.GetEnvironmentVariable("ComSpec") ?? "cmd.exe",
			Arguments = $"/d /c \"\"{Path.Combine(installationDirectory, "Start-MUD.bat")}\"\"",
			WorkingDirectory = harness.DirectoryPath,
			UseShellExecute = false,
			RedirectStandardOutput = true,
			RedirectStandardError = true
		})!;
		string standardOutput = process.StandardOutput.ReadToEnd();
		string standardError = process.StandardError.ReadToEnd();
		process.WaitForExit();

		Assert.AreEqual(1, process.ExitCode);
		StringAssert.Contains(standardOutput, "Could not find");
		Assert.IsFalse(standardError.Contains("is not recognized", StringComparison.OrdinalIgnoreCase));
	}

	[TestMethod]
	public void EnsureStartScript_LinuxLauncherRunsFromOutsideItsInstallationDirectory()
	{
		if (!OperatingSystem.IsLinux())
		{
			return;
		}

		using TemporaryDirectoryHarness harness = new();
		string enginePath = Path.Combine(harness.DirectoryPath, "MudSharp");
		File.WriteAllText(enginePath, "#!/bin/sh\n: > STOP-REBOOTING\nexit 0\n");
		File.SetUnixFileMode(
			enginePath,
			UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute |
			UnixFileMode.GroupRead | UnixFileMode.GroupExecute |
			UnixFileMode.OtherRead | UnixFileMode.OtherExecute);
		StartupScriptGenerator.EnsureStartScript(
			harness.DirectoryPath,
			"server=localhost;database=futuremud;",
			isWindows: false);
		UnixFileMode launcherMode = File.GetUnixFileMode(Path.Combine(harness.DirectoryPath, "Start-MUD.sh"));
		Assert.IsTrue((launcherMode & UnixFileMode.UserExecute) == UnixFileMode.UserExecute);
		string externalWorkingDirectory = Path.Combine(harness.DirectoryPath, "external");
		Directory.CreateDirectory(externalWorkingDirectory);

		using Process process = Process.Start(new ProcessStartInfo
		{
			FileName = Path.Combine(harness.DirectoryPath, "Start-MUD.sh"),
			WorkingDirectory = externalWorkingDirectory,
			UseShellExecute = false
		})!;
		process.WaitForExit();

		Assert.AreEqual(0, process.ExitCode);
		Assert.IsTrue(File.Exists(Path.Combine(harness.DirectoryPath, "STOP-REBOOTING")));
		Assert.IsFalse(File.Exists(Path.Combine(externalWorkingDirectory, "STOP-REBOOTING")));
	}

	private sealed class TemporaryDirectoryHarness : IDisposable
	{
		public TemporaryDirectoryHarness()
		{
			DirectoryPath = Path.Combine(Path.GetTempPath(), "FutureMUD-Codex", Guid.NewGuid().ToString("N"));
			Directory.CreateDirectory(DirectoryPath);
		}

		public string DirectoryPath { get; }

		public void Dispose()
		{
			if (Directory.Exists(DirectoryPath))
			{
				Directory.Delete(DirectoryPath, recursive: true);
			}
		}
	}
}
