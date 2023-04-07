using MudSharp.Combat;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems;

namespace MudSharp.Effects.Concrete;

public class CombatNoGetEffect : CombatEffectBase, INoGetEffect
{
	public CombatNoGetEffect(IPerceivable owner, ICombat combat)
		: base(owner, combat)
	{
	}

	protected override string SpecificEffectType => "CombatNoGetEffect";

	public override string Describe(IPerceiver voyeur)
	{
		return $"Item cannot be picked up because it is involved in an ongoing combat.";
	}

	public bool CombatRelated => true;

	public override IEffect NewEffectOnItemMorph(IGameItem oldItem, IGameItem newItem)
	{
		if (oldItem == Owner)
		{
			return new CombatNoGetEffect(newItem, Combat);
		}

		return null;
	}

	public override bool PreventsItemFromMerging(IGameItem effectOwnerItem, IGameItem targetItem)
	{
		return true;
	}
}