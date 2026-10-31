#nullable enable
using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Magic;

namespace MudSharp.GameItems.Interfaces;

/// <summary>Intrinsic item bonuses, evaluated against actual equipment state without changing base traits.</summary>
public interface IEquippedItemBonusProvider : IGameItemComponent
{
	IEnumerable<EquippedItemBonus> BonusesFor(ICharacter actor);
}

public sealed record EquippedItemBonus(EquippedItemBonusKind Kind, long TargetId, double Bonus);
