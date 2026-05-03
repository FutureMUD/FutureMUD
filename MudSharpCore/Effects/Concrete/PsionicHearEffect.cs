#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.Magic.Powers;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete;

public sealed class PsionicHearEffect : PsionicSustainedPowerEffectBase<HearPower>, ITelepathyEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("PsionicHear", (effect, owner) => new PsionicHearEffect(effect, owner));
	}

	public PsionicHearEffect(ICharacter owner, HearPower power) : base(owner, power)
	{
	}

	private PsionicHearEffect(XElement effect, IPerceivable owner) : base(effect, owner)
	{
	}

	protected override string SpecificEffectType => "PsionicHear";

	public override string Describe(IPerceiver voyeur)
	{
		return $"Listening to psionic thought and emotion via the {Power.Name.Colour(Power.School.PowerListColour)} power.";
	}

	public override bool Applies(object target)
	{
		if (target is not ICharacter thinker)
		{
			return false;
		}

		return Power.TargetIsInRange(CharacterOwner, thinker, Power.PowerDistance) &&
		       Power.TargetFilter(CharacterOwner, thinker) &&
		       Power.CheckCanHear(CharacterOwner, thinker);
	}

	public bool ShowThinks => Power.ShowThinks;
	public bool ShowFeels => Power.ShowFeels;

	public bool ShowDescription(ICharacter thinker)
	{
		return Power.ShowDescriptionProg.ExecuteBool(CharacterOwner, thinker) != false;
	}

	public bool ShowName(ICharacter thinker)
	{
		return Power.ShowName;
	}

	public bool ShowThinkEmote(ICharacter thinker)
	{
		return Power.ShowEmotes;
	}
}

