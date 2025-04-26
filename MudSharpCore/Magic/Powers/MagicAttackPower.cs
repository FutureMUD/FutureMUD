using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Effects;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.Powers;

public class MagicAttackPower : MagicPowerBase, IMagicAttackPower
{
	public override string PowerType => "Magic Attack";

	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("magicattack", (power, gameworld) => new MagicAttackPower(power, gameworld));
		MagicPowerFactory.RegisterBuilderLoader("magicattack", (gameworld, school, name, actor, command) => {
			if (command.IsFinished)
			{
				actor.OutputHandler.Send("Which skill do you want to use for the skill check?");
				return null;
			}

			var skill = gameworld.Traits.GetByIdOrName(command.PopSpeech());
			if (skill is null)
			{
				actor.OutputHandler.Send("There is no such skill or attribute.");
				return null;
			}

			if (command.IsFinished)
			{
				actor.OutputHandler.Send("Which weapon attack should be associated with this power?");
				return null;
			}

			var wa = actor.Gameworld.WeaponAttacks.GetByIdOrName(command.SafeRemainingArgument);
			if (wa is null)
			{
				actor.OutputHandler.Send($"There is no such weapon attack identified by the text {command.SafeRemainingArgument.ColourCommand()}.");
				return null;
			}

			return new MagicAttackPower(gameworld, school, name, skill, wa);
		});
	}

	protected override XElement SaveDefinition()
	{
		var definition = new XElement("Definition",
			new XElement("Verb", _verb),
			new XElement("PowerIntentions", (long)PowerIntentions),
			new XElement("ValidDefenseTypes",
				from item in _validDefenseTypes
				select new XElement("Defense", (int)item)
			),
			new XElement("WeaponAttack", WeaponAttack.Id),
			new XElement("AttackerTrait", AttackerTrait.Id),
			new XElement("Reach", Reach),
			new XElement("MoveType", (int)MoveType)
		);
		return definition;
	}

	private MagicAttackPower(IFuturemud gameworld, IMagicSchool school, string name, ITraitDefinition trait, IWeaponAttack wa) : base(gameworld, school, name)
	{
		Blurb = "Use a magical attack on others";
		_showHelpText = @$"This is a magical attack. You cannot use this attack manually but to make it happen in combat you must choose a combat setting with the ""Melee Magic"" melee strategy and COMBAT CONFIG MAGIC set to a value greater than 0%."; ;
		_verb = "blast";
		_validDefenseTypes.AddRange([DefenseType.Block, DefenseType.Parry, DefenseType.Dodge]);
		PowerIntentions = CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill;
		AttackerTrait = trait;
		WeaponAttack = wa;
		Reach = 1;
		DoDatabaseInsert();
	}

	protected MagicAttackPower(Models.MagicPower power, IFuturemud gameworld) : base(power, gameworld)
	{
		var root = XElement.Parse(power.Definition);
		var element = root.Element("Verb");
		if (element == null)
		{
			throw new ApplicationException($"MagicAttackPower {Id} ({Name}) did not contain a Verb element.");
		}

		_verb = element.Value;

		element = root.Element("PowerIntentions");
		if (element == null)
		{
			throw new ApplicationException(
				$"MagicAttackPower {Id} ({Name}) did not contain a PowerIntentions element.");
		}

		if (!long.TryParse(element.Value, out var value))
		{
			var flag = CombatMoveIntentions.None;
			var split = element.Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (var item in split)
			{
				if (Utilities.TryParseEnum<CombatMoveIntentions>(item, out var sub))
				{
					flag |= sub;
					continue;
				}

				throw new ApplicationException(
					$"MagicAttackPower {Id} ({Name}) had an invalid CombatMoveIntention - {item}");
			}

			PowerIntentions = flag;
		}
		else
		{
			PowerIntentions = (CombatMoveIntentions)value;
		}

		element = root.Element("ValidDefenseTypes");
		if (element == null)
		{
			throw new ApplicationException(
				$"MagicAttackPower {Id} ({Name}) did not contain a ValidDefenseTypes element.");
		}

		foreach (var subelement in element.Elements())
		{
			if (!Utilities.TryParseEnum<DefenseType>(subelement.Value, out var defense))
			{
				throw new ApplicationException(
					$"MagicAttackPower {Id} ({Name}) had an invalid DefenseType value - {subelement.Value}");
			}

			_validDefenseTypes.Add(defense);
		}

		element = root.Element("WeaponAttack");
		if (element == null)
		{
			throw new ApplicationException($"MagicAttackPower {Id} ({Name}) did not contain a WeaponAttack element.");
		}

		WeaponAttack = long.TryParse(element.Value, out value)
			? Gameworld.WeaponAttacks.Get(value)
			: Gameworld.WeaponAttacks.GetByName(element.Value);
		if (WeaponAttack == null)
		{
			throw new ApplicationException($"MagicAttackPower {Id} ({Name}) did not have a valid WeaponAttack.");
		}

		element = root.Element("AttackerTrait");
		if (element == null)
		{
			throw new ApplicationException($"MagicAttackPower {Id} ({Name}) did not contain an AttackerTrait element.");
		}

		AttackerTrait = long.TryParse(element.Value, out value)
			? Gameworld.Traits.Get(value)
			: Gameworld.Traits.GetByName(element.Value);
		if (AttackerTrait == null)
		{
			throw new ApplicationException(
				$"MagicAttackPower {Id} ({Name}) had an AttackerTrait that did not refer to a valid TraitDefinition.");
		}

		element = root.Element("Reach");
		Reach = int.TryParse(element?.Value ?? "0", out var reach) ? reach : 0;

		element = root.Element("MoveType");
		if (element == null)
		{
			throw new ApplicationException($"MagicAttackPower {Id} ({Name}) did not have a MoveType element.");
		}

		if (!Utilities.TryParseEnum<BuiltInCombatMoveType>(element.Value, out var movetype))
		{
			throw new ApplicationException(
				$"MagicAttackPower {Id} ({Name}) had a MoveType that did not refer to a valid BuiltInCombatMoveType - {element.Value}");
		}
	}

	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		actor.OutputHandler.Send("Directly invoking the power is coming soon.");
	}

	public bool CanInvokePower(ICharacter invoker, ICharacter target)
	{
		if (CanInvokePowerProg?.Execute<bool?>(invoker, target) == false)
		{
			return false;
		}

		if (!CanAffordToInvokePower(invoker, _verb).Truth)
		{
			return false;
		}

		if (WeaponAttack.UsabilityProg?.Execute<bool?>(invoker, null, target) == false)
		{
			return false;
		}

		return true;
	}

	public void UseAttackPower(IMagicPowerAttackMove move)
	{
		ConsumePowerCosts(move.Assailant, _verb);
	}

	private string _verb;
	public override IEnumerable<string> Verbs => new[] { _verb };
	public CombatMoveIntentions PowerIntentions { get; private set; }

	private readonly List<DefenseType> _validDefenseTypes = new();
	public IEnumerable<DefenseType> ValidDefenseTypes => _validDefenseTypes;
	public ITraitDefinition AttackerTrait { get; private set; }
	public IWeaponAttack WeaponAttack { get; private set; }
	public int Reach { get; private set; }
	public BuiltInCombatMoveType MoveType => WeaponAttack.MoveType;

	/// <inheritdoc />
	protected override void ShowSubtype(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Power Verb: {_verb.ColourCommand()}");
		sb.AppendLine($"AttackerTrait Trait: {AttackerTrait.Name.ColourValue()}");
		sb.AppendLine($"Weapon Attack: {WeaponAttack.Name.MXPSend($"wa show {WeaponAttack.Id}", $"wa show {WeaponAttack.Id}")}");
		sb.AppendLine($"Move Type: {MoveType.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Valid Defenses: {ValidDefenseTypes.Select(x => x.DescribeEnum().ColourValue()).ListToString()}");
		sb.AppendLine($"Intentions: {PowerIntentions.GetSingleFlags().Select(x => x.Describe().ColourValue()).ListToString()}");
		sb.AppendLine($"Reach: {Reach.ToString("N0", actor)}");
	}

	public double BaseDelay => WeaponAttack.BaseDelay;
	public ExertionLevel ExertionLevel => WeaponAttack.ExertionLevel;
	public double StaminaCost => WeaponAttack.StaminaCost;
	public double Weighting => WeaponAttack.Weighting;
	public Orientation Orientation => WeaponAttack.Orientation;
	public Alignment Alignment => WeaponAttack.Alignment;
	public Difficulty BaseBlockDifficulty => WeaponAttack.Profile.BaseBlockDifficulty;
	public Difficulty BaseParryDifficulty => WeaponAttack.Profile.BaseParryDifficulty;
	public Difficulty BaseDodgeDifficulty => WeaponAttack.Profile.BaseDodgeDifficulty;

	#region Building Commands
	protected override string SubtypeHelpText => @"	#3verb <verb>#0 - sets the verb to manually activate this power
	#3attack <which>#0 - sets the weapon attack associated with this power
	#3reach <##>#0 - sets the reach of this attack
	#3trait <which>#0 - sets the attack trait used with this power
	#3intention <which>#0 - toggles an intention used with this power
	#3defense <which>#0 - toggles a defense type that is usable against this attack";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "skill":
			case "trait":
				return BuildingCommandTrait(actor, command);
			case "reach":
				return BuildingCommandReach(actor, command);
			case "intention":
			case "intentions":
				return BuildingCommandIntentions(actor, command);
			case "attack":
				return BuildingCommandAttack(actor, command);
			case "defense":
			case "defence":
			case "defences":
			case "defenses":
				return BuildingCommandDefence(actor, command);
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandDefence(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which defense type do you want to toggle?\nThe valid types are {Enum.GetValues<DefenseType>().ListToColouredString()}.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<DefenseType>(out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid defense type.\nThe valid types are {Enum.GetValues<DefenseType>().ListToColouredString()}.");
			return false;
		}

		if (value == DefenseType.None)
		{
			_validDefenseTypes.Clear();
			Changed = true;
			actor.OutputHandler.Send("There will no longer be any defense against this attack.");
			return false;
		}

		if (_validDefenseTypes.Contains(value))
		{
			_validDefenseTypes.Remove(value);
			Changed = true;
			actor.OutputHandler.Send($"This attack can no longer be defended against by the {value.DescribeEnum().ColourValue()} defense type.");
			return true;
		}

		_validDefenseTypes.Add(value);
		Changed = true;
		actor.OutputHandler.Send($"This attack can now be defended against by the {value.DescribeEnum().ColourValue()} defense type.");
		return true;
	}

	private bool BuildingCommandAttack(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which weapon attack do you want this power to be associated with?");
			return false;
		}

		var wa = Gameworld.WeaponAttacks.GetByIdOrName(command.SafeRemainingArgument);
		if (wa is null)
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid weapon attack.");
			return false;
		}

		if (!wa.MoveType.IsMagicAttack())
		{
			actor.OutputHandler.Send($"The weapon attack {wa.Name.ColourName()} (#{wa.Id.ToStringN0(actor)}) is not a valid magic attack.");
			return false;
		}

		WeaponAttack = wa;
		Changed = true;
		actor.OutputHandler.Send($"This power will now use the weapon attack {wa.Name.ColourName()} (#{wa.Id.ToStringN0(actor)}).");
		return true;
	}

	private bool BuildingCommandIntentions(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which combat move intention do you want to toggle?\nValid values are {Enum.GetValues<CombatMoveIntentions>().ListToColouredString()}.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<CombatMoveIntentions>(out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid combat move intention.\nValid values are {Enum.GetValues<CombatMoveIntentions>().ListToColouredString()}.");
			return false;
		}

		if (PowerIntentions.HasFlag(value))
		{
			PowerIntentions &= ~value;
			Changed = true;
			actor.OutputHandler.Send($"This power no longer has the {value.DescribeEnum().ColourName()} intention.");
			return true;
		}

		PowerIntentions |= value;
		Changed = true;
		actor.OutputHandler.Send($"This power now has the {value.DescribeEnum().ColourName()} intention.");
		return true;
	}

	private bool BuildingCommandReach(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the reach of this weapon?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value < 0)
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid number zero or greater.");
			return false;
		}

		Reach = value;
		Changed = true;
		actor.OutputHandler.Send($"This attack now has a reach of {value.ToStringN0Colour(actor)}.");
		return true;
	}

	private bool BuildingCommandTrait(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which trait should be used for attack rolls with this power?");
			return false;
		}

		var trait = Gameworld.Traits.GetByIdOrName(command.SafeRemainingArgument);
		if (trait is null)
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid trait.");
			return false;
		}

		AttackerTrait = trait;
		Changed = true;
		actor.OutputHandler.Send($"This power will now use the trait {trait.Name.ColourValue()} for attack rolls.");
		return true;
	}

	#region Building SubCommands
	#endregion

	#endregion
}