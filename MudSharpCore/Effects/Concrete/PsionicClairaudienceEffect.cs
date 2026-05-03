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

public sealed class PsionicClairaudienceEffect : Effect, IMagicEffect, IRemoteObservationEffect
{
	public PsionicClairaudienceEffect(ICharacter owner, ICharacter observer, ClairaudiencePower power) : base(owner)
	{
		TargetCharacter = owner;
		Observer = observer;
		Power = power;
	}

	public ICharacter TargetCharacter { get; }
	public ICharacter Observer { get; }
	public ClairaudiencePower Power { get; }
	public IMagicPower PowerOrigin => Power;
	public Difficulty DetectMagicDifficulty => Power.DetectableWithDetectMagic;
	public IMagicSchool School => Power.School;

	protected override string SpecificEffectType => "PsionicClairaudience";

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect");
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"{Observer.HowSeen(voyeur)} is hearing through this mind via the {Power.Name.Colour(Power.School.PowerListColour)} power.";
	}

	public void HandleOutput(IOutput output, ILocation location)
	{
		if (!output.Flags.HasFlag(OutputFlags.PurelyAudible))
		{
			return;
		}

		if (location != TargetCharacter.Location || !output.ShouldSee(TargetCharacter))
		{
			return;
		}

		if (!CharacterState.Conscious.HasFlag(Observer.State))
		{
			return;
		}

		var text = output.ParseFor(TargetCharacter);
		if (string.IsNullOrWhiteSpace(text))
		{
			return;
		}

		Observer.OutputHandler.Send(
			$"[{TargetCharacter.HowSeen(Observer, flags: PerceiveIgnoreFlags.IgnoreConsciousness)}] {text}");
	}

	public void HandleOutput(string text, ILocation location)
	{
		// String-only room output has no channel flags, so clairaudience refuses it by policy.
	}
}
