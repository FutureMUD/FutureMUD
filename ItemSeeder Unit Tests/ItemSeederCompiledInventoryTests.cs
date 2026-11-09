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
	[TestCategory("Integration")]
	public void CompiledInventory_IsCachedAndMatchesTheAuthoritativeCapture()
	{
		var compiled = ItemSeederManifestCatalogue.LoadForRuntime();
		Assert.AreSame(compiled, ItemSeederManifestCatalogue.LoadForRuntime());
		using var context = DatabaseSeeder.ItemSeederProgram.CreateItemManifestCaptureContext();
		var actual = new ItemSeeder().CaptureManifest(context, ItemSeederManifestCatalogue.FindRepositoryRoot());
		Assert.AreEqual(actual.Entries.Count, compiled.Entries.Count);
		CollectionAssert.AreEqual(actual.Entries.Select(ItemSeederManifestCatalogue.Fingerprint).ToArray(),
			compiled.Entries.Select(ItemSeederManifestCatalogue.Fingerprint).ToArray(),
			"Refresh the derived C# ownership projection with --export-compiled-item-inventory after editing catalogue definitions.");
	}
}
