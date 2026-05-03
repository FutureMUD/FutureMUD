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

public sealed class PsionicBabbleEffect : Effect, IMagicEffect, IBabbleSpeechEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("PsionicBabble", (effect, owner) => new PsionicBabbleEffect(effect, owner));
	}

	public PsionicBabbleEffect(ICharacter owner, BabblePower power) : base(owner)
	{
		CharacterOwner = owner;
		Power = power;
	}

	private PsionicBabbleEffect(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		CharacterOwner = (ICharacter)owner;
		var root = effect.Element("Effect");
		Power = Gameworld.MagicPowers.Get(long.Parse(root!.Element("Power")!.Value)) as BabblePower ??
		        throw new ApplicationException(
			        $"The PsionicBabble effect for {owner.FrameworkItemType} #{owner.Id} referred to an invalid babble power.");
	}

	public ICharacter CharacterOwner { get; }
	public BabblePower Power { get; }
	public IMagicPower PowerOrigin => Power;
	public Difficulty DetectMagicDifficulty => Power.DetectableWithDetectMagic;
	public IMagicSchool School => Power.School;
	public override bool SavingEffect => true;

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("ApplicabilityProg", ApplicabilityProg?.Id ?? 0L),
			new XElement("Power", PowerOrigin.Id)
		);
	}

	protected override string SpecificEffectType => "PsionicBabble";

	public override string Describe(IPerceiver voyeur)
	{
		return $"Speech is psionically scrambled by the {Power.Name.Colour(Power.School.PowerListColour)} power.";
	}
}

