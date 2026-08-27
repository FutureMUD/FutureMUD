namespace TerrainPlanner.Tests;

[TestClass]
public class DeploymentContractTests
{
	private static readonly string RepositoryRoot = FindRepositoryRoot();

	[TestMethod]
	public void LinuxUnitIsHardenedAndUsesTheStableCurrentLink()
	{
		var unit = File.ReadAllText(Path.Combine(RepositoryRoot, "TerrainPlanner", "deploy", "linux",
			"terrainplanner.service"));

		Assert.IsTrue(unit.Contains("Type=simple", StringComparison.Ordinal));
		Assert.IsTrue(unit.Contains("User=terrainplanner", StringComparison.Ordinal));
		Assert.IsTrue(unit.Contains("ProtectSystem=strict", StringComparison.Ordinal));
		Assert.IsTrue(unit.Contains("/opt/futuremud/terrainplanner/current/app/TerrainPlanner", StringComparison.Ordinal));
	}

	[TestMethod]
	public void InstallersKeepSecretsAndKeysOutsideReleaseDirectories()
	{
		var template = File.ReadAllText(Path.Combine(RepositoryRoot, "TerrainPlanner", "deploy",
			"appsettings.Production.template.json"));
		var linux = File.ReadAllText(Path.Combine(RepositoryRoot, "TerrainPlanner", "deploy", "linux",
			"install-terrainplanner.sh"));
		var windows = File.ReadAllText(Path.Combine(RepositoryRoot, "TerrainPlanner", "deploy", "windows",
			"Install-TerrainPlanner.ps1"));

		Assert.IsTrue(template.Contains("REPLACE_WITH_SECRET", StringComparison.Ordinal));
		Assert.IsTrue(template.Contains("REPLACE_WITH_DURABLE_KEY_PATH", StringComparison.Ordinal));
		Assert.IsTrue(linux.Contains("/etc/futuremud/terrainplanner", StringComparison.Ordinal));
		Assert.IsTrue(linux.Contains("$DATA_ROOT/keys", StringComparison.Ordinal));
		Assert.IsTrue(windows.Contains("$sharedRoot", StringComparison.Ordinal));
		Assert.IsTrue(windows.Contains("appsettings.Production.json", StringComparison.Ordinal));
		Assert.IsTrue(windows.Contains("NT AUTHORITY\\LocalService", StringComparison.Ordinal));
		Assert.IsTrue(windows.Contains("sidtype FutureMUDTerrainPlanner unrestricted", StringComparison.Ordinal));
		Assert.IsTrue(windows.IndexOf("if (-not (Test-Path -LiteralPath $configPath))", StringComparison.Ordinal) <
			windows.IndexOf("New-Item -ItemType Directory -Path $releaseRoot", StringComparison.Ordinal));
	}

	[TestMethod]
	public void CaddyInstallersBackUpAndRestoreBothChangedFiles()
	{
		var linux = File.ReadAllText(Path.Combine(RepositoryRoot, "TerrainPlanner", "deploy", "linux",
			"install-caddy-site.sh"));
		var windows = File.ReadAllText(Path.Combine(RepositoryRoot, "TerrainPlanner", "deploy", "windows",
			"Install-CaddySite.ps1"));

		Assert.IsTrue(linux.Contains("SITE_BACKUP", StringComparison.Ordinal));
		Assert.IsTrue(linux.Contains("caddy validate", StringComparison.Ordinal));
		Assert.IsTrue(windows.Contains("$siteBackup", StringComparison.Ordinal));
		Assert.IsTrue(windows.Contains("validate --config", StringComparison.Ordinal));
		Assert.IsTrue(windows.Contains("Get-ScheduledTask -TaskName 'FutureMUD Web Client HTTPS'", StringComparison.Ordinal));
		Assert.IsTrue(windows.Contains("--adapter caddyfile", StringComparison.Ordinal));
		Assert.IsTrue(windows.Contains("Caddy already contains a site block", StringComparison.Ordinal));
		Assert.IsTrue(windows.Contains("$main -match \"(?m)^\\s*$escapedHostname\\s*\\{\"", StringComparison.Ordinal));
		Assert.IsFalse(windows.Contains("$main -match \"(?m)^\\\\s*$escapedHostname\\\\s*\\\\{\"", StringComparison.Ordinal));
	}

	[TestMethod]
	public void WindowsUpdaterDownloadsTheVersionedArchiveEndpoint()
	{
		var updater = File.ReadAllText(Path.Combine(RepositoryRoot, "TerrainPlanner", "deploy", "windows",
			"Update-TerrainPlanner.ps1"));

		Assert.IsTrue(updater.Contains("$latestBase/update-manifest.json", StringComparison.Ordinal));
		Assert.IsTrue(updater.Contains("downloads/terrainplanner/$($manifest.version)/$archiveName", StringComparison.Ordinal));
		Assert.IsFalse(updater.Contains("$latestBase/$archiveName", StringComparison.Ordinal));
	}

	[TestMethod]
	public void WindowsInstallerUsesPowerShellFiveCompatibleJunctionRollbackAndStableServiceCommand()
	{
		var installer = File.ReadAllText(Path.Combine(RepositoryRoot, "TerrainPlanner", "deploy", "windows",
			"Install-TerrainPlanner.ps1"));

		Assert.IsTrue(installer.Contains("[IO.Directory]::Delete($Path)", StringComparison.Ordinal));
		Assert.IsTrue(installer.Contains("Refusing to remove '$Path' because it is not a release junction.", StringComparison.Ordinal));
		Assert.IsTrue(installer.Contains("$targets = @($item.Target)", StringComparison.Ordinal));
		Assert.IsTrue(installer.Contains("return [string]$targets[0]", StringComparison.Ordinal));
		Assert.IsTrue(installer.Contains("New-Item -ItemType Junction -Path $currentPath -Target $previousTarget", StringComparison.Ordinal));
		Assert.IsTrue(installer.Contains("if ($existingService.Status -ne 'Stopped')", StringComparison.Ordinal));
		Assert.IsTrue(installer.Contains("New-Service -Name FutureMUDTerrainPlanner -BinaryPathName $serviceCommand", StringComparison.Ordinal));
		Assert.IsTrue(installer.Contains("Terrain Planner activation failed; the previous release was restored.", StringComparison.Ordinal));
		Assert.IsFalse(installer.Contains("ArgumentList", StringComparison.Ordinal));
		Assert.IsFalse(installer.Contains("Invoke-ServiceControl", StringComparison.Ordinal));
		Assert.IsFalse(installer.Contains("$previousTarget = $currentItem.Target", StringComparison.Ordinal));
		Assert.IsFalse(installer.Contains("sc.exe config FutureMUDTerrainPlanner binPath=", StringComparison.Ordinal));
	}

	private static string FindRepositoryRoot()
	{
		var directory = new DirectoryInfo(AppContext.BaseDirectory);
		while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "MudSharp.sln")))
		{
			directory = directory.Parent;
		}

		return directory?.FullName ?? throw new DirectoryNotFoundException("Could not locate the FutureMUD repository root.");
	}
}
