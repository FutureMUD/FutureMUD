#nullable enable

using System.Globalization;
using System.Text.RegularExpressions;
using MudSharp.Form.Shape;

namespace RPI_Engine_Worldfile_Converter;

public sealed class FutureMudNpcTransformer
{
	private static readonly Regex NonLettersRegex = new("[^A-Za-z]+", RegexOptions.Compiled);
	private static readonly IReadOnlyList<(string RaceName, string[] Needles)> SpecificAnimalRaceLexicalMap =
	[
		("Crow", [" crow "]),
		("Raven", [" raven "]),
		("Eagle", [" eagle "]),
		("Hawk", [" hawk "]),
		("Falcon", [" falcon "]),
		("Owl", [" owl "]),
		("Vulture", [" vulture "]),
		("Seagull", [" seagull ", " gull "]),
		("Duck", [" duck ", " drake ", " duckling "]),
		("Goose", [" goose ", " geese ", " gosling "]),
		("Swan", [" swan ", " cygnet "]),
		("Chicken", [" chicken ", " rooster ", " hen ", " chick "]),
		("Turkey", [" turkey ", " gobbling "]),
		("Pheasant", [" pheasant "]),
		("Quail", [" quail "]),
		("Pigeon", [" pigeon ", " dove ", " cooing "]),
		("Parrot", [" parrot "]),
		("Sparrow", [" songbird ", " sparrow ", " chirping "]),
		("Robin", [" robin "]),
		("Wren", [" wren "]),
		("Kingfisher", [" kingfisher "]),
		("Woodpecker", [" woodpecker "]),
		("Stork", [" stork "]),
		("Heron", [" heron "]),
		("Crane", [" crane "]),
		("Flamingo", [" flamingo "]),
		("Pelican", [" pelican "]),
		("Ibis", [" ibis "]),
		("Albatross", [" albatross "]),
		("Penguin", [" penguin "]),
		("Ostrich", [" ostrich "]),
		("Emu", [" emu "]),
		("Peacock", [" peacock ", " peafowl "]),
		("Cat", [" cat ", " tomcat ", " kitten "]),
		("Tiger", [" tiger ", " tigress "]),
		("Fox", [" fox ", " vixen "]),
		("Rabbit", [" rabbit ", " bunny "]),
		("Hare", [" hare "]),
		("Deer", [" deer ", " stag ", " doe ", " fawn "]),
		("Pig", [" pig ", " sow ", " piglet "]),
		("Boar", [" boar "]),
		("Sheep", [" sheep ", " ewe ", " ram ", " lamb "]),
		("Goat", [" goat ", " kid goat "]),
		("Cow", [" cow ", " bull ", " calf ", " cattle "]),
		("Crab", [" crab "]),
	];
	private static readonly HashSet<string> BestialCultureRaceNames =
	[
		"Wolf",
		"Horse",
		"Bird",
		"Rat",
		"Spider",
		"Giant Spider",
		"Warg",
		"Mule",
		"Donkey",
		"Dog",
		"Cat",
		"Snake",
		"Boar",
		.. SpecificAnimalRaceLexicalMap.Select(x => x.RaceName)
	];
	private static readonly HashSet<string> SpiritCultureRaceNames =
	[
		"Spirit",
		"Ghost",
		"Specter",
		"Wraith",
		"Ancestral Spirit",
		"Nature Spirit",
		"Elemental Spirit"
	];

	private readonly IReadOnlyDictionary<int, (string GroupKey, string ZoneName)> _zoneEvidence;

	public FutureMudNpcTransformer(IReadOnlyDictionary<int, (string GroupKey, string ZoneName)>? zoneEvidence = null)
	{
		_zoneEvidence = zoneEvidence ?? new Dictionary<int, (string GroupKey, string ZoneName)>();
	}

	public NpcConversionResult Convert(IEnumerable<RpiNpcRecord> npcs)
	{
		var converted = npcs
			.OrderBy(x => x.Zone)
			.ThenBy(x => x.Vnum)
			.Select(ConvertNpc)
			.ToList();

		return new NpcConversionResult(
			converted,
			ToSortedCounts(converted
				.SelectMany(x => x.Warnings.Where(y => y.Code.StartsWith("deferred-", StringComparison.OrdinalIgnoreCase)))
				.GroupBy(x => x.Code)),
			ToSortedCounts(converted
				.Where(x => string.IsNullOrWhiteSpace(x.RaceName))
				.GroupBy(x => x.ZoneName)),
			ToSortedCounts(converted
				.Where(x => string.IsNullOrWhiteSpace(x.CultureName))
				.GroupBy(x => x.ZoneName)));
	}

	public static IReadOnlyDictionary<int, (string GroupKey, string ZoneName)> BuildZoneEvidence(RoomConversionResult conversion)
	{
		return conversion.Zones
			.SelectMany(zone => zone.SourceZones.Select(sourceZone => (sourceZone, zone.GroupKey, zone.ZoneName)))
			.GroupBy(x => x.sourceZone)
			.ToDictionary(
				x => x.Key,
				x => (x.First().GroupKey, x.First().ZoneName));
	}

	private ConvertedNpcDefinition ConvertNpc(RpiNpcRecord npc)
	{
		var (zoneGroupKey, zoneName) = ResolveZone(npc.Zone);
		var warnings = new List<NpcConversionWarning>();
		var templateKind = npc.Flags.HasFlag(RpiNpcFlags.Variable) ? NpcTemplateKind.Variable : NpcTemplateKind.Simple;
		var classification = ClassifyNpc(npc);
		var resolution = ResolveRaceAndEthnicity(npc, zoneName, warnings);
		var culture = ResolveCulture(npc, zoneName, resolution, warnings);
		var templateName = $"RPI NPC {npc.Vnum.ToString(CultureInfo.InvariantCulture)} - {BuildLabel(npc)}";
		var nameChoice = ResolveTechnicalName(npc, resolution, culture);
		var gender = MapGender(npc.Sex);
		var genderChances = BuildGenderChances(npc, gender);
		var traits = ResolveTraits(npc, warnings);
		IReadOnlyList<NpcAttributeValue> attributes =
		[
			new NpcAttributeValue("Strength", npc.Strength, "STR"),
			new NpcAttributeValue("Intelligence", npc.Intelligence, "INT"),
			new NpcAttributeValue("Willpower", npc.Will, "WIL"),
			new NpcAttributeValue("Aura", npc.Aura, "AUR"),
			new NpcAttributeValue("Dexterity", npc.Dexterity, "DEX"),
			new NpcAttributeValue("Constitution", npc.Constitution, "CON"),
			new NpcAttributeValue("Agility", npc.Agility, "AGI"),
		];
		var aiNames = ResolveArtificialIntelligences(npc, warnings);
		var deferredFlags = ResolveDeferredBehaviorFlags(npc, warnings);
		var (heightMetres, weightKilograms) = ResolvePhysicals(npc, resolution.RaceName);

		var status = DetermineStatus(npc, resolution, culture);
		if (status == NpcConversionStatus.Deferred && !warnings.Any(x => x.Code == "deferred-vehicle"))
		{
			warnings.Add(new NpcConversionWarning("deferred-template", "NPC was deferred in this conversion pass."));
		}

		return new ConvertedNpcDefinition
		{
			Vnum = npc.Vnum,
			SourceFile = npc.SourceFile,
			Zone = npc.Zone,
			SourceKey = npc.SourceKey,
			ZoneGroupKey = zoneGroupKey,
			ZoneName = zoneName,
			Status = status,
			TemplateKind = templateKind,
			Classification = classification,
			TemplateName = templateName,
			Keywords = npc.Keywords,
			ShortDescription = npc.ShortDescription,
			LongDescription = npc.LongDescription,
			FullDescription = npc.FullDescription,
			RaceName = resolution.RaceName,
			EthnicityName = resolution.EthnicityName,
			CultureName = culture.CultureName,
			ResolutionEvidence = string.Join("; ", new[] { resolution.Evidence, culture.Evidence }.Where(x => !string.IsNullOrWhiteSpace(x))),
			TechnicalName = nameChoice.Name,
			UsesSourceDerivedName = nameChoice.UsesSourceName,
			SimpleGender = gender,
			GenderChances = genderChances,
			HeightMetres = heightMetres,
			WeightKilograms = weightKilograms,
			BirthdayDate = string.Empty,
			Attributes = attributes,
			Traits = traits,
			SpokenLanguageTraitName = traits.FirstOrDefault(x => x.IsPrimarySpokenLanguage && x.Resolved)?.TraitName,
			ArtificialIntelligenceNames = aiNames,
			DeferredBehaviorFlags = deferredFlags,
			HasShopData = npc.Shop is not null,
			HasVenomData = npc.Venom is not null,
			HasMorphData = npc.Morph is not null,
			HasClanMemberships = npc.ClanMemberships.Count > 0,
			Warnings = warnings
				.Concat(npc.ParseWarnings.Select(x => new NpcConversionWarning(x.Code, x.Message)))
				.ToList(),
			RawActFlags = npc.RawActFlags,
			RawFlags = npc.RawFlags,
			LegacyRaceId = npc.LegacyRaceId,
			LegacyArmour = npc.Armour,
			LegacyHeightInches = npc.HeightInches,
			LegacyFrame = npc.Frame,
			BirthTimestamp = npc.BirthTimestamp,
			NaturalDelay = npc.NaturalDelay,
			PoisonType = npc.PoisonType,
			NaturalAttackType = npc.NaturalAttackType,
			Shop = npc.Shop,
			Venom = npc.Venom,
			Morph = npc.Morph,
			ClanMemberships = npc.ClanMemberships,
		};
	}

	private (string GroupKey, string ZoneName) ResolveZone(int zone)
	{
		if (_zoneEvidence.TryGetValue(zone, out var evidence))
		{
			return evidence;
		}

		return ($"zone-{zone.ToString(CultureInfo.InvariantCulture)}", $"Zone {zone.ToString(CultureInfo.InvariantCulture)}");
	}

	private static NpcConversionStatus DetermineStatus(RpiNpcRecord npc, RaceResolution resolution, CultureResolution culture)
	{
		if (npc.ActFlags.HasFlag(RpiNpcActFlags.Vehicle))
		{
			return NpcConversionStatus.Deferred;
		}

		if (string.IsNullOrWhiteSpace(resolution.RaceName) || string.IsNullOrWhiteSpace(culture.CultureName))
		{
			return NpcConversionStatus.Unresolved;
		}

		return NpcConversionStatus.Ready;
	}

	private static RpiNpcClassification ClassifyNpc(RpiNpcRecord npc)
	{
		var text = $"{npc.Keywords} {npc.ShortDescription} {npc.LongDescription} {npc.FullDescription}";
		if (npc.ActFlags.HasFlag(RpiNpcActFlags.Vehicle))
		{
			return RpiNpcClassification.Vehicle;
		}

		if (ContainsAny(text, " test ", " dummy ", " qa ", "debug"))
		{
			return RpiNpcClassification.Test;
		}

		if (ContainsAny(text, "placeholder", "template", "example"))
		{
			return RpiNpcClassification.Placeholder;
		}

		if (ContainsAny(text, "helper", "assistant", "guide"))
		{
			return RpiNpcClassification.Helper;
		}

		if (npc.Shop is not null || npc.Flags.HasFlag(RpiNpcFlags.Keeper) || npc.ActFlags.HasFlag(RpiNpcActFlags.BulkTrader))
		{
			return RpiNpcClassification.Merchant;
		}

		if (ContainsAny(text, "wolf", "horse", "spider", "bird", "rat", "hound", "dog", "cat", "bear", "boar"))
		{
			return RpiNpcClassification.Animal;
		}

		return RpiNpcClassification.Standard;
	}

	private static IReadOnlyList<NpcGenderChance> BuildGenderChances(RpiNpcRecord npc, Gender mappedGender)
	{
		if (!npc.Flags.HasFlag(RpiNpcFlags.Variable))
		{
			return [new NpcGenderChance(mappedGender, 100)];
		}

		if (mappedGender is Gender.Male or Gender.Female)
		{
			return [new NpcGenderChance(mappedGender, 100)];
		}

		if (npc.ActFlags.HasFlag(RpiNpcActFlags.Enforcer))
		{
			return
			[
				new NpcGenderChance(Gender.Male, 90),
				new NpcGenderChance(Gender.Female, 10),
			];
		}

		return
		[
			new NpcGenderChance(Gender.Male, 50),
			new NpcGenderChance(Gender.Female, 50),
		];
	}

	private static IReadOnlyList<NpcTraitValue> ResolveTraits(RpiNpcRecord npc, ICollection<NpcConversionWarning> warnings)
	{
		var traits = new List<NpcTraitValue>();
		foreach (var skill in npc.Skills.OrderByDescending(x => x.Value).ThenBy(x => x.SkillId))
		{
			var resolvedName = ResolveTraitName(skill.SkillName);
			var resolved = !string.IsNullOrWhiteSpace(resolvedName) && !resolvedName.StartsWith("Skill ", StringComparison.OrdinalIgnoreCase);
			if (!resolved)
			{
				warnings.Add(new NpcConversionWarning(
					"unresolved-npc-trait",
					$"Could not map legacy skill '{skill.SkillName}' ({skill.SkillId}) to a FutureMUD trait."));
				resolvedName = skill.SkillName;
			}

			traits.Add(new NpcTraitValue(
				resolvedName!,
				skill.Value,
				skill.SkillId,
				skill.SkillName,
				IsLanguageSkill(skill.SkillName),
				skill.IsSpokenLanguage,
				resolved));
		}

		return traits;
	}

	private static IReadOnlyList<string> ResolveArtificialIntelligences(RpiNpcRecord npc, ICollection<NpcConversionWarning> warnings)
	{
		List<string> aiNames = [];

		if (npc.ActFlags.HasFlag(RpiNpcActFlags.Aggressive))
		{
			if (npc.ActFlags.HasFlag(RpiNpcActFlags.Memory))
			{
				aiNames.Add("TrackingAggressiveToAllOtherSpecies");
				warnings.Add(new NpcConversionWarning(
					"memory-approximated-as-tracking",
					"ACT_MEMORY was approximated as TrackingAggressiveToAllOtherSpecies in this pass."));
			}
			else
			{
				aiNames.Add("AggressiveToAllOtherSpecies");
			}
		}

		if (npc.ActFlags.HasFlag(RpiNpcActFlags.Enforcer))
		{
			warnings.Add(new NpcConversionWarning(
				"deferred-enforcer",
				"ACT_ENFORCER was preserved as metadata because this pass does not assume a law-seeded baseline."));
		}

		if (npc.ActFlags.HasFlag(RpiNpcActFlags.Wildlife))
		{
			warnings.Add(new NpcConversionWarning(
				"deferred-wildlife",
				"ACT_WILDLIFE was preserved as metadata because richer herd and predator behavior is deferred."));
		}

		return aiNames;
	}

	private static IReadOnlyList<string> ResolveDeferredBehaviorFlags(RpiNpcRecord npc, ICollection<NpcConversionWarning> warnings)
	{
		List<string> flags = [];
		if (npc.ActFlags.HasFlag(RpiNpcActFlags.Vehicle))
		{
			flags.Add("vehicle");
			warnings.Add(new NpcConversionWarning("deferred-vehicle", "ACT_VEHICLE mobs are deferred from NPC template import."));
		}

		if (npc.Shop is not null)
		{
			flags.Add("shop");
			warnings.Add(new NpcConversionWarning("deferred-shop-data", "Shopkeeper data was extracted but not converted in this pass."));
		}

		if (npc.Venom is not null)
		{
			flags.Add("venom");
			warnings.Add(new NpcConversionWarning("deferred-venom", "Venom payloads were extracted but not converted in this pass."));
		}

		if (npc.Morph is not null)
		{
			flags.Add("morph");
			warnings.Add(new NpcConversionWarning("deferred-morph", "Morph data was extracted but not converted in this pass."));
		}

		if (npc.ClanMemberships.Count > 0)
		{
			flags.Add("clans");
			warnings.Add(new NpcConversionWarning("deferred-clan-membership", "Clan memberships were extracted but not converted in this pass."));
		}

		if (npc.ActFlags.HasFlag(RpiNpcActFlags.StayPut))
		{
			flags.Add("stayput");
			warnings.Add(new NpcConversionWarning("legacy-stayput", "ACT_STAYPUT was preserved as provenance only."));
		}

		return flags;
	}

	private static (double HeightMetres, double WeightKilograms) ResolvePhysicals(RpiNpcRecord npc, string? raceName)
	{
		var heightMetres = npc.HeightInches > 0
			? Math.Round(npc.HeightInches * 0.0254, 3)
			: GetDefaultHeightMetres(raceName);
		var weightKilograms = EstimateWeightKilograms(heightMetres, npc.Frame, raceName);
		return (heightMetres, weightKilograms);
	}

	private static double GetDefaultHeightMetres(string? raceName)
	{
		var text = raceName?.ToLowerInvariant() ?? string.Empty;
		if (text.Contains("troll", StringComparison.Ordinal))
		{
			return 2.7;
		}

		if (text.Contains("horse", StringComparison.Ordinal))
		{
			return 1.7;
		}

		if (text.Contains("wolf", StringComparison.Ordinal) || text.Contains("warg", StringComparison.Ordinal))
		{
			return 0.9;
		}

		if (text.Contains("spider", StringComparison.Ordinal))
		{
			return 1.2;
		}

		if (text.Contains("dwarf", StringComparison.Ordinal))
		{
			return 1.35;
		}

		if (text.Contains("elf", StringComparison.Ordinal))
		{
			return 1.82;
		}

		return 1.75;
	}

	private static double EstimateWeightKilograms(double heightMetres, int frame, string? raceName)
	{
		var text = raceName?.ToLowerInvariant() ?? string.Empty;
		if (text.Contains("horse", StringComparison.Ordinal))
		{
			return 420.0;
		}

		if (text.Contains("wolf", StringComparison.Ordinal) || text.Contains("warg", StringComparison.Ordinal))
		{
			return text.Contains("warg", StringComparison.Ordinal) ? 85.0 : 55.0;
		}

		if (text.Contains("spider", StringComparison.Ordinal))
		{
			return text.Contains("giant", StringComparison.Ordinal) ? 140.0 : 25.0;
		}

		if (text.Contains("bird", StringComparison.Ordinal))
		{
			return 12.0;
		}

		if (text.Contains("rat", StringComparison.Ordinal))
		{
			return 3.0;
		}

		if (text.Contains("troll", StringComparison.Ordinal))
		{
			return Math.Round(320.0 + Math.Max(0, frame - 3) * 25.0, 1);
		}

		if (text.Contains("dwarf", StringComparison.Ordinal))
		{
			return Math.Round(75.0 + Math.Max(0, frame - 3) * 4.0, 1);
		}

		if (text.Contains("elf", StringComparison.Ordinal))
		{
			return Math.Round(68.0 + Math.Max(0, frame - 3) * 3.0, 1);
		}

		if (text.Contains("orc", StringComparison.Ordinal))
		{
			return Math.Round(90.0 + Math.Max(0, frame - 3) * 5.0, 1);
		}

		return Math.Round(Math.Max(45.0, 40.0 + ((heightMetres * 100.0) - 100.0) * 0.8 + (frame - 3) * 5.0), 1);
	}

	private static Gender MapGender(RpiNpcSex sex)
	{
		return sex switch
		{
			RpiNpcSex.Male => Gender.Male,
			RpiNpcSex.Female => Gender.Female,
			_ => Gender.Neuter,
		};
	}

	private static string BuildLabel(RpiNpcRecord npc)
	{
		var source = npc.ShortDescription;
		if (source.StartsWith("a ", StringComparison.OrdinalIgnoreCase))
		{
			source = source[2..];
		}
		else if (source.StartsWith("an ", StringComparison.OrdinalIgnoreCase))
		{
			source = source[3..];
		}
		else if (source.StartsWith("the ", StringComparison.OrdinalIgnoreCase))
		{
			source = source[4..];
		}

		source = NonLettersRegex.Replace(source, " ").Trim();
		return string.IsNullOrWhiteSpace(source) ? "Unnamed" : source;
	}

	private static (string Name, bool UsesSourceName) ResolveTechnicalName(
		RpiNpcRecord npc,
		RaceResolution resolution,
		CultureResolution culture)
	{
		var firstKeyword = npc.Keywords
			.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries)
			.FirstOrDefault()?
			.Trim() ?? string.Empty;
		if (!string.IsNullOrWhiteSpace(firstKeyword) &&
		    char.IsUpper(firstKeyword[0]) &&
		    !LooksGeneric(firstKeyword))
		{
			return (firstKeyword, true);
		}

		var prefix = resolution.EthnicityName ?? culture.CultureName ?? resolution.RaceName ?? "Imported";
		prefix = NonLettersRegex.Replace(prefix, "");
		if (string.IsNullOrWhiteSpace(prefix))
		{
			prefix = "Imported";
		}

		return ($"{prefix}{npc.Vnum.ToString(CultureInfo.InvariantCulture)}", false);
	}

	private static bool LooksGeneric(string value)
	{
		return value.Equals("a", StringComparison.OrdinalIgnoreCase) ||
		       value.Equals("an", StringComparison.OrdinalIgnoreCase) ||
		       value.Equals("the", StringComparison.OrdinalIgnoreCase) ||
		       value.Equals("orc", StringComparison.OrdinalIgnoreCase) ||
		       value.Equals("wolf", StringComparison.OrdinalIgnoreCase) ||
		       value.Equals("man", StringComparison.OrdinalIgnoreCase) ||
		       value.Equals("woman", StringComparison.OrdinalIgnoreCase);
	}

	private static string? ResolveTraitName(string skillName)
	{
		if (string.Equals(skillName, "Defunct", StringComparison.OrdinalIgnoreCase))
		{
			return null;
		}

		if (TraitAliasMap.TryGetValue(skillName, out var mapped))
		{
			return mapped;
		}

		return skillName switch
		{
			{ Length: > 0 } => skillName.Trim(),
			_ => null,
		};
	}

	private static bool IsLanguageSkill(string skillName)
	{
		return skillName is "Taliska" or "Haladin" or "Thrunon" or "Beast-Tongue" or "Valarin" or "Nandorin" or "Druag" or "Sindarin" or "Quenya" or "Avarin" or "Khuzdul" or "Orkish" or "Trollish";
	}

	private static readonly IReadOnlyDictionary<string, string> TraitAliasMap =
		new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["Brawling"] = "Brawling",
			["Small-Blade"] = "Small-Blade",
			["Double-Handed"] = "Double-Handed",
			["Sole-Wield"] = "Sole-Wield",
			["Shield-Use"] = "Shield-Use",
			["Hunting-bow"] = "Hunting-bow",
			["Warbow"] = "Warbow",
			["Danger-Sense"] = "Danger-Sense",
			["Psychic-Bolt"] = "Psychic-Bolt",
			["Aura-Sight"] = "Aura-Sight",
			["Beast-Tongue"] = "Beast-Tongue",
			["Valarin-Script"] = "Valarin-Script",
			["Black-Wise"] = "Black-Wise",
			["Grey-Wise"] = "Grey-Wise",
			["White-Wise"] = "White-Wise",
		};

	private static RaceResolution ResolveRaceAndEthnicity(
		RpiNpcRecord npc,
		string zoneName,
		ICollection<NpcConversionWarning> warnings)
	{
		var sourceText = $" {npc.Keywords} {npc.ShortDescription} {npc.LongDescription} {npc.FullDescription} ";
		var text = $"{sourceText} {zoneName} "
			.ToLowerInvariant();
		var subjectText = sourceText.ToLowerInvariant();

		if (npc.ActFlags.HasFlag(RpiNpcActFlags.Vehicle))
		{
			return new RaceResolution(null, null, "Vehicle mob deferred.", false);
		}

		if (ContainsAny(text, "uruk-hai"))
		{
			return new RaceResolution("Orc", "Uruk-Hai", "Resolved from 'uruk-hai' lexical cue.", true);
		}

		if (ContainsAny(text, " uruk "))
		{
			return new RaceResolution("Orc", "Uruk", "Resolved from 'uruk' lexical cue.", true);
		}

		if (ContainsAny(text, "olog-hai", "black troll"))
		{
			return new RaceResolution("Troll", "Olog-Hai", "Resolved from Olog-Hai lexical cue.", true);
		}

		if (ContainsAny(text, "cave troll"))
		{
			return new RaceResolution("Troll", "Cave Troll", "Resolved from cave troll lexical cue.", true);
		}

		if (ContainsAny(text, "hill troll"))
		{
			return new RaceResolution("Troll", "Hill Troll", "Resolved from hill troll lexical cue.", true);
		}

		var specificAnimalRace = ResolveSpecificAnimalRace(subjectText);
		if (specificAnimalRace is not null)
		{
			return new RaceResolution(
				specificAnimalRace,
				specificAnimalRace,
				$"Resolved from {specificAnimalRace} lexical cue.",
				true);
		}

		if (ContainsAny(text, "giant spider"))
		{
			return new RaceResolution("Giant Spider", "Giant Spider", "Resolved from giant spider lexical cue.", true);
		}

		if (ContainsAny(text, " spider "))
		{
			return new RaceResolution("Spider", "Spider", "Resolved from spider lexical cue.", true);
		}

		if (ContainsAny(text, "wolfspawn", " warg "))
		{
			return new RaceResolution("Warg", null, "Resolved from warg lexical cue.", true);
		}

		if (ContainsAny(text, " wolf "))
		{
			return new RaceResolution("Wolf", "Wolf", "Resolved from wolf lexical cue.", true);
		}

		if (ContainsAny(text, "warhorse"))
		{
			return new RaceResolution("Horse", "Horse", "Resolved from warhorse lexical cue.", true);
		}

		if (ContainsAny(text, " mare ", " stallion ", " horse "))
		{
			return new RaceResolution("Horse", "Horse", "Resolved from horse lexical cue.", true);
		}

		if (ContainsAny(text, " bird "))
		{
			return new RaceResolution("Crow", "Crow", "Resolved generic bird race conservatively to Crow.", true);
		}

		if (ContainsAny(text, " rat ", " rodent "))
		{
			return new RaceResolution("Rat", "Rat", "Resolved from rat lexical cue.", true);
		}

		if (ContainsAny(text, " wraith ", " nazgul ", " spectre "))
		{
			return new RaceResolution("Wraith", "Wraith", "Resolved from wraith lexical cue.", true);
		}

		if (ContainsAny(text, " ent "))
		{
			return new RaceResolution("Ent", "Ent", "Resolved from ent lexical cue.", true);
		}

		if (ContainsAny(text, "noldo elf", " noldo "))
		{
			return new RaceResolution("Elf", "Noldor", "Resolved from Noldor lexical cue.", true);
		}

		if (ContainsAny(text, "sinda elf", " sinda ", " grey-elf ", " grey elf ", " sindar "))
		{
			return new RaceResolution("Elf", "Sindar", "Resolved from Sindar lexical cue.", true);
		}

		if (ContainsAny(text, "silvan", "wood-elf", "wood elf"))
		{
			return new RaceResolution("Elf", "Silvan", "Resolved from Silvan lexical cue.", true);
		}

		if (ContainsAny(text, " elf "))
		{
			return new RaceResolution("Elf", null, "Resolved race from elf lexical cue.", true);
		}

		if (ContainsAny(text, " dwarf "))
		{
			return new RaceResolution("Dwarf", "Longbeard", "Resolved dwarf race conservatively to Longbeard.", true);
		}

		if (ContainsAny(text, " orc "))
		{
			return new RaceResolution("Orc", "Uruk", "Resolved generic orc lexical cue conservatively to Uruk.", true);
		}

		if (ContainsAny(text, "haradrim", " harad "))
		{
			return new RaceResolution("Human", "Haradrim", "Resolved from Haradrim lexical cue.", true);
		}

		if (ContainsAny(text, "black numenorean"))
		{
			return new RaceResolution("Human", "Black Numenorean", "Resolved from Black Numenorean lexical cue.", true);
		}

		if (ContainsAny(text, "corsair", " umbar "))
		{
			return new RaceResolution("Human", "Umbaric", "Resolved from Umbar or corsair lexical cue.", true);
		}

		if (ContainsAny(text, " variag ", " khand "))
		{
			return new RaceResolution("Human", "Variag", "Resolved from Variag lexical cue.", true);
		}

		if (ContainsAny(text, " easterling ", " rhun ", " rhûn "))
		{
			return new RaceResolution("Human", "Easterling", "Resolved from Easterling lexical cue.", true);
		}

		if (ContainsAny(text, "rohirrim", " rohan "))
		{
			return new RaceResolution("Human", "Rohirrim", "Resolved from Rohirrim lexical cue.", true);
		}

		if (ContainsAny(text, "dunlending", " dunland "))
		{
			return new RaceResolution("Human", "Dunlending", "Resolved from Dunlending lexical cue.", true);
		}

		if (ContainsAny(text, "dunedain", " dúnedain ", " ranger ", " arnor "))
		{
			var ethnicity = ContainsAny(text, "gondor", "ithilien", "minas tirith", "osgiliath")
				? "Gondorian Dunedain"
				: "Arnorian Dunedain";
			return new RaceResolution("Human", ethnicity, "Resolved from Dunedain lexical cue.", true);
		}

		if (ContainsAny(text, "gondor", " gondorian ", "ithilien", "minas tirith", "osgiliath", "pelargir", "dol amroth"))
		{
			return new RaceResolution("Human", "Gondorian", "Resolved from Gondorian lexical cue.", true);
		}

		switch (npc.LegacyRaceId)
		{
			case 1:
			case 2:
				return new RaceResolution(
					"Human",
					DetermineDefaultHumanEthnicity(zoneName, text),
					$"Resolved human race conservatively from legacy race id {npc.LegacyRaceId}.",
					true);
			case 4:
				return new RaceResolution("Troll", "Cave Troll", "Resolved from legacy cave troll race id.", true);
			case 5:
				return new RaceResolution("Elf", "Noldor", "Resolved from legacy Noldor race id.", true);
			case 6:
				return new RaceResolution("Elf", "Sindar", "Resolved from legacy Sindar race id.", true);
			case 7:
				return new RaceResolution("Troll", "Hill Troll", "Resolved from legacy hill troll race id.", true);
			case 10:
				return new RaceResolution("Spider", "Spider", "Resolved giant spider legacy race id onto seeded Spider race.", true);
			case 12:
			case 13:
				return new RaceResolution("Horse", "Horse", "Resolved from legacy horse race id.", true);
			case 14:
				return new RaceResolution("Crow", "Crow", "Resolved generic bird legacy race id conservatively to Crow.", true);
			case 15:
			case 21:
				return new RaceResolution("Warg", null, "Resolved from legacy warg race id.", true);
			case 17:
				return new RaceResolution("Hobbit", "Harfoot", "Resolved from legacy hobbit race id.", true);
			case 18:
				return new RaceResolution("Dwarf", "Longbeard", "Resolved legacy dwarf race id conservatively to Longbeard.", true);
			case 19:
				return new RaceResolution("Mule", "Mule", "Resolved from legacy mule race id.", true);
			case 20:
				return new RaceResolution("Donkey", "Donkey", "Resolved from legacy donkey race id.", true);
			case 22:
				return new RaceResolution("Dog", "Dog", "Resolved from legacy dog race id.", true);
			case 23:
				return new RaceResolution("Cat", "Cat", "Resolved from legacy cat race id.", true);
			case 25:
				return new RaceResolution("Boar", "Boar", "Resolved from legacy boar race id.", true);
			case 26:
				return new RaceResolution("Orc", "Uruk", "Resolved from legacy uruk race id.", true);
			case 27:
				return new RaceResolution("Human", "Haradrim", "Resolved from legacy Harad human race id.", true);
			case 28:
				return new RaceResolution("Human", "Easterling", "Resolved from legacy Easterling human race id.", true);
		}

		if (npc.LegacyRaceId == 0 ||
		    ContainsAny(text, " man ", " woman ", " child ", " commoner ", " guard ", " merchant ", " ostler ", " trainer "))
		{
			var ethnicity = DetermineDefaultHumanEthnicity(zoneName, text);
			return new RaceResolution("Human", ethnicity, $"Resolved human race from legacy race id {npc.LegacyRaceId}.", true);
		}

		warnings.Add(new NpcConversionWarning(
			"unresolved-race",
			$"Could not confidently resolve a FutureMUD race for legacy mob race id {npc.LegacyRaceId}."));
		return new RaceResolution(null, null, "No high-confidence race mapping.", false);
	}

	private static string DetermineDefaultHumanEthnicity(string zoneName, string text)
	{
		var combined = $"{zoneName} {text}".ToLowerInvariant();
		if (ContainsAny(combined, "morgul", "mordor"))
		{
			return "Black Numenorean";
		}

		if (ContainsAny(combined, "rohan", "rohir"))
		{
			return "Rohirrim";
		}

		if (ContainsAny(combined, "dunland", "dunlending"))
		{
			return "Dunlending";
		}

		if (ContainsAny(combined, "harad", "umbar", "corsair"))
		{
			return "Haradrim";
		}

		if (ContainsAny(combined, "easterling", "rhun"))
		{
			return "Easterling";
		}

		return "Gondorian";
	}

	private static CultureResolution ResolveCulture(
		RpiNpcRecord npc,
		string zoneName,
		RaceResolution race,
		ICollection<NpcConversionWarning> warnings)
	{
		if (string.IsNullOrWhiteSpace(race.RaceName))
		{
			return new CultureResolution(null, "Culture could not be resolved without a race.");
		}

		if (race.RaceName.Equals("Human", StringComparison.OrdinalIgnoreCase))
		{
			var culture = race.EthnicityName switch
			{
				"Gondorian" or "Gondorian Dunedain" => "Gondorian",
				"Arnorian Dunedain" => "Arnorian",
				"Rohirrim" => "Rohirrim",
				"Dunlending" => "Dunlending",
				"Easterling" => "Easterling",
				"Variag" => "Variag",
				"Umbaric" when zoneName.Contains("Umbar", StringComparison.OrdinalIgnoreCase) => "Corsair",
				"Black Numenorean" when zoneName.Contains("Umbar", StringComparison.OrdinalIgnoreCase) => "Corsair",
				"Umbaric" or "Black Numenorean" or "Haradrim" => "Haradrim",
				_ => null,
			};

			if (culture is not null)
			{
				return new CultureResolution(culture, $"Resolved human culture from ethnicity '{race.EthnicityName}'.");
			}
		}

		if (race.RaceName.Equals("Elf", StringComparison.OrdinalIgnoreCase))
		{
			var loweredZone = zoneName.ToLowerInvariant();
			if (ContainsAny(loweredZone, "rivendell", "imladris"))
			{
				return new CultureResolution("Rivendell", "Resolved elf culture from Rivendell zone cue.");
			}

			if (ContainsAny(loweredZone, "loth", "lorien", "lórien"))
			{
				return new CultureResolution("Lothlórien", "Resolved elf culture from Lothlórien zone cue.");
			}

			if (ContainsAny(loweredZone, "mithlond", "grey havens"))
			{
				return new CultureResolution("Mithlond", "Resolved elf culture from Mithlond zone cue.");
			}

			if (ContainsAny(loweredZone, "forlindon"))
			{
				return new CultureResolution("Forlindon", "Resolved elf culture from Forlindon zone cue.");
			}

			if (ContainsAny(loweredZone, "harlindon", "hardlindon"))
			{
				return new CultureResolution("Hardlindon", "Resolved elf culture from Harlindon zone cue.");
			}

			if (ContainsAny(loweredZone, "mirkwood", "woodland"))
			{
				return new CultureResolution("Woodland Realm", "Resolved elf culture from Woodland Realm zone cue.");
			}
		}

		if (race.RaceName.Equals("Dwarf", StringComparison.OrdinalIgnoreCase))
		{
			var loweredZone = zoneName.ToLowerInvariant();
			if (ContainsAny(loweredZone, "moria", "khazad"))
			{
				return new CultureResolution("Khazad-dum", "Resolved dwarf culture from Khazad-dum zone cue.");
			}

			if (ContainsAny(loweredZone, "erebor", "lonely mountain"))
			{
				return new CultureResolution("Erebor", "Resolved dwarf culture from Erebor zone cue.");
			}

			if (ContainsAny(loweredZone, "iron hills"))
			{
				return new CultureResolution("Iron Hills", "Resolved dwarf culture from Iron Hills zone cue.");
			}

			if (ContainsAny(loweredZone, "grey mountains"))
			{
				return new CultureResolution("Grey Mountains", "Resolved dwarf culture from Grey Mountains zone cue.");
			}
		}

		if (race.RaceName.Equals("Hobbit", StringComparison.OrdinalIgnoreCase))
		{
			var loweredZone = zoneName.ToLowerInvariant();
			if (ContainsAny(loweredZone, "bree"))
			{
				return new CultureResolution("Bree Hobbit", "Resolved hobbit culture from Bree zone cue.");
			}

			if (ContainsAny(loweredZone, "rhovanion", "anduin", "mirkwood"))
			{
				return new CultureResolution("Rhovanion Hobbit", "Resolved hobbit culture from Wilderland zone cue.");
			}

			return new CultureResolution("Shire Hobbit", "Resolved hobbit culture conservatively to Shire Hobbit.");
		}

		if (race.RaceName.Equals("Orc", StringComparison.OrdinalIgnoreCase))
		{
			var loweredZone = zoneName.ToLowerInvariant();
			if (ContainsAny(loweredZone, "morgul", "mordor", "barad", "udun"))
			{
				return new CultureResolution("Mordorian Orc", "Resolved orc culture from Mordor zone cue.");
			}

			if (ContainsAny(loweredZone, "misty"))
			{
				return new CultureResolution("Misty Mountains Orc", "Resolved orc culture from Misty Mountains zone cue.");
			}

			if (ContainsAny(loweredZone, "grey mountains"))
			{
				return new CultureResolution("Grey Mountains Orc", "Resolved orc culture from Grey Mountains zone cue.");
			}

			if (ContainsAny(loweredZone, "isengard"))
			{
				return new CultureResolution("Isengard Orc", "Resolved orc culture from Isengard zone cue.");
			}

			if (ContainsAny(loweredZone, "mirkwood", "dol guldur"))
			{
				return new CultureResolution("Mirkwood Orc", "Resolved orc culture from Mirkwood zone cue.");
			}
		}

		if (SpiritCultureRaceNames.Contains(race.RaceName))
		{
			return new CultureResolution("Spirit Court", "Resolved incorporeal spirit culture from supernatural seeder baseline.");
		}

		if (BestialCultureRaceNames.Contains(race.RaceName))
		{
			return new CultureResolution("Animal", "Resolved bestial culture from seeded Animal baseline.");
		}

		if (race.RaceName.Equals("Troll", StringComparison.OrdinalIgnoreCase))
		{
			return new CultureResolution("Animal", "Resolved troll culture conservatively to Animal for this pass.");
		}

		warnings.Add(new NpcConversionWarning(
			"unresolved-culture",
			$"Could not confidently resolve a culture for zone '{zoneName}' and race '{race.RaceName}'."));
		return new CultureResolution(null, "No high-confidence culture mapping.");
	}

	private static bool ContainsAny(string haystack, params string[] needles)
	{
		return needles.Any(needle => haystack.Contains(needle, StringComparison.OrdinalIgnoreCase));
	}

	private static string? ResolveSpecificAnimalRace(string sourceText)
	{
		foreach (var (raceName, needles) in SpecificAnimalRaceLexicalMap)
		{
			if (ContainsAny(sourceText, needles))
			{
				return raceName;
			}
		}

		return null;
	}

	private static IReadOnlyDictionary<string, int> ToSortedCounts<T>(IEnumerable<IGrouping<string, T>> groups)
	{
		return groups
			.OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
			.ToDictionary(x => x.Key, x => x.Count(), StringComparer.OrdinalIgnoreCase);
	}

	private sealed record RaceResolution(
		string? RaceName,
		string? EthnicityName,
		string Evidence,
		bool Resolved);

	private sealed record CultureResolution(
		string? CultureName,
		string Evidence);
}
