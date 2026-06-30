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
public class MedievalIndustryPrerequisiteAddendumTests
{
	private static readonly string[] RequiredIndustryToolComponentNames =
	[
		"Tool_Blacksmithing_General",
		"Tool_Armouring_General",
		"Tool_Weaponsmithing_General",
		"Tool_Woodcrafting_General",
		"Tool_Coopering_General",
		"Tool_Textilecraft_General",
		"Tool_Dyeing_Fulling_General",
		"Tool_Leatherworking_General",
		"Tool_Parchmentmaking_General",
		"Tool_Papermaking_General",
		"Tool_Bookbinding_General",
		"Tool_Pottery_General",
		"Tool_Masonry_General",
		"Tool_Glassblowing_General",
		"Tool_Lapidary_General",
		"Tool_Jewellery_General",
		"Tool_Apothecary_General",
		"Tool_Medical_General",
		"Tool_Printing_Woodblock_General"
	];

	private static readonly string[] RequiredNewToolTagPaths =
	[
		"Functions / Tools / Apothecary Tools",
		"Functions / Tools / Apothecary Tools / Apothecary Mortar",
		"Functions / Tools / Apothecary Tools / Apothecary Pestle",
		"Functions / Tools / Apothecary Tools / Apothecary Spatula",
		"Functions / Tools / Apothecary Tools / Apothecary Strainer",
		"Functions / Tools / Apothecary Tools / Herb Chopper",
		"Functions / Tools / Apothecary Tools / Medicine Spoon",
		"Functions / Tools / Apothecary Tools / Pill Tile",
		"Functions / Tools / Apothecary Tools / Preparation Board",
		"Functions / Tools / Jewellery Tools",
		"Functions / Tools / Jewellery Tools / Crimping Pliers",
		"Functions / Tools / Jewellery Tools / Jeweller's Anvil",
		"Functions / Tools / Jewellery Tools / Jeweller's Burnisher",
		"Functions / Tools / Jewellery Tools / Jeweller's Drawplate",
		"Functions / Tools / Jewellery Tools / Jeweller's Stakes",
		"Functions / Tools / Jewellery Tools / Stone Setting Tools",
		"Functions / Tools / Jewellery Tools / Wire Pliers",
		"Functions / Tools / Lapidary Tools",
		"Functions / Tools / Lapidary Tools / Bead Drill",
		"Functions / Tools / Lapidary Tools / Cabochon Dop Stick",
		"Functions / Tools / Lapidary Tools / Gem Drill",
		"Functions / Tools / Lapidary Tools / Lapidary Polisher",
		"Functions / Tools / Lapidary Tools / Lapidary Saw",
		"Functions / Tools / Lapidary Tools / Lapidary Wheel"
	];

	private static readonly string[] RequiredSkillPackageEntries =
	[
		"Goldsmithing",
		"Glassblowing",
		"Lapidary",
		"Fulling",
		"Parchmentmaking",
		"Papermaking",
		"Bookbinding",
		"Calligraphy",
		"Scribing",
		"Woodblock Printing",
		"Quarrying"
	];

	private static readonly string[] RequiredAuditColumns =
	[
		"stable_reference",
		"item_source_file",
		"craft_name",
		"craft_method",
		"immediate_inputs",
		"missing_input_crafts",
		"terminal_inputs",
		"terminal_source_class",
		"terminal_source_owner",
		"missing_terminal_source",
		"required_tools",
		"missing_tool_items",
		"missing_tool_components",
		"missing_tool_tags",
		"missing_component_types",
		"missing_component_prototypes",
		"missing_materials",
		"missing_tags",
		"required_skill",
		"missing_skill_package_entry",
		"owning_resolution_pass",
		"resolution_status"
	];

	[TestMethod]
	public void UsefulSeederTags_SeedsMedievalIndustryPrerequisiteToolPaths()
	{
		using FuturemudDatabaseContext context = BuildContext();
		UsefulSeeder seeder = new();

		seeder.SeedTagsForTesting(context);

		Dictionary<long, Tag> tagsById = context.Tags.ToDictionary(x => x.Id);
		HashSet<string> paths = context.Tags
			.AsEnumerable()
			.Select(x => FullTagPath(x, tagsById))
			.ToHashSet(StringComparer.OrdinalIgnoreCase);

		string[] missingPaths = RequiredNewToolTagPaths
			.Where(x => !paths.Contains(x))
			.ToArray();

		Assert.AreEqual(0, missingPaths.Length,
			$"Missing medieval industry prerequisite tool tag paths: {string.Join(", ", missingPaths)}");
	}

	[TestMethod]
	public void SeededItemComponentCatalogue_IncludesMedievalIndustryHandToolProfiles()
	{
		var catalogueSource = ReadSource("Design Documents", "Data", "Seeded_Item_Components.json");
		using var catalogue = JsonDocument.Parse(catalogueSource);

		var entries = catalogue.RootElement
			.EnumerateArray()
			.ToDictionary(
				x => x.GetProperty("Component Name").GetString()!,
				x => x.GetProperty("Component Type").GetString()!,
				StringComparer.OrdinalIgnoreCase);

		foreach (string name in RequiredIndustryToolComponentNames)
		{
			Assert.IsTrue(entries.TryGetValue(name, out string? type),
				$"Seeded_Item_Components.json should include {name}.");
			Assert.AreEqual("HandTool", type, $"{name} should be a HandTool component.");
		}
	}

	[TestMethod]
	public void SeededTagHierarchyCatalogue_IncludesMedievalIndustryPrerequisiteToolPaths()
	{
		var hierarchySource = ReadSource("Design Documents", "Data", "SeededTagHierarchy.csv");
		HashSet<string> paths = hierarchySource
			.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries)
			.Select(x => x.Split('\t'))
			.Where(x => x.Length >= 3)
			.Select(x => x[2])
			.ToHashSet(StringComparer.OrdinalIgnoreCase);

		string[] missingPaths = RequiredNewToolTagPaths
			.Where(x => !paths.Contains(x))
			.ToArray();

		Assert.AreEqual(0, missingPaths.Length,
			$"SeededTagHierarchy.csv is missing medieval industry prerequisite tool paths: {string.Join(", ", missingPaths)}");
	}

	[TestMethod]
	public void SkillPackageSeeder_IncludesRepeatedMedievalIndustryPrerequisiteSkills()
	{
		var skillSource = ReadSource("DatabaseSeeder", "Seeders", "SkillPackageSeeder.cs");

		foreach (string skill in RequiredSkillPackageEntries)
		{
			StringAssert.Contains(skillSource, $"new SkillDetails(\"{skill}\"");
		}
	}

	[TestMethod]
	public void MedievalCraftingAudit_IncludesPrerequisiteRoutingColumns()
	{
		var auditSource = ReadSource("Design Documents", "Seeding", "Medieval_Crafting_Audit.md");

		foreach (string column in RequiredAuditColumns)
		{
			StringAssert.Contains(auditSource, column);
		}
	}

	private static FuturemudDatabaseContext BuildContext()
	{
		DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		return new FuturemudDatabaseContext(options);
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
