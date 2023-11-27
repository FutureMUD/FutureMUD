using System;
using System.Globalization;
using System.Text;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat;

public class RangedWeaponTypeDefinition : FrameworkItem, IRangedWeaponType
{
	public RangedWeaponTypeDefinition(Models.RangedWeaponTypes type, IFuturemud gameworld)
	{
		_id = type.Id;
		_name = type.Name;
		Classification = (WeaponClassification)type.Classification;
		FireTrait = gameworld.Traits.Get(type.FireTraitId);
		OperateTrait = gameworld.Traits.Get(type.OperateTraitId);
		RangedWeaponType = (RangedWeaponType)type.RangedWeaponType;
		SpecificAmmunitionGrade = type.SpecificAmmunitionGrade;
		StaminaPerLoadStage = type.StaminaPerLoadStage;
		StaminaToFire = type.StaminaToFire;
		FireableInMelee = type.FireableInMelee;
		DefaultRangeInRooms = (uint)type.DefaultRangeInRooms;
		CoverBonus = type.CoverBonus;
		AmmunitionCapacity = type.AmmunitionCapacity;
		AmmunitionLoadType = (AmmunitionLoadType)type.AmmunitionLoadType;
		AccuracyBonusExpression = new TraitExpression(type.AccuracyBonusExpression, gameworld);
		DamageBonusExpression = new TraitExpression(type.DamageBonusExpression, gameworld);
		LoadCombatDelay = type.LoadDelay;
		ReadyCombatDelay = type.ReadyDelay;
		FireCombatDelay = type.FireDelay;
		AimBonusLostPerShot = type.AimBonusLostPerShot;
		BaseAimDifficulty = (Difficulty)type.BaseAimDifficulty;
		RequiresFreeHandToReady = type.RequiresFreeHandToReady;
		AlwaysRequiresTwoHandsToWield = type.AlwaysRequiresTwoHandsToWield;
	}

	public ITraitExpression AccuracyBonusExpression { get; set; }

	public int AmmunitionCapacity { get; set; }

	public AmmunitionLoadType AmmunitionLoadType { get; set; }

	public Difficulty BaseAimDifficulty { get; set; }

	public WeaponClassification Classification { get; set; }

	public double CoverBonus { get; set; }

	public ITraitExpression DamageBonusExpression { get; set; }

	public uint DefaultRangeInRooms { get; set; }

	public bool FireableInMelee { get; set; }

	public ITraitDefinition FireTrait { get; set; }

	public override string FrameworkItemType => "RangedWeaponTypeDefinition";

	public ITraitDefinition OperateTrait { get; set; }

	public string SpecificAmmunitionGrade { get; set; }

	public double StaminaPerLoadStage { get; set; }

	public double StaminaToFire { get; set; }

	public RangedWeaponType RangedWeaponType { get; set; }

	public double LoadCombatDelay { get; set; }

	public double ReadyCombatDelay { get; set; }

	public double FireCombatDelay { get; set; }

	public double AimBonusLostPerShot { get; set; }

	public bool RequiresFreeHandToReady { get; set; }

	public bool AlwaysRequiresTwoHandsToWield { get; set; }

	public CheckType FireCheck
	{
		get
		{
			switch (RangedWeaponType)
			{
				case RangedWeaponType.Bow:
					return CheckType.FireBow;
				case RangedWeaponType.Crossbow:
					return CheckType.FireCrossbow;
				case RangedWeaponType.Firearm:
				case RangedWeaponType.Laser:
				case RangedWeaponType.ModernFirearm:
					return CheckType.FireFirearm;
				case RangedWeaponType.Sling:
					return CheckType.FireSling;
				case RangedWeaponType.Thrown:
					return CheckType.ThrownWeaponCheck;
				default:
					throw new ApplicationException("Unknown RangedWeaponType in FireCheck.");
			}
		}
	}

	public string Show(ICharacter character)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Ranged Weapon Type: #{Id} - {Name}");
		sb.AppendLineColumns((uint)character.LineFormatLength, 3,
			$"Type: {RangedWeaponType.Describe().Colour(Telnet.Green)}",
			$"Range: {DefaultRangeInRooms.ToString().Colour(Telnet.Green)}",
			$"Class: {Classification.Describe().Colour(Telnet.Green)}");

		sb.AppendLineColumns((uint)character.LineFormatLength, 3,
			$"Fire Stamina: {StaminaToFire.ToString(CultureInfo.InvariantCulture).Colour(Telnet.Green)}",
			$"Load Stamina: {StaminaPerLoadStage.ToString(CultureInfo.InvariantCulture).Colour(Telnet.Green)}",
			$"Cover Bonus: {CoverBonus.ToString("N3").Colour(Telnet.Green)}");

		sb.AppendLineColumns((uint)character.LineFormatLength, 3,
			$"Aim: {BaseAimDifficulty.Describe().Colour(Telnet.Green)}", $"Aim Lost Per Shot: {AimBonusLostPerShot:P1}",
			$"Free Hand to Ready?: {RequiresFreeHandToReady}");

		sb.AppendLineColumns((uint)character.LineFormatLength, 3, $"Fire: {FireTrait.Name.Colour(Telnet.Green)}",
			$"Operate: {OperateTrait.Name.Colour(Telnet.Green)}",
			$"Melee: {FireableInMelee.ToString().Colour(Telnet.Green)}");

		sb.AppendLineColumns((uint)character.LineFormatLength, 3,
			$"Ammo: {AmmunitionCapacity.ToString().Colour(Telnet.Green)}",
			$"Ammo Type: {SpecificAmmunitionGrade.Colour(Telnet.Green)}",
			$"Load Type: {AmmunitionLoadType.Describe().Colour(Telnet.Green)}");

		sb.AppendLineColumns((uint)character.LineFormatLength, 3,
			$"Load Delay: {LoadCombatDelay.ToString("N3", character)}",
			$"Ready Delay: {ReadyCombatDelay.ToString("N3", character)}",
			$"Fire Delay: {FireCombatDelay.ToString("N3", character)}");

		sb.AppendLine($"Accuracy Bonus: {AccuracyBonusExpression.OriginalFormulaText.Colour(Telnet.Cyan)}");
		sb.AppendLine($"Damage Bonus: {AccuracyBonusExpression.OriginalFormulaText.Colour(Telnet.Cyan)}");

		return sb.ToString();
	}
}