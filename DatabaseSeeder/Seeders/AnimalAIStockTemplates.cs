#nullable enable

using DatabaseSeeder;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;

namespace DatabaseSeeder.Seeders;

internal static class AnimalAIStockTemplates
{
	private const string AnyPreyProgName = "StockAnimalAIAnyEdiblePrey";
	private const string AvoidPeopleProgName = "StockAnimalAIAvoidPeople";
	private const string AquaticCellProgName = "StockAnimalAIAquaticCell";
	private const string AmphibiousCellProgName = "StockAnimalAIAmphibiousCell";
	private const string ShelterCellProgName = "StockAnimalAIShelterCell";
	private const string NestSiteProgName = "StockAnimalAINestSite";

	public const string SmallSkittishForager = "AnimalSmallSkittishForager";
	public const string BurrowingForager = "AnimalBurrowingForager";
	public const string TerritorialGrazer = "AnimalTerritorialGrazer";
	public const string LargeDefensiveGrazer = "AnimalLargeDefensiveGrazer";
	public const string OpportunistOmnivore = "AnimalOpportunistOmnivore";
	public const string NocturnalScavenger = "AnimalNocturnalScavenger";
	public const string TerritorialPredator = "AnimalTerritorialPredator";
	public const string DenningPredator = "AnimalDenningPredator";
	public const string BurrowingAmbushPredator = "AnimalBurrowingAmbushPredator";
	public const string ArborealForager = "AnimalArborealForager";
	public const string ArborealPredator = "AnimalArborealPredator";
	public const string SkittishBird = "AnimalSkittishBird";
	public const string Raptor = "AnimalRaptor";
	public const string EternalFlier = "AnimalEternalFlier";
	public const string FlyingScavenger = "AnimalFlyingScavenger";
	public const string SwimmingForager = "AnimalSwimmingForager";
	public const string SwimmingPredator = "AnimalSwimmingPredator";
	public const string SwimmingScavenger = "AnimalSwimmingScavenger";
	public const string AmphibiousForager = "AnimalAmphibiousForager";
	public const string AmphibiousPredator = "AnimalAmphibiousPredator";
	public const string HuntingOmnivore = "AnimalHuntingOmnivore";
	public const string DenningOmnivore = "AnimalDenningOmnivore";
	public const string SurfaceSwimmingPredator = "AnimalSurfaceSwimmingPredator";
	public const string ShelteringGrazer = "AnimalShelteringGrazer";
	public const string NestingBird = "AnimalNestingBird";
	public const string ParentalDefender = "AnimalParentalDefender";
	public const string PlantlikeForager = "AnimalPlantlikeForager";
	public const string MythicGuardian = "AnimalMythicGuardian";
	public const string SapientPassive = "AnimalSapientPassive";
	public const string MythicFlyingPredator = "AnimalMythicFlyingPredator";

	private static readonly IReadOnlyList<AnimalAIStockTemplateDefinition> StockTemplates =
	[
		new()
		{
			Name = SmallSkittishForager,
			Description = "Small ground foragers that avoid visible threats before food or water.",
			Movement = "Ground",
			Home = "None",
			Feeding = "Forager",
			Threat = "Flee",
			Awareness = "Wimpy",
			Refuge = "None",
			Activity = "Crepuscular",
			MovementRange = 8,
			AwarenessRange = 3,
			AwarenessThreatsAreAnyCharacter = true,
			WanderChancePerMinute = 0.42
		},
		new()
		{
			Name = BurrowingForager,
			Description = "Small denning foragers that retreat to a burrow when fed and watered.",
			Movement = "Ground",
			Home = "Denning",
			Feeding = "Forager",
			Threat = "Flee",
			Awareness = "Wimpy",
			Refuge = "Den",
			Activity = "Crepuscular",
			MovementRange = 8,
			AwarenessRange = 4,
			AwarenessThreatsAreAnyCharacter = true,
			WanderChancePerMinute = 0.30
		},
		new()
		{
			Name = TerritorialGrazer,
			Description = "Ordinary grazers that maintain a home range and usually flee danger.",
			Movement = "Ground",
			Home = "Territorial",
			Feeding = "Forager",
			Threat = "Flee",
			Awareness = "Wary",
			Refuge = "Home",
			Activity = "Diurnal",
			MovementRange = 16,
			AwarenessRange = 5,
			WillShareTerritory = true,
			WillShareTerritoryWithOtherRaces = false
		},
		new()
		{
			Name = LargeDefensiveGrazer,
			Description = "Large grazers that hold a territory and defend themselves if pressed.",
			Movement = "Ground",
			Home = "Territorial",
			Feeding = "Forager",
			Threat = "Defend",
			Awareness = "Wary",
			Refuge = "Home",
			Activity = "Diurnal",
			MovementRange = 18,
			AwarenessRange = 5,
			WillShareTerritory = true,
			WillShareTerritoryWithOtherRaces = false,
			WanderChancePerMinute = 0.24
		},
		new()
		{
			Name = OpportunistOmnivore,
			Description = "Omnivores that forage and scavenge but do not initiate hunts.",
			Movement = "Ground",
			Home = "None",
			Feeding = "Opportunist",
			Threat = "Defend",
			Awareness = "Wary",
			Refuge = "None",
			Activity = "Always",
			MovementRange = 14,
			AwarenessRange = 5
		},
		new()
		{
			Name = HuntingOmnivore,
			Description = "True omnivores that scavenge and forage first, then hunt edible prey when still hungry.",
			Movement = "Ground",
			Home = "Territorial",
			Feeding = "Omnivore",
			Threat = "HungryPredator",
			Awareness = "Wary",
			Refuge = "Home",
			Activity = "Crepuscular",
			MovementRange = 18,
			AwarenessRange = 6,
			WillAttackAnyEdibleTarget = true,
			EngageDelayDiceExpression = "1d700+900",
			EngageEmote = "@ advance|advances warily towards $1."
		},
		new()
		{
			Name = DenningOmnivore,
			Description = "Denning omnivores that forage or scavenge first and drag hunted kills home.",
			Movement = "Ground",
			Home = "Denning",
			Feeding = "DenOmnivore",
			Threat = "HungryPredator",
			Awareness = "Wary",
			Refuge = "Den",
			Activity = "Crepuscular",
			MovementRange = 16,
			AwarenessRange = 6,
			WillAttackAnyEdibleTarget = true,
			EngageDelayDiceExpression = "1d700+900",
			EngageEmote = "@ lumber|lumbers towards $1."
		},
		new()
		{
			Name = NocturnalScavenger,
			Description = "Cautious scavengers that eat corpses or edible scraps without hunting.",
			Movement = "Ground",
			Home = "None",
			Feeding = "Scavenger",
			Threat = "Flee",
			Awareness = "Wary",
			Refuge = "None",
			Activity = "Nocturnal",
			MovementRange = 12,
			AwarenessRange = 4
		},
		new()
		{
			Name = TerritorialPredator,
			Description = "Hungry predators that hunt edible targets inside a home range.",
			Movement = "Ground",
			Home = "Territorial",
			Feeding = "Predator",
			Threat = "HungryPredator",
			Awareness = "Wary",
			Refuge = "Home",
			Activity = "Crepuscular",
			MovementRange = 18,
			AwarenessRange = 6,
			WillAttackAnyEdibleTarget = true,
			EngageDelayDiceExpression = "1d500+750",
			EngageEmote = "@ prowl|prowls towards $1."
		},
		new()
		{
			Name = DenningPredator,
			Description = "Hungry predators that drag kills back to their den before eating.",
			Movement = "Ground",
			Home = "Denning",
			Feeding = "DenPredator",
			Threat = "HungryPredator",
			Awareness = "Wary",
			Refuge = "Den",
			Activity = "Nocturnal",
			MovementRange = 16,
			AwarenessRange = 6,
			WillAttackAnyEdibleTarget = true,
			EngageDelayDiceExpression = "1d600+900",
			EngageEmote = "@ stalk|stalks towards $1."
		},
		new()
		{
			Name = BurrowingAmbushPredator,
			Description = "Low-wandering den predators suited to spiders, scorpions, worms and ambushers.",
			Movement = "Ground",
			Home = "Denning",
			Feeding = "DenPredator",
			Threat = "HungryPredator",
			Awareness = "Wary",
			Refuge = "Den",
			Activity = "Nocturnal",
			MovementRange = 10,
			AwarenessRange = 5,
			WillAttackAnyEdibleTarget = true,
			WanderChancePerMinute = 0.12,
			EngageDelayDiceExpression = "1d800+1200",
			EngageEmote = "@ lunge|lunges suddenly towards $1."
		},
		new()
		{
			Name = ArborealForager,
			Description = "Tree-moving foragers that can descend for food or water and return to branches.",
			Movement = "Arboreal",
			Home = "None",
			Feeding = "Forager",
			Threat = "Flee",
			Awareness = "Wary",
			Refuge = "Trees",
			Activity = "Diurnal",
			MovementRange = 12,
			AwarenessRange = 5,
			AllowDescent = true,
			RefugeLayer = "HighInTrees",
			PreferredTreeLayer = "HighInTrees",
			SecondaryTreeLayer = "InTrees"
		},
		new()
		{
			Name = ArborealPredator,
			Description = "Tree-moving predators that can descend to hunt and return to branches.",
			Movement = "Arboreal",
			Home = "Territorial",
			Feeding = "Predator",
			Threat = "HungryPredator",
			Awareness = "Wary",
			Refuge = "Trees",
			Activity = "Crepuscular",
			MovementRange = 14,
			AwarenessRange = 6,
			AllowDescent = true,
			RefugeLayer = "HighInTrees",
			PreferredTreeLayer = "HighInTrees",
			SecondaryTreeLayer = "InTrees",
			WillAttackAnyEdibleTarget = true,
			EngageDelayDiceExpression = "1d500+700",
			EngageEmote = "@ coil|coils towards $1 from the branches."
		},
		new()
		{
			Name = SkittishBird,
			Description = "Birds that may come down for food or water but flee back into trees.",
			Movement = "Arboreal",
			Home = "None",
			Feeding = "Forager",
			Threat = "Flee",
			Awareness = "Skittish",
			Refuge = "Trees",
			Activity = "Diurnal",
			MovementRange = 14,
			AwarenessRange = 6,
			AllowDescent = true,
			AwarenessThreatsAreAnyCharacter = true,
			RefugeLayer = "HighInTrees",
			PreferredTreeLayer = "HighInTrees",
			SecondaryTreeLayer = "InTrees"
		},
		new()
		{
			Name = Raptor,
			Description = "Flying predators that return to the air when not hunting, eating or drinking.",
			Movement = "Fly",
			Home = "Territorial",
			Feeding = "Predator",
			Threat = "HungryPredator",
			Awareness = "Wary",
			Refuge = "Sky",
			Activity = "Diurnal",
			MovementRange = 35,
			AwarenessRange = 7,
			TargetFlyingLayer = "HighInAir",
			TargetRestingLayer = "InAir",
			RefugeLayer = "HighInAir",
			WillAttackAnyEdibleTarget = true,
			EngageDelayDiceExpression = "1d400+500",
			EngageEmote = "@ dive|dives towards $1."
		},
		new()
		{
			Name = EternalFlier,
			Description = "Long-ranging flyers that prefer to remain in the sky between needs.",
			Movement = "Fly",
			Home = "None",
			Feeding = "Forager",
			Threat = "Flee",
			Awareness = "Wary",
			Refuge = "Sky",
			Activity = "Always",
			MovementRange = 55,
			AwarenessRange = 6,
			TargetFlyingLayer = "HighInAir",
			TargetRestingLayer = "HighInAir",
			RefugeLayer = "HighInAir",
			WanderChancePerMinute = 0.50
		},
		new()
		{
			Name = FlyingScavenger,
			Description = "Flying scavengers that seek corpses without initiating hunts.",
			Movement = "Fly",
			Home = "None",
			Feeding = "Scavenger",
			Threat = "Flee",
			Awareness = "Wary",
			Refuge = "Sky",
			Activity = "Diurnal",
			MovementRange = 40,
			AwarenessRange = 6,
			TargetFlyingLayer = "HighInAir",
			TargetRestingLayer = "InAir",
			RefugeLayer = "HighInAir"
		},
		new()
		{
			Name = SwimmingForager,
			Description = "Aquatic grazers, filter-feeders and passive swimmers.",
			Movement = "Swim",
			Home = "None",
			Feeding = "Forager",
			Water = "Immerse",
			Threat = "Flee",
			Awareness = "Wary",
			Refuge = "Water",
			Activity = "Always",
			MovementRange = 22,
			AwarenessRange = 5
		},
		new()
		{
			Name = SwimmingPredator,
			Description = "Aquatic predators that hunt edible targets when hungry.",
			Movement = "Swim",
			Home = "Territorial",
			Feeding = "Predator",
			Water = "Immerse",
			Threat = "HungryPredator",
			Awareness = "Wary",
			Refuge = "Water",
			Activity = "Always",
			MovementRange = 28,
			AwarenessRange = 6,
			WillAttackAnyEdibleTarget = true,
			EngageDelayDiceExpression = "1d500+600",
			EngageEmote = "@ surge|surges through the water towards $1."
		},
		new()
		{
			Name = SwimmingScavenger,
			Description = "Aquatic scavengers that eat corpses, bodyparts and edible scraps.",
			Movement = "Swim",
			Home = "None",
			Feeding = "Scavenger",
			Water = "Immerse",
			Threat = "Defend",
			Awareness = "Wary",
			Refuge = "Water",
			Activity = "Always",
			MovementRange = 18,
			AwarenessRange = 4
		},
		new()
		{
			Name = AmphibiousForager,
			Description = "Amphibious foragers that split ambient movement between land and water.",
			Movement = "Amphibious",
			Home = "None",
			Feeding = "Forager",
			Water = "Immerse",
			Threat = "Flee",
			Awareness = "Wary",
			Refuge = "Water",
			Activity = "Crepuscular",
			MovementRange = 16,
			WaterBias = 0.65,
			AwarenessRange = 4
		},
		new()
		{
			Name = AmphibiousPredator,
			Description = "Amphibious predators that move between banks and water to hunt.",
			Movement = "Amphibious",
			Home = "Territorial",
			Feeding = "Predator",
			Water = "Surface",
			Threat = "HungryPredator",
			Awareness = "Wary",
			Refuge = "Water",
			Activity = "Crepuscular",
			MovementRange = 20,
			WaterBias = 0.70,
			AwarenessRange = 6,
			WillAttackAnyEdibleTarget = true,
			EngageDelayDiceExpression = "1d600+700",
			EngageEmote = "@ snap|snaps towards $1 from the waterline."
		},
		new()
		{
			Name = SurfaceSwimmingPredator,
			Description = "Air-breathing aquatic predators that prefer surface water when thirsty or travelling.",
			Movement = "Swim",
			Home = "Territorial",
			Feeding = "Predator",
			Water = "Surface",
			Threat = "HungryPredator",
			Awareness = "Wary",
			Refuge = "Water",
			Activity = "Always",
			MovementRange = 28,
			AwarenessRange = 6,
			WillAttackAnyEdibleTarget = true,
			EngageDelayDiceExpression = "1d500+600",
			EngageEmote = "@ surface|surfaces and surges towards $1."
		},
		new()
		{
			Name = ShelteringGrazer,
			Description = "Grazers with a builder-configurable shelter instinct for weather or other unsafe conditions.",
			Movement = "Ground",
			Home = "Territorial",
			Feeding = "Forager",
			Threat = "Flee",
			Awareness = "Wary",
			Refuge = "Home",
			Activity = "Diurnal",
			MovementRange = 16,
			AwarenessRange = 5,
			EcologyShelter = true,
			WillShareTerritory = true,
			WillShareTerritoryWithOtherRaces = false
		},
		new()
		{
			Name = NestingBird,
			Description = "Birds and reptiles that return to a configured nest site when fed and watered.",
			Movement = "Arboreal",
			Home = "Denning",
			Feeding = "Forager",
			Threat = "Flee",
			Awareness = "Skittish",
			Refuge = "Trees",
			Activity = "Diurnal",
			MovementRange = 14,
			AwarenessRange = 6,
			AllowDescent = true,
			AwarenessThreatsAreAnyCharacter = true,
			RefugeLayer = "HighInTrees",
			PreferredTreeLayer = "HighInTrees",
			SecondaryTreeLayer = "InTrees",
			EcologyNesting = true
		},
		new()
		{
			Name = ParentalDefender,
			Description = "Defensive animals that can guard builder-selected young, mates or friends.",
			Movement = "Ground",
			Home = "Territorial",
			Feeding = "Forager",
			Threat = "Defend",
			Awareness = "Guarding",
			Refuge = "Home",
			Activity = "Diurnal",
			MovementRange = 16,
			AwarenessRange = 6,
			EcologyParenting = true,
			WillShareTerritory = true,
			WillShareTerritoryWithOtherRaces = false
		},
		new()
		{
			Name = PlantlikeForager,
			Description = "Plantlike or rooted creatures that browse, drink and defend a small home range.",
			Movement = "Ground",
			Home = "Territorial",
			Feeding = "Forager",
			Threat = "Defend",
			Awareness = "Wary",
			Refuge = "Home",
			Activity = "Diurnal",
			MovementRange = 8,
			AwarenessRange = 4,
			WanderChancePerMinute = 0.10
		},
		new()
		{
			Name = MythicGuardian,
			Description = "Defensive mythic beasts that browse and hold a territory without hunting.",
			Movement = "Ground",
			Home = "Territorial",
			Feeding = "Forager",
			Threat = "Defend",
			Awareness = "Guarding",
			Refuge = "Home",
			Activity = "Always",
			MovementRange = 18,
			AwarenessRange = 6,
			WillShareTerritory = false,
			WillShareTerritoryWithOtherRaces = false
		},
		new()
		{
			Name = SapientPassive,
			Description = "Passive placeholder for sapient mythic folk that builders should customise.",
			Movement = "Ground",
			Home = "None",
			Feeding = "None",
			Threat = "Passive",
			Awareness = "None",
			Refuge = "None",
			Activity = "Always",
			MovementRange = 10
		},
		new()
		{
			Name = MythicFlyingPredator,
			Description = "Large flying mythic predators that hunt when hungry and return to the sky.",
			Movement = "Fly",
			Home = "Territorial",
			Feeding = "Predator",
			Threat = "HungryPredator",
			Awareness = "Wary",
			Refuge = "Sky",
			Activity = "Always",
			MovementRange = 45,
			AwarenessRange = 8,
			TargetFlyingLayer = "HighInAir",
			TargetRestingLayer = "HighInAir",
			RefugeLayer = "HighInAir",
			WillAttackAnyEdibleTarget = true,
			EngageDelayDiceExpression = "1d600+600",
			EngageEmote = "@ wheel|wheels through the air towards $1."
		}
	];

	private static readonly IReadOnlyDictionary<string, AnimalAIStockTemplateDefinition> StockTemplatesByName =
		new ReadOnlyDictionary<string, AnimalAIStockTemplateDefinition>(
			StockTemplates.ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase));

	private static readonly IReadOnlyList<AnimalAIRaceRecommendation> AnimalRecommendations =
		BuildRecommendations(
			(SmallSkittishForager, "small skittish ground forager",
			[
				"Mouse", "Guinea Pig", "Hamster", "Hare", "Shrew", "Lizard", "Gecko", "Skink",
				"Grasshopper"
			]),
			(BurrowingForager, "burrow or retreat-oriented forager",
			[
				"Rabbit", "Beaver", "Tortoise"
			]),
			(TerritorialGrazer, "ordinary territorial grazer",
			[
				"Cow", "Elk", "Giraffe", "Goat", "Horse", "Llama", "Ox"
			]),
			(LargeDefensiveGrazer, "large defensive grazer or browser",
			[
				"Bison", "Buffalo", "Mammoth", "Moose", "Rhinocerous", "Warthog"
			]),
			(OpportunistOmnivore, "opportunistic omnivore or light scavenger",
			[
				"Ant", "Beetle"
			]),
			(HuntingOmnivore, "hunting omnivore",
			[
				"Bear", "Boar", "Coyote", "Dog", "Hyena", "Jackal", "Pig", "Rat"
			]),
			(ShelteringGrazer, "weather-sensitive or sheltering grazer",
			[
				"Alpaca", "Camel", "Deer", "Reindeer", "Sheep"
			]),
			(ParentalDefender, "parental or defensive large animal",
			[
				"Elephant", "Emu", "Goose", "Moa", "Ostrich", "Swan"
			]),
			(AmphibiousForager, "amphibious forager",
			[
				"Frog", "Hippopotamus", "Toad", "Turtle"
			]),
			(NocturnalScavenger, "cautious scavenger",
			[
				"Cockroach"
			]),
			(TerritorialPredator, "ground predator",
			[
				"Adder", "Anaconda", "Boa", "Cat", "Centipede", "Cheetah", "Cobra", "Coral Snake", "Jaguar",
				"Leopard", "Lion", "Mamba", "Mantis", "Moccasin", "Monitor Lizard", "Panther", "Python",
				"Rattlesnake", "Sabretooth Tiger", "Tiger", "Viper", "Wolf"
			]),
			(DenningPredator, "denning predator or mustelid-like hunter",
			[
				"Badger", "Ferret", "Fox", "Mink", "Polecat", "Stoat", "Weasel", "Wolverine"
			]),
			(BurrowingAmbushPredator, "burrowing or low-wandering ambush predator",
			[
				"Scorpion", "Spider", "Tarantula"
			]),
			(ArborealForager, "tree-oriented forager",
			[
				"Iguana", "Parrot", "Woodpecker"
			]),
			(ArborealPredator, "tree-oriented predator",
			[
				"Tree Python"
			]),
			(SkittishBird, "skittish ground-feeding bird",
			[
				"Crane", "Flamingo", "Heron", "Ibis", "Peacock", "Pelican", "Stork"
			]),
			(NestingBird, "nesting bird or small reptile",
			[
				"Chicken", "Duck", "Finch", "Grouse", "Pheasant", "Pigeon", "Quail", "Robin", "Sparrow",
				"Turkey", "Wren"
			]),
			(Raptor, "flying predator",
			[
				"Eagle", "Falcon", "Hawk", "Kingfisher", "Owl"
			]),
			(EternalFlier, "long-ranging or primarily airborne forager",
			[
				"Albatross", "Bee", "Butterfly", "Dragonfly", "Hornet", "Moth", "Seagull", "Swallow", "Wasp"
			]),
			(FlyingScavenger, "flying scavenger",
			[
				"Crow", "Raven", "Vulture"
			]),
			(SwimmingForager, "aquatic forager or filter-feeder",
			[
				"Anchovy", "Baleen Whale", "Carp", "Herring", "Koi", "Mackerel", "Pilchard", "Salmon",
				"Sardine"
			]),
			(SwimmingPredator, "aquatic predator",
			[
				"Cod", "Giant Squid", "Haddock", "Octopus", "Perch", "Pollock", "Shark", "Squid", "Tuna"
			]),
			(AmphibiousPredator, "amphibious predator",
			[
				"Alligator", "Crocodile", "Otter", "Penguin"
			]),
			(SurfaceSwimmingPredator, "surface-breathing aquatic predator",
			[
				"Dolphin", "Orca", "Porpoise", "Sea Lion", "Seal", "Toothed Whale", "Walrus"
			]),
			(SwimmingScavenger, "aquatic scavenger",
			[
				"Crab", "Crayfish", "Giant Crab", "Jellyfish", "Lobster", "Prawn", "Shrimp", "Small Crab"
			])
		);

	private static readonly IReadOnlyList<AnimalAIRaceRecommendation> MythicalRecommendations =
		BuildRecommendations(
			(MythicFlyingPredator, "large flying mythic predator",
			[
				"Cockatrice", "Dragon", "Eastern Dragon", "Griffin", "Hippogriff", "Manticore", "Wyvern"
			]),
			(EternalFlier, "sky-returning mythic flier",
			[
				"Phoenix"
			]),
			(MythicGuardian, "defensive mythic guardian or browser",
			[
				"Centaur", "Minotaur", "Pegacorn", "Pegasus", "Unicorn"
			]),
			(DenningPredator, "denning mythic predator",
			[
				"Dire-Wolf", "Warg"
			]),
			(DenningOmnivore, "denning mythic omnivore",
			[
				"Dire-Bear"
			]),
			(TerritorialPredator, "territorial mythic predator",
			[
				"Basilisk", "Giant Mantis"
			]),
			(BurrowingAmbushPredator, "burrowing or ambush mythic predator",
			[
				"Ankheg", "Colossal Worm", "Giant Centipede", "Giant Scorpion", "Giant Spider", "Giant Worm"
			]),
			(LargeDefensiveGrazer, "large defensive arthropod or beast",
			[
				"Giant Beetle"
			]),
			(HuntingOmnivore, "opportunistic colony or omnivore beast",
			[
				"Giant Ant"
			]),
			(SwimmingForager, "aquatic mythic forager",
			[
				"Hippocamp"
			]),
			(PlantlikeForager, "plantlike or tree-spirit forager",
			[
				"Dryad", "Ent", "Myconid", "Plantfolk"
			]),
			(SapientPassive, "sapient mythic folk placeholder",
			[
				"Avian Person", "Mermaid", "Naga", "Owlkin", "Selkie"
			])
		);

	internal static IReadOnlyList<AnimalAIStockTemplateDefinition> TemplatesForTesting => StockTemplates;

	internal static IReadOnlyDictionary<string, string> AnimalRecommendationsForTesting =>
		AnimalRecommendations.ToDictionary(x => x.RaceName, x => x.TemplateName, StringComparer.OrdinalIgnoreCase);

	internal static IReadOnlyDictionary<string, string> MythicalRecommendationsForTesting =>
		MythicalRecommendations.ToDictionary(x => x.RaceName, x => x.TemplateName, StringComparer.OrdinalIgnoreCase);

	internal static IReadOnlyCollection<string> AnimalTemplateNamesForTesting =>
		AnimalRecommendations.Select(x => x.TemplateName).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(x => x)
		                     .ToList();

	internal static IReadOnlyCollection<string> MythicalTemplateNamesForTesting =>
		MythicalRecommendations.Select(x => x.TemplateName).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(x => x)
		                       .ToList();

	internal static IReadOnlyCollection<string> AllRecommendedTemplateNamesForTesting =>
		AnimalRecommendations.Concat(MythicalRecommendations).Select(x => x.TemplateName)
		                     .Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(x => x).ToList();

	internal static ShouldSeedResult ClassifyPresence(FuturemudDatabaseContext context, IEnumerable<string> templateNames)
	{
		return SeederRepeatabilityHelper.ClassifyByPresence(
			templateNames
				.Distinct(StringComparer.OrdinalIgnoreCase)
				.Select(name => context.ArtificialIntelligences.Any(x => x.Name == name)));
	}

	internal static void SeedAnimalTemplates(FuturemudDatabaseContext context)
	{
		SeedTemplates(context, AnimalRecommendations.Select(x => x.TemplateName));
	}

	internal static void SeedMythicalTemplates(FuturemudDatabaseContext context)
	{
		SeedTemplates(context, MythicalRecommendations.Select(x => x.TemplateName));
	}

	internal static void SeedTemplates(FuturemudDatabaseContext context, IEnumerable<string> templateNames)
	{
		EnsureSupportProgs(context);

		long alwaysTrueId = context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue").Id;
		long alwaysFalseId = context.FutureProgs.First(x => x.FunctionName == "AlwaysFalse").Id;
		long alwaysOneId = context.FutureProgs.FirstOrDefault(x => x.FunctionName == "AlwaysOne")?.Id ?? 0;

		foreach (string name in templateNames.Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(x => x))
		{
			if (!StockTemplatesByName.TryGetValue(name, out AnimalAIStockTemplateDefinition? template))
			{
				throw new InvalidOperationException($"No stock AnimalAI template named {name} has been defined.");
			}

			ArtificialIntelligence ai = SeederRepeatabilityHelper.EnsureNamedEntity(
				context.ArtificialIntelligences,
				template.Name,
				x => x.Name,
				() =>
				{
					ArtificialIntelligence created = new();
					context.ArtificialIntelligences.Add(created);
					return created;
				});

			ai.Name = template.Name;
			ai.Type = "Animal";
			ai.Definition = template.BuildDefinition(alwaysTrueId, alwaysFalseId, alwaysOneId);
		}

		context.SaveChanges();
	}

	private static void EnsureSupportProgs(FuturemudDatabaseContext context)
	{
		SeederRepeatabilityHelper.EnsureProg(
			context,
			AnyPreyProgName,
			"Animal AI",
			"Stock Filters",
			ProgVariableTypes.Boolean,
			"Builder-clonable starter predator filter for stock AnimalAI templates.",
			"return true;",
			false,
			false,
			FutureProgStaticType.NotStatic,
			(ProgVariableTypes.Character, "animal"),
			(ProgVariableTypes.Character, "target"));

		SeederRepeatabilityHelper.EnsureProg(
			context,
			AvoidPeopleProgName,
			"Animal AI",
			"Stock Filters",
			ProgVariableTypes.Boolean,
			"Builder-clonable starter threat filter for animals that avoid non-animal characters.",
			"return not isanimal(@target);",
			false,
			false,
			FutureProgStaticType.NotStatic,
			(ProgVariableTypes.Character, "animal"),
			(ProgVariableTypes.Character, "target"));

		SeederRepeatabilityHelper.EnsureProg(
			context,
			AquaticCellProgName,
			"Animal AI",
			"Stock Filters",
			ProgVariableTypes.Boolean,
			"Builder-clonable starter cell filter for aquatic AnimalAI movement.",
			"return isunderwater(@cell, \"Underwater\");",
			false,
			false,
			FutureProgStaticType.NotStatic,
			(ProgVariableTypes.Character, "animal"),
			(ProgVariableTypes.Location, "cell"));

		SeederRepeatabilityHelper.EnsureProg(
			context,
			AmphibiousCellProgName,
			"Animal AI",
			"Stock Filters",
			ProgVariableTypes.Boolean,
			"Builder-clonable starter cell filter for amphibious AnimalAI movement.",
			"return true;",
			false,
			false,
			FutureProgStaticType.NotStatic,
			(ProgVariableTypes.Character, "animal"),
			(ProgVariableTypes.Location, "cell"));

		SeederRepeatabilityHelper.EnsureProg(
			context,
			ShelterCellProgName,
			"Animal AI",
			"Stock Filters",
			ProgVariableTypes.Boolean,
			"Builder-clonable starter cell filter for sheltered AnimalAI behaviour.",
			"return true;",
			false,
			false,
			FutureProgStaticType.NotStatic,
			(ProgVariableTypes.Character, "animal"),
			(ProgVariableTypes.Location, "cell"));

		SeederRepeatabilityHelper.EnsureProg(
			context,
			NestSiteProgName,
			"Animal AI",
			"Stock Filters",
			ProgVariableTypes.Boolean,
			"Builder-clonable starter cell filter for nesting AnimalAI behaviour.",
			"return true;",
			false,
			false,
			FutureProgStaticType.NotStatic,
			(ProgVariableTypes.Character, "animal"),
			(ProgVariableTypes.Location, "cell"));
	}

	private static IReadOnlyList<AnimalAIRaceRecommendation> BuildRecommendations(
		params (string TemplateName, string Rationale, string[] Races)[] groups)
	{
		return groups
		       .SelectMany(group => group.Races.Select(race =>
			       new AnimalAIRaceRecommendation(race, group.TemplateName, group.Rationale)))
		       .OrderBy(x => x.RaceName, StringComparer.OrdinalIgnoreCase)
		       .ToList();
	}
}

internal sealed record AnimalAIRaceRecommendation(
	string RaceName,
	string TemplateName,
	string Rationale
);

internal sealed class AnimalAIStockTemplateDefinition
{
	public required string Name { get; init; }
	public required string Description { get; init; }
	public required string Movement { get; init; }
	public required string Home { get; init; }
	public required string Feeding { get; init; }
	public required string Threat { get; init; }
	public required string Awareness { get; init; }
	public required string Refuge { get; init; }
	public required string Activity { get; init; }
	public string Water { get; init; } = string.Empty;
	public bool WaterEnabled { get; init; } = true;
	public int MovementRange { get; init; } = 10;
	public double WaterBias { get; init; } = 0.50;
	public double WanderChancePerMinute { get; init; } = 0.33;
	public bool AllowDescent { get; init; }
	public string TargetFlyingLayer { get; init; } = "InAir";
	public string TargetRestingLayer { get; init; } = "HighInTrees";
	public string PreferredTreeLayer { get; init; } = "HighInTrees";
	public string SecondaryTreeLayer { get; init; } = "InTrees";
	public bool WillShareTerritory { get; init; }
	public bool WillShareTerritoryWithOtherRaces { get; init; } = true;
	public bool WillAttackAnyEdibleTarget { get; init; }
	public string EngageDelayDiceExpression { get; init; } = "1000+1d1000";
	public string EngageEmote { get; init; } = string.Empty;
	public bool AwarenessThreatsAreAnyCharacter { get; init; }
	public int AwarenessRange { get; init; } = 5;
	public int AwarenessMemoryMinutes { get; init; } = 10;
	public string RefugeLayer { get; init; } = "HighInTrees";
	public int RefugeReturnSeconds { get; init; } = 60;
	public bool ActivitySleepEnabled { get; init; }
	public string ActivityRestEmote { get; init; } = string.Empty;
	public bool EcologyShelter { get; init; }
	public bool EcologySeasonal { get; init; }
	public bool EcologyNesting { get; init; }
	public bool EcologyParenting { get; init; }

	public string BuildDefinition(long alwaysTrueId, long alwaysFalseId, long alwaysOneId)
	{
		long attackProgId = WillAttackAnyEdibleTarget ? alwaysTrueId : alwaysFalseId;
		long awarenessThreatProgId = AwarenessThreatsAreAnyCharacter ? alwaysTrueId : alwaysFalseId;
		long descentProgId = AllowDescent ? alwaysTrueId : alwaysFalseId;
		string waterMode = string.IsNullOrWhiteSpace(Water)
			? WaterEnabled ? "Drink" : "Off"
			: Water;

		return new XElement("Definition",
			new XComment(Description),
			new XElement("Movement",
				new XAttribute("type", Movement),
				new XElement("Range", MovementRange),
				new XElement("AmphibiousWaterBias", WaterBias),
				new XElement("WanderChancePerMinute", WanderChancePerMinute),
				new XElement("WanderEmote", new XCData(string.Empty)),
				new XElement("MovementEnabledProg", alwaysTrueId),
				new XElement("MovementCellProg", alwaysTrueId),
				new XElement("AmphibiousLandCellProg", alwaysTrueId),
				new XElement("AmphibiousWaterCellProg", alwaysTrueId),
				new XElement("AllowDescentProg", descentProgId),
				new XElement("TargetFlyingLayer", TargetFlyingLayer),
				new XElement("TargetRestingLayer", TargetRestingLayer),
				new XElement("PreferredTreeLayer", PreferredTreeLayer),
				new XElement("SecondaryTreeLayer", SecondaryTreeLayer)),
			new XElement("Home",
				new XAttribute("type", Home),
				new XElement("SuitableTerritoryProg", alwaysTrueId),
				new XElement("DesiredTerritorySizeProg", alwaysOneId),
				new XElement("WillShareTerritory", WillShareTerritory),
				new XElement("WillShareTerritoryWithOtherRaces", WillShareTerritoryWithOtherRaces),
				new XElement("BurrowCraftId", 0),
				new XElement("BurrowSiteProg", alwaysTrueId),
				new XElement("BuildEnabledProg", alwaysTrueId),
				new XElement("HomeLocationProg", 0),
				new XElement("AnchorItemProg", 0)),
			new XElement("Feeding",
				new XAttribute("type", Feeding),
				new XElement("WillAttackProg", attackProgId),
				new XElement("EngageDelayDiceExpression", new XCData(EngageDelayDiceExpression)),
				new XElement("EngageEmote", new XCData(EngageEmote))),
			new XElement("Water", new XAttribute("type", waterMode)),
			new XElement("Threat", new XAttribute("type", Threat)),
			new XElement("Awareness",
				new XAttribute("type", Awareness),
				new XElement("ThreatProg", awarenessThreatProgId),
				new XElement("AvoidCellProg", alwaysFalseId),
				new XElement("Range", AwarenessRange),
				new XElement("MemoryMinutes", AwarenessMemoryMinutes)),
			new XElement("Refuge",
				new XAttribute("type", Refuge),
				new XElement("Layer", RefugeLayer),
				new XElement("CellProg", alwaysFalseId),
				new XElement("ReturnSeconds", RefugeReturnSeconds)),
			new XElement("Activity",
				new XAttribute("type", Activity),
				new XElement("SleepEnabled", ActivitySleepEnabled),
				new XElement("RestEmote", new XCData(ActivityRestEmote))),
			new XElement("Ecology",
				new XElement("ShelterEnabled", EcologyShelter),
				new XElement("SeasonalEnabled", EcologySeasonal),
				new XElement("NestingEnabled", EcologyNesting),
				new XElement("ParentingEnabled", EcologyParenting),
				new XElement("ShelterNeededProg", EcologyShelter ? alwaysTrueId : alwaysFalseId),
				new XElement("ShelterCellProg", EcologyShelter ? alwaysTrueId : alwaysFalseId),
				new XElement("SeasonalCellProg", EcologySeasonal ? alwaysTrueId : alwaysFalseId),
				new XElement("NestSiteProg", EcologyNesting ? alwaysTrueId : alwaysFalseId),
				new XElement("ProtectProg", EcologyParenting ? alwaysTrueId : alwaysFalseId)),
			new XElement("OpenDoors", false),
			new XElement("UseKeys", false),
			new XElement("SmashLockedDoors", false),
			new XElement("CloseDoorsBehind", false),
			new XElement("UseDoorguards", false),
			new XElement("MoveEvenIfObstructionInWay", false)
		).ToString(SaveOptions.DisableFormatting);
	}
}
