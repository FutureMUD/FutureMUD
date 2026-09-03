#nullable enable

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

public partial class IndustrialisedClothingReuseTests
{
	[DataTestMethod]
	[DataRow("unmanaged-drift", "Unmanaged craft conflict")]
	[DataRow("missing-access", "Unmanaged craft conflict")]
	[DataRow("wrong-owner-id", "ownership conflict")]
	[DataRow("other-owner", "is claimed by")]
	[DataRow("duplicate-current", "ambiguous current craft")]
	[DataRow("duplicate-current-revision", "ambiguous current craft")]
	[DataRow("missing-current", "no current revision")]
	[DataRow("unmanaged-no-current", "no current revision")]
	[DataRow("renamed-collision", "ownership conflict")]
	public void CraftOwnership_LaterConflictRejectsBeforeAnyClothingMutation(string fault, string diagnostic)
	{
		using var context = Context();
		SeedClothingCraftPrerequisites(context);
		var catalogue = CatalogueWithTwoCrafts();
		InstallClothingCrafts(context, catalogue);
		var craft = context.Crafts.Include(x => x.EditableItem).Single(x => x.Name == "sew second garters");
		var managed = context.SeederManagedRecords.Single(x => x.EntityType == "craft" && x.StableKey == "fixture_craft_second");
		switch (fault)
		{
			case "unmanaged-drift": context.SeederManagedRecords.Remove(managed); craft.Blurb = "A different builder recipe."; break;
			case "missing-access": context.SeederManagedRecords.Remove(managed); context.FutureProgs.Remove(context.FutureProgs.Single(x => x.Id == craft.WhyCannotUseProgId)); break;
			case "wrong-owner-id": managed.LogicalId = 987654; break;
			case "other-owner": context.SeederManagedRecords.Add(new() { Seeder = "Items", EntityType = "craft", StableKey = "other_craft", Module = "crafts", LogicalId = craft.Id }); break;
			case "duplicate-current": context.Crafts.Add(ConflictingCraft(craft.Id + 100, craft.Name, craft.Category)); break;
			case "duplicate-current-revision":
				var duplicate = ConflictingCraft(craft.Id, craft.Name, craft.Category);
				duplicate.RevisionNumber = duplicate.EditableItem.RevisionNumber = 1;
				context.Crafts.Add(duplicate);
				break;
			case "missing-current": craft.EditableItem.RevisionStatus = (int)RevisionStatus.Obsolete; break;
			case "unmanaged-no-current": context.SeederManagedRecords.Remove(managed); craft.EditableItem.RevisionStatus = (int)RevisionStatus.Obsolete; break;
			case "renamed-collision":
				context.Crafts.Add(ConflictingCraft(craft.Id + 100, craft.Name, craft.Category));
				craft.Name = "Builder's renamed recipe";
				break;
		}
		context.SaveChanges();
		context.ChangeTracker.Clear();
		var before = ClothingCraftOwnershipState(context);
		var seeder = new ItemSeeder(catalogue);
		var error = Assert.ThrowsException<InvalidDataException>(() => seeder.ValidateClothingPrerequisitesForTesting(context, "industrial"));
		StringAssert.Contains(error.Message, "Clothing/crafts.tsv:3");
		StringAssert.Contains(error.Message, diagnostic);
		Assert.IsFalse(context.ChangeTracker.HasChanges());
		Assert.AreEqual(before, ClothingCraftOwnershipState(context));
		Assert.AreEqual(0, seeder.GetCapturedManifestEntriesForTesting().Count);
		Assert.ThrowsException<InvalidOperationException>(() => seeder.ApplyClothingReuseForTesting("industrial"));
	}

	[DataTestMethod]
	[DataRow("unmanaged", "Unmanaged craft access prog conflict")]
	[DataRow("duplicate-name", "Ambiguous craft access prog")]
	[DataRow("wrong-owner-id", "ownership conflict")]
	[DataRow("other-owner", "is claimed by")]
	[DataRow("return-type", "incompatible callable signature")]
	[DataRow("parameters", "incompatible callable signature")]
	[DataRow("any-parameters", "incompatible callable signature")]
	[DataRow("static", "incompatible callable signature")]
	public void CraftOwnership_AccessProgConflictRejectsWholeBatchWithoutMutation(string fault, string diagnostic)
	{
		using var context = Context();
		SeedClothingCraftPrerequisites(context);
		var catalogue = CatalogueWithTwoCrafts();
		InstallClothingCrafts(context, catalogue);
		InstallClothingCrafts(context, catalogue);
		var craft = context.Crafts.Single(x => x.Name == "sew second garters");
		var prog = context.FutureProgs.Include(x => x.FutureProgsParameters).Single(x => x.Id == craft.WhyCannotUseProgId);
		var managed = context.SeederManagedRecords.Single(x => x.EntityType == "prog" && x.StableKey == prog.FunctionName);
		switch (fault)
		{
			case "unmanaged": context.SeederManagedRecords.Remove(managed); prog.FunctionText = "return \"Builder restriction\""; break;
			case "duplicate-name":
				context.FutureProgs.Add(new() { FunctionName = prog.FunctionName, FunctionText = "return false", FunctionComment = "", Category = "Other", Subcategory = "Other", ReturnType = 4 });
				break;
			case "wrong-owner-id": managed.LogicalId = 987654; break;
			case "other-owner": context.SeederManagedRecords.Add(new() { Seeder = "Items", EntityType = "prog", StableKey = "another_access_prog", Module = "crafts", LogicalId = prog.Id }); break;
			case "return-type": prog.ReturnType = 0; break;
			case "parameters": prog.FutureProgsParameters.Single().ParameterType = 0; break;
			case "any-parameters": prog.AcceptsAnyParameters = true; break;
			case "static": prog.StaticType = 1; break;
		}
		context.SaveChanges();
		context.ChangeTracker.Clear();
		var before = ClothingCraftOwnershipState(context);
		var seeder = new ItemSeeder(catalogue);
		var error = Assert.ThrowsException<InvalidDataException>(() => seeder.ValidateClothingPrerequisitesForTesting(context, "industrial"));
		StringAssert.Contains(error.Message, "Clothing/crafts.tsv:3");
		StringAssert.Contains(error.Message, diagnostic);
		Assert.IsFalse(context.ChangeTracker.HasChanges());
		Assert.AreEqual(before, ClothingCraftOwnershipState(context));
		Assert.AreEqual(0, seeder.GetCapturedManifestEntriesForTesting().Count);
	}

	[DataTestMethod]
	[DataRow("unchanged")]
	[DataRow("shared-gates")]
	[DataRow("exact-unmanaged")]
	[DataRow("custom-name")]
	[DataRow("custom-prose")]
	[DataRow("custom-prog-name")]
	[DataRow("custom-prog-text")]
	[DataRow("higher-draft")]
	[DataRow("source-update")]
	public void CraftOwnership_ReadOnlyPreflightAndRerunsPreserveStableCurrentTargets(string change)
	{
		using var context = Context();
		SeedClothingCraftPrerequisites(context);
		var catalogue = CatalogueWithTwoCrafts();
		if (change == "shared-gates") catalogue = catalogue with { Clothing = catalogue.Clothing with
		{
			Crafts = catalogue.Clothing.Crafts.Select(x => x with { MinimumTraitValue = 10 }).ToArray()
		} };
		var fresh = ClothingCraftOwnershipState(context);
		new ItemSeeder(catalogue).ValidateClothingPrerequisitesForTesting(context, "industrial");
		Assert.IsFalse(context.ChangeTracker.HasChanges());
		Assert.AreEqual(fresh, ClothingCraftOwnershipState(context), "Missing future gate progs do not block or mutate a fresh install.");
		InstallClothingCrafts(context, catalogue);
		InstallClothingCrafts(context, catalogue);
		var craft = context.Crafts.Single(x => x.Name == "sew second garters");
		var id = craft.Id;
		var prog = context.FutureProgs.Single(x => x.Id == craft.WhyCannotUseProgId);
		switch (change)
		{
			case "exact-unmanaged":
				context.SeederManagedRecords.RemoveRange(context.SeederManagedRecords.Where(x => x.EntityType == "craft" || x.EntityType == "prog"));
				break;
			case "custom-name": craft.Name = "Builder's renamed recipe"; break;
			case "custom-prose": craft.Blurb = "Builder's recipe text"; break;
			case "custom-prog-name": prog.FunctionName = "BuildersCraftRestriction"; break;
			case "custom-prog-text": prog.FunctionText = "return \"A builder-authored restriction\""; break;
			case "higher-draft":
				var draft = ConflictingCraft(craft.Id, craft.Name, craft.Category);
				draft.RevisionNumber = draft.EditableItem.RevisionNumber = 7;
				draft.EditableItem.RevisionStatus = (int)RevisionStatus.UnderDesign;
				context.Crafts.Add(draft);
				break;
			case "source-update":
				catalogue = catalogue with { Clothing = catalogue.Clothing with
				{
					Crafts = catalogue.Clothing.Crafts.Select(x => x.StableReference == "fixture_craft_second"
						? x with { Name = "sew updated garters", Blurb = "Updated authored source text" } : x).ToArray()
				} };
				break;
		}
		context.SaveChanges();
		context.ChangeTracker.Clear();
		var before = ClothingCraftOwnershipState(context);
		new ItemSeeder(catalogue).ValidateClothingPrerequisitesForTesting(context, "industrial");
		Assert.IsFalse(context.ChangeTracker.HasChanges());
		Assert.AreEqual(before, ClothingCraftOwnershipState(context));
		InstallClothingCrafts(context, catalogue);
		var after = ClothingCraftOwnershipState(context);
		if (change is not ("exact-unmanaged" or "source-update")) Assert.AreEqual(before, after);
		Assert.AreEqual(2, context.Crafts.Select(x => x.Id).Distinct().Count());
		Assert.AreEqual(id, context.SeederManagedRecords.Single(x => x.EntityType == "craft" && x.StableKey == "fixture_craft_second").LogicalId);
		if (change == "source-update") Assert.AreEqual("sew updated garters", context.Crafts.Single(x => x.Id == id).Name);
		context.ChangeTracker.Clear();
		InstallClothingCrafts(context, catalogue);
		Assert.AreEqual(after, ClothingCraftOwnershipState(context));
	}

	[TestMethod]
	public void CraftOwnership_DirectBatchChecksLaterTargetBeforeCreatingEarlierAccessProgs()
	{
		using var context = Context();
		SeedClothingCraftPrerequisites(context);
		var original = CatalogueWithTwoCrafts();
		InstallClothingCrafts(context, original);
		var catalogue = original with { Clothing = original.Clothing with
		{
			Crafts = original.Clothing.Crafts.Select(x => x.StableReference == "fixture_craft_first"
				? x with { MinimumTraitValue = 15, Blurb = "An earlier source update that must not be applied" } : x).ToArray()
		} };
		var seeder = new ItemSeeder(catalogue);
		seeder.ValidateClothingPrerequisitesForTesting(context, "industrial");
		var later = context.Crafts.Single(x => x.Name == "sew second garters");
		context.Crafts.Add(ConflictingCraft(later.Id + 100, later.Name, later.Category));
		context.SaveChanges();
		var before = ClothingCraftOwnershipState(context);
		var error = Assert.ThrowsException<TargetInvocationException>(() => typeof(ItemSeeder)
			.GetMethod("SeedIndustrialisedClothingCrafts", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(seeder, ["industrial"]));
		StringAssert.Contains(error.InnerException!.Message, "Clothing/crafts.tsv:3");
		Assert.IsFalse(context.ChangeTracker.HasChanges());
		Assert.AreEqual(before, ClothingCraftOwnershipState(context));
		Assert.AreEqual(0, seeder.GetCapturedManifestEntriesForTesting().Count);
	}

	[TestMethod]
	public void CraftOwnership_ConflictingProspectiveGateNamesRejectBeforeFreshInstallation()
	{
		using var context = Context();
		SeedClothingCraftPrerequisites(context);
		context.TraitDefinitions.Add(new() { Id = 811, Name = "Tailwind", Type = 0, TraitGroup = "Crafting", ChargenBlurb = "", ValueExpression = "" });
		context.SaveChanges();
		var original = CatalogueWithTwoCrafts();
		// Tailoring/81/minimum 10 and Tailwind/811/minimum 0 share the legacy abbreviated gate name.
		var catalogue = original with { Clothing = original.Clothing with
		{
			Crafts = original.Clothing.Crafts.Select(x => x.StableReference == "fixture_craft_second"
				? x with { Trait = "Tailwind", MinimumTraitValue = 0 } : x).ToArray()
		} };
		var before = ClothingCraftOwnershipState(context);
		var error = Assert.ThrowsException<InvalidDataException>(() => new ItemSeeder(catalogue).ValidateClothingPrerequisitesForTesting(context, "industrial"));
		StringAssert.Contains(error.Message, "Clothing/crafts.tsv:3");
		StringAssert.Contains(error.Message, "Conflicting planned craft access prog");
		Assert.IsFalse(context.ChangeTracker.HasChanges());
		Assert.AreEqual(before, ClothingCraftOwnershipState(context));
	}

	[TestMethod]
	public void CraftOwnership_DirectTraitGateWriterChecksAllGatesBeforeCreatingFirst()
	{
		using var context = Context();
		SeedClothingCraftPrerequisites(context);
		var trait = context.TraitDefinitions.Single();
		var name = (string)typeof(ItemSeeder).GetMethod("TraitGateProgName", BindingFlags.Static | BindingFlags.NonPublic)!
			.Invoke(null, ["Why", trait, 15])!;
		context.FutureProgs.Add(new() { FunctionName = name, FunctionText = "return false", FunctionComment = "Builder-owned", Category = "Other", Subcategory = "Other", ReturnType = 4 });
		context.SaveChanges();
		var seeder = new ItemSeeder(Catalogue());
		seeder.ValidateClothingPrerequisitesForTesting(context, "industrial");
		var before = ClothingCraftOwnershipState(context);
		var error = Assert.ThrowsException<TargetInvocationException>(() => typeof(ItemSeeder)
			.GetMethod("EnsureTraitGateProgs", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(seeder, [trait.Name, 15]));
		StringAssert.Contains(error.InnerException!.Message, "Unmanaged craft access prog conflict");
		Assert.IsFalse(context.ChangeTracker.HasChanges());
		Assert.AreEqual(before, ClothingCraftOwnershipState(context));
		Assert.AreEqual(0, seeder.GetCapturedManifestEntriesForTesting().Count);
	}

	[TestMethod]
	public void CraftOwnership_InactiveCraftConflictDoesNotBlockSelectedEra()
	{
		using var context = Context();
		SeedClothingCraftPrerequisites(context);
		var original = CatalogueWithTwoCrafts();
		InstallClothingCrafts(context, original);
		context.SeederManagedRecords.Single(x => x.EntityType == "craft" && x.StableKey == "fixture_craft_second").LogicalId = 987654;
		context.SaveChanges();
		context.ChangeTracker.Clear();
		var catalogue = original with { Clothing = original.Clothing with
		{
			Bases = original.Clothing.Bases.Select(x => x with { EraAdmissions = ["industrial", "modern"] }).ToArray(),
			Crafts = original.Clothing.Crafts.Select(x => x.StableReference == "fixture_craft_second"
				? x with { EraAdmissions = ["modern"] } : x).ToArray()
		} };
		var before = ClothingCraftOwnershipState(context);
		new ItemSeeder(catalogue).ValidateClothingPrerequisitesForTesting(context, "industrial");
		Assert.IsFalse(context.ChangeTracker.HasChanges());
		Assert.AreEqual(before, ClothingCraftOwnershipState(context));
		// Modern remains non-selectable; test admission by moving this synthetic row into Industrial.
		var admitted = catalogue with { Clothing = catalogue.Clothing with
		{
			Crafts = catalogue.Clothing.Crafts.Select(x => x with { EraAdmissions = ["industrial"] }).ToArray()
		} };
		var error = Assert.ThrowsException<InvalidDataException>(() => new ItemSeeder(admitted).ValidateClothingPrerequisitesForTesting(context, "industrial"));
		StringAssert.Contains(error.Message, "Clothing/crafts.tsv:3");
		StringAssert.Contains(error.Message, "ownership conflict");
	}

	[DataTestMethod]
	[DataRow("phase-order", "consecutive one-based phases")]
	[DataRow("missing-product-item", "missing or ambiguous current item")]
	[DataRow("wrong-product-skin", "missing, ambiguous or wrong-base skin")]
	[DataRow("bad-variable-input", "not supplied by input")]
	[DataRow("missing-fixed-value", "missing characteristic value")]
	[DataRow("missing-material", "missing solid material")]
	[DataRow("unsupported-input", "unsupported input type")]
	[DataRow("unsupported-tool", "unsupported tool type")]
	[DataRow("invalid-tool-state", "invalid state")]
	[DataRow("wrong-child-revision", "unknown input")]
	[DataRow("late-material-dependency", "needs input $i1 before")]
	[DataRow("invalid-lifecycle-parameters", "must accept one character parameter")]
	[DataRow("invalid-lifecycle-static", "invalid static execution mode")]
	public void PreservedCraftGraph_RejectsInvalidCustomizedGraphBeforeMutation(string fault, string diagnostic)
	{
		using var context = Context();
		SeedClothingCraftPrerequisites(context);
		var catalogue = CatalogueWithTwoCrafts();
		InstallClothingCrafts(context, catalogue);
		context.ChangeTracker.Clear();
		var craft = context.Crafts.Include(x => x.EditableItem).Include(x => x.CraftPhases).Include(x => x.CraftInputs)
			.Include(x => x.CraftTools).Include(x => x.CraftProducts).AsSplitQuery()
			.Single(x => x.Name == "sew second garters");
		var input = craft.CraftInputs.Single();
		var tool = craft.CraftTools.Single();
		var product = craft.CraftProducts.Single(x => !x.IsFailProduct);
		var productXml = XElement.Parse(product.Definition);
		switch (fault)
		{
			case "phase-order": craft.FailPhase = 99; break;
			case "missing-product-item": productXml.Element("ProductProducedId")!.Value = "999999"; product.Definition = productXml.ToString(); break;
			case "wrong-product-skin": productXml.Element("Skin")!.Value = "999999"; product.Definition = productXml.ToString(); break;
			case "bad-variable-input": productXml.Element("FixedVariable")!.Name = "Variable"; productXml.Element("Variable")!.SetAttributeValue("inputindex", 99); productXml.Element("Variable")!.Attribute("value")?.Remove(); product.Definition = productXml.ToString(); break;
			case "missing-fixed-value": productXml.Element("FixedVariable")!.SetAttributeValue("value", 999999); product.Definition = productXml.ToString(); break;
			case "missing-material":
				var commodity = XElement.Parse(input.Definition); commodity.Element("Material")!.Value = "999999"; input.Definition = commodity.ToString(); break;
			case "unsupported-input": input.InputType = "Prog"; break;
			case "unsupported-tool": tool.ToolType = "SimpleTool"; break;
			case "invalid-tool-state": tool.DesiredState = 999; break;
			case "wrong-child-revision": input.CraftRevisionNumber++; break;
			case "late-material-dependency":
				product.MaterialDefiningInputIndex = 0;
				var first = craft.CraftPhases.Single(x => x.PhaseNumber == 1);
				var second = craft.CraftPhases.Single(x => x.PhaseNumber == 2);
				first.Echo = first.FailEcho = "$0 use|uses $t1 and finish|finishes $p1.";
				second.Echo = "$0 consume|consumes $i1.";
				second.FailEcho = "$0 consume|consumes $i1 and recover|recovers $f1.";
				break;
			case "invalid-lifecycle-parameters":
				craft.OnUseProgStartId = AddLifecycleProg(context, false, (int)FutureProgStaticType.NotStatic,
					ProgVariableTypes.Text);
				break;
			case "invalid-lifecycle-static":
				craft.OnUseProgStartId = AddLifecycleProg(context, true, 999, null);
				break;
		}
		context.SaveChanges();
		context.ChangeTracker.Clear();
		var before = ClothingCraftOwnershipState(context);
		var error = Assert.ThrowsException<InvalidDataException>(() =>
			new ItemSeeder(catalogue).ValidateClothingPrerequisitesForTesting(context, "industrial"));
		StringAssert.Contains(error.Message, "Clothing/crafts.tsv:3");
		StringAssert.Contains(error.Message, diagnostic);
		Assert.IsFalse(context.ChangeTracker.HasChanges());
		Assert.AreEqual(before, ClothingCraftOwnershipState(context));
	}

	[TestMethod]
	public void PreservedCraftGraph_AcceptsBuilderSupportedAnyParameterLifecycleProgWithoutMutation()
	{
		using var context = Context();
		SeedClothingCraftPrerequisites(context);
		var catalogue = CatalogueWithTwoCrafts();
		InstallClothingCrafts(context, catalogue);
		var craft = context.Crafts.Single(x => x.Name == "sew second garters");
		craft.OnUseProgStartId = AddLifecycleProg(context, true, (int)FutureProgStaticType.StaticByParameters, null);
		context.SaveChanges();
		context.ChangeTracker.Clear();
		var before = ClothingCraftOwnershipState(context);
		new ItemSeeder(catalogue).ValidateClothingPrerequisitesForTesting(context, "industrial");
		Assert.IsFalse(context.ChangeTracker.HasChanges());
		Assert.AreEqual(before, ClothingCraftOwnershipState(context));
	}

	private static long AddLifecycleProg(FuturemudDatabaseContext context, bool acceptsAnyParameters, int staticType,
		ProgVariableTypes? parameterType)
	{
		const long id = 9876500;
		var prog = new MudSharp.Models.FutureProg
		{
			Id = id,
			FunctionName = "ClothingCraftLifecycleFixture",
			FunctionComment = "Test-only preserved craft lifecycle hook.",
			FunctionText = "return",
			ReturnType = (long)ProgVariableTypes.Void,
			Category = "Clothing",
			Subcategory = "Crafts",
			AcceptsAnyParameters = acceptsAnyParameters,
			StaticType = staticType
		};
		if (parameterType is { } type)
			prog.FutureProgsParameters.Add(new FutureProgsParameter
			{
				FutureProgId = id,
				ParameterIndex = 0,
				ParameterName = "actor",
				ParameterType = (long)type
			});
		context.FutureProgs.Add(prog);
		return id;
	}

	private static Craft ConflictingCraft(long id, string name, string category) => new()
	{
		Id = id, Name = name, Category = category, Blurb = "Builder recipe", ActionDescription = "sewing",
		ActiveCraftItemSdesc = "an in-progress garter craft", QualityFormula = "0",
		EditableItem = new() { RevisionStatus = (int)RevisionStatus.Current, BuilderAccountId = 1, BuilderDate = DateTime.UtcNow, BuilderComment = "test" }
	};

	private static IndustrialisedItemCatalogueDocument CatalogueWithTwoCrafts()
	{
		var original = Catalogue();
		var document = IndustrialisedClothingCraftPlanTests.Document();
		var keys = new[] { "fixture_craft_first", "fixture_craft_second" };
		return original with { Clothing = original.Clothing with
		{
			Crafts = keys.Select((key, index) => document.Crafts.Single() with
			{
				Source = new("Clothing/crafts.tsv", index + 2), StableReference = key, Name = index == 0 ? "sew first garters" : "sew second garters",
				Trait = "Tailoring", MinimumTraitValue = (index + 1) * 10, EraAdmissions = ["industrial"], ReviewStatus = ClothingReviewStatus.Reviewed
			}).ToArray(),
			CraftPhases = keys.SelectMany(key => document.CraftPhases.Select(x => x with { CraftReference = key })).ToArray(),
			CraftInputs = keys.SelectMany(key => document.CraftInputs.Select(x => x with { CraftReference = key, Reference = "wool" })).ToArray(),
			CraftTools = keys.SelectMany(key => document.CraftTools.Select(x => x with { CraftReference = key, Tag = "Needle" })).ToArray(),
			CraftProducts = keys.SelectMany(key => document.CraftProducts.Select(x => x with
			{
				CraftReference = key, Reference = x.FailureProduct ? "wool" : Garters, SkinReference = ""
			})).ToArray(),
			CraftColours = keys.SelectMany(key => document.CraftColours.Select(x => x with { CraftReference = key, Value = "blue", InputOrder = null })).ToArray()
		} };
	}

	private static void SeedClothingCraftPrerequisites(FuturemudDatabaseContext context)
	{
		SeedSkinPrerequisites(context);
		context.TraitDefinitions.Add(new() { Id = 81, Name = "Tailoring", Type = 0, TraitGroup = "Crafting", ChargenBlurb = "", ValueExpression = "" });
		context.Tags.Add(new() { Id = 901, Name = "Needle" });
		context.SaveChanges();
	}

	private static void InstallClothingCrafts(FuturemudDatabaseContext context, IndustrialisedItemCatalogueDocument catalogue)
	{
		var seeder = new ItemSeeder(catalogue);
		typeof(ItemSeeder).GetMethod("InitialiseCraftAuthoringForTesting", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(seeder, [context]);
		seeder.ValidateClothingPrerequisitesForTesting(context, "industrial");
		seeder.ApplyClothingReuseForTesting("industrial");
		context.SaveChanges();
		typeof(ItemSeeder).GetMethod("SeedIndustrialisedClothingPresentations", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(seeder, ["industrial"]);
		typeof(ItemSeeder).GetMethod("SeedIndustrialisedClothingCrafts", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(seeder, ["industrial"]);
		context.SaveChanges();
	}

	private static string ClothingCraftOwnershipState(FuturemudDatabaseContext context) => System.Text.Json.JsonSerializer.Serialize(new
	{
		Clothing = OutfitOwnershipState(context),
		Crafts = context.Crafts.Include(x => x.EditableItem).Include(x => x.CraftPhases).Include(x => x.CraftInputs)
			.Include(x => x.CraftTools).Include(x => x.CraftProducts).AsNoTracking().OrderBy(x => x.Id).ThenBy(x => x.RevisionNumber).AsEnumerable()
			.Select(x => new
			{
				x.Id, x.RevisionNumber, x.Name, x.Category, x.Blurb, x.EditableItem.RevisionStatus,
				x.AppearInCraftsListProgId, x.CanUseProgId, x.WhyCannotUseProgId,
				Definition = typeof(ItemSeeder).GetMethod("BuildLiveCraftManifestDefinition", BindingFlags.Static | BindingFlags.NonPublic)!.Invoke(null, [x]),
				Phases = x.CraftPhases.OrderBy(y => y.PhaseNumber).Select(y => new { y.PhaseNumber, y.Echo, y.FailEcho }).ToArray(),
				Inputs = x.CraftInputs.OrderBy(y => y.Id).Select(y => new { y.Id, y.Definition }).ToArray(),
				Tools = x.CraftTools.OrderBy(y => y.Id).Select(y => new { y.Id, y.Definition }).ToArray(),
				Products = x.CraftProducts.OrderBy(y => y.Id).Select(y => new { y.Id, y.Definition }).ToArray()
			}).ToArray(),
		Progs = context.FutureProgs.Include(x => x.FutureProgsParameters).AsNoTracking().OrderBy(x => x.Id).AsEnumerable()
			.Select(x => new { x.Id, x.FunctionName, x.FunctionText, x.ReturnType, x.StaticType, x.AcceptsAnyParameters,
				Parameters = x.FutureProgsParameters.OrderBy(y => y.ParameterIndex).Select(y => new { y.ParameterIndex, y.ParameterName, y.ParameterType }).ToArray() }).ToArray()
	});
}
