using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Commands.Trees;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat;

public class WeaponType : SaveableItem, IWeaponType
{
	public WeaponType(IFuturemud game, string name)
	{
		Gameworld = game;
		_name = name;
		_classification = WeaponClassification.Lethal;
		_parryBonus = 0.0;
		_reach = 1;
		_staminaPerParry = Gameworld.GetStaticDouble("DefaultParryStaminaCost");
		using (new FMDB())
		{
			var dbitem = new Models.WeaponType
			{
				Name = Name,
				Classification = (int)Classification,
				AttackTraitId = AttackTrait?.Id,
				ParryTraitId = ParryTrait?.Id,
				ParryBonus = ParryBonus,
				Reach = Reach,
				StaminaPerParry = StaminaPerParry
			};
			FMDB.Context.WeaponTypes.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public WeaponType(MudSharp.Models.WeaponType type, IFuturemud game)
	{
		Gameworld = game;
		_id = type.Id;
		_name = type.Name;
		_classification = (WeaponClassification)type.Classification;
		_attackTrait = game.Traits.Get(type.AttackTraitId ?? 0);
		_parryTrait = game.Traits.Get(type.ParryTraitId ?? 0);
		_parryBonus = type.ParryBonus;
		_reach = type.Reach;
		_staminaPerParry = type.StaminaPerParry;
		foreach (var attack in type.WeaponAttacks)
		{
			_attacks.Add(Gameworld.WeaponAttacks.Get(attack.Id));
		}
	}

	public WeaponType(WeaponType rhs, string newName)
	{
		Gameworld = rhs.Gameworld;
		_name = newName;
		_classification = rhs.Classification;
		_attackTrait = rhs.AttackTrait;
		_parryTrait = rhs.ParryTrait;
		_parryBonus = rhs.ParryBonus;
		_reach = rhs.Reach;
		_staminaPerParry = rhs.StaminaPerParry;

		using (new FMDB())
		{
			var dbitem = new Models.WeaponType
			{
				Name = Name,
				Classification = (int)Classification,
				AttackTraitId = AttackTrait?.Id,
				ParryTraitId = ParryTrait?.Id,
				ParryBonus = ParryBonus,
				Reach = Reach,
				StaminaPerParry = StaminaPerParry
			};
			FMDB.Context.WeaponTypes.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;

			var newAttacks = new List<Models.WeaponAttack>();
			foreach (var attack in rhs.Attacks)
			{
				var dbattack = FMDB.Context.WeaponAttacks.Find(attack.Id);
				if (dbattack is null)
				{
					continue;
				}

				var newdbattack = new Models.WeaponAttack
				{
					WeaponTypeId = _id,
					Verb = dbattack.Verb,
					FutureProgId = dbattack.FutureProgId,
					BaseAttackerDifficulty = dbattack.BaseAttackerDifficulty,
					BaseBlockDifficulty = dbattack.BaseBlockDifficulty,
					BaseDodgeDifficulty = dbattack.BaseDodgeDifficulty,
					BaseParryDifficulty = dbattack.BaseParryDifficulty,
					BaseAngleOfIncidence = dbattack.BaseAngleOfIncidence,
					RecoveryDifficultySuccess = dbattack.RecoveryDifficultySuccess,
					RecoveryDifficultyFailure = dbattack.RecoveryDifficultyFailure,
					MoveType = dbattack.MoveType,
					Intentions = dbattack.Intentions,
					ExertionLevel = dbattack.ExertionLevel,
					DamageType = dbattack.DamageType,
					DamageExpressionId = dbattack.DamageExpressionId,
					StunExpressionId = dbattack.StunExpressionId,
					PainExpressionId = dbattack.PainExpressionId,
					Weighting = dbattack.Weighting,
					BodypartShapeId = dbattack.BodypartShapeId,
					StaminaCost = dbattack.StaminaCost,
					BaseDelay = dbattack.BaseDelay,
					Name = dbattack.Name,
					Orientation = dbattack.Orientation,
					Alignment = dbattack.Alignment,
					AdditionalInfo = dbattack.AdditionalInfo,
					HandednessOptions = dbattack.HandednessOptions,
					RequiredPositionStateIds = dbattack.RequiredPositionStateIds
				};
				newAttacks.Add(newdbattack);
				foreach (var message in dbattack.CombatMessagesWeaponAttacks)
				{
					newdbattack.CombatMessagesWeaponAttacks.Add(new CombatMessagesWeaponAttacks
					{
						WeaponAttack = newdbattack,
						CombatMessageId = message.CombatMessageId
					});
				}

				FMDB.Context.WeaponAttacks.Add(newdbattack);
			}

			FMDB.Context.SaveChanges();
			foreach (var attack in newAttacks)
			{
				var gattack = WeaponAttack.LoadWeaponAttack(attack, Gameworld);
				Gameworld.Add(gattack);
				_attacks.Add(gattack);
			}
		}
	}

	public IWeaponType Clone(string newName)
	{
		return new WeaponType(this, newName);
	}

	#region Overrides of Item

	public override string FrameworkItemType { get; } = "WeaponType";

	#endregion

	#region Overrides of SaveableItem

	/// <summary>
	///     Tells the object to perform whatever save action it needs to do
	/// </summary>
	public override void Save()
	{
		Changed = false;
		using (new FMDB())
		{
			var dbitem = FMDB.Context.WeaponTypes.Find(Id);
			dbitem.Name = Name.TitleCase();
			dbitem.ParryBonus = (int)ParryBonus;
			dbitem.Reach = Reach;
			dbitem.AttackTraitId = AttackTrait?.Id;
			dbitem.ParryTraitId = ParryTrait?.Id;
			dbitem.Classification = (int)Classification;
			dbitem.StaminaPerParry = StaminaPerParry;
			var attackIds = _attacks.Select(x => x.Id).ToHashSet();
			foreach (var attack in FMDB.Context.WeaponAttacks)
			{
				if (attackIds.Contains(attack.Id))
				{
					attack.WeaponTypeId = _id;
				}
				else if (attack.WeaponTypeId == _id)
				{
					attack.WeaponTypeId = null;
				}
			}

			FMDB.Context.SaveChanges();
		}
	}

	#endregion

	#region Implementation of IWeaponType

	public void AddAttack(IWeaponAttack attack)
	{
		_attacks.Add(attack);
		Changed = true;
	}

	public void RemoveAttack(IWeaponAttack attack)
	{
		_attacks.Remove(attack);
		Changed = true;
	}

	public const string HelpText = @"You can use the following options when editing this weapon type:

	#3name <name>#0 - the name of this weapon type
	#3classification <which>#0 - changes the classification of this weapon for law enforcement
	#3skill <which>#0 - sets the skill which this weapon uses
	#3parry <which>#0 - sets the skill which this weapon parries with
	#3bonus <number>#0 - the bonus/penalty to parrying with this weapon
	#3reach <number>#0 - sets the reach of the weapon
	#3stamina <cost>#0 - how much stamina it takes to parry with this weapon";

	/// <inheritdoc />
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "classification":
			case "class":
				return BuildingCommandClassification(actor, command);
			case "attack":
			case "attacktrait":
			case "use":
			case "usetrait":
			case "trait":
			case "skill":
			case "attackskill":
			case "useskill":
				return BuildingCommandAttackSkill(actor, command);
			case "parry":
			case "parryskill":
			case "parrytrait":
				return BuildingCommandParrySkill(actor, command);
			case "parrybonus":
			case "bonus":
				return BuildingCommandParryBonus(actor, command);
			case "reach":
				return BuildingCommandReach(actor, command);
			case "stamina":
				return BuildingCommandStamina(actor, command);
			default:
				actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandStamina(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How much stamina should it take to parry with this weapon?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value) || value < 0.0)
		{
			actor.OutputHandler.Send(
				$"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid amount of stamina.");
			return false;
		}

		StaminaPerParry = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"It will now cost {value.ToString("N2", actor).ColourValue()} stamina to parry with this weapon.");
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to rename this weapon type?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Gameworld.WeaponTypes.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				$"There is already a weapon type called {name.ColourName()}. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You rename the weapon type {Name.ColourName()} to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	private bool BuildingCommandClassification(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What classification do you want to give weapons of this type? Valid options are {Enum.GetValues<WeaponClassification>().Select(x => x.Describe().ColourName()).ListToString()}.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<WeaponClassification>(out var value) ||
		    value == WeaponClassification.None)
		{
			actor.OutputHandler.Send(
				$"That is not a valid classification. The valid options are {Enum.GetValues<WeaponClassification>().Except(WeaponClassification.None).Select(x => x.Describe().ColourName()).ListToString()}.");
			return false;
		}

		Classification = value;
		Changed = true;
		actor.OutputHandler.Send($"This weapon type is now classified as {value.Describe().ColourName()}.");
		return true;
	}

	private bool BuildingCommandAttackSkill(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which skill should this weapon type use for its attacks?");
			return false;
		}

		var skill = Gameworld.Traits.GetByIdOrName(command.SafeRemainingArgument);
		if (skill is null)
		{
			actor.OutputHandler.Send("There is no such skill or trait.");
			return false;
		}

		AttackTrait = skill;
		Changed = true;
		actor.OutputHandler.Send(
			$"This weapon type will now use the {skill.Name.ColourValue()} trait for the outcome of its attacks.");
		return true;
	}

	private bool BuildingCommandParrySkill(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which skill should this weapon type use for parrying?");
			return false;
		}

		var skill = Gameworld.Traits.GetByIdOrName(command.SafeRemainingArgument);
		if (skill is null)
		{
			actor.OutputHandler.Send("There is no such skill or trait.");
			return false;
		}

		ParryTrait = skill;
		Changed = true;
		actor.OutputHandler.Send($"This weapon type will now use the {skill.Name.ColourValue()} trait for parrying.");
		return true;
	}

	private bool BuildingCommandParryBonus(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the bonus to parry checks made with this weapon type?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send(
				$"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid number.");
			return false;
		}

		ParryBonus = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This weapon type now has a bonus of {value.ToBonusString(actor)} for parrying.\nThis would make a {Difficulty.Normal.DescribeColoured()} check into {Difficulty.Normal.ApplyBonus(value).DescribeColoured()}.");
		return true;
	}

	private bool BuildingCommandReach(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How much reach should this weapon have by default?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value < 0)
		{
			actor.OutputHandler.Send("You must enter a whole number zero or greater.");
			return false;
		}

		Reach = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This weapon type will now have a reach of {value.ToString("N0", actor).ColourValue()}.");
		return true;
	}

	public string Show(ICharacter character)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Weapon Type: {Name.TitleCase().Colour(Telnet.Green)} (#{Id.ToString("N0", character)})");
		sb.AppendLine();
		sb.AppendLine($"Classification: {Classification.Describe().Colour(Telnet.Green)}");
		sb.AppendLine(
			$"Attack Trait: {AttackTrait?.Name.TitleCase().Colour(Telnet.Green) ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine(
			$"Parry Trait: {ParryTrait?.Name.TitleCase().Colour(Telnet.Green) ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine($"Parry Bonus: {ParryBonus.ToString("N0", character).ColourValue()}");
		sb.AppendLine($"Reach: {Reach.ToString("N0", character).ColourValue()}");
		sb.AppendLine();
		sb.AppendLine("Attacks:");
		sb.Append(StringUtilities.GetTextTable(
			from attack in Attacks
			let mindam = attack.Profile.DamageExpression.EvaluateWith(character, AttackTrait,
				TraitBonusContext.ArmedDamageCalculation, new (string, object)[]
				{
					("quality", 5),
					("degree", 0)
				})
			let maxdam = attack.Profile.DamageExpression.EvaluateWith(character, AttackTrait,
				TraitBonusContext.ArmedDamageCalculation, new (string, object)[]
				{
					("quality", 5),
					("degree", 5)
				})
			select new List<string>
			{
				attack.Id.ToString("N0", character),
				attack.Name,
				attack.MoveType.DescribeEnum(),
				attack.Profile.DamageType.Describe(),
				$"{attack.Profile.BaseAttackerDifficulty.DescribeColoured()} vs {attack.Profile.BaseDodgeDifficulty.DescribeColoured()}/{attack.Profile.BaseBlockDifficulty.DescribeColoured()}/{attack.Profile.BaseParryDifficulty.DescribeColoured()}",
				attack.StaminaCost.ToString("N2", character),
				$"{mindam.ToString("N1", character)} - {maxdam.ToString("N1", character)}"
			},
			new List<string>
			{
				"Id",
				"Name",
				"Attack Type",
				"Damage Type",
				"Difficulty (Att vs Do/Bl/Pa)",
				"Stamina",
				"Damage"
			},
			character,
			Telnet.Blue
		));
		sb.AppendLine(
			$"Note: Damages are shown with your stats and a {ItemQuality.Standard.Describe().ColourValue()} quality weapon.");
		return sb.ToString();
	}

	private WeaponClassification _classification;

	public WeaponClassification Classification
	{
		get => _classification;
		set
		{
			_classification = value;
			Changed = true;
		}
	}

	private readonly List<IWeaponAttack> _attacks = new();
	public IEnumerable<IWeaponAttack> Attacks => _attacks;

	public IEnumerable<IWeaponAttack> UsableAttacks(IPerceiver attacker, IGameItem weapon, IPerceiver target,
		AttackHandednessOptions handedness,
		bool ignorePosition,
		params BuiltInCombatMoveType[] types)
	{
		return _attacks.Where(x => x.UsableAttack(attacker, weapon, target, handedness, ignorePosition, types))
		               .ToList();
	}

	public IEnumerable<AttackHandednessOptions> UseableHandednessOptions(ICharacter attacker, IGameItem weapon,
		IPerceiver target,
		params BuiltInCombatMoveType[] types)
	{
		return _attacks.Where(x => x.UsableAttack(attacker, weapon, target, AttackHandednessOptions.Any, true, types))
		               .Select(x => x.HandednessOptions).Distinct().ToList();
	}

	private ITraitDefinition _attackTrait;

	public ITraitDefinition AttackTrait
	{
		get => _attackTrait;
		set
		{
			_attackTrait = value;
			Changed = true;
		}
	}

	private double _staminaPerParry;

	public double StaminaPerParry
	{
		get => _staminaPerParry;
		set
		{
			_staminaPerParry = value;
			Changed = true;
		}
	}

	private ITraitDefinition _parryTrait;

	public ITraitDefinition ParryTrait
	{
		get => _parryTrait;
		set
		{
			_parryTrait = value;
			Changed = true;
		}
	}

	private int _reach;

	public int Reach
	{
		get => _reach;
		set
		{
			_reach = value;
			Changed = true;
		}
	}

	private double _parryBonus;

	public double ParryBonus
	{
		get => _parryBonus;
		set
		{
			_parryBonus = value;
			Changed = true;
		}
	}

	#endregion
}