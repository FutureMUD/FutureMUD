#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Body.Traits.Decorators;
using MudSharp.Database;

namespace MudSharp_Unit_Tests;

[TestClass]
public class AttributeDescriberSeederTests
{
	private static FuturemudDatabaseContext Seed(string decorator, string choice = "labmud")
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new FuturemudDatabaseContext(options);
		new AttributeSeeder().SeedData(context, new Dictionary<string, string>
		{
			["choice"] = choice, ["decorator"] = decorator
		});
		return context;
	}

	[DataTestMethod]
	[DataRow("rpi", "Attribute", "Prodigious|Phenomenal|Formidable|Tremendous|Immense|Monumental|Colossal|Titanic|Mythic|Demigodlike|Godlike|Transcendent|Ineffable")]
	[DataRow("labmud", "Strength Attribute", "Prodigious|Herculean|Trans-Herculean|Towering|Immense|Monumental|Colossal|Titanic|Worldshaking|Demigodlike|Godlike|Worldbreaking|Immeasurable")]
	[DataRow("labmud", "Dexterity Attribute", "Deft|Uncanny|Preternatural|Sublime|Otherworldly|Transcendent|Celestial|Empyrean|Mythic|Demigodlike|Godlike|Beyond Divine|Ineffable")]
	[DataRow("labmud", "Constitution Attribute", "Robust|Stalwart|Ironclad|Adamantine|Inexhaustible|Deathless|Eternal|Primordial|Mythic|Demigodlike|Godlike|Beyond Divine|Indestructible")]
	[DataRow("labmud", "Intelligence Attribute", "Brilliant|Genius|Sage|Profound|Enlightened|Transcendent|Transhuman|Cosmic|Mythic|Demigodlike|Godlike|Beyond Divine|Ineffable")]
	[DataRow("labmud", "Willpower Attribute", "Resolute|Dauntless|Indomitable|Adamantine|Inexorable|Unconquerable|Eternal|Primordial|Mythic|Demigodlike|Godlike|Beyond Divine|Absolute")]
	[DataRow("labmud", "Perception Attribute", "Acute|Piercing|Uncanny|Preternatural|Oracular|Revelatory|All-Seeing|Cosmic|Mythic|Demigodlike|Godlike|Beyond Divine|Omniscient")]
	public void SeededRanges_PreserveHistoryAndDescribeEveryUpperBoundary(string option, string name, string vocabulary)
	{
		using var context = Seed(option);
		var row = context.TraitDecorators.Single(x => x.Name == name);
		Assert.IsTrue(context.TraitDefinitions.Any(x => x.DecoratorId == row.Id));
		var decorator = new RangeDecorator(row);
		(int High, string Text)[] history =
		[
			(0, "Abysmal"), (3, "Terrible"), (6, "Bad"), (9, "Poor"), (11, "Average"),
			(13, "Good"), (15, "Great"), (17, "Excellent"), (20, "Super"), (23, "Epic"), (25, "Legendary")
		];
		for (var value = -30.0; value <= 25.0; value += 0.25)
		{
			Assert.AreEqual(history.First(x => value <= x.High).Text, decorator.Decorate(value), $"{name} at {value}");
		}

		int[] boundaries = [25, 30, 40, 55, 75, 100, 140, 190, 250, 350, 500, 750, 1000];
		var expected = vocabulary.Split('|');
		for (var i = 0; i < boundaries.Length; i++)
		{
			var previous = i == 0 ? "Legendary" : expected[i - 1];
			Assert.AreEqual(previous, decorator.Decorate(boundaries[i] - 0.001));
			Assert.AreEqual(previous, decorator.Decorate(boundaries[i]));
			Assert.AreEqual(expected[i], decorator.Decorate(boundaries[i] + 0.001));
			Assert.AreEqual(expected[i], decorator.Decorate(boundaries[i] + 1));
		}
		Assert.AreEqual(expected[^1], decorator.Decorate(int.MaxValue));
		Assert.AreEqual(expected[^1], decorator.Decorate(double.MaxValue));
		var root = XElement.Parse(row.Contents);
		Assert.AreEqual("true", (string?)root.Attribute("colour_default"));
		Assert.AreEqual("true", (string?)root.Attribute("colour_buffed"));
		Assert.AreEqual("false", (string?)root.Attribute("colour_capped"));
	}

	[TestMethod]
	public void LabMudStrength_DistinguishesRepresentativeMythicalTargets()
	{
		using var context = Seed("labmud");
		var decorator = new RangeDecorator(context.TraitDecorators.Single(x => x.Name == "Strength Attribute"));
		(int Value, string Text)[] examples =
		[
			(38, "Herculean"), (52, "Trans-Herculean"), (70, "Towering"), (95, "Immense"),
			(120, "Monumental"), (180, "Colossal"), (250, "Titanic"), (285, "Worldshaking")
		];
		foreach (var (value, text) in examples)
		{
			Assert.AreEqual(text, decorator.Decorate(value));
		}
	}

	[DataTestMethod]
	[DataRow("soi")]
	[DataRow("dnd")]
	[DataRow("labmud")]
	[DataRow("split")]
	[DataRow("3stats")]
	[DataRow("rpi")]
	[DataRow("simple")]
	[DataRow("arm")]
	public void RpiDecorator_AppliesUniversalScaleToEveryAttributePackage(string choice)
	{
		using var context = Seed("rpi", choice);
		var row = context.TraitDecorators.Single();
		Assert.IsTrue(context.TraitDefinitions.All(x => x.DecoratorId == row.Id));
		Assert.AreEqual("Mythic", new RangeDecorator(row).Decorate(285));
	}

	[TestMethod]
	public void Rerun_PreservesCustomAndInstalledDescribers()
	{
		using var context = Seed("labmud");
		var strength = context.TraitDecorators.Single(x => x.Name == "Strength Attribute");
		strength.Contents = strength.Contents.Replace("Worldshaking", "Custom Strength");
		context.SaveChanges();
		var before = context.TraitDecorators.ToDictionary(x => x.Id, x => x.Contents);
		new AttributeSeeder().SeedData(context, new Dictionary<string, string>());
		Assert.AreEqual(before.Count, context.TraitDecorators.Count());
		foreach (var row in context.TraitDecorators)
		{
			Assert.AreEqual(before[row.Id], row.Contents);
		}
	}

	[TestMethod]
	public void OtherLabMudPackages_RetainTheirExistingVocabulary()
	{
		using var context = Seed("labmud", "rpi");
		var row = context.TraitDecorators.Single(x => x.Name == "Strength Attribute");
		Assert.AreEqual("Herculean", new RangeDecorator(row).Decorate(285));
	}
}
