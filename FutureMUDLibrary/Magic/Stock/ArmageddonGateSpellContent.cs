#nullable enable

using System;

namespace MudSharp.Magic;

/// <summary>Canonical Void gate, bound to owned native presentation and an explicit undead marker.</summary>
public static class ArmageddonGateSpellContent
{
	public const string Key = "arm.spell.linked_threshold";
	public static ArmageddonCanonicalSpellContribution Create(long templateId, long undeadMeritId)
	{
		if (templateId <= 0 || undeadMeritId <= 0)
			throw new ArgumentException("Linked Threshold requires exact owned presentation and undead marker IDs.");
		var configuration = new SpellGateConfiguration(templateId, 120, undeadMeritId, 60);
		var content = new ArmageddonUtilitySpellContent(Key, "Linked Threshold",
			"Opens one two-room native Nilaz passage from the current room to the fixed room recorded on exactly one carried Arcane Mark. " +
			"The passage lasts 120 seconds per selected grade. Expiry closes both external portals while occupants and goods remain; " +
			"living non-undead occupants suffer separate 1-25 hypoxia and stun rolls at an authored native 60-second cadence. " +
			"Empty closed rooms retire after loaded and persisted custody checks.",
			"0", 33, "$0 open|opens a threshold through Nilaz.",
			(resource, cost, filter) => ArmageddonUtilitySpellContent.Definition(Key, "room", resource,
				cost, filter, 30, 33, configuration.SaveEffect()));
		return new("arm.capability.void", content);
	}
}
