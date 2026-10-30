#nullable enable

extern alias EngineCompiler;

using System;
using System.Collections.Generic;
using System.Globalization;
using CultureInfo = System.Globalization.CultureInfo;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using MudSharp.FutureProg;
using MudSharp.Body;
using MudSharp.GameItems;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	internal static string NpcInventoryCapacityExpression => double.MaxValue.ToString("R", CultureInfo.InvariantCulture);
	private static readonly IReadOnlyDictionary<int, string> RaceBodyByType = new Dictionary<int, string>
	{
		[2] = "Toed Quadruped", [3] = "Insectoid", [4] = "Avian", [5] = "Serpentine",
		[6] = "Reptilian", [8] = "Arachnid", [9] = "Avian", [10] = "Cephalopod"
	};

	private Race ResolveRace(ArmWorldRace? source, int vnum, Race human,
		IReadOnlyDictionary<string, Race> named, IReadOnlyDictionary<long, Race> racesByBody)
	{
		var sourceIndex = source?.Index ?? 0;
		if (_races.TryGetValue(sourceIndex, out var cached)) return cached;
		var type = source?.Identity.ElementAtOrDefault(0) ?? 0;
		Race baseline;
		if (source is not null && named.TryGetValue(source.Name, out var exact)) baseline = exact;
		else if (type == 1) baseline = human;
		else if (!RaceBodyByType.TryGetValue(type, out var bodyName))
		{
			Adapt("npc race", vnum, $"Source race type {type} has no approved native body mapping; used configured Human race.");
			baseline = human;
		}
		else
		{
			var mappedBody = _db.BodyProtos.SingleOrDefault(x => x.Name == bodyName)
				?? throw new InvalidOperationException($"Preflight approved native NPC body {bodyName}, but it is missing.");
			var preferredBaselineName = type switch { 4 => "Ostrich", 9 => "Eagle", _ => null };
			baseline = preferredBaselineName is not null
			? _db.Races.Include(x => x.RacesAttributes).Include(x => x.DefaultHeightWeightModelNeuter).SingleOrDefault(x => x.Name == preferredBaselineName && x.BaseBodyId == mappedBody.Id)
			?? (racesByBody.TryGetValue(mappedBody.Id, out var preferredBodyRace) ? preferredBodyRace : _db.Races.Include(x => x.RacesAttributes).Include(x => x.DefaultHeightWeightModelNeuter).OrderBy(x => x.Id).First(x => x.BaseBodyId == mappedBody.Id))
			: racesByBody.TryGetValue(mappedBody.Id, out var mapped) ? mapped :
			_db.Races.Include(x => x.RacesAttributes).Include(x => x.DefaultHeightWeightModelNeuter).OrderBy(x => x.Id).First(x => x.BaseBodyId == mappedBody.Id);
		}
		var body = _db.BodyProtos.Single(x => x.Id == baseline.BaseBodyId);
		var clone = (Race)_db.Entry(baseline).CurrentValues.ToObject();
		clone.Id = Next<Race>();
		clone.Name = (source?.Name ?? "Unknown") + " (Worldfile NPC)";
		clone.ParentRaceId = baseline.Id;
		clone.BaseBodyId = body.Id;
		// The source inventory model has no practical carrying-capacity gate.
		// Use the existing race expression at the engine's largest finite weight,
		// only on this NPC-only derivative; player races and attributes are untouched.
		clone.MaximumLiftWeightExpression = NpcInventoryCapacityExpression;
		if (source is not null && _source.PhysicalGenerators.FirstOrDefault(x => x.Race == source.Index) is { } dimensions)
		{
			var mass = dimensions.Weight.Mean * 10.0;
			var size = mass switch
			{
				< 5 => SizeCategory.Tiny, < 15 => SizeCategory.VerySmall, < 40 => SizeCategory.Small,
				< 120 => SizeCategory.Normal, < 250 => SizeCategory.Large, < 600 => SizeCategory.VeryLarge,
				< 1500 => SizeCategory.Huge, < 5000 => SizeCategory.Enormous, < 15000 => SizeCategory.Gigantic,
				_ => SizeCategory.Titanic
			};
			clone.SizeStanding = clone.SizeProne = clone.SizeSitting = (int)size;
			Adapt("npc size category", source.Index, $"NPC-only body family uses native {size} from source mean mass {mass:R} kg; mass thresholds are an explicit native size approximation.");
		}
		clone.AvailabilityProgId = _settings.NativeBindings[ArmageddonNativeBinding.AlwaysFalseProg];
		clone.RacesAttributes = baseline.RacesAttributes.Select(x => new RacesAttributes
		{
			RaceId = clone.Id, AttributeId = x.AttributeId, IsHealthAttribute = x.IsHealthAttribute,
			AttributeBonus = x.AttributeBonus, DiceExpression = x.DiceExpression
		}).ToHashSet();
		clone.RacesBreathableGases = _db.RacesBreathableGases.Where(x => x.RaceId == baseline.Id).AsNoTracking().Select(x => new RacesBreathableGases
		{
			RaceId = clone.Id, GasId = x.GasId, Multiplier = x.Multiplier
		}).ToHashSet();
		clone.RacesBreathableLiquids = _db.RacesBreathableLiquids.Where(x => x.RaceId == baseline.Id).AsNoTracking().Select(x => new RacesBreathableLiquids
		{
			RaceId = clone.Id, LiquidId = x.LiquidId, Multiplier = x.Multiplier
		}).ToHashSet();
		clone.RacesAdditionalBodyparts = _db.RacesAdditionalBodyparts.Where(x => x.RaceId == baseline.Id).AsNoTracking()
			.Select(x => new RacesAdditionalBodyparts { RaceId = clone.Id, BodypartId = x.BodypartId, Usage = x.Usage }).ToHashSet();
		AddNpcInventory(clone);
		_db.Races.Add(clone);
		CaptureNpcRaceDefaults(clone);
		var ethnicityBaseline = _db.Ethnicities.Include(x => x.EthnicitiesCharacteristics).Include(x => x.EthnicitiesNameCultures)
			.FirstOrDefault(x => x.ParentRaceId == baseline.Id);
		CreateNpcEthnicity(clone, ethnicityBaseline, sourceIndex);
		_races[sourceIndex] = clone;
		Adapt("npc race", vnum, $"Source race '{source?.Name}' type {type} uses NPC-only race '{clone.Name}' on native {body.Name} anatomy, cloned from {baseline.Name}; unavailable in chargen. Native abstract inventory preserves carried reset goods without changing player anatomy.");
		return clone;
	}

	internal Ethnicity CreateNpcEthnicity(Race race, Ethnicity? baseline, int sourceIndex = 0)
	{
		var ethnicity = baseline is null
			? new Ethnicity { ChargenBlurb = "Historical NPC population.", EthnicGroup = "Common" }
			: (Ethnicity)_db.Entry(baseline).CurrentValues.ToObject();
		ethnicity.Id = Next<Ethnicity>();
		ethnicity.Name = race.Name;
		ethnicity.ParentRaceId = race.Id;
		ethnicity.AvailabilityProgId = _settings.NativeBindings[ArmageddonNativeBinding.AlwaysFalseProg];
		ethnicity.EthnicitiesCharacteristics = baseline?.EthnicitiesCharacteristics.Select(x => new EthnicitiesCharacteristics
		{
			EthnicityId = ethnicity.Id, CharacteristicDefinitionId = x.CharacteristicDefinitionId,
			CharacteristicProfileId = x.CharacteristicProfileId
		}).ToHashSet() ?? [];
		ethnicity.EthnicitiesNameCultures = baseline?.EthnicitiesNameCultures.Select(x => new EthnicitiesNameCultures
		{
			EthnicityId = ethnicity.Id, NameCultureId = x.NameCultureId, Gender = x.Gender
		}).ToHashSet() ?? [];
		_db.Ethnicities.Add(ethnicity);
		if (baseline is null) Adapt("npc ethnicity", sourceIndex, "Native body persistence requires an ethnicity; created an NPC-only population because its anatomy baseline has none. No source ethnicity or blood model is invented.");
		return ethnicity;
	}

	private readonly Dictionary<long, BodypartProto> _npcInventoryParts = new();
	private readonly List<(Race Race, PropertyValues Values)> _npcRaceDefaults = [];

	internal void CaptureNpcRaceDefaults(Race race) =>
		_npcRaceDefaults.Add((race, _db.Entry(race).CurrentValues.Clone()));

	internal void RestoreNpcRaceDefaults()
	{
		// INSERT sentinels can replace copied false/zero values with database defaults.
		// Explicit updates preserve the chosen baseline on our new NPC-only derivatives.
		foreach (var (race, values) in _npcRaceDefaults)
		{
			foreach (var property in _db.Entry(race).Properties.Where(x =>
				!x.Metadata.IsPrimaryKey() && (x.Metadata.GetDefaultValueSql() is not null ||
				 x.Metadata.FindAnnotation(RelationalAnnotationNames.DefaultValue) is not null)))
			{
				property.CurrentValue = values[property.Metadata.Name];
				property.IsModified = true;
			}
		}
	}

	private void AddNpcInventory(Race race)
	{
		var bodyIds = BodyAncestry(race.BaseBodyId);
		var additions = race.RacesAdditionalBodyparts.Where(x => x.Usage == "general").Select(x => x.BodypartId).ToArray();
		if (_db.BodypartProtos.Any(x => bodyIds.Contains(x.BodyId) && x.IsCore &&
		    x.BodypartType == (int)BodypartTypeEnum.Grabbing && x.Unary == false) ||
		    _db.BodypartProtos.Any(x => additions.Contains(x.Id) && x.BodypartType == (int)BodypartTypeEnum.Grabbing && x.Unary == false)) return;
		if (!_npcInventoryParts.TryGetValue(race.BaseBodyId, out var part))
		{
			var shape = _db.BodypartShapes.Local.FirstOrDefault(x => x.Name == "inventory")
				?? _db.BodypartShapes.FirstOrDefault(x => x.Name == "inventory");
			if (shape is null)
			{
				shape = new BodypartShape { Id = Next<BodypartShape>(), Name = "inventory" };
				_db.BodypartShapes.Add(shape);
			}
			part = new BodypartProto
			{
				Id = Next<BodypartProto>(), BodyId = race.BaseBodyId, Name = "worldfileinventory",
				Description = "inventory", BodypartShapeId = shape.Id, BodypartType = (int)BodypartTypeEnum.Grabbing,
				Unary = false, IsCore = false, IsVital = false, MaxLife = 80, SeveredThreshold = -1,
				SeverFormula = "0", RelativeHitChance = 0, RelativeInfectability = 0, DisplayOrder = 50,
				DefaultMaterialId = _db.BodypartProtos.Where(x => bodyIds.Contains(x.BodyId) && x.IsCore)
					.OrderBy(x => x.Id).Select(x => x.DefaultMaterialId).First(), Size = (int)SizeCategory.Normal
			};
			_db.BodypartProtos.Add(part);
			_npcInventoryParts.Add(race.BaseBodyId, part);
		}
		race.RacesAdditionalBodyparts.Add(new RacesAdditionalBodyparts
		{
			RaceId = race.Id, BodypartId = part.Id, Usage = "general"
		});
	}

	private static double MeanDice(string? text)
	{
		if (text is null) return 10;
		var match = Regex.Match(text, @"^\s*(\d+)\s*d\s*(\d+)(?:\s*([+-])\s*(\d+))?\s*$", RegexOptions.IgnoreCase);
		if (!match.Success) return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var constant) ? constant : 10;
		var count = double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
		var sides = double.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
		var bonus = match.Groups[4].Success ? double.Parse(match.Groups[4].Value, CultureInfo.InvariantCulture) : 0;
		return count * (sides + 1) / 2.0 + (match.Groups[3].Value == "-" ? -bonus : bonus);
	}

	private void InstallNpcTemplates()
	{
		var brain = _db.HealthStrategies.Single(x => x.Type == "BrainHitpoints" && x.Name == "Human HP");
		var brainXml = XElement.Parse(brain.Definition);
		var endurance = ResolveRecoveryAttribute(_db.TraitDefinitions.Where(x => x.Type == 1).ToArray());
		ConfigureArmageddonHitPointPolicy(brainXml, endurance.Id);
		brainXml.SetElementValue("SeparateCapacityPenalties", true);
		brainXml.SetElementValue("UseStun", true);
		brainXml.SetElementValue("UseCellularDamage", true);
		foreach (var (element, name) in new[] { ("MaximumStunExpression", "Human Max Stun Formula"), ("HealingTickStunExpression", "Human Stun Heal Per Tick") })
		{
			var native = _db.TraitExpressions.FirstOrDefault(x => x.Name == name);
			if (native is null)
			{
				native = new TraitExpression { Id = Next<TraitExpression>(), Name = "Armageddon " + name, Expression = "100" };
				_db.TraitExpressions.Add(native);
			}
			brainXml.SetElementValue(element, native.Id);
		}
		brain.Definition = brainXml.ToString();
		var humanRace = _db.Races.Single(x => x.Id == _settings.NativeBindings[ArmageddonNativeBinding.HumanRace]);
		humanRace.DefaultHealthStrategyId = brain.Id;
		var maximumExpression = brainXml.Element("MaximumHitPointsExpression")?.Value
			?? throw new InvalidOperationException("BrainHitpoints has no maximum hit-points expression.");
		var culture = _db.Cultures.Include(x => x.CulturesNameCultures).Single(x => x.Id == _settings.NativeBindings[ArmageddonNativeBinding.Culture]);
		var calendarRow = _db.Calendars.AsNoTracking().Single(x => x.Id == culture.PrimaryCalendarId);
		var calendar = new EngineCompiler::MudSharp.TimeAndDate.Date.Calendar(XElement.Parse(calendarRow.Definition));
		var nativeLanguages = _db.Languages.Include(x => x.Accents).AsNoTracking().ToArray();
		var languageNames = new[] { "Sirihish", "Bendune", "Allundean", "Mirukkim", "Kentu", "Nrizkt", "Untal",
			"Cavilish", "Tatlum", "Anyar", "Heshrak", "Vloran", "Domat", "Ghatti" };
		var fallbackLanguage = nativeLanguages.SingleOrDefault(x => x.Name == "Sirihish")
			?? nativeLanguages.Single(x => x.Id == _settings.NativeBindings[ArmageddonNativeBinding.Language]);
		if (culture.CulturesNameCultures.Count == 0) throw new InvalidOperationException("The configured NPC culture has no name cultures.");
		var fallbackNameCulture = culture.CulturesNameCultures.OrderBy(x => x.Gender).ThenBy(x => x.NameCultureId).First().NameCultureId;
		var human = _db.Races.Include(x => x.RacesAttributes).Include(x => x.DefaultHeightWeightModelNeuter)
			.Single(x => x.Id == _settings.NativeBindings[ArmageddonNativeBinding.HumanRace]);
		var raceNames = _db.Races.Include(x => x.RacesAttributes).Include(x => x.DefaultHeightWeightModelNeuter).AsNoTracking().ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
		var racesByBody = _db.Races.Include(x => x.RacesAttributes).Include(x => x.DefaultHeightWeightModelNeuter).AsNoTracking().GroupBy(x => x.BaseBodyId)
			.ToDictionary(x => x.Key, x => x.OrderBy(r => r.Id).First());
		var maxHpGroups = _source.Npcs.GroupBy(x => x.Combat[4] > 0 ? x.Combat[4] : 1).OrderBy(x => x.Key).ToArray();
		var strategies = new Dictionary<int, HealthStrategy>();
		foreach (var group in maxHpGroups)
		{
			var value = group.Key;
			var expression = new TraitExpression { Id = Next<TraitExpression>(), Name = "Armageddon NPC maximum hit points " + value, Expression = value.ToString(CultureInfo.InvariantCulture) };
			_db.TraitExpressions.Add(expression);
			var strategyXml = new XElement(brainXml);
			strategyXml.Element("MaximumHitPointsExpression")!.Value = expression.Id.ToString(CultureInfo.InvariantCulture);
			strategyXml.Element("CheckHeart")!.Value = "false";
			strategyXml.Element("UseHypoxiaDamage")!.Value = "false";
			strategyXml.SetElementValue("KnockoutOnCritical", false);
			var strategy = new HealthStrategy { Id = Next<HealthStrategy>(), Name = "Armageddon NPC " + value + " HP", Type = "BrainHitpoints", Definition = strategyXml.ToString() };
			_db.HealthStrategies.Add(strategy);
			strategies[value] = strategy;
		}
		Flush();
		var skillsByName = _db.TraitDefinitions.AsNoTracking().Where(x => x.Type == 0).ToArray()
			.GroupBy(x => x.Name, StringComparer.OrdinalIgnoreCase).ToDictionary(x => x.Key, x => x.First().Id, StringComparer.OrdinalIgnoreCase);
		foreach (var source in _source.Npcs.OrderBy(x => x.Vnum))
		{
			var sourceLanguage = SourcePrimaryLanguage(source);
			var sourceLanguageName = sourceLanguage >= 0 && sourceLanguage < languageNames.Length ? languageNames[sourceLanguage] : null;
			var language = nativeLanguages.SingleOrDefault(x => x.Name == sourceLanguageName) ?? fallbackLanguage;
			var accents = language.Accents.OrderBy(x => x.Id).ToArray();
			if (language.Name != sourceLanguageName)
				Adapt("npc language", source.Vnum, $"Source language index {sourceLanguage} ({sourceLanguageName ?? "invalid"}) has no installed equivalent; used {language.Name}.");
			var cashMinimum = Math.Max(0, source.Money[0] + Math.Min(0, source.Money[1]));
			var cashMaximum = Math.Max(0, source.Money[0] + Math.Max(0, source.Money[1]));
			var cashProg = Prog("ArmageddonWorldfileNpcCash" + source.Vnum.ToString(CultureInfo.InvariantCulture),
				$"givecurrency(tocurrency({_currency.Id}), random({cashMinimum}, {cashMaximum}), @npc, false)\nreturn true",
				ProgVariableTypes.Boolean, ("npc", ProgVariableTypes.Character));
			var hp = source.Combat[4] > 0 ? source.Combat[4] : 1;
			if (source.Combat[4] <= 0) Adapt("npc hit points", source.Vnum, $"Nonpositive source maximum hit points {source.Combat[4]} repaired to 1.");
			var sourceRace = _source.Races.FirstOrDefault(x => x.Index == source.Identity[6]);
			var sourceRaceName = sourceRace?.Name;
			var nativeRace = ResolveRace(sourceRace, source.Vnum, human, raceNames, racesByBody);
			var nativeEthnicity = _db.Ethnicities.Local.FirstOrDefault(x => x.ParentRaceId == nativeRace.Id)
				?? _db.Ethnicities.AsNoTracking().Where(x => x.ParentRaceId == nativeRace.Id).OrderBy(x => x.Id).FirstOrDefault()
				?? throw new InvalidOperationException($"NPC race {nativeRace.Name} has no native ethnicity.");
			var skills = new SortedDictionary<long, double>();
			var guild = _source.Guilds.FirstOrDefault(x => x.Index == source.Identity[3]);
			var attrRows = _db.TraitDefinitions.AsNoTracking().Where(x => x.Type == 1).ToArray();
			var hasWisdom = attrRows.Any(x => x.Name.Equals("Wisdom", StringComparison.OrdinalIgnoreCase));
			foreach (var link in nativeRace.RacesAttributes)
			{
				var attr = attrRows.FirstOrDefault(x => x.Id == link.AttributeId);
				if (attr is null) continue;
				var semantic = attr.Name.Trim().ToLowerInvariant() switch
				{
					"strength" or "upper body strength" or "lower body strength" => "strength",
					"agility" or "dexterity" or "speed" => "agility",
					"endurance" or "constitution" or "hardiness" or "resilience" or "stamina" => "endurance",
					"wisdom" or "willpower" or "perception" => "wisdom",
					"intelligence" or "mind" when !hasWisdom => "wisdom",
					_ => "neutral"
				};
				var sourceStatIndex = semantic switch { "strength" => 0, "agility" => 1, "wisdom" => 2, "endurance" => 3, _ => -1 };
				var statValue = sourceStatIndex >= 0 && sourceRace is not null && sourceRace.Stats.Length > sourceStatIndex
					? MeanDice(sourceRace.Stats[sourceStatIndex]) + (guild is not null && guild.Stats.Length > sourceStatIndex ? guild.Stats[sourceStatIndex] : 0)
					: MeanDice(link.DiceExpression) + link.AttributeBonus;
				skills[link.AttributeId] = Math.Clamp(statValue, 1, 100);
			}
			foreach (var entry in source.Skills)
			{
				if (entry.Length < 2 || entry[0] <= 0) continue;
				var name = _source.Skills.ElementAtOrDefault(entry[0]);
				if (name is not null && skillsByName.TryGetValue(name, out var id)) skills[id] = Math.Clamp(entry[1], 0, 100);
				else Adapt("npc skill", source.Vnum, $"Source skill index {entry[0]} ({name ?? "unknown"}) has no installed trait mapping; original value {entry[1]} retained in provenance.");
			}
			if (_willpowerSkill is not null)
			{
				var wisdom = sourceRace?.Stats.Length > 2
					? MeanDice(sourceRace.Stats[2]) + (guild?.Stats.Length > 2 ? guild.Stats[2] : 0)
					: 0;
				skills[_willpowerSkill.Id] = Math.Max(skills.GetValueOrDefault(_willpowerSkill.Id, 5),
					SourceWisdomLearningValue((int)Math.Clamp(Math.Floor(wisdom), 0, 100)));
			}
			skills.TryAdd(language.LinkedTraitId, 100);
			if (!HasSourceSpokenLanguage(source))
			{
				void Grant(string name, double value)
				{
					if (nativeLanguages.SingleOrDefault(x => x.Name == name) is { } grant) skills.TryAdd(grant.LinkedTraitId, value);
				}
				if (source.Identity[6] is 2 or 3) Grant("Sirihish", 100);
				if (source.Identity[6] == 33) Grant("Allundean", 60);
				if (source.Identity[6] == 32) Grant("Mirukkim", 100);
				if (guild?.Name.Equals("Merchant", StringComparison.OrdinalIgnoreCase) == true) Grant("Cavilish", 100);
				if (guild?.Name.Contains("Templar", StringComparison.OrdinalIgnoreCase) == true) Grant("Tatlum", 100);
			}
			var gender = source.Identity[2] switch { 1 => 0, 2 => 1, _ => 2 };
			var nameCulture = culture.CulturesNameCultures.FirstOrDefault(x => x.Gender == gender)?.NameCultureId;
			if (nameCulture is null)
			{
				nameCulture = fallbackNameCulture;
				Adapt("npc naming", source.Vnum, $"Culture has no name culture for native gender {gender}; used the first configured culture name entry.");
			}
			var dimensions = _source.PhysicalGenerators.Single(x => x.Race == source.Identity[6]);
			var metres = _db.UnitsOfMeasure.Single(x => x.Id == _settings.NativeBindings[ArmageddonNativeBinding.Metre]).BaseMultiplier;
			var height = dimensions.Height.Mean * 0.0254 * metres;
			var weight = dimensions.Weight.Mean * 10.0 * _kilograms;
			var age = SourceNpcAge(source.Age, sourceRace?.Identity.ElementAtOrDefault(1), nativeRace.AdultAge);
			if (source.Age == -1 && sourceRace is null)
				Adapt("npc age", source.Vnum, $"Source race is undefined; unspecified age uses native adult fallback {age}.");
			var selectedBirthday = calendar.GetDate(calendarRow.Date);
			selectedBirthday.AdvanceYears(-age, true);
			var birthday = selectedBirthday.GetDateString();
			var sourceClanMemberships = new List<XElement>();
			var sourceClanRows = source.Clans.ToList();
			if (source.Identity[4] > 0 && sourceClanRows.All(x => !x.TryGetProperty("Name", out var existingName) || ResolveClan(existingName.GetString() ?? "") != source.Identity[4]))
			{
				var legacyTribe = _source.Clans.FirstOrDefault(x => x.Index == source.Identity[4]);
				if (legacyTribe is not null)
					sourceClanRows.Add(JsonDocument.Parse(JsonSerializer.Serialize(new { Name = legacyTribe.Name, Rank = 0 })).RootElement.Clone());
			}
			foreach (var membership in sourceClanRows)
			{
				var clanName = membership.TryGetProperty("Name", out var clanNameJson) ? clanNameJson.GetString() ?? "" : "";
				var rankNumber = membership.TryGetProperty("Rank", out var rankJson) ? rankJson.GetInt32() : 0;
				var clanSourceId = ResolveClan(clanName);
				if (clanSourceId is not { } clanId || !_clans.TryGetValue(clanId, out var clan) || !_ranks.TryGetValue((clanId, Math.Max(0, rankNumber)), out var rank))
				{
					Adapt("npc clan", source.Vnum, $"Source clan membership '{clanName}' rank {rankNumber} retained as source provenance; no native clan/rank binding was available.");
					continue;
				}
				sourceClanMemberships.Add(new XElement("Membership", new XAttribute("clan", clan.Id), new XAttribute("rank", rank.Id), new XAttribute("paygrade", 0)));
			}
			var definition = new XElement("Definition", new XElement("OnLoadProg", cashProg.Id), new XElement("HealthStrategy", strategies[hp].Id),
				new XElement("DefaultCombatSetting", 0), new XElement("TemplateLoadAdditions", new XElement("ClanMemberships", sourceClanMemberships),
				new XElement("Outfits"), new XElement("Hooks"), new XElement("BankAccounts"), new XElement("BodyLoadout")),
				new XElement("SelectedEntityDescriptionPatterns"), new XElement("SelectedSdesc", source.ShortDescription),
				new XElement("SelectedFullDesc", source.FullDescription), new XElement("SelectedRace", nativeRace.Id),
				new XElement("SelectedCulture", culture.Id), new XElement("SelectedEthnicity", nativeEthnicity.Id),
				new XElement("SelectedName", new XElement("Name", new XAttribute("culture", nameCulture),
					new XElement("Element", new XAttribute("usage", "BirthName"), source.Name))),
				new XElement("SelectedBirthday", birthday), new XElement("SelectedGender", gender),
				new XElement("SelectedHeight", height), new XElement("SelectedWeight", weight),
				new XElement("SelectedAttributes", skills.Where(x => nativeRace.RacesAttributes.Any(a => a.AttributeId == x.Key)).Select(x => new XElement("Attribute", new XAttribute("Id", x.Key), new XAttribute("Value", x.Value)))),
				new XElement("SelectedSkills", skills.Where(x => !nativeRace.RacesAttributes.Any(a => a.AttributeId == x.Key)).Select(x => new XElement("Skill", new XAttribute("Value", x.Value), x.Key))),
				new XElement("SelectedNativeLanguage", language.Id), new XElement("SelectedAccents", accents.Length == 0 ? null : new XElement("Accent", accents[0].Id)),
				new XElement("SelectedCharacteristics"), new XElement("SelectedRoles", new XElement("Handedness", nativeRace.DefaultHandedness)),
				new XElement("Knowledges"), new XElement("MissingBodyparts"));
			var template = new NpcTemplate { Id = Next<NpcTemplate>(), RevisionNumber = 0, Type = "Simple", Name = "Armageddon " + source.Name,
				UniqueName = Key(source.Vnum), BuilderNotes = "Imported historical NPC. Source record and mappings are retained in seeder provenance.", Definition = definition.ToString(), EditableItem = Approval() };
			_db.NpcTemplates.Add(template);
			_npcs.Add(source.Vnum, template);
			Record("npc." + source.Vnum, nameof(NpcTemplate), template.Id, template.RevisionNumber,
				new { Source = source, NativeRace = nativeRace.Id, NativeCulture = culture.Id, NativeLanguage = language.Id,
					Skills = skills, Dimensions = dimensions, HealthStrategy = strategies[hp].Id, Decisions = "Attributes and physical dimensions use deterministic source generator means; dimensions preserve C integer division, inches and ten-stone units. Native culture/language/accent and calendar supply absent source demographics. Source associations are retained; the later behaviour pass records reviewed native AI/service mappings. Outfits are attached by the population pass." });
		}
		_counts["NPC templates"] = _npcs.Count;
		Adapt("npc programs", 0, "Historical scripts are retained in each NPC source provenance record. Reviewed C-special adaptations are recorded separately by the behaviour, service and justice passes; remaining associations are not executed.");
		Adapt("npc identity", 0, "Native culture/language and deterministic calendar birthday defaults are used where the source lacks native equivalents; source identity and literal descriptions remain archived.");
		Adapt("npc combat scalars", 0, "Source offense, defense and innate armour scalars are retained in provenance; installed body-family natural attacks/armour and mapped native traits supply combat. The source damroll field has no demonstrated active combat consumer in this source version.");
		Flush();
		RestoreNpcRaceDefaults();
		Flush();
		// Numeric zero is the legacy EF insertion sentinel for these columns;
		// otherwise the database would replace an abstract slot's zeros with
		// physical hit/infection chances. Limit the correction to our new parts.
		var inventoryPartIds = _npcInventoryParts.Values.Select(x => x.Id).ToArray();
		if (inventoryPartIds.Length > 0)
			_db.BodypartProtos.Where(x => inventoryPartIds.Contains(x.Id)).ExecuteUpdate(setters => setters
				.SetProperty(x => x.RelativeHitChance, 0)
				.SetProperty(x => x.RelativeInfectability, 0.0));
	}

	internal static TraitDefinition ResolveRecoveryAttribute(TraitDefinition[] attributes) =>
		new[] { "Endurance", "Constitution", "Hardiness", "Resilience", "Stamina" }
			.Select(name => attributes.SingleOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
			.FirstOrDefault(x => x is not null)
		?? throw new InvalidOperationException("Armageddon health recovery requires Endurance or its supported native equivalent.");

	internal static XElement ConfigureArmageddonHitPointPolicy(XElement definition, long enduranceAttributeId)
	{
		definition.SetElementValue("UseNegativeHitPoints", true);
		definition.SetElementValue("DeathHitPoints", -11);
		definition.SetElementValue("UnconsciousHitPoints", 0);
		definition.SetElementValue("MortallyWoundedHitPoints", -3);
		definition.SetElementValue("UseCriticalRecovery", true);
		definition.SetElementValue("CriticalRecoveryTrait", enduranceAttributeId);
		definition.SetElementValue("CriticalRecoveryDifficulty", 5); // Difficulty.Normal
		definition.SetElementValue("MortalRecoveryDifficulty", 6); // Difficulty.Hard
		definition.SetElementValue("CriticalRecoveryHealing", 1);
		definition.SetElementValue("CriticalRecoveryDamage", 1);
		return definition;
	}

	internal static int SourceNpcAge(int authoredAge, int? sourceMaximumAge, int nativeAdultAge) => authoredAge != -1
		? authoredAge : sourceMaximumAge.HasValue ? sourceMaximumAge.Value / 2 : nativeAdultAge > 0 ? nativeAdultAge : 18;

	private static bool HasSourceSpokenLanguage(ArmWorldNpc source) => source.Skills.Any(x =>
		x.Length > 1 && x[1] > 0 && (x[0] is >= 101 and <= 109 or 235 or 237 or 239 or 241 or 243));

	internal static int SourcePrimaryLanguage(ArmWorldNpc source) => HasSourceSpokenLanguage(source) ? source.Flags[3]
		: source.Identity[6] switch { 2 or 98 => 2, 3 => 3, 4 => 5, 12 => 4, 20 => 10, 43 => 6, _ => 0 };
}
