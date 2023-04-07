using MudSharp.Body;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Effects.Concrete;

public class CPRTarget : Effect, IEffectSubtype
{
	public CPRTarget(IBody owner, IFutureProg applicabilityProg = null) : base(owner, applicabilityProg)
	{
	}

	protected override string SpecificEffectType => "CPRTarget";

	public override string Describe(IPerceiver voyeur)
	{
		return $"Having CPR performed on them";
	}
}