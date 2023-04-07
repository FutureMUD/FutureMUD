using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.Magic.Powers;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Concrete;

public class MagicChoking : ConcentrationConsumingEffect, IMagicEffect, ICheckBonusEffect
{
	public ICharacter CharacterTarget { get; }
	public BeingMagicChoked ChildEffect { get; }

	public MagicChoking(ICharacter owner, ChokePower power, ICharacter target) : base(owner, power.School,
		power.ConcentrationPointsToSustain)
	{
		Power = power;
		CharacterTarget = target;
		ChildEffect = new BeingMagicChoked(target, this);
		target.AddEffect(ChildEffect);
		Login();
	}

	protected override void RegisterEvents()
	{
		base.RegisterEvents();
		Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat += DoSustainCostsTick;
		CharacterOwner.OnStateChanged += CharacterOwner_OnStateChanged;
		CharacterOwner.OnDeath += CharacterOwner_OnNoLongerValid;
		CharacterOwner.OnQuit += CharacterOwner_OnNoLongerValid;
		CharacterOwner.OnLocationChanged += CharacterOwner_OnLocationChanged;
	}

	public override bool Applies(object target)
	{
		if (target is IMagicEffect me)
		{
			return Power == me.PowerOrigin;
		}

		return base.Applies(target);
	}

	public void CharacterOwner_OnLocationChanged(Form.Shape.ILocateable locatable, Construction.Boundary.ICellExit exit)
	{
		if (!Power.TargetIsInRange(CharacterOwner, CharacterTarget, Power.PowerDistance))
		{
			CharacterOwner.RemoveEffect(this, true);
		}
	}

	private void CharacterOwner_OnNoLongerValid(IPerceivable owner)
	{
		CharacterOwner.RemoveEffect(this, true);
	}

	private void CharacterOwner_OnStateChanged(IPerceivable owner)
	{
		if (!CharacterState.Conscious.HasFlag(CharacterOwner.State))
		{
			CharacterOwner.RemoveEffect(this, true);
		}
	}

	public override void ReleaseEvents()
	{
		base.ReleaseEvents();
		Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat -= DoSustainCostsTick;
		CharacterOwner.OnStateChanged -= CharacterOwner_OnStateChanged;
		CharacterOwner.OnDeath -= CharacterOwner_OnNoLongerValid;
		CharacterOwner.OnQuit -= CharacterOwner_OnNoLongerValid;
		CharacterOwner.OnLocationChanged -= CharacterOwner_OnLocationChanged;
	}

	protected override string SpecificEffectType => "MagicChoking";

	public override string Describe(IPerceiver voyeur)
	{
		return $"Choking {CharacterTarget.HowSeen(voyeur)} with the {Power.Name.Colour(Telnet.Cyan)} power.";
	}

	public ChokePower Power { get; protected set; }

	public IMagicPower PowerOrigin => Power;
	public Difficulty DetectMagicDifficulty => Power.DetectableWithDetectMagic;

	public override void RemovalEffect()
	{
		ReleaseEvents();
		CharacterOwner.OutputHandler.Handle(new EmoteOutput(new Emote(Power.EndPowerEmoteText, CharacterOwner,
			CharacterOwner, CharacterTarget)));
		CharacterTarget.OutputHandler.Handle(
			new EmoteOutput(new Emote(Power.EndPowerEmoteText, CharacterOwner, CharacterOwner, CharacterTarget)),
			OutputRange.Personal);
		CharacterTarget.RemoveEffect(ChildEffect);
	}

	public override void ExpireEffect()
	{
		if (Power.TargetGetsResistanceCheck)
		{
			var check = Gameworld.GetCheck(CheckType.MagicChokePower);
			var results = check.CheckAgainstAllDifficulties(CharacterOwner, Power.SkillCheckDifficulty,
				Power.SkillCheckTrait, CharacterTarget);
			var resistCheck = Gameworld.GetCheck(CheckType.ResistMagicChokePower);
			var resistResults = resistCheck.CheckAgainstAllDifficulties(CharacterTarget, Power.ResistCheckDifficulty,
				null, CharacterOwner);
			var outcome = new OpposedOutcome(results, resistResults, Power.SkillCheckDifficulty,
				Power.ResistCheckDifficulty);
			if (outcome.Outcome != OpposedOutcomeDirection.Proponent)
			{
				CharacterOwner.OutputHandler.Handle(new EmoteOutput(new Emote(Power.TargetResistanceEmoteText,
					CharacterOwner, CharacterOwner, CharacterTarget)));
				CharacterTarget.OutputHandler.Send(new EmoteOutput(new Emote(Power.TargetResistanceEmoteTextTarget,
					CharacterOwner, CharacterOwner, CharacterTarget)));
				CharacterOwner.RemoveEffect(this, true);
				return;
			}

			CharacterOwner.Reschedule(this, Power.ResistCheckInterval);
			return;
		}

		base.ExpireEffect();
	}

	public bool AppliesToCheck(CheckType type)
	{
		return type.IsDefensiveCombatAction() || type.IsOffensiveCombatAction() || type.IsGeneralActivityCheck() ||
		       type.IsTargettedFriendlyCheck() || type.IsTargettedHostileCheck();
	}

	public double CheckBonus => Power.SustainPenalty;

	private void DoSustainCostsTick()
	{
		Power.DoSustainCostsTick(CharacterOwner);
	}
}