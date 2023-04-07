using System;
using System.Xml.Linq;
using MudSharp.Combat;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Effects.Concrete;

public abstract class CombatEffectBase : Effect, ICombatEffect
{
	protected CombatEffectBase(IPerceivable owner, ICombat combat, IFutureProg applicabilityProg = null)
		: base(owner, applicabilityProg)
	{
		Combat = combat;
		if (Combat != null)
		{
			Combat.CombatEnds += CombatOnCombatEnds;
			Combat.CombatMerged += CombatOnCombatMerged;
		}
	}

	protected CombatEffectBase(XElement effect, IPerceivable owner) : base(effect, owner)
	{
	}

	#region Overrides of Effect

	/// <summary>Fires when an effect is removed, including a matured scheduled effect</summary>
	public override void RemovalEffect()
	{
		if (Combat != null)
		{
			Combat.CombatEnds -= CombatOnCombatEnds;
			Combat.CombatMerged -= CombatOnCombatMerged;
		}
	}

	#endregion

	protected ICombat Combat { get; set; }

	private void CombatOnCombatMerged(ICombat obsoleteCombat, ICombat newCombat)
	{
		if (obsoleteCombat != null)
		{
			obsoleteCombat.CombatEnds -= CombatOnCombatEnds;
			obsoleteCombat.CombatMerged -= CombatOnCombatMerged;
		}

		Combat = newCombat;
		if (newCombat != null)
		{
			newCombat.CombatEnds += CombatOnCombatEnds;
			newCombat.CombatMerged += CombatOnCombatMerged;
		}
	}

	private void CombatOnCombatEnds(object sender, EventArgs eventArgs)
	{
		if (Combat != null)
		{
			Combat.CombatEnds -= CombatOnCombatEnds;
			Combat.CombatMerged -= CombatOnCombatMerged;
		}

		ExpireEffect();
	}
}