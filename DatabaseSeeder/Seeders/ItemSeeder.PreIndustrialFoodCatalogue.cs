#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private const string PreIndustrialPreparedFoodRoot = "Food and Drink / Prepared Foods / Pre-Industrial Catalogue";
	private const string PreIndustrialFoodLiquidRoot = "Food and Drink / Food Liquids / Pre-Industrial Catalogue";
	private const string PreIndustrialFoodIntermediateRoot =
		"Materials / Food Products / Pre-Industrial Food Commodities";
	private const string PreparedFoodComponentPrefix = "PreparedFood_Catalogue_";
	private static readonly double[] StandardFoodLiquidAlcoholValues =
		[0.0, 0.01, 0.02, 0.03, 0.04, 0.05, 0.06, 0.08, 0.1, 0.12, 0.15, 0.18, 0.2, 0.25, 0.3, 0.4, 0.5];
	private static readonly double[] StandardFoodLiquidWaterValues =
		[0.0, 0.1, 0.25, 0.5, 0.65, 0.75, 0.85, 0.9, 0.95, 1.0];
	private static readonly double[] StandardFoodLiquidSatiationValues =
		[0.0, 0.25, 0.5, 1.0, 1.5, 2.0, 3.0, 4.0, 5.0, 6.0];
	private static readonly double[] StandardFoodLiquidThirstValues =
		[0.0, 0.25, 0.5, 1.0, 1.5, 2.0, 3.0, 4.0];
	private long? _nextPreIndustrialFoodComponentId;
	private long? _nextPreIndustrialFoodLiquidId;
	private long? _nextPreIndustrialFoodTagId;
	private bool _preIndustrialFoodCatalogueValidated;

	internal static IReadOnlyList<PreIndustrialFoodItemCatalogueEntry> PreIndustrialFoodItemsForTesting =>
		PreIndustrialFoodCatalogue.Items;

	internal static IReadOnlyList<PreIndustrialFoodLiquidCatalogueEntry> PreIndustrialFoodLiquidsForTesting =>
		PreIndustrialFoodCatalogue.Liquids;

	internal static string PreIndustrialPreparedFoodDefinitionForTesting(
		PreIndustrialFoodItemCatalogueEntry entry) =>
		BuildPreparedFoodDefinition(entry);

	private void SeedSharedPreIndustrialFoodCatalogue()
	{
		EnsurePreIndustrialFoodCatalogueLiquidVessel();
		SeedPreIndustrialFoodCatalogueScope(FoodCatalogueScope.Shared);
	}

	private void EnsurePreIndustrialFoodCatalogueLiquidVessel()
	{
		CreateItem(
			"preindustrial_food_catalogue_liquid_amphora",
			"amphora",
			"a shared catalogue liquid amphora",
			null,
			"A broad-shouldered fired-clay amphora used as the shared output vessel for pre-industrial catalogue liquids. Its sealed neck and sturdy handles make it suitable for water, oils, sauces, syrups, broths, and fermented drinks.",
			SizeCategory.Large,
			ItemQuality.Standard,
			9000.0,
			32.0m,
			true,
			false,
			"earthenware",
			[
				"Functions / Container / Watertight Container",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Amphora_Urna"
			],
			null,
			null,
			null,
			null,
			"Shared liquid-capable output vessel for the pre-industrial food catalogue.");
	}

	private void SeedMedievalFoodCatalogue()
	{
		SeedPreIndustrialFoodCatalogueScope(FoodCatalogueScope.Medieval);
	}

	private void SeedRenaissanceFoodCatalogue()
	{
		SeedPreIndustrialFoodCatalogueScope(FoodCatalogueScope.Renaissance);
	}

	private void SeedEarlyModernFoodCatalogue()
	{
		SeedPreIndustrialFoodCatalogueScope(FoodCatalogueScope.EarlyModern);
	}

	private void SeedPreIndustrialFoodCatalogueScope(FoodCatalogueScope scope)
	{
		ValidatePreIndustrialFoodCatalogue();
		RefreshPreIndustrialFoodIdentifierAllocators();
		EnsurePreIndustrialFoodCatalogueTags();

		foreach (var liquid in PreIndustrialFoodCatalogue.Liquids.Where(x => x.Scope == scope))
		{
			UpsertPreIndustrialFoodLiquid(liquid);
		}

		foreach (var item in PreIndustrialFoodCatalogue.Items.Where(x => x.Scope == scope))
		{
			UpsertPreIndustrialFoodItem(item);
		}
	}

	private void ValidatePreIndustrialFoodCatalogue()
	{
		if (_preIndustrialFoodCatalogueValidated)
		{
			return;
		}

		var errors = new List<string>();
		var items = PreIndustrialFoodCatalogue.Items;
		var liquids = PreIndustrialFoodCatalogue.Liquids;

		AddDuplicates(errors, items.Select(x => x.StableReference), "item stable reference");
		AddDuplicates(errors, liquids.Select(x => x.StableReference), "liquid stable reference");
		AddDuplicates(errors, items.Select(x => x.ShortDescription), "item short description");
		AddDuplicates(errors, liquids.Select(x => x.Name), "liquid name");
		AddDuplicates(errors, items.Select(x => NormaliseProse(x.FullDescription)), "item full description");
		AddDuplicates(errors, items.Where(x => x.Kind == FoodCatalogueKind.Prepared)
			.Select(x => NormaliseProse(x.Taste)), "item taste description");
		AddDuplicates(errors, liquids.Select(x => NormaliseProse(x.LongDescription)), "liquid long description");
		AddDuplicates(errors, liquids.Select(x => NormaliseProse(x.Taste)), "liquid taste description");
		AddDuplicates(errors, liquids.Select(x => NormaliseProse(x.Smell)), "liquid smell description");
		AddRepeatedScaffolds(errors, items.Select(x => x.FullDescription), "item full descriptions");
		AddRepeatedScaffolds(errors, items.Where(x => x.Kind == FoodCatalogueKind.Prepared).Select(x => x.Taste),
			"item taste descriptions");
		AddRepeatedScaffolds(errors, liquids.Select(x => x.LongDescription), "liquid long descriptions");
		AddRepeatedScaffolds(errors, liquids.Select(x => x.Taste), "liquid taste descriptions");

		var allStableReferences = items.Select(x => x.StableReference)
			.Concat(liquids.Select(x => x.StableReference));
		AddDuplicates(errors, allStableReferences, "cross-catalogue stable reference");

		foreach (var item in items)
		{
			ValidateStableReference(errors, item.StableReference, item.Scope);
			if (item.FullDescription.Length < 45 ||
			    item.FullDescription.Contains('$') ||
			    item.FullDescription.Contains('{'))
			{
				errors.Add($"{item.StableReference} does not have a substantive authored full description.");
			}

			if (!_materials.ContainsKey(item.Material))
			{
				errors.Add($"{item.StableReference} refers to missing material '{item.Material}'.");
			}

			if (!double.IsFinite(item.WeightInGrams) || item.WeightInGrams <= 0.0 || item.Cost < 0.0m)
			{
				errors.Add($"{item.StableReference} has invalid weight or cost.");
			}

			if (item.Kind == FoodCatalogueKind.Prepared)
			{
				if (item.Nutrition == FoodNutritionBand.None ||
				    item.Freshness == FoodFreshnessBand.None ||
				    item.Taste.Length < 20)
				{
					errors.Add($"{item.StableReference} is prepared food without complete nutrition, freshness, and taste.");
				}
			}
			else if (item.Nutrition != FoodNutritionBand.None ||
			         item.Freshness != FoodFreshnessBand.None ||
			         !string.IsNullOrWhiteSpace(item.Taste))
			{
				errors.Add($"{item.StableReference} is an intermediate but declares edible-only fields.");
			}

			if (item.Nutrition is FoodNutritionBand.BleakThin or FoodNutritionBand.BleakSolid &&
			    item.Quality > ItemQuality.Standard)
			{
				errors.Add($"{item.StableReference} is bleak food above Standard quality.");
			}

			if (item.Nutrition is FoodNutritionBand.Rich or FoodNutritionBand.Feast &&
			    item.Quality <= ItemQuality.Standard)
			{
				errors.Add($"{item.StableReference} is rich food without above-Standard quality.");
			}

			ValidateScopeAdmission(errors, item.StableReference, item.Scope, item.AdmissionProfile);
		}

		foreach (var liquid in liquids)
		{
			ValidateStableReference(errors, liquid.StableReference, liquid.Scope);
			ValidateScopeAdmission(errors, liquid.StableReference, liquid.Scope, liquid.AdmissionProfile);
			if (liquid.LongDescription.Length < 45 ||
			    liquid.Taste.Length < 20 ||
			    liquid.Smell.Length < 15 ||
			    liquid.Description.Contains('$') ||
			    liquid.LongDescription.Contains('$') ||
			    liquid.Description.Contains('{') ||
			    liquid.LongDescription.Contains('{'))
			{
				errors.Add($"{liquid.StableReference} does not have complete authored liquid prose.");
			}

			if (!double.IsFinite(liquid.AlcoholLitresPerLitre) ||
			    !double.IsFinite(liquid.WaterLitresPerLitre) ||
			    !double.IsFinite(liquid.FoodSatiatedHoursPerLitre) ||
			    !double.IsFinite(liquid.DrinkSatiatedHoursPerLitre) ||
			    liquid.AlcoholLitresPerLitre is < 0.0 or > 1.0 ||
			    liquid.WaterLitresPerLitre is < 0.0 or > 1.0 ||
			    liquid.FoodSatiatedHoursPerLitre < 0.0 ||
			    liquid.DrinkSatiatedHoursPerLitre < 0.0)
			{
				errors.Add($"{liquid.StableReference} has invalid liquid nutrition values.");
			}

			if (!StandardFoodLiquidAlcoholValues.Contains(liquid.AlcoholLitresPerLitre) ||
			    !StandardFoodLiquidWaterValues.Contains(liquid.WaterLitresPerLitre) ||
			    !StandardFoodLiquidSatiationValues.Contains(liquid.FoodSatiatedHoursPerLitre) ||
			    !StandardFoodLiquidThirstValues.Contains(liquid.DrinkSatiatedHoursPerLitre))
			{
				errors.Add($"{liquid.StableReference} does not use standardized food-liquid values.");
			}

			if (Telnet.GetColour(liquid.Colour) is null)
			{
				errors.Add($"{liquid.StableReference} has invalid ANSI display colour '{liquid.Colour}'.");
			}
		}

		foreach (var requiredComponent in new[] { "Holdable", "Destroyable_Misc", "Stack_Number" })
		{
			if (!_components.ContainsKey(requiredComponent))
			{
				errors.Add($"Required food catalogue component '{requiredComponent}' is missing.");
			}
		}

		if (errors.Count > 0)
		{
			throw new InvalidOperationException(
				$"The pre-industrial food catalogue is invalid:{Environment.NewLine}{string.Join(Environment.NewLine, errors)}");
		}

		_preIndustrialFoodCatalogueValidated = true;
	}

	private static void AddDuplicates(ICollection<string> errors, IEnumerable<string> values, string label)
	{
		foreach (var duplicate in values
			         .GroupBy(x => x, StringComparer.OrdinalIgnoreCase)
			         .Where(x => x.Count() > 1)
			         .Select(x => x.Key))
		{
			errors.Add($"Duplicate {label} '{duplicate}'.");
		}
	}

	private static void AddRepeatedScaffolds(
		ICollection<string> errors,
		IEnumerable<string> values,
		string label)
	{
		const int ngramLength = 6;
		const int maximumOccurrences = 8;
		var repeated = values
			.SelectMany(value =>
			{
				var words = Regex.Matches(value.ToLowerInvariant(), @"[a-z]+(?:'[a-z]+)?")
					.Cast<Match>()
					.Select(x => x.Value)
					.ToArray();
				return Enumerable.Range(0, Math.Max(0, words.Length - ngramLength + 1))
					.Select(index => string.Join(" ", words.Skip(index).Take(ngramLength)))
					.Distinct();
			})
			.GroupBy(x => x, StringComparer.Ordinal)
			.Where(x => x.Count() > maximumOccurrences)
			.OrderByDescending(x => x.Count())
			.FirstOrDefault();

		if (repeated is not null)
		{
			errors.Add($"{label} repeat the six-word scaffold '{repeated.Key}' {repeated.Count()} times.");
		}
	}

	private static string NormaliseProse(string value)
	{
		return Regex.Replace(value.Trim().ToLowerInvariant(), @"\s+", " ");
	}

	private static void ValidateStableReference(ICollection<string> errors, string stableReference,
		FoodCatalogueScope scope)
	{
		var expectedPrefix = scope switch
		{
			FoodCatalogueScope.Shared => "preindustrial_food_",
			FoodCatalogueScope.Medieval => "medieval_food_",
			FoodCatalogueScope.Renaissance => "renaissance_food_",
			FoodCatalogueScope.EarlyModern => "earlymodern_food_",
			_ => throw new ArgumentOutOfRangeException(nameof(scope), scope, null)
		};

		if (!stableReference.StartsWith(expectedPrefix, StringComparison.Ordinal) ||
		    !Regex.IsMatch(stableReference, "^[a-z0-9_]+$"))
		{
			errors.Add($"{stableReference} does not match the stable-reference policy for {scope}.");
		}
	}

	private static void ValidateScopeAdmission(
		ICollection<string> errors,
		string stableReference,
		FoodCatalogueScope scope,
		FoodAdmissionProfile admissionProfile)
	{
		if (scope == FoodCatalogueScope.Shared && admissionProfile == FoodAdmissionProfile.EraSpecific)
		{
			errors.Add($"{stableReference} is shared but has an era-specific admission profile.");
		}

		if (scope != FoodCatalogueScope.Shared && admissionProfile != FoodAdmissionProfile.EraSpecific)
		{
			errors.Add($"{stableReference} is era-specific but does not use the era-specific admission profile.");
		}
	}

	private void EnsurePreIndustrialFoodCatalogueTags()
	{
		foreach (var scope in Enum.GetValues<FoodCatalogueScope>())
		{
			EnsurePreIndustrialFoodTagPath($"{PreIndustrialPreparedFoodRoot} / Scope / {ScopeDisplay(scope)}");
			EnsurePreIndustrialFoodTagPath($"{PreIndustrialFoodLiquidRoot} / Scope / {ScopeDisplay(scope)}");
			EnsurePreIndustrialFoodTagPath($"{PreIndustrialFoodIntermediateRoot} / Scope / {ScopeDisplay(scope)}");
		}

		foreach (var family in Enum.GetValues<FoodCatalogueFamily>())
		{
			EnsurePreIndustrialFoodTagPath($"{PreIndustrialPreparedFoodRoot} / Family / {FamilyDisplay(family)}");
			EnsurePreIndustrialFoodTagPath($"{PreIndustrialFoodLiquidRoot} / Family / {FamilyDisplay(family)}");
			EnsurePreIndustrialFoodTagPath($"{PreIndustrialFoodIntermediateRoot} / Family / {FamilyDisplay(family)}");
		}

		foreach (var register in new[] { "Bleak", "Ordinary", "Rich" })
		{
			EnsurePreIndustrialFoodTagPath($"{PreIndustrialPreparedFoodRoot} / Social Register / {register}");
		}
	}

	private void UpsertPreIndustrialFoodItem(PreIndustrialFoodItemCatalogueEntry entry)
	{
		var tags = BuildFoodItemTags(entry);
		var componentNames = new List<string> { "Holdable", "Destroyable_Misc" };
		GameItemComponentProto? preparedComponent = null;
		if (entry.Kind == FoodCatalogueKind.Prepared)
		{
			preparedComponent = UpsertPreparedFoodComponent(entry);
			componentNames.Add(preparedComponent.Name);
		}
		else
		{
			componentNames.Add("Stack_Number");
		}

		_items.TryGetValue(entry.StableReference, out var item);
		item ??= _context!.GameItemProtos.Local
			         .FirstOrDefault(x => x.UniqueName == entry.StableReference) ??
		         _context.GameItemProtos
			         .FirstOrDefault(x => x.UniqueName == entry.StableReference);
		item ??= CreateItem(
			entry.StableReference,
			entry.Noun,
			entry.ShortDescription,
			null,
			entry.FullDescription,
			SizeCategory.Small,
			entry.Quality,
			entry.WeightInGrams,
			entry.Cost,
			false,
			false,
			entry.Material,
			tags,
			componentNames,
			null,
			null,
			null,
			null,
			BuildFoodBuilderNotes(entry),
			false);

		if (item is null)
		{
			throw new InvalidOperationException($"Could not create food catalogue item {entry.StableReference}.");
		}

		item.Name = entry.Noun.ToLowerInvariant();
		item.Keywords = new ExplodedString(entry.ShortDescription.Strip_A_An())
			.Words
			.Distinct()
			.ListToCommaSeparatedValues(" ");
		item.MaterialId = _materials[entry.Material].Id;
		item.Size = (int)SizeCategory.Small;
		item.Weight = entry.WeightInGrams;
		item.BaseItemQuality = (int)entry.Quality;
		item.ShortDescription = entry.ShortDescription;
		item.LongDescription = null;
		item.FullDescription = entry.FullDescription;
		item.CostInBaseCurrency = entry.Cost;
		item.IsHiddenFromPlayers = false;
		item.PermitPlayerSkins = false;
		item.UniqueName = entry.StableReference;
		item.BuilderNotes = MergeBuilderNotes(
			RemovePriorFoodBuilderNotes(item.BuilderNotes),
			BuildFoodBuilderNotes(entry));

		ReconcileFoodItemTags(item, tags);
		ReconcileFoodItemComponents(item, componentNames, preparedComponent);
		CacheReworkItem(entry.StableReference, item);
	}

	private GameItemComponentProto UpsertPreparedFoodComponent(PreIndustrialFoodItemCatalogueEntry entry)
	{
		var name = $"{PreparedFoodComponentPrefix}{entry.StableReference}";
		var definition = BuildPreparedFoodDefinition(entry);
		if (_components.TryGetValue(name, out var existing))
		{
			existing.Type = "PreparedFood";
			existing.Description = $"Stock prepared-food profile for {entry.StableReference}.";
			existing.Definition = definition;
			return existing;
		}

		var component = new GameItemComponentProto
		{
			Id = NextPreIndustrialFoodComponentId(),
			Name = name,
			Description = $"Stock prepared-food profile for {entry.StableReference}.",
			Type = "PreparedFood",
			RevisionNumber = 0,
			Definition = definition,
			EditableItem = NewPreIndustrialFoodEditableItem()
		};
		_context!.GameItemComponentProtos.Add(component);
		_components[name] = component;
		return component;
	}

	private static string BuildPreparedFoodDefinition(PreIndustrialFoodItemCatalogueEntry entry)
	{
		var nutrition = NutritionFor(entry.Nutrition);
		var freshness = FreshnessFor(entry.Freshness);
		return new XElement("Definition",
			new XAttribute("ServingScope", "WholeItem"),
			new XAttribute("Satiation", nutrition.Satiation),
			new XAttribute("Water", nutrition.Water),
			new XAttribute("Thirst", nutrition.Thirst),
			new XAttribute("Alcohol", 0.0),
			new XAttribute("Bites", nutrition.Bites),
			new XAttribute("QualityScale", 0.08),
			new XAttribute("StaleMultiplier", freshness.StaleMultiplier),
			new XAttribute("SpoiledMultiplier", freshness.SpoiledMultiplier),
			new XAttribute("LiquidAbsorption", nutrition.LiquidAbsorption),
			new XAttribute("StaleAfterSeconds", TimeSpan.FromHours(freshness.StaleHours).TotalSeconds),
			new XAttribute("SpoilAfterSeconds", TimeSpan.FromHours(freshness.SpoilHours).TotalSeconds),
			new XAttribute("Decorator", 0),
			new XElement("Taste", new XCData(entry.Taste)),
			new XElement("Short", new XCData(string.Empty)),
			new XElement("Full", new XCData(string.Empty)),
			new XElement("OnEatProg", 0),
			new XElement("OnStaleProg", 0),
			new XElement("Ingredients",
				new XElement("Ingredient",
					new XAttribute("role", "base"),
					new XAttribute("source", 0),
					new XAttribute("material", 0),
					new XAttribute("liquid", 0),
					new XAttribute("weight", 0),
					new XAttribute("volume", 0),
					new XAttribute("quality", (int)entry.Quality),
					new XElement("Description", new XCData(entry.ShortDescription.Strip_A_An())),
					new XElement("Taste", new XCData(entry.Taste)))),
			new XElement("DrugDoses"),
			new XElement("StaleDrugDoses")).ToString();
	}

	private void ReconcileFoodItemTags(GameItemProto item, IReadOnlyCollection<string> tagPaths)
	{
		var desiredTagIds = tagPaths
			.Select(path => _tagsByFullPath[path].Id)
			.ToHashSet();
		var relationships = _context!.Entry(item).State == EntityState.Added
			? item.GameItemProtosTags.ToList()
			: _context.GameItemProtosTags
				.Where(x => x.GameItemProtoId == item.Id && x.GameItemProtoRevisionNumber == item.RevisionNumber)
				.ToList();
		relationships.AddRange(item.GameItemProtosTags.Where(x =>
			relationships.All(y => y.TagId != x.TagId)));
		var catalogueTagIds = _tagsByFullPath
			.Where(x => IsPreIndustrialFoodCatalogueTagPath(x.Key))
			.Select(x => x.Value.Id)
			.ToHashSet();
		foreach (var relationship in relationships.Where(x =>
			         catalogueTagIds.Contains(x.TagId) && !desiredTagIds.Contains(x.TagId)))
		{
			_context.GameItemProtosTags.Remove(relationship);
		}

		var existingTagIds = relationships.Select(x => x.TagId).ToHashSet();
		foreach (var path in tagPaths)
		{
			var tag = _tagsByFullPath[path];
			if (!existingTagIds.Add(tag.Id))
			{
				continue;
			}

			item.GameItemProtosTags.Add(new GameItemProtosTags
			{
				GameItemProto = item,
				GameItemProtoRevisionNumber = item.RevisionNumber,
				Tag = tag,
				TagId = tag.Id
			});
		}
	}

	private static bool IsPreIndustrialFoodCatalogueTagPath(string path)
	{
		return path.StartsWith(PreIndustrialPreparedFoodRoot, StringComparison.OrdinalIgnoreCase) ||
		       path.StartsWith(PreIndustrialFoodLiquidRoot, StringComparison.OrdinalIgnoreCase) ||
		       path.StartsWith(PreIndustrialFoodIntermediateRoot, StringComparison.OrdinalIgnoreCase);
	}

	private void ReconcileFoodItemComponents(
		GameItemProto item,
		IReadOnlyCollection<string> componentNames,
		GameItemComponentProto? preparedComponent)
	{
		var componentsById = _components.Values
			.GroupBy(x => x.Id)
			.ToDictionary(x => x.Key, x => x.OrderByDescending(y => y.RevisionNumber).First());
		var relationships = _context!.Entry(item).State == EntityState.Added
			? item.GameItemProtosGameItemComponentProtos.ToList()
			: _context.GameItemProtosGameItemComponentProtos
				.Where(x => x.GameItemProtoId == item.Id && x.GameItemProtoRevision == item.RevisionNumber)
				.ToList();
		relationships.AddRange(item.GameItemProtosGameItemComponentProtos.Where(x =>
			relationships.All(y =>
				y.GameItemComponentProtoId != x.GameItemComponentProtoId ||
				y.GameItemComponentRevision != x.GameItemComponentRevision)));

		var desiredComponentNames = componentNames.ToHashSet(StringComparer.OrdinalIgnoreCase);
		foreach (var relationship in relationships.Where(x =>
			         componentsById.TryGetValue(x.GameItemComponentProtoId, out var component) &&
			         (component.Name.StartsWith(PreparedFoodComponentPrefix, StringComparison.OrdinalIgnoreCase) ||
			          component.Name.Equals("Stack_Number", StringComparison.OrdinalIgnoreCase)) &&
			         !desiredComponentNames.Contains(component.Name)))
		{
			_context.GameItemProtosGameItemComponentProtos.Remove(relationship);
		}

		var existingIds = relationships
			.Select(x => x.GameItemComponentProtoId)
			.ToHashSet();
		foreach (var componentName in componentNames)
		{
			var component = _components[componentName];
			if (!existingIds.Add(component.Id))
			{
				continue;
			}

			item.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
			{
				GameItemProto = item,
				GameItemProtoRevision = item.RevisionNumber,
				GameItemComponent = component,
				GameItemComponentProtoId = component.Id,
				GameItemComponentRevision = component.RevisionNumber
			});
		}
	}

	private void UpsertPreIndustrialFoodLiquid(PreIndustrialFoodLiquidCatalogueEntry entry)
	{
		if (!_liquids.TryGetValue(entry.Name, out var liquid))
		{
			liquid = new Liquid
			{
				Id = NextPreIndustrialFoodLiquidId(),
				Name = entry.Name
			};
			_context!.Liquids.Add(liquid);
			_liquids[entry.Name] = liquid;
		}

		liquid.Description = entry.Description;
		liquid.LongDescription = entry.LongDescription;
		liquid.TasteText = entry.Taste;
		liquid.VagueTasteText = entry.Taste;
		liquid.SmellText = entry.Smell;
		liquid.VagueSmellText = entry.Smell;
		liquid.TasteIntensity = 500.0;
		liquid.SmellIntensity = 200.0;
		liquid.AlcoholLitresPerLitre = entry.AlcoholLitresPerLitre;
		liquid.WaterLitresPerLitre = entry.WaterLitresPerLitre;
		liquid.FoodSatiatedHoursPerLitre = entry.FoodSatiatedHoursPerLitre;
		liquid.DrinkSatiatedHoursPerLitre = entry.DrinkSatiatedHoursPerLitre;
		liquid.Viscosity = entry.Family switch
		{
			FoodCatalogueFamily.Oil => 1.8,
			FoodCatalogueFamily.Syrup => 2.0,
			FoodCatalogueFamily.Sauce => 1.5,
			_ => 1.0
		};
		liquid.Density = entry.Family is FoodCatalogueFamily.Oil ? 0.92 : 1.02;
		liquid.Organic = true;
		liquid.ThermalConductivity = 0.6;
		liquid.ElectricalConductivity = 0.0001;
		liquid.SpecificHeatCapacity = 4184.0;
		liquid.DisplayColour = entry.Colour;
		liquid.DampDescription = $"It is damp with {entry.Name}";
		liquid.WetDescription = $"It is wet with {entry.Name}";
		liquid.DrenchedDescription = $"It is soaking wet with {entry.Name}";
		liquid.DampShortDescription = "(damp)";
		liquid.WetShortDescription = "(wet)";
		liquid.DrenchedShortDescription = "(soaked)";
		liquid.SolventVolumeRatio = 1.0;
		liquid.InjectionConsequence = 0;
		liquid.ResidueVolumePercentage = 0.01;
		liquid.RelativeEnthalpy = 1.0;
		liquid.LeaveResidueInRooms = false;
		liquid.SurfaceReactionInfo = string.Empty;

		ReconcileFoodLiquidTags(liquid, BuildFoodLiquidTags(entry));
	}

	private void ReconcileFoodLiquidTags(Liquid liquid, IReadOnlyCollection<string> tagPaths)
	{
		var desiredTagIds = tagPaths
			.Select(path => _tagsByFullPath[path].Id)
			.ToHashSet();
		var relationships = _context!.Entry(liquid).State == EntityState.Added
			? new List<LiquidsTags>()
			: _context.LiquidsTags
				.Where(x => x.LiquidId == liquid.Id)
				.ToList();
		relationships.AddRange(_context.LiquidsTags.Local.Where(x =>
			x.LiquidId == liquid.Id && relationships.All(y => y.TagId != x.TagId)));
		var catalogueTagIds = _tagsByFullPath
			.Where(x => IsPreIndustrialFoodCatalogueTagPath(x.Key))
			.Select(x => x.Value.Id)
			.ToHashSet();
		foreach (var relationship in relationships.Where(x =>
			         catalogueTagIds.Contains(x.TagId) && !desiredTagIds.Contains(x.TagId)))
		{
			_context.LiquidsTags.Remove(relationship);
		}

		var existingTagIds = relationships.Select(x => x.TagId).ToHashSet();
		foreach (var path in tagPaths)
		{
			var tag = _tagsByFullPath[path];
			if (!existingTagIds.Add(tag.Id))
			{
				continue;
			}

			_context.LiquidsTags.Add(new LiquidsTags
			{
				Liquid = liquid,
				LiquidId = liquid.Id,
				Tag = tag,
				TagId = tag.Id
			});
		}
	}

	private IReadOnlyCollection<string> BuildFoodItemTags(PreIndustrialFoodItemCatalogueEntry entry)
	{
		if (entry.Kind == FoodCatalogueKind.Intermediate)
		{
			return
			[
				$"{PreIndustrialFoodIntermediateRoot} / Scope / {ScopeDisplay(entry.Scope)}",
				$"{PreIndustrialFoodIntermediateRoot} / Family / {FamilyDisplay(entry.Family)}"
			];
		}

		var register = entry.Quality switch
		{
			<= ItemQuality.Substandard => "Bleak",
			ItemQuality.Standard => "Ordinary",
			_ => "Rich"
		};
		return
		[
			$"{PreIndustrialPreparedFoodRoot} / Scope / {ScopeDisplay(entry.Scope)}",
			$"{PreIndustrialPreparedFoodRoot} / Family / {FamilyDisplay(entry.Family)}",
			$"{PreIndustrialPreparedFoodRoot} / Social Register / {register}"
		];
	}

	private static IReadOnlyCollection<string> BuildFoodLiquidTags(PreIndustrialFoodLiquidCatalogueEntry entry)
	{
		return
		[
			$"{PreIndustrialFoodLiquidRoot} / Scope / {ScopeDisplay(entry.Scope)}",
			$"{PreIndustrialFoodLiquidRoot} / Family / {FamilyDisplay(entry.Family)}"
		];
	}

	private static string BuildFoodBuilderNotes(PreIndustrialFoodItemCatalogueEntry entry)
	{
		return
			$"Hand-authored pre-industrial food catalogue row. Scope: {ScopeDisplay(entry.Scope)}. " +
			$"Admission profile: {entry.AdmissionProfile}. Nutrition band: {entry.Nutrition}.";
	}

	private static string? RemovePriorFoodBuilderNotes(string? builderNotes)
	{
		if (string.IsNullOrWhiteSpace(builderNotes))
		{
			return builderNotes;
		}

		var retained = builderNotes
			.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries)
			.Where(x => !x.TrimStart().StartsWith(
				"Hand-authored pre-industrial food catalogue row.",
				StringComparison.OrdinalIgnoreCase))
			.ToArray();
		return retained.Length == 0 ? null : string.Join(Environment.NewLine, retained);
	}

	private Tag EnsurePreIndustrialFoodTagPath(string path)
	{
		if (_tagsByFullPath.TryGetValue(path, out var existing))
		{
			return existing;
		}

		var parts = path.Split('/', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
		Tag? parent = null;
		var fullPath = string.Empty;
		foreach (var part in parts)
		{
			fullPath = string.IsNullOrWhiteSpace(fullPath) ? part : $"{fullPath} / {part}";
			if (_tagsByFullPath.TryGetValue(fullPath, out existing))
			{
				parent = existing;
				continue;
			}

			var parentId = parent?.Id;
			existing = _context!.Tags.Local
				           .FirstOrDefault(x => x.Name.Equals(part, StringComparison.OrdinalIgnoreCase) &&
				                                x.ParentId == parentId) ??
			           _context.Tags
				           .AsEnumerable()
				           .FirstOrDefault(x => x.Name.Equals(part, StringComparison.OrdinalIgnoreCase) &&
				                                x.ParentId == parentId);
			if (existing is null)
			{
				existing = new Tag
				{
					Id = NextPreIndustrialFoodTagId(),
					Name = part,
					Parent = parent,
					ParentId = parent?.Id
				};
				_context.Tags.Add(existing);
			}

			_tagsByFullPath[fullPath] = existing;
			parent = existing;
		}

		return parent!;
	}

	private long NextPreIndustrialFoodTagId()
	{
		if (_nextPreIndustrialFoodTagId is null)
		{
			RefreshPreIndustrialFoodIdentifierAllocators();
		}

		var result = _nextPreIndustrialFoodTagId!.Value;
		_nextPreIndustrialFoodTagId = result + 1L;
		return result;
	}

	private long NextPreIndustrialFoodComponentId()
	{
		if (_nextPreIndustrialFoodComponentId is null)
		{
			RefreshPreIndustrialFoodIdentifierAllocators();
		}

		var result = _nextPreIndustrialFoodComponentId!.Value;
		_nextPreIndustrialFoodComponentId = result + 1L;
		return result;
	}

	private long NextPreIndustrialFoodLiquidId()
	{
		if (_nextPreIndustrialFoodLiquidId is null)
		{
			RefreshPreIndustrialFoodIdentifierAllocators();
		}

		var result = _nextPreIndustrialFoodLiquidId!.Value;
		_nextPreIndustrialFoodLiquidId = result + 1L;
		return result;
	}

	private void RefreshPreIndustrialFoodIdentifierAllocators()
	{
		_nextPreIndustrialFoodTagId = Math.Max(
			_context!.Tags.Any() ? _context.Tags.Max(x => x.Id) : 0L,
			_context.Tags.Local.Any() ? _context.Tags.Local.Max(x => x.Id) : 0L) + 1L;
		_nextPreIndustrialFoodComponentId = Math.Max(
			_context.GameItemComponentProtos.Any() ? _context.GameItemComponentProtos.Max(x => x.Id) : 0L,
			_context.GameItemComponentProtos.Local.Any() ? _context.GameItemComponentProtos.Local.Max(x => x.Id) : 0L) + 1L;
		_nextPreIndustrialFoodLiquidId = Math.Max(
			_context.Liquids.Any() ? _context.Liquids.Max(x => x.Id) : 0L,
			_context.Liquids.Local.Any() ? _context.Liquids.Local.Max(x => x.Id) : 0L) + 1L;
	}

	private EditableItem NewPreIndustrialFoodEditableItem()
	{
		return new EditableItem
		{
			RevisionNumber = 0,
			RevisionStatus = 4,
			BuilderAccountId = _dbAccount.Id,
			BuilderDate = _now,
			BuilderComment = "Auto-generated by the pre-industrial food catalogue",
			ReviewerAccountId = _dbAccount.Id,
			ReviewerComment = "Auto-generated by the pre-industrial food catalogue",
			ReviewerDate = _now
		};
	}

	private static string ScopeDisplay(FoodCatalogueScope scope)
	{
		return scope switch
		{
			FoodCatalogueScope.Shared => "Shared",
			FoodCatalogueScope.Medieval => "Medieval",
			FoodCatalogueScope.Renaissance => "Renaissance",
			FoodCatalogueScope.EarlyModern => "Early Modern",
			_ => throw new ArgumentOutOfRangeException(nameof(scope), scope, null)
		};
	}

	private static string FamilyDisplay(FoodCatalogueFamily family)
	{
		return Regex.Replace(family.ToString(), "([a-z])([A-Z])", "$1 $2");
	}

	private static FoodNutritionValues NutritionFor(FoodNutritionBand band)
	{
		return band switch
		{
			FoodNutritionBand.BleakThin => new(1.5, 0.35, 0.1, 6, 0.5),
			FoodNutritionBand.BleakSolid => new(2.5, 0.05, -0.05, 6, 0.1),
			FoodNutritionBand.Light => new(2.5, 0.12, 0.0, 4, 0.2),
			FoodNutritionBand.Standard => new(4.0, 0.12, 0.0, 6, 0.25),
			FoodNutritionBand.Staple => new(5.0, 0.08, -0.05, 8, 0.2),
			FoodNutritionBand.Hearty => new(6.0, 0.25, 0.05, 8, 0.5),
			FoodNutritionBand.Rich => new(6.5, 0.2, 0.0, 8, 0.45),
			FoodNutritionBand.Feast => new(8.0, 0.25, 0.0, 10, 0.5),
			FoodNutritionBand.Sweet => new(3.5, 0.05, -0.05, 5, 0.15),
			FoodNutritionBand.Preserved => new(3.5, 0.02, -0.25, 6, 0.05),
			FoodNutritionBand.Fresh => new(2.0, 0.25, 0.15, 5, 0.3),
			FoodNutritionBand.Condiment => new(0.5, 0.02, -0.15, 3, 0.05),
			_ => throw new ArgumentOutOfRangeException(nameof(band), band, null)
		};
	}

	private static FoodFreshnessValues FreshnessFor(FoodFreshnessBand band)
	{
		return band switch
		{
			FoodFreshnessBand.Fresh => new(24, 72, 0.7, 0.15),
			FoodFreshnessBand.Cooked => new(48, 96, 0.7, 0.15),
			FoodFreshnessBand.Bread => new(72, 168, 0.75, 0.2),
			FoodFreshnessBand.Dry => new(168, 720, 0.85, 0.3),
			FoodFreshnessBand.Preserved => new(336, 2160, 0.9, 0.4),
			FoodFreshnessBand.Fermented => new(168, 720, 0.85, 0.3),
			FoodFreshnessBand.ShelfStable => new(720, 4320, 0.95, 0.5),
			_ => throw new ArgumentOutOfRangeException(nameof(band), band, null)
		};
	}

	private sealed record FoodNutritionValues(
		double Satiation,
		double Water,
		double Thirst,
		double Bites,
		double LiquidAbsorption);

	private sealed record FoodFreshnessValues(
		double StaleHours,
		double SpoilHours,
		double StaleMultiplier,
		double SpoiledMultiplier);
}
