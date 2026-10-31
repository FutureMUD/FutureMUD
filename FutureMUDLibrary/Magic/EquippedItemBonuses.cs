#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Magic;

public enum EquippedItemBonusKind { Trait, Health, Stun, Stamina, MagicResource, CombatAttack, CombatDefence, SpellResistance, PhysicalAbsorption }

/// <summary>One resolved, retained bonus. Capacity bonuses never change current reserves or wounds.</summary>
public interface IEquippedItemBonusEffect : IEffect
{
	EquippedItemBonusKind BonusKind { get; }
	long TargetId { get; }
	double Bonus { get; }
}

public static class EquippedItemBonuses
{
	public static IEnumerable<IEquippedItemBonusEffect> Effects(ICharacter? actor)
	{
		if (actor?.Body is not { } body) return [];
		return (body.ExternalItems ?? [])
			.Distinct<IGameItem>(ReferenceEqualityComparer.Instance)
			.Where(item => !item.Deleted && !item.Destroyed && ReferenceEquals(item.InInventoryOf, body) &&
				((body.WornItems ?? []).Any(worn => ReferenceEquals(worn, item)) ||
				 (body.WieldedItems ?? []).Any(wielded => ReferenceEquals(wielded, item))))
			.SelectMany(item => item.EffectsOfType<IEquippedItemBonusEffect>()
				.Where(effect => ReferenceEquals(effect.Owner, item) && effect.Applies(actor)));
	}

	public static IEnumerable<EquippedItemBonus> Bonuses(ICharacter? actor)
	{
		foreach (var effect in Effects(actor)) yield return new(effect.BonusKind, effect.TargetId, effect.Bonus);
		if (actor?.Body is not { } body) yield break;
		foreach (var item in (body.ExternalItems ?? []).Distinct<IGameItem>(ReferenceEqualityComparer.Instance))
		{
			if (item.Deleted || item.Destroyed || !ReferenceEquals(item.InInventoryOf, body)) continue;
			foreach (var provider in item.GetItemTypes<IEquippedItemBonusProvider>())
			foreach (var bonus in provider.BonusesFor(actor)) yield return bonus;
		}
	}

	public static double BonusFor(ICharacter? actor, EquippedItemBonusKind kind, long targetId = 0) =>
		Bonuses(actor).Where(effect => effect.Kind == kind && effect.TargetId == targetId)
			.Sum(effect => effect.Bonus);
}
