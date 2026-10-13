#nullable enable
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic;

/// <summary>Exact package-owned dependencies; callers resolve these through typed ownership records.</summary>
public sealed record ArmageddonPrimitiveSpellBindings(long StrengthTraitId, long ResistanceTraitId,
	long InvisibilityApplicabilityProgId, long EmptyAuraResourceId, Difficulty ResistanceDifficulty)
{
	public void Validate()
	{
		RequirePositive(StrengthTraitId, nameof(StrengthTraitId));
		RequirePositive(ResistanceTraitId, nameof(ResistanceTraitId));
		RequirePositive(InvisibilityApplicabilityProgId, nameof(InvisibilityApplicabilityProgId));
		RequirePositive(EmptyAuraResourceId, nameof(EmptyAuraResourceId));
		if (!Enum.IsDefined(ResistanceDifficulty))
			throw new ArgumentOutOfRangeException(nameof(ResistanceDifficulty));
	}

	internal static void RequirePositive(long value, string name)
	{
		if (value <= 0) throw new ArgumentOutOfRangeException(name, value, "A resolved positive native identity is required.");
	}
}

/// <summary>
/// Pure locked S4 definitions using native status, condition removal, trait and reserve effects.
/// These contributions do not enable admissions, alternate methods or carriers, and do not assert gameplay qualification.
/// </summary>
public static partial class ArmageddonPrimitiveSpellContent
{
	/// <summary>Named Wind concealment; only the exact viewer applicability prog is required.</summary>
	public static ArmageddonCanonicalSpellContribution FalsePresence(long applicabilityProgId)
	{
		ArmageddonPrimitiveSpellBindings.RequirePositive(applicabilityProgId, nameof(applicabilityProgId));
		return Character("wind", "false_presence", "False Presence", 7, 40, "60*grade",
			"Hide the selected recipient's visible presence for 60 seconds per grade without a trait modifier.",
			new XElement("Effect", new XAttribute("type", "invisibility"),
				new XElement("FilterProg", applicabilityProgId)));
	}

	/// <summary>
	/// Strength must be an ordinary package-bound attribute. Invisibility uses the supplied applicability prog.
	/// Still Limbs uses the supplied native resistance trait and difficulty; other entries retain unopposed
	/// native stock defaults. Heavy Slumber also respects native sleep prevention.
	/// </summary>
	public static IReadOnlyList<ArmageddonCanonicalSpellContribution> Create(ArmageddonPrimitiveSpellBindings bindings)
	{
		ArgumentNullException.ThrowIfNull(bindings);
		bindings.Validate();
		return Array.AsReadOnly(new[]
		{
			Status("fire", "borrowed_tongues", "Borrowed Tongues", "comprehendlanguage", 7, 30,
				"Comprehend speech for 60 seconds per grade without granting fluent speech."),
			Status("water", "hush_hearing", "Hush Hearing", "deafness", 7, 40,
				"Suppress the recipient's hearing for 60 seconds per grade."),
			Status("water", "seal_voice", "Seal Voice", "silence", 33, 40,
				"Prevent normal speech through native magical silence for 60 seconds per grade."),
			Status("earth", "heavy_slumber", "Heavy Slumber", "sleep", 20, 30,
				"Put a susceptible recipient into native magical sleep for 60 seconds per grade. Native sleep prevention still applies."),
			Strength("earthen_strength", "Earthen Strength", 20, 30, "grade", bindings.StrengthTraitId),
			Strength("sapping_weight", "Sapping Weight", 7, 60, "-grade", bindings.StrengthTraitId),
			Character("wind", "veil_from_sight", "Veil from Sight", 7, 30, "60*grade",
				"Apply native invisibility for 60 seconds per grade with the exact supplied applicability prog.",
				new XElement("Effect", new XAttribute("type", "invisibility"),
					new XElement("FilterProg", bindings.InvisibilityApplicabilityProgId))),
			Removal("wind", "reveal_the_hidden", "Reveal the Hidden", "removeinvisibility", 30,
				"Remove spell-induced invisibility from the selected recipient. Unrelated effects remain."),
			Status("shadow", "shroud_eyes", "Shroud Eyes", "blindness", 7, 30,
				"Apply native magical blindness for 60 seconds per grade."),
			Removal("shadow", "clear_eyes", "Clear Eyes", "removeblindness", 30,
				"Remove spell-induced blindness from the selected recipient. Unrelated effects remain."),
			Removal("shadow", "lift_hex", "Lift Hex", "removecurse", 30,
				"Remove the selected recipient's native magical curse status. Unrelated effects remain."),
			Status("shadow", "dread_presence", "Dread Presence", "fear", 7, 30,
				"Apply native magical fear for 60 seconds per grade."),
			Status("shadow", "night_eyes", "Night Eyes", "infravision", 7, 30,
				"Grant native infravision for 60 seconds per grade."),
			Status("shadow", "clinging_hex", "Clinging Hex", "curse", 7, 30,
				"Apply the native magical curse status for 60 seconds per grade without an additional trait modifier."),
			Character("lightning", "second_breath", "Second Breath", 20, 60, "0",
				"Restore at most five stamina per grade, clamped to the native maximum. No reserve is transferred.",
				new XElement("Effect", new XAttribute("type", "staminadelta"), new XElement("Formula", new XCData("5*grade")))),
			Status("lightning", "wakeful_mind", "Wakeful Mind", "insomnia", 20, 60,
				"Apply native magical wakefulness and sleep prevention for 60 seconds per grade."),
			Character("lightning", "still_limbs", "Still Limbs", 20, 30, "60*grade",
				"Apply native magical paralysis for 60 seconds per grade after the configured native resistance check.",
				new XElement("Effect", new XAttribute("type", "paralysis")),
				resistingTrait: bindings.ResistanceTraitId, resistingDifficulty: bindings.ResistanceDifficulty),
			Character("void", "empty_aura", "Empty Aura", 7, 30, "0",
				"Remove at most five units per grade from the exact configured reserve, floored at zero. The caster receives no credit.",
				new XElement("Effect", new XAttribute("type", "magicresourcedelta"),
					new XElement("Resource", bindings.EmptyAuraResourceId), new XElement("Formula", new XCData("-5*grade"))))
		});
	}

	private static ArmageddonCanonicalSpellContribution Status(string school, string suffix, string name,
		string effect, double minimum, int opening, string description) =>
		Character(school, suffix, name, minimum, opening, "60*grade", description,
			new XElement("Effect", new XAttribute("type", effect)));

	private static ArmageddonCanonicalSpellContribution Removal(string school, string suffix, string name,
		string effect, int opening, string description) =>
		Character(school, suffix, name, 7, opening, "0", description,
			new XElement("Effect", new XAttribute("type", effect)));

	private static ArmageddonCanonicalSpellContribution Strength(string suffix, string name, double minimum,
		int opening, string formula, long trait) =>
		Character("earth", suffix, name, minimum, opening, "60*grade",
			$"Apply a native strength modifier of {formula} for 60 seconds per grade to the exact package-bound ordinary attribute.",
			new XElement("Effect", new XAttribute("type", "boost"), new XAttribute("trait", trait),
				new XAttribute("bonus", 0), new XAttribute("context", (int)TraitBonusContext.None)), formula);

	private static ArmageddonCanonicalSpellContribution Character(string school, string suffix, string name,
		double minimum, int opening, string duration, string description, XElement effect,
		string? scalarFormula = null, long? resistingTrait = null, Difficulty? resistingDifficulty = null)
	{
		var key = "arm.spell." + suffix;
		var content = new ArmageddonUtilitySpellContent(key, name, description, duration, minimum,
			"$0 shape|shapes an enchantment around $1.", (resource, cost, filter) =>
			{
				ArmageddonPrimitiveSpellBindings.RequirePositive(resource, nameof(resource));
				ArmageddonPrimitiveSpellBindings.RequirePositive(cost, nameof(cost));
				if (filter < 0) throw new ArgumentOutOfRangeException(nameof(filter), filter, "Use zero for no eligibility filter or a positive resolved prog identity.");
				var definition = ArmageddonUtilitySpellContent.Definition(key, "character", resource, cost, filter,
					opening, minimum, new XElement(effect));
				if (scalarFormula is not null)
					definition.Element("ControlledPower")!.Element("ScalarBindings")!.Add(new XElement("Binding",
						new XAttribute("list", "target"), new XAttribute("index", 0), new XAttribute("effect", "boost"),
						new XAttribute("field", "Bonus"), scalarFormula));
				return definition;
			}, ResistingTraitId: resistingTrait, ResistingDifficulty: resistingDifficulty);
		return new("arm.capability." + school, content);
	}
}
