#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace MudSharp_Unit_Tests;

[TestClass]
public class MedievalJewelleryDependencyTests
{
	private static readonly string[] RequiredWearableComponentNames =
	[
		"Wear_Brooch",
		"Wear_Brooches",
		"Wear_Pin",
		"Wear_Badge",
		"Wear_Hairpin",
		"Wear_Hairpins",
		"Wear_Hair_Comb",
		"Wear_Hair_Combs",
		"Wear_Hair_Ornament",
		"Wear_Hair_Ornaments",
		"Wear_Temple_Rings",
		"Wear_Circlet",
		"Wear_Diadem",
		"Wear_Coronet",
		"Wear_Crown",
		"Wear_Chaplet",
		"Wear_Wreath",
		"Wear_Head_Garland",
		"Wear_Neck_Garland",
		"Wear_Wrist_Garland",
		"Wear_Ankle_Garland",
		"Wear_Torc",
		"Wear_Neck_Ring",
		"Wear_Waist_Chain",
		"Wear_Girdle_Ornament",
		"Wear_Belt_Ornament",
		"Wear_Belt_Plaques",
		"Wear_Waist_Ornament",
		"Wear_Forehead_Ornament"
	];

	private static readonly string[] RequiredSealStampComponentNames =
	[
		"SealStamp_Medieval_RingSignet",
		"SealStamp_Medieval_PersonalSignetRing",
		"SealStamp_Medieval_MerchantSignetRing",
		"SealStamp_Medieval_NobleSignetRing"
	];

	private static readonly string[] RequestedVariableComponentNames =
	[
		"Variable_JewelleryMotif",
		"Variable_Flower",
		"Variable_MetalFinish",
		"Variable_BeadPattern",
		"Variable_JewelleryShape",
		"Variable_InlayStyle"
	];

	private static readonly string[] RequiredJewelleryTagPaths =
	[
		"Functions / Worn Items / Jewellery / Armlets",
		"Functions / Worn Items / Jewellery / Badges",
		"Functions / Worn Items / Jewellery / Bead Strings",
		"Functions / Worn Items / Jewellery / Belt Ornaments",
		"Functions / Worn Items / Jewellery / Belt Plaques",
		"Functions / Worn Items / Jewellery / Brooches",
		"Functions / Worn Items / Jewellery / Chaplets",
		"Functions / Worn Items / Jewellery / Chokers",
		"Functions / Worn Items / Jewellery / Circlets",
		"Functions / Worn Items / Jewellery / Coronets",
		"Functions / Worn Items / Jewellery / Crowns",
		"Functions / Worn Items / Jewellery / Diadems",
		"Functions / Worn Items / Jewellery / Forehead Ornaments",
		"Functions / Worn Items / Jewellery / Garlands",
		"Functions / Worn Items / Jewellery / Girdle Ornaments",
		"Functions / Worn Items / Jewellery / Hair Ornaments",
		"Functions / Worn Items / Jewellery / Head Ornaments",
		"Functions / Worn Items / Jewellery / Neck Garlands",
		"Functions / Worn Items / Jewellery / Neck Rings",
		"Functions / Worn Items / Jewellery / Pendants",
		"Functions / Worn Items / Jewellery / Pins",
		"Functions / Worn Items / Jewellery / Temple Rings",
		"Functions / Worn Items / Jewellery / Toe Rings",
		"Functions / Worn Items / Jewellery / Torcs",
		"Functions / Worn Items / Jewellery / Waist Chains",
		"Functions / Worn Items / Jewellery / Waist Ornaments",
		"Functions / Worn Items / Jewellery / Wreaths",
		"Functions / Worn Items / Jewellery / Piercings / Ear Studs",
		"Functions / Worn Items / Jewellery / Piercings / Nose Rings",
		"Functions / Worn Items / Jewellery / Piercings / Nose Studs",
		"Functions / Worn Items / Jewellery / Garlands / Dried Garlands",
		"Functions / Worn Items / Jewellery / Garlands / Flower Garlands",
		"Functions / Worn Items / Jewellery / Garlands / Fresh Garlands",
		"Functions / Worn Items / Jewellery / Garlands / Herb Garlands",
		"Functions / Worn Items / Jewellery / Garlands / Leaf Garlands",
		"Functions / Worn Items / Jewellery / Wreaths / Dried Wreaths",
		"Functions / Worn Items / Jewellery / Wreaths / Flower Wreaths",
		"Functions / Worn Items / Jewellery / Wreaths / Fresh Wreaths",
		"Functions / Worn Items / Jewellery / Wreaths / Herb Wreaths",
		"Functions / Worn Items / Jewellery / Wreaths / Leaf Wreaths",
		"Market / Jewellery",
		"Market / Jewellery / Children's Jewellery",
		"Market / Jewellery / Commoner Jewellery",
		"Market / Jewellery / Court Jewellery",
		"Market / Jewellery / Ephemeral Jewellery",
		"Market / Jewellery / Festival Jewellery",
		"Market / Jewellery / Luxury Jewellery",
		"Market / Jewellery / Merchant Jewellery",
		"Market / Jewellery / Noble Jewellery",
		"Market / Jewellery / Professional Jewellery",
		"Market / Jewellery / Regalia",
		"Market / Jewellery / Simple Jewellery",
		"Market / Jewellery / Standard Jewellery"
	];

	private static FuturemudDatabaseContext BuildContext()
	{
		DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		return new FuturemudDatabaseContext(options);
	}

	[TestMethod]
	public void UsefulSeederTags_SeedsMedievalJewelleryDependencyPaths()
	{
		using FuturemudDatabaseContext context = BuildContext();
		UsefulSeeder seeder = new();

		seeder.SeedTagsForTesting(context);

		Dictionary<long, Tag> tagsById = context.Tags.ToDictionary(x => x.Id);
		HashSet<string> paths = context.Tags
			.AsEnumerable()
			.Select(x => FullTagPath(x, tagsById))
			.ToHashSet(StringComparer.OrdinalIgnoreCase);

		string[] missingPaths = RequiredJewelleryTagPaths
			.Where(x => !paths.Contains(x))
			.ToArray();

		Assert.AreEqual(0, missingPaths.Length,
			$"Missing medieval jewellery dependency tag paths: {string.Join(", ", missingPaths)}");
	}

	[TestMethod]
	public void SeededItemComponentCatalogue_IncludesMedievalJewelleryDependencies()
	{
		var catalogueSource = ReadSource("Design Documents", "Data", "Seeded_Item_Components.json");
		using var catalogue = JsonDocument.Parse(catalogueSource);

		var entries = catalogue.RootElement
			.EnumerateArray()
			.ToDictionary(
				x => x.GetProperty("Component Name").GetString()!,
				x => x.GetProperty("Component Type").GetString()!,
				StringComparer.OrdinalIgnoreCase);

		AssertComponents(entries, RequiredWearableComponentNames, "Wearable");
		AssertComponents(entries, RequiredSealStampComponentNames, "SealStamp");
		AssertComponents(entries, RequestedVariableComponentNames, "Variable");
	}

	[TestMethod]
	public void SeededTagHierarchyCatalogue_IncludesMedievalJewelleryDependencyPaths()
	{
		var hierarchySource = ReadSource("Design Documents", "Data", "SeededTagHierarchy.csv");
		HashSet<string> paths = hierarchySource
			.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries)
			.Select(x => x.Split('\t'))
			.Where(x => x.Length >= 3)
			.Select(x => x[2])
			.ToHashSet(StringComparer.OrdinalIgnoreCase);

		string[] missingPaths = RequiredJewelleryTagPaths
			.Where(x => !paths.Contains(x))
			.ToArray();

		Assert.AreEqual(0, missingPaths.Length,
			$"SeededTagHierarchy.csv is missing medieval jewellery dependency paths: {string.Join(", ", missingPaths)}");
	}

	private static void AssertComponents(IReadOnlyDictionary<string, string> entries, IEnumerable<string> names,
		string expectedType)
	{
		foreach (string name in names)
		{
			Assert.IsTrue(entries.TryGetValue(name, out string? type),
				$"Seeded_Item_Components.json should include {name}.");
			Assert.AreEqual(expectedType, type, $"{name} should be a {expectedType} component.");
		}
	}

	private static string FullTagPath(Tag tag, IReadOnlyDictionary<long, Tag> tagsById)
	{
		List<string> names = [tag.Name];
		Tag? current = tag.Parent ?? (tag.ParentId is not null && tagsById.TryGetValue(tag.ParentId.Value, out var initialParent)
			? initialParent
			: null);
		while (current is not null)
		{
			names.Add(current.Name);
			current = current.Parent ?? (current.ParentId is not null && tagsById.TryGetValue(current.ParentId.Value, out var nextParent)
				? nextParent
				: null);
		}

		names.Reverse();
		return string.Join(" / ", names);
	}

	private static string ReadSource(params string[] parts)
	{
		return File.ReadAllText(Path.GetFullPath(Path.Combine(
			AppContext.BaseDirectory,
			"..",
			"..",
			"..",
			"..",
			Path.Combine(parts))));
	}
}
