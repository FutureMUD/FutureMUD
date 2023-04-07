using System;
using System.Xml.Linq;
using MudSharp.Combat;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Effects.Concrete;

public class Truce : Effect, ICombatTruceEffect
{
	public Truce(IPerceiver owner, IFutureProg applicabilityProg = null) : base(owner, applicabilityProg)
	{
		Combat = owner.Combat;
		Combat.CombatEnds += Combat_CombatEnds;
	}

	public Truce(XElement effect, IPerceivable owner) : base(effect, owner)
	{
	}

	public ICombat Combat { get; set; }

	private void Combat_CombatEnds(object sender, EventArgs e)
	{
		Combat.CombatEnds -= Combat_CombatEnds;
		ExpireEffect();
	}

	#region Overrides of Effect

	public override string Describe(IPerceiver voyeur)
	{
		return $"{Owner.HowSeen(voyeur, true)} is motioning for a truce.";
	}

	protected override string SpecificEffectType { get; } = "Truce";

	#endregion
}