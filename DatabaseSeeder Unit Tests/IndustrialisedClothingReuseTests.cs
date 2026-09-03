#nullable enable

using System.Reflection;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Framework.Revision;
using MudSharp.Models;
using EditableItem = MudSharp.Models.EditableItem;

namespace MudSharp_Unit_Tests;

[TestClass]
public partial class IndustrialisedClothingReuseTests
{
	private const string Garters = "medieval_tablet_woven_garters";
	private static readonly ClothingSourceLocation Source = new("Clothing/bases.tsv", 2);

	[TestMethod]
	public void ApprovedReuse_ResolvesAll113AuthoritativeSourcesAndKeepsHistoricalOwners()
	{
		var root = ItemSeederManifestCatalogue.FindRepositoryRoot();
		var references = ApprovedReferences(root);
		Assert.AreEqual(113, references.Length);
		Assert.AreEqual(113, references.Distinct(StringComparer.Ordinal).Count());
		using var manifest = JsonDocument.Parse(File.ReadAllText(Path.Combine(root, "Design Documents/Seeding/Seeded_Item_Manifest.json")));
		var items = manifest.RootElement.GetProperty("entries").EnumerateArray()
			.Where(x => x.GetProperty("entityType").GetString() == "item")
			.ToDictionary(x => x.GetProperty("stableKey").GetString()!, StringComparer.Ordinal);
		var sources = references.Select(reference =>
		{
			var source = ItemSeeder.FindHistoricalClothingSource(reference);
			Assert.IsNotNull(source, reference);
			Assert.AreEqual(reference, source.StableReference);
			Assert.AreEqual(items[reference].GetProperty("module").GetString(), source.OwningModule, reference);
			Assert.IsFalse(string.IsNullOrWhiteSpace(source.FullDescription), reference);
			Assert.IsTrue(source.Components.Count > 0, reference);
			return source;
		}).ToArray();
		Assert.AreEqual(33, sources.Count(x => x.OwningModule == "medieval"));
		Assert.AreEqual(2, sources.Count(x => x.OwningModule == "shared-preindustrial"));
		Assert.AreEqual(78, sources.Count(x => x.OwningModule == "outfits"));
		Assert.IsNull(ItemSeeder.FindHistoricalClothingSource("unapproved_historical_item"));
		Assert.IsNull(ItemSeeder.FindHistoricalClothingSource(Garters.ToUpperInvariant()));
	}

	[TestMethod]
	public void ApprovedReuse_All113MaterialTagAndComponentNamesExistInMaintainedPrerequisites()
	{
		var root = ItemSeederManifestCatalogue.FindRepositoryRoot();
		HashSet<string> Names(string file, string property)
		{
			using var json = JsonDocument.Parse(File.ReadAllText(Path.Combine(root, "Design Documents/Data", file)));
			return json.RootElement.EnumerateArray().Select(x => x.GetProperty(property).GetString()!)
				.ToHashSet(StringComparer.OrdinalIgnoreCase);
		}
		var materials = Names("Seeded_Materials.json", "Material Name");
		var components = Names("Seeded_Item_Components.json", "Component Name");
		var tags = File.ReadLines(Path.Combine(root, "Design Documents/Data/SeededTagHierarchy.csv"))
			.Skip(1).Select(x => x.Split('\t')[2]).ToHashSet(StringComparer.OrdinalIgnoreCase);
		var errors = new List<string>();
		foreach (var reference in ApprovedReferences(root))
		{
			var source = ItemSeeder.FindHistoricalClothingSource(reference)!;
			if (!materials.Contains(source.Material)) errors.Add($"{reference}: missing material {source.Material}");
			foreach (var tag in source.Tags)
				if (!tags.Contains(tag)) errors.Add($"{reference}: missing exact tag {tag}");
			foreach (var component in source.Components)
				if (!components.Contains(component)) errors.Add($"{reference}: missing component {component}");
		}
		Assert.AreEqual(0, errors.Count, string.Join(Environment.NewLine, errors));
	}

	private static string[] ApprovedReferences(string root) =>
		File.ReadLines(Path.Combine(root, "Design Documents/Seeding/Industrialised_Clothing_Wave1_Inventory.md"))
			.Where(x => x.StartsWith("| ", StringComparison.Ordinal))
			.Select(x => x.Trim('|').Split('|').Select(y => y.Trim()).ToArray())
			.Where(x => x.Length == 8 && x[4] != "new" && x[4] != "Source")
			.Select(x => x[4]).ToArray();

	[TestMethod]
	public void ExistingComponents_ResolveAttachedRevisionNotCurrentSameName()
	{
		var item = LinkedItem();
		var attached = Component(41, 1, "Variable_FineColour");
		attached.EditableItem.RevisionStatus = (int)RevisionStatus.Obsolete;
		var current = Component(41, 2, attached.Name);
		var result = ItemSeeder.ResolveReusedClothingComponents(item, [current, attached], Source);
		Assert.AreSame(attached, result.Single());
	}

	[DataTestMethod]
	[DataRow("missing", "missing or ambiguous component 41 revision 1")]
	[DataRow("ambiguous", "missing or ambiguous component 41 revision 1")]
	[DataRow("duplicate", "multiple instances or revisions")]
	[DataRow("wrong-item", "does not belong")]
	[DataRow("wrong-item-revision", "does not belong")]
	public void ExistingComponents_RejectInvalidLinksWithSourceDiagnostic(string fault, string diagnostic)
	{
		var item = LinkedItem();
		var components = new List<GameItemComponentProto> { Component(41, 1, "Variable_FineColour") };
		switch (fault)
		{
			case "missing": components[0].RevisionNumber = 2; break;
			case "ambiguous": components.Add(Component(41, 1, "Variable_FineColour")); break;
			case "duplicate": item.GameItemProtosGameItemComponentProtos.Add(Link(item, 41, 2)); break;
			case "wrong-item": item.GameItemProtosGameItemComponentProtos.Single().GameItemProtoId++; break;
			case "wrong-item-revision": item.GameItemProtosGameItemComponentProtos.Single().GameItemProtoRevision++; break;
		}
		var error = Assert.ThrowsException<InvalidDataException>(() => ItemSeeder.ResolveReusedClothingComponents(item, components, Source));
		StringAssert.Contains(error.Message, Source.ToString());
		StringAssert.Contains(error.Message, diagnostic);
	}

	[TestMethod]
	public void FreshIndustrialReuse_CreatesCanonicalGarmentWithoutEarlierPackageAndRerunsSafely()
	{
		using var context = Context();
		SeedPrerequisites(context);
		var catalogue = Catalogue();
		var seeder = new ItemSeeder(catalogue);
		seeder.ValidateClothingPrerequisitesForTesting(context, "industrial");
		Assert.IsFalse(context.ChangeTracker.HasChanges(), "Preflight must remain read-only.");
		Assert.AreEqual(0, context.GameItemProtos.Count());
		seeder.ApplyClothingReuseForTesting("industrial");
		context.SaveChanges();
		var item = context.GameItemProtos.Single();
		var source = ItemSeeder.FindHistoricalClothingSource(Garters)!;
		Assert.AreEqual(Garters, item.UniqueName);
		Assert.AreEqual(source.ShortDescription, item.ShortDescription);
		Assert.AreEqual(source.FullDescription, item.FullDescription);
		Assert.AreEqual(source.Cost, item.CostInBaseCurrency);
		Assert.AreEqual(source.Skinnable, item.PermitPlayerSkins);
		Assert.AreEqual(source.Components.Count, item.GameItemProtosGameItemComponentProtos.Count);
		var managed = context.SeederManagedRecords.Single();
		Assert.AreEqual("medieval", managed.Module);
		Assert.AreEqual(Garters, managed.StableKey);
		var id = item.Id;
		context.ChangeTracker.Clear();
		var rerun = new ItemSeeder(catalogue);
		rerun.ValidateClothingPrerequisitesForTesting(context, "industrial");
		rerun.ApplyClothingReuseForTesting("industrial");
		context.SaveChanges();
		Assert.AreEqual(id, context.GameItemProtos.Single().Id);
		Assert.AreEqual(1, context.SeederManagedRecords.Count());
		CollectionAssert.AreEqual(new[] { "industrial" }, rerun.ResolveSelectedErasForTesting(context,
			new Dictionary<string, string> { ["eras"] = "industrial" }).ToArray());
		// Removing the later admission must not turn this dependency into proof of a Medieval installation.
		CollectionAssert.AreEqual(new[] { "industrial" }, new ItemSeeder().ResolveSelectedErasForTesting(context,
			new Dictionary<string, string> { ["eras"] = "industrial" }).ToArray());
	}

	[TestMethod]
	public void ReuseRerun_WithNewCurrentComponentDoesNotAppendAnotherRevision()
	{
		using var context = Context();
		SeedPrerequisites(context);
		var first = new ItemSeeder(Catalogue());
		first.ValidateClothingPrerequisitesForTesting(context, "industrial");
		first.ApplyClothingReuseForTesting("industrial");
		context.SaveChanges();
		var attached = context.GameItemComponentProtos.Single(x => x.Name == "Variable_FineColour");
		attached.EditableItem.RevisionStatus = (int)RevisionStatus.Obsolete;
		var newer = Component(attached.Id, 1, attached.Name);
		newer.Definition = attached.Definition;
		context.GameItemComponentProtos.Add(newer);
		context.SaveChanges();
		context.ChangeTracker.Clear();
		var rerun = new ItemSeeder(Catalogue());
		rerun.ValidateClothingPrerequisitesForTesting(context, "industrial");
		rerun.ApplyClothingReuseForTesting("industrial");
		context.SaveChanges();
		var links = context.GameItemProtosGameItemComponentProtos.Where(x => x.GameItemComponentProtoId == attached.Id).ToArray();
		Assert.AreEqual(1, links.Length, "A component upgrade must not compose two revisions on the same item.");
		Assert.AreEqual(0, links.Single().GameItemComponentRevision);
		context.ChangeTracker.Clear();
		new ItemSeeder(Catalogue()).ValidateClothingPrerequisitesForTesting(context, "industrial");
	}

	[TestMethod]
	public void Reuse_PreservesBuilderCustomisationOnRerun()
	{
		using var context = Context();
		SeedPrerequisites(context);
		var first = new ItemSeeder(Catalogue());
		first.ValidateClothingPrerequisitesForTesting(context, "industrial");
		first.ApplyClothingReuseForTesting("industrial");
		context.SaveChanges();
		context.GameItemProtos.Single().FullDescription = "Builder-authored replacement.";
		context.SaveChanges();
		context.ChangeTracker.Clear();
		var rerun = new ItemSeeder(Catalogue());
		rerun.ValidateClothingPrerequisitesForTesting(context, "industrial");
		rerun.ApplyClothingReuseForTesting("industrial");
		context.SaveChanges();
		Assert.AreEqual("Builder-authored replacement.", context.GameItemProtos.Single().FullDescription);
		Assert.AreEqual(1, context.SeederManagedRecords.Count());
	}

	[DataTestMethod]
	[DataRow(false, false)]
	[DataRow(false, true)]
	[DataRow(true, false)]
	[DataRow(true, true)]
	public void ReusePreflight_ProjectsMissingColourOnlyForUntouchedManagedStock(bool customised, bool sourceUpdate)
	{
		using var context = Context();
		SeedPrerequisites(context);
		var first = new ItemSeeder(Catalogue());
		first.ValidateClothingPrerequisitesForTesting(context, "industrial");
		first.ApplyClothingReuseForTesting("industrial");
		context.SaveChanges();
		var item = context.GameItemProtos.Single();
		var variable = context.GameItemComponentProtos.Single(x => x.Type == "Variable");
		context.GameItemProtosGameItemComponentProtos.Remove(item.GameItemProtosGameItemComponentProtos
			.Single(x => x.GameItemComponentProtoId == variable.Id));
		context.SaveChanges();
		if (sourceUpdate)
		{
			// Represent the last applied, older stock graph, which did not yet declare colour capability.
			item.FullDescription = "An earlier stock description.";
			var live = typeof(ItemSeeder).GetMethod("BuildLiveItemManifestDefinition", BindingFlags.Instance | BindingFlags.NonPublic)!
				.Invoke(first, [item, Garters])!;
			context.SeederManagedRecords.Single().AppliedFingerprint = ItemSeederManifestCatalogue.Fingerprint(live);
		}
		if (customised) item.FullDescription = "A builder's own garment.";
		context.SaveChanges();
		context.ChangeTracker.Clear();
		var rerun = new ItemSeeder(Catalogue());
		if (customised)
		{
			var error = Assert.ThrowsException<InvalidDataException>(() => rerun.ValidateClothingPrerequisitesForTesting(context, "industrial"));
			StringAssert.Contains(error.Message, "exactly one Variable component");
			Assert.IsFalse(context.ChangeTracker.HasChanges());
			Assert.ThrowsException<InvalidOperationException>(() => rerun.ApplyClothingReuseForTesting("industrial"));
			Assert.AreEqual("A builder's own garment.", context.GameItemProtos.Single().FullDescription);
		}
		else
		{
			rerun.ValidateClothingPrerequisitesForTesting(context, "industrial");
			Assert.IsFalse(context.ChangeTracker.HasChanges(), "Projection must not perform the stock update during preflight.");
			Assert.IsFalse(context.GameItemProtosGameItemComponentProtos.Any(x => x.GameItemComponentProtoId == variable.Id));
			rerun.ApplyClothingReuseForTesting("industrial");
			context.SaveChanges();
			Assert.AreEqual(1, context.GameItemProtosGameItemComponentProtos.Count(x => x.GameItemComponentProtoId == variable.Id));
			Assert.AreEqual(item.Id, context.GameItemProtos.Single().Id);
			Assert.AreEqual(ItemSeeder.FindHistoricalClothingSource(Garters)!.FullDescription, context.GameItemProtos.Single().FullDescription);
			context.ChangeTracker.Clear();
			new ItemSeeder(Catalogue()).ValidateClothingPrerequisitesForTesting(context, "industrial");
		}
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void ReusePreflight_ReplacesObsoleteComponentsOnlyOnUntouchedManagedStock(bool customised)
	{
		using var context = Context();
		SeedPrerequisites(context);
		var first = new ItemSeeder(Catalogue());
		first.ValidateClothingPrerequisitesForTesting(context, "industrial");
		first.ApplyClothingReuseForTesting("industrial");
		context.SaveChanges();
		var item = context.GameItemProtos.Single();
		var variable = context.GameItemComponentProtos.Single(x => x.Type == "Variable");
		context.GameItemProtosGameItemComponentProtos.Remove(item.GameItemProtosGameItemComponentProtos
			.Single(x => x.GameItemComponentProtoId == variable.Id));
		var earlier = Component(999, 0, "Variable_EarlierStock");
		earlier.Definition = variable.Definition;
		context.GameItemComponentProtos.Add(earlier);
		item.GameItemProtosGameItemComponentProtos.Add(new()
		{
			GameItemProto = item, GameItemProtoId = item.Id, GameItemProtoRevision = item.RevisionNumber,
			GameItemComponent = earlier, GameItemComponentProtoId = earlier.Id, GameItemComponentRevision = 0
		});
		context.SaveChanges();
		typeof(ItemSeeder).GetMethod("InitialiseDependencies", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(first, null);
		var live = typeof(ItemSeeder).GetMethod("BuildLiveItemManifestDefinition", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(first, [item, Garters])!;
		context.SeederManagedRecords.Single().AppliedFingerprint = ItemSeederManifestCatalogue.Fingerprint(live);
		if (customised) item.FullDescription = "Builder's retained garment.";
		context.SaveChanges();
		context.ChangeTracker.Clear();
		var rerun = new ItemSeeder(Catalogue());
		rerun.ValidateClothingPrerequisitesForTesting(context, "industrial");
		Assert.IsFalse(context.ChangeTracker.HasChanges());
		Assert.IsTrue(context.GameItemProtosGameItemComponentProtos.Any(x => x.GameItemComponentProtoId == earlier.Id));
		Assert.IsFalse(context.GameItemProtosGameItemComponentProtos.Any(x => x.GameItemComponentProtoId == variable.Id));
		rerun.ApplyClothingReuseForTesting("industrial");
		context.SaveChanges();
		Assert.AreEqual(customised, context.GameItemProtosGameItemComponentProtos.Any(x => x.GameItemComponentProtoId == earlier.Id));
		Assert.AreEqual(!customised, context.GameItemProtosGameItemComponentProtos.Any(x => x.GameItemComponentProtoId == variable.Id));
		Assert.IsTrue(context.GameItemComponentProtos.Any(x => x.Id == earlier.Id), "Replacing a stock link must not delete the old component definition.");
		Assert.AreEqual(item.Id, context.GameItemProtos.Single().Id);
		if (customised) Assert.AreEqual("Builder's retained garment.", context.GameItemProtos.Single().FullDescription);
		var before = context.GameItemProtosGameItemComponentProtos.AsEnumerable()
			.Select(x => (x.GameItemComponentProtoId, x.GameItemComponentRevision)).OrderBy(x => x.GameItemComponentProtoId).ToArray();
		context.ChangeTracker.Clear();
		var third = new ItemSeeder(Catalogue());
		third.ValidateClothingPrerequisitesForTesting(context, "industrial");
		third.ApplyClothingReuseForTesting("industrial");
		context.SaveChanges();
		CollectionAssert.AreEqual(before, context.GameItemProtosGameItemComponentProtos.AsEnumerable()
			.Select(x => (x.GameItemComponentProtoId, x.GameItemComponentRevision)).OrderBy(x => x.GameItemComponentProtoId).ToArray());
	}

	[TestMethod]
	public void StockReplacement_RejectsUnfingerprintedLinksWithoutRemovingAnyRelationships()
	{
		using var context = Context();
		SeedPrerequisites(context);
		var first = new ItemSeeder(Catalogue());
		first.ValidateClothingPrerequisitesForTesting(context, "industrial");
		first.ApplyClothingReuseForTesting("industrial");
		context.SaveChanges();
		var rerun = new ItemSeeder(Catalogue());
		rerun.ValidateClothingPrerequisitesForTesting(context, "industrial");
		var item = context.GameItemProtos.Single();
		// Simulate a corrupt/unresolvable link discovered after preflight. The older name-only
		// fingerprint omitted such links; it must never authorise their silent removal.
		item.GameItemProtosGameItemComponentProtos.Add(new()
		{
			GameItemProto = item, GameItemProtoId = item.Id, GameItemProtoRevision = item.RevisionNumber,
			GameItemComponentProtoId = 999999, GameItemComponentRevision = 0
		});
		context.SaveChanges();
		var count = context.GameItemProtosGameItemComponentProtos.Count();
		var fingerprint = context.SeederManagedRecords.Single().AppliedFingerprint;
		var error = Assert.ThrowsException<InvalidOperationException>(() => rerun.ApplyClothingReuseForTesting("industrial"));
		StringAssert.Contains(error.Message, "not covered by its stock fingerprint");
		Assert.IsFalse(context.ChangeTracker.HasChanges());
		Assert.AreEqual(count, context.GameItemProtosGameItemComponentProtos.Count());
		Assert.AreEqual(fingerprint, context.SeederManagedRecords.Single().AppliedFingerprint);
	}

	[TestMethod]
	public void ReuseOwnership_ExactUnmanagedStockIsAdoptedOnlyDuringApplication()
	{
		using var context = Context();
		SeedPrerequisites(context);
		var first = new ItemSeeder(Catalogue());
		first.ValidateClothingPrerequisitesForTesting(context, "industrial");
		first.ApplyClothingReuseForTesting("industrial");
		context.SaveChanges();
		context.SeederManagedRecords.RemoveRange(context.SeederManagedRecords);
		context.SaveChanges();
		context.ChangeTracker.Clear();
		var seeder = new ItemSeeder(Catalogue());
		seeder.ValidateClothingPrerequisitesForTesting(context, "industrial");
		Assert.IsFalse(context.ChangeTracker.HasChanges());
		Assert.AreEqual(0, context.SeederManagedRecords.Count());
		seeder.ApplyClothingReuseForTesting("industrial");
		context.SaveChanges();
		Assert.AreEqual(1, context.GameItemProtos.Count());
		Assert.AreEqual(1, context.SeederManagedRecords.Count());
	}

	[DataTestMethod]
	[DataRow("unmanaged-drift", "Unmanaged historical clothing conflict")]
	[DataRow("wrong-owner-id", "ownership conflict")]
	[DataRow("legacy-drift", "Unmanaged legacy item conflict")]
	public void ReuseOwnership_ConflictsFailReadOnlyPreflight(string fault, string diagnostic)
	{
		using var context = Context();
		SeedPrerequisites(context);
		var first = new ItemSeeder(Catalogue());
		first.ValidateClothingPrerequisitesForTesting(context, "industrial");
		first.ApplyClothingReuseForTesting("industrial");
		context.SaveChanges();
		if (fault == "wrong-owner-id")
			context.SeederManagedRecords.Single().LogicalId = 123456;
		else
		{
			context.SeederManagedRecords.RemoveRange(context.SeederManagedRecords);
			context.GameItemProtos.Single().FullDescription = "Unmanaged non-stock description.";
			if (fault == "legacy-drift") context.GameItemProtos.Single().UniqueName = null;
		}
		context.SaveChanges();
		context.ChangeTracker.Clear();
		var seeder = new ItemSeeder(Catalogue());
		var error = Assert.ThrowsException<InvalidDataException>(() => seeder.ValidateClothingPrerequisitesForTesting(context, "industrial"));
		StringAssert.Contains(error.Message, diagnostic);
		StringAssert.Contains(error.Message, "Clothing/bases.tsv:2");
		Assert.ThrowsException<InvalidOperationException>(() => seeder.ApplyClothingReuseForTesting("industrial"));
		Assert.IsFalse(context.ChangeTracker.HasChanges());
		Assert.AreEqual(1, context.GameItemProtos.Count());
	}

	[TestMethod]
	public void FailedPreflight_CannotApplyPartiallyResolvedReusePlans()
	{
		using var context = Context();
		SeedPrerequisites(context);
		var catalogue = Catalogue();
		catalogue = catalogue with { Clothing = catalogue.Clothing with
		{
			Colours = [catalogue.Clothing.Colours.Single() with { Definition = "Missing Colour" }]
		} };
		var seeder = new ItemSeeder(catalogue);
		Assert.ThrowsException<InvalidDataException>(() => seeder.ValidateClothingPrerequisitesForTesting(context, "industrial"));
		Assert.ThrowsException<InvalidOperationException>(() => seeder.ApplyClothingReuseForTesting("industrial"));
		Assert.IsFalse(context.ChangeTracker.HasChanges());
		Assert.AreEqual(0, context.GameItemProtos.Count());
		Assert.AreEqual(0, context.SeederManagedRecords.Count());
	}

	[DataTestMethod]
	[DataRow("empty-profile")]
	[DataRow("missing-body")]
	[DataRow("non-wearable-location")]
	public void PhysicalPreflight_RejectsInvalidGeometryBeforeItemOrProvenanceMutation(string fault)
	{
		using var context = Context();
		SeedPrerequisites(context);
		switch (fault)
		{
			case "empty-profile": context.WearProfiles.Single().WearlocProfiles = "<Profiles/>"; break;
			case "missing-body": context.WearProfiles.Single().BodyPrototypeId = 99; break;
			case "non-wearable-location": context.BodypartProtos.Single().BodypartType = (int)MudSharp.Body.BodypartTypeEnum.Tongue; break;
		}
		context.SaveChanges();
		var seeder = new ItemSeeder(Catalogue());
		Assert.ThrowsException<InvalidDataException>(() => seeder.ValidateClothingPrerequisitesForTesting(context, "industrial"));
		Assert.ThrowsException<InvalidOperationException>(() => seeder.ApplyClothingReuseForTesting("industrial"));
		Assert.IsFalse(context.ChangeTracker.HasChanges());
		Assert.AreEqual(0, context.GameItemProtos.Count());
		Assert.AreEqual(0, context.SeederManagedRecords.Count());
	}

	[DataTestMethod]
	[DataRow("remembered")]
	[DataRow("other-managed-item")]
	public void EraInference_StillRetainsGenuinelyInstalledHistoricalPackages(string evidence)
	{
		using var context = Context();
		var seeder = new ItemSeeder(Catalogue());
		if (evidence == "remembered")
			context.SeederChoices.Add(new SeederChoice
			{
				Seeder = seeder.Name, Choice = "eras", Answer = "medieval", Version = "test", DateTime = DateTime.UtcNow
			});
		else
			context.SeederManagedRecords.Add(new SeederManagedRecord
			{
				Seeder = seeder.Name, Module = "medieval", EntityType = "item", StableKey = "unrelated_medieval_item"
			});
		context.SaveChanges();
		CollectionAssert.AreEqual(new[] { "medieval", "industrial" }, seeder.ResolveSelectedErasForTesting(context,
			new Dictionary<string, string> { ["eras"] = "industrial" }).ToArray());
	}

	[DataTestMethod]
	[DataRow("too-many-layers", "MaximumLayerWeight")]
	[DataRow("bulky-conflict", "Bulky outfit entries")]
	[DataRow("unwearable-alone", "Standalone garment exceeds")]
	public void OutfitPhysicalPreflight_RejectsDefiniteLayerConflictsBeforeAnyMutation(string fault, string diagnostic)
	{
		using var context = Context();
		SeedPrerequisites(context);
		var count = fault == "too-many-layers" ? 5 : 2;
		if (fault == "bulky-conflict")
		{
			var component = context.GameItemComponentProtos.Single(x => x.Type == "Wearable");
			component.Definition = component.Definition.Replace("<Definition>", "<Definition Bulky=\"true\">");
		}
		if (fault == "unwearable-alone")
			context.StaticConfigurations.Add(new StaticConfiguration { SettingName = "MaximumLayerWeight", Definition = "0.5" });
		context.SaveChanges();
		var seeder = new ItemSeeder(CatalogueWithOutfit(count));
		var error = Assert.ThrowsException<InvalidDataException>(() => seeder.ValidateClothingPrerequisitesForTesting(context, "industrial"));
		StringAssert.Contains(error.Message, diagnostic);
		Assert.ThrowsException<InvalidOperationException>(() => seeder.ApplyClothingReuseForTesting("industrial"));
		Assert.IsFalse(context.ChangeTracker.HasChanges());
		Assert.AreEqual(0, context.GameItemProtos.Count());
		Assert.AreEqual(0, context.SeederManagedRecords.Count());
	}

	[TestMethod]
	public void OutfitPhysicalPreflight_AcceptsExactLayerLimitWithoutMutation()
	{
		using var context = Context();
		SeedPrerequisites(context);
		new ItemSeeder(CatalogueWithOutfit(4)).ValidateClothingPrerequisitesForTesting(context, "industrial");
		Assert.IsFalse(context.ChangeTracker.HasChanges());
		Assert.AreEqual(0, context.GameItemProtos.Count());
	}

	private static IndustrialisedItemCatalogueDocument CatalogueWithOutfit(int count)
	{
		var catalogue = Catalogue();
		return catalogue with { Clothing = catalogue.Clothing with
		{
			Outfits = [new(Source, "layer_fixture", "Layer fixture", "An explicitly synthetic layer check.", ["industrial"], ClothingReviewStatus.Reviewed, "test")],
			OutfitEntries = Enumerable.Range(1, count).Select(i => new ClothingOutfitEntryRow(Source, "layer_fixture", $"entry{i}", i,
				Garters, "", "Leggings", MudSharp.GameItems.OutfitTemplateItemPlacement.Worn, "", "", "test")).ToArray(),
			OutfitColours = Enumerable.Range(1, count).Select(i => new ClothingOutfitColourRow(Source, "layer_fixture", $"entry{i}", "colour", "blue")).ToArray()
		} };
	}

	private static IndustrialisedItemCatalogueDocument Catalogue()
	{
		var original = IndustrialisedClothingCatalogueTests.Load(IndustrialisedClothingCatalogueTests.Fixture());
		return ItemSeeder.IndustrialisedCatalogueForTesting with
		{
			Items = [], Crafts = [], Outfits = [],
			Clothing = new(
				[original.Bases.Single() with { ItemReference = Garters, EraAdmissions = ["industrial"], ReviewStatus = ClothingReviewStatus.Reviewed }], [],
				[original.Colours.Single() with { PresentationReference = Garters }], [], [], [], [], [], [], [], [], [], [])
		};
	}

	private static FuturemudDatabaseContext Context() => new(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
		.UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

	private static void SeedPrerequisites(FuturemudDatabaseContext context)
	{
		context.Accounts.Add(new Account
		{
			Id = 1, Name = "SeederTest", Password = "password", Salt = 1, Email = "seeder@example.com",
			LastLoginIp = "127.0.0.1", FormatLength = 80, InnerFormatLength = 78, ActiveCharactersAllowed = 1,
			UseUnicode = true, TimeZoneId = "UTC", CultureName = "en-AU", RegistrationCode = "", IsRegistered = true,
			RecoveryCode = "", UnitPreference = "metric", CreationDate = DateTime.UtcNow, PageLength = 22,
			HasBeenActiveInWeek = true, HintsEnabled = true
		});
		context.Materials.Add(new Material
		{
			Id = 1, Name = "wool", MaterialDescription = "wool", Type = 0, BehaviourType = 0, Density = 1,
			Organic = true, ResidueSdesc = "", ResidueDesc = "", ResidueColour = "grey"
		});
		var source = ItemSeeder.FindHistoricalClothingSource(Garters)!;
		var tags = new Dictionary<string, Tag>(StringComparer.Ordinal);
		foreach (var path in source.Tags)
		{
			Tag? parent = null;
			var qualified = "";
			foreach (var segment in path.Split(" / "))
			{
				qualified = qualified.Length == 0 ? segment : $"{qualified} / {segment}";
				if (!tags.TryGetValue(qualified, out var tag))
				{
					tag = new Tag { Id = tags.Count + 1, Name = segment, Parent = parent, ParentId = parent?.Id };
					tags.Add(qualified, tag);
					context.Tags.Add(tag);
				}
				parent = tag;
			}
		}
		context.CharacteristicDefinitions.Add(new CharacteristicDefinition
		{
			Id = 11, Name = "Garment Colour", Pattern = "^colour$", Model = "standard", Description = "Garment dye"
		});
		context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Id = 31, Name = "All Colours", Type = "all", TargetDefinitionId = 11, Definition = "<Values/>", Description = "Garment dyes"
		});
		foreach (var (value, index) in new[] { "blue", "cream", "black" }.Select((value, index) => (value, index)))
			context.CharacteristicValues.Add(new CharacteristicValue { Id = 21 + index, Name = value, DefinitionId = 11, Value = value });
		context.WearProfiles.Add(new WearProfile
		{
			Id = 51, Name = "Leggings", Type = "Direct", BodyPrototypeId = 1,
			WearlocProfiles = "<Profiles><Profile Bodypart=\"61\" Mandatory=\"true\" Transparent=\"false\" NoArmour=\"false\" PreventsRemoval=\"true\"/></Profiles>",
			WearStringInventory = "worn on", WearAction1st = "put", WearAction3rd = "puts", WearAffix = "on", Description = "Test profile"
		});
		context.BodyProtos.Add(new BodyProto
		{
			Id = 1, Name = "Fixture Humanoid", WielderDescriptionPlural = "hands", WielderDescriptionSingle = "hand",
			ConsiderString = "humanoid", LegDescriptionSingular = "leg", LegDescriptionPlural = "legs"
		});
		context.BodypartShapes.Add(new BodypartShape { Id = 71, Name = "Fixture leg" });
		context.BodypartProtos.Add(new BodypartProto
		{
			Id = 61, BodyId = 1, Name = "leg", Description = "leg", BodypartShapeId = 71,
			BodypartType = 0, IsCore = true, DefaultMaterialId = 1, SeverFormula = "100"
		});
		foreach (var (name, index) in source.Components.Select((name, index) => (name, index)))
		{
			var component = Component(41 + index, 0, name);
			component.Type = IndustrialisedComponentMetadataCatalogue.Document.Prototypes[name].Type;
			component.Definition = component.Type switch
			{
				"Variable" => "<Definition><Characteristic Value=\"11\" Profile=\"31\"/></Definition>",
				"Wearable" => "<Definition><Profiles Default=\"51\"><Profile>51</Profile></Profiles></Definition>",
				_ => "<Definition/>"
			};
			context.GameItemComponentProtos.Add(component);
		}
		context.SaveChanges();
	}

	private static GameItemComponentProto Component(long id, int revision, string name) => new()
	{
		Id = id, RevisionNumber = revision, Name = name, Type = "Variable", Definition = "<Definition/>", Description = "Test prerequisite",
		EditableItem = new EditableItem
		{
			RevisionNumber = revision, RevisionStatus = (int)RevisionStatus.Current, BuilderAccountId = 1,
			BuilderDate = DateTime.UtcNow, BuilderComment = "test", ReviewerAccountId = 1, ReviewerComment = "test", ReviewerDate = DateTime.UtcNow
		}
	};

	private static GameItemProto LinkedItem()
	{
		var item = new GameItemProto { Id = 7, RevisionNumber = 3, UniqueName = Garters };
		item.GameItemProtosGameItemComponentProtos.Add(Link(item, 41, 1));
		return item;
	}

	private static GameItemProtosGameItemComponentProtos Link(GameItemProto item, long component, int revision) => new()
	{
		GameItemProtoId = item.Id, GameItemProtoRevision = item.RevisionNumber,
		GameItemComponentProtoId = component, GameItemComponentRevision = revision
	};
}
