#nullable enable

using DatabaseSeeder.Seeders;
using DatabaseSeeder;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemSeederCompiledRecipeTests
{
	[TestMethod]
	public void CompiledRecipes_EveryChildSerializesLikeTheIndependentLegacyParser()
	{
		var eras = new HashSet<string>(["antiquity", "medieval", "renaissance", "earlymodern", "industrial"], StringComparer.OrdinalIgnoreCase);
		var recipes = CompiledRecipeCatalogue.Select(eras, true).ToArray();
		var characteristics = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
		void Reference(string definition, string? value)
		{
			if (!characteristics.TryGetValue(definition, out var values)) characteristics[definition] = values = new(StringComparer.OrdinalIgnoreCase);
			if (value is not null) values.Add(value);
		}
		foreach (var recipe in recipes)
		{
			foreach (var input in recipe.Inputs)
				foreach (var requirement in input switch
				{
					CatalogueCommodityRecipeInput value => value.Characteristics,
					CatalogueCommodityTagRecipeInput value => value.Characteristics,
					_ => Array.Empty<CatalogueCharacteristicRequirement>()
				}) Reference(requirement.Definition, requirement.Value);
			foreach (var product in recipe.Products.Concat(recipe.FailProducts))
			{
				if (product is CatalogueCommodityRecipeProduct commodity)
					foreach (var characteristic in commodity.Characteristics) Reference(characteristic.Definition, characteristic.Value);
				if (product is CatalogueVariableRecipeProduct variable)
					foreach (var mapping in variable.Variables) Reference(mapping.Definition, null);
			}
		}
		using var context = Program.CreateItemManifestCaptureContext();
		foreach (var (name, values) in characteristics)
		{
			var definition = new CharacteristicDefinition { Name = name, Pattern = "", Description = name, Model = "standard", Definition = "<Definition />" };
			context.CharacteristicDefinitions.Add(definition);
			foreach (var value in values)
				context.CharacteristicValues.Add(new CharacteristicValue { Name = value, Definition = definition, Value = value, AdditionalValue = "" });
		}
		context.SaveChanges();
		var seeder = new ItemSeeder();
		seeder.CaptureManifest(context, ItemSeederManifestCatalogue.FindRepositoryRoot());
		Assert.AreEqual(49623, seeder.CompareRecipeSerializersForTesting(recipes), "Every native-qualified input, tool and success/failure product must be checked.");
	}

	[TestMethod]
	public void CompiledRecipes_ValidateEverySelectedDefinitionAndPreserveFixedCounts()
	{
		var eras = new HashSet<string>(["antiquity", "medieval", "renaissance", "earlymodern", "industrial"], StringComparer.OrdinalIgnoreCase);
		var recipes = CompiledRecipeCatalogue.Select(eras, true).ToArray();
		Assert.AreEqual(14040, recipes.Length);
		FrozenCatalogueContract.AssertTypedRecipes(recipes);
		Assert.AreEqual(recipes.Length, recipes.Select(recipe => recipe.Metadata.StableKey).Distinct(StringComparer.Ordinal).Count());
		foreach (var recipe in recipes) CatalogueValidator.Validate(recipe);
		Assert.AreEqual(65, recipes.Count(recipe => recipe.IndustrialCaptureOnly));
		Assert.IsTrue(recipes.All(recipe => !recipe.UseExplicitStableReference), "The migration must preserve natural-key reconciliation.");
		Assert.AreEqual(6, recipes.SelectMany(recipe => recipe.Inputs).Select(input => input.GetType()).Distinct().Count());
		Assert.AreEqual(9, recipes.SelectMany(recipe => recipe.Products.Concat(recipe.FailProducts)).Select(product => product.GetType()).Distinct().Count());
	}

	[TestMethod]
	public void IncompleteLegacyCaptureRows_AreRetainedWithoutBecomingInstallableStock()
	{
		var research = LegacyRecipeCaptureResearch.Definitions().ToArray();
		Assert.AreEqual(36, research.Length);
		CollectionAssert.AreEquivalent(LegacyRecipeCaptureResearch.Keys.ToArray(), research.Select(recipe => recipe.Metadata.StableKey).ToArray());
		Assert.IsTrue(research.All(recipe => recipe.Metadata.Readiness == CatalogueReadiness.DependencyReviewed));
		var eras = new HashSet<string>(["antiquity", "medieval", "renaissance", "earlymodern", "industrial"], StringComparer.OrdinalIgnoreCase);
		Assert.IsFalse(CompiledRecipeCatalogue.Select(eras, true).Any(recipe => LegacyRecipeCaptureResearch.Keys.Contains(recipe.Metadata.StableKey)));
	}

	[TestMethod]
	public void PreviouslyInstalledLegacyCaptureRoute_RetainsItsOwnershipOnMigration()
	{
		using var context = ItemSeederUnifiedCatalogueTests.Fixture();
		var applied = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		var record = new SeederManagedRecord
		{
			Seeder = "Items", EntityType = "craft", StableKey = LegacyRecipeCaptureResearch.Keys.First(),
			Module = "crafts", LogicalId = 100, RevisionNumber = 2, AppliedFingerprint = "original stock fingerprint",
			ManifestVersion = ItemSeederManifestCatalogue.ManifestVersion, AppliedAt = applied, Retired = false
		};
		context.SeederManagedRecords.Add(record); context.SaveChanges();
		var seeder = new ItemSeeder();
		const System.Reflection.BindingFlags hidden = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
		typeof(ItemSeeder).GetField("_context", hidden)!.SetValue(seeder, context);
		typeof(ItemSeeder).GetMethod("RetireMissingManagedRecords", hidden)!.Invoke(seeder, null);
		Assert.IsFalse(record.Retired);
		Assert.AreEqual(100L, record.LogicalId);
		Assert.AreEqual(2, record.RevisionNumber);
		Assert.AreEqual("original stock fingerprint", record.AppliedFingerprint);
		Assert.AreEqual(applied, record.AppliedAt);
	}

	[TestMethod]
	public void CompiledRecipes_RejectInvalidVolumeAndVariableMappings()
	{
		var recipe = Recipes01Stock.PotteryMakeUnlitWorkshopHearth();
		Assert.ThrowsException<InvalidDataException>(() => CatalogueValidator.Validate(recipe with
		{
			Products = [new CatalogueLiquidRecipeProduct("vessel", 1, null, "water", 0)]
		}));
		Assert.ThrowsException<InvalidDataException>(() => CatalogueValidator.Validate(recipe with
		{
			Products = [new CatalogueVariableRecipeProduct("item", 1, null, [new("Colour", 0)])]
		}));
		Assert.ThrowsException<InvalidDataException>(() => CatalogueValidator.Validate(recipe with
		{
			Products = [new CatalogueVariableRecipeProduct("item", 1, null, [new("Colour", 1), new("Colour", 1)])]
		}));
	}

	[TestMethod]
	public void CompiledRecipes_InactiveResearchRemainsExcludedFromOrdinaryIndustrialSelection()
	{
		var eras = new HashSet<string>(["industrial"], StringComparer.OrdinalIgnoreCase);
		var installed = CompiledRecipeCatalogue.Select(eras, false).ToArray();
		Assert.IsFalse(installed.Any(recipe => recipe.IndustrialCaptureOnly));
		Assert.IsTrue(installed.Where(recipe => recipe.RequiredAnyEra.Contains("industrial"))
			.All(recipe => recipe.Metadata.EraAdmissions.Contains("industrial")));
	}

	[TestMethod]
	public void CompiledRecipeGates_AreReadOnlyLiteralRecords()
	{
		var file = Path.Combine(DatabaseSeeder.ItemSeederManifestCatalogue.FindRepositoryRoot(),
			"DatabaseSeeder/Seeders/ItemSeeder/Catalogue/Assemblies/Contracts/CatalogueRecipeGates.cs");
		var fields = Directory.EnumerateFiles(Path.GetDirectoryName(file)!, "*CatalogueRecipeGates.cs")
			.SelectMany(path => CSharpSyntaxTree.ParseText(File.ReadAllText(path)).GetRoot().DescendantNodes().OfType<FieldDeclarationSyntax>()).ToArray();
		Assert.AreEqual(448, fields.Length);
		foreach (var field in fields)
		{
			Assert.IsTrue(field.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.ReadOnlyKeyword)));
			Assert.IsFalse(field.DescendantNodes().Any(node => node is InvocationExpressionSyntax or LambdaExpressionSyntax or QueryExpressionSyntax));
		}
	}
}
