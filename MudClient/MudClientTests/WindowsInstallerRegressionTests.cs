namespace MudClientTests;

public class WindowsInstallerRegressionTests
{
	private static string InstallerScript => File.ReadAllText(
		Path.Combine(AppContext.BaseDirectory, "DeploymentScripts", "Install-MudClient.ps1"));
	private static string CommonScriptPath => Path.Combine(
		AppContext.BaseDirectory, "DeploymentScripts", "MudClientDeployment.Common.ps1");

	[Fact]
	public void RollbackGuardsOptionalPathsAndPreservesTheActivationError()
	{
		var script = InstallerScript;

		Assert.Contains("$existingServiceInstaller = if ($previousTarget)", script, StringComparison.Ordinal);
		Assert.Contains("if (-not [string]::IsNullOrWhiteSpace($existingServiceInstaller) -and (Test-Path -LiteralPath $existingServiceInstaller))", script, StringComparison.Ordinal);
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

	private static string EscapePowerShell(string value) => value.Replace("'", "''", StringComparison.Ordinal);
}
