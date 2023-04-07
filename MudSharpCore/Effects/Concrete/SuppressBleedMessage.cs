using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Effects.Concrete;

public class SuppressBleedMessage : Effect, ISuppressBleedMessage
{
	public SuppressBleedMessage(IPerceivable owner, IFutureProg applicabilityProg) : base(owner, applicabilityProg)
	{
	}

	protected override string SpecificEffectType { get; } = "SuppressBleedMessage";

	public override string Describe(IPerceiver voyeur)
	{
		return "Suppressing Bleeding Message";
	}
}