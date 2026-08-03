#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Database;
using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private sealed record RenaissanceMilitaryItemSpec(
		string StableReference,
		string Category,
		string Admission,
		string Noun,
		string ShortDescription,
		string FullDescription,
		SizeCategory Size,
		ItemQuality Quality,
		double WeightInGrams,
		decimal Cost,
		bool Skinnable,
		string Material,
		string[] Tags,
		string[] Components,
		string BuilderNotes);

	internal sealed record RenaissanceMilitaryItemSpecTestData(
		string StableReference,
		string Category,
		string Admission,
		string ShortDescription,
		string FullDescription,
		bool Skinnable,
		string Material,
		IReadOnlyCollection<string> Tags,
		IReadOnlyCollection<string> Components);

	internal sealed record RenaissanceMilitaryArmourOutfitManifestTestData(
		string StableKey,
		string Name,
		IReadOnlyList<string> ItemStableReferences);

	private static IReadOnlyList<string> RenaissanceMilitaryRequiredMaterials => RenaissanceMilitaryItemSpecs
		.Select(x => x.Material)
		.Distinct(StringComparer.OrdinalIgnoreCase)
		.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
		.ToArray();

	private static IReadOnlyList<string> RenaissanceMilitaryRequiredTags => RenaissanceMilitaryItemSpecs
		.SelectMany(x => x.Tags)
		.Distinct(StringComparer.OrdinalIgnoreCase)
		.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
		.ToArray();

	private static IReadOnlyList<string> RenaissanceMilitaryRequiredComponents => RenaissanceMilitaryItemSpecs
		.SelectMany(x => x.Components)
		.Distinct(StringComparer.OrdinalIgnoreCase)
		.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
		.ToArray();

	private const string RenaissanceMilitaryPrerequisiteRerunGuidance =
		"Rerun Combat with its weapons, ranged weapons, and early-gun packages enabled, then rerun UsefulSeeder if any listed material or tag is absent before rerunning Items.";

	private void SeedRenaissanceMilitaryFirearmsAndArmour()
	{
		var issues = ValidateRenaissanceMilitaryDependencies(
			_materials.Keys,
			_tagsByFullPath.Keys,
			_components.Keys);
		if (issues.Count > 0)
		{
			throw BuildRenaissanceMilitaryPrerequisiteException(issues);
		}

		foreach (var spec in RenaissanceMilitaryItemSpecs)
		{
			CreateItem(
				spec.StableReference,
				spec.Noun,
				spec.ShortDescription,
				null,
				spec.FullDescription,
				spec.Size,
				spec.Quality,
				spec.WeightInGrams,
				spec.Cost,
				spec.Skinnable,
				false,
				spec.Material,
				spec.Tags,
				spec.Components,
				null,
				null,
				null,
				null,
				spec.BuilderNotes,
				allowLegacyShortDescriptionMatch: false);
		}

		UpsertOutfitManifests(RenaissanceMilitaryArmourOutfitManifestSpecs);
	}

	private void ValidateRenaissanceMilitaryPrerequisites()
	{
		var issues = ValidateRenaissanceMilitaryDependencies(
			_materials.Keys,
			_tagsByFullPath.Keys,
			_components.Keys);
		if (issues.Count > 0)
		{
			throw BuildRenaissanceMilitaryPrerequisiteException(issues);
		}
	}

	private static InvalidOperationException BuildRenaissanceMilitaryPrerequisiteException(
		IEnumerable<string> issues)
	{
		return new InvalidOperationException(
			"Renaissance military prerequisites are incomplete; no Renaissance item stage has been written." +
			Environment.NewLine + string.Join(Environment.NewLine, issues.Select(x => $" - {x}")) +
			Environment.NewLine + RenaissanceMilitaryPrerequisiteRerunGuidance);
	}

	private static IReadOnlyList<string> ValidateRenaissanceMilitaryDependencies(
		IEnumerable<string> materials,
		IEnumerable<string> tags,
		IEnumerable<string> components)
	{
		var issues = new List<string>();
		AddMissingRenaissanceMilitaryDependencies("material", RenaissanceMilitaryRequiredMaterials, materials, issues);
		AddMissingRenaissanceMilitaryDependencies("tag", RenaissanceMilitaryRequiredTags, tags, issues);
		AddMissingRenaissanceMilitaryDependencies("seeded component", RenaissanceMilitaryRequiredComponents, components,
			issues);
		return issues;
	}

	private static void AddMissingRenaissanceMilitaryDependencies(
		string dependencyType,
		IEnumerable<string> required,
		IEnumerable<string> available,
		ICollection<string> issues)
	{
		var availableSet = available.ToHashSet(StringComparer.OrdinalIgnoreCase);
		foreach (var name in required.Where(x => !availableSet.Contains(x)))
		{
			issues.Add($"Missing {dependencyType}: {name}");
		}
	}

	internal static IReadOnlyCollection<RenaissanceMilitaryItemSpecTestData> RenaissanceMilitaryItemSpecsForTesting =>
		RenaissanceMilitaryItemSpecs
			.Select(x => new RenaissanceMilitaryItemSpecTestData(
				x.StableReference,
				x.Category,
				x.Admission,
				x.ShortDescription,
				x.FullDescription,
				x.Skinnable,
				x.Material,
				x.Tags,
				x.Components))
			.ToArray();

	internal static IReadOnlyList<RenaissanceMilitaryArmourOutfitManifestTestData> RenaissanceMilitaryArmourOutfitManifestSpecsForTesting =>
		RenaissanceMilitaryArmourOutfitManifestSpecs
			.Select(x => new RenaissanceMilitaryArmourOutfitManifestTestData(x.StableKey, x.Name, x.ItemStableReferences))
			.ToArray();

	internal static IReadOnlyList<string> RenaissanceMilitaryMaterialsForTesting => RenaissanceMilitaryRequiredMaterials;
	internal static IReadOnlyList<string> RenaissanceMilitaryTagsForTesting => RenaissanceMilitaryRequiredTags;
	internal static IReadOnlyList<string> RenaissanceMilitaryComponentsForTesting => RenaissanceMilitaryRequiredComponents;
	internal static string RenaissanceMilitaryPrerequisiteRerunGuidanceForTesting => RenaissanceMilitaryPrerequisiteRerunGuidance;

	internal static IReadOnlyList<string> ValidateRenaissanceMilitaryDependenciesForTesting(
		IEnumerable<string> materials,
		IEnumerable<string> tags,
		IEnumerable<string> components)
	{
		return ValidateRenaissanceMilitaryDependencies(materials, tags, components);
	}

	internal void SeedRenaissanceMilitaryForTesting(FuturemudDatabaseContext context)
	{
		if (!ReferenceEquals(_context, context))
		{
			_context = context;
			InitialiseDependencies();
		}

		SeedRenaissanceMilitaryFirearmsAndArmour();
	}
}
