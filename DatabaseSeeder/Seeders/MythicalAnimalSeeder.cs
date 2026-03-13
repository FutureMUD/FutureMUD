#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DatabaseSeeder.Seeders;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Database;
using MudSharp.Form.Characteristics;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class MythicalAnimalSeeder : IDatabaseSeeder
{
	private FuturemudDatabaseContext _context = null!;
	private Race _humanRace = null!;
	private Race _organicHumanoidRace = null!;
	private BodyProto _organicHumanoidBody = null!;
	private BodyProto _quadrupedBody = null!;
	private BodyProto _ungulateBody = null!;
	private BodyProto _toedQuadrupedBody = null!;
	private BodyProto _avianBody = null!;
	private BodyProto _vermiformBody = null!;
	private BodyProto _serpentineBody = null!;
	private BodyProto _piscineBody = null!;
	private BodyProto _scorpionBody = null!;
	private HealthStrategy _healthStrategy = null!;
	private FutureProg _alwaysTrue = null!;
	private FutureProg _alwaysFalse = null!;
	private FutureProg _alwaysZero = null!;
	private TraitDefinition _healthTrait = null!;
	private TraitDefinition _strengthTrait = null!;
	private CorpseModel _humanoidCorpse = null!;
	private CorpseModel _animalCorpse = null!;
	private Liquid _blood = null!;
	private Liquid? _sweat;
	private Gas _breathableAir = null!;
	private PopulationBloodModel? _defaultPopulationBloodModel;
	private CharacteristicDefinition _personWordDefinition = null!;
	private ArmourType? _humanoidNaturalArmour;
	private ArmourType? _animalNaturalArmour;
	private long _nextBodyProtoId;

	public IEnumerable<(string Id, string Question,
		Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
		Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions
		=> NonHumanSeederQuestions.GetQuestions();

	public int SortOrder => 302;
	public string Name => "Mythical Animal Seeder";
	public string Tagline => "Installs a catalogue of mythic beasts and hybrid folk";
	public string FullDescription =>
		@"Seeds a dedicated catalogue of mythical creatures and hybrid folk, including draconic, avian, serpentine, merfolk and centaur variants. This package depends on both the Human and Animal seeders so it can reuse their anatomy, combat, corpse and characteristic infrastructure.";
	public bool SafeToRunMoreThanOnce => true;

	public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{
		_context = context;
		_context.Database.BeginTransaction();
		LoadSharedSeederData(questionAnswers);
		var templatesToSeed = Templates.Values
			.Where(template => !_context.Races.Any(x => x.Name == template.Name))
			.ToList();
		if (templatesToSeed.Count == 0)
		{
			_context.Database.CommitTransaction();
			return "Mythical races are already installed.";
		}

		var bodyLookup = BuildBodyCatalogue(templatesToSeed);

		foreach (var template in templatesToSeed)
		{
			SeedRace(template, bodyLookup[template.BodyKey]);
		}

		_context.SaveChanges();
		_context.Database.CommitTransaction();
		var skippedCount = Templates.Count - templatesToSeed.Count;
		return skippedCount > 0
			? $"Successfully installed {templatesToSeed.Count} mythical races and skipped {skippedCount} that already existed."
			: "Successfully installed mythical races.";
	}

	public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
	{
		if (!HasPrerequisites(context))
		{
			return ShouldSeedResult.PrerequisitesNotMet;
		}

		if (Templates.Keys.All(name => context.Races.Any(x => x.Name == name)))
		{
			return ShouldSeedResult.MayAlreadyBeInstalled;
		}

		return ShouldSeedResult.ReadyToInstall;
	}

	private static bool HasPrerequisites(FuturemudDatabaseContext context)
	{
		var requiredBodies = new[]
		{
			"Organic Humanoid",
			"Quadruped Base",
			"Ungulate",
			"Toed Quadruped",
			"Avian",
			"Vermiform",
			"Serpentine",
			"Piscine",
			"Scorpion"
		};
		var requiredRaces = new[]
		{
			"Human",
			"Organic Humanoid"
		};
		var requiredProfiles = new[]
		{
			"All Eye Colours",
			"All Eye Shapes",
			"All Noses",
			"All Ears",
			"All Hair Colours",
			"All Facial Hair Colours",
			"All Hair Styles",
			"All Skin Colours",
			"All Frames",
			"Person Word"
		};

		return requiredBodies.All(body => context.BodyProtos.Any(x => x.Name == body)) &&
		       requiredRaces.All(race => context.Races.Any(x => x.Name == race)) &&
		       requiredProfiles.All(profile =>
			       context.CharacteristicProfiles.Any(x => x.Name == profile) ||
			       context.CharacteristicDefinitions.Any(x => x.Name == profile)) &&
		       context.CorpseModels.Any(x => x.Name == "Organic Human Corpse") &&
		       context.CorpseModels.Any(x => x.Name == "Organic Animal Corpse") &&
		       context.HealthStrategies.Any(x => x.Name == "Non-Human HP") &&
		       context.HealthStrategies.Any(x => x.Name == "Non-Human HP Plus") &&
		       context.HealthStrategies.Any(x => x.Name == "Non-Human Full Model");
	}

	private void LoadSharedSeederData(IReadOnlyDictionary<string, string> answers)
	{
		_humanRace = _context.Races.First(x => x.Name == "Human");
		_organicHumanoidRace = _context.Races.First(x => x.Name == "Organic Humanoid");
		_organicHumanoidBody = _context.BodyProtos.First(x => x.Name == "Organic Humanoid");
		_quadrupedBody = _context.BodyProtos.First(x => x.Name == "Quadruped Base");
		_ungulateBody = _context.BodyProtos.First(x => x.Name == "Ungulate");
		_toedQuadrupedBody = _context.BodyProtos.First(x => x.Name == "Toed Quadruped");
		_avianBody = _context.BodyProtos.First(x => x.Name == "Avian");
		_vermiformBody = _context.BodyProtos.First(x => x.Name == "Vermiform");
		_serpentineBody = _context.BodyProtos.First(x => x.Name == "Serpentine");
		_piscineBody = _context.BodyProtos.First(x => x.Name == "Piscine");
		_scorpionBody = _context.BodyProtos.First(x => x.Name == "Scorpion");
		_alwaysTrue = _context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue");
		_alwaysFalse = _context.FutureProgs.First(x => x.FunctionName == "AlwaysFalse");
		_alwaysZero = _context.FutureProgs.First(x => x.FunctionName == "AlwaysZero");
		_humanoidCorpse = _context.CorpseModels.First(x => x.Name == "Organic Human Corpse");
		_animalCorpse = _context.CorpseModels.First(x => x.Name == "Organic Animal Corpse");
		_blood = _context.Liquids.First(x => x.Name == "blood");
		_sweat = _context.Liquids.FirstOrDefault(x => x.Name == "sweat");
		_breathableAir = _context.Gases.AsEnumerable().First(x => x.Name.Contains("air", StringComparison.OrdinalIgnoreCase));
		_defaultPopulationBloodModel = _humanRace.Ethnicities.FirstOrDefault()?.PopulationBloodModel ??
		                               _context.PopulationBloodModels.FirstOrDefault();
		_personWordDefinition = _context.CharacteristicDefinitions.First(x => x.Name == "Person Word");
		_humanoidNaturalArmour = _context.ArmourTypes.FirstOrDefault(x => x.Name == "Human Natural Armour");
		_animalNaturalArmour = _context.ArmourTypes.FirstOrDefault(x => x.Name == "Non-Human Natural Armour");
		_nextBodyProtoId = _context.BodyProtos.Select(x => x.Id).AsEnumerable().DefaultIfEmpty(0).Max() + 1;

		_healthTrait = _context.TraitDefinitions
			.Where(x => x.Type == (int)TraitType.Attribute)
			.AsEnumerable()
			.First(x => x.Name.In("Constitution", "Body", "Physique", "Endurance", "Hardiness", "Stamina"));
		_strengthTrait = _context.TraitDefinitions
			.Where(x => x.Type == (int)TraitType.Attribute)
			.AsEnumerable()
			.First(x => x.Name.In("Strength", "Physique", "Body", "Upper Body Strength"));

		_healthStrategy = answers["model"].ToLowerInvariant() switch
		{
			"hp" => _context.HealthStrategies.First(x => x.Name == "Non-Human HP"),
			"hpplus" => _context.HealthStrategies.First(x => x.Name == "Non-Human HP Plus"),
			"full" => _context.HealthStrategies.First(x => x.Name == "Non-Human Full Model"),
			_ => throw new InvalidOperationException($"Unknown health model choice {answers["model"]}.")
		};
	}

	private Dictionary<string, BodyProto> BuildBodyCatalogue(IEnumerable<MythicalRaceTemplate> templatesToSeed)
	{
		var requiredBodyKeys = templatesToSeed
			.Select(x => x.BodyKey)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		var bodies = new Dictionary<string, BodyProto>(StringComparer.OrdinalIgnoreCase);
		void AddBaseBody(string key, BodyProto body)
		{
			if (requiredBodyKeys.Contains(key))
			{
				bodies[key] = body;
			}
		}

		AddBaseBody("Organic Humanoid", _organicHumanoidBody);
		AddBaseBody("Toed Quadruped", _toedQuadrupedBody);
		AddBaseBody("Ungulate", _ungulateBody);
		AddBaseBody("Avian", _avianBody);
		AddBaseBody("Serpentine", _serpentineBody);

		if (requiredBodyKeys.Contains("Horned Humanoid"))
		{
			bodies["Horned Humanoid"] = GetOrCreateBody("Horned Humanoid", CreateHornedHumanoidBody);
		}

		if (requiredBodyKeys.Contains("Winged Humanoid"))
		{
			bodies["Winged Humanoid"] = GetOrCreateBody("Winged Humanoid", CreateWingedHumanoidBody);
		}

		if (requiredBodyKeys.Contains("Naga"))
		{
			bodies["Naga"] = GetOrCreateBody("Naga", CreateNagaBody);
		}

		if (requiredBodyKeys.Contains("Eastern Dragon"))
		{
			bodies["Eastern Dragon"] = GetOrCreateBody("Eastern Dragon", CreateEasternDragonBody);
		}

		if (requiredBodyKeys.Contains("Mermaid"))
		{
			bodies["Mermaid"] = GetOrCreateBody("Mermaid", CreateMermaidBody);
		}

		if (requiredBodyKeys.Contains("Centaur"))
		{
			bodies["Centaur"] = GetOrCreateBody("Centaur", CreateCentaurBody);
		}

		if (requiredBodyKeys.Contains("Griffin"))
		{
			bodies["Griffin"] = GetOrCreateBody("Griffin", CreateGriffinBody);
		}

		if (requiredBodyKeys.Contains("Hippogriff"))
		{
			bodies["Hippogriff"] = GetOrCreateBody("Hippogriff", CreateHippogriffBody);
		}

		if (requiredBodyKeys.Contains("Manticore"))
		{
			bodies["Manticore"] = GetOrCreateBody("Manticore", CreateManticoreBody);
		}

		if (requiredBodyKeys.Contains("Wyvern"))
		{
			bodies["Wyvern"] = GetOrCreateBody("Wyvern", CreateWyvernBody);
		}

		if (requiredBodyKeys.Contains("Hippocamp"))
		{
			bodies["Hippocamp"] = GetOrCreateBody("Hippocamp", CreateHippocampBody);
		}

		return bodies;
	}

	private BodyProto GetOrCreateBody(string name, Func<BodyProto> factory)
	{
		return _context.BodyProtos.FirstOrDefault(x => x.Name == name) ?? factory();
	}

	private BodyProto CreateHornedHumanoidBody()
	{
		var body = CreateFullCloneBody("Horned Humanoid", _organicHumanoidBody);
		AddHumanoidHorns(body);
		return body;
	}

	private BodyProto CreateWingedHumanoidBody()
	{
		var body = CreateFullCloneBody("Winged Humanoid", _organicHumanoidBody, minimumWingsToFly: 2);
		AddAvianWings(body);
		AddFlyMovement(body);
		return body;
	}

	private BodyProto CreateNagaBody()
	{
		var body = CreateFullCloneBody("Naga", _organicHumanoidBody, minimumLegsToStand: 0);
		SeederBodyUtilities.RemoveBodyparts(_context, body, ["rhip", "lhip"]);
		SeederBodyUtilities.CloneBodypartSubtree(_context, _vermiformBody, body, "ubody", "groin", cloneLimbs: false);
		AddLimb(body, "Tail", LimbType.Appendage, "ubody", "ubody", "mbody", "lbody", "tail");
		AddSwimMovement(body);
		return body;
	}

	private BodyProto CreateEasternDragonBody()
	{
		var body = CreateQuadrupedBody("Eastern Dragon");
		AddToedFeet(body);
		SeederBodyUtilities.RemoveBodyparts(_context, body, ["rhorn", "lhorn", "rwingbase", "lwingbase"]);
		return body;
	}

	private BodyProto CreateMermaidBody()
	{
		var body = CreateFullCloneBody("Mermaid", _organicHumanoidBody, minimumLegsToStand: 0);
		SeederBodyUtilities.RemoveBodyparts(_context, body, ["rhip", "lhip"]);
		SeederBodyUtilities.CloneBodypartSubtree(_context, _piscineBody, body, "peduncle", "groin");
		AddSwimMovement(body);
		return body;
	}

	private BodyProto CreateCentaurBody()
	{
		var body = CreateFullCloneBody("Centaur", _organicHumanoidBody, minimumLegsToStand: 4);
		SeederBodyUtilities.RemoveBodyparts(_context, body, ["rhip", "lhip"]);
		SeederBodyUtilities.CloneBodypartSubtree(_context, _quadrupedBody, body, "ruforeleg", "abdomen");
		SeederBodyUtilities.CloneBodypartSubtree(_context, _quadrupedBody, body, "luforeleg", "abdomen");
		SeederBodyUtilities.CloneBodypartSubtree(_context, _quadrupedBody, body, "ruhindleg", "groin");
		SeederBodyUtilities.CloneBodypartSubtree(_context, _quadrupedBody, body, "luhindleg", "groin");
		SeederBodyUtilities.CloneBodypartSubtree(_context, _quadrupedBody, body, "utail", "lback");
		AddHooves(body, includeFront: true, includeRear: true);
		return body;
	}

	private BodyProto CreateGriffinBody()
	{
		var body = CreateQuadrupedBody("Griffin");
		AddToedFeet(body);
		SeederBodyUtilities.RemoveBodyparts(_context, body, ["neck"]);
		SeederBodyUtilities.CloneBodypartSubtree(_context, _avianBody, body, "neck", "uback");
		return body;
	}

	private BodyProto CreateHippogriffBody()
	{
		var body = CreateQuadrupedBody("Hippogriff");
		AddHooves(body, includeFront: true, includeRear: true);
		SeederBodyUtilities.RemoveBodyparts(_context, body, ["neck"]);
		SeederBodyUtilities.CloneBodypartSubtree(_context, _avianBody, body, "neck", "uback");
		return body;
	}

	private BodyProto CreateManticoreBody()
	{
		var body = CreateQuadrupedBody("Manticore");
		AddToedFeet(body);
		SeederBodyUtilities.CloneBodypartSubtree(_context, _scorpionBody, body, "stinger", "ltail", cloneLimbs: false);
		AddBodypartsToLimb(body, "Tail", "stinger");
		return body;
	}

	private BodyProto CreateWyvernBody()
	{
		var body = CreateFullCloneBody("Wyvern", _avianBody);
		SeederBodyUtilities.RemoveBodyparts(_context, body, ["neck"]);
		SeederBodyUtilities.CloneBodypartSubtree(_context, _quadrupedBody, body, "neck", "uback");
		return body;
	}

	private BodyProto CreateHippocampBody()
	{
		var body = CreateQuadrupedBody("Hippocamp", minimumLegsToStand: 2);
		AddHooves(body, includeFront: true, includeRear: false);
		SeederBodyUtilities.RemoveBodyparts(_context, body, ["ruhindleg", "luhindleg"]);
		SeederBodyUtilities.CloneBodypartSubtree(_context, _piscineBody, body, "peduncle", "lback");
		AddSwimMovement(body);
		return body;
	}

	private BodyProto CreateQuadrupedBody(string name, int? minimumLegsToStand = null)
	{
		var body = CreateBodyShell(name, _quadrupedBody,
			minimumLegsToStand ?? _quadrupedBody.MinimumLegsToStand,
			_quadrupedBody.MinimumWingsToFly,
			_quadrupedBody.WielderDescriptionSingle,
			_quadrupedBody.WielderDescriptionPlural);
		SeederBodyUtilities.CloneBodyDefinition(_context, _quadrupedBody, body);
		SeederBodyUtilities.CloneBodyPositionsAndSpeeds(_context, _quadrupedBody, body);
		return body;
	}

	private BodyProto CreateFullCloneBody(string name, BodyProto source, int? minimumLegsToStand = null,
		int? minimumWingsToFly = null)
	{
		var body = CreateBodyShell(name, source,
			minimumLegsToStand ?? source.MinimumLegsToStand,
			minimumWingsToFly ?? source.MinimumWingsToFly,
			source.WielderDescriptionSingle,
			source.WielderDescriptionPlural);
		SeederBodyUtilities.CloneBodyDefinition(_context, source, body);
		SeederBodyUtilities.CloneBodyPositionsAndSpeeds(_context, source, body);
		return body;
	}

	private BodyProto CreateBodyShell(string name, BodyProto source, int minimumLegsToStand, int minimumWingsToFly,
		string wielderSingle, string wielderPlural)
	{
		var body = new BodyProto
		{
			Id = _nextBodyProtoId++,
			Name = name,
			ConsiderString = source.ConsiderString,
			WielderDescriptionSingle = wielderSingle,
			WielderDescriptionPlural = wielderPlural,
			StaminaRecoveryProgId = source.StaminaRecoveryProgId,
			MinimumLegsToStand = minimumLegsToStand,
			MinimumWingsToFly = minimumWingsToFly,
			LegDescriptionPlural = source.LegDescriptionPlural,
			LegDescriptionSingular = source.LegDescriptionSingular,
			WearSizeParameter = source.WearSizeParameter
		};
		_context.BodyProtos.Add(body);
		_context.SaveChanges();
		return body;
	}

	private void AddAvianWings(BodyProto body)
	{
		SeederBodyUtilities.CloneBodypartSubtree(_context, _avianBody, body, "rwingbase", "uback");
		SeederBodyUtilities.CloneBodypartSubtree(_context, _avianBody, body, "lwingbase", "uback");
	}

	private void AddHumanoidHorns(BodyProto body)
	{
		SeederBodyUtilities.CloneBodypartSubtree(_context, _quadrupedBody, body, "rhorn", "head", cloneLimbs: false);
		SeederBodyUtilities.CloneBodypartSubtree(_context, _quadrupedBody, body, "lhorn", "head", cloneLimbs: false);
	}

	private void AddToedFeet(BodyProto body)
	{
		SeederBodyUtilities.CloneBodypartSubtree(_context, _toedQuadrupedBody, body, "rfpaw", "rfhock", cloneLimbs: false);
		SeederBodyUtilities.CloneBodypartSubtree(_context, _toedQuadrupedBody, body, "lfpaw", "lfhock", cloneLimbs: false);
		SeederBodyUtilities.CloneBodypartSubtree(_context, _toedQuadrupedBody, body, "rrpaw", "rrhock", cloneLimbs: false);
		SeederBodyUtilities.CloneBodypartSubtree(_context, _toedQuadrupedBody, body, "lrpaw", "lrhock", cloneLimbs: false);
		AddBodypartsToLimb(body, "Right Foreleg", "rfpaw", "rfclaw");
		AddBodypartsToLimb(body, "Left Foreleg", "lfpaw", "lfclaw");
		AddBodypartsToLimb(body, "Right Hindleg", "rrpaw", "rrclaw", "rrdewclaw");
		AddBodypartsToLimb(body, "Left Hindleg", "lrpaw", "lrclaw", "lrdewclaw");
	}

	private void AddHooves(BodyProto body, bool includeFront, bool includeRear)
	{
		if (includeFront)
		{
			SeederBodyUtilities.CloneBodypartSubtree(_context, _ungulateBody, body, "rfhoof", "rfhock", cloneLimbs: false);
			SeederBodyUtilities.CloneBodypartSubtree(_context, _ungulateBody, body, "lfhoof", "lfhock", cloneLimbs: false);
			AddBodypartsToLimb(body, "Right Foreleg", "rfhoof", "rffrog");
			AddBodypartsToLimb(body, "Left Foreleg", "lfhoof", "lffrog");
		}

		if (includeRear)
		{
			SeederBodyUtilities.CloneBodypartSubtree(_context, _ungulateBody, body, "rrhoof", "rrhock", cloneLimbs: false);
			SeederBodyUtilities.CloneBodypartSubtree(_context, _ungulateBody, body, "lrhoof", "lrhock", cloneLimbs: false);
			AddBodypartsToLimb(body, "Right Hindleg", "rrhoof", "rrfrog");
			AddBodypartsToLimb(body, "Left Hindleg", "lrhoof", "lrfrog");
		}
	}

	private void AddBodypartsToLimb(BodyProto body, string limbName, params string[] aliases)
	{
		var limb = _context.Limbs.FirstOrDefault(x => x.RootBodyId == body.Id && x.Name == limbName);
		if (limb is null)
		{
			return;
		}

		var existing = _context.LimbsBodypartProto
			.Where(x => x.LimbId == limb.Id)
			.Select(x => x.BodypartProtoId)
			.ToHashSet();
		var lookup = _context.BodypartProtos
			.Where(x => x.BodyId == body.Id && aliases.Contains(x.Name))
			.ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);

		foreach (var alias in aliases)
		{
			if (!lookup.TryGetValue(alias, out var part) || existing.Contains(part.Id))
			{
				continue;
			}

			_context.LimbsBodypartProto.Add(new LimbBodypartProto
			{
				Limb = limb,
				BodypartProto = part
			});
		}

		_context.SaveChanges();
	}

	private void AddLimb(BodyProto body, string name, LimbType type, string rootAlias, params string[] aliases)
	{
		var bodyparts = _context.BodypartProtos
			.Where(x => x.BodyId == body.Id && aliases.Contains(x.Name))
			.ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
		if (!bodyparts.TryGetValue(rootAlias, out var root))
		{
			return;
		}

		var limb = new Limb
		{
			Name = name,
			LimbType = (int)type,
			RootBody = body,
			RootBodypart = root,
			LimbDamageThresholdMultiplier = 0.5,
			LimbPainThresholdMultiplier = 0.5
		};
		_context.Limbs.Add(limb);
		_context.SaveChanges();

		foreach (var alias in aliases)
		{
			if (!bodyparts.TryGetValue(alias, out var part))
			{
				continue;
			}

			_context.LimbsBodypartProto.Add(new LimbBodypartProto
			{
				Limb = limb,
				BodypartProto = part
			});
		}

		_context.SaveChanges();
	}

	private void AddFlyMovement(BodyProto body)
	{
		EnsurePosition(body, 18);
		CloneSpeedAliases(_avianBody, body, "fly", "franticfly");
	}

	private void AddSwimMovement(BodyProto body)
	{
		EnsurePosition(body, 16);
		CloneSpeedAliases(_piscineBody, body, "swim", "slowswim", "quickswim");
	}

	private void EnsurePosition(BodyProto body, int positionId)
	{
		if (_context.BodyProtosPositions.Any(x => x.BodyProtoId == body.Id && x.Position == positionId))
		{
			return;
		}

		_context.BodyProtosPositions.Add(new BodyProtosPositions
		{
			BodyProto = body,
			Position = positionId
		});
		_context.SaveChanges();
	}

	private void CloneSpeedAliases(BodyProto source, BodyProto target, params string[] aliases)
	{
		var existingAliases = _context.MoveSpeeds
			.Where(x => x.BodyProtoId == target.Id)
			.Select(x => x.Alias)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		var nextId = _context.MoveSpeeds.Select(x => x.Id).AsEnumerable().DefaultIfEmpty(0).Max() + 1;

		foreach (var speed in _context.MoveSpeeds
			         .Where(x => x.BodyProtoId == source.Id && aliases.Contains(x.Alias))
			         .OrderBy(x => x.Id)
			         .ToList())
		{
			if (existingAliases.Contains(speed.Alias))
			{
				continue;
			}

			_context.MoveSpeeds.Add(new MoveSpeed
			{
				Id = nextId++,
				BodyProto = target,
				PositionId = speed.PositionId,
				Alias = speed.Alias,
				FirstPersonVerb = speed.FirstPersonVerb,
				ThirdPersonVerb = speed.ThirdPersonVerb,
				PresentParticiple = speed.PresentParticiple,
				Multiplier = speed.Multiplier,
				StaminaMultiplier = speed.StaminaMultiplier
			});
		}

		_context.SaveChanges();
	}

	private void SeedRace(MythicalRaceTemplate template, BodyProto body)
	{
		var usesHumanoidDefaults = template.HumanoidVariety ||
		                          (template.CanUseWeapons && template.BodyKey.EqualTo("Organic Humanoid"));
		var race = new Race
		{
			Name = template.Name,
			Description = template.Description,
			BaseBody = body,
			AllowedGenders = usesHumanoidDefaults ? _organicHumanoidRace.AllowedGenders : "2 3",
			ParentRace = template.HumanoidVariety ? _organicHumanoidRace : null,
			AttributeBonusProg = _alwaysZero,
			AttributeTotalCap = _context.TraitDefinitions.Count(x => x.Type == (int)TraitType.Attribute) * 12,
			IndividualAttributeCap = 20,
			DiceExpression = "3d6+1",
			IlluminationPerceptionMultiplier = 1.0,
			AvailabilityProg = template.Playable ? _alwaysTrue : _alwaysFalse,
			CorpseModel = usesHumanoidDefaults ? _humanoidCorpse : _animalCorpse,
			DefaultHealthStrategy = _healthStrategy,
			CanUseWeapons = template.CanUseWeapons,
			CanAttack = true,
			CanDefend = true,
			NeedsToBreathe = true,
			SizeStanding = (int)template.Size,
			SizeProne = (int)template.Size,
			SizeSitting = (int)template.Size,
			CommunicationStrategyType = "humanoid",
			HandednessOptions = template.CanUseWeapons ? _organicHumanoidRace.HandednessOptions : "1 3",
			DefaultHandedness = 1,
			ChildAge = template.AgeProfile.ChildAge,
			YouthAge = template.AgeProfile.YouthAge,
			YoungAdultAge = template.AgeProfile.YoungAdultAge,
			AdultAge = template.AgeProfile.AdultAge,
			ElderAge = template.AgeProfile.ElderAge,
			VenerableAge = template.AgeProfile.VenerableAge,
			CanClimb = template.CanClimb,
			CanSwim = template.CanSwim,
			MinimumSleepingPosition = 4,
			BodypartHealthMultiplier = 1.0,
			BodypartSizeModifier = 0,
			TemperatureRangeCeiling = 40,
			TemperatureRangeFloor = 0,
			CanEatCorpses = false,
			CanEatMaterialsOptIn = false,
			BiteWeight = 1000,
			EatCorpseEmoteText = string.Empty,
			RaceUsesStamina = true,
			NaturalArmourQuality = 2,
			SweatLiquid = usesHumanoidDefaults ? _sweat : null,
			SweatRateInLitresPerMinute = usesHumanoidDefaults && _sweat is not null ? 0.5 : 0.0,
			BloodLiquid = _blood,
			BloodModel = usesHumanoidDefaults ? _humanRace.BloodModel : null,
			BreathingModel = "simple",
			BreathingVolumeExpression = "7",
			HoldBreathLengthExpression = $"90+(5*con:{_healthTrait.Id})",
			MaximumLiftWeightExpression = $"str:{_strengthTrait.Id}*10000",
			MaximumDragWeightExpression = $"str:{_strengthTrait.Id}*40000",
			DefaultHeightWeightModelMale = _context.HeightWeightModels.First(x => x.Name == template.MaleHeightWeightModel),
			DefaultHeightWeightModelFemale = _context.HeightWeightModels.First(x => x.Name == template.FemaleHeightWeightModel),
			DefaultHeightWeightModelNeuter = _context.HeightWeightModels.First(x => x.Name == template.MaleHeightWeightModel),
			DefaultHeightWeightModelNonBinary = _context.HeightWeightModels.First(x => x.Name == template.FemaleHeightWeightModel),
			NaturalArmourType = usesHumanoidDefaults ? _humanoidNaturalArmour : _animalNaturalArmour
		};

		_context.Races.Add(race);
		_context.SaveChanges();

		foreach (var attribute in _context.TraitDefinitions.Where(x => x.Type == (int)TraitType.Attribute))
		{
			_context.RacesAttributes.Add(new RacesAttributes
			{
				Race = race,
				Attribute = attribute,
				IsHealthAttribute = attribute.TraitGroup == "Physical"
			});
		}

		_context.RacesBreathableGases.Add(new RacesBreathableGases
		{
			Race = race,
			Gas = _breathableAir,
			Multiplier = 1.0
		});

		var ethnicity = SeedEthnicity(race, template);
		SeedAdditionalCharacteristics(race, ethnicity, template);
		SeedRacialBodypartUsages(race, template);
		SeedNaturalAttacks(race, template);

		if (!template.HumanoidVariety)
		{
			SeedDefaultDescriptions(race, template);
		}

		_context.SaveChanges();
	}

	private Ethnicity SeedEthnicity(Race race, MythicalRaceTemplate template)
	{
		var ethnicity = new Ethnicity
		{
			Name = $"{template.Name} Stock",
			ChargenBlurb = template.Description,
			AvailabilityProg = template.Playable ? _alwaysTrue : _alwaysFalse,
			ParentRace = race,
			EthnicGroup = template.Name,
			EthnicSubgroup = "Stock",
			PopulationBloodModel = _defaultPopulationBloodModel,
			TolerableTemperatureFloorEffect = 0,
			TolerableTemperatureCeilingEffect = 0
		};
		_context.Ethnicities.Add(ethnicity);
		_context.SaveChanges();

		if (!template.HumanoidVariety)
		{
			return ethnicity;
		}

		AddEthnicityCharacteristic(ethnicity, "Eye Colour", "All Eye Colours");
		AddEthnicityCharacteristic(ethnicity, "Eye Shape", "All Eye Shapes");
		AddEthnicityCharacteristic(ethnicity, "Nose", "All Noses");
		AddEthnicityCharacteristic(ethnicity, "Ears", "All Ears");
		AddEthnicityCharacteristic(ethnicity, "Hair Colour", "All Hair Colours");
		AddEthnicityCharacteristic(ethnicity, "Facial Hair Colour", "All Facial Hair Colours");
		AddEthnicityCharacteristic(ethnicity, "Hair Style", "All Hair Styles");
		AddEthnicityCharacteristic(ethnicity, "Facial Hair Style",
			template.FacialHairProfileName ?? "All Facial Hair Styles");
		AddEthnicityCharacteristic(ethnicity, "Skin Colour", "All Skin Colours");
		AddEthnicityCharacteristic(ethnicity, "Frame", "All Frames");
		AddEthnicityCharacteristic(ethnicity, "Person Word",
			EnsureStandardProfile(_personWordDefinition, $"{template.Name} Person Words", template.PersonWords!).Name);

		if (_context.CharacteristicDefinitions.Any(x => x.Name == "Distinctive Feature") &&
		    _context.CharacteristicProfiles.Any(x => x.Name == "All Distinctive Features"))
		{
			AddEthnicityCharacteristic(ethnicity, "Distinctive Feature", "All Distinctive Features");
		}

		return ethnicity;
	}

	private void AddEthnicityCharacteristic(Ethnicity ethnicity, string definitionName, string profileName)
	{
		var definition = _context.CharacteristicDefinitions.FirstOrDefault(x => x.Name == definitionName);
		var profile = _context.CharacteristicProfiles.FirstOrDefault(x => x.Name == profileName);
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
		_context.SaveChanges();
	}

	private void SeedAdditionalCharacteristics(Race race, Ethnicity ethnicity, MythicalRaceTemplate template)
	{
		if (template.AdditionalCharacteristics is null)
		{
			return;
		}

		foreach (var characteristic in template.AdditionalCharacteristics)
		{
			var definition = EnsureCharacteristicDefinition(characteristic.DefinitionName,
				$"{characteristic.DefinitionName} for {template.Name}",
				characteristic.Type);
			var profile = EnsureStandardProfile(definition, $"All {characteristic.DefinitionName}", characteristic.Values);

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

		_context.SaveChanges();
	}

	private CharacteristicDefinition EnsureCharacteristicDefinition(string name, string description,
		CharacteristicType type = CharacteristicType.Standard)
	{
		var existing = _context.CharacteristicDefinitions.FirstOrDefault(x => x.Name == name);
		if (existing is not null)
		{
			return existing;
		}

		var definition = new CharacteristicDefinition
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
		var existing = _context.CharacteristicProfiles.FirstOrDefault(x => x.Name == profileName);
		if (existing is not null)
		{
			return existing;
		}

		var valueList = values.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
		foreach (var value in valueList)
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

		var definitionXml = new StringBuilder();
		definitionXml.Append("<Values>");
		foreach (var value in valueList)
		{
			definitionXml.Append($" <Value>{value}</Value>");
		}

		definitionXml.Append(" </Values>");

		var profile = new CharacteristicProfile
		{
			Name = profileName,
			TargetDefinition = definition,
			Type = "Standard",
			Description = $"Auto-generated profile for {profileName}",
			Definition = definitionXml.ToString()
		};
		_context.CharacteristicProfiles.Add(profile);
		_context.SaveChanges();
		return profile;
	}

	private void SeedRacialBodypartUsages(Race race, MythicalRaceTemplate template)
	{
		if (template.BodypartUsages is null)
		{
			return;
		}

		foreach (var usage in template.BodypartUsages)
		{
			var bodypart = FindBodypartOnBody(race.BaseBody, usage.BodypartAlias);
			if (bodypart is null)
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

		_context.SaveChanges();
	}

	private void SeedNaturalAttacks(Race race, MythicalRaceTemplate template)
	{
		foreach (var attackTemplate in template.Attacks)
		{
			var attack = _context.WeaponAttacks.First(x => x.Name == attackTemplate.AttackName);
			foreach (var alias in attackTemplate.BodypartAliases)
			{
				var bodypart = FindBodypartOnBody(race.BaseBody, alias);
				if (bodypart is null)
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

		_context.SaveChanges();
	}

	private void SeedDefaultDescriptions(Race race, MythicalRaceTemplate template)
	{
		if (template.ShortDescriptionPattern is null || template.FullDescriptionPattern is null)
		{
			return;
		}

		var prog = new FutureProg
		{
			FunctionName = $"Is{race.Name.CollapseString()}",
			Category = "Character",
			Subcategory = "Descriptions",
			FunctionComment = $"True if the character is a {race.Name}",
			ReturnType = (long)ProgVariableTypes.Boolean,
			StaticType = 0,
			AcceptsAnyParameters = false,
			Public = true,
			FunctionText = $"return @ch.Race == ToRace(\"{race.Name}\")"
		};
		prog.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = prog,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterType = (long)ProgVariableTypes.Toon
		});
		_context.FutureProgs.Add(prog);
		_context.SaveChanges();

		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0,
			ApplicabilityProg = prog,
			RelativeWeight = 100,
			Pattern = template.ShortDescriptionPattern
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 1,
			ApplicabilityProg = prog,
			RelativeWeight = 100,
			Pattern = template.FullDescriptionPattern
		});
	}

	private BodypartProto? FindBodypartOnBody(BodyProto body, string alias)
	{
		var bodyIds = new List<long> { body.Id };
		var counted = body.CountsAs;
		while (counted is not null)
		{
			bodyIds.Add(counted.Id);
			counted = counted.CountsAs;
		}

		return _context.BodypartProtos
			.Where(x => bodyIds.Contains(x.BodyId) && x.Name == alias)
			.OrderBy(x => x.BodyId == body.Id ? 0 : 1)
			.FirstOrDefault();
	}
}
