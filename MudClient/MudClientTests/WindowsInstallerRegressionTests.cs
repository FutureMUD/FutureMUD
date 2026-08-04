namespace MudClientTests;

public class WindowsInstallerRegressionTests
{
	private static string InstallerScript => File.ReadAllText(
		Path.Combine(AppContext.BaseDirectory, "DeploymentScripts", "Install-MudClient.ps1"));
	private static string UpdaterScript => File.ReadAllText(
		Path.Combine(AppContext.BaseDirectory, "DeploymentScripts", "Update-MudClient.ps1"));
	private static string CommonScriptPath => Path.Combine(
		AppContext.BaseDirectory, "DeploymentScripts", "MudClientDeployment.Common.ps1");
	private static string ProxyProgram => File.ReadAllText(
		Path.Combine(AppContext.BaseDirectory, "SourceFiles", "MudWebSocketProxy.Program.cs"));
	private static string ProxyServiceInstaller => File.ReadAllText(
		Path.Combine(AppContext.BaseDirectory, "DeploymentScripts", "install-mudclient-proxy.ps1"));

	[Fact]
	public void RollbackGuardsOptionalPathsAndPreservesTheActivationError()
	{
		var script = InstallerScript;

		Assert.Contains("$existingServiceInstaller = if ($previousTarget)", script, StringComparison.Ordinal);
		Assert.Contains("if ($existingService -and -not [string]::IsNullOrWhiteSpace($existingServiceInstaller) -and (Test-Path -LiteralPath $existingServiceInstaller))", script, StringComparison.Ordinal);
		Assert.Contains("Invoke-MudClientRollbackStep -Description 'Restoring the previous MudClientProxy service'", script, StringComparison.Ordinal);
		Assert.Contains("throw $activationError", script, StringComparison.Ordinal);
	}

	[Fact]
	public void MigrationRecognisesAndCompletesAnInterruptedLegacyMove()
	{
		var script = File.ReadAllText(CommonScriptPath);

		Assert.Contains("$resumableLegacyPath = $existingLegacyPaths", script, StringComparison.Ordinal);
		Assert.Contains("($hasFlatProxy -or $hasLegacyProxy)", script, StringComparison.Ordinal);
		Assert.Contains("($hasFlatWeb -or $hasLegacyWeb)", script, StringComparison.Ordinal);
		Assert.Contains("The legacy installation is incomplete.", script, StringComparison.Ordinal);
	}

	[Fact]
	public void MigrationResumesAProxyMoveWhenTheWebFolderWasAlreadyPreserved()
	{
		if (!OperatingSystem.IsWindows()) { return; }

		var testRoot = Path.Combine(Path.GetTempPath(), $"mudclient-migration-{Guid.NewGuid():N}");
		var installRoot = Path.Combine(testRoot, "install");
		var releaseRoot = Path.Combine(installRoot, "releases");
		var legacyRoot = Path.Combine(releaseRoot, "legacy-20260804000000000");
		Directory.CreateDirectory(Path.Combine(installRoot, "proxy"));
		Directory.CreateDirectory(Path.Combine(legacyRoot, "web"));
		File.WriteAllText(Path.Combine(installRoot, "proxy", "appsettings.json"), "proxy-settings");
		File.WriteAllText(Path.Combine(legacyRoot, "web", "marker.txt"), "legacy-web");

		try
		{
			var command = $". '{EscapePowerShell(CommonScriptPath)}'; " +
				$"$result = Move-MudClientLegacyInstallation -InstallRoot '{EscapePowerShell(installRoot)}' -ReleaseRoot '{EscapePowerShell(releaseRoot)}' -Migrate; " +
				"Write-Output $result";
			var startInfo = new System.Diagnostics.ProcessStartInfo("powershell.exe")
			{
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false
			};
			startInfo.ArgumentList.Add("-NoProfile");
			startInfo.ArgumentList.Add("-Command");
			startInfo.ArgumentList.Add(command);
			using var process = System.Diagnostics.Process.Start(startInfo)!;
			var output = process.StandardOutput.ReadToEnd().Trim();
			var error = process.StandardError.ReadToEnd();
			process.WaitForExit();

			Assert.True(process.ExitCode == 0, error);
			Assert.Equal(legacyRoot, output, ignoreCase: true);
			Assert.False(Directory.Exists(Path.Combine(installRoot, "proxy")));
			Assert.Equal("proxy-settings", File.ReadAllText(Path.Combine(legacyRoot, "proxy", "appsettings.json")));
			Assert.Equal("legacy-web", File.ReadAllText(Path.Combine(legacyRoot, "web", "marker.txt")));
		}
		finally
		{
			Directory.Delete(testRoot, recursive: true);
		}
	}

	[Fact]
	public void InstallerRestagesAnInactiveReleaseLeftByAFailedAttempt()
	{
		var script = InstallerScript;

		Assert.Contains("throw \"Release $version is already active.\"", script, StringComparison.Ordinal);
		Assert.Contains("Remove-MudClientPath -Path $releasePath", script, StringComparison.Ordinal);
		Assert.DoesNotContain("throw \"Release $version is already staged.\"", script, StringComparison.Ordinal);
	}

	[Fact]
	public void UpdaterCapturesManifestVerificationOutputBeforeReturningTheManifestPath()
	{
		var script = UpdaterScript;

		Assert.Contains("$verificationOutput = & $deploymentTool verify-manifest", script, StringComparison.Ordinal);
		Assert.Contains("$verificationOutput | ForEach-Object { Write-Host $_ }", script, StringComparison.Ordinal);
	}

	[Fact]
	public void InstallerStopsTheLegacyProxyBeforeMovingItsDirectory()
	{
		var script = InstallerScript;
		var findLegacyTask = script.IndexOf("$legacyTask = Get-ScheduledTask", StringComparison.Ordinal);
		var stopLegacyTask = script.IndexOf("Stop-ScheduledTask -TaskName $legacyTaskName", StringComparison.Ordinal);
		var stopLegacyProcess = script.IndexOf("Stop-MudClientLegacyProxyProcess -ProxyRoot $legacyProxy", StringComparison.Ordinal);
		var moveLegacyInstallation = script.IndexOf("$legacyReleasePath = Move-MudClientLegacyInstallation", StringComparison.Ordinal);

		Assert.True(findLegacyTask >= 0);
		Assert.True(stopLegacyTask > findLegacyTask);
		Assert.True(stopLegacyProcess > stopLegacyTask);
		Assert.True(moveLegacyInstallation > stopLegacyProcess);
		Assert.Contains("if ($legacyTaskWasRunning) { Start-ScheduledTask -TaskName $legacyTaskName }", script, StringComparison.Ordinal);
		Assert.Contains("Register-ScheduledTask -TaskName $legacyTaskName -Xml $legacyTaskXml -Force", script, StringComparison.Ordinal);
		Assert.DoesNotContain("schtasks.exe /Create", script, StringComparison.OrdinalIgnoreCase);
	}

	[Fact]
	public void InstallerPreservesDeploymentBrandingAssetsAcrossReleases()
	{
		var script = InstallerScript;

		Assert.Contains("$customWebAssets = Join-Path $ConfigRoot 'web\\custom'", script, StringComparison.Ordinal);
		Assert.Contains("Get-ChildItem -LiteralPath $customWebAssets -Force | Copy-Item", script, StringComparison.Ordinal);
		Assert.Contains("$legacyCustomAssets = Join-Path $legacyReleasePath 'web\\wwwroot\\custom'", script, StringComparison.Ordinal);
	}

	[Fact]
	public void ProxyConfiguresTheWindowsServiceLifetimeAndEventLog()
	{
		var program = ProxyProgram;

		Assert.Contains("builder.Services.AddWindowsService", program, StringComparison.Ordinal);
		Assert.Contains("options.ServiceName = \"MudClientProxy\"", program, StringComparison.Ordinal);
		Assert.Contains("logging.AddEventLog", program, StringComparison.Ordinal);
		Assert.Contains("validateSettingsOnly", program, StringComparison.Ordinal);
		Assert.Contains("MudClient proxy settings are valid.", program, StringComparison.Ordinal);
	}

	[Fact]
	public void ServiceInstallerValidatesSettingsBeforeRegisteringTheService()
	{
		var script = ProxyServiceInstaller;
		var validation = script.IndexOf("--validate-settings true", StringComparison.Ordinal);
		var registration = script.IndexOf("New-Service", StringComparison.Ordinal);

		Assert.True(validation >= 0);
		Assert.True(registration > validation);
		Assert.Contains("Windows Application event log", script, StringComparison.Ordinal);
	}

	[Fact]
	public void LegacyProxyStopHelperOnlyTargetsTheLegacyProxyDirectory()
	{
		var script = File.ReadAllText(CommonScriptPath);

		Assert.Contains("Get-Process -Name 'MudWebSocketProxy'", script, StringComparison.Ordinal);
		Assert.Contains("[System.StringComparison]::OrdinalIgnoreCase", script, StringComparison.Ordinal);
		Assert.Contains("$legacyProcesses | Stop-Process -Force", script, StringComparison.Ordinal);
		Assert.Contains("The legacy MudWebSocketProxy process did not release", script, StringComparison.Ordinal);
	}

	[Fact]
	public void LegacyProxyStopHelperTerminatesOnlyAProxyUnderTheMigratedDirectory()
	{
		if (!OperatingSystem.IsWindows()) { return; }

		var testRoot = Path.Combine(Path.GetTempPath(), $"mudclient-proxy-stop-{Guid.NewGuid():N}");
		var legacyProxyRoot = Path.Combine(testRoot, "install", "proxy");
		var unrelatedProxyRoot = Path.Combine(testRoot, "unrelated", "proxy");
		Directory.CreateDirectory(legacyProxyRoot);
		Directory.CreateDirectory(unrelatedProxyRoot);
		var commandProcessor = Environment.GetEnvironmentVariable("ComSpec") ?? Path.Combine(Environment.SystemDirectory, "cmd.exe");
		var legacyExecutable = Path.Combine(legacyProxyRoot, "MudWebSocketProxy.exe");
		var unrelatedExecutable = Path.Combine(unrelatedProxyRoot, "MudWebSocketProxy.exe");
		File.Copy(commandProcessor, legacyExecutable);
		File.Copy(commandProcessor, unrelatedExecutable);
		using var legacyProcess = StartWaitingProcess(legacyExecutable);
		using var unrelatedProcess = StartWaitingProcess(unrelatedExecutable);

		try
		{
			var command = $". '{EscapePowerShell(CommonScriptPath)}'; " +
				$"Stop-MudClientLegacyProxyProcess -ProxyRoot '{EscapePowerShell(legacyProxyRoot)}' -TimeoutSeconds 10";
			var startInfo = new System.Diagnostics.ProcessStartInfo("powershell.exe")
			{
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false
			};
			startInfo.ArgumentList.Add("-NoProfile");
			startInfo.ArgumentList.Add("-Command");
			startInfo.ArgumentList.Add(command);
			using var helperProcess = System.Diagnostics.Process.Start(startInfo)!;
			var error = helperProcess.StandardError.ReadToEnd();
			helperProcess.WaitForExit();

			Assert.True(helperProcess.ExitCode == 0, error);
			Assert.True(legacyProcess.WaitForExit(5_000));
			Assert.False(unrelatedProcess.HasExited);
		}
		finally
		{
			StopProcess(unrelatedProcess);
			StopProcess(legacyProcess);
			Directory.Delete(testRoot, recursive: true);
		}
	}

	private static System.Diagnostics.Process StartWaitingProcess(string executable)
	{
		var startInfo = new System.Diagnostics.ProcessStartInfo(executable)
		{
			CreateNoWindow = true,
			RedirectStandardInput = true,
			RedirectStandardOutput = true,
			RedirectStandardError = true,
			UseShellExecute = false
		};
		startInfo.ArgumentList.Add("/d");
		startInfo.ArgumentList.Add("/q");
		return System.Diagnostics.Process.Start(startInfo)!;
	}

	private static void StopProcess(System.Diagnostics.Process process)
	{
		if (process.HasExited) { return; }
		process.Kill(entireProcessTree: true);
		process.WaitForExit();
	}

	private static string EscapePowerShell(string value) => value.Replace("'", "''", StringComparison.Ordinal);
}
