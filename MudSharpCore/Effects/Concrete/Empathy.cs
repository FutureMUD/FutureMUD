using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Effects.Concrete;

public class Empathy : Effect, IEffectSubtype, ITelepathyEffect
{
	#region Static Initialisation

	public static void InitialiseEffectType()
	{
		RegisterFactory("Empathy", (effect, owner) => new Empathy(effect, owner));
	}

	#endregion

	#region Constructors

	public Empathy(IPerceivable owner, IFutureProg applicabilityProg = null) : base(owner, applicabilityProg)
	{
	}

	protected Empathy(XElement effect, IPerceivable owner) : base(effect, owner)
	{
	}

	#endregion

	#region Overrides of Effect

	protected override string SpecificEffectType => "Empathy";

	public override string Describe(IPerceiver voyeur)
	{
		return "Attuned to feelings of those in their location.";
	}

	public override bool SavingEffect => true;

	public override bool Applies(object target)
	{
		var location = (target as ILocateable)?.Location;
		return location != null &&
		       (location.ExitsFor(null).Any(x =>
			        x.Destination == Owner.Location || x.Destination.Room == Owner.Location.Room) ||
		        location == Owner.Location || location.Room == Owner.Location.Room);
	}

	#endregion

	#region ITelepathyEffect

	public bool ShowDescription(ICharacter thinker)
	{
		return ((ICharacter)Owner).CanSee(thinker) &&
		       (thinker.Location == Owner.Location || thinker.Location.Room == Owner.Location.Room);
	}

	public bool ShowName(ICharacter thinker)
	{
		return false;
	}

	public bool ShowThinkEmote(ICharacter thinker)
	{
		return thinker.Location == Owner.Location || thinker.Location.Room == Owner.Location.Room;
	}

	public bool ShowThinks => false;
	public bool ShowFeels => true;

	#endregion
}