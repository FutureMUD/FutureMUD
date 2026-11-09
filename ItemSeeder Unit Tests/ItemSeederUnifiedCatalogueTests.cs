#nullable enable

using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Database;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemSeederUnifiedCatalogueTests
{

	private static bool IsLiteralData(ExpressionSyntax expression) => expression switch
	{
		BinaryExpressionSyntax binary when binary.IsKind(SyntaxKind.BitwiseOrExpression) => IsLiteralData(binary.Left) && IsLiteralData(binary.Right),
		LiteralExpressionSyntax => true,
		PrefixUnaryExpressionSyntax unary => unary.Operand is LiteralExpressionSyntax,
		MemberAccessExpressionSyntax member => member.Expression is IdentifierNameSyntax name && new[]
		{
			"SizeCategory", "ItemQuality", "ItemSeederOwnershipPolicy", "CatalogueReadiness", "FoodCatalogueScope", "FoodCatalogueKind",
			"IndustrialisedFoodPhysicalKind", "IndustrialisedFoodReviewState", "FoodCatalogueFamily", "FoodNutritionBand", "FoodFreshnessBand", "FoodAdmissionProfile", "OutfitTemplateItemPlacement",
			"Difficulty", "Outcome", "ExertionLevel", "DesiredItemState", "CatalogueCharacteristicMode", "CatalogueRecipeGates", "ProgVariableTypes", "LearnableType", "VehicleScale", "VehicleOccupantSlotType",
			"VehicleMovementProfileType", "VehicleMovementEnvironment", "RouteVehiclePropulsionMode", "VehiclePropulsionType",
			"VehicleDamageEffectTargetType", "VehicleSystemStatus", "VehicleAccessPointType"
		}.Contains(name.Identifier.ValueText),
		ObjectCreationExpressionSyntax creation => (creation.ArgumentList?.Arguments.All(argument => IsLiteralData(argument.Expression)) ?? true) &&
			(creation.Initializer?.Expressions.All(value => value is AssignmentExpressionSyntax assignment && IsLiteralData(assignment.Right)) ?? true),
		ImplicitObjectCreationExpressionSyntax creation => creation.ArgumentList.Arguments.All(argument => IsLiteralData(argument.Expression)) &&
			(creation.Initializer?.Expressions.All(value => value is AssignmentExpressionSyntax assignment && IsLiteralData(assignment.Right)) ?? true),
		CollectionExpressionSyntax collection => collection.Elements.All(element => element is ExpressionElementSyntax item && IsLiteralData(item.Expression)),
		_ => false
	};
	[TestMethod]
	public void SyntheticCapturePrerequisites_NeverQueryOrFlushRelationalContexts()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseMySql("server=127.0.0.1;database=unused;uid=unused;password=unused", ServerVersion.Parse("8.0.36-mysql"))
			.Options;
		var relational = new Mock<FuturemudDatabaseContext>(options) { CallBase = true };
		relational.Setup(context => context.SaveChanges()).Throws(new InvalidOperationException("Capture must not flush relational changes."));
		using (var context = relational.Object)
		{
			ItemSeeder.PrepareSyntheticCapturePrerequisites(context, "antiquity");
			relational.Verify(context => context.SaveChanges(), Times.Never);
			relational.VerifyGet(context => context.Tags, Times.Never);
		}
		using var synthetic = ItemSeederProgram.CreateItemManifestCaptureContext();
		ItemSeeder.PrepareSyntheticCapturePrerequisites(synthetic, "medieval");
		Assert.AreEqual(0, synthetic.Tags.Count());
		ItemSeeder.PrepareSyntheticCapturePrerequisites(synthetic, "antiquity");
		ItemSeeder.PrepareSyntheticCapturePrerequisites(synthetic, "antiquity");
		Assert.AreEqual(1, synthetic.Tags.Count(tag => tag.Name == "Meat"));
	}

	[TestMethod]
	public void SourceFingerprint_TracksNestedCatalogueAndRootItemHelpers()
	{
		var root = Path.Combine(Path.GetTempPath(), "catalogue-fingerprint-" + Guid.NewGuid().ToString("N"));
		var seeders = Path.Combine(root, "ItemSeeder", "Seeders");
		var content = Path.Combine(seeders, "ItemSeeder", "Catalogue", "Content");
		Directory.CreateDirectory(content);
		try
		{
			var first = ItemSeederManifestCatalogue.ComputeSourceFingerprint(root);
			var nested = Path.Combine(content, "Stock.cs"); File.WriteAllText(nested, "first\r\n");
			var second = ItemSeederManifestCatalogue.ComputeSourceFingerprint(root); Assert.AreNotEqual(first, second);
			File.WriteAllText(nested, "first\n"); Assert.AreEqual(second, ItemSeederManifestCatalogue.ComputeSourceFingerprint(root));
			File.WriteAllText(Path.Combine(seeders, "ItemSeeder.ClothingItemOwnership.cs"), "ownership");
			Assert.AreNotEqual(second, ItemSeederManifestCatalogue.ComputeSourceFingerprint(root));
		}
		finally { Directory.Delete(root, true); }
	}

	[TestMethod]
	public void FullComparison_DetectsLostNotesSourceGraphsAndMalformedCaptures()
	{
		var root = Path.Combine(Path.GetTempPath(), "catalogue-compare-" + Guid.NewGuid().ToString("N"));
		var before = Path.Combine(root, "before"); var after = Path.Combine(root, "after");
		Directory.CreateDirectory(before); Directory.CreateDirectory(after);
		try
		{
			var row = new { Identity = new { EntityType = "item", StableKey = "test", Module = "test", EraAdmissions = new[] { "medieval" }, Dependencies = Array.Empty<string>(), OwnershipPolicy = "StockAggregate" },
				SymbolicDefinition = new { Weight = 5 }, Supplement = new { BuilderNotes = "Retain this note" } };
			var text = JsonSerializer.Serialize(row, CatalogueDefinitionCapture.JsonOptions);
			foreach (var directory in new[] { before, after })
			{
				File.WriteAllText(Path.Combine(directory, "definitions.ndjson"), text + "\n");
				File.WriteAllText(Path.Combine(directory, "source-graphs.json"), "{}");
			}
			Assert.AreEqual(0, CatalogueCaptureCommand.Compare(before, after).Count);
			File.WriteAllText(Path.Combine(after, "definitions.ndjson"), text.Replace("Retain this note", "lost"));
			Assert.IsTrue(CatalogueCaptureCommand.Compare(before, after).Any(x => x.EndsWith("supplement", StringComparison.Ordinal)));
			File.WriteAllText(Path.Combine(after, "definitions.ndjson"), text + "\n" + text);
			Assert.ThrowsExactly<InvalidDataException>(() => CatalogueCaptureCommand.Compare(before, after));
			File.WriteAllText(Path.Combine(after, "definitions.ndjson"), text);
			File.WriteAllText(Path.Combine(after, "source-graphs.json"), "{\"design\":\"retained\"}");
			Assert.IsTrue(CatalogueCaptureCommand.Compare(before, after).Contains("source-only catalogue graphs differ"));
			File.WriteAllText(Path.Combine(after, "definitions.ndjson"), "");
			Assert.ThrowsExactly<InvalidDataException>(() => CatalogueCaptureCommand.Compare(before, after));
		}
		finally { Directory.Delete(root, true); }
	}

	[TestMethod]
	public void RepresentativeVehicle_PersistsChildGraphAndPreservesCustomization()
	{
		using var context = Fixture();
		new ItemSeeder().InstallRepresentativeVehicleForTesting(context); context.SaveChanges();
		var vehicle = context.VehicleProtos.Single(); var id = vehicle.Id;
		Assert.AreEqual(1, context.VehicleCompartmentProtos.Count());
		Assert.AreEqual(1, context.VehicleCargoSpaceProtos.Count());
		Assert.AreEqual(2, context.VehicleTowPointProtos.Count());
		Assert.AreEqual(2, context.VehicleDamageZoneProtos.Count());
		vehicle.Description = "Builder's retained vehicle description"; context.SaveChanges();
		new ItemSeeder().InstallRepresentativeVehicleForTesting(context); context.SaveChanges();
		Assert.AreEqual(id, context.VehicleProtos.Single().Id);
		Assert.AreEqual("Builder's retained vehicle description", context.VehicleProtos.Single().Description);
		Assert.AreEqual(2, context.VehicleTowPointProtos.Count());
	}

	[TestMethod]
	public void InvalidAndDevelopmentDefinitions_FailBeforeApplication()
	{
		var item = RepresentativeCatalogue.Braies();
		Assert.ThrowsExactly<InvalidDataException>(() => CatalogueValidator.Validate(item with { WeightInGrams = double.NaN }));
		Assert.ThrowsExactly<InvalidDataException>(() => CatalogueValidator.Validate(item.Metadata with { Readiness = CatalogueReadiness.DependencyReviewed }));
		Assert.ThrowsExactly<InvalidDataException>(() => CatalogueValidator.Validate(RepresentativeCatalogue.WickerBeehive() with { FailPhase = 8 }));
	}

	[TestMethod]
	public void CaptureItem_RetainsMilitaryTagAndVariableComponentJoins()
	{
		GameItemProto Build(bool capture)
		{
			using var context = Fixture();
			context.Tags.AddRange(new Tag { Id = 101, Name = "Military Equipment", ParentId = 4 },
				new Tag { Id = 102, Name = "Armour", ParentId = 101 });
			context.GameItemComponentProtos.Add(new GameItemComponentProto
			{
				Id = 101, RevisionNumber = 2, Name = "Variable_2Colours", Description = "Variable test component",
				Type = "Variable", Definition = "<Definition />", EditableItem = new EditableItem { RevisionStatus = 4, BuilderAccountId = 1 }
			});
			context.SaveChanges();
			var seeder = new ItemSeeder();
			typeof(ItemSeeder).GetField("_manifestCaptureOnly", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.SetValue(seeder, capture);
			return seeder.InstallCatalogueItemForTesting(context, RepresentativeCatalogue.Braies() with
			{
				Tags = ["Functions / Military Equipment / Armour"],
				Components = ["Variable_2Colours"]
			});
		}
		var installed = Build(false);
		var captured = Build(true);
		CollectionAssert.AreEqual(installed.GameItemProtosTags.Select(link => link.TagId).ToArray(), captured.GameItemProtosTags.Select(link => link.TagId).ToArray());
		Assert.AreEqual(102L, captured.GameItemProtosTags.Single().TagId);
		var component = captured.GameItemProtosGameItemComponentProtos.Single();
		Assert.AreEqual("Variable_2Colours", component.GameItemComponent.Name);
		Assert.AreEqual(101L, component.GameItemComponentProtoId);
		Assert.AreEqual(2, component.GameItemComponentRevision);
		Assert.AreSame(captured, component.GameItemProto);
	}

	[TestMethod]
	public void GarmentAndFood_RerunsPreserveIdsAndBuilderCustomizations()
	{
		using var context = Fixture();
		var item = new ItemSeeder().InstallCatalogueItemForTesting(context, RepresentativeCatalogue.Braies());
		context.SaveChanges();
		var food = new ItemSeeder().InstallCatalogueFoodForTesting(context);
		context.SaveChanges();
		var ids = (item.Id, food.Id);
		item.ShortDescription = "a builder's linen garment"; item.Weight = 321;
		food.ShortDescription = "a builder's special pottage"; food.Weight = 777;
		context.SaveChanges();
		var retainedItem = new ItemSeeder().InstallCatalogueItemForTesting(context, RepresentativeCatalogue.Braies());
		var retainedFood = new ItemSeeder().InstallCatalogueFoodForTesting(context);
		context.SaveChanges();
		Assert.AreEqual(ids.Item1, retainedItem.Id); Assert.AreEqual(ids.Item2, retainedFood.Id);
		Assert.AreEqual("a builder's linen garment", retainedItem.ShortDescription); Assert.AreEqual(321.0, retainedItem.Weight);
		Assert.AreEqual("a builder's special pottage", retainedFood.ShortDescription); Assert.AreEqual(777.0, retainedFood.Weight);
		Assert.AreEqual(2, context.GameItemProtos.Count());
	}

	[TestMethod]
	public void TypedCraft_ResolvesActualIdsAndPreservesCustomizedGraphOnRerun()
	{
		using var context = Fixture();
		context.GameItemProtos.Add(new GameItemProto
		{
			Id = 501, RevisionNumber = 0, Name = "beehive", UniqueName = "antiquity_wicker_beehive",
			ShortDescription = "a woven wicker beehive", MaterialId = 1, Keywords = "beehive", FullDescription = "A woven beehive.", MorphEmote = "",
			EditableItem = new EditableItem { RevisionStatus = 4, BuilderAccountId = 1 }
		});
		context.SaveChanges();
		var craft = new ItemSeeder().InstallCatalogueCraftForTesting(context, RepresentativeCatalogue.WickerBeehive());
		context.SaveChanges();
		Assert.AreEqual(4, craft.CraftPhases.Count); Assert.AreEqual(3, craft.CraftInputs.Count);
		Assert.AreEqual(2, craft.CraftTools.Count); Assert.AreEqual(1, craft.CraftProducts.Count);
		Assert.AreEqual("501", XDocument.Parse(craft.CraftProducts.Single().Definition).Root!.Element("ProductProducedId")!.Value);
		Assert.AreEqual("1800", XDocument.Parse(craft.CraftInputs.First().Definition).Root!.Element("Weight")!.Value);
		var id = craft.Id; craft.Blurb = "Builder's apiary recipe"; context.SaveChanges();
		var rerun = new ItemSeeder().InstallCatalogueCraftForTesting(context, RepresentativeCatalogue.WickerBeehive());
		context.SaveChanges();
		Assert.AreEqual(id, rerun.Id); Assert.AreEqual("Builder's apiary recipe", rerun.Blurb);
		Assert.AreEqual(1, context.Crafts.Count());
	}

	internal static FuturemudDatabaseContext Fixture()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>().UseInMemoryDatabase(Guid.NewGuid().ToString())
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning)).Options;
		var context = new FuturemudDatabaseContext(options);
		context.Accounts.Add(new Account { Id = 1, Name = "Catalogue test", CultureName = "en-AU", TimeZoneId = "UTC", UnitPreference = "Metric" });
		context.TraitDefinitions.Add(new TraitDefinition { Id = 1, Name = "Basketry", TraitGroup = "Crafting", ChargenBlurb = "", ValueExpression = "" });
		var materials = new[] { "linen", "fish", "willow", "pitch", "oak" };
		for (var i = 0; i < materials.Length; i++) context.Materials.Add(new Material { Id = i + 1, Name = materials[i], MaterialDescription = materials[i] });
		var tags = new[] { "Market", "Market / Clothing", "Market / Clothing / Standard Clothing", "Functions", "Functions / Worn Items", "Functions / Worn Items / Bodywear", "Basketry Splint", "Spun Yarn", "Prepared Pitch", "Basket Knife", "Packing Bone" };
		for (var i = 0; i < tags.Length; i++)
		{
			var parent = tags[i].Contains(" / ", StringComparison.Ordinal) ? Array.IndexOf(tags, tags[i][..tags[i].LastIndexOf(" / ", StringComparison.Ordinal)]) + 1 : 0;
			context.Tags.Add(new Tag { Id = i + 1, Name = tags[i].Split(" / ").Last(), ParentId = parent == 0 ? null : parent });
		}
		var components = RepresentativeCatalogue.Braies().Components.Concat(new[] { "Destroyable_Misc", "Destroyable_WoodenHeavy", "Container_Open_Bin" }).Distinct().ToArray();
		for (var i = 0; i < components.Length; i++) context.GameItemComponentProtos.Add(new GameItemComponentProto
		{
			Id = i + 1, RevisionNumber = 0, Name = components[i], Description = "Catalogue fixture component", Type = "Test", Definition = "<Definition />",
			EditableItem = new EditableItem { RevisionStatus = 4, BuilderAccountId = 1 }
		});
		context.SaveChanges();
		return context;
	}
}
