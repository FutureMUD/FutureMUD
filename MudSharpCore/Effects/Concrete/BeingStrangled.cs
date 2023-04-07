using MudSharp.Effects.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Character;

namespace MudSharp.Effects.Concrete;

public class BeingStrangled : Effect, IBodypartIneffectiveEffect, INoQuitEffect, ICombatEffect
{
	public BeingStrangled(IPerceivable owner, ICharacter strangler, IFutureProg applicabilityProg = null) : base(owner,
		applicabilityProg)
	{
		Strangler = strangler;
	}

	public ICharacter Strangler { get; set; }

	public IBodypart Bodypart { get; set; }

	public override void RemovalEffect()
	{
		base.RemovalEffect();
		(Owner as IBody)?.CheckHealthStatus();
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"{Bodypart.FullDescription()} is being choked.";
	}

	protected override string SpecificEffectType => "ResidualChoke";

	public string NoQuitReason => "You cannot quit so soon after having been choked.";
}