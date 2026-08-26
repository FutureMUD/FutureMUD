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
		Assert.IsTrue(windows.Contains("NT SERVICE\\FutureMUDTerrainPlanner", StringComparison.Ordinal));
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
