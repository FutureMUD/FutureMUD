using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete;

public class PeacefulEffect : Effect, IPeacefulEffect
{
	public PeacefulEffect(IPerceivable owner, IFutureProg applicabilityProg = null) : base(owner, applicabilityProg)
	{
	}

	private PeacefulEffect(XElement effect, IPerceivable owner) : base(effect, owner)
	{
	}

	#region Overrides of Effect

	public override string Describe(IPerceiver voyeur)
	{
		return "This location is peaceful, and no fighting can occur here.";
	}

	protected override string SpecificEffectType { get; } = "Peaceful";

	public override bool SavingEffect { get; } = true;

	public static void InitialiseEffectType()
	{
		RegisterFactory("Peaceful", (effect, owner) => new PeacefulEffect(effect, owner));
	}

	#endregion
}