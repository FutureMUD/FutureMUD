#nullable enable

using MudSharp.Body;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

public class AdrenalineHeartSupportEffect : Effect, IStablisedOrganFunction
{
	public AdrenalineHeartSupportEffect(IBody owner, IOrganProto organ, double floor, ExertionLevel exertionCap)
		: base(owner)
	{
		Organ = organ;
		Floor = floor;
		ExertionCap = exertionCap;
	}

	protected override string SpecificEffectType => "AdrenalineHeartSupport";

	public IOrganProto Organ { get; set; }
	public double Floor { get; set; }
	public ExertionLevel ExertionCap { get; set; }
	public IBodypart Bodypart => Organ;

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"Adrenaline support keeping {Organ.FullDescription()} at a minimum of {Floor.ToStringP2(voyeur)} function.";
	}
}
