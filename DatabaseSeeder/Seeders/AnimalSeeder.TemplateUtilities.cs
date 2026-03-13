#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Body;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class AnimalSeeder
{
	private static Dictionary<string, AnimalRaceTemplate> BuildRaceTemplates()
	{
		return GetMammalRaceTemplates()
			.Concat(GetBirdRaceTemplates())
			.Concat(GetSerpentRaceTemplates())
			.Concat(GetAquaticRaceTemplates())
			.Concat(GetInsectRaceTemplates())
			.Concat(GetArachnidRaceTemplates())
			.Concat(GetReptileAmphibianRaceTemplates())
			.ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
	}

	private static Dictionary<string, AnimalHeightWeightTemplate> BuildHeightWeightTemplates()
	{
		static AnimalHeightWeightTemplate Model(string name, double meanHeight, double stdDevHeight, double meanBmi,
			double stdDevBmi)
			=> new(name, meanHeight, stdDevHeight, meanBmi, stdDevBmi);

		return new Dictionary<string, AnimalHeightWeightTemplate>(StringComparer.OrdinalIgnoreCase)
		{
			["Small Rodent"] = Model("Small Rodent", 12, 0.6, 14, 1.4),
			["Mustelid"] = Model("Mustelid", 35, 1.75, 18, 1.8),
			["Lagomorph"] = Model("Lagomorph", 30, 1.5, 16, 1.6),
			["Domestic Cat"] = Model("Domestic Cat", 35, 1.75, 81, 8.1),
			["Small Canid"] = Model("Small Canid", 55, 2.75, 55, 5.5),
			["Large Canid"] = Model("Large Canid", 80, 4.0, 78, 7.8),
			["Big Felid"] = Model("Big Felid", 105, 5.25, 128, 12.8),
			["Bear"] = Model("Bear", 165, 8.25, 82, 8.2),
			["Small Ungulate"] = Model("Small Ungulate", 70, 3.5, 110, 11),
			["Large Ungulate"] = Model("Large Ungulate", 145, 7.25, 185, 18.5),
			["Horse"] = Model("Horse", 175, 8.75, 165, 16.5),
			["Pachyderm"] = Model("Pachyderm", 220, 11, 410, 41),
			["Songbird"] = Model("Songbird", 12, 0.6, 16, 1.6),
			["Small Bird"] = Model("Small Bird", 25, 1.25, 18, 1.8),
			["Waterfowl"] = Model("Waterfowl", 35, 1.75, 28, 2.8),
			["Wader"] = Model("Wader", 60, 3.0, 15, 1.5),
			["Raptor"] = Model("Raptor", 48, 2.4, 22, 2.2),
			["Flightless Bird"] = Model("Flightless Bird", 115, 5.75, 17, 1.7),
			["Serpent"] = Model("Serpent", 50, 2.5, 20, 2),
			["Small Fish"] = Model("Small Fish", 15, 0.75, 13, 1.3),
			["Medium Fish"] = Model("Medium Fish", 30, 1.5, 8.5, 0.85),
			["Large Fish"] = Model("Large Fish", 100, 5, 3, 0.3),
			["Shark"] = Model("Shark", 450, 22.5, 111, 11.1),
			["Dolphin"] = Model("Dolphin", 250, 12.5, 28, 2.8),
			["Small Whale"] = Model("Small Whale", 1000, 50, 40, 4),
			["Large Whale"] = Model("Large Whale", 3000, 150, 200, 20),
			["Cephalopod"] = Model("Cephalopod", 45, 2.25, 4.5, 0.45),
			["Giant Cephalopod"] = Model("Giant Cephalopod", 300, 15, 14.5, 1.45),
			["Crab"] = Model("Crab", 25, 1.25, 22, 2.2),
			["Large Crab"] = Model("Large Crab", 50, 2.5, 20, 2),
			["Crustacean"] = Model("Crustacean", 30, 1.5, 14, 1.4),
			["Jellyfish"] = Model("Jellyfish", 35, 1.75, 3, 0.3),
			["Pinniped"] = Model("Pinniped", 150, 7.5, 82, 8.2),
			["Walrus"] = Model("Walrus", 250, 12.5, 135, 13.5),
			["Insect"] = Model("Insect", 10, 0.5, 18, 1.8),
			["Winged Insect"] = Model("Winged Insect", 12, 0.6, 16, 1.6),
			["Arachnid"] = Model("Arachnid", 12, 0.6, 12, 1.2),
			["Scorpion"] = Model("Scorpion", 15, 0.75, 14, 1.4),
			["Reptilian"] = Model("Reptilian", 45, 2.25, 24, 2.4),
			["Chelonian"] = Model("Chelonian", 40, 2.0, 45, 4.5),
			["Crocodilian"] = Model("Crocodilian", 160, 8, 130, 13),
			["Anuran"] = Model("Anuran", 18, 0.9, 20, 2)
		};
	}

	private static Dictionary<string, AnimalBodyAuditProfile> BuildBodyAuditProfiles()
	{
		return new Dictionary<string, AnimalBodyAuditProfile>(StringComparer.OrdinalIgnoreCase)
		{
			["toed-quadruped"] = new(
				"toed-quadruped",
				AnimalBoneExpectation.Required,
				["abdomen", "head", "mouth", "rfpaw", "lfpaw", "rrpaw", "lrpaw"],
				["brain", "heart", "liver", "stomach", "sintestines", "lintestines", "rlung", "llung"],
				["fskull", "sternum", "cvertebrae", "dvertebrae", "rhumerus", "lhumerus", "rfemur", "lfemur"],
				["Torso", "Head", "Right Foreleg", "Left Foreleg", "Right Hindleg", "Left Hindleg"]
			),
			["ungulate"] = new(
				"ungulate",
				AnimalBoneExpectation.Required,
				["abdomen", "head", "mouth", "rfhoof", "lfhoof", "rrhoof", "lrhoof"],
				["brain", "heart", "liver", "stomach", "sintestines", "lintestines", "rlung", "llung"],
				["fskull", "sternum", "cvertebrae", "dvertebrae", "rhumerus", "lhumerus", "rfemur", "lfemur"],
				["Torso", "Head", "Right Foreleg", "Left Foreleg", "Right Hindleg", "Left Hindleg"]
			),
			["avian"] = new(
				"avian",
				AnimalBoneExpectation.Required,
				["abdomen", "head", "beak", "rwing", "lwing", "rfoot", "lfoot"],
				["brain", "heart", "liver", "stomach", "sintestines", "lintestines", "rlung", "llung"],
				["fskull", "sternum", "cvertebrae", "dvertebrae", "rhumerus", "lhumerus", "rfemur", "lfemur"],
				["Torso", "Head", "Right Leg", "Left Leg", "Right Wing", "Left Wing"]
			),
			["decapod"] = new(
				"decapod",
				AnimalBoneExpectation.Forbidden,
				["carapace", "mouth", "rclaw", "lclaw", "rleg1", "lleg4"],
				["brain", "heart", "stomach", "gillcluster"],
				[],
				["Torso", "Head", "Right Claw", "Left Claw", "Right Leg 1", "Left Leg 4"]
			),
			["serpent"] = new(
				"serpent",
				AnimalBoneExpectation.Required,
				["head", "mouth", "ubody", "mbody", "lbody", "tail"],
				["brain", "heart", "liver", "stomach", "sintestines", "lintestines", "rlung", "llung"],
				["fskull", "cvertebrae", "dvertebrae", "lvertebrae", "cavertebrae"],
				["Torso", "Head", "Tail"]
			),
			["fish"] = new(
				"fish",
				AnimalBoneExpectation.Required,
				["abdomen", "head", "mouth", "peduncle", "caudalfin", "rgill", "lgill"],
				["brain", "heart", "liver", "stomach", "sintestines", "lintestines"],
				["fskull", "cvertebrae", "dvertebrae", "lvertebrae", "cavertebrae"],
				["Torso", "Head", "Tail"]
			),
			["malacostracan"] = new(
				"malacostracan",
				AnimalBoneExpectation.Forbidden,
				["carapace", "abdomen", "tailfan", "rclaw", "lclaw"],
				["brain", "heart", "stomach", "gillcluster"],
				[],
				["Torso", "Tail", "Right Claw", "Left Claw"]
			),
			["cephalopod"] = new(
				"cephalopod",
				AnimalBoneExpectation.Forbidden,
				["abdomen", "head", "mantle", "mouth", "arm1", "arm8"],
				["brain", "heart", "liver", "stomach", "intestines"],
				[],
				["Torso", "Head", "1st Arm", "8th Arm"]
			),
			["jellyfish"] = new(
				"jellyfish",
				AnimalBoneExpectation.Forbidden,
				["body", "tendril1", "tendril10"],
				[],
				[],
				["Torso", "Tendril1", "Tendril10"]
			),
			["pinniped"] = new(
				"pinniped",
				AnimalBoneExpectation.Required,
				["abdomen", "head", "mouth", "rfrontflipper", "lfrontflipper", "rhindflipper", "lhindflipper"],
				["brain", "heart", "liver", "stomach", "sintestines", "lintestines", "rlung", "llung"],
				["fskull", "sternum", "cvertebrae", "dvertebrae", "rhumerus", "lhumerus", "rfemur", "lfemur"],
				["Torso", "Head", "Right Foreleg", "Left Foreleg", "Right Hindleg", "Left Hindleg"]
			),
			["cetacean"] = new(
				"cetacean",
				AnimalBoneExpectation.Optional,
				["abdomen", "head", "mouth", "stock", "fluke"],
				["brain", "heart", "liver", "stomach", "sintestines", "lintestines"],
				[],
				["Torso", "Head", "Tail"]
			),
			["insectoid"] = new(
				"insectoid",
				AnimalBoneExpectation.Forbidden,
				["thorax", "head", "abdomen", "mandibles", "rleg1", "lleg3"],
				["brain", "heart", "gut", "spiracles"],
				[],
				["Torso", "Head", "Right Leg 1", "Left Leg 3"]
			),
			["winged-insectoid"] = new(
				"winged-insectoid",
				AnimalBoneExpectation.Forbidden,
				["thorax", "head", "abdomen", "mandibles", "rwing", "lwing"],
				["brain", "heart", "gut", "spiracles"],
				[],
				["Torso", "Head", "Right Wing", "Left Wing"]
			),
			["arachnid"] = new(
				"arachnid",
				AnimalBoneExpectation.Forbidden,
				["cephalothorax", "abdomen", "rfang", "lfang", "rleg1", "lleg4"],
				["brain", "heart", "gut", "booklungs"],
				[],
				["Torso", "Head", "Right Leg 1", "Left Leg 4"]
			),
			["scorpion"] = new(
				"scorpion",
				AnimalBoneExpectation.Forbidden,
				["cephalothorax", "abdomen", "rclaw", "lclaw", "stinger"],
				["brain", "heart", "gut", "booklungs"],
				[],
				["Torso", "Tail", "Right Claw", "Left Claw"]
			),
			["reptilian"] = new(
				"reptilian",
				AnimalBoneExpectation.Required,
				["abdomen", "head", "mouth", "rfpaw", "lfpaw", "rrpaw", "lrpaw"],
				["brain", "heart", "liver", "stomach", "sintestines", "lintestines", "rlung", "llung"],
				["fskull", "sternum", "cvertebrae", "dvertebrae", "lvertebrae"],
				["Torso", "Head", "Right Foreleg", "Left Foreleg", "Right Hindleg", "Left Hindleg"]
			),
			["anuran"] = new(
				"anuran",
				AnimalBoneExpectation.Required,
				["abdomen", "head", "mouth", "rfpaw", "lfpaw", "rrpaw", "lrpaw"],
				["brain", "heart", "liver", "stomach", "sintestines", "lintestines", "rlung", "llung"],
				["fskull", "sternum", "cvertebrae", "dvertebrae", "lvertebrae"],
				["Torso", "Head", "Right Foreleg", "Left Foreleg", "Right Hindleg", "Left Hindleg"]
			)
		};
	}

	private static string GetBreathingModeName(AnimalBreathingMode breathingMode)
	{
		return breathingMode switch
		{
			AnimalBreathingMode.Simple => "simple",
			AnimalBreathingMode.Insect => "insect",
			AnimalBreathingMode.Partless => "partless",
			AnimalBreathingMode.Freshwater => "freshwater",
			AnimalBreathingMode.Saltwater => "saltwater",
			AnimalBreathingMode.Blowhole => "blowhole",
			_ => "simple"
		};
	}

	internal static IReadOnlyList<string> ValidateTemplateCatalogForTesting()
	{
		var issues = new List<string>();
		var validBloodProfiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"equine",
			"feline",
			"canine",
			"bovine",
			"ovine"
		};
		var validAttackKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"carnivorebite",
			"carnivoresmashbite",
			"carnivorelowbite",
			"carnivorehighbite",
			"carnivorelowestbite",
			"carnivoreclinchbite",
			"carnivoreclinchhighbite",
			"carnivoreclinchhighestbite",
			"carnivoredownbite",
			"herbivorebite",
			"herbivoresmashbite",
			"smallbite",
			"smallsmashbite",
			"smalllowbite",
			"smalldownedbite",
			"clawswipe",
			"clawsmashswipe",
			"clawlowswipe",
			"clawhighswipe",
			"hoofstomp",
			"hoofstompsmash",
			"barge",
			"bargesmash",
			"clinchbarge",
			"gorehorn",
			"goreantler",
			"goretusk",
			"tusksweep",
			"crabpinch",
			"fishbite",
			"fishquickbite",
			"sharkbite",
			"sharkreelbite",
			"beakpeck",
			"talonstrike",
			"fangbite",
			"mandiblebite",
			"arachnidclaw",
			"headram",
			"tailslap"
		};

		foreach (var (raceName, template) in RaceTemplates)
		{
			if (!AgeProfiles.ContainsKey(template.AgeProfileKey))
			{
				issues.Add($"Race {raceName} uses unknown age profile {template.AgeProfileKey}.");
			}

			if (!HeightWeightTemplates.ContainsKey(template.MaleHeightWeightModel))
			{
				issues.Add($"Race {raceName} uses unknown male height/weight model {template.MaleHeightWeightModel}.");
			}

			if (!HeightWeightTemplates.ContainsKey(template.FemaleHeightWeightModel))
			{
				issues.Add($"Race {raceName} uses unknown female height/weight model {template.FemaleHeightWeightModel}.");
			}

			if (!AttackLoadouts.TryGetValue(template.AttackLoadoutKey, out var loadout))
			{
				issues.Add($"Race {raceName} uses unknown attack loadout {template.AttackLoadoutKey}.");
			}
			else
			{
				var loadoutCount = loadout.ShapeMatchedAttacks.Count +
				                  (loadout.AliasAttacks?.Count ?? 0) +
				                  (loadout.VenomAttacks?.Count ?? 0);
				if (loadoutCount == 0)
				{
					issues.Add($"Race {raceName} has an empty attack loadout {template.AttackLoadoutKey}.");
				}
			}

			if (template.BodyAuditKey is not null && !BodyAuditProfiles.ContainsKey(template.BodyAuditKey))
			{
				issues.Add($"Race {raceName} uses unknown body audit profile {template.BodyAuditKey}.");
			}

			if (template.BloodProfileKey is not null && !validBloodProfiles.Contains(template.BloodProfileKey))
			{
				issues.Add($"Race {raceName} uses unsupported blood profile {template.BloodProfileKey}.");
			}

			ValidateDescriptionVariants(raceName, template.DescriptionPack.BabyMale, 1, issues);
			ValidateDescriptionVariants(raceName, template.DescriptionPack.BabyFemale, 1, issues);
			ValidateDescriptionVariants(raceName, template.DescriptionPack.JuvenileMale, 1, issues);
			ValidateDescriptionVariants(raceName, template.DescriptionPack.JuvenileFemale, 1, issues);
			ValidateDescriptionVariants(raceName, template.DescriptionPack.AdultMale, 2, issues);
			ValidateDescriptionVariants(raceName, template.DescriptionPack.AdultFemale, 2, issues);
		}

		foreach (var (loadoutKey, loadout) in AttackLoadouts)
		{
			foreach (var attack in loadout.ShapeMatchedAttacks)
			{
				if (!validAttackKeys.Contains(attack.AttackKey))
				{
					issues.Add($"Attack loadout {loadoutKey} references unknown attack {attack.AttackKey}.");
				}
			}

			if (loadout.AliasAttacks is not null)
			{
				foreach (var attack in loadout.AliasAttacks)
				{
					if (!validAttackKeys.Contains(attack.AttackKey))
					{
						issues.Add($"Attack loadout {loadoutKey} references unknown alias attack {attack.AttackKey}.");
					}

					if (!attack.BodypartAliases.Any())
					{
						issues.Add($"Attack loadout {loadoutKey} has an alias attack without target aliases.");
					}
					else if (attack.BodypartAliases.Any(string.IsNullOrWhiteSpace))
					{
						issues.Add($"Attack loadout {loadoutKey} has a blank alias bodypart target.");
					}
				}
			}

			if (loadout.VenomAttacks is not null)
			{
				foreach (var attack in loadout.VenomAttacks)
				{
					if (!VenomProfiles.ContainsKey(attack.VenomProfileKey))
					{
						issues.Add($"Attack loadout {loadoutKey} references unknown venom profile {attack.VenomProfileKey}.");
					}

					if (string.IsNullOrWhiteSpace(attack.AttackShapeName))
					{
						issues.Add($"Attack loadout {loadoutKey} has a venom attack without an attack shape.");
					}

					if (string.IsNullOrWhiteSpace(attack.CombatMessage))
					{
						issues.Add($"Attack loadout {loadoutKey} has a venom attack without a combat message.");
					}

					if (attack.MaximumQuantity <= 0)
					{
						issues.Add($"Attack loadout {loadoutKey} has a venom attack with a non-positive quantity.");
					}

					if (attack.MinimumWoundSeverity < 0)
					{
						issues.Add($"Attack loadout {loadoutKey} has a venom attack with a negative minimum wound severity.");
					}

					if (attack.TargetBodypartAliases.Any(string.IsNullOrWhiteSpace))
					{
						issues.Add($"Attack loadout {loadoutKey} has a venom attack with a blank target alias.");
					}
				}
			}
		}

		foreach (var (profileKey, profile) in VenomProfiles)
		{
			if (!profile.Effects.Any())
			{
				issues.Add($"Venom profile {profileKey} has no effects.");
			}

			if (profile.IntensityPerGram <= 0)
			{
				issues.Add($"Venom profile {profileKey} must have a positive intensity per gram.");
			}

			if (profile.RelativeMetabolisationRate <= 0)
			{
				issues.Add($"Venom profile {profileKey} must have a positive metabolisation rate.");
			}
		}

		return issues;

		static void ValidateDescriptionVariants(string raceName, IReadOnlyList<AnimalDescriptionVariant> variants, int minimum,
			List<string> issues)
		{
			if (variants.Count < minimum)
			{
				issues.Add($"Race {raceName} has fewer than {minimum} description variants in one age bucket.");
				return;
			}

			if (variants.Any(x => string.IsNullOrWhiteSpace(x.ShortDescription) || string.IsNullOrWhiteSpace(x.FullDescription)))
			{
				issues.Add($"Race {raceName} has a blank short or full description variant.");
			}
		}
	}

	private void SeedAnimalRaces(IEnumerable<AnimalRaceTemplate> templates, params (string Key, BodyProto Body)[] bodies)
	{
		var bodyLookup = bodies.ToDictionary(x => x.Key, x => x.Body, StringComparer.OrdinalIgnoreCase);
		var templateList = templates.ToList();
		foreach (var template in templateList)
		{
			AddRace(
				template.Name,
				template.Adjective,
				template.Description,
				bodyLookup[template.BodyKey],
				template.Size,
				template.CanClimb,
				template.BodypartHealthMultiplier,
				template.MaleHeightWeightModel,
				template.FemaleHeightWeightModel,
				template.Sweats,
				GetBreathingModeName(template.BreathingMode)
			);
		}

		foreach (var template in templateList
			         .Where(x => x.BodyAuditKey is not null)
			         .GroupBy(x => $"{x.BodyKey}|{x.BodyAuditKey}", StringComparer.OrdinalIgnoreCase)
			         .Select(x => x.First()))
		{
			AuditBody(bodyLookup[template.BodyKey], template.BodyAuditKey!);
		}
	}

	private void AuditBody(BodyProto body, string auditKey)
	{
		if (!BodyAuditProfiles.TryGetValue(auditKey, out var profile))
		{
			return;
		}

		var bodies = new List<long> { body.Id };
		var countedBody = body.CountsAs;
		while (countedBody is not null)
		{
			bodies.Add(countedBody.Id);
			countedBody = countedBody.CountsAs;
		}

		var parts = _context.BodypartProtos.Where(x => bodies.Contains(x.BodyId)).ToList();
		var partLookup = BuildAuditPartLookup(bodies, parts);
		var limbNames = _context.Limbs.Where(x => bodies.Contains(x.RootBodyId)).Select(x => x.Name)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);

		static bool IsBoneType(BodypartProto part)
		{
			return ((BodypartTypeEnum)part.BodypartType) switch
			{
				BodypartTypeEnum.Bone => true,
				BodypartTypeEnum.NonImmobilisingBone => true,
				BodypartTypeEnum.MinorBone => true,
				BodypartTypeEnum.MinorNonImobilisingBone => true,
				_ => false
			};
		}

		var missing = new List<string>();
		missing.AddRange(profile.RequiredBodyparts.Where(x => !partLookup.ContainsKey(x)));
		missing.AddRange(profile.RequiredOrgans.Where(x => !partLookup.TryGetValue(x, out var part) || part.IsOrgan == 0)
			.Select(x => $"organ:{x}"));
		missing.AddRange(profile.RequiredBones.Where(x => !partLookup.TryGetValue(x, out var part) || !IsBoneType(part))
			.Select(x => $"bone:{x}"));
		missing.AddRange(profile.RequiredLimbs.Where(x => !limbNames.Contains(x)).Select(x => $"limb:{x}"));

		if (missing.Any())
		{
			throw new InvalidOperationException(
				$"Body {body.Name} failed anatomy audit {auditKey}. Missing: {string.Join(", ", missing)}");
		}
	}

	internal static IReadOnlyDictionary<string, BodypartProto> BuildAuditPartLookup(
		IReadOnlyList<long> bodies,
		IEnumerable<BodypartProto> parts)
	{
		var bodyOrder = bodies
			.Select((id, index) => (id, index))
			.ToDictionary(x => x.id, x => x.index);
		return parts
			.OrderBy(x => bodyOrder[x.BodyId])
			.GroupBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
			.ToDictionary(x => x.Key, x => x.First(), StringComparer.OrdinalIgnoreCase);
	}
}
