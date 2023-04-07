using MudSharp.Combat;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Effects.Concrete;

public class IdleCombatant : CombatEffectBase
{
	public IdleCombatant(IPerceiver owner, ICombat combat, IFutureProg applicabilityProg = null) : base(owner, combat,
		applicabilityProg)
	{
		owner.OnLeaveCombat += Owner_OnLeaveCombat;
	}

	private void Owner_OnLeaveCombat(IPerceivable owner)
	{
		((IPerceiver)owner).OnLeaveCombat -= Owner_OnLeaveCombat;
		ExpireEffect();
	}

	protected override string SpecificEffectType => "IdleCombatant";

	public override string Describe(IPerceiver voyeur)
	{
		return $"Combatant is idle; didn't take any action at last combat tick.";
	}
}