using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Form.Audio;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Health;
using MudSharp.Health.Strategies;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat;

public class AmmunitionType : SaveableItem, IAmmunitionType
{
	public AmmunitionType(Models.AmmunitionTypes type, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = type.Id;
		_name = type.Name;
		SpecificType = type.SpecificType;
		Loudness = (AudioVolume)type.Loudness;
		BreakChanceOnHit = type.BreakChanceOnHit;
		BreakChanceOnMiss = type.BreakChanceOnMiss;
		BaseAccuracy = type.BaseAccuracy;
		RangedWeaponTypes =
			type.RangedWeaponTypes.Split(' ').Select(x => (RangedWeaponType)int.Parse(x)).Distinct().ToList();

		DamageProfile = new SimpleDamageProfile
		{
			DamageType = (DamageType)type.DamageType,
			BaseAngleOfIncidence = Math.PI / 2,
			BaseAttackerDifficulty = Difficulty.Normal,
			BaseBlockDifficulty = (Difficulty)type.BaseBlockDifficulty,
			BaseDodgeDifficulty = (Difficulty)type.BaseDodgeDifficulty,
			BaseParryDifficulty = Difficulty.Impossible,
			DamageExpression = new TraitExpression(type.DamageExpression, gameworld),
			PainExpression = new TraitExpression(type.PainExpression, gameworld),
			StunExpression = new TraitExpression(type.StunExpression, gameworld)
		};

		if (RangedWeaponTypes.Contains(RangedWeaponType.Bow) || RangedWeaponTypes.Contains(RangedWeaponType.Crossbow) ||
		    RangedWeaponTypes.Contains(RangedWeaponType.Sling))
		{
			EchoType = AmmunitionEchoType.Arcing;
		}
		else if (RangedWeaponTypes.Contains(RangedWeaponType.ModernFirearm))
		{
			EchoType = AmmunitionEchoType.Supersonic;
		}
		else
		{
			EchoType = AmmunitionEchoType.Subsonic;
		}
	}

	public AmmunitionType(IFuturemud gameworld, string name, RangedWeaponType baseType)
	{
		Gameworld = gameworld;
		_name = name;
		RangedWeaponTypes = new[]
		{
			baseType
		};
		BaseAccuracy = 0.0;
		BreakChanceOnHit = 0.3;
		BreakChanceOnMiss = 0.5;
		switch (baseType)
		{
			case RangedWeaponType.Firearm:
				SpecificType = "Flintlock Musket";
				EchoType = AmmunitionEchoType.Subsonic;
				Loudness = AudioVolume.VeryLoud;
				DamageProfile = new SimpleDamageProfile
				{
					DamageType = DamageType.Ballistic,
					BaseBlockDifficulty = Difficulty.Insane,
					BaseDodgeDifficulty = Difficulty.Insane,
					DamageExpression = new TraitExpression("30 + quality * degree + (pointblank * 30)", Gameworld),
					PainExpression = new TraitExpression("30 + quality * degree + (pointblank * 30)", Gameworld),
					StunExpression = new TraitExpression("30 + quality * degree + (pointblank * 30)", Gameworld),
					BaseAngleOfIncidence = Math.PI / 2,
					BaseAttackerDifficulty = Difficulty.Normal,
					BaseParryDifficulty = Difficulty.Impossible
				};
				break;
			case RangedWeaponType.ModernFirearm:
				SpecificType = "5.56x45mm NATO";
				EchoType = AmmunitionEchoType.Supersonic;
				Loudness = AudioVolume.ExtremelyLoud;
				DamageProfile = new SimpleDamageProfile
				{
					DamageType = DamageType.Ballistic,
					BaseBlockDifficulty = Difficulty.Insane,
					BaseDodgeDifficulty = Difficulty.Impossible,
					DamageExpression = new TraitExpression("45 + quality * degree + (pointblank * 50)", Gameworld),
					PainExpression = new TraitExpression("45 + quality * degree + (pointblank * 50)", Gameworld),
					StunExpression = new TraitExpression("45 + quality * degree + (pointblank * 50)", Gameworld),
					BaseAngleOfIncidence = Math.PI / 2,
					BaseAttackerDifficulty = Difficulty.Normal,
					BaseParryDifficulty = Difficulty.Impossible
				};
				break;
			case RangedWeaponType.Bow:
				SpecificType = "Arrow";
				EchoType = AmmunitionEchoType.Arcing;
				Loudness = AudioVolume.Quiet;
				DamageProfile = new SimpleDamageProfile
				{
					DamageType = DamageType.Piercing,
					BaseBlockDifficulty = Difficulty.Easy,
					BaseDodgeDifficulty = Difficulty.VeryHard,
					DamageExpression = new TraitExpression("10 + quality * degree + (pointblank * 20)", Gameworld),
					PainExpression = new TraitExpression("10 + quality * degree + (pointblank * 20)", Gameworld),
					StunExpression = new TraitExpression("10 + quality * degree + (pointblank * 20)", Gameworld),
					BaseAngleOfIncidence = Math.PI / 2,
					BaseAttackerDifficulty = Difficulty.Normal,
					BaseParryDifficulty = Difficulty.Impossible
				};
				break;
			case RangedWeaponType.Crossbow:
				SpecificType = "Bolt";
				EchoType = AmmunitionEchoType.Arcing;
				Loudness = AudioVolume.Quiet;
				DamageProfile = new SimpleDamageProfile
				{
					DamageType = DamageType.Piercing,
					BaseBlockDifficulty = Difficulty.Easy,
					BaseDodgeDifficulty = Difficulty.VeryHard,
					DamageExpression = new TraitExpression("10 + quality * degree + (pointblank * 20)", Gameworld),
					PainExpression = new TraitExpression("10 + quality * degree + (pointblank * 20)", Gameworld),
					StunExpression = new TraitExpression("10 + quality * degree + (pointblank * 20)", Gameworld),
					BaseAngleOfIncidence = Math.PI / 2,
					BaseAttackerDifficulty = Difficulty.Normal,
					BaseParryDifficulty = Difficulty.Impossible
				};
				break;
			case RangedWeaponType.Sling:
				SpecificType = "Bullet";
				EchoType = AmmunitionEchoType.Arcing;
				Loudness = AudioVolume.Silent;
				DamageProfile = new SimpleDamageProfile
				{
					DamageType = DamageType.Piercing,
					BaseBlockDifficulty = Difficulty.VeryEasy,
					BaseDodgeDifficulty = Difficulty.Hard,
					DamageExpression = new TraitExpression("10 + quality * degree + (pointblank * 10)", Gameworld),
					PainExpression = new TraitExpression("10 + quality * degree + (pointblank * 10)", Gameworld),
					StunExpression = new TraitExpression("10 + quality * degree + (pointblank * 10)", Gameworld),
					BaseAngleOfIncidence = Math.PI / 2,
					BaseAttackerDifficulty = Difficulty.Normal,
					BaseParryDifficulty = Difficulty.Impossible
				};
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(baseType), baseType, null);
		}

		using (new FMDB())
		{
			var dbitem = new Models.AmmunitionTypes
			{
				Name = _name,
				SpecificType = SpecificType,
				RangedWeaponTypes = RangedWeaponTypes.Select(x => ((int)x).ToString("N0"))
				                                     .ListToCommaSeparatedValues(" "),
				BaseAccuracy = BaseAccuracy,
				Loudness = (int)Loudness,
				BreakChanceOnHit = BreakChanceOnHit,
				BreakChanceOnMiss = BreakChanceOnMiss,
				BaseBlockDifficulty = (int)DamageProfile.BaseBlockDifficulty,
				BaseDodgeDifficulty = (int)DamageProfile.BaseDodgeDifficulty,
				DamageExpression = DamageProfile.DamageExpression.OriginalFormulaText,
				StunExpression = DamageProfile.StunExpression.OriginalFormulaText,
				PainExpression = DamageProfile.PainExpression.OriginalFormulaText,
				DamageType = (int)DamageProfile.DamageType
			};
			FMDB.Context.AmmunitionTypes.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public AmmunitionType(AmmunitionType rhs, string newName)
	{
		Gameworld = rhs.Gameworld;
		_name = newName;
		RangedWeaponTypes = rhs.RangedWeaponTypes.ToList();
		BaseAccuracy = rhs.BaseAccuracy;
		BreakChanceOnHit = rhs.BreakChanceOnHit;
		BreakChanceOnMiss = rhs.BreakChanceOnMiss;
		DamageProfile = (SimpleDamageProfile)rhs.DamageProfile with { };
		Loudness = rhs.Loudness;
		SpecificType = rhs.SpecificType;

		using (new FMDB())
		{
			var dbitem = new Models.AmmunitionTypes
			{
				Name = _name,
				SpecificType = SpecificType,
				RangedWeaponTypes = RangedWeaponTypes.Select(x => ((int)x).ToString("N0"))
				                                     .ListToCommaSeparatedValues(" "),
				BaseAccuracy = BaseAccuracy,
				Loudness = (int)Loudness,
				BreakChanceOnHit = BreakChanceOnHit,
				BreakChanceOnMiss = BreakChanceOnMiss,
				BaseBlockDifficulty = (int)DamageProfile.BaseBlockDifficulty,
				BaseDodgeDifficulty = (int)DamageProfile.BaseDodgeDifficulty,
				DamageExpression = DamageProfile.DamageExpression.OriginalFormulaText,
				StunExpression = DamageProfile.StunExpression.OriginalFormulaText,
				PainExpression = DamageProfile.PainExpression.OriginalFormulaText,
				DamageType = (int)DamageProfile.DamageType
			};
			FMDB.Context.AmmunitionTypes.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public IAmmunitionType Clone(string newName)
	{
		return new AmmunitionType(this, newName);
	}

	public double BaseAccuracy { get; set; }

	public IDamageProfile DamageProfile { get; set; }

	public override string FrameworkItemType => "AmmunitionType";

	public AudioVolume Loudness { get; set; }

	public IEnumerable<RangedWeaponType> RangedWeaponTypes { get; set; }

	public string SpecificType { get; set; }

	public double BreakChanceOnHit { get; set; }
	public double BreakChanceOnMiss { get; set; }

	public AmmunitionEchoType EchoType { get; set; }

	public const string BuildingHelp = @"You can use the following options when editing ammunition types:

	#3name <name>#0 - sets the name
	#3grade <grade>#0 - sets the grade (mostly used for guns)
	#3volume <volume>#0 - sets the volume of the shot
	#3block <difficulty>#0 - how difficult it is to block a shot
	#3dodge <difficulty>#0 - how difficult it is to dodge a shot
	#3damagetype <type>#0 - sets the damage type dealt
	#3accuracy <bonus>#0 - sets the bonus accuracy from this ammo
	#3breakhit <%>#0 - sets the ammo break chance on hit
	#3breakmiss <%>#0 - sets the ammo break chance on miss
	#3damage <expression>#0 - sets the damage expression
	#3stun <expression>#0 - sets the stun expression
	#3pain <expression>#0 - sets the pain expression
	#3alldamage <expression>#0 - sets the damage, pain and stun expression at once

Note, with the damage/pain/stun expressions, you can use the following variables:

	#6pointblank#0 - 1 if fired at own bodypart or during coup de grace, 0 otherwise
	#6quality#0 - 0-11 for item quality, 5 = standard quality
	#6degree#0 - 0-5 for check success, 0 = marginal success, 5 = total success";

	/// <inheritdoc />
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant().CollapseString())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "rangetype":
			case "rangedtype":
			case "ranged":
			case "range":
				return BuildingCommandRangedType(actor, command);
			case "type":
			case "grade":
				return BuildingCommandType(actor, command);
			case "loud":
			case "loudness":
			case "volume":
				return BuildingCommandLoudness(actor, command);
			case "block":
				return BuildingCommandBlock(actor, command);
			case "dodge":
				return BuildingCommandDodge(actor, command);
			case "damagetype":
				return BuildingCommandDamageType(actor, command);
			case "alldamage":
				return BuildingCommandAllDamage(actor, command);
			case "damage":
				return BuildingCommandDamage(actor, command);
			case "stun":
				return BuildingCommandStun(actor, command);
			case "pain":
				return BuildingCommandPain(actor, command);
			case "breakhit":
				return BuildingCommandBreakHit(actor, command);
			case "breakmiss":
				return BuildingCommandBreakMiss(actor, command);
			case "accuracy":
			case "accurate":
			case "bonus":
				return BuildingCommandAccuracy(actor, command);
			default:
				actor.OutputHandler.Send(BuildingHelp.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandRangedType(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which ranged weapon type do you want to toggle for this ammunition? The valid choices are {Enum.GetValues<RangedWeaponType>().Select(x => x.Describe().ColourName()).ListToString()}.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<RangedWeaponType>(out var value))
		{
			actor.OutputHandler.Send(
				$"That is not a valid ranged weapon type. The valid choices are {Enum.GetValues<RangedWeaponType>().Select(x => x.Describe().ColourName()).ListToString()}.");
			return false;
		}

		if (RangedWeaponTypes.Contains(value))
		{
			actor.OutputHandler.Send(
				$"This ammunition is no longer a valid selection for {value.Describe().A_An().ColourValue()}.");
			RangedWeaponTypes = RangedWeaponTypes.Except(value).ToList();
		}
		else
		{
			RangedWeaponTypes = RangedWeaponTypes.Concat(new[] { value }).ToList();
			actor.OutputHandler.Send(
				$"This ammunition is now a valid selection for {value.Describe().A_An().ColourValue()}.");
		}

		Changed = true;
		return true;
	}

	private bool BuildingCommandType(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			var others = Gameworld.AmmunitionTypes
			                      .Where(x => x.RangedWeaponTypes.Any(y => RangedWeaponTypes.Contains(y)))
			                      .Select(x => x.SpecificType)
			                      .Distinct()
			                      .ToList();
			if (others.Any())
			{
				actor.OutputHandler.Send(
					$"What specific type or grade do you want to give this ammunition type?\nOther types and grades for these ranged weapon types include: {others.Select(x => x.ColourValue()).ListToString()}.");
				return false;
			}

			actor.OutputHandler.Send("What specific type or grade do you want to give this ammunition type?");
			return false;
		}

		SpecificType = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send(
			$"This ammunition type now has a specific type or grade of {SpecificType.ColourValue()}.");
		return true;
	}

	private bool BuildingCommandLoudness(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"How loud should the firing of this ammunition be? The valid choices are {Enum.GetValues<AudioVolume>().Select(x => x.Describe().ColourName()).ListToString()}.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<AudioVolume>(out var value))
		{
			actor.OutputHandler.Send(
				$"That is not a valid volume. The valid choices are {Enum.GetValues<AudioVolume>().Select(x => x.Describe().ColourName()).ListToString()}.");
			return false;
		}

		Loudness = value;
		Changed = true;
		actor.OutputHandler.Send($"It will now be {value.Describe().ColourValue()} when this ammunition is fired.");
		return true;
	}

	private bool BuildingCommandBlock(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"How difficult should it be to block shots with this ammunition? The valid choices are {Enum.GetValues<Difficulty>().Select(x => x.Describe().ColourName()).ListToString()}.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<Difficulty>(out var value))
		{
			actor.OutputHandler.Send(
				$"That is not a valid difficulty. The valid choices are {Enum.GetValues<Difficulty>().Select(x => x.Describe().ColourName()).ListToString()}.");
			return false;
		}

		DamageProfile = (SimpleDamageProfile)DamageProfile with { BaseBlockDifficulty = value };
		Changed = true;
		actor.OutputHandler.Send($"It will now be {value.Describe().ColourValue()} to block this projectile.");
		return true;
	}

	private bool BuildingCommandDodge(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"How difficult should it be to dodge shots with this ammunition? The valid choices are {Enum.GetValues<Difficulty>().Select(x => x.Describe().ColourName()).ListToString()}.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<Difficulty>(out var value))
		{
			actor.OutputHandler.Send(
				$"That is not a valid difficulty. The valid choices are {Enum.GetValues<Difficulty>().Select(x => x.Describe().ColourName()).ListToString()}.");
			return false;
		}

		DamageProfile = (SimpleDamageProfile)DamageProfile with { BaseDodgeDifficulty = value };
		Changed = true;
		actor.OutputHandler.Send($"It will now be {value.Describe().ColourValue()} to dodge this projectile.");
		return true;
	}

	private bool BuildingCommandDamageType(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What damage type should this projectile deal? The valid choices are {Enum.GetValues<DamageType>().Select(x => x.Describe().ColourName()).ListToString()}.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<DamageType>(out var value))
		{
			actor.OutputHandler.Send(
				$"That is not a valid damage type. The valid choices are {Enum.GetValues<DamageType>().Select(x => x.Describe().ColourName()).ListToString()}.");
			return false;
		}

		DamageProfile = (SimpleDamageProfile)DamageProfile with { DamageType = value };
		Changed = true;
		actor.OutputHandler.Send($"This projectile now deals {value.Describe().ColourValue()} damage.");
		return true;
	}

	private bool BuildingCommandAllDamage(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What should be the formula for damage, stun and pain for this projectile?\nYou can also use the variables #3quality#0, #3degree#0 and #3pointblank#0 in this expression."
					.SubstituteANSIColour());
			return false;
		}

		var expression = new TraitExpression(command.SafeRemainingArgument, Gameworld);
		if (expression.HasErrors())
		{
			actor.OutputHandler.Send(expression.Error);
			return false;
		}

		DamageProfile = (SimpleDamageProfile)DamageProfile with
		{
			DamageExpression = expression,
			StunExpression = expression,
			PainExpression = expression
		};
		Changed = true;
		actor.OutputHandler.Send(
			$"The damage, stun and pain expression for this ammunition are now all {command.SafeRemainingArgument.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandDamage(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What should be the formula for damage for this projectile?\nYou can also use the variables #3quality#0, #3degree#0 and #3pointblank#0 in this expression."
					.SubstituteANSIColour());
			return false;
		}

		var expression = new TraitExpression(command.SafeRemainingArgument, Gameworld);
		if (expression.HasErrors())
		{
			actor.OutputHandler.Send(expression.Error);
			return false;
		}

		DamageProfile = (SimpleDamageProfile)DamageProfile with
		{
			DamageExpression = expression
		};
		Changed = true;
		actor.OutputHandler.Send(
			$"The damage expression for this ammunition is now {command.SafeRemainingArgument.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandStun(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What should be the formula for stun for this projectile?\nYou can also use the variables #3quality#0, #3degree#0 and #3pointblank#0 in this expression."
					.SubstituteANSIColour());
			return false;
		}

		var expression = new TraitExpression(command.SafeRemainingArgument, Gameworld);
		if (expression.HasErrors())
		{
			actor.OutputHandler.Send(expression.Error);
			return false;
		}

		DamageProfile = (SimpleDamageProfile)DamageProfile with
		{
			StunExpression = expression
		};
		Changed = true;
		actor.OutputHandler.Send(
			$"The stun expression for this ammunition is now {command.SafeRemainingArgument.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandPain(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What should be the formula for pain for this projectile?\nYou can also use the variables #3quality#0, #3degree#0 and #3pointblank#0 in this expression."
					.SubstituteANSIColour());
			return false;
		}

		var expression = new TraitExpression(command.SafeRemainingArgument, Gameworld);
		if (expression.HasErrors())
		{
			actor.OutputHandler.Send(expression.Error);
			return false;
		}

		DamageProfile = (SimpleDamageProfile)DamageProfile with
		{
			PainExpression = expression
		};
		Changed = true;
		actor.OutputHandler.Send(
			$"The pain expression for this ammunition is now {command.SafeRemainingArgument.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandBreakHit(ICharacter actor, StringStack command)
	{
		if (command.IsFinished ||
		    !command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value))
		{
			actor.OutputHandler.Send(
				"You must specify a valid percentage change for this projectile to break on hitting a target.");
			return false;
		}

		BreakChanceOnHit = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This projectile now has a {BreakChanceOnHit.ToString("P2", actor).ColourValue()} chance to break on hitting a target.");
		return true;
	}

	private bool BuildingCommandBreakMiss(ICharacter actor, StringStack command)
	{
		if (command.IsFinished ||
		    !command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value))
		{
			actor.OutputHandler.Send(
				"You must specify a valid percentage change for this projectile to break on missing a target.");
			return false;
		}

		BreakChanceOnMiss = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This projectile now has a {BreakChanceOnMiss.ToString("P2", actor).ColourValue()} chance to break on missing a target.");
		return true;
	}

	private bool BuildingCommandAccuracy(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send("You must specify a valid bonus accuracy for this ammunition.");
			return false;
		}

		BaseAccuracy = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"The accuracy bonus for this ammunition is now {value.ToBonusString(actor)}. This would make an otherwise {Difficulty.Normal.DescribeColoured()} shot into a {Difficulty.Normal.ApplyBonus(value).DescribeColoured()} shot.");
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to this ammunition type?");
			return false;
		}

		var name = command.SafeRemainingArgument;
		if (Gameworld.AmmunitionTypes.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				$"There is already an ammunition type with the name {name.ColourName()}. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You rename the {_name.ColourName()} ammunition type to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Ammunition Type #{Id.ToString("N0", actor)} - {Name}".ColourName());
		sb.AppendLine(
			$"Ranged Weapon Types: {RangedWeaponTypes.Select(x => x.DescribeEnum(true).ColourValue()).ListToString()}");
		sb.AppendLine($"Ammo Grade: {SpecificType.ColourName()}");
		sb.AppendLine($"Echo Type: {EchoType.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Loudness: {Loudness.DescribeEnum(true).ColourValue()}");
		sb.AppendLine($"Block: {DamageProfile.BaseBlockDifficulty.DescribeColoured()}");
		sb.AppendLine($"Dodge: {DamageProfile.BaseDodgeDifficulty.DescribeColoured()}");
		sb.AppendLine($"Damage Type: {DamageProfile.DamageType.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Damage: {DamageProfile.DamageExpression.OriginalFormulaText.ColourCommand()}");
		sb.AppendLine($"Stun: {DamageProfile.StunExpression.OriginalFormulaText.ColourCommand()}");
		sb.AppendLine($"Pain: {DamageProfile.PainExpression.OriginalFormulaText.ColourCommand()}");
		sb.AppendLine(
			$"Break Chance (hit/miss): {BreakChanceOnHit.ToString("P2", actor).ColourValue()}/{BreakChanceOnMiss.ToString("P2", actor).ColourValue()}");
		sb.AppendLine($"Base Accuracy: {BaseAccuracy.ToString("N2", actor).ColourValue()}");
		return sb.ToString();
	}

	#region Overrides of SaveableItem

	/// <inheritdoc />
	public override void Save()
	{
		var dbitem = FMDB.Context.AmmunitionTypes.Find(Id);
		dbitem.Name = Name;
		dbitem.RangedWeaponTypes =
			RangedWeaponTypes.Select(x => ((int)x).ToString("F0")).ListToCommaSeparatedValues(" ");
		dbitem.SpecificType = SpecificType;
		dbitem.Loudness = (int)Loudness;
		dbitem.BaseBlockDifficulty = (int)DamageProfile.BaseBlockDifficulty;
		dbitem.BaseDodgeDifficulty = (int)DamageProfile.BaseDodgeDifficulty;
		dbitem.DamageType = (int)DamageProfile.DamageType;
		dbitem.BreakChanceOnHit = BreakChanceOnHit;
		dbitem.BreakChanceOnMiss = BreakChanceOnMiss;
		dbitem.DamageExpression = DamageProfile.DamageExpression.OriginalFormulaText;
		dbitem.StunExpression = DamageProfile.StunExpression.OriginalFormulaText;
		dbitem.PainExpression = DamageProfile.PainExpression.OriginalFormulaText;
		dbitem.BaseAccuracy = BaseAccuracy;
		Changed = false;
	}

	#endregion
}