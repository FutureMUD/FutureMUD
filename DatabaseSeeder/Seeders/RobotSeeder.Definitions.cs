#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Framework;
using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public partial class RobotSeeder
{
	internal sealed record RobotAttackBindingTemplate(string AttackName, params string[] BodypartAliases);

	internal sealed record RobotBodypartUsageTemplate(string BodypartAlias, string Usage);

	internal sealed record RobotRaceTemplate(
		string Name,
		string BodyKey,
		string Description,
		string RoleDescription,
		string? ParentRaceName,
		bool Playable,
		bool CanUseWeapons,
		bool UsesHumanoidCharacteristics,
		string BloodLiquidName,
		string HealthStrategyName,
		SizeCategory Size,
		bool CanClimb,
		bool CanSwim,
		IReadOnlyList<StockDescriptionVariant>? DescriptionVariants,
		IReadOnlyList<RobotAttackBindingTemplate> Attacks,
		IReadOnlyList<RobotBodypartUsageTemplate>? BodypartUsages = null,
		bool UsesHumanGenders = false,
		IReadOnlyList<StockDescriptionVariant>? OverlayDescriptionVariants = null,
		string combatStrategyKey = "Construct Brawler")
	{
		public string CombatStrategyKey => combatStrategyKey;
	}

	private static IReadOnlyList<RobotBodypartUsageTemplate> WithNonCyborgNippleRemovals(
		params RobotBodypartUsageTemplate[] usages)
	{
		return usages
			.Concat(
			[
				new RobotBodypartUsageTemplate("rnipple", "remove"),
				new RobotBodypartUsageTemplate("lnipple", "remove")
			])
			.ToArray();
	}

	private static IReadOnlyList<StockDescriptionVariant> Variants(
		params (string ShortDescription, string FullDescription)[] variants)
	{
		return SeederDescriptionHelpers.BuildVariantList(variants);
	}

	private static readonly IReadOnlyDictionary<string, RobotRaceTemplate> Templates =
		new Dictionary<string, RobotRaceTemplate>(StringComparer.OrdinalIgnoreCase)
		{
			["Robot Humanoid"] = new(
				"Robot Humanoid",
				"Robot Humanoid",
				"A humanoid robotic chassis designed to retain broad compatibility with standard humanoid clothing, armour, and equipment.",
				"These general-purpose chassis sit at the practical centre of a robot ecosystem, taking on service, labour, security and companionship roles wherever a society wants machine bodies that can share tools and spaces with people.",
				"Humanoid",
				false,
				true,
				true,
				"hydraulic fluid",
				"Robot Articulated Model",
				SizeCategory.Normal,
				true,
				false,
				Variants(
					("a humanoid service robot",
						"This humanoid robot is built on an articulated plated chassis with dexterous hands and a sensor-packed head designed for general tool use."),
					("a plated utility humanoid",
						"This articulated service chassis balances protective plating, fine manipulators and a broad sensor suite, making it look ready for work rather than ornament.")
				),
				[
					new("Jab", "rhand", "lhand"),
					new("Cross", "rhand", "lhand"),
					new("Hook", "rhand", "lhand"),
					new("Elbow", "relbow", "lelbow"),
					new("Bite", "mouth"),
					new("Snap Kick", "rfoot", "lfoot")
				],
				WithNonCyborgNippleRemovals(),
				combatStrategyKey: "Melee (Auto)"),
			["Spider Crawler Robot"] = new(
				"Spider Crawler Robot",
				"Spider Crawler Robot",
				"A humanoid upper chassis mounted on a sprawling multi-legged crawler base.",
				"Spider crawler frames suit environments where balance, stability and low-profile mobility matter more than social comfort, making them natural fits for maintenance, hazardous access and intimidating patrol work.",
				"Robot Humanoid",
				false,
				true,
				true,
				"hydraulic fluid",
				"Robot Articulated Model",
				SizeCategory.Normal,
				true,
				false,
				Variants(
					("a spider-legged crawler robot",
						"This robot mounts a humanoid upper chassis atop a sprawling multi-legged crawler base, letting it scuttle low and steady on articulated legs."),
					("a low-slung crawler chassis",
						"This machine keeps a recognisably humanoid upper body, but its mass is carried by a many-legged underframe built for sure footing and abrupt directional changes.")
				),
				[
					new("Jab", "rhand", "lhand"),
					new("Cross", "rhand", "lhand"),
					new("Hook", "rhand", "lhand"),
					new("Elbow", "relbow", "lelbow"),
					new("Bite", "mouth")
				],
				WithNonCyborgNippleRemovals(
					new RobotBodypartUsageTemplate("rleg1", "general"),
					new RobotBodypartUsageTemplate("lleg1", "general"),
					new RobotBodypartUsageTemplate("rleg2", "general"),
					new RobotBodypartUsageTemplate("lleg2", "general"),
					new RobotBodypartUsageTemplate("rleg3", "general"),
					new RobotBodypartUsageTemplate("lleg3", "general"),
					new RobotBodypartUsageTemplate("rleg4", "general"),
					new RobotBodypartUsageTemplate("lleg4", "general")),
				combatStrategyKey: "Melee (Auto)"),
			["Circular Saw Robot"] = new(
				"Circular Saw Robot",
				"Circular Saw Robot",
				"A worker-frame robot with integrated circular saw hand attachments in place of normal manipulators.",
				"Circular saw frames belong in industrial yards, salvage crews and demolition teams, where builders trade finesse for brute cutting power and accept that anyone meeting the machine will read it as dangerous first.",
				"Robot Humanoid",
				false,
				false,
				true,
				"hydraulic fluid",
				"Robot Articulated Model",
				SizeCategory.Normal,
				true,
				false,
				Variants(
					("a circular-saw worker robot",
						"This worker-frame robot carries circular saws in place of hands, with a rugged torso and heavy lower limbs braced for cutting work."),
					("a saw-armed industrial robot",
						"This chassis is built around reinforced joints and protected housings, its spinning hand tools giving the machine an unmistakably brutal silhouette.")
				),
				[
					new("Circular Saw Slash", "rsaw", "lsaw"),
					new("Elbow", "relbow", "lelbow"),
					new("Bite", "mouth"),
					new("Snap Kick", "rfoot", "lfoot")
				],
				WithNonCyborgNippleRemovals(
					new RobotBodypartUsageTemplate("rsaw", "general"),
					new RobotBodypartUsageTemplate("lsaw", "general")),
				combatStrategyKey: "Construct Brawler"),
			["Pneumatic Hammer Robot"] = new(
				"Pneumatic Hammer Robot",
				"Pneumatic Hammer Robot",
				"A demolition robot with paired pneumatic hammer hand attachments.",
				"Pneumatic hammer frames are purpose-built for heavy breaching and wrecking work, the kind of robot deployed when collateral noise and violent impact matter less than getting through hard material quickly.",
				"Robot Humanoid",
				false,
				false,
				true,
				"hydraulic fluid",
				"Robot Articulated Model",
				SizeCategory.Normal,
				true,
				false,
				Variants(
					("a hammer-armed demolition robot",
						"This demolition robot ends in paired pneumatic hammers rather than hands, its reinforced frame built to absorb recoil and deliver crushing strikes."),
					("a breaching robot with paired hammers",
						"This chassis is thick through the shoulders and forearms, every line of it arranged to survive vibration, impact and repeated close-range smashing.")
				),
				[
					new("Pneumatic Hammer Blow", "rhammer", "lhammer"),
					new("Elbow", "relbow", "lelbow"),
					new("Bite", "mouth"),
					new("Snap Kick", "rfoot", "lfoot")
				],
				WithNonCyborgNippleRemovals(
					new RobotBodypartUsageTemplate("rhammer", "general"),
					new RobotBodypartUsageTemplate("lhammer", "general")),
				combatStrategyKey: "Construct Brawler"),
			["Sword-Hand Robot"] = new(
				"Sword-Hand Robot",
				"Sword-Hand Robot",
				"A martial chassis with monoblade sword hands replacing both palms.",
				"Sword-hand frames are openly martial machines, used where intimidation, close combat and ceremonial menace are more important than subtlety or compatibility with ordinary civilian work.",
				"Robot Humanoid",
				false,
				false,
				true,
				"hydraulic fluid",
				"Robot Articulated Model",
				SizeCategory.Normal,
				true,
				false,
				Variants(
					("a sword-handed combat robot",
						"This combat chassis replaces both hands with monoblade sword assemblies, giving its otherwise humanoid frame a deliberately martial silhouette."),
					("a monoblade-armed battle chassis",
						"This robot's limb design prioritises reach and cutting arcs, its whole frame reading as a weapon system with just enough humanoid shape to navigate built spaces.")
				),
				[
					new("Sword-Hand Lunge", "rblade", "lblade"),
					new("Elbow", "relbow", "lelbow"),
					new("Bite", "mouth"),
					new("Snap Kick", "rfoot", "lfoot")
				],
				WithNonCyborgNippleRemovals(
					new RobotBodypartUsageTemplate("rblade", "general"),
					new RobotBodypartUsageTemplate("lblade", "general")),
				combatStrategyKey: "Construct Brawler"),
			["Winged Robot"] = new(
				"Winged Robot",
				"Winged Robot",
				"A humanoid robot fitted with broad articulated wings for lift and wing-buffet attacks.",
				"Winged frames serve couriers, scouts and prestige guardians that need a striking silhouette as much as mobility, and they blur the line between practical machine and deliberate display piece.",
				"Robot Humanoid",
				false,
				true,
				true,
				"hydraulic fluid",
				"Robot Articulated Model",
				SizeCategory.Normal,
				true,
				false,
				Variants(
					("a winged humanoid robot",
						"This humanoid robot carries broad articulated wings across its back, with a balanced plated frame designed for gliding lifts and punishing wing strikes."),
					("a broad-winged service chassis",
						"This plated frame spreads articulated wings from the shoulder line, making the machine look part aerodyne, part sentinel and entirely built to command space.")
				),
				[
					new("Jab", "rhand", "lhand"),
					new("Cross", "rhand", "lhand"),
					new("Hook", "rhand", "lhand"),
					new("Elbow", "relbow", "lelbow"),
					new("Bite", "mouth"),
					new("Snap Kick", "rfoot", "lfoot"),
					new("Wing Buffet", "rwing", "lwing")
				],
				WithNonCyborgNippleRemovals(
					new RobotBodypartUsageTemplate("rwingbase", "general"),
					new RobotBodypartUsageTemplate("lwingbase", "general"),
					new RobotBodypartUsageTemplate("rwing", "general"),
					new RobotBodypartUsageTemplate("lwing", "general")),
				combatStrategyKey: "Melee (Auto)"),
			["Jet Robot"] = new(
				"Jet Robot",
				"Jet Robot",
				"A humanoid robot with paired vector-thrust jet pods replacing biological-style wings.",
				"Jet-backed frames are specialised pursuit and rapid-response machines, built for speed, aggressive repositioning and the kind of dramatic deployment that reminds onlookers they are watching hardware rather than a person.",
				"Robot Humanoid",
				false,
				true,
				true,
				"hydraulic fluid",
				"Robot Articulated Model",
				SizeCategory.Normal,
				true,
				false,
				Variants(
					("a jet-backed humanoid robot",
						"This humanoid robot mounts paired vector-thrust jet pods where wings would sit, giving its angular chassis a sleek, flight-capable profile."),
					("a thrust-pod pursuit chassis",
						"This robot's upper frame is organised around compact jet assemblies and stabilising surfaces, giving it the impatient posture of a machine built to lunge into motion.")
				),
				[
					new("Jab", "rhand", "lhand"),
					new("Cross", "rhand", "lhand"),
					new("Hook", "rhand", "lhand"),
					new("Elbow", "relbow", "lelbow"),
					new("Bite", "mouth"),
					new("Snap Kick", "rfoot", "lfoot"),
					new("Jet Ram", "rjet", "ljet")
				],
				WithNonCyborgNippleRemovals(
					new RobotBodypartUsageTemplate("rjet", "general"),
					new RobotBodypartUsageTemplate("ljet", "general")),
				combatStrategyKey: "Melee (Auto)"),
			["Mandible Robot"] = new(
				"Mandible Robot",
				"Mandible Robot",
				"A predatory humanoid robot whose lower face houses heavy shearing mandibles.",
				"Mandible-faced frames are predator-coded by design, used where fear, capture or brutal close work matter, and they show how readily robot aesthetics can be tuned to unsettle organic observers.",
				"Robot Humanoid",
				false,
				true,
				true,
				"hydraulic fluid",
				"Robot Articulated Model",
				SizeCategory.Normal,
				true,
				false,
				Variants(
					("a mandible-faced predator robot",
						"This predatory robot has a humanoid frame crowned by a sensor head whose lower face opens into heavy shearing mandibles."),
					("a shearing-jawed hunter chassis",
						"This machine pairs a recognisably humanoid torso with a head assembly engineered to broadcast threat, the mandibles making every movement feel predatory.")
				),
				[
					new("Jab", "rhand", "lhand"),
					new("Cross", "rhand", "lhand"),
					new("Hook", "rhand", "lhand"),
					new("Elbow", "relbow", "lelbow"),
					new("Mandible Shear", "mandibles"),
					new("Snap Kick", "rfoot", "lfoot")
				],
				WithNonCyborgNippleRemovals(
					new RobotBodypartUsageTemplate("mandibles", "general")),
				combatStrategyKey: "Melee (Auto)"),
			["Wheeled Robot"] = new(
				"Wheeled Robot",
				"Wheeled Robot",
				"A humanoid robot mounted on paired wheel assemblies instead of conventional lower legs and feet.",
				"Wheeled humanoid frames make sense in warehouses, corridors and other smooth artificial environments where speed and endurance matter more than stairs, rubble or natural terrain.",
				"Robot Humanoid",
				false,
				true,
				true,
				"hydraulic fluid",
				"Robot Articulated Model",
				SizeCategory.Normal,
				false,
				false,
				Variants(
					("a wheeled humanoid robot",
						"This humanoid robot rides on paired wheel assemblies instead of legs below the hips, its upper chassis built to steer and balance at speed."),
					("a wheel-mounted service chassis",
						"This robot keeps a humanoid upper body for interaction and reach, but its underframe is unapologetically mechanical, favouring smooth rolling momentum over gait.")
				),
				[
					new("Jab", "rhand", "lhand"),
					new("Cross", "rhand", "lhand"),
					new("Hook", "rhand", "lhand"),
					new("Elbow", "relbow", "lelbow"),
					new("Bite", "mouth"),
					new("Wheel Ram", "rwheel", "lwheel")
				],
				WithNonCyborgNippleRemovals(
					new RobotBodypartUsageTemplate("rwheel", "general"),
					new RobotBodypartUsageTemplate("lwheel", "general")),
				combatStrategyKey: "Melee (Auto)"),
			["Tracked Robot"] = new(
				"Tracked Robot",
				"Tracked Robot",
				"A humanoid robot whose lower body is replaced by compact armoured track pods.",
				"Tracked humanoid frames are ruggedised for security, engineering and rough-service environments, favouring traction and persistence over grace whenever terrain or debris would defeat a simpler machine.",
				"Robot Humanoid",
				false,
				true,
				true,
				"hydraulic fluid",
				"Robot Articulated Model",
				SizeCategory.Normal,
				false,
				false,
				Variants(
					("a tracked humanoid robot",
						"This humanoid robot replaces legs with compact armoured track pods, giving its sturdy upper chassis a low, relentless stance."),
					("a track-driven patrol chassis",
						"This machine looks built to push forward through clutter and damage, its upper body mounted over armoured tracks that promise traction and stubborn momentum.")
				),
				[
					new("Jab", "rhand", "lhand"),
					new("Cross", "rhand", "lhand"),
					new("Hook", "rhand", "lhand"),
					new("Elbow", "relbow", "lelbow"),
					new("Bite", "mouth"),
					new("Track Grind", "rtrack", "ltrack")
				],
				WithNonCyborgNippleRemovals(
					new RobotBodypartUsageTemplate("rtrack", "general"),
					new RobotBodypartUsageTemplate("ltrack", "general")),
				combatStrategyKey: "Melee (Auto)"),
			["Cyborg"] = new(
				"Cyborg",
				"Cyborg Humanoid",
				"A human-passing cybernetic chassis with synthetic flesh styling over a wholly robotic internal design.",
				"Cyborg lines occupy the uneasy social space between person and platform, useful wherever a machine must move through human institutions unnoticed or where a mechanical being is deliberately made to look almost, but not quite, organic.",
				"Human",
				true,
				true,
				true,
				"hydraulic fluid",
				"Robot Articulated Model",
				SizeCategory.Normal,
				true,
				false,
				null,
				[
					new("Jab", "rhand", "lhand"),
					new("Cross", "rhand", "lhand"),
					new("Hook", "rhand", "lhand"),
					new("Elbow", "relbow", "lelbow"),
					new("Bite", "mouth"),
					new("Snap Kick", "rfoot", "lfoot")
				],
				null,
				true,
				Variants(
					("a synthetic-skinned cyborg", "This cyborg wears a carefully human-passing shell over obvious underlying precision, its proportions natural enough to read as personlike until the details linger."),
					("a nearly human cybernetic", "This cybernetic body mimics human flesh and posture closely, yet something in the surface regularity and controlled movement keeps the illusion from becoming comfortable.")
				),
				"Melee (Auto)"),
			["Roomba Robot"] = new(
				"Roomba Robot",
				"Roomba Robot",
				"A compact disc-shaped maintenance robot that hustles about on low drive wheels.",
				"These compact maintenance units belong wherever floors, vents and under-furniture spaces need constant machine attention, and they exemplify the low-status but ubiquitous labour that keeps larger facilities functioning.",
				null,
				false,
				false,
				false,
				"machine oil",
				"Robot Utility Construct",
				SizeCategory.Small,
				false,
				false,
				Variants(
					("a compact wheeled robot",
						"This compact robot is built around a low, circular chassis with intake vents, drive wheels, and a sensor cluster tucked into its shell."),
					("a disk-shaped maintenance robot",
						"This little utility unit hugs the floor on hidden drive wheels, its shell arranged around sensors and service fittings rather than anything resembling a face.")
				),
				[
					new("Wheel Ram", "rdrivewheel", "ldrivewheel"),
					new("Wheel Grind Close", "rdrivewheel", "ldrivewheel")
				],
				[
					new("rdrivewheel", "general"),
					new("ldrivewheel", "general")
				],
				combatStrategyKey: "Construct Skirmisher"),
			["Tracked Utility Robot"] = new(
				"Tracked Utility Robot",
				"Tracked Utility Robot",
				"A low-slung utility robot that moves on compact rubberised tracks instead of wheels.",
				"Tracked utility units are the practical answer for dirty or uneven service spaces, used where a low maintenance drone still needs enough grip and toughness to cross debris, mud or broken flooring.",
				null,
				false,
				false,
				false,
				"machine oil",
				"Robot Utility Construct",
				SizeCategory.Small,
				false,
				false,
				Variants(
					("a tracked utility robot",
						"This squat utility robot rides on short track units braced around a hardened service chassis and a swivelling sensor pod."),
					("a low-slung tracked drone",
						"This compact service frame sits close to the ground between short track runs, trading elegance for durability and a stubborn ability to keep moving.")
				),
				[
					new("Track Grind", "rtrack", "ltrack"),
					new("Track Crush", "rtrack", "ltrack")
				],
				[
					new("rtrack", "general"),
					new("ltrack", "general")
				],
				combatStrategyKey: "Construct Brawler"),
			["Robot Dog"] = new(
				"Robot Dog",
				"Robot Dog",
				"A quadruped robotic hound with articulated paws, a sensor-snouted head, and a durable service frame.",
				"Robotic hounds fill the familiar roles of patrol animal, tracker and companion machine, exploiting a shape people already understand while replacing fur and instinct with plating, sensors and obedient routines.",
				null,
				false,
				false,
				false,
				"hydraulic fluid",
				"Robot Articulated Model",
				SizeCategory.Normal,
				true,
				false,
				Variants(
					("a robotic hound",
						"This robotic hound combines a reinforced quadruped frame with a sensor-rich head and a servo-driven jaw built for grip and pursuit."),
					("a sensor-snouted robot dog",
						"This four-legged chassis borrows heavily from canine lines, using a familiar head-and-paw silhouette to package a purpose-built pursuit and guard machine.")
				),
				[
					new("Carnivore Bite", "mouth"),
					new("Bite", "mouth"),
					new("Claw Low Swipe", "rfclaw", "lfclaw", "rrclaw", "lrclaw"),
					new("Claw High Swipe", "rfclaw", "lfclaw", "rrclaw", "lrclaw")
				],
				[
					new("muzzle", "general"),
					new("rfpaw", "general"),
					new("lfpaw", "general"),
					new("rrpaw", "general"),
					new("lrpaw", "general")
				],
				combatStrategyKey: "Construct Brawler"),
			["Robot Cockroach"] = new(
				"Robot Cockroach",
				"Robot Cockroach",
				"A tiny insectoid robot with a chitinous shell, twitching antennae, and razor mandibles.",
				"Robot cockroaches belong to the world of inspection, infiltration and survival in cramped places, useful precisely because they can go where people and larger machines are unwelcome or unable to follow.",
				null,
				false,
				false,
				false,
				"machine oil",
				"Robot Utility Construct",
				SizeCategory.Small,
				true,
				false,
				Variants(
					("a cockroach-shaped robot",
						"This small robotic insect has a segmented shell, darting legs, and a sensor-laden head between its antennae."),
					("a chitin-shelled micro-drone",
						"This tiny machine uses an insectoid body plan for speed and access, its shell, antennae and mandibles making it look unpleasantly alive at a glance.")
				),
				[
					new("Mandible Bite", "mandibles"),
					new("Mandible Snap", "mandibles")
				],
				[
					new("mandibles", "general"),
					new("rantenna", "general"),
					new("lantenna", "general")
				],
				combatStrategyKey: "Construct Skirmisher")
		};

	internal static IReadOnlyDictionary<string, RobotRaceTemplate> TemplatesForTesting => Templates;

	internal static string BuildRaceDescriptionForTesting(RobotRaceTemplate template)
	{
		var supportingVariant = (template.DescriptionVariants ?? template.OverlayDescriptionVariants)?.FirstOrDefault();
		return SeederDescriptionHelpers.JoinParagraphs(
			SeederDescriptionHelpers.EnsureTrailingPeriod(template.Description),
			supportingVariant?.FullDescription ?? $"This stock robot line is represented by the {template.Name} chassis catalogue.",
			SeederDescriptionHelpers.EnsureTrailingPeriod(template.RoleDescription)
		);
	}

	internal static string BuildEthnicityDescriptionForTesting(RobotRaceTemplate template)
	{
		return SeederDescriptionHelpers.JoinParagraphs(
			SeederDescriptionHelpers.EnsureTrailingPeriod(
				$"The stock {template.Name.ToLowerInvariant()} ethnicity represents the default chassis lineage and presentation seeded for this robot line."),
			(template.DescriptionVariants ?? template.OverlayDescriptionVariants)?.Skip(1).FirstOrDefault()?.FullDescription ??
			(template.DescriptionVariants ?? template.OverlayDescriptionVariants)?.FirstOrDefault()?.FullDescription ??
			$"This stock build preserves the structural cues most recognisable to observers encountering a {template.Name.ToLowerInvariant()}.",
			SeederDescriptionHelpers.EnsureTrailingPeriod(template.RoleDescription)
		);
	}

	internal static IReadOnlyList<string> ValidateTemplateCatalogForTesting()
	{
		var issues = new List<string>();
		var validBodyKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"Robot Humanoid",
			"Spider Crawler Robot",
			"Circular Saw Robot",
			"Pneumatic Hammer Robot",
			"Sword-Hand Robot",
			"Winged Robot",
			"Jet Robot",
			"Mandible Robot",
			"Wheeled Robot",
			"Tracked Robot",
			"Cyborg Humanoid",
			"Roomba Robot",
			"Tracked Utility Robot",
			"Robot Dog",
			"Robot Cockroach"
		};

		foreach (var template in Templates.Values)
		{
			if (string.IsNullOrWhiteSpace(template.CombatStrategyKey))
			{
				issues.Add($"Template {template.Name} is missing a combat strategy key.");
			}
			else if (!CombatStrategySeederHelper.IsKnownStrategyName(template.CombatStrategyKey))
			{
				issues.Add($"Template {template.Name} references unknown combat strategy {template.CombatStrategyKey}.");
			}

			if (!validBodyKeys.Contains(template.BodyKey))
			{
				issues.Add($"Template {template.Name} references unknown body key {template.BodyKey}.");
			}

			if (string.IsNullOrWhiteSpace(template.Description))
			{
				issues.Add($"Template {template.Name} is missing a description.");
			}

			if (!SeederDescriptionHelpers.HasMinimumParagraphs(BuildRaceDescriptionForTesting(template)))
			{
				issues.Add($"Template {template.Name} should build a three-paragraph race description.");
			}

			if (!SeederDescriptionHelpers.HasMinimumParagraphs(BuildEthnicityDescriptionForTesting(template)))
			{
				issues.Add($"Template {template.Name} should build a three-paragraph ethnicity description.");
			}

			if (!template.Attacks.Any())
			{
				issues.Add($"Template {template.Name} should expose at least one natural attack.");
			}

			if (template.Attacks.Any(x => string.IsNullOrWhiteSpace(x.AttackName)))
			{
				issues.Add($"Template {template.Name} has an attack with no name.");
			}

			if (template.Attacks.Any(x => x.BodypartAliases.Length == 0))
			{
				issues.Add($"Template {template.Name} has an attack with no bound bodyparts.");
			}

			if (!template.Attacks.Any(x => RobotNonClinchAttackNames.Contains(x.AttackName)))
			{
				issues.Add($"Template {template.Name} must expose at least one non-clinch natural attack.");
			}

			if (!template.Attacks.Any(x => RobotClinchAttackNames.Contains(x.AttackName)))
			{
				issues.Add($"Template {template.Name} must expose at least one clinch natural attack.");
			}

			if (template.UsesHumanoidCharacteristics && template.ParentRaceName is null)
			{
				issues.Add($"Humanoid template {template.Name} should inherit from a humanoid parent race.");
			}

			if (!template.Name.EqualTo("Cyborg") &&
			    (template.DescriptionVariants == null || template.DescriptionVariants.Count < 2))
			{
				issues.Add($"Template {template.Name} should define at least two stock description variants.");
			}

			if (template.Name.EqualTo("Cyborg") &&
			    (template.OverlayDescriptionVariants == null || template.OverlayDescriptionVariants.Count < 2))
			{
				issues.Add("Template Cyborg should define at least two overlay description variants.");
			}

			if ((template.DescriptionVariants ?? template.OverlayDescriptionVariants)?.Any(x =>
				    string.IsNullOrWhiteSpace(x.ShortDescription) || string.IsNullOrWhiteSpace(x.FullDescription)) == true)
			{
				issues.Add($"Template {template.Name} contains a blank stock description variant.");
			}
		}

		var playable = Templates.Values.Where(x => x.Playable).Select(x => x.Name).OrderBy(x => x).ToArray();
		if (!playable.SequenceEqual(["Cyborg"]))
		{
			issues.Add("Only the Cyborg template should currently be playable.");
		}

		if (!SeederDescriptionHelpers.HasMinimumParagraphs(RobotCultureDescriptionForTesting))
		{
			issues.Add("Robot culture should define a three-paragraph description.");
		}

		return issues;
	}

	private static readonly HashSet<string> RobotNonClinchAttackNames = new(StringComparer.OrdinalIgnoreCase)
	{
		"Jab",
		"Cross",
		"Hook",
		"Circular Saw Slash",
		"Pneumatic Hammer Blow",
		"Sword-Hand Lunge",
		"Wing Buffet",
		"Jet Ram",
		"Wheel Ram",
		"Track Grind",
		"Snap Kick",
		"Carnivore Bite",
		"Claw Low Swipe",
		"Claw High Swipe",
		"Mandible Snap"
	};

	private static readonly HashSet<string> RobotClinchAttackNames = new(StringComparer.OrdinalIgnoreCase)
	{
		"Elbow",
		"Bite",
		"Mandible Shear",
		"Wheel Grind Close",
		"Track Crush",
		"Mandible Bite"
	};
}
