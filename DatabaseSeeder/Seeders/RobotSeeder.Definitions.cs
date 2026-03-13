#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
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
		string? ParentRaceName,
		bool Playable,
		bool CanUseWeapons,
		bool UsesHumanoidCharacteristics,
		string BloodLiquidName,
		string HealthStrategyName,
		SizeCategory Size,
		bool CanClimb,
		bool CanSwim,
		string? ShortDescriptionPattern,
		string? FullDescriptionPattern,
		IReadOnlyList<RobotAttackBindingTemplate> Attacks,
		IReadOnlyList<RobotBodypartUsageTemplate>? BodypartUsages = null);

	private static readonly IReadOnlyDictionary<string, RobotRaceTemplate> Templates =
		new Dictionary<string, RobotRaceTemplate>(StringComparer.OrdinalIgnoreCase)
		{
			["Robot Humanoid"] = new(
				"Robot Humanoid",
				"Robot Humanoid",
				"A humanoid robotic chassis designed to retain broad compatibility with standard humanoid clothing, armour, and equipment.",
				"Humanoid",
				false,
				true,
				true,
				"hydraulic fluid",
				"Robot Articulated Model",
				SizeCategory.Normal,
				true,
				false,
				null,
				null,
				[
					new("Jab", "rhand", "lhand"),
					new("Cross", "rhand", "lhand"),
					new("Hook", "rhand", "lhand"),
					new("Elbow", "relbow", "lelbow"),
					new("Bite", "mouth"),
					new("Snap Kick", "rfoot", "lfoot")
				]),
			["Spider Crawler Robot"] = new(
				"Spider Crawler Robot",
				"Spider Crawler Robot",
				"A humanoid upper chassis mounted on a sprawling multi-legged crawler base.",
				"Robot Humanoid",
				false,
				true,
				true,
				"hydraulic fluid",
				"Robot Articulated Model",
				SizeCategory.Normal,
				true,
				false,
				null,
				null,
				[
					new("Jab", "rhand", "lhand"),
					new("Cross", "rhand", "lhand"),
					new("Hook", "rhand", "lhand"),
					new("Elbow", "relbow", "lelbow"),
					new("Bite", "mouth")
				],
				[
					new("rleg1", "general"),
					new("lleg1", "general"),
					new("rleg2", "general"),
					new("lleg2", "general"),
					new("rleg3", "general"),
					new("lleg3", "general"),
					new("rleg4", "general"),
					new("lleg4", "general")
				]),
			["Circular Saw Robot"] = new(
				"Circular Saw Robot",
				"Circular Saw Robot",
				"A worker-frame robot with integrated circular saw hand attachments in place of normal manipulators.",
				"Robot Humanoid",
				false,
				false,
				true,
				"hydraulic fluid",
				"Robot Articulated Model",
				SizeCategory.Normal,
				true,
				false,
				null,
				null,
				[
					new("Circular Saw Slash", "rsaw", "lsaw"),
					new("Elbow", "relbow", "lelbow"),
					new("Bite", "mouth"),
					new("Snap Kick", "rfoot", "lfoot")
				],
				[
					new("rsaw", "general"),
					new("lsaw", "general")
				]),
			["Pneumatic Hammer Robot"] = new(
				"Pneumatic Hammer Robot",
				"Pneumatic Hammer Robot",
				"A demolition robot with paired pneumatic hammer hand attachments.",
				"Robot Humanoid",
				false,
				false,
				true,
				"hydraulic fluid",
				"Robot Articulated Model",
				SizeCategory.Normal,
				true,
				false,
				null,
				null,
				[
					new("Pneumatic Hammer Blow", "rhammer", "lhammer"),
					new("Elbow", "relbow", "lelbow"),
					new("Bite", "mouth"),
					new("Snap Kick", "rfoot", "lfoot")
				],
				[
					new("rhammer", "general"),
					new("lhammer", "general")
				]),
			["Sword-Hand Robot"] = new(
				"Sword-Hand Robot",
				"Sword-Hand Robot",
				"A martial chassis with monoblade sword hands replacing both palms.",
				"Robot Humanoid",
				false,
				false,
				true,
				"hydraulic fluid",
				"Robot Articulated Model",
				SizeCategory.Normal,
				true,
				false,
				null,
				null,
				[
					new("Sword-Hand Lunge", "rblade", "lblade"),
					new("Elbow", "relbow", "lelbow"),
					new("Bite", "mouth"),
					new("Snap Kick", "rfoot", "lfoot")
				],
				[
					new("rblade", "general"),
					new("lblade", "general")
				]),
			["Winged Robot"] = new(
				"Winged Robot",
				"Winged Robot",
				"A humanoid robot fitted with broad articulated wings for lift and wing-buffet attacks.",
				"Robot Humanoid",
				false,
				true,
				true,
				"hydraulic fluid",
				"Robot Articulated Model",
				SizeCategory.Normal,
				true,
				false,
				null,
				null,
				[
					new("Jab", "rhand", "lhand"),
					new("Cross", "rhand", "lhand"),
					new("Hook", "rhand", "lhand"),
					new("Elbow", "relbow", "lelbow"),
					new("Bite", "mouth"),
					new("Snap Kick", "rfoot", "lfoot"),
					new("Wing Buffet", "rwing", "lwing")
				],
				[
					new("rwingbase", "general"),
					new("lwingbase", "general"),
					new("rwing", "general"),
					new("lwing", "general")
				]),
			["Jet Robot"] = new(
				"Jet Robot",
				"Jet Robot",
				"A humanoid robot with paired vector-thrust jet pods replacing biological-style wings.",
				"Robot Humanoid",
				false,
				true,
				true,
				"hydraulic fluid",
				"Robot Articulated Model",
				SizeCategory.Normal,
				true,
				false,
				null,
				null,
				[
					new("Jab", "rhand", "lhand"),
					new("Cross", "rhand", "lhand"),
					new("Hook", "rhand", "lhand"),
					new("Elbow", "relbow", "lelbow"),
					new("Bite", "mouth"),
					new("Snap Kick", "rfoot", "lfoot"),
					new("Jet Ram", "rjet", "ljet")
				],
				[
					new("rjet", "general"),
					new("ljet", "general")
				]),
			["Mandible Robot"] = new(
				"Mandible Robot",
				"Mandible Robot",
				"A predatory humanoid robot whose lower face houses heavy shearing mandibles.",
				"Robot Humanoid",
				false,
				true,
				true,
				"hydraulic fluid",
				"Robot Articulated Model",
				SizeCategory.Normal,
				true,
				false,
				null,
				null,
				[
					new("Jab", "rhand", "lhand"),
					new("Cross", "rhand", "lhand"),
					new("Hook", "rhand", "lhand"),
					new("Elbow", "relbow", "lelbow"),
					new("Mandible Shear", "mandibles"),
					new("Snap Kick", "rfoot", "lfoot")
				],
				[
					new("mandibles", "general")
				]),
			["Wheeled Robot"] = new(
				"Wheeled Robot",
				"Wheeled Robot",
				"A humanoid robot mounted on paired wheel assemblies instead of conventional lower legs and feet.",
				"Robot Humanoid",
				false,
				true,
				true,
				"hydraulic fluid",
				"Robot Articulated Model",
				SizeCategory.Normal,
				false,
				false,
				null,
				null,
				[
					new("Jab", "rhand", "lhand"),
					new("Cross", "rhand", "lhand"),
					new("Hook", "rhand", "lhand"),
					new("Elbow", "relbow", "lelbow"),
					new("Bite", "mouth"),
					new("Wheel Ram", "rwheel", "lwheel")
				],
				[
					new("rwheel", "general"),
					new("lwheel", "general")
				]),
			["Tracked Robot"] = new(
				"Tracked Robot",
				"Tracked Robot",
				"A humanoid robot whose lower body is replaced by compact armoured track pods.",
				"Robot Humanoid",
				false,
				true,
				true,
				"hydraulic fluid",
				"Robot Articulated Model",
				SizeCategory.Normal,
				false,
				false,
				null,
				null,
				[
					new("Jab", "rhand", "lhand"),
					new("Cross", "rhand", "lhand"),
					new("Hook", "rhand", "lhand"),
					new("Elbow", "relbow", "lelbow"),
					new("Bite", "mouth"),
					new("Track Grind", "rtrack", "ltrack")
				],
				[
					new("rtrack", "general"),
					new("ltrack", "general")
				]),
			["Cyborg"] = new(
				"Cyborg",
				"Cyborg Humanoid",
				"A human-passing cybernetic chassis with synthetic flesh styling over a wholly robotic internal design.",
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
				null,
				[
					new("Jab", "rhand", "lhand"),
					new("Cross", "rhand", "lhand"),
					new("Hook", "rhand", "lhand"),
					new("Elbow", "relbow", "lelbow"),
					new("Bite", "mouth"),
					new("Snap Kick", "rfoot", "lfoot")
				]),
			["Roomba Robot"] = new(
				"Roomba Robot",
				"Roomba Robot",
				"A compact disc-shaped maintenance robot that hustles about on low drive wheels.",
				null,
				false,
				false,
				false,
				"machine oil",
				"Robot Utility Construct",
				SizeCategory.Small,
				false,
				false,
				"a compact wheeled robot",
				"This compact robot is built around a low, circular chassis with intake vents, drive wheels, and a sensor cluster tucked into its shell.",
				[
					new("Wheel Ram", "rdrivewheel", "ldrivewheel")
				],
				[
					new("rdrivewheel", "general"),
					new("ldrivewheel", "general")
				]),
			["Tracked Utility Robot"] = new(
				"Tracked Utility Robot",
				"Tracked Utility Robot",
				"A low-slung utility robot that moves on compact rubberised tracks instead of wheels.",
				null,
				false,
				false,
				false,
				"machine oil",
				"Robot Utility Construct",
				SizeCategory.Small,
				false,
				false,
				"a tracked utility robot",
				"This squat utility robot rides on short track units braced around a hardened service chassis and a swivelling sensor pod.",
				[
					new("Track Grind", "rtrack", "ltrack")
				],
				[
					new("rtrack", "general"),
					new("ltrack", "general")
				]),
			["Robot Dog"] = new(
				"Robot Dog",
				"Robot Dog",
				"A quadruped robotic hound with articulated paws, a sensor-snouted head, and a durable service frame.",
				null,
				false,
				false,
				false,
				"hydraulic fluid",
				"Robot Articulated Model",
				SizeCategory.Normal,
				true,
				false,
				"a robotic hound",
				"This robotic hound combines a reinforced quadruped frame with a sensor-rich head and a servo-driven jaw built for grip and pursuit.",
				[
					new("Carnivore Bite", "mouth"),
					new("Claw Low Swipe", "rfclaw", "lfclaw", "rrclaw", "lrclaw"),
					new("Claw High Swipe", "rfclaw", "lfclaw", "rrclaw", "lrclaw")
				],
				[
					new("muzzle", "general"),
					new("rfpaw", "general"),
					new("lfpaw", "general"),
					new("rrpaw", "general"),
					new("lrpaw", "general")
				]),
			["Robot Cockroach"] = new(
				"Robot Cockroach",
				"Robot Cockroach",
				"A tiny insectoid robot with a chitinous shell, twitching antennae, and razor mandibles.",
				null,
				false,
				false,
				false,
				"machine oil",
				"Robot Utility Construct",
				SizeCategory.Small,
				true,
				false,
				"a cockroach-shaped robot",
				"This small robotic insect has a segmented shell, darting legs, and a sensor-laden head between its antennae.",
				[
					new("Mandible Bite", "mandibles")
				],
				[
					new("mandibles", "general"),
					new("rantenna", "general"),
					new("lantenna", "general")
				])
		};

	internal static IReadOnlyDictionary<string, RobotRaceTemplate> TemplatesForTesting => Templates;

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
			if (!validBodyKeys.Contains(template.BodyKey))
			{
				issues.Add($"Template {template.Name} references unknown body key {template.BodyKey}.");
			}

			if (string.IsNullOrWhiteSpace(template.Description))
			{
				issues.Add($"Template {template.Name} is missing a description.");
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

			if (template.UsesHumanoidCharacteristics && template.ParentRaceName is null)
			{
				issues.Add($"Humanoid template {template.Name} should inherit from a humanoid parent race.");
			}
		}

		var playable = Templates.Values.Where(x => x.Playable).Select(x => x.Name).OrderBy(x => x).ToArray();
		if (!playable.SequenceEqual(["Cyborg"]))
		{
			issues.Add("Only the Cyborg template should currently be playable.");
		}

		return issues;
	}
}
