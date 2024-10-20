using System;
using System.Globalization;
using System.Linq;
using System.Text;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Commands.Trees;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.RPG.Checks;
using Parlot.Fluent;

namespace MudSharp.Combat;

public class RangedWeaponTypeDefinition : SaveableItem, IRangedWeaponType
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

	private RangedWeaponTypeDefinition(RangedWeaponTypeDefinition rhs, string name)
	{
		Gameworld = rhs.Gameworld;
		_name = name;
		AccuracyBonusExpression = rhs.AccuracyBonusExpression;
		AmmunitionLoadType = rhs.AmmunitionLoadType;
		AmmunitionCapacity = rhs.AmmunitionCapacity;
		BaseAimDifficulty = rhs.BaseAimDifficulty;
		Classification = rhs.Classification;
		CoverBonus = rhs.CoverBonus;
		DamageBonusExpression = new TraitExpression(rhs.DamageBonusExpression.OriginalFormulaText, Gameworld);
		DefaultRangeInRooms = rhs.DefaultRangeInRooms;
		FireableInMelee = rhs.FireableInMelee;
		FireTrait = rhs.FireTrait;
		OperateTrait = rhs.OperateTrait;
		SpecificAmmunitionGrade = rhs.SpecificAmmunitionGrade;
		StaminaPerLoadStage = rhs.StaminaPerLoadStage;
		RangedWeaponType = rhs.RangedWeaponType;
		LoadCombatDelay = rhs.LoadCombatDelay;
		FireCombatDelay = rhs.FireCombatDelay;
		ReadyCombatDelay = rhs.ReadyCombatDelay;
		AimBonusLostPerShot = rhs.AimBonusLostPerShot;
		RequiresFreeHandToReady = rhs.RequiresFreeHandToReady;
		AlwaysRequiresTwoHandsToWield = rhs.AlwaysRequiresTwoHandsToWield;
		DoDatabaseInsert();
	}

	public RangedWeaponTypeDefinition(IFuturemud gameworld, string name, RangedWeaponType type, ITraitDefinition trait)
	{
		Gameworld = gameworld;
		_name = name;
		RangedWeaponType = type;
		OperateTrait = trait;
		FireTrait = trait;
		Classification = WeaponClassification.Lethal;
		FireableInMelee = false;
		CoverBonus = 2.0;
		AmmunitionCapacity = 1;
		BaseAimDifficulty = Difficulty.Normal;
		FireCombatDelay = 0.1;
		ReadyCombatDelay = 0.1;
		LoadCombatDelay = 1.0;
		AimBonusLostPerShot = 1.0;
		RequiresFreeHandToReady = true;
		AlwaysRequiresTwoHandsToWield = false;
		switch (type)
		{
			case RangedWeaponType.Thrown:
				DefaultRangeInRooms = 0;
				StaminaPerLoadStage = 0.0;
				StaminaToFire = 20.0;
				SpecificAmmunitionGrade = "Throwing";
				AmmunitionLoadType = AmmunitionLoadType.Direct;
				DamageBonusExpression = new TraitExpression("quality - (8.0*range) + ((str:1-10)*1.5)", Gameworld);
				AccuracyBonusExpression = new TraitExpression("(-6 * range) + (pow(1 - aim, 2) * -15)", Gameworld);
				break;
			case RangedWeaponType.Firearm:
				DefaultRangeInRooms = 4;
				StaminaPerLoadStage = 2.0;
				StaminaToFire = 1.0;
				SpecificAmmunitionGrade = "9x19mm Parabellum";
				AmmunitionLoadType = AmmunitionLoadType.Magazine;
				DamageBonusExpression = new TraitExpression("2*quality - (2.0*range)", Gameworld);
				AccuracyBonusExpression = new TraitExpression("9 + (-3 * range) + (pow(1 - aim, 2) * -15)", Gameworld);
				break;
			case RangedWeaponType.ModernFirearm:
				DefaultRangeInRooms = 6;
				StaminaPerLoadStage = 1.0;
				StaminaToFire = 1.0;
				SpecificAmmunitionGrade = "9x19mm Parabellum";
				AmmunitionLoadType = AmmunitionLoadType.Magazine;
				DamageBonusExpression = new TraitExpression("2*quality - (1.0*range)", Gameworld);
				AccuracyBonusExpression = new TraitExpression("9 + (-3 * range) + (pow(1 - aim, 2) * -15)", Gameworld);
				break;
			case RangedWeaponType.Laser:
				DefaultRangeInRooms = 3;
				StaminaPerLoadStage = 1.0;
				StaminaToFire = 1.0;
				SpecificAmmunitionGrade = "Laser";
				AmmunitionLoadType = AmmunitionLoadType.Direct;
				DamageBonusExpression = new TraitExpression("2*quality - (2.0*range)", Gameworld);
				AccuracyBonusExpression = new TraitExpression("9 + (-3 * range) + (pow(1 - aim, 2) * -15)", Gameworld);
				break;
			case RangedWeaponType.Bow:
				DefaultRangeInRooms = 3;
				StaminaPerLoadStage = 10.0;
				StaminaToFire = 5.0;
				SpecificAmmunitionGrade = "Arrow";
				AmmunitionLoadType = AmmunitionLoadType.Direct;
				DamageBonusExpression = new TraitExpression("2*quality - (4.0*range) + ((str:1-10)*1.5)", Gameworld);
				AccuracyBonusExpression = new TraitExpression("(-3.0 * range) + (pow(1 - aim, 2) * -15)", Gameworld);
				break;
			case RangedWeaponType.Crossbow:
				DefaultRangeInRooms = 3;
				StaminaPerLoadStage = 20.0;
				StaminaToFire = 5.0;
				SpecificAmmunitionGrade = "Bolt";
				AmmunitionLoadType = AmmunitionLoadType.Direct;
				DamageBonusExpression = new TraitExpression("2*quality - (4.0*range)", Gameworld);
				AccuracyBonusExpression = new TraitExpression("(-3.0 * range) + (pow(1 - aim, 2) * -15)", Gameworld);
				break;
			case RangedWeaponType.Sling:
				StaminaPerLoadStage = 15.0;
				DefaultRangeInRooms = 2;
				StaminaToFire = 20.0;
				SpecificAmmunitionGrade = "Sling Bullet";
				AmmunitionLoadType = AmmunitionLoadType.Direct;
				DamageBonusExpression = new TraitExpression("quality - (7.0*range) + ((str:1-10)*1.5)", Gameworld);
				AccuracyBonusExpression = new TraitExpression("(-6 * range) + (pow(1 - aim, 2) * -15)", Gameworld);
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(type), type, null);
		}
		DoDatabaseInsert();
	}

	public IRangedWeaponType Clone(string name)
	{
		return new RangedWeaponTypeDefinition(this, name);
	}

	private void DoDatabaseInsert()
	{
		using (new FMDB())
		{
			var dbitem = new Models.RangedWeaponTypes();
			SaveToDBItem(dbitem);
			FMDB.Context.RangedWeaponTypes.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	private void SaveToDBItem(Models.RangedWeaponTypes dbitem)
	{
		dbitem.Name = Name;
		dbitem.AccuracyBonusExpression = AccuracyBonusExpression.OriginalFormulaText;
		dbitem.AmmunitionCapacity = AmmunitionCapacity;
		dbitem.AmmunitionLoadType = (int)AmmunitionLoadType;
		dbitem.BaseAimDifficulty = (int)BaseAimDifficulty;
		dbitem.Classification = (int)Classification;
		dbitem.CoverBonus = CoverBonus;
		dbitem.DamageBonusExpression = DamageBonusExpression.OriginalFormulaText;
		dbitem.DefaultRangeInRooms = (int)DefaultRangeInRooms;
		dbitem.FireableInMelee = FireableInMelee;
		dbitem.FireTraitId = FireTrait.Id;
		dbitem.OperateTraitId = OperateTrait.Id;
		dbitem.SpecificAmmunitionGrade = SpecificAmmunitionGrade;
		dbitem.StaminaPerLoadStage = StaminaPerLoadStage;
		dbitem.RangedWeaponType = (int)RangedWeaponType;
		dbitem.LoadDelay = LoadCombatDelay;
		dbitem.ReadyDelay = ReadyCombatDelay;
		dbitem.FireDelay = FireCombatDelay;
		dbitem.AimBonusLostPerShot = AimBonusLostPerShot;
		dbitem.RequiresFreeHandToReady = RequiresFreeHandToReady;
		dbitem.AlwaysRequiresTwoHandsToWield = AlwaysRequiresTwoHandsToWield;
	}

	/// <inheritdoc />
	public override void Save()
	{
		var dbitem = FMDB.Context.RangedWeaponTypes.Find(Id);
		SaveToDBItem(dbitem);
		Changed = false;
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
		sb.AppendLine($"Ranged Weapon Type: #{Id} - {Name}".GetLineWithTitle(character, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLineColumns(Math.Min((uint)character.LineFormatLength, 120U), 3,
			$"Type: {RangedWeaponType.Describe().Colour(Telnet.Green)}",
			$"Range: {DefaultRangeInRooms.ToString().Colour(Telnet.Green)}",
			$"Class: {Classification.Describe().Colour(Telnet.Green)}");

		sb.AppendLineColumns(Math.Min((uint)character.LineFormatLength, 120U), 3,
			$"Fire Stamina: {StaminaToFire.ToString(CultureInfo.InvariantCulture).Colour(Telnet.Green)}",
			$"Load Stamina: {StaminaPerLoadStage.ToString(CultureInfo.InvariantCulture).Colour(Telnet.Green)}",
			$"Cover Bonus: {CoverBonus.ToBonusString(character)}");

		sb.AppendLineColumns(Math.Min((uint)character.LineFormatLength, 120U), 3,
			$"Aim: {BaseAimDifficulty.DescribeColoured()}", 
			$"Aim Lost Per Shot: {AimBonusLostPerShot.ToStringP2Colour(character)}",
			$"Free Hand to Ready?: {RequiresFreeHandToReady.ToColouredString()}");

		sb.AppendLineColumns(Math.Min((uint)character.LineFormatLength, 120U), 3, 
			$"Fire: {FireTrait.Name.Colour(Telnet.Green)}",
			$"Operate: {OperateTrait.Name.Colour(Telnet.Green)}",
			$"Melee: {FireableInMelee.ToColouredString()}");

		sb.AppendLineColumns(Math.Min((uint)character.LineFormatLength, 120U), 3,
			$"Ammo: {AmmunitionCapacity.ToString().Colour(Telnet.Green)}",
			$"Ammo Type: {SpecificAmmunitionGrade.Colour(Telnet.Green)}",
			$"Load Type: {AmmunitionLoadType.Describe().Colour(Telnet.Green)}");

		sb.AppendLineColumns(Math.Min((uint)character.LineFormatLength, 120U), 3,
			$"Load Delay: {TimeSpan.FromSeconds(LoadCombatDelay).DescribePreciseBrief(character).ColourValue()}",
			$"Ready Delay: {TimeSpan.FromSeconds(ReadyCombatDelay).DescribePreciseBrief(character).ColourValue()}",
			$"Fire Delay: {TimeSpan.FromSeconds(FireCombatDelay).DescribePreciseBrief(character).ColourValue()}");

		sb.AppendLineColumns(Math.Min((uint)character.LineFormatLength, 120U), 3,
			$"Always 2-Handed: {AlwaysRequiresTwoHandsToWield.ToColouredString()}",
			$"",
			$"");
		sb.AppendLine();
		sb.AppendLine($"Accuracy Bonus: {AccuracyBonusExpression.OriginalFormulaText.Colour(Telnet.Cyan)}");
		sb.AppendLine($"Damage Bonus: {DamageBonusExpression.OriginalFormulaText.Colour(Telnet.Cyan)}");

		return sb.ToString();
	}

	public const string BuildingCommandHelpText = @"You can use the following options with this command:

	#3name <name>#0 - changes the name
	#3type <type>#0 - changes the ranged weapon type that it codedly applies to
	#3range <##>#0 - sets the range in rooms that this weapon can fire
	#3class <class>#0 - sets the weapon classification of this weapon
	#3firestamina <##>#0 - the amount of stamina needed to fire this weapon	
	#3loadstamina <##>#0 - the amount of stamina needed to load this weapon
	#3cover <##>#0 - the bonus (-ve for penalty) against targets in cover
	#3aimdifficulty <difficulty>#0 - the difficulty of the aim check
	#3aimloss <%>#0 - how much percentage of the aim to lose after firing
	#3freehand#0 - toggles needing a free hand to ready the weapon
	#3fireskill <skill>#0 - sets the skill used when firing the weapon
	#3operateskill <skill>#0 - sets the skill used when loading/readying the weapon
	#3melee#0 - toggles being able to use the weapon to shoot in melee
	#3ammotype <grade>#0 - sets the ammo type that ammunition must match
	#3ammocapacity <##>#0 - sets the internal capacity for ammunition
	#3loadtype <loadtype>#0 - sets the ammunition load type
	#3loaddelay <seconds>#0 - sets the delay after loading
	#3readydelay <seconds>#0 - sets the delay after readying
	#3firedelay <seconds>#0 - sets the delay after firing
	#3accuracy <formula>#0 - sets the formula for accuracy bonus with the weapon
	#3damage <formula>#0 - sets the formula for damage bonus with the weapon

For the accuracy formula you can use the following parameters:

	#6quality#0 - the quality of the weapon item
	#6range#0 - the range in rooms
	#6inmelee#0 - 1 if being fired in melee, 0 otherwise
	#6aim#0 - the aim percentage between 0 and 1
	#6variable#0 - the character's current value of the fire skill

For the damage formula you can use the following parameters:

	#6quality#0 - the quality of the weapon item
	#6range#0 - the range in rooms
	#6inmelee#0 - 1 if being fired in melee, or 0 otherwise
	#6pointblank#0 - 1 if fired point blank, or 0 otherwise
	#6degree#0 - the opposed outcome degree between 0 (marginal) and 5 (total)
	#6variable#0 - the character's current value of the fire skill";
	public bool BuildingCommand(ICharacter actor, StringStack ss)
	{
		switch (ss.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, ss);
			case "type":
				return BuildingCommandType(actor, ss);
			case "range":
				return BuildingCommandRange(actor, ss);
			case "class":
				return BuildingCommandClass(actor, ss);
			case "firestamina":
			case "staminafire":
				return BuildingCommandStaminaFire(actor, ss);
			case "loadstamina":
			case "staminaload":
				return BuildingCommandStaminaLoad(actor, ss);
			case "cover":
				return BuildingCommandCover(actor, ss);
			case "aimdifficulty":
				return BuildingCommandAimDifficulty(actor, ss);
			case "aimloss":
			case "aimlosspershot":
			case "aimpershot":
				return BuildingCommandAimLossPerShot(actor, ss);
			case "freehand":
				return BuildingCommandFreeHand(actor);
			case "firetrait":
			case "fireskill":
				return BuildingCommandFireTrait(actor, ss);
			case "operatetrait":
			case "operateskill":
				return BuildingCommandOperateTrait(actor, ss);
			case "melee":
				return BuildingCommandMelee(actor);
			case "ammotype":
				return BuildingCommandAmmoType(actor, ss);
			case "ammocapacity":
				return BuildingCommandAmmoCapacity(actor, ss);
			case "loadtype":
				return BuildingCommandLoadType(actor, ss);
			case "loaddelay":
				return BuildingCommandLoadDelay(actor, ss);
			case "readydelay":
				return BuildingCommandReadyDelay(actor, ss);
			case "firedelay":
				return BuildingCommandFireDelay(actor, ss);
			case "accuracy":
			case "accuracybonus":
				return BuildingCommandAccuracyBonus(actor, ss);
			case "damage":
			case "damagebonus":
				return BuildingCommandDamageBonus(actor, ss);
			default:
				actor.OutputHandler.Send(BuildingCommandHelpText.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandDamageBonus(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to set the formula for damage bonus to?");
			return false;
		}

		var expression = new TraitExpression(ss.SafeRemainingArgument, Gameworld);
		if (expression.HasErrors())
		{
			actor.OutputHandler.Send(expression.Error);
			return false;
		}

		DamageBonusExpression = expression;
		Changed = true;
		actor.OutputHandler.Send($"The damage bonus expression for this weapon is now {ss.SafeRemainingArgument.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandAccuracyBonus(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to set the formula for accuracy bonus to?");
			return false;
		}

		var expression = new TraitExpression(ss.SafeRemainingArgument, Gameworld);
		if (expression.HasErrors())
		{
			actor.OutputHandler.Send(expression.Error);
			return false;
		}

		AccuracyBonusExpression = expression;
		Changed = true;
		actor.OutputHandler.Send($"The accuracy bonus expression for this weapon is now {ss.SafeRemainingArgument.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandFireDelay(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("How many seconds should combat actions be delayed after firing this weapon?");
			return false;
		}

		if (!double.TryParse(ss.SafeRemainingArgument, out var value) || value < 0.0)
		{
			actor.OutputHandler.Send($"The text {ss.SafeRemainingArgument.ColourCommand()} is not a valid number zero or greater.");
			return false;
		}

		FireCombatDelay = value;
		Changed = true;
		actor.OutputHandler.Send($"There will now be a {TimeSpan.FromSeconds(value).DescribePreciseBrief(actor).ColourValue()} delay after firing this weapon type.");
		return true;
	}

	private bool BuildingCommandReadyDelay(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("How many seconds should combat actions be delayed after readying this weapon?");
			return false;
		}

		if (!double.TryParse(ss.SafeRemainingArgument, out var value) || value < 0.0)
		{
			actor.OutputHandler.Send($"The text {ss.SafeRemainingArgument.ColourCommand()} is not a valid number zero or greater.");
			return false;
		}

		ReadyCombatDelay = value;
		Changed = true;
		actor.OutputHandler.Send($"There will now be a {TimeSpan.FromSeconds(value).DescribePreciseBrief(actor).ColourValue()} delay after readying this weapon type.");
		return true;
	}

	private bool BuildingCommandLoadDelay(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("How many seconds should combat actions be delayed after loading this weapon?");
			return false;
		}

		if (!double.TryParse(ss.SafeRemainingArgument, out var value) || value < 0.0)
		{
			actor.OutputHandler.Send($"The text {ss.SafeRemainingArgument.ColourCommand()} is not a valid number zero or greater.");
			return false;
		}

		LoadCombatDelay = value;
		Changed = true;
		actor.OutputHandler.Send($"There will now be a {TimeSpan.FromSeconds(value).DescribePreciseBrief(actor).ColourValue()} delay after loading this weapon type.");
		return true;
	}

	private bool BuildingCommandLoadType(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send($"What ammunition load type should this ranged weapon have? Valid options are {Enum.GetValues<AmmunitionLoadType>().ListToColouredString()}.");
			return false;
		}

		if (!ss.SafeRemainingArgument.TryParseEnum<AmmunitionLoadType>(out var value))
		{
			actor.OutputHandler.Send($"The text {ss.SafeRemainingArgument.ColourCommand()} is not a valid ammunition load type. The valid values are {Enum.GetValues<AmmunitionLoadType>().ListToColouredString()}.");
			return false;
		}

		AmmunitionLoadType = value;
		Changed = true;
		actor.OutputHandler.Send($"This weapon's ammunition load type is now {value.DescribeEnum().ColourValue()}.");
		return true;
	}

	private bool BuildingCommandAmmoCapacity(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What should be the internal ammunition capacity of this weapon type?");
			return false;
		}

		if (!int.TryParse(ss.SafeRemainingArgument, out var value) || value < 1)
		{
			actor.OutputHandler.Send($"The text {ss.SafeRemainingArgument.ColourCommand()} is not a valid number one or greater.");
			return false;
		}

		AmmunitionCapacity = value;
		Changed = true;
		actor.OutputHandler.Send($"This weapon type now has a {value.ToStringN0Colour(actor)} internal ammunition capacity.");
		return true;
	}

	private bool BuildingCommandAmmoType(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What should be the specific ammunition type that ammunition has to match to be used by this ranged weapon type?");
			return false;
		}

		var grade = ss.SafeRemainingArgument.ToLowerInvariant();
		SpecificAmmunitionGrade = ss.SafeRemainingArgument;
		Changed = true;
		var sameTypes = Gameworld.RangedWeaponTypes.Where(x => x.RangedWeaponType == RangedWeaponType).ToList();
		var warning = sameTypes.Any() && sameTypes.All(x => !x.SpecificAmmunitionGrade.EqualTo(grade)) && Gameworld.AmmunitionTypes.Where(x => x.RangedWeaponTypes.Contains(RangedWeaponType)).All(x => !x.SpecificType.EqualTo(grade));
		actor.OutputHandler.Send($"This ranged weapon now requires ammunition with a matching type {grade.ColourCommand()}.{(warning ? "\nWarning: no other ranged weapons of this type or ammunition types use this. Make sure you didn't make a typo.".ColourError() : "")}");
		return true;
	}

	private bool BuildingCommandMelee(ICharacter actor)
	{
		FireableInMelee = !FireableInMelee;
		Changed = true;
		actor.OutputHandler.Send($"This ranged weapon type will {FireableInMelee.NowNoLonger()} be able to be fired in melee combat.");
		return true;
	}

	private bool BuildingCommandOperateTrait(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send($"Which skill or trait should be used to operate (i.e. load or ready) this ranged weapon type?");
			return false;
		}

		var trait = Gameworld.Traits.GetByIdOrName(ss.SafeRemainingArgument);
		if (trait is null)
		{
			actor.OutputHandler.Send($"There is no skill or trait identified by the text {ss.SafeRemainingArgument.ColourCommand()}.");
			return false;
		}

		OperateTrait = trait;
		Changed = true;
		actor.OutputHandler.Send($"This ranged weapon type now uses the {trait.Name.ColourValue()} trait for operate checks.");
		return true;
	}

	private bool BuildingCommandFireTrait(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send($"Which skill or trait should be used to fire this ranged weapon type?");
			return false;
		}

		var trait = Gameworld.Traits.GetByIdOrName(ss.SafeRemainingArgument);
		if (trait is null)
		{
			actor.OutputHandler.Send($"There is no skill or trait identified by the text {ss.SafeRemainingArgument.ColourCommand()}.");
			return false;
		}

		FireTrait = trait;
		Changed = true;
		actor.OutputHandler.Send($"This ranged weapon type now uses the {trait.Name.ColourValue()} trait for fire checks.");
		return true;
	}

	private bool BuildingCommandFreeHand(ICharacter actor)
	{
		RequiresFreeHandToReady = !RequiresFreeHandToReady;
		Changed = true;
		actor.OutputHandler.Send($"This ranged weapon type {RequiresFreeHandToReady.NowNoLonger()} required a free hand in order to ready.");
		return true;
	}

	private bool BuildingCommandAimLossPerShot(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What percentage of aim should be lost per shot with this weapon?");
			return false;
		}

		if (!ss.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value))
		{
			actor.OutputHandler.Send($"The text {ss.SafeRemainingArgument.ColourCommand()} is not a valid percentage.");
			return false;
		}

		AimBonusLostPerShot = value;
		Changed = true;
		actor.OutputHandler.Send($"This ranged weapon type will now lose {AimBonusLostPerShot.ToStringP2Colour(actor)} percentage aim per shot.");
		return true;
	}

	private bool BuildingCommandAimDifficulty(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send($"How difficult should it be to aim this weapon?");
			return false;
		}

		if (!ss.SafeRemainingArgument.TryParseEnum<Difficulty>(out var value))
		{
			actor.OutputHandler.Send($"The text {ss.SafeRemainingArgument.ColourCommand()} is not a valid difficulty. The valid values are {Enum.GetValues<Difficulty>().ListToColouredString()}.");
			return false;
		}

		BaseAimDifficulty = value;
		Changed = true;
		actor.OutputHandler.Send($"The base difficulty to aim this ranged weapon is now {value.DescribeColoured()}.");
		return true;
	}

	private bool BuildingCommandCover(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What should be the bonus to the fire check applied to this ranged weapon when the target is in cover? Remember, negatives are penalties.");
			return false;
		}

		if (!double.TryParse(ss.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send($"The text {ss.SafeRemainingArgument.ColourCommand()} is not a valid bonus.");
			return false;
		}

		CoverBonus = value;
		Changed = true;
		actor.OutputHandler.Send($"This weapon will now impose a {value.ToBonusString(actor)} bonus to firing checks against targets in cover.");
		return true;
	}

	private bool BuildingCommandStaminaLoad(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("How much stamina should be used to load this weapon?");
			return false;
		}

		if (!double.TryParse(ss.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send($"The text {ss.SafeRemainingArgument.ColourCommand()} is not a valid number.");
			return false;
		}

		StaminaPerLoadStage = value;
		Changed = true;
		actor.OutputHandler.Send($"This weapon type will now use {value.ToStringN2Colour(actor)} stamina to load.");
		return true;
	}

	private bool BuildingCommandStaminaFire(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("How much stamina should be used to fire this weapon?");
			return false;
		}

		if (!double.TryParse(ss.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send($"The text {ss.SafeRemainingArgument.ColourCommand()} is not a valid number.");
			return false;
		}

		StaminaToFire = value;
		Changed = true;
		actor.OutputHandler.Send($"This weapon type will now use {value.ToStringN2Colour(actor)} stamina to fire.");
		return true;
	}

	private bool BuildingCommandClass(ICharacter actor, StringStack ss)
	{
		if(ss.IsFinished)
		{
			actor.OutputHandler.Send($"What class of weapon should this ranged weapon be? Valid options are {Enum.GetValues<WeaponClassification>().ListToColouredString()}.");
			return false;
		}

		if (!ss.SafeRemainingArgument.TryParseEnum<WeaponClassification>(out var value))
		{
			actor.OutputHandler.Send($"The text {ss.SafeRemainingArgument.ColourCommand()} is not a valid weapon classification. The valid values are {Enum.GetValues<WeaponClassification>().ListToColouredString()}.");
			return false;
		}

		Classification = value;
		Changed = true;
		actor.OutputHandler.Send($"This weapon's purpose is now classified as {value.DescribeEnum().ColourValue()}.");
		return true;
	}

	private bool BuildingCommandRange(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("How many rooms away should this ranged weapon be able to fire?");
			return false;
		}

		if (!uint.TryParse(ss.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send($"The text {ss.SafeRemainingArgument.ColourCommand()} is not a valid positive number.");
			return false;
		}

		DefaultRangeInRooms = value;
		Changed = true;
		actor.OutputHandler.Send($"This ranged weapon type will now be able to fire up to {value.ToStringN0Colour(actor)} {"room".Pluralise(value != 1)} away.");
		return true;
	}

	private bool BuildingCommandType(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send($"What type of ranged weapon should this ranged weapon be? Valid options are {Enum.GetValues<RangedWeaponType>().ListToColouredString()}.");
			return false;
		}

		if (!ss.SafeRemainingArgument.TryParseEnum<RangedWeaponType>(out var value))
		{
			actor.OutputHandler.Send($"The text {ss.SafeRemainingArgument.ColourCommand()} is not a valid ranged weapon type. The valid values are {Enum.GetValues<RangedWeaponType>().ListToColouredString()}.");
			return false;
		}

		RangedWeaponType = value;
		Changed = true;
		actor.OutputHandler.Send($"This weapon's is now of the {value.DescribeEnum().ColourValue()} type.");
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to rename this ranged weapon type to?");
			return false;
		}

		var name = ss.SafeRemainingArgument.TitleCase();
		if (Gameworld.RangedWeaponTypes.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send($"There is already a ranged weapon type called {name.ColourName()}. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You rename this ranged weapon type from {_name.ColourName()} to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}
}