using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.Magic.Powers;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Concrete;

public class ConnectMindEffect : ConcentrationConsumingEffect, IMagicEffect, ICheckBonusEffect
{
	public ICharacter TargetCharacter { get; protected set; }
	public MindConnectedToEffect ChildEffect { get; protected set; }

	public ConnectMindEffect(ICharacter owner, ICharacter target, ConnectMindPower power) : base(owner, power.School,
		power.ConcentrationPointsToSustain)
	{
		TargetCharacter = target;
		MindPower = power;
		DetectMagicDifficulty = power.DetectableWithDetectMagic;
		ChildEffect = new MindConnectedToEffect(TargetCharacter, CharacterOwner, this);
		TargetCharacter.AddEffect(ChildEffect);
		Login();
	}

	protected override void RegisterEvents()
	{
		base.RegisterEvents();
		CharacterOwner.OnStateChanged += CharacterOwner_OnStateChanged;
		CharacterOwner.OnDeath += CharacterOwner_OnNoLongerValid;
		CharacterOwner.OnQuit += CharacterOwner_OnNoLongerValid;
		CharacterOwner.OnLocationChanged += CharacterOwner_OnLocationChanged;
		Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat += DoSustainCostsTick;
	}

	public override void Login()
	{
		base.Login();
		ChildEffect.Login();
	}

	public void CharacterOwner_OnLocationChanged(Form.Shape.ILocateable locatable, Construction.Boundary.ICellExit exit)
	{
		if (!MindPower.TargetIsInRange(CharacterOwner, TargetCharacter, MindPower.PowerDistance))
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
		CharacterOwner.OnStateChanged -= CharacterOwner_OnStateChanged;
		CharacterOwner.OnDeath -= CharacterOwner_OnNoLongerValid;
		CharacterOwner.OnQuit -= CharacterOwner_OnNoLongerValid;
		CharacterOwner.OnLocationChanged -= CharacterOwner_OnLocationChanged;
		Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat -= DoSustainCostsTick;
	}

	#region Overrides of Effect

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"Mind is connected to {TargetCharacter.HowSeen(voyeur)} via {School.Name.Colour(School.PowerListColour)}.";
	}

	protected override string SpecificEffectType => "ConnectMind";

	public override void RemovalEffect()
	{
		ReleaseEvents();
		CharacterOwner.OutputHandler.Send(new EmoteOutput(new Emote(MindPower.SelfEmoteForDisconnect, CharacterOwner,
			CharacterOwner, TargetCharacter)));
		var targetEmote = MindPower.GetAppropriateDisconnectEmote(CharacterOwner, TargetCharacter);
		if (!string.IsNullOrWhiteSpace(targetEmote))
		{
			TargetCharacter.OutputHandler.Send(new EmoteOutput(new Emote(targetEmote, CharacterOwner, CharacterOwner,
				TargetCharacter)));
		}

		TargetCharacter.RemoveEffect(ChildEffect, true);
		Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat -= DoSustainCostsTick;
	}

	private void DoSustainCostsTick()
	{
		MindPower.DoSustainCostsTick(CharacterOwner);
	}

	#endregion

	#region Implementation of IMagicEffect

	public ConnectMindPower MindPower { get; set; }
	public IMagicPower PowerOrigin => MindPower;
	public Difficulty DetectMagicDifficulty { get; set; }

	#endregion

	#region ICheckBonusEffect Implementation

	public bool AppliesToCheck(CheckType type)
	{
		return type.IsDefensiveCombatAction() || type.IsOffensiveCombatAction() || type.IsGeneralActivityCheck() ||
		       type.IsTargettedFriendlyCheck() || type.IsTargettedHostileCheck();
	}

	public double CheckBonus => Gameworld.GetStaticDouble("ConcentrationToCheckBonusPenaltyForConnectMind") *
	                            ConcentrationPointsConsumed;

	#endregion
}