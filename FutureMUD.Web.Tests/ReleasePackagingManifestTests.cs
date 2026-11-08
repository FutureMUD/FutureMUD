#nullable enable

using FutureMUD.Web.Publishing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;

namespace FutureMUD.Web.Tests;

[TestClass]
public class ReleasePackagingManifestTests
{
	[TestMethod]
	public async Task StableProducts_UseFrameworkDependentSingleFilePackaging()
	{
		var path = Path.Combine(AppContext.BaseDirectory, "Configuration", "release-products.json");
		await using var stream = File.OpenRead(path);
		var manifest = await JsonSerializer.DeserializeAsync<ReleaseProductManifest>(
			stream,
			new JsonSerializerOptions(JsonSerializerDefaults.Web));

		Assert.IsNotNull(manifest);
		Assert.AreEqual(7, manifest.Products.Count);
		foreach (var product in manifest.Products.Where(product => product.Id is not ("mudclient" or "terrainplanner")))
		{
			Assert.AreEqual("single-file", product.PackageKind, product.Id);
			Assert.IsTrue(product.FrameworkDependent, product.Id);
			Assert.IsTrue(product.SingleFile, product.Id);
			Assert.IsTrue(product.IncludeNativeLibrariesForSelfExtract, product.Id);
		}

		var mudClient = manifest.Products.Single(product => product.Id == "mudclient");
		Assert.AreEqual("mudclient", mudClient.PackageKind);
		Assert.IsFalse(mudClient.FrameworkDependent);
		Assert.IsTrue(mudClient.SingleFile);
		Assert.IsTrue(mudClient.IncludeNativeLibrariesForSelfExtract);
		Assert.AreEqual("MudClient/MudClientBlazor/MudClientBlazor.csproj", mudClient.WebProjectPath);
		Assert.AreEqual("MudClient/scripts/Publish-ProductPackage.ps1", mudClient.PackageScriptPath);

		var terrainPlanner = manifest.Products.Single(product => product.Id == "terrainplanner");
		Assert.AreEqual("terrainplanner", terrainPlanner.PackageKind);
		Assert.IsFalse(terrainPlanner.FrameworkDependent);
		Assert.AreEqual(3, terrainPlanner.Runtimes.Count);
		Assert.AreEqual("TerrainPlanner/scripts/Publish-ProductPackage.ps1", terrainPlanner.PackageScriptPath);
		Assert.IsTrue(manifest.Products.Single(product => product.Id == "terrainapi").Retired);

		var itemSeeder = manifest.Products.Single(product => product.Id == "itemseeder");
		Assert.AreEqual("FutureMUD Item Seeder", itemSeeder.PublicName);
		Assert.AreEqual("itemseeder-v", itemSeeder.TagPrefix);
		Assert.AreEqual("ItemSeeder/ItemSeeder.csproj", itemSeeder.ProjectPath);
		Assert.AreEqual("itemseeder-{version}-{runtime}.zip", itemSeeder.ArchiveName);
		CollectionAssert.AreEqual(new[] { "win-x64", "linux-x64", "linux-arm64" }, itemSeeder.Runtimes.ToArray());
	}

	[TestMethod]
	public void PublishWorkflow_AllowsRetiredTerrainApiManifestEntryWithoutPublishingIt()
	{
		var repositoryRoot = LocateRepositoryRoot();
		var workflow = File.ReadAllText(Path.Combine(repositoryRoot.FullName, ".github", "workflows", "publish-products.yml"));

		StringAssert.Contains(
			workflow,
			"$allowedIds = [string[]]@('engine', 'seeder', 'itemseeder', 'discordbot', 'terrainplanner', 'terrainapi', 'mudclient')");
		StringAssert.Contains(workflow, "TagPrefix = 'itemseeder-v'");
		StringAssert.Contains(workflow, "itemseeder-{version}-{runtime}.zip");
		StringAssert.Contains(workflow, "$legacyAllowedIds = [string[]]@('engine', 'seeder', 'discordbot', 'terrainplanner', 'terrainapi', 'mudclient')");
		StringAssert.Contains(workflow, "-not (Test-Path -LiteralPath 'ItemSeeder/ItemSeeder.csproj' -PathType Leaf)");
		StringAssert.Contains(workflow, "terrainapi = [pscustomobject]@{");
		StringAssert.Contains(workflow, "Retired = $true");
		var publishingTagRegex = workflow
			.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
			.Single(line => line.Contains(@"\A(?<product>", StringComparison.Ordinal));
		Assert.IsFalse(publishingTagRegex.Contains("terrainapi", StringComparison.Ordinal));
		StringAssert.Contains(publishingTagRegex, "itemseeder");

		var historicalBackfill = File.ReadAllText(Path.Combine(repositoryRoot.FullName, ".github", "workflows", "backfill-products.yml"));
		StringAssert.Contains(historicalBackfill, "itemseeder = [pscustomobject]@");
		Assert.IsFalse(historicalBackfill.Contains("itemseeder-v1.0.0", StringComparison.Ordinal));
		StringAssert.Contains(historicalBackfill, "mudclient = [pscustomobject]@");
		StringAssert.Contains(historicalBackfill, "'terrainapi', 'mudclient')");
		var backfillTagRegex = historicalBackfill
			.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
			.Single(line => line.Contains("$env:RELEASE_TAG -cnotmatch", StringComparison.Ordinal));
		Assert.IsFalse(backfillTagRegex.Contains("itemseeder", StringComparison.Ordinal));
	}

	[TestMethod]
	public async Task ReleaseWorkflows_ValidateCurrentAndPreSplitProductInventories()
	{
		var repositoryRoot = LocateRepositoryRoot();
		var manifestPath = Path.Combine(repositoryRoot.FullName, "FutureMUD.Web", "Configuration", "release-products.json");
		await using var stream = File.OpenRead(manifestPath);
		var manifest = await JsonSerializer.DeserializeAsync<ReleaseProductManifest>(
			stream,
			new JsonSerializerOptions(JsonSerializerDefaults.Web));
		Assert.IsNotNull(manifest);

		var currentIds = manifest.Products.Select(product => product.Id).ToArray();
		CollectionAssert.AreEquivalent(
			new[] { "engine", "seeder", "itemseeder", "discordbot", "terrainplanner", "terrainapi", "mudclient" },
			currentIds);
		CollectionAssert.AreEquivalent(
			new[] { "engine", "seeder", "discordbot", "terrainplanner", "terrainapi", "mudclient" },
			currentIds.Where(id => id != "itemseeder").ToArray());

		var publishWorkflow = File.ReadAllText(Path.Combine(repositoryRoot.FullName, ".github", "workflows", "publish-products.yml"));
		StringAssert.Contains(publishWorkflow, "$definitions.Count -eq $legacyAllowedIds.Count");
		StringAssert.Contains(publishWorkflow, "[void]$allowedDefinitions.Remove('itemseeder')");

		var backfillWorkflow = File.ReadAllText(Path.Combine(repositoryRoot.FullName, ".github", "workflows", "backfill-products.yml"));
		var backfillIdsLine = backfillWorkflow
			.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
			.Single(line => line.Contains("$allowedIds = [string[]]", StringComparison.Ordinal));
		StringAssert.Contains(backfillIdsLine, "'mudclient'");
		StringAssert.Contains(backfillIdsLine, "'itemseeder'");
	}

	private static DirectoryInfo LocateRepositoryRoot()
	{
		var directory = new DirectoryInfo(AppContext.BaseDirectory);
		while (directory is not null && !Directory.Exists(Path.Combine(directory.FullName, ".github")))
		{
			directory = directory.Parent;
		}

		return directory ?? throw new DirectoryNotFoundException("Could not locate the FutureMUD repository root.");
	}
}
