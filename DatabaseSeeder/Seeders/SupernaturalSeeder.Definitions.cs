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
		"Balrog",
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
			double health = 1.0,
			string? description = null)
		{
			return new(
				name,
				family,
				body,
				size,
				planar,
				needs,
				stats,
				description ?? throw new InvalidOperationException($"Supernatural race {name} is missing a seeded description."),
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
			Attack("Radiant Gaze", ItemQuality.Good, "reye", "leye"),
			Attack("Heavenly Choir", ItemQuality.Good, "mouth"),
			Attack("Canticle of Awe", ItemQuality.Good, "mouth"),
			Attack("Word of Command", ItemQuality.Good, "mouth"),
			Attack("Crown of Stars", ItemQuality.Good, "reye", "leye"),
			Attack("Mercy-Searing Grasp", ItemQuality.Good, "rhand", "lhand")
		};
		var wingedAngelAttacks = angelAttacks.Concat(
		[
			Attack("Wing Buffet", ItemQuality.Standard, "rwingbase", "lwingbase"),
			Attack("Seraphic Wingstorm", ItemQuality.Good, "rwingbase", "lwingbase")
		]).ToArray();
		var highAngelAttacks = wingedAngelAttacks.Concat(
		[
			Attack("Trumpet Peal", ItemQuality.VeryGood, "mouth"),
			Attack("Starfire Breath", ItemQuality.VeryGood, "mouth")
		]).ToArray();
		var ophanicAngelAttacks = new[]
		{
			Attack("Wheel Crush", ItemQuality.VeryGood, "abdomen"),
			Attack("Wheel of Judgment", ItemQuality.VeryGood, "abdomen"),
			Attack("Radiant Gaze", ItemQuality.Good, "reye", "leye"),
			Attack("Many-Eyed Ray", ItemQuality.Good, "reye", "leye"),
			Attack("Crown of Stars", ItemQuality.Good, "reye", "leye"),
			Attack("Heavenly Choir", ItemQuality.Good, "mouth"),
			Attack("Word of Command", ItemQuality.Good, "mouth")
		};
		var demonAttacks = new[]
		{
			Attack("Infernal Claw", ItemQuality.Good, "rhand", "lhand"),
			Attack("Horn Gore", ItemQuality.Standard, "rhorn", "lhorn"),
			Attack("Fanged Bite", ItemQuality.Standard, "mouth"),
			Attack("Brimstone Spit", ItemQuality.Standard, "mouth"),
			Attack("Infernal Trip", ItemQuality.Standard, "rfoot", "lfoot"),
			Attack("Damnation Barge", ItemQuality.Standard, "abdomen"),
			Attack("Hellish Headbutt", ItemQuality.Standard, "forehead"),
			Attack("Soul Hook", ItemQuality.Good, "rhand", "lhand"),
			Attack("Sinner's Clinch", ItemQuality.Good, "rhand", "lhand"),
			Attack("Barbed Tail Slap", ItemQuality.Standard, "utail", "mtail", "ltail"),
			Attack("Fallen Choir", ItemQuality.Good, "mouth")
		};
		var highDemonAttacks = demonAttacks.Concat(
		[
			Attack("Hellfire Breath", ItemQuality.VeryGood, "mouth"),
			Attack("Abyssal Chain Lash", ItemQuality.Good, "rhand", "lhand")
		]).ToArray();
		var fallenOphanicAttacks = new[]
		{
			Attack("Wheel Crush", ItemQuality.VeryGood, "abdomen"),
			Attack("Wheel of Judgment", ItemQuality.VeryGood, "abdomen"),
			Attack("Soul Chill", ItemQuality.Good, "reye", "leye"),
			Attack("Many-Eyed Ray", ItemQuality.Good, "reye", "leye"),
			Attack("Hellfire Breath", ItemQuality.VeryGood, "mouth"),
			Attack("Infernal Trip", ItemQuality.Standard, "rfoot", "lfoot"),
			Attack("Fallen Choir", ItemQuality.Good, "mouth")
		};
		var hellhoundAttacks = new[]
		{
			Attack("Fanged Bite", ItemQuality.VeryGood, "mouth"),
			Attack("Infernal Claw", ItemQuality.Good, "rfclaw", "lfclaw", "rrclaw", "lrclaw"),
			Attack("Hellfire Breath", ItemQuality.Good, "mouth"),
			Attack("Brimstone Spit", ItemQuality.Standard, "mouth"),
			Attack("Damnation Barge", ItemQuality.Good, "abdomen"),
			Attack("Hellish Headbutt", ItemQuality.Standard, "head"),
			Attack("Infernal Trip", ItemQuality.Standard, "rfpaw", "lfpaw", "rrpaw", "lrpaw"),
			Attack("Barbed Tail Slap", ItemQuality.Standard, "utail", "mtail", "ltail")
		};
		var spiritAttacks = new[]
		{
			Attack("Spectral Touch", ItemQuality.Standard, "rhand", "lhand"),
			Attack("Soul Chill", ItemQuality.Standard, "rhand", "lhand"),
			Attack("Wailing Dirge", ItemQuality.Good, "mouth"),
			Attack("Grave Drag", ItemQuality.Standard, "rhand", "lhand"),
			Attack("Deathly Pall", ItemQuality.Standard, "mouth")
		};
		var undeadAttacks = new[]
		{
			Attack("Grave Claw", ItemQuality.Standard, "rhand", "lhand"),
			Attack("Fanged Bite", ItemQuality.Standard, "mouth"),
			Attack("Grasp of the Dead", ItemQuality.Standard, "rhand", "lhand"),
			Attack("Grave Drag", ItemQuality.Standard, "rhand", "lhand"),
			Attack("Bone Rattle", ItemQuality.Standard, "forehead"),
			Attack("Crypt Dust Breath", ItemQuality.Standard, "mouth"),
			Attack("Deathly Pall", ItemQuality.Standard, "mouth")
		};
		var werewolfAttacks = new[]
		{
			Attack("Fanged Bite", ItemQuality.Standard, "mouth"),
			Attack("Hamstring Snap", ItemQuality.Standard, "mouth"),
			Attack("Wolf Trip", ItemQuality.Standard, "rfoot", "lfoot"),
			Attack("Crushing Pounce", ItemQuality.Standard, "abdomen")
		};
		var werewolfHybridAttacks = new[]
		{
			Attack("Fanged Bite", ItemQuality.VeryGood, "mouth"),
			Attack("Hamstring Snap", ItemQuality.Good, "mouth"),
			Attack("Wolf Trip", ItemQuality.Good, "rfoot", "lfoot"),
			Attack("Crushing Pounce", ItemQuality.Good, "abdomen"),
			Attack("Raking Maul", ItemQuality.Good, "rhand", "lhand")
		};

		        Add(Template("Chayot HaKodesh", SupernaturalFamily.Angel, "Supernatural Many-Winged Angel", SizeCategory.Large,
            SupernaturalPlanarProfile.DualNatured, SupernaturalNeedsProfile.NonLiving,
            Stats(6, 6, 1, 1, 4, 3, 5, "2d4+6"),
            "celestial order and divine emissary play", highAngelAttacks, [radiant, wings, eyes], "Celestial Host", "Angelic Name",
            health: 1.2,
            description: """
            The Chayot HaKodesh are high angelic living-creatures, many-winged and overwhelming, whose presence suggests a throne-room seen through fire and storm. They are not simply messengers; they are bearers of holy motion, sacred authority and the terrible vitality nearest the divine centre.

            Their forms may combine wings, eyes, radiant crowns and impossible animal majesty into a body that feels too symbolically dense for mortal comfort. To look on one is to feel that life itself has been appointed, ordered and armed.

            They belong in scenes of revelation, cosmic procession, divine judgement and awe. A Chayot HaKodesh should make even powerful mortals feel like witnesses standing at the edge of something vastly older than law or kingship.
            """));
        Add(Template("Ophanim", SupernaturalFamily.Angel, "Supernatural Ophanic Wheel", SizeCategory.Large,
            SupernaturalPlanarProfile.DualNatured, SupernaturalNeedsProfile.NonLiving,
            Stats(6, 6, 1, 1, 4, 3, 5, "2d4+6"),
            "celestial order and divine emissary play", ophanicAngelAttacks, [radiant, wings, eyes], "Celestial Host", "Angelic Name",
            health: 1.2,
            description: """
            The Ophanim are wheel-like angelic powers of motion, vigilance and judgement. Their nature is not merely winged or humanoid but geometric, a turning holiness marked by eyes, radiance and the sense that nothing escapes being seen.

            They embody the frightening precision of divine order in motion. Where other angels may speak or descend, an Ophan seems to revolve through command itself, bringing verdict, transport and witness in one impossible form.

            They suit visions, gates, celestial engines and moments when judgement feels impersonal but alive. An Ophan should make space feel measured from every direction at once.
            """));
        Add(Template("Erelim", SupernaturalFamily.Angel, "Supernatural Winged Angel", SizeCategory.Normal,
            SupernaturalPlanarProfile.DualNatured, SupernaturalNeedsProfile.NonLiving,
            Stats(5, 5, 1, 1, 4, 3, 5, "2d4+6"),
            "celestial order and divine emissary play", highAngelAttacks, [radiant, wings, eyes], "Celestial Host", "Angelic Name",
            health: 1.2,
            description: """
            The Erelim are mighty angelic guardians associated with solemn strength, courage and the gravity of sacred loss. They are less intimate than messengers and less abstract than wheels, standing instead as luminous powers of endurance and witness.

            Their bearing should feel grave rather than gentle: bright wings, still eyes and the composure of beings who have stood beside death, judgement and difficult mercy without turning away. They are the angels who remain when awe must become duty.

            They fit tombs of saints, battlefields after the noise has ended, royal thresholds and divine courts where courage is measured by restraint. An Erel reads as a guardian of weighty moments.
            """));
        Add(Template("Hashmallim", SupernaturalFamily.Angel, "Supernatural Winged Angel", SizeCategory.Normal,
            SupernaturalPlanarProfile.DualNatured, SupernaturalNeedsProfile.NonLiving,
            Stats(5, 5, 1, 1, 4, 3, 5, "2d4+6"),
            "celestial order and divine emissary play", highAngelAttacks, [radiant, wings, eyes], "Celestial Host", "Angelic Name",
            health: 1.2,
            description: """
            The Hashmallim are angelic beings of charged silence, luminous fire and restrained speech. Their presence suggests power held behind a veil, as though the air has become bright with words too dangerous to utter lightly.

            They are not merely radiant; they feel electrically poised. Light, hush, ember and sudden command gather around them, making their stillness more unnerving than many creatures' threats.

            They suit revelations delayed, sacred councils, hidden sanctums and divine commands that arrive after unbearable quiet. A Hashmal should make silence feel active, intelligent and close to ignition.
            """));
        Add(Template("Seraphim", SupernaturalFamily.Angel, "Supernatural Many-Winged Angel", SizeCategory.Normal,
            SupernaturalPlanarProfile.DualNatured, SupernaturalNeedsProfile.NonLiving,
            Stats(4, 4, 1, 1, 4, 3, 5, "2d4+6"),
            "celestial order and divine emissary play", highAngelAttacks, [radiant, wings, eyes], "Celestial Host", "Angelic Name",
            health: 1.2,
            description: """
            The Seraphim are burning many-winged angels of praise, purification and unbearable nearness to holy fire. Their radiance is not decorative; it is cleansing, exalting and dangerous to anything unprepared for it.

            Their wings, voices and flame-bright presence make worship feel like force. A Seraph can comfort through glory, but the same glory can sear away corruption, falsehood or presumption without needing a blade.

            They belong in sanctuaries, apocalyptic visions and moments when divinity is felt as heat rather than law. A Seraph should make beauty and danger indistinguishable.
            """));
        Add(Template("Malakhim", SupernaturalFamily.Angel, "Supernatural Winged Angel", SizeCategory.Normal,
            SupernaturalPlanarProfile.DualNatured, SupernaturalNeedsProfile.NonLiving,
            Stats(4, 4, 1, 1, 4, 3, 5, "2d4+6"),
            "celestial order and divine emissary play", wingedAngelAttacks, [radiant, wings, eyes], "Celestial Host", "Angelic Name",
            health: 1.2,
            description: """
            The Malakhim are angelic messengers and envoys, defined by service, command-bearing and the terrible clarity of a word delivered from beyond mortal authority. They are often the most recognisably emissary-like of the angelic orders.

            Their power lies in arrival and declaration. Wings, radiance and calm bearing matter, but the central fact is that a Malakh carries a message that can bless, warn, sentence or redirect a life.

            They suit dreams, roads, thresholds and sudden appearances in ordinary places. A Malakh should feel approachable enough to speak with and alien enough that the conversation changes everything.
            """));
        Add(Template("Elohim", SupernaturalFamily.Angel, "Supernatural Winged Angel", SizeCategory.Normal,
            SupernaturalPlanarProfile.DualNatured, SupernaturalNeedsProfile.NonLiving,
            Stats(3, 3, 1, 1, 4, 3, 5, "2d4+6"),
            "celestial order and divine emissary play", wingedAngelAttacks, [radiant, wings, eyes], "Celestial Host", "Angelic Name",
            health: 1.2,
            description: """
            The Elohim are angelic powers of authority, judgement and delegated dominion. They appear less as attendants and more as luminous magistrates, bearing the weight of law, creation and command in their posture.

            Their nature is sovereign without being divine in themselves. Eyes, voice, radiance and composure should suggest beings entrusted with power that can shape outcomes, settle disputes and enact sentences far beyond mortal courts.

            They belong in councils, high courts, covenants and scenes where spiritual law takes visible form. An Elohim should make authority feel numinous, exacting and difficult to appeal.
            """));
        Add(Template("Bene Elohim", SupernaturalFamily.Angel, "Supernatural Winged Angel", SizeCategory.Normal,
            SupernaturalPlanarProfile.DualNatured, SupernaturalNeedsProfile.NonLiving,
            Stats(3, 3, 1, 1, 4, 3, 5, "2d4+6"),
            "celestial order and divine emissary play", wingedAngelAttacks, [radiant, wings, eyes], "Celestial Host", "Angelic Name",
            health: 1.2,
            description: """
            The Bene Elohim are angelic scions and watchers, luminous beings close enough to creation to be fascinated by it and high enough to be dangerous when they intervene. Their nature carries inheritance, attention and the temptation of proximity.

            They read as celestial nobles, observers or patrons whose interest in mortal affairs may be protective, curious or troubling. Their radiance is less impersonal than an Ophan's and less ceremonial than a Seraph's, but it carries its own pressure.

            They suit watchtowers, pacts, hidden tutelage and stories where divine order brushes against mortal desire. A Bene Elohim should feel like a watcher whose attention is itself a consequence.
            """));
        Add(Template("Cherubim", SupernaturalFamily.Angel, "Supernatural Winged Angel", SizeCategory.Normal,
            SupernaturalPlanarProfile.DualNatured, SupernaturalNeedsProfile.NonLiving,
            Stats(2, 2, 1, 1, 4, 3, 5, "2d4+6"),
            "celestial order and divine emissary play", wingedAngelAttacks, [radiant, wings, eyes], "Celestial Host", "Angelic Name",
            health: 1.2,
            description: """
            The Cherubim are angelic guardians of thresholds, sanctuaries and forbidden things. Their role is protective but not soft; they stand where approach must be earned, halted or judged.

            They may bear wings, bright eyes, composite features or an almost sculptural poise, but the essential impression is warding. A Cherub makes a gate feel real even before the wall is seen, because the body itself says that passage is conditional.

            They suit temple doors, sealed gardens, relic chambers and the borders between mercy and prohibition. A Cherub should make trespass feel like a theological mistake as much as a tactical one.
            """));
        Add(Template("Ishim", SupernaturalFamily.Angel, "Supernatural Angelic Humanoid", SizeCategory.Normal,
            SupernaturalPlanarProfile.DualNatured, SupernaturalNeedsProfile.NonLiving,
            Stats(2, 2, 1, 1, 4, 3, 5, "2d4+6"),
            "celestial order and divine emissary play", angelAttacks, [radiant, wings, eyes], "Celestial Host", "Angelic Name",
            health: 1.2,
            description: """
            The Ishim are the most human-facing of the angelic orders, close to mortal lives, prayers, dreams and crises. Their forms may be less overwhelming than higher angels, but their nearness makes them unsettling in a more intimate way.

            They can speak across tables, appear on roads and stand beside the dying without shattering the scene around them. Their radiance is often moderated, hidden or focused through eyes, voice and timing rather than vast anatomy.

            They suit counsel, warning, quiet miracles and moments when the divine chooses to be comprehensible. An Ishim should feel like a stranger who knows exactly when to arrive.
            """));

		        Add(Template("Fallen Chayot HaKodesh", SupernaturalFamily.Demon, "Supernatural Horned Fiend", SizeCategory.Large,
            SupernaturalPlanarProfile.AstralNative, SupernaturalNeedsProfile.NonLiving,
            Stats(6, 5, 1, 1, 4, 2, 4, "2d4+5"),
            "fallen celestial adversary and infernal court play", highDemonAttacks, [infernal, horns, tail, eyes], "Fallen Host",
            "Demonic Name", health: 1.2,
            description: """
            Fallen Chayot HaKodesh are ruined living-creatures of high angelic order, bearers of sacred motion whose vitality has been bent into rebellion, tyranny or blasphemous sovereignty. They retain grandeur, which makes the corruption worse.

            Their many wings, crowns, eyes and composite majesty may still burn, but the fire no longer comforts. They suggest a throne procession broken from its rightful course, still powerful enough to drag lesser beings into its wake.

            They suit fallen courts, apocalyptic war, corrupted revelation and ancient exiles who remember standing near the centre of holiness. A Fallen Chayot HaKodesh should feel like sacred life weaponised against its source.
            """));
        Add(Template("Fallen Ophanim", SupernaturalFamily.Demon, "Supernatural Ophanic Wheel", SizeCategory.Large,
            SupernaturalPlanarProfile.AstralNative, SupernaturalNeedsProfile.NonLiving,
            Stats(6, 5, 1, 1, 4, 2, 4, "2d4+5"),
            "fallen celestial adversary and infernal court play", fallenOphanicAttacks, [infernal, horns, tail, eyes], "Fallen Host",
            "Demonic Name", health: 1.2,
            description: """
            Fallen Ophanim are darkened wheels of judgement, revolving angelic powers whose perfect motion has become accusation, obsession or merciless sentence. They remain terrifyingly ordered, but the order no longer implies justice.

            Their eyes may watch without compassion, and their turning geometry can make pursuit feel inevitable. Rather than chaos, they embody corrupted inevitability: a verdict that keeps arriving whether or not it is deserved.

            They fit cursed gates, infernal engines, fallen courts and visions of judgement stripped of mercy. A Fallen Ophan should make space feel trapped inside a sentence.
            """));
        Add(Template("Fallen Erelim", SupernaturalFamily.Demon, "Supernatural Horned Fiend", SizeCategory.Normal,
            SupernaturalPlanarProfile.AstralNative, SupernaturalNeedsProfile.NonLiving,
            Stats(6, 5, 1, 1, 4, 2, 4, "2d4+5"),
            "fallen celestial adversary and infernal court play", highDemonAttacks, [infernal, horns, tail, eyes], "Fallen Host",
            "Demonic Name", health: 1.2,
            description: """
            Fallen Erelim are mighty guardians whose solemn strength has curdled into grief, wrath or bitter honour. They still carry the bearing of sacred soldiers and witnesses, but their endurance now serves a broken cause.

            Their radiance may be bruised, their wings scarred and their silence heavy with remembered duty. They are not frivolous tempters; they are the dangerous dead weight of courage severed from mercy.

            They belong beside ruined battlefields, forsaken tombs and rebel hosts that speak in the language of loss. A Fallen Erel should feel tragic without becoming safe.
            """));
        Add(Template("Fallen Hashmallim", SupernaturalFamily.Demon, "Supernatural Horned Fiend", SizeCategory.Normal,
            SupernaturalPlanarProfile.AstralNative, SupernaturalNeedsProfile.NonLiving,
            Stats(5, 4, 1, 1, 4, 2, 4, "2d4+5"),
            "fallen celestial adversary and infernal court play", highDemonAttacks, [infernal, horns, tail, eyes], "Fallen Host",
            "Demonic Name", health: 1.2,
            description: """
            Fallen Hashmallim are corrupted angels of charged silence and hidden flame. Where their unfallen kin hold command in luminous restraint, these beings make silence feel like suppression, threat and the pause before cruelty.

            Their light may stutter, smoke or burn behind the skin of the world, accompanied by voices withheld until the worst possible moment. They feel like revelations twisted into blackmail or commands stripped of grace.

            They suit secret inquisitions, infernal councils, forbidden vaults and pacts sealed without witnesses. A Fallen Hashmal should make quiet feel unsafe to break.
            """));
        Add(Template("Fallen Seraphim", SupernaturalFamily.Demon, "Supernatural Horned Fiend", SizeCategory.Normal,
            SupernaturalPlanarProfile.AstralNative, SupernaturalNeedsProfile.NonLiving,
            Stats(5, 4, 1, 1, 4, 2, 4, "2d4+5"),
            "fallen celestial adversary and infernal court play", demonAttacks, [infernal, horns, tail, eyes], "Fallen Host",
            "Demonic Name", health: 1.2,
            description: """
            Fallen Seraphim are burning angels whose praise has become hunger, pride or purification without love. They remain beautiful in the way fire is beautiful: capable of making observers forget that beauty can consume.

            Their many wings and furnace-bright presence still carry the memory of sanctity, but the heat now judges by possession, rage or zeal. They do not merely destroy; they insist destruction is a form of truth.

            They belong in volcanic sanctuaries, blasphemous choirs and holy wars gone rotten. A Fallen Seraph should make flame feel articulate, ecstatic and merciless.
            """));
        Add(Template("Fallen Malakhim", SupernaturalFamily.Demon, "Supernatural Horned Fiend", SizeCategory.Normal,
            SupernaturalPlanarProfile.AstralNative, SupernaturalNeedsProfile.NonLiving,
            Stats(5, 4, 1, 1, 4, 2, 4, "2d4+5"),
            "fallen celestial adversary and infernal court play", demonAttacks, [infernal, horns, tail, eyes], "Fallen Host",
            "Demonic Name", health: 1.2,
            description: """
            Fallen Malakhim are messengers whose sacred office has become deception, false revelation or weaponised command. They understand the power of a delivered word and have learned to bend that power toward ruin.

            They need not look monstrous at first. A calm voice, a timely arrival and a message that almost sounds righteous can be more dangerous than claws, because their corruption works through trust and obedience.

            They suit false prophets, cursed dreams, poisoned diplomacy and infernal errands. A Fallen Malakh should make every instruction suspect, especially the persuasive ones.
            """));
        Add(Template("Fallen Elohim", SupernaturalFamily.Demon, "Supernatural Horned Fiend", SizeCategory.Normal,
            SupernaturalPlanarProfile.AstralNative, SupernaturalNeedsProfile.NonLiving,
            Stats(4, 3, 1, 1, 4, 2, 4, "2d4+5"),
            "fallen celestial adversary and infernal court play", demonAttacks, [infernal, horns, tail, eyes], "Fallen Host",
            "Demonic Name", health: 1.2,
            description: """
            Fallen Elohim are corrupted powers of authority and judgement, luminous magistrates turned tyrants, usurpers or divine pretenders. Their danger lies in command that still sounds lawful even when its centre has gone hollow.

            They carry sovereignty like a weapon. Radiance, voice and bearing suggest courts, edicts and punishments, but the justice they enact bends toward domination, pride or cosmic grievance.

            They belong in fallen principalities, cults of obedience and thrones built from stolen reverence. A Fallen Elohim should make rebellion feel morally necessary and terrifyingly costly.
            """));
        Add(Template("Fallen Bene Elohim", SupernaturalFamily.Demon, "Supernatural Horned Fiend", SizeCategory.Normal,
            SupernaturalPlanarProfile.AstralNative, SupernaturalNeedsProfile.NonLiving,
            Stats(4, 3, 1, 1, 4, 2, 4, "2d4+5"),
            "fallen celestial adversary and infernal court play", demonAttacks, [infernal, horns, tail, eyes], "Fallen Host",
            "Demonic Name", health: 1.2,
            description: """
            Fallen Bene Elohim are watchers and celestial scions who crossed the boundary between attention and appetite. Their fall often feels intimate: fascination with mortal life becoming possession, interference or forbidden allegiance.

            They may retain beauty, nobility and the manners of patrons, which makes them especially dangerous. Their corruption is not always crude; it can arrive as gifts, tutelage, desire or protection that slowly becomes a claim.

            They suit stories of oathbreakers, secret lineages, forbidden teachers and watchers who loved the world badly. A Fallen Bene Elohim should make divine interest feel compromised.
            """));
        Add(Template("Fallen Cherubim", SupernaturalFamily.Demon, "Supernatural Horned Fiend", SizeCategory.Normal,
            SupernaturalPlanarProfile.AstralNative, SupernaturalNeedsProfile.NonLiving,
            Stats(4, 3, 1, 1, 4, 2, 4, "2d4+5"),
            "fallen celestial adversary and infernal court play", demonAttacks, [infernal, horns, tail, eyes], "Fallen Host",
            "Demonic Name", health: 1.2,
            description: """
            Fallen Cherubim are guardians whose warding nature has become exclusion, imprisonment or desecration. They still understand gates, thresholds and holy boundaries, but now use that knowledge to deny, trap or profane.

            Their wings and composite forms may seem carved for defence, yet the space around them feels like a locked room rather than a sanctuary. They are the memory of protection turned into a weapon against those seeking refuge.

            They fit cursed temples, sealed prisons, corrupted gardens and gatehouses no one should have built. A Fallen Cherub should make passage feel like a bargain with the wrong keeper.
            """));
        Add(Template("Fallen Ishim", SupernaturalFamily.Demon, "Supernatural Horned Fiend", SizeCategory.Normal,
            SupernaturalPlanarProfile.AstralNative, SupernaturalNeedsProfile.NonLiving,
            Stats(3, 2, 1, 1, 4, 2, 4, "2d4+5"),
            "fallen celestial adversary and infernal court play", demonAttacks, [infernal, horns, tail, eyes], "Fallen Host",
            "Demonic Name", health: 1.2,
            description: """
            Fallen Ishim are near-mortal angels whose closeness to human life has turned into manipulation, envy or intimate cruelty. They know the language of comfort, advice and timely arrival, and that knowledge makes them dangerous.

            Their forms may pass almost gently through mortal society, marked only by a tarnished glow, too-knowing gaze or the sense that sympathy has become a hook. They corrupt through proximity rather than spectacle.

            They suit tempters, false counsellors, household curses and tragedies where help arrives wearing the wrong face. A Fallen Ishim should make the familiar feel spiritually unsafe.
            """));

		Add(Template("Incubus", SupernaturalFamily.Demon, "Supernatural Horned Fiend", SizeCategory.Normal,
			SupernaturalPlanarProfile.AstralNative, SupernaturalNeedsProfile.NonLiving, Stats(1, 1, 2, 2, 3, 2, 4, "2d4+4"),
			"social predator and temptation storylines", demonAttacks, [infernal, horns, tail], "Fallen Host", "Demonic Name",
			description: """
			Incubi are demonic social predators whose power works through attention, desire and the careful reading of weakness. They are dangerous not because they cannot be resisted, but because they make resistance feel like a private negotiation already half lost.

			Their demonic nature may appear through horns, infernal marks, unnatural beauty or a gaze too practised to be merely charming. The body is a lure, but the real weapon is intimacy sharpened into appetite.

			They belong in courts, dreams, salons, cults and private rooms where corruption can look like consent until the cost is clear. An incubus should feel personal, elegant and predatory.
			"""));
		Add(Template("Succubus", SupernaturalFamily.Demon, "Supernatural Horned Fiend", SizeCategory.Normal,
			SupernaturalPlanarProfile.AstralNative, SupernaturalNeedsProfile.NonLiving, Stats(1, 1, 2, 2, 3, 2, 4, "2d4+4"),
			"social predator and temptation storylines", demonAttacks, [infernal, horns, tail], "Fallen Host", "Demonic Name",
			description: """
			Succubi are demonic tempters and social hunters, using allure, empathy and performance as instruments of appetite. Their danger is rarely brute force first; it is the ability to make longing, vanity or loneliness answer when called.

			Infernal marks, poised movement and deliberate beauty should never reduce them to ornament. A succubus is a strategist of closeness, capable of turning conversation, promise and touch into territory.

			They suit intrigue, dream-haunting, cult recruitment and bargains that begin pleasantly enough to be remembered with shame. A succubus should make desire feel like a doorway with something waiting behind it.
			"""));
		Add(Template("Fury", SupernaturalFamily.Demon, "Supernatural Horned Fiend", SizeCategory.Normal,
			SupernaturalPlanarProfile.DualNatured, SupernaturalNeedsProfile.NonLiving, Stats(3, 2, 3, 1, 3, 3, 3, "2d4+3"),
			"vengeance spirit and relentless hunter play", highDemonAttacks, [infernal, wings, eyes], "Fallen Host", "Demonic Name",
			combat: "Beast Brawler",
			description: """
			Furies are demonic powers of vengeance, pursuit and punishment given a body sharp enough to chase guilt across borders. Their nature is not patient temptation but relentless consequence.

			Wings, burning eyes, talons or infernal scars can all serve the same impression: a being animated by outrage refined into purpose. A Fury does not merely hate; it prosecutes with claws, voice and motion.

			They belong in blood-debts, broken oaths, cursed lineages and hunts that do not end when the quarry changes roads. A Fury should make wrongdoing feel tracked.
			"""));
		Add(Template("Imp", SupernaturalFamily.Demon, "Supernatural Familiar", SizeCategory.Small,
			SupernaturalPlanarProfile.AstralNative, SupernaturalNeedsProfile.NonLiving, Stats(-2, -1, 3, 2, 1, 2, 2, "1d4+2"),
			"minor demon, spy and nuisance familiar play", demonAttacks, [infernal, horns, tail], "Fallen Host", "Demonic Name",
			weapons: false, combat: "Beast Skirmisher", health: 0.75,
			description: """
			Imps are minor demons of nuisance, spying, sabotage and malicious curiosity. Their small size makes them easy to dismiss until one remembers how much harm can be done by a creature that fits through vents, shutters and careless wards.

			They tend toward quick limbs, sharp faces, restless tails and a gleeful sense of stolen permission. An imp is rarely the centre of a grand evil, but it is often the hand that opens the latch.

			They suit familiars gone wrong, infernal errands, comic malice and genuinely dangerous infiltration. An imp should feel irritating, clever and harder to catch than pride allows.
			"""));
		Add(Template("Familiar", SupernaturalFamily.Demon, "Supernatural Familiar", SizeCategory.Small,
			SupernaturalPlanarProfile.AstralNative, SupernaturalNeedsProfile.NonLiving, Stats(-3, -1, 4, 2, 2, 3, 2, "1d4+2"),
			"bound companion, spy and occult assistant play", demonAttacks, [infernal, tail, eyes], "Fallen Host", "Demonic Name",
			weapons: false, combat: "Beast Skirmisher", health: 0.7,
			description: """
			Demonic familiars are bound companion-spirits and occult assistants whose usefulness is inseparable from suspicion. They may serve, advise, spy or fetch, but every obedience carries the question of who benefits most from the bond.

			Their forms are often small, animal-like or impish, marked by infernal eyes, unnatural stillness or a tail that seems to have its own opinions. They belong close to the hand, shoulder, hearth or ritual circle.

			They suit witches, warlocks, forbidden libraries and quiet betrayals. A familiar should feel useful enough to keep and dangerous enough that keeping it says something about its master.
			"""));
		Add(Template("Fiend", SupernaturalFamily.Demon, "Supernatural Horned Fiend", SizeCategory.Large,
			SupernaturalPlanarProfile.AstralNative, SupernaturalNeedsProfile.NonLiving, Stats(5, 4, 0, 0, 3, 2, 3, "2d4+4"),
			"brutal infernal monster and war-demon play", highDemonAttacks, [infernal, horns, tail], "Fallen Host", "Demonic Name",
			combat: "Beast Brawler", health: 1.3,
			description: """
			Fiends are large brutal demons, broad enough for war and coarse enough that subtlety seems optional rather than impossible. They are the infernal body made direct: horn, claw, fang, tail and appetite arranged for domination.

			Their nature is not merely bestial. A fiend carries malice with structure, as if cruelty has been trained into tactics and then wrapped in muscle. It can guard, command, punish or simply break what softer tempters have prepared.

			They suit battlefields, infernal courts, sacrificial chambers and any scene that needs demonic force without refinement. A fiend should make violence feel organised and eager.
			"""));
		Add(Template("Balrog", SupernaturalFamily.Demon, "Supernatural Horned Fiend", SizeCategory.Huge,
			SupernaturalPlanarProfile.DualNatured, SupernaturalNeedsProfile.NonLiving, Stats(8, 7, 0, 0, 5, 3, 6, "2d6+6"),
			"ancient fire-and-shadow war demon play", highDemonAttacks, [infernal, horns, tail, eyes], "Fallen Host", "Demonic Name",
			combat: "Beast Brawler", health: 1.7,
			description: """
			Balrogs are ancient demons of fire, shadow and war, vast enough to make ordinary fiends seem newly made. The horned frame feels like old catastrophe given shape, carrying smoke, furnace heat and the memory of battles that became legend.

			They are not common soldiers of darkness. A balrog suggests command, ruin and deep time: a being that has burned through kingdoms, slept beneath stone or waited in places where old rebellions never fully cooled.

			They belong in abyssal halls, volcanic sanctums, mythic war histories and final thresholds. A balrog should make retreat feel less cowardly than literate.
			""") with
		{
			RoleDescription = SeederDescriptionHelpers.JoinParagraphs(
				"Balrogs are intended for ancient fire-and-shadow war demon play.",
				"They provide a Middle-earth-ready stock demon without hard-coding any one cosmology, allegiance or named individual.",
				"Builders can reskin them as abyssal captains, fallen spirits, volcanic guardians or legendary dungeon threats."),
			DescriptionVariants = SeederDescriptionHelpers.BuildVariantList(
				(
					"a fire-wreathed balrog",
					SeederDescriptionHelpers.JoinParagraphs(
						"This towering demon burns with a furnace-like presence, its horned outline and barbed tail visible through a shroud of smoke and heat.",
						"The body is massive and warlike rather than merely bestial, with claws, fangs and a posture that makes retreat feel like wisdom.",
						"Every movement suggests old wrath, stone-breaking strength and flame held just barely in check.")
				),
				(
					"a shadow-crowned balrog",
					SeederDescriptionHelpers.JoinParagraphs(
						"This huge infernal figure carries darkness around itself like a mantle, broken by ember-bright eyes and the hard geometry of horns.",
						"Its limbs are powerful enough to make the ground feel small beneath it, while its mouth and claws promise violence at close range.",
						"The whole creature reads as an ancient battlefield memory given a body of smoke, fire and malice.")
				))
		});
		Add(Template("Hellhound", SupernaturalFamily.Demon, "Supernatural Hellhound", SizeCategory.Large,
			SupernaturalPlanarProfile.Material, SupernaturalNeedsProfile.NonLiving, Stats(4, 3, 2, 0, 2, 4, 2, "1d4+2"),
			"infernal hunting beast play", hellhoundAttacks,
			[infernal, eyes], "Fallen Host", "Demonic Name", weapons: false, humanoid: false, combat: "Beast Brawler", health: 1.2,
			description: """
			Hellhounds are infernal hunting beasts shaped like great hounds and disciplined by malice, smoke and fire. They are not simply demonic dogs; they are pursuit made supernatural, bred or summoned to find what fear tries to hide.

			Burning eyes, heavy jaws, ember breath and dark paws give them the presence of a pack animal from a worse country. Their obedience is frightening because it suggests a master, a scent and a purpose already chosen.

			They suit cursed hunts, gate guards, infernal kennels and nights when escape fails one footprint at a time. A hellhound should make running feel like participation in the ritual.
			"""));

		Add(Template("Demigod", SupernaturalFamily.Divine, "Supernatural Divine Humanoid", SizeCategory.Normal,
			SupernaturalPlanarProfile.DualNatured, SupernaturalNeedsProfile.NonLiving, Stats(3, 3, 1, 1, 3, 2, 5, "2d4+5"),
			"divine-blooded heroes, avatars and staff-guided legends", angelAttacks, [radiant, eyes], "Celestial Host", "Angelic Name",
			description: """
			Demigods are divine-blooded beings whose mortal shape is charged with more power than flesh can comfortably hold. Their supernatural nature is apparent through radiant inheritance, heroic danger and the tension between person and symbol, giving them a presence that mortal bodies cannot convincingly imitate.

			They have a coherent physical and spiritual identity of their own. Anatomy, motion, gaze, voice and silence all contribute to the impression that this is not a mortal body with an unusual costume, but a being governed by different conditions of existence.

			Their precise court, curse, theology, bloodline or summoning tradition may differ from place to place. What remains constant is the race's visible force: a complete supernatural presence that can stand on its own in play.
			"""));
		Add(Template("Lesser God", SupernaturalFamily.Divine, "Supernatural Many-Winged Angel", SizeCategory.Large,
			SupernaturalPlanarProfile.DualNatured, SupernaturalNeedsProfile.NonLiving, Stats(6, 6, 2, 2, 6, 4, 8, "2d6+8"),
			"local deities, patrons and divine NPCs", highAngelAttacks, [radiant, wings, eyes], "Celestial Host", "Angelic Name",
			health: 1.5,
			description: """
			Lesser gods are local divine beings whose presence gathers worship, place and miracle into a person-like form. Their supernatural nature is apparent through patronage, awe and the authority of a shrine given a body, giving them a presence that mortal bodies cannot convincingly imitate.

			They have a coherent physical and spiritual identity of their own. Anatomy, motion, gaze, voice and silence all contribute to the impression that this is not a mortal body with an unusual costume, but a being governed by different conditions of existence.

			Their precise court, curse, theology, bloodline or summoning tradition may differ from place to place. What remains constant is the race's visible force: a complete supernatural presence that can stand on its own in play.
			"""));
		Add(Template("Greater God", SupernaturalFamily.Divine, "Supernatural Many-Winged Angel", SizeCategory.VeryLarge,
			SupernaturalPlanarProfile.DualNatured, SupernaturalNeedsProfile.NonLiving, Stats(8, 8, 2, 2, 8, 5, 10, "2d6+10"),
			"major deities and administrator-facing divine examples", highAngelAttacks, [radiant, wings, eyes], "Celestial Host",
			"Angelic Name", health: 1.8,
			description: """
			Greater gods are major divine beings whose forms are less bodies than overwhelming statements of power. Their supernatural nature is apparent through majesty, impossible scale and the sense of a law temporarily taking shape, giving them a presence that mortal bodies cannot convincingly imitate.

			They have a coherent physical and spiritual identity of their own. Anatomy, motion, gaze, voice and silence all contribute to the impression that this is not a mortal body with an unusual costume, but a being governed by different conditions of existence.

			Their precise court, curse, theology, bloodline or summoning tradition may differ from place to place. What remains constant is the race's visible force: a complete supernatural presence that can stand on its own in play.
			"""));

		Add(Template("Spirit", SupernaturalFamily.Spirit, "Supernatural Spirit Form", SizeCategory.Normal,
			SupernaturalPlanarProfile.Incorporeal, SupernaturalNeedsProfile.NonLiving, Stats(-1, 0, 2, 1, 3, 3, 3, "2d4+3"),
			"generic incorporeal spirits", spiritAttacks, [spirit, eyes], "Spirit Court", "Spirit Name", combat: "Beast Skirmisher",
			description: """
			Spirits are incorporeal beings shaped by memory, place, force or idea. Their supernatural nature is apparent through thin outlines, strange light and movement that does not fully obey weight, giving them a presence that mortal bodies cannot convincingly imitate.

			They have a coherent physical and spiritual identity of their own. Anatomy, motion, gaze, voice and silence all contribute to the impression that this is not a mortal body with an unusual costume, but a being governed by different conditions of existence.

			Their precise court, curse, theology, bloodline or summoning tradition may differ from place to place. What remains constant is the race's visible force: a complete supernatural presence that can stand on its own in play.
			"""));
		Add(Template("Ghost", SupernaturalFamily.Spirit, "Supernatural Spirit Form", SizeCategory.Normal,
			SupernaturalPlanarProfile.Incorporeal, SupernaturalNeedsProfile.NonLiving, Stats(-2, 0, 2, 1, 3, 3, 3, "2d4+3"),
			"dead souls and haunting NPCs", spiritAttacks, [spirit, death], "Spirit Court", "Spirit Name", combat: "Beast Skirmisher",
			description: """
			Ghosts are spirits of the dead whose forms remember the lives they have lost. Their supernatural nature is apparent through grave-pale light, unfinished emotion and the quiet pressure of haunting, giving them a presence that mortal bodies cannot convincingly imitate.

			They have a coherent physical and spiritual identity of their own. Anatomy, motion, gaze, voice and silence all contribute to the impression that this is not a mortal body with an unusual costume, but a being governed by different conditions of existence.

			Their precise court, curse, theology, bloodline or summoning tradition may differ from place to place. What remains constant is the race's visible force: a complete supernatural presence that can stand on its own in play.
			"""));
		Add(Template("Specter", SupernaturalFamily.Spirit, "Supernatural Spirit Form", SizeCategory.Normal,
			SupernaturalPlanarProfile.Incorporeal, SupernaturalNeedsProfile.NonLiving, Stats(-1, 1, 3, 2, 4, 4, 3, "2d4+3"),
			"predatory hauntings and feared incorporeal monsters", spiritAttacks, [spirit, death], "Spirit Court", "Spirit Name",
			combat: "Beast Skirmisher",
			description: """
			Specters are predatory hauntings that have shed much of the person they once were. Their supernatural nature is apparent through thin malice, cold motion and hunger without a living body, giving them a presence that mortal bodies cannot convincingly imitate.

			They have a coherent physical and spiritual identity of their own. Anatomy, motion, gaze, voice and silence all contribute to the impression that this is not a mortal body with an unusual costume, but a being governed by different conditions of existence.

			Their precise court, curse, theology, bloodline or summoning tradition may differ from place to place. What remains constant is the race's visible force: a complete supernatural presence that can stand on its own in play.
			"""));
		Add(Template("Wraith", SupernaturalFamily.Spirit, "Supernatural Spirit Form", SizeCategory.Normal,
			SupernaturalPlanarProfile.Incorporeal, SupernaturalNeedsProfile.NonLiving, Stats(0, 1, 3, 2, 4, 4, 4, "2d4+4"),
			"dangerous deathly spirits", spiritAttacks, [spirit, death], "Spirit Court", "Spirit Name", combat: "Beast Skirmisher",
			description: """
			Wraiths are dangerous deathly spirits whose presence drains warmth and courage. Their supernatural nature is apparent through shadow, grave-light and the implacable hunger of the unquiet dead, giving them a presence that mortal bodies cannot convincingly imitate.

			They have a coherent physical and spiritual identity of their own. Anatomy, motion, gaze, voice and silence all contribute to the impression that this is not a mortal body with an unusual costume, but a being governed by different conditions of existence.

			Their precise court, curse, theology, bloodline or summoning tradition may differ from place to place. What remains constant is the race's visible force: a complete supernatural presence that can stand on its own in play.
			"""));
		Add(Template("Ancestral Spirit", SupernaturalFamily.Spirit, "Supernatural Spirit Form", SizeCategory.Normal,
			SupernaturalPlanarProfile.Incorporeal, SupernaturalNeedsProfile.NonLiving, Stats(-1, 0, 1, 1, 4, 3, 4, "2d4+4"),
			"ancestor worship, family guardians and cultural spirit play", spiritAttacks, [spirit, eyes], "Spirit Court", "Spirit Name",
			description: """
			Ancestral spirits are incorporeal ancestors whose identity is tied to family, lineage and remembered obligation. Their supernatural nature is apparent through old names, inherited regard and the watchfulness of the dead over the living, giving them a presence that mortal bodies cannot convincingly imitate.

			They have a coherent physical and spiritual identity of their own. Anatomy, motion, gaze, voice and silence all contribute to the impression that this is not a mortal body with an unusual costume, but a being governed by different conditions of existence.

			Their precise court, curse, theology, bloodline or summoning tradition may differ from place to place. What remains constant is the race's visible force: a complete supernatural presence that can stand on its own in play.
			"""));
		Add(Template("Nature Spirit", SupernaturalFamily.Spirit, "Supernatural Spirit Form", SizeCategory.Normal,
			SupernaturalPlanarProfile.Incorporeal, SupernaturalNeedsProfile.NonLiving, Stats(0, 1, 2, 1, 3, 4, 4, "2d4+4"),
			"wilderness, grove and land-spirit play", spiritAttacks, [spirit, eyes], "Spirit Court", "Spirit Name",
			description: """
			Nature spirits are spirits of grove, stream, hill, storm, beast or season. Their supernatural nature is apparent through wild signs, changing light and a body that echoes the place it belongs to, giving them a presence that mortal bodies cannot convincingly imitate.

			They have a coherent physical and spiritual identity of their own. Anatomy, motion, gaze, voice and silence all contribute to the impression that this is not a mortal body with an unusual costume, but a being governed by different conditions of existence.

			Their precise court, curse, theology, bloodline or summoning tradition may differ from place to place. What remains constant is the race's visible force: a complete supernatural presence that can stand on its own in play.
			"""));
		Add(Template("Elemental Spirit", SupernaturalFamily.Spirit, "Supernatural Spirit Form", SizeCategory.Normal,
			SupernaturalPlanarProfile.Incorporeal, SupernaturalNeedsProfile.NonLiving, Stats(1, 1, 2, 1, 3, 3, 4, "2d4+4"),
			"fire, storm, water, earth and other elemental courts", spiritAttacks, [spirit, eyes], "Spirit Court", "Spirit Name",
			description: """
			Elemental spirits are spirits shaped by fire, storm, water, earth or other primal forces. Their supernatural nature is apparent through elemental motion, unstable outline and a body more force than flesh, giving them a presence that mortal bodies cannot convincingly imitate.

			They have a coherent physical and spiritual identity of their own. Anatomy, motion, gaze, voice and silence all contribute to the impression that this is not a mortal body with an unusual costume, but a being governed by different conditions of existence.

			Their precise court, curse, theology, bloodline or summoning tradition may differ from place to place. What remains constant is the race's visible force: a complete supernatural presence that can stand on its own in play.
			"""));

		Add(Template("Werewolf", SupernaturalFamily.Therianthrope, "Organic Humanoid", SizeCategory.Normal,
			SupernaturalPlanarProfile.Material, SupernaturalNeedsProfile.Living, Stats(1, 1, 1, 0, 1, 2, 0),
			"cursed or hereditary shapeshifter play", werewolfAttacks, [beast],
			"Therianthrope", "Mortal Name",
			description: """
			Werewolfs are living shapeshifters marked by the wolf beneath the human skin. Their supernatural nature is apparent through yellow eyes, coarse hackles, sharpened instincts and the pressure of a curse or bloodline, giving them a presence that mortal bodies cannot convincingly imitate.

			They have a coherent physical and spiritual identity of their own. Anatomy, motion, gaze, voice and silence all contribute to the impression that this is not a mortal body with an unusual costume, but a being governed by different conditions of existence.

			Their precise court, curse, theology, bloodline or summoning tradition may differ from place to place. What remains constant is the race's visible force: a complete supernatural presence that can stand on its own in play.
			"""));
		Add(Template("Werewolf Hybrid", SupernaturalFamily.Therianthrope, "Supernatural Werewolf Hybrid", SizeCategory.Large,
			SupernaturalPlanarProfile.Material, SupernaturalNeedsProfile.Living, Stats(4, 3, 2, 0, 2, 3, 0),
			"wolf-man battle forms and cursed transformation play", werewolfHybridAttacks,
			[beast], "Therianthrope", "Mortal Name", weapons: false, combat: "Beast Brawler", health: 1.25,
			description: """
			Werewolf hybrids are wolf-man battle forms in which human structure and lupine violence meet openly. Their supernatural nature is apparent through muzzled features, claws, hackles and a forward-leaning hunger for the fight, giving them a presence that mortal bodies cannot convincingly imitate.

			They have a coherent physical and spiritual identity of their own. Anatomy, motion, gaze, voice and silence all contribute to the impression that this is not a mortal body with an unusual costume, but a being governed by different conditions of existence.

			Their precise court, curse, theology, bloodline or summoning tradition may differ from place to place. What remains constant is the race's visible force: a complete supernatural presence that can stand on its own in play.
			"""));

		Add(Template("Vampire", SupernaturalFamily.Undead, "Organic Humanoid", SizeCategory.Normal,
			SupernaturalPlanarProfile.Material, SupernaturalNeedsProfile.NonLiving, Stats(2, 2, 2, 1, 3, 3, 3, "2d4+3"),
			"deathless social predators without hard-coded feeding mechanics", undeadAttacks, [death, eyes], "Undead Remnant", "Undead Name",
			description: """
			Vampires are deathless social predators who preserve personhood while losing ordinary life. Their supernatural nature is apparent through pale stillness, sharpened hunger and the elegance of a corpse that learned to smile, giving them a presence that mortal bodies cannot convincingly imitate.

			They have a coherent physical and spiritual identity of their own. Anatomy, motion, gaze, voice and silence all contribute to the impression that this is not a mortal body with an unusual costume, but a being governed by different conditions of existence.

			Their precise court, curse, theology, bloodline or summoning tradition may differ from place to place. What remains constant is the race's visible force: a complete supernatural presence that can stand on its own in play.
			"""));
		Add(Template("Lich", SupernaturalFamily.Undead, "Supernatural Lich Skeleton", SizeCategory.Normal,
			SupernaturalPlanarProfile.DualNatured, SupernaturalNeedsProfile.NonLiving, Stats(-1, 1, 0, 1, 6, 4, 6, "2d6+6"),
			"undead sorcerers, ancient powers and staff-controlled antagonists", undeadAttacks, [death, corpse, eyes], "Undead Remnant", "Undead Name",
			description: """
			Liches are undead sorcerers whose bodies are vessels for will, memory and forbidden endurance. Their supernatural nature is apparent through bone, parchment flesh, occult focus and intellect surviving rot, giving them a presence that mortal bodies cannot convincingly imitate.

			They have a coherent physical and spiritual identity of their own. Anatomy, motion, gaze, voice and silence all contribute to the impression that this is not a mortal body with an unusual costume, but a being governed by different conditions of existence.

			Their precise court, curse, theology, bloodline or summoning tradition may differ from place to place. What remains constant is the race's visible force: a complete supernatural presence that can stand on its own in play.
			"""));
		Add(Template("Ghoul", SupernaturalFamily.Undead, "Supernatural Decayed Undead", SizeCategory.Normal,
			SupernaturalPlanarProfile.Material, SupernaturalNeedsProfile.NonLiving, Stats(2, 2, 1, 0, 1, 2, 0),
			"grave-hungry undead servants without bespoke feeding mechanics", undeadAttacks, [death, corpse], "Undead Remnant", "Undead Name",
			weapons: false, combat: "Beast Brawler",
			description: """
			Ghouls are grave-hungry undead whose bodies are animated by appetite and decay. Their supernatural nature is apparent through ashen flesh, clawing hands and the practical ugliness of carrion hunger, giving them a presence that mortal bodies cannot convincingly imitate.

			They have a coherent physical and spiritual identity of their own. Anatomy, motion, gaze, voice and silence all contribute to the impression that this is not a mortal body with an unusual costume, but a being governed by different conditions of existence.

			Their precise court, curse, theology, bloodline or summoning tradition may differ from place to place. What remains constant is the race's visible force: a complete supernatural presence that can stand on its own in play.
			"""));
		Add(Template("Zombie", SupernaturalFamily.Undead, "Supernatural Decayed Undead", SizeCategory.Normal,
			SupernaturalPlanarProfile.Material, SupernaturalNeedsProfile.NonLiving, Stats(2, 4, -2, -2, -2, -1, 0),
			"durable animated corpses and simple undead threats", undeadAttacks, [death, corpse], "Undead Remnant", "Undead Name",
			weapons: false, combat: "Beast Brawler", health: 1.35,
			description: """
			Zombies are animated corpses driven by stubborn motion rather than living thought. Their supernatural nature is apparent through dead weight, ruined flesh and a body that keeps moving past every natural limit, giving them a presence that mortal bodies cannot convincingly imitate.

			They have a coherent physical and spiritual identity of their own. Anatomy, motion, gaze, voice and silence all contribute to the impression that this is not a mortal body with an unusual costume, but a being governed by different conditions of existence.

			Their precise court, curse, theology, bloodline or summoning tradition may differ from place to place. What remains constant is the race's visible force: a complete supernatural presence that can stand on its own in play.
			"""));
		Add(Template("Skeleton", SupernaturalFamily.Undead, "Supernatural Skeleton", SizeCategory.Normal,
			SupernaturalPlanarProfile.Material, SupernaturalNeedsProfile.NonLiving, Stats(0, 1, 1, 0, 0, 1, 0),
			"animated bones and low-maintenance undead guards", undeadAttacks, [death, corpse], "Undead Remnant", "Undead Name",
			weapons: true, health: 0.9,
			description: """
			Skeletons are animated bones held together by magic, curse or command. Their supernatural nature is apparent through bare joints, rattling movement and the unnerving economy of a body stripped to structure, giving them a presence that mortal bodies cannot convincingly imitate.

			They have a coherent physical and spiritual identity of their own. Anatomy, motion, gaze, voice and silence all contribute to the impression that this is not a mortal body with an unusual costume, but a being governed by different conditions of existence.

			Their precise court, curse, theology, bloodline or summoning tradition may differ from place to place. What remains constant is the race's visible force: a complete supernatural presence that can stand on its own in play.
			"""));
		Add(Template("Mummy", SupernaturalFamily.Undead, "Supernatural Decayed Undead", SizeCategory.Normal,
			SupernaturalPlanarProfile.Material, SupernaturalNeedsProfile.NonLiving, Stats(3, 4, -1, -1, 3, 1, 2, "1d4+2"),
			"preserved cursed dead and ancient tomb guardians", undeadAttacks, [death, corpse], "Undead Remnant", "Undead Name",
			weapons: true, combat: "Beast Brawler", health: 1.4,
			description: """
			Mummies are preserved cursed dead whose bodies carry tomb, ritual and old punishment with them. Their supernatural nature is apparent through wrappings, dry flesh, resin, dust and a patience that belongs to sealed rooms, giving them a presence that mortal bodies cannot convincingly imitate.

			They have a coherent physical and spiritual identity of their own. Anatomy, motion, gaze, voice and silence all contribute to the impression that this is not a mortal body with an unusual costume, but a being governed by different conditions of existence.

			Their precise court, curse, theology, bloodline or summoning tradition may differ from place to place. What remains constant is the race's visible force: a complete supernatural presence that can stand on its own in play.
			"""));

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
