using MudSharp.Combat;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete;

public class WardBeaten : CombatEffectBase, IWardBeatenEffect
{
	protected WardBeaten(XElement effect, IPerceivable owner) : base(effect, owner)
	{
	}

	public WardBeaten(IPerceivable owner, ICombat combat, IFutureProg applicabilityProg = null)
		: base(owner, combat, applicabilityProg)
	{
	}

	protected override string SpecificEffectType => "WardBeaten";

	public override string Describe(IPerceiver voyeur)
	{
		return "Ward defense has been beaten.";
	}
}