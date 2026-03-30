#nullable enable
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace $rootnamespace$.Concrete;

public class $safeitemrootname$ : Effect, IEffect
{
	public $safeitemrootname$(IPerceivable owner, IFutureProg applicabilityProg = null)
		: base(owner, applicabilityProg)
	{
	}

	protected override string SpecificEffectType => "$safeitemrootname$";

	public override string Describe(IPerceiver voyeur)
	{
		return "An undescribed effect.";
	}
}
