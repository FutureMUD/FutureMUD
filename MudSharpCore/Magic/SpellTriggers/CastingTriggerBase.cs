using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Update.Internal;
using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.Magic.SpellTriggers;

public abstract class CastingTriggerBase : ICastMagicTrigger
{
	protected CastingTriggerBase(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		MinimumPower = (SpellPower)int.Parse(root.Element("MinimumPower").Value);
		MaximumPower = (SpellPower)int.Parse(root.Element("MaximumPower").Value);
	}

	public IMagicSpell Spell { get; }
	public SpellPower MinimumPower { get; private set; }
	public SpellPower MaximumPower { get; private set; }

	#region Implementation of IXmlSavable

	public abstract XElement SaveToXml();

	#endregion

	#region Implementation of IMagicTrigger

	public MagicTriggerType TriggerType => MagicTriggerType.CastKeyword;
	public abstract IMagicTrigger Clone();

	public abstract bool TriggerYieldsTarget { get; }

	public abstract string SubtypeBuildingCommandHelp { get; }

	public string BuildingCommandHelp => $@"You can use the following options with this trigger:
{SubtypeBuildingCommandHelp}
    minpower <power> - sets the minimum power for casting
    maxpower <power> - sets the maximum power for casting";

	public virtual bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant().CollapseString())
		{
			case "minpower":
			case "minimumpower":
				return BuildingCommandMinimumPower(actor, command);
			case "maxpower":
			case "maximumpower":
				return BuildingCommandMaximumPower(actor, command);

			default:
				actor.OutputHandler.Send(BuildingCommandHelp);
				return false;
		}
	}

	private bool BuildingCommandMaximumPower(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the maximum spell power at which this spell can be cast?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<SpellPower>(out var power))
		{
			actor.OutputHandler.Send(
				$"That is not a valid spell power. The valid values are {Enum.GetValues<SpellPower>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return false;
		}

		MaximumPower = power;
		Spell.Changed = true;
		actor.OutputHandler.Send(
			$"This spell will now be able to cast at a maximum spell power of {power.DescribeEnum().ColourValue()}.");
		return true;
	}

	private bool BuildingCommandMinimumPower(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the minimum spell power at which this spell can be cast?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<SpellPower>(out var power))
		{
			actor.OutputHandler.Send(
				$"That is not a valid spell power. The valid values are {Enum.GetValues<SpellPower>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return false;
		}

		MinimumPower = power;
		Spell.Changed = true;
		actor.OutputHandler.Send(
			$"This spell will now be able to cast at a minimum spell power of {power.DescribeEnum().ColourValue()}.");
		return true;
	}

	public virtual string Show(ICharacter actor)
	{
		return $"Power: {MinimumPower.DescribeEnum().ColourValue()}-{MaximumPower.DescribeEnum().ColourValue()}";
	}

	public abstract void DoTriggerCast(ICharacter actor, StringStack additionalArguments);

	protected bool CheckBaseTriggerCase(ICharacter actor, StringStack additionalArguments, out SpellPower power)
	{
		if (additionalArguments.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a spell power argument for that spell.");
			power = SpellPower.Insignificant;
			return false;
		}

		if (!additionalArguments.PopSpeech().TryParseEnum<SpellPower>(out power))
		{
			actor.OutputHandler.Send(
				$"That is not a valid spell power. The valid spell powers are {Enum.GetValues<SpellPower>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return false;
		}

		if (power < MinimumPower || power > MaximumPower)
		{
			actor.OutputHandler.Send(
				$"That spell can only be cast at spell powers between {MinimumPower.DescribeEnum().ColourValue()} and {MaximumPower.DescribeEnum().ColourValue()}.");
			return false;
		}

		return true;
	}

	public abstract string ShowPlayer(ICharacter actor);

	#endregion
}