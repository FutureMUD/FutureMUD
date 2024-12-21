using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.Powers;

public class MindLookPower : MagicPowerBase
{
	public override string PowerType => "Mind Look";
	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("mindlook", (power, gameworld) => new MindLookPower(power, gameworld));
	}

	protected override XElement SaveDefinition()
	{
		var definition = new XElement("Definition",
			new XElement("LookRoomVerb", LookRoomVerb),
			new XElement("LookThingVerb", LookThingVerb),
			new XElement("LookInThingVerb", LookInThingVerb),
			new XElement("EmoteText", new XCData(EmoteText)),
			new XElement("FailEmoteText", new XCData(FailEmoteText)),
			new XElement("SkillCheckDifficultyLookRoom", (int)SkillCheckDifficultyLookRoom),
			new XElement("SkillCheckDifficultyLookThing", (int)SkillCheckDifficultyLookThing),
			new XElement("SkillCheckDifficultyLookInThing", (int)SkillCheckDifficultyLookInThing),
			new XElement("MinimumSuccessThresholdLookRoom", (int)MinimumSuccessThresholdLookRoom),
			new XElement("MinimumSuccessThresholdLookThing", (int)MinimumSuccessThresholdLookThing),
			new XElement("MinimumSuccessThresholdLookInThing", (int)MinimumSuccessThresholdLookInThing),
			new XElement("SkillCheckTrait", SkillCheckTrait.Id)
		);
		return definition;
	}

	protected MindLookPower(MagicPower power, IFuturemud gameworld) : base(power, gameworld)
	{
		var root = XElement.Parse(power.Definition);
		var element = root.Element("LookRoomVerb");
		if (element == null)
		{
			throw new ApplicationException($"The MindLookPower #{Id} ({Name}) was missing a LookRoomVerb element.");
		}

		LookRoomVerb = element.Value.ToLowerInvariant();

		element = root.Element("LookThingVerb");
		if (element == null)
		{
			throw new ApplicationException($"The MindLookPower #{Id} ({Name}) was missing a LookThingVerb element.");
		}

		LookThingVerb = element.Value.ToLowerInvariant();

		element = root.Element("LookInThingVerb");
		if (element == null)
		{
			throw new ApplicationException($"The MindLookPower #{Id} ({Name}) was missing a LookInThingVerb element.");
		}

		LookInThingVerb = element.Value.ToLowerInvariant();

		element = root.Element("EmoteText");
		if (element == null)
		{
			throw new ApplicationException($"The MindLookPower #{Id} ({Name}) was missing a EmoteText element.");
		}

		EmoteText = element.Value.ToLowerInvariant();

		element = root.Element("FailEmoteText");
		if (element == null)
		{
			throw new ApplicationException($"The MindLookPower #{Id} ({Name}) was missing a FailEmoteText element.");
		}

		FailEmoteText = element.Value.ToLowerInvariant();

		element = root.Element("SkillCheckDifficultyLookRoom");
		if (element == null)
		{
			throw new ApplicationException(
				$"The MindLookPower #{Id} ({Name}) was missing a SkillCheckDifficultyLookRoom element.");
		}

		if (!int.TryParse(element.Value, out var ivalue))
		{
			if (!CheckExtensions.GetDifficulty(element.Value, out var diff))
			{
				throw new ApplicationException(
					$"The MindLookPower #{Id} ({Name}) had a SkillCheckDifficultyLookRoom value that did not map to a valid Difficulty.");
			}

			SkillCheckDifficultyLookRoom = diff;
		}
		else
		{
			SkillCheckDifficultyLookRoom = (Difficulty)ivalue;
		}

		element = root.Element("SkillCheckDifficultyLookThing");
		if (element == null)
		{
			throw new ApplicationException(
				$"The MindLookPower #{Id} ({Name}) was missing a SkillCheckDifficultyLookThing element.");
		}

		if (!int.TryParse(element.Value, out ivalue))
		{
			if (!CheckExtensions.GetDifficulty(element.Value, out var diff))
			{
				throw new ApplicationException(
					$"The MindLookPower #{Id} ({Name}) had a SkillCheckDifficultyLookThing value that did not map to a valid Difficulty.");
			}

			SkillCheckDifficultyLookThing = diff;
		}
		else
		{
			SkillCheckDifficultyLookThing = (Difficulty)ivalue;
		}

		element = root.Element("SkillCheckDifficultyLookInThing");
		if (element == null)
		{
			throw new ApplicationException(
				$"The MindLookPower #{Id} ({Name}) was missing a SkillCheckDifficultyLookInThing element.");
		}

		if (!int.TryParse(element.Value, out ivalue))
		{
			if (!CheckExtensions.GetDifficulty(element.Value, out var diff))
			{
				throw new ApplicationException(
					$"The MindLookPower #{Id} ({Name}) had a SkillCheckDifficultyLookInThing value that did not map to a valid Difficulty.");
			}

			SkillCheckDifficultyLookInThing = diff;
		}
		else
		{
			SkillCheckDifficultyLookInThing = (Difficulty)ivalue;
		}

		element = root.Element("MinimumSuccessThresholdLookRoom");
		if (element == null)
		{
			throw new ApplicationException(
				$"The MindLookPower #{Id} ({Name}) was missing a MinimumSuccessThresholdLookRoom element.");
		}

		if (!int.TryParse(element.Value, out ivalue))
		{
			if (!CheckExtensions.GetOutcome(element.Value, out var outcome))
			{
				throw new ApplicationException(
					$"The MindLookPower #{Id} ({Name}) had a MinimumSuccessThresholdLookRoom value that did not map to a valid Outcome.");
			}

			MinimumSuccessThresholdLookRoom = outcome;
		}
		else
		{
			MinimumSuccessThresholdLookRoom = (Outcome)ivalue;
		}

		element = root.Element("MinimumSuccessThresholdLookThing");
		if (element == null)
		{
			throw new ApplicationException(
				$"The MindLookPower #{Id} ({Name}) was missing a MinimumSuccessThresholdLookThing element.");
		}

		if (!int.TryParse(element.Value, out ivalue))
		{
			if (!CheckExtensions.GetOutcome(element.Value, out var outcome))
			{
				throw new ApplicationException(
					$"The MindLookPower #{Id} ({Name}) had a MinimumSuccessThresholdLookThing value that did not map to a valid Outcome.");
			}

			MinimumSuccessThresholdLookThing = outcome;
		}
		else
		{
			MinimumSuccessThresholdLookThing = (Outcome)ivalue;
		}

		element = root.Element("MinimumSuccessThresholdLookInThing");
		if (element == null)
		{
			throw new ApplicationException(
				$"The MindLookPower #{Id} ({Name}) was missing a MinimumSuccessThresholdLookInThing element.");
		}

		if (!int.TryParse(element.Value, out ivalue))
		{
			if (!CheckExtensions.GetOutcome(element.Value, out var outcome))
			{
				throw new ApplicationException(
					$"The MindLookPower #{Id} ({Name}) had a MinimumSuccessThresholdLookInThing value that did not map to a valid Outcome.");
			}

			MinimumSuccessThresholdLookInThing = outcome;
		}
		else
		{
			MinimumSuccessThresholdLookInThing = (Outcome)ivalue;
		}

		element = root.Element("SkillCheckTrait");
		if (element == null)
		{
			throw new ApplicationException($"The MindLookPower #{Id} ({Name}) was missing a SkillCheckTrait element.");
		}

		var trait = long.TryParse(element.Value, out var value)
			? Gameworld.Traits.Get(value)
			: Gameworld.Traits.GetByName(element.Value);

		SkillCheckTrait = trait ?? throw new ApplicationException(
			$"The MindLookPower #{Id} ({Name}) had a SkillCheckTrait element that pointed to a null Trait.");
	}

	private enum UseCommandVerb
	{
		Room,
		Thing,
		InThing
	}

	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		var (truth, missing) = CanAffordToInvokePower(actor, verb);
		if (!truth)
		{
			actor.OutputHandler.Send(
				$"You can't do that because you lack sufficient {missing.Name.Colour(Telnet.BoldMagenta)}.");
			return;
		}

		var effect = actor.EffectsOfType<ConnectMindEffect>().FirstOrDefault(x => x.School == School);
		if (effect == null)
		{
			actor.OutputHandler.Send(
				$"Your mind isn't connected to the mind of anyone else via {School.Name.Colour(School.PowerListColour)}.");
			return;
		}

		;

		if (CanInvokePowerProg.ExecuteBool(actor, effect.TargetCharacter) == false)
		{
			actor.OutputHandler.Send(string.Format(
				WhyCantInvokePowerProg.Execute(actor, effect.TargetCharacter)?.ToString() ??
				"You cannot send any messages to {0}.", effect.TargetCharacter.HowSeen(actor)));
			return;
		}

		var check = Gameworld.GetCheck(CheckType.MindLookPower);
		Difficulty difficulty;
		Outcome threshold;
		UseCommandVerb everb;
		if (verb.EqualTo(LookRoomVerb))
		{
			difficulty = SkillCheckDifficultyLookRoom;
			threshold = MinimumSuccessThresholdLookRoom;
			everb = UseCommandVerb.Room;
		}
		else if (verb.EqualTo(LookThingVerb))
		{
			difficulty = SkillCheckDifficultyLookThing;
			threshold = MinimumSuccessThresholdLookThing;
			everb = UseCommandVerb.Thing;
		}
		else
		{
			difficulty = SkillCheckDifficultyLookInThing;
			threshold = MinimumSuccessThresholdLookInThing;
			everb = UseCommandVerb.InThing;
		}

		var outcome = check.Check(actor, difficulty, SkillCheckTrait, effect.TargetCharacter);
		if (outcome < threshold)
		{
			actor.OutputHandler.Send(new EmoteOutput(new Emote(FailEmoteText, actor, actor, effect.TargetCharacter)));
			return;
		}

		switch (everb)
		{
			case UseCommandVerb.Room:
				if (!string.IsNullOrEmpty(EmoteText))
				{
					actor.OutputHandler.Send(
						new EmoteOutput(new Emote(EmoteText, actor, actor, effect.TargetCharacter)));
				}

				actor.OutputHandler.Send(effect.TargetCharacter.Body.LookText(false), false, true);
				return;
			default:
				if (command.IsFinished)
				{
					actor.OutputHandler.Send("You must specify a target thing to use that power on.");
					return;
				}

				var target = effect.TargetCharacter.Target(command.PopSpeech());
				if (target == null)
				{
					actor.OutputHandler.Send("You cannot see anything like that which your target can also see.");
					return;
				}

				if (!string.IsNullOrEmpty(EmoteText))
				{
					actor.OutputHandler.Send(
						new EmoteOutput(new Emote(EmoteText, actor, actor, effect.TargetCharacter)));
				}

				if (everb == UseCommandVerb.InThing)
				{
					actor.OutputHandler.Send(effect.TargetCharacter.Body.LookInText(target), true, false);
					return;
				}

				actor.OutputHandler.Send(effect.TargetCharacter.Body.LookText(target), true, true);
				break;
		}
	}

	public override IEnumerable<string> Verbs => new[]
	{
		LookRoomVerb,
		LookThingVerb,
		LookInThingVerb
	};

	public string LookRoomVerb { get; protected set; }
	public string LookThingVerb { get; protected set; }
	public string LookInThingVerb { get; protected set; }

	public Difficulty SkillCheckDifficultyLookRoom { get; protected set; }
	public Difficulty SkillCheckDifficultyLookThing { get; protected set; }
	public Difficulty SkillCheckDifficultyLookInThing { get; protected set; }
	public ITraitDefinition SkillCheckTrait { get; protected set; }
	public Outcome MinimumSuccessThresholdLookRoom { get; protected set; }
	public Outcome MinimumSuccessThresholdLookThing { get; protected set; }
	public Outcome MinimumSuccessThresholdLookInThing { get; protected set; }

	public string EmoteText { get; set; }
	public string FailEmoteText { get; set; }

	protected override void ShowSubtype(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Look Room Verb: {LookRoomVerb.ColourCommand()}");
		sb.AppendLine($"Look Thing Verb: {LookThingVerb.ColourCommand()}");
		sb.AppendLine($"Look In Thing Verb: {LookInThingVerb.ColourCommand()}");
		sb.AppendLine($"Skill Check Trait: {SkillCheckTrait.Name.ColourValue()}");
		sb.AppendLine($"Look Room Difficulty: {SkillCheckDifficultyLookRoom.DescribeColoured()}");
		sb.AppendLine($"Look Thing Success Threshold: {MinimumSuccessThresholdLookRoom.DescribeColour()}");
		sb.AppendLine($"Look Thing Success Threshold: {MinimumSuccessThresholdLookRoom.DescribeColour()}");
		sb.AppendLine($"Look In Thing Success Threshold: {MinimumSuccessThresholdLookRoom.DescribeColour()}");
		sb.AppendLine();
		sb.AppendLine("Emotes:");
		sb.AppendLine();
		sb.AppendLine($"Emote: {EmoteText.ColourCommand()}");
		sb.AppendLine($"Fail Emote: {FailEmoteText.ColourCommand()}");
	}

	#region Building Commands
	/// <inheritdoc />
	protected override string SubtypeHelpText => @"	#3roomverb <verb>#0 - sets the verb to activate this power on a room
	#3thingverb <verb>#0 - sets the verb to activate this power on a thing
	#3inthingverb <verb>#0 - sets the verb to activate this power to look in a thing
	#3difficultyroom <difficulty>#0 - sets the difficulty of the skill check for rooms
	#3difficultything <difficulty>#0 - sets the difficulty of the skill check for things
	#3difficultyinthing <difficulty>#0 - sets the difficulty of the skill check for looking in things
	#3thresholdroom <outcome>#0 - sets the minimum outcome for skill success for rooms
	#3thresholdthing <outcome>#0 - sets the minimum outcome for skill success for things
	#3thresholdinthing <outcome>#0 - sets the minimum outcome for skill success for looking in things
	#3skill <which>#0 - sets the skill used in the skill check";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "lookroomverb":
			case "roomverb":
				return BuildingCommandLookRoomVerb(actor, command);
			case "lookthingverb":
			case "thingverb":
				return BuildingCommandLookThingVerb(actor, command);
			case "lookinthingverb":
			case "inthingverb":
				return BuildingCommandLookInThingVerb(actor, command);
			case "skill":
			case "trait":
				return BuildingCommandSkill(actor, command);
			case "difficultyroom":
				return BuildingCommandDifficultyRoom(actor, command);
			case "difficultything":
				return BuildingCommandDifficultyThing(actor, command);
			case "difficultyinthing":
				return BuildingCommandDifficultyInThing(actor, command);
			case "thresholdroom":
				return BuildingCommandThresholdRoom(actor, command);
			case "thresholdthing":
				return BuildingCommandThresholdThing(actor, command);
			case "thresholdinthing":
				return BuildingCommandThresholdInThing(actor, command);
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	#region Building Subcommands

	private bool BuildingCommandThresholdRoom(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What is the minimum success threshold for this power to work on a room? See {"show outcomes".MXPSend("show outcomes")} for a list of valid values.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum(out Outcome value))
		{
			actor.OutputHandler.Send($"That is not a valid outcome. See {"show outcomes".MXPSend("show outcomes")} for a list of valid values.");
			return false;
		}

		MinimumSuccessThresholdLookRoom = value;
		Changed = true;
		actor.OutputHandler.Send($"The power user will now need to achieve a {value.DescribeColour()} in order to activate this power to look in a room.");
		return true;
	}

	private bool BuildingCommandThresholdThing(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What is the minimum success threshold for this power to work on a thing? See {"show outcomes".MXPSend("show outcomes")} for a list of valid values.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum(out Outcome value))
		{
			actor.OutputHandler.Send($"That is not a valid outcome. See {"show outcomes".MXPSend("show outcomes")} for a list of valid values.");
			return false;
		}

		MinimumSuccessThresholdLookThing = value;
		Changed = true;
		actor.OutputHandler.Send($"The power user will now need to achieve a {value.DescribeColour()} in order to activate this power to look at a thing.");
		return true;
	}

	private bool BuildingCommandThresholdInThing(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What is the minimum success threshold for this power to work looking in a thing? See {"show outcomes".MXPSend("show outcomes")} for a list of valid values.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum(out Outcome value))
		{
			actor.OutputHandler.Send($"That is not a valid outcome. See {"show outcomes".MXPSend("show outcomes")} for a list of valid values.");
			return false;
		}

		MinimumSuccessThresholdLookInThing = value;
		Changed = true;
		actor.OutputHandler.Send($"The power user will now need to achieve a {value.DescribeColour()} in order to activate this power to look in a thing.");
		return true;
	}

	private bool BuildingCommandDifficultyRoom(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What difficulty should the skill check for this power be to look at a room? See {"show difficulties".MXPSend("show difficulties")} for a list of values.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum(out Difficulty value))
		{
			actor.OutputHandler.Send($"That is not a valid difficulty. See {"show difficulties".MXPSend("show difficulties")} for a list of values.");
			return false;
		}

		SkillCheckDifficultyLookRoom = value;
		Changed = true;
		actor.OutputHandler.Send($"This power's skill check will now be at a difficulty of {value.DescribeColoured()} for rooms.");
		return true;
	}

	private bool BuildingCommandDifficultyThing(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What difficulty should the skill check for this power be to look at a thing? See {"show difficulties".MXPSend("show difficulties")} for a list of values.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum(out Difficulty value))
		{
			actor.OutputHandler.Send($"That is not a valid difficulty. See {"show difficulties".MXPSend("show difficulties")} for a list of values.");
			return false;
		}

		SkillCheckDifficultyLookThing = value;
		Changed = true;
		actor.OutputHandler.Send($"This power's skill check will now be at a difficulty of {value.DescribeColoured()} for things.");
		return true;
	}

	private bool BuildingCommandDifficultyInThing(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What difficulty should the skill check for this power be to look in a thing? See {"show difficulties".MXPSend("show difficulties")} for a list of values.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum(out Difficulty value))
		{
			actor.OutputHandler.Send($"That is not a valid difficulty. See {"show difficulties".MXPSend("show difficulties")} for a list of values.");
			return false;
		}

		SkillCheckDifficultyLookInThing = value;
		Changed = true;
		actor.OutputHandler.Send($"This power's skill check will now be at a difficulty of {value.DescribeColoured()} for looking in things.");
		return true;
	}

	private bool BuildingCommandSkill(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which skill or trait should be used for this power's skill check?");
			return false;
		}

		var skill = Gameworld.Traits.GetByIdOrName(command.SafeRemainingArgument);
		if (skill is null)
		{
			actor.OutputHandler.Send("That is not a valid skill or trait.");
			return false;
		}

		SkillCheckTrait = skill;
		Changed = true;
		actor.OutputHandler.Send($"This magic power will now use the {skill.Name.ColourName()} skill for its skill check.");
		return true;
	}

	private bool BuildingCommandLookRoomVerb(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which verb should be used to activate this power to look at a room?");
			return false;
		}

		var verb = command.SafeRemainingArgument.ToLowerInvariant();
		if (LookThingVerb.EqualTo(verb) || LookInThingVerb.EqualTo(verb))
		{
			actor.OutputHandler.Send("The Look Room, Look Thing and Look In Things verbs must all be different to one another.");
			return false;
		}

		var costs = InvocationCosts[LookRoomVerb].ToList();
		InvocationCosts[verb] = costs;
		InvocationCosts.Remove(LookRoomVerb);
		LookRoomVerb = verb;
		Changed = true;
		actor.OutputHandler.Send($"This magic power will now use the verb {verb.ColourCommand()} to invoke the power as a look room.");
		return true;
	}

	private bool BuildingCommandLookThingVerb(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which verb should be used to activate this power to look at a thing?");
			return false;
		}

		var verb = command.SafeRemainingArgument.ToLowerInvariant();

		if (LookRoomVerb.EqualTo(verb) || LookInThingVerb.EqualTo(verb))
		{
			actor.OutputHandler.Send("The Look Room, Look Thing and Look In Things verbs must all be different to one another.");
			return false;
		}

		var costs = InvocationCosts[LookThingVerb].ToList();
		InvocationCosts[verb] = costs;
		InvocationCosts.Remove(LookThingVerb);
		LookThingVerb = verb;
		Changed = true;
		actor.OutputHandler.Send($"This magic power will now use the verb {verb.ColourCommand()} to invoke the power to look at a thing.");
		return true;
	}

	private bool BuildingCommandLookInThingVerb(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which verb should be used to activate this power to look in a thing?");
			return false;
		}

		var verb = command.SafeRemainingArgument.ToLowerInvariant();

		if (LookThingVerb.EqualTo(verb) || LookRoomVerb.EqualTo(verb))
		{
			actor.OutputHandler.Send("The Look Room, Look Thing and Look In Things verbs must all be different to one another.");
			return false;
		}

		var costs = InvocationCosts[LookInThingVerb].ToList();
		InvocationCosts[verb] = costs;
		InvocationCosts.Remove(LookInThingVerb);
		LookInThingVerb = verb;
		Changed = true;
		actor.OutputHandler.Send($"This magic power will now use the verb {verb.ColourCommand()} to invoke the power to look in a thing.");
		return true;
	}
	#endregion Building Subcommands
	#endregion Building Commands
}