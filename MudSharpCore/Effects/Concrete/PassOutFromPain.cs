using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Health;

namespace MudSharp.Effects.Concrete;

public class PassOutFromPain : Effect, ILossOfConsciousnessEffect
{
	public PassOutFromPain(IPerceivable owner, IFutureProg applicabilityProg = null) : base(owner, applicabilityProg)
	{
	}

	#region Overrides of Effect

	public override void RemovalEffect()
	{
		base.RemovalEffect();
		((IBody)Owner).CheckHealthStatus();
	}


	public override string Describe(IPerceiver voyeur)
	{
		return "Passed out from pain";
	}

	protected override string SpecificEffectType => "PassOutFromPain";

	#endregion

	#region Implementation of ILossOfConsciousnessEffect

	public HealthTickResult UnconType => HealthTickResult.PassOut;

	#endregion
}