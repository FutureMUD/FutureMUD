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

public sealed class MagicClairaudienceConcentrationEffect : PsionicSustainedPowerEffectBase<ClairaudiencePower>
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("MagicClairaudienceConcentration",
			(effect, owner) => new MagicClairaudienceConcentrationEffect(effect, owner));
	}

	public MagicClairaudienceConcentrationEffect(ICharacter owner, ClairaudiencePower power, ICharacter target)
		: base(owner, power)
	{
		TargetCharacter = target;
		EnsureChildEffect();
	}

	private MagicClairaudienceConcentrationEffect(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		var root = effect.Element("Effect");
		TargetCharacter = Gameworld.TryGetCharacter(long.Parse(root!.Element("Target")!.Value), true);
	}

	public ICharacter? TargetCharacter { get; private set; }

	protected override XElement SaveDefinition()
	{
		return SaveToXml(
			new XElement("Power", PowerOrigin.Id),
			new XElement("Target", TargetCharacter?.Id ?? 0L)
		);
	}

	protected override string SpecificEffectType => "MagicClairaudienceConcentration";

	public override void Login()
	{
		base.Login();
		EnsureChildEffect();
	}

	public override void RemovalEffect()
	{
		base.RemovalEffect();
		if (TargetCharacter is null)
		{
			return;
		}

		foreach (var effect in TargetCharacter.EffectsOfType<PsionicClairaudienceEffect>()
		                                      .Where(x => x.Observer == CharacterOwner && x.Power == Power)
		                                      .ToList())
		{
			TargetCharacter.RemoveEffect(effect, true);
		}
	}

	public override string Describe(IPerceiver voyeur)
	{
		return TargetCharacter is null
			? $"Clairaudience from the {Power.Name.Colour(Power.School.PowerListColour)} power."
			: $"Clairaudience through {TargetCharacter.HowSeen(voyeur)} via the {Power.Name.Colour(Power.School.PowerListColour)} power.";
	}

	private void EnsureChildEffect()
	{
		if (TargetCharacter is null)
		{
			return;
		}

		if (TargetCharacter.EffectsOfType<PsionicClairaudienceEffect>()
		                   .Any(x => x.Observer == CharacterOwner && x.Power == Power))
		{
			return;
		}

		TargetCharacter.AddEffect(new PsionicClairaudienceEffect(TargetCharacter, CharacterOwner, Power));
	}
}

