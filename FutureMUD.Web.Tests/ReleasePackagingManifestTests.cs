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
		foreach (var product in manifest.Products.Where(product => product.Id != "mudclient"))
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
	}
}
