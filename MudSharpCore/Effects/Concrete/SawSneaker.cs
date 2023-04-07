using MudSharp.Effects.Interfaces;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

public class SawSneaker : Effect, ISawSneakerEffect
{
	public SawSneaker(IPerceivable owner, IPerceivable sneaker, bool success)
		: base(owner)
	{
		Sneaker = sneaker;
		Success = success;
	}

	protected override string SpecificEffectType => "SawSneaker";
	public bool Success { get; set; }

	public IPerceivable Sneaker { get; set; }

	public override string Describe(IPerceiver voyeur)
	{
		return $"Saw {Sneaker.HowSeen(voyeur, colour: false)} sneaking";
	}

	public override bool Applies(object target)
	{
		return base.Applies(target) && Success && Sneaker == target;
	}
}