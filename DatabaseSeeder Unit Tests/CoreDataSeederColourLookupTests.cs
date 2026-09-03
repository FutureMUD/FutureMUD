#nullable enable

using System;
using CultureInfo = System.Globalization.CultureInfo;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CoreDataSeederColourLookupTests
{
	private static FuturemudDatabaseContext Stock()
	{
		var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
		var definition = new CharacteristicDefinition
		{
			Id = 9000, Name = "Prior stock characteristic", Pattern = "^prior$", Model = "standard", Description = "Prior stock"
		};
		context.CharacteristicDefinitions.Add(definition);
		context.CharacteristicValues.Add(new CharacteristicValue { Id = 1, Name = "prior", Value = "prior", Definition = definition });
		context.SaveChanges();
		new CoreDataSeeder().SeedColours(context);
		return context;
	}

	private static CharacteristicValue Value(FuturemudDatabaseContext context, long colourId) =>
		context.CharacteristicValues.Single(x => x.Definition.Name == "Colour" && x.Value == colourId.ToString(CultureInfo.InvariantCulture));

	private static void MakeLegacy(FuturemudDatabaseContext context)
	{
		foreach (var repair in CoreDataSeeder.StockColourLookupRepairs) Value(context, repair.Secondary.Id).Name = repair.Name;
		foreach (var definition in context.CharacteristicDefinitions.Where(x => x.Name == "Colour" || x.Name == "Colour1" || x.Name == "Colour2" || x.Name == "Colour3"))
			definition.Pattern = definition.Pattern.Trim('^', '$');
		context.SaveChanges();
	}

	private static string Snapshot(FuturemudDatabaseContext context) => JsonSerializer.Serialize(new
	{
		Values = context.CharacteristicValues.OrderBy(x => x.Id).Select(x => new
		{
			x.Id, x.Name, x.DefinitionId, x.Value, x.Default, x.AdditionalValue, x.Pluralisation, x.FutureProgId, x.OngoingValidityProgId
		}).ToArray(),
		Colours = context.Colours.OrderBy(x => x.Id).ToArray(),
		Definitions = context.CharacteristicDefinitions.OrderBy(x => x.Id)
			.Select(x => new { x.Id, x.Name, x.Pattern, x.ParentId, x.Model, x.Type, x.Description }).ToArray(),
		Profiles = context.CharacteristicProfiles.OrderBy(x => x.Id).Select(x => new { x.Id, x.Name, x.Type, x.TargetDefinitionId, x.Definition }).ToArray()
	});

	[TestMethod]
	public void FreshStock_UsesExactUniqueLookupNamesWithoutChangingColourIdentities()
	{
		using var context = Stock();
		Assert.AreEqual(34, CoreDataSeeder.StockColourLookupRepairs.Count);
		var values = context.CharacteristicValues.Where(x => x.Definition.Name == "Colour").ToArray();
		Assert.AreEqual(values.Length, values.Select(x => x.Name).Distinct(StringComparer.OrdinalIgnoreCase).Count());
		foreach (var repair in CoreDataSeeder.StockColourLookupRepairs)
		{
			Assert.AreEqual(repair.Name, Value(context, repair.Primary.Id).Name);
			Assert.AreEqual(repair.QualifiedName, Value(context, repair.Secondary.Id).Name);
			Assert.IsTrue(repair.Primary.Matches(context.Colours.Single(x => x.Id == repair.Primary.Id), repair.Name));
			Assert.IsTrue(repair.Secondary.Matches(context.Colours.Single(x => x.Id == repair.Secondary.Id), repair.Name));
		}
		var snapshot = Snapshot(context);
		new CoreDataSeeder().SeedColours(context);
		Assert.AreEqual(snapshot, Snapshot(context));
	}

	[TestMethod]
	public void ClothingProfiles_UseExistingExactColourValuesAndRerunAdditively()
	{
		using var context = Stock();
		var expected = new[]
		{
			("Clothing_Leather_Colours", new[] { "black", "brown", "light brown", "dark brown", "tan brown", "reddish brown", "red", "green" }),
			("Clothing_Wood_Colours", new[] { "light brown", "brown", "dark brown", "reddish brown", "beige" }),
			("Clothing_Lacquer_Colours", new[] { "dark brown", "black", "red" })
		};
		var colourId = context.CharacteristicDefinitions.Single(x => x.Name == "Colour").Id;
		var valueIds = context.CharacteristicValues.Select(x => x.Id).OrderBy(x => x).ToArray();
		foreach (var (name, values) in expected)
		{
			var profile = context.CharacteristicProfiles.Single(x => x.Name == name);
			Assert.AreEqual("Standard", profile.Type);
			Assert.AreEqual(colourId, profile.TargetDefinitionId);
			CollectionAssert.AreEqual(values, XElement.Parse(profile.Definition).Elements("Value").Select(x => x.Value).ToArray());
			foreach (var value in values)
				Assert.AreEqual(1, context.CharacteristicValues.Count(x => x.DefinitionId == colourId && x.Name == value));
		}
		// Simulate a pre-extension database. No existing character/item selection is removed.
		context.CharacteristicProfiles.RemoveRange(context.CharacteristicProfiles.Where(x => x.Name.StartsWith("Clothing_")));
		context.SaveChanges();
		var retained = context.CharacteristicProfiles.ToDictionary(x => x.Id, x => x.Definition);
		new CoreDataSeeder().SeedColours(context);
		foreach (var (id, xml) in retained) Assert.AreEqual(xml, context.CharacteristicProfiles.Single(x => x.Id == id).Definition);
		CollectionAssert.AreEqual(valueIds, context.CharacteristicValues.Select(x => x.Id).OrderBy(x => x).ToArray());
		Assert.AreEqual(3, context.CharacteristicProfiles.Count(x => x.Name.StartsWith("Clothing_")));
		var populated = Snapshot(context);
		new CoreDataSeeder().SeedColours(context);
		Assert.AreEqual(populated, Snapshot(context));
	}

	[DataTestMethod]
	[DataRow("missing-value")]
	[DataRow("duplicate-value")]
	[DataRow("case-value")]
	[DataRow("definition")]
	public void MissingClothingProfiles_FailBeforeAddingAnyProfile(string mutation)
	{
		using var context = Stock();
		context.CharacteristicProfiles.RemoveRange(context.CharacteristicProfiles.Where(x => x.Name.StartsWith("Clothing_")));
		var value = context.CharacteristicValues.Single(x => x.Definition.Name == "Colour" && x.Name == "beige");
		switch (mutation)
		{
			case "missing-value": context.CharacteristicValues.Remove(value); break;
			case "case-value": value.Name = "BEIGE"; break;
			case "duplicate-value": context.CharacteristicValues.Add(new CharacteristicValue { Id = 20000, Name = "beige", DefinitionId = value.DefinitionId, Value = value.Value }); break;
			case "definition": context.CharacteristicDefinitions.Single(x => x.Id == value.DefinitionId).Pattern = "^builder$"; break;
			default: Assert.Fail(mutation); break;
		}
		context.SaveChanges();
		var before = Snapshot(context);
		Assert.ThrowsException<InvalidOperationException>(() => CoreDataSeeder.EnsureClothingColourProfiles(context));
		Assert.AreEqual(before, Snapshot(context));
		Assert.IsFalse(context.ChangeTracker.Entries().Any(x => x.State != EntityState.Unchanged));
	}

	[TestMethod]
	public void ExistingClothingProfiles_AreNotOverwrittenIncludingCustomNamesAndPalettes()
	{
		using var context = Stock();
		var profile = context.CharacteristicProfiles.Single(x => x.Name == "Clothing_Wood_Colours");
		profile.Name = "CLOTHING_WOOD_COLOURS";
		profile.Definition = "<Definition><Value>brown</Value></Definition>";
		profile.Description = "Builder's local timber palette";
		context.SaveChanges();
		var before = Snapshot(context);
		new CoreDataSeeder().SeedColours(context);
		Assert.AreEqual(before, Snapshot(context));
	}

	[TestMethod]
	public void LegacyRerun_MatchesFreshStockAndPreservesEveryValueIdAndStoredReference()
	{
		using var context = Stock();
		var fresh = Snapshot(context);
		MakeLegacy(context);
		var secondary = Value(context, 284);
		var itemXml = $"<Definition><Value Definition=\"{secondary.DefinitionId}\" Value=\"{secondary.Id}\"/></Definition>";
		context.GameItemComponents.Add(new GameItemComponent { Id = 700, GameItemId = 800, GameItemComponentProtoId = 900, Definition = itemXml });
		context.Characteristics.Add(new Characteristic { BodyId = 1000, Type = 1, CharacteristicId = secondary.Id });
		context.SaveChanges();
		context.ChangeTracker.Clear();
		new CoreDataSeeder().SeedColours(context);
		context.ChangeTracker.Clear();
		Assert.AreEqual(fresh, Snapshot(context));
		Assert.AreEqual(itemXml, context.GameItemComponents.Single().Definition);
		Assert.AreEqual(secondary.Id, context.Characteristics.Single().CharacteristicId);
		Assert.AreEqual(1, context.GameItemComponents.Count());
		Assert.AreEqual(1, context.Characteristics.Count());
		new CoreDataSeeder().SeedColours(context);
		Assert.AreEqual(fresh, Snapshot(context));
	}

	[TestMethod]
	public void LegacyPatternRepair_AnchorsStockChannelsButPreservesCustomChildPatterns()
	{
		using var context = Stock();
		MakeLegacy(context);
		var child = context.CharacteristicDefinitions.Single(x => x.Name == "Colour1");
		child.Pattern = "^builder_colour$";
		context.SaveChanges();
		new CoreDataSeeder().SeedColours(context);
		Assert.AreEqual("^builder_colour$", child.Pattern);
		Assert.AreEqual("^colou?r$", context.CharacteristicDefinitions.Single(x => x.Name == "Colour").Pattern);
		Assert.AreEqual("^colou?r2$", context.CharacteristicDefinitions.Single(x => x.Name == "Colour2").Pattern);
		Assert.AreEqual("^colou?r3$", context.CharacteristicDefinitions.Single(x => x.Name == "Colour3").Pattern);
		Assert.AreEqual("^finecolou?r(ed)?$", context.CharacteristicDefinitions.Single(x => x.Name == "Fine Colour").Pattern);
	}

	[DataTestMethod]
	[DataRow("name")]
	[DataRow("default")]
	[DataRow("pluralisation")]
	[DataRow("additional")]
	[DataRow("chargen-prog")]
	[DataRow("ongoing-prog")]
	[DataRow("target")]
	[DataRow("rgb")]
	[DataRow("basic")]
	[DataRow("fancy")]
	[DataRow("colour-name")]
	[DataRow("primary-name")]
	[DataRow("primary-rgb")]
	[DataRow("definition-pattern")]
	[DataRow("definition-model")]
	[DataRow("definition-parent")]
	[DataRow("alias-collision")]
	[DataRow("third-name")]
	[DataRow("duplicate-target")]
	public void Rerun_PreservesCustomOrAmbiguousRecords(string mutation)
	{
		using var context = Stock();
		MakeLegacy(context);
		var repair = CoreDataSeeder.StockColourLookupRepairs.Single(x => x.Secondary.Id == 284);
		var value = Value(context, 284);
		var colour = context.Colours.Single(x => x.Id == 284);
		var primary = Value(context, 239);
		var definition = context.CharacteristicDefinitions.Single(x => x.Id == value.DefinitionId);
		switch (mutation)
		{
			case "name": value.Name = "Builder yellow"; break;
			case "default": value.Default = true; break;
			case "pluralisation": value.Pluralisation = 1; break;
			case "additional": value.AdditionalValue = "builder"; break;
			case "chargen-prog": value.FutureProgId = 1200; break;
			case "ongoing-prog": value.OngoingValidityProgId = 1200; break;
			case "target": value.Value = "2"; break;
			case "rgb": colour.Red = 200; break;
			case "basic": colour.Basic = 4; break;
			case "fancy": colour.Fancy = "builder yellow"; break;
			case "colour-name": colour.Name = "builder yellow"; break;
			case "primary-name": primary.Name = "builder canonical"; break;
			case "primary-rgb": context.Colours.Single(x => x.Id == 239).Blue = 20; break;
			case "definition-pattern": definition.Pattern = "^builder$"; break;
			case "definition-model": definition.Model = "bodypart"; break;
			case "definition-parent": definition.ParentId = 9000; break;
			case "alias-collision":
			case "third-name":
			case "duplicate-target":
				context.CharacteristicValues.Add(new CharacteristicValue
				{
					Id = 20000, Definition = definition,
					Name = mutation == "alias-collision" ? repair.QualifiedName : repair.Name,
					Value = mutation == "duplicate-target" ? "284" : "1"
				});
				break;
			default: Assert.Fail(mutation); break;
		}
		context.SaveChanges();
		var expected = context.Entry(value).CurrentValues.Clone();
		var colourExpected = context.Entry(colour).CurrentValues.Clone();
		new CoreDataSeeder().SeedColours(context);
		foreach (var property in expected.Properties)
			Assert.AreEqual(expected[property], context.Entry(value).CurrentValues[property], property.Name);
		foreach (var property in colourExpected.Properties)
			Assert.AreEqual(colourExpected[property], context.Entry(colour).CurrentValues[property], property.Name);
	}
}
