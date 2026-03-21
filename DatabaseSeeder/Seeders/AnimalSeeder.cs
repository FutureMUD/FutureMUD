using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public partial class AnimalSeeder : IDatabaseSeeder
{
	private readonly Dictionary<string, WeaponAttack> _attacks =
		new(StringComparer.OrdinalIgnoreCase);

	private readonly Dictionary<string, BodypartProto> _cachedBodyparts =
		new(StringComparer.OrdinalIgnoreCase);

	private readonly List<(BodypartProto Child, BodypartProto Parent)> _cachedBodypartUpstreams =
		new();

	private readonly Dictionary<string, BodypartProto> _cachedBones =
		new(StringComparer.OrdinalIgnoreCase);

	private readonly CollectionDictionary<string, BodypartProto> _cachedLimbs =
		new(StringComparer.OrdinalIgnoreCase);

	private readonly Dictionary<string, Material> _cachedMaterials =
		new(StringComparer.OrdinalIgnoreCase);

	private readonly Dictionary<string, BodypartProto> _cachedOrgans =
		new(StringComparer.OrdinalIgnoreCase);

	private readonly Dictionary<string, BodypartShape> _cachedShapes =
		new(StringComparer.OrdinalIgnoreCase);

	private readonly List<Liquid> _freshWaters = new();

	private readonly Dictionary<string, HeightWeightModel> _hwModels = new(StringComparer.OrdinalIgnoreCase);

	private FutureProg _alwaysFalse;
	private FutureProg _alwaysTrue;
	private IEnumerable<TraitDefinition> _attributes;
	private Liquid _bloodLiquid;
	private ArmourType _boneArmour;

	private Liquid _brackishWater;

	private Gas _breathableAir;
	private FuturemudDatabaseContext _context;

	private CorpseModel _defaultCorpseModel;
	private TraitDefinition _dexterityTrait;
	private HealthStrategy _healthStrategy;
	private TraitDefinition _healthTrait;
	private TraitDefinition _intelligenceTrait;

	private ArmourType _naturalArmour;
	private ArmourType _organArmour;
	private IReadOnlyDictionary<string, string> _questionAnswers;
	private Liquid _saltWater;
	private bool _sever;
	private TraitExpression _snakeBiteDamage;

	private readonly Stopwatch _stopwatch = new();
	private TraitDefinition _strengthTrait;
	private Liquid _sweatLiquid;

	public IEnumerable<(string Id, string Question,
		Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
		Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions
		=> NonHumanSeederQuestions.GetQuestions();

	#region Core Methods
	public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{
		context.Database.BeginTransaction();
		_context = context;
		Console.WriteLine("Performing initial setup...");
		_stopwatch.Start();
		ResetSeeder();
		SetupCorpseModel();
		SeedCombatStrategies();
		_questionAnswers = CombatSeederMessageStyleHelper.MergeQuestionAnswersWithRecordedChoice(context, questionAnswers);
		_sever = _questionAnswers.ContainsKey("sever") && _questionAnswers["sever"].ToLowerInvariant().In("yes", "y");

		var firsttime = _context.TraitExpressions.All(x => x.Name != "Non-Human Max HP Formula");

		#region Health Strategy

		_healthTrait = context.TraitDefinitions
			.Where(x => x.Type == 1)
			.AsEnumerable()
			.First(x => x.Name.In("Constitution", "Body", "Physique", "Endurance", "Hardiness", "Stamina"));

		var hpExpression = new TraitExpression
		{
			Name = "Non-Human Max HP Formula",
			Expression = $"100+con:{_healthTrait.Id}"
		};
		context.TraitExpressions.Add(hpExpression);
		var hpTick = new TraitExpression
		{
			Name = "Non-Human HP Heal Per Tick",
			Expression = $"100+con:{_healthTrait.Id}"
		};
		context.TraitExpressions.Add(hpTick);
		var secondaryTrait = context.TraitDefinitions
								 .Where(x => x.Type == 1)
								 .AsEnumerable()
								 .FirstOrDefault(x => x.Name.In("Willpower", "Resilience", "Mind")) ??
							 _healthTrait;

		_strengthTrait = context.TraitDefinitions
			.Where(x => x.Type == 1)
			.AsEnumerable()
			.First(x => x.Name.In("Strength", "Physique", "Body", "Upper Body Strength"));
		context.SaveChanges();

		var strategy = SetupHealthModel(context, _questionAnswers, hpExpression, hpTick, secondaryTrait);

		#endregion

		// Progs
		var staminaRecoveryProg = new FutureProg
		{
			FunctionName = "NonHumanStaminaRecovery",
			Category = "Character",
			Subcategory = "Stamina",
			FunctionComment = "Determines the stamina gain per 10 seconds for non-humans",
			ReturnType = 2,
			AcceptsAnyParameters = false,
			Public = true,
			StaticType = 0,
			FunctionText = $"return GetTrait(@ch, ToTrait({_healthTrait.Id}))"
		};
		staminaRecoveryProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = staminaRecoveryProg,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterType = 8200
		});
		context.FutureProgs.Add(staminaRecoveryProg);
		context.SaveChanges();

		// Cultures
		var animalCulture = new Culture
		{
			Name = "Animal",
			Description = AnimalCultureDescriptionForTesting,
			PersonWordMale = "Animal",
			PersonWordFemale = "Animal",
			PersonWordIndeterminate = "Animal",
			PersonWordNeuter = "Animal",
			SkillStartingValueProg = context.FutureProgs.First(x => x.FunctionName == "AlwaysZero"),
			AvailabilityProg = context.FutureProgs.First(x => x.FunctionName == "AlwaysFalse"),
			TolerableTemperatureCeilingEffect = 0,
			TolerableTemperatureFloorEffect = 0,
			PrimaryCalendarId = context.Calendars.First().Id
		};
		context.Cultures.Add(animalCulture);
		var simpleNC = context.NameCultures.First(x => x.Name == "Simple");
		animalCulture.CulturesNameCultures.Add(new CulturesNameCultures
		{ Culture = animalCulture, NameCulture = simpleNC, Gender = (short)Gender.Male });
		animalCulture.CulturesNameCultures.Add(new CulturesNameCultures
		{ Culture = animalCulture, NameCulture = simpleNC, Gender = (short)Gender.Female });
		animalCulture.CulturesNameCultures.Add(new CulturesNameCultures
		{ Culture = animalCulture, NameCulture = simpleNC, Gender = (short)Gender.Neuter });
		animalCulture.CulturesNameCultures.Add(new CulturesNameCultures
		{ Culture = animalCulture, NameCulture = simpleNC, Gender = (short)Gender.NonBinary });
		animalCulture.CulturesNameCultures.Add(new CulturesNameCultures
		{ Culture = animalCulture, NameCulture = simpleNC, Gender = (short)Gender.Indeterminate });

		context.SaveChanges();

		SetupHeightWeightModels();

		// Body Prototypes
		var wearSize = new WearableSizeParameterRule
		{
			BodyProtoId = 0,
			MinHeightFactor = 0.5,
			MaxHeightFactor = 1.5,
			MinWeightFactor = 0.5,
			MaxWeightFactor = 1.5,
			IgnoreTrait = true,
			WeightVolumeRatios =
				"<Ratios><Ratio Item=\"5\" Min=\"0\" Max=\"0.75\"/><Ratio Item=\"4\" Min=\"0.75\" Max=\"0.95\"/><Ratio Item=\"3\" Min=\"0.95\" Max=\"0.99\"/><Ratio Item=\"2\" Min=\"0.99\" Max=\"1.01\"/><Ratio Item=\"3\" Min=\"1.01\" Max=\"1.05\"/><Ratio Item=\"1\" Min=\"1.05\" Max=\"1.3\"/><Ratio Item=\"0\" Min=\"1.3\" Max=\"1000\"/></Ratios>",
			TraitVolumeRatios =
				"<Ratios><Ratio Item=\"5\" Min=\"0\" Max=\"0.75\"/><Ratio Item=\"4\" Min=\"0.75\" Max=\"0.95\"/><Ratio Item=\"3\" Min=\"0.95\" Max=\"0.99\"/><Ratio Item=\"2\" Min=\"0.99\" Max=\"1.01\"/><Ratio Item=\"3\" Min=\"1.01\" Max=\"1.05\"/><Ratio Item=\"1\" Min=\"1.05\" Max=\"1.3\"/><Ratio Item=\"0\" Min=\"1.3\" Max=\"1000\"/></Ratios>",
			HeightLinearRatios =
				"<Ratios><Ratio Item=\"0\" Min=\"0\" Max=\"0.90\"/><Ratio Item=\"1\" Min=\"0.90\" Max=\"0.95\"/><Ratio Item=\"2\" Min=\"0.95\" Max=\"0.99\"/><Ratio Item=\"3\" Min=\"0.99\" Max=\"1.01\"/><Ratio Item=\"2\" Min=\"1.01\" Max=\"1.05\"/><Ratio Item=\"4\" Min=\"1.05\" Max=\"1.1\"/><Ratio Item=\"5\" Min=\"1.1\" Max=\"1000\"/></Ratios>"
		};
		context.WearableSizeParameterRule.Add(wearSize);
		context.SaveChanges();

		SetupArmourTypes();
		SetupShapesAndMaterials();
		SetupAttacks(firsttime);

		var nextId = context.BodyProtos.Select(x => x.Id).AsEnumerable().DefaultIfEmpty(0).Max() + 1;
		Console.WriteLine("Installing quadrupeds...");
		var quadrupedBody = new BodyProto
		{
			Id = nextId++,
			Name = "Quadruped Base",
			ConsiderString = "",
			WielderDescriptionSingle = "mouth",
			WielderDescriptionPlural = "mouths",
			StaminaRecoveryProgId = staminaRecoveryProg.Id,
			MinimumLegsToStand = 3,
			MinimumWingsToFly = 2,
			LegDescriptionPlural = "legs",
			LegDescriptionSingular = "leg",
			WearSizeParameter = wearSize
		};
		context.BodyProtos.Add(quadrupedBody);
		context.SaveChanges();

		var ungulateBody = new BodyProto
		{
			Id = nextId++,
			CountsAs = quadrupedBody,
			Name = "Ungulate",
			ConsiderString = "",
			WielderDescriptionSingle = "mouth",
			WielderDescriptionPlural = "mouths",
			StaminaRecoveryProgId = staminaRecoveryProg.Id,
			MinimumLegsToStand = 3,
			MinimumWingsToFly = 2,
			LegDescriptionPlural = "legs",
			LegDescriptionSingular = "leg",
			WearSizeParameter = wearSize
		};
		context.BodyProtos.Add(ungulateBody);
		context.SaveChanges();

		var toedQuadruped = new BodyProto
		{
			Id = nextId++,
			CountsAs = quadrupedBody,
			Name = "Toed Quadruped",
			ConsiderString = "",
			WielderDescriptionSingle = "mouth",
			WielderDescriptionPlural = "mouths",
			StaminaRecoveryProgId = staminaRecoveryProg.Id,
			MinimumLegsToStand = 3,
			MinimumWingsToFly = 2,
			LegDescriptionPlural = "legs",
			LegDescriptionSingular = "leg",
			WearSizeParameter = wearSize
		};
		context.BodyProtos.Add(toedQuadruped);
		context.SaveChanges();

		SeedQuadruped(quadrupedBody, ungulateBody, toedQuadruped);

		Console.WriteLine("Installing avians...");
		var avianBody = new BodyProto
		{
			Id = nextId++,
			Name = "Avian",
			ConsiderString = "",
			WielderDescriptionSingle = "talon",
			WielderDescriptionPlural = "talons",
			StaminaRecoveryProgId = staminaRecoveryProg.Id,
			MinimumLegsToStand = 2,
			MinimumWingsToFly = 2,
			LegDescriptionPlural = "legs",
			LegDescriptionSingular = "leg",
			WearSizeParameter = wearSize
		};
		context.BodyProtos.Add(avianBody);
		context.SaveChanges();
		SeedAvian(avianBody);

		Console.WriteLine("Installing serpents...");
		var wormBody = new BodyProto
		{
			Id = nextId++,
			Name = "Vermiform",
			ConsiderString = "",
			WielderDescriptionSingle = "mouth",
			WielderDescriptionPlural = "mouths",
			StaminaRecoveryProgId = staminaRecoveryProg.Id,
			MinimumLegsToStand = 3,
			MinimumWingsToFly = 2,
			LegDescriptionPlural = "legs",
			LegDescriptionSingular = "leg",
			WearSizeParameter = wearSize
		};
		context.BodyProtos.Add(wormBody);
		context.SaveChanges();

		var serpentineBody = new BodyProto
		{
			Id = nextId++,
			CountsAs = wormBody,
			Name = "Serpentine",
			ConsiderString = "",
			WielderDescriptionSingle = "mouth",
			WielderDescriptionPlural = "mouths",
			StaminaRecoveryProgId = staminaRecoveryProg.Id,
			MinimumLegsToStand = 3,
			MinimumWingsToFly = 2,
			LegDescriptionPlural = "legs",
			LegDescriptionSingular = "leg",
			WearSizeParameter = wearSize
		};
		context.BodyProtos.Add(serpentineBody);
		context.SaveChanges();
		SeedSerpents(wormBody, serpentineBody);

		Console.WriteLine("Installing aquatic animals...");
		var fishBody = new BodyProto
		{
			Id = nextId++,
			Name = "Piscine",
			ConsiderString = "",
			WielderDescriptionSingle = "mouth",
			WielderDescriptionPlural = "mouths",
			StaminaRecoveryProgId = staminaRecoveryProg.Id,
			MinimumLegsToStand = 3,
			MinimumWingsToFly = 2,
			LegDescriptionPlural = "legs",
			LegDescriptionSingular = "leg",
			WearSizeParameter = wearSize
		};
		context.BodyProtos.Add(fishBody);
		context.SaveChanges();

		var crabBody = new BodyProto
		{
			Id = nextId++,
			Name = "Decapod",
			ConsiderString = "",
			WielderDescriptionSingle = "pincer",
			WielderDescriptionPlural = "pincers",
			StaminaRecoveryProgId = staminaRecoveryProg.Id,
			MinimumLegsToStand = 5,
			MinimumWingsToFly = 2,
			LegDescriptionPlural = "legs",
			LegDescriptionSingular = "leg",
			WearSizeParameter = wearSize
		};
		context.BodyProtos.Add(crabBody);
		context.SaveChanges();

		var malacostracanBody = new BodyProto
		{
			Id = nextId++,
			CountsAs = crabBody,
			Name = "Malacostracan",
			ConsiderString = "",
			WielderDescriptionSingle = "claw",
			WielderDescriptionPlural = "claws",
			StaminaRecoveryProgId = staminaRecoveryProg.Id,
			MinimumLegsToStand = 4,
			MinimumWingsToFly = 2,
			LegDescriptionPlural = "legs",
			LegDescriptionSingular = "leg",
			WearSizeParameter = wearSize
		};
		context.BodyProtos.Add(malacostracanBody);
		context.SaveChanges();

		var cephalopod = new BodyProto
		{
			Id = nextId++,
			Name = "Cephalopod",
			ConsiderString = "",
			WielderDescriptionSingle = "tentacle",
			WielderDescriptionPlural = "tentacles",
			StaminaRecoveryProgId = staminaRecoveryProg.Id,
			MinimumLegsToStand = 3,
			MinimumWingsToFly = 2,
			LegDescriptionPlural = "tentacle",
			LegDescriptionSingular = "tentacles",
			WearSizeParameter = wearSize
		};
		context.BodyProtos.Add(cephalopod);
		context.SaveChanges();

		var jellyfish = new BodyProto
		{
			Id = nextId++,
			Name = "Jellyfish",
			ConsiderString = "",
			WielderDescriptionSingle = "tentacle",
			WielderDescriptionPlural = "tentacles",
			StaminaRecoveryProgId = staminaRecoveryProg.Id,
			MinimumLegsToStand = 3,
			MinimumWingsToFly = 2,
			LegDescriptionPlural = "legs",
			LegDescriptionSingular = "leg",
			WearSizeParameter = wearSize
		};
		context.BodyProtos.Add(jellyfish);
		context.SaveChanges();

		var pinniped = new BodyProto
		{
			Id = nextId++,
			Name = "Pinniped",
			ConsiderString = "",
			WielderDescriptionSingle = "mouth",
			WielderDescriptionPlural = "mouths",
			StaminaRecoveryProgId = staminaRecoveryProg.Id,
			MinimumLegsToStand = 3,
			MinimumWingsToFly = 2,
			LegDescriptionPlural = "legs",
			LegDescriptionSingular = "leg",
			WearSizeParameter = wearSize
		};
		context.BodyProtos.Add(pinniped);
		context.SaveChanges();

		var cetacean = new BodyProto
		{
			Id = nextId++,
			Name = "Cetacean",
			ConsiderString = "",
			WielderDescriptionSingle = "mouth",
			WielderDescriptionPlural = "mouths",
			StaminaRecoveryProgId = staminaRecoveryProg.Id,
			MinimumLegsToStand = 3,
			MinimumWingsToFly = 2,
			LegDescriptionPlural = "legs",
			LegDescriptionSingular = "leg",
			WearSizeParameter = wearSize
		};
		context.BodyProtos.Add(cetacean);
		context.SaveChanges();

		SeedAquatic(fishBody, crabBody, malacostracanBody, cephalopod, jellyfish, pinniped, cetacean);

		Console.WriteLine("Installing insectoids...");
		var insectBody = new BodyProto
		{
			Id = nextId++,
			Name = "Insectoid",
			ConsiderString = "",
			WielderDescriptionSingle = "mandible",
			WielderDescriptionPlural = "mandibles",
			StaminaRecoveryProgId = staminaRecoveryProg.Id,
			MinimumLegsToStand = 6,
			MinimumWingsToFly = 2,
			LegDescriptionPlural = "legs",
			LegDescriptionSingular = "leg",
			WearSizeParameter = wearSize
		};
		context.BodyProtos.Add(insectBody);
		context.SaveChanges();

		var wingedInsectBody = new BodyProto
		{
			Id = nextId++,
			Name = "Winged Insectoid",
			ConsiderString = "",
			WielderDescriptionSingle = "mandible",
			WielderDescriptionPlural = "mandibles",
			StaminaRecoveryProgId = staminaRecoveryProg.Id,
			MinimumLegsToStand = 6,
			MinimumWingsToFly = 2,
			LegDescriptionPlural = "legs",
			LegDescriptionSingular = "leg",
			WearSizeParameter = wearSize
		};
		context.BodyProtos.Add(wingedInsectBody);
		context.SaveChanges();

		SeedInsectoid(insectBody);
		SeedWingedInsectoid(wingedInsectBody);

		var arachnidBody = new BodyProto
		{
			Id = nextId++,
			CountsAs = insectBody,
			Name = "Arachnid",
			ConsiderString = "",
			WielderDescriptionSingle = "fang",
			WielderDescriptionPlural = "fangs",
			StaminaRecoveryProgId = staminaRecoveryProg.Id,
			MinimumLegsToStand = 6,
			MinimumWingsToFly = 2,
			LegDescriptionPlural = "legs",
			LegDescriptionSingular = "leg",
			WearSizeParameter = wearSize
		};
		context.BodyProtos.Add(arachnidBody);
		context.SaveChanges();

		var scorpionBody = new BodyProto
		{
			Id = nextId++,
			CountsAs = arachnidBody,
			Name = "Scorpion",
			ConsiderString = "",
			WielderDescriptionSingle = "claw",
			WielderDescriptionPlural = "claws",
			StaminaRecoveryProgId = staminaRecoveryProg.Id,
			MinimumLegsToStand = 6,
			MinimumWingsToFly = 2,
			LegDescriptionPlural = "legs",
			LegDescriptionSingular = "leg",
			WearSizeParameter = wearSize
		};
		context.BodyProtos.Add(scorpionBody);
		context.SaveChanges();

		SeedArachnidBody(arachnidBody, false);
		Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Races...");
		SeedAnimalRaces(GetArachnidRaceTemplates().Where(x => x.BodyKey == "Arachnid"),
			("Arachnid", arachnidBody));

		SeedArachnidBody(scorpionBody, true);
		Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Races...");
		SeedAnimalRaces(GetArachnidRaceTemplates().Where(x => x.BodyKey == "Scorpion"),
			("Scorpion", scorpionBody));

		var reptilianBody = new BodyProto
		{
			Id = nextId++,
			CountsAs = toedQuadruped,
			Name = "Reptilian",
			ConsiderString = "",
			WielderDescriptionSingle = "mouth",
			WielderDescriptionPlural = "mouths",
			StaminaRecoveryProgId = staminaRecoveryProg.Id,
			MinimumLegsToStand = 3,
			MinimumWingsToFly = 2,
			LegDescriptionPlural = "legs",
			LegDescriptionSingular = "leg",
			WearSizeParameter = wearSize
		};
		context.BodyProtos.Add(reptilianBody);
		context.SaveChanges();

		var anuranBody = new BodyProto
		{
			Id = nextId++,
			CountsAs = reptilianBody,
			Name = "Anuran",
			ConsiderString = "",
			WielderDescriptionSingle = "mouth",
			WielderDescriptionPlural = "mouths",
			StaminaRecoveryProgId = staminaRecoveryProg.Id,
			MinimumLegsToStand = 2,
			MinimumWingsToFly = 2,
			LegDescriptionPlural = "legs",
			LegDescriptionSingular = "leg",
			WearSizeParameter = wearSize
		};
		context.BodyProtos.Add(anuranBody);
		context.SaveChanges();

		SeedReptilianBodies(toedQuadruped, reptilianBody, anuranBody);
		Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Races...");
		SeedAnimalRaces(GetReptileAmphibianRaceTemplates(),
			("Reptilian", reptilianBody),
			("Anuran", anuranBody));

		SetupPositions(quadrupedBody, avianBody, serpentineBody, fishBody, crabBody, cephalopod, jellyfish, pinniped,
				cetacean, wormBody, insectBody, wingedInsectBody);
		SetupSpeeds(quadrupedBody, avianBody, serpentineBody, fishBody, crabBody, cephalopod, jellyfish, pinniped,
				cetacean, wormBody, insectBody, wingedInsectBody);
		CloneBodyPositionsAndSpeeds(crabBody, malacostracanBody);
		CloneBodyPositionsAndSpeeds(insectBody, arachnidBody);
		CloneBodyPositionsAndSpeeds(insectBody, scorpionBody);
		CloneBodyPositionsAndSpeeds(toedQuadruped, reptilianBody);
		CloneBodyPositionsAndSpeeds(toedQuadruped, anuranBody);
		ApplyDefaultCombatSettingsToSeededRaces();

		context.Database.CommitTransaction();

		return "Successfully installed animal prototypes";
	}

	public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
	{
		if (!context.BodyProtos.Any(x => x.Name == "Humanoid") || !context.NameCultures.Any(x => x.Name == "Simple"))
			return ShouldSeedResult.PrerequisitesNotMet;

		if (context.BodyProtos.Any(x => x.Name == "Quadruped Base")) return ShouldSeedResult.MayAlreadyBeInstalled;

		return ShouldSeedResult.ReadyToInstall;
	}
	private void ResetSeeder()
	{
		_cachedBodyparts.Clear();
		_cachedBodypartUpstreams.Clear();
		_cachedLimbs.Clear();
		_cachedOrgans.Clear();
		_cachedBones.Clear();
		_cachedMaterials.Clear();
		_cachedShapes.Clear();
		_sever = false;
		_breathableAir = _context.Gases.First();
		// TODO - make these optional
		_saltWater = _context.Liquids.First(x => x.Name == "salt water");
		_brackishWater = _context.Liquids.First(x => x.Name == "brackish water");
		_freshWaters.Clear();
		_freshWaters.Add(_context.Liquids.First(x => x.Name == "swamp water"));
		_freshWaters.Add(_context.Liquids.First(x => x.Name == "lake water"));
		_freshWaters.Add(_context.Liquids.First(x => x.Name == "river water"));
		_freshWaters.Add(_context.Liquids.First(x => x.Name == "pool water"));
		_freshWaters.Add(_context.Liquids.First(x => x.Name == "spring water"));
		_freshWaters.Add(_context.Liquids.First(x => x.Name == "tap water"));
		_freshWaters.Add(_context.Liquids.First(x => x.Name == "rain water"));
		_freshWaters.Add(_context.Liquids.First(x => x.Name == "water"));
		_bloodLiquid = _context.Liquids.FirstOrDefault(x => x.Name == "blood");
		if (_bloodLiquid is null)
		{
			var driedBlood = new Material
			{
				Name = "dried Blood",
				MaterialDescription = "dried blood",
				Density = 1520,
				Organic = true,
				Type = 0,
				BehaviourType = 19,
				ThermalConductivity = 0.2,
				ElectricalConductivity = 0.0001,
				SpecificHeatCapacity = 420,
				IgnitionPoint = 555.3722,
				HeatDamagePoint = 412.0389,
				ImpactFracture = 1000,
				ImpactYield = 1000,
				ImpactStrainAtYield = 2,
				ShearFracture = 1000,
				ShearYield = 1000,
				ShearStrainAtYield = 2,
				YoungsModulus = 0.1,
				SolventId = 1,
				SolventVolumeRatio = 4,
				ResidueDesc = "It is covered in {0}dried blood",
				ResidueColour = "red",
				Absorbency = 0
			};
			_context.Materials.Add(driedBlood);
			var blood = new Liquid
			{
				Name = "Blood",
				Description = "blood",
				LongDescription = "a virtually opaque dark red fluid",
				TasteText = "It has a sharply metallic, umami taste",
				VagueTasteText = "It has a metallic taste",
				SmellText = "It has a metallic, coppery smell",
				VagueSmellText = "It has a faintly metallic smell",
				TasteIntensity = 200,
				SmellIntensity = 10,
				AlcoholLitresPerLitre = 0,
				WaterLitresPerLitre = 0.8,
				DrinkSatiatedHoursPerLitre = 6,
				FoodSatiatedHoursPerLitre = 4,
				Viscosity = 1,
				Density = 1,
				Organic = true,
				ThermalConductivity = 0.609,
				ElectricalConductivity = 0.005,
				SpecificHeatCapacity = 4181,
				FreezingPoint = -20,
				BoilingPoint = 100,
				DisplayColour = "bold red",
				DampDescription = "It is damp with blood",
				WetDescription = "It is wet with blood",
				DrenchedDescription = "It is drenched with blood",
				DampShortDescription = "(blood damp)",
				WetShortDescription = "(bloody)",
				DrenchedShortDescription = "(blood drenched)",
				SolventId = 1,
				SolventVolumeRatio = 5,
				InjectionConsequence = (int)LiquidInjectionConsequence.BloodReplacement,
				ResidueVolumePercentage = 0.05,
				DriedResidue = driedBlood
			};
			_context.Liquids.Add(blood);
			_bloodLiquid = blood;
		}

		_sweatLiquid = _context.Liquids.FirstOrDefault(x => x.Name == "sweat");
		if (_sweatLiquid is null)
		{
			var driedSweat = new Material
			{
				Name = "dried Sweat",
				MaterialDescription = "dried sweat",
				Density = 1520,
				Organic = true,
				Type = 0,
				BehaviourType = 19,
				ThermalConductivity = 0.2,
				ElectricalConductivity = 0.0001,
				SpecificHeatCapacity = 420,
				IgnitionPoint = 555.3722,
				HeatDamagePoint = 412.0389,
				ImpactFracture = 1000,
				ImpactYield = 1000,
				ImpactStrainAtYield = 2,
				ShearFracture = 1000,
				ShearYield = 1000,
				ShearStrainAtYield = 2,
				YoungsModulus = 0.1,
				SolventId = 1,
				SolventVolumeRatio = 3,
				ResidueDesc = "It is covered in {0}dried sweat",
				ResidueColour = "yellow",
				Absorbency = 0
			};
			_context.Materials.Add(driedSweat);
			var sweat = new Liquid
			{
				Name = "Sweat",
				Description = "sweat",
				LongDescription = "a relatively clear, translucent fluid that smells strongly of body odor",
				TasteText = "It tastes like a pungent, salty lick of someone's underarms",
				VagueTasteText = "It tastes very unpleasant, like underarm stench",
				SmellText = "It has the sharp, pungent smell of body odor",
				VagueSmellText = "It has the sharp, pungent smell of body odor",
				TasteIntensity = 200,
				SmellIntensity = 200,
				AlcoholLitresPerLitre = 0,
				WaterLitresPerLitre = 0.95,
				DrinkSatiatedHoursPerLitre = 5,
				FoodSatiatedHoursPerLitre = 0,
				Viscosity = 1,
				Density = 1,
				Organic = true,
				ThermalConductivity = 0.609,
				ElectricalConductivity = 0.005,
				SpecificHeatCapacity = 4181,
				FreezingPoint = -20,
				BoilingPoint = 100,
				DisplayColour = "yellow",
				DampDescription = "It is damp with sweat",
				WetDescription = "It is wet and smelly with sweat",
				DrenchedDescription = "It is soaking wet and smelly with sweat",
				DampShortDescription = "(sweat-damp)",
				WetShortDescription = "(sweaty)",
				DrenchedShortDescription = "(sweat-drenched)",
				SolventId = 1,
				SolventVolumeRatio = 5,
				InjectionConsequence = (int)LiquidInjectionConsequence.Harmful,
				ResidueVolumePercentage = 0.05,
				DriedResidue = driedSweat
			};
			_context.Liquids.Add(sweat);
			_sweatLiquid = sweat;
		}

		_alwaysFalse = _context.FutureProgs.First(x => x.FunctionName == "AlwaysFalse");
		_alwaysTrue = _context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue");
		_attributes = _context.TraitDefinitions.Where(x => x.Type == 1 || x.Type == 3).ToList();
	}

	private void ResetCachedParts()
	{
		_cachedBodyparts.Clear();
		_cachedBodypartUpstreams.Clear();
		_cachedBones.Clear();
		_cachedLimbs.Clear();
		_cachedOrgans.Clear();
	}
	#endregion
	public int SortOrder => 300;
	public string Name => "Animal Seeder";
	public string Tagline => "Installs body types for animals";

	public string FullDescription =>
		@"This seeder will install a variety of animal bodies, races, descriptions and attacks. It currently includes a variety of terrestrial animals that can easily be extended to include fantasy or science-fiction races.

Warning: There is an enormous amount of data contained in this seeder, and it may take a long time to run.";

	public bool Enabled => true;



	private void SetupCorpseModel()
	{
		_defaultCorpseModel = new CorpseModel
		{
			Name = "Organic Animal Corpse",
			Description = "This corpse will decay over time and eventually disappear",
			Type = "Standard",
			Definition = @"<?xml version=""1.0""?>
  <CorpseModel>
	<EdiblePercentage>0.35</EdiblePercentage>
	<Ranges>
	  <Range state=""0"" lower=""0"" upper=""7200""/>
	  <Range state=""1"" lower=""7200"" upper=""21600""/>
	  <Range state=""2"" lower=""21600"" upper=""65000""/>
	  <Range state=""3"" lower=""65000"" upper=""400000""/>
	  <Range state=""4"" lower=""400000"" upper=""2650000""/>
	  <Range state=""5"" lower=""2650000"" upper=""2650001""/>
	</Ranges>
	<Terrains default=""10"">
	  <Terrain terrain=""void"" rate=""0""/>
	</Terrains>
	<Descriptions>
	  <PartDescriptions>
	  <Description state=""0""><![CDATA[&a_an[@@shorteat[, ]freshly severed {1} {0}]]]></Description>
	  <Description state=""1""><![CDATA[&a_an[@@shorteat[, ]severed {1} {0}]]]></Description>
	  <Description state=""2""><![CDATA[&a_an[@@shorteat[, ]decaying severed {1} {0}]]]></Description>
	  <Description state=""3""><![CDATA[&a_an[@@shorteat[, ]decayed severed {0}]]]></Description>
	  <Description state=""4""><![CDATA[&a_an[@@shorteat[, ]heavily decayed severed {0}]]]></Description>
	  <Description state=""5""><![CDATA[the @@shorteat[, ]skeletal remains of &a_an[severed {0}]]]></Description>
	</PartDescriptions>
	  <ShortDescriptions>
		<Description state=""0""><![CDATA[the @@shorteat[, ]fresh corpse of @@sdesc]]></Description>
		<Description state=""1""><![CDATA[the @@shorteat[, ]corpse of @@sdesc]]></Description>
		<Description state=""2""><![CDATA[the @@shorteat[, ]decaying corpse of @@sdesc]]></Description>
		<Description state=""3""><![CDATA[the @@shorteat[, ]decayed corpse of &a_an[&male &race]]]></Description>
		<Description state=""4""><![CDATA[the @@shorteat[, ]heavily decayed &race corpse]]></Description>
		<Description state=""5""><![CDATA[the @@shorteat[, ]skeletal remains of &a_an[&race]]]></Description>
	  </ShortDescriptions>
	  <FullDescriptions>
		<Description state=""0""><![CDATA[@@desc
  #3Death has taken hold of this critter, but it still looks much the same as it did in life.#0
  
  @@eaten[
  
  ]@@wounds
  
  @@inv]]></Description>
		<Description state=""1""><![CDATA[@@desc
  #3Rigor Mortis has set in with this critter, and the blood has begun to pool in the extremities. Flies and other carrion insects have begun to lay eggs in its skin.#0
  
  @@eaten[
  
  ]@@wounds
  
  @@inv]]></Description>
		<Description state=""2""><![CDATA[@@desc
  #3This corpse has begun to bloat and putrefy as decay sets in. Maggots and other carrion insects have firmly taken hold of the corpse.#0
  
  @@eaten[
  
  ]@@wounds
  
  @@inv]]></Description>
		<Description state=""3""><![CDATA[This is the decayed corpse of a &male &race. The process of decay is far enough advanced that it is difficult to identify any individual features any longer.
  #3This corpse is well along the process of decay. The flesh has sloughed off in places and particularly soft parts such as the eyes have been eaten by carrion insects.#0
  
  @@eaten[
  
  ]@@wounds
  
  @@inv]]></Description>
		<Description state=""4""><![CDATA[This is the heavily decayed corpse of &a_an[&race] of indeterminate gender. Very little identifiable features remain.
  #3This corpse in a very advanced state of decay. The flesh is almost entirely gone and the corpse has largely liquified. At this stage, the only parts remaining are what the carrion eaters find tough to digest.#0
  
  @@eaten[
  
  ]@@inv]]></Description>
		<Description state=""5""><![CDATA[This is the skeletal remains of &a_an[&race]. Nothing remains of its other features.
  
  @@eaten[
  
  ]@@inv]]></Description>
	  </FullDescriptions>
	  <ContentsDescriptions>
		<Description state=""0""><![CDATA[@@desc
  #3Death has taken hold of this critter, but it still looks much the same as it did in life.#0
  
  @@eaten[
  
  ]@@wounds
  
  @@inv]]></Description>
		<Description state=""1""><![CDATA[@@desc
  #3Rigor Mortis has set in with this critter, and the blood has begun to pool in the extremities. Flies and other carrion insects have begun to lay eggs in the skin.#0
  
  @@eaten[
  
  ]@@wounds
  
  @@inv]]></Description>
		<Description state=""2""><![CDATA[@@desc
  #3This corpse has begun to bloat and putrefy as decay sets in. Maggots and other carrion insects have firmly taken hold of the corpse.#0
  
  @@eaten[
  
  ]@@wounds
  
  @@inv]]></Description>
		<Description state=""3""><![CDATA[This is the decayed corpse of a &male &race. The process of decay is far enough advanced that it is difficult to identify any individual features any longer.
  #3This corpse is well along the process of decay. The flesh has sloughed off in places and particularly soft parts such as the eyes have been eaten by carrion insects.#0
  
  @@eaten[
  
  ]@@wounds
  
  @@inv]]></Description>
		<Description state=""4""><![CDATA[This is the heavily decayed corpse of &a_an[&race] of indeterminate gender. Very little identifiable features remain.
  #3This corpse in a very advanced state of decay. The flesh is almost entirely gone and the corpse has largely liquified. At this stage, the only parts remaining are what the carrion eaters find tough to digest.#0
  
  @@eaten[
  
  ]@@inv]]></Description>
		<Description state=""5""><![CDATA[This is the skeletal remains of &a_an[&race]. Nothing remains of their other features.
  
  @@eaten[
  
  ]@@inv]]></Description>
	  </ContentsDescriptions>
	</Descriptions>
  </CorpseModel>"
		};
		_context.CorpseModels.Add(_defaultCorpseModel);
		_context.SaveChanges();
	}

	private HealthStrategy SetupHealthModel(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> questionAnswers,
		TraitExpression hpExpression, TraitExpression hpTick, TraitDefinition secondaryTrait)
	{
		Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Health Models...");
		var hpStrategy = new HealthStrategy
		{
			Name = NonHumanSeederHealthStrategyHelper.HpStrategyName,
			Type = "BrainHitpoints",
			Definition =
				$"<Definition> <MaximumHitPointsExpression>{hpExpression.Id}</MaximumHitPointsExpression> <HealingTickDamageExpression>{hpTick.Id}</HealingTickDamageExpression><PercentageHealthPerPenalty>0.2</PercentageHealthPerPenalty><LodgeDamageExpression>max(0, damage-15)</LodgeDamageExpression> <SeverityRanges> <Severity value=\"0\" lower=\"-1\" upper=\"0\"/> <Severity value=\"1\" lower=\"0\" upper=\"2\"/> <Severity value=\"2\" lower=\"2\" upper=\"4\"/> <Severity value=\"3\" lower=\"4\" upper=\"7\"/> <Severity value=\"4\" lower=\"7\" upper=\"12\"/> <Severity value=\"5\" lower=\"12\" upper=\"18\"/> <Severity value=\"6\" lower=\"18\" upper=\"27\"/> <Severity value=\"7\" lower=\"27\" upper=\"40\"/> <Severity value=\"8\" lower=\"40\" upper=\"100\"/> </SeverityRanges><CheckHeart>false</CheckHeart> <UseHypoxiaDamage>false</UseHypoxiaDamage><KnockoutOnCritical>true</KnockoutOnCritical><KnockoutDuration>240</KnockoutDuration></Definition>"
		};
		context.HealthStrategies.Add(hpStrategy);

		var hpPlusStrategy = new HealthStrategy
		{
			Name = NonHumanSeederHealthStrategyHelper.HpPlusStrategyName,
			Type = "BrainHitpoints",
			Definition =
				$"<Definition> <MaximumHitPointsExpression>{hpExpression.Id}</MaximumHitPointsExpression> <HealingTickDamageExpression>{hpTick.Id}</HealingTickDamageExpression><PercentageHealthPerPenalty>0.2</PercentageHealthPerPenalty><LodgeDamageExpression>max(0,damage-15)</LodgeDamageExpression> <SeverityRanges> <Severity value=\"0\" lower=\"-1\" upper=\"0\"/> <Severity value=\"1\" lower=\"0\" upper=\"2\"/> <Severity value=\"2\" lower=\"2\" upper=\"4\"/> <Severity value=\"3\" lower=\"4\" upper=\"7\"/> <Severity value=\"4\" lower=\"7\" upper=\"12\"/> <Severity value=\"5\" lower=\"12\" upper=\"18\"/> <Severity value=\"6\" lower=\"18\" upper=\"27\"/> <Severity value=\"7\" lower=\"27\" upper=\"40\"/> <Severity value=\"8\" lower=\"40\" upper=\"100\"/> </SeverityRanges> <CheckHeart>true</CheckHeart> <UseHypoxiaDamage>true</UseHypoxiaDamage><KnockoutOnCritical>true</KnockoutOnCritical><KnockoutDuration>240</KnockoutDuration> </Definition>"
		};
		context.HealthStrategies.Add(hpPlusStrategy);

		var stunExpression = new TraitExpression
		{
			Name = "Non-Human Max Stun Formula",
			Expression = $"100+(con:{_healthTrait.Id}+wil:{secondaryTrait.Id})/2"
		};
		context.TraitExpressions.Add(stunExpression);

		var painExpression = new TraitExpression
		{
			Name = "Non-Human Max Pain Formula",
			Expression = $"100+wil:{secondaryTrait.Id}"
		};
		context.TraitExpressions.Add(painExpression);

		var stunTick = new TraitExpression
		{
			Name = "Non-Human Stun Heal Per Tick",
			Expression = $"100+con:{_healthTrait.Id}"
		};
		context.TraitExpressions.Add(stunTick);

		var painTick = new TraitExpression
		{
			Name = "Non-Human Pain Heal Per Tick",
			Expression = $"100+con:{_healthTrait.Id}"
		};
		context.TraitExpressions.Add(painTick);
		context.SaveChanges();

		var fullStrategy = new HealthStrategy
		{
			Name = NonHumanSeederHealthStrategyHelper.FullStrategyName,
			Type = "ComplexLiving",
			Definition =
				$"<Definition> <MaximumHitPointsExpression>{hpExpression.Id}</MaximumHitPointsExpression> <MaximumStunExpression>{stunExpression.Id}</MaximumStunExpression> <MaximumPainExpression>{painExpression.Id}</MaximumPainExpression> <HealingTickDamageExpression>{hpTick.Id}</HealingTickDamageExpression> <HealingTickStunExpression>{stunTick.Id}</HealingTickStunExpression> <HealingTickPainExpression>{painTick.Id}</HealingTickPainExpression> <LodgeDamageExpression>max(0, damage - 30)</LodgeDamageExpression> <PercentageHealthPerPenalty>0.2</PercentageHealthPerPenalty> <PercentageStunPerPenalty>0.2</PercentageStunPerPenalty> <PercentagePainPerPenalty>0.2</PercentagePainPerPenalty> <SeverityRanges> <Severity value=\"0\" lower=\"-2\" upper=\"-1\" lowerpec=\"-100\" upperperc=\"0\"/> <Severity value=\"1\" lower=\"-1\" upper=\"2\" lowerpec=\"0\" upperperc=\"0.4\"/> <Severity value=\"2\" lower=\"2\" upper=\"4\" lowerpec=\"0.4\" upperperc=\"0.55\"/> <Severity value=\"3\" lower=\"4\" upper=\"7\" lowerpec=\"0.55\" upperperc=\"0.65\"/> <Severity value=\"4\" lower=\"7\" upper=\"12\" lowerpec=\"0.65\" upperperc=\"0.75\"/> <Severity value=\"5\" lower=\"12\" upper=\"18\" lowerpec=\"0.75\" upperperc=\"0.85\"/> <Severity value=\"6\" lower=\"18\" upper=\"27\" lowerpec=\"0.85\" upperperc=\"0.9\"/> <Severity value=\"7\" lower=\"27\" upper=\"40\" lowerpec=\"0.9\" upperperc=\"0.95\"/> <Severity value=\"8\" lower=\"40\" upper=\"100\" lowerpec=\"0.95\" upperperc=\"100\"/> </SeverityRanges> </Definition>"
		};
		context.HealthStrategies.Add(fullStrategy);

		_healthStrategy = questionAnswers["model"].ToLowerInvariant() switch
		{
			"hp" => hpStrategy,
			"hpplus" => hpPlusStrategy,
			"full" => fullStrategy,
			_ => throw new ArgumentOutOfRangeException()
		};

		_context.SaveChanges();
		return _healthStrategy;
	}

	private void SetupHeightWeightModels()
	{
		Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Height Weight Models...");
		foreach (var template in HeightWeightTemplates.Values)
		{
			AddHWModel(template.Name, template.MeanHeight, template.StandardDeviationHeight, template.MeanBmi,
				template.StandardDeviationBmi);
		}
	}

	private void AddHWModel(string name, double meanHeight, double stddevheight, double meanbmi, double stddevbmi)
	{
		var hwModel = new HeightWeightModel
		{
			Name = name,
			MeanHeight = meanHeight,
			MeanBmi = meanbmi,
			StddevHeight = stddevheight,
			StddevBmi = stddevbmi,
			Bmimultiplier = 0.1
		};
		_context.Add(hwModel);
		_hwModels.Add(name, hwModel);
		_context.SaveChanges();
	}

	#region Combat
	private void SeedCombatStrategy(string name, string description, double weaponUse, double naturalUse,
			double auxilliaryUse, bool preferFavourite, bool preferArmed, bool preferNonContact, bool preferShields,
			bool attackCritical, bool attackUnarmed, bool skirmish, bool fallbackToUnarmed,
			bool automaticallyMoveToTarget, bool manualPositionManagement, bool moveToMeleeIfCannotRange,
			PursuitMode pursuit, CombatStrategyMode melee, CombatStrategyMode ranged,
			AutomaticInventorySettings inventory, AutomaticMovementSettings movement,
			AutomaticRangedSettings rangesettings, AttackHandednessOptions setup, GrappleResponse grapple,
			double requiredMinimumAim, double minmumStamina, DefenseType defaultDefenseType,
			IEnumerable<MeleeAttackOrderPreference> order,
			CombatMoveIntentions forbiddenIntentions = CombatMoveIntentions.None,
			CombatMoveIntentions preferredIntentions = CombatMoveIntentions.None)
	{
		if (_context.CharacterCombatSettings.Any(x => x.Name == name))
		{
			return;
		}

		var strategy = new CharacterCombatSetting
		{
			Name = name,
			Description = description,
			GlobalTemplate = true,
			AvailabilityProg = _alwaysTrue,
			WeaponUsePercentage = weaponUse,
			MagicUsePercentage = 0.0,
			PsychicUsePercentage = 0.0,
			NaturalWeaponPercentage = naturalUse,
			AuxiliaryPercentage = auxilliaryUse,
			PreferFavouriteWeapon = preferFavourite,
			PreferToFightArmed = preferArmed,
			PreferNonContactClinchBreaking = preferNonContact,
			PreferShieldUse = preferShields,
			ClassificationsAllowed = "1 2 3 4 5 7",
			RequiredIntentions = 0,
			ForbiddenIntentions = (long)forbiddenIntentions,
			PreferredIntentions = (long)preferredIntentions,
			AttackCriticallyInjured = attackCritical,
			AttackHelpless = attackUnarmed,
			SkirmishToOtherLocations = skirmish,
			PursuitMode = (int)pursuit,
			DefaultPreferredDefenseType = (int)defaultDefenseType,
			PreferredMeleeMode = (int)melee,
			PreferredRangedMode = (int)ranged,
			FallbackToUnarmedIfNoWeapon = fallbackToUnarmed,
			AutomaticallyMoveTowardsTarget = automaticallyMoveToTarget,
			InventoryManagement = (int)inventory,
			MovementManagement = (int)movement,
			RangedManagement = (int)rangesettings,
			ManualPositionManagement = manualPositionManagement,
			MinimumStaminaToAttack = minmumStamina,
			MoveToMeleeIfCannotEngageInRangedCombat = moveToMeleeIfCannotRange,
			PreferredWeaponSetup = (int)setup,
			RequiredMinimumAim = requiredMinimumAim,
			MeleeAttackOrderPreference = order.Select(selector: x => ((int)x).ToString()).ListToCommaSeparatedValues(separator: " "),
			GrappleResponse = (int)grapple
		};
		_context.CharacterCombatSettings.Add(entity: strategy);
		_context.SaveChanges();
	}

	private void SeedCombatStrategies()
	{
		var defaultOrder = new List<MeleeAttackOrderPreference>
		{
			MeleeAttackOrderPreference.Weapon,
			MeleeAttackOrderPreference.Implant,
			MeleeAttackOrderPreference.Prosthetic,
			MeleeAttackOrderPreference.Magic,
			MeleeAttackOrderPreference.Psychic
		};

		SeedCombatStrategy(name: "Animal", description: "Fully automatic brawler designed for use with animals", weaponUse: 0.0, naturalUse: 1.0, auxilliaryUse: 0.0, preferFavourite: false,
			preferArmed: false, preferNonContact: false, preferShields: false, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: true, manualPositionManagement: false, moveToMeleeIfCannotRange: true, pursuit: PursuitMode.AlwaysPursue,
			melee: CombatStrategyMode.StandardMelee, ranged: CombatStrategyMode.FullAdvance,
			inventory: AutomaticInventorySettings.FullyAutomatic, movement: AutomaticMovementSettings.FullyAutomatic,
			rangesettings: AutomaticRangedSettings.ContinueFiringOnly, setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Avoidance, requiredMinimumAim: 0.5,
			minmumStamina: 5.0, defaultDefenseType: DefenseType.Dodge, order: defaultOrder);
		SeedCombatStrategy(name: "Biter", description: "Fully automatic clinch-brawler designed for use with animals with bite attacks", weaponUse: 0.0, naturalUse: 1.0, auxilliaryUse: 0.0, preferFavourite: false,
			preferArmed: false, preferNonContact: false, preferShields: false, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: true, manualPositionManagement: false, moveToMeleeIfCannotRange: true, pursuit: PursuitMode.AlwaysPursue,
			melee: CombatStrategyMode.Clinch, ranged: CombatStrategyMode.FullAdvance,
			inventory: AutomaticInventorySettings.FullyAutomatic, movement: AutomaticMovementSettings.FullyAutomatic,
			rangesettings: AutomaticRangedSettings.ContinueFiringOnly, setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Avoidance, requiredMinimumAim: 0.5,
			minmumStamina: 5.0, defaultDefenseType: DefenseType.Dodge, order: defaultOrder);
		SeedCombatStrategy(name: "Behemoth", description: "Fully automatic brawler designed for use with big, strong animals", weaponUse: 0.0, naturalUse: 1.0, auxilliaryUse: 0.0, preferFavourite: false,
			preferArmed: false, preferNonContact: true, preferShields: false, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: true, manualPositionManagement: false, moveToMeleeIfCannotRange: true, pursuit: PursuitMode.AlwaysPursue,
			melee: CombatStrategyMode.StandardMelee, ranged: CombatStrategyMode.FullAdvance,
			inventory: AutomaticInventorySettings.FullyAutomatic, movement: AutomaticMovementSettings.FullyAutomatic,
			rangesettings: AutomaticRangedSettings.ContinueFiringOnly, setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Counter, requiredMinimumAim: 0.5,
			minmumStamina: 5.0, defaultDefenseType: DefenseType.Dodge, order: defaultOrder);
		SeedCombatStrategy(name: "Swooper", description: "Fully automatic flying hunter designed for creatures that dive through enemies with breath and wing attacks", weaponUse: 0.0, naturalUse: 1.0, auxilliaryUse: 0.0, preferFavourite: false,
			preferArmed: false, preferNonContact: true, preferShields: false, attackCritical: true, attackUnarmed: true, skirmish: true, fallbackToUnarmed: true, automaticallyMoveToTarget: true, manualPositionManagement: false, moveToMeleeIfCannotRange: true, pursuit: PursuitMode.AlwaysPursue,
			melee: CombatStrategyMode.Swooper, ranged: CombatStrategyMode.Swooper,
			inventory: AutomaticInventorySettings.FullyAutomatic, movement: AutomaticMovementSettings.FullyAutomatic,
			rangesettings: AutomaticRangedSettings.ContinueFiringOnly, setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Avoidance, requiredMinimumAim: 0.4,
			minmumStamina: 5.0, defaultDefenseType: DefenseType.Dodge, order: defaultOrder);
		SeedCombatStrategy(name: "Wimpy Animal", description: "Fully automatic wimpy designed for use with animals", weaponUse: 0.0, naturalUse: 0.1, auxilliaryUse: 0.9, preferFavourite: false,
			preferArmed: false, preferNonContact: false, preferShields: false, attackCritical: true, attackUnarmed: true, skirmish: false, fallbackToUnarmed: true, automaticallyMoveToTarget: false, manualPositionManagement: false, moveToMeleeIfCannotRange: false, pursuit: PursuitMode.NeverPursue,
			melee: CombatStrategyMode.Flee, ranged: CombatStrategyMode.Flee,
			inventory: AutomaticInventorySettings.FullyAutomatic, movement: AutomaticMovementSettings.FullyAutomatic,
			rangesettings: AutomaticRangedSettings.ContinueFiringOnly, setup: AttackHandednessOptions.Any, grapple: GrappleResponse.Avoidance, requiredMinimumAim: 0.5,
			minmumStamina: 5.0, defaultDefenseType: DefenseType.Dodge, order: defaultOrder);
	}

	private WeaponAttack AddAttack(string name, BuiltInCombatMoveType moveType,
		MeleeWeaponVerb verb, Difficulty attacker, Difficulty dodge, Difficulty parry, Difficulty block,
		Alignment alignment, Orientation orientation, double stamina, double relativeSpeed,
		BodypartShape shape, TraitExpression damage, string attackMessage,
		DamageType damageType = DamageType.Crushing, double weighting = 100,
		CombatMoveIntentions intentions = CombatMoveIntentions.Attack | CombatMoveIntentions.Wound,
		string additionalInfo = null)
	{
		var formattedAttackMessage = CombatSeederMessageStyleHelper.FormatAttackMessage(
			attackMessage,
			CombatSeederMessageStyleHelper.Parse(_questionAnswers["messagestyle"]));

		var attack = new WeaponAttack
		{
			Verb = (int)verb,
			BaseAttackerDifficulty = (int)attacker,
			BaseBlockDifficulty = (int)block,
			BaseDodgeDifficulty = (int)dodge,
			BaseParryDifficulty = (int)parry,
			MoveType = (int)moveType,
			RecoveryDifficultySuccess = (int)Difficulty.Easy,
			RecoveryDifficultyFailure = (int)Difficulty.Hard,
			Intentions = (long)intentions,
			Weighting = weighting,
			ExertionLevel = (int)ExertionLevel.Heavy,
			DamageType = (int)damageType,
			DamageExpression = damage,
			StunExpression = damage,
			PainExpression = damage,
			BodypartShapeId = shape.Id,
			StaminaCost = stamina,
			BaseDelay = relativeSpeed,
			Name = name,
			Orientation = (int)orientation,
			Alignment = (int)alignment,
			HandednessOptions = 0,
			AdditionalInfo = additionalInfo
		};
		_context.WeaponAttacks.Add(attack);
		_context.SaveChanges();

		var message = new CombatMessage
		{
			Type = (int)moveType,
			Message = formattedAttackMessage,
			Priority = 50,
			Verb = (int)verb,
			Chance = 1.0,
			FailureMessage = formattedAttackMessage
		};
		message.CombatMessagesWeaponAttacks.Add(new CombatMessagesWeaponAttacks
		{ CombatMessage = message, WeaponAttack = attack });
		_context.CombatMessages.Add(message);
		_context.SaveChanges();
		return attack;
	}

	private void AddAttackToRace(string whichAttack, Race race, ItemQuality quality)
	{
		var bodies = new List<long>();
		bodies.Add(race.BaseBodyId);
		var body = race.BaseBody.CountsAs;
		while (body != null)
		{
			bodies.Add(body.Id);
			body = body.CountsAs;
		}

		var attack = _attacks[whichAttack];
		foreach (var bodypart in _context.BodypartProtos.Where(x =>
					 bodies.Contains(x.BodyId) && x.BodypartShapeId == attack.BodypartShapeId))
			_context.RacesWeaponAttacks.Add(new RacesWeaponAttacks
			{
				Bodypart = bodypart,
				Race = race,
				WeaponAttack = attack,
				Quality = (int)quality
			});
	}

	private void AddJellyfishAttack(Race race)
	{
		var attackAddendum =
			CombatSeederMessageStyleHelper.AttackSuffix(
				CombatSeederMessageStyleHelper.Parse(_questionAnswers["messagestyle"]));

		var tendrilShape = _context.BodypartShapes.First(x => x.Name == "Tendril");
		var tendrils = _context.BodypartProtos.Where(x => x.Body == race.BaseBody && x.BodypartShape == tendrilShape)
			.ToList();

		var venom = new Drug
		{
			Name = $"{race.Name} Venom",
			IntensityPerGram = 10,
			DrugVectors = (int)(DrugVector.Touched | DrugVector.Injected),
			RelativeMetabolisationRate = 0.05
		};
		_context.Drugs.Add(venom);
		_context.SaveChanges();

		var liquid = new Liquid
		{
			Name = $"{race.Name} Venom".ToLowerInvariant(),
			Description = "a clear liquid",
			LongDescription = "a clear, translucent liquid",
			TasteText =
				"It has one of the most intense bitter tastes and instantly makes your tongue and lips wrack with pain",
			VagueTasteText =
				"It has one of the most intense bitter tastes and instantly makes your tongue and lips wrack with pain",
			SmellText =
				"It smells extremely bitter and actually makes your nose and throat burn like hell just from smelling it",
			VagueSmellText =
				"It smells extremely bitter and actually makes your nose and throat burn like hell just from smelling it",
			TasteIntensity = 2000,
			SmellIntensity = 2000,
			AlcoholLitresPerLitre = 0,
			WaterLitresPerLitre = 0.5,
			DrinkSatiatedHoursPerLitre = 6,
			FoodSatiatedHoursPerLitre = 4,
			Viscosity = 1,
			Density = 1,
			Organic = true,
			ThermalConductivity = 0.609,
			ElectricalConductivity = 0.005,
			SpecificHeatCapacity = 4181,
			FreezingPoint = -20,
			BoilingPoint = 100,
			DisplayColour = "bold pink",
			DampDescription = "It is damp",
			WetDescription = "It is wet",
			DrenchedDescription = "It is drenched",
			DampShortDescription = "(damp)",
			WetShortDescription = "(wet)",
			DrenchedShortDescription = "(drenched)",
			SolventId = 1,
			SolventVolumeRatio = 5,
			InjectionConsequence = (int)LiquidInjectionConsequence.Deadly,
			ResidueVolumePercentage = 0.05,
			DriedResidue = null,
			Drug = venom,
			DrugGramsPerUnitVolume = 1000
		};
		_context.Liquids.Add(liquid);
		_context.SaveChanges();

		var attack = AddAttack($"{race.Name} Sting", BuiltInCombatMoveType.EnvenomingAttackClinch,
			MeleeWeaponVerb.Strike, Difficulty.Normal, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard,
			Alignment.Front, Orientation.Low, 0.5, 1.5, tendrilShape, _snakeBiteDamage,
			$"@ drift|drifts close and brush|brushes a stinging tendril across $1{attackAddendum}", DamageType.Cellular,
			additionalInfo: @$"<Data>
   <Liquid>{liquid.Id}</Liquid>
   <MaximumQuantity>0.08</MaximumQuantity>
   <MinimumWoundSeverity>0</MinimumWoundSeverity>
 </Data>");


		foreach (var part in tendrils)
			_context.RacesWeaponAttacks.Add(new RacesWeaponAttacks
			{
				Bodypart = part,
				Race = race,
				WeaponAttack = attack,
				Quality = (int)ItemQuality.Standard
			});

		_context.SaveChanges();
	}

	private void AddSerpentAttack(Race race, bool venomous)
	{
		var attackAddendum =
			CombatSeederMessageStyleHelper.AttackSuffix(
				CombatSeederMessageStyleHelper.Parse(_questionAnswers["messagestyle"]));

		var fangShape = _context.BodypartShapes.First(x => x.Name == "Fang");
		var fang = _context.BodypartProtos.First(x => x.Body == race.BaseBody && x.BodypartShape == fangShape);

		WeaponAttack attack;
		if (venomous)
		{
			var venom = new Drug
			{
				Name = $"{race.Name} Venom",
				IntensityPerGram = 1,
				DrugVectors = (int)DrugVector.Injected,
				RelativeMetabolisationRate = 0.05
			};
			_context.Drugs.Add(venom);
			_context.SaveChanges();

			var liquid = new Liquid
			{
				Name = $"{race.Name} Venom".ToLowerInvariant(),
				Description = "a clear liquid",
				LongDescription = "a clear, translucent liquid",
				TasteText = "It has one of the most intense bitter tastes and instantly makes your tongue go numb",
				VagueTasteText =
					"It has one of the most intense bitter tastes and instantly makes your tongue go numb",
				SmellText =
					"It smells extremely bitter and actually makes your nose and throat hurt just from smelling it",
				VagueSmellText =
					"It smells extremely bitter and actually makes your nose and throat hurt just from smelling it",
				TasteIntensity = 2000,
				SmellIntensity = 2000,
				AlcoholLitresPerLitre = 0,
				WaterLitresPerLitre = 0.5,
				DrinkSatiatedHoursPerLitre = 6,
				FoodSatiatedHoursPerLitre = 4,
				Viscosity = 1,
				Density = 1,
				Organic = true,
				ThermalConductivity = 0.609,
				ElectricalConductivity = 0.005,
				SpecificHeatCapacity = 4181,
				FreezingPoint = -20,
				BoilingPoint = 100,
				DisplayColour = "bold pink",
				DampDescription = "It is damp",
				WetDescription = "It is wet",
				DrenchedDescription = "It is drenched",
				DampShortDescription = "(damp)",
				WetShortDescription = "(wet)",
				DrenchedShortDescription = "(drenched)",
				SolventId = 1,
				SolventVolumeRatio = 5,
				InjectionConsequence = (int)LiquidInjectionConsequence.Harmful,
				ResidueVolumePercentage = 0.05,
				DriedResidue = null,
				Drug = venom,
				DrugGramsPerUnitVolume = 1000
			};
			_context.Liquids.Add(liquid);
			_context.SaveChanges();

			attack = AddAttack($"{race.Name} Bite", BuiltInCombatMoveType.EnvenomingAttackClinch,
				MeleeWeaponVerb.Bite, Difficulty.Normal, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard,
				Alignment.Front, Orientation.Low, 4.0, 1.0, fangShape, _snakeBiteDamage,
				$"@ strike|strikes with a quick bite at $1{attackAddendum}", DamageType.Bite,
				additionalInfo: @$"<Data>
   <Liquid>{liquid.Id}</Liquid>
   <MaximumQuantity>0.005</MaximumQuantity>
   <MinimumWoundSeverity>3</MinimumWoundSeverity>
 </Data>");
		}
		else
		{
			attack = AddAttack($"{race.Name} Bite", BuiltInCombatMoveType.ClinchUnarmedAttack, MeleeWeaponVerb.Bite,
				Difficulty.Normal, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard, Alignment.Front,
				Orientation.Low, 4.0, 1.0, fangShape, _snakeBiteDamage,
				$"@ dart|darts in and sink|sinks &0's fangs into $1{attackAddendum}", DamageType.Bite);
		}

		_context.RacesWeaponAttacks.Add(new RacesWeaponAttacks
		{
			Bodypart = fang,
			Race = race,
			WeaponAttack = attack,
			Quality = (int)ItemQuality.Standard
		});
		_context.SaveChanges();
	}

	private void AddAttacks(Race race, ItemQuality quality, bool carnivoreBites = false,
		bool herbivoreBites = false, bool smallAnimalBites = false, bool clawAttacks = false,
		bool hoofAttacks = false, bool chargeAttacks = false, bool hornAttacks = false, bool tuskAttacks = false,
		bool antlerAttacks = false, bool fishBites = false, bool crabAttacks = false, bool sharkAttacks = false)
	{
		if (fishBites)
		{
			AddAttackToRace("fishbite", race, quality);
			AddAttackToRace("fishquickbite", race, quality);
		}

		if (sharkAttacks)
		{
			AddAttackToRace("sharkbite", race, quality);
			AddAttackToRace("sharkreelbite", race, quality);
		}

		if (crabAttacks) AddAttackToRace("crabpinch", race, quality);

		if (carnivoreBites)
		{
			AddAttackToRace("carnivorebite", race, quality);
			AddAttackToRace("carnivoresmashbite", race, quality);
			AddAttackToRace("carnivorelowbite", race, quality);
			AddAttackToRace("carnivorehighbite", race, quality);
			AddAttackToRace("carnivorelowestbite", race, quality);
			AddAttackToRace("carnivoreclinchbite", race, quality);
			AddAttackToRace("carnivoreclinchhighbite", race, quality);
			AddAttackToRace("carnivoreclinchhighestbite", race, quality);
			AddAttackToRace("carnivoredownbite", race, quality);
		}

		if (herbivoreBites)
		{
			AddAttackToRace("herbivorebite", race, quality);
			AddAttackToRace("herbivoresmashbite", race, quality);
		}

		if (smallAnimalBites)
		{
			AddAttackToRace("smallbite", race, quality);
			AddAttackToRace("smallsmashbite", race, quality);
			AddAttackToRace("smalllowbite", race, quality);
			AddAttackToRace("smalldownedbite", race, quality);
		}

		if (clawAttacks)
		{
			AddAttackToRace("clawswipe", race, quality);
			AddAttackToRace("clawsmashswipe", race, quality);
			AddAttackToRace("clawlowswipe", race, quality);
			AddAttackToRace("clawhighswipe", race, quality);
		}

		if (hoofAttacks)
		{
			AddAttackToRace("hoofstomp", race, quality);
			AddAttackToRace("hoofstompsmash", race, quality);
		}

		if (chargeAttacks)
		{
			AddAttackToRace("barge", race, quality);
			AddAttackToRace("bargesmash", race, quality);
			AddAttackToRace("clinchbarge", race, quality);
		}

		if (hornAttacks) AddAttackToRace("gorehorn", race, quality);

		if (antlerAttacks) AddAttackToRace("goreantler", race, quality);

		if (tuskAttacks)
		{
			AddAttackToRace("goretusk", race, quality);
			AddAttackToRace("tusksweep", race, quality);
		}
	}

	private void SetupAttacks(bool firstTime)
	{
		if (!firstTime)
		{
			var randomPortion = "";
			switch (_questionAnswers["random"].ToLowerInvariant())
			{
				case "partial":
					randomPortion = " * rand(0.7,1.0)";
					break;
				case "random":
					randomPortion = " * rand(0.2,1.0)";
					break;
			}

			TraitExpression GetOrCreateExpression(string name, string expression)
			{
				var existing = _context.TraitExpressions.FirstOrDefault(x => x.Name == name);
				if (existing is not null)
				{
					return existing;
				}

				var created = new TraitExpression
				{
					Name = name,
					Expression = expression
				};
				_context.TraitExpressions.Add(created);
				_context.SaveChanges();
				return created;
			}

			var peckDamage = GetOrCreateExpression("Animal Peck Damage",
				$"0.45 * (str:{_strengthTrait.Id} + (2 * quality)) * sqrt(degree+1){randomPortion}");
			var talonDamage = GetOrCreateExpression("Animal Talon Damage",
				$"0.8 * (str:{_strengthTrait.Id} + (2 * quality)) * sqrt(degree+1){randomPortion}");
			var mandibleDamage = GetOrCreateExpression("Animal Mandible Damage",
				$"0.35 * (str:{_strengthTrait.Id} + (2 * quality)) * sqrt(degree+1){randomPortion}");
			var ramDamage = GetOrCreateExpression("Animal Ram Damage",
				$"0.9 * (str:{_strengthTrait.Id} + (2 * quality)) * sqrt(degree+1){randomPortion}");
			_snakeBiteDamage = GetOrCreateExpression("Snake Bite Damage",
				$"0.5 * (str:{_strengthTrait.Id} + (3 * quality)) * sqrt(degree+1){randomPortion}");

			_attacks["carnivorebite"] = _context.WeaponAttacks.First(x => x.Name == "Carnivore Bite");
			_attacks["carnivoresmashbite"] = _context.WeaponAttacks.First(x => x.Name == "Carnivore Smash Bite");
			_attacks["carnivorelowbite"] = _context.WeaponAttacks.First(x => x.Name == "Carnivore Low Bite");
			_attacks["carnivorehighbite"] = _context.WeaponAttacks.First(x => x.Name == "Carnivore High Bite");
			_attacks["carnivorelowestbite"] = _context.WeaponAttacks.First(x => x.Name == "Carnivore Lowest Bite");
			_attacks["carnivoreclinchbite"] = _context.WeaponAttacks.First(x => x.Name == "Carnivore Clinch Bite");
			_attacks["carnivoreclinchhighbite"] =
				_context.WeaponAttacks.First(x => x.Name == "Carnivore High Clinch Bite");
			_attacks["carnivoreclinchhighestbite"] =
				_context.WeaponAttacks.First(x => x.Name == "Carnivore Highest Clinch Bite");
			_attacks["carnivoredownbite"] = _context.WeaponAttacks.First(x => x.Name == "Carnivore Downed Bite");
			_attacks["herbivorebite"] = _context.WeaponAttacks.First(x => x.Name == "Herbivore Bite");
			_attacks["herbivoresmashbite"] = _context.WeaponAttacks.First(x => x.Name == "Herbivore Smash Bite");
			_attacks["smallbite"] = _context.WeaponAttacks.First(x => x.Name == "Small Animal Bite");
			_attacks["smallsmashbite"] = _context.WeaponAttacks.First(x => x.Name == "Small Animal Smash Bite");
			_attacks["smalllowbite"] = _context.WeaponAttacks.First(x => x.Name == "Small Animal Low Bite");
			_attacks["smalldownedbite"] = _context.WeaponAttacks.First(x => x.Name == "Small Animal Downed Bite");
			_attacks["clawswipe"] = _context.WeaponAttacks.First(x => x.Name == "Claw Swipe");
			_attacks["clawsmashswipe"] = _context.WeaponAttacks.First(x => x.Name == "Claw Smash Swipe");
			_attacks["clawlowswipe"] = _context.WeaponAttacks.First(x => x.Name == "Claw Low Swipe");
			_attacks["clawhighswipe"] = _context.WeaponAttacks.First(x => x.Name == "Claw High Swipe");
			_attacks["hoofstomp"] = _context.WeaponAttacks.First(x => x.Name == "Hoof Stomp");
			_attacks["hoofstompsmash"] = _context.WeaponAttacks.First(x => x.Name == "Hoof Stomp Smash");
			_attacks["barge"] = _context.WeaponAttacks.First(x => x.Name == "Animal Barge");
			_attacks["bargesmash"] = _context.WeaponAttacks.First(x => x.Name == "Animal Barge Smash");
			_attacks["clinchbarge"] = _context.WeaponAttacks.First(x => x.Name == "Animal Clinch Barge");
			_attacks["gorehorn"] = _context.WeaponAttacks.First(x => x.Name == "Horn Gore");
			_attacks["goreantler"] = _context.WeaponAttacks.First(x => x.Name == "Antler Gore");
			_attacks["goretusk"] = _context.WeaponAttacks.First(x => x.Name == "Tusk Gore");
			_attacks["tusksweep"] = _context.WeaponAttacks.First(x => x.Name == "Tusk Sweep");
			_attacks["crabpinch"] = _context.WeaponAttacks.First(x => x.Name == "Crab Pinch");
			_attacks["fishbite"] = _context.WeaponAttacks.First(x => x.Name == "Fish Bite");
			_attacks["fishquickbite"] = _context.WeaponAttacks.First(x => x.Name == "Fish Quick Bite");
			_attacks["sharkbite"] = _context.WeaponAttacks.First(x => x.Name == "Shark Bite");
			_attacks["sharkreelbite"] = _context.WeaponAttacks.First(x => x.Name == "Shark Reel Bite");

			var beakShape = _context.BodypartShapes.First(x => x.Name == "Beak");
			var talonShape = _context.BodypartShapes.First(x => x.Name == "Talon");
			var fangShape = _context.BodypartShapes.First(x => x.Name == "Fang");
			var mandibleShape = _context.BodypartShapes.First(x => x.Name == "Mandible");
			var mouthShape = _context.BodypartShapes.First(x => x.Name == "Mouth");
			var headShape = _context.BodypartShapes.First(x => x.Name == "Head");
			var tailShape = _context.BodypartShapes.First(x => x.Name == "Tail");
			var tendrilShape = _context.BodypartShapes.First(x => x.Name == "Tendril");
			var clawShape = _context.BodypartShapes.First(x => x.Name == "Claw");

			var attackAddendum = _questionAnswers["messagestyle"].ToLowerInvariant() switch
			{
				"sentences" => ".",
				"sparse" => ".",
				_ => ""
			};

			_attacks["beakpeck"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Beak Peck") ??
				AddAttack("Beak Peck", BuiltInCombatMoveType.NaturalWeaponAttack,
					MeleeWeaponVerb.Stab, Difficulty.Easy, Difficulty.Normal, Difficulty.Hard, Difficulty.Easy,
					Alignment.Front, Orientation.High, 2.5, 0.7, beakShape, peckDamage,
					$"@ dart|darts forward and peck|pecks sharply at $1 with &0's {{0}}{attackAddendum}",
					DamageType.Piercing);
			_attacks["talonstrike"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Talon Strike") ??
				AddAttack("Talon Strike", BuiltInCombatMoveType.NaturalWeaponAttack,
					MeleeWeaponVerb.Claw, Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.Easy,
					Alignment.FrontRight, Orientation.Low, 3.5, 0.9, talonShape, talonDamage,
					$"@ slash|slashes at $1 with &0's {{0}}{attackAddendum}", DamageType.Claw);
			_attacks["fangbite"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Fang Bite") ??
				AddAttack("Fang Bite", BuiltInCombatMoveType.ClinchUnarmedAttack,
					MeleeWeaponVerb.Bite, Difficulty.Normal, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard,
					Alignment.Front, Orientation.Low, 3.0, 0.7, fangShape, _snakeBiteDamage,
					$"@ dart|darts in and try|tries to bite $1 with &0's {{0}}{attackAddendum}", DamageType.Bite);
			_attacks["bite"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Bite") ??
				AddAttack("Bite", BuiltInCombatMoveType.ClinchUnarmedAttack,
					MeleeWeaponVerb.Bite, Difficulty.VeryHard, Difficulty.Normal, Difficulty.ExtremelyHard,
					Difficulty.VeryHard, Alignment.Front, Orientation.High, 3.0, 1.4, mouthShape, ramDamage,
					$"@ lean|leans in and try|tries to bite $1{attackAddendum}", DamageType.Bite);
			_attacks["headbutt"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Headbutt") ??
				AddAttack("Headbutt", BuiltInCombatMoveType.StaggeringBlowClinch,
					MeleeWeaponVerb.Strike, Difficulty.VeryHard, Difficulty.VeryHard, Difficulty.ExtremelyHard,
					Difficulty.VeryHard, Alignment.Front, Orientation.Highest, 5.0, 1.0, headShape, ramDamage,
					$"@ jerk|jerks forward and crack|cracks &0's head into $1{attackAddendum}", DamageType.Crushing,
					additionalInfo: ((int)Difficulty.Hard).ToString());
			_attacks["mandiblebite"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Mandible Bite") ??
				AddAttack("Mandible Bite", BuiltInCombatMoveType.ClinchUnarmedAttack,
					MeleeWeaponVerb.Bite, Difficulty.VeryEasy, Difficulty.Normal, Difficulty.Hard, Difficulty.Easy,
					Alignment.Front, Orientation.Centre, 1.5, 0.4, mandibleShape, mandibleDamage,
					$"@ snap|snaps &0's {{0}} at $1{attackAddendum}", DamageType.Shearing);
			_attacks["beakbite"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Beak Bite") ??
				AddAttack("Beak Bite", BuiltInCombatMoveType.ClinchUnarmedAttack,
					MeleeWeaponVerb.Bite, Difficulty.Normal, Difficulty.Hard, Difficulty.ExtremelyHard, Difficulty.Easy,
					Alignment.Front, Orientation.High, 2.5, 0.6, beakShape, peckDamage,
					$"@ dart|darts in close and jab|jabs &0's beak into $1{attackAddendum}", DamageType.Piercing);
			_attacks["arachnidclaw"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Arachnid Claw") ??
				AddAttack("Arachnid Claw", BuiltInCombatMoveType.NaturalWeaponAttack,
					MeleeWeaponVerb.Swipe, Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.Easy,
					Alignment.FrontRight, Orientation.Centre, 2.5, 0.8, clawShape, _context.TraitExpressions.First(x => x.Name == "Animal Claw Damage"),
					$"@ lash|lashes out with &0's {{0}} at $1{attackAddendum}", DamageType.Claw);
			_attacks["clawclamp"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Claw Clamp") ??
				AddAttack("Claw Clamp", BuiltInCombatMoveType.ClinchUnarmedAttack,
					MeleeWeaponVerb.Claw, Difficulty.Easy, Difficulty.Normal, Difficulty.ExtremelyHard, Difficulty.Easy,
					Alignment.FrontRight, Orientation.Centre, 2.5, 0.7, clawShape, mandibleDamage,
					$"@ clamp|clamps &0's {{0}} onto $1{attackAddendum}", DamageType.Shearing);
			_attacks["headram"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Head Ram") ??
				AddAttack("Head Ram", BuiltInCombatMoveType.StaggeringBlowUnarmed,
					MeleeWeaponVerb.Strike, Difficulty.Normal, Difficulty.Easy, Difficulty.Insane, Difficulty.VeryHard,
					Alignment.Front, Orientation.High, 5.0, 1.1, headShape, ramDamage,
					$"@ surge|surges forward and slam|slams &0's head into $1{attackAddendum}", DamageType.Crushing,
					additionalInfo: ((int)Difficulty.Hard).ToString());
			_attacks["tailslap"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Tail Slap") ??
				AddAttack("Tail Slap", BuiltInCombatMoveType.StaggeringBlowUnarmed,
					MeleeWeaponVerb.Sweep, Difficulty.Normal, Difficulty.Easy, Difficulty.Hard, Difficulty.Hard,
					Alignment.Rear, Orientation.Centre, 4.5, 1.1, tailShape, ramDamage,
					$"@ whip|whips &0's tail around at $1{attackAddendum}", DamageType.Crushing,
					additionalInfo: ((int)Difficulty.Normal).ToString());
			_attacks["tendrillash"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Tendril Lash") ??
				AddAttack("Tendril Lash", BuiltInCombatMoveType.NaturalWeaponAttack,
					MeleeWeaponVerb.Sweep, Difficulty.Easy, Difficulty.Easy, Difficulty.Hard, Difficulty.Easy,
					Alignment.Front, Orientation.Centre, 3.5, 0.5, tendrilShape, peckDamage,
					$"@ lash|lashes a tendril at $1{attackAddendum}", DamageType.Cellular);
		}
		else
		{
			var randomPortion = "";
			switch (_questionAnswers["random"].ToLowerInvariant())
			{
				case "static":
					randomPortion = "";
					break;
				case "partial":
					randomPortion = " * rand(0.7,1.0)";
					break;
				case "random":
					randomPortion = " * rand(0.2,1.0)";
					break;
			}

			var smallBite = new TraitExpression
			{
				Name = "Small Animal Bite Damage",
				Expression = $"0.5 * (str:{_strengthTrait.Id} + (2 * quality)) * sqrt(degree+1){randomPortion}"
			};
			_context.TraitExpressions.Add(smallBite);
			_context.SaveChanges();

			var fishBite = new TraitExpression
			{
				Name = "Fish Bite Damage",
				Expression = $"0.5 * (str:{_strengthTrait.Id} + (2 * quality)) * sqrt(degree+1){randomPortion}"
			};
			_context.TraitExpressions.Add(fishBite);
			_context.SaveChanges();

			var herbivoreBite = new TraitExpression
			{
				Name = "Herbivorous Animal Bite Damage",
				Expression = $"0.5 * (str:{_strengthTrait.Id} + (3 * quality)) * sqrt(degree+1){randomPortion}"
			};
			_context.TraitExpressions.Add(herbivoreBite);
			_context.SaveChanges();

			var carnivoreBite = new TraitExpression
			{
				Name = "Carnivorous Animal Bite Damage",
				Expression = $"1.0 * (str:{_strengthTrait.Id} + (3 * quality)) * sqrt(degree+1){randomPortion}"
			};
			_context.TraitExpressions.Add(carnivoreBite);
			_context.SaveChanges();

			var sharkBite = new TraitExpression
			{
				Name = "Shark Bite Damage",
				Expression = $"1.0 * (str:{_strengthTrait.Id} + (3 * quality)) * sqrt(degree+1){randomPortion}"
			};
			_context.TraitExpressions.Add(sharkBite);
			_context.SaveChanges();

			var clawDamage = new TraitExpression
			{
				Name = "Animal Claw Damage",
				Expression = $"1.0 * (str:{_strengthTrait.Id} + (3 * quality)) * sqrt(degree+1){randomPortion}"
			};
			_context.TraitExpressions.Add(clawDamage);
			_context.SaveChanges();

			var peckDamage = new TraitExpression
			{
				Name = "Animal Peck Damage",
				Expression = $"0.45 * (str:{_strengthTrait.Id} + (2 * quality)) * sqrt(degree+1){randomPortion}"
			};
			_context.TraitExpressions.Add(peckDamage);
			_context.SaveChanges();

			var talonDamage = new TraitExpression
			{
				Name = "Animal Talon Damage",
				Expression = $"0.8 * (str:{_strengthTrait.Id} + (2 * quality)) * sqrt(degree+1){randomPortion}"
			};
			_context.TraitExpressions.Add(talonDamage);
			_context.SaveChanges();

			var mandibleDamage = new TraitExpression
			{
				Name = "Animal Mandible Damage",
				Expression = $"0.35 * (str:{_strengthTrait.Id} + (2 * quality)) * sqrt(degree+1){randomPortion}"
			};
			_context.TraitExpressions.Add(mandibleDamage);
			_context.SaveChanges();

			var ramDamage = new TraitExpression
			{
				Name = "Animal Ram Damage",
				Expression = $"0.9 * (str:{_strengthTrait.Id} + (2 * quality)) * sqrt(degree+1){randomPortion}"
			};
			_context.TraitExpressions.Add(ramDamage);
			_context.SaveChanges();

			var smashDamage = new TraitExpression
			{
				Name = "Animal Smash Damage",
				Expression = $"0.8 * (str:{_strengthTrait.Id} + (2 * quality)) * sqrt(degree+1){randomPortion}"
			};
			_context.TraitExpressions.Add(smashDamage);
			_context.SaveChanges();

			var coupDamage = new TraitExpression
			{
				Name = "Animal Coup De Grace Damage",
				Expression = $"1.5 * str:{_strengthTrait.Id} * quality * sqrt(degree+1){randomPortion}"
			};
			_context.TraitExpressions.Add(coupDamage);
			_context.SaveChanges();

			_snakeBiteDamage = new TraitExpression
			{
				Name = "Snake Bite Damage",
				Expression = $"0.5 * (str:{_strengthTrait.Id} + (3 * quality)) * sqrt(degree+1){randomPortion}"
			};
			_context.TraitExpressions.Add(_snakeBiteDamage);
			_context.SaveChanges();

			var mouthshape = _context.BodypartShapes.First(x => x.Name == "Mouth");
			var clawShape = _context.BodypartShapes.First(x => x.Name == "Claw");
			var hoofShape = _context.BodypartShapes.First(x => x.Name == "Hoof");
			var shoulderShape = _context.BodypartShapes.First(x => x.Name == "Shoulder");
			var antlerShape = _context.BodypartShapes.First(x => x.Name == "Antler");
			var tuskShape = _context.BodypartShapes.First(x => x.Name == "Tusk");
			var hornShape = _context.BodypartShapes.First(x => x.Name == "Horn");
			var beakShape = _context.BodypartShapes.First(x => x.Name == "Beak");
			var talonShape = _context.BodypartShapes.First(x => x.Name == "Talon");
			var fangShape = _context.BodypartShapes.First(x => x.Name == "Fang");
			var mandibleShape = _context.BodypartShapes.First(x => x.Name == "Mandible");
			var headShape = _context.BodypartShapes.First(x => x.Name == "Head");
			var tailShape = _context.BodypartShapes.First(x => x.Name == "Tail");
			var tendrilShape = _context.BodypartShapes.First(x => x.Name == "Tendril");

			var attackAddendum =
				CombatSeederMessageStyleHelper.AttackSuffix(
					CombatSeederMessageStyleHelper.Parse(_questionAnswers["messagestyle"]));
			var defaultWater = _context.Liquids.FirstOrDefault(x => x.Name == "water") ?? _freshWaters.Last();
			var animalSpittle = _context.Liquids.FirstOrDefault(x => x.Name == "animal spittle");
			if (animalSpittle is null)
			{
				animalSpittle = new Liquid
				{
					Name = "animal spittle",
					Description = "spittle",
					LongDescription = "a cloudy, stringy animal spittle",
					TasteText = "It tastes rank and faintly salty.",
					VagueTasteText = "It tastes rank and faintly salty.",
					SmellText = "It smells of musk and stale saliva.",
					VagueSmellText = "It smells faintly musky.",
					TasteIntensity = 60,
					SmellIntensity = 30,
					AlcoholLitresPerLitre = 0,
					WaterLitresPerLitre = 0.98,
					FoodSatiatedHoursPerLitre = 0,
					DrinkSatiatedHoursPerLitre = 0,
					Viscosity = 1.2,
					Density = 1.0,
					Organic = true,
					ThermalConductivity = 0.609,
					ElectricalConductivity = 0.005,
					SpecificHeatCapacity = 4181,
					FreezingPoint = -2,
					BoilingPoint = 100,
					DisplayColour = "bold green",
					DampDescription = "It is slimed with spittle",
					WetDescription = "It is wet with spittle",
					DrenchedDescription = "It is drenched in spittle",
					DampShortDescription = "(slimed)",
					WetShortDescription = "(spat on)",
					DrenchedShortDescription = "(drenched in spit)",
					SolventId = defaultWater.Id,
					CountAsId = defaultWater.Id,
					SolventVolumeRatio = 1,
					InjectionConsequence = (int)LiquidInjectionConsequence.Harmful,
					ResidueVolumePercentage = 0.05
				};
				_context.Liquids.Add(animalSpittle);
				_context.SaveChanges();
			}

			string RangedAttackData(int rangeInRooms, RangedScatterType scatterType) =>
				new XElement("Data",
					new XElement("RangeInRooms", rangeInRooms),
					new XElement("ScatterType", scatterType.ToString())
				).ToString();

			string SpitAttackData(int rangeInRooms, RangedScatterType scatterType, long liquidId, double maximumQuantity) =>
				new XElement("Data",
					new XElement("RangeInRooms", rangeInRooms),
					new XElement("ScatterType", scatterType.ToString()),
					new XElement("Liquid", liquidId),
					new XElement("MaximumQuantity", maximumQuantity)
				).ToString();

			string BreathAttackData(int rangeInRooms, RangedScatterType scatterType, int additionalTargets,
				int bodypartsPerTarget, double igniteChance, string fireName, double damagePerTick, double painPerTick,
				double stunPerTick, double thermalLoadPerTick, double spreadChance, double minimumOxidation,
				bool selfOxidising) =>
				new XElement("Data",
					new XElement("RangeInRooms", rangeInRooms),
					new XElement("ScatterType", scatterType.ToString()),
					new XElement("AdditionalTargetLimit", additionalTargets),
					new XElement("BodypartsHitPerTarget", bodypartsPerTarget),
					new XElement("IgniteChance", igniteChance),
					new XElement("FireProfile",
						new XElement("Name", new XCData(fireName)),
						new XElement("DamageType", (int)DamageType.Burning),
						new XElement("DamagePerTick", damagePerTick),
						new XElement("PainPerTick", painPerTick),
						new XElement("StunPerTick", stunPerTick),
						new XElement("ThermalLoadPerTick", thermalLoadPerTick),
						new XElement("SpreadChance", spreadChance),
						new XElement("MinimumOxidation", minimumOxidation),
						new XElement("SelfOxidising", selfOxidising),
						new XElement("TickFrequencySeconds", 10),
						new XElement("ExtinguishTags")))
				.ToString();

			string ExplosiveAttackData(int rangeInRooms, RangedScatterType scatterType, SizeCategory explosionSize,
				Proximity maximumProximity) =>
				new XElement("Data",
					new XElement("RangeInRooms", rangeInRooms),
					new XElement("ScatterType", scatterType.ToString()),
					new XElement("ExplosionSize", explosionSize.ToString()),
					new XElement("MaximumProximity", maximumProximity.ToString())
				).ToString();

			string BuffetingAttackData(int rangeInRooms, RangedScatterType scatterType, int maximumPushDistance,
				double offensiveAdvantagePerDegree, double defensiveAdvantagePerDegree, bool inflictsDamage) =>
				new XElement("Data",
					new XElement("RangeInRooms", rangeInRooms),
					new XElement("ScatterType", scatterType.ToString()),
					new XElement("MaximumPushDistance", maximumPushDistance),
					new XElement("OffensiveAdvantagePerDegree", offensiveAdvantagePerDegree),
					new XElement("DefensiveAdvantagePerDegree", defensiveAdvantagePerDegree),
					new XElement("InflictsDamage", inflictsDamage)
				).ToString();

			_attacks["carnivorebite"] = AddAttack("Carnivore Bite", BuiltInCombatMoveType.NaturalWeaponAttack,
				MeleeWeaponVerb.Bite, Difficulty.Normal, Difficulty.Hard, Difficulty.Easy, Difficulty.Easy,
				Alignment.Front, Orientation.Centre, 4.0, 1.0, mouthshape, carnivoreBite,
				$"@ snap|snaps &0's jaws at $1{attackAddendum}", DamageType.Bite);
			_attacks["carnivoresmashbite"] = AddAttack("Carnivore Smash Bite",
				BuiltInCombatMoveType.UnarmedSmashItem, MeleeWeaponVerb.Bite, Difficulty.Normal, Difficulty.Hard,
				Difficulty.Easy, Difficulty.Easy, Alignment.Front, Orientation.Centre, 4.0, 1.0, mouthshape,
				carnivoreBite, $"@ lunge|lunges in and clamp|clamps &0's jaws onto $1{attackAddendum}", DamageType.Bite);
			_attacks["carnivorelowbite"] = AddAttack("Carnivore Low Bite",
				BuiltInCombatMoveType.NaturalWeaponAttack, MeleeWeaponVerb.Bite, Difficulty.Normal, Difficulty.Hard,
				Difficulty.Easy, Difficulty.VeryEasy, Alignment.Front, Orientation.Low, 4.0, 1.0, mouthshape,
				carnivoreBite, $"@ dart|darts low and snap|snaps at $1's legs{attackAddendum}",
				DamageType.Bite);
			_attacks["carnivorelowestbite"] = AddAttack("Carnivore Lowest Bite",
				BuiltInCombatMoveType.NaturalWeaponAttack, MeleeWeaponVerb.Bite, Difficulty.Hard, Difficulty.Hard,
				Difficulty.VeryEasy, Difficulty.Easy, Alignment.Front, Orientation.Lowest, 4.0, 0.9, mouthshape,
				carnivoreBite,
				$"@ surge|surges in ankle-low and snap|snaps at $1's feet in an attempt to drag &1 down{attackAddendum}",
				DamageType.Bite);
			_attacks["carnivorehighbite"] = AddAttack("Carnivore High Bite",
				BuiltInCombatMoveType.StaggeringBlowUnarmed, MeleeWeaponVerb.Bite, Difficulty.Normal,
				Difficulty.Normal, Difficulty.Hard, Difficulty.Easy, Alignment.Front, Orientation.High, 6.0, 1.3,
				mouthshape, carnivoreBite,
				$"@ spring|springs up at $1 and snap|snaps for &1's upper body{attackAddendum}",
				DamageType.Bite, additionalInfo: "7");
			_attacks["carnivoreclinchbite"] = AddAttack("Carnivore Clinch Bite",
				BuiltInCombatMoveType.ClinchUnarmedAttack, MeleeWeaponVerb.Bite, Difficulty.VeryEasy,
				Difficulty.Hard, Difficulty.Easy, Difficulty.Easy, Alignment.Front, Orientation.Centre, 3.0, 0.6,
				mouthshape, carnivoreBite, $"@ wrench|wrenches &0's head sideways and try|tries to savage $1 with a close bite{attackAddendum}", DamageType.Bite);
			_attacks["carnivoreclinchhighbite"] = AddAttack("Carnivore High Clinch Bite",
				BuiltInCombatMoveType.ClinchUnarmedAttack, MeleeWeaponVerb.Bite, Difficulty.VeryEasy,
				Difficulty.Hard, Difficulty.Easy, Difficulty.Easy, Alignment.Front, Orientation.High, 3.0, 0.6,
				mouthshape, carnivoreBite, $"@ crane|cranes &0's head up and snap|snaps for $1's throat{attackAddendum}", DamageType.Bite);
			_attacks["carnivoreclinchhighestbite"] = AddAttack("Carnivore Highest Clinch Bite",
				BuiltInCombatMoveType.ClinchUnarmedAttack, MeleeWeaponVerb.Bite, Difficulty.VeryEasy,
				Difficulty.Hard, Difficulty.Easy, Difficulty.Easy, Alignment.Front, Orientation.Highest, 3.0, 0.6,
				mouthshape, carnivoreBite, $"@ surge|surges up and snap|snaps for $1's face{attackAddendum}", DamageType.Bite);
			_attacks["carnivoredownbite"] = AddAttack("Carnivore Downed Bite",
				BuiltInCombatMoveType.DownedAttackUnarmed, MeleeWeaponVerb.Bite, Difficulty.VeryEasy,
				Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Alignment.Front, Orientation.High, 2.0, 0.5,
				mouthshape, carnivoreBite,
				$"@ savage|savages $1 with a bite while #1 %1|are|is down{attackAddendum}", DamageType.Bite,
				additionalInfo: ((int)Difficulty.ExtremelyEasy).ToString());

			_attacks["herbivorebite"] = AddAttack("Herbivore Bite", BuiltInCombatMoveType.ClinchUnarmedAttack,
				MeleeWeaponVerb.Bite, Difficulty.VeryEasy, Difficulty.Hard, Difficulty.Easy, Difficulty.Easy,
				Alignment.Front, Orientation.Centre, 3.0, 1.0, mouthshape, herbivoreBite,
				$"@ nip|nips at $1{attackAddendum}", DamageType.Bite);
			_attacks["herbivoresmashbite"] = AddAttack("Herbivore Smash Bite",
				BuiltInCombatMoveType.UnarmedSmashItem, MeleeWeaponVerb.Bite, Difficulty.VeryEasy, Difficulty.Hard,
				Difficulty.Easy, Difficulty.Easy, Alignment.Front, Orientation.Centre, 3.0, 1.0, mouthshape,
				herbivoreBite, $"@ lunge|lunges in and bite|bites at $1{attackAddendum}", DamageType.Bite);

			_attacks["smallbite"] = AddAttack("Small Animal Bite", BuiltInCombatMoveType.ClinchUnarmedAttack,
				MeleeWeaponVerb.Bite, Difficulty.VeryEasy, Difficulty.Normal, Difficulty.Easy, Difficulty.Easy,
				Alignment.Front, Orientation.Low, 2.0, 0.5, mouthshape, smallBite,
				$"@ scamper|scampers in and nip|nips at $1{attackAddendum}", DamageType.Bite);
			_attacks["smallsmashbite"] = AddAttack("Small Animal Smash Bite",
				BuiltInCombatMoveType.UnarmedSmashItem, MeleeWeaponVerb.Bite, Difficulty.VeryEasy,
				Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Alignment.Front, Orientation.Low, 2.0, 0.5,
				mouthshape, smallBite, $"@ dart|darts in and bite|bites at $1{attackAddendum}", DamageType.Bite);
			_attacks["smalllowbite"] = AddAttack("Small Animal Low Bite", BuiltInCombatMoveType.ClinchUnarmedAttack,
				MeleeWeaponVerb.Bite, Difficulty.VeryEasy, Difficulty.Normal, Difficulty.Easy, Difficulty.Easy,
				Alignment.Front, Orientation.Lowest, 2.0, 0.5, mouthshape, smallBite,
				$"@ scamper|scampers low and snap|snaps at $1's ankles{attackAddendum}", DamageType.Bite);
			_attacks["smalldownedbite"] = AddAttack("Small Animal Downed Bite",
				BuiltInCombatMoveType.DownedAttackUnarmed, MeleeWeaponVerb.Bite, Difficulty.VeryEasy,
				Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Alignment.Front, Orientation.High, 2.0, 0.5,
				mouthshape, smallBite, $"@ worry|worries at $1 with quick bites while #1 %1|are|is down{attackAddendum}",
				DamageType.Bite, additionalInfo: ((int)Difficulty.Trivial).ToString());

			_attacks["clawswipe"] = AddAttack("Claw Swipe", BuiltInCombatMoveType.NaturalWeaponAttack,
				MeleeWeaponVerb.Swipe, Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.Easy,
				Alignment.FrontRight, Orientation.High, 4.0, 1.3, clawShape, clawDamage,
				$"@ rear|rears up and rake|rakes &0's {{0}} across $1{attackAddendum}", DamageType.Claw);
			_attacks["clawsmashswipe"] = AddAttack("Claw Swipe Smash", BuiltInCombatMoveType.UnarmedSmashItem,
				MeleeWeaponVerb.Swipe, Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.Easy,
				Alignment.FrontRight, Orientation.High, 4.0, 1.3, clawShape, clawDamage,
				$"@ rear|rears up and rake|rakes &0's {{0}} hard across $1{attackAddendum}", DamageType.Claw);
			_attacks["clawhighswipe"] = AddAttack("Claw High Swipe", BuiltInCombatMoveType.NaturalWeaponAttack,
				MeleeWeaponVerb.Swipe, Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.Easy,
				Alignment.FrontRight, Orientation.Highest, 4.0, 1.3, clawShape, clawDamage,
				$"@ rear|rears up and slash|slashes high at $1 with &0's {{0}}{attackAddendum}", DamageType.Claw);
			_attacks["clawlowswipe"] = AddAttack("Claw Low Swipe", BuiltInCombatMoveType.NaturalWeaponAttack,
				MeleeWeaponVerb.Swipe, Difficulty.Normal, Difficulty.VeryEasy, Difficulty.Easy, Difficulty.Easy,
				Alignment.FrontRight, Orientation.Low, 4.0, 1.3, clawShape, clawDamage,
				$"@ crouch|crouches low and rake|rakes at $1's legs with &0's {{0}}{attackAddendum}", DamageType.Claw);

			_attacks["hoofstomp"] = AddAttack("Hoof Stomp", BuiltInCombatMoveType.DownedAttackUnarmed,
				MeleeWeaponVerb.Kick, Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.Easy,
				Alignment.Front, Orientation.High, 5.0, 1.3, hoofShape, smashDamage,
				$"@ rear|rears and stamp|stamps at $1 while #1 %1|are|is down{attackAddendum}", DamageType.Crushing,
				additionalInfo: ((int)Difficulty.Hard).ToString());
			_attacks["hoofstompsmash"] = AddAttack("Hoof Stomp Smash", BuiltInCombatMoveType.UnarmedSmashItem,
				MeleeWeaponVerb.Kick, Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.Easy,
				Alignment.Front, Orientation.High, 5.0, 1.3, hoofShape, smashDamage,
				$"@ hammer|hammers at $1 with stomping blows while #1 %1|are|is down{attackAddendum}", DamageType.Crushing,
				additionalInfo: ((int)Difficulty.Hard).ToString());
			_attacks["barge"] = AddAttack("Animal Barge", BuiltInCombatMoveType.StaggeringBlowUnarmed,
				MeleeWeaponVerb.Bash, Difficulty.Normal, Difficulty.Easy, Difficulty.Insane, Difficulty.VeryHard,
				Alignment.Front, Orientation.Centre, 8.0, 1.8, shoulderShape, smashDamage,
				$"@ charge|charges forward and throw|throws &0's bulk at $1{attackAddendum}", DamageType.Crushing,
				additionalInfo: ((int)Difficulty.VeryHard).ToString());
			_attacks["bargesmash"] = AddAttack("Animal Barge Smash", BuiltInCombatMoveType.UnarmedSmashItem,
				MeleeWeaponVerb.Bash, Difficulty.Normal, Difficulty.Easy, Difficulty.Insane, Difficulty.VeryHard,
				Alignment.Front, Orientation.Centre, 8.0, 1.8, shoulderShape, smashDamage,
				$"@ charge|charges forward and throw|throws &0's bulk at $1{attackAddendum}", DamageType.Crushing,
				additionalInfo: ((int)Difficulty.VeryHard).ToString());
			_attacks["clinchbarge"] = AddAttack("Animal Clinch Barge", BuiltInCombatMoveType.StaggeringBlowUnarmed,
				MeleeWeaponVerb.Bash, Difficulty.Normal, Difficulty.Easy, Difficulty.Insane, Difficulty.VeryHard,
				Alignment.Front, Orientation.Centre, 8.0, 1.2, shoulderShape, smashDamage,
				$"@ thrash|thrashes and slam|slams &0's weight into $1{attackAddendum}", DamageType.Crushing,
				additionalInfo: ((int)Difficulty.Hard).ToString());

			_attacks["gorehorn"] = AddAttack("Horn Gore", BuiltInCombatMoveType.NaturalWeaponAttack,
				MeleeWeaponVerb.Stab, Difficulty.Easy, Difficulty.Easy, Difficulty.Hard, Difficulty.Easy,
				Alignment.Front, Orientation.High, 5.0, 1.3, hornShape, smashDamage,
				$"@ lower|lowers &0's head and drive|drives &0's horns toward $1{attackAddendum}",
				DamageType.Piercing);
			_attacks["goreantler"] = AddAttack("Antler Gore", BuiltInCombatMoveType.NaturalWeaponAttack,
				MeleeWeaponVerb.Stab, Difficulty.Easy, Difficulty.Easy, Difficulty.Hard, Difficulty.Easy,
				Alignment.Front, Orientation.High, 5.0, 1.3, antlerShape, smashDamage,
				$"@ dip|dips &0's antlers and hook|hooks them toward $1{attackAddendum}",
				DamageType.Piercing);
			_attacks["goretusk"] = AddAttack("Tusk Gore", BuiltInCombatMoveType.NaturalWeaponAttack,
				MeleeWeaponVerb.Stab, Difficulty.Easy, Difficulty.Easy, Difficulty.Hard, Difficulty.Easy,
				Alignment.Front, Orientation.High, 5.0, 1.3, tuskShape, smashDamage,
				$"@ surge|surges forward and drive|drives &0's tusks at $1{attackAddendum}",
				DamageType.Piercing);
			_attacks["tusksweep"] = AddAttack("Tusk Sweep", BuiltInCombatMoveType.UnbalancingBlowUnarmed,
				MeleeWeaponVerb.Sweep, Difficulty.Hard, Difficulty.Easy, Difficulty.ExtremelyHard, Difficulty.Hard,
				Alignment.FrontRight, Orientation.Low, 5.0, 1.5, tuskShape, smashDamage,
				$"@ sweep|sweeps &0's tusks low across $1 in an attempt to topple &1{attackAddendum}",
				DamageType.Crushing, additionalInfo: ((int)Difficulty.Hard).ToString());

			_attacks["fishbite"] = AddAttack("Fish Bite", BuiltInCombatMoveType.ClinchUnarmedAttack,
				MeleeWeaponVerb.Bite, Difficulty.VeryEasy, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard,
				Alignment.Front, Orientation.Low, 2.0, 0.5, mouthshape, fishBite,
				$"@ dart|darts in and snap|snaps at $1{attackAddendum}", DamageType.Bite);
			_attacks["fishquickbite"] = AddAttack("Fish Quick Bite", BuiltInCombatMoveType.ClinchUnarmedAttack,
				MeleeWeaponVerb.Bite, Difficulty.Normal, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard,
				Alignment.Front, Orientation.Low, 2.0, 0.3, mouthshape, fishBite,
				$"@ flash|flashes in and nip|nips at $1{attackAddendum}", DamageType.Bite);
			_attacks["bite"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Bite") ??
				AddAttack("Bite", BuiltInCombatMoveType.ClinchUnarmedAttack,
					MeleeWeaponVerb.Bite, Difficulty.VeryHard, Difficulty.Normal, Difficulty.ExtremelyHard,
					Difficulty.VeryHard, Alignment.Front, Orientation.High, 3.0, 1.4, mouthshape, carnivoreBite,
					$"@ lean|leans in and try|tries to bite $1{attackAddendum}", DamageType.Bite);
			_attacks["sharkbite"] = AddAttack("Shark Bite", BuiltInCombatMoveType.NaturalWeaponAttack,
				MeleeWeaponVerb.Bite, Difficulty.Normal, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard,
				Alignment.Front, Orientation.Centre, 4.0, 1.0, mouthshape, sharkBite,
				$"@ surge|surges in and bite|bites down on $1{attackAddendum}", DamageType.Bite);
			_attacks["sharkreelbite"] = AddAttack("Shark Reel Bite", BuiltInCombatMoveType.StaggeringBlowUnarmed,
				MeleeWeaponVerb.Bite, Difficulty.Normal, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard,
				Alignment.Front, Orientation.Centre, 5.0, 1.2, mouthshape, sharkBite,
				$"@ surge|surges in, bite|bites down on $1, and wrench|wrenches away{attackAddendum}", DamageType.Bite,
				additionalInfo: ((int)Difficulty.VeryHard).ToString());

			_attacks["crabpinch"] = AddAttack("Crab Pinch", BuiltInCombatMoveType.NaturalWeaponAttack,
				MeleeWeaponVerb.Swipe, Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.Easy,
				Alignment.FrontRight, Orientation.Low, 4.0, 1.3, clawShape, clawDamage,
				$"@ snap|snaps &0's {{0}} shut on $1{attackAddendum}", DamageType.Shearing);
			_attacks["beakpeck"] = AddAttack("Beak Peck", BuiltInCombatMoveType.NaturalWeaponAttack,
				MeleeWeaponVerb.Stab, Difficulty.Easy, Difficulty.Normal, Difficulty.Hard, Difficulty.Easy,
				Alignment.Front, Orientation.High, 2.5, 0.7, beakShape, peckDamage,
				$"@ dart|darts forward and peck|pecks sharply at $1 with &0's {{0}}{attackAddendum}", DamageType.Piercing);
			_attacks["beakbite"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Beak Bite") ??
				AddAttack("Beak Bite", BuiltInCombatMoveType.ClinchUnarmedAttack,
					MeleeWeaponVerb.Bite, Difficulty.Normal, Difficulty.Hard, Difficulty.ExtremelyHard, Difficulty.Easy,
					Alignment.Front, Orientation.High, 2.5, 0.6, beakShape, peckDamage,
					$"@ dart|darts in close and jab|jabs &0's beak into $1{attackAddendum}", DamageType.Piercing);
			_attacks["talonstrike"] = AddAttack("Talon Strike", BuiltInCombatMoveType.NaturalWeaponAttack,
				MeleeWeaponVerb.Claw, Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.Easy,
				Alignment.FrontRight, Orientation.Low, 3.5, 0.9, talonShape, talonDamage,
				$"@ rake|rakes at $1 with &0's {{0}}{attackAddendum}", DamageType.Claw);
			_attacks["headbutt"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Headbutt") ??
				AddAttack("Headbutt", BuiltInCombatMoveType.StaggeringBlowClinch,
					MeleeWeaponVerb.Strike, Difficulty.VeryHard, Difficulty.VeryHard, Difficulty.ExtremelyHard,
					Difficulty.VeryHard, Alignment.Front, Orientation.Highest, 5.0, 1.0, headShape, ramDamage,
					$"@ jerk|jerks forward and crack|cracks &0's head into $1{attackAddendum}", DamageType.Crushing,
					additionalInfo: ((int)Difficulty.Hard).ToString());
			_attacks["fangbite"] = AddAttack("Fang Bite", BuiltInCombatMoveType.ClinchUnarmedAttack,
				MeleeWeaponVerb.Bite, Difficulty.Normal, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard,
				Alignment.Front, Orientation.Low, 3.0, 0.7, fangShape, _snakeBiteDamage,
				$"@ dart|darts in and sink|sinks &0's {{0}} into $1{attackAddendum}", DamageType.Bite);
			_attacks["mandiblebite"] = AddAttack("Mandible Bite", BuiltInCombatMoveType.ClinchUnarmedAttack,
				MeleeWeaponVerb.Bite, Difficulty.VeryEasy, Difficulty.Normal, Difficulty.Hard, Difficulty.Easy,
				Alignment.Front, Orientation.Centre, 1.5, 0.4, mandibleShape, mandibleDamage,
				$"@ clamp|clamps &0's {{0}} on $1{attackAddendum}", DamageType.Shearing);
			_attacks["clawclamp"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Claw Clamp") ??
				AddAttack("Claw Clamp", BuiltInCombatMoveType.ClinchUnarmedAttack,
					MeleeWeaponVerb.Claw, Difficulty.Easy, Difficulty.Normal, Difficulty.ExtremelyHard, Difficulty.Easy,
					Alignment.FrontRight, Orientation.Centre, 2.5, 0.7, clawShape, mandibleDamage,
					$"@ clamp|clamps &0's {{0}} onto $1{attackAddendum}", DamageType.Shearing);
			_attacks["arachnidclaw"] = AddAttack("Arachnid Claw", BuiltInCombatMoveType.NaturalWeaponAttack,
				MeleeWeaponVerb.Swipe, Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.Easy,
				Alignment.FrontRight, Orientation.Centre, 2.5, 0.8, clawShape, clawDamage,
				$"@ rake|rakes $1 with &0's {{0}}{attackAddendum}", DamageType.Claw);
			_attacks["headram"] = AddAttack("Head Ram", BuiltInCombatMoveType.StaggeringBlowUnarmed,
				MeleeWeaponVerb.Strike, Difficulty.Normal, Difficulty.Easy, Difficulty.Insane, Difficulty.VeryHard,
				Alignment.Front, Orientation.High, 5.0, 1.1, headShape, ramDamage,
				$"@ lunge|lunges in and slam|slams &0's head into $1{attackAddendum}", DamageType.Crushing,
				additionalInfo: ((int)Difficulty.Hard).ToString());
			_attacks["tailslap"] = AddAttack("Tail Slap", BuiltInCombatMoveType.StaggeringBlowUnarmed,
				MeleeWeaponVerb.Sweep, Difficulty.Normal, Difficulty.Easy, Difficulty.Hard, Difficulty.Hard,
				Alignment.Rear, Orientation.Centre, 4.5, 1.1, tailShape, ramDamage,
				$"@ whip|whips &0's tail across $1{attackAddendum}", DamageType.Crushing,
				additionalInfo: ((int)Difficulty.Normal).ToString());
			_attacks["tendrillash"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Tendril Lash") ??
				AddAttack("Tendril Lash", BuiltInCombatMoveType.NaturalWeaponAttack,
					MeleeWeaponVerb.Sweep, Difficulty.Easy, Difficulty.Easy, Difficulty.Hard, Difficulty.Easy,
					Alignment.Front, Orientation.Centre, 3.5, 0.5, tendrilShape, peckDamage,
					$"@ lash|lashes a tendril at $1{attackAddendum}", DamageType.Cellular);
			_attacks["llamaspit"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Llama Spit") ??
				AddAttack("Llama Spit", BuiltInCombatMoveType.SpitNaturalAttack,
					MeleeWeaponVerb.Blast, Difficulty.Normal, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard,
					Alignment.Front, Orientation.High, 2.0, 0.9, mouthshape, peckDamage,
					$"@ rear|rears back and spit|spits a foul gobbet at $1{attackAddendum}", DamageType.Chemical,
					intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Disadvantage,
					additionalInfo: SpitAttackData(1, RangedScatterType.Arcing, animalSpittle.Id, 0.025));
			_attacks["dragonfirebreath"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Dragonfire Breath") ??
				AddAttack("Dragonfire Breath", BuiltInCombatMoveType.BreathWeaponAttack,
					MeleeWeaponVerb.Blast, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard,
					Alignment.Front, Orientation.High, 10.0, 1.6, mouthshape, smashDamage,
					$"@ rear|rears up and unleash|unleashes a roaring cone of dragonfire at $1{attackAddendum}", DamageType.Burning,
					intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Burning | CombatMoveIntentions.Hard | CombatMoveIntentions.Slow,
					additionalInfo: BreathAttackData(2, RangedScatterType.Light, 3, 2, 0.35, "Dragonfire", 0.45, 0.35, 0.1, 2.5, 0.2, 0.05, true));
			_attacks["wingbuffet"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Wing Buffet") ??
				AddAttack("Wing Buffet", BuiltInCombatMoveType.BuffetingNaturalAttack,
					MeleeWeaponVerb.Sweep, Difficulty.Normal, Difficulty.Normal, Difficulty.Hard, Difficulty.Hard,
					Alignment.Front, Orientation.Centre, 5.0, 1.2, shoulderShape, ramDamage,
					$"@ beat|beats &0's wings and buffet|buffets $1 with a crashing gust{attackAddendum}", DamageType.Crushing,
					intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Advantage | CombatMoveIntentions.Disadvantage,
					additionalInfo: BuffetingAttackData(1, RangedScatterType.Light, 1, 0.1, 0.15, true));
			_attacks["tailspike"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Tail Spike") ??
				AddAttack("Tail Spike", BuiltInCombatMoveType.RangedNaturalAttack,
					MeleeWeaponVerb.Stab, Difficulty.Normal, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard,
					Alignment.Rear, Orientation.Centre, 3.0, 0.8, tailShape, peckDamage,
					$"@ snap|snaps &0's tail and launch|launches a wicked spike at $1{attackAddendum}", DamageType.Piercing,
					intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Fast,
					additionalInfo: RangedAttackData(2, RangedScatterType.Ballistic));
			_attacks["bombardierspray"] = _context.WeaponAttacks.FirstOrDefault(x => x.Name == "Bombardier Spray") ??
				AddAttack("Bombardier Spray", BuiltInCombatMoveType.ExplosiveNaturalAttack,
					MeleeWeaponVerb.Blast, Difficulty.Normal, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard,
					Alignment.Rear, Orientation.Centre, 2.0, 0.8, mandibleShape, peckDamage,
					$"@ vent|vents a crackling chemical burst toward $1{attackAddendum}", DamageType.Burning,
					intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Burning | CombatMoveIntentions.Fast,
					additionalInfo: ExplosiveAttackData(1, RangedScatterType.Arcing, SizeCategory.VerySmall, Proximity.Immediate));
		}
	}

	private void CreateRaceAttacks(Race race)
	{
		if (TryApplyTemplateRaceAttacks(race))
		{
			return;
		}

		switch (race.Name)
		{
			case "Python":
			case "Tree Python":
			case "Boa":
			case "Anaconda":
				AddSerpentAttack(race, false);
				break;
			case "Cobra":
			case "Adder":
			case "Rattlesnake":
			case "Viper":
			case "Mamba":
			case "Coral Snake":
			case "Moccasin":
				AddSerpentAttack(race, true);
				break;
			case "Mouse":
			case "Rat":
			case "Guinea Pig":
			case "Hamster":
			case "Ferret":
			case "Rabbit":
			case "Hare":
				AddAttacks(race, ItemQuality.ExtremelyBad, smallAnimalBites: true);
				break;
			case "Otter":
			case "Fox":
				AddAttacks(race, ItemQuality.Standard, smallAnimalBites: true);
				AddAttacks(race, ItemQuality.Poor, clawAttacks: true);
				break;
			case "Beaver":
				AddAttacks(race, ItemQuality.Substandard, true);
				AddAttacks(race, ItemQuality.Standard, clawAttacks: true);
				break;
			case "Cat":
				AddAttacks(race, ItemQuality.Bad, true);
				AddAttacks(race, ItemQuality.Bad, clawAttacks: true);
				break;
			case "Dog":
				AddAttacks(race, ItemQuality.Poor, true);
				AddAttacks(race, ItemQuality.Bad, clawAttacks: true);
				break;
			case "Wolf":
			case "Coyote":
			case "Hyena":
			case "Jackal":
			case "Cheetah":
			case "Leopard":
			case "Panther":
			case "Jaguar":
				AddAttacks(race, ItemQuality.Standard, true);
				AddAttacks(race, ItemQuality.Substandard, clawAttacks: true);
				break;
			case "Lion":
			case "Tiger":
				AddAttacks(race, ItemQuality.VeryGood, true);
				AddAttacks(race, ItemQuality.Standard, clawAttacks: true);
				break;
			case "Wolverine":
			case "Badger":
				AddAttacks(race, ItemQuality.Poor, true);
				AddAttacks(race, ItemQuality.Standard, clawAttacks: true);
				break;
			case "Bear":
				AddAttacks(race, ItemQuality.VeryGood, true);
				AddAttacks(race, ItemQuality.VeryGood, clawAttacks: true);
				AddAttacks(race, ItemQuality.Standard, chargeAttacks: true);
				break;
			case "Goat":
				AddAttacks(race, ItemQuality.Standard, herbivoreBites: true);
				AddAttacks(race, ItemQuality.Poor, chargeAttacks: true);
				AddAttacks(race, ItemQuality.Substandard, hornAttacks: true);
				break;
			case "Sheep":
			case "Pig":
			case "Llama":
			case "Alpaca":
			case "Giraffe":
				AddAttacks(race, ItemQuality.Standard, herbivoreBites: true);
				AddAttacks(race, ItemQuality.Poor, chargeAttacks: true);
				break;
			case "Boar":
			case "Warthog":
				AddAttacks(race, ItemQuality.Good, herbivoreBites: true);
				AddAttacks(race, ItemQuality.Standard, chargeAttacks: true);
				AddAttacks(race, ItemQuality.Substandard, tuskAttacks: true);
				break;
			case "Horse":
				AddAttacks(race, ItemQuality.Good, herbivoreBites: true);
				AddAttacks(race, ItemQuality.Good, chargeAttacks: true);
				break;
			case "Deer":
				AddAttacks(race, ItemQuality.Standard, herbivoreBites: true);
				AddAttacks(race, ItemQuality.Poor, chargeAttacks: true);
				AddAttacks(race, ItemQuality.Standard, antlerAttacks: true);
				break;
			case "Moose":
				AddAttacks(race, ItemQuality.Good, herbivoreBites: true);
				AddAttacks(race, ItemQuality.Substandard, chargeAttacks: true);
				AddAttacks(race, ItemQuality.VeryGood, antlerAttacks: true);
				break;
			case "Cow":
			case "Ox":
			case "Buffalo":
			case "Bison":
				AddAttacks(race, ItemQuality.Good, herbivoreBites: true);
				AddAttacks(race, ItemQuality.VeryGood, chargeAttacks: true);
				AddAttacks(race, ItemQuality.Standard, hornAttacks: true);
				break;
			case "Hippopotamus":
				AddAttacks(race, ItemQuality.Great, true);
				AddAttacks(race, ItemQuality.VeryGood, chargeAttacks: true);
				break;
			case "Rhinocerous":
				AddAttacks(race, ItemQuality.Poor, herbivoreBites: true);
				AddAttacks(race, ItemQuality.Great, chargeAttacks: true);
				AddAttacks(race, ItemQuality.Standard, hornAttacks: true);
				break;
			case "Elephant":
				AddAttacks(race, ItemQuality.Poor, herbivoreBites: true);
				AddAttacks(race, ItemQuality.Great, chargeAttacks: true);
				AddAttacks(race, ItemQuality.Great, tuskAttacks: true);
				break;
			case "Carp":
			case "Cod":
			case "Haddock":
			case "Koi":
			case "Pilchard":
			case "Perch":
			case "Herring":
			case "Mackerel":
			case "Anchovy":
			case "Sardine":
			case "Pollock":
			case "Salmon":
			case "Tuna":
				AddAttacks(race, ItemQuality.Substandard, fishBites: true);
				break;
			case "Shark":
				AddAttacks(race, ItemQuality.Great, sharkAttacks: true);
				break;
			case "Small Crab":
				AddAttacks(race, ItemQuality.Bad, crabAttacks: true);
				break;
			case "Crab":
			case "Lobster":
				AddAttacks(race, ItemQuality.Substandard, crabAttacks: true);
				break;
			case "Giant Crab":
				AddAttacks(race, ItemQuality.Great, crabAttacks: true);
				break;
			case "Jellyfish":
				AddJellyfishAttack(race);
				break;
			case "Octopus":
			case "Squid":
				AddAttacks(race, ItemQuality.Bad, fishBites: true);
				break;
			case "Giant Squid":
				AddAttacks(race, ItemQuality.Standard, fishBites: true);
				break;
			case "Sea Lion":
			case "Seal":
			case "Walrus":
				AddAttacks(race, ItemQuality.Poor, herbivoreBites: true);
				break;
			case "Dolphin":
			case "Porpoise":
				AddAttacks(race, ItemQuality.Standard, fishBites: true);
				break;
			case "Orca":
				AddAttacks(race, ItemQuality.Heroic, sharkAttacks: true);
				break;
			case "Baleen Whale":
			case "Toothed Whale":
				break;
			case "Pigeon":
				AddAttacks(race, ItemQuality.Bad, smallAnimalBites: true, clawAttacks: true);
				break;
			case "Parrot":
				AddAttacks(race, ItemQuality.Poor, smallAnimalBites: true, clawAttacks: true);
				break;
			case "Swallow":
				AddAttacks(race, ItemQuality.Bad, smallAnimalBites: true, clawAttacks: true);
				break;
			case "Sparrow":
				AddAttacks(race, ItemQuality.Bad, smallAnimalBites: true, clawAttacks: true);
				break;
			case "Quail":
				AddAttacks(race, ItemQuality.Terrible, smallAnimalBites: true, clawAttacks: true);
				break;
			case "Duck":
				AddAttacks(race, ItemQuality.Poor, smallAnimalBites: true, clawAttacks: true);
				break;
			case "Goose":
				AddAttacks(race, ItemQuality.Poor, smallAnimalBites: true, clawAttacks: true);
				break;
			case "Swan":
				AddAttacks(race, ItemQuality.Poor, smallAnimalBites: true, clawAttacks: true);
				break;
			case "Grouse":
				AddAttacks(race, ItemQuality.Poor, smallAnimalBites: true, clawAttacks: true);
				break;
			case "Pheasant":
				AddAttacks(race, ItemQuality.Poor, smallAnimalBites: true, clawAttacks: true);
				break;
			case "Chicken":
				AddAttacks(race, ItemQuality.Poor, smallAnimalBites: true, clawAttacks: true);
				break;
			case "Turkey":
				AddAttacks(race, ItemQuality.Poor, smallAnimalBites: true, clawAttacks: true);
				break;
			case "Seagull":
				AddAttacks(race, ItemQuality.Poor, smallAnimalBites: true, clawAttacks: true);
				break;
			case "Albatross":
				AddAttacks(race, ItemQuality.Poor, smallAnimalBites: true, clawAttacks: true);
				break;
			case "Heron":
				AddAttacks(race, ItemQuality.Poor, smallAnimalBites: true, clawAttacks: true);
				break;
			case "Crane":
				AddAttacks(race, ItemQuality.Poor, smallAnimalBites: true, clawAttacks: true);
				break;
			case "Flamingo":
				AddAttacks(race, ItemQuality.Poor, smallAnimalBites: true, clawAttacks: true);
				break;
			case "Peacock":
				AddAttacks(race, ItemQuality.Poor, smallAnimalBites: true, clawAttacks: true);
				break;
			case "Ibis":
				AddAttacks(race, ItemQuality.Poor, smallAnimalBites: true, clawAttacks: true);
				break;
			case "Pelican":
				AddAttacks(race, ItemQuality.Poor, smallAnimalBites: true, clawAttacks: true);
				break;
			case "Crow":
				AddAttacks(race, ItemQuality.Poor, smallAnimalBites: true, clawAttacks: true);
				break;
			case "Raven":
				AddAttacks(race, ItemQuality.Poor, smallAnimalBites: true, clawAttacks: true);
				break;
			case "Emu":
				AddAttacks(race, ItemQuality.Substandard, smallAnimalBites: true, clawAttacks: true);
				break;
			case "Ostrich":
				AddAttacks(race, ItemQuality.Substandard, smallAnimalBites: true, clawAttacks: true);
				break;
			case "Moa":
				AddAttacks(race, ItemQuality.Substandard, smallAnimalBites: true, clawAttacks: true);
				break;
			case "Vulture":
				AddAttacks(race, ItemQuality.Standard, smallAnimalBites: true, clawAttacks: true);
				break;
			case "Hawk":
				AddAttacks(race, ItemQuality.Standard, smallAnimalBites: true, clawAttacks: true);
				break;
			case "Eagle":
				AddAttacks(race, ItemQuality.Good, smallAnimalBites: true, clawAttacks: true);
				break;
			case "Falcon":
				AddAttacks(race, ItemQuality.Standard, smallAnimalBites: true, clawAttacks: true);
				break;
			case "Woodpecker":
				AddAttacks(race, ItemQuality.Standard, smallAnimalBites: true, clawAttacks: true);
				break;
			case "Owl":
				AddAttacks(race, ItemQuality.Standard, smallAnimalBites: true, clawAttacks: true);
				break;
			case "Kingfisher":
				AddAttacks(race, ItemQuality.Standard, smallAnimalBites: true, clawAttacks: true);
				break;
			case "Stork":
				AddAttacks(race, ItemQuality.Poor, smallAnimalBites: true, clawAttacks: true);
				break;
			case "Penguin":
				AddAttacks(race, ItemQuality.Poor, smallAnimalBites: true, clawAttacks: true);
				break;
		}
	}

	#endregion

	#region Descriptions
	private void CreateDescription(EntityDescriptionType type, string text, FutureProg prog)
	{
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			ApplicabilityProg = prog,
			Type = (int)type,
			RelativeWeight = 100,
			Pattern = text
		});
	}

	private void DogLongDescriptions(Race race)
	{
		void AddDogDescription(Ethnicity ethnicity, string description)
		{
			var prog = new FutureProg
			{
				FunctionName = $"IsDog{ethnicity.Name.CollapseString()}",
				FunctionComment =
					$"Determines whether a character or character template is a dog of the {ethnicity.Name} breed",
				AcceptsAnyParameters = false,
				Category = "Character",
				Subcategory = "Descriptions",
				ReturnType = (long)ProgVariableTypes.Boolean,
				FunctionText =
					$"return @ch.Race == ToRace(\"{race.Name}\") and @ch.Ethnicity == ToEthnicity({ethnicity.Id})"
			};
			prog.FutureProgsParameters.Add(new FutureProgsParameter
			{
				FutureProg = prog,
				ParameterIndex = 0,
				ParameterName = "ch",
				ParameterType = (long)ProgVariableTypes.Toon
			});
			_context.FutureProgs.Add(prog);

			CreateDescription(EntityDescriptionType.FullDescription, description, prog);
		}

		Ethnicity Breed(string name) =>
			_context.Ethnicities.First(x => x.ParentRaceId == race.Id && x.Name == name);

		AddDogDescription(Breed("Terrier"),
			"This is &a_an[&age], &male terrier; a small, wiry and generally fearless breed of dog with short legs and an eager nature. They are excellent diggers and have a natural instinct to hunt vermin.");
		AddDogDescription(Breed("Setter"),
			"This is &a_an[&age], &male setter; a medium sized hunting dog that was bred to hunt birds. They hold their head high in the air as they move rather than tracking along the ground and they have an instinct to freeze or 'set' when they see their prey. They are readily trainable and very disciplined.");
		AddDogDescription(Breed("Pointer"),
			"This is &a_an[&age], &male pointer; a medium sized hunting dog that was bred to assist hunters using ranged weapons. They stalk prey in dense vegetation and upon sighting it will stand still in a 'point' with their muzzle at their prey.");
		AddDogDescription(Breed("Retriever"),
			"This is &a_an[&age], &male retriever; a medium sized hunting dog that was bred to bring back downed prey. They are loyal and friendly in disposition and relatively soft-mouthed, so make excellent pets.");
		AddDogDescription(Breed("Spaniel"),
			"This is &a_an[&age], &male spaniel; a small hunting dog bred for flushing game out of dense brush. They have distinctive long, silky coats and big droopy ears.");
		AddDogDescription(Breed("Water Dog"),
			"This is &a_an[&age], &male water dog; hunting and companion dogs that are excellent swimmers. They have long and thick hair around their torso to keep them warm in icy-cold water but short-coated limbs to reduce drag while swimming.");
		AddDogDescription(Breed("Sighthound"),
			"This is &a_an[&age], &male sighthound; a large, long-legged and lanky breed of dog with tremendous speed, flexibility and agility. They have long muzzles and flexible backs.");
		AddDogDescription(Breed("Scenthound"),
			"This is &a_an[&age], &male scenthound; a dog with a phenomenal sense of smell, even for their species. These dogs are short-legged and low to the ground, and have big, floppy ears and wet mouths.");
		AddDogDescription(Breed("Bulldog"),
			"This is &a_an[&age], &male bulldog; a stocky, square-bodied dog originally bred for fighting. They have flat faces and tremendous muscle.");
		AddDogDescription(Breed("Mastiff"),
			"This is &a_an[&age], &male mastiff; a large dog originally bred for guarding homes and for war. They have broad, somewhat flat faces and big feet.");
		AddDogDescription(Breed("Herding Dog"),
			"This is &a_an[&age], &male herding dog; a medium sized dog bred to assist in the herding of livestock. They tend to be low to the ground and agile, and have an instinct to nip at the heels of animals and herd small children.");
		AddDogDescription(Breed("Lap Dog"),
			"This is &a_an[&age], &male lap dog; a small, lap-sized dog with little use as a working dog. Purely ornamental, they tend to have a lazy but grumpy disposition and prefer to spend their time in the laps of their owners.");
		AddDogDescription(Breed("Mongrel"),
			"This is &a_an[&age], &male mongrel; a dog of indeterminate parentage and medium size.");
	}

	private void CreateDescriptionsForRace(Race race)
	{
		#region Progs

		var isRaceProg = new FutureProg
		{
			FunctionName = $"Is{race.Name.CollapseString()}",
			FunctionComment = $"Determines whether a character or character template is a {race.Name}",
			AcceptsAnyParameters = false,
			Category = "Character",
			Subcategory = "Descriptions",
			ReturnType = (long)ProgVariableTypes.Boolean,
			FunctionText = $"return @ch.Race == ToRace(\"{race.Name}\")"
		};
		isRaceProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = isRaceProg,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterType = (long)ProgVariableTypes.Toon
		});
		_context.FutureProgs.Add(isRaceProg);

		var isAdultMaleRaceProg = new FutureProg
		{
			FunctionName = $"Is{race.Name.CollapseString()}AdultMale",
			FunctionComment = $"Determines whether a character or character template is an adult male {race.Name}",
			AcceptsAnyParameters = false,
			Category = "Character",
			Subcategory = "Descriptions",
			ReturnType = (long)ProgVariableTypes.Boolean,
			FunctionText =
				$"return @ch.Race == ToRace(\"{race.Name}\") and @ch.Gender == ToGender(\"Male\") and In(@ch.AgeCategory, \"YoungAdult\", \"Adult\", \"Elder\", \"Venerable\")"
		};
		isAdultMaleRaceProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = isAdultMaleRaceProg,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterType = (long)ProgVariableTypes.Toon
		});
		_context.FutureProgs.Add(isAdultMaleRaceProg);

		var isAdultFemaleRaceProg = new FutureProg
		{
			FunctionName = $"Is{race.Name.CollapseString()}AdultFemale",
			FunctionComment =
				$"Determines whether a character or character template is an adult female {race.Name}",
			AcceptsAnyParameters = false,
			Category = "Character",
			Subcategory = "Descriptions",
			ReturnType = (long)ProgVariableTypes.Boolean,
			FunctionText =
				$"return @ch.Race == ToRace(\"{race.Name}\") and @ch.Gender == ToGender(\"Female\") and In(@ch.AgeCategory, \"YoungAdult\", \"Adult\", \"Elder\", \"Venerable\")"
		};
		isAdultFemaleRaceProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = isAdultFemaleRaceProg,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterType = (long)ProgVariableTypes.Toon
		});
		_context.FutureProgs.Add(isAdultFemaleRaceProg);

		var isJuvenileMaleRaceProg = new FutureProg
		{
			FunctionName = $"Is{race.Name.CollapseString()}JuvenileMale",
			FunctionComment =
				$"Determines whether a character or character template is a juvenile male {race.Name}",
			AcceptsAnyParameters = false,
			Category = "Character",
			Subcategory = "Descriptions",
			ReturnType = (long)ProgVariableTypes.Boolean,
			FunctionText =
				$"return @ch.Race == ToRace(\"{race.Name}\") and @ch.Gender == ToGender(\"Male\") and In(@ch.AgeCategory, \"Child\", \"Youth\")"
		};
		isJuvenileMaleRaceProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = isJuvenileMaleRaceProg,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterType = (long)ProgVariableTypes.Toon
		});
		_context.FutureProgs.Add(isJuvenileMaleRaceProg);

		var isJuvenileFemaleRaceProg = new FutureProg
		{
			FunctionName = $"Is{race.Name.CollapseString()}JuvenileFemale",
			FunctionComment =
				$"Determines whether a character or character template is a juvenile female {race.Name}",
			AcceptsAnyParameters = false,
			Category = "Character",
			Subcategory = "Descriptions",
			ReturnType = (long)ProgVariableTypes.Boolean,
			FunctionText =
				$"return @ch.Race == ToRace(\"{race.Name}\") and @ch.Gender == ToGender(\"Female\") and In(@ch.AgeCategory, \"Child\", \"Youth\")"
		};
		isJuvenileFemaleRaceProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = isJuvenileFemaleRaceProg,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterType = (long)ProgVariableTypes.Toon
		});
		_context.FutureProgs.Add(isJuvenileFemaleRaceProg);

		var isBabyMaleRaceProg = new FutureProg
		{
			FunctionName = $"Is{race.Name.CollapseString()}BabyMale",
			FunctionComment = $"Determines whether a character or character template is a baby male {race.Name}",
			AcceptsAnyParameters = false,
			Category = "Character",
			Subcategory = "Descriptions",
			ReturnType = (long)ProgVariableTypes.Boolean,
			FunctionText =
				$"return @ch.Race == ToRace(\"{race.Name}\") and @ch.Gender == ToGender(\"Male\") and @ch.AgeCategory == \"Baby\""
		};
		isBabyMaleRaceProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = isBabyMaleRaceProg,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterType = (long)ProgVariableTypes.Toon
		});
		_context.FutureProgs.Add(isBabyMaleRaceProg);

		var isBabyFemaleRaceProg = new FutureProg
		{
			FunctionName = $"Is{race.Name.CollapseString()}BabyFemale",
			FunctionComment = $"Determines whether a character or character template is a baby female {race.Name}",
			AcceptsAnyParameters = false,
			Category = "Character",
			Subcategory = "Descriptions",
			ReturnType = (long)ProgVariableTypes.Boolean,
			FunctionText =
				$"return @ch.Race == ToRace(\"{race.Name}\") and @ch.Gender == ToGender(\"Female\") and @ch.AgeCategory == \"Baby\""
		};
		isBabyFemaleRaceProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = isBabyFemaleRaceProg,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterType = (long)ProgVariableTypes.Toon
		});
		_context.FutureProgs.Add(isBabyFemaleRaceProg);

		#endregion

		void DoLazyDescriptions(string babyName, string juvenileName, string adultMaleName, string adultFemaleName)
		{
			CreateDescription(EntityDescriptionType.ShortDescription, $"a &male {babyName}", isBabyMaleRaceProg);
			CreateDescription(EntityDescriptionType.ShortDescription, $"a &male {babyName}", isBabyFemaleRaceProg);
			CreateDescription(EntityDescriptionType.ShortDescription, $"a &male {juvenileName}",
				isJuvenileMaleRaceProg);
			CreateDescription(EntityDescriptionType.ShortDescription, $"a &male {juvenileName}",
				isJuvenileFemaleRaceProg);
			CreateDescription(EntityDescriptionType.ShortDescription, $"&a_an[&age] {adultMaleName}",
				isAdultMaleRaceProg);
			CreateDescription(EntityDescriptionType.ShortDescription, $"&a_an[&age] {adultFemaleName}",
				isAdultFemaleRaceProg);
			CreateDescription(EntityDescriptionType.FullDescription,
				$"This is a &male {babyName}. &he is young enough to be wholly dependent on &his mother.",
				isBabyMaleRaceProg);
			CreateDescription(EntityDescriptionType.FullDescription,
				$"This is a &male {babyName}. &he is young enough to be wholly dependent on &his mother.",
				isBabyFemaleRaceProg);
			CreateDescription(EntityDescriptionType.FullDescription,
				$"This is a &male {juvenileName}, not yet fully sized. &he has characteristics somewhere between that of a baby and an adult, and is not yet sexually mature.",
				isJuvenileMaleRaceProg);
			CreateDescription(EntityDescriptionType.FullDescription,
				$"This is a &male {juvenileName}, not yet fully sized. &he has characteristics somewhere between that of a baby and an adult, and is not yet sexually mature.",
				isJuvenileFemaleRaceProg);
			CreateDescription(EntityDescriptionType.FullDescription,
				$"This is &a_an[&age] {adultMaleName}. &he is fully sized and sexually mature.",
				isAdultMaleRaceProg);
			CreateDescription(EntityDescriptionType.FullDescription,
				$"This is &a_an[&age] {adultFemaleName}. &he is fully sized and sexually mature.",
				isAdultFemaleRaceProg);
		}

		void DoLazyDescriptionsWithMultipleJuvenile(string babyName, string maleJuvenileName,
			string femaleJuvenileName, string adultMaleName, string adultFemaleName)
		{
			CreateDescription(EntityDescriptionType.ShortDescription, $"a &male {babyName}", isBabyMaleRaceProg);
			CreateDescription(EntityDescriptionType.ShortDescription, $"a &male {babyName}", isBabyFemaleRaceProg);
			CreateDescription(EntityDescriptionType.ShortDescription, $"a {maleJuvenileName}",
				isJuvenileMaleRaceProg);
			CreateDescription(EntityDescriptionType.ShortDescription, $"a {femaleJuvenileName}",
				isJuvenileFemaleRaceProg);
			CreateDescription(EntityDescriptionType.ShortDescription, $"&a_an[&age] {adultMaleName}",
				isAdultMaleRaceProg);
			CreateDescription(EntityDescriptionType.ShortDescription, $"&a_an[&age] {adultFemaleName}",
				isAdultFemaleRaceProg);
			CreateDescription(EntityDescriptionType.FullDescription,
				$"This is a &male {babyName}. &he is young enough to be wholly dependent on &his mother.",
				isBabyMaleRaceProg);
			CreateDescription(EntityDescriptionType.FullDescription,
				$"This is a &male {babyName}. &he is young enough to be wholly dependent on &his mother.",
				isBabyFemaleRaceProg);
			CreateDescription(EntityDescriptionType.FullDescription,
				$"This is a {maleJuvenileName}, not yet fully sized. &he has characteristics somewhere between that of a baby and an adult, and is not yet sexually mature.",
				isJuvenileMaleRaceProg);
			CreateDescription(EntityDescriptionType.FullDescription,
				$"This is a {femaleJuvenileName}, not yet fully sized. &he has characteristics somewhere between that of a baby and an adult, and is not yet sexually mature.",
				isJuvenileFemaleRaceProg);
			CreateDescription(EntityDescriptionType.FullDescription,
				$"This is &a_an[&age] {adultMaleName}. &he is fully sized and sexually mature.",
				isAdultMaleRaceProg);
			CreateDescription(EntityDescriptionType.FullDescription,
				$"This is &a_an[&age] {adultFemaleName}. &he is fully sized and sexually mature.",
				isAdultFemaleRaceProg);
		}

		if (race.Name == "Cat")
		{
			var definition = new CharacteristicDefinition
			{
				Name = "Cat Coat",
				Type = (int)CharacteristicType.Multiform,
				Pattern = "catcoat",
				Description = "Various coats that cats can have",
				ChargenDisplayType = (int)CharacterGenerationDisplayType.DisplayAll,
				Model = "standard",
				Definition = ""
			};
			_context.CharacteristicDefinitions.Add(definition);

			var profile = new CharacteristicProfile
			{
				Name = "All Cat Coats",
				Definition = "<Profile/>",
				Description = "All values of the Cat Coats characteristic definition",
				TargetDefinition = definition,
				Type = "all"
			};
			_context.CharacteristicProfiles.Add(profile);

			race.RacesAdditionalCharacteristics.Add(new RacesAdditionalCharacteristics
			{
				Race = race,
				CharacteristicDefinition = definition,
				Usage = "base"
			});
			var ethnicity = _context.Ethnicities.Local.First(x => x.ParentRace == race);
			ethnicity.EthnicitiesCharacteristics.Add(new EthnicitiesCharacteristics
			{
				Ethnicity = ethnicity,
				CharacteristicDefinition = definition,
				CharacteristicProfile = profile
			});

			var solidCats = new List<CharacteristicValue>();
			var tabbyCats = new List<CharacteristicValue>();
			var orientalCats = new List<CharacteristicValue>();

			void AddCharacteristicValue(string name, string fancy, bool solid = false, bool tabby = false,
				bool oriental = false)
			{
				var value = new CharacteristicValue
				{
					Definition = definition,
					Default = false,
					Pluralisation = 0,
					Name = name,
					Value = $"{name}-coated",
					AdditionalValue = fancy
				};
				_context.CharacteristicValues.Add(value);

				if (solid) solidCats.Add(value);

				if (tabby) tabbyCats.Add(value);

				if (oriental) orientalCats.Add(value);
			}

			AddCharacteristicValue("black", "&his coat is a solid, evenly distributed black.", true);
			AddCharacteristicValue("blue-grey", "&his coat is a solid, evenly distributed blue-grey.", true);
			AddCharacteristicValue("blue-caramel", "&his coat is a solid, evenly distributed blue-caramel.",
				true);
			AddCharacteristicValue("chocolate", "&his coat is a solid, evenly distributed chocolate.", true);
			AddCharacteristicValue("lilac", "&his coat is a solid, evenly distributed lilac.", true);
			AddCharacteristicValue("taupe", "&his coat is a solid, evenly distributed taupe.", true);
			AddCharacteristicValue("cinnamon", "&his coat is a solid, evenly distributed cinnamon.", true);
			AddCharacteristicValue("fawn", "&his coat is a solid, evenly distributed fawn.", true);
			AddCharacteristicValue("ginger", "&his coat is a solid, evenly distributed ginger.", true);
			AddCharacteristicValue("cream", "&his coat is a solid, evenly distributed cream.", true);
			AddCharacteristicValue("apricot", "&his coat is a solid, evenly distributed apricot.", true);
			AddCharacteristicValue("albino", "&he is an albino cat, with unpigmented fur and pink eyes.",
				true);
			AddCharacteristicValue("white", "&his coat is a solid, evenly distributed white.", true);

			AddCharacteristicValue("brown tabby",
				"&his coat is a tabby pattern of black on coppery-brown, in a pattern almost reminiscent of a tiger.",
				tabby: true);
			AddCharacteristicValue("blue tabby",
				"&his coat is a tabby pattern of blue-grey on warm brown, in a pattern almost reminiscent of a tiger.",
				tabby: true);
			AddCharacteristicValue("chocolate tabby",
				"&his coat is a tabby pattern of dark chocolate on light chocolate brown, in a pattern almost reminiscent of a tiger.",
				tabby: true);
			AddCharacteristicValue("lilac tabby",
				"&his coat is a tabby pattern of frosty grey on pale lavender, in a pattern almost reminiscent of a tiger.",
				tabby: true);
			AddCharacteristicValue("cinnamon tabby",
				"&his coat is a tabby pattern of cinnamon on honey, in a pattern almost reminiscent of a tiger.",
				tabby: true);
			AddCharacteristicValue("fawn tabby",
				"&his coat is a tabby pattern of dense fawn on pale ivory, in a pattern almost reminiscent of a tiger.",
				tabby: true);
			AddCharacteristicValue("red tabby",
				"&his coat is a tabby pattern of deep red on orange, in a pattern almost reminiscent of a tiger.",
				tabby: true);
			AddCharacteristicValue("cream tabby",
				"&his coat is a tabby pattern of peach on pale cream, in a pattern almost reminiscent of a tiger.",
				tabby: true);

			AddCharacteristicValue("seal point",
				"&he has a form of albinism where the 'points' or face, tail and feet are a darker colour than the rest of the body. In this case, &his points are deep seal and the rest pale fawn.",
				oriental: true);
			AddCharacteristicValue("blue point",
				"&he has a form of albinism where the 'points' or face, tail and feet are a darker colour than the rest of the body. In this case, &his points are blue grey and the rest bluish white.",
				oriental: true);
			AddCharacteristicValue("chocolate point",
				"&he has a form of albinism where the 'points' or face, tail and feet are a darker colour than the rest of the body. In this case, &his points are milk chocolate and the rest ivory.",
				oriental: true);
			AddCharacteristicValue("lilac point",
				"&he has a form of albinism where the 'points' or face, tail and feet are a darker colour than the rest of the body. In this case, &his points are pinkish grey and the rest pearly grey.",
				oriental: true);
			AddCharacteristicValue("cinnamon point",
				"&he has a form of albinism where the 'points' or face, tail and feet are a darker colour than the rest of the body. In this case, &his points are bright reddish-brown and the rest warm ivory.",
				oriental: true);
			AddCharacteristicValue("fawn point",
				"&he has a form of albinism where the 'points' or face, tail and feet are a darker colour than the rest of the body. In this case, &his points are warm tan and the rest pale ivory.",
				oriental: true);
			AddCharacteristicValue("flame point",
				"&he has a form of albinism where the 'points' or face, tail and feet are a darker colour than the rest of the body. In this case, &his points are orange-red and the rest white.",
				oriental: true);
			AddCharacteristicValue("cream point",
				"&he has a form of albinism where the 'points' or face, tail and feet are a darker colour than the rest of the body. In this case, &his points are cream and the rest white.",
				oriental: true);

			AddCharacteristicValue("seal mink",
				"&he has a form of albinism where the 'points' or face, tail and feet are a darker colour than the rest of the body. In this case, &his points are dark brown and the rest medium brown.",
				oriental: true);
			AddCharacteristicValue("blue mink",
				"&he has a form of albinism where the 'points' or face, tail and feet are a darker colour than the rest of the body. In this case, &his points are slate blue and the rest soft blue-grey.",
				oriental: true);
			AddCharacteristicValue("chocolate mink",
				"&he has a form of albinism where the 'points' or face, tail and feet are a darker colour than the rest of the body. In this case, &his points are medium brown and the rest buff-cream.",
				oriental: true);
			AddCharacteristicValue("lilac mink",
				"&he has a form of albinism where the 'points' or face, tail and feet are a darker colour than the rest of the body. In this case, &his points are frosty grey and the rest silvery grey.",
				oriental: true);
			AddCharacteristicValue("cinnamon mink",
				"&he has a form of albinism where the 'points' or face, tail and feet are a darker colour than the rest of the body. In this case, &his points are bright reddish-brown and the rest warm ivory.",
				oriental: true);
			AddCharacteristicValue("fawn mink",
				"&he has a form of albinism where the 'points' or face, tail and feet are a darker colour than the rest of the body. In this case, &his points are warm tan and the rest pale ivory.",
				oriental: true);
			AddCharacteristicValue("flame mink",
				"&he has a form of albinism where the 'points' or face, tail and feet are a darker colour than the rest of the body. In this case, &his points are orange-red and the rest white.",
				oriental: true);
			AddCharacteristicValue("cream mink",
				"&he has a form of albinism where the 'points' or face, tail and feet are a darker colour than the rest of the body. In this case, &his points are cream and the rest white.",
				oriental: true);

			AddCharacteristicValue("seal sepia",
				"&he has a form of albinism where the 'points' or face, tail and feet are a darker colour than the rest of the body. In this case, &his points are dark brown and the rest sable.",
				oriental: true);
			AddCharacteristicValue("blue sepia",
				"&he has a form of albinism where the 'points' or face, tail and feet are a darker colour than the rest of the body. In this case, &his points are medium blue and the rest soft lighter blue-grey.",
				oriental: true);
			AddCharacteristicValue("chocolate sepia",
				"&he has a form of albinism where the 'points' or face, tail and feet are a darker colour than the rest of the body. In this case, &his points are medium brown and the rest coffee-brown.",
				oriental: true);
			AddCharacteristicValue("lilac sepia",
				"&he has a form of albinism where the 'points' or face, tail and feet are a darker colour than the rest of the body. In this case, &his points are frosty grey and the rest dove grey.",
				oriental: true);
			AddCharacteristicValue("cinnamon sepia",
				"&he has a form of albinism where the 'points' or face, tail and feet are a darker colour than the rest of the body. In this case, &his points are bright reddish-brown and the rest warm ivory.",
				oriental: true);
			AddCharacteristicValue("fawn sepia",
				"&he has a form of albinism where the 'points' or face, tail and feet are a darker colour than the rest of the body. In this case, &his points are warm tan and the rest pale ivory.",
				oriental: true);
			AddCharacteristicValue("flame sepia",
				"&he has a form of albinism where the 'points' or face, tail and feet are a darker colour than the rest of the body. In this case, &his points are orange-red and the rest white.",
				oriental: true);
			AddCharacteristicValue("cream sepia",
				"&he has a form of albinism where the 'points' or face, tail and feet are a darker colour than the rest of the body. In this case, &his points are cream and the rest white.",
				oriental: true);

			AddCharacteristicValue("tuxedo",
				"&he has a black coat with white mitts, white belly, white chin and a white tail tip.");
			AddCharacteristicValue("blue tuxedo",
				"&he has a blue-grey coat with white mitts, white belly, white chin and a white tail tip.");
			AddCharacteristicValue("chocolate tuxedo",
				"&he has a chocolate-brown coat with white mitts, white belly, white chin and a white tail tip.");
			AddCharacteristicValue("ginger tuxedo",
				"&he has a ginger coat with white mitts, white belly, white chin and a white tail tip.");

			profile = new CharacteristicProfile
			{
				Name = "Solid Cat Coats",
				Definition =
					$"<Values>\n{(from value in solidCats select $"<Value>{value.Id}</Value>").ListToLines(true)}</Values>",
				Description = "All solid colour cat coats",
				TargetDefinition = definition,
				Type = "Standard"
			};
			_context.CharacteristicProfiles.Add(profile);

			profile = new CharacteristicProfile
			{
				Name = "Tabby Cat Coats",
				Definition =
					$"<Values>\n{(from value in tabbyCats select $"<Value>{value.Id}</Value>").ListToLines(true)}</Values>",
				Description = "All tabby colour cat coats",
				TargetDefinition = definition,
				Type = "Standard"
			};
			_context.CharacteristicProfiles.Add(profile);

			profile = new CharacteristicProfile
			{
				Name = "Oriental Cat Coats",
				Definition =
					$"<Values>\n{(from value in orientalCats select $"<Value>{value.Id}</Value>").ListToLines(true)}</Values>",
				Description = "All oriental cat coats (siamese, tonkinese, burmese)",
				TargetDefinition = definition,
				Type = "Standard"
			};
			_context.CharacteristicProfiles.Add(profile);
		}

		if (TryGetRaceTemplate(race.Name, out var template))
		{
			CreateDescriptionsFromPack(race, template.DescriptionPack, isAdultMaleRaceProg, isAdultFemaleRaceProg,
				isJuvenileMaleRaceProg, isJuvenileFemaleRaceProg, isBabyMaleRaceProg, isBabyFemaleRaceProg);
			return;
		}

		var extraAdultDescriptions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
{
{"Coyote", "A rangy canine with tawny fur and sharp features. Its wary eyes and lean frame speak of a life spent hunting small game and scavenging."},
{"Hyena", "This hyena has a powerful frame with a sloping back and mottled fur. Its muzzle ends in strong jaws capable of crushing bone."},
{"Rabbit", "A small mammal with long ears, soft fur and a twitching nose. Its powerful hind legs allow it to bound away at the first sign of trouble."},
{"Hare", "Slender and long-legged, this hare sports large ears and a short tail. Its muscles are built for rapid bursts of speed across open ground."},
{"Beaver", "Stout and water-loving, the beaver has dense brown fur and large teeth for gnawing wood. A wide, flat tail aids in swimming."},
{"Otter", "Sleek and playful, this otter has glossy fur and webbed feet. Its streamlined body is built for a life spent swimming and hunting fish."},
{"Lion", "A majestic big cat with a muscular body and, in males, a thick mane. Its amber eyes regard the world with regal confidence."},
{"Cheetah", "This spotted cat has a lean build and long legs made for incredible bursts of speed. Black tear lines run from eyes to mouth."},
{"Leopard", "A powerful feline covered in rosetted spots. Muscular limbs and strong jaws make it an adept climber and ambush predator."},
{"Tiger", "A large striped cat with a massive head and muscular frame. Its orange coat with black stripes helps it stalk through dense foliage."},
{"Panther", "A lithe big cat with a glossy dark coat. Its movements are quiet and precise, well suited to stalking prey."},
{"Jaguar", "Stockier than the leopard, this big cat's rosettes have central spots. It has a broad head and immensely powerful jaws."},
{"Jackal", "A small, cunning canine with a bushy tail and large ears. Its coat is a mix of tawny and grey, perfect for blending into scrubland."},
{"Deer", "This graceful herbivore has slender legs and a sleek coat. Mature males sport antlers that are shed and regrown each year."},
{"Moose", "Towering and long-legged, the moose has a bulbous nose and a heavy dewlap. The males' broad antlers spread wide from their heads."},
{"Boar", "A wild boar with bristly dark hair and sharp tusks protruding from its lower jaw. Its body is thickset and muscular."},
{"Warthog", "Sparse-bristled and wiry, this pig has prominent facial warts and upward curving tusks. It carries itself with surprising speed."},
{"Cow", "A placid bovine with a broad muzzle and gentle eyes. Its large stomach betrays its grass-fed diet."},
{"Ox", "A powerfully built bovine used for labour, with a strong neck and broad shoulders. It bears a patient disposition."},
{"Buffalo", "This buffalo is sturdy and heavyset with sweeping horns. Its hide is tough and it moves with deliberate strength."},
{"Bison", "A hulking creature with a massive head and a shaggy mane of dark fur around its shoulders, tapering to a smaller hindquarters."},
{"Hippopotamus", "Huge and barrel-bodied, the hippo spends much of its time wallowing in water. Its enormous mouth opens to reveal long tusks."},
{"Horse", "A large hoofed animal with a flowing mane and tail. Its powerful legs and strong back make it well suited to riding or hauling."},
{"Bear", "Broad and thickly furred, this bear walks with a lumbering gait. It possesses long claws and a keen sense of smell."},
{"Rhinocerous", "Heavily built with thick grey skin and one or two prominent horns on its snout. Its hide bears folds that look like natural armour."},
{"Giraffe", "An extraordinarily tall creature with a spotted coat and a very long neck. It moves with a slow, loping stride."},
{"Elephant", "An enormous herbivore with a long trunk and large ears. Its ivory tusks curve outward from a massive head."},
{"Fox", "A small, agile canine with reddish fur and a bushy tail. Its pointed ears and muzzle give it a cunning appearance."},
{"Rat", "A small rodent with coarse fur, a scaly tail and beady black eyes. It sniffs the air constantly with twitching whiskers."},
{"Mouse", "Tiny and quick, this mouse has soft fur and large ears. Its long tail helps it balance while scurrying about."},
{"Hamster", "A chubby little rodent with expandable cheek pouches. It has soft fur and tiny paws well adapted for digging."},
{"Ferret", "Slender and inquisitive, the ferret has a long body and short legs. Its coat is silky, and its nose is always searching for scents."},
{"Weasel", "This small predator has a sleek body and sharp teeth. Its fur is smooth and its movements fast and sinuous."},
{"Guinea Pig", "A plump rodent with short legs and no tail. It makes a variety of squeaks and whistles when excited."},
{"Badger", "Stocky and low to the ground, the badger has a distinctive striped face and formidable digging claws."},
{"Wolverine", "Muscular and thick-furred, the wolverine is larger than a badger and known for its ferocity. Its broad paws help it move through snow."},
{"Goat", "A sure-footed ungulate with a short tail and backward curving horns. It happily grazes on rough plants that many animals avoid."},
	{"Llama", "This long-necked camelid has a shaggy coat and an alert expression. It's often used as a pack animal in mountainous regions."},
	{"Alpaca", "Smaller than a llama, this camelid is prized for its soft, luxurious fleece. Large eyes give it a gentle appearance."},

	{"Pigeon", "A plump, short-legged bird with iridescent feathers about its neck. It coos softly while bobbing its head."},
	{"Swallow", "A small bird with long, pointed wings and a forked tail. It darts through the air catching insects."},
	{"Sparrow", "A tiny brown songbird with streaked plumage and a quick, hopping gait."},
	{"Quail", "This squat ground bird sports a topknot of feathers and short rounded wings."},
	{"Grouse", "A chicken-like bird with mottled feathers that blend well with woodland undergrowth."},
	{"Pheasant", "A brightly coloured game bird with a long tail and vivid plumage, especially on the males."},
	{"Seagull", "A coastal bird with grey wings and a raucous cry, often seen scavenging along the shoreline."},
	{"Albatross", "Large and long-winged, this seabird glides effortlessly over the waves for hours at a time."},
	{"Heron", "A tall wading bird with a spear-like beak and long legs for stalking fish in shallow waters."},
	{"Crane", "Elegant and long-legged, the crane moves with slow, deliberate steps and has a bugling call."},
	{"Flamingo", "This pink bird stands on one leg while filtering food from the water with its curved bill."},
	{"Peacock", "A spectacular bird with a metallic blue neck and a fan of brilliant eye-spotted tail feathers."},
	{"Ibis", "A long-legged wader with a down-curved bill, probing mud for small creatures."},
	{"Pelican", "Large and heavy-billed, the pelican scoops up fish with the flexible pouch beneath its beak."},
	{"Crow", "A glossy black bird known for its intelligence and harsh cawing voice."},
	{"Raven", "Bigger than a crow, the raven has shaggy throat feathers and a deep, resonant croak."},
	{"Emu", "A tall, flightless bird with shaggy feathers and powerful legs built for running."},
	{"Ostrich", "The largest of birds, the ostrich has a long neck and strong legs capable of swift kicks."},
	{"Moa", "A massive, extinct flightless bird recreated here with heavy legs and a thick body."},
	{"Vulture", "A bald-headed scavenger with broad wings, circling patiently for carrion."},
	{"Parrot", "A colourful bird with a hooked beak and a squawking voice, often mimicking sounds."},
	{"Woodpecker", "This bird clings to tree trunks, hammering with its beak in search of insects."},
	{"Kingfisher", "A small bird with a large head and vivid plumage, diving swiftly for fish."},
	{"Stork", "Long-legged and long-billed, the stork is often seen wading in wetlands."},
	{"Penguin", "A tuxedo-feathered swimmer that waddles awkwardly on land but soars through the sea."},
	{"Duck", "A waterfowl with a broad bill and webbed feet, quacking softly as it paddles."},
	{"Goose", "Larger than a duck, this bird has a long neck and is quick to honk at intruders."},
	{"Swan", "Graceful and white-feathered, the swan glides serenely across the water."},
	{"Chicken", "A domesticated fowl kept for its eggs and meat, clucking as it scratches at the ground."},
	{"Turkey", "A large game bird with a fan-shaped tail and a fleshy wattle dangling from its beak."},
	{"Hawk", "A sharp-eyed raptor with hooked talons, circling high before diving on prey."},
	{"Eagle", "Powerful and regal, this large bird of prey has a commanding wingspan and piercing gaze."},
	{"Falcon", "A sleek raptor built for speed, with narrow wings and swift, decisive strikes."},
	{"Owl", "A nocturnal hunter with a rounded face and silent wings, staring from huge eyes."}
};

		void AddExtraAdultDescription()
		{
			if (extraAdultDescriptions.TryGetValue(race.Name, out var text))
			{
				CreateDescription(EntityDescriptionType.FullDescription, text, isAdultMaleRaceProg);
				CreateDescription(EntityDescriptionType.FullDescription, text, isAdultFemaleRaceProg);
			}
		}

		switch (race.Name)
		{
			case "Dog":
				CreateDescription(EntityDescriptionType.ShortDescription, "a male &ethnicity pup",
					isBabyMaleRaceProg);
				CreateDescription(EntityDescriptionType.ShortDescription, "a male &ethnicity pup",
					isBabyFemaleRaceProg);
				CreateDescription(EntityDescriptionType.ShortDescription, "&a_an[&age] &male &ethnicity",
					isJuvenileMaleRaceProg);
				CreateDescription(EntityDescriptionType.ShortDescription, "&a_an[&age] &male &ethnicity",
					isJuvenileFemaleRaceProg);
				CreateDescription(EntityDescriptionType.ShortDescription, "&a_an[&age] &male &ethnicity",
					isAdultMaleRaceProg);
				CreateDescription(EntityDescriptionType.ShortDescription, "&a_an[&age] &male &ethnicity",
					isAdultFemaleRaceProg);
				_context.SaveChanges();
				DogLongDescriptions(race);
				break;
			case "Wolf":
				CreateDescription(EntityDescriptionType.ShortDescription, "a male grey wolf pup",
					isBabyMaleRaceProg);
				CreateDescription(EntityDescriptionType.ShortDescription, "a female grey wolf pup",
					isBabyFemaleRaceProg);
				CreateDescription(EntityDescriptionType.ShortDescription, "a male grey wolf whelp",
					isJuvenileMaleRaceProg);
				CreateDescription(EntityDescriptionType.ShortDescription, "a female grey wolf whelp",
					isJuvenileFemaleRaceProg);
				CreateDescription(EntityDescriptionType.ShortDescription, "a male grey wolf", isAdultMaleRaceProg);
				CreateDescription(EntityDescriptionType.ShortDescription, "a female grey wolf",
					isAdultFemaleRaceProg);
				CreateDescription(EntityDescriptionType.FullDescription,
					"This animal is a pup, too young to survive on its own. Small and somewhat cute, it is covered in grey fur. When fully grown, this will be a male gray wolf.",
					isBabyMaleRaceProg);
				CreateDescription(EntityDescriptionType.FullDescription,
					"This animal is a pup, too young to survive on its own. Small and somewhat cute, it is covered in grey fur. When fully grown, this will be a female gray wolf.",
					isBabyFemaleRaceProg);
				CreateDescription(EntityDescriptionType.FullDescription,
					"This animal is a young &male wolf. Not yet fully mature, &he would nonetheless stand a chance at survival if isolated from &his pack. &his features resemble that of an adult of &his species, but there are still some puppy-like qualities to &his form and behaviour.",
					isJuvenileMaleRaceProg);
				CreateDescription(EntityDescriptionType.FullDescription,
					"This animal is a young &male wolf. Not yet fully mature, &he would nonetheless stand a chance at survival if isolated from &his pack. &his features resemble that of an adult of &his species, but there are still some puppy-like qualities to &his form and behaviour.",
					isJuvenileFemaleRaceProg);
				CreateDescription(EntityDescriptionType.FullDescription,
					"This is &a_an[&age] &male wolf. &he is slender and powerfully built with a deeply descending rib cage, a sloping back and a heavily muscled neck. &he has small, triangular ears and long legs that signal swift movement. &he has large, heavy teeth and a powerful jaw adapted to crushing bone.",
					isAdultMaleRaceProg);
				CreateDescription(EntityDescriptionType.FullDescription,
					"This is &a_an[&age] &male wolf. &he is slender and powerfully built with a deeply descending rib cage, a sloping back and a heavily muscled neck. &he has small, triangular ears and long legs that signal swift movement. &he has large, heavy teeth and a powerful jaw adapted to crushing bone.",
					isAdultFemaleRaceProg);
				break;
			case "Coyote":
				DoLazyDescriptions("coyote pup", "coyote whelp", "male coyote", "female coyote");
				AddExtraAdultDescription();
				break;
			case "Hyena":
				DoLazyDescriptions("hyena pup", "hyena whelp", "male hyena", "female hyena");
				AddExtraAdultDescription();
				break;
			case "Rabbit":
				DoLazyDescriptions("bunny", "young rabbit", "buck rabbit", "doe rabbit");
				AddExtraAdultDescription();
				break;
			case "Hare":
				DoLazyDescriptions("hare bunny", "young hare", "buck hare", "doe hare");
				AddExtraAdultDescription();
				break;
			case "Beaver":
				DoLazyDescriptions("beaver cub", "young beaver", "male beaver", "female beaver");
				AddExtraAdultDescription();
				break;
			case "Otter":
				DoLazyDescriptions("otter cub", "young otter", "male otter", "female otter");
				AddExtraAdultDescription();
				break;
			case "Cat":
				CreateDescription(EntityDescriptionType.ShortDescription, "&a_an[$catcoat] kitten",
					isBabyMaleRaceProg);
				CreateDescription(EntityDescriptionType.ShortDescription, "&a_an[$catcoat] kitten",
					isBabyFemaleRaceProg);
				CreateDescription(EntityDescriptionType.ShortDescription, "a young $catcoat cat",
					isJuvenileMaleRaceProg);
				CreateDescription(EntityDescriptionType.ShortDescription, "a young $catcoat cat",
					isJuvenileFemaleRaceProg);
				CreateDescription(EntityDescriptionType.ShortDescription, "&a_an[$catcoat] tomcat",
					isAdultMaleRaceProg);
				CreateDescription(EntityDescriptionType.ShortDescription, "&a_an[$catcoat] cat",
					isAdultFemaleRaceProg);
				CreateDescription(EntityDescriptionType.FullDescription,
					"This is a &male kitten of the common domestic cat. $catcoatfancy", isBabyMaleRaceProg);
				CreateDescription(EntityDescriptionType.FullDescription,
					"This is a &male kitten of the common domestic cat. $catcoatfancy", isBabyFemaleRaceProg);
				CreateDescription(EntityDescriptionType.FullDescription,
					"This animal is a young &male common domestic cat. $catcoatfancy.", isJuvenileMaleRaceProg);
				CreateDescription(EntityDescriptionType.FullDescription,
					"This animal is a young &male common domestic cat. $catcoatfancy", isJuvenileFemaleRaceProg);
				CreateDescription(EntityDescriptionType.FullDescription,
					"This is &a_an[&age] &male common domestic cat. $catcoatfancy", isAdultMaleRaceProg);
				CreateDescription(EntityDescriptionType.FullDescription,
					"This is &a_an[&age] &male common domestic cat. $catcoatfancy", isAdultFemaleRaceProg);
				break;
			case "Lion":
				DoLazyDescriptionsWithMultipleJuvenile("lion cub", "juvenile lion", "juvenile lioness", "lion",
						"lioness");
				AddExtraAdultDescription();
				break;
			case "Cheetah":
				DoLazyDescriptions("cheetah cub", "juvenile cheetah", "cheetah", "she-cheetah");
				AddExtraAdultDescription();
				break;
			case "Leopard":
				DoLazyDescriptionsWithMultipleJuvenile("leopard cub", "juvenile leopard", "juvenile leopardess",
						"leopard", "leopardess");
				AddExtraAdultDescription();
				break;
			case "Tiger":
				DoLazyDescriptionsWithMultipleJuvenile("tiger cub", "juvenile tiger", "juvenile tigress", "tiger",
						"tigress");
				AddExtraAdultDescription();
				break;
			case "Panther":
				DoLazyDescriptions("panther cub", "juvenile panther", "male panther", "female panther");
				AddExtraAdultDescription();
				break;
			case "Jaguar":
				DoLazyDescriptions("jaguar cub", "juvenile jaguar", "male jaguar", "female jaguar");
				AddExtraAdultDescription();
				break;
			case "Jackal":
				DoLazyDescriptions("jackal cub", "juvenile jackal", "male jackal", "female jackal");
				AddExtraAdultDescription();
				break;
			case "Deer":
				DoLazyDescriptionsWithMultipleJuvenile("fawn", "juvenile stag", "juvenile doe", "stag", "doe");
				AddExtraAdultDescription();
				break;
			case "Moose":
				DoLazyDescriptionsWithMultipleJuvenile("moose calf", "young bull moose", "moose heifer",
						"bull moose", "moose cow");
				AddExtraAdultDescription();
				break;
			case "Pig":
				CreateDescription(EntityDescriptionType.ShortDescription, "a &male piglet", isBabyMaleRaceProg);
				CreateDescription(EntityDescriptionType.ShortDescription, "a &male piglet", isBabyFemaleRaceProg);
				CreateDescription(EntityDescriptionType.ShortDescription, "&a_an[&age] hog",
					isJuvenileMaleRaceProg);
				CreateDescription(EntityDescriptionType.ShortDescription, "&a_an[&age] sow",
					isJuvenileFemaleRaceProg);
				CreateDescription(EntityDescriptionType.ShortDescription, "&a_an[&age] hog", isAdultMaleRaceProg);
				CreateDescription(EntityDescriptionType.ShortDescription, "&a_an[&age] sow", isAdultFemaleRaceProg);
				CreateDescription(EntityDescriptionType.FullDescription,
					"This is an adorable &male piglet with big, curious eyes and a little curly tail. It is still small and likely fully reliant on its mother.",
					isBabyMaleRaceProg);
				CreateDescription(EntityDescriptionType.FullDescription,
					"This is an adorable &male piglet with big, curious eyes and a little curly tail. It is still small and likely fully reliant on its mother.",
					isBabyFemaleRaceProg);
				CreateDescription(EntityDescriptionType.FullDescription,
					"This is a juvenile hog, not yet fully sized. He has a large head and a long snout, and feet with four hoofed toes each.",
					isJuvenileMaleRaceProg);
				CreateDescription(EntityDescriptionType.FullDescription,
					"This is a juvenile sow, not yet fully sized. She has a large head and a long snout, and feet with four hoofed toes each.",
					isJuvenileFemaleRaceProg);
				CreateDescription(EntityDescriptionType.FullDescription,
					"This is &a_an[&age] hog. He is very large in size and has a large head and a long snout, and feet with four hoofed toes each.",
					isAdultMaleRaceProg);
				CreateDescription(EntityDescriptionType.FullDescription,
					"This is &a_an[&age] &male wolf. She has a large head and a long snout, and feet with four hoofed toes each.",
					isAdultFemaleRaceProg);
				break;
			case "Sheep":
				CreateDescription(EntityDescriptionType.ShortDescription, "a &male lamb", isBabyMaleRaceProg);
				CreateDescription(EntityDescriptionType.ShortDescription, "a &male lamb", isBabyFemaleRaceProg);
				CreateDescription(EntityDescriptionType.ShortDescription, "a &male hogget", isJuvenileMaleRaceProg);
				CreateDescription(EntityDescriptionType.ShortDescription, "a &male hogget",
					isJuvenileFemaleRaceProg);
				CreateDescription(EntityDescriptionType.ShortDescription, "&a_an[&age] ram", isAdultMaleRaceProg);
				CreateDescription(EntityDescriptionType.ShortDescription, "&a_an[&age] ewe", isAdultFemaleRaceProg);
				CreateDescription(EntityDescriptionType.FullDescription, "This is a &male lamb.",
					isBabyMaleRaceProg);
				CreateDescription(EntityDescriptionType.FullDescription, "This is a &male lamb.",
					isBabyFemaleRaceProg);
				CreateDescription(EntityDescriptionType.FullDescription,
					"This is a &male hogget, or yearling sheep.", isJuvenileMaleRaceProg);
				CreateDescription(EntityDescriptionType.FullDescription,
					"This is a &male hogget, or yearling sheep.", isJuvenileFemaleRaceProg);
				CreateDescription(EntityDescriptionType.FullDescription, "This is &a_an[&age] &male sheep, or ram.",
					isAdultMaleRaceProg);
				CreateDescription(EntityDescriptionType.FullDescription, "This is &a_an[&age] &male sheep, or ewe.",
					isAdultFemaleRaceProg);
				break;
			case "Boar":
				DoLazyDescriptionsWithMultipleJuvenile("boar piglet", "young wild boar", "young wild sow",
						"wild boar", "wild sow");
				AddExtraAdultDescription();
				break;
			case "Warthog":
				DoLazyDescriptionsWithMultipleJuvenile("warthog piglet", "young warthog boar", "young warthog sow",
						"warthog boar", "warthog sow");
				AddExtraAdultDescription();
				break;
			case "Cow":
				DoLazyDescriptionsWithMultipleJuvenile("calf", "young bull", "heifer", "bull", "cow");
				AddExtraAdultDescription();
				break;
			case "Ox":
				DoLazyDescriptionsWithMultipleJuvenile("ox calf", "young ox bull", "ox heifer", "ox bull",
						"ox cow");
				AddExtraAdultDescription();
				break;
			case "Buffalo":
				DoLazyDescriptionsWithMultipleJuvenile("buffalo calf", "young buffalo bull", "buffalo heifer",
						"buffalo bull", "buffalo cow");
				AddExtraAdultDescription();
				break;
			case "Bison":
				DoLazyDescriptionsWithMultipleJuvenile("bison calf", "young bison bull", "bison heifer",
						"bison bull", "bison cow");
				AddExtraAdultDescription();
				break;
			case "Hippopotamus":
				DoLazyDescriptionsWithMultipleJuvenile("hippo calf", "young hippo bull", "young hippo cow",
						"hippo bull", "hippo cow");
				AddExtraAdultDescription();
				break;
			case "Horse":
				DoLazyDescriptionsWithMultipleJuvenile("foal", "colt", "filly", "stallion", "mare");
				AddExtraAdultDescription();
				break;
			case "Bear":
				DoLazyDescriptions("&ethnicity cub", "young &ethnicity", "male &ethnicity", "female &ethnicity");
				AddExtraAdultDescription();
				break;
			case "Rhinocerous":
				DoLazyDescriptions("rhino calf", "young rhino", "rhino bull", "rhino cow");
				AddExtraAdultDescription();
				break;
			case "Giraffe":
				DoLazyDescriptions("giraffe calf", "young giraffe", "male giraffe", "female giraffe");
				AddExtraAdultDescription();
				break;
			case "Elephant":
				DoLazyDescriptionsWithMultipleJuvenile("elephant calf", "juvenile bull elephant",
						"juvenile elephant", "bull elephant", "elephant");
				AddExtraAdultDescription();
				break;
			case "Fox":
				DoLazyDescriptionsWithMultipleJuvenile("fox kit", "juvenile male fox", "juvenile fox vixen",
						"male fox", "fox vixen");
				AddExtraAdultDescription();
				break;
			case "Rat":
				DoLazyDescriptions("rat pup", "young rat", "buck rat", "doe rat");
				AddExtraAdultDescription();
				break;
			case "Mouse":
				DoLazyDescriptions("mouse pup", "mouse rat", "buck mouse", "doe mouse");
				AddExtraAdultDescription();
				break;
			case "Hamster":
				DoLazyDescriptions("hamster pup", "young hamster", "buck hamster", "doe hamster");
				AddExtraAdultDescription();
				break;
			case "Ferret":
				DoLazyDescriptions("ferret kit", "young ferret", "jack ferret", "jill ferret");
				AddExtraAdultDescription();
				break;
			case "Weasel":
				DoLazyDescriptions("weasel kit", "young weasel", "jack weasel", "jill weasel");
				AddExtraAdultDescription();
				break;
			case "Guinea Pig":
				DoLazyDescriptions("guinea pig pup", "young guinea pig", "guinea pig boar", "guinea pig sow");
				AddExtraAdultDescription();
				break;
			case "Badger":
				DoLazyDescriptions("badger kit", "young badger", "boar badger", "sow badger");
				AddExtraAdultDescription();
				break;
			case "Wolverine":
				DoLazyDescriptions("wolverine kit", "young wolverine", "boar wolverine", "sow wolverine");
				AddExtraAdultDescription();
				break;
			case "Goat":
				DoLazyDescriptionsWithMultipleJuvenile("kid goat", "buckling goat",
						"doeling goat", "billy goat", "nanny goat");
				AddExtraAdultDescription();
				break;
			case "Llama":
				DoLazyDescriptions("llama cria", "young llama", "llama macho", "llama hembra");
				AddExtraAdultDescription();
				break;
			case "Alpaca":
				DoLazyDescriptions("alpaca cria", "young alpaca", "alpaca macho", "alpaca hembra");
				AddExtraAdultDescription();
				break;
			case "Pigeon":
			case "Swallow":
			case "Sparrow":
			case "Quail":
			case "Grouse":
			case "Pheasant":
			case "Seagull":
			case "Albatross":
			case "Heron":
			case "Crane":
			case "Flamingo":
			case "Peacock":
			case "Ibis":
			case "Pelican":
			case "Crow":
			case "Raven":
			case "Emu":
			case "Ostrich":
			case "Moa":
			case "Vulture":
			case "Parrot":
			case "Woodpecker":
			case "Kingfisher":
			case "Stork":
			case "Penguin":
				DoLazyDescriptions($"{race.Name.ToLowerInvariant()} chick",
						$" fledgling {race.Name.ToLowerInvariant()}", $"male {race.Name.ToLowerInvariant()}",
						$"female {race.Name.ToLowerInvariant()}");
				AddExtraAdultDescription();
				break;

			case "Duck":
				DoLazyDescriptionsWithMultipleJuvenile("duckling", "fledgling drake", "fledgling duck", "drake",
						"duck");
				AddExtraAdultDescription();
				break;
			case "Goose":
				DoLazyDescriptionsWithMultipleJuvenile("gosling", "fledgling gander", "fledgling goose", "gander",
						"goose");
				AddExtraAdultDescription();
				break;
			case "Swan":
				DoLazyDescriptionsWithMultipleJuvenile("cygnet", "fledgling swan cob", "fledgling swan pen",
						"swan cob", "swan pen");
				AddExtraAdultDescription();
				break;
			case "Chicken":
				DoLazyDescriptionsWithMultipleJuvenile("chicklet", "fledgling rooster", "fledgling hen", "rooster",
						"hen");
				AddExtraAdultDescription();
				break;
			case "Turkey":
				DoLazyDescriptionsWithMultipleJuvenile("poult", "fledgling turkey gobbler", "fledgling turkey hen",
						"turkey gobbler", "turkey hen");
				AddExtraAdultDescription();
				break;
			case "Hawk":
				DoLazyDescriptions("hawk chick", "fledgling hawk", "male hawk", "female hawk");
				AddExtraAdultDescription();
				break;
			case "Eagle":
				DoLazyDescriptions("eaglet", "fledgling eagle", "male eagle", "female eagle");
				AddExtraAdultDescription();
				break;
			case "Falcon":
				DoLazyDescriptions("falcon chick", "fledgling falcon", "male falcon", "female falcon");
				AddExtraAdultDescription();
				break;
			case "Owl":
				DoLazyDescriptions("owlet", "fledgling owl", "male owl", "female owl");
				AddExtraAdultDescription();
				break;
			default:
				DoLazyDescriptions($"baby {race.Name.ToLowerInvariant()}",
					$"juvenile {race.Name.ToLowerInvariant()}", race.Name.ToLowerInvariant(),
					race.Name.ToLowerInvariant());
				break;
		}
	}
	#endregion

	private void CreateEthnicitiesForRace(Race race)
	{
		PopulationBloodModel? model = null;
		switch (race.Name)
		{
			case "Dog":
				model = _context.PopulationBloodModels.FirstOrDefault(x =>
					x.Name == "Ordinary Canine Blood Type Distribution");
				break;
			case "Cat":
				model = _context.PopulationBloodModels.FirstOrDefault(x =>
					x.Name == "Ordinary Feline Blood Type Distribution");
				break;
			case "Horse":
				model = _context.PopulationBloodModels.FirstOrDefault(x =>
					x.Name == "Ordinary Equine Blood Type Distribution");
				break;
			case "Cow":
				model = _context.PopulationBloodModels.FirstOrDefault(x =>
					x.Name == "Ordinary Bovine Blood Type Distribution");
				break;
			case "Sheep":
				model = _context.PopulationBloodModels.FirstOrDefault(x =>
					x.Name == "Ordinary Ovine Blood Type Distribution");
				break;
		}

		switch (race.Name)
		{
			case "Dog":
				foreach (var (name, group) in new[]
				         {
					         ("Terrier", "Terrier"),
					         ("Setter", "Setter"),
					         ("Pointer", "Pointer"),
					         ("Retriever", "Retriever"),
					         ("Spaniel", "Spaniel"),
					         ("Water Dog", "Water Dog"),
					         ("Sighthound", "Hound"),
					         ("Scenthound", "Hound"),
					         ("Bulldog", "Bulldog"),
					         ("Mastiff", "Mastiff"),
					         ("Herding Dog", "Herding Dog"),
					         ("Lap Dog", "Lap Dog"),
					         ("Mongrel", "Mongrel")
				         })
				{
					_context.Ethnicities.Add(new Ethnicity
					{
						ParentRace = race,
						Name = name,
						ChargenBlurb = BuildEthnicityDescriptionForTesting(race.Name, name),
						EthnicGroup = group,
						TolerableTemperatureFloorEffect = 0,
						TolerableTemperatureCeilingEffect = 0,
						PopulationBloodModel = model
					});
				}

				break;
			case "Bear":
				foreach (var (name, floor, ceiling) in new[]
				         {
					         ("Black Bear", 0.0, 0.0),
					         ("Moon Bear", 0.0, 0.0),
					         ("Brown Bear", -10.0, 0.0),
					         ("Polar Bear", -30.0, -10.0)
				         })
				{
					_context.Ethnicities.Add(new Ethnicity
					{
						ParentRace = race,
						Name = name,
						ChargenBlurb = BuildEthnicityDescriptionForTesting(race.Name, name),
						EthnicGroup = name,
						TolerableTemperatureFloorEffect = floor,
						TolerableTemperatureCeilingEffect = ceiling,
						PopulationBloodModel = model
					});
				}

				break;
			default:
				_context.Ethnicities.Add(new Ethnicity
				{
					ParentRace = race,
					Name = race.Name,
					ChargenBlurb = BuildEthnicityDescriptionForTesting(race.Name, race.Name),
					EthnicGroup = "Common",
					TolerableTemperatureFloorEffect = 0,
					TolerableTemperatureCeilingEffect = 0,
					PopulationBloodModel = model
				});
				break;
		}
	}

	private void AddRace(string name, string adjective, string? description, BodyProto body, SizeCategory size,
		bool canClimb, double bodypartHealth, string hwMale, string hwFemale, bool sweats = true,
		string breathing = "simple")
	{
		var model = _defaultCorpseModel;
		TryGetRaceTemplate(name, out var raceTemplate);
		var seededDescription = raceTemplate is not null
			? BuildRaceDescriptionForTesting(raceTemplate)
			: description;
		int childage, youthage, youngadultage, adultage, elderage, venerableage;
		if (raceTemplate is not null && AgeProfiles.TryGetValue(raceTemplate.AgeProfileKey, out var ageProfile))
		{
			childage = ageProfile.ChildAge;
			youthage = ageProfile.YouthAge;
			youngadultage = ageProfile.YoungAdultAge;
			adultage = ageProfile.AdultAge;
			elderage = ageProfile.ElderAge;
			venerableage = ageProfile.VenerableAge;
		}
		else
		{
			switch (name)
			{
				case "Mouse":
				case "Rat":
				case "Guinea Pig":
				case "Hamster":
				case "Ferret":
				case "Rabbit":
				case "Hare":
				case "Pigeon":
				case "Swallow":
				case "Sparrow":
				case "Quail":
				case "Chicken":
					childage = 1;
					youthage = 2;
					youngadultage = 3;
					adultage = 4;
					elderage = 5;
					venerableage = 6;
					break;
				default:
					childage = 1;
					youthage = 2;
					youngadultage = 3;
					adultage = 7;
					elderage = 12;
					venerableage = 15;
					break;
			}
		}

		var sb = new StringBuilder();
		sb.AppendLine("switch (@trait.Name)");
		foreach (var attribute in _context.TraitDefinitions.Where(x => x.Type == (int)TraitType.Attribute))
		{
			sb.AppendLine($"  case (\"{attribute.Name}\")");
			sb.AppendLine("    return 0");
		}

		sb.AppendLine("end switch\nreturn 0");
		var progText = sb.ToString();

		var attributeBonusProg = new FutureProg
		{
			FunctionName = $"{name.CollapseString()}AttributeBonus",
			FunctionComment = $"Racial attribute bonuses for the {name} race",
			AcceptsAnyParameters = false,
			Category = "Character",
			Subcategory = "Attributes",
			ReturnType = (long)ProgVariableTypes.Number,
			FunctionText = progText
		};
		attributeBonusProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = attributeBonusProg,
			ParameterIndex = 0,
			ParameterName = "trait",
			ParameterType = (long)ProgVariableTypes.Trait
		});
		_context.FutureProgs.Add(attributeBonusProg);

		var driedBlood = new Material
		{
			Name = $"dried {adjective.ToLowerInvariant()} blood",
			MaterialDescription = "dried blood",
			Density = 1520,
			Organic = true,
			Type = 0,
			BehaviourType = 19,
			ThermalConductivity = 0.2,
			ElectricalConductivity = 0.0001,
			SpecificHeatCapacity = 420,
			IgnitionPoint = 555.3722,
			HeatDamagePoint = 412.0389,
			ImpactFracture = 1000,
			ImpactYield = 1000,
			ImpactStrainAtYield = 2,
			ShearFracture = 1000,
			ShearYield = 1000,
			ShearStrainAtYield = 2,
			YoungsModulus = 0.1,
			SolventId = 1,
			SolventVolumeRatio = 4,
			ResidueDesc = "It is covered in {0}dried blood",
			ResidueColour = "red",
			Absorbency = 0
		};
		_context.Materials.Add(driedBlood);
		var bloodLiquid = new Liquid
		{
			Name = $"{adjective} Blood",
			Description = "blood",
			LongDescription = "a virtually opaque dark red fluid",
			TasteText = "It has a sharply metallic, umami taste",
			VagueTasteText = "It has a metallic taste",
			SmellText = "It has a metallic, coppery smell",
			VagueSmellText = "It has a faintly metallic smell",
			TasteIntensity = 200,
			SmellIntensity = 10,
			AlcoholLitresPerLitre = 0,
			WaterLitresPerLitre = 0.8,
			DrinkSatiatedHoursPerLitre = 6,
			FoodSatiatedHoursPerLitre = 4,
			Viscosity = 1,
			Density = 1,
			Organic = true,
			ThermalConductivity = 0.609,
			ElectricalConductivity = 0.005,
			SpecificHeatCapacity = 4181,
			FreezingPoint = -20,
			BoilingPoint = 100,
			DisplayColour = "bold red",
			DampDescription = "It is damp with blood",
			WetDescription = "It is wet with blood",
			DrenchedDescription = "It is drenched with blood",
			DampShortDescription = "(blood damp)",
			WetShortDescription = "(bloody)",
			DrenchedShortDescription = "(blood drenched)",
			SolventId = 1,
			SolventVolumeRatio = 5,
			InjectionConsequence = (int)LiquidInjectionConsequence.BloodReplacement,
			ResidueVolumePercentage = 0.05,
			DriedResidue = driedBlood,
			CountAsQuality = (int)ItemQuality.Legendary,
			CountAs = _bloodLiquid
		};
		_context.Liquids.Add(bloodLiquid);

		Liquid sweat = null;
		if (sweats)
		{
			var driedSweat = new Material
			{
				Name = $"dried {adjective.ToLowerInvariant()} sweat",
				MaterialDescription = "dried sweat",
				Density = 1520,
				Organic = true,
				Type = 0,
				BehaviourType = 19,
				ThermalConductivity = 0.2,
				ElectricalConductivity = 0.0001,
				SpecificHeatCapacity = 420,
				IgnitionPoint = 555.3722,
				HeatDamagePoint = 412.0389,
				ImpactFracture = 1000,
				ImpactYield = 1000,
				ImpactStrainAtYield = 2,
				ShearFracture = 1000,
				ShearYield = 1000,
				ShearStrainAtYield = 2,
				YoungsModulus = 0.1,
				SolventId = 1,
				SolventVolumeRatio = 3,
				ResidueDesc = "It is covered in {0}dried sweat",
				ResidueColour = "yellow",
				Absorbency = 0
			};
			_context.Materials.Add(driedSweat);
			sweat = new Liquid
			{
				Name = $"{adjective} Sweat",
				Description = "sweat",
				LongDescription = "a relatively clear, translucent fluid that smells strongly of wild animal odor",
				TasteText = "It tastes like a pungent, salty lick of someone's underarms",
				VagueTasteText = "It tastes very unpleasant, like underarm stench",
				SmellText = "It has the sharp, pungent smell of animal body odor",
				VagueSmellText = "It has the sharp, pungent smell of animal body odor",
				TasteIntensity = 200,
				SmellIntensity = 200,
				AlcoholLitresPerLitre = 0,
				WaterLitresPerLitre = 0.95,
				DrinkSatiatedHoursPerLitre = 5,
				FoodSatiatedHoursPerLitre = 0,
				Viscosity = 1,
				Density = 1,
				Organic = true,
				ThermalConductivity = 0.609,
				ElectricalConductivity = 0.005,
				SpecificHeatCapacity = 4181,
				FreezingPoint = -20,
				BoilingPoint = 100,
				DisplayColour = "yellow",
				DampDescription = "It is damp with sweat",
				WetDescription = "It is wet and smelly with sweat",
				DrenchedDescription = "It is soaking wet and smelly with sweat",
				DampShortDescription = "(sweat-damp)",
				WetShortDescription = "(sweaty)",
				DrenchedShortDescription = "(sweat-drenched)",
				SolventId = 1,
				SolventVolumeRatio = 5,
				InjectionConsequence = (int)LiquidInjectionConsequence.Harmful,
				ResidueVolumePercentage = 0.05,
				DriedResidue = driedSweat,
				CountAsQuality = (int)ItemQuality.Legendary,
				CountAs = _sweatLiquid
			};
			_context.Liquids.Add(sweat);
		}

		BloodModel? bloodModel = null;
		PopulationBloodModel? populationModel = null;
		if (raceTemplate?.BloodProfileKey is not null)
		{
			var bloodProfile = CreateBloodProfile(raceTemplate.BloodProfileKey);
			if (bloodProfile is not null)
			{
				(bloodModel, populationModel) = bloodProfile.Value;
			}
		}

			var race = new Race
		{
			Name = name,
			Description = string.IsNullOrWhiteSpace(description)
								? $"{name}s are {adjective.ToLowerInvariant()} creatures"
								: description,
			BaseBody = body,
			AllowedGenders = "2 3",
			AttributeBonusProg = attributeBonusProg,
			AttributeTotalCap = _context.TraitDefinitions.Count(x => x.Type == 1) * 12,
			IndividualAttributeCap = 20,
			DiceExpression = "3d6+1",
			IlluminationPerceptionMultiplier = 1.0,
			AvailabilityProg = _alwaysFalse,
			CorpseModel = model,
			DefaultHealthStrategy = _healthStrategy,
			DefaultCombatSetting = CombatStrategySeederHelper.EnsureCombatStrategy(_context, raceTemplate?.CombatStrategyKey ?? "Beast Brawler"),
			CanUseWeapons = true,
			CanAttack = true,
			CanDefend = true,
			NeedsToBreathe = true,
			SizeStanding = (int)size,
			SizeProne = (int)size,
			SizeSitting = (int)size,
			CommunicationStrategyType = "humanoid",
			HandednessOptions = "1 3",
			DefaultHandedness = 1,
			ChildAge = childage,
			YouthAge = youthage,
			YoungAdultAge = youngadultage,
			AdultAge = adultage,
			ElderAge = elderage,
			VenerableAge = venerableage,
			CanClimb = canClimb,
			CanSwim = true,
			MinimumSleepingPosition = 4,
			BodypartHealthMultiplier = bodypartHealth,
			BodypartSizeModifier = 0,
			TemperatureRangeCeiling = 40,
			TemperatureRangeFloor = 0,
			CanEatCorpses = false,
			CanEatMaterialsOptIn = false,
			BiteWeight = 1000,
			EatCorpseEmoteText = "",
			RaceUsesStamina = true,
			NaturalArmourQuality = 2,
			SweatLiquid = sweat,
			SweatRateInLitresPerMinute = 0.8,
			BloodLiquid = bloodLiquid,
			BloodModel = bloodModel,
			BreathingVolumeExpression = "7",
			HoldBreathLengthExpression = $"90+(5*con:{_healthTrait.Id})",
			MaximumLiftWeightExpression = $"str:{_strengthTrait.Id}*10000",
			MaximumDragWeightExpression = $"str:{_strengthTrait.Id}*40000",
			DefaultHeightWeightModelMale = _hwModels[hwMale],
			DefaultHeightWeightModelNeuter = _hwModels[hwMale],
			DefaultHeightWeightModelFemale = _hwModels[hwFemale],
			DefaultHeightWeightModelNonBinary = _hwModels[hwFemale]
		};
		_context.Races.Add(race);

		foreach (var attribute in _attributes)
			_context.RacesAttributes.Add(new RacesAttributes
			{ Race = race, Attribute = attribute, IsHealthAttribute = attribute.TraitGroup == "Physical" });

		CreateEthnicitiesForRace(race);
		_context.SaveChanges();
		CreateDescriptionsForRace(race);
		CreateRaceAttacks(race);

		switch (breathing)
		{
			case "simple":
				_context.RacesBreathableGases.Add(new RacesBreathableGases
				{
					Race = race,
					Gas = _breathableAir,
					Multiplier = 1.0
				});
				race.BreathingModel = "simple";
				break;
			case "insect":
				_context.RacesBreathableGases.Add(new RacesBreathableGases
				{
					Race = race,
					Gas = _breathableAir,
					Multiplier = 1.0
				});
				race.BreathingModel = "partless";
				break;
			case "partless":
				_context.RacesBreathableLiquids.Add(new RacesBreathableLiquids
				{
					Race = race,
					Liquid = _saltWater,
					Multiplier = 1.0
				});
				_context.RacesBreathableLiquids.Add(new RacesBreathableLiquids
				{
					Race = race,
					Liquid = _brackishWater,
					Multiplier = 0.5
				});
				race.BreathingModel = "partless";
				break;
			case "freshwater":
				foreach (var liquid in _freshWaters)
					_context.RacesBreathableLiquids.Add(new RacesBreathableLiquids
					{
						Race = race,
						Liquid = liquid,
						Multiplier = 1.0
					});

				_context.RacesBreathableLiquids.Add(new RacesBreathableLiquids
				{
					Race = race,
					Liquid = _brackishWater,
					Multiplier = 0.5
				});
				race.BreathingModel = "gills";
				break;
			case "blowhole":
				_context.RacesBreathableLiquids.Add(new RacesBreathableLiquids
				{
					Race = race,
					Liquid = _saltWater,
					Multiplier = 1.0
				});
				_context.RacesBreathableLiquids.Add(new RacesBreathableLiquids
				{
					Race = race,
					Liquid = _brackishWater,
					Multiplier = 0.5
				});
				race.BreathingModel = "blowhole";
				break;
			case "saltwater":
				_context.RacesBreathableLiquids.Add(new RacesBreathableLiquids
				{
					Race = race,
					Liquid = _saltWater,
					Multiplier = 1.0
				});
				_context.RacesBreathableLiquids.Add(new RacesBreathableLiquids
				{
					Race = race,
					Liquid = _brackishWater,
					Multiplier = 0.5
				});
				race.BreathingModel = "gills";
				break;
		}

		if (raceTemplate?.AdditionalBodypartUsages is not null)
		{
			foreach (var usage in raceTemplate.AdditionalBodypartUsages)
			{
				AddRacialBodypartUsage(usage.BodypartAlias, usage.Usage, race);
			}
		}
		else
		{
			switch (name)
			{
				case "Cow":
				case "Pig":
				case "Sheep":
				case "Ox":
				case "Bison":
				case "Buffalo":
				case "Deer":
				case "Moose":
				case "Llama":
				case "Alpaca":
				case "Rhinocerous":
				case "Horse":
					AddRacialBodypartUsage("udder", "female", race);
					break;
			}

			switch (name)
			{
				case "Ox":
				case "Cow":
				case "Bison":
				case "Buffalo":
				case "Goat":
				case "Sheep":
					AddRacialBodypartUsage("rhorn", "male", race);
					AddRacialBodypartUsage("lhorn", "male", race);
					break;
				case "Rhinocerous":
					AddRacialBodypartUsage("horn", "general", race);
					break;
				case "Deer":
				case "Moose":
					AddRacialBodypartUsage("rantler", "male", race);
					AddRacialBodypartUsage("lantler", "male", race);
					break;
				case "Boar":
				case "Warthog":
					AddRacialBodypartUsage("rtusk", "male", race);
					AddRacialBodypartUsage("ltusk", "male", race);
					break;
				case "Elephant":
				case "Hippopotamus":
					AddRacialBodypartUsage("rtusk", "general", race);
					AddRacialBodypartUsage("ltusk", "general", race);
					break;
			}
		}
	}


	private void SetupArmourTypes()
	{
		_naturalArmour = new ArmourType
		{
			Name = "Non-Human Natural Armour",
			MinimumPenetrationDegree = 1,
			BaseDifficultyDegrees = 0,
			StackedDifficultyDegrees = 0,
			Definition = @"<ArmourType>

	<!-- Damage Transformations change damage passed on to bones/organs/items into a different damage type when severity is under a certain  threshold 
		
		Damage Types:
		
		Slashing = 0
		Chopping = 1
		Crushing = 2
		Piercing = 3
		Ballistic = 4
		Burning = 5
		Freezing = 6
		Chemical = 7
		Shockwave = 8
		Bite = 9
		Claw = 10
		Electrical = 11
		Hypoxia = 12
		Cellular = 13
		Sonic = 14
		Shearing = 15
		ArmourPiercing = 16
		Wrenching = 17
		Shrapnel = 18
		Necrotic = 19
		Falling = 20
		Eldritch = 21
		Arcane = 22
		
		Severity Values:
		
		None = 0
		Superficial = 1
		Minor = 2
		Small = 3
		Moderate = 4
		Severe = 5
		VerySevere = 6
		Grievous = 7
		Horrifying = 8
	-->
	<DamageTransformations>
		<Transform fromtype=""0"" totype=""2"" severity=""5""></Transform> <!-- Slashing to Crushing when <= Severe -->
		<Transform fromtype=""1"" totype=""2"" severity=""5""></Transform> <!-- Chopping to Crushing when <= Severe -->
		<Transform fromtype=""3"" totype=""2"" severity=""4""></Transform> <!-- Piercing to Crushing when <= Moderate -->
		<Transform fromtype=""4"" totype=""2"" severity=""4""></Transform> <!-- Ballistic to Crushing when <= Moderate -->
		<Transform fromtype=""9"" totype=""2"" severity=""5""></Transform> <!-- Bite to Crushing when <= Severe -->
		<Transform fromtype=""10"" totype=""2"" severity=""5""></Transform> <!-- Claw to Crushing when <= Severe -->
		<Transform fromtype=""15"" totype=""2"" severity=""5""></Transform> <!-- Shearing to Crushing when <= Severe -->
		<Transform fromtype=""16"" totype=""2"" severity=""3""></Transform> <!-- ArmourPiercing to Crushing when <= Small -->
		<Transform fromtype=""17"" totype=""2"" severity=""5""></Transform> <!-- Wrenching to Crushing when <= Severe -->
	</DamageTransformations>
	<!-- 
	
		Dissipate expressions are applied before the item/part takes damage. 
		If they reduce the damage to zero, it neither suffers nor passes on any damage. 
		
		Parameters: 
		* damage, pain or stun (as appropriate) = the raw damage/pain/stun suffered
		* quality = the quality of the armour, rated 0 (Abysmal) to 11 (Legendary)
		* angle = the angle in radians of the attack (e.g. 1.5708rad = 90 degrees)
		* density = the density in kg/m3 of the material that the armour is made from
		* electrical = the electrical conductivity of the material that the armour is made from (1/ohm metres)
		* thermal = the thermal conductivity of the material that the armour is made from (watts per meter3 per kelvin)
		* organic = if the material that the armour is made from is organic (1 for true, 0 for false)
		* strength = either ImpactYield or ShearYield of the armour material depending on the damage type, in Pascals.
		
		Hint: 25000 can be considered ""base"" ShearYield and 10000 can be considered ""base"" ImpactYield
	-->
	<DissipateExpressions>
		<Expression damagetype=""0"">damage - (quality * strength/25000 * 0.75)</Expression>    <!-- Slashing -->
		<Expression damagetype=""1"">damage - (quality * strength/25000 * 0.75)</Expression>    <!-- Chopping -->  
		<Expression damagetype=""2"">damage - (quality * strength/10000 * 0.75)</Expression>    <!-- Crushing -->  
		<Expression damagetype=""3"">damage - (quality * strength/25000 * 0.75)</Expression>    <!-- Piercing -->  
		<Expression damagetype=""4"">damage - (quality * strength/25000 * 0.75)</Expression>    <!-- Ballistic -->  
		<Expression damagetype=""5"">damage - (quality * 0.75)</Expression>    			      <!-- Burning -->
		<Expression damagetype=""6"">damage - (quality * 0.75)</Expression>                     <!-- Freezing -->
		<Expression damagetype=""7"">damage - (quality * 0.75)</Expression>                     <!-- Chemical -->
		<Expression damagetype=""8"">damage - (quality * strength/10000 * 0.75)</Expression>    <!-- Shockwave -->
		<Expression damagetype=""9"">damage - (quality * strength/25000 * 0.75)</Expression>    <!-- Bite -->
		<Expression damagetype=""10"">damage - (quality * strength/25000 * 0.75)</Expression>   <!-- Claw -->
		<Expression damagetype=""11"">damage - (quality * 0.75)</Expression>                    <!-- Electrical -->
		<Expression damagetype=""12"">damage - (quality * 0.75)</Expression>                    <!-- Hypoxia -->
		<Expression damagetype=""13"">damage - (quality * 0.75)</Expression>                    <!-- Cellular -->
		<Expression damagetype=""14"">damage - (quality * strength/10000 * 0.75)</Expression>   <!-- Sonic -->
		<Expression damagetype=""15"">damage - (quality * strength/25000 * 0.75)</Expression>   <!-- Shearing --> 
		<Expression damagetype=""16"">damage - (quality * strength/25000 * 0.75)</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">damage - (quality * strength/10000 * 0.75)</Expression>   <!-- Wrenching -->
		<Expression damagetype=""18"">damage - (quality * strength/25000 * 0.75)</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">damage - (quality * 0.75)</Expression>                    <!-- Necrotic -->   
		<Expression damagetype=""20"">damage - (quality * strength/10000 * 0.75)</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">damage - (quality * 0.75)</Expression>                    <!-- Eldritch -->   
		<Expression damagetype=""22"">damage - (quality * 0.75)</Expression>                    <!-- Arcane -->   
	</DissipateExpressions>  
	<DissipateExpressionsPain>
		<Expression damagetype=""0"">pain - (quality * strength/25000 * 0.75)</Expression>    <!-- Slashing -->
		<Expression damagetype=""1"">pain - (quality * strength/25000 * 0.75)</Expression>    <!-- Chopping -->  
		<Expression damagetype=""2"">pain - (quality * strength/10000 * 0.75)</Expression>    <!-- Crushing -->  
		<Expression damagetype=""3"">pain - (quality * strength/25000 * 0.75)</Expression>    <!-- Piercing -->  
		<Expression damagetype=""4"">pain - (quality * strength/25000 * 0.75)</Expression>    <!-- Ballistic -->  
		<Expression damagetype=""5"">pain - (quality * 0.75)</Expression>    			        <!-- Burning -->
		<Expression damagetype=""6"">pain - (quality * 0.75)</Expression>                     <!-- Freezing -->
		<Expression damagetype=""7"">pain - (quality * 0.75)</Expression>                     <!-- Chemical -->
		<Expression damagetype=""8"">pain - (quality * strength/10000 * 0.75)</Expression>    <!-- Shockwave -->
		<Expression damagetype=""9"">pain - (quality * strength/25000 * 0.75)</Expression>    <!-- Bite -->
		<Expression damagetype=""10"">pain - (quality * strength/25000 * 0.75)</Expression>   <!-- Claw -->
		<Expression damagetype=""11"">pain - (quality * 0.75)</Expression>                    <!-- Electrical -->
		<Expression damagetype=""12"">pain - (quality * 0.75)</Expression>                    <!-- Hypoxia -->
		<Expression damagetype=""13"">pain - (quality * 0.75)</Expression>                    <!-- Cellular -->
		<Expression damagetype=""14"">pain - (quality * strength/10000 * 0.75)</Expression>   <!-- Sonic -->
		<Expression damagetype=""15"">pain - (quality * strength/25000 * 0.75)</Expression>   <!-- Shearing --> 
		<Expression damagetype=""16"">pain - (quality * strength/25000 * 0.75)</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">pain - (quality * strength/10000 * 0.75)</Expression>   <!-- Wrenching -->
		<Expression damagetype=""18"">pain - (quality * strength/25000 * 0.75)</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">pain - (quality * 0.75)</Expression>                    <!-- Necrotic -->   
		<Expression damagetype=""20"">pain - (quality * strength/10000 * 0.75)</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">pain - (quality * 0.75)</Expression>                    <!-- Eldritch -->   
		<Expression damagetype=""22"">pain - (quality * 0.75)</Expression>                    <!-- Arcane -->   
	</DissipateExpressionsPain>  
	<DissipateExpressionsStun>
		<Expression damagetype=""0"">stun - (quality * strength/25000 * 0.75)</Expression>    <!-- Slashing -->
		<Expression damagetype=""1"">stun - (quality * strength/25000 * 0.75)</Expression>    <!-- Chopping -->  
		<Expression damagetype=""2"">stun - (quality * strength/10000 * 0.75)</Expression>    <!-- Crushing -->  
		<Expression damagetype=""3"">stun - (quality * strength/25000 * 0.75)</Expression>    <!-- Piercing -->  
		<Expression damagetype=""4"">stun - (quality * strength/25000 * 0.75)</Expression>    <!-- Ballistic -->  
		<Expression damagetype=""5"">stun - (quality * 0.75)</Expression>    			        <!-- Burning -->
		<Expression damagetype=""6"">stun - (quality * 0.75)</Expression>                     <!-- Freezing -->
		<Expression damagetype=""7"">stun - (quality * 0.75)</Expression>                     <!-- Chemical -->
		<Expression damagetype=""8"">stun - (quality * strength/10000 * 0.75)</Expression>    <!-- Shockwave -->
		<Expression damagetype=""9"">stun - (quality * strength/25000 * 0.75)</Expression>    <!-- Bite -->
		<Expression damagetype=""10"">stun - (quality * strength/25000 * 0.75)</Expression>   <!-- Claw -->
		<Expression damagetype=""11"">stun - (quality * 0.75)</Expression>                    <!-- Electrical -->
		<Expression damagetype=""12"">stun - (quality * 0.75)</Expression>                    <!-- Hypoxia -->
		<Expression damagetype=""13"">stun - (quality * 0.75)</Expression>                    <!-- Cellular -->
		<Expression damagetype=""14"">stun - (quality * strength/10000 * 0.75)</Expression>   <!-- Sonic -->
		<Expression damagetype=""15"">stun - (quality * strength/25000 * 0.75)</Expression>   <!-- Shearing --> 
		<Expression damagetype=""16"">stun - (quality * strength/25000 * 0.75)</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">stun - (quality * strength/10000 * 0.75)</Expression>   <!-- Wrenching -->
		<Expression damagetype=""18"">stun - (quality * strength/25000 * 0.75)</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">stun - (quality * 0.75)</Expression>                    <!-- Necrotic -->   
		<Expression damagetype=""20"">stun - (quality * strength/10000 * 0.75)</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">stun - (quality * 0.75)</Expression>                    <!-- Eldritch -->   
		<Expression damagetype=""22"">stun - (quality * 0.75)</Expression>                    <!-- Arcane -->   
	</DissipateExpressionsStun>  
	<!-- 
	
		Absorb expressions are applied after dissipate expressions and item/part damage. 
		The after-absorb values are what is passed on to anything ""below"" e.g. bones, organs, parts worn under armour, etc 
		
		Parameters: 
		* damage, pain or stun (as appropriate) = the residual damage/pain/stun after dissipate step
		* quality = the quality of the armour, rated 0 (Abysmal) to 11 (Legendary)
		* angle = the angle in radians of the attack (e.g. 1.5708rad = 90 degrees)
		* density = the density in kg/m3 of the material that the armour is made from
		* electrical = the electrical conductivity of the material that the armour is made from (1/ohm metres)
		* thermal = the thermal conductivity of the material that the armour is made from (watts per meter3 per kelvin)
		* organic = if the material that the armour is made from is organic (1 for true, 0 for false)
		* strength = either ImpactYield or ShearYield of the armour material depending on the damage type, in Pascals.
		
		Hint: 25000 can be considered ""base"" ShearYield and 10000 can be considered ""base"" ImpactYield
		
		-->
	<AbsorbExpressions>
		<Expression damagetype=""0"">damage*0.8</Expression>    <!-- Slashing -->
		<Expression damagetype=""1"">damage*0.8</Expression>    <!-- Chopping -->  
		<Expression damagetype=""2"">damage*0.8</Expression>    <!-- Crushing -->  
		<Expression damagetype=""3"">damage*0.8</Expression>    <!-- Piercing -->  
		<Expression damagetype=""4"">damage*0.8</Expression>    <!-- Ballistic -->  
		<Expression damagetype=""5"">damage*0.8</Expression>    <!-- Burning -->
		<Expression damagetype=""6"">damage*0.8</Expression>    <!-- Freezing -->
		<Expression damagetype=""7"">damage*0.8</Expression>    <!-- Chemical -->
		<Expression damagetype=""8"">damage*0.8</Expression>    <!-- Shockwave -->
		<Expression damagetype=""9"">damage*0.8</Expression>    <!-- Bite -->
		<Expression damagetype=""10"">damage*0.8</Expression>   <!-- Claw -->
		<Expression damagetype=""11"">damage*0.8</Expression>   <!-- Electrical -->
		<Expression damagetype=""12"">0</Expression>        <!-- Hypoxia -->
		<Expression damagetype=""13"">0</Expression>        <!-- Cellular -->
		<Expression damagetype=""14"">damage*0.8</Expression>   <!-- Sonic -->
		<Expression damagetype=""15"">damage*0.8</Expression>   <!-- Shearing --> 
		<Expression damagetype=""16"">damage*0.8</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">damage*0.8</Expression>   <!-- Wrenching -->
		<Expression damagetype=""18"">damage*0.8</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">damage*0.8</Expression>   <!-- Necrotic -->   
		<Expression damagetype=""20"">damage*0.8</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">damage*0.8</Expression>   <!-- Eldritch -->   
		<Expression damagetype=""22"">damage*0.8</Expression>   <!-- Arcane -->   
	</AbsorbExpressions>  
	<AbsorbExpressionsPain>
		<Expression damagetype=""0"">pain*0.8</Expression>    <!-- Slashing -->
		<Expression damagetype=""1"">pain*0.8</Expression>    <!-- Chopping -->  
		<Expression damagetype=""2"">pain*0.8</Expression>    <!-- Crushing -->  
		<Expression damagetype=""3"">pain*0.8</Expression>    <!-- Piercing -->  
		<Expression damagetype=""4"">pain*0.8</Expression>    <!-- Ballistic -->  
		<Expression damagetype=""5"">pain*0.8</Expression>    <!-- Burning -->
		<Expression damagetype=""6"">pain*0.8</Expression>    <!-- Freezing -->
		<Expression damagetype=""7"">pain*0.8</Expression>    <!-- Chemical -->
		<Expression damagetype=""8"">pain*0.8</Expression>    <!-- Shockwave -->
		<Expression damagetype=""9"">pain*0.8</Expression>    <!-- Bite -->
		<Expression damagetype=""10"">pain*0.8</Expression>   <!-- Claw -->
		<Expression damagetype=""11"">pain*0.8</Expression>   <!-- Electrical -->
		<Expression damagetype=""12"">0</Expression>        <!-- Hypoxia -->
		<Expression damagetype=""13"">0</Expression>        <!-- Cellular -->
		<Expression damagetype=""14"">pain*0.8</Expression>   <!-- Sonic -->
		<Expression damagetype=""15"">pain*0.8</Expression>   <!-- Shearing --> 
		<Expression damagetype=""16"">pain*0.8</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">pain*0.8</Expression>   <!-- Wrenching -->
		<Expression damagetype=""18"">pain*0.8</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">pain*0.8</Expression>   <!-- Necrotic -->   
		<Expression damagetype=""20"">pain*0.8</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">pain*0.8</Expression>   <!-- Eldritch -->   
		<Expression damagetype=""22"">pain*0.8</Expression>   <!-- Arcane -->   
	</AbsorbExpressionsPain>  
	<AbsorbExpressionsStun>
		<Expression damagetype=""0"">stun</Expression>    <!-- Slashing -->
		<Expression damagetype=""1"">stun</Expression>    <!-- Chopping -->  
		<Expression damagetype=""2"">stun</Expression>    <!-- Crushing -->  
		<Expression damagetype=""3"">stun</Expression>    <!-- Piercing -->  
		<Expression damagetype=""4"">stun</Expression>    <!-- Ballistic -->  
		<Expression damagetype=""5"">stun</Expression>    <!-- Burning -->
		<Expression damagetype=""6"">stun</Expression>    <!-- Freezing -->
		<Expression damagetype=""7"">stun</Expression>    <!-- Chemical -->
		<Expression damagetype=""8"">stun</Expression>    <!-- Shockwave -->
		<Expression damagetype=""9"">stun</Expression>    <!-- Bite -->
		<Expression damagetype=""10"">stun</Expression>   <!-- Claw -->
		<Expression damagetype=""11"">stun</Expression>   <!-- Electrical -->
		<Expression damagetype=""12"">0</Expression>        <!-- Hypoxia -->
		<Expression damagetype=""13"">0</Expression>        <!-- Cellular -->
		<Expression damagetype=""14"">stun</Expression>   <!-- Sonic -->
		<Expression damagetype=""15"">stun</Expression>   <!-- Shearing --> 
		<Expression damagetype=""16"">stun</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">stun</Expression>   <!-- Wrenching -->
		<Expression damagetype=""18"">stun</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">stun</Expression>   <!-- Necrotic -->   
		<Expression damagetype=""20"">stun</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">stun</Expression>   <!-- Eldritch -->   
		<Expression damagetype=""22"">stun</Expression>   <!-- Arcane -->   
	</AbsorbExpressionsStun>
 </ArmourType>"
		};
		_context.ArmourTypes.Add(_naturalArmour);
		_context.SaveChanges();

		_organArmour = new ArmourType
		{
			Name = "Non-Human Natural Organ Armour",
			MinimumPenetrationDegree = 1,
			BaseDifficultyDegrees = 0,
			StackedDifficultyDegrees = 0,
			Definition = @"<ArmourType>

	<!-- 
	
		Dissipate expressions are applied before the item/part takes damage. 
		If they reduce the damage to zero, it neither suffers nor passes on any damage. 
		
		Parameters: 
		* damage, pain or stun (as appropriate) = the raw damage/pain/stun suffered
		* quality = the quality of the armour, rated 0 (Abysmal) to 11 (Legendary)
		* angle = the angle in radians of the attack (e.g. 1.5708rad = 90 degrees)
		* density = the density in kg/m3 of the material that the armour is made from
		* electrical = the electrical conductivity of the material that the armour is made from (1/ohm metres)
		* thermal = the thermal conductivity of the material that the armour is made from (watts per meter3 per kelvin)
		* organic = if the material that the armour is made from is organic (1 for true, 0 for false)
		* strength = either ImpactYield or ShearYield of the armour material depending on the damage type, in Pascals.
		
		Hint: 25000 can be considered ""base"" ShearYield and 10000 can be considered ""base"" ImpactYield
	-->
	<DissipateExpressions>
		<Expression damagetype=""0"">damage-(1.0*quality)</Expression>         <!-- Slashing -->
		<Expression damagetype=""1"">damage-(1.0*quality)</Expression>         <!-- Chopping -->  
		<Expression damagetype=""2"">damage-(1.0*quality)</Expression>         <!-- Crushing -->  
		<Expression damagetype=""3"">damage-(1.0*quality)</Expression>         <!-- Piercing -->  
		<Expression damagetype=""4"">damage*1.15-(1.0*quality)</Expression>    <!-- Ballistic -->  
		<Expression damagetype=""5"">damage-(1.0*quality)</Expression>    	 <!-- Burning -->
		<Expression damagetype=""6"">damage-(1.0*quality)</Expression>         <!-- Freezing -->
		<Expression damagetype=""7"">damage-(1.0*quality)</Expression>         <!-- Chemical -->
		<Expression damagetype=""8"">damage*1.15-(1.0*quality)</Expression>    <!-- Shockwave -->
		<Expression damagetype=""9"">damage-(1.0*quality)</Expression>         <!-- Bite -->
		<Expression damagetype=""10"">damage-(1.0*quality)</Expression>        <!-- Claw -->
		<Expression damagetype=""11"">damage-(1.0*quality)</Expression>        <!-- Electrical -->
		<Expression damagetype=""12"">damage-(quality*0.75)</Expression>       <!-- Hypoxia -->
		<Expression damagetype=""13"">damage-(quality*0.75)</Expression>       <!-- Cellular -->
		<Expression damagetype=""14"">damage-(1.0*quality)</Expression>        <!-- Sonic -->
		<Expression damagetype=""15"">damage-(1.0*quality)</Expression>        <!-- Shearing --> 
		<Expression damagetype=""16"">damage-(1.0*quality)</Expression>        <!-- ArmourPiercing -->
		<Expression damagetype=""17"">damage-(1.0*quality)</Expression>        <!-- Wrenching -->
		<Expression damagetype=""18"">damage*1.15-(1.0*quality)</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">damage-(1.0*quality)</Expression>        <!-- Necrotic -->   
		<Expression damagetype=""20"">damage-(1.0*quality)</Expression>        <!-- Falling -->   
		<Expression damagetype=""21"">damage-(1.0*quality)</Expression>        <!-- Eldritch -->   
		<Expression damagetype=""22"">damage-(1.0*quality)</Expression>        <!-- Arcane -->   
	</DissipateExpressions>  
	<DissipateExpressionsPain>
		<Expression damagetype=""0"">pain-(1.0*quality)</Expression>         <!-- Slashing -->
		<Expression damagetype=""1"">pain-(1.0*quality)</Expression>         <!-- Chopping -->  
		<Expression damagetype=""2"">pain-(1.0*quality)</Expression>         <!-- Crushing -->  
		<Expression damagetype=""3"">pain-(1.0*quality)</Expression>         <!-- Piercing -->  
		<Expression damagetype=""4"">pain*1.15-(1.0*quality)</Expression>    <!-- Ballistic -->  
		<Expression damagetype=""5"">pain-(1.0*quality)</Expression>    	 <!-- Burning -->
		<Expression damagetype=""6"">pain-(1.0*quality)</Expression>         <!-- Freezing -->
		<Expression damagetype=""7"">pain-(1.0*quality)</Expression>         <!-- Chemical -->
		<Expression damagetype=""8"">pain*1.15-(1.0*quality)</Expression>    <!-- Shockwave -->
		<Expression damagetype=""9"">pain-(1.0*quality)</Expression>         <!-- Bite -->
		<Expression damagetype=""10"">pain-(1.0*quality)</Expression>        <!-- Claw -->
		<Expression damagetype=""11"">pain-(1.0*quality)</Expression>        <!-- Electrical -->
		<Expression damagetype=""12"">pain-(quality*0.75)</Expression>       <!-- Hypoxia -->
		<Expression damagetype=""13"">pain-(quality*0.75)</Expression>       <!-- Cellular -->
		<Expression damagetype=""14"">pain-(1.0*quality)</Expression>        <!-- Sonic -->
		<Expression damagetype=""15"">pain-(1.0*quality)</Expression>        <!-- Shearing --> 
		<Expression damagetype=""16"">pain-(1.0*quality)</Expression>        <!-- ArmourPiercing -->
		<Expression damagetype=""17"">pain-(1.0*quality)</Expression>        <!-- Wrenching -->
		<Expression damagetype=""18"">pain*1.15-(1.0*quality)</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">pain-(1.0*quality)</Expression>        <!-- Necrotic -->   
		<Expression damagetype=""20"">pain-(1.0*quality)</Expression>        <!-- Falling -->   
		<Expression damagetype=""21"">pain-(1.0*quality)</Expression>        <!-- Eldritch -->   
		<Expression damagetype=""22"">pain-(1.0*quality)</Expression>        <!-- Arcane -->   
	</DissipateExpressionsPain>  
	<DissipateExpressionsStun>
		<Expression damagetype=""0"">stun-(1.0*quality)</Expression>         <!-- Slashing -->
		<Expression damagetype=""1"">stun-(1.0*quality)</Expression>         <!-- Chopping -->  
		<Expression damagetype=""2"">stun-(1.0*quality)</Expression>         <!-- Crushing -->  
		<Expression damagetype=""3"">stun-(1.0*quality)</Expression>         <!-- Piercing -->  
		<Expression damagetype=""4"">stun*1.15-(1.0*quality)</Expression>    <!-- Ballistic -->  
		<Expression damagetype=""5"">stun-(1.0*quality)</Expression>    	 <!-- Burning -->
		<Expression damagetype=""6"">stun-(1.0*quality)</Expression>         <!-- Freezing -->
		<Expression damagetype=""7"">stun-(1.0*quality)</Expression>         <!-- Chemical -->
		<Expression damagetype=""8"">stun*1.15-(1.0*quality)</Expression>    <!-- Shockwave -->
		<Expression damagetype=""9"">stun-(1.0*quality)</Expression>         <!-- Bite -->
		<Expression damagetype=""10"">stun-(1.0*quality)</Expression>        <!-- Claw -->
		<Expression damagetype=""11"">stun-(1.0*quality)</Expression>        <!-- Electrical -->
		<Expression damagetype=""12"">stun-(quality*0.75)</Expression>       <!-- Hypoxia -->
		<Expression damagetype=""13"">stun-(quality*0.75)</Expression>       <!-- Cellular -->
		<Expression damagetype=""14"">stun-(1.0*quality)</Expression>        <!-- Sonic -->
		<Expression damagetype=""15"">stun-(1.0*quality)</Expression>        <!-- Shearing --> 
		<Expression damagetype=""16"">stun-(1.0*quality)</Expression>        <!-- ArmourPiercing -->
		<Expression damagetype=""17"">stun-(1.0*quality)</Expression>        <!-- Wrenching -->
		<Expression damagetype=""18"">stun*1.15-(1.0*quality)</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">stun-(1.0*quality)</Expression>        <!-- Necrotic -->   
		<Expression damagetype=""20"">stun-(1.0*quality)</Expression>        <!-- Falling -->   
		<Expression damagetype=""21"">stun-(1.0*quality)</Expression>        <!-- Eldritch -->   
		<Expression damagetype=""22"">stun-(1.0*quality)</Expression>        <!-- Arcane -->    
	</DissipateExpressionsStun>  
	
	<!-- Note: Organ Damage is final - there's no ""lower layer"" to pass on to, therefore there is no need for Absorb expressions -->
 </ArmourType>"
		};
		_context.ArmourTypes.Add(_organArmour);
		_context.SaveChanges();

		_boneArmour = new ArmourType
		{
			Name = "Non-Human Natural Bone Armour",
			MinimumPenetrationDegree = 1,
			BaseDifficultyDegrees = 0,
			StackedDifficultyDegrees = 0,
			Definition = @"<ArmourType>

	<!-- Damage Transformations change damage passed on to bones/organs/items into a different damage type when severity is under a certain  threshold 
		
		Damage Types:
		
		Slashing = 0
		Chopping = 1
		Crushing = 2
		Piercing = 3
		Ballistic = 4
		Burning = 5
		Freezing = 6
		Chemical = 7
		Shockwave = 8
		Bite = 9
		Claw = 10
		Electrical = 11
		Hypoxia = 12
		Cellular = 13
		Sonic = 14
		Shearing = 15
		ArmourPiercing = 16
		Wrenching = 17
		Shrapnel = 18
		Necrotic = 19
		Falling = 20
		Eldritch = 21
		Arcane = 22
		
		Severity Values:
		
		None = 0
		Superficial = 1
		Minor = 2
		Small = 3
		Moderate = 4
		Severe = 5
		VerySevere = 6
		Grievous = 7
		Horrifying = 8
	-->
	<DamageTransformations>
		<Transform fromtype=""0"" totype=""2"" severity=""5""></Transform> <!-- Slashing to Crushing when <= Severe -->
		<Transform fromtype=""1"" totype=""2"" severity=""5""></Transform> <!-- Chopping to Crushing when <= Severe -->
		<Transform fromtype=""3"" totype=""2"" severity=""4""></Transform> <!-- Piercing to Crushing when <= Moderate -->
		<Transform fromtype=""4"" totype=""2"" severity=""4""></Transform> <!-- Ballistic to Crushing when <= Moderate -->
		<Transform fromtype=""9"" totype=""2"" severity=""5""></Transform> <!-- Bite to Crushing when <= Severe -->
		<Transform fromtype=""10"" totype=""2"" severity=""5""></Transform> <!-- Claw to Crushing when <= Severe -->
		<Transform fromtype=""15"" totype=""2"" severity=""5""></Transform> <!-- Shearing to Crushing when <= Severe -->
		<Transform fromtype=""16"" totype=""2"" severity=""3""></Transform> <!-- ArmourPiercing to Crushing when <= Small -->
		<Transform fromtype=""17"" totype=""2"" severity=""5""></Transform> <!-- Wrenching to Crushing when <= Severe -->
	</DamageTransformations>
	<!-- 
	
		Dissipate expressions are applied before the item/part takes damage. 
		If they reduce the damage to zero, it neither suffers nor passes on any damage. 
		
		Parameters: 
		* damage, pain or stun (as appropriate) = the raw damage/pain/stun suffered
		* quality = the quality of the armour, rated 0 (Abysmal) to 11 (Legendary)
		* angle = the angle in radians of the attack (e.g. 1.5708rad = 90 degrees)
		* density = the density in kg/m3 of the material that the armour is made from
		* electrical = the electrical conductivity of the material that the armour is made from (1/ohm metres)
		* thermal = the thermal conductivity of the material that the armour is made from (watts per meter3 per kelvin)
		* organic = if the material that the armour is made from is organic (1 for true, 0 for false)
		* strength = either ImpactYield or ShearYield of the armour material depending on the damage type, in Pascals.
		
		Hint: 25000 can be considered ""base"" ShearYield and 10000 can be considered ""base"" ImpactYield
	-->
	<DissipateExpressions>
		<Expression damagetype=""0"">max(damage*0.1,damage-(quality * 2 * strength/115000))</Expression>    <!-- Slashing -->
		<Expression damagetype=""1"">max(damage*0.1,damage-(quality * 2 * strength/115000)))</Expression>    <!-- Chopping -->  
		<Expression damagetype=""2"">max(damage*0.1,damage-(quality * 2 * strength/200000))</Expression>    <!-- Crushing -->  
		<Expression damagetype=""3"">max(damage*0.1,damage-(quality * 2 * strength/115000))</Expression>    <!-- Piercing -->  
		<Expression damagetype=""4"">max(damage*0.1,damage-(quality * 2 * strength/115000))</Expression>    <!-- Ballistic -->  
		<Expression damagetype=""5"">damage - (quality * 2)</Expression>    			      <!-- Burning -->
		<Expression damagetype=""6"">damage - (quality * 2)</Expression>                     <!-- Freezing -->
		<Expression damagetype=""7"">damage - (quality * 2)</Expression>                     <!-- Chemical -->
		<Expression damagetype=""8"">max(damage*0.1,damage-(quality * 2 * strength/200000))</Expression>    <!-- Shockwave -->
		<Expression damagetype=""9"">max(damage*0.1,damage-(quality * 2 * strength/115000))</Expression>    <!-- Bite -->
		<Expression damagetype=""10"">max(damage*0.1,damage-(quality * 2 * strength/115000))</Expression>   <!-- Claw -->
		<Expression damagetype=""11"">damage - (quality * 2)</Expression>                    <!-- Electrical -->
		<Expression damagetype=""12"">0</Expression>                    <!-- Hypoxia -->
		<Expression damagetype=""13"">damage - (quality * 2)</Expression>                    <!-- Cellular -->
		<Expression damagetype=""14"">max(damage*0.1,damage-(quality * 2 * strength/200000))</Expression>   <!-- Sonic -->
		<Expression damagetype=""15"">max(damage*0.1,damage-(quality * 2 * strength/115000))</Expression>   <!-- Shearing --> 
		<Expression damagetype=""16"">max(damage*0.1,damage-(quality * 2 * strength/115000))</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">max(damage*0.1,damage-(quality * 2 * strength/200000))</Expression>   <!-- Wrenching -->
		<Expression damagetype=""18"">max(damage*0.1,damage-(quality * 2 * strength/115000))</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">damage - (quality * 2)</Expression>                    <!-- Necrotic -->   
		<Expression damagetype=""20"">max(damage*0.1,damage-(quality * 2 * strength/200000))</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">damage - (quality * 2)</Expression>                    <!-- Eldritch -->   
		<Expression damagetype=""22"">damage - (quality * 2)</Expression>                    <!-- Arcane -->   
	</DissipateExpressions>  
	<DissipateExpressionsPain>
		<Expression damagetype=""0"">pain - (quality * strength/25000 * 0.75)</Expression>    <!-- Slashing -->
		<Expression damagetype=""1"">pain - (quality * strength/25000 * 0.75)</Expression>    <!-- Chopping -->  
		<Expression damagetype=""2"">pain - (quality * strength/10000 * 0.75)</Expression>    <!-- Crushing -->  
		<Expression damagetype=""3"">pain - (quality * strength/25000 * 0.75)</Expression>    <!-- Piercing -->  
		<Expression damagetype=""4"">pain - (quality * strength/25000 * 0.75)</Expression>    <!-- Ballistic -->  
		<Expression damagetype=""5"">pain - (quality * 0.75)</Expression>    			        <!-- Burning -->
		<Expression damagetype=""6"">pain - (quality * 0.75)</Expression>                     <!-- Freezing -->
		<Expression damagetype=""7"">pain - (quality * 0.75)</Expression>                     <!-- Chemical -->
		<Expression damagetype=""8"">pain - (quality * strength/10000 * 0.75)</Expression>    <!-- Shockwave -->
		<Expression damagetype=""9"">pain - (quality * strength/25000 * 0.75)</Expression>    <!-- Bite -->
		<Expression damagetype=""10"">pain - (quality * strength/25000 * 0.75)</Expression>   <!-- Claw -->
		<Expression damagetype=""11"">pain - (quality * 0.75)</Expression>                    <!-- Electrical -->
		<Expression damagetype=""12"">pain - (quality * 0.75)</Expression>                    <!-- Hypoxia -->
		<Expression damagetype=""13"">pain - (quality * 0.75)</Expression>                    <!-- Cellular -->
		<Expression damagetype=""14"">pain - (quality * strength/10000 * 0.75)</Expression>   <!-- Sonic -->
		<Expression damagetype=""15"">pain - (quality * strength/25000 * 0.75)</Expression>   <!-- Shearing --> 
		<Expression damagetype=""16"">pain - (quality * strength/25000 * 0.75)</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">pain - (quality * strength/10000 * 0.75)</Expression>   <!-- Wrenching -->
		<Expression damagetype=""18"">pain - (quality * strength/25000 * 0.75)</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">pain - (quality * 0.75)</Expression>                    <!-- Necrotic -->   
		<Expression damagetype=""20"">pain - (quality * strength/10000 * 0.75)</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">pain - (quality * 0.75)</Expression>                    <!-- Eldritch -->   
		<Expression damagetype=""22"">pain - (quality * 0.75)</Expression>                    <!-- Arcane -->   
	</DissipateExpressionsPain>  
	<DissipateExpressionsStun>
		<Expression damagetype=""0"">stun - (quality * strength/25000 * 0.75)</Expression>    <!-- Slashing -->
		<Expression damagetype=""1"">stun - (quality * strength/25000 * 0.75)</Expression>    <!-- Chopping -->  
		<Expression damagetype=""2"">stun - (quality * strength/10000 * 0.75)</Expression>    <!-- Crushing -->  
		<Expression damagetype=""3"">stun - (quality * strength/25000 * 0.75)</Expression>    <!-- Piercing -->  
		<Expression damagetype=""4"">stun - (quality * strength/25000 * 0.75)</Expression>    <!-- Ballistic -->  
		<Expression damagetype=""5"">stun - (quality * 0.75)</Expression>    			        <!-- Burning -->
		<Expression damagetype=""6"">stun - (quality * 0.75)</Expression>                     <!-- Freezing -->
		<Expression damagetype=""7"">stun - (quality * 0.75)</Expression>                     <!-- Chemical -->
		<Expression damagetype=""8"">stun - (quality * strength/10000 * 0.75)</Expression>    <!-- Shockwave -->
		<Expression damagetype=""9"">stun - (quality * strength/25000 * 0.75)</Expression>    <!-- Bite -->
		<Expression damagetype=""10"">stun - (quality * strength/25000 * 0.75)</Expression>   <!-- Claw -->
		<Expression damagetype=""11"">stun - (quality * 0.75)</Expression>                    <!-- Electrical -->
		<Expression damagetype=""12"">stun - (quality * 0.75)</Expression>                    <!-- Hypoxia -->
		<Expression damagetype=""13"">stun - (quality * 0.75)</Expression>                    <!-- Cellular -->
		<Expression damagetype=""14"">stun - (quality * strength/10000 * 0.75)</Expression>   <!-- Sonic -->
		<Expression damagetype=""15"">stun - (quality * strength/25000 * 0.75)</Expression>   <!-- Shearing --> 
		<Expression damagetype=""16"">stun - (quality * strength/25000 * 0.75)</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">stun - (quality * strength/10000 * 0.75)</Expression>   <!-- Wrenching -->
		<Expression damagetype=""18"">stun - (quality * strength/25000 * 0.75)</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">stun - (quality * 0.75)</Expression>                    <!-- Necrotic -->   
		<Expression damagetype=""20"">stun - (quality * strength/10000 * 0.75)</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">stun - (quality * 0.75)</Expression>                    <!-- Eldritch -->   
		<Expression damagetype=""22"">stun - (quality * 0.75)</Expression>                    <!-- Arcane -->   
	</DissipateExpressionsStun>  
	<!-- 
	
		Absorb expressions are applied after dissipate expressions and item/part damage. 
		The after-absorb values are what is passed on to anything ""below"" e.g. bones, organs, parts worn under armour, etc 
		
		Parameters: 
		* damage, pain or stun (as appropriate) = the residual damage/pain/stun after dissipate step
		* quality = the quality of the armour, rated 0 (Abysmal) to 11 (Legendary)
		* angle = the angle in radians of the attack (e.g. 1.5708rad = 90 degrees)
		* density = the density in kg/m3 of the material that the armour is made from
		* electrical = the electrical conductivity of the material that the armour is made from (1/ohm metres)
		* thermal = the thermal conductivity of the material that the armour is made from (watts per meter3 per kelvin)
		* organic = if the material that the armour is made from is organic (1 for true, 0 for false)
		* strength = either ImpactYield or ShearYield of the armour material depending on the damage type, in Pascals.
		
		Hint: 25000 can be considered ""base"" ShearYield and 10000 can be considered ""base"" ImpactYield
		
		-->
	<AbsorbExpressions>
		<Expression damagetype=""0"">damage*(0.8-(quality*0.02))</Expression>    <!-- Slashing -->
		<Expression damagetype=""1"">damage*(0.8-(quality*0.02))</Expression>    <!-- Chopping -->  
		<Expression damagetype=""2"">damage*(0.8-(quality*0.02))</Expression>    <!-- Crushing -->  
		<Expression damagetype=""3"">damage*(0.8-(quality*0.02))</Expression>    <!-- Piercing -->  
		<Expression damagetype=""4"">damage*(0.9-(quality*0.02))</Expression>    <!-- Ballistic -->  
		<Expression damagetype=""5"">damage*(0.5-(quality*0.02))</Expression>    <!-- Burning -->
		<Expression damagetype=""6"">damage*(0.5-(quality*0.02))</Expression>    <!-- Freezing -->
		<Expression damagetype=""7"">damage*(0.5-(quality*0.02))</Expression>    <!-- Chemical -->
		<Expression damagetype=""8"">damage*(0.8-(quality*0.02))</Expression>    <!-- Shockwave -->
		<Expression damagetype=""9"">damage*(0.8-(quality*0.02))</Expression>    <!-- Bite -->
		<Expression damagetype=""10"">damage*(0.8-(quality*0.02))</Expression>   <!-- Claw -->
		<Expression damagetype=""11"">damage*(0.5-(quality*0.02))</Expression>   <!-- Electrical -->
		<Expression damagetype=""12"">0</Expression>                             <!-- Hypoxia -->
		<Expression damagetype=""13"">0</Expression>                             <!-- Cellular -->
		<Expression damagetype=""14"">damage*(0.8-(quality*0.02))</Expression>   <!-- Sonic -->
		<Expression damagetype=""15"">damage*(0.8-(quality*0.02))</Expression>   <!-- Shearing --> 
		<Expression damagetype=""16"">damage*(1.0-(quality*0.02))</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">damage*(0.8-(quality*0.02))</Expression>   <!-- Wrenching -->
		<Expression damagetype=""18"">damage*(0.8-(quality*0.02))</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">damage*(0.8-(quality*0.02))</Expression>   <!-- Necrotic -->   
		<Expression damagetype=""20"">damage*(0.8-(quality*0.02))</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">damage*(0.8-(quality*0.02))</Expression>   <!-- Eldritch -->   
		<Expression damagetype=""22"">damage*(0.8-(quality*0.02))</Expression>   <!-- Arcane -->   
	</AbsorbExpressions>  
	<AbsorbExpressionsPain>
		<Expression damagetype=""0"">pain*(0.8-(quality*0.02))</Expression>    <!-- Slashing -->
		<Expression damagetype=""1"">pain*(0.8-(quality*0.02))</Expression>    <!-- Chopping -->  
		<Expression damagetype=""2"">pain*(0.8-(quality*0.02))</Expression>    <!-- Crushing -->  
		<Expression damagetype=""3"">pain*(0.8-(quality*0.02))</Expression>    <!-- Piercing -->  
		<Expression damagetype=""4"">pain*(0.9-(quality*0.02))</Expression>    <!-- Ballistic -->  
		<Expression damagetype=""5"">pain*(0.5-(quality*0.02))</Expression>    <!-- Burning -->
		<Expression damagetype=""6"">pain*(0.5-(quality*0.02))</Expression>    <!-- Freezing -->
		<Expression damagetype=""7"">pain*(0.5-(quality*0.02))</Expression>    <!-- Chemical -->
		<Expression damagetype=""8"">pain*(0.8-(quality*0.02))</Expression>    <!-- Shockwave -->
		<Expression damagetype=""9"">pain*(0.8-(quality*0.02))</Expression>    <!-- Bite -->
		<Expression damagetype=""10"">pain*(0.8-(quality*0.02))</Expression>   <!-- Claw -->
		<Expression damagetype=""11"">pain*(0.5-(quality*0.02))</Expression>   <!-- Electrical -->
		<Expression damagetype=""12"">0</Expression>                             <!-- Hypoxia -->
		<Expression damagetype=""13"">0</Expression>                             <!-- Cellular -->
		<Expression damagetype=""14"">pain*(0.8-(quality*0.02))</Expression>   <!-- Sonic -->
		<Expression damagetype=""15"">pain*(0.8-(quality*0.02))</Expression>   <!-- Shearing --> 
		<Expression damagetype=""16"">pain*(1.0-(quality*0.02))</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">pain*(0.8-(quality*0.02))</Expression>   <!-- Wrenching -->
		<Expression damagetype=""18"">pain*(0.8-(quality*0.02))</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">pain*(0.8-(quality*0.02))</Expression>   <!-- Necrotic -->   
		<Expression damagetype=""20"">pain*(0.8-(quality*0.02))</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">pain*(0.8-(quality*0.02))</Expression>   <!-- Eldritch -->   
		<Expression damagetype=""22"">pain*(0.8-(quality*0.02))</Expression>   <!-- Arcane -->   
	</AbsorbExpressionsPain>  
	<AbsorbExpressionsStun>
		<Expression damagetype=""0"">stun*(0.8-(quality*0.02))</Expression>    <!-- Slashing -->
		<Expression damagetype=""1"">stun*(0.8-(quality*0.02))</Expression>    <!-- Chopping -->  
		<Expression damagetype=""2"">stun*(0.8-(quality*0.02))</Expression>    <!-- Crushing -->  
		<Expression damagetype=""3"">stun*(0.8-(quality*0.02))</Expression>    <!-- Piercing -->  
		<Expression damagetype=""4"">stun*(0.9-(quality*0.02))</Expression>    <!-- Ballistic -->  
		<Expression damagetype=""5"">stun*(0.5-(quality*0.02))</Expression>    <!-- Burning -->
		<Expression damagetype=""6"">stun*(0.5-(quality*0.02))</Expression>    <!-- Freezing -->
		<Expression damagetype=""7"">stun*(0.5-(quality*0.02))</Expression>    <!-- Chemical -->
		<Expression damagetype=""8"">stun*(0.8-(quality*0.02))</Expression>    <!-- Shockwave -->
		<Expression damagetype=""9"">stun*(0.8-(quality*0.02))</Expression>    <!-- Bite -->
		<Expression damagetype=""10"">stun*(0.8-(quality*0.02))</Expression>   <!-- Claw -->
		<Expression damagetype=""11"">stun*(0.5-(quality*0.02))</Expression>   <!-- Electrical -->
		<Expression damagetype=""12"">0</Expression>                             <!-- Hypoxia -->
		<Expression damagetype=""13"">0</Expression>                             <!-- Cellular -->
		<Expression damagetype=""14"">stun*(0.8-(quality*0.02))</Expression>   <!-- Sonic -->
		<Expression damagetype=""15"">stun*(0.8-(quality*0.02))</Expression>   <!-- Shearing --> 
		<Expression damagetype=""16"">stun*(1.0-(quality*0.02))</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">stun*(0.8-(quality*0.02))</Expression>   <!-- Wrenching -->
		<Expression damagetype=""18"">stun*(0.8-(quality*0.02))</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">stun*(0.8-(quality*0.02))</Expression>   <!-- Necrotic -->   
		<Expression damagetype=""20"">stun*(0.8-(quality*0.02))</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">stun*(0.8-(quality*0.02))</Expression>   <!-- Eldritch -->   
		<Expression damagetype=""22"">stun*(0.8-(quality*0.02))</Expression>   <!-- Arcane -->   
	</AbsorbExpressionsStun>
 </ArmourType>"
		};
		_context.ArmourTypes.Add(_boneArmour);
		_context.SaveChanges();
	}

	private void SetupShapesAndMaterials()
	{
		Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Bodypart Shapes...");
		AddShape("Hoof");
		AddShape("Frog");
		AddShape("Claw");
		AddShape("Flank");
		AddShape("Withers");
		AddShape("Loin");
		AddShape("Rump");
		AddShape("Muzzle");
		AddShape("Upper Foreleg");
		AddShape("Lower Foreleg");
		AddShape("Upper Hindleg");
		AddShape("Lower Hindleg");
		AddShape("Front Hock");
		AddShape("Rear Hock");
		AddShape("Upper Leg");
		AddShape("Lower Leg");
		AddShape("Talon");
		AddShape("Dewclaw");
		AddShape("Tail");
		AddShape("Wing Base");
		AddShape("Wing");
		AddShape("Tusk");
		AddShape("Antler");
		AddShape("Horn");
		AddShape("Udder");
		AddShape("Fang");
		AddShape("Serpent Body");
		AddShape("Fin");
		AddShape("Gill");
		AddShape("Jaw");
		AddShape("Paw");
		AddShape("Beak");
		AddShape("Fin");
		AddShape("Peduncle");
		AddShape("Tendril");
		AddShape("Fluke");
		AddShape("Stock");
		AddShape("Mantle");
		AddShape("Arm");
		AddShape("Tentacle");
		AddShape("Body");
		AddShape("Head");
		AddShape("Front Flipper");
		AddShape("Hind Flipper");
		AddShape("Blowhole");
		AddShape("Insect Thorax");
		AddShape("Insect Abdomen");
		AddShape("Antenna");
		AddShape("Mandible");
		AddShape("Compound Eye");
		AddShape("Stinger");

		_context.SaveChanges();

		foreach (var shape in _context.BodypartShapes) _cachedShapes[shape.Name] = shape;

		foreach (var material in _context.Materials) _cachedMaterials[material.Name] = material;
	}

	#region Different Body Types

	private void SeedSerpents(BodyProto wormProto, BodyProto serpentProto)
	{
		ResetCachedParts();
		var order = 1;
		Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Bodyparts...");

		#region Bodyparts

		AddBodypart(wormProto, "head", "head", "head back", BodypartTypeEnum.Wear, null, Alignment.Front,
			Orientation.Highest, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true,
			isVital: true, implantSpace: 5, stunMultiplier: 1.0);
		AddBodypart(wormProto, "mouth", "mouth", "mouth", BodypartTypeEnum.Wear, "head", Alignment.Front,
			Orientation.Highest, 40, -1, 50, order++, "Bony Flesh", SizeCategory.Small, "Head", true,
			isVital: false, implantSpace: 0, stunMultiplier: 1.0);
		AddBodypart(serpentProto, "fangs", "fangs", "fang", BodypartTypeEnum.Wear, "mouth", Alignment.Front,
			Orientation.Highest, 40, -1, 50, order++, "Tooth", SizeCategory.Small, "Head", true, isVital: false,
			implantSpace: 0, stunMultiplier: 1.0);
		AddBodypart(wormProto, "reyesocket", "right eye socket", "eye socket", BodypartTypeEnum.Wear,
			"head", Alignment.FrontRight, Orientation.Highest, 80, -1, 100, order++, "Dense Bony Flesh",
			SizeCategory.Small, "Head", true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(wormProto, "leyesocket", "left eye socket", "eye socket", BodypartTypeEnum.Wear, "head",
			Alignment.FrontLeft, Orientation.Highest, 80, -1, 100, order++, "Dense Bony Flesh", SizeCategory.Small,
			"Head", true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(wormProto, "reye", "right eye", "eye", BodypartTypeEnum.Eye, "reyesocket",
			Alignment.FrontRight, Orientation.Highest, 10, 30, 100, order++, "Flesh", SizeCategory.Small, "Head",
			true, isVital: false, implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(wormProto, "leye", "left eye", "eye", BodypartTypeEnum.Eye, "leyesocket",
			Alignment.FrontLeft, Orientation.Highest, 10, 30, 100, order++, "Flesh", SizeCategory.Small, "Head",
			true, isVital: false, implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(serpentProto, "tongue", "tongue", "tongue", BodypartTypeEnum.Tongue, "mouth", Alignment.Front,
			Orientation.Highest, 10, 30, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true,
			isVital: true, implantSpace: 5, stunMultiplier: 1.0);
		AddBodypart(wormProto, "neck", "neck", "neck", BodypartTypeEnum.Wear, "head", Alignment.Front,
			Orientation.Highest, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true,
			isVital: true, implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(wormProto, "ubody", "upper body", "serpent body", BodypartTypeEnum.Wear, "neck",
			Alignment.Front, Orientation.High, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal, "Torso",
			true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(wormProto, "mbody", "middle body", "serpent body", BodypartTypeEnum.Wear, "ubody",
			Alignment.Front, Orientation.Centre, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal, "Torso",
			true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(wormProto, "lbody", "lower body", "serpent body", BodypartTypeEnum.Wear, "mbody",
			Alignment.Front, Orientation.Low, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal, "Torso",
			true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(wormProto, "tail", "tail", "tail", BodypartTypeEnum.Wear, "lbody", Alignment.Rear,
			Orientation.Lowest, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal, "Tail", true,
			isVital: true, implantSpace: 5, stunMultiplier: 0.5);

		#endregion

		_context.SaveChanges();

		#region Bones

		AddBone(serpentProto, "fskull", "frontal skull bone", BodypartTypeEnum.NonImmobilisingBone, 200,
			"Compact Bone");
		AddBone(serpentProto, "cvertebrae", "cervical vertebrae", BodypartTypeEnum.NonImmobilisingBone, 100,
			"Compact Bone");
		AddBone(serpentProto, "dvertebrae", "dorsal vertebrae", BodypartTypeEnum.NonImmobilisingBone, 100,
			"Compact Bone");
		AddBone(serpentProto, "lvertebrae", "lumbar vertebrae", BodypartTypeEnum.NonImmobilisingBone, 100,
			"Compact Bone");
		AddBone(serpentProto, "cavertebrae", "caudal vertebrae", BodypartTypeEnum.NonImmobilisingBone, 100,
			"Compact Bone");
		_context.SaveChanges();

		AddBoneInternal("fskull", "head", 100);
		AddBoneInternal("cvertebrae", "neck", 100);
		AddBoneInternal("dvertebrae", "ubody", 100);
		AddBoneInternal("lvertebrae", "mbody", 100);
		AddBoneInternal("lvertebrae", "lbody", 100, false);
		AddBoneInternal("cavertebrae", "tail", 100);
		_context.SaveChanges();

		#endregion

		Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Organs...");

		#region Organs

		AddOrgan(wormProto, "brain", "brain", BodypartTypeEnum.Brain, 2.0, 50, 0.2, 0.2, 0.1, stunModifier: 1.0);
		AddOrgan(wormProto, "heart", "heart", BodypartTypeEnum.Heart, 1.0, 50, 0.2, 1.0, 1.0);
		AddOrgan(wormProto, "liver", "liver", BodypartTypeEnum.Liver, 3.0, 50, 0.2, 1.0, 0.05);
		AddOrgan(wormProto, "spleen", "spleen", BodypartTypeEnum.Spleen, 1.0, 50, 0.2, 1.0, 0.05);
		AddOrgan(wormProto, "stomach", "stomach", BodypartTypeEnum.Stomach, 1.0, 50, 0.2, 1.0, 0.05);
		AddOrgan(wormProto, "lintestines", "large intestines", BodypartTypeEnum.Intestines, 0.5, 50, 0.2, 1.0,
			0.05);
		AddOrgan(wormProto, "sintestines", "small intestines", BodypartTypeEnum.Intestines, 2.0, 50, 0.2, 1.0,
			0.05);
		AddOrgan(wormProto, "rkidney", "right kidney", BodypartTypeEnum.Kidney, 0.5, 50, 0.2, 2.0, 0.05,
			painModifier: 3.0);
		AddOrgan(wormProto, "lkidney", "left kidney", BodypartTypeEnum.Kidney, 0.5, 50, 0.2, 2.0, 0.05,
			painModifier: 3.0);
		AddOrgan(wormProto, "rlung", "right lung", BodypartTypeEnum.Lung, 2.0, 50, 0.2, 1.0, 0.05);
		AddOrgan(wormProto, "llung", "left lung", BodypartTypeEnum.Lung, 2.0, 50, 0.2, 1.0, 0.05);
		AddOrgan(wormProto, "trachea", "trachea", BodypartTypeEnum.Trachea, 1.0, 50, 0.2, 1.0, 0.05);
		AddOrgan(wormProto, "esophagus", "esophagus", BodypartTypeEnum.Esophagus, 1.0, 50, 0.2, 1.0, 0.05);
		AddOrgan(wormProto, "spinalcord", "spinal cord", BodypartTypeEnum.Spine, 1.0, 15, 0.2, 1.0, 0.05,
			stunModifier: 1.0, painModifier: 2.0);
		AddOrgan(wormProto, "rinnerear", "right inner ear", BodypartTypeEnum.Ear, 1.0, 15, 0.2, 1.0, 0.05);
		AddOrgan(wormProto, "linnerear", "left inner ear", BodypartTypeEnum.Ear, 1.0, 15, 0.2, 1.0, 0.05);

		AddOrganCoverage("brain", "head", 100, true);
		AddOrganCoverage("brain", "reyesocket", 85);
		AddOrganCoverage("brain", "leyesocket", 85);
		AddOrganCoverage("brain", "reye", 85);
		AddOrganCoverage("brain", "leye", 85);
		AddOrganCoverage("brain", "mouth", 10);

		AddOrganCoverage("linnerear", "head", 5, true);
		AddOrganCoverage("rinnerear", "head", 5, true);
		AddOrganCoverage("esophagus", "neck", 50, true);
		AddOrganCoverage("trachea", "neck", 50, true);

		AddOrganCoverage("rlung", "ubody", 100, true);
		AddOrganCoverage("llung", "ubody", 100, true);

		AddOrganCoverage("heart", "ubody", 33, true);

		AddOrganCoverage("spinalcord", "neck", 10, true);
		AddOrganCoverage("spinalcord", "ubody", 2);

		AddOrganCoverage("liver", "mbody", 33, true);
		AddOrganCoverage("spleen", "mbody", 20, true);
		AddOrganCoverage("stomach", "mbody", 20, true);

		AddOrganCoverage("lintestines", "lbody", 5, true);
		AddOrganCoverage("sintestines", "lbody", 50, true);

		AddOrganCoverage("rkidney", "lbody", 20, true);
		AddOrganCoverage("lkidney", "lbody", 20, true);

		AddBoneCover("fskull", "brain", 100);
		AddBoneCover("cvertebrae", "spinalcord", 100);
		AddBoneCover("dvertebrae", "spinalcord", 100);
		AddBoneCover("lvertebrae", "spinalcord", 100);
		AddBoneCover("cavertebrae", "spinalcord", 100);
		_context.SaveChanges();

		#endregion

		_context.SaveChanges();

		foreach (var (child, parent) in _cachedBodypartUpstreams)
			_context.BodypartProtoBodypartProtoUpstream.Add(new BodypartProtoBodypartProtoUpstream
			{
				Child = child.Id,
				Parent = parent.Id
			});

		_context.SaveChanges();

		Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Limbs...");

		#region Limbs

		var limbs = new Dictionary<string, Limb>(StringComparer.OrdinalIgnoreCase);

		void AddLimb(string name, LimbType limbType, string rootPart, double damageThreshold,
			double painThreshold)
		{
			var limb = new Limb
			{
				Name = name,
				LimbType = (int)limbType,
				RootBody = wormProto,
				RootBodypart = _cachedBodyparts[rootPart],
				LimbDamageThresholdMultiplier = damageThreshold,
				LimbPainThresholdMultiplier = painThreshold
			};
			_context.Limbs.Add(limb);
			limbs[name] = limb;
		}

		AddLimb("Torso", LimbType.Torso, "ubody", 1.0, 1.0);
		AddLimb("Head", LimbType.Head, "neck", 1.0, 1.0);
		AddLimb("Tail", LimbType.Appendage, "tail", 0.5, 0.5);
		_context.SaveChanges();

		foreach (var limb in limbs.Values)
		{
			foreach (var part in _cachedLimbs[limb.Name])
				_context.LimbsBodypartProto.Add(new LimbBodypartProto { BodypartProto = part, Limb = limb });

			switch (limb.Name)
			{
				case "Torso":
					_context.LimbsSpinalParts.Add(new LimbsSpinalPart
					{ Limb = limb, BodypartProto = _cachedOrgans["spinalcord"] });
					break;
			}
		}

		_context.SaveChanges();

		#endregion

		Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Groups...");

		#region Groups

		AddBodypartGroupDescriberShape(serpentProto, "body", "The whole torso of a worm",
			("serpent body", 1, 3),
			("tail", 0, 1),
			("neck", 0, 1)
		);

		AddBodypartGroupDescriberShape(serpentProto, "eyes", "A pair of serpent eyes",
			("eye socket", 2, 2),
			("eye", 0, 2)
		);

		#endregion

		_context.SaveChanges();

		Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Races...");

		#region Races

		SeedAnimalRaces(GetSerpentRaceTemplates(),
			("Serpentine", serpentProto));

		#endregion
	}

	private void SeedAquatic(BodyProto fishProto, BodyProto crabProto, BodyProto malacostracanProto, BodyProto octopusProto,
		BodyProto jellyfishProto, BodyProto pinnipedProto, BodyProto cetaceanProto)
	{
		Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Fish...");

		#region Fish

		ResetCachedParts();
		var order = 1;

		#region Torso

		AddBodypart(fishProto, "abdomen", "abdomen", "abdomen", BodypartTypeEnum.BonyDrapeable, null, Alignment.Front,
			Orientation.Low, 80, -1, 100, order++, "Fatty Flesh", SizeCategory.Normal, "Torso", true, isVital: true,
			implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(fishProto, "rbreast", "right breast", "breast", BodypartTypeEnum.BonyDrapeable, "abdomen",
			Alignment.FrontRight, Orientation.Low, 80, -1, 100, order++, "Flesh", SizeCategory.Normal, "Torso",
			true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(fishProto, "lbreast", "left breast", "breast", BodypartTypeEnum.BonyDrapeable, "abdomen",
			Alignment.FrontLeft, Orientation.Low, 80, -1, 100, order++, "Flesh", SizeCategory.Normal, "Torso", true,
			isVital: true, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(fishProto, "urflank", "upper right flank", "flank", BodypartTypeEnum.BonyDrapeable, "rbreast",
			Alignment.Right, Orientation.Centre, 80, -1, 200, order++, "Flesh", SizeCategory.Normal, "Torso", true,
			isVital: true, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(fishProto, "ulflank", "upper left flank", "flank", BodypartTypeEnum.BonyDrapeable, "lbreast",
			Alignment.Left, Orientation.Centre, 80, -1, 200, order++, "Flesh", SizeCategory.Normal, "Torso", true,
			isVital: true, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(fishProto, "lrflank", "lower right flank", "flank", BodypartTypeEnum.BonyDrapeable, "abdomen",
			Alignment.RearRight, Orientation.Centre, 80, -1, 200, order++, "Flesh", SizeCategory.Normal, "Torso",
			true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(fishProto, "llflank", "lower left flank", "flank", BodypartTypeEnum.BonyDrapeable, "abdomen",
			Alignment.RearLeft, Orientation.Centre, 80, -1, 200, order++, "Flesh", SizeCategory.Normal, "Torso",
			true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(fishProto, "belly", "belly", "belly", BodypartTypeEnum.Wear, "abdomen", Alignment.Front,
			Orientation.Low, 80, -1, 100, order++, "Fatty Flesh", SizeCategory.Normal, "Torso", true, isVital: true,
			implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(fishProto, "uback", "upper back", "upper back", BodypartTypeEnum.BonyDrapeable, "abdomen",
			Alignment.Front, Orientation.High, 80, -1, 200, order++, "Bony Flesh", SizeCategory.Normal, "Torso",
			true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(fishProto, "lback", "lower back", "lower back", BodypartTypeEnum.BonyDrapeable, "abdomen",
			Alignment.Rear, Orientation.High, 80, -1, 200, order++, "Bony Flesh", SizeCategory.Normal, "Torso",
			true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(fishProto, "loin", "loin", "loin", BodypartTypeEnum.Wear, "belly", Alignment.Rear,
			Orientation.Low, 80, -1, 100, order++, "Fatty Flesh", SizeCategory.Normal, "Torso", true, isVital: true,
			implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(fishProto, "dorsalfin", "dorsal fin", "fin", BodypartTypeEnum.Fin, "uback", Alignment.Rear,
			Orientation.Highest, 20, 35, 100, order++, "Fin", SizeCategory.Normal, "Torso");
		AddBodypart(fishProto, "analfin", "anal fin", "fin", BodypartTypeEnum.Fin, "loin", Alignment.Rear,
			Orientation.Lowest, 20, 35, 100, order++, "Fin", SizeCategory.Normal, "Torso");
		AddBodypart(fishProto, "rpectoralfin", "right pectoral fin", "fin", BodypartTypeEnum.Fin, "urflank",
			Alignment.Right, Orientation.Centre, 20, 35, 100, order++, "Fin", SizeCategory.Normal, "Torso");
		AddBodypart(fishProto, "lpectoralfin", "left pectoral fin", "fin", BodypartTypeEnum.Fin, "ulflank",
			Alignment.Left, Orientation.Centre, 20, 35, 100, order++, "Fin", SizeCategory.Normal, "Torso");
		AddBodypart(fishProto, "rpelvicfin", "right pelvic fin", "fin", BodypartTypeEnum.Fin, "belly",
			Alignment.Right, Orientation.Centre, 20, 35, 100, order++, "Fin", SizeCategory.Normal, "Torso");
		AddBodypart(fishProto, "lpelvicfin", "left pelvic fin", "fin", BodypartTypeEnum.Fin, "belly",
			Alignment.Left, Orientation.Low, 20, 35, 100, order++, "Fin", SizeCategory.Normal, "Torso");

		#endregion

		#region Head

		AddBodypart(fishProto, "neck", "neck", "neck", BodypartTypeEnum.BonyDrapeable, "uback", Alignment.Front,
			Orientation.High, 80, 100, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true, isVital: true,
			implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(fishProto, "rgill", "right gills", "gill", BodypartTypeEnum.Gill, "urflank", Alignment.Right,
			Orientation.High, 80, 100, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true, isVital: true,
			implantSpace: 5, stunMultiplier: 1.0);
		AddBodypart(fishProto, "lgill", "left gills", "gill", BodypartTypeEnum.Gill, "ulflank", Alignment.Left,
			Orientation.High, 80, 100, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true, isVital: true,
			implantSpace: 5, stunMultiplier: 1.0);
		AddBodypart(fishProto, "head", "head", "face", BodypartTypeEnum.BonyDrapeable, "neck", Alignment.Front,
			Orientation.Highest, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true,
			isVital: true, implantSpace: 5, stunMultiplier: 1.0);
		AddBodypart(fishProto, "reyesocket", "right eye socket", "eye socket", BodypartTypeEnum.BonyDrapeable, "head",
			Alignment.FrontRight, Orientation.Highest, 80, -1, 100, order++, "Dense Bony Flesh", SizeCategory.Small,
			"Head", true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(fishProto, "leyesocket", "left eye socket", "eye socket", BodypartTypeEnum.BonyDrapeable, "head",
			Alignment.FrontLeft, Orientation.Highest, 80, -1, 100, order++, "Dense Bony Flesh", SizeCategory.Small,
			"Head", true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(fishProto, "reye", "right eye", "eye", BodypartTypeEnum.Eye, "reyesocket", Alignment.FrontRight,
			Orientation.Highest, 10, 30, 100, order++, "Dense Bony Flesh", SizeCategory.Small, "Head", true,
			isVital: true, implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(fishProto, "leye", "left eye", "eye", BodypartTypeEnum.Eye, "leyesocket", Alignment.FrontLeft,
			Orientation.Highest, 10, 30, 100, order++, "Dense Bony Flesh", SizeCategory.Small, "Head", true,
			isVital: true, implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(fishProto, "mouth", "mouth", "mouth", BodypartTypeEnum.Mouth, "head", Alignment.Front,
			Orientation.Highest, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true,
			isVital: true, implantSpace: 5, stunMultiplier: 1.0);

		#endregion

		#region Tail

		AddBodypart(fishProto, "peduncle", "peduncle", "Peduncle", BodypartTypeEnum.Wear, "lback", Alignment.Rear,
	Orientation.Centre, 30, 50, 100, order++, "Flesh", SizeCategory.Normal, "Tail");
		AddBodypart(fishProto, "caudalfin", "caudal fin", "fin", BodypartTypeEnum.Fin, "peduncle", Alignment.Rear,
			Orientation.Centre, 20, 35, 100, order++, "Fin", SizeCategory.Normal, "Tail");

		#endregion

		_context.SaveChanges();

		#region Bones

		AddBone(fishProto, "fskull", "frontal skull bone", BodypartTypeEnum.NonImmobilisingBone, 200,
			"Compact Bone");
		AddBone(fishProto, "cvertebrae", "cervical vertebrae", BodypartTypeEnum.NonImmobilisingBone, 100,
			"Compact Bone");
		AddBone(fishProto, "dvertebrae", "dorsal vertebrae", BodypartTypeEnum.NonImmobilisingBone, 100,
			"Compact Bone");
		AddBone(fishProto, "lvertebrae", "lumbar vertebrae", BodypartTypeEnum.NonImmobilisingBone, 100,
			"Compact Bone");
		AddBone(fishProto, "cavertebrae", "caudal vertebrae", BodypartTypeEnum.NonImmobilisingBone, 100,
			"Compact Bone");
		_context.SaveChanges();

		AddBoneInternal("fskull", "head", 100);
		AddBoneInternal("cvertebrae", "neck", 100);
		AddBoneInternal("dvertebrae", "uback", 100);
		AddBoneInternal("lvertebrae", "lback", 100);
		AddBoneInternal("cavertebrae", "peduncle", 100);
		_context.SaveChanges();

		#endregion

		#region Organs

		AddOrgan(fishProto, "brain", "brain", BodypartTypeEnum.Brain, 2.0, 50, 0.2, 0.2, 0.1, stunModifier: 1.0);
		AddOrgan(fishProto, "heart", "heart", BodypartTypeEnum.Heart, 1.0, 50, 0.2, 1.0, 1.0);
		AddOrgan(fishProto, "liver", "liver", BodypartTypeEnum.Liver, 3.0, 50, 0.2, 1.0, 0.05);
		AddOrgan(fishProto, "spleen", "spleen", BodypartTypeEnum.Spleen, 1.0, 50, 0.2, 1.0, 0.05);
		AddOrgan(fishProto, "stomach", "stomach", BodypartTypeEnum.Stomach, 1.0, 50, 0.2, 1.0, 0.05);
		AddOrgan(fishProto, "lintestines", "large intestines", BodypartTypeEnum.Intestines, 0.5, 50, 0.2, 1.0,
			0.05);
		AddOrgan(fishProto, "sintestines", "small intestines", BodypartTypeEnum.Intestines, 2.0, 50, 0.2, 1.0,
			0.05);
		AddOrgan(fishProto, "rkidney", "right kidney", BodypartTypeEnum.Kidney, 0.5, 50, 0.2, 2.0, 0.05,
			painModifier: 3.0);
		AddOrgan(fishProto, "lkidney", "left kidney", BodypartTypeEnum.Kidney, 0.5, 50, 0.2, 2.0, 0.05,
			painModifier: 3.0);
		AddOrgan(fishProto, "esophagus", "esophagus", BodypartTypeEnum.Esophagus, 1.0, 50, 0.2, 1.0, 0.05);
		AddOrgan(fishProto, "uspinalcord", "upper spinal cord", BodypartTypeEnum.Spine, 1.0, 15, 0.2, 1.0, 0.05,
			stunModifier: 1.0, painModifier: 2.0);
		AddOrgan(fishProto, "mspinalcord", "middle spinal cord", BodypartTypeEnum.Spine, 1.0, 15, 0.2, 1.0, 0.05,
			stunModifier: 1.0, painModifier: 2.0);
		AddOrgan(fishProto, "lspinalcord", "lower spinal cord", BodypartTypeEnum.Spine, 1.0, 15, 0.2, 1.0, 0.05,
			stunModifier: 1.0, painModifier: 2.0);
		AddOrgan(fishProto, "rinnerear", "right inner ear", BodypartTypeEnum.Ear, 1.0, 15, 0.2, 1.0, 0.05);
		AddOrgan(fishProto, "linnerear", "left inner ear", BodypartTypeEnum.Ear, 1.0, 15, 0.2, 1.0, 0.05);

		AddOrganCoverage("brain", "head", 100, true);
		AddOrganCoverage("brain", "reyesocket", 85);
		AddOrganCoverage("brain", "leyesocket", 85);
		AddOrganCoverage("brain", "reye", 85);
		AddOrganCoverage("brain", "leye", 85);

		AddOrganCoverage("linnerear", "head", 10, true);
		AddOrganCoverage("rinnerear", "head", 10, true);
		AddOrganCoverage("esophagus", "neck", 20, true);

		AddOrganCoverage("heart", "lbreast", 33, true);

		AddOrganCoverage("uspinalcord", "neck", 2, true);
		AddOrganCoverage("mspinalcord", "uback", 10, true);
		AddOrganCoverage("lspinalcord", "lback", 10, true);
		AddOrganCoverage("lspinalcord", "peduncle", 10);

		AddOrganCoverage("liver", "abdomen", 33, true);
		AddOrganCoverage("spleen", "abdomen", 20, true);
		AddOrganCoverage("stomach", "abdomen", 20, true);
		AddOrganCoverage("liver", "uback", 15);
		AddOrganCoverage("spleen", "uback", 10);
		AddOrganCoverage("stomach", "uback", 5);

		AddOrganCoverage("lintestines", "belly", 5, true);
		AddOrganCoverage("sintestines", "belly", 50, true);
		AddOrganCoverage("lintestines", "lback", 5);
		AddOrganCoverage("sintestines", "lback", 33);
		AddOrganCoverage("lintestines", "loin", 5);

		AddOrganCoverage("rkidney", "lback", 20, true);
		AddOrganCoverage("lkidney", "lback", 20, true);
		AddOrganCoverage("rkidney", "belly", 5);
		AddOrganCoverage("lkidney", "belly", 5);

		AddBoneCover("fskull", "brain", 100);
		AddBoneCover("cvertebrae", "uspinalcord", 100);
		AddBoneCover("dvertebrae", "mspinalcord", 100);
		AddBoneCover("lvertebrae", "lspinalcord", 100);
		AddBoneCover("cavertebrae", "lspinalcord", 100);
		_context.SaveChanges();

		#endregion

		_context.SaveChanges();

		foreach (var (child, parent) in _cachedBodypartUpstreams)
			_context.BodypartProtoBodypartProtoUpstream.Add(new BodypartProtoBodypartProtoUpstream
			{
				Child = child.Id,
				Parent = parent.Id
			});

		_context.SaveChanges();

		#region Limbs

		var limbs = new Dictionary<string, Limb>(StringComparer.OrdinalIgnoreCase);

		void AddLimb(string name, LimbType limbType, string rootPart, double damageThreshold,
			double painThreshold, BodyProto proto)
		{
			var limb = new Limb
			{
				Name = name,
				LimbType = (int)limbType,
				RootBody = proto,
				RootBodypart = _cachedBodyparts[rootPart],
				LimbDamageThresholdMultiplier = damageThreshold,
				LimbPainThresholdMultiplier = painThreshold
			};
			_context.Limbs.Add(limb);
			limbs[name] = limb;
		}

		AddLimb("Torso", LimbType.Torso, "abdomen", 1.0, 1.0, fishProto);
		AddLimb("Head", LimbType.Head, "neck", 1.0, 1.0, fishProto);
		AddLimb("Tail", LimbType.Appendage, "peduncle", 0.5, 0.5, fishProto);
		_context.SaveChanges();

		foreach (var limb in limbs.Values)
		{
			foreach (var part in _cachedLimbs[limb.Name])
				_context.LimbsBodypartProto.Add(new LimbBodypartProto { BodypartProto = part, Limb = limb });

			switch (limb.Name)
			{
				case "Torso":
					_context.LimbsSpinalParts.Add(new LimbsSpinalPart
					{ Limb = limb, BodypartProto = _cachedOrgans["uspinalcord"] });
					_context.LimbsSpinalParts.Add(new LimbsSpinalPart
					{ Limb = limb, BodypartProto = _cachedOrgans["mspinalcord"] });
					break;
				case "Tail":
					_context.LimbsSpinalParts.Add(new LimbsSpinalPart
					{ Limb = limb, BodypartProto = _cachedOrgans["lspinalcord"] });
					break;
			}
		}

		_context.SaveChanges();

		#endregion

		#region Groups

		AddBodypartGroupDescriberShape(fishProto, "body", "The whole torso of a fish",
			("abdomen", 0, 1),
			("breast", 0, 2),
			("flank", 0, 4),
			("belly", 0, 1),
			("loin", 0, 1),
			("upper back", 1, 1),
			("lower back", 1, 1),
			("neck", 0, 1),
			("gill", 0, 2),
			("peduncle", 0, 1)
		);

		AddBodypartGroupDescriberShape(fishProto, "fins", "All the fins of a fish",
			("fin", 2, 6)
		);

		AddBodypartGroupDescriberDirect(fishProto, "pectoral fins", "The pectoral fins of a fish",
			("rpectoralfin", true),
			("lpectoralfin", true)
		);

		AddBodypartGroupDescriberShape(fishProto, "head", "The eyes of a fish",
			("eye socket", 0, 2),
			("eye", 0, 2)
		);

		AddBodypartGroupDescriberDirect(fishProto, "gills", "The gills of a fish",
			("rgill", true),
			("lgill", true)
		);

		AddBodypartGroupDescriberShape(fishProto, "head", "The whole head of a fish",
			("face", 0, 1),
			("eye socket", 0, 2),
			("eye", 0, 2),
			("mouth", 0, 1),
			("neck", 0, 1),
			("gill", 0, 2)
		);

		AddBodypartGroupDescriberDirect(fishProto, "pelvic fins", "The pelvic fins of a fish",
			("rpelvicfin", true),
			("lpelvicfin", true)
		);

		AddBodypartGroupDescriberDirect(fishProto, "tail", "The whole of a fish",
			("caudalfin", true),
			("peduncle", true)
		);

		#endregion

		_context.SaveChanges();

		#endregion

		#region Crabs

		SeedDecapodBody(crabProto);
		SeedMalacostracanBody(malacostracanProto);

		#endregion

		Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Cephalopods...");

		#region Cephalopods

		ResetCachedParts();
		order = 1;

		#region Torso

		AddBodypart(octopusProto, "abdomen", "abdomen", "abdomen", BodypartTypeEnum.Wear, null, Alignment.Front,
			Orientation.Centre, 80, -1, 100, order++, "Fatty Flesh", SizeCategory.Normal, "Torso", true, isVital: true,
			implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(octopusProto, "mouth", "mouth", "mouth", BodypartTypeEnum.Mouth, "abdomen", Alignment.Front,
			Orientation.Centre, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal, "Torso", true,
			isVital: true, implantSpace: 5, stunMultiplier: 1.0);

		#endregion

		#region Head

		AddBodypart(octopusProto, "head", "head", "head", BodypartTypeEnum.Wear, "abdomen", Alignment.Front,
			Orientation.High, 80, 100, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true, isVital: true,
			implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(octopusProto, "mantle", "mantle", "mantle", BodypartTypeEnum.Wear, "head", Alignment.Front,
			Orientation.Highest, 80, 100, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true, isVital: true,
			implantSpace: 5, stunMultiplier: 1.0);
		AddBodypart(octopusProto, "reye", "right eye", "eye", BodypartTypeEnum.Eye, "head", Alignment.FrontRight,
			Orientation.Highest, 10, 30, 100, order++, "Flesh", SizeCategory.Small, "Head", true,
			isVital: true, implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(octopusProto, "leye", "left eye", "eye", BodypartTypeEnum.Eye, "head", Alignment.FrontLeft,
			Orientation.Highest, 10, 30, 100, order++, "Flesh", SizeCategory.Small, "Head", true,
			isVital: true, implantSpace: 5, stunMultiplier: 0.5);

		#endregion

		#region Appendages

		AddBodypart(octopusProto, "arm1", "1st Arm", "arm", BodypartTypeEnum.GrabbingWielding, "abdomen",
			Alignment.Front,
			Orientation.Low, 30, 50, 100, order++, "Flesh", SizeCategory.Normal, "1st Arm");
		AddBodypart(octopusProto, "arm2", "2nd Arm", "arm", BodypartTypeEnum.GrabbingWielding, "abdomen",
			Alignment.Front,
			Orientation.Low, 30, 50, 100, order++, "Flesh", SizeCategory.Normal, "2nd Arm");
		AddBodypart(octopusProto, "arm3", "3rd Arm", "arm", BodypartTypeEnum.GrabbingWielding, "abdomen",
			Alignment.Front,
			Orientation.Low, 30, 50, 100, order++, "Flesh", SizeCategory.Normal, "3rd Arm");
		AddBodypart(octopusProto, "arm4", "4th Arm", "arm", BodypartTypeEnum.GrabbingWielding, "abdomen",
			Alignment.Front,
			Orientation.Low, 30, 50, 100, order++, "Flesh", SizeCategory.Normal, "4th Arm");
		AddBodypart(octopusProto, "arm5", "5th Arm", "arm", BodypartTypeEnum.GrabbingWielding, "abdomen",
			Alignment.Front,
			Orientation.Low, 30, 50, 100, order++, "Flesh", SizeCategory.Normal, "5th Arm");
		AddBodypart(octopusProto, "arm6", "6th Arm", "arm", BodypartTypeEnum.GrabbingWielding, "abdomen",
			Alignment.Front,
			Orientation.Low, 30, 50, 100, order++, "Flesh", SizeCategory.Normal, "6th Arm");
		AddBodypart(octopusProto, "arm7", "7th Arm", "arm", BodypartTypeEnum.GrabbingWielding, "abdomen",
			Alignment.Front,
			Orientation.Low, 30, 50, 100, order++, "Flesh", SizeCategory.Normal, "7th Arm");
		AddBodypart(octopusProto, "arm8", "8th Arm", "arm", BodypartTypeEnum.GrabbingWielding, "abdomen",
			Alignment.Front,
			Orientation.Low, 30, 50, 100, order++, "Flesh", SizeCategory.Normal, "8th Arm");
		AddBodypart(octopusProto, "tentacle1", "1st Tentacle", "tentacle", BodypartTypeEnum.GrabbingWielding, "abdomen",
			Alignment.Front,
			Orientation.Lowest, 30, 50, 100, order++, "Flesh", SizeCategory.Normal, "1st Tentacle", isCore: false);
		AddBodypart(octopusProto, "tentacle2", "2nd Tentacle", "tentacle", BodypartTypeEnum.GrabbingWielding, "abdomen",
			Alignment.Front,
			Orientation.Lowest, 30, 50, 100, order++, "Flesh", SizeCategory.Normal, "2nd Tentacle", isCore: false);

		#endregion

		_context.SaveChanges();

		#region Organs

		AddOrgan(octopusProto, "brain", "brain", BodypartTypeEnum.Brain, 2.0, 50, 0.2, 0.2, 0.1, stunModifier: 1.0);
		AddOrgan(octopusProto, "heart", "heart", BodypartTypeEnum.Heart, 1.0, 50, 0.2, 1.0, 1.0);
		AddOrgan(octopusProto, "liver", "liver", BodypartTypeEnum.Liver, 3.0, 50, 0.2, 1.0, 0.05);
		AddOrgan(octopusProto, "spleen", "spleen", BodypartTypeEnum.Spleen, 1.0, 50, 0.2, 1.0, 0.05);
		AddOrgan(octopusProto, "stomach", "stomach", BodypartTypeEnum.Stomach, 1.0, 50, 0.2, 1.0, 0.05);
		AddOrgan(octopusProto, "intestines", "intestines", BodypartTypeEnum.Intestines, 0.5, 50, 0.2, 1.0,
			0.05);
		AddOrgan(octopusProto, "rkidney", "right kidney", BodypartTypeEnum.Kidney, 0.5, 50, 0.2, 2.0, 0.05,
			painModifier: 3.0);
		AddOrgan(octopusProto, "lkidney", "left kidney", BodypartTypeEnum.Kidney, 0.5, 50, 0.2, 2.0, 0.05,
			painModifier: 3.0);
		AddOrgan(octopusProto, "esophagus", "esophagus", BodypartTypeEnum.Esophagus, 1.0, 50, 0.2, 1.0, 0.05);
		AddOrgan(octopusProto, "rinnerear", "right inner ear", BodypartTypeEnum.Ear, 1.0, 15, 0.2, 1.0, 0.05);
		AddOrgan(octopusProto, "linnerear", "left inner ear", BodypartTypeEnum.Ear, 1.0, 15, 0.2, 1.0, 0.05);

		AddOrganCoverage("brain", "head", 100, true);
		AddOrganCoverage("brain", "reye", 85);
		AddOrganCoverage("brain", "leye", 85);
		AddOrganCoverage("brain", "mouth", 5);
		AddOrganCoverage("linnerear", "head", 10, true);
		AddOrganCoverage("rinnerear", "head", 10, true);
		AddOrganCoverage("esophagus", "abdomen", 20, true);
		AddOrganCoverage("heart", "mantle", 33, true);
		AddOrganCoverage("liver", "mantle", 33, true);
		AddOrganCoverage("spleen", "mantle", 20, true);
		AddOrganCoverage("stomach", "mantle", 20, true);
		AddOrganCoverage("intestines", "mantle", 5, true);
		AddOrganCoverage("rkidney", "mantle", 20, true);
		AddOrganCoverage("lkidney", "mantle", 20, true);
		_context.SaveChanges();

		#endregion

		_context.SaveChanges();

		foreach (var (child, parent) in _cachedBodypartUpstreams)
			_context.BodypartProtoBodypartProtoUpstream.Add(new BodypartProtoBodypartProtoUpstream
			{
				Child = child.Id,
				Parent = parent.Id
			});

		_context.SaveChanges();

		#region Limbs

		limbs = new Dictionary<string, Limb>(StringComparer.OrdinalIgnoreCase);
		AddLimb("Torso", LimbType.Torso, "abdomen", 1.0, 1.0, octopusProto);
		AddLimb("Head", LimbType.Head, "head", 1.0, 1.0, octopusProto);
		AddLimb("1st Arm", LimbType.Arm, "arm1", 0.5, 0.5, octopusProto);
		AddLimb("2nd Arm", LimbType.Arm, "arm2", 0.5, 0.5, octopusProto);
		AddLimb("3rd Arm", LimbType.Arm, "arm3", 0.5, 0.5, octopusProto);
		AddLimb("4th Arm", LimbType.Arm, "arm4", 0.5, 0.5, octopusProto);
		AddLimb("5th Arm", LimbType.Arm, "arm5", 0.5, 0.5, octopusProto);
		AddLimb("6th Arm", LimbType.Arm, "arm6", 0.5, 0.5, octopusProto);
		AddLimb("7th Arm", LimbType.Arm, "arm7", 0.5, 0.5, octopusProto);
		AddLimb("8th Arm", LimbType.Arm, "arm8", 0.5, 0.5, octopusProto);
		AddLimb("1st Tentacle", LimbType.Appendage, "tentacle1", 0.5, 0.5, octopusProto);
		AddLimb("2nd Tentacle", LimbType.Appendage, "tentacle2", 0.5, 0.5, octopusProto);
		_context.SaveChanges();

		foreach (var limb in limbs.Values)
			foreach (var part in _cachedLimbs[limb.Name])
				_context.LimbsBodypartProto.Add(new LimbBodypartProto { BodypartProto = part, Limb = limb });

		#endregion

		#region Groups

		AddBodypartGroupDescriberShape(octopusProto, "body", "The whole torso of a cephalopod",
			("abdomen", 0, 1),
			("head", 0, 1),
			("mantle", 0, 1),
			("mouth", 0, 1)
		);

		AddBodypartGroupDescriberShape(octopusProto, "arms", "All the arms of a cephalopod",
			("arm", 2, 8)
		);

		AddBodypartGroupDescriberShape(octopusProto, "tentacles", "All the tentacles of a cephalopod",
			("tentacle", 2, 2)
		);

		AddBodypartGroupDescriberShape(octopusProto, "eyes", "The eyes of a cephalopod",
			("eye", 0, 2)
		);

		#endregion

		_context.SaveChanges();

		#endregion

		Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Jellyfish...");

		#region Jellyfish

		ResetCachedParts();
		order = 1;

		#region Torso

		AddBodypart(jellyfishProto, "body", "body", "abdomen", BodypartTypeEnum.Wear, null, Alignment.Front,
			Orientation.Low, 80, -1, 100, order++, "Fatty Flesh", SizeCategory.Normal, "Torso", true, isVital: true,
			implantSpace: 5, stunMultiplier: 0.2);

		#endregion

		#region Appendages

		for (var i = 1; i < 11; i++)
			AddBodypart(jellyfishProto, $"tendril{i}", $"{i.ToOrdinal()} tendril", "tendril",
				BodypartTypeEnum.Wear, "body", Alignment.Front,
				Orientation.Lowest, 30, 50, 100, order++, "Flesh", SizeCategory.Small, $"Tendril{i}");

		#endregion

		_context.SaveChanges();

		foreach (var (child, parent) in _cachedBodypartUpstreams)
			_context.BodypartProtoBodypartProtoUpstream.Add(new BodypartProtoBodypartProtoUpstream
			{
				Child = child.Id,
				Parent = parent.Id
			});

		_context.SaveChanges();

		#region Limbs

		limbs = new Dictionary<string, Limb>(StringComparer.OrdinalIgnoreCase);
		AddLimb("Torso", LimbType.Torso, "body", 1.0, 1.0, jellyfishProto);
		for (var i = 1; i < 11; i++)
			AddLimb($"Tendril{i}", LimbType.Appendage, $"tendril{i}", 0.5, 0.5, jellyfishProto);

		_context.SaveChanges();

		foreach (var limb in limbs.Values)
			foreach (var part in _cachedLimbs[limb.Name])
				_context.LimbsBodypartProto.Add(new LimbBodypartProto { BodypartProto = part, Limb = limb });

		#endregion

		#region Groups

		AddBodypartGroupDescriberShape(jellyfishProto, "tendrils", "All the tendrils of a jellyfish",
			("tendril", 2, 10)
		);

		#endregion

		_context.SaveChanges();

		#endregion Jellyfish

		Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Pinnipeds...");

		#region Pinnipeds

		ResetCachedParts();
		order = 1;

		#region Torso

		AddBodypart(pinnipedProto, "abdomen", "abdomen", "abdomen", BodypartTypeEnum.Wear, null,
			Alignment.Front, Orientation.Low, 80, -1, 100, order++, "Fatty Flesh", SizeCategory.Normal, "Torso",
			true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(pinnipedProto, "rbreast", "right breast", "breast", BodypartTypeEnum.Wear, "abdomen",
			Alignment.FrontRight, Orientation.Low, 80, -1, 100, order++, "Flesh", SizeCategory.Normal, "Torso",
			true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(pinnipedProto, "lbreast", "left breast", "breast", BodypartTypeEnum.Wear, "abdomen",
			Alignment.FrontLeft, Orientation.Low, 80, -1, 100, order++, "Flesh", SizeCategory.Normal, "Torso", true,
			isVital: true, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(pinnipedProto, "urflank", "upper right flank", "flank", BodypartTypeEnum.Wear, "rbreast",
			Alignment.Right, Orientation.Centre, 80, -1, 200, order++, "Flesh", SizeCategory.Normal, "Torso", true,
			isVital: true, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(pinnipedProto, "ulflank", "upper left flank", "flank", BodypartTypeEnum.Wear, "lbreast",
			Alignment.Left, Orientation.Centre, 80, -1, 200, order++, "Flesh", SizeCategory.Normal, "Torso", true,
			isVital: true, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(pinnipedProto, "lrflank", "lower right flank", "flank", BodypartTypeEnum.Wear, "abdomen",
			Alignment.RearRight, Orientation.Centre, 80, -1, 200, order++, "Flesh", SizeCategory.Normal, "Torso",
			true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(pinnipedProto, "llflank", "lower left flank", "flank", BodypartTypeEnum.Wear, "abdomen",
			Alignment.RearLeft, Orientation.Centre, 80, -1, 200, order++, "Flesh", SizeCategory.Normal, "Torso",
			true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(pinnipedProto, "belly", "belly", "belly", BodypartTypeEnum.Wear, "abdomen",
			Alignment.Front, Orientation.Low, 80, -1, 100, order++, "Fatty Flesh", SizeCategory.Normal, "Torso",
			true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(pinnipedProto, "rshoulder", "right shoulder", "shoulder", BodypartTypeEnum.Wear, "rbreast",
			Alignment.FrontRight, Orientation.Centre, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal,
			"Torso", true, isVital: false, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(pinnipedProto, "lshoulder", "left shoulder", "shoulder", BodypartTypeEnum.Wear, "lbreast",
			Alignment.FrontLeft, Orientation.Centre, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal,
			"Torso", true, isVital: false, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(pinnipedProto, "uback", "upper back", "upper back", BodypartTypeEnum.Wear, "abdomen",
			Alignment.Front, Orientation.High, 80, -1, 200, order++, "Bony Flesh", SizeCategory.Normal, "Torso",
			true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(pinnipedProto, "lback", "lower back", "lower back", BodypartTypeEnum.Wear, "abdomen",
			Alignment.Rear, Orientation.High, 80, -1, 200, order++, "Bony Flesh", SizeCategory.Normal, "Torso",
			true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(pinnipedProto, "withers", "withers", "withers", BodypartTypeEnum.Wear, "uback",
			Alignment.Front, Orientation.High, 80, -1, 50, order++, "Flesh", SizeCategory.Normal, "Torso", true,
			isVital: false, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(pinnipedProto, "rrump", "right rump", "rump", BodypartTypeEnum.Wear, "lback",
			Alignment.RearRight, Orientation.Centre, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal,
			"Torso", true, isVital: false, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(pinnipedProto, "lrump", "left rump", "rump", BodypartTypeEnum.Wear, "lback",
			Alignment.RearLeft, Orientation.Centre, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal,
			"Torso", true, isVital: false, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(pinnipedProto, "rloin", "right loin", "loin", BodypartTypeEnum.Wear, "belly",
			Alignment.RearRight, Orientation.Low, 80, -1, 100, order++, "Fatty Flesh", SizeCategory.Normal, "Torso",
			true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(pinnipedProto, "lloin", "left loin", "loin", BodypartTypeEnum.Wear, "belly",
			Alignment.RearLeft, Orientation.Low, 80, -1, 100, order++, "Fatty Flesh", SizeCategory.Normal, "Torso",
			true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);

		#endregion

		#region Head

		AddBodypart(pinnipedProto, "neck", "neck", "neck", BodypartTypeEnum.Wear, "uback", Alignment.Front,
			Orientation.High, 80, 100, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true, isVital: true,
			implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(pinnipedProto, "bneck", "neck back", "neck back", BodypartTypeEnum.Wear, "neck",
			Alignment.Rear, Orientation.Highest, 80, 100, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head",
			true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(pinnipedProto, "throat", "throat", "throat", BodypartTypeEnum.Wear, "neck",
			Alignment.Front, Orientation.Highest, 40, 50, 100, order++, "Fatty Flesh", SizeCategory.Normal, "Head",
			true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(pinnipedProto, "head", "head", "face", BodypartTypeEnum.Wear, "neck", Alignment.Front,
			Orientation.Highest, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true,
			isVital: true, implantSpace: 5, stunMultiplier: 1.0);
		AddBodypart(pinnipedProto, "bhead", "head back", "head back", BodypartTypeEnum.Wear, "bneck",
			Alignment.Rear, Orientation.Highest, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head",
			true, isVital: true, implantSpace: 5, stunMultiplier: 1.0);
		AddBodypart(pinnipedProto, "rjaw", "right jaw", "jaw", BodypartTypeEnum.Wear, "head",
			Alignment.FrontRight, Orientation.Highest, 40, -1, 100, order++, "Bony Flesh", SizeCategory.Small,
			"Head", true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(pinnipedProto, "ljaw", "left jaw", "jaw", BodypartTypeEnum.Wear, "head",
			Alignment.FrontLeft, Orientation.Highest, 40, -1, 100, order++, "Bony Flesh", SizeCategory.Small,
			"Head", true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(pinnipedProto, "rcheek", "right cheek", "cheek", BodypartTypeEnum.Wear, "head",
			Alignment.Right, Orientation.Highest, 40, -1, 100, order++, "Bony Flesh", SizeCategory.Small, "Head",
			true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(pinnipedProto, "lcheek", "left cheek", "cheek", BodypartTypeEnum.Wear, "head",
			Alignment.Left, Orientation.Highest, 40, -1, 100, order++, "Bony Flesh", SizeCategory.Small, "Head",
			true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(pinnipedProto, "reyesocket", "right eye socket", "eye socket", BodypartTypeEnum.Wear,
			"head", Alignment.FrontRight, Orientation.Highest, 80, -1, 100, order++, "Dense Bony Flesh",
			SizeCategory.Small, "Head", true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(pinnipedProto, "leyesocket", "left eye socket", "eye socket", BodypartTypeEnum.Wear,
			"head", Alignment.FrontLeft, Orientation.Highest, 80, -1, 100, order++, "Dense Bony Flesh",
			SizeCategory.Small, "Head", true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(pinnipedProto, "reye", "right eye", "eye", BodypartTypeEnum.Eye, "reyesocket",
			Alignment.FrontRight, Orientation.Highest, 10, 30, 100, order++, "Dense Bony Flesh", SizeCategory.Small,
			"Head", true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(pinnipedProto, "leye", "left eye", "eye", BodypartTypeEnum.Eye, "leyesocket",
			Alignment.FrontLeft, Orientation.Highest, 10, 30, 100, order++, "Dense Bony Flesh", SizeCategory.Small,
			"Head", true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(pinnipedProto, "rear", "right ear", "ear", BodypartTypeEnum.Wear, "head", Alignment.Right,
			Orientation.Highest, 10, 30, 100, order++, "Flesh", SizeCategory.Small, "Head", true, isVital: false,
			implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(pinnipedProto, "lear", "left ear", "ear", BodypartTypeEnum.Wear, "head", Alignment.Left,
			Orientation.Highest, 10, 30, 100, order++, "Flesh", SizeCategory.Small, "Head", true, isVital: false,
			implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(pinnipedProto, "muzzle", "muzzle", "muzzle", BodypartTypeEnum.Wear, "head",
			Alignment.Front, Orientation.Highest, 50, 100, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head",
			true, isVital: true, implantSpace: 5, stunMultiplier: 1.0);
		AddBodypart(pinnipedProto, "mouth", "mouth", "mouth", BodypartTypeEnum.Mouth, "muzzle", Alignment.Front,
			Orientation.Highest, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true,
			isVital: true, implantSpace: 5, stunMultiplier: 1.0);
		AddBodypart(pinnipedProto, "tongue", "tongue", "tongue", BodypartTypeEnum.Tongue, "mouth", Alignment.Front,
			Orientation.Highest, 10, 30, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true,
			isVital: true, implantSpace: 5, stunMultiplier: 1.0);
		AddBodypart(pinnipedProto, "nose", "nose", "nose", BodypartTypeEnum.Wear, "mouth", Alignment.Front,
			Orientation.Highest, 10, 30, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true,
			isVital: true, implantSpace: 5, stunMultiplier: 1.0);

		#endregion

		#region Legs

		AddBodypart(pinnipedProto, "rfrontflipper", "right front flipper", "front flipper", BodypartTypeEnum.Standing,
			"rshoulder", Alignment.FrontRight, Orientation.Low, 80, 100, 100, order++, "Bony Flesh",
			SizeCategory.Normal, "Right Foreleg");
		AddBodypart(pinnipedProto, "lfrontflipper", "left front flipper", "front flipper", BodypartTypeEnum.Standing,
			"lshoulder", Alignment.FrontLeft, Orientation.Low, 80, 100, 100, order++, "Bony Flesh",
			SizeCategory.Normal, "Left Foreleg");
		AddBodypart(pinnipedProto, "rhindflipper", "right hind flipper", "hind flipper", BodypartTypeEnum.Standing,
			"rrump", Alignment.RearRight, Orientation.Low, 80, 100, 100, order++, "Bony Flesh", SizeCategory.Normal,
			"Right Hindleg");
		AddBodypart(pinnipedProto, "lhindflipper", "left hind flipper", "hind flipper", BodypartTypeEnum.Standing,
			"lrump", Alignment.RearLeft, Orientation.Low, 80, 100, 100, order++, "Bony Flesh", SizeCategory.Normal,
			"Left Hindleg");

		#endregion

		#region Tail

		AddBodypart(pinnipedProto, "tail", "tail", "tail", BodypartTypeEnum.Wear, "lback",
			Alignment.Rear, Orientation.Centre, 30, 50, 100, order++, "Flesh", SizeCategory.Normal, "Tail");

		#endregion

		#region Genitals

		AddBodypart(pinnipedProto, "groin", "groin", "groin", BodypartTypeEnum.Wear, "belly", Alignment.Rear,
			Orientation.Low, 30, -1, 100, order++, "Fatty Flesh", SizeCategory.Small, "Genitals");
		AddBodypart(pinnipedProto, "testicles", "testicles", "testicles", BodypartTypeEnum.Wear, "groin",
			Alignment.Rear, Orientation.Low, 10, 30, 100, order++, "Fatty Flesh", SizeCategory.Small, "Genitals",
			true, isCore: false);
		AddBodypart(pinnipedProto, "penis", "penis", "penis", BodypartTypeEnum.Wear, "groin", Alignment.Rear,
			Orientation.Low, 10, 30, 100, order++, "Fatty Flesh", SizeCategory.Small, "Genitals", true,
			isCore: false);
		AddBodypartUsage("penis", "male", pinnipedProto);
		AddBodypartUsage("testicles", "male", pinnipedProto);

		#endregion

		#region Misceallaneous

		AddBodypart(pinnipedProto, "rtusk", "right tusk", "tusk", BodypartTypeEnum.Wear, "rjaw",
			Alignment.FrontRight, Orientation.Highest, 40, 60, 100, order++, "Keratin", SizeCategory.Small, "Head",
			false, isCore: false);
		AddBodypart(pinnipedProto, "ltusk", "left tusk", "tusk", BodypartTypeEnum.Wear, "ljaw",
			Alignment.FrontLeft, Orientation.Highest, 40, 60, 100, order++, "Keratin", SizeCategory.Small, "Head",
			false, isCore: false);

		#endregion

		_context.SaveChanges();

		#region Organs

		AddOrgan(pinnipedProto, "brain", "brain", BodypartTypeEnum.Brain, 2.0, 50, 0.2, 0.2, 0.1,
			stunModifier: 1.0);
		AddOrgan(pinnipedProto, "heart", "heart", BodypartTypeEnum.Heart, 1.0, 50, 0.2, 1.0, 1.0);
		AddOrgan(pinnipedProto, "liver", "liver", BodypartTypeEnum.Liver, 3.0, 50, 0.2, 1.0, 0.05);
		AddOrgan(pinnipedProto, "spleen", "spleen", BodypartTypeEnum.Spleen, 1.0, 50, 0.2, 1.0, 0.05);
		AddOrgan(pinnipedProto, "stomach", "stomach", BodypartTypeEnum.Stomach, 1.0, 50, 0.2, 1.0, 0.05);
		AddOrgan(pinnipedProto, "lintestines", "large intestines", BodypartTypeEnum.Intestines, 0.5, 50, 0.2, 1.0,
			0.05);
		AddOrgan(pinnipedProto, "sintestines", "small intestines", BodypartTypeEnum.Intestines, 2.0, 50, 0.2, 1.0,
			0.05);
		AddOrgan(pinnipedProto, "rkidney", "right kidney", BodypartTypeEnum.Kidney, 0.5, 50, 0.2, 2.0, 0.05,
			painModifier: 3.0);
		AddOrgan(pinnipedProto, "lkidney", "left kidney", BodypartTypeEnum.Kidney, 0.5, 50, 0.2, 2.0, 0.05,
			painModifier: 3.0);
		AddOrgan(pinnipedProto, "rlung", "right lung", BodypartTypeEnum.Lung, 2.0, 50, 0.2, 1.0, 0.05);
		AddOrgan(pinnipedProto, "llung", "left lung", BodypartTypeEnum.Lung, 2.0, 50, 0.2, 1.0, 0.05);
		AddOrgan(pinnipedProto, "trachea", "trachea", BodypartTypeEnum.Trachea, 1.0, 50, 0.2, 1.0, 0.05);
		AddOrgan(pinnipedProto, "esophagus", "esophagus", BodypartTypeEnum.Esophagus, 1.0, 50, 0.2, 1.0, 0.05);
		AddOrgan(pinnipedProto, "uspinalcord", "upper spinal cord", BodypartTypeEnum.Spine, 1.0, 15, 0.2, 1.0, 0.05,
			stunModifier: 1.0, painModifier: 2.0);
		AddOrgan(pinnipedProto, "mspinalcord", "middle spinal cord", BodypartTypeEnum.Spine, 1.0, 15, 0.2, 1.0,
			0.05, stunModifier: 1.0, painModifier: 2.0);
		AddOrgan(pinnipedProto, "lspinalcord", "lower spinal cord", BodypartTypeEnum.Spine, 1.0, 15, 0.2, 1.0, 0.05,
			stunModifier: 1.0, painModifier: 2.0);
		AddOrgan(pinnipedProto, "rinnerear", "right inner ear", BodypartTypeEnum.Ear, 1.0, 15, 0.2, 1.0, 0.05);
		AddOrgan(pinnipedProto, "linnerear", "left inner ear", BodypartTypeEnum.Ear, 1.0, 15, 0.2, 1.0, 0.05);

		AddOrganCoverage("brain", "head", 100, true);
		AddOrganCoverage("brain", "bhead", 100);
		AddOrganCoverage("brain", "rcheek", 85);
		AddOrganCoverage("brain", "lcheek", 85);
		AddOrganCoverage("brain", "reyesocket", 85);
		AddOrganCoverage("brain", "leyesocket", 85);
		AddOrganCoverage("brain", "reye", 85);
		AddOrganCoverage("brain", "leye", 85);
		AddOrganCoverage("brain", "muzzle", 10);
		AddOrganCoverage("brain", "mouth", 30);
		AddOrganCoverage("brain", "lear", 10);
		AddOrganCoverage("brain", "rear", 10);

		AddOrganCoverage("linnerear", "lear", 33, true);
		AddOrganCoverage("rinnerear", "rear", 33, true);
		AddOrganCoverage("esophagus", "throat", 50, true);
		AddOrganCoverage("esophagus", "neck", 20);
		AddOrganCoverage("esophagus", "bneck", 5);
		AddOrganCoverage("trachea", "throat", 50, true);
		AddOrganCoverage("trachea", "neck", 20);
		AddOrganCoverage("trachea", "bneck", 5);

		AddOrganCoverage("rlung", "rbreast", 100, true);
		AddOrganCoverage("llung", "lbreast", 100, true);
		AddOrganCoverage("rlung", "uback", 15);
		AddOrganCoverage("llung", "uback", 15);
		AddOrganCoverage("rlung", "rshoulder", 66);
		AddOrganCoverage("llung", "lshoulder", 66);

		AddOrganCoverage("heart", "lbreast", 33, true);
		AddOrganCoverage("heart", "lshoulder", 20);

		AddOrganCoverage("uspinalcord", "bneck", 10, true);
		AddOrganCoverage("uspinalcord", "neck", 2);
		AddOrganCoverage("uspinalcord", "throat", 5);
		AddOrganCoverage("mspinalcord", "uback", 10, true);
		AddOrganCoverage("mspinalcord", "withers", 2);
		AddOrganCoverage("lspinalcord", "lback", 10, true);

		AddOrganCoverage("liver", "abdomen", 33, true);
		AddOrganCoverage("spleen", "abdomen", 20, true);
		AddOrganCoverage("stomach", "abdomen", 20, true);
		AddOrganCoverage("liver", "uback", 15);
		AddOrganCoverage("spleen", "uback", 10);
		AddOrganCoverage("stomach", "uback", 5);

		AddOrganCoverage("lintestines", "belly", 5, true);
		AddOrganCoverage("sintestines", "belly", 50, true);
		AddOrganCoverage("lintestines", "lback", 5);
		AddOrganCoverage("sintestines", "lback", 33);
		AddOrganCoverage("lintestines", "groin", 5);
		AddOrganCoverage("lintestines", "rloin", 10);
		AddOrganCoverage("lintestines", "lloin", 10);

		AddOrganCoverage("rkidney", "lback", 20, true);
		AddOrganCoverage("lkidney", "lback", 20, true);
		AddOrganCoverage("rkidney", "belly", 5);
		AddOrganCoverage("lkidney", "belly", 5);
		_context.SaveChanges();

		#endregion

		_context.SaveChanges();

		#region Bones

		AddBone(pinnipedProto, "smaxillary", "superior maxillary", BodypartTypeEnum.NonImmobilisingBone, 100,
			"Compact Bone");
		AddBone(pinnipedProto, "rimaxillary", "right inferior maxillary", BodypartTypeEnum.NonImmobilisingBone, 100,
			"Compact Bone");
		AddBone(pinnipedProto, "limaxillary", "left inferior maxillary", BodypartTypeEnum.NonImmobilisingBone, 100,
			"Compact Bone");
		AddBone(pinnipedProto, "fskull", "frontal skull bone", BodypartTypeEnum.NonImmobilisingBone, 200,
			"Compact Bone");
		AddBone(pinnipedProto, "cvertebrae", "cervical vertebrae", BodypartTypeEnum.NonImmobilisingBone, 100,
			"Compact Bone");
		AddBone(pinnipedProto, "dvertebrae", "dorsal vertebrae", BodypartTypeEnum.NonImmobilisingBone, 100,
			"Compact Bone");
		AddBone(pinnipedProto, "lvertebrae", "lumbar vertebrae", BodypartTypeEnum.NonImmobilisingBone, 100,
			"Compact Bone");
		AddBone(pinnipedProto, "svertebrae", "sacral vertebrae", BodypartTypeEnum.NonImmobilisingBone, 100,
			"Compact Bone");
		AddBone(pinnipedProto, "cavertebrae", "caudal vertebrae", BodypartTypeEnum.NonImmobilisingBone, 100,
			"Compact Bone");
		AddBone(pinnipedProto, "rscapula", "right scapula", BodypartTypeEnum.NonImmobilisingBone, 150,
			"Compact Bone");
		AddBone(pinnipedProto, "lscapula", "left scapula", BodypartTypeEnum.NonImmobilisingBone, 150,
			"Compact Bone");
		AddBone(pinnipedProto, "rhumerus", "right humerus", BodypartTypeEnum.Bone, 140, "Compact Bone");
		AddBone(pinnipedProto, "lhumerus", "left humerus", BodypartTypeEnum.Bone, 140, "Compact Bone");
		AddBone(pinnipedProto, "rradius", "right radius", BodypartTypeEnum.Bone, 140, "Compact Bone");
		AddBone(pinnipedProto, "lradius", "left radius", BodypartTypeEnum.Bone, 140, "Compact Bone");
		AddBone(pinnipedProto, "rulna", "right ulna", BodypartTypeEnum.Bone, 120, "Compact Bone");
		AddBone(pinnipedProto, "lulna", "left ulna", BodypartTypeEnum.Bone, 120, "Compact Bone");
		AddBone(pinnipedProto, "rcarpal", "right carpal", BodypartTypeEnum.MinorBone, 40, "Compact Bone");
		AddBone(pinnipedProto, "lcarpal", "left carpal", BodypartTypeEnum.MinorBone, 40, "Compact Bone");
		AddBone(pinnipedProto, "rmetacarpal", "right metacarpal", BodypartTypeEnum.MinorBone, 40, "Compact Bone");
		AddBone(pinnipedProto, "lmetacarpal", "left metacarpal", BodypartTypeEnum.MinorBone, 40, "Compact Bone");
		AddBone(pinnipedProto, "sternum", "sternum", BodypartTypeEnum.NonImmobilisingBone, 200, "Compact Bone");
		AddBone(pinnipedProto, "rrib1", "right first rib", BodypartTypeEnum.NonImmobilisingBone, 100,
			"Compact Bone");
		AddBone(pinnipedProto, "lrib1", "left first rib", BodypartTypeEnum.NonImmobilisingBone, 100,
			"Compact Bone");
		AddBone(pinnipedProto, "rrib2", "right second rib", BodypartTypeEnum.NonImmobilisingBone, 100,
			"Compact Bone");
		AddBone(pinnipedProto, "lrib2", "left second rib", BodypartTypeEnum.NonImmobilisingBone, 100,
			"Compact Bone");
		AddBone(pinnipedProto, "rrib3", "right third rib", BodypartTypeEnum.NonImmobilisingBone, 100,
			"Compact Bone");
		AddBone(pinnipedProto, "lrib3", "left third rib", BodypartTypeEnum.NonImmobilisingBone, 100,
			"Compact Bone");
		AddBone(pinnipedProto, "rrib4", "right fourth rib", BodypartTypeEnum.NonImmobilisingBone, 100,
			"Compact Bone");
		AddBone(pinnipedProto, "lrib4", "left fourth rib", BodypartTypeEnum.NonImmobilisingBone, 100,
			"Compact Bone");
		AddBone(pinnipedProto, "rrib5", "right fifth rib", BodypartTypeEnum.NonImmobilisingBone, 100,
			"Compact Bone");
		AddBone(pinnipedProto, "lrib5", "left fifth rib", BodypartTypeEnum.NonImmobilisingBone, 100,
			"Compact Bone");
		AddBone(pinnipedProto, "rrib6", "right sixth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
			"Compact Bone");
		AddBone(pinnipedProto, "lrib6", "left sixth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
			"Compact Bone");
		AddBone(pinnipedProto, "rrib7", "right seventh rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
			"Compact Bone");
		AddBone(pinnipedProto, "lrib7", "left seventh rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
			"Compact Bone");
		AddBone(pinnipedProto, "rrib8", "right eighth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
			"Compact Bone");
		AddBone(pinnipedProto, "lrib8", "left eighth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
			"Compact Bone");
		AddBone(pinnipedProto, "rrib9", "right ninth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
			"Compact Bone");
		AddBone(pinnipedProto, "lrib9", "left ninth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
			"Compact Bone");
		AddBone(pinnipedProto, "rrib10", "right tenth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
			"Compact Bone");
		AddBone(pinnipedProto, "lrib10", "left tenth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
			"Compact Bone");
		AddBone(pinnipedProto, "rrib11", "right eleventh rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
			"Compact Bone");
		AddBone(pinnipedProto, "lrib11", "left eleventh rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
			"Compact Bone");
		AddBone(pinnipedProto, "rrib12", "right twelth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
			"Compact Bone");
		AddBone(pinnipedProto, "lrib12", "left twelth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
			"Compact Bone");
		AddBone(pinnipedProto, "rilium", "right ilium", BodypartTypeEnum.NonImmobilisingBone, 150, "Compact Bone");
		AddBone(pinnipedProto, "lilium", "left ilium", BodypartTypeEnum.NonImmobilisingBone, 150, "Compact Bone");
		AddBone(pinnipedProto, "sacrum", "sacrum", BodypartTypeEnum.NonImmobilisingBone, 150, "Compact Bone");
		AddBone(pinnipedProto, "rpubis", "right pubis", BodypartTypeEnum.NonImmobilisingBone, 150, "Compact Bone");
		AddBone(pinnipedProto, "lpubis", "left pubis", BodypartTypeEnum.NonImmobilisingBone, 150, "Compact Bone");
		AddBone(pinnipedProto, "rischium", "right ischium", BodypartTypeEnum.NonImmobilisingBone, 150,
			"Compact Bone");
		AddBone(pinnipedProto, "lischium", "left ischium", BodypartTypeEnum.NonImmobilisingBone, 150,
			"Compact Bone");
		AddBone(pinnipedProto, "rfemur", "right femur", BodypartTypeEnum.Bone, 200, "Compact Bone");
		AddBone(pinnipedProto, "lfemur", "left femur", BodypartTypeEnum.Bone, 200, "Compact Bone");
		AddBone(pinnipedProto, "rpatella", "right patella", BodypartTypeEnum.Bone, 90, "Compact Bone");
		AddBone(pinnipedProto, "lpatella", "left patella", BodypartTypeEnum.Bone, 90, "Compact Bone");
		AddBone(pinnipedProto, "rtibia", "right tibia", BodypartTypeEnum.Bone, 150, "Compact Bone");
		AddBone(pinnipedProto, "ltibia", "left tibia", BodypartTypeEnum.Bone, 150, "Compact Bone");
		AddBone(pinnipedProto, "rfibula", "right fibula", BodypartTypeEnum.Bone, 150, "Compact Bone");
		AddBone(pinnipedProto, "lfibula", "left fibula", BodypartTypeEnum.Bone, 150, "Compact Bone");
		AddBone(pinnipedProto, "rcalcaneus", "right calcaneus", BodypartTypeEnum.Bone, 80, "Compact Bone");
		AddBone(pinnipedProto, "lcalcaneus", "left calcaneus", BodypartTypeEnum.Bone, 80, "Compact Bone");
		AddBone(pinnipedProto, "rtalus", "right talus", BodypartTypeEnum.Bone, 80, "Compact Bone");
		AddBone(pinnipedProto, "ltalus", "left talus", BodypartTypeEnum.Bone, 80, "Compact Bone");
		AddBone(pinnipedProto, "rtarsus", "right tarsus", BodypartTypeEnum.Bone, 80, "Compact Bone");
		AddBone(pinnipedProto, "ltarsus", "left tarsus", BodypartTypeEnum.Bone, 80, "Compact Bone");
		AddBone(pinnipedProto, "rmetatarsus", "right metatarsus", BodypartTypeEnum.Bone, 80, "Compact Bone");
		AddBone(pinnipedProto, "lmetatarsus", "left metatarsus", BodypartTypeEnum.Bone, 80, "Compact Bone");

		// TORSO BONES
		AddBoneInternal("sternum", "abdomen", 50);
		AddBoneInternal("rrib1", "rshoulder", 10);
		AddBoneInternal("lrib1", "lshoulder", 10);
		AddBoneInternal("rrib2", "rbreast", 5);
		AddBoneInternal("lrib2", "lbreast", 5);
		AddBoneInternal("rrib3", "rbreast", 5);
		AddBoneInternal("lrib3", "lbreast", 5);
		AddBoneInternal("rrib4", "rbreast", 5);
		AddBoneInternal("lrib4", "lbreast", 5);
		AddBoneInternal("rrib5", "rbreast", 5);
		AddBoneInternal("lrib5", "lbreast", 5);
		AddBoneInternal("rrib6", "rbreast", 5);
		AddBoneInternal("lrib6", "lbreast", 5);
		AddBoneInternal("rrib7", "rbreast", 5);
		AddBoneInternal("lrib7", "lbreast", 5);
		AddBoneInternal("rrib8", "rbreast", 5);
		AddBoneInternal("lrib8", "lbreast", 5);
		AddBoneInternal("rrib9", "rbreast", 5);
		AddBoneInternal("lrib9", "lbreast", 5);
		AddBoneInternal("rrib10", "rbreast", 5);
		AddBoneInternal("lrib10", "lbreast", 5);
		AddBoneInternal("rrib11", "rbreast", 5);
		AddBoneInternal("lrib11", "lbreast", 5);
		AddBoneInternal("rrib12", "rbreast", 5);
		AddBoneInternal("lrib12", "lbreast", 5);
		AddBoneInternal("cvertebrae", "bneck", 35);
		AddBoneInternal("dvertebrae", "uback", 20);
		AddBoneInternal("dvertebrae", "withers", 20, false);
		AddBoneInternal("lvertebrae", "lback", 20);
		AddBoneInternal("svertebrae", "tail", 90);
		AddBoneInternal("cavertebrae", "tail", 80);
		AddBoneInternal("sacrum", "lback", 15);
		AddBoneInternal("rilium", "rrump", 50);
		AddBoneInternal("lilium", "lrump", 50);
		AddBoneInternal("rilium", "lback", 4, false);
		AddBoneInternal("lilium", "lback", 4, false);
		AddBoneInternal("rpubis", "groin", 20);
		AddBoneInternal("lpubis", "groin", 20);
		AddBoneInternal("rischium", "rrump", 20);
		AddBoneInternal("lischium", "lrump", 20);

		// HEAD BONES
		AddBoneInternal("smaxillary", "muzzle", 100);
		AddBoneInternal("smaxillary", "mouth", 40, false);
		AddBoneInternal("smaxillary", "rcheek", 20, false);
		AddBoneInternal("smaxillary", "lcheek", 20, false);
		AddBoneInternal("rimaxillary", "rjaw", 100);
		AddBoneInternal("limaxillary", "ljaw", 100);
		AddBoneInternal("fskull", "head", 100);
		AddBoneInternal("fskull", "bhead", 40, false);

		// ARM BONES
		AddBoneInternal("rscapula", "rshoulder", 100);
		AddBoneInternal("lscapula", "lshoulder", 100);
		AddBoneInternal("rhumerus", "rfrontflipper", 50);
		AddBoneInternal("lhumerus", "lfrontflipper", 50);
		AddBoneInternal("rradius", "rfrontflipper", 33);
		AddBoneInternal("lradius", "lfrontflipper", 33);
		AddBoneInternal("rulna", "rfrontflipper", 33);
		AddBoneInternal("lulna", "lfrontflipper", 33);
		AddBoneInternal("rcarpal", "rfrontflipper", 50);
		AddBoneInternal("lcarpal", "lfrontflipper", 50);
		AddBoneInternal("rmetacarpal", "rfrontflipper", 50);
		AddBoneInternal("lmetacarpal", "lfrontflipper", 50);

		// LEG BONES
		AddBoneInternal("rfemur", "rhindflipper", 50);
		AddBoneInternal("lfemur", "lhindflipper", 50);
		AddBoneInternal("rpatella", "rhindflipper", 100);
		AddBoneInternal("lpatella", "lhindflipper", 100);
		AddBoneInternal("rtibia", "rhindflipper", 100);
		AddBoneInternal("ltibia", "lhindflipper", 100);
		AddBoneInternal("rfibula", "rhindflipper", 33);
		AddBoneInternal("lfibula", "lhindflipper", 33);
		AddBoneInternal("rcalcaneus", "rhindflipper", 20);
		AddBoneInternal("lcalcaneus", "lhindflipper", 20);
		AddBoneInternal("rtalus", "rhindflipper", 20);
		AddBoneInternal("ltalus", "lhindflipper", 20);
		AddBoneInternal("rtarsus", "rhindflipper", 50);
		AddBoneInternal("ltarsus", "lhindflipper", 50);
		AddBoneInternal("rmetatarsus", "rhindflipper", 50);
		AddBoneInternal("lmetatarsus", "lhindflipper", 50);
		_context.SaveChanges();

		AddBoneCover("fskull", "brain", 100);
		AddBoneCover("smaxillary", "brain", 90);

		AddBoneCover("cvertebrae", "uspinalcord", 100);
		AddBoneCover("dvertebrae", "mspinalcord", 100);
		AddBoneCover("lvertebrae", "lspinalcord", 100);

		AddBoneCover("sternum", "heart", 80);
		AddBoneCover("sternum", "rlung", 17.5);
		AddBoneCover("sternum", "llung", 17.5);
		AddBoneCover("lrib1", "heart", 5);
		AddBoneCover("lrib2", "heart", 10);
		AddBoneCover("lrib3", "heart", 15);
		AddBoneCover("lrib4", "heart", 15);
		AddBoneCover("lrib5", "heart", 15);
		AddBoneCover("lrib6", "heart", 15);
		AddBoneCover("lrib1", "llung", 10);
		AddBoneCover("lrib2", "llung", 15);
		AddBoneCover("lrib3", "llung", 20);
		AddBoneCover("lrib4", "llung", 20);
		AddBoneCover("lrib5", "llung", 20);
		AddBoneCover("lrib6", "llung", 20);
		AddBoneCover("lrib7", "llung", 20);
		AddBoneCover("rrib1", "rlung", 10);
		AddBoneCover("rrib2", "rlung", 15);
		AddBoneCover("rrib3", "rlung", 20);
		AddBoneCover("rrib4", "rlung", 20);
		AddBoneCover("rrib5", "rlung", 20);
		AddBoneCover("rrib6", "rlung", 20);
		AddBoneCover("rrib7", "rlung", 20);

		AddBoneCover("rrib6", "liver", 30);
		AddBoneCover("rrib7", "liver", 45);
		AddBoneCover("lrib6", "liver", 30);
		AddBoneCover("lrib7", "liver", 45);

		AddBoneCover("lrib8", "liver", 80);
		AddBoneCover("lrib8", "spleen", 25);
		AddBoneCover("rrib8", "liver", 80);
		AddBoneCover("rrib8", "spleen", 25);

		AddBoneCover("lrib9", "liver", 60);
		AddBoneCover("lrib9", "spleen", 20);
		AddBoneCover("rrib9", "liver", 60);
		AddBoneCover("rrib9", "spleen", 20);

		AddBoneCover("lrib10", "liver", 15);
		AddBoneCover("lrib10", "lkidney", 20);
		AddBoneCover("rrib10", "liver", 15);
		AddBoneCover("rrib10", "rkidney", 20);

		AddBoneCover("rscapula", "rlung", 70);
		AddBoneCover("lscapula", "llung", 70);

		AddBoneCover("rilium", "sintestines", 20);
		AddBoneCover("lilium", "sintestines", 20);
		AddBoneCover("rilium", "lintestines", 40);
		AddBoneCover("lilium", "lintestines", 40);

		AddBoneCover("rischium", "lintestines", 40);
		AddBoneCover("lischium", "lintestines", 40);
		_context.SaveChanges();

		#endregion

		_context.SaveChanges();

		foreach (var (child, parent) in _cachedBodypartUpstreams)
			_context.BodypartProtoBodypartProtoUpstream.Add(new BodypartProtoBodypartProtoUpstream
			{
				Child = child.Id,
				Parent = parent.Id
			});

		_context.SaveChanges();

		#region Limbs

		limbs = new Dictionary<string, Limb>(StringComparer.OrdinalIgnoreCase);
		AddLimb("Torso", LimbType.Torso, "abdomen", 1.0, 1.0, pinnipedProto);
		AddLimb("Head", LimbType.Head, "neck", 1.0, 1.0, pinnipedProto);
		AddLimb("Genitals", LimbType.Genitals, "groin", 0.5, 0.5, pinnipedProto);
		AddLimb("Right Foreleg", LimbType.Leg, "rfrontflipper", 0.5, 0.5, pinnipedProto);
		AddLimb("Left Foreleg", LimbType.Leg, "lfrontflipper", 0.5, 0.5, pinnipedProto);
		AddLimb("Right Hindleg", LimbType.Leg, "rhindflipper", 0.5, 0.5, pinnipedProto);
		AddLimb("Left Hindleg", LimbType.Leg, "lhindflipper", 0.5, 0.5, pinnipedProto);
		AddLimb("Tail", LimbType.Appendage, "tail", 0.5, 0.5, pinnipedProto);
		_context.SaveChanges();

		foreach (var limb in limbs.Values)
		{
			foreach (var part in _cachedLimbs[limb.Name])
				_context.LimbsBodypartProto.Add(new LimbBodypartProto { BodypartProto = part, Limb = limb });

			switch (limb.Name)
			{
				case "Torso":
					_context.LimbsSpinalParts.Add(new LimbsSpinalPart
					{ Limb = limb, BodypartProto = _cachedOrgans["uspinalcord"] });
					break;
				case "Genitals":
				case "Right Foreleg":
				case "Left Foreleg":
					_context.LimbsSpinalParts.Add(new LimbsSpinalPart
					{ Limb = limb, BodypartProto = _cachedOrgans["mspinalcord"] });
					break;
				case "Leg Hindleg":
				case "Right Hindleg":
				case "Tail":
					_context.LimbsSpinalParts.Add(new LimbsSpinalPart
					{ Limb = limb, BodypartProto = _cachedOrgans["lspinalcord"] });
					break;
			}
		}

		_context.SaveChanges();

		#endregion

		#region Groups

		AddBodypartGroupDescriberShape(pinnipedProto, "body", "The whole torso of a pinniped",
			("abdomen", 1, 1),
			("belly", 1, 1),
			("withers", 1, 1),
			("breast", 1, 2),
			("flank", 1, 2),
			("loin", 1, 2),
			("shoulder", 1, 2),
			("upper back", 1, 1),
			("lower back", 1, 1),
			("rump", 1, 2),
			("neck", 0, 1),
			("neck back", 0, 1),
			("throat", 0, 1)
		);
		AddBodypartGroupDescriberShape(pinnipedProto, "flippers", "Four flippers of a pinniped",
			("front flipper", 1, 2),
			("hind flipper", 1, 2)
		);
		AddBodypartGroupDescriberShape(pinnipedProto, "front flippers", "Both front flippers of a pinniped",
			("front flipper", 2, 2)
		);
		AddBodypartGroupDescriberShape(pinnipedProto, "hind flippers", "Both hind flippers of a pinniped",
			("hind flipper", 2, 2)
		);

		AddBodypartGroupDescriberShape(pinnipedProto, "head", "A pinniped head",
			("face", 1, 1),
			("head back", 0, 1),
			("eye socket", 0, 2),
			("eye", 0, 2),
			("ear", 0, 2),
			("jaw", 0, 2),
			("muzzle", 0, 1),
			("nose", 0, 1),
			("mouth", 0, 1),
			("tongue", 0, 1),
			("cheek", 0, 2),
			("throat", 0, 1),
			("withers", 0, 1),
			("neck", 0, 1),
			("neck back", 0, 1),
			("tusk", 0, 2)
		);

		AddBodypartGroupDescriberShape(pinnipedProto, "back", "A pinniped back",
			("upper back", 1, 1),
			("lower back", 1, 1),
			("flank", 0, 4),
			("rump", 0, 2),
			("withers", 0, 1),
			("neck back", 0, 1)
		);

		AddBodypartGroupDescriberShape(pinnipedProto, "eyes", "A pair of pinniped eyes",
			("eye socket", 2, 2),
			("eye", 0, 2)
		);

		AddBodypartGroupDescriberShape(pinnipedProto, "ears", "A pair of pinniped ears",
			("ear", 2, 2)
		);

		AddBodypartGroupDescriberShape(pinnipedProto, "tusks", "A pair of pinniped tusks",
			("tusk", 2, 2)
		);

		AddBodypartGroupDescriberShape(pinnipedProto, "shoulders", "A group of pinniped shoulders",
			("shoulder", 2, 4)
		);

		_context.SaveChanges();

		#endregion

		#endregion

		Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Cetaceans...");

		#region Cetaceans

		ResetCachedParts();
		order = 1;

		#region Torso

		AddBodypart(cetaceanProto, "abdomen", "abdomen", "abdomen", BodypartTypeEnum.Wear, null, Alignment.Front,
			Orientation.Low, 80, -1, 100, order++, "Fatty Flesh", SizeCategory.Normal, "Torso", true, isVital: true,
			implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(cetaceanProto, "rbreast", "right breast", "breast", BodypartTypeEnum.Wear, "abdomen",
			Alignment.FrontRight, Orientation.Low, 80, -1, 100, order++, "Flesh", SizeCategory.Normal, "Torso",
			true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(cetaceanProto, "lbreast", "left breast", "breast", BodypartTypeEnum.Wear, "abdomen",
			Alignment.FrontLeft, Orientation.Low, 80, -1, 100, order++, "Flesh", SizeCategory.Normal, "Torso", true,
			isVital: true, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(cetaceanProto, "urflank", "upper right flank", "flank", BodypartTypeEnum.Wear, "rbreast",
			Alignment.Right, Orientation.Centre, 80, -1, 200, order++, "Flesh", SizeCategory.Normal, "Torso", true,
			isVital: true, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(cetaceanProto, "ulflank", "upper left flank", "flank", BodypartTypeEnum.Wear, "lbreast",
			Alignment.Left, Orientation.Centre, 80, -1, 200, order++, "Flesh", SizeCategory.Normal, "Torso", true,
			isVital: true, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(cetaceanProto, "lrflank", "lower right flank", "flank", BodypartTypeEnum.Wear, "abdomen",
			Alignment.RearRight, Orientation.Centre, 80, -1, 200, order++, "Flesh", SizeCategory.Normal, "Torso",
			true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(cetaceanProto, "llflank", "lower left flank", "flank", BodypartTypeEnum.Wear, "abdomen",
			Alignment.RearLeft, Orientation.Centre, 80, -1, 200, order++, "Flesh", SizeCategory.Normal, "Torso",
			true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(cetaceanProto, "belly", "belly", "belly", BodypartTypeEnum.Wear, "abdomen", Alignment.Front,
			Orientation.Low, 80, -1, 100, order++, "Fatty Flesh", SizeCategory.Normal, "Torso", true, isVital: true,
			implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(cetaceanProto, "uback", "upper back", "upper back", BodypartTypeEnum.Wear, "abdomen",
			Alignment.Front, Orientation.High, 80, -1, 200, order++, "Bony Flesh", SizeCategory.Normal, "Torso",
			true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(cetaceanProto, "lback", "lower back", "lower back", BodypartTypeEnum.Wear, "abdomen",
			Alignment.Rear, Orientation.High, 80, -1, 200, order++, "Bony Flesh", SizeCategory.Normal, "Torso",
			true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(cetaceanProto, "loin", "loin", "loin", BodypartTypeEnum.Wear, "belly", Alignment.Rear,
			Orientation.Low, 80, -1, 100, order++, "Fatty Flesh", SizeCategory.Normal, "Torso", true, isVital: true,
			implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(cetaceanProto, "dorsalfin", "dorsal fin", "fin", BodypartTypeEnum.Fin, "uback", Alignment.Rear,
			Orientation.Highest, 20, 35, 100, order++, "Flesh", SizeCategory.Normal, "Torso");
		AddBodypart(cetaceanProto, "rpectoralfin", "right pectoral fin", "fin", BodypartTypeEnum.Fin, "urflank",
			Alignment.Right, Orientation.Centre, 20, 35, 100, order++, "Flesh", SizeCategory.Normal, "Torso");
		AddBodypart(cetaceanProto, "lpectoralfin", "left pectoral fin", "fin", BodypartTypeEnum.Fin, "ulflank",
			Alignment.Left, Orientation.Centre, 20, 35, 100, order++, "Flesh", SizeCategory.Normal, "Torso");

		#endregion

		#region Head

		AddBodypart(cetaceanProto, "neck", "neck", "neck", BodypartTypeEnum.Wear, "uback", Alignment.Front,
			Orientation.High, 80, 100, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true, isVital: true,
			implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(cetaceanProto, "head", "head", "face", BodypartTypeEnum.Wear, "neck", Alignment.Front,
			Orientation.Highest, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true,
			isVital: true, implantSpace: 5, stunMultiplier: 1.0);
		AddBodypart(cetaceanProto, "blowhole", "blowhole", "blowhole", BodypartTypeEnum.Blowhole, "head",
			Alignment.Front,
			Orientation.High, 80, 100, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true, isVital: true,
			implantSpace: 5, stunMultiplier: 1.0);
		AddBodypart(cetaceanProto, "reyesocket", "right eye socket", "eye socket", BodypartTypeEnum.Wear, "head",
			Alignment.FrontRight, Orientation.Highest, 80, -1, 100, order++, "Dense Bony Flesh", SizeCategory.Small,
			"Head", true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(cetaceanProto, "leyesocket", "left eye socket", "eye socket", BodypartTypeEnum.Wear, "head",
			Alignment.FrontLeft, Orientation.Highest, 80, -1, 100, order++, "Dense Bony Flesh", SizeCategory.Small,
			"Head", true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(cetaceanProto, "reye", "right eye", "eye", BodypartTypeEnum.Eye, "reyesocket", Alignment.FrontRight,
			Orientation.Highest, 10, 30, 100, order++, "Dense Bony Flesh", SizeCategory.Small, "Head", true,
			isVital: true, implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(cetaceanProto, "leye", "left eye", "eye", BodypartTypeEnum.Eye, "leyesocket", Alignment.FrontLeft,
			Orientation.Highest, 10, 30, 100, order++, "Dense Bony Flesh", SizeCategory.Small, "Head", true,
			isVital: true, implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(cetaceanProto, "mouth", "mouth", "mouth", BodypartTypeEnum.Mouth, "head", Alignment.Front,
			Orientation.Highest, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true,
			isVital: true, implantSpace: 5, stunMultiplier: 1.0);

		#endregion

		#region Tail

		AddBodypart(cetaceanProto, "stock", "tail stock", "tail", BodypartTypeEnum.Wear, "lback", Alignment.Rear,
			Orientation.Centre, 30, 50, 100, order++, "Flesh", SizeCategory.Normal, "Tail");
		AddBodypart(cetaceanProto, "fluke", "fluke", "tail", BodypartTypeEnum.Fin, "stock", Alignment.Rear,
			Orientation.Centre, 20, 35, 100, order++, "Flesh", SizeCategory.Normal, "Tail");

		#endregion

		_context.SaveChanges();

		#region Organs

		AddOrgan(cetaceanProto, "brain", "brain", BodypartTypeEnum.Brain, 2.0, 50, 0.2, 0.2, 0.1, stunModifier: 1.0);
		AddOrgan(cetaceanProto, "heart", "heart", BodypartTypeEnum.Heart, 1.0, 50, 0.2, 1.0, 1.0);
		AddOrgan(cetaceanProto, "liver", "liver", BodypartTypeEnum.Liver, 3.0, 50, 0.2, 1.0, 0.05);
		AddOrgan(cetaceanProto, "spleen", "spleen", BodypartTypeEnum.Spleen, 1.0, 50, 0.2, 1.0, 0.05);
		AddOrgan(cetaceanProto, "stomach", "stomach", BodypartTypeEnum.Stomach, 1.0, 50, 0.2, 1.0, 0.05);
		AddOrgan(cetaceanProto, "lintestines", "large intestines", BodypartTypeEnum.Intestines, 0.5, 50, 0.2, 1.0,
			0.05);
		AddOrgan(cetaceanProto, "sintestines", "small intestines", BodypartTypeEnum.Intestines, 2.0, 50, 0.2, 1.0,
			0.05);
		AddOrgan(cetaceanProto, "rkidney", "right kidney", BodypartTypeEnum.Kidney, 0.5, 50, 0.2, 2.0, 0.05,
			painModifier: 3.0);
		AddOrgan(cetaceanProto, "lkidney", "left kidney", BodypartTypeEnum.Kidney, 0.5, 50, 0.2, 2.0, 0.05,
			painModifier: 3.0);
		AddOrgan(cetaceanProto, "esophagus", "esophagus", BodypartTypeEnum.Esophagus, 1.0, 50, 0.2, 1.0, 0.05);
		AddOrgan(cetaceanProto, "uspinalcord", "upper spinal cord", BodypartTypeEnum.Spine, 1.0, 15, 0.2, 1.0, 0.05,
			stunModifier: 1.0, painModifier: 2.0);
		AddOrgan(cetaceanProto, "mspinalcord", "middle spinal cord", BodypartTypeEnum.Spine, 1.0, 15, 0.2, 1.0, 0.05,
			stunModifier: 1.0, painModifier: 2.0);
		AddOrgan(cetaceanProto, "lspinalcord", "lower spinal cord", BodypartTypeEnum.Spine, 1.0, 15, 0.2, 1.0, 0.05,
			stunModifier: 1.0, painModifier: 2.0);
		AddOrgan(cetaceanProto, "rinnerear", "right inner ear", BodypartTypeEnum.Ear, 1.0, 15, 0.2, 1.0, 0.05);
		AddOrgan(cetaceanProto, "linnerear", "left inner ear", BodypartTypeEnum.Ear, 1.0, 15, 0.2, 1.0, 0.05);

		AddOrganCoverage("brain", "head", 100, true);
		AddOrganCoverage("brain", "reyesocket", 85);
		AddOrganCoverage("brain", "leyesocket", 85);
		AddOrganCoverage("brain", "reye", 85);
		AddOrganCoverage("brain", "leye", 85);

		AddOrganCoverage("linnerear", "head", 10, true);
		AddOrganCoverage("rinnerear", "head", 10, true);
		AddOrganCoverage("esophagus", "neck", 20, true);

		AddOrganCoverage("heart", "lbreast", 33, true);

		AddOrganCoverage("uspinalcord", "neck", 2, true);
		AddOrganCoverage("mspinalcord", "uback", 10, true);
		AddOrganCoverage("lspinalcord", "lback", 10, true);

		AddOrganCoverage("liver", "abdomen", 33, true);
		AddOrganCoverage("spleen", "abdomen", 20, true);
		AddOrganCoverage("stomach", "abdomen", 20, true);
		AddOrganCoverage("liver", "uback", 15);
		AddOrganCoverage("spleen", "uback", 10);
		AddOrganCoverage("stomach", "uback", 5);

		AddOrganCoverage("lintestines", "belly", 5, true);
		AddOrganCoverage("sintestines", "belly", 50, true);
		AddOrganCoverage("lintestines", "lback", 5);
		AddOrganCoverage("sintestines", "lback", 33);
		AddOrganCoverage("lintestines", "loin", 5);

		AddOrganCoverage("rkidney", "lback", 20, true);
		AddOrganCoverage("lkidney", "lback", 20, true);
		AddOrganCoverage("rkidney", "belly", 5);
		AddOrganCoverage("lkidney", "belly", 5);
		_context.SaveChanges();

		#endregion

		_context.SaveChanges();

		foreach (var (child, parent) in _cachedBodypartUpstreams)
			_context.BodypartProtoBodypartProtoUpstream.Add(new BodypartProtoBodypartProtoUpstream
			{
				Child = child.Id,
				Parent = parent.Id
			});

		_context.SaveChanges();

		#region Limbs

		limbs = new Dictionary<string, Limb>(StringComparer.OrdinalIgnoreCase);
		AddLimb("Torso", LimbType.Torso, "abdomen", 1.0, 1.0, cetaceanProto);
		AddLimb("Head", LimbType.Head, "neck", 1.0, 1.0, cetaceanProto);
		AddLimb("Tail", LimbType.Appendage, "stock", 0.5, 0.5, cetaceanProto);
		_context.SaveChanges();

		foreach (var limb in limbs.Values)
		{
			foreach (var part in _cachedLimbs[limb.Name])
				_context.LimbsBodypartProto.Add(new LimbBodypartProto { BodypartProto = part, Limb = limb });

			switch (limb.Name)
			{
				case "Torso":
					_context.LimbsSpinalParts.Add(new LimbsSpinalPart
					{ Limb = limb, BodypartProto = _cachedOrgans["uspinalcord"] });
					_context.LimbsSpinalParts.Add(new LimbsSpinalPart
					{ Limb = limb, BodypartProto = _cachedOrgans["mspinalcord"] });
					break;
				case "Tail":
					_context.LimbsSpinalParts.Add(new LimbsSpinalPart
					{ Limb = limb, BodypartProto = _cachedOrgans["lspinalcord"] });
					break;
			}
		}

		_context.SaveChanges();

		#endregion

		#region Groups

		AddBodypartGroupDescriberShape(cetaceanProto, "body", "The whole torso of a cetacean",
			("abdomen", 0, 1),
			("breast", 0, 2),
			("flank", 0, 4),
			("belly", 0, 1),
			("loin", 0, 1),
			("upper back", 1, 1),
			("lower back", 1, 1),
			("neck", 0, 1),
			("blowhole", 0, 2),
			("tail", 0, 2)
		);

		AddBodypartGroupDescriberShape(cetaceanProto, "fins", "All the fins of a cetacean",
			("fin", 2, 3)
		);

		AddBodypartGroupDescriberDirect(cetaceanProto, "pectoral fins", "The pectoral fins of a cetacean",
			("rpectoralfin", true),
			("lpectoralfin", true)
		);

		AddBodypartGroupDescriberShape(cetaceanProto, "eyes", "The eyes of a cetacean",
			("eye socket", 0, 2),
			("eye", 0, 2)
		);

		AddBodypartGroupDescriberShape(cetaceanProto, "head", "The whole head of a cetacean",
			("face", 0, 1),
			("eye socket", 0, 2),
			("eye", 0, 2),
			("mouth", 0, 1),
			("neck", 0, 1),
			("blowhole", 0, 2)
		);

		AddBodypartGroupDescriberDirect(cetaceanProto, "tail", "The whole of a cetacean's tail",
			("fluke", true),
			("stock", true)
		);

		#endregion

		_context.SaveChanges();

		#endregion

		Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Races...");

		#region Races

		SeedAnimalRaces(GetAquaticRaceTemplates(),
			("Piscine", fishProto),
			("Decapod", crabProto),
			("Malacostracan", malacostracanProto),
			("Cephalopod", octopusProto),
			("Jellyfish", jellyfishProto),
			("Pinniped", pinnipedProto),
			("Cetacean", cetaceanProto));

		#endregion
	}

	private void SeedQuadruped(BodyProto quadrupedBody, BodyProto ungulateBody, BodyProto toedQuadruped)
	{
		ResetCachedParts();
		var order = 1;
		Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Bodyparts...");

		#region Torso

		AddBodypart(quadrupedBody, "abdomen", "abdomen", "abdomen", BodypartTypeEnum.Wear, null,
			Alignment.Front, Orientation.Low, 80, -1, 100, order++, "Fatty Flesh", SizeCategory.Normal, "Torso",
			true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(quadrupedBody, "rbreast", "right breast", "breast", BodypartTypeEnum.Wear, "abdomen",
			Alignment.FrontRight, Orientation.Low, 80, -1, 100, order++, "Flesh", SizeCategory.Normal, "Torso",
			true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(quadrupedBody, "lbreast", "left breast", "breast", BodypartTypeEnum.Wear, "abdomen",
			Alignment.FrontLeft, Orientation.Low, 80, -1, 100, order++, "Flesh", SizeCategory.Normal, "Torso", true,
			isVital: true, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(quadrupedBody, "urflank", "upper right flank", "flank", BodypartTypeEnum.Wear, "rbreast",
			Alignment.Right, Orientation.Centre, 80, -1, 200, order++, "Flesh", SizeCategory.Normal, "Torso", true,
			isVital: true, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(quadrupedBody, "ulflank", "upper left flank", "flank", BodypartTypeEnum.Wear, "lbreast",
			Alignment.Left, Orientation.Centre, 80, -1, 200, order++, "Flesh", SizeCategory.Normal, "Torso", true,
			isVital: true, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(quadrupedBody, "lrflank", "lower right flank", "flank", BodypartTypeEnum.Wear, "abdomen",
			Alignment.RearRight, Orientation.Centre, 80, -1, 200, order++, "Flesh", SizeCategory.Normal, "Torso",
			true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(quadrupedBody, "llflank", "lower left flank", "flank", BodypartTypeEnum.Wear, "abdomen",
			Alignment.RearLeft, Orientation.Centre, 80, -1, 200, order++, "Flesh", SizeCategory.Normal, "Torso",
			true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(quadrupedBody, "belly", "belly", "belly", BodypartTypeEnum.Wear, "abdomen",
			Alignment.Front, Orientation.Low, 80, -1, 100, order++, "Fatty Flesh", SizeCategory.Normal, "Torso",
			true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(quadrupedBody, "rshoulder", "right shoulder", "shoulder", BodypartTypeEnum.Wear, "rbreast",
			Alignment.FrontRight, Orientation.Centre, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal,
			"Torso", true, isVital: false, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(quadrupedBody, "lshoulder", "left shoulder", "shoulder", BodypartTypeEnum.Wear, "lbreast",
			Alignment.FrontLeft, Orientation.Centre, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal,
			"Torso", true, isVital: false, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(quadrupedBody, "uback", "upper back", "upper back", BodypartTypeEnum.Wear, "abdomen",
			Alignment.Front, Orientation.High, 80, -1, 200, order++, "Bony Flesh", SizeCategory.Normal, "Torso",
			true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(quadrupedBody, "lback", "lower back", "lower back", BodypartTypeEnum.Wear, "abdomen",
			Alignment.Rear, Orientation.High, 80, -1, 200, order++, "Bony Flesh", SizeCategory.Normal, "Torso",
			true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(quadrupedBody, "withers", "withers", "withers", BodypartTypeEnum.Wear, "uback",
			Alignment.Front, Orientation.High, 80, -1, 50, order++, "Flesh", SizeCategory.Normal, "Torso", true,
			isVital: false, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(quadrupedBody, "rrump", "right rump", "rump", BodypartTypeEnum.Wear, "lback",
			Alignment.RearRight, Orientation.Centre, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal,
			"Torso", true, isVital: false, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(quadrupedBody, "lrump", "left rump", "rump", BodypartTypeEnum.Wear, "lback",
			Alignment.RearLeft, Orientation.Centre, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal,
			"Torso", true, isVital: false, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(quadrupedBody, "rloin", "right loin", "loin", BodypartTypeEnum.Wear, "belly",
			Alignment.RearRight, Orientation.Low, 80, -1, 100, order++, "Fatty Flesh", SizeCategory.Normal, "Torso",
			true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(quadrupedBody, "lloin", "left loin", "loin", BodypartTypeEnum.Wear, "belly",
			Alignment.RearLeft, Orientation.Low, 80, -1, 100, order++, "Fatty Flesh", SizeCategory.Normal, "Torso",
			true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);

		#endregion

		#region Head

		AddBodypart(quadrupedBody, "neck", "neck", "neck", BodypartTypeEnum.Wear, "uback", Alignment.Front,
			Orientation.High, 80, 100, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true, isVital: true,
			implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(quadrupedBody, "bneck", "neck back", "neck back", BodypartTypeEnum.Wear, "neck",
			Alignment.Rear, Orientation.Highest, 80, 100, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head",
			true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(quadrupedBody, "throat", "throat", "throat", BodypartTypeEnum.Wear, "neck",
			Alignment.Front, Orientation.Highest, 40, 50, 100, order++, "Fatty Flesh", SizeCategory.Normal, "Head",
			true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(quadrupedBody, "head", "head", "face", BodypartTypeEnum.Wear, "neck", Alignment.Front,
			Orientation.Highest, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true,
			isVital: true, implantSpace: 5, stunMultiplier: 1.0);
		AddBodypart(quadrupedBody, "bhead", "head back", "head back", BodypartTypeEnum.Wear, "bneck",
			Alignment.Rear, Orientation.Highest, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head",
			true, isVital: true, implantSpace: 5, stunMultiplier: 1.0);
		AddBodypart(quadrupedBody, "rjaw", "right jaw", "jaw", BodypartTypeEnum.Wear, "head",
			Alignment.FrontRight, Orientation.Highest, 40, -1, 100, order++, "Bony Flesh", SizeCategory.Small,
			"Head", true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(quadrupedBody, "ljaw", "left jaw", "jaw", BodypartTypeEnum.Wear, "head",
			Alignment.FrontLeft, Orientation.Highest, 40, -1, 100, order++, "Bony Flesh", SizeCategory.Small,
			"Head", true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(quadrupedBody, "rcheek", "right cheek", "cheek", BodypartTypeEnum.Wear, "head",
			Alignment.Right, Orientation.Highest, 40, -1, 100, order++, "Bony Flesh", SizeCategory.Small, "Head",
			true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(quadrupedBody, "lcheek", "left cheek", "cheek", BodypartTypeEnum.Wear, "head",
			Alignment.Left, Orientation.Highest, 40, -1, 100, order++, "Bony Flesh", SizeCategory.Small, "Head",
			true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(quadrupedBody, "reyesocket", "right eye socket", "eye socket", BodypartTypeEnum.Wear,
			"head", Alignment.FrontRight, Orientation.Highest, 80, -1, 100, order++, "Dense Bony Flesh",
			SizeCategory.Small, "Head", true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(quadrupedBody, "leyesocket", "left eye socket", "eye socket", BodypartTypeEnum.Wear,
			"head", Alignment.FrontLeft, Orientation.Highest, 80, -1, 100, order++, "Dense Bony Flesh",
			SizeCategory.Small, "Head", true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(quadrupedBody, "reye", "right eye", "eye", BodypartTypeEnum.Eye, "reyesocket",
			Alignment.FrontRight, Orientation.Highest, 10, 30, 100, order++, "Dense Bony Flesh", SizeCategory.Small,
			"Head", true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(quadrupedBody, "leye", "left eye", "eye", BodypartTypeEnum.Eye, "leyesocket",
			Alignment.FrontLeft, Orientation.Highest, 10, 30, 100, order++, "Dense Bony Flesh", SizeCategory.Small,
			"Head", true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(quadrupedBody, "rear", "right ear", "ear", BodypartTypeEnum.Wear, "head", Alignment.Right,
			Orientation.Highest, 10, 30, 100, order++, "Flesh", SizeCategory.Small, "Head", true, isVital: false,
			implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(quadrupedBody, "lear", "left ear", "ear", BodypartTypeEnum.Wear, "head", Alignment.Left,
			Orientation.Highest, 10, 30, 100, order++, "Flesh", SizeCategory.Small, "Head", true, isVital: false,
			implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(quadrupedBody, "muzzle", "muzzle", "muzzle", BodypartTypeEnum.Wear, "head",
			Alignment.Front, Orientation.Highest, 50, 100, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head",
			true, isVital: true, implantSpace: 5, stunMultiplier: 1.0);
		AddBodypart(quadrupedBody, "mouth", "mouth", "mouth", BodypartTypeEnum.Mouth, "muzzle", Alignment.Front,
			Orientation.Highest, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true,
			isVital: true, implantSpace: 5, stunMultiplier: 1.0);
		AddBodypart(quadrupedBody, "tongue", "tongue", "tongue", BodypartTypeEnum.Tongue, "mouth", Alignment.Front,
			Orientation.Highest, 10, 30, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true,
			isVital: true, implantSpace: 5, stunMultiplier: 1.0);
		AddBodypart(quadrupedBody, "nose", "nose", "nose", BodypartTypeEnum.Wear, "mouth", Alignment.Front,
			Orientation.Highest, 10, 30, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true,
			isVital: true, implantSpace: 5, stunMultiplier: 1.0);

		#endregion

		#region Legs

		AddBodypart(quadrupedBody, "ruforeleg", "right upper foreleg", "upper foreleg", BodypartTypeEnum.Wear,
			"rshoulder", Alignment.FrontRight, Orientation.Low, 80, 100, 100, order++, "Bony Flesh",
			SizeCategory.Normal, "Right Foreleg");
		AddBodypart(quadrupedBody, "luforeleg", "left upper foreleg", "upper foreleg", BodypartTypeEnum.Wear,
			"lshoulder", Alignment.FrontLeft, Orientation.Low, 80, 100, 100, order++, "Bony Flesh",
			SizeCategory.Normal, "Left Foreleg");
		AddBodypart(quadrupedBody, "ruhindleg", "right upper hindleg", "upper hindleg", BodypartTypeEnum.Wear,
			"rrump", Alignment.RearRight, Orientation.Low, 80, 100, 100, order++, "Bony Flesh", SizeCategory.Normal,
			"Right Hindleg");
		AddBodypart(quadrupedBody, "luhindleg", "left upper hindleg", "upper hindleg", BodypartTypeEnum.Wear,
			"lrump", Alignment.RearLeft, Orientation.Low, 80, 100, 100, order++, "Bony Flesh", SizeCategory.Normal,
			"Left Hindleg");
		AddBodypart(quadrupedBody, "rfknee", "right front knee", "knee", BodypartTypeEnum.Wear, "ruforeleg",
			Alignment.FrontRight, Orientation.Low, 60, 80, 30, order++, "Dense Bony Flesh", SizeCategory.Normal,
			"Right Foreleg");
		AddBodypart(quadrupedBody, "lfknee", "left front knee", "knee", BodypartTypeEnum.Wear, "luforeleg",
			Alignment.FrontLeft, Orientation.Low, 60, 80, 30, order++, "Dense Bony Flesh", SizeCategory.Normal,
			"Left Foreleg");
		AddBodypart(quadrupedBody, "rrknee", "right rear knee", "knee", BodypartTypeEnum.Wear, "ruhindleg",
			Alignment.RearRight, Orientation.Low, 60, 80, 30, order++, "Dense Bony Flesh", SizeCategory.Normal,
			"Right Hindleg");
		AddBodypart(quadrupedBody, "rlknee", "left rear knee", "knee", BodypartTypeEnum.Wear, "luhindleg",
			Alignment.RearLeft, Orientation.Low, 60, 80, 30, order++, "Dense Bony Flesh", SizeCategory.Normal,
			"Left Hindleg");
		AddBodypart(quadrupedBody, "rlforeleg", "right lower foreleg", "lower foreleg", BodypartTypeEnum.Wear,
			"rfknee", Alignment.FrontRight, Orientation.Lowest, 40, 50, 100, order++, "Dense Bony Flesh",
			SizeCategory.Normal, "Right Foreleg");
		AddBodypart(quadrupedBody, "llforeleg", "left lower foreleg", "lower foreleg", BodypartTypeEnum.Wear,
			"lfknee", Alignment.FrontLeft, Orientation.Lowest, 40, 50, 100, order++, "Dense Bony Flesh",
			SizeCategory.Normal, "Left Foreleg");
		AddBodypart(quadrupedBody, "rlhindleg", "right lower hindleg", "lower hindleg", BodypartTypeEnum.Wear,
			"rrknee", Alignment.RearRight, Orientation.Lowest, 40, 50, 100, order++, "Dense Bony Flesh",
			SizeCategory.Normal, "Right Hindleg");
		AddBodypart(quadrupedBody, "llhindleg", "left lower hindleg", "lower hindleg", BodypartTypeEnum.Wear,
			"rlknee", Alignment.RearLeft, Orientation.Lowest, 40, 50, 100, order++, "Dense Bony Flesh",
			SizeCategory.Normal, "Left Hindleg");
		AddBodypart(quadrupedBody, "rfhock", "right front hock", "front hock", BodypartTypeEnum.Wear,
			"rlforeleg", Alignment.FrontRight, Orientation.Lowest, 40, 50, 50, order++, "Dense Bony Flesh",
			SizeCategory.Normal, "Right Foreleg");
		AddBodypart(quadrupedBody, "lfhock", "left front hock", "front hock", BodypartTypeEnum.Wear,
			"llforeleg", Alignment.FrontLeft, Orientation.Lowest, 40, 50, 50, order++, "Dense Bony Flesh",
			SizeCategory.Normal, "Left Foreleg");
		AddBodypart(quadrupedBody, "rrhock", "right rear hock", "rear hock", BodypartTypeEnum.Wear,
			"rlhindleg", Alignment.RearRight, Orientation.Lowest, 40, 50, 50, order++, "Dense Bony Flesh",
			SizeCategory.Normal, "Right Hindleg");
		AddBodypart(quadrupedBody, "lrhock", "left rear hock", "rear hock", BodypartTypeEnum.Wear, "llhindleg",
			Alignment.RearLeft, Orientation.Lowest, 40, 50, 50, order++, "Dense Bony Flesh", SizeCategory.Normal,
			"Left Hindleg");

		#endregion

		#region Tail

		AddBodypart(quadrupedBody, "utail", "upper tail", "tail", BodypartTypeEnum.Wear, "lback",
			Alignment.Rear, Orientation.Centre, 30, 50, 100, order++, "Flesh", SizeCategory.Normal, "Tail");
		AddBodypart(quadrupedBody, "mtail", "middle tail", "tail", BodypartTypeEnum.Wear, "utail",
			Alignment.Rear, Orientation.Centre, 30, 50, 100, order++, "Flesh", SizeCategory.Normal, "Tail");
		AddBodypart(quadrupedBody, "ltail", "lower tail", "tail", BodypartTypeEnum.Wear, "mtail",
			Alignment.Rear, Orientation.Centre, 30, 50, 100, order++, "Flesh", SizeCategory.Normal, "Tail");

		#endregion

		#region Genitals

		AddBodypart(quadrupedBody, "groin", "groin", "groin", BodypartTypeEnum.Wear, "belly", Alignment.Rear,
			Orientation.Low, 30, -1, 100, order++, "Fatty Flesh", SizeCategory.Small, "Genitals");
		AddBodypart(quadrupedBody, "testicles", "testicles", "testicles", BodypartTypeEnum.Wear, "groin",
			Alignment.Rear, Orientation.Low, 10, 30, 100, order++, "Fatty Flesh", SizeCategory.Small, "Genitals",
			true, isCore: false);
		AddBodypart(quadrupedBody, "penis", "penis", "penis", BodypartTypeEnum.Wear, "groin", Alignment.Rear,
			Orientation.Low, 10, 30, 100, order++, "Fatty Flesh", SizeCategory.Small, "Genitals", true,
			isCore: false);
		AddBodypartUsage("penis", "male", quadrupedBody);
		AddBodypartUsage("testicles", "male", quadrupedBody);

		#endregion

		#region Wings

		AddBodypart(quadrupedBody, "rwingbase", "right wing base", "wing base", BodypartTypeEnum.Wear, "uback",
			Alignment.FrontRight, Orientation.High, 40, -1, 100, order++, "Flesh", SizeCategory.Normal,
			"Right Wing", true, isCore: false);
		AddBodypart(quadrupedBody, "lwingbase", "left wing base", "wing base", BodypartTypeEnum.Wear, "uback",
			Alignment.FrontLeft, Orientation.High, 40, -1, 100, order++, "Flesh", SizeCategory.Normal, "Left Wing",
			true, isCore: false);
		AddBodypart(quadrupedBody, "rwing", "right wing", "wing", BodypartTypeEnum.Wing, "rwingbase",
			Alignment.FrontRight, Orientation.High, 40, 50, 100, order++, "Flesh", SizeCategory.Normal,
			"Right Wing", true, isCore: false);
		AddBodypart(quadrupedBody, "lwing", "left wing", "wing", BodypartTypeEnum.Wing, "lwingbase",
			Alignment.FrontLeft, Orientation.High, 40, 50, 100, order++, "Flesh", SizeCategory.Normal, "Left Wing",
			true, isCore: false);

		#endregion

		#region Misceallaneous

		AddBodypart(quadrupedBody, "udder", "udder", "udder", BodypartTypeEnum.Wear, "belly", Alignment.Rear,
			Orientation.Low, 40, 60, 100, order++, "Flesh", SizeCategory.Normal, "Torso", false, isCore: false);
		AddBodypart(quadrupedBody, "rhorn", "right horn", "horn", BodypartTypeEnum.Wear, "head",
			Alignment.FrontRight, Orientation.Highest, 40, 60, 100, order++, "Keratin", SizeCategory.Small, "Head",
			false, isCore: false);
		AddBodypart(quadrupedBody, "lhorn", "left horn", "horn", BodypartTypeEnum.Wear, "head",
			Alignment.FrontLeft, Orientation.Highest, 40, 60, 100, order++, "Keratin", SizeCategory.Small, "Head",
			false, isCore: false);
		AddBodypart(quadrupedBody, "horn", "horn", "horn", BodypartTypeEnum.Wear, "head", Alignment.Front,
			Orientation.Highest, 40, 60, 100, order++, "Keratin", SizeCategory.Small, "Head", false, isCore: false);
		AddBodypart(quadrupedBody, "rantler", "right antler", "antler", BodypartTypeEnum.Wear, "head",
			Alignment.FrontRight, Orientation.Highest, 40, 60, 100, order++, "Antler", SizeCategory.Small, "Head",
			false, isCore: false);
		AddBodypart(quadrupedBody, "lantler", "left antler", "antler", BodypartTypeEnum.Wear, "head",
			Alignment.FrontLeft, Orientation.Highest, 40, 60, 100, order++, "Antler", SizeCategory.Small, "Head",
			false, isCore: false);
		AddBodypart(quadrupedBody, "rtusk", "right tusk", "tusk", BodypartTypeEnum.Wear, "rjaw",
			Alignment.FrontRight, Orientation.Highest, 40, 60, 100, order++, "Keratin", SizeCategory.Small, "Head",
			false, isCore: false);
		AddBodypart(quadrupedBody, "ltusk", "left tusk", "tusk", BodypartTypeEnum.Wear, "ljaw",
			Alignment.FrontLeft, Orientation.Highest, 40, 60, 100, order++, "Keratin", SizeCategory.Small, "Head",
			false, isCore: false);

		#endregion

		#region Ungulates

		AddBodypart(ungulateBody, "rfhoof", "right front hoof", "hoof", BodypartTypeEnum.Wear, "rfhock",
			Alignment.FrontRight, Orientation.Lowest, 40, 50, 100, order++, "Dense Bony Flesh", SizeCategory.Small,
			"Right Foreleg");
		AddBodypart(ungulateBody, "lfhoof", "left front hoof", "hoof", BodypartTypeEnum.Wear, "lfhock",
			Alignment.FrontLeft, Orientation.Lowest, 40, 50, 100, order++, "Dense Bony Flesh", SizeCategory.Small,
			"Left Foreleg");
		AddBodypart(ungulateBody, "rrhoof", "right rear hoof", "hoof", BodypartTypeEnum.Wear, "rrhock",
			Alignment.RearRight, Orientation.Lowest, 40, 50, 100, order++, "Dense Bony Flesh", SizeCategory.Small,
			"Right Hindleg");
		AddBodypart(ungulateBody, "lrhoof", "left rear hoof", "hoof", BodypartTypeEnum.Wear, "lrhock",
			Alignment.RearLeft, Orientation.Lowest, 40, 50, 100, order++, "Dense Bony Flesh", SizeCategory.Small,
			"Left Hindleg");
		AddBodypart(ungulateBody, "rffrog", "right front frog", "hoof", BodypartTypeEnum.Standing, "rfhoof",
			Alignment.FrontRight, Orientation.Lowest, 20, 50, 10, order++, "Dense Bony Flesh", SizeCategory.Small,
			"Right Foreleg");
		AddBodypart(ungulateBody, "lffrog", "left front frog", "hoof", BodypartTypeEnum.Standing, "lfhoof",
			Alignment.FrontLeft, Orientation.Lowest, 20, 50, 10, order++, "Dense Bony Flesh", SizeCategory.Small,
			"Left Foreleg");
		AddBodypart(ungulateBody, "rrfrog", "right rear frog", "hoof", BodypartTypeEnum.Standing, "rrhoof",
			Alignment.RearRight, Orientation.Lowest, 20, 50, 10, order++, "Dense Bony Flesh", SizeCategory.Small,
			"Right Hindleg");
		AddBodypart(ungulateBody, "lrfrog", "left rear frog", "hoof", BodypartTypeEnum.Standing, "lrhoof",
			Alignment.RearLeft, Orientation.Lowest, 20, 50, 10, order++, "Dense Bony Flesh", SizeCategory.Small,
			"Left Hindleg");

		#endregion

		#region Toed

		AddBodypart(toedQuadruped, "rfpaw", "right front paw", "paw", BodypartTypeEnum.Standing, "rfhock",
			Alignment.FrontRight, Orientation.Lowest, 40, 50, 50, order++, "Bony Flesh", SizeCategory.Normal,
			"Right Foreleg");
		AddBodypart(toedQuadruped, "lfpaw", "left front paw", "paw", BodypartTypeEnum.Standing, "lfhock",
			Alignment.FrontLeft, Orientation.Lowest, 40, 50, 50, order++, "Bony Flesh", SizeCategory.Normal,
			"Left Foreleg");
		AddBodypart(toedQuadruped, "rrpaw", "right rear paw", "paw", BodypartTypeEnum.Standing, "rrhock",
			Alignment.RearRight, Orientation.Lowest, 40, 50, 50, order++, "Bony Flesh", SizeCategory.Normal,
			"Right Hindleg");
		AddBodypart(toedQuadruped, "lrpaw", "left rear paw", "paw", BodypartTypeEnum.Standing, "lrhock",
			Alignment.RearLeft, Orientation.Lowest, 40, 50, 50, order++, "Bony Flesh", SizeCategory.Normal,
			"Left Hindleg");
		AddBodypart(toedQuadruped, "rfclaw", "right front claws", "claw", BodypartTypeEnum.Wear, "rfpaw",
			Alignment.FrontRight, Orientation.Lowest, 40, 50, 50, order++, "Dense Bony Flesh", SizeCategory.Normal,
			"Right Foreleg", false, isVital: false);
		AddBodypart(toedQuadruped, "lfclaw", "left front claws", "claw", BodypartTypeEnum.Wear, "lfpaw",
			Alignment.FrontLeft, Orientation.Lowest, 40, 50, 50, order++, "Dense Bony Flesh", SizeCategory.Normal,
			"Left Foreleg", false, isVital: false);
		AddBodypart(toedQuadruped, "rrclaw", "right rear claws", "claw", BodypartTypeEnum.Wear, "rrpaw",
			Alignment.RearRight, Orientation.Lowest, 40, 50, 50, order++, "Dense Bony Flesh", SizeCategory.Normal,
			"Right Hindleg", false, isVital: false);
		AddBodypart(toedQuadruped, "lrclaw", "left rear claws", "claw", BodypartTypeEnum.Wear, "lrpaw",
			Alignment.RearLeft, Orientation.Lowest, 40, 50, 50, order++, "Dense Bony Flesh", SizeCategory.Normal,
			"Left Hindleg", false, isVital: false);
		AddBodypart(toedQuadruped, "rrdewclaw", "right dewclaw", "dewclaw", BodypartTypeEnum.Wear, "rrpaw",
			Alignment.RearRight, Orientation.Lowest, 10, 50, 5, order++, "Dense Bony Flesh", SizeCategory.Normal,
			"Right Hindleg", false, isVital: false);
		AddBodypart(toedQuadruped, "lrdewclaw", "left dewclaw", "dewclaw", BodypartTypeEnum.Wear, "lrpaw",
			Alignment.RearLeft, Orientation.Lowest, 10, 50, 5, order++, "Dense Bony Flesh", SizeCategory.Normal,
			"Left Hindleg", false, isVital: false);

		#endregion

		_context.SaveChanges();

		Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Organs...");

		#region Organs

		AddOrgan(quadrupedBody, "brain", "brain", BodypartTypeEnum.Brain, 2.0, 50, 0.2, 0.2, 0.1,
			stunModifier: 1.0);
		AddOrgan(quadrupedBody, "heart", "heart", BodypartTypeEnum.Heart, 1.0, 50, 0.2, 1.0, 1.0);
		AddOrgan(quadrupedBody, "liver", "liver", BodypartTypeEnum.Liver, 3.0, 50, 0.2, 1.0, 0.05);
		AddOrgan(quadrupedBody, "spleen", "spleen", BodypartTypeEnum.Spleen, 1.0, 50, 0.2, 1.0, 0.05);
		AddOrgan(quadrupedBody, "stomach", "stomach", BodypartTypeEnum.Stomach, 1.0, 50, 0.2, 1.0, 0.05);
		AddOrgan(quadrupedBody, "lintestines", "large intestines", BodypartTypeEnum.Intestines, 0.5, 50, 0.2, 1.0,
			0.05);
		AddOrgan(quadrupedBody, "sintestines", "small intestines", BodypartTypeEnum.Intestines, 2.0, 50, 0.2, 1.0,
			0.05);
		AddOrgan(quadrupedBody, "rkidney", "right kidney", BodypartTypeEnum.Kidney, 0.5, 50, 0.2, 2.0, 0.05,
			painModifier: 3.0);
		AddOrgan(quadrupedBody, "lkidney", "left kidney", BodypartTypeEnum.Kidney, 0.5, 50, 0.2, 2.0, 0.05,
			painModifier: 3.0);
		AddOrgan(quadrupedBody, "rlung", "right lung", BodypartTypeEnum.Lung, 2.0, 50, 0.2, 1.0, 0.05);
		AddOrgan(quadrupedBody, "llung", "left lung", BodypartTypeEnum.Lung, 2.0, 50, 0.2, 1.0, 0.05);
		AddOrgan(quadrupedBody, "trachea", "trachea", BodypartTypeEnum.Trachea, 1.0, 50, 0.2, 1.0, 0.05);
		AddOrgan(quadrupedBody, "esophagus", "esophagus", BodypartTypeEnum.Esophagus, 1.0, 50, 0.2, 1.0, 0.05);
		AddOrgan(quadrupedBody, "uspinalcord", "upper spinal cord", BodypartTypeEnum.Spine, 1.0, 15, 0.2, 1.0, 0.05,
			stunModifier: 1.0, painModifier: 2.0);
		AddOrgan(quadrupedBody, "mspinalcord", "middle spinal cord", BodypartTypeEnum.Spine, 1.0, 15, 0.2, 1.0,
			0.05, stunModifier: 1.0, painModifier: 2.0);
		AddOrgan(quadrupedBody, "lspinalcord", "lower spinal cord", BodypartTypeEnum.Spine, 1.0, 15, 0.2, 1.0, 0.05,
			stunModifier: 1.0, painModifier: 2.0);
		AddOrgan(quadrupedBody, "rinnerear", "right inner ear", BodypartTypeEnum.Ear, 1.0, 15, 0.2, 1.0, 0.05);
		AddOrgan(quadrupedBody, "linnerear", "left inner ear", BodypartTypeEnum.Ear, 1.0, 15, 0.2, 1.0, 0.05);

		AddOrganCoverage("brain", "head", 100, true);
		AddOrganCoverage("brain", "bhead", 100);
		AddOrganCoverage("brain", "rcheek", 85);
		AddOrganCoverage("brain", "lcheek", 85);
		AddOrganCoverage("brain", "reyesocket", 85);
		AddOrganCoverage("brain", "leyesocket", 85);
		AddOrganCoverage("brain", "reye", 85);
		AddOrganCoverage("brain", "leye", 85);
		AddOrganCoverage("brain", "muzzle", 10);
		AddOrganCoverage("brain", "mouth", 30);
		AddOrganCoverage("brain", "lear", 10);
		AddOrganCoverage("brain", "rear", 10);

		AddOrganCoverage("linnerear", "lear", 33, true);
		AddOrganCoverage("rinnerear", "rear", 33, true);
		AddOrganCoverage("esophagus", "throat", 50, true);
		AddOrganCoverage("esophagus", "neck", 20);
		AddOrganCoverage("esophagus", "bneck", 5);
		AddOrganCoverage("trachea", "throat", 50, true);
		AddOrganCoverage("trachea", "neck", 20);
		AddOrganCoverage("trachea", "bneck", 5);

		AddOrganCoverage("rlung", "rbreast", 100, true);
		AddOrganCoverage("llung", "lbreast", 100, true);
		AddOrganCoverage("rlung", "uback", 15);
		AddOrganCoverage("llung", "uback", 15);
		AddOrganCoverage("rlung", "rshoulder", 66);
		AddOrganCoverage("llung", "lshoulder", 66);

		AddOrganCoverage("heart", "lbreast", 33, true);
		AddOrganCoverage("heart", "lshoulder", 20);

		AddOrganCoverage("uspinalcord", "bneck", 10, true);
		AddOrganCoverage("uspinalcord", "neck", 2);
		AddOrganCoverage("uspinalcord", "throat", 5);
		AddOrganCoverage("mspinalcord", "uback", 10, true);
		AddOrganCoverage("mspinalcord", "withers", 2);
		AddOrganCoverage("lspinalcord", "lback", 10, true);

		AddOrganCoverage("liver", "abdomen", 33, true);
		AddOrganCoverage("spleen", "abdomen", 20, true);
		AddOrganCoverage("stomach", "abdomen", 20, true);
		AddOrganCoverage("liver", "uback", 15);
		AddOrganCoverage("spleen", "uback", 10);
		AddOrganCoverage("stomach", "uback", 5);

		AddOrganCoverage("lintestines", "belly", 5, true);
		AddOrganCoverage("sintestines", "belly", 50, true);
		AddOrganCoverage("lintestines", "lback", 5);
		AddOrganCoverage("sintestines", "lback", 33);
		AddOrganCoverage("lintestines", "groin", 5);
		AddOrganCoverage("lintestines", "rloin", 10);
		AddOrganCoverage("lintestines", "lloin", 10);

		AddOrganCoverage("rkidney", "lback", 20, true);
		AddOrganCoverage("lkidney", "lback", 20, true);
		AddOrganCoverage("rkidney", "belly", 5);
		AddOrganCoverage("lkidney", "belly", 5);
		_context.SaveChanges();

		#endregion

		_context.SaveChanges();

		Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Bones...");

		#region Bones

		AddBone(quadrupedBody, "smaxillary", "superior maxillary", BodypartTypeEnum.NonImmobilisingBone, 100,
			"Compact Bone");
		AddBone(quadrupedBody, "rimaxillary", "right inferior maxillary", BodypartTypeEnum.NonImmobilisingBone, 100,
			"Compact Bone");
		AddBone(quadrupedBody, "limaxillary", "left inferior maxillary", BodypartTypeEnum.NonImmobilisingBone, 100,
			"Compact Bone");
		AddBone(quadrupedBody, "fskull", "frontal skull bone", BodypartTypeEnum.NonImmobilisingBone, 200,
			"Compact Bone");
		AddBone(quadrupedBody, "cvertebrae", "cervical vertebrae", BodypartTypeEnum.NonImmobilisingBone, 100,
			"Compact Bone");
		AddBone(quadrupedBody, "dvertebrae", "dorsal vertebrae", BodypartTypeEnum.NonImmobilisingBone, 100,
			"Compact Bone");
		AddBone(quadrupedBody, "lvertebrae", "lumbar vertebrae", BodypartTypeEnum.NonImmobilisingBone, 100,
			"Compact Bone");
		AddBone(quadrupedBody, "svertebrae", "sacral vertebrae", BodypartTypeEnum.NonImmobilisingBone, 100,
			"Compact Bone");
		AddBone(quadrupedBody, "cavertebrae", "caudal vertebrae", BodypartTypeEnum.NonImmobilisingBone, 100,
			"Compact Bone");
		AddBone(quadrupedBody, "rscapula", "right scapula", BodypartTypeEnum.NonImmobilisingBone, 150,
			"Compact Bone");
		AddBone(quadrupedBody, "lscapula", "left scapula", BodypartTypeEnum.NonImmobilisingBone, 150,
			"Compact Bone");
		AddBone(quadrupedBody, "rhumerus", "right humerus", BodypartTypeEnum.Bone, 140, "Compact Bone");
		AddBone(quadrupedBody, "lhumerus", "left humerus", BodypartTypeEnum.Bone, 140, "Compact Bone");
		AddBone(quadrupedBody, "rradius", "right radius", BodypartTypeEnum.Bone, 140, "Compact Bone");
		AddBone(quadrupedBody, "lradius", "left radius", BodypartTypeEnum.Bone, 140, "Compact Bone");
		AddBone(quadrupedBody, "rulna", "right ulna", BodypartTypeEnum.Bone, 120, "Compact Bone");
		AddBone(quadrupedBody, "lulna", "left ulna", BodypartTypeEnum.Bone, 120, "Compact Bone");
		AddBone(quadrupedBody, "rcarpal", "right carpal", BodypartTypeEnum.MinorBone, 40, "Compact Bone");
		AddBone(quadrupedBody, "lcarpal", "left carpal", BodypartTypeEnum.MinorBone, 40, "Compact Bone");
		AddBone(quadrupedBody, "rmetacarpal", "right metacarpal", BodypartTypeEnum.MinorBone, 40, "Compact Bone");
		AddBone(quadrupedBody, "lmetacarpal", "left metacarpal", BodypartTypeEnum.MinorBone, 40, "Compact Bone");
		AddBone(quadrupedBody, "sternum", "sternum", BodypartTypeEnum.NonImmobilisingBone, 200, "Compact Bone");
		AddBone(quadrupedBody, "rrib1", "right first rib", BodypartTypeEnum.NonImmobilisingBone, 100,
			"Compact Bone");
		AddBone(quadrupedBody, "lrib1", "left first rib", BodypartTypeEnum.NonImmobilisingBone, 100,
			"Compact Bone");
		AddBone(quadrupedBody, "rrib2", "right second rib", BodypartTypeEnum.NonImmobilisingBone, 100,
			"Compact Bone");
		AddBone(quadrupedBody, "lrib2", "left second rib", BodypartTypeEnum.NonImmobilisingBone, 100,
			"Compact Bone");
		AddBone(quadrupedBody, "rrib3", "right third rib", BodypartTypeEnum.NonImmobilisingBone, 100,
			"Compact Bone");
		AddBone(quadrupedBody, "lrib3", "left third rib", BodypartTypeEnum.NonImmobilisingBone, 100,
			"Compact Bone");
		AddBone(quadrupedBody, "rrib4", "right fourth rib", BodypartTypeEnum.NonImmobilisingBone, 100,
			"Compact Bone");
		AddBone(quadrupedBody, "lrib4", "left fourth rib", BodypartTypeEnum.NonImmobilisingBone, 100,
			"Compact Bone");
		AddBone(quadrupedBody, "rrib5", "right fifth rib", BodypartTypeEnum.NonImmobilisingBone, 100,
			"Compact Bone");
		AddBone(quadrupedBody, "lrib5", "left fifth rib", BodypartTypeEnum.NonImmobilisingBone, 100,
			"Compact Bone");
		AddBone(quadrupedBody, "rrib6", "right sixth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
			"Compact Bone");
		AddBone(quadrupedBody, "lrib6", "left sixth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
			"Compact Bone");
		AddBone(quadrupedBody, "rrib7", "right seventh rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
			"Compact Bone");
		AddBone(quadrupedBody, "lrib7", "left seventh rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
			"Compact Bone");
		AddBone(quadrupedBody, "rrib8", "right eighth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
			"Compact Bone");
		AddBone(quadrupedBody, "lrib8", "left eighth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
			"Compact Bone");
		AddBone(quadrupedBody, "rrib9", "right ninth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
			"Compact Bone");
		AddBone(quadrupedBody, "lrib9", "left ninth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
			"Compact Bone");
		AddBone(quadrupedBody, "rrib10", "right tenth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
			"Compact Bone");
		AddBone(quadrupedBody, "lrib10", "left tenth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
			"Compact Bone");
		AddBone(quadrupedBody, "rrib11", "right eleventh rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
			"Compact Bone");
		AddBone(quadrupedBody, "lrib11", "left eleventh rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
			"Compact Bone");
		AddBone(quadrupedBody, "rrib12", "right twelth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
			"Compact Bone");
		AddBone(quadrupedBody, "lrib12", "left twelth rib", BodypartTypeEnum.MinorNonImobilisingBone, 100,
			"Compact Bone");
		AddBone(quadrupedBody, "rilium", "right ilium", BodypartTypeEnum.NonImmobilisingBone, 150, "Compact Bone");
		AddBone(quadrupedBody, "lilium", "left ilium", BodypartTypeEnum.NonImmobilisingBone, 150, "Compact Bone");
		AddBone(quadrupedBody, "sacrum", "sacrum", BodypartTypeEnum.NonImmobilisingBone, 150, "Compact Bone");
		AddBone(quadrupedBody, "rpubis", "right pubis", BodypartTypeEnum.NonImmobilisingBone, 150, "Compact Bone");
		AddBone(quadrupedBody, "lpubis", "left pubis", BodypartTypeEnum.NonImmobilisingBone, 150, "Compact Bone");
		AddBone(quadrupedBody, "rischium", "right ischium", BodypartTypeEnum.NonImmobilisingBone, 150,
			"Compact Bone");
		AddBone(quadrupedBody, "lischium", "left ischium", BodypartTypeEnum.NonImmobilisingBone, 150,
			"Compact Bone");
		AddBone(quadrupedBody, "rfemur", "right femur", BodypartTypeEnum.Bone, 200, "Compact Bone");
		AddBone(quadrupedBody, "lfemur", "left femur", BodypartTypeEnum.Bone, 200, "Compact Bone");
		AddBone(quadrupedBody, "rpatella", "right patella", BodypartTypeEnum.Bone, 90, "Compact Bone");
		AddBone(quadrupedBody, "lpatella", "left patella", BodypartTypeEnum.Bone, 90, "Compact Bone");
		AddBone(quadrupedBody, "rtibia", "right tibia", BodypartTypeEnum.Bone, 150, "Compact Bone");
		AddBone(quadrupedBody, "ltibia", "left tibia", BodypartTypeEnum.Bone, 150, "Compact Bone");
		AddBone(quadrupedBody, "rfibula", "right fibula", BodypartTypeEnum.Bone, 150, "Compact Bone");
		AddBone(quadrupedBody, "lfibula", "left fibula", BodypartTypeEnum.Bone, 150, "Compact Bone");
		AddBone(quadrupedBody, "rcalcaneus", "right calcaneus", BodypartTypeEnum.Bone, 80, "Compact Bone");
		AddBone(quadrupedBody, "lcalcaneus", "left calcaneus", BodypartTypeEnum.Bone, 80, "Compact Bone");
		AddBone(quadrupedBody, "rtalus", "right talus", BodypartTypeEnum.Bone, 80, "Compact Bone");
		AddBone(quadrupedBody, "ltalus", "left talus", BodypartTypeEnum.Bone, 80, "Compact Bone");
		AddBone(quadrupedBody, "rtarsus", "right tarsus", BodypartTypeEnum.Bone, 80, "Compact Bone");
		AddBone(quadrupedBody, "ltarsus", "left tarsus", BodypartTypeEnum.Bone, 80, "Compact Bone");
		AddBone(quadrupedBody, "rmetatarsus", "right metatarsus", BodypartTypeEnum.Bone, 80, "Compact Bone");
		AddBone(quadrupedBody, "lmetatarsus", "left metatarsus", BodypartTypeEnum.Bone, 80, "Compact Bone");

		// TORSO BONES
		AddBoneInternal("sternum", "abdomen", 50);
		AddBoneInternal("rrib1", "rshoulder", 10);
		AddBoneInternal("lrib1", "lshoulder", 10);
		AddBoneInternal("rrib2", "rbreast", 5);
		AddBoneInternal("lrib2", "lbreast", 5);
		AddBoneInternal("rrib3", "rbreast", 5);
		AddBoneInternal("lrib3", "lbreast", 5);
		AddBoneInternal("rrib4", "rbreast", 5);
		AddBoneInternal("lrib4", "lbreast", 5);
		AddBoneInternal("rrib5", "rbreast", 5);
		AddBoneInternal("lrib5", "lbreast", 5);
		AddBoneInternal("rrib6", "rbreast", 5);
		AddBoneInternal("lrib6", "lbreast", 5);
		AddBoneInternal("rrib7", "rbreast", 5);
		AddBoneInternal("lrib7", "lbreast", 5);
		AddBoneInternal("rrib8", "rbreast", 5);
		AddBoneInternal("lrib8", "lbreast", 5);
		AddBoneInternal("rrib9", "rbreast", 5);
		AddBoneInternal("lrib9", "lbreast", 5);
		AddBoneInternal("rrib10", "rbreast", 5);
		AddBoneInternal("lrib10", "lbreast", 5);
		AddBoneInternal("rrib11", "rbreast", 5);
		AddBoneInternal("lrib11", "lbreast", 5);
		AddBoneInternal("rrib12", "rbreast", 5);
		AddBoneInternal("lrib12", "lbreast", 5);
		AddBoneInternal("cvertebrae", "bneck", 35);
		AddBoneInternal("dvertebrae", "uback", 20);
		AddBoneInternal("dvertebrae", "withers", 20, false);
		AddBoneInternal("lvertebrae", "lback", 20);
		AddBoneInternal("svertebrae", "utail", 90);
		AddBoneInternal("svertebrae", "mtail", 90, false);
		AddBoneInternal("cavertebrae", "ltail", 80);
		AddBoneInternal("sacrum", "lback", 15);
		AddBoneInternal("rilium", "rrump", 50);
		AddBoneInternal("lilium", "lrump", 50);
		AddBoneInternal("rilium", "lback", 4, false);
		AddBoneInternal("lilium", "lback", 4, false);
		AddBoneInternal("rpubis", "groin", 20);
		AddBoneInternal("lpubis", "groin", 20);
		AddBoneInternal("rischium", "rrump", 20);
		AddBoneInternal("lischium", "lrump", 20);

		// HEAD BONES
		AddBoneInternal("smaxillary", "muzzle", 100);
		AddBoneInternal("smaxillary", "mouth", 40, false);
		AddBoneInternal("smaxillary", "rcheek", 20, false);
		AddBoneInternal("smaxillary", "lcheek", 20, false);
		AddBoneInternal("rimaxillary", "rjaw", 100);
		AddBoneInternal("limaxillary", "ljaw", 100);
		AddBoneInternal("fskull", "head", 100);
		AddBoneInternal("fskull", "bhead", 40, false);

		// ARM BONES
		AddBoneInternal("rscapula", "rshoulder", 100);
		AddBoneInternal("lscapula", "lshoulder", 100);
		AddBoneInternal("rhumerus", "ruforeleg", 50);
		AddBoneInternal("lhumerus", "luforeleg", 50);
		AddBoneInternal("rradius", "rlforeleg", 33);
		AddBoneInternal("lradius", "llforeleg", 33);
		AddBoneInternal("rulna", "rlforeleg", 33);
		AddBoneInternal("lulna", "llforeleg", 33);
		AddBoneInternal("rcarpal", "rfhock", 50);
		AddBoneInternal("lcarpal", "lfhock", 50);
		AddBoneInternal("rmetacarpal", "rfhoof", 50);
		AddBoneInternal("lmetacarpal", "lfhoof", 50);
		AddBoneInternal("rmetacarpal", "rfpaw", 50);
		AddBoneInternal("lmetacarpal", "lfpaw", 50);

		// LEG BONES
		AddBoneInternal("rfemur", "ruforeleg", 50);
		AddBoneInternal("lfemur", "luforeleg", 50);
		AddBoneInternal("rpatella", "rrknee", 100);
		AddBoneInternal("lpatella", "rlknee", 100);
		AddBoneInternal("rtibia", "rlforeleg", 100);
		AddBoneInternal("ltibia", "llforeleg", 100);
		AddBoneInternal("rfibula", "rlforeleg", 33);
		AddBoneInternal("lfibula", "llforeleg", 33);
		AddBoneInternal("rcalcaneus", "rrhock", 20);
		AddBoneInternal("lcalcaneus", "lrhock", 20);
		AddBoneInternal("rtalus", "rrhock", 20);
		AddBoneInternal("ltalus", "lrhock", 20);
		AddBoneInternal("rtarsus", "rrhock", 50);
		AddBoneInternal("ltarsus", "lrhock", 50);
		AddBoneInternal("rmetatarsus", "rrhoof", 50);
		AddBoneInternal("lmetatarsus", "lrhoof", 50);
		AddBoneInternal("rmetatarsus", "rrpaw", 50);
		AddBoneInternal("lmetatarsus", "lrpaw", 50);
		_context.SaveChanges();

		AddBoneCover("fskull", "brain", 100);
		AddBoneCover("smaxillary", "brain", 90);

		AddBoneCover("cvertebrae", "uspinalcord", 100);
		AddBoneCover("dvertebrae", "mspinalcord", 100);
		AddBoneCover("lvertebrae", "lspinalcord", 100);

		AddBoneCover("sternum", "heart", 80);
		AddBoneCover("sternum", "rlung", 17.5);
		AddBoneCover("sternum", "llung", 17.5);
		AddBoneCover("lrib1", "heart", 5);
		AddBoneCover("lrib2", "heart", 10);
		AddBoneCover("lrib3", "heart", 15);
		AddBoneCover("lrib4", "heart", 15);
		AddBoneCover("lrib5", "heart", 15);
		AddBoneCover("lrib6", "heart", 15);
		AddBoneCover("lrib1", "llung", 10);
		AddBoneCover("lrib2", "llung", 15);
		AddBoneCover("lrib3", "llung", 20);
		AddBoneCover("lrib4", "llung", 20);
		AddBoneCover("lrib5", "llung", 20);
		AddBoneCover("lrib6", "llung", 20);
		AddBoneCover("lrib7", "llung", 20);
		AddBoneCover("rrib1", "rlung", 10);
		AddBoneCover("rrib2", "rlung", 15);
		AddBoneCover("rrib3", "rlung", 20);
		AddBoneCover("rrib4", "rlung", 20);
		AddBoneCover("rrib5", "rlung", 20);
		AddBoneCover("rrib6", "rlung", 20);
		AddBoneCover("rrib7", "rlung", 20);

		AddBoneCover("rrib6", "liver", 30);
		AddBoneCover("rrib7", "liver", 45);
		AddBoneCover("lrib6", "liver", 30);
		AddBoneCover("lrib7", "liver", 45);

		AddBoneCover("lrib8", "liver", 80);
		AddBoneCover("lrib8", "spleen", 25);
		AddBoneCover("rrib8", "liver", 80);
		AddBoneCover("rrib8", "spleen", 25);

		AddBoneCover("lrib9", "liver", 60);
		AddBoneCover("lrib9", "spleen", 20);
		AddBoneCover("rrib9", "liver", 60);
		AddBoneCover("rrib9", "spleen", 20);

		AddBoneCover("lrib10", "liver", 15);
		AddBoneCover("lrib10", "lkidney", 20);
		AddBoneCover("rrib10", "liver", 15);
		AddBoneCover("rrib10", "rkidney", 20);

		AddBoneCover("rscapula", "rlung", 70);
		AddBoneCover("lscapula", "llung", 70);

		AddBoneCover("rilium", "sintestines", 20);
		AddBoneCover("lilium", "sintestines", 20);
		AddBoneCover("rilium", "lintestines", 40);
		AddBoneCover("lilium", "lintestines", 40);

		AddBoneCover("rischium", "lintestines", 40);
		AddBoneCover("lischium", "lintestines", 40);
		_context.SaveChanges();

		#endregion

		_context.SaveChanges();

		foreach (var (child, parent) in _cachedBodypartUpstreams)
			_context.BodypartProtoBodypartProtoUpstream.Add(new BodypartProtoBodypartProtoUpstream
			{
				Child = child.Id,
				Parent = parent.Id
			});

		_context.SaveChanges();

		Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Limbs...");

		#region Limbs

		var limbs = new Dictionary<string, Limb>(StringComparer.OrdinalIgnoreCase);

		void AddLimb(string name, LimbType limbType, string rootPart, double damageThreshold,
			double painThreshold)
		{
			var limb = new Limb
			{
				Name = name,
				LimbType = (int)limbType,
				RootBody = quadrupedBody,
				RootBodypart = _cachedBodyparts[rootPart],
				LimbDamageThresholdMultiplier = damageThreshold,
				LimbPainThresholdMultiplier = painThreshold
			};
			_context.Limbs.Add(limb);
			limbs[name] = limb;
		}

		AddLimb("Torso", LimbType.Torso, "abdomen", 1.0, 1.0);
		AddLimb("Head", LimbType.Head, "neck", 1.0, 1.0);
		AddLimb("Genitals", LimbType.Genitals, "groin", 0.5, 0.5);
		AddLimb("Right Foreleg", LimbType.Leg, "ruforeleg", 0.5, 0.5);
		AddLimb("Left Foreleg", LimbType.Leg, "luforeleg", 0.5, 0.5);
		AddLimb("Right Hindleg", LimbType.Leg, "ruhindleg", 0.5, 0.5);
		AddLimb("Left Hindleg", LimbType.Leg, "luhindleg", 0.5, 0.5);
		AddLimb("Tail", LimbType.Appendage, "utail", 0.5, 0.5);
		AddLimb("Right Wing", LimbType.Wing, "rwingbase", 0.5, 0.5);
		AddLimb("Left Wing", LimbType.Wing, "lwingbase", 0.5, 0.5);
		_context.SaveChanges();

		foreach (var limb in limbs.Values)
		{
			foreach (var part in _cachedLimbs[limb.Name])
				_context.LimbsBodypartProto.Add(new LimbBodypartProto { BodypartProto = part, Limb = limb });

			switch (limb.Name)
			{
				case "Torso":
				case "Right Wing":
				case "Left Wing":
					_context.LimbsSpinalParts.Add(new LimbsSpinalPart
					{ Limb = limb, BodypartProto = _cachedOrgans["uspinalcord"] });
					break;
				case "Genitals":
				case "Right Foreleg":
				case "Left Foreleg":
					_context.LimbsSpinalParts.Add(new LimbsSpinalPart
					{ Limb = limb, BodypartProto = _cachedOrgans["mspinalcord"] });
					break;
				case "Leg Hindleg":
				case "Right Hindleg":
				case "Tail":
					_context.LimbsSpinalParts.Add(new LimbsSpinalPart
					{ Limb = limb, BodypartProto = _cachedOrgans["lspinalcord"] });
					break;
			}
		}

		_context.SaveChanges();

		#endregion

		Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Group Describers...");

		#region Groups

		AddBodypartGroupDescriberShape(quadrupedBody, "body", "The whole torso of a quadruped",
			("abdomen", 1, 1),
			("belly", 1, 1),
			("withers", 1, 1),
			("breast", 1, 2),
			("flank", 1, 2),
			("loin", 1, 2),
			("shoulder", 1, 2),
			("upper back", 1, 1),
			("lower back", 1, 1),
			("rump", 1, 2),
			("neck", 0, 1),
			("neck back", 0, 1),
			("throat", 0, 1),
			("udder", 0, 1)
		);
		AddBodypartGroupDescriberShape(quadrupedBody, "legs", "Four legs of a quadruped",
			("upper hindleg", 1, 2),
			("upper foreleg", 1, 2),
			("lower hindleg", 0, 2),
			("lower foreleg", 0, 2),
			("knee", 0, 4),
			("front hock", 0, 2),
			("rear hock", 0, 2),
			("hoof", 0, 4),
			("paw", 0, 4),
			("claw", 0, 4),
			("frog", 0, 4),
			("dewclaw", 0, 2)
		);
		AddBodypartGroupDescriberShape(quadrupedBody, "forelegs", "Both forelegs of a quadruped",
			("upper foreleg", 2, 2),
			("lower foreleg", 0, 2),
			("knee", 0, 2),
			("front hock", 0, 2),
			("hoof", 0, 2),
			("paw", 0, 2),
			("claw", 0, 2),
			("frog", 0, 2)
		);
		AddBodypartGroupDescriberShape(quadrupedBody, "hindlegs", "Both hindlegs of a quadruped",
			("upper hindleg", 2, 2),
			("lower hindleg", 0, 2),
			("knee", 0, 2),
			("rear hock", 0, 2),
			("hoof", 0, 2),
			("paw", 0, 2),
			("claw", 0, 2),
			("frog", 0, 2),
			("dewclaw", 0, 2)
		);
		AddBodypartGroupDescriberShape(quadrupedBody, "tail", "The tail bodyparts",
			("tail", 1, 3)
		);

		AddBodypartGroupDescriberShape(quadrupedBody, "head", "A quadruped head",
			("face", 1, 1),
			("head back", 0, 1),
			("eye socket", 0, 2),
			("eye", 0, 2),
			("ear", 0, 2),
			("jaw", 0, 2),
			("muzzle", 0, 1),
			("nose", 0, 1),
			("mouth", 0, 1),
			("tongue", 0, 1),
			("cheek", 0, 2),
			("throat", 0, 1),
			("withers", 0, 1),
			("neck", 0, 1),
			("neck back", 0, 1),
			("horn", 0, 2),
			("tusk", 0, 2),
			("antler", 0, 2)
		);

		AddBodypartGroupDescriberShape(quadrupedBody, "wings", "A pair of quadruped wings",
			("wing base", 2, 2),
			("wing", 2, 2)
		);

		AddBodypartGroupDescriberShape(quadrupedBody, "back", "A quadruped back",
			("upper back", 1, 1),
			("lower back", 1, 1),
			("flank", 0, 4),
			("rump", 0, 2),
			("withers", 0, 1),
			("neck back", 0, 1)
		);

		AddBodypartGroupDescriberShape(quadrupedBody, "eyes", "A pair of quadruped eyes",
			("eye socket", 2, 2),
			("eye", 0, 2)
		);

		AddBodypartGroupDescriberShape(quadrupedBody, "ears", "A pair of quadruped ears",
			("ear", 2, 2)
		);

		AddBodypartGroupDescriberShape(quadrupedBody, "horns", "A pair of quadruped horns",
			("horn", 2, 2)
		);

		AddBodypartGroupDescriberShape(quadrupedBody, "tusks", "A pair of quadruped tusks",
			("tusk", 2, 2)
		);

		AddBodypartGroupDescriberShape(quadrupedBody, "antlers", "A pair of quadruped antlers",
			("antler", 2, 2)
		);

		AddBodypartGroupDescriberShape(quadrupedBody, "hooves", "A group of quadruped hooves",
			("hoof", 2, 4)
		);

		AddBodypartGroupDescriberShape(quadrupedBody, "paws", "A group of quadruped paws",
			("paw", 2, 4)
		);

		AddBodypartGroupDescriberShape(quadrupedBody, "claws", "A group of quadruped claws",
			("claw", 2, 4)
		);

		AddBodypartGroupDescriberShape(quadrupedBody, "knees", "A group of quadruped knees",
			("knee", 2, 4)
		);

		AddBodypartGroupDescriberShape(quadrupedBody, "shoulders", "A group of quadruped shoulders",
			("shoulder", 2, 4)
		);

		_context.SaveChanges();

		#endregion

		Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Races...");

		#region Races

		SeedAnimalRaces(GetMammalRaceTemplates(),
			("Ungulate", ungulateBody),
			("Toed Quadruped", toedQuadruped));

		#endregion
	}
	#endregion

	private (BloodModel BloodModel, PopulationBloodModel PopulationBloodModel) SetupBloodModel(string race,
		IEnumerable<string> antigens,
		IEnumerable<(string Name, IEnumerable<string> Antigens, double weight)> types)
	{
		var model = new BloodModel
		{
			Name = $"{race} Blood Model"
		};
		_context.BloodModels.Add(model);

		var dbantigens = new Dictionary<string, BloodtypeAntigen>();
		foreach (var antigen in antigens)
		{
			var dbantigen = new BloodtypeAntigen
			{
				Name = antigen
			};
			_context.BloodtypeAntigens.Add(dbantigen);
			dbantigens[antigen] = dbantigen;
		}

		var populationModel = new PopulationBloodModel
		{
			Name = $"Ordinary {race} Blood Type Distribution"
		};
		_context.PopulationBloodModels.Add(populationModel);

		foreach (var (name, contained, weight) in types)
		{
			var bloodtype = new Bloodtype
			{
				Name = name
			};
			foreach (var item in contained)
				bloodtype.BloodtypesBloodtypeAntigens.Add(new BloodtypesBloodtypeAntigens
				{
					Bloodtype = bloodtype,
					BloodtypeAntigen = dbantigens[item]
				});

			_context.Bloodtypes.Add(bloodtype);
			model.BloodModelsBloodtypes.Add(new BloodModelsBloodtypes
			{
				BloodModel = model,
				Bloodtype = bloodtype
			});
			populationModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
			{
				Bloodtype = bloodtype,
				PopulationBloodModel = populationModel,
				Weight = weight
			});
		}

		return (model, populationModel);
	}


	#region Bodyparts
	private void AddShape(string name)
	{
		_context.BodypartShapes.Add(new BodypartShape
		{
			Name = name
		});
	}

	private void AddBodypartUsage(string bodypart, string usage, BodyProto body)
	{
		_context.BodyProtosAdditionalBodyparts.Add(new BodyProtosAdditionalBodyparts
		{
			BodyProto = body,
			Usage = usage,
			Bodypart = _cachedBodyparts[bodypart]
		});
	}

	private void AddRacialBodypartUsage(string bodypart, string usage, Race race)
	{
		_context.RacesAdditionalBodyparts.Add(new RacesAdditionalBodyparts
		{
			Race = race,
			Usage = usage,
			Bodypart = _cachedBodyparts[bodypart]
		});
	}

	private static int GetAnimalRelativeHitChance(BodyProto body, string alias, int fallback) => body.Name switch
	{
		"Quadruped Base" => GetQuadrupedRelativeHitChance(alias, fallback),
		"Ungulate" => GetUngulateRelativeHitChance(alias, fallback),
		"Toed Quadruped" => GetToedQuadrupedRelativeHitChance(alias, fallback),
		"Pinniped" => GetPinnipedRelativeHitChance(alias, fallback),
		"Avian" => GetAvianRelativeHitChance(alias, fallback),
		"Vermiform" or "Serpentine" => GetSerpentRelativeHitChance(alias, fallback),
		"Piscine" => GetFishRelativeHitChance(alias, fallback),
		"Cetacean" => GetCetaceanRelativeHitChance(alias, fallback),
		"Cephalopod" => GetCephalopodRelativeHitChance(alias, fallback),
		"Jellyfish" => GetJellyfishRelativeHitChance(alias, fallback),
		"Insectoid" or "Winged Insectoid" => GetInsectoidRelativeHitChance(alias, fallback),
		_ => fallback
	};

	private static int GetQuadrupedRelativeHitChance(string alias, int fallback) => alias switch
	{
		"abdomen" => 90,
		"rbreast" or "lbreast" => 60,
		"urflank" or "ulflank" or "lrflank" or "llflank" => 100,
		"belly" => 80,
		"rshoulder" or "lshoulder" => 35,
		"uback" or "lback" => 100,
		"withers" => 20,
		"rrump" or "lrump" => 40,
		"rloin" or "lloin" => 35,
		"neck" => 30,
		"bneck" => 18,
		"throat" => 10,
		"head" => 45,
		"bhead" => 25,
		"rjaw" or "ljaw" => 12,
		"rcheek" or "lcheek" => 10,
		"reyesocket" or "leyesocket" => 5,
		"reye" or "leye" => 3,
		"rear" or "lear" => 4,
		"muzzle" => 15,
		"mouth" => 10,
		"tongue" => 1,
		"nose" => 5,
		"ruforeleg" or "luforeleg" or "ruhindleg" or "luhindleg" => 50,
		"rfknee" or "lfknee" or "rrknee" or "rlknee" => 12,
		"rlforeleg" or "llforeleg" or "rlhindleg" or "llhindleg" => 35,
		"rfhock" or "lfhock" or "rrhock" or "lrhock" => 8,
		"utail" or "mtail" or "ltail" => 20,
		"groin" => 8,
		"testicles" or "penis" => 1,
		"rwingbase" or "lwingbase" => 15,
		"rwing" or "lwing" => 60,
		"udder" => 10,
		"rhorn" or "lhorn" or "horn" or "rantler" or "lantler" or "rtusk" or "ltusk" => 2,
		_ => fallback
	};

	private static int GetUngulateRelativeHitChance(string alias, int fallback) => alias switch
	{
		"rfhoof" or "lfhoof" or "rrhoof" or "lrhoof" => 15,
		"rffrog" or "lffrog" or "rrfrog" or "lrfrog" => 1,
		_ => GetQuadrupedRelativeHitChance(alias, fallback)
	};

	private static int GetToedQuadrupedRelativeHitChance(string alias, int fallback) => alias switch
	{
		"rfpaw" or "lfpaw" or "rrpaw" or "lrpaw" => 15,
		"rfclaw" or "lfclaw" or "rrclaw" or "lrclaw" => 3,
		"rrdewclaw" or "lrdewclaw" => 1,
		_ => GetQuadrupedRelativeHitChance(alias, fallback)
	};

	private static int GetPinnipedRelativeHitChance(string alias, int fallback) => alias switch
	{
		"rfrontflipper" or "lfrontflipper" or "rhindflipper" or "lhindflipper" => 50,
		"tail" => 15,
		"rtusk" or "ltusk" => 2,
		_ => GetQuadrupedRelativeHitChance(alias, fallback)
	};

	private static int GetAvianRelativeHitChance(string alias, int fallback) => alias switch
	{
		"abdomen" => 65,
		"rbreast" or "lbreast" => 55,
		"urflank" or "ulflank" or "lrflank" or "llflank" => 45,
		"belly" => 55,
		"rshoulder" or "lshoulder" => 20,
		"uback" or "lback" => 55,
		"rump" => 30,
		"loin" => 25,
		"neck" => 22,
		"bneck" => 12,
		"throat" => 8,
		"head" => 28,
		"bhead" => 18,
		"rcheek" or "lcheek" => 6,
		"reyesocket" or "leyesocket" => 4,
		"reye" or "leye" => 2,
		"rear" or "lear" => 2,
		"beak" => 10,
		"tongue" => 1,
		"nose" => 3,
		"rupperleg" or "lupperleg" => 20,
		"rknee" or "lknee" => 5,
		"rlowerleg" or "llowerleg" => 16,
		"rankle" or "lankle" => 4,
		"rfoot" or "lfoot" => 10,
		"rtalons" or "ltalons" => 3,
		"tail" => 18,
		"groin" => 4,
		"rwingbase" or "lwingbase" => 10,
		"rwing" or "lwing" => 65,
		_ => fallback
	};

	private static int GetSerpentRelativeHitChance(string alias, int fallback) => alias switch
	{
		"head" => 25,
		"mouth" => 8,
		"fangs" => 1,
		"reyesocket" or "leyesocket" => 3,
		"reye" or "leye" => 2,
		"tongue" => 1,
		"neck" => 12,
		"ubody" => 80,
		"mbody" => 100,
		"lbody" => 70,
		"tail" => 35,
		_ => fallback
	};

	private static int GetFishRelativeHitChance(string alias, int fallback) => alias switch
	{
		"abdomen" => 80,
		"rbreast" or "lbreast" => 50,
		"urflank" or "ulflank" or "lrflank" or "llflank" => 90,
		"belly" => 65,
		"uback" or "lback" => 80,
		"loin" => 55,
		"dorsalfin" or "analfin" or "rpectoralfin" or "lpectoralfin" or "rpelvicfin" or "lpelvicfin" => 12,
		"neck" => 12,
		"rgill" or "lgill" => 10,
		"head" => 35,
		"reyesocket" or "leyesocket" => 4,
		"reye" or "leye" => 2,
		"mouth" => 12,
		"peduncle" => 30,
		"caudalfin" => 20,
		_ => fallback
	};

	private static int GetCetaceanRelativeHitChance(string alias, int fallback) => alias switch
	{
		"abdomen" => 90,
		"rbreast" or "lbreast" => 55,
		"urflank" or "ulflank" or "lrflank" or "llflank" => 100,
		"belly" => 70,
		"uback" or "lback" => 90,
		"loin" => 60,
		"dorsalfin" => 10,
		"rpectoralfin" or "lpectoralfin" => 18,
		"neck" => 10,
		"blowhole" => 4,
		"head" => 40,
		"reyesocket" or "leyesocket" => 4,
		"reye" or "leye" => 2,
		"mouth" => 14,
		"stock" => 25,
		"fluke" => 30,
		_ => fallback
	};

	private static int GetCephalopodRelativeHitChance(string alias, int fallback)
	{
		if (alias.StartsWith("arm", StringComparison.OrdinalIgnoreCase))
		{
			return 18;
		}

		if (alias.StartsWith("tentacle", StringComparison.OrdinalIgnoreCase))
		{
			return 20;
		}

		return alias switch
		{
			"abdomen" => 70,
			"mouth" => 8,
			"head" => 25,
			"mantle" => 55,
			"reye" or "leye" => 3,
			_ => fallback
		};
	}

	private static int GetJellyfishRelativeHitChance(string alias, int fallback)
	{
		if (alias.StartsWith("tendril", StringComparison.OrdinalIgnoreCase))
		{
			return 6;
		}

		return alias switch
		{
			"body" => 50,
			_ => fallback
		};
	}

	private static int GetInsectoidRelativeHitChance(string alias, int fallback)
	{
		if (alias.StartsWith("rleg", StringComparison.OrdinalIgnoreCase) ||
		    alias.StartsWith("lleg", StringComparison.OrdinalIgnoreCase))
		{
			return 5;
		}

		return alias switch
		{
			"thorax" => 35,
			"head" => 18,
			"abdomen" => 30,
			"rantenna" or "lantenna" => 1,
			"mandibles" => 2,
			"reye" or "leye" => 1,
			"rwingbase" or "lwingbase" => 3,
			"rwing" or "lwing" => 10,
			_ => fallback
		};
	}

	private void AddBodypart(BodyProto body, string alias, string name, string shape,
		BodypartTypeEnum type, string? upstreamPartName, Alignment alignment,
		Orientation orientation, int hitPoints, int severThreshold, int hitChance, int displayOrder,
		string material, SizeCategory size, string limb, bool isSignificant = true, double infectability = 1.0,
		bool isVital = false, double hypoxia = 0.0, double implantSpace = 0, double implantSpaceOccupied = 0,
		bool isCore = true, double bleedMultiplier = 1.0, double damageMultiplier = 1.0,
		double painMultiplier = 1.0, double stunMultiplier = 0.0)
	{
		var bodypart = new BodypartProto
		{
			BodypartShape = _cachedShapes[shape],
			Body = body,
			Name = alias,
			Description = name,
			BodypartType = (int)type,
			Alignment = (int)alignment,
			Location = (int)orientation,
			BleedModifier = bleedMultiplier,
			DamageModifier = damageMultiplier,
			PainModifier = painMultiplier,
			StunModifier = stunMultiplier,
			MaxLife = hitPoints,
			SeveredThreshold = _sever ? severThreshold : -1,
			IsCore = isCore,
			IsVital = isVital,
			Significant = isSignificant,
			RelativeInfectability = infectability,
			HypoxiaDamagePerTick = hypoxia,
			ImplantSpace = implantSpace,
			ImplantSpaceOccupied = implantSpaceOccupied,
			Size = (int)size,
			DisplayOrder = displayOrder,
			RelativeHitChance = GetAnimalRelativeHitChance(body, alias, hitChance),
			DefaultMaterial = _cachedMaterials[material],
			ArmourType = _naturalArmour
		};

		switch (type)
		{
			case BodypartTypeEnum.Grabbing:
				bodypart.Unary = false;
				break;
			case BodypartTypeEnum.Wielding:
				bodypart.Unary = true;
				bodypart.MaxSingleSize = (int)SizeCategory.Normal;
				break;
			case BodypartTypeEnum.GrabbingWielding:
				bodypart.Unary = true;
				bodypart.MaxSingleSize = (int)SizeCategory.Normal;
				break;
		}

		_context.BodypartProtos.Add(bodypart);
		_cachedBodyparts[alias] = bodypart;
		_cachedLimbs.Add(limb, bodypart);
		if (!string.IsNullOrEmpty(upstreamPartName))
			_cachedBodypartUpstreams.Add((bodypart, _cachedBodyparts[upstreamPartName]));
	}

	private void AddOrgan(BodyProto body, string alias, string description, BodypartTypeEnum type,
		double implantSpaceOccupied, int hitPoints, double bleedModifier, double infectionModifier,
		double hypoxiaDamage, double damageModifier = 1.0, double stunModifier = 0.0, double painModifier = 1.0)
	{
		var organ = new BodypartProto
		{
			Name = alias,
			Description = description,
			Body = body,
			BodypartType = (int)type,
			IsCore = true,
			IsOrgan = 1,
			IsVital = true,
			MaxLife = hitPoints,
			SeveredThreshold = -1,
			DisplayOrder = 1,
			BleedModifier = bleedModifier,
			DamageModifier = damageModifier,
			PainModifier = painModifier,
			StunModifier = stunModifier,
			HypoxiaDamagePerTick = hypoxiaDamage,
			RelativeInfectability = infectionModifier,
			Size = (int)SizeCategory.Small,
			Location = (int)Orientation.Irrelevant,
			Alignment = (int)Alignment.Irrelevant,
			BodypartShape = _cachedShapes["organ"],
			RelativeHitChance = 0,
			DefaultMaterial = _cachedMaterials["viscera"],
			ImplantSpaceOccupied = implantSpaceOccupied,
			ArmourType = _organArmour
		};
		_context.BodypartProtos.Add(organ);
		_cachedOrgans[alias] = organ;
	}

	private void AddOrganCoverage(string whichOrgan, string whichBodypart, int hitChance, bool isPrimary = false)
	{
		_context.BodypartInternalInfos.Add(new BodypartInternalInfos
		{
			BodypartProto = _cachedBodyparts[whichBodypart],
			InternalPart = _cachedOrgans[whichOrgan],
			HitChance = hitChance,
			IsPrimaryOrganLocation = isPrimary
		});
	}

	private void AddBone(BodyProto body, string alias, string description, BodypartTypeEnum type, int hitPoints,
		string material, SizeCategory size = SizeCategory.Small)
	{
		var bone = new BodypartProto
		{
			Name = alias,
			Body = body,
			Description = description,
			BodypartType = (int)type,
			MaxLife = hitPoints,
			DefaultMaterial = _cachedMaterials[material],
			Size = (int)size,
			RelativeHitChance = 0,
			StunModifier = 0,
			Location = (int)Orientation.Irrelevant,
			Alignment = (int)Alignment.Irrelevant,
			BodypartShape = _cachedShapes["bone"],
			HypoxiaDamagePerTick = 0,
			BleedModifier = 0,
			DamageModifier = 1.0,
			PainModifier = 1.0,
			RelativeInfectability = 0.0,
			DisplayOrder = 1,
			SeveredThreshold = -1,
			IsCore = true,
			IsOrgan = 0,
			IsVital = false,
			ArmourType = _boneArmour
		};
		_context.BodypartProtos.Add(bone);
		_cachedBones[alias] = bone;
	}

	private void AddBoneInternal(string whichBone, string whichBodypart, int hitChance, bool isPrimary = true)
	{
		_context.BodypartInternalInfos.Add(new BodypartInternalInfos
		{
			BodypartProto = _cachedBodyparts[whichBodypart],
			InternalPart = _cachedBones[whichBone],
			HitChance = hitChance,
			IsPrimaryOrganLocation = isPrimary
		});
	}

	private void AddBoneCover(string bone, string organ, double coverage)
	{
		_context.BoneOrganCoverages.Add(new BoneOrganCoverage
		{
			Bone = _cachedBones[bone],
			Organ = _cachedOrgans[organ],
			CoverageChance = coverage
		});
	}

	private void AddBodypartGroupDescriberShape(BodyProto body, string describedAs, string comment,
		params (string Shape, int MinCount, int MaxCount)[] includedShapes)
	{
		var describer = new BodypartGroupDescriber
		{
			DescribedAs = describedAs,
			Comment = comment,
			Type = "shape"
		};
		_context.BodypartGroupDescribers.Add(describer);
		foreach (var (shape, minCount, maxCount) in includedShapes)
			describer.BodypartGroupDescribersShapeCount.Add(new BodypartGroupDescribersShapeCount
			{
				Target = _cachedShapes[shape],
				BodypartGroupDescriptionRule = describer,
				MinCount = minCount,
				MaxCount = maxCount
			});

		describer.BodypartGroupDescribersBodyProtos.Add(new BodypartGroupDescribersBodyProtos
		{ BodypartGroupDescriber = describer, BodyProto = body });
	}

	private void AddBodypartGroupDescriberDirect(BodyProto body, string describedAs, string comment,
		params (string Part, bool Mandatory)[] includedParts)
	{
		var describer = new BodypartGroupDescriber
		{
			DescribedAs = describedAs,
			Comment = comment,
			Type = "bodypart"
		};
		_context.BodypartGroupDescribers.Add(describer);
		foreach (var (part, mandatory) in includedParts)
			describer.BodypartGroupDescribersBodypartProtos.Add(new BodypartGroupDescribersBodypartProtos
			{
				BodypartGroupDescriber = describer,
				BodypartProto = _cachedBodyparts[part],
				Mandatory = mandatory
			});

		describer.BodypartGroupDescribersBodyProtos.Add(new BodypartGroupDescribersBodyProtos
		{ BodypartGroupDescriber = describer, BodyProto = body });
	}

	#endregion

	#region Speeds and Positions
	private void SetupSpeeds(BodyProto quadrupedBody, BodyProto avianBody, BodyProto serpentBody,
			BodyProto fishProto, BodyProto crabProto, BodyProto octopusProto, BodyProto jellyfishProto,
			BodyProto pinnipedProto, BodyProto cetaceanProto, BodyProto wormBody, BodyProto insectBody,
			BodyProto wingedInsectBody)
	{
		Console.WriteLine($"[{_stopwatch.Elapsed.TotalSeconds:N1}s] Setting up Speeds");
		var nextId = _context.MoveSpeeds.Select(x => x.Id).AsEnumerable().DefaultIfEmpty(0).Max() + 1;

		#region Speeds

		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = quadrupedBody,
			PositionId = 1,
			Alias = "stalk",
			FirstPersonVerb = "stalk",
			ThirdPersonVerb = "stalks",
			PresentParticiple = "stalking",
			Multiplier = 2,
			StaminaMultiplier = 0.4
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = quadrupedBody,
			PositionId = 1,
			Alias = "walk",
			FirstPersonVerb = "walk",
			ThirdPersonVerb = "walks",
			PresentParticiple = "walking",
			Multiplier = 1,
			StaminaMultiplier = 0.8
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = quadrupedBody,
			PositionId = 1,
			Alias = "amble",
			FirstPersonVerb = "amble",
			ThirdPersonVerb = "ambles",
			PresentParticiple = "ambling",
			Multiplier = 0.8,
			StaminaMultiplier = 1.2
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = quadrupedBody,
			PositionId = 1,
			Alias = "pace",
			FirstPersonVerb = "pace",
			ThirdPersonVerb = "paces",
			PresentParticiple = "pacing",
			Multiplier = 0.6,
			StaminaMultiplier = 1.9
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = quadrupedBody,
			PositionId = 1,
			Alias = "trot",
			FirstPersonVerb = "trot",
			ThirdPersonVerb = "trots",
			PresentParticiple = "trotting",
			Multiplier = 0.4,
			StaminaMultiplier = 2.4
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = quadrupedBody,
			PositionId = 1,
			Alias = "gallop",
			FirstPersonVerb = "gallop",
			ThirdPersonVerb = "gallops",
			PresentParticiple = "galloping",
			Multiplier = 0.2,
			StaminaMultiplier = 4.0
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = quadrupedBody,
			PositionId = 6,
			Alias = "crawl",
			FirstPersonVerb = "crawl",
			ThirdPersonVerb = "crawls",
			PresentParticiple = "crawling",
			Multiplier = 5,
			StaminaMultiplier = 1.25
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = quadrupedBody,
			PositionId = 7,
			Alias = "shuffle",
			FirstPersonVerb = "shuffle",
			ThirdPersonVerb = "shuffles",
			PresentParticiple = "shuffling",
			Multiplier = 7,
			StaminaMultiplier = 2
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = quadrupedBody,
			PositionId = 15,
			Alias = "climb",
			FirstPersonVerb = "climb",
			ThirdPersonVerb = "climbs",
			PresentParticiple = "climbing",
			Multiplier = 3,
			StaminaMultiplier = 3
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = quadrupedBody,
			PositionId = 16,
			Alias = "swim",
			FirstPersonVerb = "swim",
			ThirdPersonVerb = "swims",
			PresentParticiple = "swimming",
			Multiplier = 1.5,
			StaminaMultiplier = 2
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = quadrupedBody,
			PositionId = 16,
			Alias = "slowswim",
			FirstPersonVerb = "swim",
			ThirdPersonVerb = "swims",
			PresentParticiple = "swimming",
			Multiplier = 2,
			StaminaMultiplier = 1.5
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = quadrupedBody,
			PositionId = 18,
			Alias = "fly",
			FirstPersonVerb = "fly",
			ThirdPersonVerb = "flies",
			PresentParticiple = "flying",
			Multiplier = 1.8,
			StaminaMultiplier = 15
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = quadrupedBody,
			PositionId = 18,
			Alias = "franticfly",
			FirstPersonVerb = "frantically fly",
			ThirdPersonVerb = "frantically flies",
			PresentParticiple = "frantically flying",
			Multiplier = 1.4,
			StaminaMultiplier = 25
		});

		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = avianBody,
			PositionId = 1,
			Alias = "hop",
			FirstPersonVerb = "hop",
			ThirdPersonVerb = "hops",
			PresentParticiple = "hopping",
			Multiplier = 1,
			StaminaMultiplier = 0.8
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = avianBody,
			PositionId = 6,
			Alias = "crawl",
			FirstPersonVerb = "crawl",
			ThirdPersonVerb = "crawls",
			PresentParticiple = "crawling",
			Multiplier = 5,
			StaminaMultiplier = 1.25
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = avianBody,
			PositionId = 7,
			Alias = "shuffle",
			FirstPersonVerb = "shuffle",
			ThirdPersonVerb = "shuffles",
			PresentParticiple = "shuffling",
			Multiplier = 7,
			StaminaMultiplier = 2
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = avianBody,
			PositionId = 15,
			Alias = "climb",
			FirstPersonVerb = "climb",
			ThirdPersonVerb = "climbs",
			PresentParticiple = "climbing",
			Multiplier = 3,
			StaminaMultiplier = 3
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = avianBody,
			PositionId = 16,
			Alias = "swim",
			FirstPersonVerb = "swim",
			ThirdPersonVerb = "swims",
			PresentParticiple = "swimming",
			Multiplier = 1.5,
			StaminaMultiplier = 2
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = avianBody,
			PositionId = 16,
			Alias = "slowswim",
			FirstPersonVerb = "swim",
			ThirdPersonVerb = "swims",
			PresentParticiple = "swimming",
			Multiplier = 2,
			StaminaMultiplier = 1.5
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = avianBody,
			PositionId = 18,
			Alias = "slowfly",
			FirstPersonVerb = "slowly fly",
			ThirdPersonVerb = "slowly flies",
			PresentParticiple = "slowly flying",
			Multiplier = 2.7,
			StaminaMultiplier = 8
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = avianBody,
			PositionId = 18,
			Alias = "fly",
			FirstPersonVerb = "fly",
			ThirdPersonVerb = "flies",
			PresentParticiple = "flying",
			Multiplier = 1.8,
			StaminaMultiplier = 15
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = avianBody,
			PositionId = 18,
			Alias = "franticfly",
			FirstPersonVerb = "frantically fly",
			ThirdPersonVerb = "frantically flies",
			PresentParticiple = "frantically flying",
			Multiplier = 1.4,
			StaminaMultiplier = 25
		});

		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = fishProto,
			PositionId = 6,
			Alias = "flop",
			FirstPersonVerb = "flop",
			ThirdPersonVerb = "flops",
			PresentParticiple = "flopping",
			Multiplier = 6,
			StaminaMultiplier = 3.0
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = fishProto,
			PositionId = 16,
			Alias = "swim",
			FirstPersonVerb = "swim",
			ThirdPersonVerb = "swims",
			PresentParticiple = "swimming",
			Multiplier = 1.5,
			StaminaMultiplier = 2
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = fishProto,
			PositionId = 16,
			Alias = "slowswim",
			FirstPersonVerb = "swim",
			ThirdPersonVerb = "swims",
			PresentParticiple = "swimming",
			Multiplier = 2,
			StaminaMultiplier = 1.5
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = fishProto,
			PositionId = 16,
			Alias = "quickswim",
			FirstPersonVerb = "swim quickly",
			ThirdPersonVerb = "swims quickly",
			PresentParticiple = "swimming quickly",
			Multiplier = 1.0,
			StaminaMultiplier = 2
		});

		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = jellyfishProto,
			PositionId = 16,
			Alias = "float",
			FirstPersonVerb = "float",
			ThirdPersonVerb = "floats",
			PresentParticiple = "floating",
			Multiplier = 1.5,
			StaminaMultiplier = 2
		});

		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = serpentBody,
			PositionId = 6,
			Alias = "slither",
			FirstPersonVerb = "slither",
			ThirdPersonVerb = "slithers",
			PresentParticiple = "slithering",
			Multiplier = 1.5,
			StaminaMultiplier = 1.25
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = serpentBody,
			PositionId = 6,
			Alias = "slowslither",
			FirstPersonVerb = "slither slowly",
			ThirdPersonVerb = "slithers slowly",
			PresentParticiple = "slowly slithering",
			Multiplier = 2.5,
			StaminaMultiplier = 0.75
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = serpentBody,
			PositionId = 6,
			Alias = "quickslither",
			FirstPersonVerb = "slither quickly",
			ThirdPersonVerb = "slithers quickly",
			PresentParticiple = "quickly slithering",
			Multiplier = 1.0,
			StaminaMultiplier = 2.0
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = serpentBody,
			PositionId = 15,
			Alias = "climb",
			FirstPersonVerb = "climb",
			ThirdPersonVerb = "climbs",
			PresentParticiple = "climbing",
			Multiplier = 3,
			StaminaMultiplier = 3
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = serpentBody,
			PositionId = 16,
			Alias = "swim",
			FirstPersonVerb = "swim",
			ThirdPersonVerb = "swims",
			PresentParticiple = "swimming",
			Multiplier = 1.5,
			StaminaMultiplier = 2
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = serpentBody,
			PositionId = 16,
			Alias = "slowswim",
			FirstPersonVerb = "swim",
			ThirdPersonVerb = "swims",
			PresentParticiple = "swimming",
			Multiplier = 2,
			StaminaMultiplier = 1.5
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = serpentBody,
			PositionId = 18,
			Alias = "fly",
			FirstPersonVerb = "fly",
			ThirdPersonVerb = "flies",
			PresentParticiple = "flying",
			Multiplier = 1.8,
			StaminaMultiplier = 15
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = serpentBody,
			PositionId = 18,
			Alias = "franticfly",
			FirstPersonVerb = "frantically fly",
			ThirdPersonVerb = "frantically flies",
			PresentParticiple = "frantically flying",
			Multiplier = 1.4,
			StaminaMultiplier = 25
		});

		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = wormBody,
			PositionId = 6,
			Alias = "slither",
			FirstPersonVerb = "slither",
			ThirdPersonVerb = "slithers",
			PresentParticiple = "slithering",
			Multiplier = 1.5,
			StaminaMultiplier = 1.25
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = wormBody,
			PositionId = 6,
			Alias = "slowslither",
			FirstPersonVerb = "slither slowly",
			ThirdPersonVerb = "slithers slowly",
			PresentParticiple = "slowly slithering",
			Multiplier = 2.5,
			StaminaMultiplier = 0.75
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = wormBody,
			PositionId = 6,
			Alias = "quickslither",
			FirstPersonVerb = "slither quickly",
			ThirdPersonVerb = "slithers quickly",
			PresentParticiple = "quickly slithering",
			Multiplier = 1.0,
			StaminaMultiplier = 2.0
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = wormBody,
			PositionId = 15,
			Alias = "climb",
			FirstPersonVerb = "climb",
			ThirdPersonVerb = "climbs",
			PresentParticiple = "climbing",
			Multiplier = 3,
			StaminaMultiplier = 3
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = wormBody,
			PositionId = 16,
			Alias = "swim",
			FirstPersonVerb = "swim",
			ThirdPersonVerb = "swims",
			PresentParticiple = "swimming",
			Multiplier = 1.5,
			StaminaMultiplier = 2
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = wormBody,
			PositionId = 16,
			Alias = "slowswim",
			FirstPersonVerb = "swim",
			ThirdPersonVerb = "swims",
			PresentParticiple = "swimming",
			Multiplier = 2,
			StaminaMultiplier = 1.5
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = wormBody,
			PositionId = 18,
			Alias = "fly",
			FirstPersonVerb = "fly",
			ThirdPersonVerb = "flies",
			PresentParticiple = "flying",
			Multiplier = 1.8,
			StaminaMultiplier = 15
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = wormBody,
			PositionId = 18,
			Alias = "franticfly",
			FirstPersonVerb = "frantically fly",
			ThirdPersonVerb = "frantically flies",
			PresentParticiple = "frantically flying",
			Multiplier = 1.4,
			StaminaMultiplier = 25
		});

		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = insectBody,
			PositionId = 1,
			Alias = "walk",
			FirstPersonVerb = "walk",
			ThirdPersonVerb = "walks",
			PresentParticiple = "walking",
			Multiplier = 1,
			StaminaMultiplier = 0.8
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = insectBody,
			PositionId = 1,
			Alias = "run",
			FirstPersonVerb = "run",
			ThirdPersonVerb = "runs",
			PresentParticiple = "running",
			Multiplier = 0.5,
			StaminaMultiplier = 1.5
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = insectBody,
			PositionId = 6,
			Alias = "crawl",
			FirstPersonVerb = "crawl",
			ThirdPersonVerb = "crawls",
			PresentParticiple = "crawling",
			Multiplier = 1.5,
			StaminaMultiplier = 1.0
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = insectBody,
			PositionId = 15,
			Alias = "climb",
			FirstPersonVerb = "climb",
			ThirdPersonVerb = "climbs",
			PresentParticiple = "climbing",
			Multiplier = 2,
			StaminaMultiplier = 2
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = insectBody,
			PositionId = 18,
			Alias = "fly",
			FirstPersonVerb = "fly",
			ThirdPersonVerb = "flies",
			PresentParticiple = "flying",
			Multiplier = 1.5,
			StaminaMultiplier = 10
		});

		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = wingedInsectBody,
			PositionId = 1,
			Alias = "walk",
			FirstPersonVerb = "walk",
			ThirdPersonVerb = "walks",
			PresentParticiple = "walking",
			Multiplier = 1,
			StaminaMultiplier = 0.8
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = wingedInsectBody,
			PositionId = 1,
			Alias = "run",
			FirstPersonVerb = "run",
			ThirdPersonVerb = "runs",
			PresentParticiple = "running",
			Multiplier = 0.5,
			StaminaMultiplier = 1.5
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = wingedInsectBody,
			PositionId = 6,
			Alias = "crawl",
			FirstPersonVerb = "crawl",
			ThirdPersonVerb = "crawls",
			PresentParticiple = "crawling",
			Multiplier = 1.5,
			StaminaMultiplier = 1.0
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = wingedInsectBody,
			PositionId = 15,
			Alias = "climb",
			FirstPersonVerb = "climb",
			ThirdPersonVerb = "climbs",
			PresentParticiple = "climbing",
			Multiplier = 2,
			StaminaMultiplier = 2
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = wingedInsectBody,
			PositionId = 18,
			Alias = "fly",
			FirstPersonVerb = "fly",
			ThirdPersonVerb = "flies",
			PresentParticiple = "flying",
			Multiplier = 1.5,
			StaminaMultiplier = 10
		});

		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = pinnipedProto,
			PositionId = 1,
			Alias = "walk",
			FirstPersonVerb = "walk",
			ThirdPersonVerb = "walks",
			PresentParticiple = "walking",
			Multiplier = 3,
			StaminaMultiplier = 3.0
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = pinnipedProto,
			PositionId = 1,
			Alias = "run",
			FirstPersonVerb = "run",
			ThirdPersonVerb = "runs",
			PresentParticiple = "running",
			Multiplier = 1.75,
			StaminaMultiplier = 5.0
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = pinnipedProto,
			PositionId = 6,
			Alias = "crawl",
			FirstPersonVerb = "crawl",
			ThirdPersonVerb = "crawls",
			PresentParticiple = "crawling",
			Multiplier = 5,
			StaminaMultiplier = 3.0
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = pinnipedProto,
			PositionId = 7,
			Alias = "shuffle",
			FirstPersonVerb = "shuffle",
			ThirdPersonVerb = "shuffles",
			PresentParticiple = "shuffling",
			Multiplier = 7,
			StaminaMultiplier = 5.0
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = pinnipedProto,
			PositionId = 15,
			Alias = "climb",
			FirstPersonVerb = "climb",
			ThirdPersonVerb = "climbs",
			PresentParticiple = "climbing",
			Multiplier = 3,
			StaminaMultiplier = 3
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = pinnipedProto,
			PositionId = 16,
			Alias = "swim",
			FirstPersonVerb = "swim",
			ThirdPersonVerb = "swims",
			PresentParticiple = "swimming",
			Multiplier = 1.5,
			StaminaMultiplier = 1.5
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = pinnipedProto,
			PositionId = 16,
			Alias = "slowswim",
			FirstPersonVerb = "swim slowly",
			ThirdPersonVerb = "swims slowly",
			PresentParticiple = "slowly swimming",
			Multiplier = 2,
			StaminaMultiplier = 1.0
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = pinnipedProto,
			PositionId = 16,
			Alias = "quickswim",
			FirstPersonVerb = "swim quickly",
			ThirdPersonVerb = "swims quickly",
			PresentParticiple = "swimming quickly",
			Multiplier = 1.0,
			StaminaMultiplier = 2
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = pinnipedProto,
			PositionId = 18,
			Alias = "fly",
			FirstPersonVerb = "fly",
			ThirdPersonVerb = "flies",
			PresentParticiple = "flying",
			Multiplier = 1.8,
			StaminaMultiplier = 15
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = pinnipedProto,
			PositionId = 18,
			Alias = "franticfly",
			FirstPersonVerb = "frantically fly",
			ThirdPersonVerb = "frantically flies",
			PresentParticiple = "frantically flying",
			Multiplier = 1.4,
			StaminaMultiplier = 25
		});

		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = crabProto,
			PositionId = 1,
			Alias = "walk",
			FirstPersonVerb = "walk",
			ThirdPersonVerb = "walks",
			PresentParticiple = "walking",
			Multiplier = 1.5,
			StaminaMultiplier = 1.0
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = crabProto,
			PositionId = 1,
			Alias = "skitter",
			FirstPersonVerb = "skitter",
			ThirdPersonVerb = "skitters",
			PresentParticiple = "skittering",
			Multiplier = 1.0,
			StaminaMultiplier = 2.25
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = crabProto,
			PositionId = 6,
			Alias = "crawl",
			FirstPersonVerb = "crawl",
			ThirdPersonVerb = "crawls",
			PresentParticiple = "crawling",
			Multiplier = 5,
			StaminaMultiplier = 3.0
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = crabProto,
			PositionId = 15,
			Alias = "climb",
			FirstPersonVerb = "climb",
			ThirdPersonVerb = "climbs",
			PresentParticiple = "climbing",
			Multiplier = 3,
			StaminaMultiplier = 3
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = crabProto,
			PositionId = 16,
			Alias = "swim",
			FirstPersonVerb = "swim",
			ThirdPersonVerb = "swims",
			PresentParticiple = "swimming",
			Multiplier = 1.5,
			StaminaMultiplier = 1.5
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = crabProto,
			PositionId = 16,
			Alias = "slowswim",
			FirstPersonVerb = "swim slowly",
			ThirdPersonVerb = "swims slowly",
			PresentParticiple = "slowly swimming",
			Multiplier = 2,
			StaminaMultiplier = 1.0
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = crabProto,
			PositionId = 16,
			Alias = "quickswim",
			FirstPersonVerb = "swim quickly",
			ThirdPersonVerb = "swims quickly",
			PresentParticiple = "swimming quickly",
			Multiplier = 1.0,
			StaminaMultiplier = 2
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = crabProto,
			PositionId = 18,
			Alias = "fly",
			FirstPersonVerb = "fly",
			ThirdPersonVerb = "flies",
			PresentParticiple = "flying",
			Multiplier = 1.8,
			StaminaMultiplier = 15
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = crabProto,
			PositionId = 18,
			Alias = "franticfly",
			FirstPersonVerb = "frantically fly",
			ThirdPersonVerb = "frantically flies",
			PresentParticiple = "frantically flying",
			Multiplier = 1.4,
			StaminaMultiplier = 25
		});

		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = octopusProto,
			PositionId = 1,
			Alias = "walk",
			FirstPersonVerb = "walk",
			ThirdPersonVerb = "walks",
			PresentParticiple = "walking",
			Multiplier = 1.5,
			StaminaMultiplier = 1.0
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = octopusProto,
			PositionId = 1,
			Alias = "run",
			FirstPersonVerb = "run",
			ThirdPersonVerb = "runs",
			PresentParticiple = "running",
			Multiplier = 1.0,
			StaminaMultiplier = 2.25
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = octopusProto,
			PositionId = 6,
			Alias = "crawl",
			FirstPersonVerb = "crawl",
			ThirdPersonVerb = "crawls",
			PresentParticiple = "crawling",
			Multiplier = 5,
			StaminaMultiplier = 3.0
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = octopusProto,
			PositionId = 15,
			Alias = "climb",
			FirstPersonVerb = "climb",
			ThirdPersonVerb = "climbs",
			PresentParticiple = "climbing",
			Multiplier = 3,
			StaminaMultiplier = 3
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = octopusProto,
			PositionId = 16,
			Alias = "swim",
			FirstPersonVerb = "swim",
			ThirdPersonVerb = "swims",
			PresentParticiple = "swimming",
			Multiplier = 1.5,
			StaminaMultiplier = 1.5
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = octopusProto,
			PositionId = 16,
			Alias = "slowswim",
			FirstPersonVerb = "swim slowly",
			ThirdPersonVerb = "swims slowly",
			PresentParticiple = "slowly swimming",
			Multiplier = 2,
			StaminaMultiplier = 1.0
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = octopusProto,
			PositionId = 16,
			Alias = "quickswim",
			FirstPersonVerb = "swim quickly",
			ThirdPersonVerb = "swims quickly",
			PresentParticiple = "swimming quickly",
			Multiplier = 1.0,
			StaminaMultiplier = 2
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = octopusProto,
			PositionId = 18,
			Alias = "fly",
			FirstPersonVerb = "fly",
			ThirdPersonVerb = "flies",
			PresentParticiple = "flying",
			Multiplier = 1.8,
			StaminaMultiplier = 15
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = octopusProto,
			PositionId = 18,
			Alias = "franticfly",
			FirstPersonVerb = "frantically fly",
			ThirdPersonVerb = "frantically flies",
			PresentParticiple = "frantically flying",
			Multiplier = 1.4,
			StaminaMultiplier = 25
		});

		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = cetaceanProto,
			PositionId = 6,
			Alias = "flop",
			FirstPersonVerb = "flop",
			ThirdPersonVerb = "flops",
			PresentParticiple = "flopping",
			Multiplier = 6,
			StaminaMultiplier = 3.0
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = cetaceanProto,
			PositionId = 16,
			Alias = "swim",
			FirstPersonVerb = "swim",
			ThirdPersonVerb = "swims",
			PresentParticiple = "swimming",
			Multiplier = 1.5,
			StaminaMultiplier = 2
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = cetaceanProto,
			PositionId = 16,
			Alias = "slowswim",
			FirstPersonVerb = "swim slowly",
			ThirdPersonVerb = "swims slowly",
			PresentParticiple = "swimming slowly",
			Multiplier = 2,
			StaminaMultiplier = 1.5
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++,
			BodyProto = cetaceanProto,
			PositionId = 16,
			Alias = "quickswim",
			FirstPersonVerb = "swim quickly",
			ThirdPersonVerb = "swims quickly",
			PresentParticiple = "swimming quickly",
			Multiplier = 1.0,
			StaminaMultiplier = 2
		});
		_context.SaveChanges();

		#endregion
	}

	private void SetupPositions(BodyProto quadrupedBody, BodyProto avianBody, BodyProto serpentBody,
			BodyProto fishProto, BodyProto crabProto, BodyProto octopusProto, BodyProto jellyfishProto,
			BodyProto pinnipedProto, BodyProto cetaceanProto, BodyProto wormProto, BodyProto insectBody,
			BodyProto wingedInsectBody)
	{
		Console.WriteLine($"[{_stopwatch.Elapsed.TotalSeconds:N1}s] Setting up Positions");
		_context.BodyProtosPositions.Add(new BodyProtosPositions
		{ BodyProto = quadrupedBody, Position = 1 }); // Standing
		_context.BodyProtosPositions.Add(new BodyProtosPositions
		{ BodyProto = quadrupedBody, Position = 2 }); // Sitting
		_context.BodyProtosPositions.Add(new BodyProtosPositions
		{ BodyProto = quadrupedBody, Position = 3 }); // Kneeling
		_context.BodyProtosPositions.Add(new BodyProtosPositions
		{ BodyProto = quadrupedBody, Position = 4 }); // Lounging
		_context.BodyProtosPositions.Add(new BodyProtosPositions
		{ BodyProto = quadrupedBody, Position = 5 }); // Lying Down
		_context.BodyProtosPositions.Add(new BodyProtosPositions
		{ BodyProto = quadrupedBody, Position = 8 }); // Sprawled
		_context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = quadrupedBody, Position = 6 }); // Prone
		_context.BodyProtosPositions.Add(new BodyProtosPositions
		{ BodyProto = quadrupedBody, Position = 7 }); // Prostrate
		_context.BodyProtosPositions.Add(new BodyProtosPositions
		{ BodyProto = quadrupedBody, Position = 11 }); // Leaning
		_context.BodyProtosPositions.Add(new BodyProtosPositions
		{ BodyProto = quadrupedBody, Position = 18 }); // Flying
		_context.BodyProtosPositions.Add(new BodyProtosPositions
		{ BodyProto = quadrupedBody, Position = 16 }); // Swimming
		_context.BodyProtosPositions.Add(new BodyProtosPositions
		{ BodyProto = quadrupedBody, Position = 15 }); // Climbing

		_context.BodyProtosPositions.Add(new BodyProtosPositions
		{ BodyProto = pinnipedProto, Position = 1 }); // Standing
		_context.BodyProtosPositions.Add(new BodyProtosPositions
		{ BodyProto = pinnipedProto, Position = 2 }); // Sitting
		_context.BodyProtosPositions.Add(new BodyProtosPositions
		{ BodyProto = pinnipedProto, Position = 4 }); // Lounging
		_context.BodyProtosPositions.Add(new BodyProtosPositions
		{ BodyProto = pinnipedProto, Position = 5 }); // Lying Down
		_context.BodyProtosPositions.Add(new BodyProtosPositions
		{ BodyProto = pinnipedProto, Position = 8 }); // Sprawled
		_context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = pinnipedProto, Position = 6 }); // Prone
		_context.BodyProtosPositions.Add(new BodyProtosPositions
		{ BodyProto = pinnipedProto, Position = 11 }); // Leaning
		_context.BodyProtosPositions.Add(new BodyProtosPositions
		{ BodyProto = pinnipedProto, Position = 18 }); // Flying
		_context.BodyProtosPositions.Add(new BodyProtosPositions
		{ BodyProto = pinnipedProto, Position = 16 }); // Swimming
		_context.BodyProtosPositions.Add(new BodyProtosPositions
		{ BodyProto = pinnipedProto, Position = 15 }); // Climbing

		_context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = crabProto, Position = 1 }); // Standing
		_context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = crabProto, Position = 2 }); // Sitting
		_context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = crabProto, Position = 3 }); // Kneeling
		_context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = crabProto, Position = 5 }); // Lying Down
		_context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = crabProto, Position = 8 }); // Sprawled
		_context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = crabProto, Position = 6 }); // Prone
		_context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = crabProto, Position = 7 }); // Prostrate
		_context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = crabProto, Position = 18 }); // Flying
		_context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = crabProto, Position = 16 }); // Swimming
		_context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = crabProto, Position = 15 }); // Climbing

		_context.BodyProtosPositions.Add(new BodyProtosPositions
		{ BodyProto = octopusProto, Position = 1 }); // Standing
		_context.BodyProtosPositions.Add(new BodyProtosPositions
		{ BodyProto = octopusProto, Position = 5 }); // Lying Down
		_context.BodyProtosPositions.Add(new BodyProtosPositions
		{ BodyProto = octopusProto, Position = 8 }); // Sprawled
		_context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = octopusProto, Position = 6 }); // Prone
		_context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = octopusProto, Position = 18 }); // Flying
		_context.BodyProtosPositions.Add(new BodyProtosPositions
		{ BodyProto = octopusProto, Position = 16 }); // Swimming
		_context.BodyProtosPositions.Add(new BodyProtosPositions
		{ BodyProto = octopusProto, Position = 15 }); // Climbing

		_context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = avianBody, Position = 1 }); // Standing
		_context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = avianBody, Position = 2 }); // Sitting
		_context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = avianBody, Position = 5 }); // Lying Down
		_context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = avianBody, Position = 8 }); // Sprawled
		_context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = avianBody, Position = 6 }); // Prone
		_context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = avianBody, Position = 18 }); // Flying
		_context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = avianBody, Position = 16 }); // Swimming
		_context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = avianBody, Position = 15 }); // Climbing

		_context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = serpentBody, Position = 8 }); // Sprawled
		_context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = serpentBody, Position = 6 }); // Prone
		_context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = serpentBody, Position = 18 }); // Flying
		_context.BodyProtosPositions.Add(new BodyProtosPositions
		{ BodyProto = serpentBody, Position = 16 }); // Swimming
		_context.BodyProtosPositions.Add(new BodyProtosPositions
		{ BodyProto = serpentBody, Position = 15 }); // Climbing

		_context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = wormProto, Position = 8 }); // Sprawled
		_context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = wormProto, Position = 6 }); // Prone
		_context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = wormProto, Position = 18 }); // Flying
		_context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = wormProto, Position = 16 }); // Swimming
		_context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = wormProto, Position = 15 }); // Climbing

		_context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = insectBody, Position = 1 }); // Standing
		_context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = insectBody, Position = 8 }); // Sprawled
		_context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = insectBody, Position = 6 }); // Prone
		_context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = insectBody, Position = 18 }); // Flying
		_context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = insectBody, Position = 15 }); // Climbing

		_context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = wingedInsectBody, Position = 1 }); // Standing
		_context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = wingedInsectBody, Position = 8 }); // Sprawled
		_context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = wingedInsectBody, Position = 6 }); // Prone
		_context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = wingedInsectBody, Position = 18 }); // Flying
		_context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = wingedInsectBody, Position = 15 }); // Climbing

		_context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = fishProto, Position = 8 }); // Sprawled
		_context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = fishProto, Position = 6 }); // Prone
		_context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = fishProto, Position = 18 }); // Flying
		_context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = fishProto, Position = 16 }); // Swimming
		_context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = fishProto, Position = 15 }); // Climbing

		_context.BodyProtosPositions.Add(new BodyProtosPositions
		{ BodyProto = cetaceanProto, Position = 8 }); // Sprawled
		_context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = cetaceanProto, Position = 6 }); // Prone
		_context.BodyProtosPositions.Add(new BodyProtosPositions
		{ BodyProto = cetaceanProto, Position = 18 }); // Flying
		_context.BodyProtosPositions.Add(new BodyProtosPositions
		{ BodyProto = cetaceanProto, Position = 16 }); // Swimming
		_context.BodyProtosPositions.Add(new BodyProtosPositions
		{ BodyProto = cetaceanProto, Position = 15 }); // Climbing

		_context.BodyProtosPositions.Add(new BodyProtosPositions
		{ BodyProto = jellyfishProto, Position = 8 }); // Sprawled
		_context.BodyProtosPositions.Add(new BodyProtosPositions
		{ BodyProto = jellyfishProto, Position = 18 }); // Flying
		_context.BodyProtosPositions.Add(new BodyProtosPositions
		{ BodyProto = jellyfishProto, Position = 16 }); // Swimming
		_context.SaveChanges();
	}

	private void ApplyDefaultCombatSettingsToSeededRaces()
	{
		foreach (var template in RaceTemplates.Values)
		{
			var race = _context.Races.FirstOrDefault(x => x.Name == template.Name);
			if (race is null)
			{
				continue;
			}

			var setting = CombatStrategySeederHelper.EnsureCombatStrategy(_context, template.CombatStrategyKey);
			if (race.DefaultCombatSettingId == setting.Id)
			{
				continue;
			}

			race.DefaultCombatSetting = setting;
		}

		_context.SaveChanges();
	}

	#endregion
}
