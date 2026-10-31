#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.GameItems.Interfaces;
using MudSharp.Magic;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private void InstallItemAffects()
	{
		var attributes = _db.TraitDefinitions.Where(x => x.Type == 1).ToArray();
		long Attribute(params string[] names) => names.Select(name => attributes.SingleOrDefault(x =>
			x.Name.Equals(name, StringComparison.OrdinalIgnoreCase))).FirstOrDefault(x => x is not null)?.Id
			?? throw new InvalidOperationException($"Item affects require a native attribute matching {string.Join("/", names)}.");
		long Trait(int location) => location switch
		{
			1 => Attribute("Strength", "Upper Body Strength"),
			2 => Attribute("Agility", "Dexterity"),
			3 => Attribute("Wisdom", "Willpower", "Intelligence", "Mind"),
			5 => Attribute("Endurance", "Constitution", "Hardiness", "Resilience", "Stamina"),
			25 or 26 or 27 or 28 or 348 => OwnedBehaviourId("arm.skill." +
				(location switch { 25 => "sneak", 26 => "hide", 27 => "listen", 28 => "climb", _ => "skinning" }), nameof(TraitDefinition))
				?? throw new InvalidOperationException($"Item affect {location} requires its installed native skill."),
			_ => throw new ArgumentOutOfRangeException(nameof(location))
		};
		var excluded = _wearProfiles.GetValueOrDefault(1 << 9, []).Select(x => x.Id).ToArray();
		foreach (var (id, effective) in _effectiveItems)
		{
			var source = effective.Source;
			var rows = source.Affects.Where(x => x.Length >= 2 && x[0] != 0 && x[1] != 0).ToArray();
			if (rows.Length == 0) continue;
			var bonuses = ResolveItemAffects(rows, Trait, () =>
				OwnedBehaviourId(ArmageddonProgressionInstaller.ManaResourceKey, nameof(MagicResource))
				?? throw new InvalidOperationException("Item affects require the installed common Mana resource."));
			var proto = _db.GameItemProtos.Local.Single(x => x.Id == id);
			if (bonuses.Count > 0)
			{
				var component = Component("itemaffects." + id, "Equipment Bonuses", new XElement("Definition",
					new XElement("Worn", true), new XElement("Wielded", true), new XElement("Held", true),
					new XElement("ExcludedProfiles", excluded.Select(x => new XElement("Profile", x))),
					bonuses.Select(x => new XElement("Bonus", new XAttribute("kind", x.Kind),
						new XAttribute("target", x.TargetId), new XAttribute("value", x.Bonus)))));
				AttachDeliveryComponent(proto, component, new XElement("Definition"));
			}
			var pending = rows.Where(x => x[0] is not (1 or 2 or 3 or 5 or 12 or 13 or 14 or 17 or 18 or 24 or 25 or 26 or 27 or 28 or 29 or 348)).ToArray();
			Record("itemaffects." + id, nameof(GameItemProto), id, proto.RevisionNumber, new
			{
				SourceVnum = source.Vnum, Affects = rows, ActiveBonuses = bonuses, UnsupportedMappings = pending,
				Disposition = "Additive equipped modifiers; back profile and belt/sheath storage excluded. Physical hand holding qualifies; abstract inventory carrying does not. Native HP-capacity semantics, ordinary equipped skill bonuses, flat native combat/spell resistance bonuses and AC/10 physical damage/stun absorption are approved adaptations."
			});
			if (pending.Length > 0) Adapt("unsupported item affect", source.Vnum,
				$"Effective prototype {id} retains unresolved affect locations {string.Join(", ", pending.Select(x => x[0]).Distinct())} as provenance; no unsupported behaviour is inferred.");
		}
		Flush();
	}

	internal static IReadOnlyList<EquippedItemBonus> ResolveItemAffects(IEnumerable<int[]> rows,
		Func<int, long> trait, Func<long> mana)
	{
		var bonuses = new List<EquippedItemBonus>();
		foreach (var row in rows.Where(x => x.Length >= 2 && x[1] != 0))
		{
			switch (row[0])
			{
				case 1: case 2: case 3: case 5: case 25: case 26: case 27: case 28: case 348:
					bonuses.Add(new(EquippedItemBonusKind.Trait, trait(row[0]), row[1])); break;
				case 12: bonuses.Add(new(EquippedItemBonusKind.MagicResource, mana(), row[1])); break;
				case 13: bonuses.Add(new(EquippedItemBonusKind.Health, 0, row[1])); break;
				case 17: bonuses.Add(new(EquippedItemBonusKind.PhysicalAbsorption, 0, row[1] / 10.0)); break;
				case 18: bonuses.Add(new(EquippedItemBonusKind.CombatAttack, 0, row[1])); break;
				case 24: bonuses.Add(new(EquippedItemBonusKind.SpellResistance, 0, row[1])); break;
				case 29: bonuses.Add(new(EquippedItemBonusKind.CombatDefence, 0, row[1])); break;
				case 14: bonuses.Add(new(EquippedItemBonusKind.Stamina, 0, row[1])); break;
			}
		}
		return bonuses.GroupBy(x => (x.Kind, x.TargetId)).Select(group =>
			new EquippedItemBonus(group.Key.Kind, group.Key.TargetId, group.Sum(x => x.Bonus)))
			.Where(x => x.Bonus != 0).ToArray();
	}
}
