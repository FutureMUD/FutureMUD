#nullable enable

using MudSharp.Models;
using MudSharp.Framework;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private readonly Dictionary<string, ItemSeederManifestEntry> _manifestEntries =
		new(StringComparer.OrdinalIgnoreCase);
	private string _activeManifestModule = "foundations";
	private IReadOnlyCollection<string> _activeManifestEras = [];
	private readonly Dictionary<string, ItemSeederReconciliationResult> _manifestResults =
		new(StringComparer.OrdinalIgnoreCase);
	private readonly HashSet<string> _customizedManifestAggregates = new(StringComparer.OrdinalIgnoreCase);
	private bool _manifestCaptureOnly;

	private static string BuildManifestBackedDescription()
	{
		try
		{
			var repositoryRoot = ItemSeederManifestCatalogue.FindRepositoryRoot();
			var path = System.IO.Path.Combine(repositoryRoot,
				ItemSeederManifestCatalogue.DefaultRelativePath.Replace('/', System.IO.Path.DirectorySeparatorChar));
			if (System.IO.File.Exists(path))
			{
				var manifest = ItemSeederManifestCatalogue.Load(path);
				var itemCount = manifest.Entries.Count(x => x.EntityType.Equals("item", StringComparison.OrdinalIgnoreCase));
				var craftCount = manifest.Entries.Count(x => x.EntityType.Equals("craft", StringComparison.OrdinalIgnoreCase));
				var otherCount = manifest.Entries.Count - itemCount - craftCount;
				return $"This seeder installs and safely reconciles {itemCount:N0} stock item prototypes, {craftCount:N0} crafts, and {otherCount:N0} supporting stock aggregates across the implemented eras. Reruns repair untouched stock, retain builder customization, and can add eras without removing previously installed content.";
			}
		}
		catch (Exception)
		{
			// Packaged builds may not carry the repository layout used by development tooling.
		}

		return "This seeder installs and safely reconciles the manifest-backed stock item, craft, outfit, food, and vehicle catalogues for the implemented eras. Reruns repair untouched stock, retain builder customization, and can add eras without removing previously installed content.";
	}

	private enum ManifestAggregateDisposition
	{
		Insert,
		Unchanged,
		Update,
		Customized
	}

	private sealed record ItemManifestDefinition(
		string StableReference,
		string Noun,
		string ShortDescription,
		string Keywords,
		string? LongDescription,
		string FullDescription,
		int Size,
		int Quality,
		double WeightInGrams,
		decimal Cost,
		bool Skinnable,
		bool HiddenFromPlayers,
		string Material,
		IReadOnlyCollection<string> Tags,
		IReadOnlyCollection<string> Components,
		string? MorphToStableReference,
		string? MorphEmote,
		int MorphTimeSeconds,
		string? DestroyedItemStableReference);

	private sealed record CraftManifestDefinition(
		string Name,
		string Category,
		string Blurb,
		string Action,
		string ActiveCraftItemSdesc,
		string? AppearProg,
		string? CanUseProg,
		string? WhyCannotUseProg,
		string? OnStartProg,
		string? OnFinishProg,
		string? OnCancelProg,
		string? Trait,
		int Difficulty,
		int Threshold,
		int FreeChecks,
		int FailPhase,
		bool Interruptable,
		IReadOnlyCollection<CraftPhaseSpec> Phases,
		IReadOnlyCollection<CraftInputSpec> Inputs,
		IReadOnlyCollection<CraftToolSpec> Tools,
		IReadOnlyCollection<CraftProductSpec> Products,
		IReadOnlyCollection<CraftProductSpec> FailProducts);

	private sealed record CraftLivePhase(
		int Number,
		double Seconds,
		string Echo,
		string FailEcho,
		int Exertion,
		double Stamina);

	private sealed record CraftLiveInput(string Type, double QualityWeight, string Definition);
	private sealed record CraftLiveTool(
		string Type,
		double QualityWeight,
		int DesiredState,
		bool UseToolDuration,
		string Definition);
	private sealed record CraftLiveProduct(
		string Type,
		bool IsFailProduct,
		int? MaterialDefiningInputIndex,
		string Definition);

	private sealed record CraftLiveDefinition(
		string Name,
		string Category,
		string Blurb,
		string Action,
		string ActiveCraftItemSdesc,
		long? AppearProgId,
		long? CanUseProgId,
		long? WhyCannotUseProgId,
		long? OnStartProgId,
		long? OnFinishProgId,
		long? OnCancelProgId,
		long? TraitId,
		int Difficulty,
		int Threshold,
		int FreeChecks,
		int FailPhase,
		bool Interruptable,
		string QualityFormula,
		double CheckQualityWeighting,
		double InputQualityWeighting,
		double ToolQualityWeighting,
		bool IsPracticalCheck,
		IReadOnlyCollection<CraftLivePhase> Phases,
		IReadOnlyCollection<CraftLiveInput> Inputs,
		IReadOnlyCollection<CraftLiveTool> Tools,
		IReadOnlyCollection<CraftLiveProduct> Products);

	private sealed record ComponentManifestDefinition(
		string Name,
		string Description,
		string Type,
		int RevisionNumber,
		string Definition);

	private sealed record MaterialManifestDefinition(
		string Name,
		string MaterialDescription,
		int BehaviourType,
		string ResidueSdesc,
		string ResidueDesc,
		string ResidueColour);

	private sealed record ProgManifestDefinition(
		string Name,
		string Category,
		string Subcategory,
		long ReturnType,
		string Comment,
		string Text,
		IReadOnlyCollection<ProgParameterManifestDefinition> Parameters);

	private sealed record ProgParameterManifestDefinition(long Type, string Name);

	private sealed record KnowledgeManifestDefinition(
		string Name,
		string Type,
		string Subtype,
		string Description,
		string LongDescription,
		int LearnableType,
		int LearnDifficulty,
		int TeachDifficulty,
		int LearningSessionsRequired,
		string CanAcquireProg,
		string CanLearnProg);

	private sealed record OutfitManifestItemDefinition(
		string ItemStableReference,
		string? SkinStableReference,
		string EntryKey,
		string? WearProfile,
		int Placement,
		string? ContainerKey,
		string LoadArguments,
		int WearOrder);

	private sealed record OutfitManifestDefinition(
		string StableKey,
		string Name,
		string Description,
		int Exclusivity,
		IReadOnlyCollection<OutfitManifestItemDefinition> Items);

	private sealed record ItemSkinManifestDefinition(
		string StableReference,
		string BaseItemStableReference,
		string? ItemName,
		string? ShortDescription,
		string? FullDescription,
		string? LongDescription,
		int? Quality,
		bool IsPublic,
		string CanUseSkinProg);

	private sealed record LiquidManifestDefinition(
		string Name,
		string Description,
		string LongDescription,
		string Taste,
		string Smell,
		double TasteIntensity,
		double SmellIntensity,
		double Alcohol,
		double Water,
		double FoodSatiation,
		double DrinkSatiation,
		double Viscosity,
		double Density,
		bool Organic,
		double ThermalConductivity,
		double ElectricalConductivity,
		double SpecificHeatCapacity,
		string DisplayColour,
		double SolventVolumeRatio,
		int InjectionConsequence,
		double ResidueVolumePercentage,
		double RelativeEnthalpy,
		bool LeaveResidueInRooms,
		string SurfaceReactionInfo,
		IReadOnlyCollection<string> Tags);

	private sealed record TagManifestDefinition(string FullPath, string Name, string? ParentPath);

	private sealed record CommoditySpoilageManifestDefinition(
		string Name,
		string Description,
		bool Enabled,
		int Priority,
		string? Material,
		string? MaterialTag,
		string? CommodityTag,
		string ResultMaterial,
		string? ResultCommodityTag,
		long SecondsUntilSpoiled,
		string? SpoilEcho);

	private sealed record VariableManifestDefinition(
		long OwnerType,
		string Property,
		long ContainedType,
		string DefaultValue);

	private sealed record VehicleGraphManifestDefinition(
		string Vehicle,
		IReadOnlyDictionary<string, IReadOnlyCollection<string>> Children);

	private IDisposable UseManifestModule(string module, params string[] eraAdmissions)
	{
		var priorModule = _activeManifestModule;
		var priorEras = _activeManifestEras;
		_activeManifestModule = module;
		_activeManifestEras = eraAdmissions;
		return new ManifestModuleScope(() =>
		{
			_activeManifestModule = priorModule;
			_activeManifestEras = priorEras;
		});
	}

	private ItemSeederManifestEntry RegisterManifestAggregate(
		string entityType,
		string stableKey,
		object canonicalDefinition,
		IEnumerable<string>? dependencies = null,
		ItemSeederOwnershipPolicy ownershipPolicy = ItemSeederOwnershipPolicy.StockAggregate,
		string? module = null,
		IEnumerable<string>? eraAdmissions = null)
	{
		if (entityType == "item" && FindHistoricalClothingSource(stableKey) is not null)
		{
			// Reuse expands the existing aggregate's admissions; it never gives the same item a second owner or identity.
			eraAdmissions = (eraAdmissions ?? _activeManifestEras).Concat(IndustrialisedCatalogue.Clothing.Bases
				.Where(x => x.ItemReference == stableKey).SelectMany(x => x.EraAdmissions));
		}
		var entry = new ItemSeederManifestEntry(
			entityType,
			stableKey.Trim(),
			module ?? _activeManifestModule,
			(eraAdmissions ?? _activeManifestEras)
				.Distinct(StringComparer.OrdinalIgnoreCase)
				.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
				.ToArray(),
			(dependencies ?? [])
				.Distinct(StringComparer.OrdinalIgnoreCase)
				.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
				.ToArray(),
			ownershipPolicy,
			ItemSeederManifestCatalogue.Fingerprint(canonicalDefinition));
		var identity = $"{entry.EntityType}\u001f{entry.StableKey}";
		if (_manifestEntries.TryGetValue(identity, out var existing))
		{
			if (!existing.Fingerprint.Equals(entry.Fingerprint, StringComparison.OrdinalIgnoreCase))
			{
				throw new InvalidOperationException(
					$"ItemSeeder manifest identity {entry.EntityType}:{entry.StableKey} was registered with two different definitions.");
			}

			return existing;
		}

		_manifestEntries.Add(identity, entry);
		return entry;
	}

	internal IReadOnlyCollection<ItemSeederManifestEntry> GetCapturedManifestEntriesForTesting()
	{
		return _manifestEntries.Values.ToArray();
	}

	private bool IsManifestAggregateRegistered(string entityType, string stableKey)
	{
		return _manifestEntries.ContainsKey($"{entityType}\u001f{stableKey}");
	}

	internal ItemSeederManifestDocument CaptureManifest(
		MudSharp.Database.FuturemudDatabaseContext context,
		string repositoryRoot)
	{
		_manifestCaptureOnly = true;
		_manifestEntries.Clear();
		_manifestResults.Clear();
		_customizedManifestAggregates.Clear();
		try
		{
			SeedData(context, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
			{
				["eras"] = "antiquity medieval renaissance earlymodern industrial",
				["technologyprofile"] = "neutral"
			});
			return ItemSeederManifestCatalogue.BuildDocument(
				_manifestEntries.Values,
				ItemSeederManifestCatalogue.ComputeSourceFingerprint(repositoryRoot));
		}
		finally
		{
			_manifestCaptureOnly = false;
		}
	}

	private ItemManifestDefinition BuildItemManifestDefinition(
		string stableReference,
		string noun,
		string shortDescription,
		string? longDescription,
		string fullDescription,
		int size,
		int quality,
		double weightInGrams,
		decimal cost,
		bool skinnable,
		bool hiddenFromPlayers,
		string material,
		IEnumerable<string> tags,
		IEnumerable<string> components,
		string? morphToStableReference,
		string? morphEmote,
		TimeSpan? morphTimer,
		string? destroyedItemStableReference)
	{
		return new ItemManifestDefinition(
			stableReference.Trim(),
			noun.ToLowerInvariant(),
			shortDescription,
			new ExplodedString(shortDescription.Strip_A_An()).Words.Distinct().ListToCommaSeparatedValues(" "),
			longDescription,
			fullDescription,
			size,
			quality,
			weightInGrams,
			cost,
			skinnable,
			hiddenFromPlayers,
			material,
			tags.Where(x => !string.IsNullOrWhiteSpace(x))
				.Distinct(StringComparer.OrdinalIgnoreCase)
				.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
				.ToArray(),
			components.Where(x => !string.IsNullOrWhiteSpace(x))
				.Distinct(StringComparer.OrdinalIgnoreCase)
				.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
				.ToArray(),
			morphToStableReference,
			morphEmote ?? "$0 $?1|morphs into $1|decays into nothing$.",
			(int)(morphTimer?.TotalSeconds ?? 0),
			destroyedItemStableReference);
	}

	private static CraftManifestDefinition BuildCraftManifestDefinition(CraftDefinitionSpec spec)
	{
		return new CraftManifestDefinition(
			spec.Name,
			spec.Category,
			spec.Blurb,
			spec.Action,
			spec.ActiveCraftItemSdesc,
			spec.AppearProg?.FunctionName,
			spec.CanUseProg?.FunctionName,
			spec.WhyCannotUseProg?.FunctionName,
			spec.OnStartProg?.FunctionName,
			spec.OnFinishProg?.FunctionName,
			spec.OnCancelProg?.FunctionName,
			spec.Trait?.Name,
			(int)spec.Difficulty,
			(int)spec.Threshold,
			spec.FreeChecks,
			spec.FailPhase,
			spec.Interruptable,
			spec.Phases,
			spec.Inputs,
			spec.Tools,
			spec.Products,
			spec.FailProducts);
	}

	private static CraftLiveDefinition BuildLiveCraftManifestDefinition(Craft craft)
	{
		return new CraftLiveDefinition(
			craft.Name,
			craft.Category,
			craft.Blurb,
			craft.ActionDescription,
			craft.ActiveCraftItemSdesc,
			craft.AppearInCraftsListProgId,
			craft.CanUseProgId,
			craft.WhyCannotUseProgId,
			craft.OnUseProgStartId,
			craft.OnUseProgCompleteId,
			craft.OnUseProgCancelId,
			craft.CheckTraitId,
			craft.CheckDifficulty,
			craft.FailThreshold,
			craft.FreeSkillChecks,
			craft.FailPhase,
			craft.Interruptable,
			craft.QualityFormula,
			craft.CheckQualityWeighting,
			craft.InputQualityWeighting,
			craft.ToolQualityWeighting,
			craft.IsPracticalCheck,
			craft.CraftPhases
				.OrderBy(x => x.PhaseNumber)
				.Select(x => new CraftLivePhase(
					x.PhaseNumber,
					x.PhaseLengthInSeconds,
					x.Echo,
					x.FailEcho,
					x.ExertionLevel,
					x.StaminaUsage))
				.ToArray(),
			craft.CraftInputs
				.OrderBy(x => x.OriginalAdditionTime)
				.ThenBy(x => x.Id)
				.Select(x => new CraftLiveInput(x.InputType, x.InputQualityWeight, x.Definition))
				.ToArray(),
			craft.CraftTools
				.OrderBy(x => x.OriginalAdditionTime)
				.ThenBy(x => x.Id)
				.Select(x => new CraftLiveTool(
					x.ToolType,
					x.ToolQualityWeight,
					x.DesiredState,
					x.UseToolDuration,
					x.Definition))
				.ToArray(),
			craft.CraftProducts
				.OrderBy(x => x.IsFailProduct)
				.ThenBy(x => x.OriginalAdditionTime)
				.ThenBy(x => x.Id)
				.Select(x => new CraftLiveProduct(
					x.ProductType,
					x.IsFailProduct,
					x.MaterialDefiningInputIndex,
					x.Definition))
				.ToArray());
	}

	private ItemManifestDefinition BuildLiveItemManifestDefinition(
		GameItemProto item,
		string stableReference)
	{
		var tagsById = _tagsByFullPath.Values
			.GroupBy(x => x.Id)
			.ToDictionary(x => x.Key, x => _tagsByFullPath.First(y => y.Value.Id == x.Key).Key);
		var tagIds = item.GameItemProtosTags.Select(x => x.TagId).ToHashSet();

		var componentKeys = item.GameItemProtosGameItemComponentProtos
			.Select(x => (x.GameItemComponentProtoId, x.GameItemComponentRevision))
			.ToHashSet();

		var componentNames = componentKeys
			.Where(_componentNamesByKey.ContainsKey)
			.Select(x => _componentNamesByKey[x])
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
			.ToArray();
		var materialName = _materialNamesById.GetValueOrDefault(item.MaterialId) ??
			_context!.Materials.Where(x => x.Id == item.MaterialId).Select(x => x.Name).First();
		var morphReference = ResolveItemStableReference(item.MorphGameItemProtoId);
		var destroyedReference = ResolveItemStableReference(item.OnDestroyedGameItemProtoId);

		return new ItemManifestDefinition(
			stableReference.Trim(),
			item.Name,
			item.ShortDescription,
			item.Keywords,
			item.LongDescription,
			item.FullDescription,
			item.Size,
			item.BaseItemQuality,
			item.Weight,
			item.CostInBaseCurrency,
			item.PermitPlayerSkins,
			item.IsHiddenFromPlayers,
			materialName,
			tagIds.Where(tagsById.ContainsKey)
				.Select(x => tagsById[x])
				.Distinct(StringComparer.OrdinalIgnoreCase)
				.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
				.ToArray(),
			componentNames,
			morphReference,
			item.MorphEmote,
			item.MorphTimeSeconds,
			destroyedReference);
	}

	private static bool IsRepairableMissingItemStock(
		ItemManifestDefinition live,
		ItemManifestDefinition expected)
	{
		if (live.Tags.Except(expected.Tags, StringComparer.OrdinalIgnoreCase).Any() ||
		    live.Components.Except(expected.Components, StringComparer.OrdinalIgnoreCase).Any())
		{
			return false;
		}

		if (live.MorphToStableReference is not null &&
		    !live.MorphToStableReference.Equals(expected.MorphToStableReference, StringComparison.OrdinalIgnoreCase))
		{
			return false;
		}
		if (live.DestroyedItemStableReference is not null &&
		    !live.DestroyedItemStableReference.Equals(expected.DestroyedItemStableReference, StringComparison.OrdinalIgnoreCase))
		{
			return false;
		}

		var normalized = live with
		{
			Tags = expected.Tags,
			Components = expected.Components
		};
		if (expected.MorphToStableReference is not null || expected.DestroyedItemStableReference is not null ||
		    expected.MorphTimeSeconds > 0)
		{
			normalized = normalized with
			{
				MorphToStableReference = expected.MorphToStableReference,
				MorphEmote = expected.MorphEmote,
				MorphTimeSeconds = expected.MorphTimeSeconds,
				DestroyedItemStableReference = expected.DestroyedItemStableReference
			};
		}
		return ItemSeederManifestCatalogue.Fingerprint(normalized)
			.Equals(ItemSeederManifestCatalogue.Fingerprint(expected), StringComparison.OrdinalIgnoreCase);
	}

	private string? ResolveItemStableReference(long? logicalId)
	{
		if (logicalId is null)
		{
			return null;
		}

		return _itemStableReferencesById.GetValueOrDefault(logicalId.Value);
	}

	private LiquidManifestDefinition BuildLiveLiquidManifestDefinition(Liquid liquid)
	{
		var pathsByTagId = _tagsByFullPath
			.GroupBy(x => x.Value.Id)
			.ToDictionary(x => x.Key, x => x.First().Key);
		var tagIds = _context!.LiquidsTags.Local
			.Where(x => x.LiquidId == liquid.Id)
			.Select(x => x.TagId)
			.Concat(_context.LiquidsTags.Where(x => x.LiquidId == liquid.Id).Select(x => x.TagId))
			.Distinct()
			.ToArray();
		return new LiquidManifestDefinition(
			liquid.Name,
			liquid.Description,
			liquid.LongDescription,
			liquid.TasteText,
			liquid.SmellText,
			liquid.TasteIntensity,
			liquid.SmellIntensity,
			liquid.AlcoholLitresPerLitre,
			liquid.WaterLitresPerLitre,
			liquid.FoodSatiatedHoursPerLitre,
			liquid.DrinkSatiatedHoursPerLitre,
			liquid.Viscosity,
			liquid.Density,
			liquid.Organic,
			liquid.ThermalConductivity,
			liquid.ElectricalConductivity,
			liquid.SpecificHeatCapacity,
			liquid.DisplayColour,
			liquid.SolventVolumeRatio,
			liquid.InjectionConsequence,
			liquid.ResidueVolumePercentage,
			liquid.RelativeEnthalpy,
			liquid.LeaveResidueInRooms,
			liquid.SurfaceReactionInfo,
			tagIds.Where(pathsByTagId.ContainsKey)
				.Select(x => pathsByTagId[x])
				.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
				.ToArray());
	}

	private VehicleGraphManifestDefinition BuildLiveVehicleGraphManifestDefinition(VehicleProto vehicle)
	{
		if (_context!.Entry(vehicle).State != Microsoft.EntityFrameworkCore.EntityState.Added)
		{
			_context.Entry(vehicle).Collection(x => x.Compartments).Load();
			_context.Entry(vehicle).Collection(x => x.CompartmentLinks).Load();
			_context.Entry(vehicle).Collection(x => x.OccupantSlots).Load();
			_context.Entry(vehicle).Collection(x => x.ControlStations).Load();
			_context.Entry(vehicle).Collection(x => x.MovementProfiles).Load();
			_context.Entry(vehicle).Collection(x => x.AccessPoints).Load();
			_context.Entry(vehicle).Collection(x => x.CargoSpaces).Load();
			_context.Entry(vehicle).Collection(x => x.InstallationPoints).Load();
			_context.Entry(vehicle).Collection(x => x.TowPoints).Load();
			_context.Entry(vehicle).Collection(x => x.DamageZones).Load();
			foreach (var movementProfile in vehicle.MovementProfiles)
			{
				_context.Entry(movementProfile).Collection(x => x.PropulsionProfiles).Load();
			}
			foreach (var damageZone in vehicle.DamageZones)
			{
				_context.Entry(damageZone).Collection(x => x.Effects).Load();
			}
		}

		var children = new SortedDictionary<string, IReadOnlyCollection<string>>(StringComparer.Ordinal)
		{
			[nameof(vehicle.Compartments)] = ScalarSignatures(vehicle.Compartments),
			[nameof(vehicle.CompartmentLinks)] = ScalarSignatures(vehicle.CompartmentLinks),
			[nameof(vehicle.OccupantSlots)] = ScalarSignatures(vehicle.OccupantSlots),
			[nameof(vehicle.ControlStations)] = ScalarSignatures(vehicle.ControlStations),
			[nameof(vehicle.MovementProfiles)] = ScalarSignatures(vehicle.MovementProfiles),
			["PropulsionProfiles"] = ScalarSignatures(vehicle.MovementProfiles.SelectMany(x => x.PropulsionProfiles)),
			[nameof(vehicle.AccessPoints)] = ScalarSignatures(vehicle.AccessPoints),
			[nameof(vehicle.CargoSpaces)] = ScalarSignatures(vehicle.CargoSpaces),
			[nameof(vehicle.InstallationPoints)] = ScalarSignatures(vehicle.InstallationPoints),
			[nameof(vehicle.TowPoints)] = ScalarSignatures(vehicle.TowPoints),
			[nameof(vehicle.DamageZones)] = ScalarSignatures(vehicle.DamageZones),
			["DamageZoneEffects"] = ScalarSignatures(vehicle.DamageZones.SelectMany(x => x.Effects))
		};
		return new VehicleGraphManifestDefinition(ScalarSignature(vehicle), children);
	}

	private static IReadOnlyCollection<string> ScalarSignatures<T>(IEnumerable<T> values)
	{
		return values
			.Where(x => x is not null)
			.Select(x => ScalarSignature(x!))
			.OrderBy(x => x, StringComparer.Ordinal)
			.ToArray();
	}

	private static string ScalarSignature(object value)
	{
		return string.Join("|", value.GetType()
			.GetProperties(BindingFlags.Public | BindingFlags.Instance)
			.Where(x => x.CanRead && IsScalarManifestType(x.PropertyType))
			.OrderBy(x => x.Name, StringComparer.Ordinal)
			.Select(x => $"{x.Name}={Convert.ToString(x.GetValue(value), System.Globalization.CultureInfo.InvariantCulture)}"));
	}

	private static bool IsScalarManifestType(Type type)
	{
		var underlying = Nullable.GetUnderlyingType(type) ?? type;
		return underlying.IsPrimitive || underlying.IsEnum || underlying == typeof(string) ||
		       underlying == typeof(decimal) || underlying == typeof(DateTime) || underlying == typeof(Guid);
	}

	private void IncrementManifestResult(string module, Func<ItemSeederReconciliationResult, ItemSeederReconciliationResult> update)
	{
		_manifestResults.TryGetValue(module, out var current);
		_manifestResults[module] = update(current ?? new ItemSeederReconciliationResult(module));
	}

	private ManifestAggregateDisposition InspectManifestAggregate(
		ItemSeederManifestEntry entry,
		long logicalId,
		object liveDefinition)
	{
		if (_manifestCaptureOnly)
		{
			return ManifestAggregateDisposition.Unchanged;
		}

		var liveFingerprint = ItemSeederManifestCatalogue.Fingerprint(liveDefinition);
		var managedRecord = FindManagedRecord(entry.EntityType, entry.StableKey);
		if (managedRecord is null)
		{
			if (!liveFingerprint.Equals(entry.Fingerprint, StringComparison.OrdinalIgnoreCase))
			{
				IncrementManifestResult(entry.Module, x => x with { Blocked = x.Blocked + 1 });
				throw new InvalidOperationException(
					$"Unmanaged {entry.EntityType} conflict for '{entry.StableKey}'. The record does not have the complete stock signature and will not be claimed or overwritten.");
			}

			RecordAppliedManifestEntry(entry, logicalId, null, liveFingerprint);
			IncrementManifestResult(entry.Module, x => x with { Unchanged = x.Unchanged + 1 });
			return ManifestAggregateDisposition.Unchanged;
		}

		if (managedRecord.LogicalId is not null && managedRecord.LogicalId != logicalId)
		{
			IncrementManifestResult(entry.Module, x => x with { Blocked = x.Blocked + 1 });
			throw new InvalidOperationException(
				$"ItemSeeder ownership conflict for {entry.EntityType}:{entry.StableKey}: provenance names ID {managedRecord.LogicalId:N0}, but the stable identity resolves to {logicalId:N0}.");
		}

		if (!liveFingerprint.Equals(managedRecord.AppliedFingerprint, StringComparison.OrdinalIgnoreCase))
		{
			MarkManifestAggregateCustomized(entry.EntityType, entry.StableKey);
			IncrementManifestResult(entry.Module, x => x with { Customized = x.Customized + 1 });
			return ManifestAggregateDisposition.Customized;
		}

		if (liveFingerprint.Equals(entry.Fingerprint, StringComparison.OrdinalIgnoreCase))
		{
			RecordAppliedManifestEntry(entry, logicalId, null, liveFingerprint);
			IncrementManifestResult(entry.Module, x => x with { Unchanged = x.Unchanged + 1 });
			return ManifestAggregateDisposition.Unchanged;
		}

		return ManifestAggregateDisposition.Update;
	}

	private void CompleteManifestAggregate(
		ItemSeederManifestEntry entry,
		long? logicalId,
		object liveDefinition,
		ManifestAggregateDisposition disposition)
	{
		if (_manifestCaptureOnly)
		{
			return;
		}

		var fingerprint = ItemSeederManifestCatalogue.Fingerprint(liveDefinition);
		RecordAppliedManifestEntry(entry, logicalId, null, fingerprint);
		IncrementManifestResult(entry.Module, x => disposition switch
		{
			ManifestAggregateDisposition.Insert => x with { Inserted = x.Inserted + 1 },
			ManifestAggregateDisposition.Update => x with { Updated = x.Updated + 1 },
			_ => x
		});
	}

	private string BuildManifestResultSummary()
	{
		return string.Join(Environment.NewLine, _manifestResults.Values
			.OrderBy(x => x.Module, StringComparer.OrdinalIgnoreCase)
			.Select(x =>
				$"{x.Module}: inserted {x.Inserted:N0}, updated {x.Updated:N0}, linked {x.Linked:N0}, unchanged {x.Unchanged:N0}, customized {x.Customized:N0}, retired {x.Retired:N0}, blocked {x.Blocked:N0}"));
	}

	private void RetireMissingManagedRecords()
	{
		if (_manifestCaptureOnly)
		{
			return;
		}

		var activeIdentities = _manifestEntries.Values
			.Select(x => ManagedRecordIdentity(x.EntityType, x.StableKey))
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		foreach (var record in _context!.SeederManagedRecords
		         .Where(x => x.Seeder == Name && !x.Retired)
		         .ToArray())
		{
			if (activeIdentities.Contains(ManagedRecordIdentity(record.EntityType, record.StableKey)))
			{
				continue;
			}

			record.Retired = true;
			record.AppliedAt = DateTime.UtcNow;
			IncrementManifestResult(record.Module, x => x with { Retired = x.Retired + 1 });
		}
	}

	private static string ManagedRecordIdentity(string entityType, string stableKey)
	{
		return $"{entityType}\u001f{stableKey}";
	}

	private void MarkManifestAggregateCustomized(string entityType, string stableKey)
	{
		_customizedManifestAggregates.Add(ManagedRecordIdentity(entityType, stableKey));
	}

	private bool IsManifestAggregateCustomized(string entityType, string stableKey)
	{
		return _customizedManifestAggregates.Contains(ManagedRecordIdentity(entityType, stableKey));
	}

	private SeederManagedRecord? FindManagedRecord(string entityType, string stableKey)
	{
		if (_manifestCaptureOnly)
		{
			return null;
		}

		var identity = ManagedRecordIdentity(entityType, stableKey);
		if (_managedRecordsByIdentity.TryGetValue(identity, out var cachedRecord))
		{
			var entry = _context!.Entry(cachedRecord);
			if (entry.State == EntityState.Detached)
			{
				_context.Attach(cachedRecord);
				entry = _context.Entry(cachedRecord);
			}

			if (entry.State != EntityState.Deleted)
			{
				return cachedRecord;
			}

			_managedRecordsByIdentity.Remove(identity);
		}

		// InitialiseDependencies indexed every existing record for this seeder. A miss is
		// therefore a genuinely new aggregate; querying it again once per new item or
		// craft turns a fresh installation into thousands of database round-trips.
		return null;
	}

	private void RecordAppliedManifestEntry(
		ItemSeederManifestEntry entry,
		long? logicalId,
		int? revisionNumber,
		string? appliedFingerprint = null)
	{
		if (_manifestCaptureOnly)
		{
			return;
		}

		var record = FindManagedRecord(entry.EntityType, entry.StableKey);
		if (record is null)
		{
			record = new SeederManagedRecord
			{
				Seeder = Name,
				EntityType = entry.EntityType,
				StableKey = entry.StableKey
			};
			_context!.SeederManagedRecords.Add(record);
			_managedRecordsByIdentity[ManagedRecordIdentity(entry.EntityType, entry.StableKey)] = record;
		}

		record.Module = entry.Module;
		record.LogicalId = logicalId;
		record.RevisionNumber = revisionNumber;
		record.AppliedFingerprint = appliedFingerprint ?? entry.Fingerprint;
		record.ManifestVersion = ItemSeederManifestCatalogue.ManifestVersion;
		record.AppliedAt = DateTime.UtcNow;
		record.Retired = false;
	}

	private sealed class ManifestModuleScope(Action onDispose) : IDisposable
	{
		private Action? _onDispose = onDispose;

		public void Dispose()
		{
			_onDispose?.Invoke();
			_onDispose = null;
		}
	}
}
