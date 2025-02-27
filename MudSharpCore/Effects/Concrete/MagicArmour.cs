using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.Magic.Powers;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Concrete;

public class MagicArmour : ConcentrationConsumingEffect, IMagicEffect, ICheckBonusEffect, IMagicArmour,
	IDescriptionAdditionEffect
{
	#region Static Initialisation

	public static void InitialiseEffectType()
	{
		RegisterFactory("MagicArmour", (effect, owner) => new MagicArmour(effect, owner));
	}

	#endregion

	public MagicArmour(ICharacter owner, MagicArmourPower power) : base(owner, power.School,
		power.ConcentrationPointsToSustain)
	{
		Power = power;
		ApplicabilityProg = Power.ArmourAppliesProg;
	}

	public MagicArmour(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		var root = effect.Element("Effect");
		Power = (MagicArmourPower)Gameworld.MagicPowers.Get(long.Parse(root.Element("Power").Value));
		ApplicabilityProg = Power.ArmourAppliesProg;
	}

	protected override bool EffectCanPersistOnLogout => true;

	protected override void RegisterEvents()
	{
		base.RegisterEvents();
		CharacterOwner.OnStateChanged += CharacterOwner_OnStateChanged;
		CharacterOwner.OnDeath += CharacterOwner_OnNoLongerValid;
		Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat += DoSustainCostsTick;
	}

	public override void ReleaseEvents()
	{
		base.ReleaseEvents();
		CharacterOwner.OnStateChanged -= CharacterOwner_OnStateChanged;
		CharacterOwner.OnDeath -= CharacterOwner_OnNoLongerValid;
		Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat -= DoSustainCostsTick;
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

	public MagicArmourPower Power { get; protected set; }
	public string MagicArmourOriginDescription => $"{Power.Name.Colour(Power.School.PowerListColour)} Power";

	public IMagicPower PowerOrigin => Power;
	public Difficulty DetectMagicDifficulty => Power.DetectableWithDetectMagic;

	#region Overrides of Effect

	public override string Describe(IPerceiver voyeur)
	{
		return $"Magic Armour from the {Power.Name} power.";
	}

	protected override string SpecificEffectType => "MagicArmour";

	protected override XElement SaveDefinition()
	{
		return SaveToXml(new XElement("Power", PowerOrigin.Id));
	}

	public override bool SavingEffect => true;

	/// <summary>Fires when an effect is removed, including a matured scheduled effect</summary>
	public override void RemovalEffect()
	{
		ReleaseEvents();
	}

	#endregion

	#region Implementation of ICheckBonusEffect

	public bool AppliesToCheck(CheckType type)
	{
		return type.IsDefensiveCombatAction() || type.IsOffensiveCombatAction() || type.IsGeneralActivityCheck() ||
			   type.IsTargettedFriendlyCheck() || type.IsTargettedHostileCheck();
	}

	public double CheckBonus => Power.SustainPenalty;

	#endregion

	private void DoSustainCostsTick()
	{
		Power.DoSustainCostsTick(CharacterOwner);
	}

	#region Implementation of IAbsorbDamage

	public double TotalDamageAbsorbed { get; protected set; }

	private void CheckDamageAbsorbed()
	{
		var max = Power.MaximumDamageAbsorbed.Evaluate(CharacterOwner);
		if (max <= 0.0)
		{
			return;
		}

		if (TotalDamageAbsorbed >= max)
		{
			CharacterOwner.RemoveEffect(this, true);
		}
	}

	public IDamage SufferDamage(IDamage damage, ref List<IWound> wounds)
	{
		var (passOn, self) =
			ArmourType.AbsorbDamageViaSpell(damage, Power.ArmourMaterial, Quality, CharacterOwner, true);
		TotalDamageAbsorbed += passOn?.DamageAmount ?? 0.0;
		CheckDamageAbsorbed();
		return self;
	}

	public IDamage PassiveSufferDamage(IDamage damage, ref List<IWound> wounds)
	{
		var (passOn, self) =
			ArmourType.AbsorbDamageViaSpell(damage, Power.ArmourMaterial, Quality, CharacterOwner, true);
		TotalDamageAbsorbed += passOn?.DamageAmount ?? 0.0;
		CheckDamageAbsorbed();
		return self;
	}

	public void ProcessPassiveWound(IWound wound)
	{
		// Do nothing
	}

	#endregion

	#region Implementation of IMagicArmour

	public IArmourType ArmourType => Power.ArmourType;
	public ItemQuality Quality => Power.Quality;

	public bool AppliesToPart(IBodypart bodypart)
	{
		return Power.AppliesToBodypart(bodypart);
	}

	#endregion

	#region IDescriptionAdditionEffect

	public string GetAdditionalText(IPerceiver voyeur, bool colour)
	{
		if (!string.IsNullOrEmpty(Power.FullDescriptionAddendum))
		{
			if (!Power.ArmourCanBeObscuredByInventory ||
				CharacterOwner.Body.ExposedBodyparts.All(x => !AppliesToPart(x)))
			{
				return new EmoteOutput(new Emote(Power.FullDescriptionAddendum.SubstituteANSIColour(), CharacterOwner, CharacterOwner))
					.ParseFor(voyeur);
			}
		}

		return string.Empty;
	}

	public bool PlayerSet => false;

	#endregion
}