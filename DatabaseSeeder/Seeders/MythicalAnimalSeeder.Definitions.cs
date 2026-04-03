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
		string RoleDescription,
		IReadOnlyList<StockDescriptionVariant>? DescriptionVariants,
		IReadOnlyList<MythicalAttackTemplate> Attacks,
		IReadOnlyList<MythicalBodypartUsageTemplate>? BodypartUsages = null,
		IReadOnlyList<string>? PersonWords = null,
		string? FacialHairProfileName = null,
		IReadOnlyList<MythicalCharacteristicTemplate>? AdditionalCharacteristics = null,
		IReadOnlyList<StockDescriptionVariant>? OverlayDescriptionVariants = null,
		string CombatStrategyKey = "Beast Brawler",
		IReadOnlyList<SeederTattooTemplateDefinition>? TattooTemplates = null,
		IReadOnlyList<SeederScarTemplateDefinition>? ScarTemplates = null
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
		static IReadOnlyList<StockDescriptionVariant> Variants(
			params (string ShortDescription, string FullDescription)[] variants)
			=> SeederDescriptionHelpers.BuildVariantList(variants);
		static MythicalRaceTemplate BeastRace(
			string name,
			string bodyKey,
			SizeCategory size,
			string model,
			MythicalAgeProfile ageProfile,
			string description,
			string roleDescription,
			IReadOnlyList<StockDescriptionVariant> descriptionVariants,
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
				roleDescription,
				descriptionVariants,
				attacks,
				usages,
				null,
				null,
				additionalCharacteristics,
				CombatStrategyKey: combatStrategyKey
			);
		static MythicalRaceTemplate HumanoidRace(
			string name,
			string bodyKey,
			SizeCategory size,
			string maleModel,
			string femaleModel,
			MythicalAgeProfile ageProfile,
			string description,
			string roleDescription,
			IReadOnlyList<MythicalAttackTemplate> attacks,
			IReadOnlyList<string> personWords,
			IReadOnlyList<StockDescriptionVariant>? overlayDescriptionVariants = null,
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
				roleDescription,
				null,
				attacks,
				usages,
				personWords,
				facialHairProfile,
				null,
				overlayDescriptionVariants,
				CombatStrategyKey: combatStrategyKey
			);
		static MythicalRaceTemplate SapientRace(
			string name,
			string bodyKey,
			SizeCategory size,
			string maleModel,
			string femaleModel,
			MythicalAgeProfile ageProfile,
			string description,
			string roleDescription,
			IReadOnlyList<StockDescriptionVariant> descriptionVariants,
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
				roleDescription,
				descriptionVariants,
				attacks,
				usages,
				null,
				null,
				additionalCharacteristics,
				CombatStrategyKey: combatStrategyKey
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
				"In most worlds dragons stand apart from ordinary beasts, looming over landscapes as apex predators, hoard-lords, or living catastrophes that lesser peoples must placate, evade, or confront with great preparation.",
				Variants(
					("a horn-crowned dragon",
						"This colossal draconic beast combines a heavily muscled quadrupedal frame with broad wings, curving horns and a predator's jaws."),
					("a vast scale-armoured dragon",
						"This immense reptile wears overlapping scales across a barrel chest and a sinuous tail, its whole frame built for terror, endurance and sudden violence.")
				),
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
				"Griffins often fill the role of proud sky-hunters and sacred or royal beasts, creatures that dominate high crags and open skies while also appearing in heraldry, legend and elite riding traditions.",
				Variants(
					("a hooked-beaked griffin",
						"This imposing mythic predator bears a hooked avian beak and far-seeing eyes above a leonine quadruped body built for bounding flight and savage pounces."),
					("a broad-winged griffin",
						"This hybrid beast balances raptorial foreparts and powerful leonine hindquarters, giving it the look of something bred equally for altitude and brutal close attack.")
				),
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
				"Hippogriffs often occupy the boundary between wild mount and dangerous aerial grazer, prized by riders who can tame them and feared by travellers who mistake their grace for docility.",
				Variants(
					("a keen-eyed hippogriff",
						"This powerful hybrid has an avian head and beating wings set atop an equine frame, with hoofed legs built equally for galloping starts and airborne strikes."),
					("a feather-crested hippogriff",
						"This large chimera marries a raptor's forequarters to an equine body, giving it both a war-mount's breadth and a predator's abrupt, striking violence.")
				),
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
				"Unicorns are usually treated less as livestock than as numinous presences, creatures tied to sanctuaries, omens, purity traditions, and stories in which the worthy may approach what the crude can never catch.",
				Variants(
					("a spiralled-horn unicorn",
						"This elegant equine myth-beast carries itself with the poise of a fine horse, its singular horn and bright, intelligent eyes marking it as something far stranger."),
					("a moon-bright unicorn",
						"This horned beast looks horse-like at a glance, yet its unnerving composure and faultless carriage make it seem more like an embodied blessing than an animal.")
				),
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
				"Pegasi are commonly imagined as noble aerial mounts and heraldic wonders, but in the wild they remain strong-willed creatures whose flight ranges and herd instincts make them difficult to manage.",
				Variants(
					("a feather-winged pegasus",
						"This broad-winged equine is all coiled athletic power, its feathered wings and strong hooves making it as dangerous in the air as on the ground."),
					("a sky-bred pegasus",
						"This winged horse combines a rider's familiar silhouette with the deep chest, clean limbs and flight muscles of a creature meant to launch hard and stay aloft.")
				),
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
				"Minotaurs are commonly cast as warriors, guardians and dwellers of enclosed strongholds, their size and imposing presence making them natural figures of labour, soldiery and intimidation in mythic societies.",
				[
					Attack("Horn Gore", ItemQuality.Standard, "rhorn", "lhorn"),
					Attack("Head Ram", ItemQuality.Standard, "head"),
					Attack("Elbow", ItemQuality.Standard, "relbow", "lelbow")
				],
				["minotaur"],
				Variants(
					("a horned minotaur", "This horned minotaur has the towering frame and bull-cast features typical of the race, with a heavy brow, broad muzzle and deep chest."),
					("a broad-shouldered minotaur", "This minotaur's build is massively robust, its horned head and thick musculature giving it the intimidating silhouette of a born charger.")
				),
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
				"Eastern dragons often appear as imperial, spiritual or elemental beings rather than mere monsters, entwined with rivers, weather, dynasties and the idea of ancient, watchful power.",
				Variants(
					("a long-bodied eastern dragon",
						"This immense draconic predator combines a serpentine cast and a powerful quadrupedal frame, with taloned feet, a long body and a sweeping tail in place of wings."),
					("a whiskered eastern dragon",
						"This drake's elongated body and flowing profile give it a more sinuous majesty than a western dragon, though its claws, jaws and sheer size remain openly dangerous.")
				),
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
				"Naga usually occupy the role of riverine nobles, temple guardians or old marsh powers, creatures whose poise and patience make them feel as political and ceremonial as they are dangerous.",
				[
					Attack("Carnivore Bite", ItemQuality.Standard, "mouth"),
					Attack("Bite", ItemQuality.Standard, "mouth"),
					Attack("Tail Slap", ItemQuality.Standard, "tail")
				],
				["naga"],
				overlayDescriptionVariants: Variants(
					("a coiled naga", "This naga presents a recognisably humanoid upper torso above a long serpentine lower body, the whole figure poised in smooth, deliberate coils."),
					("a serpent-bodied naga", "This naga's human-like arms and shoulders rise from a scaled, sinuous body whose coiling strength and low centre of gravity suggest sudden violence.")
				),
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
				"Merfolk usually fill the mythic niche of sea-dwellers, reef guardians and shoreline enigmas, engaging with landfolk through trade, song, rescue, raiding or careful distance depending on the setting.",
				[
					Attack("Carnivore Bite", ItemQuality.Bad, "mouth"),
					Attack("Bite", ItemQuality.Bad, "mouth"),
					Attack("Tail Slap", ItemQuality.Good, "caudalfin")
				],
				["merfolk"],
				overlayDescriptionVariants: Variants(
					("a fin-tailed merfolk", "This merfolk body combines a humanoid upper torso with a powerful scaled tail, built more for darting turns and long swims than for any life on land."),
					("a sea-borne merfolk", "This merfolk's shoulders and arms are recognisably person-like, but the gleam of scales and the muscular sweep of the tail place them firmly in the water's domain.")
				),
				combatStrategyKey: "Melee (Auto)"
			),
			["Manticore"] = BeastRace(
				"Manticore",
				"Manticore",
				SizeCategory.Large,
				"Big Felid",
				GreatBeast(),
				"Manticores are broad-winged leonine predators with a venomous tail-spike.",
				"Manticores occupy the role of nightmare apex predators, the sort of thing that turns caravan routes, borderlands and lonely heights into places where every rumour deserves to be believed.",
				Variants(
					("a stinger-tailed manticore",
						"This winged predator couples a leonine quadruped body and raking claws with a barbed stinger poised at the end of its tail."),
					("a broad-winged manticore",
						"This leonine chimera looks built to overwhelm prey by stages, first with speed and claws, then with its tail-spike and crushing mass.")
				),
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
				"Wyverns are often treated as brutal cousins to true dragons, less regal but no less feared, and they commonly dominate cliffs, ruins and badlands where flight and aggression decide territory.",
				Variants(
					("a taloned wyvern",
						"This draconic flyer stands on powerful taloned legs beneath a scaled torso, its jaws and whipping tail making close quarters especially dangerous."),
					("a leathery-winged wyvern",
						"This wyvern's body is leaner and more openly predatory than a true dragon's, giving it the look of something evolved purely to dive, seize and tear.")
				),
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
				"Phoenixes fill the symbolic role of sacred fire, omen and renewal, but even stripped of miraculous mechanics they remain rare, intimidating creatures whose beauty does not make them safe.",
				Variants(
					("a radiant phoenix",
						"This majestic firebird has an avian frame and proud bearing, its whole presence suggesting heat, renewal and dangerous beauty."),
					("an ember-bright phoenix",
						"This great bird carries itself like a raptor haloed by furnace light, each movement suggesting heat, splendour and peril in equal measure.")
				),
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
				"Basilisks are classic terror-beasts of ruins, tombs and desolate places, remembered less as animals to study than as lurking calamities whose presence can poison whole stretches of land or memory.",
				Variants(
					("a heavy-coiled basilisk",
						"This huge serpent drapes itself in heavy coils and watches with an unsettling, predatory stillness that makes its sudden strikes all the worse."),
					("a malign-eyed basilisk",
						"This monstrous serpent looks thicker, older and more deliberate than any natural snake, as though sheer age and venom had made it into something mythic.")
				),
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
				"Cockatrices fill the ecological niche of invasive, spiteful scavenger-predators in many stories, and their small size only makes them more troublesome around farms, granaries and rocky ruins.",
				Variants(
					("a reptilian cockatrice",
						"This wiry, ill-tempered creature has an avian body and a reptilian cast to its features, all sharp beak, clawed feet and restless hostility."),
					("a spiteful cockatrice",
						"This little monster looks like a bad dream of bird and lizard combined, with thin legs, a stabbing beak and a temperament that promises trouble.")
				),
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
				"Hippocamps often serve as steeds, sacred sea-herd animals or elusive prizes for coastal peoples, their strange shape making them valuable to myth, pageantry and any culture that dreams of riding the surf.",
				Variants(
					("a sea-tailed hippocamp",
						"This aquatic myth-beast bears a horse-like forebody and forelegs, but from the loins back it flows into a muscular fish-tail built for swift, powerful swimming."),
					("a surf-bred hippocamp",
						"This creature looks as though a warhorse had been reworked for open water, its strong chest and lifted neck carried before a tail meant for thrust rather than gallop.")
				),
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
				"Selkies usually inhabit the role of liminal coastal people, bound to coves, islands and cold waters, where they are known through trade, kinship, secrecy and stories of departure and return.",
				[
					Attack("Carnivore Bite", ItemQuality.Bad, "mouth"),
					Attack("Bite", ItemQuality.Bad, "mouth")
				],
				["selkie"],
				overlayDescriptionVariants: Variants(
					("a seal-blooded selkie", "This selkie has a recognisably humanoid frame softened by an aquatic grace, the race's seal-blooded heritage evident in the smooth lines and sea-going poise."),
					("a sea-graceful selkie", "This selkie carries themself with the easy balance of someone more at home on wave-washed rock and in cold surf than on dry inland roads.")
				),
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
				"Myconids usually occupy the mythic niche of hidden underworld communities, patient spore-keepers and eerie but not always hostile neighbours whose alien biology shapes every custom they keep.",
				Variants(
					("a cap-headed myconid",
						"This stooped fungus-being has a humanoid shape but a cap-like head and an organic, spongy texture that marks it as something far removed from ordinary flesh."),
					("a spore-soft myconid",
						"This fungal person looks less built from muscle and bone than from flexible growth, with a stillness that feels communal and subterranean rather than animal.")
				),
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
				"Plantfolk often stand in myth as wardens, gardeners, slow-speaking elders or embodiments of a place itself, with social rhythms shaped by season, light, soil and patient memory.",
				Variants(
					("a bark-skinned plantfolk",
						"This plant-being stands in a recognisably humanoid form, but bark-like surfaces, fibrous growths and living greenery make every motion seem rooted in the natural world."),
					("a leaf-grown plantfolk",
						"This vegetative person looks less like flesh made green than like a walking tangle of living wood and pliant stems coaxed into a humanoid shape.")
				),
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
				"Owlkin commonly fill the role of nocturnal scholars, hunters and sentries, their cultural identity often bound to keen perception, silence, altitude and a strong sense of territory.",
				[
					Attack("Jab", ItemQuality.Standard, "rhand", "lhand"),
					Attack("Elbow", ItemQuality.Standard, "relbow", "lelbow")
				],
				["owlkin"],
				Variants(
					("a feathered owlkin", "This owlkin carries a humanoid frame beneath plumage and wings, with an intense gaze and avian lines that immediately set them apart from ordinary people."),
					("a keen-eyed owlkin", "This owlkin's feathered features and broad wings give them the look of a night hunter shaped into a mostly humanoid form.")
				),
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
				"Avian peoples usually read as aerial citizens rather than monsters, with cultures oriented toward roosting space, wind, lookout duties and the practical consequences of living with wings and height.",
				[
					Attack("Jab", ItemQuality.Standard, "rhand", "lhand"),
					Attack("Elbow", ItemQuality.Standard, "relbow", "lelbow")
				],
				["birdfolk", "avian"],
				Variants(
					("a broad-winged avian", "This avian person has a mostly humanoid build, but feathering, wings and a birdlike cast to the face shift the impression immediately skyward."),
					("a feather-cloaked birdfolk", "This birdfolk figure remains recognisably person-shaped, yet every line of the wings and plumage suggests roosts, thermals and open air.")
				),
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
				"Centaurs are frequently cast as nomads, outriders and peoples of open country, their societies shaped by speed, horizon, herd memory and a bodily scale that changes how they build, travel and fight.",
				[
					Attack("Hoof Stomp", ItemQuality.Good, "rfhoof", "lfhoof", "rrhoof", "lrhoof"),
					Attack("Head Ram", ItemQuality.Standard, "head"),
					Attack("Elbow", ItemQuality.Standard, "relbow", "lelbow")
				],
				["centaur"],
				overlayDescriptionVariants: Variants(
					("a deep-chested centaur", "This centaur combines a humanoid torso and arms with a powerful equine lower body, making the whole figure look fast, stable and difficult to dislodge."),
					("a long-striding centaur", "This centaur's human upper body rises from a broad horse-frame whose musculature and stance suggest endurance, mobility and hard impact.")
				),
				combatStrategyKey: "Melee (Auto)"
			),
			["Pegacorn"] = BeastRace(
				"Pegacorn",
				"Ungulate",
				SizeCategory.Large,
				"Horse",
				GreatBeast(),
				"Pegacorns combine the broad wings of a pegasus with the spiralled horn of a unicorn.",
				"Pegacorns occupy the rarest and most ceremonial niche of the winged equines, appearing in myths as omens, impossible mounts and embodiments of wonder that blend swiftness with sanctity.",
				Variants(
					("a horned winged pegacorn",
						"This mythic equine bears both sweeping feathered wings and a singular horn, giving it the grace of a unicorn and the power of a pegasus."),
					("a sky-bright pegacorn",
						"This creature looks impossibly ornate even by mythic standards, as though an already noble flying horse had been elevated into something sacred and untouchable.")
				),
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

	internal static string BuildRaceDescriptionForTesting(MythicalRaceTemplate template)
	{
		var supportingVariant = (template.DescriptionVariants ?? template.OverlayDescriptionVariants)?.FirstOrDefault();
		return SeederDescriptionHelpers.JoinParagraphs(
			SeederDescriptionHelpers.EnsureTrailingPeriod(template.Description),
			supportingVariant?.FullDescription ?? $"This race is represented by the {template.Name} stock catalogue.",
			SeederDescriptionHelpers.EnsureTrailingPeriod(template.RoleDescription)
		);
	}

	internal static string BuildEthnicityDescriptionForTesting(MythicalRaceTemplate template)
	{
		return SeederDescriptionHelpers.JoinParagraphs(
			SeederDescriptionHelpers.EnsureTrailingPeriod(
				$"The stock {template.Name.ToLowerInvariant()} ethnicity represents the default lineage, upbringing and outward presentation seeded for this mythic race."),
			(template.DescriptionVariants ?? template.OverlayDescriptionVariants)?.Skip(1).FirstOrDefault()?.FullDescription ??
			(template.DescriptionVariants ?? template.OverlayDescriptionVariants)?.FirstOrDefault()?.FullDescription ??
			$"This stock heritage keeps the visual hallmarks and bodily proportions associated with {template.Name.ToLowerInvariant()} characters.",
			SeederDescriptionHelpers.EnsureTrailingPeriod(template.RoleDescription)
		);
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

			if (!SeederDescriptionHelpers.HasMinimumParagraphs(BuildRaceDescriptionForTesting(template)))
			{
				issues.Add($"Race {name} should build a three-paragraph race description.");
			}

			if (!SeederDescriptionHelpers.HasMinimumParagraphs(BuildEthnicityDescriptionForTesting(template)))
			{
				issues.Add($"Race {name} should build a three-paragraph stock ethnicity description.");
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

				if (template.OverlayDescriptionVariants == null || template.OverlayDescriptionVariants.Count < 2)
				{
					issues.Add($"Humanoid variety race {name} should define at least two overlay description variants.");
				}
			}
			else
			{
				if (template.DescriptionVariants == null || template.DescriptionVariants.Count < 2)
				{
					issues.Add($"Bestial race {name} should define at least two stock description variants.");
				}
			}

			if ((template.DescriptionVariants ?? template.OverlayDescriptionVariants)?.Any(x =>
				    string.IsNullOrWhiteSpace(x.ShortDescription) || string.IsNullOrWhiteSpace(x.FullDescription)) == true)
			{
				issues.Add($"Race {name} has a blank stock description variant.");
				}
		}

		return issues;
	}
}
