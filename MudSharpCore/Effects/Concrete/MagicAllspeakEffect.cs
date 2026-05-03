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

public sealed class MagicAllspeakEffect : PsionicSustainedPowerEffectBase<AllspeakPower>, IComprehendLanguageEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("MagicAllspeak", (effect, owner) => new MagicAllspeakEffect(effect, owner));
	}

	public MagicAllspeakEffect(ICharacter owner, AllspeakPower power) : base(owner, power)
	{
	}

	private MagicAllspeakEffect(XElement effect, IPerceivable owner) : base(effect, owner)
	{
	}

	protected override string SpecificEffectType => "MagicAllspeak";

	public override string Describe(IPerceiver voyeur)
	{
		return $"Comprehending languages via the {Power.Name.Colour(Power.School.PowerListColour)} power.";
	}
}

