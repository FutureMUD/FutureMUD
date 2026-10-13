#nullable enable

using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace MudSharp.Magic;

/// <summary>The exact native reserve is authored and resolved by ownership; it is never inferred from a target or caster.</summary>
public sealed record ArmageddonNativeReserveSpellBindings(long EmptyAuraResourceId)
{
	public void Validate() => ArmageddonPrimitiveSpellBindings.RequirePositive(EmptyAuraResourceId, nameof(EmptyAuraResourceId));
}

/// <summary>Pure locked Second Breath, Steal Breath and Empty Aura definitions. No admissions or alternate variants are enabled.</summary>
public static class ArmageddonNativeReserveSpellContent
{
	public const string SecondBreathKey = "arm.spell.second_breath";
	public const string StealBreathKey = "arm.spell.steal_breath";
	public const string EmptyAuraKey = "arm.spell.empty_aura";
	public const double AmountPerGrade = 5.0;
	public const int MaximumGrade = 7;

	public static double AmountForGrade(int grade)
	{
		if (grade is < 1 or > MaximumGrade) throw new ArgumentOutOfRangeException(nameof(grade), grade, "Use the external selected grade from 1 through 7.");
		return AmountPerGrade * grade;
	}
	public static IReadOnlyList<ArmageddonCanonicalSpellContribution> Create(ArmageddonNativeReserveSpellBindings bindings)
	{
		ArgumentNullException.ThrowIfNull(bindings); bindings.Validate();
		return Array.AsReadOnly(new[] { SecondBreath(), StealBreath(), EmptyAura(bindings.EmptyAuraResourceId) });
	}
	public static ArmageddonCanonicalSpellContribution SecondBreath() => Contribution("lightning", SecondBreathKey,
		"Second Breath", 20, 60,
		"Restore at most five stamina per selected grade, clamped to the native maximum. The signed native body adjustment bypasses expenditure multipliers and second winds.",
		new XElement("Effect", new XAttribute("type", "staminadelta"), new XElement("Formula", new XCData("5*grade")),
			new XElement("DirectStaminaAdjustment", true)));
	public static ArmageddonCanonicalSpellContribution StealBreath() => Contribution("lightning", StealBreathKey,
		"Steal Breath", 7, 30,
		"Remove at most the current stamina or five stamina per selected grade, whichever is less. No stamina is transferred and no second wind or expenditure multiplier is invoked.",
		new XElement("Effect", new XAttribute("type", "staminadelta"), new XElement("Formula", new XCData("-5*grade")),
			new XElement("DirectStaminaAdjustment", true)));
	public static ArmageddonCanonicalSpellContribution EmptyAura(long resourceId)
	{
		ArmageddonPrimitiveSpellBindings.RequirePositive(resourceId, nameof(resourceId));
		return Contribution("void", EmptyAuraKey, "Empty Aura", 7, 30,
			"Remove at most the exact configured reserve balance or five units per selected grade, whichever is less. No caster credit, substituted reserve, or negative balance is permitted. An absent exact reserve produces no change.",
			new XElement("Effect", new XAttribute("type", "magicresourcedelta"), new XElement("Resource", resourceId),
				new XElement("Formula", new XCData("-5*grade")), new XElement("ExactResourceDepletion", true)));
	}
	private static ArmageddonCanonicalSpellContribution Contribution(string school, string key, string name,
		double minimum, int opening, string description, XElement effect) => new("arm.capability." + school,
		new ArmageddonUtilitySpellContent(key, name, description, "0", minimum,
			"$0 shape|shapes an enchantment around $1.", (resource, cost, filter) =>
			{
				ArmageddonPrimitiveSpellBindings.RequirePositive(resource, nameof(resource));
				ArmageddonPrimitiveSpellBindings.RequirePositive(cost, nameof(cost));
				if (filter < 0) throw new ArgumentOutOfRangeException(nameof(filter));
				return ArmageddonUtilitySpellContent.Definition(key, "character", resource, cost, filter,
					opening, minimum, new XElement(effect));
			}));
}
