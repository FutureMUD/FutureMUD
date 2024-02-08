using System;
using System.Linq;
using System.Text;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Movement;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.Construction;
using MudSharp.PerceptionEngine.Outputs;

namespace MudSharp.Commands.Modules;

internal class MovementModule : Module<ICharacter>
{
	private MovementModule()
		: base("Movement")
	{
		IsNecessary = true;
	}

	public static MovementModule Instance { get; } = new();

	public override int CommandsDisplayOrder => 1;

	[PlayerCommand("Move", "n", "e", "s", "w", "u", "d", "ne", "nw", "se", "sw",
		"north", "east", "south", "west",
		"up", "down", "northeast", "northwest", "southeast", "southwest", "enter", "leave")]
	[DelayBlock("movement", "You cannot move until you stop {0}.")]
	[RequiredCharacterState(CharacterState.Able)]
	[DisplayOptions(CommandDisplayOptions.DisplayCommandWords)]
	protected static void Move(ICharacter actor, string input)
	{
		if (actor.Combat != null && actor.MeleeRange)
		{
			actor.Send("You must escape melee combat before you can move anywhere!");
			return;
		}

		var ss = new StringStack(input);
		var direction = ss.Pop();
		var emote = new PlayerEmote(ss.PopParentheses(), actor.Body);

		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return;
		}

		if (actor.Movement != null)
		{
			if (Constants.CardinalDirectionStringToDirection.ContainsKey(direction) && !actor.QueuedMoveCommands.Any())
			{
				var targetDirection = Constants.CardinalDirectionStringToDirection[direction];
				if (targetDirection.IsOpposingDirection(actor.Movement.Exit.OutboundDirection) &&
				    actor.Movement.CanBeVoluntarilyCancelled && actor.Movement.IsMovementLeader(actor))
				{
					actor.Movement.Cancel();
					actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ turn|turns around.", actor)));
					return;
				}
			}

			if (actor.Combat != null)
			{
				actor.Send("You cannot stack movement commands in combat. Wait until you stop moving first.");
				return;
			}

			actor.QueuedMoveCommands.Enqueue(input);
			actor.OutputHandler.Send("");
			return;
		}

		actor.Move(input);
	}

	[PlayerCommand("Follow", "follow", "fol")]
	[RequiredCharacterState(CharacterState.Awake)]
	[CommandPermission(PermissionLevel.NPC)]
	[HelpInfo("follow", @"The follow command is used to begin following someone, without their consent. Essentially, you will move in the same direction as the target when they start to move, but you don't otherwise count as in their party. It's as if you were very rapidly entering the same movement commands as they were when you see them move.

The syntax is as follows:

	#3follow <target>#0 - begins following a target
	#3follow self#0 - stops following a target

#1Note: You may follow targets into situations that are dangerous or illegal with this command.#0", AutoHelp.HelpArg)]
	protected static void Follow(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var starget = ss.SafeRemainingArgument;

		var target = actor.TargetActor(starget);
		if (target is null)
		{
			actor.OutputHandler.Send("You don't see anyone like that.");
			return;
		}

		if (target == actor)
		{
			if (actor.Following == null)
			{
				actor.Send("You are not following anyone.");
				return;
			}

			actor.Send("You stop following {0}.", actor.Following.HowSeen(actor));
			actor.Follow(null);
			return;
		}

		actor.Follow(target);
		actor.Send("You will now try to follow {0}.", actor.Following.HowSeen(actor));

	}

	public const string PartyHelp =
		@"The party command is used to create and manage parties, which are groups of people who will follow each other when moving around. All parties have a leader, and whenever the leader moves the rest of the party will move with them. Parties will move at the speed of their slowest member so that everyone arrives at the same time.

The syntax for this command is as follows:

	#3party#0 - show information on your current party
	#3party invite <target>#0 - invite the target to join your party with you as a leader
	#3party join <target>#0 - petition the target to let you join their party
	#3party merge <target>#0 - propose to the target that your two parties should merge into one with you as the leader
	#3party leave#0 - leaves your current party
	#3party eject <target>#0 - as the leader, eject someone from your party
	#3party disband#0 - as the leader, disband your party
	#3party promote <target>#0 - as the leader, promote someone else to leader (you will no longer be the leader)";

	[PlayerCommand("Party", "party")]
	[RequiredCharacterState(CharacterState.Awake)]
	[HelpInfo("party", PartyHelp, AutoHelp.HelpArg)]
	protected static void Party(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var cmd = ss.PopSpeech().ToLowerInvariant();
		if (cmd.Length == 0)
		{
			if (actor.Party == null)
			{
				actor.OutputHandler.Send("You are not in a party.");
				return;
			}

			var sb = new StringBuilder("Your party consists of the following:\n\n");
			sb.AppendLine(actor.Party.DisplayMembers(actor));

			if (actor.Party.Party != null)
			{
				sb.AppendLine("Your party is following " + actor.Party.Party.Leader.HowSeen(actor) + "'s party.");
			}

			actor.OutputHandler.Send(sb.ToString());
			return;
		}

		ICharacter target = null;
		switch (cmd)
		{
			case "join":
				cmd = ss.Pop().ToLowerInvariant();
				if (cmd.Length == 0)
				{
					actor.OutputHandler.Send("Who do you wish to petition to join their party?");
					return;
				}

				target = actor.TargetActor(cmd);
				if (target == null)
				{
					actor.OutputHandler.Send(
						"You do not see anyone like that to petition about joining their party.");
					return;
				}

				if (target.Party != null && target.Party == actor.Party)
				{
					actor.OutputHandler.Send("You are already in the same party as " +
					                         target.HowSeen(actor.Body) + ".");
					return;
				}

				target.AddEffect(new Accept(target, new PartyJoinProposal(actor, target)),
					TimeSpan.FromSeconds(60));
				actor.Body.OutputHandler.Send("You request to join " + target.HowSeen(actor.Body) + "'s party.");
				target.OutputHandler.Send(actor.Body.HowSeen(target, true) +
				                          " has asked to join your party. Use " + "ACCEPT".Colour(Telnet.Cyan) +
				                          " or " + "DECLINE".Colour(Telnet.Cyan) + " to respond to " +
				                          actor.Body.ApparentGender(target).Possessive() + " request.");
				break;
			case "invite":
				cmd = ss.Pop().ToLowerInvariant();
				if (cmd.Length == 0)
				{
					actor.OutputHandler.Send("Who do you wish to invite to join your party?");
					return;
				}

				target = actor.TargetActor(cmd);
				if (target == null)
				{
					actor.OutputHandler.Send("You do not see anyone like that to invite to join your party.");
					return;
				}

				if (target.Party != null && target.Party == actor.Party)
				{
					actor.OutputHandler.Send("You are already in the same party as " +
					                         target.HowSeen(actor.Body) + ".");
					return;
				}

				target.AddEffect(new Accept(target, new PartyInviteProposal(actor, target)),
					TimeSpan.FromSeconds(60));
				actor.OutputHandler.Send("You request that " + target.HowSeen(actor.Body) + " joins your party.");
				target.OutputHandler.Send(actor.Body.HowSeen(target, true) + " has asked you to join " +
				                          actor.Body.ApparentGender(target).Possessive() + " party. Use " +
				                          "ACCEPT".Colour(Telnet.Cyan) + " or " + "DECLINE".Colour(Telnet.Cyan) +
				                          " to respond to " + actor.Body.ApparentGender(target).Possessive() +
				                          " request.");
				break;
			case "promote":
				cmd = ss.Pop().ToLowerInvariant();
				if (cmd.Length == 0)
				{
					actor.OutputHandler.Send("Who do you wish to promote to the new party leader?");
					return;
				}

				target = actor.TargetActor(cmd);
				if (target == null)
				{
					actor.OutputHandler.Send("You do not see anyone like that to promote to party leader.");
					return;
				}

				if (actor.Party == null || target.Party != actor.Party)
				{
					actor.OutputHandler.Send("You are not in the same party as " + target.HowSeen(actor.Body) +
					                         ".");
					return;
				}

				if (actor.Party.Leader != actor)
				{
					actor.OutputHandler.Send("You are not the leader of your party.");
					return;
				}

				actor.Party.SetLeader(target);
				foreach (var ch in actor.Party.Members)
				{
					if (ch == actor)
					{
						continue;
					}

					ch.OutputHandler.Send(actor.HowSeen(ch, true) + " has promoted " + target.HowSeen(ch) +
					                      " to the leader of your party.");
				}

				actor.OutputHandler.Send("You promote " + target.HowSeen(actor) +
				                         " to the leader of your party.");

				break;
			case "leave":
				if (actor.Party == null)
				{
					actor.OutputHandler.Send("You are not in a party.");
					return;
				}

				foreach (var ch in actor.Party.Members)
				{
					if (ch == actor)
					{
						continue;
					}

					ch.OutputHandler.Send(actor.HowSeen(ch, true) + " has left the party.");
				}

				actor.OutputHandler.Send("You leave the party.");
				actor.LeaveParty();
				break;
			case "disband":
				if (actor.Party == null)
				{
					actor.OutputHandler.Send("You are not in a party.");
					return;
				}

				if (actor.Party.Leader != actor)
				{
					actor.OutputHandler.Send("You are not the leader of your party.");
					return;
				}

				foreach (var ch in actor.Party.Members)
				{
					if (ch == actor)
					{
						continue;
					}

					ch.OutputHandler.Send(actor.HowSeen(ch, true) + " has disbanded the party.");
				}

				actor.OutputHandler.Send("You disband your party.");
				actor.Party.Disband();
				break;
			case "merge":
				if (actor.Party == null)
				{
					actor.OutputHandler.Send("You are not in a party.");
					return;
				}

				if (actor.Party.Leader != actor)
				{
					actor.OutputHandler.Send("You are not the leader of your party.");
					return;
				}

				cmd = ss.Pop().ToLowerInvariant();
				if (cmd.Length == 0)
				{
					actor.OutputHandler.Send("With whose party do you wish to merge?");
					return;
				}

				target = actor.TargetActor(cmd);
				if (target == null)
				{
					actor.OutputHandler.Send("You do not see anyone like that with whom to merge parties.");
					return;
				}

				if (target.Party == null)
				{
					actor.OutputHandler.Send(target.HowSeen(actor, true) +
					                         " is not in a party, and so you cannot merge with them.");
					return;
				}

				if (target.Party == actor.Party)
				{
					actor.OutputHandler.Send("You are already in the same party as " + target.HowSeen(actor) +
					                         ".");
					return;
				}

				target.AddEffect(new Accept(target, new PartyMergeProposal(actor, target)),
					TimeSpan.FromSeconds(120));
				actor.OutputHandler.Send("You request that your party be merged with that of " +
				                         target.HowSeen(actor) + ".");
				target.OutputHandler.Send(actor.HowSeen(target, true) + " has asked merge " +
				                          actor.ApparentGender(target).Possessive() + " party with yours. Use " +
				                          "ACCEPT".Colour(Telnet.Cyan) + " or " + "DECLINE".Colour(Telnet.Cyan) +
				                          " to respond to " + actor.Body.ApparentGender(target).Possessive() +
				                          " request.");
				break;
			//case "follow":
			//case "stop":
			case "eject":
				if (actor.Party == null)
				{
					actor.OutputHandler.Send("You are not in a party.");
					return;
				}

				if (actor.Party.Leader != actor)
				{
					actor.OutputHandler.Send("You are not the leader of your party.");
					return;
				}

				cmd = ss.Pop().ToLowerInvariant();
				if (cmd.Length == 0)
				{
					actor.OutputHandler.Send("Who do you wish to eject from your party?");
					return;
				}

				target = actor.TargetActor(cmd);
				if (target == null)
				{
					actor.OutputHandler.Send("You do not see anyone like that to eject from your party.");
					return;
				}

				if (target.Party != actor.Party)
				{
					actor.OutputHandler.Send(target.HowSeen(actor.Body, true) + " is not in your party.");
					return;
				}

				actor.OutputHandler.Send("You eject " + target.HowSeen(actor) + " from your party.");
				foreach (var ch in actor.Party.Members.Except(new IMove[] { actor, target }))
				{
					ch.OutputHandler.Send(actor.HowSeen(ch, true) + " ejects " + target.HowSeen(ch) +
					                      " from your party.");
				}

				target.OutputHandler.Send(actor.HowSeen(target, true) + " ejects you from " +
				                          actor.ApparentGender(target).Possessive() + " party.");
				target.LeaveParty();

				break;
			default:
				actor.OutputHandler.Send(PartyHelp.SubstituteANSIColour());
				return;
		}
	}

	[PlayerCommand("Speed", "speed")]
	protected static void Speed(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var cmd = ss.Pop().ToLowerInvariant();

		if (cmd.Length == 0)
		{
			var sb = new StringBuilder();
			sb.AppendLine(
				$"You can adopt the following speeds for each position, sorted in ascending order of quickness:");
			foreach (var position in actor.Speeds.Select(x => x.Position).Distinct())
			{
				sb.AppendLine($"\tWhen {position.DescribeLocationMovementParticiple.TitleCase().ColourValue()}");
				foreach (var item in actor.Speeds.Where(x => x.Position == position)
				                          .OrderByDescending(x => x.Multiplier))
				{
					sb.AppendLine($"\t\t{item.Name.TitleCase().ColourCommand()}");
				}
			}

			actor.OutputHandler.Send(sb.ToString());
			return;
		}

		var speed = actor.Speeds.FirstOrDefault(x => x.Name.EqualTo(cmd)) ??
		            actor.Speeds.FirstOrDefault(
			            x => x.Name.StartsWith(cmd, StringComparison.InvariantCultureIgnoreCase));
		if (speed == null)
		{
			actor.OutputHandler.Send("That is not a valid speed for you.");
			return;
		}

		actor.CurrentSpeeds[speed.Position] = speed;
		actor.OutputHandler.Send(
			$"When you move while you are {speed.Position.DescribeLocationMovementParticiple.ColourValue()}, you will now {speed.FirstPersonVerb.TitleCase().ColourCommand()}");
	}
}