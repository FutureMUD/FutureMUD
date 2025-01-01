using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.Magic.Powers;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete;

public class MagicTelepathyEffect : ConcentrationConsumingEffect, IMagicEffect, ICheckBonusEffect, ITelepathyEffect
{
	#region Static Initialisation

	public static void InitialiseEffectType()
	{
		RegisterFactory("MagicTelepathy", (effect, owner) => new MagicTelepathyEffect(effect, owner));
	}

	#endregion

	public MagicTelepathyEffect(ICharacter owner, TelepathyPower power) : base(owner, power.School,
		power.ConcentrationPointsToSustain)
	{
		TelepathyPower = power;
		Login();
	}

	protected MagicTelepathyEffect(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		var root = effect.Element("Effect");
		TelepathyPower = (TelepathyPower)Gameworld.MagicPowers.Get(long.Parse(root.Element("Power").Value));
		Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat += DoSustainCostsTick;
	}

	public override bool SavingEffect => true;

	protected override XElement SaveDefinition()
	{
		return SaveToXml(new XElement("Power", PowerOrigin.Id));
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Magic Telepathy from {TelepathyPower.Name} power.";
	}

	protected override string SpecificEffectType => "MagicTelepathy";

	public override bool Applies(object target)
	{
		var tch = (ICharacter)target;
		switch (TelepathyPower.PowerDistance)
		{
			case MagicPowerDistance.AnyConnectedMind:
				if (CharacterOwner.EffectsOfType<ConnectMindEffect>().All(x => x.TargetCharacter != tch))
				{
					return false;
				}

				break;
			case MagicPowerDistance.SameLocationOnly:
				if (CharacterOwner.Location != tch.Location && CharacterOwner.Location.Room != tch.Location.Room)
				{
					return false;
				}

				break;
			case MagicPowerDistance.AdjacentLocationsOnly:
				if (CharacterOwner.Location != tch.Location && CharacterOwner.Location.Room != tch.Location.Room && tch
					    .Location.ExitsFor(null).Any(x =>
						    x.Destination == CharacterOwner.Location ||
						    x.Destination.Room == CharacterOwner.Location.Room))
				{
					return false;
				}

				break;
			case MagicPowerDistance.SameAreaOnly:
			// TODO - currently just treat the same as SameZoneOnly
			case MagicPowerDistance.SameZoneOnly:
				if (CharacterOwner.Location.Zone != tch.Location.Zone)
				{
					return false;
				}

				break;
			case MagicPowerDistance.SameShardOnly:
				if (CharacterOwner.Location.Shard != tch.Location.Shard)
				{
					return false;
				}

				break;
			case MagicPowerDistance.SamePlaneOnly:
				// TODO - currently just always true
				break;
		}

		var check = Gameworld.GetCheck(CheckType.MagicTelepathyCheck);
		var result = check.Check(CharacterOwner, TelepathyPower.SkillCheckDifficulty, TelepathyPower.SkillCheckTrait,
			tch);
		return result.Outcome >= TelepathyPower.MinimumSuccessThreshold;
	}

	public TelepathyPower TelepathyPower { get; protected set; }
	public IMagicPower PowerOrigin => TelepathyPower;
	public Difficulty DetectMagicDifficulty => TelepathyPower.DetectableWithDetectMagic;

	public bool AppliesToCheck(CheckType type)
	{
		return type.IsDefensiveCombatAction() || type.IsOffensiveCombatAction() || type.IsGeneralActivityCheck() ||
		       type.IsTargettedFriendlyCheck() || type.IsTargettedHostileCheck();
	}

	public double CheckBonus => TelepathyPower.SustainPenalty;
	public bool ShowThinks => TelepathyPower.ShowThinks;
	public bool ShowFeels => TelepathyPower.ShowFeels;

	public bool ShowDescription(ICharacter thinker)
	{
		return TelepathyPower.ShowThinkerDescription.ExecuteBool(CharacterOwner, thinker);
	}

	public bool ShowName(ICharacter thinker)
	{
		return false;
	}

	public bool ShowThinkEmote(ICharacter thinker)
	{
		return TelepathyPower.ShowThinkEmote;
	}

	#region Overrides of Effect

	/// <summary>Fires when an effect is removed, including a matured scheduled effect</summary>
	public override void RemovalEffect()
	{
		ReleaseEvents();
	}

	public override void Login()
	{
		RegisterEvents();
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

	private void DoSustainCostsTick()
	{
		TelepathyPower.DoSustainCostsTick(CharacterOwner);
	}

	#endregion
}