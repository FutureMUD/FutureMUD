#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Health;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class RobotSeeder : IDatabaseSeeder
{
	private static readonly string[] RequiredToolTags =
	[
		"Arterial Clamp",
		"Bonesaw",
		"Forceps",
		"Scalpel",
		"Surgical Suture Needle"
	];

	private static readonly string[] RequiredPrerequisiteAttackNames =
	[
		"Jab",
		"Cross",
		"Hook",
		"Elbow",
		"Bite",
		"Snap Kick",
		"Carnivore Bite",
		"Claw Low Swipe",
		"Claw High Swipe",
		"Mandible Bite",
		"Animal Barge",
		"Hoof Stomp Smash"
	];
	private static readonly HashSet<string> AvianDependentBodyKeys = new(StringComparer.OrdinalIgnoreCase)
	{
		"Winged Robot",
		"Jet Robot"
	};

	private readonly Dictionary<string, Tag> _tags = new(StringComparer.OrdinalIgnoreCase);
	private readonly Dictionary<string, MudSharp.Models.Knowledge> _knowledges = new(StringComparer.OrdinalIgnoreCase);
	private readonly Dictionary<string, MudSharp.Models.SurgicalProcedure> _procedures = new(StringComparer.OrdinalIgnoreCase);

	private FuturemudDatabaseContext _context = null!;
	private Race _humanRace = null!;
	private Race _humanoidRace = null!;
	private BodyProto _humanoidBody = null!;
	private BodyProto? _avianBody;
	private BodyProto _toedQuadrupedBody = null!;
	private BodyProto _insectoidBody = null!;
	private BodyProto _arachnidBody = null!;
	private FutureProg _alwaysTrue = null!;
	private FutureProg _alwaysFalse = null!;
	private TraitDefinition _healthTrait = null!;
	private TraitDefinition _secondaryTrait = null!;
	private TraitDefinition _strengthTrait = null!;
	private CorpseModel _organicHumanCorpse = null!;
	private CorpseModel _organicAnimalCorpse = null!;
	private Liquid _blood = null!;
	private Material _chassisAlloy = null!;
	private Material _circuitryMaterial = null!;
	private Material _hydraulicResidue = null!;
	private Material _oilResidue = null!;
	private Liquid _hydraulicFluid = null!;
	private Liquid _machineOil = null!;
	private ArmourType _robotPlatingArmour = null!;
	private ArmourType _robotLightPlatingArmour = null!;
	private ArmourType _robotInternalArmour = null!;
	private CorpseModel _robotHumanoidCorpse = null!;
	private CorpseModel _robotAnimalCorpse = null!;
	private FutureProg _robotStaminaRecoveryProg = null!;
	private HealthStrategy _robotArticulatedStrategy = null!;
	private HealthStrategy _robotUtilityStrategy = null!;

	private sealed class RobotSeedSummary
	{
		public int BodiesAdded { get; set; }
		public int RacesAdded { get; set; }
		public int ProceduresAdded { get; set; }
	}

	public IEnumerable<(string Id, string Question,
		Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
		Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions =>
		Array.Empty<(string Id, string Question, Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter, Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)>();

	public int SortOrder => 305;
	public string Name => "Robot Seeder";
	public string Tagline => "Installs robotic races, chassis bodies, and robot maintenance content";
	public string FullDescription =>
		"Seeds a rerunnable catalogue of robotic humanoids, utility robots, robot beasts, robot health strategies, and robot maintenance procedures. This package reuses existing humanoid and selected animal body layouts so robotic races remain compatible with stock equipment where appropriate.";
	public bool SafeToRunMoreThanOnce => true;

	public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{
		_context = context;
		LoadSharedData();
		RefreshToolTags();

		var summary = new RobotSeedSummary();
		_context.Database.BeginTransaction();

		SeedSharedRobotContent();
		var bodyCatalogue = EnsureBodyCatalogue(summary);
		SeedKnowledges();
		SeedRaces(bodyCatalogue, summary);
		SeedRobotProcedures(bodyCatalogue, summary);

		_context.SaveChanges();
		_context.Database.CommitTransaction();

		return
			$"Successfully installed or updated robot content. Added {summary.BodiesAdded} bodies, {summary.RacesAdded} races, and {summary.ProceduresAdded} procedures.";
	}

	public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
	{
		if (!HasPrerequisites(context))
		{
			return ShouldSeedResult.PrerequisitesNotMet;
		}

		var requiredBodies = new[]
		{
			"Robot Humanoid",
			"Robot Quadruped",
			"Robot Insectoid",
			"Robot Utility"
		};
		var requiredStrategies = new[] { "Robot Articulated Model", "Robot Utility Construct" };
		var requiredKnowledges = RobotKnowledgeTemplates.Select(x => x.Name).ToArray();
		var requiredProcedures = new[]
		{
			"Robot Diagnostics (Humanoid)",
			"Robot Diagnostics (Quadruped)",
			"Robot Diagnostics (Insectoid)",
			"Robot Diagnostics (Utility)"
		};

		var availableBodyNames = context.BodyProtos.Select(x => x.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
		var installableTemplates = Templates.Values
			.Where(template => CanSupportBodyKey(availableBodyNames, template.BodyKey))
			.ToList();

		if (installableTemplates.All(template => context.Races.Any(x => x.Name == template.Name)) &&
		    requiredBodies.All(name => context.BodyProtos.Any(x => x.Name == name)) &&
		    requiredStrategies.All(name => context.HealthStrategies.Any(x => x.Name == name)) &&
		    requiredKnowledges.All(name => context.Knowledges.Any(x => x.Name == name)) &&
		    requiredProcedures.All(name => context.SurgicalProcedures.Any(x => x.Name == name)))
		{
			return ShouldSeedResult.MayAlreadyBeInstalled;
		}

		return ShouldSeedResult.ReadyToInstall;
	}

	private static bool HasPrerequisites(FuturemudDatabaseContext context)
	{
		var requiredBodies = new[] { "Humanoid", "Toed Quadruped", "Insectoid", "Arachnid" };
		var requiredRaces = new[] { "Human", "Humanoid" };
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
		var requiredProgs = new[] { "AlwaysTrue", "AlwaysFalse" };
		var hasBodies = requiredBodies.All(body => context.BodyProtos.Any(x => x.Name == body));
		var hasRaces = requiredRaces.All(race => context.Races.Any(x => x.Name == race));
		var hasRequiredProfiles = requiredProfiles.All(profile =>
				   context.CharacteristicProfiles.Any(x => x.Name == profile) ||
				   context.CharacteristicDefinitions.Any(x => x.Name == profile));
		var hasProgs = requiredProgs.All(prog => context.FutureProgs.Any(x => x.FunctionName == prog));
		var hasTools = RequiredToolTags.All(tag => context.Tags.Any(x => x.Name == tag));
		var hasAttacks = RequiredPrerequisiteAttackNames.All(attack => context.WeaponAttacks.Any(x => x.Name == attack));

		return hasBodies &&
		       hasRaces &&
		       hasRequiredProfiles &&
		       hasProgs &&
		       hasTools &&
		       hasAttacks &&
		       context.CorpseModels.Any(x => x.Name == "Organic Human Corpse") &&
		       context.CorpseModels.Any(x => x.Name == "Organic Animal Corpse");
	}

	private void LoadSharedData()
	{
		_humanRace = _context.Races.First(x => x.Name == "Human");
		_humanoidRace = _context.Races.First(x => x.Name == "Humanoid");
		_humanoidBody = _context.BodyProtos.First(x => x.Name == "Humanoid");
		_avianBody = _context.BodyProtos.FirstOrDefault(x => x.Name == "Avian");
		_toedQuadrupedBody = _context.BodyProtos.First(x => x.Name == "Toed Quadruped");
		_insectoidBody = _context.BodyProtos.First(x => x.Name == "Insectoid");
		_arachnidBody = _context.BodyProtos.First(x => x.Name == "Arachnid");
		_alwaysTrue = _context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue");
		_alwaysFalse = _context.FutureProgs.First(x => x.FunctionName == "AlwaysFalse");
		_organicHumanCorpse = _context.CorpseModels.First(x => x.Name == "Organic Human Corpse");
		_organicAnimalCorpse = _context.CorpseModels.First(x => x.Name == "Organic Animal Corpse");
		_blood = _context.Liquids.First(x => x.Name == "blood");

		_healthTrait = _context.TraitDefinitions
			.Where(x => x.Type == (int)TraitType.Attribute)
			.AsEnumerable()
			.First(x => x.Name.In("Constitution", "Body", "Physique", "Endurance", "Hardiness", "Stamina"));
		_secondaryTrait = _context.TraitDefinitions
			                  .Where(x => x.Type == (int)TraitType.Attribute)
			                  .AsEnumerable()
			                  .FirstOrDefault(x => x.Name.In("Willpower", "Resilience", "Mind")) ??
		                  _healthTrait;
		_strengthTrait = _context.TraitDefinitions
			.Where(x => x.Type == (int)TraitType.Attribute)
			.AsEnumerable()
			.First(x => x.Name.In("Strength", "Physique", "Body", "Upper Body Strength"));
	}

	private void RefreshToolTags()
	{
		_tags.Clear();
		foreach (var tag in _context.Tags.ToList())
		{
			_tags[tag.Name] = tag;
		}
	}

	internal static bool CanSupportBodyKeyForTesting(IReadOnlyCollection<string> availableBodyNames, string bodyKey)
	{
		return CanSupportBodyKey(availableBodyNames, bodyKey);
	}

	private static bool CanSupportBodyKey(IReadOnlyCollection<string> availableBodyNames, string bodyKey)
	{
		return !AvianDependentBodyKeys.Contains(bodyKey) ||
		       availableBodyNames.Contains("Avian") ||
		       availableBodyNames.Contains(bodyKey);
	}
}
