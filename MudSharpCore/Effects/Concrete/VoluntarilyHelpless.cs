using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Effects.Concrete;

public class VoluntarilyHelpless : Effect, IHelplessEffect
{
	public VoluntarilyHelpless(IPerceivable owner, IFutureProg applicabilityProg = null) : base(owner,
		applicabilityProg)
	{
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"{Owner.HowSeen(voyeur, true)} is voluntarily helpless, and will submit to their fate.";
	}

	protected override string SpecificEffectType => "VoluntarilyHelpless";
}