using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete;

public class FixedInPlaceEffect : Effect, INoGetEffect, IZeroGravityAnchorEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("FixedInPlace", (effect, owner) => new FixedInPlaceEffect(effect, owner));
	}

	public FixedInPlaceEffect(IPerceivable owner, IFutureProg applicabilityProg = null) : base(owner, applicabilityProg)
	{
	}

	protected FixedInPlaceEffect(XElement effect, IPerceivable owner) : base(effect, owner)
	{
	}

	protected override string SpecificEffectType => "FixedInPlace";

	public override bool SavingEffect => true;

	public bool CombatRelated => false;

	public bool AllowsZeroGravityPushOff => true;

	public override string Describe(IPerceiver voyeur)
	{
		return "Fixed in place and usable as a zero-gravity anchor.";
	}
}
