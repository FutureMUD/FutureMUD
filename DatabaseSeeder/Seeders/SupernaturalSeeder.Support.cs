#nullable enable

using MudSharp.Combat;
using MudSharp.Character.Name;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.FutureProg;
using MudSharp.Framework;
using MudSharp.Models;
using MudSharp.Planes;
using MudSharp.RPG.Merits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DatabaseSeeder.Seeders;

public partial class SupernaturalSeeder
{
	private const string SupernaturalUndeadCorpseModelName = "Supernatural Nondecaying Undead Remains";
	private const string SupernaturalSpiritCorpseModelName = "Supernatural Dissipating Spirit Remains";

	internal sealed record SupernaturalAttackDefinition(
		string DonorName,
		string Message,
		IReadOnlyList<string> Categories,
		BuiltInCombatMoveType? MoveTypeOverride = null,
		string? FixedTargetBodypartShape = null,
		bool SharedStockAttack = false);

	private static readonly IReadOnlyDictionary<string, (string SourceBody, SupernaturalPlanarProfile PlanarProfile)> CustomBodyProfiles =
		new Dictionary<string, (string SourceBody, SupernaturalPlanarProfile PlanarProfile)>(StringComparer.OrdinalIgnoreCase)
		{
			["Supernatural Ophanic Wheel"] = ("Organic Humanoid", SupernaturalPlanarProfile.DualNatured),
			["Supernatural Many-Winged Angel"] = ("Winged Humanoid", SupernaturalPlanarProfile.DualNatured),
			["Supernatural Winged Angel"] = ("Winged Humanoid", SupernaturalPlanarProfile.DualNatured),
			["Supernatural Angelic Humanoid"] = ("Organic Humanoid", SupernaturalPlanarProfile.DualNatured),
			["Supernatural Divine Humanoid"] = ("Organic Humanoid", SupernaturalPlanarProfile.DualNatured),
			["Supernatural Horned Fiend"] = ("Horned Humanoid", SupernaturalPlanarProfile.AstralNative),
			["Supernatural Familiar"] = ("Horned Humanoid", SupernaturalPlanarProfile.AstralNative),
			["Supernatural Hellhound"] = ("Toed Quadruped", SupernaturalPlanarProfile.Material),
			["Supernatural Spirit Form"] = ("Organic Humanoid", SupernaturalPlanarProfile.Incorporeal),
			["Supernatural Werewolf Hybrid"] = ("Organic Humanoid", SupernaturalPlanarProfile.Material),
			["Supernatural Skeleton"] = ("Organic Humanoid", SupernaturalPlanarProfile.Material),
			["Supernatural Lich Skeleton"] = ("Organic Humanoid", SupernaturalPlanarProfile.DualNatured),
			["Supernatural Decayed Undead"] = ("Organic Humanoid", SupernaturalPlanarProfile.Material)
		};

	private static readonly IReadOnlyDictionary<string, string[]> CustomBodyAdditionalAliases =
		new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
		{
			["Supernatural Horned Fiend"] = ["utail", "mtail", "ltail"],
			["Supernatural Familiar"] = ["utail", "mtail", "ltail"]
		};

	internal static IReadOnlyDictionary<string, string[]> SupernaturalBodyAdditionalAliasesForTesting =>
		CustomBodyAdditionalAliases;

	private static SupernaturalAttackDefinition AttackDefinition(string donorName, string message,
		params string[] categories)
	{
		return new SupernaturalAttackDefinition(donorName, message, categories);
	}

	private static SupernaturalAttackDefinition SonicAttackDefinition(string donorName, string message,
		params string[] categories)
	{
		return new SupernaturalAttackDefinition(donorName, message, categories,
			BuiltInCombatMoveType.ScreechAttack, "Ear");
	}

	private static SupernaturalAttackDefinition SharedAttackDefinition(string donorName, string message,
		params string[] categories)
	{
		return new SupernaturalAttackDefinition(donorName, message, categories, SharedStockAttack: true);
	}

	private static readonly IReadOnlyDictionary<string, SupernaturalAttackDefinition> SupernaturalAttackCloneDefinitions =
		new Dictionary<string, SupernaturalAttackDefinition>(StringComparer.OrdinalIgnoreCase)
		{
			["Radiant Touch"] = AttackDefinition("Bite",
				"@ sear|sears $1 with a touch of radiant power.", "radiant", "clinch"),
			["Radiant Gaze"] = AttackDefinition("Bite",
				"@ strike|strikes $1 with a radiant gaze.", "radiant"),
			["Wing Buffet"] = SharedAttackDefinition("Wing Buffet",
				"@ beat|beats &0's {0} and buffet|buffets $1 with a crashing gust.", "buffeting"),
			["Infernal Claw"] = AttackDefinition("Claw High Swipe",
				"@ rake|rakes $1 with infernal claws.", "infernal", "claw"),
			["Horn Gore"] = SharedAttackDefinition("Horn Gore",
				"@ gore|gores $1 with supernatural horns.", "horn"),
			["Fanged Bite"] = AttackDefinition("Carnivore Bite",
				"@ bite|bites $1 with supernatural fangs.", "bite"),
			["Soul Chill"] = AttackDefinition("Bite",
				"@ chill|chills $1 with deathly spiritual force.", "spirit", "clinch"),
			["Grave Claw"] = AttackDefinition("Claw Low Swipe",
				"@ claw|claws $1 with grave-cold hands.", "undead", "claw"),
			["Wheel Crush"] = AttackDefinition("Animal Barge",
				"@ crush|crushes into $1 with a living wheel of eyes and flame.", "stagger", "wheel"),
			["Spectral Touch"] = AttackDefinition("Bite",
				"@ pass|passes a spectral touch through $1.", "spirit", "clinch"),

			["Heavenly Choir"] = SonicAttackDefinition("Bite",
				"@ open|opens &0's {0} and a choir of heavenly voices pour|pours through the air.",
				"sonic", "radiant", "angelic"),
			["Canticle of Awe"] = SonicAttackDefinition("Bite",
				"@ sing|sings through &0's {0}, filling the air with a many-voiced canticle of awe.",
				"sonic", "radiant", "angelic"),
			["Trumpet Peal"] = SonicAttackDefinition("Bite",
				"@ sound|sounds a trumpet-bright note from &0's {0}, bright as a choir at dawn.",
				"sonic", "radiant", "angelic"),
			["Word of Command"] = SonicAttackDefinition("Bite",
				"@ speak|speaks a word of command from &0's {0}, layered with impossible voices.",
				"sonic", "radiant", "angelic"),
			["Crown of Stars"] = AttackDefinition("Tail Spike",
				"@ loose|looses a star-bright ray from &0's {0} toward $1.", "ranged", "radiant"),
			["Starfire Breath"] = AttackDefinition("Dragonfire Breath",
				"@ breathe|breathes a cone of white starfire from &0's {0} toward $1.", "breath", "radiant"),
			["Seraphic Wingstorm"] = AttackDefinition("Wing Buffet",
				"@ beat|beats &0's {0}, driving a storm of radiant wind into $1.", "buffeting", "radiant"),
			["Mercy-Searing Grasp"] = AttackDefinition("Tree Haul",
				"@ seize|seizes $1 with &0's {0} and haul|hauls &1 through searing mercy.",
				"forced movement", "radiant"),
			["Wheel of Judgment"] = AttackDefinition("Animal Barge Pushback",
				"@ thunder|thunders into $1 as a wheel of judgment and drive|drives &1 back.",
				"pushback", "radiant", "wheel"),
			["Many-Eyed Ray"] = AttackDefinition("Tail Spike",
				"@ focus|focuses many watchful eyes and lance|lances $1 with a radiant ray.", "ranged", "radiant"),

			["Hellfire Breath"] = AttackDefinition("Dragonfire Breath",
				"@ breathe|breathes a cone of hellfire from &0's {0} toward $1.", "breath", "infernal"),
			["Brimstone Spit"] = AttackDefinition("Acid Spit",
				"@ spit|spits a burning gobbet of brimstone from &0's {0} at $1.", "spit", "infernal"),
			["Infernal Trip"] = AttackDefinition("Tusk Sweep",
				"@ sweep|sweeps &0's {0} low, trying to trip $1 with infernal force.",
				"trip", "unbalance", "infernal"),
			["Damnation Barge"] = AttackDefinition("Animal Barge Pushback",
				"@ drive|drives into $1 with damnation's weight and force|forces &1 back.",
				"pushback", "infernal"),
			["Hellish Headbutt"] = AttackDefinition("Headbutt",
				"@ crack|cracks &0's {0} into $1 with hellish force.", "stagger", "infernal"),
			["Soul Hook"] = AttackDefinition("Tree Haul",
				"@ hook|hooks $1 with &0's {0} and drag|drags &1 with soul-deep force.",
				"forced movement", "infernal"),
			["Abyssal Chain Lash"] = AttackDefinition("Water Drag",
				"@ lash|lashes out from &0's {0} with abyssal force and drag|drags at $1.",
				"forced movement", "infernal"),
			["Sinner's Clinch"] = AttackDefinition("Claw Clamp",
				"@ clamp|clamps &0's {0} onto $1 with pitiless strength.", "clinch", "infernal"),
			["Barbed Tail Slap"] = AttackDefinition("Tail Slap",
				"@ whip|whips &0's {0} around in a barbed slap at $1.", "stagger", "infernal"),
			["Fallen Choir"] = SonicAttackDefinition("Bite",
				"@ open|opens &0's {0} and a ruined choir of fallen voices tear|tears through the air.",
				"sonic", "infernal"),

			["Wailing Dirge"] = SonicAttackDefinition("Bite",
				"@ wail|wails through &0's {0}, a mourning dirge that scrapes at every ear.",
				"sonic", "spirit"),
			["Grave Drag"] = AttackDefinition("Water Drag",
				"@ drag|drags at $1 with grave-cold force from &0's {0}.", "forced movement", "undead"),
			["Grasp of the Dead"] = AttackDefinition("Claw Clamp",
				"@ clamp|clamps &0's {0} onto $1 with the grasp of the dead.", "clinch", "undead"),
			["Bone Rattle"] = AttackDefinition("Head Ram",
				"@ rattle|rattles forward and slam|slams &0's {0} into $1.", "stagger", "undead"),
			["Crypt Dust Breath"] = AttackDefinition("Dragonfire Breath",
				"@ breathe|breathes a choking cloud of crypt dust from &0's {0} toward $1.",
				"breath", "undead"),
			["Deathly Pall"] = AttackDefinition("Llama Spit",
				"@ exhale|exhales a deathly pall from &0's {0} toward $1.", "spit", "spirit", "undead"),

			["Raking Maul"] = AttackDefinition("Claw High Swipe",
				"@ maul|mauls $1 with a raking swipe of &0's {0}.", "claw", "therianthrope"),
			["Hamstring Snap"] = AttackDefinition("Carnivore Low Bite",
				"@ snap|snaps low at $1 with a hamstringing bite from &0's {0}.", "bite", "therianthrope"),
			["Crushing Pounce"] = AttackDefinition("Animal Barge",
				"@ pounce|pounces into $1 with crushing bestial force.", "stagger", "therianthrope"),
			["Wolf Trip"] = AttackDefinition("Tusk Sweep",
				"@ sweep|sweeps &0's {0} low, trying to send $1 sprawling.", "trip", "unbalance", "therianthrope")
		};

	internal static IReadOnlyDictionary<string, SupernaturalAttackDefinition> SupernaturalAttackDefinitionsForTesting =>
		SupernaturalAttackCloneDefinitions;

	private static IReadOnlyCollection<string> SupernaturalAttackNames =>
		Templates.Values
			.SelectMany(x => x.Attacks)
			.Select(x => x.AttackName)
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToArray();

	internal static IReadOnlyCollection<string> SupernaturalAttackNamesForTesting => SupernaturalAttackNames;

	private static readonly IReadOnlyDictionary<string, (string PersonWord, string Description, string NameCulture)> SupernaturalCultureDefinitions =
		new Dictionary<string, (string PersonWord, string Description, string NameCulture)>(StringComparer.OrdinalIgnoreCase)
		{
			["Celestial Host"] = ("celestial",
				SeederDescriptionHelpers.JoinParagraphs(
					"The Celestial Host culture is a builder-facing stock culture for angels, divine messengers and godlike beings.",
					"It assumes identities formed around rank, charge, office, oath and manifestation rather than ordinary mortal household structures.",
					"The culture is intentionally broad so a game can turn it into a holy choir, divine bureaucracy, cosmic court or local pantheon."),
				"Angelic Name"),
			["Fallen Host"] = ("fallen one",
				SeederDescriptionHelpers.JoinParagraphs(
					"The Fallen Host culture is a stock supernatural culture for demons, fallen angels and infernal courts.",
					"It frames names and social identity around titles, bindings, legions, temptations, bargains and remembered celestial rank.",
					"Builders can make it theological, folkloric, occult or entirely setting-specific without needing to rebuild the race support data."),
				"Demonic Name"),
			["Spirit Court"] = ("spirit",
				SeederDescriptionHelpers.JoinParagraphs(
					"The Spirit Court culture supports ghosts, ancestral spirits, nature spirits and other incorporeal supernatural beings.",
					"It treats identity as a blend of memory, domain, place, death, obligation and manifestation rather than normal mortal descent.",
					"The stock setup is useful for hauntings, guardian spirits, animist courts, summoned beings and other metaphysical NPCs."),
				"Spirit Name"),
			["Undead Remnant"] = ("undead",
				SeederDescriptionHelpers.JoinParagraphs(
					"The Undead Remnant culture represents deathless beings that still retain enough identity to operate as characters.",
					"It avoids hard-coded feeding, resurrection or corpse-vessel mechanics and instead gives builders a safe social wrapper for playable or NPC undead.",
					"Games can split this into vampire courts, necromancer legions, lich lineages, grave cults or mummy dynasties as needed."),
				"Undead Name"),
			["Therianthrope"] = ("shifter",
				SeederDescriptionHelpers.JoinParagraphs(
					"The Therianthrope culture covers hereditary and cursed shapeshifters whose social identity sits between ordinary mortal life and bestial transformation.",
					"It is deliberately mundane enough to support secret werewolf bloodlines, open shapeshifter clans or isolated curse victims.",
					"The seeded alternate-form merits give builders practical examples for transformation without forcing one moon-cycle rule into every world."),
				"Mortal Name")
		};

	private static readonly IReadOnlyDictionary<string, string[]> SupernaturalNameDefinitions =
		new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
		{
			["Angelic Name"] =
			[
				"Ariel", "Cassiel", "Eremiel", "Ithuriel", "Lailiel", "Mahanael", "Orifiel", "Sandaliel",
				"Seraphel", "Tzuriel", "Uriel", "Zahariel"
			],
			["Demonic Name"] =
			[
				"Ashkavar", "Belthra", "Drazhul", "Ezrakk", "Harketh", "Kazmiel", "Malthera", "Nerezza",
				"Rhaziel", "Sathra", "Vorkath", "Zerukai"
			],
			["Spirit Name"] =
			[
				"Ash-Beneath-Reed", "Bright Over Stone", "Cold Lantern", "Dawn in Rain", "Echo of Bells",
				"Last Ember", "Mist at the Ford", "Old Root", "River Memory", "Salt-Wind", "Seven Leaves", "Winter Glass"
			],
			["Undead Name"] =
			[
				"Ankharet", "Barrow-King", "Cairn Voss", "Dutiful Remnant", "Grey Haman", "Ioseph the Pale",
				"Khemra", "Marrow Saint", "Neseret", "Old Silas", "Sepulchre", "Vigil of Dust"
			],
			["Mortal Name"] =
			[
				"Alex", "Sam", "Morgan", "Rowan", "Casey", "Jordan", "Riley", "Taylor",
				"Blake", "Devon", "Jamie", "Quinn"
			]
		};

	internal static IReadOnlyList<string> ValidateTemplateCatalogForTesting()
	{
		List<string> issues = new();
		HashSet<string> knownBodies = new(StringComparer.OrdinalIgnoreCase)
		{
			"Organic Humanoid",
			"Winged Humanoid",
			"Horned Humanoid",
			"Toed Quadruped"
		};
		knownBodies.UnionWith(CustomBodyProfiles.Keys);

		foreach ((string name, SupernaturalRaceTemplate template) in Templates)
		{
			if (!knownBodies.Contains(template.BodyKey))
			{
				issues.Add($"{name} references unknown body key {template.BodyKey}.");
			}

			if (!SupernaturalCultureDefinitions.ContainsKey(template.CultureKey))
			{
				issues.Add($"{name} references unknown culture key {template.CultureKey}.");
			}

			if (!SupernaturalNameDefinitions.ContainsKey(template.NameCultureKey))
			{
				issues.Add($"{name} references unknown name culture key {template.NameCultureKey}.");
			}

			if (template.Attacks.Count == 0)
			{
				issues.Add($"{name} does not define any natural attacks.");
			}

			foreach (SupernaturalAttackTemplate attack in template.Attacks)
			{
				if (!SupernaturalAttackCloneDefinitions.ContainsKey(attack.AttackName))
				{
					issues.Add($"{name} references unknown supernatural attack {attack.AttackName}.");
				}
			}

			if (template.Characteristics.Count == 0)
			{
				issues.Add($"{name} does not define supernatural characteristics.");
			}

			if (!SeederDescriptionHelpers.HasMinimumParagraphs(BuildRaceDescriptionForTesting(template)))
			{
				issues.Add($"{name} race description should have at least three paragraphs.");
			}

			if (!SeederDescriptionHelpers.HasMinimumParagraphs(BuildEthnicityDescriptionForTesting(template)))
			{
				issues.Add($"{name} ethnicity description should have at least three paragraphs.");
			}

			if (template.DescriptionVariants.Count < 2)
			{
				issues.Add($"{name} should have at least two description variants.");
			}
		}

		foreach (SupernaturalFormTemplate form in FormTemplates)
		{
			if (!Templates.ContainsKey(form.TargetRaceName) && !form.TargetRaceName.Equals("Wolf", StringComparison.OrdinalIgnoreCase))
			{
				issues.Add($"{form.MeritName} targets unknown race {form.TargetRaceName}.");
			}
		}

		return issues;
	}

	private void EnsureSupernaturalMaterials()
	{
		if (_context.Materials.Any(x => x.Name == "spirit energy"))
		{
			return;
		}

		_context.Materials.Add(new Material
		{
			Name = "spirit energy",
			MaterialDescription = "spirit energy",
			BehaviourType = (int)MaterialBehaviourType.Spirit,
			Type = (int)MaterialType.Solid,
			Density = 1.0,
			Organic = false,
			Absorbency = 0.0,
			ShearYield = 1000,
			ImpactYield = 1000,
			ElectricalConductivity = 0.0001,
			ThermalConductivity = 0.01,
			SpecificHeatCapacity = 1000,
			ResidueColour = "pale",
			ResidueSdesc = "a trace of spirit energy",
			ResidueDesc = "It is marked by {0}faint spirit energy",
			SolventVolumeRatio = 1.0
		});
		_context.SaveChanges();
	}

	private CorpseModel EnsureNonDecayingCorpseModel(string name, string description, string sdesc, string fdesc,
		string partDesc, string materialName)
	{
		Material material = _context.Materials.FirstOrDefault(x => x.Name == materialName) ??
		                    _context.Materials.First(x => x.Name == "bone");
		CorpseModel model = SeederRepeatabilityHelper.EnsureNamedEntity(
			_context.CorpseModels,
			name,
			x => x.Name,
			() =>
			{
				CorpseModel created = new();
				_context.CorpseModels.Add(created);
				return created;
			});

		model.Name = name;
		model.Description = description;
		model.Type = "NonDecaying";
		model.Definition = new XElement("CorpseModel",
			new XElement("SDesc", sdesc),
			new XElement("FDesc", fdesc),
			new XElement("PartDesc", partDesc),
			new XElement("CorpseMaterial", material.Id),
			new XElement("EdiblePercentage", 0.0)).ToString();
		_context.SaveChanges();
		return model;
	}

	private void EnsureSupernaturalAttacks(SupernaturalSeedSummary summary)
	{
		foreach ((string name, SupernaturalAttackDefinition definition) in SupernaturalAttackCloneDefinitions)
		{
			if (_context.WeaponAttacks.Any(x => x.Name == name))
			{
				continue;
			}

			WeaponAttack donor = _context.WeaponAttacks.First(x => x.Name == definition.DonorName);
			int moveType = (int)(definition.MoveTypeOverride ?? (BuiltInCombatMoveType)donor.MoveType);
			string additionalInfo = definition.FixedTargetBodypartShape is null
				? donor.AdditionalInfo
				: _context.BodypartShapes.First(x => x.Name == definition.FixedTargetBodypartShape).Id.ToString();
			WeaponAttack attack = new()
			{
				Name = name,
				WeaponTypeId = donor.WeaponTypeId,
				Verb = donor.Verb,
				FutureProgId = donor.FutureProgId,
				BaseAttackerDifficulty = donor.BaseAttackerDifficulty,
				BaseBlockDifficulty = donor.BaseBlockDifficulty,
				BaseDodgeDifficulty = donor.BaseDodgeDifficulty,
				BaseParryDifficulty = donor.BaseParryDifficulty,
				BaseAngleOfIncidence = donor.BaseAngleOfIncidence,
				RecoveryDifficultySuccess = donor.RecoveryDifficultySuccess,
				RecoveryDifficultyFailure = donor.RecoveryDifficultyFailure,
				MoveType = moveType,
				Intentions = donor.Intentions,
				ExertionLevel = donor.ExertionLevel,
				DamageType = donor.DamageType,
				DamageExpressionId = donor.DamageExpressionId,
				StunExpressionId = donor.StunExpressionId,
				PainExpressionId = donor.PainExpressionId,
				Weighting = donor.Weighting,
				BodypartShapeId = donor.BodypartShapeId,
				StaminaCost = donor.StaminaCost,
				BaseDelay = donor.BaseDelay,
				Orientation = donor.Orientation,
				Alignment = donor.Alignment,
				AdditionalInfo = additionalInfo,
				HandednessOptions = donor.HandednessOptions,
				RequiredPositionStateIds = donor.RequiredPositionStateIds,
				OnUseProgId = donor.OnUseProgId
			};
			_context.WeaponAttacks.Add(attack);
			_context.SaveChanges();

			CombatMessage combatMessage = new()
			{
				Type = moveType,
				Message = definition.Message,
				Priority = 50,
				Verb = donor.Verb,
				Chance = 1.0,
				FailureMessage = definition.Message
			};
			combatMessage.CombatMessagesWeaponAttacks.Add(new CombatMessagesWeaponAttacks
			{
				CombatMessage = combatMessage,
				WeaponAttack = attack
			});
			_context.CombatMessages.Add(combatMessage);
			summary.AttacksAdded++;
		}

		_context.SaveChanges();
	}

	private void EnsureSupernaturalCulturesAndNames(SupernaturalSeedSummary summary)
	{
		foreach ((string name, string[] names) in SupernaturalNameDefinitions)
		{
			NameCulture culture = EnsureNameCulture(name);
			_nameCultures[name] = culture;
			EnsureRandomNameProfile(culture, names);
			summary.NameCulturesRefreshed++;
		}

		foreach ((string name, (string personWord, string description, string nameCulture)) in SupernaturalCultureDefinitions)
		{
			Culture? existing = _context.Cultures.FirstOrDefault(x => x.Name == name);
			bool created = existing is null;
			Culture culture = existing ?? new Culture();
			culture.Name = name;
			culture.Description = description;
			culture.PersonWordMale = personWord;
			culture.PersonWordFemale = personWord;
			culture.PersonWordNeuter = personWord;
			culture.PersonWordIndeterminate = personWord;
			culture.SkillStartingValueProg = _alwaysZero;
			culture.AvailabilityProg = _alwaysFalse;
			culture.TolerableTemperatureCeilingEffect = 0;
			culture.TolerableTemperatureFloorEffect = 0;
			culture.PrimaryCalendarId = _context.Calendars.First().Id;
			if (created)
			{
				_context.Cultures.Add(culture);
				summary.CulturesAdded++;
			}

			_context.SaveChanges();
			foreach (Gender gender in Enum.GetValues<Gender>())
			{
				if (_context.CulturesNameCultures.Any(x =>
					    x.CultureId == culture.Id &&
					    x.NameCultureId == _nameCultures[nameCulture].Id &&
					    x.Gender == (short)gender))
				{
					continue;
				}

				_context.CulturesNameCultures.Add(new CulturesNameCultures
				{
					Culture = culture,
					NameCulture = _nameCultures[nameCulture],
					Gender = (short)gender
				});
			}
		}

		_context.SaveChanges();
	}

	private NameCulture EnsureNameCulture(string name)
	{
		NameCulture culture = SeederRepeatabilityHelper.EnsureNamedEntity(
			_context.NameCultures,
			name,
			x => x.Name,
			() =>
			{
				NameCulture created = new();
				_context.NameCultures.Add(created);
				return created;
			});

		culture.Name = name;
		culture.Definition = BuildSimpleNameCultureDefinition(name);
		_context.SaveChanges();
		return culture;
	}

	private static string BuildSimpleNameCultureDefinition(string name)
	{
		return new XElement("NameCulture",
			new XElement("Counts",
				new XElement("Count",
					new XAttribute("Usage", (int)NameUsage.BirthName),
					new XAttribute("Min", 1),
					new XAttribute("Max", 1))),
			new XElement("Patterns",
				Enum.GetValues<NameStyle>()
					.Select(style => new XElement("Pattern",
						new XAttribute("Style", (int)style),
						new XAttribute("Text", "{0}"),
						new XAttribute("Params", "0")))),
			new XElement("Elements",
				new XElement("Element",
					new XAttribute("Usage", (int)NameUsage.BirthName),
					new XAttribute("MinimumCount", 1),
					new XAttribute("MaximumCount", 1),
					new XAttribute("Name", $"{name} Element"),
					new XCData($"A stock {name.ToLowerInvariant()} used by the Supernatural Seeder."))),
			new XElement("NameEntryRegex", new XCData(@"^(?<birthname>[\w '\-]+)$"))).ToString();
	}

	private void EnsureRandomNameProfile(NameCulture nameCulture, IEnumerable<string> names)
	{
		foreach (Gender gender in Enum.GetValues<Gender>())
		{
			string profileName = $"{nameCulture.Name} {gender.DescribeEnum()}";
			RandomNameProfile profile = SeederRepeatabilityHelper.EnsureEntity(
				_context.RandomNameProfiles,
				x => x.Name.Equals(profileName, StringComparison.OrdinalIgnoreCase) && x.Gender == (int)gender,
				x => x.Gender == (int)gender,
				() =>
				{
					RandomNameProfile created = new();
					_context.RandomNameProfiles.Add(created);
					return created;
				});

			profile.Name = profileName;
			profile.Gender = (int)gender;
			profile.NameCulture = nameCulture;
			profile.UseForChargenSuggestionsProg = _alwaysTrue;
			_context.SaveChanges();

			if (!_context.RandomNameProfilesDiceExpressions.Any(x =>
				    x.RandomNameProfileId == profile.Id &&
				    x.NameUsage == (int)NameUsage.BirthName))
			{
				_context.RandomNameProfilesDiceExpressions.Add(new RandomNameProfilesDiceExpressions
				{
					RandomNameProfile = profile,
					NameUsage = (int)NameUsage.BirthName,
					DiceExpression = "1d1"
				});
			}

			foreach (string item in names)
			{
				if (_context.RandomNameProfilesElements.Any(x =>
					    x.RandomNameProfileId == profile.Id &&
					    x.NameUsage == (int)NameUsage.BirthName &&
					    x.Name == item))
				{
					continue;
				}

				_context.RandomNameProfilesElements.Add(new RandomNameProfilesElements
				{
					RandomNameProfile = profile,
					NameUsage = (int)NameUsage.BirthName,
					Name = item,
					Weighting = 1
				});
			}
		}

		_context.SaveChanges();
	}

	private void ApplyEthnicityNameCulture(Ethnicity ethnicity, SupernaturalRaceTemplate template)
	{
		NameCulture culture = _nameCultures[template.NameCultureKey];
		foreach (Gender gender in Enum.GetValues<Gender>())
		{
			if (_context.EthnicitiesNameCultures.Any(x =>
				    x.EthnicityId == ethnicity.Id &&
				    x.NameCultureId == culture.Id &&
				    x.Gender == (short)gender))
			{
				continue;
			}

			_context.EthnicitiesNameCultures.Add(new EthnicitiesNameCultures
			{
				Ethnicity = ethnicity,
				NameCulture = culture,
				Gender = (short)gender
			});
		}
	}

	private void SeedFormMerits(SupernaturalSeedSummary summary)
	{
		foreach (SupernaturalFormTemplate template in FormTemplates)
		{
			Race? targetRace = _context.Races.FirstOrDefault(x => x.Name == template.TargetRaceName);
			if (targetRace is null)
			{
				continue;
			}

			Merit? existing = _context.Merits.FirstOrDefault(x => x.Name == template.MeritName);
			bool created = existing is null;
			Merit merit = existing ?? new Merit();
			merit.Name = template.MeritName;
			merit.Type = "Additional Body Form";
			merit.MeritType = (int)MeritType.Merit;
			merit.MeritScope = (int)MeritScope.Character;
			merit.ParentId = null;
			merit.Definition = BuildAdditionalBodyFormMeritDefinition(
				template,
				targetRace.Id,
				_alwaysTrue.Id,
				_alwaysFalse.Id).ToString();
			if (created)
			{
				_context.Merits.Add(merit);
				summary.FormMeritsAdded++;
			}
			else
			{
				summary.FormMeritsRefreshed++;
			}
		}
	}

	private static XElement BuildAdditionalBodyFormMeritDefinition(SupernaturalFormTemplate template, long raceId,
		long alwaysTrueProgId, long alwaysFalseProgId)
	{
		return new XElement("Merit",
			new XElement("ChargenAvailableProg", alwaysFalseProgId),
			new XElement("ApplicabilityProg", alwaysTrueProgId),
			new XElement("ChargenBlurb", new XCData(template.ChargenBlurb)),
			new XElement("DescriptionText", new XCData(template.DescriptionText)),
			new XElement("Race", raceId),
			new XElement("Ethnicity", 0L),
			new XElement("Gender", -1),
			new XElement("Alias", template.Alias),
			new XElement("SortOrder", template.SortOrder),
			new XElement("TraumaMode", (int)template.TraumaMode),
			new XElement("TransformationEcho",
				new XAttribute("mode", "default"),
				string.Empty),
			new XElement("AllowVoluntarySwitch", template.AllowVoluntarySwitch),
			new XElement("CanVoluntarilySwitchProg", alwaysTrueProgId),
			new XElement("WhyCannotVoluntarilySwitchProg", 0L),
			new XElement("CanSeeFormProg", alwaysTrueProgId),
			new XElement("ShortDescriptionPattern", 0L),
			new XElement("FullDescriptionPattern", 0L),
			new XElement("AutoTransformWhenApplicable", template.AutoTransformWhenApplicable),
			new XElement("ForcedTransformationPriorityBand", 0),
			new XElement("ForcedTransformationPriorityOffset", 0),
			new XElement("ApplicabilityRecheckCadence", 1));
	}

	private static PlanarPresenceDefinition BuildPlanarProfile(long primePlaneId, long astralPlaneId,
		SupernaturalPlanarProfile profile)
	{
		long[] primeOnly = [primePlaneId];
		long[] astralOnly = [astralPlaneId];
		long[] both = [primePlaneId, astralPlaneId];

		static IDictionary<PlanarInteractionKind, IEnumerable<long>> InteractionMap(
			IEnumerable<long> nonPhysical,
			IEnumerable<long> physical)
		{
			long[] nonPhysicalArray = nonPhysical.ToArray();
			long[] physicalArray = physical.ToArray();
			return Enum.GetValues<PlanarInteractionKind>()
				.ToDictionary(
					kind => kind,
					kind => kind is PlanarInteractionKind.Observe or PlanarInteractionKind.Hear or
						PlanarInteractionKind.Speak or PlanarInteractionKind.Magic
							? (IEnumerable<long>)nonPhysicalArray
							: physicalArray);
		}

		return profile switch
		{
			SupernaturalPlanarProfile.AstralNative => new PlanarPresenceDefinition(
				astralOnly,
				astralOnly,
				both,
				InteractionMap(both, astralOnly),
				true,
				false,
				true,
				false,
				true,
				true,
				true,
				"astral-native"),
			SupernaturalPlanarProfile.Incorporeal => new PlanarPresenceDefinition(
				astralOnly,
				both,
				both,
				InteractionMap(both, Array.Empty<long>()),
				true,
				false,
				true,
				false,
				true,
				true,
				true,
				"incorporeal"),
			SupernaturalPlanarProfile.DualNatured => new PlanarPresenceDefinition(
				both,
				both,
				both,
				InteractionMap(both, both),
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				"dual-natured"),
			SupernaturalPlanarProfile.Manifested => new PlanarPresenceDefinition(
				primeOnly,
				primeOnly,
				both,
				InteractionMap(both, primeOnly),
				false,
				true,
				false,
				false,
				false,
				false,
				true,
				"manifested"),
			_ => PlanarPresenceDefinition.DefaultMaterial(primePlaneId)
		};
	}

	private sealed class SupernaturalSeedSummary
	{
		public int BodiesAdded { get; set; }
		public int BodiesRefreshed { get; set; }
		public int RacesAdded { get; set; }
		public int RacesRefreshed { get; set; }
		public int AttacksAdded { get; set; }
		public int CulturesAdded { get; set; }
		public int NameCulturesRefreshed { get; set; }
		public int FormMeritsAdded { get; set; }
		public int FormMeritsRefreshed { get; set; }
		public int DescriptionPatternsAdded { get; set; }

		public string ToMessage(int templateCount)
		{
			List<string> parts = new()
			{
				RacesAdded > 0
					? $"installed {RacesAdded} supernatural races"
					: $"refreshed {RacesRefreshed} existing supernatural races",
				BodiesAdded > 0
					? $"created {BodiesAdded} supernatural body prototypes"
					: $"refreshed {BodiesRefreshed} supernatural body prototypes",
				$"verified {templateCount} catalogue templates"
			};

			if (AttacksAdded > 0)
			{
				parts.Add($"added {AttacksAdded} supernatural attacks");
			}

			if (CulturesAdded > 0)
			{
				parts.Add($"added {CulturesAdded} cultures");
			}

			if (NameCulturesRefreshed > 0)
			{
				parts.Add($"refreshed {NameCulturesRefreshed} name cultures");
			}

			if (FormMeritsAdded > 0 || FormMeritsRefreshed > 0)
			{
				parts.Add($"prepared {FormMeritsAdded + FormMeritsRefreshed} additional-body-form merits");
			}

			if (DescriptionPatternsAdded > 0)
			{
				parts.Add($"added {DescriptionPatternsAdded} description patterns");
			}

			return $"{string.Join(", ", parts)}.";
		}
	}
}
