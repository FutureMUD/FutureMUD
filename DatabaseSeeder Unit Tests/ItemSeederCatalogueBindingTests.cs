#nullable enable

using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;
using MudSharp.Models;
using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;
using Tag = MudSharp.Models.Tag;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemSeederCatalogueBindingTests
{
	private const BindingFlags Hidden = BindingFlags.Instance | BindingFlags.NonPublic;

	[TestMethod]
	public void RecipeCheckTrait_ConfiguredSpecialistWinsOverSharedKnowledgeGateFallback()
	{
		var recipes = CompiledRecipeCatalogue.Select(new System.Collections.Generic.HashSet<string>(["antiquity", "medieval", "renaissance", "earlymodern"]), false)
			.Where(x => x.CheckTraitChoices.Count > 0).ToArray();
		Assert.AreEqual(40, recipes.Length);
		var recipe = recipes.Single(x => x.Name == "bake Antiquity flatbread");
		var cooking = new TraitDefinition { Id = 1, Name = "Cookery" };
		var baking = new TraitDefinition { Id = 2, Name = "Baking" };
		var seeder = new ItemSeeder();
		var traits = (DictionaryWithDefault<string, TraitDefinition>)typeof(ItemSeeder).GetField("_traits", Hidden)!.GetValue(seeder)!;
		traits["Cooking"] = cooking;
		var resolve = typeof(ItemSeeder).GetMethod("ResolveCatalogueRecipeCheckTrait", Hidden)!;
		Assert.AreSame(cooking, resolve.Invoke(seeder, new object[] { recipe }));
		traits["Baking"] = baking;
		Assert.AreSame(baking, resolve.Invoke(seeder, new object[] { recipe }), "Knowledge-only access programs can be shared while check skills differ.");
	}

	[TestMethod]
	public void RecipeWriter_UsesConfiguredCheckSkillBeforeMissingCapturedFallback()
	{
		using var context = Program.CreateItemManifestCaptureContext();
		var threshing = new TraitDefinition { Id = 2, Name = "Threshing", TraitGroup = "Crafting", ChargenBlurb = "", ValueExpression = "" };
		context.TraitDefinitions.Add(threshing);
		context.SaveChanges();
		var recipe = CompiledRecipeCatalogue.Select(new System.Collections.Generic.HashSet<string>(["antiquity"]), false)
			.Single(x => x.Name == "thresh grain into heads");
		recipe = recipe with { AppearProg = null, CanUseProg = null, WhyCannotUseProg = null };
		Assert.AreEqual("Farming", recipe.Trait);
		var seeder = new ItemSeeder();
		typeof(ItemSeeder).GetField("_context", Hidden)!.SetValue(seeder, context);
		typeof(ItemSeeder).GetField("_manifestCaptureOnly", Hidden)!.SetValue(seeder, true);
		typeof(ItemSeeder).GetMethod("InitialiseDependencies", Hidden)!.Invoke(seeder, null);
		// Capture validates the complete writer path without needing the game's agriculture tags.
		typeof(ItemSeeder).GetMethod("InstallCatalogueRecipe", Hidden)!.Invoke(seeder, new object[] { recipe });
		Assert.IsFalse(context.TraitDefinitions.Any(x => x.Name == "Farming"));
	}

	[TestMethod]
	public void ExistingTagPath_RebindsHomonymsAndRetainsAllOwnedParentsOnRerun()
	{
		using var context = ItemSeederUnifiedCatalogueTests.Fixture();
		var genericMeat = new Tag { Id = 1000, Name = "Meat", ParentId = 1 };
		context.Tags.Add(genericMeat); context.SaveChanges();
		ItemSeeder Seeder()
		{
			var seeder = new ItemSeeder();
			typeof(ItemSeeder).GetField("_context", Hidden)!.SetValue(seeder, context);
			typeof(ItemSeeder).GetMethod("InitialiseDependencies", Hidden)!.Invoke(seeder, null);
			return seeder;
		}
		const string path = "Materials / Natural Materials / Food / Meat";
		var first = Seeder();
		var tag = (Tag)typeof(ItemSeeder).GetMethod("EnsureAntiquityTagPath", Hidden)!.Invoke(first, new object[] { path })!;
		typeof(ItemSeeder).GetMethod("SaveManifestChanges", Hidden)!.Invoke(first, null);
		var before = context.SeederManagedRecords.Where(x => x.EntityType == "tag").OrderBy(x => x.StableKey).Select(x => new { x.StableKey, x.LogicalId, x.AppliedFingerprint }).ToArray();
		Assert.AreEqual(4, before.Length);
		var parents = context.SeederManagedRecords.Where(x => x.EntityType == "tag" && x.StableKey != path).OrderBy(x => x.StableKey).ToArray();
		foreach (var record in parents)
		{
			record.Module = record.StableKey == "Materials" ? "vehicles" : "antiquity";
			record.AppliedAt = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		}
		context.SaveChanges();
		var parentProvenance = JsonSerializer.Serialize(parents);
		var rerun = Seeder();
		typeof(ItemSeeder).GetMethod("EnsureAntiquityTagPath", Hidden)!.Invoke(rerun, new object[] { path });
		typeof(ItemSeeder).GetMethod("RetireMissingManagedRecords", Hidden)!.Invoke(rerun, null);
		Assert.IsTrue((bool)typeof(ItemSeeder).GetMethod("IsManifestAggregateRegistered", Hidden)!.Invoke(rerun, new object[] { "tag", path })!);
		Assert.AreEqual(parentProvenance, JsonSerializer.Serialize(parents), "Retaining owned ancestors must preserve every provenance field.");
		Assert.AreEqual(tag.Id, ((Tag)typeof(ItemSeeder).GetMethod("LookupTag", Hidden)!.Invoke(rerun, new object[] { "Meat" })!).Id);
		Assert.IsTrue(context.SeederManagedRecords.Where(x => x.EntityType == "tag").All(x => !x.Retired));
		CollectionAssert.AreEqual(before, context.SeederManagedRecords.Where(x => x.EntityType == "tag").OrderBy(x => x.StableKey).Select(x => new { x.StableKey, x.LogicalId, x.AppliedFingerprint }).ToArray());
	}

	[TestMethod]
	public void ExistingUnownedTagPath_ClaimsOnlyRequestedLeaf()
	{
		using var context = ItemSeederUnifiedCatalogueTests.Fixture();
		var seeder = Initialise(context);
		Ensure(seeder, "Market / Clothing / Standard Clothing");
		context.SaveChanges();
		var owned = context.SeederManagedRecords.Where(x => x.EntityType == "tag").ToArray();
		Assert.AreEqual(1, owned.Length);
		Assert.AreEqual("Market / Clothing / Standard Clothing", owned[0].StableKey);
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void RecipeProduct_RequiresPermanentSelectorIdentityBeforeSerialising(bool cooked)
	{
		var seeder = new ItemSeeder();
		var progs = (System.Collections.Generic.Dictionary<string, FutureProg>)typeof(ItemSeeder).GetField("_progs", Hidden)!.GetValue(seeder)!;
		var prog = new FutureProg { FunctionName = "UnsavedSelector" };
		progs[prog.FunctionName] = prog;
		CatalogueRecipeProduct product = cooked
			? new CatalogueProgCookedRecipeProduct(prog.FunctionName, false, [])
			: new CatalogueProgRecipeProduct(prog.FunctionName);
		var write = typeof(ItemSeeder).GetMethod("BuildRecipeProduct", Hidden)!;
		var error = Assert.ThrowsException<TargetInvocationException>(() => write.Invoke(seeder, new object[] { new Craft(), product }));
		Assert.IsInstanceOfType(error.InnerException, typeof(InvalidOperationException));
		StringAssert.Contains(error.InnerException!.Message, "permanent database identity");
		prog.Id = 123;
		var result = ((string Type, string Definition))write.Invoke(seeder, new object[] { new Craft(), product })!;
		Assert.AreEqual("123", XDocument.Parse(result.Definition).Root!.Element("ItemProg")!.Value);
	}

	[TestMethod]
	public void ExistingOwnedTagLeaf_RetainsItsOriginalModuleWhenRequestedElsewhere()
	{
		using var context = ItemSeederUnifiedCatalogueTests.Fixture();
		const string path = "New Root / New Parent / New Leaf";
		Ensure(Initialise(context), path);
		context.SaveChanges();
		var leaf = context.SeederManagedRecords.Single(x => x.StableKey == path);
		leaf.Module = "shared-preindustrial";
		context.SaveChanges();
		var rerun = Initialise(context);
		typeof(ItemSeeder).GetField("_activeManifestModule", Hidden)!.SetValue(rerun, "antiquity");
		Ensure(rerun, path);
		context.SaveChanges();
		Assert.AreEqual("shared-preindustrial", leaf.Module);
	}

	[TestMethod]
	public void NewTagPath_KeepsOriginalPrefixRegistration()
	{
		using var context = ItemSeederUnifiedCatalogueTests.Fixture();
		Ensure(Initialise(context), "Market / Clothing / Standard Clothing / New Child");
		context.SaveChanges();
		Assert.AreEqual(4, context.SeederManagedRecords.Count(x => x.EntityType == "tag"));
		Assert.AreEqual(1, context.Tags.Count(x => x.Name == "New Child"));
	}

	[TestMethod]
	public void OwnedParentWithDifferentLogicalIdentity_FailsClosed()
	{
		using var context = ItemSeederUnifiedCatalogueTests.Fixture();
		const string path = "New Root / New Parent / New Leaf";
		Ensure(Initialise(context), path);
		context.SaveChanges();
		context.SeederManagedRecords.Single(x => x.StableKey == "New Root").LogicalId = 99999;
		context.SaveChanges();
		var error = Assert.ThrowsException<TargetInvocationException>(() => Ensure(Initialise(context), path));
		Assert.IsInstanceOfType(error.InnerException, typeof(InvalidOperationException));
		StringAssert.Contains(error.InnerException!.Message, "ownership conflict for tag:New Root");
	}

	[TestMethod]
	public void ReinitialisingDependencies_DoesNotCarryObservedParentsIntoAnotherRun()
	{
		using var context = ItemSeederUnifiedCatalogueTests.Fixture();
		const string path = "New Root / New Parent / New Leaf";
		Ensure(Initialise(context), path);
		context.SaveChanges();
		var rerun = Initialise(context);
		Ensure(rerun, path);
		typeof(ItemSeeder).GetMethod("InitialiseDependencies", Hidden)!.Invoke(rerun, null);
		typeof(ItemSeeder).GetMethod("RetireMissingManagedRecords", Hidden)!.Invoke(rerun, null);
		Assert.IsTrue(context.SeederManagedRecords.Single(x => x.StableKey == "New Root").Retired);
		Assert.IsTrue(context.SeederManagedRecords.Single(x => x.StableKey == "New Root / New Parent").Retired);
		Assert.IsFalse(context.SeederManagedRecords.Single(x => x.StableKey == path).Retired);
	}

	private static ItemSeeder Initialise(MudSharp.Database.FuturemudDatabaseContext context)
	{
		var seeder = new ItemSeeder();
		typeof(ItemSeeder).GetField("_context", Hidden)!.SetValue(seeder, context);
		typeof(ItemSeeder).GetMethod("InitialiseDependencies", Hidden)!.Invoke(seeder, null);
		return seeder;
	}

	private static void Ensure(ItemSeeder seeder, string path) =>
		typeof(ItemSeeder).GetMethod("EnsureAntiquityTagPath", Hidden)!.Invoke(seeder, new object[] { path });
}
