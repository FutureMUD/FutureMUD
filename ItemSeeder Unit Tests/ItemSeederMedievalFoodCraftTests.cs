#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemSeederMedievalFoodCraftTests
{

	private static readonly string[] MaterialNames =
	[
		"oak",
		"wrought iron",
		"bronze",
		"willow",
		"wheat",
		"salt",
		"olive crop",
		"grape",
		"meat",
		"fish"
	];

	[TestMethod]
	public void MedievalFoodCrafts_SeedTwiceWithoutDuplicates()
	{
		using var context = BuildContext();
		SeedPrerequisites(context);
		var seeder = new ItemSeeder();

		seeder.SeedMedievalFoodBeverageCraftsForTesting(context);
		var firstCraftCount = context.Crafts.Count();
		var firstInputCount = context.CraftInputs.Count();
		var firstToolCount = context.CraftTools.Count();
		var firstProductCount = context.CraftProducts.Count();
		seeder.SeedMedievalFoodBeverageCraftsForTesting(context);

		Assert.IsTrue(firstCraftCount > 0);
		Assert.AreEqual(firstCraftCount, context.Crafts.Count());
		Assert.AreEqual(firstInputCount, context.CraftInputs.Count());
		Assert.AreEqual(firstToolCount, context.CraftTools.Count());
		Assert.AreEqual(firstProductCount, context.CraftProducts.Count());

	}

	private static string? ProductStableReference(string product)
	{
		if (product.StartsWith("medieval_", StringComparison.Ordinal))
		{
			return product;
		}

		if (product.StartsWith("liquid:", StringComparison.Ordinal))
		{
			return product.Split(':')[1];
		}

		return null;
	}

	private static FuturemudDatabaseContext BuildContext()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		return new FuturemudDatabaseContext(options);
	}

	private static void SeedPrerequisites(FuturemudDatabaseContext context)
	{
		context.Accounts.Add(new Account
		{
			Id = 1,
			Name = "SeederTest",
			Password = "password",
			Salt = 1,
			AccessStatus = 0,
			Email = "seeder@example.com",
			LastLoginIp = "127.0.0.1",
			FormatLength = 80,
			InnerFormatLength = 78,
			UseMxp = false,
			UseMsp = false,
			UseMccp = false,
			ActiveCharactersAllowed = 1,
			UseUnicode = true,
			TimeZoneId = "UTC",
			CultureName = "en-AU",
			RegistrationCode = string.Empty,
			IsRegistered = true,
			RecoveryCode = string.Empty,
			UnitPreference = "metric",
			CreationDate = DateTime.UtcNow,
			PageLength = 22,
			PromptType = 0,
			TabRoomDescriptions = false,
			CodedRoomDescriptionAdditionsOnNewLine = false,
			CharacterNameOverlaySetting = 0,
			AppendNewlinesBetweenMultipleEchoesPerPrompt = false,
			ActLawfully = false,
			HasBeenActiveInWeek = true,
			HintsEnabled = true,
			AutoReacquireTargets = false
		});

		var specs = ItemSeeder.MedievalFoodCraftSpecsForTesting;
		var traitNames = specs.Select(x => x.Trait).Distinct(StringComparer.OrdinalIgnoreCase);
		context.TraitDefinitions.AddRange(traitNames.Select((name, index) => new TraitDefinition
		{
			Id = index + 1,
			Name = name,
			Type = 0,
			OwnerScope = 0,
			TraitGroup = "Crafting",
			ChargenBlurb = string.Empty,
			ValueExpression = string.Empty
		}));

		var toolTags = specs
			.SelectMany(x => x.Tools)
			.Select(x => Regex.Match(x, @"with the (?<tag>.+) tag$", RegexOptions.CultureInvariant).Groups["tag"].Value);
		var inputTags = specs
			.SelectMany(x => x.Inputs)
			.SelectMany(x => new[]
			{
				Regex.Match(x, @"with the (?<tag>.+) tag$", RegexOptions.CultureInvariant).Groups["tag"].Value,
				Regex.Match(x, @"tagged as (?<tag>.+?)(?:;|$)", RegexOptions.CultureInvariant).Groups["tag"].Value,
				Regex.Match(x, @"; piletag (?<tag>.+)$", RegexOptions.CultureInvariant).Groups["tag"].Value
			});
		var tags = toolTags
			.Concat(inputTags)
			.Concat(ItemSeeder.PreIndustrialFoodCommodityTagsForTesting)
			.Where(x => !string.IsNullOrWhiteSpace(x))
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToArray();
		context.Tags.AddRange(tags.Select((name, index) => new Tag
		{
			Id = index + 1,
			Name = name
		}));

		context.Materials.AddRange(MaterialNames.Select((name, index) => new Material
		{
			Id = index + 1,
			Name = name,
			MaterialDescription = name,
			Type = 0,
			BehaviourType = 0,
			Density = 1.0,
			Organic = true,
			ResidueSdesc = string.Empty,
			ResidueDesc = string.Empty,
			ResidueColour = "grey"
		}));

		context.Liquids.AddRange(new[] { "Water", "vegetable oil", "amber ale", "red wine" }
			.Select((name, index) => new Liquid
			{
				Id = index + 1,
				Name = name,
				Description = name,
				LongDescription = name,
				TasteText = name,
				VagueTasteText = name,
				SmellText = name,
				VagueSmellText = name,
				DisplayColour = "yellow",
				DampDescription = "damp",
				WetDescription = "wet",
				DrenchedDescription = "drenched",
				DampShortDescription = "damp",
				WetShortDescription = "wet",
				DrenchedShortDescription = "drenched",
				SurfaceReactionInfo = string.Empty,
				Density = 1.0,
				SpecificHeatCapacity = 1.0
			}));

		var stableReferences = specs
			.SelectMany(x => x.Dependencies)
			.Where(x => x.StableReference is not null)
			.Select(x => x.StableReference!)
			.Concat(specs
				.SelectMany(x => x.Products)
				.Select(ProductStableReference)
				.Where(x => x is not null)
				.Cast<string>())
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToArray();
		context.GameItemProtos.AddRange(stableReferences.Select((stableReference, index) =>
			new GameItemProto
			{
				Id = index + 100,
				Name = stableReference,
				UniqueName = stableReference,
				Keywords = stableReference.Replace('_', ' '),
				MaterialId = 1,
				EditableItem = Editable(),
				RevisionNumber = 0,
				Size = 0,
				Weight = 1.0,
				ReadOnly = false,
				LongDescription = string.Empty,
				BaseItemQuality = 0,
				CustomColour = string.Empty,
				MorphTimeSeconds = 0,
				MorphEmote = string.Empty,
				ShortDescription = $"a test {stableReference.Replace('_', ' ')}",
				FullDescription = string.Empty,
				PermitPlayerSkins = false,
				CostInBaseCurrency = 0.0M,
				IsHiddenFromPlayers = false,
				PlanarData = string.Empty
			}));

		context.SaveChanges();
	}

	private static EditableItem Editable()
	{
		return new EditableItem
		{
			RevisionNumber = 0,
			RevisionStatus = 4,
			BuilderAccountId = 1,
			BuilderDate = DateTime.UtcNow,
			BuilderComment = "test",
			ReviewerAccountId = 1,
			ReviewerComment = "test",
			ReviewerDate = DateTime.UtcNow
		};
	}
}
