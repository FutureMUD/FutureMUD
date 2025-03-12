using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.NPC;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Commands.Modules;

public class SharedModule : Module<ICharacter>
{
	private SharedModule()
		: base("Shared")
	{
		IsNecessary = true;
	}

	public static SharedModule Instance { get; } = new();

	public static bool AdminOrAdminFilterFunction(object actorObject, string commandWord)
	{
		var actor = (ICharacter)actorObject;
		if (actor.IsAdministrator() || actor is INPC)
		{
			return true;
		}

		return false;
	}

	private static readonly Regex _echoOnFailureRegex = new("^[.+]\\s*\\([.+]\\)$");

	[PlayerCommand("Vis", "vis")]
	[DisplayOptions(CommandDisplayOptions.DisplayToAdminsAndNPCs)]
	[CustomModuleName("Storyteller")]
	protected static void Vis(ICharacter actor, string input)
	{
		if (actor is INPC)
		{
			if (!actor.AffectedBy<Switched>())
			{
				actor.OutputHandler.Send("Only possessed NPCs can do this command.");
				return;
			}
		}
		else if (!actor.IsAdministrator())
		{
			actor.OutputHandler.Send(actor.Gameworld.GetStaticString("FailedToFindCommand"));
			return;
		}

		var ss = new StringStack(input.RemoveFirstWord());
		var cmd = ss.Pop().ToLowerInvariant();

		if (cmd == "silent" || cmd == "discrete")
		{
			if (!actor.AffectedBy<IAdminInvisEffect>())
			{
				actor.OutputHandler.Send("You are already visible.");
				return;
			}

			actor.RemoveAllEffects(x => x.IsEffectType<IAdminInvisEffect>());
			actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ fade|fades into view discretely.", actor),
				flags: OutputFlags.WizOnly));
		}

		if (!string.IsNullOrEmpty(cmd))
		{
			var target = actor.Target(cmd);
			if (target == null)
			{
				actor.Send("You don't see anything or anyone like that to turn visible.");
				return;
			}

			if (!target.AffectedBy<IAdminInvisEffect>())
			{
				actor.Send($"{target.HowSeen(actor, true)} is already visible.");
				return;
			}

			target.RemoveAllEffects(x => x.IsEffectType<IAdminInvisEffect>());
			target.OutputHandler.Handle(new EmoteOutput(new Emote("@ fade|fades into view.", (IPerceiver)target)));
			return;
		}

		if (!actor.AffectedBy<IAdminInvisEffect>())
		{
			actor.OutputHandler.Send("You are already visible.");
			return;
		}

		actor.RemoveAllEffects(x => x.IsEffectType<IAdminInvisEffect>());
		actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ fade|fades into view.", actor)));
	}

	[PlayerCommand("Invis", "invis")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	[DisplayOptions(CommandDisplayOptions.DisplayToAdminsAndNPCs)]
	[CustomModuleName("Storyteller")]
	protected static void Invis(ICharacter actor, string input)
	{
		if (actor is INPC)
		{
			if (!actor.AffectedBy<Switched>())
			{
				actor.OutputHandler.Send("Only possessed NPCs can do this command.");
				return;
			}
		}
		else if (!actor.IsAdministrator())
		{
			actor.OutputHandler.Send(actor.Gameworld.GetStaticString("FailedToFindCommand"));
			return;
		}

		var ss = new StringStack(input.RemoveFirstWord());
		var cmd = ss.Pop().ToLowerInvariant();
		if (cmd == "silent" || cmd == "discrete")
		{
			if (actor.AffectedBy<IAdminInvisEffect>())
			{
				actor.OutputHandler.Send("You are already invisible.");
				return;
			}

			actor.OutputHandler.Handle(
				new EmoteOutput(new Emote("@ disappear|disappears from view discretely.", actor.Body),
					flags: OutputFlags.WizOnly));
			actor.AddEffect(new AdminInvis(actor));
			return;
		}

		if (!string.IsNullOrEmpty(cmd))
		{
			var target = actor.Target(cmd);
			if (target == null)
			{
				actor.Send("You don't see anything or anyone like that to turn invisible.");
				return;
			}

			if (target.AffectedBy<IAdminInvisEffect>())
			{
				actor.Send($"{target.HowSeen(actor, true)} is already invisible.");
				return;
			}

			target.AddEffect(new AdminInvis(target));
			target.OutputHandler.Handle(
				new EmoteOutput(new Emote("@ disappear|disappears from view.", (IPerceiver)target)));
			return;
		}

		if (actor.AffectedBy<IAdminInvisEffect>())
		{
			actor.OutputHandler.Send("You are already invisible.");
			return;
		}

		actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ disappear|disappears from view.", actor)));
		actor.AddEffect(new AdminInvis(actor));
	}

	[PlayerCommand("Goto", "goto")]
	[RequiredCharacterState(CharacterState.Conscious)]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	[HelpInfo("goto",
		@"The GOTO command allows you to move yourself to a particular location in the gameworld. If you are not invisible, this will echo to the room - otherwise, if you have your admin invisibility up only other admins in the origin or destination will see you go.

There are several different ways you can use this command, as per below:

	#3goto <roomnumber>#0 - Go to a particular room identified by number
	#3goto <character name>#0 - Go to the location of a particular named character
	#3goto <character keywords>#0 - Go to the location of a character by keywords
	#3goto #<room keywords>#0 - Override for room descriptions when there is a clash with a character
	#3goto @<number>#0 - Go to a recently created room. See below for explanation:

#6For example, @1 is the most recently created new room, @2 is the 2nd most recently created room etc.
This only works for rooms created since the last reboot.
This command is useful when you write-up a bunch of room creation commands in a text file to paste into the MUD at once, so you can refer to the rooms that you create rather than having to presuppose what the room ID will be.#0",
		AutoHelp.HelpArgOrNoArg)]
	[DisplayOptions(CommandDisplayOptions.DisplayToAdminsAndNPCs)]
	[CustomModuleName("Storyteller")]
	protected static void Goto(ICharacter actor, string input)
	{
		if (actor is INPC)
		{
			if (!actor.AffectedBy<Switched>())
			{
				actor.OutputHandler.Send("Only possessed NPCs can do this command.");
				return;
			}
		}
		else if (!actor.IsAdministrator())
		{
			actor.OutputHandler.Send(actor.Gameworld.GetStaticString("FailedToFindCommand"));
			return;
		}

		var ss = new StringStack(input.RemoveFirstWord());
		var cmd = ss.SafeRemainingArgument;
		var destinationLayer = actor.RoomLayer;
		var target = actor.Gameworld.Actors
						  .Where(x => !x.State.HasFlag(CharacterState.Dead))
						  .OrderByDescending(x => x.IsPlayerCharacter)
						  .GetFromItemListByKeywordIncludingNames(cmd, actor);
		ICell destination;
		if (long.TryParse(cmd, out var roomid))
		{
			destination = actor.Gameworld.Cells.Get(roomid);
			if (destination == null)
			{

				actor.OutputHandler.Send("There is no location with that ID.");
				return;
			}
		}
		else
		{
			if (target is null)
			{
				if (cmd.Length > 1 && cmd[0] == '#')
				{
					cmd = cmd.Substring(1);
				}

				destination = RoomBuilderModule.LookupCell(actor.Gameworld, cmd);
				if (destination == null)
				{

					actor.OutputHandler.Send("There are no locations and no-one with that name or keyword to go to.");
					return;
				}
			}
			else
			{
				destination = target.Location;
				destinationLayer = target.RoomLayer;
			}
		}

		if (destination == actor.Location && destinationLayer == actor.RoomLayer)
		{
			actor.OutputHandler.Send("You are already there!");
			return;
		}

		actor.TransferTo(destination, destinationLayer);
	}

	[PlayerCommand("Echo", "echo")]
	[DisplayOptions(CommandDisplayOptions.DisplayToAdminsAndNPCs)]
	[CustomModuleName("Storyteller")]
	protected static void Echo(ICharacter actor, string input)
	{
		if (actor is INPC)
		{
			if (!actor.AffectedBy<Switched>())
			{
				actor.OutputHandler.Send("Only possessed NPCs can do this command.");
				return;
			}
		}
		else if (!actor.IsAdministrator())
		{
			actor.OutputHandler.Send(actor.Gameworld.GetStaticString("FailedToFindCommand"));
			return;
		}

		bool auditory = false, visual = false;
		var difficulty = Difficulty.Automatic;
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.Peek().StartsWith("-", StringComparison.Ordinal))
		{
			switch (ss.Pop().RemoveFirstCharacter().ToLowerInvariant())
			{
				case "a":
					auditory = true;
					break;
				case "v":
					visual = true;
					break;
				case "av":
					auditory = true;
					visual = true;
					break;
				default:
					actor.OutputHandler.Send("Valid options are -a, -v or -av.");
					return;
			}

			if (!Enum.TryParse(ss.PopSpeech(), true, out difficulty))
			{
				actor.OutputHandler.Send("That is not a valid difficulty. Valid difficulties are " +
										 (from differ in Enum.GetNames(typeof(Difficulty))
										  select differ.Colour(Telnet.Cyan)).ListToString() + ".");
				return;
			}
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What did you want to echo?");
			return;
		}

		string primaryText, secondaryText = "";
		var match = _echoOnFailureRegex.Match(ss.RemainingArgument);
		if (match.Success)
		{
			primaryText = match.Groups[1].Value.SubstituteANSIColour();
			secondaryText = match.Groups[2].Value.SubstituteANSIColour();
		}
		else
		{
			primaryText = ss.RemainingArgument.SubstituteANSIColour();
		}

		var primaryEmote = new PlayerEmote(primaryText, actor, false);
		if (!primaryEmote.Valid)
		{
			actor.OutputHandler.Send(primaryEmote.ErrorMessage);
			return;
		}

		var secondaryEmote = new PlayerEmote(secondaryText, actor, false);
		if (!string.IsNullOrEmpty(secondaryText) && !secondaryEmote.Valid)
		{
			actor.OutputHandler.Send(secondaryEmote.ErrorMessage);
			return;
		}

		var primaryOutput = new EmoteOutput(primaryEmote);
		var secondaryOutput = new EmoteOutput(secondaryEmote);

		actor.OutputHandler.Send("You send out an echo:");
		actor.OutputHandler.Send(primaryOutput);
		var audioCheck = actor.Gameworld.GetCheck(CheckType.GenericListenCheck);
		var visualCheck = actor.Gameworld.GetCheck(CheckType.GenericSpotCheck);
		foreach (var person in actor.Location.Characters.Except(actor))
		{
			if (!person.IsAdministrator() &&
				((auditory && audioCheck.Check(person, difficulty).IsFail()) ||
				 (visual && visualCheck.Check(person, difficulty).IsFail())))
			{
				if (secondaryText.Length != 0)
				{
					person.OutputHandler.Send(secondaryOutput);
				}

				actor.Send("{0} failed {1} check.", person.HowSeen(actor, true), person.Gender.Possessive());
				continue;
			}

			person.OutputHandler.Send(primaryOutput);
		}
	}

	[PlayerCommand("ZEcho", "zecho")]
	[DisplayOptions(CommandDisplayOptions.DisplayToAdminsAndNPCs)]
	[CustomModuleName("Storyteller")]
	protected static void ZEcho(ICharacter actor, string input)
	{
		if (actor is INPC)
		{
			if (!actor.AffectedBy<Switched>())
			{
				actor.OutputHandler.Send("Only possessed NPCs can do this command.");
				return;
			}
		}
		else if (!actor.IsAdministrator())
		{
			actor.OutputHandler.Send(actor.Gameworld.GetStaticString("FailedToFindCommand"));
			return;
		}

		var echo = input.RemoveFirstWord();
		if (string.IsNullOrEmpty(echo))
		{
			actor.Send("What do you want to zecho?");
			return;
		}

		actor.Send("Echoing to zone...");
		var message = echo.SubstituteANSIColour().ProperSentences();
		foreach (var perceiver in actor.Location.Room.Zone.Characters)
		{
			perceiver.Send(message);
		}
	}

	[PlayerCommand("PEcho", "pecho")]
	[DisplayOptions(CommandDisplayOptions.DisplayToAdminsAndNPCs)]
	[CustomModuleName("Storyteller")]
	protected static void PEcho(ICharacter actor, string input)
	{
		if (actor is INPC)
		{
			if (!actor.AffectedBy<Switched>())
			{
				actor.OutputHandler.Send("Only possessed NPCs can do this command.");
				return;
			}
		}
		else if (!actor.IsAdministrator())
		{
			actor.OutputHandler.Send(actor.Gameworld.GetStaticString("FailedToFindCommand"));
			return;
		}

		var ss = new StringStack(input.RemoveFirstWord());
		var targets = new List<ICharacter>();
		foreach (var starget in ss.Pop().Split(','))
		{
			var target = actor.TargetActor(starget);
			if (target == null)
			{
				actor.Send("You do not see anyone like that to pecho to.");
				return;
			}

			if (targets.Contains(target))
			{
				continue;
			}

			targets.Add(target);
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to echo?");
			return;
		}

		var emoteData = new PlayerEmote(ss.RemainingArgument.SubstituteANSIColour(), actor.Body);
		if (emoteData.Valid)
		{
			foreach (var target in targets)
			{
				target.OutputHandler.Handle(new EmoteOutput(emoteData), OutputRange.Personal);
			}

			actor.OutputHandler.Send(
				$"You echo to {targets.Select(x => x.HowSeen(actor)).ListToString()}: \n{emoteData.ParseFor(actor)}");
		}
		else
		{
			actor.OutputHandler.Send(emoteData.ErrorMessage);
		}
	}
}