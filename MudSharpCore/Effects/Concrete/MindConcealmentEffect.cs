#nullable enable

using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.Magic.Powers;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using System;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete;

public class MindConcealmentEffect : ConcentrationConsumingEffect, IMagicEffect, ICheckBonusEffect,
	IMindContactConcealmentEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("MindConcealment", (effect, owner) => new MindConcealmentEffect(effect, owner));
	}

	public MindConcealmentEffect(ICharacter owner, MindConcealPower power) : base(owner, power.School,
		power.ConcentrationPointsToSustain)
	{
		MindPower = power;
		Login();
	}

	protected MindConcealmentEffect(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		var root = effect.Element("Effect");
		var powerId = long.Parse(root!.Element("Power")!.Value);
		MindPower = Gameworld.MagicPowers.Get(powerId) as MindConcealPower ??
		            throw new ApplicationException(
			            $"The MindConcealment effect for {owner.FrameworkItemType} #{owner.Id} referred to invalid mindconceal power #{powerId}.");
	}

	protected override XElement SaveDefinition()
	{
		return SaveToXml(new XElement("Power", PowerOrigin.Id));
	}

	public override bool SavingEffect => true;

	protected override string SpecificEffectType => "MindConcealment";

	protected override bool EffectCanPersistOnLogout => true;

	public MindConcealPower MindPower { get; protected set; }
	public IMagicPower PowerOrigin => MindPower;
	public Difficulty DetectMagicDifficulty => MindPower.DetectableWithDetectMagic;
	public string UnknownIdentityDescription => MindPower.UnknownIdentityDescription;
	public int AuditDifficultyStages => MindPower.AuditDifficultyStages;

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"Mind-contact identity is concealed as {UnknownIdentityDescription.ColourCharacter()} via {MindPower.Name.Colour(School.PowerListColour)}.";
	}

	public bool ConcealsIdentityFrom(ICharacter source, ICharacter observer, IMagicSchool school)
	{
		return source == CharacterOwner &&
		       MindPower.AppliesToSchool(school) &&
		       MindPower.AppliesToCharacterProg.Execute<bool?>(CharacterOwner, observer) != false;
	}

	protected override void RegisterEvents()
	{
		base.RegisterEvents();
		CharacterOwner.OnStateChanged += CharacterOwner_OnStateChanged;
		CharacterOwner.OnDeath += CharacterOwner_OnNoLongerValid;
		if (Gameworld?.HeartbeatManager is not null)
		{
			Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat += DoSustainCostsTick;
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
		CharacterOwner.OnStateChanged -= CharacterOwner_OnStateChanged;
		CharacterOwner.OnDeath -= CharacterOwner_OnNoLongerValid;
		if (Gameworld?.HeartbeatManager is not null)
		{
			Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat -= DoSustainCostsTick;
		}
	}

	public override void RemovalEffect()
	{
		ReleaseEvents();
		CharacterOwner.OutputHandler.Send(new EmoteOutput(new Emote(MindPower.EmoteForEndSelf, CharacterOwner,
			CharacterOwner)));
		if (!string.IsNullOrWhiteSpace(MindPower.EmoteForEnd))
		{
			CharacterOwner.OutputHandler.Handle(new EmoteOutput(new Emote(MindPower.EmoteForEnd, CharacterOwner,
				CharacterOwner), flags: OutputFlags.SuppressSource), OutputRange.Local);
		}
	}

	private void DoSustainCostsTick()
	{
		MindPower.DoSustainCostsTick(CharacterOwner);
	}

	public bool AppliesToCheck(CheckType type)
	{
		return type.IsDefensiveCombatAction() || type.IsOffensiveCombatAction() || type.IsGeneralActivityCheck() ||
		       type.IsTargettedFriendlyCheck() || type.IsTargettedHostileCheck();
	}

	public double CheckBonus => MindPower.SustainPenalty;
}
