#nullable enable

using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.CharacterCreation;
using MudSharp.Character.Name;
using MudSharp.Database;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Models;
using MudSharp.Planes;
using MudSharp.RPG.Merits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace DatabaseSeeder.Seeders;

public partial class SupernaturalSeeder : IDatabaseSeeder
{
	private FuturemudDatabaseContext _context = null!;
	private Race _humanRace = null!;
	private Race _organicHumanoidRace = null!;
	private BodyProto _organicHumanoidBody = null!;
	private BodyProto _wingedHumanoidBody = null!;
	private BodyProto _hornedHumanoidBody = null!;
	private BodyProto _quadrupedBody = null!;
	private BodyProto _toedQuadrupedBody = null!;
	private FutureProg _alwaysTrue = null!;
	private FutureProg _alwaysFalse = null!;
	private FutureProg _alwaysZero = null!;
	private HealthStrategy _healthStrategy = null!;
	private TraitDefinition _healthTrait = null!;
	private TraitDefinition _strengthTrait = null!;
	private CorpseModel _humanoidCorpse = null!;
	private CorpseModel _animalCorpse = null!;
	private CorpseModel _undeadCorpse = null!;
	private CorpseModel _spiritCorpse = null!;
	private Liquid _blood = null!;
	private Liquid? _sweat;
	private Gas _breathableAir = null!;
	private PopulationBloodModel? _defaultPopulationBloodModel;
	private CharacteristicDefinition _personWordDefinition = null!;
	private ArmourType? _humanoidNaturalArmour;
	private long _primePlaneId;
	private long _astralPlaneId;
	private long _nextBodyProtoId;
	private readonly Dictionary<string, NameCulture> _nameCultures = new(StringComparer.OrdinalIgnoreCase);

	public IEnumerable<(string Id, string Question,
		Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
		Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions
		=> NonHumanSeederQuestions.GetQuestions();

	public int SortOrder => 303;
	public string Name => "Supernatural Seeder";
	public string Tagline => "Installs stock supernatural races, forms, planar bodies and undead examples";
	public string FullDescription =>
		@"Seeds a builder-facing catalogue of supernatural races including angels, fallen angels, demons, spirits, ghosts, gods, werewolves and supported undead. The package uses existing body, merit, planar and non-human race infrastructure rather than inventing new supernatural mechanics.";
	public bool SafeToRunMoreThanOnce => true;

	public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{
		_context = context;
		_context.Database.BeginTransaction();
		IReadOnlyDictionary<string, string> effectiveAnswers =
			CombatBalanceProfileHelper.MergeQuestionAnswersWithRecordedChoice(context, questionAnswers);
		LoadSharedSeederData(effectiveAnswers);

		var summary = new SupernaturalSeedSummary();
		Dictionary<string, BodyProto> bodyLookup = BuildBodyCatalogue(summary);
		EnsureSupernaturalSupport(summary);

		foreach (SupernaturalRaceTemplate template in Templates.Values)
		{
			SeedOrRefreshRace(template, bodyLookup[template.BodyKey], summary);
		}

		SeedFormMerits(summary);

		_context.SaveChanges();
		_context.Database.CommitTransaction();
		return summary.ToMessage(Templates.Count);
	}

	public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
	{
		if (!HasPrerequisites(context))
		{
			return ShouldSeedResult.PrerequisitesNotMet;
		}

		if (Templates.Keys.Any(name => !context.Races.Any(x => x.Name == name)))
		{
			return ShouldSeedResult.ReadyToInstall;
		}

		return HasMissingSupernaturalSupport(context)
			? ShouldSeedResult.ExtraPackagesAvailable
			: ShouldSeedResult.MayAlreadyBeInstalled;
	}

	private static bool HasPrerequisites(FuturemudDatabaseContext context)
	{
		string[] requiredBodies =
		[
			"Organic Humanoid",
			"Winged Humanoid",
			"Horned Humanoid",
			"Quadruped Base",
			"Toed Quadruped"
		];
		string[] requiredRaces =
		[
			"Human",
			"Organic Humanoid",
			"Wolf"
		];
		string[] requiredCharacteristicProfiles =
		[
			"All Eye Colours",
			"All Eye Shapes",
			"All Noses",
			"All Ears",
			"All Hair Colours",
			"All Facial Hair Colours",
			"All Hair Styles",
			"All Skin Colours",
			"All Frames"
		];
		string[] requiredAttacks =
		[
			"Bite",
			"Carnivore Bite",
			"Carnivore Low Bite",
			"Claw High Swipe",
			"Claw Low Swipe",
			"Animal Barge",
			"Animal Barge Pushback",
			"Horn Gore",
			"Wing Buffet",
			"Tail Spike",
			"Acid Spit",
			"Llama Spit",
			"Dragonfire Breath",
			"Tusk Sweep",
			"Head Ram",
			"Headbutt",
			"Claw Clamp",
			"Tree Haul",
			"Water Drag",
			"Tail Slap"
		];
		string[] healthTraitNames = ["Constitution", "Body", "Physique", "Endurance", "Hardiness", "Stamina"];
		string[] strengthTraitNames = ["Strength", "Physique", "Body", "Upper Body Strength"];

		return requiredBodies.All(body => context.BodyProtos.Any(x => x.Name == body)) &&
		       requiredRaces.All(race => context.Races.Any(x => x.Name == race)) &&
		       requiredCharacteristicProfiles.All(profile =>
			       context.CharacteristicProfiles.Any(x => x.Name == profile) ||
			       context.CharacteristicDefinitions.Any(x => x.Name == profile)) &&
		       context.CharacteristicDefinitions.Any(x => x.Name == "Person Word") &&
		       requiredAttacks.All(attack => context.WeaponAttacks.Any(x => x.Name == attack)) &&
		       context.FutureProgs.Any(x => x.FunctionName == "AlwaysTrue") &&
		       context.FutureProgs.Any(x => x.FunctionName == "AlwaysFalse") &&
		       context.FutureProgs.Any(x => x.FunctionName == "AlwaysZero") &&
		       context.CorpseModels.Any(x => x.Name == "Organic Human Corpse") &&
		       context.CorpseModels.Any(x => x.Name == "Organic Animal Corpse") &&
		       context.Liquids.Any(x => x.Name == "blood") &&
		       context.Gases.AsEnumerable().Any(x =>
			       x.Name.Contains("Breathable Atmosphere", StringComparison.OrdinalIgnoreCase)) &&
		       context.TraitDefinitions
			       .Where(x => x.Type == (int)TraitType.Attribute)
			       .AsEnumerable()
			       .Any(x => x.Name.In(healthTraitNames)) &&
		       context.TraitDefinitions
			       .Where(x => x.Type == (int)TraitType.Attribute)
			       .AsEnumerable()
			       .Any(x => x.Name.In(strengthTraitNames)) &&
		       context.Calendars.Any() &&
		       NonHumanSeederHealthStrategyHelper.AllStrategyNames.All(name =>
			       context.HealthStrategies.Any(x => x.Name == name));
	}

	private static bool HasMissingSupernaturalSupport(FuturemudDatabaseContext context)
	{
		return CustomBodyProfiles.Keys.Any(name => !context.BodyProtos.Any(x => x.Name == name)) ||
		       SupernaturalAttackNames.Any(name => !context.WeaponAttacks.Any(x => x.Name == name)) ||
		       SupernaturalCultureDefinitions.Keys.Any(name => !context.Cultures.Any(x => x.Name == name)) ||
		       SupernaturalNameDefinitions.Keys.Any(name => !context.NameCultures.Any(x => x.Name == name)) ||
		       FormTemplates.Any(template => !context.Merits.Any(x => x.Name == template.MeritName)) ||
		       !context.CorpseModels.Any(x => x.Name == SupernaturalUndeadCorpseModelName) ||
		       !context.CorpseModels.Any(x => x.Name == SupernaturalSpiritCorpseModelName) ||
		       Templates.Values
			       .Where(x => x.NeedsProfile == SupernaturalNeedsProfile.NonLiving)
			       .Any(template =>
				       context.Races.FirstOrDefault(x => x.Name == template.Name) is { NeedsToBreathe: true });
	}

	private void LoadSharedSeederData(IReadOnlyDictionary<string, string> answers)
	{
		_humanRace = _context.Races.First(x => x.Name == "Human");
		_organicHumanoidRace = _context.Races.First(x => x.Name == "Organic Humanoid");
		_organicHumanoidBody = _context.BodyProtos.First(x => x.Name == "Organic Humanoid");
		_wingedHumanoidBody = _context.BodyProtos.First(x => x.Name == "Winged Humanoid");
		_hornedHumanoidBody = _context.BodyProtos.First(x => x.Name == "Horned Humanoid");
		_quadrupedBody = _context.BodyProtos.First(x => x.Name == SupernaturalHumanoidTailDonorBodyName);
		_toedQuadrupedBody = _context.BodyProtos.First(x => x.Name == "Toed Quadruped");
		_alwaysTrue = _context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue");
		_alwaysFalse = _context.FutureProgs.First(x => x.FunctionName == "AlwaysFalse");
		_alwaysZero = _context.FutureProgs.First(x => x.FunctionName == "AlwaysZero");
		_humanoidCorpse = _context.CorpseModels.First(x => x.Name == "Organic Human Corpse");
		_animalCorpse = _context.CorpseModels.First(x => x.Name == "Organic Animal Corpse");
		_blood = _context.Liquids.First(x => x.Name == "blood");
		_sweat = _context.Liquids.FirstOrDefault(x => x.Name == "sweat");
		_breathableAir = _context.Gases
			.AsEnumerable()
			.First(x => x.Name.Contains("Breathable Atmosphere", StringComparison.OrdinalIgnoreCase));
		_defaultPopulationBloodModel = _humanRace.Ethnicities.FirstOrDefault()?.PopulationBloodModel ??
		                               _context.PopulationBloodModels.FirstOrDefault();
		_personWordDefinition = _context.CharacteristicDefinitions.First(x => x.Name == "Person Word");
		_humanoidNaturalArmour = _context.ArmourTypes.FirstOrDefault(x => x.Name == "Human Racial Tissue Armour");
		_nextBodyProtoId = _context.BodyProtos.Select(x => x.Id).AsEnumerable().DefaultIfEmpty(0).Max() + 1;
		_healthTrait = _context.TraitDefinitions
			.Where(x => x.Type == (int)TraitType.Attribute)
			.AsEnumerable()
			.First(x => x.Name.In("Constitution", "Body", "Physique", "Endurance", "Hardiness", "Stamina"));
		_strengthTrait = _context.TraitDefinitions
			.Where(x => x.Type == (int)TraitType.Attribute)
			.AsEnumerable()
			.First(x => x.Name.In("Strength", "Physique", "Body", "Upper Body Strength"));
		_healthStrategy = _context.HealthStrategies.First(x =>
			x.Name == NonHumanSeederHealthStrategyHelper.GetStrategyName(answers["model"]));

		IReadOnlyDictionary<string, Plane> planes = CoreDataSeeder.SeedDefaultPlanes(_context);
		_primePlaneId = planes["Prime Material"].Id;
		_astralPlaneId = planes["Astral Plane"].Id;
	}

	private void EnsureSupernaturalSupport(SupernaturalSeedSummary summary)
	{
		EnsureSupernaturalMaterials();
		_undeadCorpse = EnsureNonDecayingCorpseModel(
			SupernaturalUndeadCorpseModelName,
			"Preserved supernatural remains",
			"a preserved corpse of {0}",
			"{0}\n\nThe body shows no ordinary sign of decay. Supernatural force, curse or necromancy keeps it from changing further.",
			"{0} from a preserved undead body",
			"bone");
		_spiritCorpse = EnsureNonDecayingCorpseModel(
			SupernaturalSpiritCorpseModelName,
			"Dissipated spirit remains",
			"a fading spiritual remnant of {0}",
			"{0}\n\nThe shape is more impression than corpse, a residue of spirit energy lingering where a supernatural form was broken.",
			"{0} from a dissipated spirit",
			"spirit energy");
		EnsureSupernaturalAttacks(summary);
		EnsureSupernaturalCulturesAndNames(summary);
	}

	private Dictionary<string, BodyProto> BuildBodyCatalogue(SupernaturalSeedSummary summary)
	{
		Dictionary<string, BodyProto> bodies = new(StringComparer.OrdinalIgnoreCase)
		{
			["Organic Humanoid"] = _organicHumanoidBody,
			["Winged Humanoid"] = _wingedHumanoidBody,
			["Horned Humanoid"] = _hornedHumanoidBody,
			["Toed Quadruped"] = _toedQuadrupedBody
		};

		foreach ((string bodyName, (string SourceBody, SupernaturalPlanarProfile PlanarProfile)) in CustomBodyProfiles)
		{
			BodyProto source = bodyName switch
			{
				"Supernatural Winged Angel" or "Supernatural Many-Winged Angel" => _wingedHumanoidBody,
				"Supernatural Horned Fiend" => _hornedHumanoidBody,
				"Supernatural Familiar" => _hornedHumanoidBody,
				"Supernatural Hellhound" => _toedQuadrupedBody,
				_ when bodies.TryGetValue(SourceBody, out BodyProto? body) => body,
				_ => _organicHumanoidBody
			};
			BodyProto supernaturalBody = EnsureCustomBody(bodyName, source, PlanarProfile, summary);
			EnsureCustomBodyAdditions(bodyName, supernaturalBody);
			bodies[bodyName] = supernaturalBody;
		}

		foreach (string bodyKey in Templates.Values.Select(x => x.BodyKey).Distinct(StringComparer.OrdinalIgnoreCase))
		{
			if (!bodies.ContainsKey(bodyKey))
			{
				throw new InvalidOperationException($"Supernatural template body key {bodyKey} was not available.");
			}
		}

		return bodies;
	}

	private BodyProto EnsureCustomBody(string name, BodyProto source, SupernaturalPlanarProfile planarProfile,
		SupernaturalSeedSummary summary)
	{
		BodyProto? existing = _context.BodyProtos.FirstOrDefault(x => x.Name == name);
		bool created = existing is null;
		BodyProto body = existing ?? new BodyProto
		{
			Id = _nextBodyProtoId++,
			Name = name
		};

		body.Name = name;
		body.CountsAs = source;
		body.CountsAsId = source.Id;
		body.ConsiderString = source.ConsiderString;
		body.WielderDescriptionSingle = source.WielderDescriptionSingle;
		body.WielderDescriptionPlural = source.WielderDescriptionPlural;
		body.StaminaRecoveryProgId = source.StaminaRecoveryProgId;
		body.MinimumLegsToStand = source.MinimumLegsToStand;
		body.MinimumWingsToFly = name.Contains("Wing", StringComparison.OrdinalIgnoreCase) ? Math.Max(2, source.MinimumWingsToFly) : source.MinimumWingsToFly;
		body.LegDescriptionPlural = source.LegDescriptionPlural;
		body.LegDescriptionSingular = source.LegDescriptionSingular;
		body.WearSizeParameterId = source.WearSizeParameterId;
		body.NameForTracking = name;
		body.PlanarData = BuildPlanarProfile(_primePlaneId, _astralPlaneId, planarProfile).SaveToXml().ToString();

		if (created)
		{
			_context.BodyProtos.Add(body);
			_context.SaveChanges();
			SeederBodyUtilities.CloneBodyPositionsAndSpeeds(_context, source, body);
			summary.BodiesAdded++;
		}
		else
		{
			summary.BodiesRefreshed++;
		}

		return body;
	}

	private void EnsureCustomBodyAdditions(string bodyName, BodyProto body)
	{
		if (!CustomBodyAdditionalAliases.ContainsKey(bodyName))
		{
			return;
		}

		EnsureHumanoidTail(body);
	}

	private void EnsureHumanoidTail(BodyProto body)
	{
		if (FindBodypartOnBody(body, "utail") is null)
		{
			SeederBodyUtilities.CloneBodypartSubtree(_context, _quadrupedBody, body, "utail", "lback");
		}

		EnsureBodypartLimb(body, "Tail", LimbType.Appendage, "utail", "utail", "mtail", "ltail");
	}

	private void EnsureBodypartLimb(BodyProto body, string limbName, LimbType limbType, string rootAlias,
		params string[] aliases)
	{
		Limb? limb = _context.Limbs.FirstOrDefault(x => x.RootBodyId == body.Id && x.Name == limbName);
		if (limb is null)
		{
			BodypartProto? root = FindBodypartOnBody(body, rootAlias);
			if (root is null)
			{
				return;
			}

			limb = new Limb
			{
				Name = limbName,
				LimbType = (int)limbType,
				RootBody = body,
				RootBodypart = root,
				LimbDamageThresholdMultiplier = 0.5,
				LimbPainThresholdMultiplier = 0.5
			};
			_context.Limbs.Add(limb);
			_context.SaveChanges();
		}

		HashSet<long> existing = _context.LimbsBodypartProto
			.Where(x => x.LimbId == limb.Id)
			.Select(x => x.BodypartProtoId)
			.ToHashSet();
		List<long> bodyIds = SeederBodyUtilities.GetBodyAndAncestorIds(_context, body);
		IReadOnlyDictionary<string, IReadOnlyList<BodypartProto>> lookup = SeederBodyUtilities.BuildBodypartAliasLookup(
			_context.BodypartProtos
				.Where(x => bodyIds.Contains(x.BodyId) && aliases.Contains(x.Name))
				.ToList());

		foreach (string alias in aliases)
		{
			if (!lookup.TryGetValue(alias, out IReadOnlyList<BodypartProto>? matchingParts))
			{
				continue;
			}

			foreach (BodypartProto part in matchingParts.Where(x => existing.Add(x.Id)))
			{
				_context.LimbsBodypartProto.Add(new LimbBodypartProto
				{
					Limb = limb,
					BodypartProto = part
				});
			}
		}

		_context.SaveChanges();
	}

	private void SeedOrRefreshRace(SupernaturalRaceTemplate template, BodyProto body, SupernaturalSeedSummary summary)
	{
		Race? race = _context.Races.FirstOrDefault(x => x.Name == template.Name);
		bool created = race is null;
		race ??= new Race { Name = template.Name };

		race.Name = template.Name;
		race.Description = BuildRaceDescriptionForTesting(template);
		race.BaseBody = body;
		race.AllowedGenders = _organicHumanoidRace.AllowedGenders;
		race.ParentRace = template.UsesHumanoidCharacteristics ? _organicHumanoidRace : null;
		race.AttributeTotalCap = _humanRace.AttributeTotalCap;
		race.IndividualAttributeCap = _humanRace.IndividualAttributeCap;
		race.DiceExpression = _humanRace.DiceExpression;
		race.IlluminationPerceptionMultiplier = 1.0;
		race.AvailabilityProg = template.PlayableDefault ? _alwaysTrue : _alwaysFalse;
		race.CorpseModel = template.Family switch
		{
			SupernaturalFamily.Spirit => _spiritCorpse,
			SupernaturalFamily.Undead => _undeadCorpse,
			_ when template.NeedsProfile == SupernaturalNeedsProfile.NonLiving => _spiritCorpse,
			_ => template.UsesHumanoidCharacteristics ? _humanoidCorpse : _animalCorpse
		};
		race.DefaultHealthStrategy = _healthStrategy;
		race.DefaultCombatSetting = CombatStrategySeederHelper.EnsureCombatStrategy(_context, template.CombatStrategyKey);
		race.CanUseWeapons = template.CanUseWeapons;
		race.CanAttack = true;
		race.CanDefend = true;
		race.NaturalArmourType = template.UsesHumanoidCharacteristics ? _humanoidNaturalArmour : null;
		race.NaturalArmourQuality = template.Family is SupernaturalFamily.Divine or SupernaturalFamily.Demon or SupernaturalFamily.Angel ? 3 : 2;
		race.NaturalArmourMaterial = null;
		race.BloodLiquid = _blood;
		race.BloodModel = _humanRace.BloodModel;
		race.SizeStanding = (int)template.Size;
		race.SizeProne = (int)template.Size;
		race.SizeSitting = (int)template.Size;
		race.CommunicationStrategyType = "humanoid";
		race.DefaultHandedness = _humanRace.DefaultHandedness;
		race.HandednessOptions = template.CanUseWeapons ? _organicHumanoidRace.HandednessOptions : "1 3";
		race.MaximumDragWeightExpression = $"str:{_strengthTrait.Id}*40000";
		race.MaximumLiftWeightExpression = $"str:{_strengthTrait.Id}*10000";
		race.RaceUsesStamina = true;
		race.CanEatCorpses = false;
		race.EatCorpseEmoteText = string.Empty;
		race.CanEatMaterialsOptIn = false;
		race.BiteWeight = 1000;
		race.TemperatureRangeFloor = -40;
		race.TemperatureRangeCeiling = 80;
		race.BodypartSizeModifier = 0;
		race.BodypartHealthMultiplier = template.BodypartHealthMultiplier;
		race.CanClimb = template.CanClimb;
		race.CanSwim = template.CanSwim;
		race.MinimumSleepingPosition = _humanRace.MinimumSleepingPosition;
		race.ChildAge = template.NeedsProfile == SupernaturalNeedsProfile.Living ? 2 : 0;
		race.YouthAge = template.NeedsProfile == SupernaturalNeedsProfile.Living ? 6 : 1;
		race.YoungAdultAge = template.NeedsProfile == SupernaturalNeedsProfile.Living ? 12 : 2;
		race.AdultAge = template.NeedsProfile == SupernaturalNeedsProfile.Living ? 18 : 3;
		race.ElderAge = template.NeedsProfile == SupernaturalNeedsProfile.Living ? 65 : 1000;
		race.VenerableAge = template.NeedsProfile == SupernaturalNeedsProfile.Living ? 95 : 10000;
		race.DefaultHeightWeightModelMale = _humanRace.DefaultHeightWeightModelMale;
		race.DefaultHeightWeightModelFemale = _humanRace.DefaultHeightWeightModelFemale;
		race.DefaultHeightWeightModelNeuter = _humanRace.DefaultHeightWeightModelNeuter ?? _humanRace.DefaultHeightWeightModelMale;
		race.DefaultHeightWeightModelNonBinary = _humanRace.DefaultHeightWeightModelNonBinary ?? _humanRace.DefaultHeightWeightModelFemale;
		race.MaximumFoodSatiatedHours = _humanRace.MaximumFoodSatiatedHours;
		race.MaximumDrinkSatiatedHours = _humanRace.MaximumDrinkSatiatedHours;
		race.TrackIntensityVisual = _humanRace.TrackIntensityVisual;
		race.TrackIntensityOlfactory = template.Family == SupernaturalFamily.Spirit ? 0.0 : _humanRace.TrackIntensityOlfactory;
		race.TrackingAbilityVisual = _humanRace.TrackingAbilityVisual;
		race.TrackingAbilityOlfactory = template.Family == SupernaturalFamily.Spirit ? 0.0 : _humanRace.TrackingAbilityOlfactory;

		ApplyNeedsProfile(race, template);

		if (created)
		{
			_context.Races.Add(race);
			_context.SaveChanges();
			ApplyNeedsProfile(race, template);
			summary.RacesAdded++;
		}
		else
		{
			summary.RacesRefreshed++;
		}

		SeedRaceAttributes(race, template);
		Ethnicity ethnicity = SeedOrRefreshEthnicity(race, template);
		SeedAdditionalCharacteristics(race, ethnicity, template);
		SeedRacialBodypartUsages(race, template);
		SeedNaturalAttacks(race, template);
		SeedDefaultDescriptions(race, template, summary);
		_context.SaveChanges();
	}

	private void ApplyNeedsProfile(Race race, SupernaturalRaceTemplate template)
	{
		if (template.NeedsProfile == SupernaturalNeedsProfile.NonLiving)
		{
			race.NeedsToBreathe = false;
			race.BreathingModel = NonBreathingModel;
			race.BreathingVolumeExpression = "0";
			race.HoldBreathLengthExpression = "999999";
			race.HungerRate = 0.0;
			race.ThirstRate = 0.0;
			race.SweatLiquid = null;
			race.SweatRateInLitresPerMinute = 0.0;
			_context.RacesBreathableGases.RemoveRange(_context.RacesBreathableGases.Where(x => x.RaceId == race.Id).ToList());
			_context.RacesBreathableLiquids.RemoveRange(_context.RacesBreathableLiquids.Where(x => x.RaceId == race.Id).ToList());
			return;
		}

		race.NeedsToBreathe = true;
		race.BreathingModel = "simple";
		race.BreathingVolumeExpression = "7";
		race.HoldBreathLengthExpression = $"90+(5*con:{_healthTrait.Id})";
		race.HungerRate = _humanRace.HungerRate;
		race.ThirstRate = _humanRace.ThirstRate;
		race.SweatLiquid = _sweat;
		race.SweatRateInLitresPerMinute = _sweat is null ? 0.0 : 0.5;

		if (race.Id != 0 &&
		    !_context.RacesBreathableGases.Any(x => x.RaceId == race.Id && x.GasId == _breathableAir.Id))
		{
			_context.RacesBreathableGases.Add(new RacesBreathableGases
			{
				Race = race,
				Gas = _breathableAir,
				Multiplier = 1.0
			});
		}
	}

	private void SeedRaceAttributes(Race race, SupernaturalRaceTemplate template)
	{
		foreach (TraitDefinition attribute in _context.TraitDefinitions
			         .Where(x => x.Type == (int)TraitType.Attribute || x.Type == (int)TraitType.DerivedAttribute)
			         .ToList())
		{
			RacesAttributes? existing = _context.RacesAttributes
				.FirstOrDefault(x => x.RaceId == race.Id && x.AttributeId == attribute.Id);
			if (existing is null)
			{
				_context.RacesAttributes.Add(new RacesAttributes
				{
					Race = race,
					Attribute = attribute,
					IsHealthAttribute = attribute.TraitGroup == "Physical",
					AttributeBonus = NonHumanAttributeScalingHelper.GetAttributeBonus(attribute, template.AttributeProfile),
					DiceExpression = NonHumanAttributeScalingHelper.GetAttributeDiceExpression(attribute, template.AttributeProfile)
				});
				continue;
			}

			existing.IsHealthAttribute = attribute.TraitGroup == "Physical";
			existing.AttributeBonus = NonHumanAttributeScalingHelper.GetAttributeBonus(attribute, template.AttributeProfile);
			existing.DiceExpression = NonHumanAttributeScalingHelper.GetAttributeDiceExpression(attribute, template.AttributeProfile);
		}
	}

	private Ethnicity SeedOrRefreshEthnicity(Race race, SupernaturalRaceTemplate template)
	{
		string name = $"{template.Name} Stock";
		Ethnicity ethnicity = SeederRepeatabilityHelper.EnsureNamedEntity(
			_context.Ethnicities,
			name,
			x => x.Name,
			() =>
			{
				Ethnicity created = new();
				_context.Ethnicities.Add(created);
				return created;
			});

		ethnicity.Name = name;
		ethnicity.ChargenBlurb = BuildEthnicityDescriptionForTesting(template);
		ethnicity.AvailabilityProg = _alwaysFalse;
		ethnicity.ParentRace = race;
		ethnicity.EthnicGroup = template.Name;
		ethnicity.EthnicSubgroup = "Stock";
		ethnicity.PopulationBloodModel = _defaultPopulationBloodModel;
		ethnicity.TolerableTemperatureFloorEffect = 0;
		ethnicity.TolerableTemperatureCeilingEffect = 0;
		_context.SaveChanges();

		if (template.UsesHumanoidCharacteristics)
		{
			AddEthnicityCharacteristic(ethnicity, "Eye Colour", "All Eye Colours");
			AddEthnicityCharacteristic(ethnicity, "Eye Shape", "All Eye Shapes");
			AddEthnicityCharacteristic(ethnicity, "Nose", "All Noses");
			AddEthnicityCharacteristic(ethnicity, "Ears", "All Ears");
			AddEthnicityCharacteristic(ethnicity, "Hair Colour", "All Hair Colours");
			AddEthnicityCharacteristic(ethnicity, "Facial Hair Colour", "All Facial Hair Colours");
			AddEthnicityCharacteristic(ethnicity, "Hair Style", "All Hair Styles");
			AddEthnicityCharacteristic(ethnicity, "Facial Hair Style", "All Facial Hair Styles");
			AddEthnicityCharacteristic(ethnicity, "Skin Colour", "All Skin Colours");
			AddEthnicityCharacteristic(ethnicity, "Frame", "All Frames");
			AddEthnicityCharacteristic(ethnicity, "Person Word",
				EnsureStandardProfile(_personWordDefinition, $"{template.Name} Person Words",
					GetPersonWords(template)).Name);
		}

		ApplyEthnicityNameCulture(ethnicity, template);
		return ethnicity;
	}

	private static IReadOnlyList<string> GetPersonWords(SupernaturalRaceTemplate template)
	{
		return template.Family switch
		{
			SupernaturalFamily.Angel => ["angel", "messenger", "celestial"],
			SupernaturalFamily.Demon => ["demon", "fiend", "fallen one"],
			SupernaturalFamily.Divine => ["god", "deity", "avatar"],
			SupernaturalFamily.Spirit => ["spirit", "shade", "apparition"],
			SupernaturalFamily.Therianthrope => ["werewolf", "shifter", "wolf-blooded one"],
			SupernaturalFamily.Undead => ["undead", "deathless one", "corpse"],
			_ => ["being"]
		};
	}

	private void AddEthnicityCharacteristic(Ethnicity ethnicity, string definitionName, string profileName)
	{
		CharacteristicDefinition? definition = _context.CharacteristicDefinitions.FirstOrDefault(x => x.Name == definitionName);
		CharacteristicProfile? profile = _context.CharacteristicProfiles.FirstOrDefault(x => x.Name == profileName);
		if (definition is null || profile is null)
		{
			return;
		}

		if (_context.EthnicitiesCharacteristics.Any(x =>
			    x.EthnicityId == ethnicity.Id && x.CharacteristicDefinitionId == definition.Id))
		{
			return;
		}

		_context.EthnicitiesCharacteristics.Add(new EthnicitiesCharacteristics
		{
			Ethnicity = ethnicity,
			CharacteristicDefinition = definition,
			CharacteristicProfile = profile
		});
	}

	private void SeedAdditionalCharacteristics(Race race, Ethnicity ethnicity, SupernaturalRaceTemplate template)
	{
		foreach (SupernaturalCharacteristicTemplate characteristic in template.Characteristics)
		{
			CharacteristicDefinition definition = EnsureCharacteristicDefinition(
				characteristic.DefinitionName,
				$"{characteristic.DefinitionName} for supernatural races",
				characteristic.Type);
			CharacteristicProfile profile = EnsureStandardProfile(
				definition,
				$"All {characteristic.DefinitionName}",
				characteristic.Values);

			if (!_context.RacesAdditionalCharacteristics.Any(x =>
				    x.RaceId == race.Id && x.CharacteristicDefinitionId == definition.Id))
			{
				_context.RacesAdditionalCharacteristics.Add(new RacesAdditionalCharacteristics
				{
					Race = race,
					CharacteristicDefinition = definition,
					Usage = characteristic.Usage
				});
			}

			if (!_context.EthnicitiesCharacteristics.Any(x =>
				    x.EthnicityId == ethnicity.Id && x.CharacteristicDefinitionId == definition.Id))
			{
				_context.EthnicitiesCharacteristics.Add(new EthnicitiesCharacteristics
				{
					Ethnicity = ethnicity,
					CharacteristicDefinition = definition,
					CharacteristicProfile = profile
				});
			}
		}
	}

	private CharacteristicDefinition EnsureCharacteristicDefinition(string name, string description,
		CharacteristicType type = CharacteristicType.Standard)
	{
		CharacteristicDefinition? existing = _context.CharacteristicDefinitions.FirstOrDefault(x => x.Name == name);
		if (existing is not null)
		{
			return existing;
		}

		CharacteristicDefinition definition = new()
		{
			Name = name,
			Type = (int)type,
			Pattern = name.ToLowerInvariant().Replace(" ", string.Empty),
			Description = description,
			ChargenDisplayType = (int)CharacterGenerationDisplayType.DisplayAll,
			Model = "standard",
			Definition = string.Empty
		};
		_context.CharacteristicDefinitions.Add(definition);
		_context.SaveChanges();
		return definition;
	}

	private CharacteristicProfile EnsureStandardProfile(CharacteristicDefinition definition, string profileName,
		IEnumerable<string> values)
	{
		List<string> valueList = values.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
		foreach (string value in valueList)
		{
			if (_context.CharacteristicValues.Any(x => x.DefinitionId == definition.Id && x.Name == value))
			{
				continue;
			}

			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Name = value,
				Definition = definition,
				Value = value,
				AdditionalValue = string.Empty,
				Default = false,
				Pluralisation = 0
			});
		}

		CharacteristicProfile profile = SeederRepeatabilityHelper.EnsureNamedEntity(
			_context.CharacteristicProfiles,
			profileName,
			x => x.Name,
			() =>
			{
				CharacteristicProfile created = new();
				_context.CharacteristicProfiles.Add(created);
				return created;
			});

		StringBuilder definitionXml = new();
		definitionXml.Append("<Values>");
		foreach (string value in valueList)
		{
			definitionXml.Append($" <Value>{value}</Value>");
		}
		definitionXml.Append(" </Values>");

		profile.Name = profileName;
		profile.TargetDefinition = definition;
		profile.Type = "Standard";
		profile.Description = $"Auto-generated profile for {profileName}";
		profile.Definition = definitionXml.ToString();
		_context.SaveChanges();
		return profile;
	}

	private void SeedRacialBodypartUsages(Race race, SupernaturalRaceTemplate template)
	{
		if (template.BodypartUsages is null)
		{
			return;
		}

		foreach (SupernaturalBodypartUsageTemplate usage in template.BodypartUsages)
		{
			BodypartProto? bodypart = FindBodypartOnBody(race.BaseBody, usage.BodypartAlias);
			if (bodypart is null ||
			    _context.RacesAdditionalBodyparts.Any(x =>
				    x.RaceId == race.Id && x.BodypartId == bodypart.Id && x.Usage == usage.Usage))
			{
				continue;
			}

			_context.RacesAdditionalBodyparts.Add(new RacesAdditionalBodyparts
			{
				Race = race,
				Bodypart = bodypart,
				Usage = usage.Usage
			});
		}
	}

	private void SeedNaturalAttacks(Race race, SupernaturalRaceTemplate template)
	{
		foreach (SupernaturalAttackTemplate attackTemplate in template.Attacks)
		{
			WeaponAttack? attack = _context.WeaponAttacks.FirstOrDefault(x => x.Name == attackTemplate.AttackName);
			if (attack is null)
			{
				continue;
			}

			foreach (string alias in attackTemplate.BodypartAliases)
			{
				BodypartProto? bodypart = FindBodypartOnBody(race.BaseBody, alias);
				if (bodypart is null ||
				    _context.RacesWeaponAttacks.Any(x =>
					    x.RaceId == race.Id && x.BodypartId == bodypart.Id && x.WeaponAttackId == attack.Id))
				{
					continue;
				}

				_context.RacesWeaponAttacks.Add(new RacesWeaponAttacks
				{
					Race = race,
					Bodypart = bodypart,
					WeaponAttack = attack,
					Quality = (int)attackTemplate.Quality
				});
			}
		}
	}

	private BodypartProto? FindBodypartOnBody(BodyProto body, string alias)
	{
		return SeederBodyUtilities.FindBodypartOnBodyOrAncestors(_context, body, alias);
	}

	private void SeedDefaultDescriptions(Race race, SupernaturalRaceTemplate template, SupernaturalSeedSummary summary)
	{
		FutureProg prog = EnsureRaceDescriptionApplicabilityProg(race);
		int before = _context.EntityDescriptionPatterns.Count(x => x.ApplicabilityProgId == prog.Id);
		EnsureDescriptionVariants(prog, template.DescriptionVariants);
		int after = _context.EntityDescriptionPatterns.Count(x => x.ApplicabilityProgId == prog.Id);
		summary.DescriptionPatternsAdded += Math.Max(0, after - before);
	}

	private FutureProg EnsureRaceDescriptionApplicabilityProg(Race race)
	{
		string functionName = $"Is{race.Name.CollapseString()}";
		FutureProg prog = SeederRepeatabilityHelper.EnsureProg(
			_context,
			functionName,
			"Character",
			"Descriptions",
			ProgVariableTypes.Boolean,
			$"True if the character is a {race.Name}",
			$"return @ch.Race == ToRace(\"{race.Name}\")",
			true,
			false,
			FutureProgStaticType.NotStatic,
			(ProgVariableTypes.Toon, "ch"));
		_context.SaveChanges();
		return prog;
	}

	private void EnsureDescriptionVariants(FutureProg prog, IEnumerable<StockDescriptionVariant> variants)
	{
		foreach (StockDescriptionVariant variant in variants)
		{
			if (!_context.EntityDescriptionPatterns.Any(x =>
				    x.ApplicabilityProgId == prog.Id &&
				    x.Type == 0 &&
				    x.Pattern == variant.ShortDescription))
			{
				_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
				{
					Type = 0,
					ApplicabilityProg = prog,
					RelativeWeight = 100,
					Pattern = variant.ShortDescription
				});
			}

			if (!_context.EntityDescriptionPatterns.Any(x =>
				    x.ApplicabilityProgId == prog.Id &&
				    x.Type == 1 &&
				    x.Pattern == variant.FullDescription))
			{
				_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
				{
					Type = 1,
					ApplicabilityProg = prog,
					RelativeWeight = 100,
					Pattern = variant.FullDescription
				});
			}
		}
	}
}
