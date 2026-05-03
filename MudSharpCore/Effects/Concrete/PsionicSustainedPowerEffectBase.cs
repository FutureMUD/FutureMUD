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

public abstract class PsionicSustainedPowerEffectBase<TPower> : ConcentrationConsumingEffect, IMagicEffect, ICheckBonusEffect
	where TPower : SustainedMagicPower
{
	protected PsionicSustainedPowerEffectBase(ICharacter owner, TPower power)
		: base(owner, power.School, power.ConcentrationPointsToSustain)
	{
		Power = power;
		Login();
	}

	protected PsionicSustainedPowerEffectBase(XElement effect, IPerceivable owner)
		: base(effect, owner)
	{
		var root = effect.Element("Effect");
		Power = Gameworld.MagicPowers.Get(long.Parse(root!.Element("Power")!.Value)) as TPower ??
		        throw new ApplicationException(
			        $"The {GetType().Name} effect for {owner.FrameworkItemType} #{owner.Id} referred to an invalid power.");
	}

	public TPower Power { get; protected set; }
	public IMagicPower PowerOrigin => Power;
	public Difficulty DetectMagicDifficulty => Power.DetectableWithDetectMagic;
	public override bool SavingEffect => true;

	protected override XElement SaveDefinition()
	{
		return SaveToXml(new XElement("Power", PowerOrigin.Id));
	}

	protected override void RegisterEvents()
	{
		base.RegisterEvents();
		Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat += DoSustainCostsTick;
	}

	public override void ReleaseEvents()
	{
		base.ReleaseEvents();
		Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat -= DoSustainCostsTick;
	}

	public override void RemovalEffect()
	{
		ReleaseEvents();
	}

	private void DoSustainCostsTick()
	{
		Power.DoSustainCostsTick(CharacterOwner);
	}

	public bool AppliesToCheck(CheckType type)
	{
		return type.IsDefensiveCombatAction() || type.IsOffensiveCombatAction() || type.IsGeneralActivityCheck() ||
		       type.IsTargettedFriendlyCheck() || type.IsTargettedHostileCheck();
	}

	public double CheckBonus => Power.SustainPenalty;
}

