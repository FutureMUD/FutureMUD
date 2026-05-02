#nullable enable

using System.Globalization;
using System.Text.RegularExpressions;
using MudSharp.GameItems;

namespace RPI_Engine_Worldfile_Converter;

public enum ConversionStatus
{
	FunctionalImport,
	PropImport,
	DeferredBehaviorPropImport,
}

public sealed record RpiConversionWarning(string Code, string Message);

public sealed record FutureMudTraitReference(string TraitName, int Modifier, RPISkill SourceSkill);

public sealed record FutureMudLiquidReference(string? LiquidName, int RawLiquidValue);

public sealed record FutureMudBoardClanRestriction(string ClanAlias, string RankName);

public sealed record FutureMudBoardDefinition(
	string BoardName,
	string ComponentName,
	string LegacyBoardKey,
	IReadOnlyList<FutureMudBoardClanRestriction> ClanRestrictions);

public sealed record ConvertedItemDefinition
{
	public required int Vnum { get; init; }
	public required string SourceFile { get; init; }
	public required int Zone { get; init; }
	public required string SourceKey { get; init; }
	public required RPIItemType SourceItemType { get; init; }
	public required ConversionStatus Status { get; init; }
	public required string BaseName { get; init; }
	public required string Keywords { get; init; }
	public required string ShortDescription { get; init; }
	public required string LongDescription { get; init; }
	public required string FullDescription { get; init; }
	public required string MaterialName { get; init; }
	public required int BaseItemQuality { get; init; }
	public required int Size { get; init; }
	public required double WeightGrams { get; init; }
	public required decimal CostInBaseCurrency { get; init; }
	public required IReadOnlyList<int> RawOvals { get; init; }
	public IReadOnlyList<string> ComponentNames { get; init; } = Array.Empty<string>();
	public IReadOnlyList<string> TagNames { get; init; } = Array.Empty<string>();
	public IReadOnlyList<FutureMudTraitReference> TraitReferences { get; init; } = Array.Empty<FutureMudTraitReference>();
	public FutureMudLiquidReference? LiquidReference { get; init; }
	public FutureMudBoardDefinition? BoardDefinition { get; init; }
	public string? DescKeys { get; init; }
	public string? InkColour { get; init; }
	public string? CustomColour { get; init; }
	public bool PermitPlayerSkins { get; init; } = true;
	public IReadOnlyList<RpiConversionWarning> Warnings { get; init; } = Array.Empty<RpiConversionWarning>();
}

public sealed class FutureMUDItemTransformer
{
	private static readonly Regex NonKeywordRegex = new(@"[^a-z0-9'-]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
	private static readonly Regex BoundedAnsiColourRegex = new(@"^\s*(?<colour>#[1-9a-oA-O])(?<text>[\s\S]*?)#0\s*$", RegexOptions.Compiled);
	private static readonly Regex LegacyVariableRegex = new(@"\$[A-Za-z][A-Za-z0-9_]*", RegexOptions.Compiled);
	private static readonly Regex NonComponentNameRegex = new(@"[^a-z0-9_]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
	private static readonly IReadOnlyDictionary<string, string> AnsiColourCodeNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
	{
		["#1"] = "red",
		["#2"] = "green",
		["#3"] = "yellow",
		["#4"] = "blue",
		["#5"] = "magenta",
		["#6"] = "cyan",
		["#7"] = "bold black",
		["#8"] = "orange",
		["#9"] = "bold red",
		["#a"] = "bold green",
		["#b"] = "bold yellow",
		["#c"] = "bold blue",
		["#d"] = "bold magenta",
		["#e"] = "bold cyan",
		["#f"] = "bold white",
		["#g"] = "bold orange",
		["#h"] = "bold pink",
		["#i"] = "pink",
		["#j"] = "function yellow",
		["#k"] = "variable green",
		["#l"] = "keyword blue",
		["#m"] = "variable cyan",
		["#n"] = "text red",
		["#o"] = "keyword pink"
	};

	private enum LegacyVariableProfile
	{
		BasicColour,
		FineColour,
		DrabColour,
		Gem,
		FineGem,
		CommonStone
	}

	private sealed record ConvertedDescriptions(
		string ShortDescription,
		string LongDescription,
		string FullDescription,
		string? CustomColour);

	private static readonly string[] DeferredBehaviorTypes =
	[
		nameof(RPIItemType.Ticket),
		nameof(RPIItemType.MerchTicket),
		nameof(RPIItemType.RoomRental),
		nameof(RPIItemType.NPC_Object),
		nameof(RPIItemType.Dwelling),
		nameof(RPIItemType.Skull),
	];

	private readonly FutureMudBaselineCatalog? _catalog;

	public FutureMUDItemTransformer(FutureMudBaselineCatalog? catalog = null)
	{
		_catalog = catalog;
	}

	public IReadOnlyList<ConvertedItemDefinition> Convert(IEnumerable<RpiItemRecord> items)
	{
		return items
			.OrderBy(x => x.Zone)
			.ThenBy(x => x.Vnum)
			.Select(Convert)
			.ToList();
	}

	public ConvertedItemDefinition Convert(RpiItemRecord item)
	{
		var warnings = new List<RpiConversionWarning>();
		var components = new List<string>();
		var tags = new List<string>();
		var traits = MapTraitReferences(item, warnings);
		FutureMudLiquidReference? liquidReference = null;
		FutureMudBoardDefinition? boardDefinition = null;
		var status = ConversionStatus.FunctionalImport;

		if (ShouldAddHoldable(item))
		{
			AddUnique(components, ChooseComponent("Holdable"));
		}

		switch (item.ItemType)
		{
			case RPIItemType.Worn:
				MapWorn(item, components, tags, warnings);
				break;
			case RPIItemType.Armor:
				MapArmour(item, components, tags, warnings);
				break;
			case RPIItemType.Weapon:
				MapWeapon(item, components, tags, warnings);
				break;
			case RPIItemType.Shield:
				MapShield(item, components, tags, warnings);
				break;
			case RPIItemType.Container:
				MapContainer(item, components, tags, warnings);
				break;
			case RPIItemType.Light:
				MapLight(item, components, tags, warnings);
				liquidReference = MapLiquidReference(item);
				break;
			case RPIItemType.Liquid_container:
				MapLiquidContainer(item, components, tags, warnings);
				liquidReference = MapLiquidReference(item);
				break;
			case RPIItemType.Fountain:
				MapFountain(item, components, tags, warnings);
				liquidReference = MapLiquidReference(item);
				break;
			case RPIItemType.Food:
				status = MapFood(item, components, tags, warnings);
				break;
			case RPIItemType.Key:
				MapKey(item, components, tags);
				break;
			case RPIItemType.Missile:
			case RPIItemType.Bullet:
				MapAmmunition(item, components, tags);
				break;
			case RPIItemType.Quiver:
				MapQuiver(item, components, tags);
				break;
			case RPIItemType.Sheath:
				MapSheath(item, components, tags);
				break;
			case RPIItemType.Keyring:
				MapKeyring(item, components, tags);
				break;
			case RPIItemType.Repair:
				MapRepairKit(item, components, tags);
				break;
			case RPIItemType.Lockpick:
				MapLockpick(item, components, tags);
				break;
			case RPIItemType.Parchment:
				MapParchment(item, components, tags);
				break;
			case RPIItemType.Book:
				MapBook(item, components, tags);
				break;
			case RPIItemType.Writing_inst:
				status = MapWritingImplement(item, components, tags, warnings);
				break;
			case RPIItemType.Ink:
				status = MapInk(item, components, tags, warnings);
				break;
			case RPIItemType.Board:
				boardDefinition = MapBoard(item, components, tags, warnings);
				break;
			case RPIItemType.Ticket:
			case RPIItemType.MerchTicket:
			case RPIItemType.RoomRental:
			case RPIItemType.NPC_Object:
			case RPIItemType.Dwelling:
			case RPIItemType.Skull:
				status = ConversionStatus.DeferredBehaviorPropImport;
				MapGenericProp(item, components, tags, warnings, ConversionStatus.DeferredBehaviorPropImport);
				warnings.Add(new RpiConversionWarning(
					"deferred-behaviour",
					$"{item.ItemType} needs gameplay behaviour that is being preserved as a tagged prop for pass one."));
				break;
			default:
				status = ConversionStatus.PropImport;
				MapGenericProp(item, components, tags, warnings);
				warnings.Add(new RpiConversionWarning(
					"prop-fallback",
					$"{item.ItemType} is being imported as a tagged prop in pass one."));
				break;
		}

		var convertedDescriptions = ConvertDescriptions(item, components, warnings);
		var materialName = ResolveMaterialName(item, warnings);
		var baseName = InferBaseName(item);
		if (string.IsNullOrWhiteSpace(baseName))
		{
			baseName = "item";
			warnings.Add(new RpiConversionWarning("missing-base-name", "Could not infer a clean base name; using 'item'."));
		}

		if (components.Count == 0)
		{
			status = status == ConversionStatus.DeferredBehaviorPropImport
				? ConversionStatus.DeferredBehaviorPropImport
				: ConversionStatus.PropImport;
			if (ShouldAddHoldable(item))
			{
				AddUnique(components, ChooseComponent("Holdable"));
			}

			AddUnique(components, ChooseDestroyableComponent(item, status));
			warnings.Add(new RpiConversionWarning("missing-components", "No functional component mapping was found; imported as a simple prop."));
		}

		return new ConvertedItemDefinition
		{
			Vnum = item.Vnum,
			SourceFile = item.SourceFile,
			Zone = item.Zone,
			SourceKey = item.SourceKey,
			SourceItemType = item.ItemType,
			Status = status,
			BaseName = baseName,
			Keywords = BuildKeywords(item, baseName),
			ShortDescription = convertedDescriptions.ShortDescription,
			LongDescription = convertedDescriptions.LongDescription,
			FullDescription = convertedDescriptions.FullDescription,
			MaterialName = materialName,
			BaseItemQuality = InferQuality(item),
			Size = InferSize(item),
			WeightGrams = ConvertWeightToGrams(item.Weight),
			CostInBaseCurrency = decimal.Round((decimal)item.NumericTail.Farthings, 2, MidpointRounding.AwayFromZero),
			RawOvals = item.RawOvals.AsList,
			ComponentNames = components,
			TagNames = tags,
			TraitReferences = traits,
			LiquidReference = liquidReference,
			BoardDefinition = boardDefinition,
			DescKeys = item.DescKeys,
			InkColour = item.InkColour,
			CustomColour = convertedDescriptions.CustomColour,
			PermitPlayerSkins = status != ConversionStatus.DeferredBehaviorPropImport,
			Warnings = warnings
				.Concat(item.ParseWarnings.Select(x => new RpiConversionWarning(x.Code, x.Message)))
				.ToList()
		};
	}

	private ConvertedDescriptions ConvertDescriptions(
		RpiItemRecord item,
		ICollection<string> components,
		ICollection<RpiConversionWarning> warnings)
	{
		var shortDescription = item.ShortDescription;
		var longDescription = item.LongDescription;
		var fullDescription = item.FullDescription;
		string? customColour = null;

		if (TryExtractBoundedAnsiColour(shortDescription, out var cleanedShort, out var shortColour))
		{
			shortDescription = cleanedShort;
			customColour = shortColour;
		}

		if (TryExtractBoundedAnsiColour(longDescription, out var cleanedLong, out var longColour))
		{
			if (customColour is null || customColour.Equals(longColour, StringComparison.OrdinalIgnoreCase))
			{
				longDescription = cleanedLong;
				customColour ??= longColour;
			}
			else
			{
				warnings.Add(new RpiConversionWarning(
					"conflicting-ansi-colour",
					$"Short and long descriptions use different bounded ANSI colours ({customColour} vs {longColour}); the long description colour tags were left inline."));
			}
		}

		var variableConversion = ConvertLegacyVariables(item, shortDescription, longDescription, fullDescription, warnings);
		shortDescription = variableConversion.ShortDescription;
		longDescription = variableConversion.LongDescription;
		fullDescription = variableConversion.FullDescription;
		if (!string.IsNullOrWhiteSpace(variableConversion.ComponentName))
		{
			AddUnique(components, ChooseComponent(variableConversion.ComponentName));
		}

		return new ConvertedDescriptions(shortDescription, longDescription, fullDescription, customColour);
	}

	private static bool TryExtractBoundedAnsiColour(string text, out string cleanedText, out string colourName)
	{
		cleanedText = text;
		colourName = string.Empty;
		if (string.IsNullOrWhiteSpace(text))
		{
			return false;
		}

		var match = BoundedAnsiColourRegex.Match(text);
		if (!match.Success)
		{
			return false;
		}

		var innerText = match.Groups["text"].Value;
		if (innerText.Contains('#', StringComparison.Ordinal))
		{
			return false;
		}

		var code = match.Groups["colour"].Value.ToLowerInvariant();
		if (!AnsiColourCodeNames.TryGetValue(code, out colourName!))
		{
			return false;
		}

		cleanedText = innerText.Trim();
		return true;
	}

	private sealed record LegacyVariableConversion(
		string ShortDescription,
		string LongDescription,
		string FullDescription,
		string? ComponentName);

	private LegacyVariableConversion ConvertLegacyVariables(
		RpiItemRecord item,
		string shortDescription,
		string longDescription,
		string fullDescription,
		ICollection<RpiConversionWarning> warnings)
	{
		var combinedText = $"{item.RawName} {shortDescription} {longDescription} {fullDescription}";
		var tokens = LegacyVariableRegex
			.Matches(combinedText)
			.Select(x => x.Value.ToLowerInvariant())
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToList();

		if (tokens.Count == 0)
		{
			return new LegacyVariableConversion(shortDescription, longDescription, fullDescription, null);
		}

		var recognisedProfiles = new Dictionary<LegacyVariableProfile, int>();
		foreach (var token in tokens)
		{
			var profile = LegacyVariableProfileForToken(token);
			if (profile is null)
			{
				warnings.Add(new RpiConversionWarning(
					"unmapped-variable-token",
					$"The source item uses legacy variable {token}, which does not currently have a seeded FutureMUD variable component mapping."));
				continue;
			}

			recognisedProfiles[profile.Value] = recognisedProfiles.GetValueOrDefault(profile.Value) + 1;
		}

		if (recognisedProfiles.Count == 0)
		{
			return new LegacyVariableConversion(shortDescription, longDescription, fullDescription, null);
		}

		var componentName = recognisedProfiles.Count == 1
			? ComponentNameForProfile(recognisedProfiles.Keys.First())
			: "Variable_RpiMixedVariables";

		return new LegacyVariableConversion(
			NormalizeLegacyVariableTokens(shortDescription),
			NormalizeLegacyVariableTokens(longDescription),
			NormalizeLegacyVariableTokens(fullDescription),
			componentName);
	}

	private static LegacyVariableProfile? LegacyVariableProfileForToken(string token)
	{
		return token switch
		{
			"$color" or "$colour" => LegacyVariableProfile.BasicColour,
			"$finecolor" or "$finecolour" or "$finecolored" => LegacyVariableProfile.FineColour,
			"$drabcolor" or "$drabcolour" or "$drobcolor" => LegacyVariableProfile.DrabColour,
			"$gem" or "$gemcolor" or "$gemcolour" => LegacyVariableProfile.Gem,
			"$finegem" or "$finegemcolor" or "$finegemcolour" => LegacyVariableProfile.FineGem,
			"$stone" or "$commonstone" => LegacyVariableProfile.CommonStone,
			_ => null
		};
	}

	private static string ComponentNameForProfile(LegacyVariableProfile profile)
	{
		return profile switch
		{
			LegacyVariableProfile.BasicColour => "Variable_BasicColour",
			LegacyVariableProfile.FineColour => "Variable_FineColour",
			LegacyVariableProfile.DrabColour => "Variable_DrabColour",
			LegacyVariableProfile.Gem => "Variable_Gem",
			LegacyVariableProfile.FineGem => "Variable_FineGem",
			LegacyVariableProfile.CommonStone => "Variable_CommonStone",
			_ => "Variable_Colour"
		};
	}

	private static string NormalizeLegacyVariableTokens(string text)
	{
		return LegacyVariableRegex.Replace(text, match =>
		{
			var token = match.Value.ToLowerInvariant();
			return token switch
			{
				"$colour" => "$color",
				"$finecolour" or "$finecolored" => "$finecolor",
				"$drabcolour" or "$drobcolor" => "$drabcolor",
				"$gemcolour" => "$gemcolor",
				"$finegem" or "$finegemcolour" => "$finegemcolor",
				_ => LegacyVariableProfileForToken(token) is null ? match.Value : token
			};
		});
	}

	private void MapWorn(RpiItemRecord item, ICollection<string> components, ICollection<string> tags, ICollection<RpiConversionWarning> warnings)
	{
		var wearComponent = ResolveWearComponent(item, warnings, false);
		if (wearComponent is not null)
		{
			AddUnique(components, wearComponent);
		}

		AddUnique(components, ChooseClothingOrArmourComponent(item, false));
		AddUnique(components, ChooseDestroyableComponent(item, ConversionStatus.FunctionalImport));
		AddWearTags(item, tags);

		if (item.WearBits.HasFlag(RPIWearBits.Blindfold))
		{
			AddUnique(components, ChooseComponent("Blindfold"));
		}

		if (item.WearBits.HasFlag(RPIWearBits.Waist))
		{
			AddUnique(components, ChooseComponent("Belt_2"));
		}
	}

	private void MapArmour(RpiItemRecord item, ICollection<string> components, ICollection<string> tags, ICollection<RpiConversionWarning> warnings)
	{
		var wearComponent = ResolveWearComponent(item, warnings, true);
		if (wearComponent is not null)
		{
			AddUnique(components, wearComponent);
		}

		var armourComponent = item.ArmourData?.ArmourType switch
		{
			0 => "Armour_HeavyClothing",
			1 => "Armour_UltraHeavyClothing",
			2 => "Armour_BoiledLeather",
			3 => "Armour_MetalScale",
			4 => "Armour_Chainmail",
			5 => "Armour_Platemail",
			_ => ChooseClothingOrArmourComponent(item, true),
		};

		AddUnique(components, ChooseComponent(armourComponent));
		AddUnique(components, ChooseDestroyableComponent(item, ConversionStatus.FunctionalImport));
		AddWearTags(item, tags);
		AddUnique(tags, ChooseTag("Primitive Armour", "Leather Armour", "Mail Armour", "Plate Armour"));
	}

	private void MapShield(RpiItemRecord item, ICollection<string> components, ICollection<string> tags, ICollection<RpiConversionWarning> warnings)
	{
		AddUnique(components, ChooseComponent("Melee_Shield"));

		if (ContainsAny(item, "tower"))
		{
			AddUnique(components, ChooseComponent("Shield_Tower"));
		}
		else if (ContainsAny(item, "buckler"))
		{
			AddUnique(components, ChooseComponent("Shield_Buckler"));
		}
		else
		{
			AddUnique(components, ChooseComponent("Shield_Heater"));
		}

		AddUnique(components, ChooseDestroyableComponent(item, ConversionStatus.FunctionalImport));
		AddUnique(tags, ChooseTag("Shields"));

		if (item.WearBits.HasFlag(RPIWearBits.Wshield))
		{
			AddUnique(components, ResolveWearComponent(item, warnings, true) ?? ChooseComponent("Wear_Shoulder"));
		}
	}

	private void MapWeapon(RpiItemRecord item, ICollection<string> components, ICollection<string> tags, ICollection<RpiConversionWarning> warnings)
	{
		var data = item.WeaponData;
		var componentName = data?.UseSkill switch
		{
			RPISkill.Shortbow => "Shortbow",
			RPISkill.Longbow => "Longbow",
			RPISkill.Crossbow => "Crossbow",
			RPISkill.Sling => ChooseComponentOrNull("Sling"),
			_ => InferMeleeWeaponComponent(item),
		};

		if (string.IsNullOrWhiteSpace(componentName))
		{
			MapGenericProp(item, components, tags, warnings);
			warnings.Add(new RpiConversionWarning("weapon-fallback", "Could not resolve a weapon component; imported as a prop."));
			return;
		}

		AddUnique(components, ChooseComponent(componentName));
		AddUnique(components, ChooseDestroyableComponent(item, ConversionStatus.FunctionalImport));
		AddWeaponTags(item, tags);

		if (item.ExtraBits.HasFlag(RPIExtraBits.Thrown) || data?.UseSkill == RPISkill.Thrown || ContainsAny(item, "throwing", "javelin"))
		{
			var throwingComponent = InferThrowingWeaponComponent(item);
			if (!string.IsNullOrWhiteSpace(throwingComponent))
			{
				AddUnique(components, ChooseComponent(throwingComponent));
			}
		}
	}

	private void MapContainer(RpiItemRecord item, ICollection<string> components, ICollection<string> tags, ICollection<RpiConversionWarning> warnings)
	{
		var data = item.ContainerData;
		var baseContainer = InferContainerComponent(item);
		AddUnique(components, ChooseComponent(baseContainer));
		AddUnique(components, ChooseDestroyableComponent(item, ConversionStatus.FunctionalImport));
		AddContainerTags(item, tags);

		if (data is not null && (data.KeyVnum > 0 || data.Flags != 0))
		{
			var lockComponent = data.Capacity switch
			{
				<= 2000 => "LockingContainer_Lockbox",
				<= 25000 => "LockingContainer_Footlocker",
				_ => "LockingContainer_SafeChest",
			};
			AddUnique(components, ChooseComponent(lockComponent));
		}
	}

	private void MapLight(RpiItemRecord item, ICollection<string> components, ICollection<string> tags, ICollection<RpiConversionWarning> warnings)
	{
		if (ContainsAny(item, "lantern", "lamp") || (item.LightData?.LiquidValue ?? 0) > 0)
		{
			AddUnique(components, ChooseComponent("Lantern"));
			AddUnique(tags, ChooseTag("Lamps"));
		}
		else
		{
			AddUnique(components, ChooseComponent("Torch_1Hour"));
			AddUnique(tags, ChooseTag("Torches"));
		}

		AddUnique(components, ChooseDestroyableComponent(item, ConversionStatus.FunctionalImport));
	}

	private void MapLiquidContainer(RpiItemRecord item, ICollection<string> components, ICollection<string> tags, ICollection<RpiConversionWarning> warnings)
	{
		AddUnique(components, ChooseComponent(InferLiquidContainerComponent(item)));
		AddUnique(components, ChooseDestroyableComponent(item, ConversionStatus.FunctionalImport));
		AddUnique(tags, ChooseTag("Watertight Container"));
	}

	private void MapFountain(RpiItemRecord item, ICollection<string> components, ICollection<string> tags, ICollection<RpiConversionWarning> warnings)
	{
		if (item.DrinkContainerData?.IsInfiniteSource == true)
		{
			var infiniteWaterComponent = ChooseComponentOrNull("Infinite_WaterSource");
			if (!string.IsNullOrWhiteSpace(infiniteWaterComponent))
			{
				AddUnique(components, infiniteWaterComponent);
			}
			else
			{
				AddUnique(components, ChooseComponent("LContainer_Puncheon"));
				warnings.Add(new RpiConversionWarning("missing-infinite-source", "Baseline did not expose Infinite_WaterSource; using a large liquid container instead."));
			}
		}
		else
		{
			AddUnique(components, ChooseComponent("LContainer_Puncheon"));
		}

		AddUnique(tags, ChooseTag("Watertight Container"));
	}

	private ConversionStatus MapFood(RpiItemRecord item, ICollection<string> components, ICollection<string> tags, ICollection<RpiConversionWarning> warnings)
	{
		var foodComponent = _catalog?.FindFirstComponentByType("Food");
		if (string.IsNullOrWhiteSpace(foodComponent))
		{
			MapGenericProp(item, components, tags, warnings);
			warnings.Add(new RpiConversionWarning("missing-food-component", "No seeded Food component was available; imported as a prop."));
			return ConversionStatus.PropImport;
		}

		AddUnique(components, foodComponent);
		AddUnique(components, ChooseDestroyableComponent(item, ConversionStatus.FunctionalImport));
		AddUnique(tags, ChooseTag("Food"));
		return ConversionStatus.FunctionalImport;
	}

	private void MapKey(RpiItemRecord item, ICollection<string> components, ICollection<string> tags)
	{
		AddUnique(components, ChooseComponent("Warded_Key"));
		AddUnique(components, ChooseDestroyableComponent(item, ConversionStatus.FunctionalImport));
		AddUnique(tags, ChooseTag("Luxury Wares"));
	}

	private void MapAmmunition(RpiItemRecord item, ICollection<string> components, ICollection<string> tags)
	{
		var ammoComponent = item.ItemType == RPIItemType.Bullet || ContainsAny(item, "sling", "stone", "bullet")
			? "Ammo_SlingBullet"
			: ContainsAny(item, "bolt", "quarrel", "crossbow")
				? "Ammo_BroadheadBolt"
				: "Ammo_BroadheadArrow";

		AddUnique(components, ChooseComponent(ammoComponent));
		AddUnique(components, ChooseDestroyableComponent(item, ConversionStatus.FunctionalImport));
		AddUnique(tags, ChooseTag("Ammunition"));
	}

	private void MapQuiver(RpiItemRecord item, ICollection<string> components, ICollection<string> tags)
	{
		AddUnique(components, ChooseComponent("Container_Quiver"));
		AddUnique(components, ChooseDestroyableComponent(item, ConversionStatus.FunctionalImport));
		AddUnique(tags, ChooseTag("Open Container", "Military Goods"));
	}

	private void MapSheath(RpiItemRecord item, ICollection<string> components, ICollection<string> tags)
	{
		var capacity = item.ContainerData?.Capacity ?? item.RawOvals.Oval0;
		AddUnique(components, ChooseComponent(capacity <= 800 ? "Sheath_Small" : "Sheath_Large"));
		AddUnique(components, ChooseDestroyableComponent(item, ConversionStatus.FunctionalImport));
		AddUnique(tags, ChooseTag("Belts"));
	}

	private void MapKeyring(RpiItemRecord item, ICollection<string> components, ICollection<string> tags)
	{
		var capacity = item.ContainerData?.Capacity ?? item.RawOvals.Oval0;
		AddUnique(components, ChooseComponent(capacity <= 4 ? "Keyring_Small" : "Keyring_Large"));
		AddUnique(components, ChooseDestroyableComponent(item, ConversionStatus.FunctionalImport));
	}

	private void MapRepairKit(RpiItemRecord item, ICollection<string> components, ICollection<string> tags)
	{
		var component = item.RepairKitData?.RepairItemTypeValue switch
		{
			(int)RPIItemType.Armor => "Repair_Metal_Armour",
			(int)RPIItemType.Weapon => "Repair_Metal_Weapon",
			(int)RPIItemType.Worn or (int)RPIItemType.Cloth => "Repair_Cloth",
			_ => ContainsAny(item, "sew", "thread", "cloth", "tailor") ? "Repair_Cloth" :
				ContainsAny(item, "armour", "armor", "mail", "plate", "leather") ? "Repair_Metal_Armour" :
				"Repair_Metal_Weapon",
		};

		component = AdjustRepairQuality(component, item);
		AddUnique(components, ChooseComponent(component));
		AddUnique(components, ChooseDestroyableComponent(item, ConversionStatus.FunctionalImport));
		AddUnique(tags, ChooseTag("Repairing", "Standard Tools"));
	}

	private void MapLockpick(RpiItemRecord item, ICollection<string> components, ICollection<string> tags)
	{
		var component = InferQuality(item) switch
		{
			<= 3 => "Locksmithing_Poor",
			>= 8 => "Locksmithing_Fine",
			_ => "Locksmithing_Standard",
		};

		AddUnique(components, ChooseComponent(component));
		AddUnique(components, ChooseDestroyableComponent(item, ConversionStatus.FunctionalImport));
		AddUnique(tags, ChooseTag("Standard Tools"));
	}

	private void MapParchment(RpiItemRecord item, ICollection<string> components, ICollection<string> tags)
	{
		AddUnique(components, ChooseComponent("Stack_Number"));
		AddUnique(components, ChooseComponent(InferPaperComponent(item)));
		AddUnique(components, ChooseDestroyableComponent(item, ConversionStatus.FunctionalImport));
	}

	private void MapBook(RpiItemRecord item, ICollection<string> components, ICollection<string> tags)
	{
		var pages = item.WritingData?.Pages ?? Math.Max(1, item.RawOvals.Oval0);
		AddUnique(components, ChooseComponent(InferBookComponent(item, pages)));
		AddUnique(components, ChooseDestroyableComponent(item, ConversionStatus.FunctionalImport));
	}

	private ConversionStatus MapWritingImplement(RpiItemRecord item, ICollection<string> components, ICollection<string> tags, ICollection<RpiConversionWarning> warnings)
	{
		var candidate = ContainsAny(item, "pencil", "charcoal") ? ChooseComponentOrNull("Pencil_Black") :
			ContainsAny(item, "blue") ? ChooseComponentOrNull("Biro_Blue") :
			ContainsAny(item, "red") ? ChooseComponentOrNull("Biro_Red") :
			ChooseComponentOrNull("Biro_Black");

		if (string.IsNullOrWhiteSpace(candidate))
		{
			MapGenericProp(item, components, tags, warnings);
			warnings.Add(new RpiConversionWarning("missing-writing-implement", "No seeded writing-implement component was available; imported as a prop."));
			return ConversionStatus.PropImport;
		}

		AddUnique(components, candidate);
		AddUnique(components, ChooseDestroyableComponent(item, ConversionStatus.FunctionalImport));
		return ConversionStatus.FunctionalImport;
	}

	private ConversionStatus MapInk(RpiItemRecord item, ICollection<string> components, ICollection<string> tags, ICollection<RpiConversionWarning> warnings)
	{
		MapGenericProp(item, components, tags, warnings);
		warnings.Add(new RpiConversionWarning(
			"ink-prop-fallback",
			"Standalone ink reservoirs do not have a seeded FutureMUD equivalent in this importer pass; imported as a tagged prop."));
		return ConversionStatus.PropImport;
	}

	private FutureMudBoardDefinition MapBoard(
		RpiItemRecord item,
		ICollection<string> components,
		ICollection<string> tags,
		ICollection<RpiConversionWarning> warnings)
	{
		var legacyBoardKey = InferLegacyBoardKey(item);
		var componentName = BuildBoardComponentName(legacyBoardKey, item.Vnum);
		AddUnique(components, componentName);
		AddUnique(components, ChooseDestroyableComponent(item, ConversionStatus.FunctionalImport));

		var clanRestrictions = item.Clans
			.Where(x => !string.IsNullOrWhiteSpace(x.Name))
			.Select(x => new FutureMudBoardClanRestriction(x.Name.Trim(), x.Rank.Trim()))
			.GroupBy(x => $"{x.ClanAlias}\n{x.RankName}", StringComparer.OrdinalIgnoreCase)
			.Select(x => x.First())
			.ToList();

		if (item.Clans.Count > 0 && clanRestrictions.Count == 0)
		{
			warnings.Add(new RpiConversionWarning(
				"board-clan-access-empty",
				"Legacy board clan restrictions were present on the source item, but none had a clan alias to convert."));
		}

		return new FutureMudBoardDefinition(
			BuildBoardName(legacyBoardKey, item.Vnum),
			componentName,
			legacyBoardKey,
			clanRestrictions);
	}

	private static string InferLegacyBoardKey(RpiItemRecord item)
	{
		var sourceKeyword = item.NameKeywords
			.Select(CleanupKeyword)
			.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x) && !ShouldSkipSourceKeyword(x));

		return string.IsNullOrWhiteSpace(sourceKeyword)
			? $"board-{item.Vnum.ToString(CultureInfo.InvariantCulture)}"
			: sourceKeyword;
	}

	private static string BuildBoardComponentName(string legacyBoardKey, int vnum)
	{
		var componentSuffix = NonComponentNameRegex
			.Replace(legacyBoardKey.ToLowerInvariant(), "_")
			.Trim('_');
		if (string.IsNullOrWhiteSpace(componentSuffix))
		{
			componentSuffix = $"vnum_{vnum.ToString(CultureInfo.InvariantCulture)}";
		}

		if (componentSuffix.Length > 80)
		{
			componentSuffix = componentSuffix[..80].Trim('_');
		}

		return $"RPI_Board_{componentSuffix}";
	}

	private static string BuildBoardName(string legacyBoardKey, int vnum)
	{
		var boardName = string.IsNullOrWhiteSpace(legacyBoardKey)
			? $"board-{vnum.ToString(CultureInfo.InvariantCulture)}"
			: legacyBoardKey.Trim();

		return boardName.Length <= 45 ? boardName : boardName[..45];
	}

	private void MapGenericProp(
		RpiItemRecord item,
		ICollection<string> components,
		ICollection<string> tags,
		ICollection<RpiConversionWarning> warnings,
		ConversionStatus status = ConversionStatus.PropImport)
	{
		AddUnique(components, ChooseDestroyableComponent(item, status));

		if (item.ItemType == RPIItemType.Perfume)
		{
			AddUnique(tags, ChooseTag("Luxury Wares"));
		}

		if (item.ItemType == RPIItemType.Grain || item.ItemType == RPIItemType.Herb || item.ItemType == RPIItemType.Plant)
		{
			AddUnique(tags, ChooseTag("Standard Wares"));
		}

		if (ContainsAny(item, "box", "crate", "case") && item.ItemType is RPIItemType.Other or RPIItemType.Treasure)
		{
			AddUnique(components, ChooseComponent("Container_Pouch"));
			AddUnique(tags, ChooseTag("Open Container"));
		}
	}

	private static void AddUnique(ICollection<string> target, string? value)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			return;
		}

		if (!target.Contains(value, StringComparer.OrdinalIgnoreCase))
		{
			target.Add(value);
		}
	}

	private static bool ContainsAny(RpiItemRecord item, params string[] fragments)
	{
		var text = $"{item.RawName} {item.ShortDescription} {item.LongDescription} {item.DescKeys}";
		return fragments.Any(fragment => text.Contains(fragment, StringComparison.OrdinalIgnoreCase));
	}

	private static IEnumerable<string> Tokenise(RpiItemRecord item)
	{
		return ExtractKeywordWords($"{item.RawName} {item.ShortDescription} {item.DescKeys}");
	}

	private List<FutureMudTraitReference> MapTraitReferences(RpiItemRecord item, ICollection<RpiConversionWarning> warnings)
	{
		List<FutureMudTraitReference> results = [];

		foreach (var affect in item.Affects)
		{
			if (affect.Skill is null)
			{
				continue;
			}

			var traitName = affect.Skill.Value switch
			{
				RPISkill.Brawling => "Brawling",
				RPISkill.LightEdge => "Light-Edge",
				RPISkill.MediumEdge => "Medium-Edge",
				RPISkill.HeavyEdge => "Heavy-Edge",
				RPISkill.LightBlunt => "Light-Blunt",
				RPISkill.MediumBlunt => "Medium-Blunt",
				RPISkill.HeavyBlunt => "Heavy-Blunt",
				RPISkill.LightPierce => "Light-Pierce",
				RPISkill.MediumPierce => "Medium-Pierce",
				RPISkill.HeavyPierce => "Heavy-Pierce",
				RPISkill.Staff => "Staff",
				RPISkill.Polearm => "Polearm",
				RPISkill.Thrown => "Throwing",
				RPISkill.Blowgun => "Blowgun",
				RPISkill.Sling => "Sling",
				RPISkill.Shortbow => "Bows",
				RPISkill.Longbow => "Bows",
				RPISkill.Crossbow => "Crossbows",
				RPISkill.Dual => "Dual-Wield",
				RPISkill.Block => "Block",
				RPISkill.Parry => "Parry",
				RPISkill.Subdue => "Subdue",
				RPISkill.Disarm => "Disarm",
				RPISkill.Sneak => "Sneak",
				RPISkill.Hide => "Hide",
				RPISkill.Steal => "Steal",
				RPISkill.Pick => "Picklock",
				RPISkill.Search => "Search",
				RPISkill.Listen => "Listen",
				RPISkill.Forage => "Forage",
				RPISkill.Ritual => "Ritual",
				RPISkill.Scan => "Scan",
				RPISkill.Backstab => "Backstab",
				RPISkill.Barter => "Barter",
				RPISkill.Ride => "Ride",
				RPISkill.Climb => "Climb",
				RPISkill.Swimming => "Swimming",
				RPISkill.Hunt => "Hunt",
				RPISkill.Skin => "Skin",
				RPISkill.Sail => "Seafaring",
				RPISkill.Poisoning => "Poisoning",
				RPISkill.Alchemy => "Alchemy",
				RPISkill.Herbalism => "Herbalism",
				RPISkill.Clairvoyance => "Clairvoyance",
				RPISkill.DangerSense => "Danger-Sense",
				RPISkill.EmpathicHeal => "Empathy",
				RPISkill.Hex => "Hex",
				RPISkill.MentalBolt => "Psychic-Bolt",
				RPISkill.Prescience => "Prescience",
				RPISkill.Sensitivity => "Aura-Sight",
				RPISkill.Telepathy => "Telepathy",
				RPISkill.Seafaring => "Seafaring",
				RPISkill.Dodge => "Dodge",
				RPISkill.Tame => "Tame",
				RPISkill.Break => "Break",
				RPISkill.Metalcraft => "Metalcraft",
				RPISkill.Woodcraft => "Woodcraft",
				RPISkill.Textilecraft => "Textilecraft",
				RPISkill.Cookery => "Cookery",
				RPISkill.Baking => "Baking",
				RPISkill.Hideworking => "Hideworking",
				RPISkill.Stonecraft => "Stonecraft",
				RPISkill.Candlery => "Candlery",
				RPISkill.Brewing => "Brewing",
				RPISkill.Distilling => "Distilling",
				RPISkill.Literacy => "Literacy",
				RPISkill.Dyecraft => "Dyecraft",
				RPISkill.Apothecary => "Apothecary",
				RPISkill.Glasswork => "Glasswork",
				RPISkill.Gemcraft => "Gemcraft",
				RPISkill.Milling => "Milling",
				RPISkill.Mining => "Mining",
				RPISkill.Perfumery => "Perfumery",
				RPISkill.Pottery => "Pottery",
				RPISkill.Tracking => "Tracking",
				RPISkill.Farming => "Farming",
				RPISkill.Healing => "Healing",
				RPISkill.SpeakAtliduk => "Atliduk",
				RPISkill.SpeakAdunaic => "Adunaic",
				RPISkill.SpeakHaradaic => "Haradaic",
				RPISkill.SpeakWestron => "Westron",
				RPISkill.SpeakDunael => "Dunael",
				RPISkill.SpeakLabba => "Labba",
				RPISkill.SpeakNorliduk => "Norliduk",
				RPISkill.SpeakRohirric => "Rohirric",
				RPISkill.SpeakTalathic => "Talathic",
				RPISkill.SpeakUmitic => "Umitic",
				RPISkill.SpeakNahaiduk => "Nahaiduk",
				RPISkill.SpeakPukael => "Pukael",
				RPISkill.SpeakSindarin => "Sindarin",
				RPISkill.SpeakQuenya => "Quenya",
				RPISkill.SpeakSilvan => "Silvan",
				RPISkill.SpeakKhuzdul => "Khuzdul",
				RPISkill.SpeakOrkish => "Orkish",
				RPISkill.SpeakBlackSpeech => "Black Speech",
				RPISkill.ScriptSarati => "Sarati",
				RPISkill.ScriptTengwar => "Tengwar",
				RPISkill.ScriptBeleriandTengwar => "Tengwar",
				RPISkill.ScriptCerthasDaeron => "Cirth",
				RPISkill.ScriptAngerthasDaeron => "Cirth",
				RPISkill.ScriptQuenyanTengwar => "Tengwar",
				RPISkill.ScriptAngerthasMoria => "Cirth",
				RPISkill.ScriptGondorianTengwar => "Tengwar",
				RPISkill.ScriptArnorianTengwar => "Tengwar",
				RPISkill.ScriptNumenianTengwar => "Tengwar",
				RPISkill.ScriptNorthernTengwar => "Tengwar",
				RPISkill.ScriptAngerthasErebor => "Cirth",
				RPISkill.BlackWise => "Black-Wise",
				RPISkill.GreyWise => "Grey-Wise",
				RPISkill.WhiteWise => "White-Wise",
				RPISkill.Runecasting => "Runecasting",
				RPISkill.Gambling => "Gambling",
				RPISkill.Bonecarving => "Bonecarving",
				RPISkill.Gardening => "Gardening",
				RPISkill.Sleight => "Sleight",
				RPISkill.Astronomy => "Astronomy",
				_ => null,
			};

			if (string.IsNullOrWhiteSpace(traitName))
			{
				warnings.Add(new RpiConversionWarning(
					"unmapped-trait-modifier",
					$"Could not map item affect skill {affect.Skill.Value} to a FutureMUD trait."));
				continue;
			}

			results.Add(new FutureMudTraitReference(traitName, affect.Modifier, affect.Skill.Value));
		}

		return results;
	}

	private FutureMudLiquidReference? MapLiquidReference(RpiItemRecord item)
	{
		var rawValue = item.ItemType switch
		{
			RPIItemType.Light => item.LightData?.LiquidValue,
			RPIItemType.Liquid_container or RPIItemType.Fountain => item.DrinkContainerData?.LiquidValue,
			_ => null,
		};

		if (rawValue is null)
		{
			return null;
		}

		return new FutureMudLiquidReference(ResolveLiquidName(rawValue.Value), rawValue.Value);
	}

	private string ResolveMaterialName(RpiItemRecord item, ICollection<RpiConversionWarning> warnings)
	{
		var candidates = GetMaterialCandidates(item).ToList();
		if (_catalog is not null)
		{
			var resolved = _catalog.ChooseMaterial(candidates);
			if (!string.IsNullOrWhiteSpace(resolved))
			{
				return resolved;
			}
		}

		if (candidates.Count > 0)
		{
			return candidates[0];
		}

		warnings.Add(new RpiConversionWarning("missing-material", "Could not infer a target material; defaulting to 'other'."));
		return "other";
	}

	private IEnumerable<string> GetMaterialCandidates(RpiItemRecord item)
	{
		var text = $"{item.RawName} {item.ShortDescription} {item.DescKeys}";
		switch (item.InferredMaterial)
		{
			case RPIMaterial.Metal:
				if (text.Contains("gold", StringComparison.OrdinalIgnoreCase)) yield return "gold";
				if (text.Contains("silver", StringComparison.OrdinalIgnoreCase)) yield return "silver";
				if (text.Contains("bronze", StringComparison.OrdinalIgnoreCase)) yield return "bronze";
				if (text.Contains("brass", StringComparison.OrdinalIgnoreCase)) yield return "brass";
				if (text.Contains("copper", StringComparison.OrdinalIgnoreCase)) yield return "copper";
				if (text.Contains("steel", StringComparison.OrdinalIgnoreCase)) yield return "steel";
				if (text.Contains("iron", StringComparison.OrdinalIgnoreCase)) yield return "wrought iron";
				yield return "metal";
				yield break;
			case RPIMaterial.Wood:
				if (text.Contains("oak", StringComparison.OrdinalIgnoreCase)) yield return "oak";
				if (text.Contains("ash", StringComparison.OrdinalIgnoreCase)) yield return "ash";
				if (text.Contains("pine", StringComparison.OrdinalIgnoreCase)) yield return "pine";
				if (text.Contains("teak", StringComparison.OrdinalIgnoreCase)) yield return "teak";
				yield return "wood";
				yield break;
			case RPIMaterial.Textile:
				if (text.Contains("linen", StringComparison.OrdinalIgnoreCase)) yield return "linen";
				if (text.Contains("silk", StringComparison.OrdinalIgnoreCase)) yield return "silk";
				if (text.Contains("wool", StringComparison.OrdinalIgnoreCase)) yield return "wool";
				if (text.Contains("cotton", StringComparison.OrdinalIgnoreCase)) yield return "cotton";
				if (text.Contains("burlap", StringComparison.OrdinalIgnoreCase)) yield return "burlap";
				yield return "textile";
				yield break;
			case RPIMaterial.Leather:
				if (text.Contains("deer", StringComparison.OrdinalIgnoreCase)) yield return "deer leather";
				if (text.Contains("calf", StringComparison.OrdinalIgnoreCase)) yield return "calfskin";
				yield return "leather";
				yield break;
			case RPIMaterial.Stone:
				if (text.Contains("marble", StringComparison.OrdinalIgnoreCase)) yield return "marble";
				yield return "stone";
				yield break;
			case RPIMaterial.Glass:
				if (text.Contains("crystal", StringComparison.OrdinalIgnoreCase)) yield return "crystal";
				yield return "glass";
				yield break;
			case RPIMaterial.Parchment:
				yield return "Paper";
				yield return "parchment";
				yield break;
			case RPIMaterial.Ceramic:
				if (text.Contains("porcelain", StringComparison.OrdinalIgnoreCase)) yield return "porcelain";
				if (text.Contains("earthenware", StringComparison.OrdinalIgnoreCase)) yield return "earthenware";
				yield return "ceramic";
				yield break;
			case RPIMaterial.Meat:
				yield return "meat";
				yield break;
			case RPIMaterial.Vegetation:
				yield return "vegetation";
				yield return "wood";
				yield break;
		}

		yield return "other";
	}

	private string InferBaseName(RpiItemRecord item)
	{
		foreach (var keyword in item.NameKeywords)
		{
			if (ShouldSkipSourceKeyword(keyword))
			{
				continue;
			}

			var cleaned = CleanupKeyword(keyword);
			if (!string.IsNullOrWhiteSpace(cleaned))
			{
				return cleaned;
			}
		}

		return ExtractKeywordWords(item.ShortDescription).FirstOrDefault() ?? "item";
	}

	private string BuildKeywords(RpiItemRecord item, string baseName)
	{
		return string.Join(
			" ",
			Tokenise(item)
				.Append(baseName)
				.Where(x => !string.IsNullOrWhiteSpace(x))
				.Select(x => x.ToLowerInvariant())
				.Distinct(StringComparer.OrdinalIgnoreCase)
				.Take(18));
	}

	private static IEnumerable<string> ExtractKeywordWords(string? text)
	{
		if (string.IsNullOrWhiteSpace(text))
		{
			yield break;
		}

		var cleaned = NonKeywordRegex.Replace(text, " ");
		foreach (var token in cleaned.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
		{
			yield return token;
		}
	}

	private static string CleanupKeyword(string keyword)
	{
		var cleaned = keyword.Trim().TrimEnd('~').Trim();
		if (string.IsNullOrWhiteSpace(cleaned))
		{
			return string.Empty;
		}

		cleaned = cleaned.Replace("_", " ", StringComparison.Ordinal);
		cleaned = ExtractKeywordWords(cleaned).FirstOrDefault() ?? cleaned;
		return cleaned.ToLowerInvariant();
	}

	private static bool ShouldSkipSourceKeyword(string keyword)
	{
		return keyword.EndsWith("_QUALITY", StringComparison.OrdinalIgnoreCase)
		       || keyword.StartsWith("UNIQUE_", StringComparison.OrdinalIgnoreCase)
		       || keyword.StartsWith("WAREHOUSE_", StringComparison.OrdinalIgnoreCase)
		       || keyword.All(char.IsUpper)
		       || keyword.StartsWith('$')
		       || keyword.Contains("craft", StringComparison.OrdinalIgnoreCase);
	}

	private string? ResolveWearComponent(RpiItemRecord item, ICollection<RpiConversionWarning> warnings, bool armourMode)
	{
		if (item.WearBits.HasFlag(RPIWearBits.Head))
		{
			if (ContainsAny(item, "sallet"))
			{
				return ChooseComponent("Wear_Sallet_Helmet");
			}

			if (ContainsAny(item, "helm", "helmet", "cap"))
			{
				return ChooseComponent(armourMode ? "Wear_Half_Helmet" : "Wear_Skullcap");
			}

			return ChooseComponent("Wear_Hat");
		}

		if (item.WearBits.HasFlag(RPIWearBits.Body))
		{
			if (ContainsAny(item, "cuirass"))
			{
				return ChooseComponent("Wear_Cuirass");
			}

			if (ContainsAny(item, "hauberk"))
			{
				return ChooseComponent("Wear_Hauberk");
			}

			if (ContainsAny(item, "shirt"))
			{
				return ChooseComponent("Wear_Shirt");
			}

			if (ContainsAny(item, "doublet"))
			{
				return ChooseComponent("Wear_Doublet");
			}

			if (ContainsAny(item, "jerkin", "vest"))
			{
				return ChooseComponent("Wear_Jerkin");
			}

			if (ContainsAny(item, "dress", "robe", "gown", "kirtle"))
			{
				return ChooseComponent("Wear_Gown");
			}

			if (ContainsAny(item, "tunic", "gambeson", "chemise"))
			{
				return ChooseComponent(ContainsAny(item, "long", "sleeve") ? "Wear_Long-Sleeved_Tunic" : "Wear_Tunic");
			}

			return ChooseComponent("Wear_Tunic");
		}

		if (item.WearBits.HasFlag(RPIWearBits.Legs))
		{
			if (ContainsAny(item, "chausse"))
			{
				return ChooseComponent("Wear_Chausses");
			}

			if (ContainsAny(item, "greave"))
			{
				return ChooseComponent("Wear_Greaves");
			}

			if (ContainsAny(item, "cuisse", "tasset"))
			{
				return ChooseComponent("Wear_Cuisses");
			}

			if (ContainsAny(item, "skirt", "kilt"))
			{
				return ChooseComponent("Wear_Skirt");
			}

			if (ContainsAny(item, "hose"))
			{
				return ChooseComponent("Wear_Stockings");
			}

			if (ContainsAny(item, "braies", "shorts"))
			{
				return ChooseComponent("Wear_Shorts");
			}

			return ChooseComponent("Wear_Trousers");
		}

		if (item.WearBits.HasFlag(RPIWearBits.Hands))
		{
			return ChooseComponent("Wear_Gauntlets");
		}

		if (item.WearBits.HasFlag(RPIWearBits.Arms))
		{
			if (ContainsAny(item, "vambrace"))
			{
				return ChooseComponent("Wear_Vambraces");
			}

			return ChooseComponent("Wear_Bracers");
		}

		if (item.WearBits.HasFlag(RPIWearBits.Finger))
		{
			return ChooseComponent("Wear_Ring");
		}

		if (item.WearBits.HasFlag(RPIWearBits.Neck))
		{
			if (armourMode)
			{
				return ChooseComponentFromCandidates("Wear_Gorget", "Wear_Bevor");
			}

			if (ContainsAny(item, "scarf"))
			{
				return ChooseComponent("Wear_Scarf");
			}

			if (ContainsAny(item, "choker", "collar"))
			{
				return ChooseComponent("Wear_Choker");
			}

			return ChooseComponent("Wear_Necklace");
		}

		if (item.WearBits.HasFlag(RPIWearBits.Feet))
		{
			return ChooseComponent("Wear_Shoes");
		}

		if (item.WearBits.HasFlag(RPIWearBits.Waist))
		{
			return ChooseComponent("Wear_Waist");
		}

		if (item.WearBits.HasFlag(RPIWearBits.Belt) || item.WearBits.HasFlag(RPIWearBits.Sheath))
		{
			return ChooseComponent("Wear_Waist");
		}

		if (item.WearBits.HasFlag(RPIWearBits.Back))
		{
			if (armourMode || ContainsAny(item, "backplate"))
			{
				return ChooseComponentFromCandidates("Wear_Backplate", "Wear_Backpack");
			}

			return ChooseComponent("Wear_Backpack");
		}

		if (item.WearBits.HasFlag(RPIWearBits.About))
		{
			if (ContainsAny(item, "cape"))
			{
				return ChooseComponent("Wear_Cape");
			}

			if (ContainsAny(item, "open"))
			{
				return ChooseComponentFromCandidates("Wear_Cloak_(Open)", "Wear_Cape");
			}

			return ChooseComponentFromCandidates("Wear_Cloak_(Closed)", "Wear_Cloak_(Open)", "Wear_Cape");
		}

		if (item.WearBits.HasFlag(RPIWearBits.Wrist))
		{
			if (armourMode || ContainsAny(item, "bracer", "vambrace"))
			{
				return ChooseComponent("Wear_Bracers");
			}

			return ChooseComponentFromCandidates("Wear_Bracelets", "Wear_Bracelet");
		}

		if (item.WearBits.HasFlag(RPIWearBits.Shoulder))
		{
			if (ContainsAny(item, "pauldron"))
			{
				return ChooseComponent("Wear_Pauldrons");
			}

			return ChooseComponent("Wear_Shoulder");
		}

		if (item.WearBits.HasFlag(RPIWearBits.Throat) && armourMode)
		{
			return ChooseComponentFromCandidates("Wear_Bevor", "Wear_Gorget");
		}

		if (item.WearBits.HasFlag(RPIWearBits.Throat))
		{
			if (ContainsAny(item, "scarf"))
			{
				return ChooseComponent("Wear_Scarf");
			}

			return ChooseComponentFromCandidates("Wear_Choker", "Wear_Necklace");
		}

		if (item.WearBits.HasFlag(RPIWearBits.Ears))
		{
			return ChooseComponentFromCandidates("Wear_Earrings", "Wear_Earring");
		}

		if (item.WearBits.HasFlag(RPIWearBits.Ankle))
		{
			return ChooseComponentFromCandidates("Wear_Anklets", "Wear_Anklet");
		}

		if (item.WearBits.HasFlag(RPIWearBits.Hair))
		{
			return ChooseComponentFromCandidates("Wear_Wig", "Wear_Headband");
		}

		if (item.WearBits.HasFlag(RPIWearBits.Armband))
		{
			return ChooseComponent("Wear_Armlet");
		}

		if (item.WearBits.HasFlag(RPIWearBits.Face))
		{
			if (ContainsAny(item, "veil"))
			{
				return ChooseComponentFromCandidates("Wear_Veil", "Wear_Mask");
			}

			return ChooseComponent("Wear_Mask");
		}

		warnings.Add(new RpiConversionWarning(
			"unmapped-wear-profile",
			$"Could not find a seeded wear component for wear bits {item.WearBits}."));
		return null;
	}

	private string ChooseClothingOrArmourComponent(RpiItemRecord item, bool armourMode)
	{
		if (armourMode)
		{
			return item.ArmourData?.ArmourFamilyName switch
			{
				"Fur" => "Armour_HeavyClothing",
				"Quilted" => "Armour_UltraHeavyClothing",
				"Leather" => "Armour_BoiledLeather",
				"Scale" => "Armour_MetalScale",
				"Mail" => "Armour_Chainmail",
				"Plate" => "Armour_Platemail",
				_ => "Armour_HeavyClothing",
			};
		}

		var weight = ConvertWeightToGrams(item.Weight);
		if (weight <= 500)
		{
			return "Armour_LightClothing";
		}

		return weight <= 1800 ? "Armour_HeavyClothing" : "Armour_UltraHeavyClothing";
	}

	private static double ConvertWeightToGrams(int rawWeight)
	{
		return Math.Round(rawWeight / 100.0 * 453.59237, 2, MidpointRounding.AwayFromZero);
	}

	private int InferQuality(RpiItemRecord item)
	{
		if (item.QualityKeyword is not null)
		{
			if (item.QualityKeyword.Contains("POOR", StringComparison.OrdinalIgnoreCase))
			{
				return (int)ItemQuality.Poor;
			}

			if (item.QualityKeyword.Contains("FINE", StringComparison.OrdinalIgnoreCase)
			    || item.QualityKeyword.Contains("EXCELLENT", StringComparison.OrdinalIgnoreCase))
			{
				return (int)ItemQuality.Great;
			}

			return (int)ItemQuality.Standard;
		}

		return item.Quality switch
		{
			>= 45 => (int)ItemQuality.Great,
			>= 35 => (int)ItemQuality.Good,
			>= 25 => (int)ItemQuality.Standard,
			> 0 => (int)ItemQuality.Poor,
			_ => (int)ItemQuality.Standard,
		};
	}

	private int InferSize(RpiItemRecord item)
	{
		var grams = ConvertWeightToGrams(item.Weight);
		if (ContainsAny(item, "ring", "key"))
		{
			return (int)SizeCategory.Tiny;
		}

		if (ContainsAny(item, "arrow", "bolt", "bullet"))
		{
			return (int)SizeCategory.VerySmall;
		}

		return grams switch
		{
			<= 50 => (int)SizeCategory.Tiny,
			<= 250 => (int)SizeCategory.VerySmall,
			<= 1200 => (int)SizeCategory.Small,
			<= 5000 => (int)SizeCategory.Normal,
			<= 15000 => (int)SizeCategory.Large,
			<= 50000 => (int)SizeCategory.VeryLarge,
			<= 250000 => (int)SizeCategory.Huge,
			_ => (int)SizeCategory.Enormous,
		};
	}

	private string InferMeleeWeaponComponent(RpiItemRecord item)
	{
		if (ContainsAny(item, "dagger"))
		{
			return "Melee_Dagger";
		}

		if (ContainsAny(item, "knife"))
		{
			return "Melee_Knife";
		}

		if (ContainsAny(item, "rapier"))
		{
			return "Melee_Rapier";
		}

		if (ContainsAny(item, "zweihander", "bastard", "greatsword", "two-handed"))
		{
			return "Melee_Two Handed Sword";
		}

		if (ContainsAny(item, "sword"))
		{
			return "Melee_Longsword";
		}

		if (ContainsAny(item, "halberd", "glaive", "polearm"))
		{
			return "Melee_Halberd";
		}

		if (ContainsAny(item, "pike", "spear", "lance", "javelin"))
		{
			return item.WeaponData?.Hands >= 2 || ContainsAny(item, "pike", "long")
				? "Melee_Long Spear"
				: "Melee_Short Spear";
		}

		if (ContainsAny(item, "warhammer", "hammer", "maul"))
		{
			return "Melee_Warhammer";
		}

		if (ContainsAny(item, "club", "mace", "flail"))
		{
			return "Melee_Club";
		}

		if (ContainsAny(item, "axe", "hatchet"))
		{
			return "Melee_Axe";
		}

		if (ContainsAny(item, "mattock", "pick"))
		{
			return "Melee_Mattock";
		}

		return item.WeaponData?.UseSkill switch
		{
			RPISkill.LightEdge or RPISkill.MediumEdge or RPISkill.HeavyEdge => "Melee_Longsword",
			RPISkill.LightPierce or RPISkill.MediumPierce or RPISkill.HeavyPierce => "Melee_Short Spear",
			RPISkill.Polearm => "Melee_Halberd",
			RPISkill.LightBlunt or RPISkill.MediumBlunt or RPISkill.HeavyBlunt => "Melee_Club",
			_ => "Melee_Improvised Bludgeon",
		};
	}

	private string InferThrowingWeaponComponent(RpiItemRecord item)
	{
		if (ContainsAny(item, "knife", "dagger"))
		{
			return "Throwing_Knife";
		}

		if (ContainsAny(item, "axe"))
		{
			return "Throwing_Axe";
		}

		if (ContainsAny(item, "javelin", "spear"))
		{
			return "Throwing_Spear";
		}

		return string.Empty;
	}

	private string InferContainerComponent(RpiItemRecord item)
	{
		if (ContainsAny(item, "backpack", "pack", "haversack", "satchel"))
		{
			return "Container_Pack";
		}

		if (ContainsAny(item, "bag", "sack"))
		{
			return "Container_Sack";
		}

		if (ContainsAny(item, "box", "pouch", "case"))
		{
			return "Container_Pouch";
		}

		if (ContainsAny(item, "chest", "trunk", "locker", "strongbox", "crate"))
		{
			return "Container_Drum";
		}

		if (ContainsAny(item, "bin", "bucket"))
		{
			return "Container_Small_Drum";
		}

		if (ContainsAny(item, "table", "desk", "bed", "cot"))
		{
			return "Container_Small_Table";
		}

		return "Container_Pouch";
	}

	private string InferLiquidContainerComponent(RpiItemRecord item)
	{
		if (ContainsAny(item, "waterskin", "canteen", "bottle"))
		{
			return "LContainer_40ozBottle";
		}

		if (ContainsAny(item, "cup"))
		{
			return "LContainer_HalfPint";
		}

		if (ContainsAny(item, "goblet", "wineglass"))
		{
			return "LContainer_WineGlass";
		}

		if (ContainsAny(item, "flask"))
		{
			return "LContainer_Flask";
		}

		if (ContainsAny(item, "pot", "jug"))
		{
			return "LContainer_Jug";
		}

		if (ContainsAny(item, "bucket"))
		{
			return "LContainer_Amphora_Quadrantal";
		}

		if (ContainsAny(item, "barrel"))
		{
			return "LContainer_Puncheon";
		}

		return "LContainer_40ozBottle";
	}

	private string InferPaperComponent(RpiItemRecord item)
	{
		if (ContainsAny(item, "large"))
		{
			return "Paper_A3";
		}

		if (ContainsAny(item, "small", "note", "receipt"))
		{
			return "Paper_A5";
		}

		return "Paper_A4";
	}

	private string InferBookComponent(RpiItemRecord item, int pages)
	{
		var prefix = ContainsAny(item, "large") ? "Book_Large_" : ContainsAny(item, "small", "receipt", "ledger") ? "Book_Small_" : "Book_";
		var size = pages switch
		{
			<= 20 => "20_Page",
			<= 40 => "40_Page",
			<= 90 => "90_Page",
			<= 200 => "200_Page",
			<= 500 => "500_Page",
			_ => "1000_Page",
		};

		return $"{prefix}{size}";
	}

	private static string AdjustRepairQuality(string baseComponent, RpiItemRecord item)
	{
		if (baseComponent == "Repair_Cloth")
		{
			return baseComponent;
		}

		return item.QualityKeyword switch
		{
			not null when item.QualityKeyword.Contains("POOR", StringComparison.OrdinalIgnoreCase) => $"{baseComponent}_Poor",
			not null when item.QualityKeyword.Contains("FINE", StringComparison.OrdinalIgnoreCase) => $"{baseComponent}_Good",
			_ => baseComponent,
		};
	}

	private string ChooseDestroyableComponent(RpiItemRecord item, ConversionStatus status)
	{
		if (item.ItemType is RPIItemType.Parchment or RPIItemType.Book || item.InferredMaterial == RPIMaterial.Parchment)
		{
			return ChooseComponent("Destroyable_Paper");
		}

		if (item.ItemType is RPIItemType.Weapon or RPIItemType.Shield)
		{
			return ChooseComponent("Destroyable_Weapon");
		}

		if (item.ItemType is RPIItemType.Armor)
		{
			return ChooseComponent("Destroyable_Armour");
		}

		if (item.ItemType is RPIItemType.Worn or RPIItemType.Quiver or RPIItemType.Sheath or RPIItemType.Keyring)
		{
			return ChooseComponent(item.InferredMaterial is RPIMaterial.Leather or RPIMaterial.Textile ? "Destroyable_Clothing" : "Destroyable_Armour");
		}

		if (status == ConversionStatus.DeferredBehaviorPropImport && item.ItemType == RPIItemType.Dwelling)
		{
			return ChooseComponent("Destroyable_Door");
		}

		return ChooseComponent("Destroyable_Misc");
	}

	private void AddWearTags(RpiItemRecord item, ICollection<string> tags)
	{
		if (item.WearBits.HasFlag(RPIWearBits.Head))
		{
			AddUnique(tags, ChooseTag("Hats"));
		}

		if (item.WearBits.HasFlag(RPIWearBits.Body))
		{
			AddUnique(tags, ChooseTag("Bodywear"));
		}

		if (item.WearBits.HasFlag(RPIWearBits.Legs))
		{
			AddUnique(tags, ChooseTag("Legwear"));
		}

		if (item.WearBits.HasFlag(RPIWearBits.Hands))
		{
			AddUnique(tags, ChooseTag("Gloves"));
		}

		if (item.WearBits.HasFlag(RPIWearBits.Feet))
		{
			AddUnique(tags, ChooseTag("Footwear"));
		}

		if (item.WearBits.HasFlag(RPIWearBits.Waist))
		{
			AddUnique(tags, ChooseTag("Belts"));
		}
	}

	private void AddWeaponTags(RpiItemRecord item, ICollection<string> tags)
	{
		if (ContainsAny(item, "knife", "dagger"))
		{
			AddUnique(tags, ChooseTag("Daggers", "Knife"));
			return;
		}

		if (ContainsAny(item, "sword", "rapier"))
		{
			AddUnique(tags, ChooseTag("Swords"));
			return;
		}

		if (ContainsAny(item, "axe"))
		{
			AddUnique(tags, ChooseTag("Axes"));
			return;
		}

		if (ContainsAny(item, "spear", "pike", "halberd", "javelin"))
		{
			AddUnique(tags, ChooseTag("Spears"));
			return;
		}

		if (ContainsAny(item, "hammer", "mace", "club"))
		{
			AddUnique(tags, ChooseTag("Hammers", "Clubs"));
		}
	}

	private void AddContainerTags(RpiItemRecord item, ICollection<string> tags)
	{
		if (ContainsAny(item, "bag", "sack", "pack", "backpack", "haversack"))
		{
			AddUnique(tags, ChooseTag("Porous Container"));
		}
		else
		{
			AddUnique(tags, ChooseTag("Open Container"));
		}

		if (ContainsAny(item, "quiver", "bandolier"))
		{
			AddUnique(tags, ChooseTag("Military Goods"));
		}
	}

	private bool ShouldAddHoldable(RpiItemRecord item)
	{
		if (item.ItemType == RPIItemType.Fountain || item.ItemType == RPIItemType.Dwelling)
		{
			return false;
		}

		return item.WearBits.HasFlag(RPIWearBits.Take);
	}

	private string ChooseComponent(string name)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			return name;
		}

		return _catalog?.ChooseComponent([name]) ?? name;
	}

	private string? ChooseComponentOrNull(string name)
	{
		if (_catalog is null)
		{
			return name;
		}

		return _catalog.ChooseComponent([name]);
	}

	private string ChooseComponentFromCandidates(params string[] candidates)
	{
		var filtered = candidates.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
		if (filtered.Count == 0)
		{
			return string.Empty;
		}

		return _catalog?.ChooseComponent(filtered) ?? filtered[0];
	}

	private string ChooseTag(params string[] candidates)
	{
		var filtered = candidates.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
		if (filtered.Count == 0)
		{
			return string.Empty;
		}

		return _catalog?.ChooseTag(filtered) ?? filtered[0];
	}

	private string? ResolveLiquidName(int rawLiquidValue)
	{
		var legacyName = rawLiquidValue switch
		{
			0 => "water",
			1 => "ale",
			2 => "beer",
			3 => "cider",
			4 => "mead",
			5 => "wine",
			6 => "brandy",
			7 => "soup",
			8 => "milk",
			9 => "tea",
			10 => "saltwater",
			11 => "dark ale",
			12 => "amber ale",
			13 => "pale ale",
			14 => "dark beer",
			15 => "white wine",
			16 => "red wine",
			17 => "herbal tea",
			18 => "black tea",
			19 => "chamomile tea",
			20 => "tonic",
			21 => "blood",
			22 => "apple juice",
			23 => "peach nectar",
			24 => "pear juice",
			25 => "carrot juice",
			26 => "hard cider",
			27 => "whiskey",
			_ => null,
		};

		if (_catalog is null || string.IsNullOrWhiteSpace(legacyName))
		{
			return legacyName;
		}

		return _catalog.ChooseLiquid([legacyName, legacyName.Replace(" ", string.Empty, StringComparison.Ordinal), "water"]) ?? legacyName;
	}
}
