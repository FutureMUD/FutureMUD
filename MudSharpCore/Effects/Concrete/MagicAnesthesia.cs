using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.Magic.Powers;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Concrete;

public class BeingMagicallyAnesthetised : Effect, ICauseDrugEffect
{
	public BeingMagicallyAnesthetised(ICharacter owner, MagicAnesthesia originator) : base(owner)
	{
		CharacterOwner = owner;
		OriginatorEffect = originator;
		CharacterOwner.OnQuit += CharacterOwner_OnNoLongerValid;
		CharacterOwner.OnDeleted += CharacterOwner_OnNoLongerValid;
		CharacterOwner.OnDeath += CharacterOwner_OnNoLongerValid;
		CharacterOwner.OnLocationChanged += CharacterOwner_OnLocationChanged;
	}

	private void CharacterOwner_OnLocationChanged(Form.Shape.ILocateable locatable,
		Construction.Boundary.ICellExit exit)
	{
		OriginatorEffect.CharacterOwner_OnLocationChanged(locatable, exit);
	}

	private void CharacterOwner_OnNoLongerValid(IPerceivable owner)
	{
		OriginatorCharacter.RemoveEffect(OriginatorEffect, true);
	}

	public void ReleaseEvents()
	{
		CharacterOwner.OnQuit -= CharacterOwner_OnNoLongerValid;
		CharacterOwner.OnDeleted -= CharacterOwner_OnNoLongerValid;
		CharacterOwner.OnDeath -= CharacterOwner_OnNoLongerValid;
		CharacterOwner.OnLocationChanged -= CharacterOwner_OnLocationChanged;
	}

	public ICharacter CharacterOwner { get; }
	public ICharacter OriginatorCharacter => OriginatorEffect.CharacterOwner;

	public MagicAnesthesia OriginatorEffect { get; }

	protected override string SpecificEffectType => "BeingMagicallyAnesthetised";

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"Being magically anaesthetised by {OriginatorCharacter.HowSeen(voyeur)} @ intensity {OriginatorEffect.CurrentIntensity.ToString("N2", voyeur).ColourValue()}/{OriginatorEffect.TargetIntensity.ToString("N2", voyeur).ColourValue()} through the {OriginatorEffect.AnesthesiaPower.Name.ColourValue()} power.";
	}

	public string NoQuitReason => "You're feeling a little bit too sleepy to quit...";

	public override bool Applies(object target, object thirdparty)
	{
		if (target is ICharacter ch && thirdparty is MindAnesthesiaPower mp)
		{
			return ch == OriginatorCharacter && OriginatorEffect.AnesthesiaPower == mp;
		}

		return base.Applies(target, thirdparty);
	}

	public IEnumerable<DrugType> AffectedDrugTypes { get; } = new[] { DrugType.Anesthesia };

	public double AddedIntensity(ICharacter character, DrugType drugtype)
	{
		if (drugtype == DrugType.Anesthesia)
		{
			return OriginatorEffect.CurrentIntensity;
		}

		return 0.0;
	}
}

public class MagicAnesthesia : ConcentrationConsumingEffect, IMagicEffect, ICheckBonusEffect
{
	#region Constructors

	public MagicAnesthesia(ICharacter owner, MindAnesthesiaPower power, ICharacter target, double targetIntensity, double sustainMultiplier) :
		base(owner, power.School, power.ConcentrationPointsToSustain)
	{
		AnesthesiaPower = power;
		ApplicabilityProg = power.AppliesProg;
		TargetIntensity = targetIntensity;
		CharacterTarget = target;
		SustainCostMultiplier = sustainMultiplier;
		ChildEffect = new BeingMagicallyAnesthetised(CharacterTarget, this);
		CharacterTarget.AddEffect(ChildEffect);
		Login();
	}

	#endregion

	public BeingMagicallyAnesthetised ChildEffect { get; }

	protected override string SpecificEffectType => "MagicAnesthesia";

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"Magically anesthetizing {CharacterTarget.HowSeen(voyeur)} @ intensity {CurrentIntensity.ToString("N2", voyeur).ColourValue()}/{TargetIntensity.ToString("N2", voyeur).ColourValue()} through the {AnesthesiaPower.Name.ColourValue()} power.";
	}

	public void CharacterOwner_OnLocationChanged(Form.Shape.ILocateable locatable, Construction.Boundary.ICellExit exit)
	{
		if (!AnesthesiaPower.TargetIsInRange(CharacterOwner, CharacterTarget, AnesthesiaPower.PowerDistance))
		{
			CharacterOwner.RemoveEffect(this, true);
		}
	}

	public ICharacter CharacterTarget { get; protected set; }

	public MindAnesthesiaPower AnesthesiaPower { get; }
	public IMagicPower PowerOrigin => AnesthesiaPower;
	public Difficulty DetectMagicDifficulty => AnesthesiaPower.DetectableWithDetectMagic;

	public double CurrentIntensity { get; protected set; }
	public double TargetIntensity { get; protected set; }
	public double SustainCostMultiplier { get; protected set; }

	public bool AppliesToCheck(CheckType type)
	{
		return type.IsDefensiveCombatAction() || type.IsOffensiveCombatAction() || type.IsGeneralActivityCheck() ||
			   type.IsTargettedFriendlyCheck() || type.IsTargettedHostileCheck();
	}

	public double CheckBonus => AnesthesiaPower.SustainPenalty;

	private void DoSustainCostsTick()
	{
		AnesthesiaPower.DoSustainCostsTick(CharacterOwner, SustainCostMultiplier);
	}

	public override void ExpireEffect()
	{
		CurrentIntensity += AnesthesiaPower.RampRatePerTick * TargetIntensity;
		if (CurrentIntensity >= TargetIntensity)
		{
			CurrentIntensity = TargetIntensity;
		}

		Changed = true;
		Owner.Reschedule(this, AnesthesiaPower.TickLength);
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
			return AnesthesiaPower == me.PowerOrigin;
		}

		return base.Applies(target);
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

	public override void RemovalEffect()
	{
		ReleaseEvents();
		if (!string.IsNullOrWhiteSpace(AnesthesiaPower.EndPowerEmoteText))
		{
			CharacterOwner.OutputHandler.Handle(new EmoteOutput(new Emote(AnesthesiaPower.EndPowerEmoteText,
				CharacterOwner, CharacterOwner, CharacterTarget)));
		}

		if (!string.IsNullOrWhiteSpace(AnesthesiaPower.EndPowerEmoteTextTarget))
		{
			CharacterTarget.OutputHandler.Handle(
				new EmoteOutput(new Emote(AnesthesiaPower.EndPowerEmoteTextTarget, CharacterOwner, CharacterOwner,
					CharacterTarget)), OutputRange.Personal);
		}

		CharacterTarget.RemoveEffect(ChildEffect);
	}
}