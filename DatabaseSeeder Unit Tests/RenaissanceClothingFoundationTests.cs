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
public class RenaissanceClothingFoundationTests
{
	private static FuturemudDatabaseContext BuildContext()
	{
		DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		return new FuturemudDatabaseContext(options);
	}

	[TestMethod]
	public void UsefulSeederTags_SeedsRenaissanceClothingHierarchyByFullPath()
	{
		using FuturemudDatabaseContext context = BuildContext();
		UsefulSeeder seeder = new();

		seeder.SeedTagsForTesting(context);
		seeder.SeedTagsForTesting(context);

		Dictionary<long, Tag> tagsById = context.Tags.ToDictionary(x => x.Id);
		HashSet<string> paths = context.Tags
			.AsEnumerable()
			.Select(x => FullTagPath(x, tagsById))
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		string[] missing = ItemSeeder.RenaissanceClothingTagsForTesting
			.Where(x => !paths.Contains(x))
			.ToArray();

		Assert.AreEqual(0, missing.Length,
			$"Missing Renaissance clothing tag paths: {string.Join(", ", missing)}");
		Assert.AreEqual(1, paths.Count(x => x == "Culture / Renaissance / Shared"),
			"The full-path tag cache should preserve the intended Renaissance Shared branch.");
	}

	[TestMethod]
	public void RenaissanceClothingDependencyValidation_AcceptsCompleteFoundationAndReportsExactMissingRows()
	{
		IReadOnlyList<string> complete = ItemSeeder.ValidateRenaissanceClothingDependenciesForTesting(
			ItemSeeder.RenaissanceClothingWearProfilesForTesting,
			ItemSeeder.RenaissanceClothingMaterialsForTesting,
			ItemSeeder.RenaissanceClothingTagsForTesting,
			ItemSeeder.RenaissanceClothingComponentsForTesting);
		Assert.AreEqual(0, complete.Count, string.Join(Environment.NewLine, complete));

		IReadOnlyList<string> missing = ItemSeeder.ValidateRenaissanceClothingDependenciesForTesting(
			ItemSeeder.RenaissanceClothingWearProfilesForTesting,
			ItemSeeder.RenaissanceClothingMaterialsForTesting.Where(x => x != "barkcloth"),
			ItemSeeder.RenaissanceClothingTagsForTesting,
			ItemSeeder.RenaissanceClothingComponentsForTesting);
		CollectionAssert.AreEqual(new[] { "Missing material: barkcloth" }, missing.ToArray());
	}

	[TestMethod]
	public void MaintainedCatalogues_ContainRenaissanceClothingFoundationRows()
	{
		using JsonDocument materialsDocument = JsonDocument.Parse(
			ReadSource("Design Documents", "Data", "Seeded_Materials.json"));
		HashSet<string> materials = materialsDocument.RootElement
			.EnumerateArray()
			.Select(x => x.GetProperty("Material Name").GetString()!)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		CollectionAssert.IsSubsetOf(ItemSeeder.RenaissanceClothingMaterialsForTesting.ToArray(), materials.ToArray());

		using JsonDocument componentsDocument = JsonDocument.Parse(
			ReadSource("Design Documents", "Data", "Seeded_Item_Components.json"));
		HashSet<string> components = componentsDocument.RootElement
			.EnumerateArray()
			.Select(x => x.GetProperty("Component Name").GetString()!)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		CollectionAssert.IsSubsetOf(ItemSeeder.RenaissanceClothingComponentsForTesting.ToArray(), components.ToArray());

		HashSet<string> tagPaths = ReadSource("Design Documents", "Data", "SeededTagHierarchy.csv")
			.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries)
			.Select(x => x.Split('\t'))
			.Where(x => x.Length >= 3)
			.Select(x => x[2])
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		CollectionAssert.IsSubsetOf(ItemSeeder.RenaissanceClothingTagsForTesting.ToArray(), tagPaths.ToArray());
	}

	private static string FullTagPath(Tag tag, IReadOnlyDictionary<long, Tag> tagsById)
	{
		List<string> names = [tag.Name];
		Tag? current = tag.Parent ??
		               (tag.ParentId is not null && tagsById.TryGetValue(tag.ParentId.Value, out var initialParent)
			               ? initialParent
			               : null);
		while (current is not null)
		{
			names.Add(current.Name);
			current = current.Parent ??
			          (current.ParentId is not null && tagsById.TryGetValue(current.ParentId.Value, out var nextParent)
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
