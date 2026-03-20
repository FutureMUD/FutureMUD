#nullable enable

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MudSharp.Body;
using MudSharp.Form.Characteristics;
using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public partial class MythicalAnimalSeeder
{
	internal sealed record MythicalAgeProfile(
		int ChildAge,
		int YouthAge,
		int YoungAdultAge,
		int AdultAge,
		int ElderAge,
		int VenerableAge
	);

	internal sealed record MythicalAttackTemplate(
		string AttackName,
		ItemQuality Quality,
		IReadOnlyList<string> BodypartAliases
	);

	internal sealed record MythicalBodypartUsageTemplate(
		string BodypartAlias,
		string Usage
	);

	internal sealed record MythicalCharacteristicTemplate(
		string DefinitionName,
		IReadOnlyList<string> Values,
		string Usage = "base",
		CharacteristicType Type = CharacteristicType.Standard
	);

	internal sealed record MythicalRaceTemplate(
		string Name,
		string BodyKey,
		SizeCategory Size,
		string MaleHeightWeightModel,
		string FemaleHeightWeightModel,
		MythicalAgeProfile AgeProfile,
		bool HumanoidVariety,
		bool CanUseWeapons,
		bool CanClimb,
		bool CanSwim,
		bool Playable,
		string Description,
		string? ShortDescriptionPattern,
		string? FullDescriptionPattern,
		IReadOnlyList<MythicalAttackTemplate> Attacks,
		IReadOnlyList<MythicalBodypartUsageTemplate>? BodypartUsages = null,
		IReadOnlyList<string>? PersonWords = null,
		string? FacialHairProfileName = null,
		IReadOnlyList<MythicalCharacteristicTemplate>? AdditionalCharacteristics = null,
		string CombatStrategyKey = "Beast Brawler"
	);

	internal static IReadOnlyDictionary<string, MythicalRaceTemplate> TemplatesForTesting => Templates;

	private static readonly IReadOnlyDictionary<string, MythicalRaceTemplate> Templates =
		new ReadOnlyDictionary<string, MythicalRaceTemplate>(
			BuildTemplates()
		);

	private static Dictionary<string, MythicalRaceTemplate> BuildTemplates()
	{
		static MythicalAgeProfile StandardHumanoid() => new(2, 6, 12, 18, 55, 80);
		static MythicalAgeProfile LongLivedHumanoid() => new(4, 10, 20, 35, 120, 180);
		static MythicalAgeProfile GreatBeast() => new(2, 8, 16, 30, 120, 200);
		static MythicalAgeProfile Beast() => new(1, 4, 8, 16, 40, 70);
		static MythicalAttackTemplate Attack(string name, ItemQuality quality, params string[] aliases) =>
			new(name, quality, aliases);
		static MythicalBodypartUsageTemplate Usage(string alias, string usage) => new(alias, usage);
		static MythicalCharacteristicTemplate Characteristic(string name, params string[] values) =>
			new(name, values);
		static MythicalRaceTemplate BeastRace(
			string name,
			string bodyKey,
			SizeCategory size,
			string model,
			MythicalAgeProfile ageProfile,
			string description,
			string shortDesc,
			string fullDesc,
			IReadOnlyList<MythicalAttackTemplate> attacks,
			IReadOnlyList<MythicalBodypartUsageTemplate>? usages = null,
			bool canClimb = false,
			bool canSwim = true,
			bool playable = false,
			IReadOnlyList<MythicalCharacteristicTemplate>? additionalCharacteristics = null,
			string combatStrategyKey = "Beast Brawler")
			=> new(
				name,
				bodyKey,
				size,
				model,
				model,
				ageProfile,
				false,
				false,
				canClimb,
				canSwim,
				playable,
				description,
				shortDesc,
				fullDesc,
				attacks,
				usages,
				null,
				null,
				additionalCharacteristics,
				combatStrategyKey
			);
		static MythicalRaceTemplate HumanoidRace(
			string name,
			string bodyKey,
			SizeCategory size,
			string maleModel,
			string femaleModel,
			MythicalAgeProfile ageProfile,
			string description,
			IReadOnlyList<MythicalAttackTemplate> attacks,
			IReadOnlyList<string> personWords,
			IReadOnlyList<MythicalBodypartUsageTemplate>? usages = null,
			bool canClimb = false,
			bool canSwim = true,
			string? facialHairProfile = null,
			string combatStrategyKey = "Melee (Auto)")
			=> new(
				name,
				bodyKey,
				size,
				maleModel,
				femaleModel,
				ageProfile,
				true,
				true,
				canClimb,
				canSwim,
				true,
				description,
				null,
				null,
				attacks,
				usages,
				personWords,
				facialHairProfile,
				null,
				combatStrategyKey
			);
		static MythicalRaceTemplate SapientRace(
			string name,
			string bodyKey,
			SizeCategory size,
			string maleModel,
			string femaleModel,
			MythicalAgeProfile ageProfile,
			string description,
			string shortDesc,
			string fullDesc,
			IReadOnlyList<MythicalAttackTemplate> attacks,
			IReadOnlyList<MythicalBodypartUsageTemplate>? usages = null,
			bool canClimb = false,
			bool canSwim = true,
			IReadOnlyList<MythicalCharacteristicTemplate>? additionalCharacteristics = null,
			string combatStrategyKey = "Melee (Auto)")
			=> new(
				name,
				bodyKey,
				size,
				maleModel,
				femaleModel,
				ageProfile,
				false,
				true,
				canClimb,
				canSwim,
				true,
				description,
				shortDesc,
				fullDesc,
				attacks,
				usages,
				null,
				null,
				additionalCharacteristics,
				combatStrategyKey
			);

		return new Dictionary<string, MythicalRaceTemplate>(StringComparer.OrdinalIgnoreCase)
		{
			["Dragon"] = BeastRace(
				"Dragon",
				"Toed Quadruped",
				SizeCategory.VeryLarge,
				"Horse",
				GreatBeast(),
				"Dragons are immense, winged reptiles with claws, horns and a powerful tail.",
				"a dragon",
				"This colossal draconic beast combines a heavily muscled quadrupedal frame with broad wings, curving horns and a predator's jaws.",
				[
					Attack("Carnivore Bite", ItemQuality.Legendary, "mouth"),
					Attack("Dragonfire Breath", ItemQuality.Legendary, "mouth"),
					Attack("Bite", ItemQuality.Good, "mouth"),
					Attack("Claw Swipe", ItemQuality.Great, "rfpaw", "lfpaw", "rrpaw", "lrpaw"),
					Attack("Horn Gore", ItemQuality.Great, "rhorn", "lhorn"),
					Attack("Tail Slap", ItemQuality.Good, "ltail"),
					Attack("Wing Buffet", ItemQuality.Good, "rwingbase", "lwingbase")
				],
				[
					Usage("rhorn", "general"),
					Usage("lhorn", "general"),
					Usage("rwingbase", "general"),
					Usage("lwingbase", "general"),
					Usage("rwing", "general"),
					Usage("lwing", "general")
				],
				additionalCharacteristics:
				[
					Characteristic("Scale Colour", "red", "green", "black", "gold")
				],
				combatStrategyKey: "Beast Artillery"
			),
			["Griffin"] = BeastRace(
				"Griffin",
				"Griffin",
				SizeCategory.Large,
				"Big Felid",
				Beast(),
				"Griffins combine an eagle's head and wings with a leonine hindbody and foreclaws.",
				"a griffin",
				"This imposing mythic predator bears a hooked avian beak and far-seeing eyes above a leonine quadruped body built for bounding flight and savage pounces.",
				[
					Attack("Beak Peck", ItemQuality.Good, "beak"),
					Attack("Beak Bite", ItemQuality.Standard, "beak"),
					Attack("Claw Swipe", ItemQuality.Good, "rfpaw", "lfpaw", "rrpaw", "lrpaw"),
					Attack("Tail Slap", ItemQuality.Standard, "ltail"),
					Attack("Wing Buffet", ItemQuality.Standard, "rwingbase", "lwingbase")
				],
				combatStrategyKey: "Beast Swooper"
			),
			["Hippogriff"] = BeastRace(
				"Hippogriff",
				"Hippogriff",
				SizeCategory.Large,
				"Horse",
				Beast(),
				"Hippogriffs blend an eagle's forequarters and wings with an equine lower body.",
				"a hippogriff",
				"This powerful hybrid has an avian head and beating wings set atop an equine frame, with hoofed legs built equally for galloping starts and airborne strikes.",
				[
					Attack("Beak Peck", ItemQuality.Standard, "beak"),
					Attack("Beak Bite", ItemQuality.Poor, "beak"),
					Attack("Hoof Stomp", ItemQuality.Good, "rfhoof", "lfhoof", "rrhoof", "lrhoof"),
					Attack("Wing Buffet", ItemQuality.Standard, "rwingbase", "lwingbase")
				],
				combatStrategyKey: "Beast Swooper"
			),
			["Unicorn"] = BeastRace(
				"Unicorn",
				"Ungulate",
				SizeCategory.Large,
				"Horse",
				GreatBeast(),
				"Unicorns are horse-like beings distinguished by a single spiralled horn and uncanny grace.",
				"a unicorn",
				"This elegant, equine myth-beast carries itself with the poise of a fine horse, its singular horn and bright, intelligent eyes marking it as something far stranger.",
				[
					Attack("Bite", ItemQuality.Bad, "mouth"),
					Attack("Hoof Stomp", ItemQuality.Good, "rfhoof", "lfhoof", "rrhoof", "lrhoof"),
					Attack("Horn Gore", ItemQuality.Good, "horn")
				],
				[
					Usage("horn", "general")
				],
				combatStrategyKey: "Beast Behemoth"
			),
			["Pegasus"] = BeastRace(
				"Pegasus",
				"Ungulate",
				SizeCategory.Large,
				"Horse",
				GreatBeast(),
				"Pegasi are winged horses capable of powerful, sustained flight.",
				"a pegasus",
				"This broad-winged equine is all coiled athletic power, its feathered wings and strong hooves making it as dangerous in the air as on the ground.",
				[
					Attack("Bite", ItemQuality.Bad, "mouth"),
					Attack("Hoof Stomp", ItemQuality.Good, "rfhoof", "lfhoof", "rrhoof", "lrhoof"),
					Attack("Head Ram", ItemQuality.Standard, "head"),
					Attack("Wing Buffet", ItemQuality.Standard, "rwingbase", "lwingbase")
				],
				[
					Usage("rwingbase", "general"),
					Usage("lwingbase", "general"),
					Usage("rwing", "general"),
					Usage("lwing", "general")
				],
				combatStrategyKey: "Beast Swooper"
			),
			["Minotaur"] = HumanoidRace(
				"Minotaur",
				"Horned Humanoid",
				SizeCategory.Normal,
				"Human Male",
				"Human Female",
				StandardHumanoid(),
				"Minotaurs are broad, horned humanoids with a bestial cast to their features and physiques.",
				[
					Attack("Horn Gore", ItemQuality.Standard, "rhorn", "lhorn"),
					Attack("Head Ram", ItemQuality.Standard, "head"),
					Attack("Elbow", ItemQuality.Standard, "relbow", "lelbow")
				],
				["minotaur"],
				[
					Usage("rhorn", "general"),
					Usage("lhorn", "general")
				]
			),
			["Eastern Dragon"] = BeastRace(
				"Eastern Dragon",
				"Eastern Dragon",
				SizeCategory.VeryLarge,
				"Horse",
				GreatBeast(),
				"Eastern dragons are long, sinuous drakes that prowl on four clawed limbs without relying on wings for flight.",
				"an eastern dragon",
				"This immense draconic predator combines a serpentine cast and a powerful quadrupedal frame, with taloned feet, a long body and a sweeping tail in place of wings.",
				[
					Attack("Carnivore Bite", ItemQuality.Legendary, "mouth"),
					Attack("Dragonfire Breath", ItemQuality.Legendary, "mouth"),
					Attack("Bite", ItemQuality.Good, "mouth"),
					Attack("Claw Swipe", ItemQuality.Great, "rfpaw", "lfpaw", "rrpaw", "lrpaw"),
					Attack("Tail Slap", ItemQuality.Good, "ltail")
				],
				additionalCharacteristics:
				[
					Characteristic("Scale Colour", "red", "green", "black", "gold")
				],
				combatStrategyKey: "Beast Artillery"
			),
			["Naga"] = HumanoidRace(
				"Naga",
				"Naga",
				SizeCategory.Normal,
				"Human Male",
				"Human Female",
				LongLivedHumanoid(),
				"Naga are humanoid from the waist up, with serpentine lower bodies and a sinuous, coiled bearing.",
				[
					Attack("Carnivore Bite", ItemQuality.Standard, "mouth"),
					Attack("Bite", ItemQuality.Standard, "mouth"),
					Attack("Tail Slap", ItemQuality.Standard, "tail")
				],
				["naga"],
				canClimb: true,
				combatStrategyKey: "Melee (Auto)"
			),
			["Mermaid"] = HumanoidRace(
				"Mermaid",
				"Mermaid",
				SizeCategory.Normal,
				"Human Male",
				"Human Female",
				LongLivedHumanoid(),
				"Merfolk have human torsos and arms paired with powerful piscine tails built for swimming.",
				[
					Attack("Carnivore Bite", ItemQuality.Bad, "mouth"),
					Attack("Bite", ItemQuality.Bad, "mouth"),
					Attack("Tail Slap", ItemQuality.Good, "caudalfin")
				],
				["merfolk"],
				combatStrategyKey: "Melee (Auto)"
			),
			["Manticore"] = BeastRace(
				"Manticore",
				"Manticore",
				SizeCategory.Large,
				"Big Felid",
				GreatBeast(),
				"Manticores are broad-winged leonine predators with a venomous tail-spike.",
				"a manticore",
				"This winged predator couples a leonine quadruped body and raking claws with a barbed stinger poised at the end of its tail.",
				[
					Attack("Carnivore Bite", ItemQuality.Good, "mouth"),
					Attack("Bite", ItemQuality.Standard, "mouth"),
					Attack("Claw Swipe", ItemQuality.Good, "rfpaw", "lfpaw", "rrpaw", "lrpaw"),
					Attack("Tail Slap", ItemQuality.Good, "ltail"),
					Attack("Tail Spike", ItemQuality.Good, "stinger"),
					Attack("Wing Buffet", ItemQuality.Standard, "rwingbase", "lwingbase")
				],
				[
					Usage("rwingbase", "general"),
					Usage("lwingbase", "general"),
					Usage("rwing", "general"),
					Usage("lwing", "general"),
					Usage("stinger", "general")
				],
				combatStrategyKey: "Beast Artillery"
			),
			["Wyvern"] = BeastRace(
				"Wyvern",
				"Wyvern",
				SizeCategory.Large,
				"Raptor",
				GreatBeast(),
				"Wyverns are draconic two-legged fliers, all leathery wings, grasping talons and snapping jaws.",
				"a wyvern",
				"This draconic flyer stands on powerful taloned legs beneath a scaled torso, its jaws and whipping tail making close quarters especially dangerous.",
				[
					Attack("Carnivore Bite", ItemQuality.Good, "mouth"),
					Attack("Dragonfire Breath", ItemQuality.Good, "mouth"),
					Attack("Bite", ItemQuality.Standard, "mouth"),
					Attack("Talon Strike", ItemQuality.Good, "rtalons", "ltalons"),
					Attack("Tail Slap", ItemQuality.Standard, "tail"),
					Attack("Wing Buffet", ItemQuality.Standard, "rwingbase", "lwingbase")
				],
				combatStrategyKey: "Beast Artillery"
			),
			["Phoenix"] = BeastRace(
				"Phoenix",
				"Avian",
				SizeCategory.Normal,
				"Raptor",
				GreatBeast(),
				"Phoenixes are radiant birds of fire and ash, here seeded without any resurrection-specific mechanics.",
				"a phoenix",
				"This majestic firebird has an avian frame and proud bearing, its whole presence suggesting heat, renewal and dangerous beauty.",
				[
					Attack("Beak Peck", ItemQuality.Good, "beak"),
					Attack("Beak Bite", ItemQuality.Standard, "beak"),
					Attack("Talon Strike", ItemQuality.Good, "rtalons", "ltalons")
				],
				combatStrategyKey: "Beast Swooper"
			),
			["Basilisk"] = BeastRace(
				"Basilisk",
				"Serpentine",
				SizeCategory.Normal,
				"Serpent",
				GreatBeast(),
				"Basilisks are immense, sinister serpents famed for their malignant aspect and deadly bite.",
				"a basilisk",
				"This huge serpent drapes itself in heavy coils and watches with an unsettling, predatory stillness that makes its sudden strikes all the worse.",
				[
					Attack("Carnivore Bite", ItemQuality.Good, "mouth"),
					Attack("Bite", ItemQuality.Standard, "mouth"),
					Attack("Tail Slap", ItemQuality.Standard, "tail")
				],
				combatStrategyKey: "Beast Clincher"
			),
			["Cockatrice"] = BeastRace(
				"Cockatrice",
				"Avian",
				SizeCategory.Small,
				"Small Bird",
				Beast(),
				"Cockatrices are vicious little reptilian birds with pecking beaks and slashing talons.",
				"a cockatrice",
				"This wiry, ill-tempered creature has an avian body and a reptilian cast to its features, all sharp beak, clawed feet and restless hostility.",
				[
					Attack("Beak Peck", ItemQuality.Standard, "beak"),
					Attack("Beak Bite", ItemQuality.Terrible, "beak"),
					Attack("Talon Strike", ItemQuality.Standard, "rtalons", "ltalons")
				],
				combatStrategyKey: "Beast Swooper"
			),
			["Hippocamp"] = BeastRace(
				"Hippocamp",
				"Hippocamp",
				SizeCategory.Large,
				"Horse",
				GreatBeast(),
				"Hippocamps marry an equine forebody to a powerful fish-tail suited to open water.",
				"a hippocamp",
				"This aquatic myth-beast bears a horse-like forebody and forelegs, but from the loins back it flows into a muscular fish-tail built for swift, powerful swimming.",
				[
					Attack("Herbivore Bite", ItemQuality.Standard, "mouth"),
					Attack("Hoof Stomp", ItemQuality.Standard, "rfhoof", "lfhoof"),
					Attack("Tail Slap", ItemQuality.Good, "caudalfin")
				],
				combatStrategyKey: "Beast Behemoth"
			),
			["Selkie"] = HumanoidRace(
				"Selkie",
				"Organic Humanoid",
				SizeCategory.Normal,
				"Human Male",
				"Human Female",
				LongLivedHumanoid(),
				"Selkies are graceful seal-folk who can move comfortably between shore and sea.",
				[
					Attack("Carnivore Bite", ItemQuality.Bad, "mouth"),
					Attack("Bite", ItemQuality.Bad, "mouth")
				],
				["selkie"],
				combatStrategyKey: "Melee (Auto)"
			),
			["Myconid"] = SapientRace(
				"Myconid",
				"Organic Humanoid",
				SizeCategory.Normal,
				"Human Male",
				"Human Female",
				LongLivedHumanoid(),
				"Myconids are humanoid fungal folk with broad caps, soft flesh and an unsettlingly quiet demeanor.",
				"a myconid",
				"This stooped fungus-being has a humanoid shape but a cap-like head and an organic, spongy texture that marks it as something far removed from ordinary flesh.",
				[
					Attack("Jab", ItemQuality.Bad, "rhand", "lhand"),
					Attack("Elbow", ItemQuality.Bad, "relbow", "lelbow")
				],
				additionalCharacteristics:
				[
					Characteristic("Fungus Colour", "white", "brown", "red", "purple")
				],
				combatStrategyKey: "Melee (Auto)"
			),
			["Plantfolk"] = SapientRace(
				"Plantfolk",
				"Organic Humanoid",
				SizeCategory.Normal,
				"Human Male",
				"Human Female",
				LongLivedHumanoid(),
				"Plantfolk are humanoid vegetative beings of bark, fibre and leaf.",
				"a plantfolk",
				"This plant-being stands in a recognisably humanoid form, but bark-like surfaces, fibrous growths and living greenery make every motion seem rooted in the natural world.",
				[
					Attack("Jab", ItemQuality.Bad, "rhand", "lhand"),
					Attack("Elbow", ItemQuality.Bad, "relbow", "lelbow")
				],
				combatStrategyKey: "Melee (Auto)"
			),
			["Owlkin"] = HumanoidRace(
				"Owlkin",
				"Winged Humanoid",
				SizeCategory.Normal,
				"Human Male",
				"Human Female",
				LongLivedHumanoid(),
				"Owlkin are feathered, winged people with a keen gaze and a marked avian cast.",
				[
					Attack("Jab", ItemQuality.Standard, "rhand", "lhand"),
					Attack("Elbow", ItemQuality.Standard, "relbow", "lelbow")
				],
				["owlkin"],
				[
					Usage("rwingbase", "general"),
					Usage("lwingbase", "general"),
					Usage("rwing", "general"),
					Usage("lwing", "general")
				],
				canClimb: true,
				facialHairProfile: "No_Facial_Hair",
				combatStrategyKey: "Melee (Auto)"
			),
			["Avian Person"] = HumanoidRace(
				"Avian Person",
				"Winged Humanoid",
				SizeCategory.Normal,
				"Human Male",
				"Human Female",
				LongLivedHumanoid(),
				"Avian people are broad-winged birdfolk whose forms remain largely humanoid aside from their wings and avian features.",
				[
					Attack("Jab", ItemQuality.Standard, "rhand", "lhand"),
					Attack("Elbow", ItemQuality.Standard, "relbow", "lelbow")
				],
				["birdfolk", "avian"],
				[
					Usage("rwingbase", "general"),
					Usage("lwingbase", "general"),
					Usage("rwing", "general"),
					Usage("lwing", "general")
				],
				canClimb: true,
				facialHairProfile: "No_Facial_Hair",
				combatStrategyKey: "Melee (Auto)"
			),
			["Centaur"] = HumanoidRace(
				"Centaur",
				"Centaur",
				SizeCategory.Large,
				"Horse",
				"Horse",
				LongLivedHumanoid(),
				"Centaurs combine human torsos and arms with a four-legged equine lower body.",
				[
					Attack("Hoof Stomp", ItemQuality.Good, "rfhoof", "lfhoof", "rrhoof", "lrhoof"),
					Attack("Head Ram", ItemQuality.Standard, "head"),
					Attack("Elbow", ItemQuality.Standard, "relbow", "lelbow")
				],
				["centaur"],
				combatStrategyKey: "Melee (Auto)"
			),
			["Pegacorn"] = BeastRace(
				"Pegacorn",
				"Ungulate",
				SizeCategory.Large,
				"Horse",
				GreatBeast(),
				"Pegacorns combine the broad wings of a pegasus with the spiralled horn of a unicorn.",
				"a pegacorn",
				"This mythic equine bears both sweeping feathered wings and a singular horn, giving it the grace of a unicorn and the power of a pegasus.",
				[
					Attack("Bite", ItemQuality.Bad, "mouth"),
					Attack("Hoof Stomp", ItemQuality.Good, "rfhoof", "lfhoof", "rrhoof", "lrhoof"),
					Attack("Horn Gore", ItemQuality.Good, "horn")
				],
				[
					Usage("horn", "general"),
					Usage("rwingbase", "general"),
					Usage("lwingbase", "general"),
					Usage("rwing", "general"),
					Usage("lwing", "general")
				],
				combatStrategyKey: "Beast Swooper"
			)
		};
	}

	internal static IReadOnlyList<string> ValidateTemplateCatalogForTesting()
	{
		var issues = new List<string>();
		var validBodyKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"Toed Quadruped",
			"Griffin",
			"Hippogriff",
			"Ungulate",
			"Horned Humanoid",
			"Eastern Dragon",
			"Naga",
			"Mermaid",
			"Manticore",
			"Wyvern",
			"Avian",
			"Serpentine",
			"Hippocamp",
			"Organic Humanoid",
			"Winged Humanoid",
			"Centaur"
		};
		var validAttackNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"Carnivore Bite",
			"Bite",
			"Claw Swipe",
			"Horn Gore",
			"Tail Slap",
			"Dragonfire Breath",
			"Beak Peck",
			"Beak Bite",
			"Hoof Stomp",
			"Head Ram",
			"Talon Strike",
			"Wing Buffet",
			"Tail Spike",
			"Herbivore Bite",
			"Jab",
			"Elbow"
		};
		var nonClinchAttackNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"Carnivore Bite",
			"Claw Swipe",
			"Horn Gore",
			"Tail Slap",
			"Dragonfire Breath",
			"Beak Peck",
			"Head Ram",
			"Talon Strike",
			"Wing Buffet",
			"Tail Spike",
			"Jab"
		};
		var clinchAttackNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"Bite",
			"Beak Bite",
			"Herbivore Bite",
			"Elbow"
		};

		if (Templates.Count != 22)
		{
			issues.Add($"Expected 22 mythical race templates but found {Templates.Count}.");
		}

		foreach (var (name, template) in Templates)
		{
			if (string.IsNullOrWhiteSpace(template.CombatStrategyKey))
			{
				issues.Add($"Mythical race {name} is missing a combat strategy key.");
			}
			else if (!CombatStrategySeederHelper.IsKnownStrategyName(template.CombatStrategyKey))
			{
				issues.Add($"Mythical race {name} references unknown combat strategy {template.CombatStrategyKey}.");
			}

			if (!validBodyKeys.Contains(template.BodyKey))
			{
				issues.Add($"Race {name} uses unknown body key {template.BodyKey}.");
			}

			if (!template.Attacks.All(x => validAttackNames.Contains(x.AttackName)))
			{
				issues.Add($"Race {name} references an unsupported attack name.");
			}

			if (template.Attacks.Count == 0)
			{
				issues.Add($"Race {name} must expose at least one natural attack.");
			}
			else
			{
				if (!template.Attacks.Any(x => nonClinchAttackNames.Contains(x.AttackName)))
				{
					issues.Add($"Race {name} must expose at least one non-clinch natural attack.");
				}

				if (!template.Attacks.Any(x => clinchAttackNames.Contains(x.AttackName)))
				{
					issues.Add($"Race {name} must expose at least one clinch natural attack.");
				}
			}

			if (template.HumanoidVariety)
			{
				if (template.PersonWords == null || template.PersonWords.Count == 0)
				{
					issues.Add($"Humanoid variety race {name} is missing person words.");
				}

				if (template.CanUseWeapons == false)
				{
					issues.Add($"Humanoid variety race {name} should support weapon use.");
				}
			}
			else
			{
				if (string.IsNullOrWhiteSpace(template.ShortDescriptionPattern) ||
				    string.IsNullOrWhiteSpace(template.FullDescriptionPattern))
				{
					issues.Add($"Bestial race {name} is missing default description patterns.");
				}
			}
		}

		return issues;
	}
}
