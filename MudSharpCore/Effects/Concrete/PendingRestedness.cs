using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.Effects.Concrete;

public class PendingRestedness : Effect, IEffectSubtype
{
	public PendingRestedness(IPerceivable owner, IFutureProg applicabilityProg = null) : base(owner, applicabilityProg)
	{
	}

	public PendingRestedness(XElement effect, IPerceivable owner) : base(effect, owner)
	{
	}

	#region Overrides of Effect

	public override string Describe(IPerceiver voyeur)
	{
		return "Soon to be well rested";
	}

	protected override string SpecificEffectType => "PendingRestedness";

	public override void ExpireEffect()
	{
		base.ExpireEffect();
		var ch = (ICharacter)Owner;
		if (ch.Merits.Where(x => x.Applies(ch)).All(x => !(x is INoRestedBonusMerit)))
		{
			var effect = ch.EffectsOfType<WellRested>().FirstOrDefault();
			if (effect == null)
			{
				effect = new WellRested(ch);
				ch.AddEffect(effect);
			}

			ch.RescheduleIfLonger(effect, TimeSpan.FromSeconds(Gameworld.GetStaticDouble("WellRestedDuration")));
			ch.OutputHandler.Send("You feel well rested.");
		}
	}

	#endregion
}