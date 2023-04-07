using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems;

namespace MudSharp.Effects.Concrete;

public class CombatGetItemEffect : CombatEffectBase, ICombatGetItemEffect
{
	public CombatGetItemEffect(ICharacter owner, IGameItem targetItem) : base(owner, owner.Combat)
	{
		TargetItem = targetItem;
	}

	protected override string SpecificEffectType => "CombatGetItemEffect";

	public IGameItem TargetItem { get; }

	public override string Describe(IPerceiver voyeur)
	{
		return $"Wanting to retrieve {TargetItem.HowSeen(voyeur)}, lost in combat.";
	}
}