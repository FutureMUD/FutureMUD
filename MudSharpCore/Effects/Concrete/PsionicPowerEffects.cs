#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.Magic.Powers;
using MudSharp.NPC;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete;

[Flags]
public enum PsionicHexCheckCategory
{
	None = 0,
	General = 1,
	OffensiveCombat = 2,
	DefensiveCombat = 4,
	TargetedHostile = 8,
	TargetedFriendly = 16,
	All = General | OffensiveCombat | DefensiveCombat | TargetedHostile | TargetedFriendly
}

public sealed class MagicHexEffect : Effect, IMagicEffect, ICheckBonusEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("MagicHex", (effect, owner) => new MagicHexEffect(effect, owner));
	}

	public MagicHexEffect(ICharacter owner, HexPower power, double penalty, PsionicHexCheckCategory categories) :
		base(owner)
	{
		Power = power;
		CheckBonus = -Math.Abs(penalty);
		Categories = categories;
	}

	private MagicHexEffect(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		var root = effect.Element("Effect")!;
		Power = Gameworld.MagicPowers.Get(long.Parse(root.Element("Power")!.Value)) as HexPower ??
		        throw new ApplicationException(
			        $"The MagicHex effect for {owner.FrameworkItemType} #{owner.Id} referred to an invalid power.");
		CheckBonus = double.Parse(root.Element("Penalty")!.Value);
		Categories = Enum.Parse<PsionicHexCheckCategory>(root.Element("Categories")!.Value, true);
	}

	public HexPower Power { get; }
	public PsionicHexCheckCategory Categories { get; }
	public IMagicSchool School => Power.School;
	public IMagicPower PowerOrigin => Power;
	public Difficulty DetectMagicDifficulty => Power.DetectableWithDetectMagic;
	public override bool SavingEffect => true;
	public double CheckBonus { get; }

	protected override string SpecificEffectType => "MagicHex";

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("Power", Power.Id),
			new XElement("Penalty", CheckBonus),
			new XElement("Categories", Categories)
		);
	}

	public bool AppliesToCheck(CheckType type)
	{
		if (Categories.HasFlag(PsionicHexCheckCategory.General) && type.IsGeneralActivityCheck())
		{
			return true;
		}

		if (Categories.HasFlag(PsionicHexCheckCategory.OffensiveCombat) && type.IsOffensiveCombatAction())
		{
			return true;
		}

		if (Categories.HasFlag(PsionicHexCheckCategory.DefensiveCombat) && type.IsDefensiveCombatAction())
		{
			return true;
		}

		if (Categories.HasFlag(PsionicHexCheckCategory.TargetedHostile) && type.IsTargettedHostileCheck())
		{
			return true;
		}

		return Categories.HasFlag(PsionicHexCheckCategory.TargetedFriendly) && type.IsTargettedFriendlyCheck();
	}

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"Hexed by {Power.Name.Colour(Power.School.PowerListColour)} for {CheckBonus.ToBonusString(voyeur)} on {Categories.DescribeEnum().ColourValue()} checks.";
	}
}

public sealed class DangerSenseDefensiveEdge : Effect, ICheckBonusEffect
{
	public DangerSenseDefensiveEdge(ICharacter owner, DangerSensePower power, double bonus) : base(owner)
	{
		Power = power;
		CheckBonus = Math.Abs(bonus);
	}

	public DangerSensePower Power { get; }
	public double CheckBonus { get; }
	protected override string SpecificEffectType => "DangerSenseDefensiveEdge";

	public bool AppliesToCheck(CheckType type)
	{
		return type.IsDefensiveCombatAction();
	}

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"Forewarned by {Power.Name.Colour(Power.School.PowerListColour)}, receiving {CheckBonus.ToBonusString(voyeur)} to defensive combat checks.";
	}
}

public sealed class MagicDangerSenseEffect : ConcentrationConsumingEffect, IMagicEffect
{
	private DateTime _nextWarning = DateTime.MinValue;

	public static void InitialiseEffectType()
	{
		RegisterFactory("MagicDangerSense", (effect, owner) => new MagicDangerSenseEffect(effect, owner));
	}

	public MagicDangerSenseEffect(ICharacter owner, DangerSensePower power) :
		base(owner, power.School, power.ConcentrationPointsToSustain)
	{
		Power = power;
		Login();
	}

	private MagicDangerSenseEffect(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		var root = effect.Element("Effect")!;
		Power = Gameworld.MagicPowers.Get(long.Parse(root.Element("Power")!.Value)) as DangerSensePower ??
		        throw new ApplicationException(
			        $"The MagicDangerSense effect for {owner.FrameworkItemType} #{owner.Id} referred to an invalid power.");
	}

	public DangerSensePower Power { get; }
	public IMagicPower PowerOrigin => Power;
	public Difficulty DetectMagicDifficulty => Power.DetectableWithDetectMagic;
	public override bool SavingEffect => true;
	protected override string SpecificEffectType => "MagicDangerSense";

	protected override XElement SaveDefinition()
	{
		return SaveToXml(new XElement("Power", Power.Id));
	}

	protected override void RegisterEvents()
	{
		base.RegisterEvents();
		Gameworld.HeartbeatManager.FuzzyTenSecondHeartbeat += TenSecondHeartbeat;
	}

	public override void ReleaseEvents()
	{
		base.ReleaseEvents();
		Gameworld.HeartbeatManager.FuzzyTenSecondHeartbeat -= TenSecondHeartbeat;
	}

	public override void RemovalEffect()
	{
		ReleaseEvents();
		CharacterOwner.RemoveAllEffects(x => x is DangerSenseDefensiveEdge edge && edge.Power == Power);
	}

	private void TenSecondHeartbeat()
	{
		Power.DoSustainCostsTick(CharacterOwner, Power.SustainTickMultiplier);
		if (DateTime.UtcNow >= _nextWarning && Power.CheckNearbyThreats(CharacterOwner))
		{
			_nextWarning = DateTime.UtcNow + Power.ThreatWarningInterval;
		}

		Power.RefreshDefensiveEdge(CharacterOwner);
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Sustaining danger sense via {Power.Name.Colour(Power.School.PowerListColour)}.";
	}
}

public sealed class PsionicSensitivityEffect : PsionicSustainedPowerEffectBase<SensitivityPower>
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("PsionicSensitivity", (effect, owner) => new PsionicSensitivityEffect(effect, owner));
	}

	public PsionicSensitivityEffect(ICharacter owner, SensitivityPower power) : base(owner, power)
	{
	}

	private PsionicSensitivityEffect(XElement effect, IPerceivable owner) : base(effect, owner)
	{
	}

	protected override string SpecificEffectType => "PsionicSensitivity";
	public override PerceptionTypes PerceptionGranting => Power.GrantedPerceptions;

	public void NotifyActivity(PsionicActivity activity)
	{
		if (!Power.ActivityKinds.Contains(activity.Kind))
		{
			return;
		}

		if (!Power.NotifySelf && activity.Source == CharacterOwner)
		{
			return;
		}

		if (activity.Source.Location is null || CharacterOwner.Location is null)
		{
			return;
		}

		if (!CharacterOwner.Location.CellsInVicinity(Power.ActivityRange, true, true).Contains(activity.Source.Location))
		{
			return;
		}

		var outcome = Gameworld.GetCheck(CheckType.SensitivityPower)
		                       .Check(CharacterOwner, Power.ActivityDifficulty, Power.SkillCheckTrait, activity.Source);
		if (outcome < Power.MinimumSuccessThreshold)
		{
			return;
		}

		CharacterOwner.OutputHandler.Send(Power.FormatActivityEcho(CharacterOwner, activity));
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Sustaining psychic sensitivity via {Power.Name.Colour(Power.School.PowerListColour)}.";
	}
}
