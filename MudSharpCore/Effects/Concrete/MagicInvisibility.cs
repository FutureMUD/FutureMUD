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

public class MagicInvisibility : ConcentrationConsumingEffect, IMagicEffect, ICheckBonusEffect
{
	#region Static Initialisation

	public static void InitialiseEffectType()
	{
		RegisterFactory("MagicInvisibility", (effect, owner) => new MagicInvisibility(effect, owner));
	}

	#endregion

	public InvisibilityPower InvisibilityPower { get; protected set; }

	#region Constructors

	public MagicInvisibility(ICharacter owner, InvisibilityPower power) : base(owner, power.School,
		power.ConcentrationPointsToSustain)
	{
		InvisibilityPower = power;
		ApplicabilityProg = power.InvisibilityAppliesProg;
		Login();
	}

	protected MagicInvisibility(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		var root = effect.Element("Effect");
		InvisibilityPower = (InvisibilityPower)Gameworld.MagicPowers.Get(long.Parse(root.Element("Power").Value));
		ApplicabilityProg = InvisibilityPower.InvisibilityAppliesProg;
	}

	#endregion

	#region Saving and Loading

	protected override XElement SaveDefinition()
	{
		return SaveToXml(new XElement("Power", PowerOrigin.Id));
	}

	#endregion

	#region Overrides of Effect
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

	protected override string SpecificEffectType => "MagicInvisibility";

	/// <summary>Fires when an effect is removed, including a matured scheduled effect</summary>
	public override void RemovalEffect()
	{
		ReleaseEvents();
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Magic Invisibility from the {PowerOrigin.Name.Colour(Telnet.Cyan)} power.";
	}

	public override bool SavingEffect => true;

	public override PerceptionTypes Obscuring => InvisibilityPower.PerceptionTypes;

	public override bool Applies(object target)
	{
		if (target is InvisibilityPower power)
		{
			return InvisibilityPower == power;
		}

		return base.Applies(target);
	}

	#endregion

	public IMagicPower PowerOrigin => InvisibilityPower;
	public Difficulty DetectMagicDifficulty => InvisibilityPower.DetectableWithDetectMagic;

	public bool AppliesToCheck(CheckType type)
	{
		return type.IsDefensiveCombatAction() || type.IsOffensiveCombatAction() || type.IsGeneralActivityCheck() ||
		       type.IsTargettedFriendlyCheck() || type.IsTargettedHostileCheck();
	}

	public double CheckBonus => InvisibilityPower.SustainPenalty;

	private void DoSustainCostsTick()
	{
		InvisibilityPower.DoSustainCostsTick(CharacterOwner);
	}
}