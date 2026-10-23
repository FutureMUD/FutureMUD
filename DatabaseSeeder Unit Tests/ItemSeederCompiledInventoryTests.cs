#nullable enable

using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemSeederCompiledInventoryTests
{
	[TestMethod]
	public void CompiledInventory_IsCachedAndMatchesTheAuthoritativeCapture()
	{
		var compiled = ItemSeederManifestCatalogue.LoadForRuntime();
		Assert.AreSame(compiled, ItemSeederManifestCatalogue.LoadForRuntime());
		using var context = DatabaseSeeder.Program.CreateItemManifestCaptureContext();
		var actual = new ItemSeeder().CaptureManifest(context, ItemSeederManifestCatalogue.FindRepositoryRoot());
		Assert.AreEqual(actual.Entries.Count, compiled.Entries.Count);
		CollectionAssert.AreEqual(actual.Entries.Select(ItemSeederManifestCatalogue.Fingerprint).ToArray(),
			compiled.Entries.Select(ItemSeederManifestCatalogue.Fingerprint).ToArray(),
			"Refresh the derived C# ownership projection with --export-compiled-item-inventory after editing catalogue definitions.");
	}

	[TestMethod]
	public void ClothingLookup_PreservesTheCanonicalAndDocumentedSourceVariants()
	{
		var canonical = CompiledAuthoredItemCatalogue.Get("medieval_daoist_cloud_sleeved_robe");
		var documented = CompiledAuthoredItemCatalogue.Get("medieval_daoist_cloud_sleeved_robe", "SeedDocumentedClothingItems");
		Assert.IsNull(canonical.BuilderNotes);
		Assert.IsTrue(canonical.AllowLegacyShortDescriptionMatch);
		Assert.AreEqual("Early Modern admission of an exact Medieval clothing definition.", documented.BuilderNotes);
		Assert.IsFalse(documented.AllowLegacyShortDescriptionMatch);
		Assert.AreEqual(canonical.FullDescription, documented.FullDescription);
		Assert.AreEqual(canonical.Cost, documented.Cost);
	}
}
