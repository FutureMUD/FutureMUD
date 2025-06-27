using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Combat;
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
		=> new List<(string Id, string Question,
			Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
			Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)>
		{
			("model",
				@"#DHealth Models#F

Which health model should animals use by default? This can be overriden for individual NPCs (so you can make HP-based mooks even if you use the full medical system). Even if you use the full medical model for humans, you may not want to use it for animals by default - but then again you may.

The valid choices are as follows:

#Bhp#F	- this system will use hitpoints or destruction of the brain only to determine death.
#Bhpplus#F	- this system uses hp and brain destruction, but also enables heart and organ damage.
#Bfull#F	- this system uses the full medical model, where the only way to die is via death of the brain.

Your choice: ",
				(context, answers) => true,
				(text, context) =>
				{
					switch (text.ToLowerInvariant())
					{
						case "hp":
						case "hpplus":
						case "full":
							return (true, string.Empty);
					}

					return (false, "That is not a valid selection.");
				}
			),

			("random",
				@"#DDamage Formulas#F

You can configure your damage formulas to be consistent or random. The engine already takes into account a number of variables such as relative success of attacker and defender, type of defense used, all of which ensure that the damage is mitigated differently each attack. However, a good hit is usually pretty impactful in that kind of setup.

Randomness in damage is sometimes used to add complexity or choice to weapon types when the outcome of the attack is fairly likely (see D20 systems where before long hitting is almost guaranteed). This can work too but it can be disappointing for someone to land a good blow with all the factors right and then simply do little damage because of RNG, whereas another just-barely hit might do full damage.

There are three options that you can choose for randomness:

#BStatic:#F In this option (which was used in LabMUD) base damage is static. A hit with the same quality weapon, the same strength and the same attack/defense result will lead to the same damage
#BPartial#F: In this option 30% of the damage will be random - this adds a little bit of uncertainty and variety but still makes hits /largely/ a function of relative success
#BRandom#F: In this option damage can be 20-100% of the maximum. This means outcomes will vary wildly.

Which option do you want to use for random results in your animal damage formulas? ", (context, answers) => true,
				(answer, context) =>
				{
					return (answer.EqualToAny("static", "partial", "random"),
						"You must answer static, partial or random.");
				}),

			("messagestyle",
				@"#DCombat Messages#F

Combat messages can be presented in a number of different styles. Fundamentally, the attack and the defense against the attack are different messages. You can either have them come together to form a single sentence, or you can keep them separate sentences, or you can put them on entirely different lines. For example, here are the three options you could consider:

#BCompact#F

	A tall, bearded man swings a steel longsword at a pudgy, brown-haired codger, who tries to dodge but gets hit on the head!

#BSentences#F

	A tall, bearded man swings a steel longsword at a pudgy, brown-haired codger. He tries to dodge out of the way but is unsuccessful. He is hit on the head.

#BSparse#F

	A tall, bearded man swings a steel longsword at a pudgy, brown-haired codger.
	He tries to dodge out of the way but is unsuccessful.
	He is hit on the head!

You can change your decision later, you're just going to have to go and edit your combat messages (mostly the defenses) to match the style you want. One advantage to doing Sentences or Sparse is that you can easily colour whole elements if you prefer (some people prefer not to of course).

You can choose #3Compact#f, #3Sentences#f or #3Sparse#f: ", (context, answers) => true,
				(answer, context) =>
				{
					return (answer.EqualToAny("compact", "sentences", "sparse"),
						"You must answer Compact, Sentences or Sparse.");
				})
		};

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
		_questionAnswers = questionAnswers;
		_sever = questionAnswers.ContainsKey("sever") && questionAnswers["sever"].ToLowerInvariant().In("yes", "y");

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

		var strategy = SetupHealthModel(context, questionAnswers, hpExpression, hpTick, secondaryTrait);

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
			Description = "This is a culture for wild animals.",
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

                SeedAquatic(fishBody, crabBody, cephalopod, jellyfish, pinniped, cetacean);

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

                SetupPositions(quadrupedBody, avianBody, serpentineBody, fishBody, crabBody, cephalopod, jellyfish, pinniped,
                        cetacean, wormBody, insectBody, wingedInsectBody);
                SetupSpeeds(quadrupedBody, avianBody, serpentineBody, fishBody, crabBody, cephalopod, jellyfish, pinniped,
                        cetacean, wormBody, insectBody, wingedInsectBody);

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
				CaloriesPerLitre = 800,
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
				CaloriesPerLitre = 0,
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
		HealthStrategy strategy;
		switch (questionAnswers["model"].ToLowerInvariant())
		{
			case "hp":
				strategy = new HealthStrategy
				{
					Name = "Non-Human HP",
					Type = "BrainHitpoints",
					Definition =
						$"<Definition> <MaximumHitPointsExpression>{hpExpression.Id}</MaximumHitPointsExpression> <HealingTickDamageExpression>{hpTick.Id}</HealingTickDamageExpression><PercentageHealthPerPenalty>0.2</PercentageHealthPerPenalty><LodgeDamageExpression>max(0, damage-15)</LodgeDamageExpression> <SeverityRanges> <Severity value=\"0\" lower=\"-1\" upper=\"0\"/> <Severity value=\"1\" lower=\"0\" upper=\"2\"/> <Severity value=\"2\" lower=\"2\" upper=\"4\"/> <Severity value=\"3\" lower=\"4\" upper=\"7\"/> <Severity value=\"4\" lower=\"7\" upper=\"12\"/> <Severity value=\"5\" lower=\"12\" upper=\"18\"/> <Severity value=\"6\" lower=\"18\" upper=\"27\"/> <Severity value=\"7\" lower=\"27\" upper=\"40\"/> <Severity value=\"8\" lower=\"40\" upper=\"100\"/> </SeverityRanges><CheckHeart>false</CheckHeart> <UseHypoxiaDamage>false</UseHypoxiaDamage><KnockoutOnCritical>true</KnockoutOnCritical><KnockoutDuration>240</KnockoutDuration></Definition>"
				};
				break;
			case "hpplus":
				strategy = new HealthStrategy
				{
					Name = "Non-Human HP",
					Type = "BrainHitpoints",
					Definition =
						$"<Definition> <MaximumHitPointsExpression>{hpExpression.Id}</MaximumHitPointsExpression> <HealingTickDamageExpression>{hpTick.Id}</HealingTickDamageExpression><PercentageHealthPerPenalty>0.2</PercentageHealthPerPenalty><LodgeDamageExpression>max(0,damage-15)</LodgeDamageExpression> <SeverityRanges> <Severity value=\"0\" lower=\"-1\" upper=\"0\"/> <Severity value=\"1\" lower=\"0\" upper=\"2\"/> <Severity value=\"2\" lower=\"2\" upper=\"4\"/> <Severity value=\"3\" lower=\"4\" upper=\"7\"/> <Severity value=\"4\" lower=\"7\" upper=\"12\"/> <Severity value=\"5\" lower=\"12\" upper=\"18\"/> <Severity value=\"6\" lower=\"18\" upper=\"27\"/> <Severity value=\"7\" lower=\"27\" upper=\"40\"/> <Severity value=\"8\" lower=\"40\" upper=\"100\"/> </SeverityRanges> <CheckHeart>true</CheckHeart> <UseHypoxiaDamage>true</UseHypoxiaDamage><KnockoutOnCritical>true</KnockoutOnCritical><KnockoutDuration>240</KnockoutDuration> </Definition>"
				};
				break;
			case "full":

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
				strategy = new HealthStrategy
				{
					Name = "Non-Human Health Model",
					Type = "ComplexLiving",
					Definition =
						$"<Definition> <MaximumHitPointsExpression>{hpExpression.Id}</MaximumHitPointsExpression> <MaximumStunExpression>{stunExpression.Id}</MaximumStunExpression> <MaximumPainExpression>{painExpression.Id}</MaximumPainExpression> <HealingTickDamageExpression>{hpTick.Id}</HealingTickDamageExpression> <HealingTickStunExpression>{stunTick.Id}</HealingTickStunExpression> <HealingTickPainExpression>{painTick.Id}</HealingTickPainExpression> <LodgeDamageExpression>max(0, damage - 30)</LodgeDamageExpression> <PercentageHealthPerPenalty>0.2</PercentageHealthPerPenalty> <PercentageStunPerPenalty>0.2</PercentageStunPerPenalty> <PercentagePainPerPenalty>0.2</PercentagePainPerPenalty> <SeverityRanges> <Severity value=\"0\" lower=\"-2\" upper=\"-1\" lowerpec=\"-100\" upperperc=\"0\"/> <Severity value=\"1\" lower=\"-1\" upper=\"2\" lowerpec=\"0\" upperperc=\"0.4\"/> <Severity value=\"2\" lower=\"2\" upper=\"4\" lowerpec=\"0.4\" upperperc=\"0.55\"/> <Severity value=\"3\" lower=\"4\" upper=\"7\" lowerpec=\"0.55\" upperperc=\"0.65\"/> <Severity value=\"4\" lower=\"7\" upper=\"12\" lowerpec=\"0.65\" upperperc=\"0.75\"/> <Severity value=\"5\" lower=\"12\" upper=\"18\" lowerpec=\"0.75\" upperperc=\"0.85\"/> <Severity value=\"6\" lower=\"18\" upper=\"27\" lowerpec=\"0.85\" upperperc=\"0.9\"/> <Severity value=\"7\" lower=\"27\" upper=\"40\" lowerpec=\"0.9\" upperperc=\"0.95\"/> <Severity value=\"8\" lower=\"40\" upper=\"100\" lowerpec=\"0.95\" upperperc=\"100\"/> </SeverityRanges> </Definition>"
				};
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		context.HealthStrategies.Add(strategy);
		_healthStrategy = strategy;
		_context.SaveChanges();
		return strategy;
	}

	private void SetupHeightWeightModels()
	{
		Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Height Weight Models...");
		AddHWModel("Big Cat Baby", 30, 1.5, 111.111111111111, 11.1111111111111);
		AddHWModel("Big Cat Female", 60, 3, 125, 12.5);
		AddHWModel("Big Cat Juvenile", 40, 2, 175, 17.5);
		AddHWModel("Big Cat Male", 65, 3.25, 142.011834319527, 14.2011834319527);
		AddHWModel("Cat", 35, 1.75, 81.6326530612245, 8.16326530612245);
		AddHWModel("Cow Baby", 80, 4, 121.875, 12.1875);
		AddHWModel("Cow Female", 180, 9, 154.320987654321, 15.4320987654321);
		AddHWModel("Cow Juvenile", 130, 6.5, 118.343195266272, 11.8343195266272);
		AddHWModel("Cow Male", 200, 10, 212.5, 21.25);
		AddHWModel("Deer Baby", 30, 1.5, 111.111111111111, 11.1111111111111);
		AddHWModel("Deer Female", 65, 3.25, 118.343195266272, 11.8343195266272);
		AddHWModel("Deer Juvenile", 45, 2.25, 148.148148148148, 14.8148148148148);
		AddHWModel("Deer Male", 80, 4, 234.375, 23.4375);
		AddHWModel("Dog", 65, 3.25, 71.0059171597633, 7.10059171597633);
		AddHWModel("Wolf", 65, 3.25, 71.0059171597633, 7.10059171597633);
		AddHWModel("Elephant Baby", 150, 7.5, 311.111111111111, 31.1111111111111);
		AddHWModel("Elephant Female", 310, 15.5, 322.58064516129, 32.258064516129);
		AddHWModel("Elephant Juvenile", 220, 11, 247.933884297521, 24.7933884297521);
		AddHWModel("Elephant Male", 330, 16.5, 550.964187327824, 55.0964187327824);
		AddHWModel("Hippo Baby", 90, 4.5, 555.555555555556, 55.5555555555556);
		AddHWModel("Hippo Female", 150, 7.5, 577.777777777778, 57.7777777777778);
		AddHWModel("Hippo Juvenile", 120, 6, 541.666666666667, 54.1666666666667);
		AddHWModel("Hippo Male", 160, 8, 585.9375, 58.59375);
		AddHWModel("Huge Bird", 150, 7.5, 8.88888888888889, 0.888888888888889);
		AddHWModel("Large Bird", 90, 4.5, 11.1111111111111, 1.11111111111111);
		AddHWModel("Lion Baby", 40, 2, 62.5, 6.25);
		AddHWModel("Lion Female", 110, 5.5, 123.96694214876, 12.396694214876);
		AddHWModel("Lion Juvenile", 80, 4, 109.375, 10.9375);
		AddHWModel("Lion Male", 120, 6, 131.944444444444, 13.1944444444444);
		AddHWModel("Large Mammal", 120, 6, 131.944444444444, 13.1944444444444);
		AddHWModel("Tiger Baby", 40, 2, 62.5, 6.25);
		AddHWModel("Tiger Female", 110, 5.5, 123.96694214876, 12.396694214876);
		AddHWModel("Tiger Juvenile", 80, 4, 109.375, 10.9375);
		AddHWModel("Tiger Male", 120, 6, 131.944444444444, 13.1944444444444);
		AddHWModel("Moose Baby", 100, 5, 100, 10);
		AddHWModel("Moose Female", 200, 10, 125, 12.5);
		AddHWModel("Moose Juvenile", 170, 8.5, 103.806228373702, 10.3806228373702);
		AddHWModel("Moose Male", 230, 11.5, 151.228733459357, 15.1228733459357);
		AddHWModel("Pig Baby", 30, 1.5, 22.2222222222222, 2.22222222222222);
		AddHWModel("Pig Female", 50, 2.5, 720, 72);
		AddHWModel("Pig Juvenile", 45, 2.25, 592.592592592593, 59.2592592592593);
		AddHWModel("Pig Male", 70, 3.5, 448.979591836735, 44.8979591836735);
		AddHWModel("Sheep Baby", 30, 1.5, 22.2222222222222, 2.22222222222222);
		AddHWModel("Sheep Female", 50, 2.5, 720, 72);
		AddHWModel("Sheep Juvenile", 45, 2.25, 592.592592592593, 59.2592592592593);
		AddHWModel("Sheep Male", 70, 3.5, 448.979591836735, 44.8979591836735);
		AddHWModel("Goat Baby", 20, 1.5, 22.2222222222222, 2.22222222222222);
		AddHWModel("Goat Female", 40, 2.5, 720, 72);
		AddHWModel("Goat Juvenile", 30, 2.25, 592.592592592593, 59.2592592592593);
		AddHWModel("Goat Male", 45, 3.5, 448.979591836735, 44.8979591836735);
		AddHWModel("Rhino Baby", 100, 5, 500, 50);
		AddHWModel("Rhino Female", 180, 9, 493.827160493827, 49.3827160493827);
		AddHWModel("Rhino Juvenile", 130, 6.5, 591.715976331361, 59.1715976331361);
		AddHWModel("Rhino Male", 200, 10, 600, 60);
		AddHWModel("Huge Mammal", 200, 10, 600, 60);
		AddHWModel("Small Bird", 20, 1, 20, 2);
		AddHWModel("Small Mammal", 30, 1.5, 13.8888888888889, 1.38888888888889);
		AddHWModel("Small Mammal Baby", 5, 0.25, 40, 4);
		AddHWModel("Tiny Bird", 10, 0.5, 20, 2);
		AddHWModel("Tiny Mammal", 10, 0.5, 12, 1.2);
		AddHWModel("Tiny Mammal Baby", 2, 0.1, 87.5, 8.75);
		AddHWModel("Bear Male", 180, 9, 84.8765432098765, 8.48765432098766);
		AddHWModel("Bear Female", 150, 7.5, 66.6666666666667, 6.66666666666667);
		AddHWModel("Bear Juvenile", 130, 6.5, 71.0059171597633, 7.10059171597633);
		AddHWModel("Bear Baby", 80, 4, 78.125, 7.8125);
		AddHWModel("Fowl", 25, 1.25, 48, 4.8);
		AddHWModel("Medium Bird", 50, 2.5, 20, 2);
		AddHWModel("Serpent", 50, 2.5, 20, 2);
		AddHWModel("Horse Male", 180, 9, 154.320987654321, 15.4320987654321);
		AddHWModel("Horse Female", 160, 8, 167.96875, 16.796875);
		AddHWModel("Horse Juvenile", 120, 6, 208.333333333333, 20.8333333333333);
		AddHWModel("Horse Baby", 80, 4, 156.25, 15.625);
		AddHWModel("Warhorse Male", 210, 10.5, 181.40589569161, 18.140589569161);
		AddHWModel("Warhorse Female", 185, 9.25, 219.138056975895, 21.9138056975895);
		AddHWModel("Warhorse Juvenile", 140, 7, 255.102040816327, 25.5102040816327);
		AddHWModel("Warhorse Baby", 100, 5, 200, 20);
		AddHWModel("Medium Mammal", 100, 5, 75, 7.5);
		AddHWModel("Medium Mammal Baby", 30, 1.5, 111.111111111111, 11.1111111111111);
		AddHWModel("Tiny Fish", 5, 0.25, 20, 2);
		AddHWModel("Small Fish", 15, 0.75, 13.3333333333333, 1.33333333333333);
		AddHWModel("Medium Fish", 30, 1.5, 8.33333333333333, 0.833333333333333);
		AddHWModel("Large Fish", 100, 5, 3, 0.3);
		AddHWModel("Huge Fish", 200, 10, 37.5, 3.75);
		AddHWModel("Shark", 450, 22.5, 111.111111111111, 11.1111111111111);
		AddHWModel("Small Whale", 1000, 50, 40, 4);
		AddHWModel("Dolphin", 250, 12.5, 28, 2.8);
		AddHWModel("Large Whale", 3000, 150, 200, 20);
		AddHWModel("Octopus", 50, 2.5, 4, 0.4);
		AddHWModel("Squid", 30, 1.5, 5.55555555555556, 0.555555555555556);
		AddHWModel("Giant Squid", 300, 15, 14.4444444444444, 1.44444444444444);
		AddHWModel("Small Crab", 10, 0.5, 30, 3);
		AddHWModel("Medium Crab", 30, 1.5, 20, 2);
		AddHWModel("Large Crab", 50, 2.5, 20, 2);
		AddHWModel("Lobster", 30, 1.5, 13.3333333333333, 1.33333333333333);
		AddHWModel("Shrimp", 10, 0.5, 5, 0.5);
		AddHWModel("Small Jellyfish", 10, 0.5, 5, 0.5);
		AddHWModel("Medium Jellyfish", 30, 1.5, 4.44444444444444, 0.444444444444444);
		AddHWModel("Large Jellyfish", 100, 5, 1.2, 0.12);
		AddHWModel("Seal Male", 180, 9, 92.5925925925926, 0);
		AddHWModel("Seal Female", 120, 6, 69.4444444444444, 0);
		AddHWModel("Seal Juvenile", 100, 5, 80, 0);
		AddHWModel("Seal Baby", 40, 2, 125, 0);
		AddHWModel("Walrus Male", 300, 15, 144.444444444444, 0);
		AddHWModel("Walrus Female", 200, 10, 125, 0);
                AddHWModel("Walrus Juvenile", 150, 7.5, 133.333333333333, 0);
                AddHWModel("Walrus Baby", 80, 4, 156.25, 0);
                AddHWModel("Small Insect", 5, 0.25, 20, 2);
                AddHWModel("Medium Insect", 15, 0.75, 18, 1.8);
                AddHWModel("Large Insect", 30, 1.5, 17, 1.7);
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
			Message = attackMessage,
			Priority = 50,
			Verb = (int)verb,
			Chance = 1.0,
			FailureMessage = attackMessage
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
		var attackAddendum = "";
		switch (_questionAnswers["messagestyle"].ToLowerInvariant())
		{
			case "sentences":
			case "sparse":
				attackAddendum = ".";
				break;
		}

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
			CaloriesPerLitre = 800,
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
			$"@ attempt|attempts to brush a tendril against $1{attackAddendum}", DamageType.Cellular,
			additionalInfo: @$"<Data>
   <Liquid>{liquid.Id}</Liquid>
   <MaximumQuantity>0.1</MaximumQuantity>
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
		var attackAddendum = "";
		switch (_questionAnswers["messagestyle"].ToLowerInvariant())
		{
			case "sentences":
			case "sparse":
				attackAddendum = ".";
				break;
		}

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
				CaloriesPerLitre = 800,
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
				$"@ lunge|lunges forward and try|tries to bite $1{attackAddendum}", DamageType.Bite,
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
				$"@ lunge|lunges forward and try|tries to bite $1{attackAddendum}", DamageType.Bite);
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

			var attackAddendum = "";
			switch (_questionAnswers["messagestyle"].ToLowerInvariant())
			{
				case "sentences":
				case "sparse":
					attackAddendum = ".";
					break;
			}

			_attacks["carnivorebite"] = AddAttack("Carnivore Bite", BuiltInCombatMoveType.NaturalWeaponAttack,
				MeleeWeaponVerb.Bite, Difficulty.Normal, Difficulty.Hard, Difficulty.Easy, Difficulty.Easy,
				Alignment.Front, Orientation.Centre, 4.0, 1.0, mouthshape, carnivoreBite,
				$"@ lunge|lunges forward and try|tries to bite $1{attackAddendum}", DamageType.Bite);
			_attacks["carnivoresmashbite"] = AddAttack("Carnivore Smash Bite",
				BuiltInCombatMoveType.UnarmedSmashItem, MeleeWeaponVerb.Bite, Difficulty.Normal, Difficulty.Hard,
				Difficulty.Easy, Difficulty.Easy, Alignment.Front, Orientation.Centre, 4.0, 1.0, mouthshape,
				carnivoreBite, $"@ lunge|lunges forward and try|tries to bite $1{attackAddendum}", DamageType.Bite);
			_attacks["carnivorelowbite"] = AddAttack("Carnivore Low Bite",
				BuiltInCombatMoveType.NaturalWeaponAttack, MeleeWeaponVerb.Bite, Difficulty.Normal, Difficulty.Hard,
				Difficulty.Easy, Difficulty.VeryEasy, Alignment.Front, Orientation.Low, 4.0, 1.0, mouthshape,
				carnivoreBite, $"@ lunge|lunges forward and low, and try|tries to bite $1{attackAddendum}",
				DamageType.Bite);
			_attacks["carnivorelowestbite"] = AddAttack("Carnivore Lowest Bite",
				BuiltInCombatMoveType.NaturalWeaponAttack, MeleeWeaponVerb.Bite, Difficulty.Hard, Difficulty.Hard,
				Difficulty.VeryEasy, Difficulty.Easy, Alignment.Front, Orientation.Lowest, 4.0, 0.9, mouthshape,
				carnivoreBite,
				$"@ lunge|lunges forward and very low, trying to bite $1's legs and trip &1 up{attackAddendum}",
				DamageType.Bite);
			_attacks["carnivorehighbite"] = AddAttack("Carnivore High Bite",
				BuiltInCombatMoveType.StaggeringBlowUnarmed, MeleeWeaponVerb.Bite, Difficulty.Normal,
				Difficulty.Normal, Difficulty.Hard, Difficulty.Easy, Alignment.Front, Orientation.High, 6.0, 1.3,
				mouthshape, carnivoreBite,
				$"@ leap|leaps forward and throw|throws &0's bulk towards $1, while try|trying to bite &1{attackAddendum}",
				DamageType.Bite, additionalInfo: "7");
			_attacks["carnivoreclinchbite"] = AddAttack("Carnivore Clinch Bite",
				BuiltInCombatMoveType.ClinchUnarmedAttack, MeleeWeaponVerb.Bite, Difficulty.VeryEasy,
				Difficulty.Hard, Difficulty.Easy, Difficulty.Easy, Alignment.Front, Orientation.Centre, 3.0, 0.6,
				mouthshape, carnivoreBite, $"@ try|tries to savagely bite $1{attackAddendum}", DamageType.Bite);
			_attacks["carnivoreclinchhighbite"] = AddAttack("Carnivore High Clinch Bite",
				BuiltInCombatMoveType.ClinchUnarmedAttack, MeleeWeaponVerb.Bite, Difficulty.VeryEasy,
				Difficulty.Hard, Difficulty.Easy, Difficulty.Easy, Alignment.Front, Orientation.High, 3.0, 0.6,
				mouthshape, carnivoreBite, $"@ try|tries to savagely bite $1{attackAddendum}", DamageType.Bite);
			_attacks["carnivoreclinchhighestbite"] = AddAttack("Carnivore Highest Clinch Bite",
				BuiltInCombatMoveType.ClinchUnarmedAttack, MeleeWeaponVerb.Bite, Difficulty.VeryEasy,
				Difficulty.Hard, Difficulty.Easy, Difficulty.Easy, Alignment.Front, Orientation.Highest, 3.0, 0.6,
				mouthshape, carnivoreBite, $"@ try|tries to savagely bite $1{attackAddendum}", DamageType.Bite);
			_attacks["carnivoredownbite"] = AddAttack("Carnivore Downed Bite",
				BuiltInCombatMoveType.DownedAttackUnarmed, MeleeWeaponVerb.Bite, Difficulty.VeryEasy,
				Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Alignment.Front, Orientation.High, 2.0, 0.5,
				mouthshape, carnivoreBite,
				$"@ try|tries to savagely bite $1 while #1 %1|are|is down{attackAddendum}", DamageType.Bite,
				additionalInfo: ((int)Difficulty.ExtremelyEasy).ToString());

			_attacks["herbivorebite"] = AddAttack("Herbivore Bite", BuiltInCombatMoveType.ClinchUnarmedAttack,
				MeleeWeaponVerb.Bite, Difficulty.VeryEasy, Difficulty.Hard, Difficulty.Easy, Difficulty.Easy,
				Alignment.Front, Orientation.Centre, 3.0, 1.0, mouthshape, herbivoreBite,
				$"@ try|tries to bite $1{attackAddendum}", DamageType.Bite);
			_attacks["herbivoresmashbite"] = AddAttack("Herbivore Smash Bite",
				BuiltInCombatMoveType.UnarmedSmashItem, MeleeWeaponVerb.Bite, Difficulty.VeryEasy, Difficulty.Hard,
				Difficulty.Easy, Difficulty.Easy, Alignment.Front, Orientation.Centre, 3.0, 1.0, mouthshape,
				herbivoreBite, $"@ try|tries to bite $1{attackAddendum}", DamageType.Bite);

			_attacks["smallbite"] = AddAttack("Small Animal Bite", BuiltInCombatMoveType.ClinchUnarmedAttack,
				MeleeWeaponVerb.Bite, Difficulty.VeryEasy, Difficulty.Normal, Difficulty.Easy, Difficulty.Easy,
				Alignment.Front, Orientation.Low, 2.0, 0.5, mouthshape, smallBite,
				$"@ try|tries to bite $1{attackAddendum}", DamageType.Bite);
			_attacks["smallsmashbite"] = AddAttack("Small Animal Smash Bite",
				BuiltInCombatMoveType.UnarmedSmashItem, MeleeWeaponVerb.Bite, Difficulty.VeryEasy,
				Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Alignment.Front, Orientation.Low, 2.0, 0.5,
				mouthshape, smallBite, $"@ try|tries to bite $1{attackAddendum}", DamageType.Bite);
			_attacks["smalllowbite"] = AddAttack("Small Animal Low Bite", BuiltInCombatMoveType.ClinchUnarmedAttack,
				MeleeWeaponVerb.Bite, Difficulty.VeryEasy, Difficulty.Normal, Difficulty.Easy, Difficulty.Easy,
				Alignment.Front, Orientation.Lowest, 2.0, 0.5, mouthshape, smallBite,
				$"@ try|tries to bite $1{attackAddendum}", DamageType.Bite);
			_attacks["smalldownedbite"] = AddAttack("Small Animal Downed Bite",
				BuiltInCombatMoveType.DownedAttackUnarmed, MeleeWeaponVerb.Bite, Difficulty.VeryEasy,
				Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Alignment.Front, Orientation.High, 2.0, 0.5,
				mouthshape, smallBite, $"@ try|tries to bite $1 while #1 %1|are|is down{attackAddendum}",
				DamageType.Bite, additionalInfo: ((int)Difficulty.Trivial).ToString());

			_attacks["clawswipe"] = AddAttack("Claw Swipe", BuiltInCombatMoveType.NaturalWeaponAttack,
				MeleeWeaponVerb.Swipe, Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.Easy,
				Alignment.FrontRight, Orientation.High, 4.0, 1.3, clawShape, clawDamage,
				$"@ rear|rears up and swipe|swipes &0's {{0}} at $1{attackAddendum}", DamageType.Claw);
			_attacks["clawsmashswipe"] = AddAttack("Claw Swipe Smash", BuiltInCombatMoveType.UnarmedSmashItem,
				MeleeWeaponVerb.Swipe, Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.Easy,
				Alignment.FrontRight, Orientation.High, 4.0, 1.3, clawShape, clawDamage,
				$"@ rear|rears up and swipe|swipes &0's {{0}} at $1{attackAddendum}", DamageType.Claw);
			_attacks["clawhighswipe"] = AddAttack("Claw High Swipe", BuiltInCombatMoveType.NaturalWeaponAttack,
				MeleeWeaponVerb.Swipe, Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.Easy,
				Alignment.FrontRight, Orientation.Centre, 4.0, 1.3, clawShape, clawDamage,
				$"@ rear|rears up and swipe|swipes &0's {{0}} at $1{attackAddendum}", DamageType.Claw);
			_attacks["clawlowswipe"] = AddAttack("Claw Low Swipe", BuiltInCombatMoveType.NaturalWeaponAttack,
				MeleeWeaponVerb.Swipe, Difficulty.Normal, Difficulty.VeryEasy, Difficulty.Easy, Difficulty.Easy,
				Alignment.FrontRight, Orientation.Highest, 4.0, 1.3, clawShape, clawDamage,
				$"@ rear|rears up and swipe|swipes &0's {{0}} at $1{attackAddendum}", DamageType.Claw);

			_attacks["hoofstomp"] = AddAttack("Hoof Stomp", BuiltInCombatMoveType.DownedAttackUnarmed,
				MeleeWeaponVerb.Kick, Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.Easy,
				Alignment.Front, Orientation.High, 5.0, 1.3, hoofShape, smashDamage,
				$"@ frantically stomp|stomps on $1 while #1 %1|are|is down{attackAddendum}", DamageType.Crushing,
				additionalInfo: ((int)Difficulty.Hard).ToString());
			_attacks["hoofstompsmash"] = AddAttack("Hoof Stomp Smash", BuiltInCombatMoveType.UnarmedSmashItem,
				MeleeWeaponVerb.Kick, Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.Easy,
				Alignment.Front, Orientation.High, 5.0, 1.3, hoofShape, smashDamage,
				$"@ frantically stomp|stomps on $1 while #1 %1|are|is down{attackAddendum}", DamageType.Crushing,
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
				$"@ thrash|thrashes about and throw|throws &0's bulk at $1{attackAddendum}", DamageType.Crushing,
				additionalInfo: ((int)Difficulty.Hard).ToString());

			_attacks["gorehorn"] = AddAttack("Horn Gore", BuiltInCombatMoveType.NaturalWeaponAttack,
				MeleeWeaponVerb.Stab, Difficulty.Easy, Difficulty.Easy, Difficulty.Hard, Difficulty.Easy,
				Alignment.Front, Orientation.High, 5.0, 1.3, hornShape, smashDamage,
				$"@ lower|lowers &0's head and lunge|lunges forward to try to gore $1 with &0's horns{attackAddendum}",
				DamageType.Piercing);
			_attacks["goreantler"] = AddAttack("Antler Gore", BuiltInCombatMoveType.NaturalWeaponAttack,
				MeleeWeaponVerb.Stab, Difficulty.Easy, Difficulty.Easy, Difficulty.Hard, Difficulty.Easy,
				Alignment.Front, Orientation.High, 5.0, 1.3, antlerShape, smashDamage,
				$"@ lower|lowers &0's head and lunge|lunges forward to try to gore $1 with &0's antlers{attackAddendum}",
				DamageType.Piercing);
			_attacks["goretusk"] = AddAttack("Tusk Gore", BuiltInCombatMoveType.NaturalWeaponAttack,
				MeleeWeaponVerb.Stab, Difficulty.Easy, Difficulty.Easy, Difficulty.Hard, Difficulty.Easy,
				Alignment.Front, Orientation.High, 5.0, 1.3, tuskShape, smashDamage,
				$"@ lower|lowers &0's head and lunge|lunges forward to try to gore $1 with &0's tusks{attackAddendum}",
				DamageType.Piercing);
			_attacks["tusksweep"] = AddAttack("Tusk Sweep", BuiltInCombatMoveType.UnbalancingBlowUnarmed,
				MeleeWeaponVerb.Sweep, Difficulty.Hard, Difficulty.Easy, Difficulty.ExtremelyHard, Difficulty.Hard,
				Alignment.FrontRight, Orientation.Low, 5.0, 1.5, tuskShape, smashDamage,
				$"@ swing|swings &0's head from one side to the other and try|tries to trip up $1 with &0's tusks{attackAddendum}",
				DamageType.Crushing, additionalInfo: ((int)Difficulty.Hard).ToString());

			_attacks["fishbite"] = AddAttack("Fish Bite", BuiltInCombatMoveType.ClinchUnarmedAttack,
				MeleeWeaponVerb.Bite, Difficulty.VeryEasy, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard,
				Alignment.Front, Orientation.Low, 2.0, 0.5, mouthshape, fishBite,
				$"@ try|tries to bite $1{attackAddendum}", DamageType.Bite);
			_attacks["fishquickbite"] = AddAttack("Fish Quick Bite", BuiltInCombatMoveType.ClinchUnarmedAttack,
				MeleeWeaponVerb.Bite, Difficulty.Normal, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard,
				Alignment.Front, Orientation.Low, 2.0, 0.3, mouthshape, fishBite,
				$"@ quickly try|tries to bite $1{attackAddendum}", DamageType.Bite);
			_attacks["sharkbite"] = AddAttack("Shark Bite", BuiltInCombatMoveType.NaturalWeaponAttack,
				MeleeWeaponVerb.Bite, Difficulty.Normal, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard,
				Alignment.Front, Orientation.Centre, 4.0, 1.0, mouthshape, sharkBite,
				$"@ lunge|lunges forward and try|tries to bite $1{attackAddendum}", DamageType.Bite);
			_attacks["sharkreelbite"] = AddAttack("Shark Reel Bite", BuiltInCombatMoveType.StaggeringBlowUnarmed,
				MeleeWeaponVerb.Bite, Difficulty.Normal, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard,
				Alignment.Front, Orientation.Centre, 5.0, 1.2, mouthshape, sharkBite,
				$"@ lunge|lunges forward and try|tries to bite $1{attackAddendum}", DamageType.Bite,
				additionalInfo: ((int)Difficulty.VeryHard).ToString());

			_attacks["crabpinch"] = AddAttack("Crab Pinch", BuiltInCombatMoveType.NaturalWeaponAttack,
				MeleeWeaponVerb.Swipe, Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.Easy,
				Alignment.FrontRight, Orientation.Low, 4.0, 1.3, clawShape, clawDamage,
				$"@ lash|lashes out and try|tries to pinch $1 with &0's {{0}}{attackAddendum}", DamageType.Shearing);
		}
	}

	private void CreateRaceAttacks(Race race)
	{
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
				FutureProg = prog, ParameterIndex = 0, ParameterName = "ch",
				ParameterType = (long)ProgVariableTypes.Toon
			});
			_context.FutureProgs.Add(prog);

			CreateDescription(EntityDescriptionType.FullDescription, description, prog);
		}

		AddDogDescription(_context.Ethnicities.First(x => x.Name == "Terrier"),
			"This is &a_an[&age], &male terrier; a small, wiry and generally fearless breed of dog with short legs and an eager nature. They are excellent diggers and have a natural instinct to hunt vermin.");
		AddDogDescription(_context.Ethnicities.First(x => x.Name == "Setter"),
			"This is &a_an[&age], &male setter; a medium sized hunting dog that was bred to hunt birds. They hold their head high in the air as they move rather than tracking along the ground and they have an instinct to freeze or 'set' when they see their prey. They are readily trainable and very disciplined.");
		AddDogDescription(_context.Ethnicities.First(x => x.Name == "Pointer"),
			"This is &a_an[&age], &male pointer; a medium sized hunting dog that was bred to assist hunters using ranged weapons. They stalk prey in dense vegetation and upon sighting it will stand still in a 'point' with their muzzle at their prey.");
		AddDogDescription(_context.Ethnicities.First(x => x.Name == "Retriever"),
			"This is &a_an[&age], &male retriever; a medium sized hunting dog that was bred to bring back downed prey. They are loyal and friendly in disposition and relatively soft-mouthed, so make excellent pets.");
		AddDogDescription(_context.Ethnicities.First(x => x.Name == "Spaniel"),
			"This is &a_an[&age], &male spaniel; a small hunting dog bred for flushing game out of dense brush. They have distinctive long, silky coats and big droopy ears.");
		AddDogDescription(_context.Ethnicities.First(x => x.Name == "Water Dog"),
			"This is &a_an[&age], &male water dog; hunting and companion dogs that are excellent swimmers. They have long and thick hair around their torso to keep them warm in icy-cold water but short-coated limbs to reduce drag while swimming.");
		AddDogDescription(_context.Ethnicities.First(x => x.Name == "Sighthound"),
			"This is &a_an[&age], &male sighthound; a large, long-legged and lanky breed of dog with tremendous speed, flexibility and agility. They have long muzzles and flexible backs.");
		AddDogDescription(_context.Ethnicities.First(x => x.Name == "Scenthound"),
			"This is &a_an[&age], &male scenthound; a dog with a phenomenal sense of smell, even for their species. These dogs are short-legged and low to the ground, and have big, floppy ears and wet mouths.");
		AddDogDescription(_context.Ethnicities.First(x => x.Name == "Bulldog"),
			"This is &a_an[&age], &male bulldog; a stocky, square-bodied dog originally bred for fighting. They have flat faces and tremendous muscle.");
		AddDogDescription(_context.Ethnicities.First(x => x.Name == "Mastiff"),
			"This is &a_an[&age], &male mastiff; a large dog originally bred for guarding homes and for war. They have broad, somewhat flat faces and big feet.");
		AddDogDescription(_context.Ethnicities.First(x => x.Name == "Herding Dog"),
			"This is &a_an[&age], &male herding dog; a medium sized dog bred to assist in the herding of livestock. They tend to be low to the ground and agile, and have an instinct to nip at the heels of animals and herd small children.");
		AddDogDescription(_context.Ethnicities.First(x => x.Name == "Lap Dog"),
			"This is &a_an[&age], &male lap dog; a small, lap-sized dog with little use as a working dog. Purely ornamental, they tend to have a lazy but grumpy disposition and prefer to spend their time in the laps of their owners.");
		AddDogDescription(_context.Ethnicities.First(x => x.Name == "Mongrel"),
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
				break;
			case "Hyena":
				DoLazyDescriptions("hyena pup", "hyena whelp", "male hyena", "female hyena");
				break;
			case "Rabbit":
				DoLazyDescriptions("bunny", "young rabbit", "buck rabbit", "doe rabbit");
				break;
			case "Hare":
				DoLazyDescriptions("hare bunny", "young hare", "buck hare", "doe hare");
				break;
			case "Beaver":
				DoLazyDescriptions("beaver cub", "young beaver", "male beaver", "female beaver");
				break;
			case "Otter":
				DoLazyDescriptions("otter cub", "young otter", "male otter", "female otter");
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
				break;
			case "Cheetah":
				DoLazyDescriptions("cheetah cub", "juvenile cheetah", "cheetah", "she-cheetah");
				break;
			case "Leopard":
				DoLazyDescriptionsWithMultipleJuvenile("leopard cub", "juvenile leopard", "juvenile leopardess",
					"leopard", "leopardess");
				break;
			case "Tiger":
				DoLazyDescriptionsWithMultipleJuvenile("tiger cub", "juvenile tiger", "juvenile tigress", "tiger",
					"tigress");
				break;
			case "Panther":
				DoLazyDescriptions("panther cub", "juvenile panther", "male panther", "female panther");
				break;
			case "Jaguar":
				DoLazyDescriptions("jaguar cub", "juvenile jaguar", "male jaguar", "female jaguar");
				break;
			case "Jackal":
				DoLazyDescriptions("jackal cub", "juvenile jackal", "male jackal", "female jackal");
				break;
			case "Deer":
				DoLazyDescriptionsWithMultipleJuvenile("fawn", "juvenile stag", "juvenile doe", "stag", "doe");
				break;
			case "Moose":
				DoLazyDescriptionsWithMultipleJuvenile("moose calf", "young bull moose", "moose heifer",
					"bull moose", "moose cow");
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
				break;
			case "Warthog":
				DoLazyDescriptionsWithMultipleJuvenile("warthog piglet", "young warthog boar", "young warthog sow",
					"warthog boar", "warthog sow");
				break;
			case "Cow":
				DoLazyDescriptionsWithMultipleJuvenile("calf", "young bull", "heifer", "bull", "cow");
				break;
			case "Ox":
				DoLazyDescriptionsWithMultipleJuvenile("ox calf", "young ox bull", "ox heifer", "ox bull",
					"ox cow");
				break;
			case "Buffalo":
				DoLazyDescriptionsWithMultipleJuvenile("buffalo calf", "young buffalo bull", "buffalo heifer",
					"buffalo bull", "buffalo cow");
				break;
			case "Bison":
				DoLazyDescriptionsWithMultipleJuvenile("bison calf", "young bison bull", "bison heifer",
					"bison bull", "bison cow");
				break;
			case "Hippopotamus":
				DoLazyDescriptionsWithMultipleJuvenile("hippo calf", "young hippo bull", "young hippo cow",
					"hippo bull", "hippo cow");
				break;
			case "Horse":
				DoLazyDescriptionsWithMultipleJuvenile("foal", "colt", "filly", "stallion", "mare");
				break;
			case "Bear":
				DoLazyDescriptions("&ethnicity cub", "young &ethnicity", "male &ethnicity", "female &ethnicity");
				break;
			case "Rhinocerous":
				DoLazyDescriptions("rhino calf", "young rhino", "rhino bull", "rhino cow");
				break;
			case "Giraffe":
				DoLazyDescriptions("giraffe calf", "young giraffe", "male giraffe", "female giraffe");
				break;
			case "Elephant":
				DoLazyDescriptionsWithMultipleJuvenile("elephant calf", "juvenile bull elephant",
					"juvenile elephant", "bull elephant", "elephant");
				break;
			case "Fox":
				DoLazyDescriptionsWithMultipleJuvenile("fox kit", "juvenile male fox", "juvenile fox vixen",
					"male fox", "fox vixen");
				break;
			case "Rat":
				DoLazyDescriptions("rat pup", "young rat", "buck rat", "doe rat");
				break;
			case "Mouse":
				DoLazyDescriptions("mouse pup", "mouse rat", "buck mouse", "doe mouse");
				break;
			case "Hamster":
				DoLazyDescriptions("hamster pup", "young hamster", "buck hamster", "doe hamster");
				break;
			case "Ferret":
				DoLazyDescriptions("ferret kit", "young ferret", "jack ferret", "jill ferret");
				break;
			case "Weasel":
				DoLazyDescriptions("weasel kit", "young weasel", "jack weasel", "jill weasel");
				break;
			case "Guinea Pig":
				DoLazyDescriptions("guinea pig pup", "young guinea pig", "guinea pig boar", "guinea pig sow");
				break;
			case "Badger":
				DoLazyDescriptions("badger kit", "young badger", "boar badger", "sow badger");
				break;
			case "Wolverine":
				DoLazyDescriptions("wolverine kit", "young wolverine", "boar wolverine", "sow wolverine");
				break;
			case "Goat":
				DoLazyDescriptionsWithMultipleJuvenile("kid goat", "buckling goat",
					"doeling goat", "billy goat", "nanny goat");
				break;
			case "Llama":
				DoLazyDescriptions("llama cria", "young llama", "llama macho", "llama hembra");
				break;
			case "Alpaca":
				DoLazyDescriptions("alpaca cria", "young alpaca", "alpaca macho", "alpaca hembra");
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
				break;

			case "Duck":
				DoLazyDescriptionsWithMultipleJuvenile("duckling", "fledgling drake", "fledgling duck", "drake",
					"duck");
				break;
			case "Goose":
				DoLazyDescriptionsWithMultipleJuvenile("gosling", "fledgling gander", "fledgling goose", "gander",
					"goose");
				break;
			case "Swan":
				DoLazyDescriptionsWithMultipleJuvenile("cygnet", "fledgling swan cob", "fledgling swan pen",
					"swan cob", "swan pen");
				break;
			case "Chicken":
				DoLazyDescriptionsWithMultipleJuvenile("chicklet", "fledgling rooster", "fledgling hen", "rooster",
					"hen");
				break;
			case "Turkey":
				DoLazyDescriptionsWithMultipleJuvenile("poult", "fledgling turkey gobbler", "fledgling turkey hen",
					"turkey gobbler", "turkey hen");
				break;
			case "Hawk":
				DoLazyDescriptions("hawk chick", "fledgling hawk", "male hawk", "female hawk");
				break;
			case "Eagle":
				DoLazyDescriptions("eaglet", "fledgling eagle", "male eagle", "female eagle");
				break;
			case "Falcon":
				DoLazyDescriptions("falcon chick", "fledgling falcon", "male falcon", "female falcon");
				break;
			case "Owl":
				DoLazyDescriptions("owlet", "fledgling owl", "male owl", "female owl");
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
				_context.Ethnicities.Add(new Ethnicity
				{
					ParentRace = race,
					Name = "Terrier",
					ChargenBlurb =
						"Terriers are a small, wiry and generally fearless breed of dog originally bred to hunt vermin. They are excellent diggers and have an eager nature.",
					EthnicGroup = "Terrier",
					TolerableTemperatureFloorEffect = 0,
					TolerableTemperatureCeilingEffect = 0,
					PopulationBloodModel = model
				});
				_context.Ethnicities.Add(new Ethnicity
				{
					ParentRace = race,
					Name = "Setter",
					ChargenBlurb =
						"Setters are a type of hunting dog originally bred to assist with the hunting of birds. They tend to be very intelligent and disciplined dogs. They hold their head high in the air rather than following ground scents and will pause and 'set' themselves in position upon encountering their prey, indicating to their hunter master that they are ready to flush out the game.",
					EthnicGroup = "Setter",
					TolerableTemperatureFloorEffect = 0,
					TolerableTemperatureCeilingEffect = 0,
					PopulationBloodModel = model
				});
				_context.Ethnicities.Add(new Ethnicity
				{
					ParentRace = race,
					Name = "Pointer",
					ChargenBlurb =
						"Pointers are a type of hunting dog bred to alert their owner to the precise position of their prey. They have a powerful instinct to 'point' at prey by freezing and pointing their muzzle in the direction of their quarry. This allows the hunter to shoot at a target they might not otherwise be able to see.",
					EthnicGroup = "Pointer",
					TolerableTemperatureFloorEffect = 0,
					TolerableTemperatureCeilingEffect = 0,
					PopulationBloodModel = model
				});
				_context.Ethnicities.Add(new Ethnicity
				{
					ParentRace = race,
					Name = "Retriever",
					ChargenBlurb =
						"Retrievers are a type of hunting dog that were bred to return wounded or dead game to their master without damaging it. They are generally larger, with a soft mouth and a loyal disposition that aims to please. Due to their good nature and the ease of training them they are also perennially popular pets.",
					EthnicGroup = "Retriever",
					TolerableTemperatureFloorEffect = 0,
					TolerableTemperatureCeilingEffect = 0,
					PopulationBloodModel = model
				});
				_context.Ethnicities.Add(new Ethnicity
				{
					ParentRace = race,
					Name = "Spaniel",
					ChargenBlurb =
						"Spaniels are a type of hunting dog originally bred for flushing out game from dense brush. They are known for their distinctive long silky coats and big droopy ears. The smaller members of the type are often used as lapdogs, where their qualities such as intelligence, affection and alertness are highly sought after.",
					EthnicGroup = "Spaniel",
					TolerableTemperatureFloorEffect = 0,
					TolerableTemperatureCeilingEffect = 0,
					PopulationBloodModel = model
				});
				_context.Ethnicities.Add(new Ethnicity
				{
					ParentRace = race,
					Name = "Water Dog",
					ChargenBlurb =
						"Water dogs are a type of hunting dog with a strong desire to swim. They are often kept as companions aboard boats where they are used to retrieve items that fall overboard, but they are also used as retrievers for the hunting of water fowl and even for retrieving missed arrows. Some breeds of water dog are ornamental and bred as companions, such as the poodle.",
					EthnicGroup = "Water Dog",
					TolerableTemperatureFloorEffect = 0,
					TolerableTemperatureCeilingEffect = 0,
					PopulationBloodModel = model
				});
				_context.Ethnicities.Add(new Ethnicity
				{
					ParentRace = race,
					Name = "Sighthound",
					ChargenBlurb =
						"Sighthounds are a type of hound bred for speed, visual acuity and endurance. They are typically long-legged with long, narrow snouts. They have an extremely flexible back and are very agile. ",
					EthnicGroup = "Hound",
					TolerableTemperatureFloorEffect = 0,
					TolerableTemperatureCeilingEffect = 0,
					PopulationBloodModel = model
				});
				_context.Ethnicities.Add(new Ethnicity
				{
					ParentRace = race,
					Name = "Scenthound",
					ChargenBlurb =
						"Scenthounds are a type of hound bred for their almost unparallelled ability to track prey by smell. Typically low to the ground and with large, droopy ears, wet lips and a deep, loud bark. They can be taught to recognise a variety of scents.",
					EthnicGroup = "Hound",
					TolerableTemperatureFloorEffect = 0,
					TolerableTemperatureCeilingEffect = 0,
					PopulationBloodModel = model
				});
				_context.Ethnicities.Add(new Ethnicity
				{
					ParentRace = race,
					Name = "Bulldog",
					ChargenBlurb =
						"Bulldogs are a type of dog originally bred for fighting, with a short, stocky build and a brachycephalic muzzle. They are typically very loyal to their masters but mistrustful of strangers and therefore make good guard dogs.",
					EthnicGroup = "Bulldog",
					TolerableTemperatureFloorEffect = 0,
					TolerableTemperatureCeilingEffect = 0,
					PopulationBloodModel = model
				});
				_context.Ethnicities.Add(new Ethnicity
				{
					ParentRace = race,
					Name = "Mastiff",
					ChargenBlurb =
						"Mastiffs are a type of dog historically kept as guard dogs and occasionally dogs of war. They tend to be among the larger breeds of dogs, with large feet and broad, flat faces and short, pendant-shaped ears.",
					EthnicGroup = "Mastiff",
					TolerableTemperatureFloorEffect = 0,
					TolerableTemperatureCeilingEffect = 0,
					PopulationBloodModel = model
				});
				_context.Ethnicities.Add(new Ethnicity
				{
					ParentRace = race,
					Name = "Herding Dog",
					ChargenBlurb =
						"Herding dogs are a type of dogs bred to herd livestock. They typically have been bred to preserve some of their predatory instincts such as nipping at the heels of larger animals, but are also trained for obedience and intelligence. They are excellent companion dogs and are readily trainable.",
					EthnicGroup = "Herding Dog",
					TolerableTemperatureFloorEffect = 0,
					TolerableTemperatureCeilingEffect = 0,
					PopulationBloodModel = model
				});
				_context.Ethnicities.Add(new Ethnicity
				{
					ParentRace = race,
					Name = "Lap Dog",
					ChargenBlurb =
						"Lap dogs are a type of dogs that cuts across various other traditional types where the dogs have been bred to be small and inclined to sit in the laps of their owners. These breeds typically show a high degree of neoteny (meaning they retain many puppy-like features and behaviours into adulthood) and also tend to be short-muzzled and big-eyed.",
					EthnicGroup = "Lap Dog",
					TolerableTemperatureFloorEffect = 0,
					TolerableTemperatureCeilingEffect = 0,
					PopulationBloodModel = model
				});
				_context.Ethnicities.Add(new Ethnicity
				{
					ParentRace = race,
					Name = "Mongrel",
					ChargenBlurb =
						"A mongrel is a dog that has been bred from a mixture of unrelated purebreds that does not otherwise fall into a recognised crossbred type. They may show some distinct features of one breed or they may be very hard to identify. Typically they are healthier and longer lived than other dog breeds though they may lack some of the trainability and focused usefulness of the purebreeds.",
					EthnicGroup = "Mongrel",
					TolerableTemperatureFloorEffect = 0,
					TolerableTemperatureCeilingEffect = 0,
					PopulationBloodModel = model
				});
				break;
			case "Bear":
				_context.Ethnicities.Add(new Ethnicity
				{
					ParentRace = race,
					Name = "Black Bear",
					ChargenBlurb =
						"Black Bears are a small, omnivorous species of bear. They are typically cautious in nature compared to some of their cousins.",
					EthnicGroup = "Black Bear",
					TolerableTemperatureFloorEffect = 0,
					TolerableTemperatureCeilingEffect = 0,
					PopulationBloodModel = model
				});
				_context.Ethnicities.Add(new Ethnicity
				{
					ParentRace = race,
					Name = "Moon Bear",
					ChargenBlurb =
						"Moon bears are a medium-sized black bear with a distinctive crescent-moon shaped white patch of fur on their chest. They tend to live in arboreal environments.",
					EthnicGroup = "Moon Bear",
					TolerableTemperatureFloorEffect = 0,
					TolerableTemperatureCeilingEffect = 0,
					PopulationBloodModel = model
				});
				_context.Ethnicities.Add(new Ethnicity
				{
					ParentRace = race,
					Name = "Brown Bear",
					ChargenBlurb =
						"Brown bears, also sometimes called Grizzly bears, are large and somewhat territorial bears. They are the largest terrestrial carnivores by mass.",
					EthnicGroup = "Brown Bear",
					TolerableTemperatureFloorEffect = -10,
					TolerableTemperatureCeilingEffect = 0,
					PopulationBloodModel = model
				});
				_context.Ethnicities.Add(new Ethnicity
				{
					ParentRace = race,
					Name = "Polar Bear",
					ChargenBlurb =
						"Polar bears are a white-coated species of bear adapted to live in arctic environments. They have a surprising tolerance to cold temperatures, are almost exclusively carnivorous, and are extraordinarily dangerous.",
					EthnicGroup = "Polar Bear",
					TolerableTemperatureFloorEffect = -30,
					TolerableTemperatureCeilingEffect = -10,
					PopulationBloodModel = model
				});
				break;
			default:
				_context.Ethnicities.Add(new Ethnicity
				{
					ParentRace = race,
					Name = race.Name,
					ChargenBlurb = $"The ordinary, common form of {race.Name.A_An()}.",
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
		int childage, youthage, youngadultage, adultage, elderage, venerableage;
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

			case "Dog":
			case "Beaver":
			case "Otter":
			case "Wolf":
			case "Fox":
			case "Coyote":
			case "Hyena":
			case "Cat":
			case "Lion":
			case "Cheetah":
			case "Leopard":
			case "Tiger":
			case "Panther":
			case "Deer":
			case "Jaguar":
			case "Jackal":
			case "Wolverine":
			case "Badger":
			case "Goat":
			case "Python":
			case "Tree Python":
			case "Boa":
			case "Anaconda":
			case "Cobra":
			case "Adder":
			case "Rattlesnake":
			case "Viper":
			case "Mamba":
			case "Coral Snake":
			case "Moccasin":
				childage = 1;
				youthage = 2;
				youngadultage = 3;
				adultage = 7;
				elderage = 12;
				venerableage = 15;
				break;
			case "Pig":
			case "Boar":
			case "Warthog":
			case "Sheep":
			case "Cow":
			case "Ox":
			case "Buffalo":
			case "Bison":
			case "Hippopotamus":
			case "Moose":
			case "Llama":
			case "Alpaca":
				childage = 1;
				youthage = 3;
				youngadultage = 5;
				adultage = 9;
				elderage = 14;
				venerableage = 20;
				break;
			case "Horse":
			case "Bear":
			case "Rhinocerous":
			case "Giraffe":
				childage = 1;
				youthage = 3;
				youngadultage = 5;
				adultage = 9;
				elderage = 16;
				venerableage = 25;
				break;

			case "Duck":
			case "Goose":
			case "Swan":
			case "Grouse":
			case "Parrot":
			case "Pheasant":
			case "Turkey":
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
			case "Hawk":
			case "Eagle":
			case "Falcon":
			case "Woodpecker":
			case "Owl":
			case "Kingfisher":
			case "Stork":
			case "Penguin":
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
			case "Shark":
			case "Small Crab":
			case "Crab":
			case "Giant Crab":
			case "Lobster":
			case "Jellyfish":
			case "Octopus":
			case "Squid":
			case "Giant Squid":
			case "Sea Lion":
			case "Seal":
			case "Walrus":

				childage = 1;
				youthage = 3;
				youngadultage = 5;
				adultage = 9;
				elderage = 20;
				venerableage = 35;
				break;
			case "Elephant":
				childage = 2;
				youthage = 5;
				youngadultage = 8;
				adultage = 12;
				elderage = 35;
				venerableage = 50;
				break;

			case "Dolphin":
			case "Porpoise":
			case "Orca":
			case "Baleen Whale":
			case "Toothed Whale":
				childage = 2;
				youthage = 5;
				youngadultage = 8;
				adultage = 15;
				elderage = 45;
				venerableage = 75;
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
			CaloriesPerLitre = 800,
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
				CaloriesPerLitre = 0,
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
		switch (name)
		{
			case "Horse":
				(bloodModel, populationModel) = SetupBloodModel("Equine", new List<string>
				{
					"Equine A-Antigen",
					"Equine C-Antigen",
					"Equine Q-Antigen"
				}, new List<(string Name, IEnumerable<string> Antigens, double weight)>
				{
					("Equine ACQ", new List<string> { "Equine A-Antigen", "Equine C-Antigen", "Equine Q-Antigen" },
						10),
					("Equine AC", new List<string> { "Equine A-Antigen", "Equine C-Antigen" }, 10),
					("Equine A", new List<string> { "Equine A-Antigen" }, 10),
					("Equine CQ", new List<string> { "Equine C-Antigen", "Equine Q-Antigen" }, 10),
					("Equine Q", new List<string> { "Equine Q-Antigen" }, 10),
					("Equine C", new List<string> { "Equine C-Antigen" }, 10)
				});
				break;
			case "Cat":
				(bloodModel, populationModel) = SetupBloodModel("Feline",
					new List<string> { "Feline A-Antigen", "Feline B-Antigen" },
					new List<(string Name, IEnumerable<string> Antigens, double weight)>
					{
						("Feline A", new List<string> { "Feline A-Antigen" }, 87),
						("Feline B", new List<string> { "Feline B-Antigen" }, 10),
						("Feline AB", new List<string> { "Feline A-Antigen", "Feline B-Antigen" }, 3)
					});
				break;
			case "Dog":
			case "Wolf":
				(bloodModel, populationModel) = SetupBloodModel("Canine", new List<string> { "DEA 1.1" },
					new List<(string Name, IEnumerable<string> Antigens, double weight)>
					{
						("Canine DEA 1.1 Positive", new List<string> { "DEA 1.1" }, 40),
						("Canine DEA 1.1 Negative", new List<string>(), 60)
					});
				break;
			case "Cow":
				(bloodModel, populationModel) = SetupBloodModel("Bovine", new List<string> { "Bovine B", "Bovine J" },
					new List<(string Name, IEnumerable<string> Antigens, double weight)>
					{
						("Bovine BJ", new List<string> { "Bovine B", "Bovine J" }, 20),
						("Bovine B", new List<string> { "Bovine B" }, 40),
						("Bovine J", new List<string> { "Bovine J" }, 40)
					});
				break;
			case "Sheep":
				(bloodModel, populationModel) = SetupBloodModel("Ovine", new List<string> { "Ovine B", "Ovine R" },
					new List<(string Name, IEnumerable<string> Antigens, double weight)>
					{
						("Ovine BR", new List<string> { "Ovine B", "Ovine R" }, 20),
						("Ovine B", new List<string> { "Ovine B" }, 40),
						("Ovine R", new List<string> { "Ovine R" }, 40)
					});
				break;
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
			case "Rhino":
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

		_context.SaveChanges();

		foreach (var shape in _context.BodypartShapes) _cachedShapes[shape.Name] = shape;

		foreach (var material in _context.Materials) _cachedMaterials[material.Name] = material;
	}
	
	#region Different Body Types
	private void SeedAvian(BodyProto avianProto)
	{
		ResetCachedParts();
		var order = 1;
		Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Bodyparts...");

		#region Torso

		AddBodypart(avianProto, "abdomen", "abdomen", "abdomen", BodypartTypeEnum.Wear, null, Alignment.Front,
			Orientation.Low, 80, -1, 100, order++, "Fatty Flesh", SizeCategory.Normal, "Torso", true, isVital: true,
			implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(avianProto, "rbreast", "right breast", "breast", BodypartTypeEnum.BonyDrapeable, "abdomen",
			Alignment.FrontRight, Orientation.Low, 80, -1, 100, order++, "Flesh", SizeCategory.Normal, "Torso",
			true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(avianProto, "lbreast", "left breast", "breast", BodypartTypeEnum.BonyDrapeable, "abdomen",
			Alignment.FrontLeft, Orientation.Low, 80, -1, 100, order++, "Flesh", SizeCategory.Normal, "Torso", true,
			isVital: true, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(avianProto, "urflank", "upper right flank", "flank", BodypartTypeEnum.BonyDrapeable, "rbreast",
			Alignment.Right, Orientation.Centre, 80, -1, 200, order++, "Flesh", SizeCategory.Normal, "Torso", true,
			isVital: true, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(avianProto, "ulflank", "upper left flank", "flank", BodypartTypeEnum.BonyDrapeable, "lbreast",
			Alignment.Left, Orientation.Centre, 80, -1, 200, order++, "Flesh", SizeCategory.Normal, "Torso", true,
			isVital: true, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(avianProto, "lrflank", "lower right flank", "flank", BodypartTypeEnum.BonyDrapeable, "abdomen",
			Alignment.RearRight, Orientation.Centre, 80, -1, 200, order++, "Flesh", SizeCategory.Normal, "Torso",
			true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(avianProto, "llflank", "lower left flank", "flank", BodypartTypeEnum.BonyDrapeable, "abdomen",
			Alignment.RearLeft, Orientation.Centre, 80, -1, 200, order++, "Flesh", SizeCategory.Normal, "Torso",
			true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(avianProto, "belly", "belly", "belly", BodypartTypeEnum.Wear, "abdomen", Alignment.Front,
			Orientation.Low, 80, -1, 100, order++, "Fatty Flesh", SizeCategory.Normal, "Torso", true, isVital: true,
			implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(avianProto, "rshoulder", "right shoulder", "shoulder", BodypartTypeEnum.BonyDrapeable, "rbreast",
			Alignment.FrontRight, Orientation.Centre, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal,
			"Torso", true, isVital: false, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(avianProto, "lshoulder", "left shoulder", "shoulder", BodypartTypeEnum.BonyDrapeable, "lbreast",
			Alignment.FrontLeft, Orientation.Centre, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal,
			"Torso", true, isVital: false, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(avianProto, "uback", "upper back", "upper back", BodypartTypeEnum.BonyDrapeable, "abdomen",
			Alignment.Front, Orientation.High, 80, -1, 200, order++, "Bony Flesh", SizeCategory.Normal, "Torso",
			true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(avianProto, "lback", "lower back", "lower back", BodypartTypeEnum.BonyDrapeable, "abdomen",
			Alignment.Rear, Orientation.High, 80, -1, 200, order++, "Bony Flesh", SizeCategory.Normal, "Torso",
			true, isVital: true, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(avianProto, "rump", "rump", "rump", BodypartTypeEnum.Wear, "lback", Alignment.Rear,
			Orientation.Centre, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal, "Torso", true,
			isVital: false, implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(avianProto, "loin", "loin", "loin", BodypartTypeEnum.Wear, "belly", Alignment.Rear,
			Orientation.Low, 80, -1, 100, order++, "Fatty Flesh", SizeCategory.Normal, "Torso", true, isVital: true,
			implantSpace: 5, stunMultiplier: 0.2);

		#endregion

		#region Head

		AddBodypart(avianProto, "neck", "neck", "neck", BodypartTypeEnum.BonyDrapeable, "uback", Alignment.Front,
			Orientation.High, 80, 100, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true, isVital: true,
			implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(avianProto, "bneck", "neck back", "neck back", BodypartTypeEnum.BonyDrapeable, "neck",
			Alignment.Front, Orientation.Highest, 80, 100, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head",
			true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(avianProto, "throat", "throat", "throat", BodypartTypeEnum.Wear, "neck", Alignment.Front,
			Orientation.Highest, 40, 50, 100, order++, "Fatty Flesh", SizeCategory.Normal, "Head", true,
			isVital: true, implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(avianProto, "head", "head", "face", BodypartTypeEnum.BonyDrapeable, "neck", Alignment.Front,
			Orientation.Highest, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true,
			isVital: true, implantSpace: 5, stunMultiplier: 1.0);
		AddBodypart(avianProto, "bhead", "head back", "head back", BodypartTypeEnum.BonyDrapeable, "bneck",
			Alignment.Rear, Orientation.Highest, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head",
			true, isVital: true, implantSpace: 5, stunMultiplier: 1.0);
		AddBodypart(avianProto, "rcheek", "right cheek", "cheek", BodypartTypeEnum.BonyDrapeable, "head",
			Alignment.Right, Orientation.Highest, 40, -1, 100, order++, "Bony Flesh", SizeCategory.Small, "Head",
			true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(avianProto, "lcheek", "left cheek", "cheek", BodypartTypeEnum.BonyDrapeable, "head", Alignment.Left,
			Orientation.Highest, 40, -1, 100, order++, "Bony Flesh", SizeCategory.Small, "Head", true,
			isVital: true, implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(avianProto, "reyesocket", "right eye socket", "eye socket", BodypartTypeEnum.BonyDrapeable, "head",
			Alignment.FrontRight, Orientation.Highest, 80, -1, 100, order++, "Dense Bony Flesh", SizeCategory.Small,
			"Head", true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(avianProto, "leyesocket", "left eye socket", "eye socket", BodypartTypeEnum.BonyDrapeable, "head",
			Alignment.FrontLeft, Orientation.Highest, 80, -1, 100, order++, "Dense Bony Flesh", SizeCategory.Small,
			"Head", true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(avianProto, "reye", "right eye", "eye", BodypartTypeEnum.Eye, "reyesocket",
			Alignment.FrontRight, Orientation.Highest, 10, 30, 100, order++, "Dense Bony Flesh", SizeCategory.Small,
			"Head", true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(avianProto, "leye", "left eye", "eye", BodypartTypeEnum.Eye, "leyesocket", Alignment.FrontLeft,
			Orientation.Highest, 10, 30, 100, order++, "Dense Bony Flesh", SizeCategory.Small, "Head", true,
			isVital: true, implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(avianProto, "rear", "right ear", "ear", BodypartTypeEnum.Wear, "head", Alignment.Right,
			Orientation.Highest, 10, 30, 100, order++, "Flesh", SizeCategory.Small, "Head", true, isVital: false,
			implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(avianProto, "lear", "left ear", "ear", BodypartTypeEnum.Wear, "head", Alignment.Left,
			Orientation.Highest, 10, 30, 100, order++, "Flesh", SizeCategory.Small, "Head", true, isVital: false,
			implantSpace: 5, stunMultiplier: 0.2);
		AddBodypart(avianProto, "beak", "beak", "beak", BodypartTypeEnum.Mouth, "head", Alignment.Front,
			Orientation.Highest, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true,
			isVital: true, implantSpace: 5, stunMultiplier: 1.0);
		AddBodypart(avianProto, "tongue", "tongue", "tongue", BodypartTypeEnum.Tongue, "beak", Alignment.Front,
			Orientation.Highest, 10, 30, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true,
			isVital: true, implantSpace: 5, stunMultiplier: 1.0);
		AddBodypart(avianProto, "nose", "nose", "nose", BodypartTypeEnum.BonyDrapeable, "head", Alignment.Front,
			Orientation.Highest, 10, 30, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true,
			isVital: true, implantSpace: 5, stunMultiplier: 1.0);

		#endregion

		#region Legs

		AddBodypart(avianProto, "rupperleg", "right upper leg", "upper leg", BodypartTypeEnum.BonyDrapeable,
			"rshoulder", Alignment.FrontRight, Orientation.Low, 80, 100, 100, order++, "Bony Flesh",
			SizeCategory.Normal, "Right Leg");
		AddBodypart(avianProto, "lupperleg", "left upper leg", "upper leg", BodypartTypeEnum.BonyDrapeable, "lshoulder",
			Alignment.FrontLeft, Orientation.Low, 80, 100, 100, order++, "Bony Flesh", SizeCategory.Normal,
			"Left Leg");
		AddBodypart(avianProto, "rknee", "right knee", "knee", BodypartTypeEnum.BonyDrapeable, "rupperleg",
			Alignment.FrontRight, Orientation.Low, 60, 80, 30, order++, "Dense Bony Flesh", SizeCategory.Normal,
			"Right Leg");
		AddBodypart(avianProto, "lknee", "left knee", "knee", BodypartTypeEnum.BonyDrapeable, "lupperleg",
			Alignment.FrontLeft, Orientation.Low, 60, 80, 30, order++, "Dense Bony Flesh", SizeCategory.Normal,
			"Left Leg");
		AddBodypart(avianProto, "rlowerleg", "right lower leg", "lower leg", BodypartTypeEnum.BonyDrapeable, "rknee",
			Alignment.FrontRight, Orientation.Lowest, 40, 50, 100, order++, "Dense Bony Flesh", SizeCategory.Normal,
			"Right Leg");
		AddBodypart(avianProto, "llowerleg", "left lower leg", "lower leg", BodypartTypeEnum.BonyDrapeable, "lknee",
			Alignment.FrontLeft, Orientation.Lowest, 40, 50, 100, order++, "Dense Bony Flesh", SizeCategory.Normal,
			"Left Leg");
		AddBodypart(avianProto, "rankle", "right ankle", "ankle", BodypartTypeEnum.BonyDrapeable, "rlowerleg",
			Alignment.FrontRight, Orientation.Lowest, 40, 50, 50, order++, "Dense Bony Flesh", SizeCategory.Normal,
			"Right Leg");
		AddBodypart(avianProto, "lankle", "left ankle", "ankle", BodypartTypeEnum.BonyDrapeable, "llowerleg",
			Alignment.FrontLeft, Orientation.Lowest, 40, 50, 50, order++, "Dense Bony Flesh", SizeCategory.Normal,
			"Left Leg");
		AddBodypart(avianProto, "rfoot", "right foot", "foot", BodypartTypeEnum.Standing, "rankle",
			Alignment.FrontRight, Orientation.Lowest, 40, 50, 50, order++, "Bony Flesh", SizeCategory.Normal,
			"Right Leg");
		AddBodypart(avianProto, "lfoot", "left foot", "foot", BodypartTypeEnum.Standing, "lankle",
			Alignment.FrontLeft, Orientation.Lowest, 40, 50, 50, order++, "Bony Flesh", SizeCategory.Normal,
			"Left Leg");
		AddBodypart(avianProto, "rtalons", "right talons", "talon", BodypartTypeEnum.Wear, "rfoot",
			Alignment.FrontRight, Orientation.Lowest, 40, 50, 50, order++, "Dense Bony Flesh", SizeCategory.Normal,
			"Right Leg", false, isVital: false);
		AddBodypart(avianProto, "ltalons", "left talons", "talon", BodypartTypeEnum.Wear, "lfoot",
			Alignment.FrontLeft, Orientation.Lowest, 40, 50, 50, order++, "Dense Bony Flesh", SizeCategory.Normal,
			"Left Leg", false, isVital: false);

		#endregion

		#region Tail

		AddBodypart(avianProto, "tail", "tail", "tail", BodypartTypeEnum.Wear, "uback", Alignment.Rear,
			Orientation.Centre, 30, 50, 100, order++, "Flesh", SizeCategory.Normal, "Tail");

		#endregion

		#region Genitals

		AddBodypart(avianProto, "groin", "groin", "groin", BodypartTypeEnum.Wear, "loin", Alignment.Rear,
			Orientation.Low, 30, -1, 100, order++, "Fatty Flesh", SizeCategory.Small, "Genitals");

		#endregion

		#region Wings

		AddBodypart(avianProto, "rwingbase", "right wing base", "wing base", BodypartTypeEnum.BonyDrapeable, "uback",
			Alignment.FrontRight, Orientation.High, 40, -1, 100, order++, "Flesh", SizeCategory.Normal,
			"Right Wing", true, isCore: false);
		AddBodypart(avianProto, "lwingbase", "left wing base", "wing base", BodypartTypeEnum.BonyDrapeable, "uback",
			Alignment.FrontLeft, Orientation.High, 40, -1, 100, order++, "Flesh", SizeCategory.Normal, "Left Wing",
			true, isCore: false);
		AddBodypart(avianProto, "rwing", "right wing", "wing", BodypartTypeEnum.Wing, "rwingbase",
			Alignment.FrontRight, Orientation.High, 40, 50, 100, order++, "Flesh", SizeCategory.Normal,
			"Right Wing", true, isCore: false);
		AddBodypart(avianProto, "lwing", "left wing", "wing", BodypartTypeEnum.Wing, "lwingbase",
			Alignment.FrontLeft, Orientation.High, 40, 50, 100, order++, "Flesh", SizeCategory.Normal, "Left Wing",
			true, isCore: false);

		#endregion

		_context.SaveChanges();

		Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Organs...");

		#region Organs

		AddOrgan(avianProto, "brain", "brain", BodypartTypeEnum.Brain, 2.0, 50, 0.2, 0.2, 0.1, stunModifier: 1.0);
		AddOrgan(avianProto, "heart", "heart", BodypartTypeEnum.Heart, 1.0, 50, 0.2, 1.0, 1.0);
		AddOrgan(avianProto, "liver", "liver", BodypartTypeEnum.Liver, 3.0, 50, 0.2, 1.0, 0.05);
		AddOrgan(avianProto, "spleen", "spleen", BodypartTypeEnum.Spleen, 1.0, 50, 0.2, 1.0, 0.05);
		AddOrgan(avianProto, "stomach", "stomach", BodypartTypeEnum.Stomach, 1.0, 50, 0.2, 1.0, 0.05);
		AddOrgan(avianProto, "lintestines", "large intestines", BodypartTypeEnum.Intestines, 0.5, 50, 0.2, 1.0,
			0.05);
		AddOrgan(avianProto, "sintestines", "small intestines", BodypartTypeEnum.Intestines, 2.0, 50, 0.2, 1.0,
			0.05);
		AddOrgan(avianProto, "rkidney", "right kidney", BodypartTypeEnum.Kidney, 0.5, 50, 0.2, 2.0, 0.05,
			painModifier: 3.0);
		AddOrgan(avianProto, "lkidney", "left kidney", BodypartTypeEnum.Kidney, 0.5, 50, 0.2, 2.0, 0.05,
			painModifier: 3.0);
		AddOrgan(avianProto, "rlung", "right lung", BodypartTypeEnum.Lung, 2.0, 50, 0.2, 1.0, 0.05);
		AddOrgan(avianProto, "llung", "left lung", BodypartTypeEnum.Lung, 2.0, 50, 0.2, 1.0, 0.05);
		AddOrgan(avianProto, "trachea", "trachea", BodypartTypeEnum.Trachea, 1.0, 50, 0.2, 1.0, 0.05);
		AddOrgan(avianProto, "esophagus", "esophagus", BodypartTypeEnum.Esophagus, 1.0, 50, 0.2, 1.0, 0.05);
		AddOrgan(avianProto, "uspinalcord", "upper spinal cord", BodypartTypeEnum.Spine, 1.0, 15, 0.2, 1.0, 0.05,
			stunModifier: 1.0, painModifier: 2.0);
		AddOrgan(avianProto, "mspinalcord", "middle spinal cord", BodypartTypeEnum.Spine, 1.0, 15, 0.2, 1.0, 0.05,
			stunModifier: 1.0, painModifier: 2.0);
		AddOrgan(avianProto, "lspinalcord", "lower spinal cord", BodypartTypeEnum.Spine, 1.0, 15, 0.2, 1.0, 0.05,
			stunModifier: 1.0, painModifier: 2.0);
		AddOrgan(avianProto, "rinnerear", "lower spinal cord", BodypartTypeEnum.Ear, 1.0, 15, 0.2, 1.0, 0.05);
		AddOrgan(avianProto, "linnerear", "lower spinal cord", BodypartTypeEnum.Ear, 1.0, 15, 0.2, 1.0, 0.05);

		AddOrganCoverage("brain", "head", 100, true);
		AddOrganCoverage("brain", "bhead", 100);
		AddOrganCoverage("brain", "rcheek", 85);
		AddOrganCoverage("brain", "lcheek", 85);
		AddOrganCoverage("brain", "reyesocket", 85);
		AddOrganCoverage("brain", "leyesocket", 85);
		AddOrganCoverage("brain", "reye", 85);
		AddOrganCoverage("brain", "leye", 85);
		AddOrganCoverage("brain", "beak", 10);
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

		AddOrganCoverage("uspinalcord", "bneck", 10, true);
		AddOrganCoverage("uspinalcord", "neck", 2);
		AddOrganCoverage("uspinalcord", "throat", 5);
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
		AddOrganCoverage("lintestines", "groin", 5);
		AddOrganCoverage("lintestines", "loin", 10);

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
				RootBody = avianProto,
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
		AddLimb("Right Leg", LimbType.Leg, "rupperleg", 0.5, 0.5);
		AddLimb("Left Leg", LimbType.Leg, "lupperleg", 0.5, 0.5);
		AddLimb("Tail", LimbType.Appendage, "tail", 0.5, 0.5);
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
					_context.LimbsSpinalParts.Add(new LimbsSpinalPart
						{ Limb = limb, BodypartProto = _cachedOrgans["uspinalcord"] });
					break;
				case "Genitals":
				case "Right Wing":
				case "Left Wing":
				case "Right Arm":
				case "Left Arm":
					_context.LimbsSpinalParts.Add(new LimbsSpinalPart
						{ Limb = limb, BodypartProto = _cachedOrgans["mspinalcord"] });
					break;
				case "Leg Leg":
				case "Right Leg":
				case "Tail":
					_context.LimbsSpinalParts.Add(new LimbsSpinalPart
						{ Limb = limb, BodypartProto = _cachedOrgans["lspinalcord"] });
					break;
			}
		}

		_context.SaveChanges();

		#endregion

		Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Groups...");

		#region Groups

		AddBodypartGroupDescriberShape(avianProto, "body", "The whole torso of an avian",
			("abdomen", 1, 1),
			("belly", 1, 1),
			("breast", 0, 2),
			("flank", 0, 4),
			("loin", 0, 1),
			("shoulder", 0, 2),
			("upper back", 1, 1),
			("lower back", 1, 1),
			("rump", 0, 2),
			("neck", 0, 1),
			("neck back", 0, 1),
			("throat", 0, 1)
		);
		AddBodypartGroupDescriberShape(avianProto, "legs", "Both legs of an avian",
			("upper leg", 2, 2),
			("lower leg", 0, 2),
			("knee", 0, 2),
			("ankle", 0, 2),
			("foot", 0, 2),
			("talon", 0, 2)
		);

		AddBodypartGroupDescriberShape(avianProto, "head", "An avian head",
			("face", 1, 1),
			("head back", 0, 1),
			("eye socket", 0, 2),
			("eye", 0, 2),
			("ear", 0, 2),
			("beak", 0, 1),
			("nose", 0, 1),
			("tongue", 0, 1),
			("cheek", 0, 2),
			("throat", 0, 1),
			("neck", 0, 1),
			("neck back", 0, 1)
		);

		AddBodypartGroupDescriberShape(avianProto, "back", "An avian back",
			("upper back", 1, 1),
			("lower back", 1, 1),
			("flank", 0, 4)
		);

		AddBodypartGroupDescriberShape(avianProto, "wings", "A pair of avian wings",
			("wing base", 2, 2),
			("wing", 2, 2)
		);

		AddBodypartGroupDescriberShape(avianProto, "talons", "A pair of avian talons",
			("talon", 2, 2)
		);

		AddBodypartGroupDescriberShape(avianProto, "feet", "A pair of avian feet",
			("foot", 2, 2)
		);

		AddBodypartGroupDescriberShape(avianProto, "eyes", "A pair of avian eyes",
			("eye socket", 2, 2),
			("eye", 0, 2)
		);

		AddBodypartGroupDescriberShape(avianProto, "ears", "A pair of avian ears",
			("ear", 2, 2)
		);

		#endregion

		_context.SaveChanges();

		Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Races...");

		#region Races

		AddRace("Pigeon", "Pigeon", null, avianProto, SizeCategory.VerySmall, false, 0.1, "Small Bird", "Small Bird",
			false);
		AddRace("Parrot", "Parrot", null, avianProto, SizeCategory.VerySmall, false, 0.1, "Small Bird", "Small Bird",
			false);
		AddRace("Swallow", "Swallow", null, avianProto, SizeCategory.VerySmall, false, 0.1, "Tiny Bird", "Tiny Bird",
			false);
		AddRace("Sparrow", "Sparrow", null, avianProto, SizeCategory.VerySmall, false, 0.05, "Tiny Bird", "Tiny Bird",
			false);
		AddRace("Finch", "Finch", null, avianProto, SizeCategory.VerySmall, false, 0.05, "Tiny Bird", "Tiny Bird",
			false);
		AddRace("Robin", "Robin", null, avianProto, SizeCategory.VerySmall, false, 0.05, "Tiny Bird", "Tiny Bird",
			false);
		AddRace("Wren", "Wren", null, avianProto, SizeCategory.VerySmall, false, 0.05, "Tiny Bird", "Tiny Bird", false);
		AddRace("Quail", "Quail", null, avianProto, SizeCategory.VerySmall, false, 0.1, "Tiny Bird", "Tiny Bird",
			false);
		AddRace("Duck", "Duck", null, avianProto, SizeCategory.Small, false, 0.2, "Fowl", "Fowl", false);
		AddRace("Goose", "Goose", null, avianProto, SizeCategory.Small, false, 0.4, "Fowl", "Fowl", false);
		AddRace("Swan", "Swan", null, avianProto, SizeCategory.Small, false, 0.4, "Fowl", "Fowl", false);
		AddRace("Grouse", "Grouse", null, avianProto, SizeCategory.Small, false, 0.2, "Fowl", "Fowl", false);
		AddRace("Pheasant", "Pheasant", null, avianProto, SizeCategory.Small, false, 0.2, "Fowl", "Fowl", false);
		AddRace("Chicken", "Chicken", null, avianProto, SizeCategory.Small, false, 0.2, "Fowl", "Fowl", false);
		AddRace("Turkey", "Turkey", null, avianProto, SizeCategory.Small, false, 0.35, "Fowl", "Fowl", false);
		AddRace("Seagull", "Seagull", null, avianProto, SizeCategory.Small, false, 0.2, "Small Bird", "Small Bird",
			false);
		AddRace("Albatross", "Albatross", null, avianProto, SizeCategory.Small, false, 0.35, "Medium Bird",
			"Medium Bird", false);
		AddRace("Heron", "Heron", null, avianProto, SizeCategory.Small, false, 0.2, "Medium Bird", "Medium Bird",
			false);
		AddRace("Crane", "Crane", null, avianProto, SizeCategory.Small, false, 0.2, "Medium Bird", "Medium Bird",
			false);
		AddRace("Flamingo", "Flamingo", null, avianProto, SizeCategory.Small, false, 0.2, "Medium Bird", "Medium Bird",
			false);
		AddRace("Peacock", "Peacock", null, avianProto, SizeCategory.Small, false, 0.2, "Medium Bird", "Medium Bird",
			false);
		AddRace("Ibis", "Ibis", null, avianProto, SizeCategory.Small, false, 0.2, "Medium Bird", "Medium Bird", false);
		AddRace("Pelican", "Pelican", null, avianProto, SizeCategory.Small, false, 0.5, "Large Bird", "Large Bird",
			false);
		AddRace("Crow", "Crow", null, avianProto, SizeCategory.Small, false, 0.2, "Small Bird", "Small Bird", false);
		AddRace("Raven", "Raven", null, avianProto, SizeCategory.Small, false, 0.2, "Small Bird", "Small Bird", false);
		AddRace("Emu", "Emu", null, avianProto, SizeCategory.Normal, false, 0.8, "Large Bird", "Large Bird", false);
		AddRace("Ostrich", "Ostrich", null, avianProto, SizeCategory.Normal, false, 0.8, "Large Bird", "Large Bird",
			false);
		AddRace("Moa", "Moa", null, avianProto, SizeCategory.Normal, false, 0.8, "Large Bird", "Large Bird", false);
		AddRace("Vulture", "Vulture", null, avianProto, SizeCategory.Small, false, 0.35, "Medium Bird", "Medium Bird",
			false);
		AddRace("Hawk", "Hawk", null, avianProto, SizeCategory.Small, false, 0.35, "Small Bird", "Small Bird", false);
		AddRace("Eagle", "Eagle", null, avianProto, SizeCategory.Normal, false, 0.7, "Medium Bird", "Medium Bird",
			false);
		AddRace("Falcon", "Falcon", null, avianProto, SizeCategory.Small, false, 0.35, "Small Bird", "Small Bird",
			false);
		AddRace("Woodpecker", "Woodpecker", null, avianProto, SizeCategory.Small, false, 0.35, "Small Bird",
			"Small Bird", false);
		AddRace("Owl", "Owl", null, avianProto, SizeCategory.Small, false, 0.35, "Medium Bird", "Medium Bird", false);
		AddRace("Kingfisher", "Kingfisher", null, avianProto, SizeCategory.Small, false, 0.35, "Small Bird",
			"Small Bird", false);
		AddRace("Stork", "Stork", null, avianProto, SizeCategory.Small, false, 0.35, "Medium Bird", "Medium Bird",
			false);
		AddRace("Penguin", "Penguin", null, avianProto, SizeCategory.Small, false, 0.2, "Small Bird", "Small Bird",
			false);

		#endregion
	}

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
			Alignment.FrontRight, Orientation.Highest, 10, 30, 100, order++, "Flesh", SizeCategory.Small, "Head",
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
		AddBodypart(wormProto, "tail", "tail", "tail", BodypartTypeEnum.Wear, "lbody", Alignment.Front,
			Orientation.Lowest, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal, "Torso", true,
			isVital: true, implantSpace: 5, stunMultiplier: 0.5);

		#endregion

		_context.SaveChanges();

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

		AddRace("Python", "Python", null, serpentProto, SizeCategory.Small, false, 0.1, "Serpent", "Serpent", false);
		AddRace("Tree Python", "Tree Python", null, serpentProto, SizeCategory.Normal, true, 0.4, "Serpent", "Serpent",
			false);
		AddRace("Boa", "Boa", null, serpentProto, SizeCategory.Normal, false, 0.6, "Serpent", "Serpent", false);
		AddRace("Anaconda", "Anaconda", null, serpentProto, SizeCategory.Large, true, 1.0, "Serpent", "Serpent", false);
		AddRace("Cobra", "Cobra", null, serpentProto, SizeCategory.Small, false, 0.2, "Serpent", "Serpent", false);
		AddRace("Adder", "Adder", null, serpentProto, SizeCategory.Small, false, 0.2, "Serpent", "Serpent", false);
		AddRace("Rattlesnake", "Rattlesnake", null, serpentProto, SizeCategory.Small, false, 0.2, "Serpent", "Serpent",
			false);
		AddRace("Viper", "Viper", null, serpentProto, SizeCategory.Small, false, 0.2, "Serpent", "Serpent", false);
		AddRace("Mamba", "Mamba", null, serpentProto, SizeCategory.Small, false, 0.2, "Serpent", "Serpent", false);
		AddRace("Coral Snake", "Coral Snake", null, serpentProto, SizeCategory.Small, false, 0.2, "Serpent", "Serpent",
			false);
		AddRace("Moccasin", "Moccasin", null, serpentProto, SizeCategory.Small, false, 0.2, "Serpent", "Serpent",
			false);

		#endregion
	}

	private void SeedAquatic(BodyProto fishProto, BodyProto crabProto, BodyProto octopusProto,
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
		AddBodypart(fishProto, "llflank", "lower left flank", "flank", BodypartTypeEnum.Wear, "abdomen",
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
		AddBodypart(fishProto, "rgill", "right gills", "gill", BodypartTypeEnum.Gill, "urflank", Alignment.Front,
			Orientation.High, 80, 100, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true, isVital: true,
			implantSpace: 5, stunMultiplier: 1.0);
		AddBodypart(fishProto, "lgill", "left gills", "gill", BodypartTypeEnum.Gill, "ulflank", Alignment.Front,
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
			Alignment.Front, Orientation.Highest, 80, 100, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head",
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
		AddBodypart(pinnipedProto, "rjaw", "right jaw", "jaw", BodypartTypeEnum.Wear, "neck",
			Alignment.FrontRight, Orientation.Highest, 40, -1, 100, order++, "Bony Flesh", SizeCategory.Small,
			"Head", true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(pinnipedProto, "ljaw", "left jaw", "jaw", BodypartTypeEnum.Wear, "neck",
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
		AddOrgan(pinnipedProto, "rinnerear", "lower spinal cord", BodypartTypeEnum.Ear, 1.0, 15, 0.2, 1.0, 0.05);
		AddOrgan(pinnipedProto, "linnerear", "lower spinal cord", BodypartTypeEnum.Ear, 1.0, 15, 0.2, 1.0, 0.05);

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
		AddBodypart(cetaceanProto, "blowhole", "blowhole", "blowhole", BodypartTypeEnum.Blowhole, "neck",
			Alignment.Front,
			Orientation.High, 80, 100, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true, isVital: true,
			implantSpace: 5, stunMultiplier: 1.0);
		AddBodypart(cetaceanProto, "head", "head", "face", BodypartTypeEnum.Wear, "neck", Alignment.Front,
			Orientation.Highest, 80, -1, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head", true,
			isVital: true, implantSpace: 5, stunMultiplier: 1.0);
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

		AddRace("Carp", "Carp", null, fishProto, SizeCategory.Small, false, 0.2, "Small Fish", "Small Fish", false,
			"freshwater");
		AddRace("Cod", "Cod", null, fishProto, SizeCategory.Small, false, 0.2, "Small Fish", "Small Fish", false,
			"freshwater");
		AddRace("Haddock", "Haddock", null, fishProto, SizeCategory.Small, false, 0.2, "Small Fish", "Small Fish",
			false, "saltwater");
		AddRace("Koi", "Koi", null, fishProto, SizeCategory.Small, false, 0.2, "Small Fish", "Small Fish", false,
			"freshwater");
		AddRace("Pilchard", "Pilchard", null, fishProto, SizeCategory.Small, false, 0.2, "Small Fish", "Small Fish",
			false, "saltwater");
		AddRace("Perch", "Perch", null, fishProto, SizeCategory.Small, false, 0.2, "Small Fish", "Small Fish", false,
			"saltwater");
		AddRace("Herring", "Herring", null, fishProto, SizeCategory.VerySmall, false, 0.1, "Tiny Fish", "Tiny Fish",
			false, "saltwater");
		AddRace("Mackerel", "Mackerel", null, fishProto, SizeCategory.VerySmall, false, 0.1, "Tiny Fish", "Tiny Fish",
			false, "saltwater");
		AddRace("Anchovy", "Anchovy", null, fishProto, SizeCategory.VerySmall, false, 0.1, "Tiny Fish", "Tiny Fish",
			false, "saltwater");
		AddRace("Sardine", "Sardine", null, fishProto, SizeCategory.VerySmall, false, 0.1, "Tiny Fish", "Tiny Fish",
			false, "saltwater");
		AddRace("Pollock", "Pollock", null, fishProto, SizeCategory.Small, false, 0.2, "Small Fish", "Small Fish",
			false, "saltwater");
		AddRace("Salmon", "Salmon", null, fishProto, SizeCategory.Small, false, 0.2, "Small Fish", "Small Fish", false,
			"freshwater");
		AddRace("Tuna", "Tuna", null, fishProto, SizeCategory.Normal, false, 0.5, "Large Fish", "Large Fish", false,
			"saltwater");
		AddRace("Shark", "Shark", null, fishProto, SizeCategory.Large, false, 1.2, "Shark", "Shark", false,
			"saltwater");

		AddRace("Small Crab", "Small Crab", null, crabProto, SizeCategory.VerySmall, false, 0.2, "Small Crab",
			"Small Crab", false, "saltwater");
		AddRace("Crab", "Crab", null, crabProto, SizeCategory.Small, false, 0.6, "Medium Crab", "Medium Crab", false,
			"saltwater");
		AddRace("Giant Crab", "Giant Crab", null, crabProto, SizeCategory.Normal, false, 1.2, "Large Crab",
			"Large Crab", false, "saltwater");
		AddRace("Lobster", "Lobster", null, crabProto, SizeCategory.Small, false, 0.6, "Lobster", "Lobster", false,
			"saltwater");

		AddRace("Jellyfish", "Jellyfish", null, jellyfishProto, SizeCategory.Small, false, 0.1, "Small Jellyfish",
			"Small Jellyfish", false, "partless");

		AddRace("Octopus", "Octopus", null, octopusProto, SizeCategory.Small, true, 0.4, "Octopus", "Octopus", false,
			"partless");
		AddRace("Squid", "Squid", null, octopusProto, SizeCategory.Small, false, 0.4, "Squid", "Squid", false,
			"partless");
		AddRace("Giant Squid", "Giant Squid", null, octopusProto, SizeCategory.Large, false, 1.2, "Giant Squid",
			"Giant Squid", false, "partless");

		AddRace("Sea Lion", "Sea Lion", null, pinnipedProto, SizeCategory.Normal, false, 0.8, "Seal Male",
			"Seal Female", false);
		AddRace("Seal", "Seal", null, pinnipedProto, SizeCategory.Normal, false, 0.8, "Seal Male", "Seal Female",
			false);
		AddRace("Walrus", "Walrus", null, pinnipedProto, SizeCategory.Large, false, 1.4, "Walrus Male", "Walrus Female",
			false);

		AddRace("Dolphin", "Dolphin", null, cetaceanProto, SizeCategory.Normal, false, 1.0, "Dolphin", "Dolphin", false,
			"blowhole");
		AddRace("Porpoise", "Porpoise", null, cetaceanProto, SizeCategory.Normal, false, 1.0, "Dolphin", "Dolphin",
			false, "blowhole");
		AddRace("Orca", "Orca", null, cetaceanProto, SizeCategory.VeryLarge, false, 2.0, "Small Whale", "Small Whale",
			false, "blowhole");
		AddRace("Baleen Whale", "Baleen Whale", null, cetaceanProto, SizeCategory.Huge, false, 3.0, "Large Whale",
			"Large Whale", false, "blowhole");
		AddRace("Toothed Whale", "Toothed Whale", null, cetaceanProto, SizeCategory.Huge, false, 3.0, "Small Whale",
			"Small Whale", false, "blowhole");

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
			Alignment.Front, Orientation.Highest, 80, 100, 100, order++, "Bony Flesh", SizeCategory.Normal, "Head",
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
		AddBodypart(quadrupedBody, "rjaw", "right jaw", "jaw", BodypartTypeEnum.Wear, "neck",
			Alignment.FrontRight, Orientation.Highest, 40, -1, 100, order++, "Bony Flesh", SizeCategory.Small,
			"Head", true, isVital: true, implantSpace: 5, stunMultiplier: 0.5);
		AddBodypart(quadrupedBody, "ljaw", "left jaw", "jaw", BodypartTypeEnum.Wear, "neck",
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
		AddBodypart(toedQuadruped, "rrclaw", "right dewclaw", "dewclaw", BodypartTypeEnum.Wear, "rrpaw",
			Alignment.RearRight, Orientation.Lowest, 10, 50, 5, order++, "Dense Bony Flesh", SizeCategory.Normal,
			"Right Hindleg", false, isVital: false);
		AddBodypart(toedQuadruped, "lrclaw", "left dewclaw", "claw", BodypartTypeEnum.Wear, "lrpaw",
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
		AddOrgan(quadrupedBody, "rinnerear", "lower spinal cord", BodypartTypeEnum.Ear, 1.0, 15, 0.2, 1.0, 0.05);
		AddOrgan(quadrupedBody, "linnerear", "lower spinal cord", BodypartTypeEnum.Ear, 1.0, 15, 0.2, 1.0, 0.05);

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

		AddRace("Rabbit", "Leporine", null, toedQuadruped, SizeCategory.VerySmall, false, 0.3, "Small Mammal",
			"Small Mammal");
		AddRace("Hare", "Hare", null, toedQuadruped, SizeCategory.VerySmall, false, 0.5, "Small Mammal",
			"Small Mammal");
		AddRace("Beaver", "Beaver", null, toedQuadruped, SizeCategory.Small, false, 0.8, "Small Mammal",
			"Small Mammal");
		AddRace("Otter", "Otter", null, toedQuadruped, SizeCategory.VerySmall, false, 0.5, "Small Mammal",
			"Small Mammal");
		AddRace("Dog", "Canine", "Common domestic dogs", toedQuadruped, SizeCategory.Small, false, 0.8, "Dog", "Dog");
		AddRace("Cat", "Feline", "Common domestic cats", toedQuadruped, SizeCategory.Small, true, 0.5, "Cat", "Cat");
		AddRace("Wolf", "Lupine", "Wild grey wolves", toedQuadruped, SizeCategory.Normal, false, 1.0, "Wolf", "Wolf");
		AddRace("Fox", "Vulpine", "Foxes", toedQuadruped, SizeCategory.Small, false, 0.8, "Small Mammal",
			"Small Mammal");
		AddRace("Coyote", "Coyote", "Coyote", toedQuadruped, SizeCategory.Small, false, 0.8, "Dog", "Dog");
		AddRace("Hyena", "Hyena", "Hyena", toedQuadruped, SizeCategory.Small, false, 0.8, "Dog", "Dog");
		AddRace("Lion", "Leonine", "Lions", toedQuadruped, SizeCategory.Normal, true, 1.3, "Lion Male", "Lion Female");
		AddRace("Tiger", "Tigrine", "Tigers", toedQuadruped, SizeCategory.Normal, true, 1.3, "Tiger Male",
			"Tiger Female");
		AddRace("Cheetah", "Cheetah", "Cheetah", toedQuadruped, SizeCategory.Small, false, 0.8, "Big Cat Male",
			"Big Cat Female");
		AddRace("Leopard", "Leopard", "Leopard", toedQuadruped, SizeCategory.Small, false, 0.8, "Big Cat Male",
			"Big Cat Female");
		AddRace("Panther", "Panther", "Panther", toedQuadruped, SizeCategory.Small, false, 0.8, "Big Cat Male",
			"Big Cat Female");
		AddRace("Jaguar", "Jaguar", "Jaguar", toedQuadruped, SizeCategory.Small, false, 0.8, "Big Cat Male",
			"Big Cat Female");
		AddRace("Jackal", "Jackal", "Jackal", toedQuadruped, SizeCategory.Small, false, 0.8, "Dog", "Dog");

		AddRace("Deer", "Cervine", "Deer", ungulateBody, SizeCategory.Normal, false, 0.8, "Deer Male", "Deer Female");
		AddRace("Moose", "Moose", "Moose", ungulateBody, SizeCategory.Large, false, 1.5, "Moose Male", "Moose Female");
		AddRace("Pig", "Porcine", "Pigs", ungulateBody, SizeCategory.Normal, false, 1.0, "Pig Male", "Pig Female");
		AddRace("Boar", "Boar", null, ungulateBody, SizeCategory.Normal, false, 1.2, "Pig Male", "Pig Female");
		AddRace("Warthog", "Warthog", null, ungulateBody, SizeCategory.Normal, false, 1.2, "Pig Male", "Pig Female");
		AddRace("Sheep", "Ovine", "Sheep", ungulateBody, SizeCategory.Normal, false, 0.8, "Sheep Male", "Sheep Female");
		AddRace("Goat", "Caprine", "Goats", ungulateBody, SizeCategory.Normal, false, 0.8, "Goat Male", "Goat Female");
		AddRace("Llama", "Llama", "Llamas", ungulateBody, SizeCategory.Normal, false, 1.0, "Large Mammal",
			"Large Mammal");
		AddRace("Alpaca", "Alpaca", "Alpacas", ungulateBody, SizeCategory.Normal, false, 0.8, "Large Mammal",
			"Large Mammal");

		AddRace("Bear", "Ursine", "Bears", ungulateBody, SizeCategory.Large, false, 1.4, "Bear Male", "Bear Female");
		AddRace("Mouse", "Murine", "Mice", ungulateBody, SizeCategory.Tiny, true, 0.1, "Tiny Mammal", "Tiny Mammal");
		AddRace("Rat", "Rat", "Rats", ungulateBody, SizeCategory.Tiny, true, 0.2, "Tiny Mammal", "Tiny Mammal");
		AddRace("Hamster", "Cricetid", "Hamsters", ungulateBody, SizeCategory.Tiny, false, 0.1, "Tiny Mammal",
			"Tiny Mammal");
		AddRace("Guinea Pig", "Guinea Pig", "Guinea Pigs", ungulateBody, SizeCategory.Tiny, false, 0.1, "Tiny Mammal",
			"Tiny Mammal");
		AddRace("Ferret", "Ferret", "Ferrets", ungulateBody, SizeCategory.VerySmall, true, 0.2, "Small Mammal",
			"Small Mammal");
		AddRace("Weasel", "Weasel", "Weasels", ungulateBody, SizeCategory.VerySmall, true, 0.2, "Small Mammal",
			"Small Mammal");
		AddRace("Badger", "Badger", "Badgers", ungulateBody, SizeCategory.Small, false, 0.8, "Medium Mammal",
			"Medium Mammal");
		AddRace("Wolverine", "Wolverine", "Wolverines", ungulateBody, SizeCategory.Small, false, 0.5, "Medium Mammal",
			"Medium Mammal");

		AddRace("Cow", "Bovine", "Cows", ungulateBody, SizeCategory.Large, false, 1.5, "Cow Male", "Cow Female");
		AddRace("Ox", "Ox", "Oxen", ungulateBody, SizeCategory.Large, false, 1.7, "Cow Male", "Cow Female");
		AddRace("Bison", "Bison", "Bisons", ungulateBody, SizeCategory.Large, false, 1.5, "Cow Male", "Cow Female");
		AddRace("Buffalo", "Buffalo", "Buffalos", ungulateBody, SizeCategory.Large, false, 1.5, "Cow Male",
			"Cow Female");
		AddRace("Horse", "Equine", "Horse", ungulateBody, SizeCategory.Large, false, 1.5, "Horse Male", "Horse Female");

		AddRace("Rhinocerous", "Ceratorhine", "Rhinocerouses", ungulateBody, SizeCategory.Large, false, 1.5,
			"Rhino Male", "Rhino Female");
		AddRace("Hippopotamus", "Hippopotamine", "Hippopotamuses", ungulateBody, SizeCategory.Large, false, 1.5,
			"Hippo Male", "Hippo Female");
		AddRace("Elephant", "Elephantine", "Elephants", ungulateBody, SizeCategory.VeryLarge, false, 2.0,
			"Elephant Male", "Elephant Female");
		AddRace("Giraffe", "Giraffine", "Giraffes", ungulateBody, SizeCategory.VeryLarge, false, 1.0, "Huge Mammal",
			"Huge Mammal");

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
			RelativeHitChance = hitChance,
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
			Id = nextId++, BodyProto = quadrupedBody, PositionId = 1, Alias = "stalk", FirstPersonVerb = "stalk",
			ThirdPersonVerb = "stalks", PresentParticiple = "stalking", Multiplier = 2, StaminaMultiplier = 0.4
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = quadrupedBody, PositionId = 1, Alias = "walk", FirstPersonVerb = "walk",
			ThirdPersonVerb = "walks", PresentParticiple = "walking", Multiplier = 1, StaminaMultiplier = 0.8
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = quadrupedBody, PositionId = 1, Alias = "amble", FirstPersonVerb = "amble",
			ThirdPersonVerb = "ambles", PresentParticiple = "ambling", Multiplier = 0.8, StaminaMultiplier = 1.2
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = quadrupedBody, PositionId = 1, Alias = "pace", FirstPersonVerb = "pace",
			ThirdPersonVerb = "paces", PresentParticiple = "pacing", Multiplier = 0.6, StaminaMultiplier = 1.9
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = quadrupedBody, PositionId = 1, Alias = "trot", FirstPersonVerb = "trot",
			ThirdPersonVerb = "trots", PresentParticiple = "troting", Multiplier = 0.4, StaminaMultiplier = 2.4
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = quadrupedBody, PositionId = 1, Alias = "gallop", FirstPersonVerb = "gallop",
			ThirdPersonVerb = "gallops", PresentParticiple = "galloping", Multiplier = 0.2, StaminaMultiplier = 4.0
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = quadrupedBody, PositionId = 6, Alias = "crawl", FirstPersonVerb = "crawl",
			ThirdPersonVerb = "crawls", PresentParticiple = "crawling", Multiplier = 5, StaminaMultiplier = 1.25
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = quadrupedBody, PositionId = 7, Alias = "shuffle",
			FirstPersonVerb = "shuffle", ThirdPersonVerb = "shuffles", PresentParticiple = "shuffling",
			Multiplier = 7, StaminaMultiplier = 2
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = quadrupedBody, PositionId = 15, Alias = "climb", FirstPersonVerb = "climb",
			ThirdPersonVerb = "climbs", PresentParticiple = "climbing", Multiplier = 3, StaminaMultiplier = 3
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = quadrupedBody, PositionId = 16, Alias = "swim", FirstPersonVerb = "swim",
			ThirdPersonVerb = "swims", PresentParticiple = "swimming", Multiplier = 1.5, StaminaMultiplier = 2
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = quadrupedBody, PositionId = 16, Alias = "slowswim", FirstPersonVerb = "swim",
			ThirdPersonVerb = "swims", PresentParticiple = "swimming", Multiplier = 2, StaminaMultiplier = 1.5
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = quadrupedBody, PositionId = 18, Alias = "fly", FirstPersonVerb = "fly",
			ThirdPersonVerb = "flies", PresentParticiple = "flying", Multiplier = 1.8, StaminaMultiplier = 15
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = quadrupedBody, PositionId = 18, Alias = "franticfly",
			FirstPersonVerb = "franticly fly", ThirdPersonVerb = "franticly flies",
			PresentParticiple = "franticly flying", Multiplier = 1.4, StaminaMultiplier = 25
		});

		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = avianBody, PositionId = 1, Alias = "hop", FirstPersonVerb = "hop",
			ThirdPersonVerb = "hops", PresentParticiple = "hopping", Multiplier = 1, StaminaMultiplier = 0.8
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = avianBody, PositionId = 6, Alias = "crawl", FirstPersonVerb = "crawl",
			ThirdPersonVerb = "crawls", PresentParticiple = "crawling", Multiplier = 5, StaminaMultiplier = 1.25
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = avianBody, PositionId = 7, Alias = "shuffle", FirstPersonVerb = "shuffle",
			ThirdPersonVerb = "shuffles", PresentParticiple = "shuffling", Multiplier = 7, StaminaMultiplier = 2
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = avianBody, PositionId = 15, Alias = "climb", FirstPersonVerb = "climb",
			ThirdPersonVerb = "climbs", PresentParticiple = "climbing", Multiplier = 3, StaminaMultiplier = 3
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = avianBody, PositionId = 16, Alias = "swim", FirstPersonVerb = "swim",
			ThirdPersonVerb = "swims", PresentParticiple = "swimming", Multiplier = 1.5, StaminaMultiplier = 2
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = avianBody, PositionId = 16, Alias = "slowswim", FirstPersonVerb = "swim",
			ThirdPersonVerb = "swims", PresentParticiple = "swimming", Multiplier = 2, StaminaMultiplier = 1.5
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = avianBody, PositionId = 18, Alias = "slowfly",
			FirstPersonVerb = "slowly fly", ThirdPersonVerb = "slowly flies", PresentParticiple = "slowly flying",
			Multiplier = 2.7, StaminaMultiplier = 8
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = avianBody, PositionId = 18, Alias = "fly", FirstPersonVerb = "fly",
			ThirdPersonVerb = "flies", PresentParticiple = "flying", Multiplier = 1.8, StaminaMultiplier = 15
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = avianBody, PositionId = 18, Alias = "franticfly",
			FirstPersonVerb = "franticly fly", ThirdPersonVerb = "franticly flies",
			PresentParticiple = "franticly flying", Multiplier = 1.4, StaminaMultiplier = 25
		});

		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = fishProto, PositionId = 6, Alias = "flop", FirstPersonVerb = "flop",
			ThirdPersonVerb = "flops", PresentParticiple = "flopping", Multiplier = 6, StaminaMultiplier = 3.0
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = fishProto, PositionId = 16, Alias = "swim", FirstPersonVerb = "swim",
			ThirdPersonVerb = "swims", PresentParticiple = "swimming", Multiplier = 1.5, StaminaMultiplier = 2
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = fishProto, PositionId = 16, Alias = "slowswim", FirstPersonVerb = "swim",
			ThirdPersonVerb = "swims", PresentParticiple = "swimming", Multiplier = 2, StaminaMultiplier = 1.5
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = fishProto, PositionId = 16, Alias = "quickswim",
			FirstPersonVerb = "swim quickly",
			ThirdPersonVerb = "swims quickly", PresentParticiple = "swimming quickly", Multiplier = 1.0,
			StaminaMultiplier = 2
		});

		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = jellyfishProto, PositionId = 16, Alias = "float", FirstPersonVerb = "float",
			ThirdPersonVerb = "floats", PresentParticiple = "floating", Multiplier = 1.5, StaminaMultiplier = 2
		});

		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = serpentBody, PositionId = 6, Alias = "slither", FirstPersonVerb = "slither",
			ThirdPersonVerb = "slithers", PresentParticiple = "slithering", Multiplier = 1.5, StaminaMultiplier = 1.25
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = serpentBody, PositionId = 6, Alias = "slowslither",
			FirstPersonVerb = "slither slowly",
			ThirdPersonVerb = "slithers slowly", PresentParticiple = "slowly slithering", Multiplier = 2.5,
			StaminaMultiplier = 0.75
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = serpentBody, PositionId = 6, Alias = "quickslither",
			FirstPersonVerb = "slither quickly",
			ThirdPersonVerb = "slithers quickly", PresentParticiple = "quickly slithering", Multiplier = 1.0,
			StaminaMultiplier = 2.0
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = serpentBody, PositionId = 15, Alias = "climb", FirstPersonVerb = "climb",
			ThirdPersonVerb = "climbs", PresentParticiple = "climbing", Multiplier = 3, StaminaMultiplier = 3
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = serpentBody, PositionId = 16, Alias = "swim", FirstPersonVerb = "swim",
			ThirdPersonVerb = "swims", PresentParticiple = "swimming", Multiplier = 1.5, StaminaMultiplier = 2
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = serpentBody, PositionId = 16, Alias = "slowswim", FirstPersonVerb = "swim",
			ThirdPersonVerb = "swims", PresentParticiple = "swimming", Multiplier = 2, StaminaMultiplier = 1.5
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = serpentBody, PositionId = 18, Alias = "fly", FirstPersonVerb = "fly",
			ThirdPersonVerb = "flies", PresentParticiple = "flying", Multiplier = 1.8, StaminaMultiplier = 15
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = serpentBody, PositionId = 18, Alias = "franticfly",
			FirstPersonVerb = "franticly fly", ThirdPersonVerb = "franticly flies",
			PresentParticiple = "franticly flying", Multiplier = 1.4, StaminaMultiplier = 25
		});

		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = wormBody, PositionId = 6, Alias = "slither", FirstPersonVerb = "slither",
			ThirdPersonVerb = "slithers", PresentParticiple = "slithering", Multiplier = 1.5, StaminaMultiplier = 1.25
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = wormBody, PositionId = 6, Alias = "slowslither",
			FirstPersonVerb = "slither slowly",
			ThirdPersonVerb = "slithers slowly", PresentParticiple = "slowly slithering", Multiplier = 2.5,
			StaminaMultiplier = 0.75
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = wormBody, PositionId = 6, Alias = "quickslither",
			FirstPersonVerb = "slither quickly",
			ThirdPersonVerb = "slithers quickly", PresentParticiple = "quickly slithering", Multiplier = 1.0,
			StaminaMultiplier = 2.0
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = wormBody, PositionId = 15, Alias = "climb", FirstPersonVerb = "climb",
			ThirdPersonVerb = "climbs", PresentParticiple = "climbing", Multiplier = 3, StaminaMultiplier = 3
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = wormBody, PositionId = 16, Alias = "swim", FirstPersonVerb = "swim",
			ThirdPersonVerb = "swims", PresentParticiple = "swimming", Multiplier = 1.5, StaminaMultiplier = 2
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = wormBody, PositionId = 16, Alias = "slowswim", FirstPersonVerb = "swim",
			ThirdPersonVerb = "swims", PresentParticiple = "swimming", Multiplier = 2, StaminaMultiplier = 1.5
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = wormBody, PositionId = 18, Alias = "fly", FirstPersonVerb = "fly",
			ThirdPersonVerb = "flies", PresentParticiple = "flying", Multiplier = 1.8, StaminaMultiplier = 15
		});
                _context.MoveSpeeds.Add(new MoveSpeed
                {
                        Id = nextId++, BodyProto = wormBody, PositionId = 18, Alias = "franticfly",
                        FirstPersonVerb = "franticly fly", ThirdPersonVerb = "franticly flies",
                        PresentParticiple = "franticly flying", Multiplier = 1.4, StaminaMultiplier = 25
                });

                _context.MoveSpeeds.Add(new MoveSpeed
                {
                        Id = nextId++, BodyProto = insectBody, PositionId = 1, Alias = "walk", FirstPersonVerb = "walk",
                        ThirdPersonVerb = "walks", PresentParticiple = "walking", Multiplier = 1, StaminaMultiplier = 0.8
                });
                _context.MoveSpeeds.Add(new MoveSpeed
                {
                        Id = nextId++, BodyProto = insectBody, PositionId = 1, Alias = "run", FirstPersonVerb = "run",
                        ThirdPersonVerb = "runs", PresentParticiple = "running", Multiplier = 0.5, StaminaMultiplier = 1.5
                });
                _context.MoveSpeeds.Add(new MoveSpeed
                {
                        Id = nextId++, BodyProto = insectBody, PositionId = 6, Alias = "crawl", FirstPersonVerb = "crawl",
                        ThirdPersonVerb = "crawls", PresentParticiple = "crawling", Multiplier = 1.5, StaminaMultiplier = 1.0
                });
                _context.MoveSpeeds.Add(new MoveSpeed
                {
                        Id = nextId++, BodyProto = insectBody, PositionId = 15, Alias = "climb", FirstPersonVerb = "climb",
                        ThirdPersonVerb = "climbs", PresentParticiple = "climbing", Multiplier = 2, StaminaMultiplier = 2
                });
                _context.MoveSpeeds.Add(new MoveSpeed
                {
                        Id = nextId++, BodyProto = insectBody, PositionId = 18, Alias = "fly", FirstPersonVerb = "fly",
                        ThirdPersonVerb = "flies", PresentParticiple = "flying", Multiplier = 1.5, StaminaMultiplier = 10
                });

                _context.MoveSpeeds.Add(new MoveSpeed
                {
                        Id = nextId++, BodyProto = wingedInsectBody, PositionId = 1, Alias = "walk", FirstPersonVerb = "walk",
                        ThirdPersonVerb = "walks", PresentParticiple = "walking", Multiplier = 1, StaminaMultiplier = 0.8
                });
                _context.MoveSpeeds.Add(new MoveSpeed
                {
                        Id = nextId++, BodyProto = wingedInsectBody, PositionId = 1, Alias = "run", FirstPersonVerb = "run",
                        ThirdPersonVerb = "runs", PresentParticiple = "running", Multiplier = 0.5, StaminaMultiplier = 1.5
                });
                _context.MoveSpeeds.Add(new MoveSpeed
                {
                        Id = nextId++, BodyProto = wingedInsectBody, PositionId = 6, Alias = "crawl", FirstPersonVerb = "crawl",
                        ThirdPersonVerb = "crawls", PresentParticiple = "crawling", Multiplier = 1.5, StaminaMultiplier = 1.0
                });
                _context.MoveSpeeds.Add(new MoveSpeed
                {
                        Id = nextId++, BodyProto = wingedInsectBody, PositionId = 15, Alias = "climb", FirstPersonVerb = "climb",
                        ThirdPersonVerb = "climbs", PresentParticiple = "climbing", Multiplier = 2, StaminaMultiplier = 2
                });
                _context.MoveSpeeds.Add(new MoveSpeed
                {
                        Id = nextId++, BodyProto = wingedInsectBody, PositionId = 18, Alias = "fly", FirstPersonVerb = "fly",
                        ThirdPersonVerb = "flies", PresentParticiple = "flying", Multiplier = 1.5, StaminaMultiplier = 10
                });

		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = pinnipedProto, PositionId = 1, Alias = "walk", FirstPersonVerb = "walk",
			ThirdPersonVerb = "walks", PresentParticiple = "walking", Multiplier = 3, StaminaMultiplier = 3.0
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = pinnipedProto, PositionId = 1, Alias = "run", FirstPersonVerb = "run",
			ThirdPersonVerb = "runs", PresentParticiple = "running", Multiplier = 1.75, StaminaMultiplier = 5.0
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = pinnipedProto, PositionId = 6, Alias = "crawl", FirstPersonVerb = "crawl",
			ThirdPersonVerb = "crawls", PresentParticiple = "crawling", Multiplier = 5, StaminaMultiplier = 3.0
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = pinnipedProto, PositionId = 7, Alias = "shuffle",
			FirstPersonVerb = "shuffle", ThirdPersonVerb = "shuffles", PresentParticiple = "shuffling",
			Multiplier = 7, StaminaMultiplier = 5.0
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = pinnipedProto, PositionId = 15, Alias = "climb", FirstPersonVerb = "climb",
			ThirdPersonVerb = "climbs", PresentParticiple = "climbing", Multiplier = 3, StaminaMultiplier = 3
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = pinnipedProto, PositionId = 16, Alias = "swim", FirstPersonVerb = "swim",
			ThirdPersonVerb = "swims", PresentParticiple = "swimming", Multiplier = 1.5, StaminaMultiplier = 1.5
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = pinnipedProto, PositionId = 16, Alias = "slowswim",
			FirstPersonVerb = "swim slowly",
			ThirdPersonVerb = "swims slowly", PresentParticiple = "slowly swimming", Multiplier = 2,
			StaminaMultiplier = 1.0
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = pinnipedProto, PositionId = 16, Alias = "quickswim",
			FirstPersonVerb = "swim quickly",
			ThirdPersonVerb = "swims quickly", PresentParticiple = "swimming quickly", Multiplier = 1.0,
			StaminaMultiplier = 2
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = pinnipedProto, PositionId = 18, Alias = "fly", FirstPersonVerb = "fly",
			ThirdPersonVerb = "flies", PresentParticiple = "flying", Multiplier = 1.8, StaminaMultiplier = 15
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = pinnipedProto, PositionId = 18, Alias = "franticfly",
			FirstPersonVerb = "franticly fly", ThirdPersonVerb = "franticly flies",
			PresentParticiple = "franticly flying", Multiplier = 1.4, StaminaMultiplier = 25
		});

		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = crabProto, PositionId = 1, Alias = "walk", FirstPersonVerb = "walk",
			ThirdPersonVerb = "walks", PresentParticiple = "walking", Multiplier = 1.5, StaminaMultiplier = 1.0
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = crabProto, PositionId = 1, Alias = "skitter", FirstPersonVerb = "skitter",
			ThirdPersonVerb = "skitters", PresentParticiple = "skittering", Multiplier = 1.0, StaminaMultiplier = 2.25
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = crabProto, PositionId = 6, Alias = "crawl", FirstPersonVerb = "crawl",
			ThirdPersonVerb = "crawls", PresentParticiple = "crawling", Multiplier = 5, StaminaMultiplier = 3.0
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = crabProto, PositionId = 15, Alias = "climb", FirstPersonVerb = "climb",
			ThirdPersonVerb = "climbs", PresentParticiple = "climbing", Multiplier = 3, StaminaMultiplier = 3
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = crabProto, PositionId = 16, Alias = "swim", FirstPersonVerb = "swim",
			ThirdPersonVerb = "swims", PresentParticiple = "swimming", Multiplier = 1.5, StaminaMultiplier = 1.5
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = crabProto, PositionId = 16, Alias = "slowswim", FirstPersonVerb = "swim slowly",
			ThirdPersonVerb = "swims slowly", PresentParticiple = "slowly swimming", Multiplier = 2,
			StaminaMultiplier = 1.0
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = crabProto, PositionId = 16, Alias = "quickswim",
			FirstPersonVerb = "swim quickly",
			ThirdPersonVerb = "swims quickly", PresentParticiple = "swimming quickly", Multiplier = 1.0,
			StaminaMultiplier = 2
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = crabProto, PositionId = 18, Alias = "fly", FirstPersonVerb = "fly",
			ThirdPersonVerb = "flies", PresentParticiple = "flying", Multiplier = 1.8, StaminaMultiplier = 15
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = crabProto, PositionId = 18, Alias = "franticfly",
			FirstPersonVerb = "franticly fly", ThirdPersonVerb = "franticly flies",
			PresentParticiple = "franticly flying", Multiplier = 1.4, StaminaMultiplier = 25
		});

		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = octopusProto, PositionId = 1, Alias = "walk", FirstPersonVerb = "walk",
			ThirdPersonVerb = "walks", PresentParticiple = "walking", Multiplier = 1.5, StaminaMultiplier = 1.0
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = octopusProto, PositionId = 1, Alias = "run", FirstPersonVerb = "run",
			ThirdPersonVerb = "runs", PresentParticiple = "running", Multiplier = 1.0, StaminaMultiplier = 2.25
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = octopusProto, PositionId = 6, Alias = "crawl", FirstPersonVerb = "crawl",
			ThirdPersonVerb = "crawls", PresentParticiple = "crawling", Multiplier = 5, StaminaMultiplier = 3.0
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = octopusProto, PositionId = 15, Alias = "climb", FirstPersonVerb = "climb",
			ThirdPersonVerb = "climbs", PresentParticiple = "climbing", Multiplier = 3, StaminaMultiplier = 3
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = octopusProto, PositionId = 16, Alias = "swim", FirstPersonVerb = "swim",
			ThirdPersonVerb = "swims", PresentParticiple = "swimming", Multiplier = 1.5, StaminaMultiplier = 1.5
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = octopusProto, PositionId = 16, Alias = "slowswim",
			FirstPersonVerb = "swim slowly",
			ThirdPersonVerb = "swims slowly", PresentParticiple = "slowly swimming", Multiplier = 2,
			StaminaMultiplier = 1.0
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = octopusProto, PositionId = 16, Alias = "quickswim",
			FirstPersonVerb = "swim quickly",
			ThirdPersonVerb = "swims quickly", PresentParticiple = "swimming quickly", Multiplier = 1.0,
			StaminaMultiplier = 2
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = octopusProto, PositionId = 18, Alias = "fly", FirstPersonVerb = "fly",
			ThirdPersonVerb = "flies", PresentParticiple = "flying", Multiplier = 1.8, StaminaMultiplier = 15
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = octopusProto, PositionId = 18, Alias = "franticfly",
			FirstPersonVerb = "franticly fly", ThirdPersonVerb = "franticly flies",
			PresentParticiple = "franticly flying", Multiplier = 1.4, StaminaMultiplier = 25
		});

		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = cetaceanProto, PositionId = 6, Alias = "flop", FirstPersonVerb = "flop",
			ThirdPersonVerb = "flops", PresentParticiple = "flopping", Multiplier = 6, StaminaMultiplier = 3.0
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = cetaceanProto, PositionId = 16, Alias = "swim", FirstPersonVerb = "swim",
			ThirdPersonVerb = "swims", PresentParticiple = "swimming", Multiplier = 1.5, StaminaMultiplier = 2
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = cetaceanProto, PositionId = 16, Alias = "slowswim",
			FirstPersonVerb = "swim slowly",
			ThirdPersonVerb = "swims slowly", PresentParticiple = "swimming slowly", Multiplier = 2,
			StaminaMultiplier = 1.5
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = cetaceanProto, PositionId = 16, Alias = "quickswim",
			FirstPersonVerb = "swim quickly",
			ThirdPersonVerb = "swims quickly", PresentParticiple = "swimming quickly", Multiplier = 1.0,
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
	#endregion
}