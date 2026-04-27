#nullable enable

using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Form.Characteristics;
using MudSharp.GameItems;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;

namespace DatabaseSeeder.Seeders;

public partial class SupernaturalSeeder
{
	internal const string NonBreathingModel = "non-breather";

	internal enum SupernaturalFamily
	{
		Angel,
		Demon,
		Divine,
		Spirit,
		Therianthrope,
		Undead
	}

	internal enum SupernaturalPlanarProfile
	{
		Material,
		AstralNative,
		Incorporeal,
		DualNatured,
		Manifested
	}

	internal enum SupernaturalNeedsProfile
	{
		Living,
		NonLiving
	}

	internal sealed record SupernaturalAgeProfile(
		int ChildAge,
		int YouthAge,
		int YoungAdultAge,
		int AdultAge,
		int ElderAge,
		int VenerableAge
	);

	internal sealed record SupernaturalAttackTemplate(
		string AttackName,
		ItemQuality Quality,
		IReadOnlyList<string> BodypartAliases
	);

	internal sealed record SupernaturalBodypartUsageTemplate(
		string BodypartAlias,
		string Usage
	);

	internal sealed record SupernaturalCharacteristicTemplate(
		string DefinitionName,
		IReadOnlyList<string> Values,
		string Usage = "base",
		CharacteristicType Type = CharacteristicType.Standard
	);

	internal sealed record SupernaturalRaceTemplate(
		string Name,
		SupernaturalFamily Family,
		string BodyKey,
		SizeCategory Size,
		SupernaturalPlanarProfile PlanarProfile,
		SupernaturalNeedsProfile NeedsProfile,
		NonHumanAttributeProfile AttributeProfile,
		string Description,
		string RoleDescription,
		IReadOnlyList<StockDescriptionVariant> DescriptionVariants,
		IReadOnlyList<SupernaturalAttackTemplate> Attacks,
		IReadOnlyList<SupernaturalCharacteristicTemplate> Characteristics,
		IReadOnlyList<SupernaturalBodypartUsageTemplate>? BodypartUsages = null,
		string CultureKey = "Spirit Court",
		string NameCultureKey = "Spirit Name",
		string CombatStrategyKey = "Melee (Auto)",
		bool PlayableDefault = false,
		bool CanUseWeapons = true,
		bool CanClimb = true,
		bool CanSwim = true,
		bool UsesHumanoidCharacteristics = true,
		double BodypartHealthMultiplier = 1.0
	);

	internal sealed record SupernaturalFormTemplate(
		string MeritName,
		string TargetRaceName,
		string Alias,
		int SortOrder,
		bool AllowVoluntarySwitch,
		bool AutoTransformWhenApplicable,
		BodySwitchTraumaMode TraumaMode,
		string ChargenBlurb,
		string DescriptionText
	);

	internal static IReadOnlyDictionary<string, SupernaturalRaceTemplate> TemplatesForTesting => Templates;
	internal static IReadOnlyList<string> AngelicRankOrderForTesting => AngelicRankOrder;
	internal static IReadOnlyList<string> CommonDemonNamesForTesting => CommonDemonNames;
	internal static IReadOnlyList<string> SupportedUndeadNamesForTesting => SupportedUndeadNames;
	internal static IReadOnlyList<SupernaturalFormTemplate> FormTemplatesForTesting => FormTemplates;
	internal static string NonBreathingModelForTesting => NonBreathingModel;

	private static readonly IReadOnlyList<string> AngelicRankOrder =
	[
		"Chayot HaKodesh",
		"Ophanim",
		"Erelim",
		"Hashmallim",
		"Seraphim",
		"Malakhim",
		"Elohim",
		"Bene Elohim",
		"Cherubim",
		"Ishim"
	];

	private static readonly IReadOnlyList<string> CommonDemonNames =
	[
		"Incubus",
		"Succubus",
		"Fury",
		"Imp",
		"Familiar",
		"Fiend",
		"Hellhound"
	];

	private static readonly IReadOnlyList<string> SupportedUndeadNames =
	[
		"Ghost",
		"Wraith",
		"Vampire",
		"Lich",
		"Ghoul",
		"Zombie",
		"Skeleton",
		"Mummy"
	];

	private static readonly IReadOnlyList<SupernaturalFormTemplate> FormTemplates =
	[
		new(
			"Supernatural Werewolf Hybrid Form",
			"Werewolf Hybrid",
			"hybrid",
			10,
			true,
			false,
			BodySwitchTraumaMode.Automatic,
			"Grants access to a builder-controlled wolf-man battle form.",
			"$0 have|has access to a builder-controlled werewolf hybrid form."),
		new(
			"Supernatural Werewolf Wolf Form",
			"Wolf",
			"wolf",
			20,
			true,
			false,
			BodySwitchTraumaMode.Automatic,
			"Grants access to a builder-controlled wolf form when the Animal package is installed.",
			"$0 have|has access to a builder-controlled wolf form."),
		new(
			"Supernatural Ghost Manifestation",
			"Ghost",
			"manifested ghost",
			10,
			true,
			false,
			BodySwitchTraumaMode.Stash,
			"Grants access to a visible ghostly manifestation form.",
			"$0 can|can appear in a visible ghostly manifestation."),
		new(
			"Supernatural Spirit Manifestation",
			"Spirit",
			"manifested spirit",
			10,
			true,
			false,
			BodySwitchTraumaMode.Stash,
			"Grants access to a visible spirit manifestation form.",
			"$0 can|can appear in a visible spirit manifestation."),
		new(
			"Supernatural Angelic Manifestation",
			"Ishim",
			"angelic manifestation",
			10,
			true,
			false,
			BodySwitchTraumaMode.Stash,
			"Grants access to a human-facing angelic manifestation form.",
			"$0 can|can assume a human-facing angelic manifestation."),
		new(
			"Supernatural Demonic Manifestation",
			"Fiend",
			"demonic manifestation",
			10,
			true,
			false,
			BodySwitchTraumaMode.Stash,
			"Grants access to a human-facing demonic manifestation form.",
			"$0 can|can assume a human-facing demonic manifestation.")
	];

	private static readonly IReadOnlyDictionary<string, SupernaturalRaceTemplate> Templates =
		new ReadOnlyDictionary<string, SupernaturalRaceTemplate>(BuildTemplates());

	private static Dictionary<string, SupernaturalRaceTemplate> BuildTemplates()
	{
		static SupernaturalAttackTemplate Attack(string name, ItemQuality quality, params string[] aliases)
		{
			return new(name, quality, aliases);
		}

		static SupernaturalCharacteristicTemplate Characteristic(string name, params string[] values)
		{
			return new(name, values);
		}

		static NonHumanAttributeProfile Stats(
			int strength,
			int constitution,
			int agility,
			int dexterity,
			int willpower,
			int perception,
			int aura,
			string? auraDiceExpression = null)
		{
			return new(
				strength,
				constitution,
				agility,
				dexterity,
				willpower,
				perception,
				aura,
				AuraDiceExpression: auraDiceExpression);
		}

		static IReadOnlyList<StockDescriptionVariant> Variants(
			string type,
			string signature,
			string bearing)
		{
			return SeederDescriptionHelpers.BuildVariantList(
				(
					$"a {signature} {type}",
					SeederDescriptionHelpers.JoinParagraphs(
						$"This {type} carries a {signature} presence that makes the air around it feel deliberate and charged.",
						$"Its body language is {bearing}, with small details that give builders a strong first impression without locking the creature to one setting.",
						"The stock description leaves room for local cosmology, patron, cult, court or curse details to be layered through normal builder tools.")
				),
				(
					$"a {bearing} {type}",
					SeederDescriptionHelpers.JoinParagraphs(
						$"This {type} has a {bearing} aspect, marked by controlled movement and a supernatural stillness.",
						$"The {signature} details are visible enough to distinguish it from ordinary mortal stock while still supporting varied worlds.",
						"Builders can use this variant as-is or attach more specific description patterns for named orders, hosts, courts or bloodlines.")
				));
		}

		static string RaceDescription(string name, string family, string role)
		{
			return SeederDescriptionHelpers.JoinParagraphs(
				$"{name} are part of the stock supernatural {family} catalogue seeded for builders who want powerful non-mortal characters, NPCs, patrons or adversaries.",
				$"The template focuses on {role}, supplying a complete playable engine shape while leaving cosmology, theology and setting-specific allegiance under builder control.",
				"By default the race is builder-only in chargen. Its anatomy, needs, planar profile, description variables and combat hooks are installed so it can function immediately once a builder chooses to expose it.");
		}

		static string RoleDescription(string name, string role)
		{
			return SeederDescriptionHelpers.JoinParagraphs(
				$"{name} are intended for {role}.",
				"They are seeded as stock examples, not as a statement that every game must use the same mythology.",
				"Their strongest value is giving builders working examples of supernatural anatomy, planar presence, alternate forms and race-specific description variables.");
		}

		static SupernaturalRaceTemplate Template(
			string name,
			SupernaturalFamily family,
			string body,
			SizeCategory size,
			SupernaturalPlanarProfile planar,
			SupernaturalNeedsProfile needs,
			NonHumanAttributeProfile stats,
			string role,
			IReadOnlyList<SupernaturalAttackTemplate> attacks,
			IReadOnlyList<SupernaturalCharacteristicTemplate> characteristics,
			string culture,
			string nameCulture,
			bool weapons = true,
			bool humanoid = true,
			string combat = "Melee (Auto)",
			double health = 1.0)
		{
			string familyName = family.ToString().ToLowerInvariant();
			return new(
				name,
				family,
				body,
				size,
				planar,
				needs,
				stats,
				RaceDescription(name, familyName, role),
				RoleDescription(name, role),
				Variants(name.ToLowerInvariant(), characteristics.First().Values.First().ToLowerInvariant(), role.ToLowerInvariant()),
				attacks,
				characteristics,
				CultureKey: culture,
				NameCultureKey: nameCulture,
				CombatStrategyKey: combat,
				CanUseWeapons: weapons,
				CanClimb: humanoid,
				CanSwim: true,
				UsesHumanoidCharacteristics: humanoid,
				BodypartHealthMultiplier: health);
		}

		var templates = new Dictionary<string, SupernaturalRaceTemplate>(StringComparer.OrdinalIgnoreCase);

		void Add(SupernaturalRaceTemplate template)
		{
			templates[template.Name] = template;
		}

		var radiant = Characteristic("Halo Radiance", "soft aureole", "burning crown", "star-bright nimbus", "hidden glimmer");
		var wings = Characteristic("Wing Aspect", "snow-white wings", "bronze-edged wings", "many-eyed pinions", "shadowless wings");
		var eyes = Characteristic("Eye Motif", "golden eyes", "many watchful eyes", "flame-blue eyes", "mirror-bright eyes");
		var infernal = Characteristic("Infernal Mark", "ember-scored sigils", "blackened veins", "smoke-wreathed aura", "hellish brand");
		var horns = Characteristic("Horn Style", "swept-back horns", "broken horns", "spiralled horns", "small ivory horns");
		var tail = Characteristic("Tail Form", "barbed tail", "serpentine tail", "forked tail", "lash-like tail");
		var death = Characteristic("Death Mark", "grave-pale skin", "ashen stains", "mummified flesh", "bone-white pallor");
		var spirit = Characteristic("Spirit Manifestation", "mist-thin outline", "lantern glow", "smoke-edged silhouette", "rainbow shimmer");
		var corpse = Characteristic("Corpse Condition", "freshly dead", "ancient bones", "wrapped and preserved", "restless remains");
		var beast = Characteristic("Bestial Transformation Tell", "yellow wolf eyes", "coarse hackles", "clawed hands", "muzzled profile");

		var angelAttacks = new[]
		{
			Attack("Radiant Touch", ItemQuality.Good, "rhand", "lhand"),
			Attack("Radiant Gaze", ItemQuality.Good, "reye", "leye")
		};
		var wingedAngelAttacks = angelAttacks.Append(Attack("Wing Buffet", ItemQuality.Standard, "rwingbase", "lwingbase")).ToArray();
		var demonAttacks = new[]
		{
			Attack("Infernal Claw", ItemQuality.Good, "rhand", "lhand"),
			Attack("Horn Gore", ItemQuality.Standard, "rhorn", "lhorn"),
			Attack("Fanged Bite", ItemQuality.Standard, "mouth")
		};
		var spiritAttacks = new[]
		{
			Attack("Spectral Touch", ItemQuality.Standard, "rhand", "lhand"),
			Attack("Soul Chill", ItemQuality.Standard, "rhand", "lhand")
		};
		var undeadAttacks = new[]
		{
			Attack("Grave Claw", ItemQuality.Standard, "rhand", "lhand"),
			Attack("Fanged Bite", ItemQuality.Standard, "mouth")
		};

		foreach ((string rank, int index) in AngelicRankOrder.Select((rank, index) => (rank, index)))
		{
			string body = rank switch
			{
				"Ophanim" => "Supernatural Ophanic Wheel",
				"Chayot HaKodesh" or "Seraphim" => "Supernatural Many-Winged Angel",
				"Ishim" => "Supernatural Angelic Humanoid",
				_ => "Supernatural Winged Angel"
			};
			SupernaturalAttackTemplate[] attacks = rank == "Ophanim"
				? [Attack("Wheel Crush", ItemQuality.VeryGood, "torso"), Attack("Radiant Gaze", ItemQuality.Good, "reye", "leye")]
				: wingedAngelAttacks;
			Add(Template(rank, SupernaturalFamily.Angel, body, index < 2 ? SizeCategory.Large : SizeCategory.Normal,
				SupernaturalPlanarProfile.DualNatured, SupernaturalNeedsProfile.NonLiving,
				Stats(2 + Math.Max(0, 4 - index / 2), 2 + Math.Max(0, 4 - index / 2), 1, 1, 4, 3, 5, "2d4+6"),
				"celestial order and divine emissary play", attacks, [radiant, wings, eyes], "Celestial Host", "Angelic Name",
				health: 1.2));
		}

		foreach ((string rank, int index) in AngelicRankOrder.Select((rank, index) => (rank, index)))
		{
			string fallenName = $"Fallen {rank}";
			string body = rank == "Ophanim" ? "Supernatural Ophanic Wheel" : "Supernatural Horned Fiend";
			SupernaturalAttackTemplate[] attacks = rank == "Ophanim"
				? [Attack("Wheel Crush", ItemQuality.VeryGood, "torso"), Attack("Soul Chill", ItemQuality.Good, "reye", "leye")]
				: demonAttacks;
			Add(Template(fallenName, SupernaturalFamily.Demon, body, index < 2 ? SizeCategory.Large : SizeCategory.Normal,
				SupernaturalPlanarProfile.AstralNative, SupernaturalNeedsProfile.NonLiving,
				Stats(3 + Math.Max(0, 3 - index / 3), 2 + Math.Max(0, 3 - index / 3), 1, 1, 4, 2, 4, "2d4+5"),
				"fallen celestial adversary and infernal court play", attacks, [infernal, horns, tail, eyes], "Fallen Host",
				"Demonic Name", health: 1.2));
		}

		Add(Template("Incubus", SupernaturalFamily.Demon, "Supernatural Horned Fiend", SizeCategory.Normal,
			SupernaturalPlanarProfile.AstralNative, SupernaturalNeedsProfile.NonLiving, Stats(1, 1, 2, 2, 3, 2, 4, "2d4+4"),
			"social predator and temptation storylines", demonAttacks, [infernal, horns, tail], "Fallen Host", "Demonic Name"));
		Add(Template("Succubus", SupernaturalFamily.Demon, "Supernatural Horned Fiend", SizeCategory.Normal,
			SupernaturalPlanarProfile.AstralNative, SupernaturalNeedsProfile.NonLiving, Stats(1, 1, 2, 2, 3, 2, 4, "2d4+4"),
			"social predator and temptation storylines", demonAttacks, [infernal, horns, tail], "Fallen Host", "Demonic Name"));
		Add(Template("Fury", SupernaturalFamily.Demon, "Supernatural Horned Fiend", SizeCategory.Normal,
			SupernaturalPlanarProfile.DualNatured, SupernaturalNeedsProfile.NonLiving, Stats(3, 2, 3, 1, 3, 3, 3, "2d4+3"),
			"vengeance spirit and relentless hunter play", demonAttacks, [infernal, wings, eyes], "Fallen Host", "Demonic Name",
			combat: "Beast Brawler"));
		Add(Template("Imp", SupernaturalFamily.Demon, "Supernatural Familiar", SizeCategory.Small,
			SupernaturalPlanarProfile.AstralNative, SupernaturalNeedsProfile.NonLiving, Stats(-2, -1, 3, 2, 1, 2, 2, "1d4+2"),
			"minor demon, spy and nuisance familiar play", demonAttacks, [infernal, horns, tail], "Fallen Host", "Demonic Name",
			weapons: false, combat: "Beast Skirmisher", health: 0.75));
		Add(Template("Familiar", SupernaturalFamily.Demon, "Supernatural Familiar", SizeCategory.Small,
			SupernaturalPlanarProfile.AstralNative, SupernaturalNeedsProfile.NonLiving, Stats(-3, -1, 4, 2, 2, 3, 2, "1d4+2"),
			"bound companion, spy and occult assistant play", demonAttacks, [infernal, tail, eyes], "Fallen Host", "Demonic Name",
			weapons: false, combat: "Beast Skirmisher", health: 0.7));
		Add(Template("Fiend", SupernaturalFamily.Demon, "Supernatural Horned Fiend", SizeCategory.Large,
			SupernaturalPlanarProfile.AstralNative, SupernaturalNeedsProfile.NonLiving, Stats(5, 4, 0, 0, 3, 2, 3, "2d4+4"),
			"brutal infernal monster and war-demon play", demonAttacks, [infernal, horns, tail], "Fallen Host", "Demonic Name",
			combat: "Beast Brawler", health: 1.3));
		Add(Template("Hellhound", SupernaturalFamily.Demon, "Supernatural Hellhound", SizeCategory.Large,
			SupernaturalPlanarProfile.Material, SupernaturalNeedsProfile.NonLiving, Stats(4, 3, 2, 0, 2, 4, 2, "1d4+2"),
			"infernal hunting beast play", [Attack("Fanged Bite", ItemQuality.VeryGood, "mouth"), Attack("Infernal Claw", ItemQuality.Good, "rfclaw", "lfclaw", "rrclaw", "lrclaw")],
			[infernal, eyes], "Fallen Host", "Demonic Name", weapons: false, humanoid: false, combat: "Beast Brawler", health: 1.2));

		Add(Template("Demigod", SupernaturalFamily.Divine, "Supernatural Divine Humanoid", SizeCategory.Normal,
			SupernaturalPlanarProfile.DualNatured, SupernaturalNeedsProfile.NonLiving, Stats(3, 3, 1, 1, 3, 2, 5, "2d4+5"),
			"divine-blooded heroes, avatars and staff-guided legends", angelAttacks, [radiant, eyes], "Celestial Host", "Angelic Name"));
		Add(Template("Lesser God", SupernaturalFamily.Divine, "Supernatural Many-Winged Angel", SizeCategory.Large,
			SupernaturalPlanarProfile.DualNatured, SupernaturalNeedsProfile.NonLiving, Stats(6, 6, 2, 2, 6, 4, 8, "2d6+8"),
			"local deities, patrons and divine NPCs", wingedAngelAttacks, [radiant, wings, eyes], "Celestial Host", "Angelic Name",
			health: 1.5));
		Add(Template("Greater God", SupernaturalFamily.Divine, "Supernatural Many-Winged Angel", SizeCategory.VeryLarge,
			SupernaturalPlanarProfile.DualNatured, SupernaturalNeedsProfile.NonLiving, Stats(8, 8, 2, 2, 8, 5, 10, "2d6+10"),
			"major deities and administrator-facing divine examples", wingedAngelAttacks, [radiant, wings, eyes], "Celestial Host",
			"Angelic Name", health: 1.8));

		Add(Template("Spirit", SupernaturalFamily.Spirit, "Supernatural Spirit Form", SizeCategory.Normal,
			SupernaturalPlanarProfile.Incorporeal, SupernaturalNeedsProfile.NonLiving, Stats(-1, 0, 2, 1, 3, 3, 3, "2d4+3"),
			"generic incorporeal spirits", spiritAttacks, [spirit, eyes], "Spirit Court", "Spirit Name", combat: "Beast Skirmisher"));
		Add(Template("Ghost", SupernaturalFamily.Spirit, "Supernatural Spirit Form", SizeCategory.Normal,
			SupernaturalPlanarProfile.Incorporeal, SupernaturalNeedsProfile.NonLiving, Stats(-2, 0, 2, 1, 3, 3, 3, "2d4+3"),
			"dead souls and haunting NPCs", spiritAttacks, [spirit, death], "Spirit Court", "Spirit Name", combat: "Beast Skirmisher"));
		Add(Template("Specter", SupernaturalFamily.Spirit, "Supernatural Spirit Form", SizeCategory.Normal,
			SupernaturalPlanarProfile.Incorporeal, SupernaturalNeedsProfile.NonLiving, Stats(-1, 1, 3, 2, 4, 4, 3, "2d4+3"),
			"predatory hauntings and feared incorporeal monsters", spiritAttacks, [spirit, death], "Spirit Court", "Spirit Name",
			combat: "Beast Skirmisher"));
		Add(Template("Wraith", SupernaturalFamily.Spirit, "Supernatural Spirit Form", SizeCategory.Normal,
			SupernaturalPlanarProfile.Incorporeal, SupernaturalNeedsProfile.NonLiving, Stats(0, 1, 3, 2, 4, 4, 4, "2d4+4"),
			"dangerous deathly spirits", spiritAttacks, [spirit, death], "Spirit Court", "Spirit Name", combat: "Beast Skirmisher"));
		Add(Template("Ancestral Spirit", SupernaturalFamily.Spirit, "Supernatural Spirit Form", SizeCategory.Normal,
			SupernaturalPlanarProfile.Incorporeal, SupernaturalNeedsProfile.NonLiving, Stats(-1, 0, 1, 1, 4, 3, 4, "2d4+4"),
			"ancestor worship, family guardians and cultural spirit play", spiritAttacks, [spirit, eyes], "Spirit Court", "Spirit Name"));
		Add(Template("Nature Spirit", SupernaturalFamily.Spirit, "Supernatural Spirit Form", SizeCategory.Normal,
			SupernaturalPlanarProfile.Incorporeal, SupernaturalNeedsProfile.NonLiving, Stats(0, 1, 2, 1, 3, 4, 4, "2d4+4"),
			"wilderness, grove and land-spirit play", spiritAttacks, [spirit, eyes], "Spirit Court", "Spirit Name"));
		Add(Template("Elemental Spirit", SupernaturalFamily.Spirit, "Supernatural Spirit Form", SizeCategory.Normal,
			SupernaturalPlanarProfile.Incorporeal, SupernaturalNeedsProfile.NonLiving, Stats(1, 1, 2, 1, 3, 3, 4, "2d4+4"),
			"fire, storm, water, earth and other elemental courts", spiritAttacks, [spirit, eyes], "Spirit Court", "Spirit Name"));

		Add(Template("Werewolf", SupernaturalFamily.Therianthrope, "Organic Humanoid", SizeCategory.Normal,
			SupernaturalPlanarProfile.Material, SupernaturalNeedsProfile.Living, Stats(1, 1, 1, 0, 1, 2, 0),
			"cursed or hereditary shapeshifter play", [Attack("Fanged Bite", ItemQuality.Standard, "mouth")], [beast],
			"Therianthrope", "Mortal Name"));
		Add(Template("Werewolf Hybrid", SupernaturalFamily.Therianthrope, "Supernatural Werewolf Hybrid", SizeCategory.Large,
			SupernaturalPlanarProfile.Material, SupernaturalNeedsProfile.Living, Stats(4, 3, 2, 0, 2, 3, 0),
			"wolf-man battle forms and cursed transformation play", [Attack("Fanged Bite", ItemQuality.VeryGood, "mouth"), Attack("Infernal Claw", ItemQuality.Good, "rhand", "lhand")],
			[beast], "Therianthrope", "Mortal Name", weapons: false, combat: "Beast Brawler", health: 1.25));

		Add(Template("Vampire", SupernaturalFamily.Undead, "Organic Humanoid", SizeCategory.Normal,
			SupernaturalPlanarProfile.Material, SupernaturalNeedsProfile.NonLiving, Stats(2, 2, 2, 1, 3, 3, 3, "2d4+3"),
			"deathless social predators without hard-coded feeding mechanics", undeadAttacks, [death, eyes], "Undead Remnant", "Undead Name"));
		Add(Template("Lich", SupernaturalFamily.Undead, "Supernatural Lich Skeleton", SizeCategory.Normal,
			SupernaturalPlanarProfile.DualNatured, SupernaturalNeedsProfile.NonLiving, Stats(-1, 1, 0, 1, 6, 4, 6, "2d6+6"),
			"undead sorcerers, ancient powers and staff-controlled antagonists", undeadAttacks, [death, corpse, eyes], "Undead Remnant", "Undead Name"));
		Add(Template("Ghoul", SupernaturalFamily.Undead, "Supernatural Decayed Undead", SizeCategory.Normal,
			SupernaturalPlanarProfile.Material, SupernaturalNeedsProfile.NonLiving, Stats(2, 2, 1, 0, 1, 2, 0),
			"grave-hungry undead servants without bespoke feeding mechanics", undeadAttacks, [death, corpse], "Undead Remnant", "Undead Name",
			weapons: false, combat: "Beast Brawler"));
		Add(Template("Zombie", SupernaturalFamily.Undead, "Supernatural Decayed Undead", SizeCategory.Normal,
			SupernaturalPlanarProfile.Material, SupernaturalNeedsProfile.NonLiving, Stats(2, 4, -2, -2, -2, -1, 0),
			"durable animated corpses and simple undead threats", undeadAttacks, [death, corpse], "Undead Remnant", "Undead Name",
			weapons: false, combat: "Beast Brawler", health: 1.35));
		Add(Template("Skeleton", SupernaturalFamily.Undead, "Supernatural Skeleton", SizeCategory.Normal,
			SupernaturalPlanarProfile.Material, SupernaturalNeedsProfile.NonLiving, Stats(0, 1, 1, 0, 0, 1, 0),
			"animated bones and low-maintenance undead guards", undeadAttacks, [death, corpse], "Undead Remnant", "Undead Name",
			weapons: true, health: 0.9));
		Add(Template("Mummy", SupernaturalFamily.Undead, "Supernatural Decayed Undead", SizeCategory.Normal,
			SupernaturalPlanarProfile.Material, SupernaturalNeedsProfile.NonLiving, Stats(3, 4, -1, -1, 3, 1, 2, "1d4+2"),
			"preserved cursed dead and ancient tomb guardians", undeadAttacks, [death, corpse], "Undead Remnant", "Undead Name",
			weapons: true, combat: "Beast Brawler", health: 1.4));

		return templates;
	}

	internal static string BuildRaceDescriptionForTesting(SupernaturalRaceTemplate template)
	{
		return template.Description;
	}

	internal static string BuildEthnicityDescriptionForTesting(SupernaturalRaceTemplate template)
	{
		return SeederDescriptionHelpers.JoinParagraphs(
			$"{template.Name} Stock is the default supernatural ethnicity installed for the {template.Name} race.",
			$"It provides characteristic profiles and random description support for {template.RoleDescription.Split('\n')[0].TrimEnd('.')}.",
			"Games can rename, clone or extend it for specific hosts, courts, bloodlines, cults, pantheons, curses or necromantic traditions.");
	}

	internal static XElement BuildPlanarProfileXmlForTesting(long primePlaneId, long astralPlaneId,
		SupernaturalPlanarProfile profile)
	{
		return BuildPlanarProfile(primePlaneId, astralPlaneId, profile).SaveToXml();
	}

	internal static XElement BuildAdditionalBodyFormMeritDefinitionForTesting(
		SupernaturalFormTemplate template,
		long raceId,
		long alwaysTrueProgId,
		long alwaysFalseProgId)
	{
		return BuildAdditionalBodyFormMeritDefinition(template, raceId, alwaysTrueProgId, alwaysFalseProgId);
	}
}
