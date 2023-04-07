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
using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete;

public class MindBarrierEffect : ConcentrationConsumingEffect, IMagicEffect, ICheckBonusEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("MindBarrier", (effect, owner) => new MindBarrierEffect(effect, owner));
	}

	public MindBarrierEffect(ICharacter owner, double bonus, MindBarrierPower power) : base(owner, power.School,
		power.ConcentrationPointsToSustain)
	{
		Bonus = bonus;
		MindPower = power;
	}

	protected MindBarrierEffect(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		var root = effect.Element("Effect");
		MindPower = (MindBarrierPower)Gameworld.MagicPowers.Get(long.Parse(root.Element("Power").Value));
		Bonus = double.Parse(root.Element("Bonus").Value);
	}

	protected override XElement SaveDefinition()
	{
		return SaveToXml(new XElement("Power", PowerOrigin.Id), new XElement("Bonus", Bonus));
	}

	public override bool SavingEffect => true;

	protected override string SpecificEffectType => "MindBarrier";

	public double Bonus { get; protected set; }

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"Has a mind barrier up with a bonus of {Bonus.ToString("N2", voyeur).ColourValue()} via {MindPower.Name.Colour(School.PowerListColour)}.";
	}
	protected override bool EffectCanPersistOnLogout => true;
	protected override void RegisterEvents()
	{
		base.RegisterEvents();
		CharacterOwner.OnStateChanged += CharacterOwner_OnStateChanged;
		CharacterOwner.OnDeath += CharacterOwner_OnNoLongerValid;
		Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat += DoSustainCostsTick;
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
		CharacterOwner.OnStateChanged -= CharacterOwner_OnStateChanged;
		CharacterOwner.OnDeath -= CharacterOwner_OnNoLongerValid;
		Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat -= DoSustainCostsTick;
	}

	public override void RemovalEffect()
	{
		ReleaseEvents();
		CharacterOwner.OutputHandler.Send(new EmoteOutput(new Emote(MindPower.EmoteForEndSelf, CharacterOwner, CharacterOwner)));
		if (!string.IsNullOrWhiteSpace(MindPower.EmoteForEnd))
		{
			CharacterOwner.OutputHandler.Handle(new EmoteOutput(new Emote(MindPower.EmoteForEnd, CharacterOwner, CharacterOwner), flags: PerceptionEngine.OutputFlags.SuppressSource), OutputRange.Local);
		}
	}

	private void DoSustainCostsTick()
	{
		MindPower.DoSustainCostsTick(CharacterOwner);
	}

	public MindBarrierPower MindPower { get; set; }
	public IMagicPower PowerOrigin => MindPower;
	public Difficulty DetectMagicDifficulty => MindPower.DetectableWithDetectMagic;
	public bool AppliesToCheck(CheckType type)
	{
		return type.IsDefensiveCombatAction() || type.IsOffensiveCombatAction() || type.IsGeneralActivityCheck() ||
			   type.IsTargettedFriendlyCheck() || type.IsTargettedHostileCheck();
	}
	public double CheckBonus => MindPower.SustainPenalty;

	public override bool Applies(object target)
	{
		if (target is ICharacter ch)
		{
			if (MindPower.PermitAllies && CharacterOwner.IsAlly(ch))
			{
				return false;
			}

			if (MindPower.PermitTrustedAllies && CharacterOwner.IsTrustedAlly(ch))
			{
				return false;
			}

			if ((MindPower.AppliesToCharacterProg?.Execute<bool?>(ch, CharacterOwner)) != false)
			{
				return false;
			}

			return true;
		}
		return base.Applies(target);
	}

	public void Shatter(ICharacter who)
	{
		CharacterOwner.OutputHandler.Send(new EmoteOutput(new Emote(MindPower.OvercomeEmoteSelf, who, who, CharacterOwner)));
		if (!string.IsNullOrEmpty(MindPower.OvercomeEmoteTarget))
		{
			who.OutputHandler.Send(new EmoteOutput(new Emote(MindPower.OvercomeEmoteTarget, who, who, CharacterOwner)));
		}

		ReleaseEvents();
		CharacterOwner.RemoveEffect(this, false);
	}
}
