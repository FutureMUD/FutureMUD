
#nullable enable

namespace MudSharp.Effects.Concrete;

public class AstralProjectionAnchorEffect : Effect, IHelplessEffect
{
	public AstralProjectionAnchorEffect(IPerceivable owner, long projectionInstanceId,
		IFutureProg? applicabilityProg = null)
		: base(owner, applicabilityProg)
	{
		ProjectionInstanceId = projectionInstanceId;
	}

	public long ProjectionInstanceId { get; }

	public override string Describe(IPerceiver voyeur)
	{
		return $"{Owner.HowSeen(voyeur, true)} is anchored helplessly while projecting astrally.";
	}

	protected override string SpecificEffectType => "AstralProjectionAnchor";
}
