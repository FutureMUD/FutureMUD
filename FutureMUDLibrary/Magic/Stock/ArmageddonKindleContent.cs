#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.FutureProg;

namespace MudSharp.Magic;

/// <summary>Empower skill 47's selected bonuses, with explicit native trait and reserve identities.</summary>
public static class ArmageddonKindleContent
{
	public const string Key = "arm.spell.kindle_enchantment";
	public static ArmageddonCanonicalSpellContribution Create(long strength, long agility, long wisdom, long endurance,
		IReadOnlyDictionary<string, long> reserves)
	{
		foreach (var id in new[] { strength, agility, wisdom, endurance })
			ArmageddonPrimitiveSpellBindings.RequirePositive(id, nameof(id));
		if (reserves.Count == 0 || reserves.Any(x => string.IsNullOrWhiteSpace(x.Key) || x.Value <= 0))
			throw new ArgumentException("Kindle requires explicit native character reserve mappings.", nameof(reserves));
		XElement Option(string name, EquippedItemBonusKind kind, long target, params double[] amounts) =>
			new("Option", new XAttribute("name", name), new XAttribute("kind", kind), new XAttribute("target", target),
				amounts.Select(amount => new XElement("Amount", amount)));
		string ResourceOption(string key) => key == "mana" ? "mana" : "mana-" + key;
		var effect = new XElement("Effect", new XAttribute("type", "itemempower"), new XAttribute("version", 1),
			Option("strength", EquippedItemBonusKind.Trait, strength, 0, 0, 0, 1, 2, 3, 4),
			Option("agility", EquippedItemBonusKind.Trait, agility, 0, 0, 0, 1, 2, 3, 4),
			Option("wisdom", EquippedItemBonusKind.Trait, wisdom, 0, 0, 0, 1, 1, 2, 3),
			Option("endurance", EquippedItemBonusKind.Trait, endurance, 0, 0, 1, 2, 3, 4, 5),
			Option("health", EquippedItemBonusKind.Health, 0, 1, 1, 2, 2, 3, 6, 8),
			Option("stun", EquippedItemBonusKind.Stun, 0, 1, 1, 2, 2, 3, 6, 8),
			Option("stamina", EquippedItemBonusKind.Stamina, 0, 1, 2, 4, 8, 14, 18, 20),
			reserves.OrderBy(x => x.Key, StringComparer.Ordinal).Select(x =>
				Option(ResourceOption(x.Key), EquippedItemBonusKind.MagicResource, x.Value, 1, 2, 4, 6, 8, 10, 12)));
		return new("arm.capability.fire", new ArmageddonUtilitySpellContent(Key, "Kindle Enchantment",
			"Enchant one carried item with a selected attribute or resource-capacity bonus for 60 seconds per grade. " +
			"The bonus applies while worn or wielded. Choose strength, agility, wisdom, endurance, health, stun, stamina, or " +
			string.Join(", ", reserves.Keys.Order(StringComparer.Ordinal).Select(ResourceOption)) +
			". Zero source bonuses are unavailable at that grade. Raising capacity does not heal wounds or refill current reserves; " +
			"combat benefits flow through the selected native attribute. Identical spell parents are exclusive.",
			"60*grade", 33, "$0 shape|shapes an enchantment around $1.", (resource, cost, filter) =>
			{
				ArmageddonPrimitiveSpellBindings.RequirePositive(resource, nameof(resource));
				ArmageddonPrimitiveSpellBindings.RequirePositive(cost, nameof(cost));
				if (filter < 0) throw new ArgumentOutOfRangeException(nameof(filter));
				var definition = ArmageddonUtilitySpellContent.Definition(Key, "itemchoice", resource, cost, filter, 40, 33, new XElement(effect));
				return definition;
			}, "return true", ProgVariableTypes.Item));
	}
}
