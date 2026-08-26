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
		Assert.AreEqual(6, manifest.Products.Count);
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
	}
}
