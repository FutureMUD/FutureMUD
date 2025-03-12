using System.Linq;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.NPC;
using MudSharp.NPC.AI;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Law;
using MudSharp.Accounts;
using MudSharp.Construction;

namespace MudSharp.Commands.Modules;

internal class NPCOnlyModule : Module<ICharacter>
{
	private NPCOnlyModule()
		: base("NPC Only")
	{
		IsNecessary = true;
	}

	public static NPCOnlyModule Instance { get; } = new();

	[PlayerCommand("Pause", "pause")]
	protected static void Pause(ICharacter actor, string command)
	{
		if (actor.EffectsOfType<PauseAI>().Any())
		{
			actor.RemoveAllEffects(x => x.IsEffectType<PauseAI>());
			actor.Send("You resume any AI routines that you had.");
			return;
		}

		actor.AddEffect(new PauseAI(actor));
		actor.Send("You pause all of your AI routines until further notice.");
	}

	[PlayerCommand("Doorguard", "doorguard")]
	protected static void DoorGuard(ICharacter actor, string command)
	{
		if (actor.AffectedBy<IDoorguardModeEffect>())
		{
			actor.OutputHandler.Handle(
				new EmoteOutput(new Emote("@ are|is no longer acting as a door guard.", actor)));
			actor.RemoveAllEffects(x => x.IsEffectType<IDoorguardModeEffect>());
		}
		else
		{
			actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ are|is now acting as a door guard.", actor)));
			actor.AddEffect(new DoorguardMode(actor));
		}
	}

	[PlayerCommand("Enforcer", "enforcer")]
	protected static void Enforcer(ICharacter actor, string command)
	{
		if (actor.AffectedBy<EnforcerEffect>())
		{
			actor.RemoveAllEffects<EnforcerEffect>(fireRemovalAction: true);
			actor.OutputHandler.Send("You are no longer in enforcer mode.");
			return;
		}

		var npc = (INPC)actor;
		var ss = new StringStack(command.RemoveFirstWord());
		if (!npc.AIs.Any(x => x is EnforcerAI))
		{
			actor.OutputHandler.Send("You do not have any enforcer AIs, and so cannot be an enforcer.");
			return;
		}

		var possibleLegalAuthorities = actor.Gameworld.LegalAuthorities
		                                    .Where(x => x.GetEnforcementAuthority(actor) != null).ToList();
		if (possibleLegalAuthorities.Count == 0)
		{
			actor.OutputHandler.Send("You are not an enforcer for any legal authorities.");
			return;
		}

		ILegalAuthority authority;
		if (possibleLegalAuthorities.Count > 1)
		{
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send(
					"You could be an enforcer for more than one legal authority. Please specify the legal authority that you want to go into enforcer mode for.");
				return;
			}

			authority = long.TryParse(ss.SafeRemainingArgument, out var value)
				? actor.Gameworld.LegalAuthorities.Get(value)
				: actor.Gameworld.LegalAuthorities.GetByName(ss.SafeRemainingArgument);
			if (authority == null || !possibleLegalAuthorities.Contains(authority))
			{
				actor.OutputHandler.Send("You are not an enforcer for any such legal authority.");
				return;
			}
		}
		else
		{
			authority = possibleLegalAuthorities.Single();
		}

		actor.AddEffect(new EnforcerEffect(actor, authority));
		actor.OutputHandler.Send(
			$"You are now in enforcer mode for the {authority.Name.ColourName()} legal authority.");
	}

	[PlayerCommand("Bodyguard", "bodyguard")]
	protected static void Bodyguard(ICharacter actor, string command)
	{
		var npc = (INPC)actor;
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			if (npc.BodyguardingCharacterID == null)
			{
				actor.OutputHandler.Send("You're not currently bodyguarding anyone.");
				return;
			}

			actor.OutputHandler.Send(
				$"You're currently bodyguarding {actor.Gameworld.TryGetCharacter(npc.BodyguardingCharacterID.Value).HowSeen(actor)}.");
			return;
		}

		ICharacter oldCh;
		if (ss.Peek().EqualToAny("none", "clear", "off"))
		{
			oldCh = actor.Gameworld.TryGetCharacter(npc.BodyguardingCharacterID ?? 0, true);
			oldCh?.OutputHandler?.Send($"{npc.HowSeen(oldCh, true)} is no longer bodyguarding you.");
			npc.BodyguardingCharacterID = null;
			actor.OutputHandler.Send("You're no longer bodyguarding anyone.");
			return;
		}

		var target = actor.TargetActor(ss.PopSpeech());
		if (target == null)
		{
			actor.OutputHandler.Send("You don't see anyone like that here.");
			return;
		}

		if (target == actor)
		{
			actor.OutputHandler.Send("You cannot bodyguard yourself.");
			return;
		}

		oldCh = actor.Gameworld.TryGetCharacter(npc.BodyguardingCharacterID ?? 0, true);
		oldCh?.OutputHandler?.Send($"{npc.HowSeen(oldCh, true)} is no longer bodyguarding you.");
		npc.BodyguardingCharacterID = target.Id;
		target.OutputHandler.Send($"{npc.HowSeen(target, true)} is now bodyguarding you.");
	}

	[PlayerCommand("IgnoreForce", "ignoreforce")]
	protected static void IgnoreForce(ICharacter actor, string command)
	{
		if (actor.AffectedBy<IgnoreForce>())
		{
			actor.RemoveAllEffects(x => x.IsEffectType<IgnoreForce>());
			actor.OutputHandler.Send("You are no longer ignoring FORCE commands.");
			return;
		}

		actor.AddEffect(new IgnoreForce(actor));
		actor.OutputHandler.Send("You are now ignoring FORCE commands.");
	}

	[PlayerCommand("Return", "return")]
	protected static void Return(ICharacter actor, string input)
	{
		var switched = actor.EffectsOfType<Switched>().FirstOrDefault();
		if (switched == null)
		{
			actor.Send("You are not possessing any NPCs.");
			return;
		}

		actor.RemoveEffect(switched, true);
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
	protected static void Goto(ICharacter actor, string input)
	{
		if (!actor.AffectedBy<Switched>())
		{
			actor.OutputHandler.Send("Only possessed NPCs can do this command.");
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
}