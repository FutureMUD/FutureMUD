#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.GameItems;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class HistoricalClothingColourTests
{

	private sealed record TextileContract(string Reference, string Noun, string Material, SizeCategory Size,
		ItemQuality Quality, double Weight, decimal Cost, string Components, string Tags, string? DefaultArguments,
		string VariableComponent = "Variable_Colour");

	[TestMethod]
	[DataRow("missing")]
	[DataRow("duplicate")]
	[DataRow("case")]
	[DataRow("target")]
	public void NativeVariablePrerequisiteFailure_AddsNoPartialComponentBatch(string mutation)
	{
		using var context = Stock();
		context.GameItemComponentProtos.RemoveRange(context.GameItemComponentProtos);
		var profile = context.CharacteristicProfiles.Single(x => x.Name == "Clothing_Lacquer_Colours");
		switch (mutation)
		{
			case "missing": context.CharacteristicProfiles.Remove(profile); break;
			case "case": profile.Name = "CLOTHING_LACQUER_COLOURS"; break;
			case "target": profile.TargetDefinitionId = context.CharacteristicDefinitions.Single(x => x.Name == "Colour1").Id; break;
			case "duplicate": context.CharacteristicProfiles.Add(new CharacteristicProfile
			{
				Name = profile.Name, Type = profile.Type, Description = "Duplicate prerequisite",
				TargetDefinitionId = profile.TargetDefinitionId, Definition = profile.Definition
			}); break;
			default: Assert.Fail(mutation); break;
		}
		context.SaveChanges();
		var exception = Assert.ThrowsExactly<InvalidOperationException>(() => new UsefulSeeder().SeedVariablesForTesting(context));
		StringAssert.Contains(exception.Message, "Clothing_Lacquer_Colours");
		Assert.AreEqual(0, context.GameItemComponentProtos.Count());
		Assert.IsFalse(context.ChangeTracker.Entries().Any(x => x.State != EntityState.Unchanged));
	}

	private static FuturemudDatabaseContext Stock()
	{
		var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
		context.Accounts.Add(new Account
		{
			Id = 1, Name = "ColourTest", Password = "unused", Salt = 1, Email = "test@example.invalid",
			LastLoginIp = "127.0.0.1", FormatLength = 80, InnerFormatLength = 78, ActiveCharactersAllowed = 1,
			TimeZoneId = "UTC", CultureName = "en-AU", RegistrationCode = "", RecoveryCode = "",
			UnitPreference = "metric", CreationDate = DateTime.UtcNow
		});
		// CoreDataSeeder installs earlier characteristic values before its colour catalogue.
		var priorDefinition = new CharacteristicDefinition
		{
			Id = 9000, Name = "Prior stock characteristic", Pattern = "^prior$",
			Description = "Colour-writer prerequisite", Model = "standard"
		};
		context.CharacteristicDefinitions.Add(priorDefinition);
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = 1, Name = "prior", Definition = priorDefinition, Value = "prior"
		});
		context.SaveChanges();
		new CoreDataSeeder().SeedColours(context);
		new UsefulSeeder().SeedVariablesForTesting(context);
		return context;
	}

	[TestMethod]
	[TestCategory("Integration")]
	public void AllVariableCapableInventoryRows_ResolveTheirActualStockChannelsAndCompleteProfiles()
	{
		using var context = Stock();
		var snapshot = IndustrialisedClothingColourBindings.Read(context);
		var location = new ClothingSourceLocation("HistoricalClothingColourTests", 1);
		var definitions = context.CharacteristicDefinitions.ToDictionary(x => x.Id);
		var values = context.CharacteristicValues.ToArray();
		var profiles = context.CharacteristicProfiles.ToDictionary(x => x.Id);
		var components = context.GameItemComponentProtos.ToDictionary(x => x.Name);
		var rows = IndustrialisedClothingDependencyPlan.Rows
			.Where(x => x.Components.Any(c => c.StartsWith("Variable_", StringComparison.Ordinal))).ToArray();
		Assert.IsTrue(rows.Length > 0);
		foreach (var row in rows)
		{
			var component = components[row.Components.Single(x => x.StartsWith("Variable_", StringComparison.Ordinal))];
			var channels = new Dictionary<string, ClothingColourRow>(StringComparer.Ordinal);
			foreach (var characteristic in XElement.Parse(component.Definition).Elements("Characteristic"))
			{
				var definition = definitions[(long)characteristic.Attribute("Value")!];
				var profile = profiles[(long)characteristic.Attribute("Profile")!];
				var ancestors = new HashSet<long>();
				for (CharacteristicDefinition? ancestor = definitions[profile.TargetDefinitionId]; ancestor is not null;
					ancestor = ancestor.ParentId is { } parent ? definitions[parent] : null)
					Assert.IsTrue(ancestors.Add(ancestor.Id), row.ItemReference);
				var candidates = values.Where(x => ancestors.Contains(x.DefinitionId)).ToArray();
				Assert.IsTrue(profile.Type.Equals("all", StringComparison.OrdinalIgnoreCase) ||
					profile.Type.Equals("standard", StringComparison.OrdinalIgnoreCase), profile.Name);
				var allowed = profile.Type.Equals("all", StringComparison.OrdinalIgnoreCase)
					? candidates.Select(x => x.Name).ToArray()
					: XElement.Parse(profile.Definition).Elements("Value")
						.Select(x => candidates.Single(v => v.Name == x.Value || v.Id.ToString() == x.Value).Name)
						.Distinct().ToArray();
				var variable = new[] { "colour", "colour1", "colour2", "colour3", "finecolour", "drabcolour", "finish" }
					.First(x => Regex.IsMatch(x, definition.Pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant));
				channels.Add(variable, new(location, row.ItemReference, variable, definition.Name, profile.Name,
					allowed, allowed[0], "", "", ""));
			}
			var bound = snapshot.Bind(channels, [component], location);
			Assert.AreEqual(channels.Count, bound.Count, row.ItemReference);
			// This proves stock channel/profile integrity, not material-specific palette approval.
		}
	}
}
