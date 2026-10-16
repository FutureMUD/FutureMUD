#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.GameItems;

namespace MudSharp.Magic;

public enum EquippedItemBonusKind { Trait, Health, Stun, Stamina, MagicResource }

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

	public static double BonusFor(ICharacter? actor, EquippedItemBonusKind kind, long targetId = 0) =>
		Effects(actor).Where(effect => effect.BonusKind == kind && effect.TargetId == targetId)
			.Sum(effect => effect.Bonus);
}
