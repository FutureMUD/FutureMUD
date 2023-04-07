using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Effects.Concrete;

public class PauseAI : Effect, IPauseAIEffect
{
	public PauseAI(IPerceivable owner, IFutureProg applicabilityProg = null) : base(owner, applicabilityProg)
	{
	}

	protected PauseAI(XElement effect, IPerceivable owner) : base(effect, owner)
	{
	}

	public override string Describe(IPerceiver voyeur)
	{
		return "All AI Subroutines are paused.";
	}

	protected override string SpecificEffectType => "PauseAI";

	public override bool SavingEffect => true;

	public static void InitialiseEffectType()
	{
		RegisterFactory("PauseAI", (effect, owner) => new PauseAI(effect, owner));
	}
}