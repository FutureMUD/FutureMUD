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
		Assert.AreEqual(5, manifest.Products.Count);
		foreach (var product in manifest.Products)
		{
			Assert.IsTrue(product.FrameworkDependent, product.Id);
			Assert.IsTrue(product.SingleFile, product.Id);
			Assert.IsTrue(product.IncludeNativeLibrariesForSelfExtract, product.Id);
		}
	}
}
