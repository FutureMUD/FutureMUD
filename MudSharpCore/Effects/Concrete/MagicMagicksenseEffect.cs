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

public sealed class MagicMagicksenseEffect : PsionicSustainedPowerEffectBase<MagicksensePower>
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("MagicMagicksense", (effect, owner) => new MagicMagicksenseEffect(effect, owner));
	}

	public MagicMagicksenseEffect(ICharacter owner, MagicksensePower power) : base(owner, power)
	{
	}

	private MagicMagicksenseEffect(XElement effect, IPerceivable owner) : base(effect, owner)
	{
	}

	protected override string SpecificEffectType => "MagicMagicksense";

	public override PerceptionTypes PerceptionGranting => PerceptionTypes.SenseMagical;

	public override string Describe(IPerceiver voyeur)
	{
		return $"Sensing magical auras via the {Power.Name.Colour(Power.School.PowerListColour)} power.";
	}
}

