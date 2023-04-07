using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

public class FixedCombatMoveType : CombatEffectBase
{
	public IEnumerable<BuiltInCombatMoveType> FixedTypes { get; set; }

	public bool Indefinite { get; set; }

	public FixedCombatMoveType(ICharacter owner, IEnumerable<BuiltInCombatMoveType> fixedtypes, bool indefinite) : base(
		owner, owner.Combat)
	{
		FixedTypes = fixedtypes;
		Indefinite = indefinite;
	}

	protected override string SpecificEffectType => "FixedCombatMoveType";

	public override string Describe(IPerceiver voyeur)
	{
		return $"Endeavouring to use a fixed move type next time in melee.";
	}
}