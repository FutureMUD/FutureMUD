﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Communication.Language;
using MudSharp.Database;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Knowledge;
using MudSharp.Form.Material;
using MudSharp.NPC;
using System.Reflection;
using Humanizer;
using MudSharp.Accounts;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character.Name;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Form.Shape;
using MudSharp.FutureProg.Statements;
using MudSharp.Work.Foraging;

namespace MudSharp.Commands.Modules;

internal class GameModule : Module<ICharacter>
{
	private GameModule()
		: base("Game")
	{
		IsNecessary = true;
	}

	public static GameModule Instance { get; } = new();
	public override int CommandsDisplayOrder => 0;

	[PlayerCommand("SkillLevels", "skilllevels")]
	[HelpInfo("skilllevels",
		@"This command allows you to see the skill level descriptors for one of your skills or stats, in the order that they appear. The syntax is SKILLLEVELS <skill/attribute>.",
		AutoHelp.HelpArgOrNoArg)]
	protected static void SkillLevels(ICharacter actor, string command)
	{
		var traitName = command.RemoveFirstWord();
		var traits = actor.IsAdministrator()
			? actor.Gameworld.Traits.ToList()
			: actor.Traits.Select(x => x.Definition).ToList();
		var trait = traits.FirstOrDefault(x => x.Name.EqualTo(traitName)) ??
					traits.FirstOrDefault(
						x => x.Name.StartsWith(traitName, StringComparison.InvariantCultureIgnoreCase)) ??
					traits.OfType<IAttributeDefinition>().FirstOrDefault(x => x.Alias.EqualTo(traitName));

		if (trait == null)
		{
			actor.OutputHandler.Send(actor.IsAdministrator()
				? "There is no such skill or attribute"
				: "You do not have any such skill or attribute.");
			return;
		}

		var sb = new StringBuilder();
		sb.Append("Levels for ");
		switch (trait.TraitType)
		{
			case TraitType.Attribute:
				sb.Append("attribute");
				break;
			case TraitType.Skill:
				sb.Append("skill");
				break;
		}

		sb.Append(" ");
		sb.Append(trait.Name.TitleCase().ColourValue());
		sb.AppendLine(":");
		foreach (var level in trait.Decorator.OrderedDescriptors)
		{
			sb.AppendLine($"\t{level.TitleCase()}");
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	[PlayerCommand("Command", "command")]
	[HelpInfo("command",
		"This command is used to issue commands to NPCs that you are authorised to issue commands to. Commands can be issued even when you can't see your target or are unconscious. The syntax is COMMAND <target> <input that you want them to enter>.",
		AutoHelp.HelpArgOrNoArg)]
	protected static void Command(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		var target = actor.TargetActor(ss.PopSpeech(),
			PerceiveIgnoreFlags.IgnoreConsciousness | PerceiveIgnoreFlags.IgnoreDark);
		if (target == null)
		{
			actor.Send("You don't see anyone like that to issue a command to.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("What command do you want to issue?");
			return;
		}

		if (!(target is INPC npc) || !npc.HandlesEvent(Events.EventType.CommandIssuedToCharacter))
		{
			if (actor.CanSee(target))
			{
				actor.Send($"You can't issue commands to {target.HowSeen(actor)}.");
			}
			else
			{
				actor.Send("You don't see anyone like that to issue a command to.");
			}

			return;
		}

		npc.HandleEvent(Events.EventType.CommandIssuedToCharacter, target, actor, ss.RemainingArgument);
	}

	[PlayerCommand("Notify", "notify")]
	[HelpInfo("notify", @"The #3notify#0 command is used to let other people know that you are online and available for roleplay, and respond to such notifications.

You can either notify all players in a clan with #3notify <clan name>#0, or notify a specific character with #3notify <character name>#0. Anyone online will get a prompt to type #3notify#0 to respond to let you know they're online.

This command exists to let players coordinate between one another, but overuse of this command is discouraged.", AutoHelp.HelpArg)]
	protected static void Notify(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());

		if (string.IsNullOrEmpty(ss.PopSpeech()))
		{
			var effects = actor.EffectsOfType<INotifyEffect>().ToList();
			if (!effects.Any())
			{
				actor.OutputHandler.Send("Nobody has notified you that they are online.");
				return;
			}

			var effect = effects.First();
			actor.OutputHandler.Send(
				$"You notify {effect.NotifyTarget.HowSeen(actor, true, flags: PerceiveIgnoreFlags.IgnoreCanSee)} that you are online and available to roleplay.");
			effect.NotifyTarget.OutputHandler.Send(
				$"[{actor.HowSeen(effect.NotifyTarget, true, flags: PerceiveIgnoreFlags.IgnoreCanSee)} is online.]".ColourIncludingReset(Telnet.Yellow));
			actor.RemoveEffect(effect);
			return;
		}

		var clan =
			actor.ClanMemberships.FirstOrDefault(
				x => x.Clan.FullName.Equals(ss.Last, StringComparison.InvariantCultureIgnoreCase)) ??
			actor.ClanMemberships.FirstOrDefault(
				x => x.Clan.Alias.Equals(ss.Last, StringComparison.InvariantCultureIgnoreCase));
		if (clan == null)
		{
			var characters = actor.Gameworld.Characters.Except(actor).GetAllByName(ss.Last);
			foreach (var character in characters)
			{
				character.AddEffect(new Notify(character, actor, false), TimeSpan.FromSeconds(300));
				character.OutputHandler.Send(
					new EmoteOutput(
						new Emote(
							string.Format(
								"[{1} is online (notified you as {0}). Use notify to reply in kind.]".ColourIncludingReset(
									Telnet.Yellow),
								ss.Last.TitleCase(),
								actor.HowSeen(character, true, colour: true, flags: PerceiveIgnoreFlags.IgnoreCanSee)
							), character, actor)));
			}

			actor.OutputHandler.Send("If any such individuals are online, they have been notified.");
			return;
		}

		var clanCharacters =
			actor.Gameworld.Characters.Except(actor).Where(x => x.ClanMemberships.Any(y => y.Clan == clan.Clan));
		foreach (var character in clanCharacters)
		{
			character.AddEffect(new Notify(character, actor, true), TimeSpan.FromSeconds(300));
			character.OutputHandler.Send(
				new EmoteOutput(
					new Emote(
						string.Format("[{1} ({0}) is online. Use notify to reply in kind.]".Colour(Telnet.Yellow),
							clan.Clan.FullName.TitleCase(),
							actor.HowSeen(character, true, colour: false, flags: PerceiveIgnoreFlags.IgnoreCanSee)
						), character, actor)));
		}

		actor.OutputHandler.Send("All clan members currently online have been notified.");
	}

	[PlayerCommand("Helpless", "helpless")]
	[HelpInfo("helpless", @"The helpless command is used to make yourself completely and utterly helpless; you will not resist attempts to subdue, manipulate, interact with your inventory, perform medical procedures or even attack you.

The syntax for this command is #3helpless#0, which will toggle the effect.", AutoHelp.HelpArg)]
	protected static void Helpless(ICharacter actor, string input)
	{
		if (actor.EffectsOfType<VoluntarilyHelpless>().Any())
		{
			actor.RemoveAllEffects(x => x.IsEffectType<VoluntarilyHelpless>());
			actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ are|is no longer passively accepting &0's fate.",
				actor, actor)));
			return;
		}

		actor.AddEffect(new VoluntarilyHelpless(actor));
		actor.OutputHandler.Handle(new EmoteOutput(new Emote(
			"@ are|is now voluntarily helpless: #0 will now passively accept whatever is done to them.", actor,
			actor)));
		actor.Send($"Type {"helpless".Colour(Telnet.Yellow)} again to remove this effect.");
	}

	[PlayerCommand("Attacks", "attacks")]
	[HelpInfo("Attacks",
		@"This command is used to list the available attacks that you have, either from weapons or otherwise any other source.

The syntax is either #3attacks#0 to see all attacks (unarmed, weapons, implants or magic) or #3attacks <weapon>#0 to see just attacks with a particular weapon.

For a full list of difficulties, see #3SHOW DIFFICULTIES#0.
For a full list of combat flags, see #3SHOW COMBATFLAGS#0", AutoHelp.HelpArg)]
	protected static void Attacks(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var sb = new StringBuilder();

		void AddAttacksForWeapon(IMeleeWeapon targetWeapon)
		{
			if (sb.Length > 0)
			{
				sb.AppendLine();
				sb.AppendLine();
			}
			sb.AppendLine($"You are currently able to use the following attacks with {targetWeapon.Parent.HowSeen(actor)}:\n");
			sb.AppendLine(StringUtilities.GetTextTable(
				from attack in targetWeapon.WeaponType
				                           .UsableAttacks(actor, targetWeapon.Parent, null, AttackHandednessOptions.Any, true,
					                           BuiltInCombatMoveType.UseWeaponAttack,
					                           BuiltInCombatMoveType.UnbalancingBlow,
					                           BuiltInCombatMoveType.StaggeringBlow,
					                           BuiltInCombatMoveType.WardFreeAttack,
					                           BuiltInCombatMoveType.ClinchAttack,
					                           BuiltInCombatMoveType.MeleeWeaponSmashItem)
				                           .Distinct()
				select new List<string>
				{
					attack.Name,
					$"{attack.Orientation.Describe()} {attack.Alignment.Describe()}",
					attack.MoveType.Describe(),
					attack.Intentions.DescribeBrief(),
					attack.StaminaCost.ToStringN2Colour(actor),
					TimeSpan.FromSeconds(attack.BaseDelay).Humanize(1, actor.Account.Culture, TimeUnit.Millisecond),
					attack.Profile.BaseAttackerDifficulty.DescribeBrief(true),
					attack.Profile.BaseBlockDifficulty.DescribeBrief(true),
					attack.Profile.BaseParryDifficulty.DescribeBrief(true),
					attack.Profile.BaseDodgeDifficulty.DescribeBrief(true),
					attack.SpecialListText
				},
				new List<string>
				{
					"Name",
					"Orientation",
					"Type",
					"Intentions",
					"Stamina",
					"Delay",
					"Difficulty",
					"Block",
					"Parry",
					"Dodge",
					"Special"
				},
				actor,
				Telnet.Red
			));
		}

		if (ss.IsFinished)
		{
			sb.AppendLine($"You are currently able to use the following unarmed attacks:\n");
			sb.AppendLine(StringUtilities.GetTextTable(
				from attack in actor.Race.UsableNaturalWeaponAttacks(actor, null, true, BuiltInCombatMoveType.NaturalWeaponAttack,
					BuiltInCombatMoveType.StaggeringBlowUnarmed,
					BuiltInCombatMoveType.DownedAttackUnarmed,
					BuiltInCombatMoveType.UnbalancingBlowUnarmed,
					BuiltInCombatMoveType.ScreechAttack,
					BuiltInCombatMoveType.WardFreeUnarmedAttack,
					BuiltInCombatMoveType.ClinchUnarmedAttack,
					BuiltInCombatMoveType.UnarmedSmashItem,
					BuiltInCombatMoveType.EnvenomingAttack,
					BuiltInCombatMoveType.EnvenomingAttackClinch,
					BuiltInCombatMoveType.UnbalancingBlowClinch,
					BuiltInCombatMoveType.StaggeringBlowClinch,
					BuiltInCombatMoveType.SwoopAttackUnarmed).Select(x => x.Attack).Distinct()
				select new List<string>
				{
					attack.Name,
					$"{attack.Orientation.Describe()} {attack.Alignment.Describe()}",
					attack.MoveType.Describe(),
					attack.Intentions.DescribeBrief(),
					attack.StaminaCost.ToStringN2Colour(actor),
					TimeSpan.FromSeconds(attack.BaseDelay).Humanize(1, actor.Account.Culture, TimeUnit.Millisecond),
					attack.Profile.BaseAttackerDifficulty.DescribeBrief(true),
					attack.Profile.BaseBlockDifficulty.DescribeBrief(true),
					attack.Profile.BaseParryDifficulty.DescribeBrief(true),
					attack.Profile.BaseDodgeDifficulty.DescribeBrief(true),
					attack.SpecialListText

				},
				new List<string>
				{
					"Name",
					"Orientation",
					"Type",
					"Intentions",
					"Stamina",
					"Delay",
					"Difficulty",
					"Block",
					"Parry",
					"Dodge",
					"Special"
				},
				actor,
				Telnet.Red
			));

			foreach (var weapon in actor.Body.WieldedItems.SelectNotNull(x => x.GetItemType<IMeleeWeapon>()))
			{
				AddAttacksForWeapon(weapon);
			}

			foreach (var weapon in actor.Body.Implants.OfType<IImplantMeleeWeapon>())
			{
				AddAttacksForWeapon(weapon);
			}

			actor.OutputHandler.Send(sb.ToString());
			return;
		}

		var target = actor.TargetHeldItem(ss.Pop());
		if (target == null)
		{
			actor.Send("You aren't holding anything like that to see your attacks with.");
			return;
		}

		var targetAsWeapon = target.GetItemType<IMeleeWeapon>();
		if (targetAsWeapon == null)
		{
			actor.Send($"{target.HowSeen(actor, true)} is not a melee weapon.");
			return;
		}

		AddAttacksForWeapon(targetAsWeapon);
		actor.Send(sb.ToString());
	}

	[PlayerCommand("More", "more")]
	protected static void More(ICharacter actor, string input)
	{
		actor.OutputHandler.More();
	}

	[PlayerCommand("Keyword", "keyword")]
	[RequiredCharacterState(CharacterState.Conscious)]
	[HelpInfo("keyword",
		"The keyword command allows you to trial the results of using a particular keyword to target something. The syntax is KEYWORD <keyword>, or KEYWORD IN <container> <keyword> to test the contents of a container.",
		AutoHelp.HelpArgOrNoArg)]
	protected static void Keyword(ICharacter actor, string input)
	{
		input = input.RemoveFirstWord().ToLowerInvariant();
		var ss = new StringStack(input);

		var strings = input.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
		List<string> keystrings;
		var sb = new StringBuilder();
		var i = 1;

		if (ss.Peek().EqualTo("in"))
		{
			ss.Pop();
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("Check for keywords in what container?");
				return;
			}

			var target = actor.TargetItem(ss.Pop());
			if (target == null)
			{
				actor.OutputHandler.Send("You don't see anything like that to check keywords in.");
				return;
			}

			var container = target.GetItemType<IContainer>();
			if (container == null)
			{
				actor.OutputHandler.Send($"{target.HowSeen(actor, true)} is not a container.");
				return;
			}

			if (!container.Transparent && target.GetItemType<IOpenable>()?.IsOpen == false)
			{
				actor.OutputHandler.Send($"{target.HowSeen(actor, true)} is not open, and you cannot see inside.");
				return;
			}

			input = ss.SafeRemainingArgument;
			strings = input.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			if (strings.Length == 0)
			{
				actor.OutputHandler.Send("Which keywords did you want to test in that container?");
				return;
			}

			keystrings = int.TryParse(strings[0], out _) ? strings.Skip(1).ToList() : strings.ToList();
			sb.AppendLine($"The following targets match the supplied keyword in {target.HowSeen(actor)}:");
			var matchingItems =
				container.Contents.Where(x => x.HasKeywords(keystrings, actor, true) || actor.HasDubFor(x, keystrings))
						 .ToList();
			foreach (var item in matchingItems)
			{
				sb.AppendLine($"\t{i++}: {item.HowSeen(actor)}");
			}

			actor.OutputHandler.Send(sb.ToString());
			return;
		}

		keystrings = strings.Length > 0 && int.TryParse(strings[0], out _)
			? strings.Skip(1).ToList()
			: strings.ToList();

		sb.AppendLine($"The following targets match the supplied keyword {input.Colour(Telnet.Yellow)}:");

		var matchingChars =
			actor.Location.LayerCharacters(actor.RoomLayer).Except(actor)
				 .Where(x => x.HasKeywords(keystrings, actor, true) || actor.HasDubFor(x, keystrings))
				 .ToList();
		var matchingRoomObjects =
			actor.Location.LayerGameItems(actor.RoomLayer).Where(x =>
					 x.HasKeywords(keystrings, actor, true) || actor.HasDubFor(x, keystrings))
				 .ToList();
		var matchingPersonalObjects =
			actor.Body.ExternalItems
				 .Where(x => x.HasKeywords(keystrings, actor, true) || actor.HasDubFor(x, keystrings)).ToList();

		sb.AppendLine("If the target can be either a person or item:".FluentTagMXP("U"));
		foreach (
			var item in
			matchingChars.Cast<IPerceivable>().Concat(matchingPersonalObjects.Concat(matchingRoomObjects)))
		{
			sb.AppendLine($"\t{i++}: {item.HowSeen(actor)}");
		}

		sb.AppendLine();
		sb.AppendLine("If the target is only a person:".FluentTagMXP("U"));
		i = 1;
		foreach (var item in matchingChars)
		{
			sb.AppendLine($"\t{i++}: {item.HowSeen(actor)}");
		}

		sb.AppendLine();
		sb.AppendLine("If the target is only an item (in room or inventory):".FluentTagMXP("U"));
		i = 1;
		foreach (var item in matchingPersonalObjects.Concat(matchingRoomObjects))
		{
			sb.AppendLine($"\t{i++}: {item.HowSeen(actor)}");
		}

		sb.AppendLine();
		sb.AppendLine("If the target is only an inventory item:".FluentTagMXP("U"));
		i = 1;
		foreach (var item in matchingPersonalObjects)
		{
			sb.AppendLine($"\t{i++}: {item.HowSeen(actor)}");
		}

		sb.AppendLine();
		sb.AppendLine("If the target is only a room item:".FluentTagMXP("U"));
		i = 1;
		foreach (var item in matchingRoomObjects)
		{
			sb.AppendLine($"\t{i++}: {item.HowSeen(actor)}");
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	[PlayerCommand("Statistics", "statistics", "stats")]
	protected static void Statistics(ICharacter actor, string input)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"The following statistics are available regarding {actor.Gameworld.Name.Proper()}:");
		sb.AppendLine();
		var version = Assembly.GetCallingAssembly().GetName().Version;
		var versionString =
			$"v{version.Major}.{version.Minor}.{version.Build}.{version.Revision.ToString("0000", actor)}"
				.ColourValue();
		sb.AppendLine(
			$"{actor.Gameworld.Name.Proper().Colour(Telnet.Cyan)} is running on {versionString} of the FutureMUD engine.");
		sb.AppendLine(
			string.Format(actor,
				"The record number of players online at one time was {0}, which was achieved on {1}.",
				actor.Gameworld.GameStatistics.RecordOnlinePlayers.ToString("N0", actor).Colour(Telnet.Green),
				actor.Gameworld.GameStatistics.RecordOnlinePlayersDateTime.GetLocalDateString(actor)
					 .Colour(Telnet.Green)
			));
		sb.AppendLine(
			string.Format(actor, "The MUD was last booted on {0}, and took {1}, with a current uptime of {2}.",
				actor.Gameworld.GameStatistics.LastBootTime.GetLocalDateString(actor).Colour(Telnet.Green),
				actor.Gameworld.GameStatistics.LastStartupSpan.Describe().Colour(Telnet.Green),
				(DateTime.UtcNow - actor.Gameworld.GameStatistics.LastBootTime).Describe().Colour(Telnet.Green)
			));
		using (new FMDB())
		{
			var weeklyStats = actor.Gameworld.GameStatistics.GetOrCreateWeeklyStatistic();
			var sinceTime = DateTime.UtcNow.AddDays(-60);
			sb.AppendLine(string.Format(actor,
				"There are {0} registered accounts, of which {1} have logged on in the last 60 days.",
				FMDB.Context.Accounts.Count().ToString("N0", actor).Colour(Telnet.Green),
				FMDB.Context.Accounts.Count(x => x.LastLoginTime != null && x.LastLoginTime >= sinceTime)
					.ToString("N0", actor)
					.Colour(Telnet.Green)
			));

			sb.AppendLine(
				$"This week there has been {weeklyStats.NewAccounts.ToString("N0", actor).ColourValue()} new {"account".Pluralise(weeklyStats.NewAccounts != 1)} and {weeklyStats.ActiveAccounts.ToString("N0", actor).ColourValue()} unique {"login".Pluralise(weeklyStats.ActiveAccounts != 1)}.");

			var now = DateTime.UtcNow;
			var totalTime =
				FMDB.Context.Characters.Where(x => x.Account != null).Sum(x => (long)x.TotalMinutesPlayed) +
				(long)actor.Gameworld.Characters.Sum(x => (now - x.LoginDateTime).TotalMinutes);
			sb.AppendLine(
				$"Players have spent a total of {TimeSpan.FromMinutes(totalTime).Describe().Colour(Telnet.Green)} playing {actor.Gameworld.Name.Proper()}.");
		}

		sb.AppendLine(string.Format(actor,
			"There are a total of {0} rooms, {1} items and {2} NPCs built.",
			actor.Gameworld.Cells.Count().ToString("N0", actor).Colour(Telnet.Green),
			actor.Gameworld.ItemProtos.Select(x => x.Id).Distinct().Count().ToString("N0", actor).Colour(Telnet.Green),
			actor.Gameworld.NpcTemplates.Select(x => x.Id).Distinct().Count().ToString("N0", actor)
				 .Colour(Telnet.Green)));
		sb.AppendLineFormat("There are {0} items and {1} NPCs in the game world.",
			actor.Gameworld.Items.Count().ToString("N0", actor).Colour(Telnet.Green),
			actor.Gameworld.NPCs.Count().ToString("N0", actor).Colour(Telnet.Green)
		);
		sb.AppendLine(
			$"There are {actor.Gameworld.Crafts.Select(x => x.Id).Distinct().Count().ToString("N0", actor).Colour(Telnet.Green)} distinct crafts.");

		actor.OutputHandler.Send(sb.ToString());
		actor.AddEffect(new CommandDelay(actor, "Statistics"), TimeSpan.FromSeconds(10));
	}

	[PlayerCommand("Who", "who")]
	protected static void Who(ICharacter actor, string input)
	{
		var whocharacters =
			actor.Gameworld.Characters
			     .Where(x => !x.IsAdministrator() && !x.IsGuest)
			     .Concat(
				     actor.Gameworld.NPCs.Where(x => x.EffectsOfType<ICountForWho>().Any())
			     )
			     .ToList();
		var count = whocharacters.Count;
		var guestCount = actor.Gameworld.Characters.Count(x => x.IsGuest);
		
		var sphere = actor.Gameworld.FutureProgs.Get(actor.Gameworld.GetStaticLong("CharacterSphereProgId"))?.ExecuteString(actor) ?? string.Empty;
		var extraText = new StringBuilder();
		foreach (var clan in actor.ClanMemberships.Where(x => x.Clan.ShowClanMembersInWho))
		{
			if (!string.IsNullOrEmpty(sphere) && !clan.Clan.Sphere.EqualTo(sphere))
			{
				continue;
			}

			var clanMembers = whocharacters.Where(x => x.ClanMemberships.Any(y => y.Clan == clan.Clan)).ToList();
			extraText.AppendFormat(actor, "\nThere are {0} {1} of {2} online.", 
				clanMembers.Count.ToStringN0Colour(actor),
				clanMembers.Count == 1 ? "member" : "members", 
				clan.Clan.FullName.Colour(Telnet.Green));
		}

		var meetingPlaces = actor.Gameworld.Cells
		                         .SelectNotNull(x => x.EffectsOfType<LandmarkEffect>().FirstOrDefault())
		                         .Where(x => 
			                         x.IsMeetingPlace &&
									 x.ApplicabilityProg?.ExecuteBool(false, x.Owner, actor) != false &&
									 (string.IsNullOrEmpty(sphere) || x.Sphere.EqualTo(sphere))
			                      )
		                         .ToList()
		                         ;
		foreach (var place in meetingPlaces)
		{
			var cellPlace = (ICell)place.Owner;
			var locationCount = whocharacters.Count(x => x.Location == cellPlace);
			if (locationCount == 0)
			{
				continue;
			}

			if (locationCount == 1)
			{
				extraText.AppendLine();
				extraText.Append(string.Format(actor, actor.Gameworld.GetStaticString("WhoTextMeetingPlaceOnePerson"), 1, cellPlace.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee)));
				continue;
			}

			extraText.AppendLine();
			extraText.Append(string.Format(actor, actor.Gameworld.GetStaticString("WhoTextMeetingPlaceMultiplePersons"), locationCount, cellPlace.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee)));
		}

		var availableAdmins = actor.Gameworld.Characters.Where(x => x.AffectedBy<IAdminAvailableEffect>()).ToList();
		if (availableAdmins.Any())
		{
			extraText.Append(
				$"\n\nThe following members of staff are available:\n{availableAdmins.Select(x => "\t" + x.Account.Name.TitleCase().ColourName()).ListToString(separator: "\n", twoItemJoiner: "\n", conjunction: "")}");
		}

		actor.OutputHandler.Send(string.Format(actor,
			actor.Gameworld.GetStaticString(count == 0
				? "WhoTextNoneOnline"
				: count == 1
					? "WhoTextOneOnline"
					: "WhoText"),
			count.ToStringN0Colour(actor),
			actor.Gameworld.GameStatistics.RecordOnlinePlayers.ToStringN0Colour(actor),
			actor.Gameworld.GameStatistics.RecordOnlinePlayersDateTime.GetLocalDateString(actor).ColourValue(),
			extraText.Length > 0
				? extraText.ToString()
				: "",
			guestCount > 0
				? $"\nThere are {guestCount.ToStringN0Colour(actor)} guest{(guestCount == 1 ? "" : "s")} in the guest lounge."
				: ""
		).SubstituteANSIColour().Wrap(actor.InnerLineFormatLength));
	}

	[PlayerCommand("Petition", "petition")]
	[HelpInfo("petition", @"The petition command is used to send a message to staff. This will also send the message to a staff discord, in case nobody is on, and log it to a board that they can read in game.

The syntax for this command is as follows:

	#3petition all <message>#0 - petition all staff
	#3petition guides <message>#0 - petition all staff and guides
	#3petition <who> <message>#0 - send a message to a specific visible online staff member", AutoHelp.HelpArgOrNoArg)]
	protected static void Petition(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var targetText = ss.Pop();

		var petitionText = ss.RemainingArgument.ProperSentences().Fullstop();
		if (petitionText.Length > 350)
		{
			actor.Send("You cannot petition that much text at once. Please keep it under 350 characters.");
			return;
		}

		if (targetText.Equals("all", StringComparison.InvariantCultureIgnoreCase))
		{
			var staff = actor.Gameworld.Actors.Where(x => x.IsAdministrator() || x.AffectedBy<Switched>()).ToList();
			staff.Handle(
				$"{$"[Petition: {actor.PersonalName.GetName(NameStyle.FullName)} ({actor.Account.Name.TitleCase()})]".Colour(Telnet.Cyan)} {ss.RemainingArgument.ProperSentences().Fullstop()}");
			actor.OutputHandler.Send(("You petition to all staff: " + petitionText).Wrap(actor.InnerLineFormatLength));
			if (!staff.Any())
			{
				actor.Gameworld.Boards.Get(actor.Gameworld.GetStaticLong("PetitionsBoardId"))?.MakeNewPost(
					actor.Account,
					$"Missed petition from {actor.Id} ({actor.PersonalName.GetName(NameStyle.FullWithNickname)})",
					$"Petition text was:\n\n{petitionText.Wrap(80)}"
				);
			}

			actor.Gameworld.DiscordConnection.NotifyPetition(actor.Account.Name,
				$"{actor.Location.HowSeen(actor, false, Form.Shape.DescriptionType.Short, false, PerceiveIgnoreFlags.IgnoreCanSee)} (#{actor.Location.Id})",
				petitionText);
		}
		else if (targetText.Equals("guides", StringComparison.InvariantCultureIgnoreCase))
		{
			if (actor.PermissionLevel < PermissionLevel.Guide && !actor.AffectedBy<INewPlayerEffect>())
			{
				actor.OutputHandler.Send(
					"You are not a new player, and so cannot petition guides. Consider petitioning the staff instead.");
				return;
			}

			actor.Gameworld.Actors.Where(x => x.PermissionLevel >= PermissionLevel.Guide || x.AffectedBy<Switched>()).Handle(
				$"{$"[Guide Petition: {actor.Account.Name.TitleCase()}]".Colour(Telnet.Cyan)} {ss.RemainingArgument.ProperSentences().Fullstop()}");
			actor.OutputHandler.Send("You petition to all guides and staff: " + petitionText);
		}
		else
		{
			var target =
				actor.Gameworld.Characters.Where(x => x.AffectedBy<IAdminAvailableEffect>())
					 .FirstOrDefault(
						 x => x.Account.Name.Equals(targetText, StringComparison.InvariantCultureIgnoreCase));
			if (target == null)
			{
				actor.OutputHandler.Send(
					"No member of staff by that name is available to take petitions. Consider using " +
					"petition all".Colour(Telnet.Yellow) + ".");
				return;
			}

			target.Send("{0} {1}",
				$"[Direct Petition: {actor.PersonalName.GetName(NameStyle.FullName)}]".Colour(Telnet.Cyan),
				ss.RemainingArgument.ProperSentences().Fullstop());
			actor.Send("You petition to {0}: {1}", target.Account.Name.TitleCase(), petitionText.Fullstop());
		}
	}

	[PlayerCommand("Stop", "stop")]
	[RequiredCharacterState(CharacterState.Conscious)]
	protected static void Stop(ICharacter actor, string input)
	{
		actor.Stop(false);
	}

	[PlayerCommand("Landmarks", "landmarks")]
	[RequiredCharacterState(CharacterState.Conscious)]
	[HelpInfo("landmarks", @"The #3landmarks#0 command is used to view landmarks, which are locations that are commonly known to people in the game world.

You can use the #3landmarks#0 syntax to see all landmarks that you know, and #3landmarks <which>#0 to see detailed information about a landmark, including directions to get there from where you are.", AutoHelp.HelpArg)]
	protected static void Landmarks(ICharacter actor, string input)
	{
		var sphere = actor.Gameworld.FutureProgs.Get(actor.Gameworld.GetStaticLong("CharacterSphereProgId"))?.ExecuteString(actor) ?? string.Empty;
		var landmarks = actor.Gameworld.Cells
		                         .SelectNotNull(x => x.EffectsOfType<LandmarkEffect>().FirstOrDefault())
		                         .Where(x =>
			                         x.ApplicabilityProg?.ExecuteBool(false, x.Owner, actor) != false &&
			                         (string.IsNullOrEmpty(sphere) || x.Sphere.EqualTo(sphere))
		                         )
		                         .ToList()
			;

		var sb = new StringBuilder();

		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			sb.AppendLine("You know about the following landmarks:");
			sb.AppendLine();
			sb.AppendLine(StringUtilities.GetTextTable(
				from item in landmarks
				let cell = (ICell)item.Owner
				let distance = actor.Location == cell ? -1 : actor.DistanceBetween(cell, 50)
				select new List<string>
				{
					cell.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee),
					cell.CurrentOverlay.Terrain.Name,
					item.IsMeetingPlace.ToColouredString(),
					cell.SafeQuit.ToColouredString()
				},
				new List<string>
				{
					"Landmark",
					"Terrain",
					"Meeting Place",
					"Safe Quit"
				},
				actor,
				Telnet.Yellow
			));
			actor.OutputHandler.Send(sb.ToString());
			actor.AddEffect(new CommandDelay(actor, ["Landmarks", "Navigate"]), TimeSpan.FromSeconds(5));
			return;
		}

		var cells = landmarks.Select(x => (ICell)x.Owner).ToList();
		var targetText = ss.SafeRemainingArgument;
		var target = cells.FirstOrDefault(x => x.Name.EqualTo(targetText)) ??
		             cells.FirstOrDefault(x => x.Name.StartsWith(targetText, StringComparison.InvariantCultureIgnoreCase)) ??
		             cells.FirstOrDefault(x => x.Name.Contains(targetText, StringComparison.InvariantCultureIgnoreCase));
		if (target is null)
		{
			actor.OutputHandler.Send($"You're not aware of any landmark with the keyword {ss.SafeRemainingArgument.ColourCommand()}.");
			return;
		}

		var landmark = landmarks.First(x => x.Owner == target);
		sb.AppendLine("Known Landmark".GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Location: {target.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee)}");
		sb.AppendLine();
		sb.AppendLine($"Description:");
		sb.AppendLine();
		sb.AppendLine(target.ProcessedFullDescription(actor, PerceiveIgnoreFlags.IgnoreCanSee, target.CurrentOverlay).Wrap(actor.InnerLineFormatLength, "\t"));
		sb.AppendLine();
		sb.AppendLine($"Terrain: {target.CurrentOverlay.Terrain.Name.ColourValue()}");
		sb.AppendLine($"Meeting Place: {landmark.IsMeetingPlace.ToColouredString()}");
		sb.AppendLine($"Safe Quit: {target.SafeQuit.ToColouredString()}");
		sb.AppendLine();
		
		var path = actor.ExitsBetween(target, 100).ToList();
		string pathDescription = "";
		if (path.Count == 0)
		{
			if (target == actor.Location)
			{
				pathDescription = "";
			}
			else
			{
				pathDescription = "no viable path".ColourError();
			}
		}
		else
		{
			if (path.Count > actor.Gameworld.GetStaticInt("MaximumLandmarkDirectionsDistance"))
			{
				pathDescription = $"very far away to {path.DescribeDirection()}";
			}
			else
			{
				pathDescription = path
								  .Select(x =>
									{
										if (x.OutboundDirection != CardinalDirection.Unknown)
										{
											return x.OutboundDirection.DescribeBrief();
										}

										return x is NonCardinalCellExit nc ? $"'{nc.Verb} {nc.PrimaryKeyword}'".ToLowerInvariant() : "??";
									})
									  .ListToString(separator: " ", conjunction: "");
			}
		}
		
		sb.AppendLine($"Path: {pathDescription.ColourCommand()}".Wrap(actor.InnerLineFormatLength));
		var index = 1;
		foreach (var text in landmark.LandmarkDescriptionTexts.Where(x => x.Prog.ExecuteBool(actor)))
		{
			sb.AppendLine();
			sb.AppendLine($"About #{index++.ToStringN0(actor)}".GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
			sb.AppendLine();
			sb.AppendLine(text.Text.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength));
		}
		actor.OutputHandler.Send(sb.ToString());
		actor.AddEffect(new CommandDelay(actor, ["Landmarks", "Navigate"]), TimeSpan.FromSeconds(5));

	}

	[PlayerCommand("TagSearch", "tagsearch")]
	[HelpInfo("tagsearch", @"The #3tagsearch#0 command allows you to search for items that have the tag you specify. This might be a tag for an item you need to complete a craft or a project for example.

The syntax is #3tagsearch <tag>#0.", AutoHelp.HelpArgOrNoArg)]
	protected static void TagSearch(ICharacter actor, string input)
	{
		var cmd = input.RemoveFirstWord();
		var tag = actor.Gameworld.Tags.GetByIdOrName(cmd);
		if (tag is null || tag.ShouldSeeProg?.ExecuteBool(actor) == false)
		{
			actor.OutputHandler.Send("You don't know of any tag like that.");
			return;
		}

		var items = actor.Gameworld.ItemProtos.Where(x =>
			x.Status == Framework.Revision.RevisionStatus.Current &&
			x.Tags.Any(y => y.IsA(tag)) &&
			!x.IsHiddenFromPlayers
		).ToList();
		if (items.Count == 0)
		{
			actor.OutputHandler.Send($"The tag {tag.FullName.ColourName()} does not have any items that you know of.");
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine($"The tag {tag.FullName.ColourName()} has the following items that you know of:");
		sb.AppendLine(StringUtilities.GetTextTable(
			from item in items
			select new List<string>
			{
				item.Name,
				item.ShortDescription.Colour(item.CustomColour ?? Telnet.Green)
			},
			new List<string>
			{
				"Name",
				"Description"
			},
			actor,
			Telnet.Green
		));
		actor.OutputHandler.Send(sb.ToString());
	}

	[PlayerCommand("Teach", "teach")]
	[CommandPermission(PermissionLevel.Player)]
	[RequiredCharacterState(CharacterState.Conscious)]
	[NoHideCommand]
	[NoCombatCommand]
	[NoMovementCommand]
	[HelpInfo("teach",
		"This command allows you to teach a skill or a knowledge to a student. If you want to specify a knowledge that has the same name as a skill you know, you can begin the knowledge's name with a * to specify that you want the knowledge.\nWhen you teach someone, they gain a little bit of learning fatigue which gets longer and longer the more it is added to. In this way, spamming the skill is actually worse over the long term.\nSome skills and knowledges may take several successful lessons to branch, and some are easier to teach or learn than others. The teacher also gets an opportunity to improve their own skills through the experience.\nSyntax: teach <target> <skill/knowledge>.\nWarning: This command must be supported by adequate roleplay to justify the lesson.",
		AutoHelp.HelpArgOrNoArg)]
	protected static void Teach(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var target = actor.TargetActor(ss.Pop());
		if (target == null)
		{
			actor.Send("You do not see that person to teach.");
			return;
		}

		if (target == actor)
		{
			actor.Send("You cannot teach things to yourself.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("What do you want to teach to {0}?", target.HowSeen(actor));
			return;
		}

		var skilltext = ss.PopSpeech();
		var skillValue =
			actor.TraitsOfType(TraitType.Skill)
				 .FirstOrDefault(
					 x => x.Definition.Name.StartsWith(skilltext, StringComparison.InvariantCultureIgnoreCase));
		ISkillDefinition skill = null;
		if (skillValue == null || skilltext[0] == '*')
		{
			if (skilltext[0] == '*')
			{
				skilltext = skilltext.Substring(1);
				if (skilltext.Length < 1)
				{
					actor.Send("Which knowledge do you want to teach?");
					return;
				}
			}

			var knowledge =
				actor.CharacterKnowledges.FirstOrDefault(
					x => x.Knowledge.Name.StartsWith(skilltext, StringComparison.InvariantCultureIgnoreCase));
			if (knowledge == null)
			{
				actor.Send("That is not a valid skill or knowledge that you know.");
				return;
			}

			if (!knowledge.Knowledge.Learnable.HasFlag(LearnableType.LearnableFromTeacher))
			{
				actor.Send("The knowledge of {0} is not something that can be taught.",
					knowledge.Knowledge.Name.Colour(Telnet.Cyan));
				return;
			}

			target.AddEffect(new Accept(target, new KnowledgeLessonProposal(actor, target, knowledge.Knowledge)),
				TimeSpan.FromSeconds(90));
			actor.OutputHandler.Handle(new EmoteOutput(new Emote(
				$"@ offer|offers to teach {knowledge.Knowledge.Name.Colour(Telnet.Cyan)} to $1.", actor, actor,
				target), flags: OutputFlags.SuppressObscured));
			return;
		}

		skill = skillValue.Definition as ISkillDefinition;
		if (!skill.CanTeach(actor))
		{
			actor.Send("The {0} skill is not something that you can teach.", skill.Name.Colour(Telnet.Cyan));
			return;
		}

		target.AddEffect(new Accept(target, new SkillLessonProposal(actor, target, skill)), TimeSpan.FromSeconds(90));
		actor.OutputHandler.Handle(new EmoteOutput(new Emote(
				$"@ offer|offers to teach {skill.Name.Colour(Telnet.Cyan)} to $1.", actor, actor, target),
			flags: OutputFlags.SuppressObscured));
	}

	[PlayerCommand("Learn", "learn")]
	[CommandPermission(PermissionLevel.Player)]
	protected static void Learn(ICharacter actor, string input)
	{
		var fatigue = actor.EffectsOfType<LearningFatigueEffect>().FirstOrDefault();
		var branch = actor.EffectsOfType<IncreasedBranchChance>().FirstOrDefault();
		var accent = actor.EffectsOfType<NoAccentGain>().FirstOrDefault();
		var traits = actor.EffectsOfType<INoTraitGainEffect>().ToList();

		var sb = new StringBuilder();
		if (fatigue == null)
		{
			sb.AppendLine(
				"You are not suffering from any learning fatigue at the moment.".Colour(Telnet.BoldGreen));
		}
		else
		{
			if (fatigue.BlockUntil >= DateTime.UtcNow)
			{
				sb.AppendLine(
					"You are too fatigued of learning to get any benefit from instructor's lessons at present."
						.Colour(Telnet.BoldRed));
			}
			else
			{
				sb.AppendLine(
					$"You are suffering from learning fatigue, and all learning checks will be {fatigue.FatigueDegrees.ToString("N0", actor)} degrees of difficulty harder."
						.Colour(Telnet.BoldYellow));
			}
		}

		if (traits.Any())
		{
			sb.AppendLine(
				$"You have cooldowns for skill-ups on {traits.Select(x => x.Trait.Name.Colour(Telnet.Green)).ListToString()}.");
		}
		else
		{
			sb.AppendLine("None of your skills are on cooldown for skill-ups.");
		}

		if (branch == null)
		{
			sb.AppendLine("You do not have any accumulated skill branch modifiers.");
			sb.AppendLine("You are not in the progress of learning any new knowledges.");
		}
		else
		{
			var skills = branch.GetSkills().ToList();
			if (skills.Any())
			{
				sb.AppendLine("You have accumulated the following modifiers to your BASE branch chances:");
				var expr = IncreasedBranchChance.IncreasedCapExpression;
				foreach (var skill in skills)
				{
					expr.Formula.Parameters["base"] = 1.0;
					expr.Formula.Parameters["attempts"] = skill.Attempts;
					var language = actor.Gameworld.Languages.FirstOrDefault(x => x.LinkedTrait == skill.Skill);
					var skillName = language == null
						? skill.Skill.Name
						: language.UnknownLanguageSpokenDescription;
					sb.AppendLine(
						$"\t{expr.Evaluate(actor).ToString("N2", actor)}x with {skillName.Colour(Telnet.Green)}");
				}
			}
			else
			{
				sb.AppendLine("You do not have any accumulated skill branch modifiers.");
			}

			var knowledges = branch.GetKnowledges().ToList();
			if (knowledges.Any())
			{
				foreach (var knowledge in knowledges)
				{
					var ratio = knowledge.Lessons / knowledge.Knowledge.LearnerSessionsRequired;
					if (ratio >= 0.75)
					{
						sb.AppendLine(
							$"You feel like you are on the cusp of understanding the {knowledge.Knowledge.Name.Colour(Telnet.Green)} knowledge.");
					}
					else if (ratio >= 0.5)
					{
						sb.AppendLine(
							$"You feel like you are most of the way to understanding the {knowledge.Knowledge.Name.Colour(Telnet.Green)} knowledge.");
					}
					else if (ratio >= 0.1)
					{
						sb.AppendLine(
							$"You feel like you are making headway to understanding the {knowledge.Knowledge.Name.Colour(Telnet.Green)} knowledge.");
					}
					else
					{
						sb.AppendLine(
							$"You feel like you still have a long way to understanding the {knowledge.Knowledge.Name.Colour(Telnet.Green)} knowledge.");
					}
				}
			}
			else
			{
				sb.AppendLine("You are not in the progress of learning any new knowledges.");
			}
		}

		var realAccents = accent?.Accents.Where(x => actor.Languages.Contains(x.Language)).ToList() ??
						  new List<IAccent>();
		if (realAccents.Any())
		{
			sb.AppendLine("You are not gaining any familiarity with the following accents:");

			foreach (var item in realAccents)
			{
				sb.AppendLine(
					$"\t{item.Name.Colour(Telnet.Green)} in {item.Language.Name.Colour(Telnet.Cyan)}.");
			}
		}
		else
		{
			sb.AppendLine("You are gaining familiarity with all accents that you hear at the present time.");
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	[PlayerCommand("Quit", "quit")]
	[RequiredCharacterState(CharacterState.Quittable)]
	[DelayBlock("general", "You must first stop {0} before you can quit.")]
	[NoCombatCommand]
	[NoMovementCommand]
	protected static void Quit(ICharacter actor, string input)
	{
		if (actor is NPC.NPC)
		{
			actor.OutputHandler.Send("NPCs cannot quit.");
			return;
		}

		if (!new StringStack(input).Pop().Equals("quit", StringComparison.InvariantCultureIgnoreCase))
		{
			actor.Send("You must type out the entire word {0} to quit.", "quit".Colour(Telnet.Yellow));
			return;
		}


		if (!actor.IsAdministrator() && !actor.Location.SafeQuit)
		{
			actor.OutputHandler?.Send(actor.Gameworld.GetStaticString("CantQuitNoSafeQuitEcho"));
			return;
		}

		if (actor.CombinedEffectsOfType<INoQuitEffect>().Any(x => x.Applies()))
		{
			actor.OutputHandler?.Send(actor.CombinedEffectsOfType<INoQuitEffect>().First(x => x.Applies())
										   .NoQuitReason);
			return;
		}

		actor.Quit();
	}

	[PlayerCommand("Forage", "forage")]
	[DelayBlock("general", "You must first stop {0} before you can quit.")]
	[RequiredCharacterState(CharacterState.Conscious)]
	[NoHideCommand]
	[NoCombatCommand]
	[NoMovementCommand]
	[HelpInfo("forage",
		@"The forage command is used to look for useful items in your location. You must always forage against a 'yield type', for example you might be foraging for 'food', or for 'wood'. As you forage items from these categories there is less of it to find in the current location, and it may or may not regenerate over time.

The syntax to use with this command is as follows:

	forage <yield> [into <container>]

You can also type 'forage' on its own to see what kinds of yields you can search for in the area.", AutoHelp.HelpArg)]
	protected static void Forage(ICharacter actor, string input)
	{
		var profile = actor.Location.ForagableProfile;
		if (profile == null)
		{
			actor.Send("This location has absolutely nothing that may be foraged.");
			return;
		}

		var forageTypes =
			profile.Foragables.SelectMany(x => x.ForagableTypes)
				   .Select(x => x.ToLowerInvariant())
				   .Where(x => !string.IsNullOrEmpty(x))
				   .Distinct()
				   .ToList();
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.Send("You must specify whether you want to forage for {0} in this location.",
				forageTypes.Select(x => x.Colour(Telnet.Green)).ListToString());
			return;
		}

		var type = ss.PopSpeech();
		IGameItem targetContainer = null;
		if (!ss.IsFinished)
		{
			if (ss.Peek().Equals("into", StringComparison.InvariantCultureIgnoreCase))
			{
				ss.Pop();
				if (ss.IsFinished)
				{
					actor.Send("Into what container do you want to forage for items?");
					return;
				}

				targetContainer = actor.TargetItem(ss.Pop());
				if (targetContainer == null)
				{
					actor.Send("There is no such container into which you can forage.");
					return;
				}

				if (!targetContainer.IsItemType<IContainer>())
				{
					actor.Send("{0} is not a container.", targetContainer.HowSeen(actor, true));
					return;
				}

				var (truth, error) = actor.CanManipulateItem(targetContainer);
				if (!truth)
				{
					actor.OutputHandler.Send(error);
					return;
				}
			}
			else
			{
				actor.Send("Foraging for specific items is coming soon.");
				return;
			}
		}

		var time = Dice.Roll(Foragable.BaseForageTimeExpression);
		actor.AddEffect(new SimpleCharacterAction(actor, perceivable =>
			{
				var forageCheck = actor.Gameworld.GetCheck(CheckType.ForageCheck);
				var forageOutcome =
					forageCheck.CheckAgainstAllDifficulties(actor, Difficulty.Normal, null,
						customParameters: ("yield", type));
				var foragable = profile.GetForageResult(actor, forageOutcome, type);
				if (foragable == null)
				{
					actor.OutputHandler.Handle(
						new EmoteOutput(
							new Emote(
								"@ finish|finishes foraging, but are|is not successful in finding what #0 were|was looking for.",
								actor, actor)));
					return;
				}

				if (foragable.ForagableTypes.All(x => actor.Location.GetForagableYield(x) <= 0.0))
				{
					actor.OutputHandler.Handle(
						new EmoteOutput(
							new Emote(
								"@ finish|finishes foraging, but are|is not successful in finding what #0 were|was looking for.",
								actor, actor)));
					if (forageOutcome[Difficulty.Normal] == Outcome.MajorPass)
					{
						actor.Send(
							"You are fairly certain that the area is picked clean of that type of thing for now.".Colour
								(Telnet.Yellow));
					}

					return;
				}

				actor.Location.ConsumeYieldFor(foragable);
				var newItems = new List<IGameItem>();
				var quantity =
					(int)Math.Floor(new TraitExpression(foragable.QuantityDiceExpression, actor.Gameworld).EvaluateWith(
						actor,
						values: ("outcome", forageOutcome[foragable.ForageDifficulty])));
				if (foragable.ItemProto.IsItemType<StackableGameItemComponentProto>())
				{
					var newItem = foragable.ItemProto.CreateNew(actor);
					newItem.GetItemType<IStackable>().Quantity = quantity;
					newItems.Add(newItem);
					actor.Gameworld.Add(newItem);
				}
				else
				{
					for (var i = 0; i < quantity; i++)
					{
						var newItem = foragable.ItemProto.CreateNew(actor);
						newItems.Add(newItem);
						actor.Gameworld.Add(newItem);
					}
				}

				if (foragable.OnForageProg != null)
				{
					foreach (var item in newItems)
					{
						foragable.OnForageProg.Execute(actor, foragable.Id, item, 1);
						item.HandleEvent(EventType.ItemFinishedLoading, item);
						item.Login();
					}
				}

				actor.OutputHandler.Handle(new EmoteOutput(new Emote(
					$"@ finish|finishes foraging, and find|finds {newItems.Select(x => x.HowSeen(actor)).ListToString()}.",
					actor, actor)));

				if (targetContainer != null &&
					(targetContainer.ContainedIn != null ||
					 (targetContainer.IsItemType<IOpenable>() && !targetContainer.GetItemType<IOpenable>().IsOpen) ||
					 !targetContainer.TrueLocations.Contains(actor.Location) ||
					 newItems.Any(x => !x.IsItemType<IHoldable>() || x.GetItemType<IHoldable>()?.IsHoldable == false)
					))
				{
					targetContainer = null;
				}

				if (targetContainer != null)
				{
					var container = targetContainer.GetItemType<IContainer>();
					bool notInContainer = false, someInRoom = false;
					foreach (var item in newItems)
					{
						if (container.CanPut(item))
						{
							container.Put(actor, item);
						}
						else if (actor.Body.CanGet(item, 0))
						{
							actor.Body.Get(item, 0, silent: true);
							notInContainer = true;
						}
						else
						{
							item.RoomLayer = actor.RoomLayer;
							actor.Location.Insert(item);
							someInRoom = true;
						}
					}

					if (!notInContainer && !someInRoom)
					{
						actor.Send("You put all of the items into {0}.", targetContainer.HowSeen(actor));
					}
					else if (!someInRoom)
					{
						actor.Send("You put most of the items into {0}, but have to hold on to some.",
							targetContainer.HowSeen(actor));
					}
					else if (!notInContainer)
					{
						actor.Send("You put most of the items into {0}, but have to put some of them down.",
							targetContainer.HowSeen(actor));
					}
					else
					{
						actor.Send(
							"You put most of the items into {0}, but have to hold on to some and put others down.",
							targetContainer.HowSeen(actor));
					}

					return;
				}

				var anyInLocation = false;
				foreach (var item in newItems)
				{
					if (actor.Body.CanGet(item, 0))
					{
						actor.Body.Get(item, 0, silent: true);
					}
					else
					{
						item.RoomLayer = actor.RoomLayer;
						actor.Location.Insert(item);
						anyInLocation = true;
					}

					item.HandleEvent(Events.EventType.ItemFinishedLoading, item);
					item.Login();
				}

				if (anyInLocation)
				{
					actor.Send("You put some of the items onto the ground as you cannot hold them all.");
				}
			}, "foraging for " + type, new[] { "general", "movement" }, "foraging about the area"),
			TimeSpan.FromSeconds(time));

		actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ begin|begins foraging about the area.", actor)));
	}

	[PlayerCommand("Implant", "implant")]
	[RequiredCharacterState(CharacterState.Conscious)]
	protected static void Implant(ICharacter actor, string input)
	{
		if (!actor.Gameworld.GetStaticBool("ImplantCommandEnabled"))
		{
			actor.Send(actor.Gameworld.GetStaticString("FailedToFindCommand"));
			return;
		}

		var dni = actor.Body.Implants.OfType<IImplantNeuralLink>().ToList();
		if (!dni.Any())
		{
			actor.OutputHandler.Send(
				"You do not have any direct neural interface implants, so you cannot issue any commands to your implants.");
			return;
		}

		if (!dni.Any(x => x.DNIConnected))
		{
			actor.OutputHandler.Send("Your neural interfaces are unpowered and offline.");
			return;
		}

		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.Peek().EqualTo("status"))
		{
			foreach (var implant in dni)
			{
				if (!implant.DNIConnected)
				{
					continue;
				}

				implant.DoReportStatus();
			}

			return;
		}

		var alias = ss.PopSpeech();
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which command did you want to issue to that implant?");
			return;
		}

		var command = ss.PopSpeech();

		foreach (var implant in dni)
		{
			if (!implant.DNIConnected)
			{
				continue;
			}

			implant.IssueCommand(alias, command, new StringStack(ss.RemainingArgument));
		}
	}

	[PlayerCommand("Clean", "clean")]
	[DelayBlock("general", "You must first stop {0} before you can clean anything.")]
	[RequiredCharacterState(CharacterState.Conscious)]
	[NoHideCommand]
	[NoCombatCommand]
	[NoMovementCommand]
	[HelpInfo("clean",
		"The clean command allows you to clean items that have become dirty. You can clean a specific item, you can CLEAN MINE to clean all items on your person, and you can CLEAN ALL to clean all items on your person and in the vicinity. Note that with respect to items that you are wearing, you can only clean your external layers; if you want to clean your underpants, you must first remove your trousers. Also note that certain kinds of messes require specific liquids or solvents to remove.",
		AutoHelp.HelpArg)]
	protected static void Clean(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			CleanAll(actor, false);
			return;
		}

		if (ss.Pop().EqualTo("all"))
		{
			CleanAll(actor, false);
			return;
		}

		if (ss.Last.EqualTo("mine"))
		{
			CleanAll(actor, true);
			return;
		}

		var target = actor.Target(ss.Last);
		if (target == null)
		{
			actor.Send("You don't see anything like that here to clean.");
			return;
		}

		CleanTarget(actor, target);
	}

	[PlayerCommand("Accept", "accept", "decline", "abort")]
	[DisplayOptions(CommandDisplayOptions.DisplayCommandWords)]
	[RequiredCharacterState(CharacterState.Conscious)]
	protected static void Accept(ICharacter actor, string input)
	{
		var ss = new StringStack(input);
		var cmd = ss.Pop();
		if ("accept".StartsWith(cmd, StringComparison.InvariantCultureIgnoreCase))
		{
			cmd = "accept";
		}
		else if ("decline".StartsWith(cmd, StringComparison.InvariantCultureIgnoreCase))
		{
			cmd = "decline";
		}
		else if ("abort".StartsWith(cmd, StringComparison.InvariantCultureIgnoreCase))
		{
			cmd = "abort";
		}
		else
		{
			throw new NotSupportedException("Invalid command word in Accept command");
		}

		switch (ss.Peek())
		{
			case "":
				ProposalNoArgument(actor, cmd);
				break;
			case "?":
				ListEffects(actor);
				break;
			case "all":
				ProposalAll(actor, cmd);
				break;
			default:
				ProposalKeyword(actor, ss, cmd);
				break;
		}
	}

	private static void ListEffects(ICharacter actor)
	{
		var effects = actor.EffectsOfType<IProposalEffect>().ToList();
		if (!effects.Any())
		{
			actor.OutputHandler.Send("You have no pending transactions.");
			return;
		}

		var sb = new StringBuilder("You have the following pending transactions:\n\n");
		foreach (var effect in effects)
		{
			sb.AppendLine("\t" + effect.Proposal.Describe(actor));
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	private static void ProposalNoArgument(ICharacter actor, string action)
	{
		var effect = actor.EffectsOfType<IProposalEffect>().FirstOrDefault();
		if (effect == null)
		{
			actor.OutputHandler.Send("You have no pending transactions.");
			return;
		}

		switch (action)
		{
			case "accept":
				effect.Proposal.Accept(); // Accept action is responsible for any echoes
				break;
			case "decline":
				effect.Proposal.Reject(); // Reject action is responsible for any echoes
				break;
			case "abort":
				actor.Send("You abort {0}.", effect.Proposal.Describe(actor));
				break;
			default:
				throw new NotSupportedException("Invalid action type in ProposalNoArgument");
		}

		actor.RemoveEffect(effect);
	}

	private static void ProposalAll(ICharacter actor, string action)
	{
		var effects = actor.EffectsOfType<IProposalEffect>().ToList();
		if (!effects.Any())
		{
			actor.OutputHandler.Send("You have no pending transactions.");
			return;
		}

		foreach (var effect in effects)
		{
			switch (action)
			{
				case "accept":
					effect.Proposal.Accept();
					break;
				case "decline":
					effect.Proposal.Reject();
					break;
				case "abort":
					break;
				default:
					throw new NotSupportedException("Not supported action type in ProposalAll");
			}

			actor.RemoveEffect(effect);
		}


		if (action == "abort")
		{
			actor.Send("You abort all of your pending transactions.");
		}
	}

	private static void ProposalKeyword(ICharacter actor, StringStack ss, string action)
	{
		var item = actor.EffectsOfType<IProposalEffect>().GetFromItemListByKeyword(ss.Pop(), actor);
		if (item == null)
		{
			actor.OutputHandler.Send("You have no such pending transaction.");
			return;
		}

		var comment = ss.IsFinished ? "" : ss.SafeRemainingArgument;

		switch (action)
		{
			case "accept":
				item.Proposal.Accept(comment);
				break;
			case "decline":
				item.Proposal.Reject(comment);
				break;
			case "abort":
				actor.Send("You abort {0}.", item.Proposal.Describe(actor));
				break;
			default:
				throw new NotSupportedException("Unsupported action in ProposalKeyword");
		}

		actor.RemoveEffect(item);
	}
	
	public static int GetCommandGroupScore(string group)
	{
		switch (group?.ToLowerInvariant())
		{
			case "game":
				return 0;
			case "movement":
				return 1;
			case "position":
				return 2;
			case "perception":
				return 3;
			case "inventory":
				return 4;
			case "manipulation":
				return 5;
			case "communications":
				return 6;
			case "character information":
				return 7;
			case "world":
				return 8;
			case "combat":
				return 9;
			case "health":
				return 10;
			case "clan":
				return 11;
			case "craft":
				return 12;
			case "economy":
				return 13;
		}

		return 100;
	}

	public static IComparer<string> CommandGroupComparer =>
		C5.ComparerFactory<string>.CreateComparer(
			(group1, group2) =>
			{
				var result1 = GetCommandGroupScore(group1);
				var result2 = GetCommandGroupScore(group2);

				if (result1 == result2)
				{
					return 0;
				}

				if (result1 < result2)
				{
					return -1;
				}

				return 1;
			});

	[PlayerCommand("Commands", "commands")]
	protected static void _Commands(ICharacter actor, string input)
	{
		if (actor.Gameworld.GetStaticBool("SplitCommandsIntoGroups"))
		{
			var sb = new StringBuilder();
			foreach (var group in actor.CommandTree.Commands.ReportCommandsInGroups(
						 actor.PermissionLevel <= PermissionLevel.Guide
							 ? actor.PermissionLevel
							 : PermissionLevel.Guide, actor).OrderBy(x => x.Key, CommandGroupComparer))
			{
				sb.AppendLine(group.Key.GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldYellow));
				sb.AppendLine();
				sb.AppendLine(group.Select(x => x.ToLowerInvariant()).Distinct().OrderBy(x => x)
								   .ArrangeStringsOntoLines(5, (uint)actor.LineFormatLength));
			}

			actor.OutputHandler.Send(sb.ToString(), nopage: true);
		}
		else
		{
			actor.OutputHandler.Send(
				actor.CommandTree.Commands.ReportCommands(actor.PermissionLevel <= PermissionLevel.Guide
					? actor.PermissionLevel
					: PermissionLevel.Guide, actor).ArrangeStringsOntoLines(5, (uint)actor.LineFormatLength),
				nopage: true);
		}
	}

	#region Clean SubCommands

	private static Action<IPerceivable> CleanAction(IPerceivable gitem, ICharacter actor,
		Queue<ICleanableEffect> effectQueue, Queue<IPerceivable> itemQueue)
	{
		bool CanBeCleaned(ICleanableEffect effect, IPerceivable item)
		{
			return item.Effects.Contains(effect) &&
				   actor.ContextualItems.Contains(item) &&
				   actor.CanSee(item) &&
				   (effect.CleaningToolTag != null || effect.LiquidRequired != null) &&
				   (effect.CleaningToolTag == null ||
					actor.Body.HeldOrWieldedItems.Any(x => x.IsA(effect.CleaningToolTag))) &&
				   (effect.LiquidRequired == null || actor.ContextualItems
														  .SelectNotNull(x => x.GetItemType<ILiquidContainer>()).Any(
															  x => x.LiquidMixture?.CountsAs(effect.LiquidRequired)
																	.Truth == true && x.IsOpen));
		}

		return item =>
		{
			ICleanableEffect[] effect = { effectQueue.Dequeue() };
			while (effect[0] == null || !CanBeCleaned(effect[0], gitem)
				  )
			{
				if (!effectQueue.Any())
				{
					//empty effectQueue means current item has no more cleanable effects
					actor.OutputHandler.Handle(
						new EmoteOutput(new Emote("@ have|has finished cleaning $0.", actor, gitem)));
					if (itemQueue.Any())
					{
						//if we have any cleanable items left, queue up the next one
						var newItem = itemQueue.Dequeue();
						var newEffectQueue = GetCleanableEffectQueue(newItem, actor, false);
						while (!newEffectQueue.Any())
						{
							newItem = itemQueue.Any() ? itemQueue.Dequeue() : null;
							if (newItem == null)
							{
								actor.OutputHandler.Handle(
									new EmoteOutput(new Emote("@ have|has finished cleaning.", actor, gitem)));
								return;
							}

							newEffectQueue = GetCleanableEffectQueue(newItem, actor, false);
						}

						actor.AddEffect(
							new SimpleCharacterAction(actor, CleanAction(newItem, actor, newEffectQueue, itemQueue),
								$"cleaning {newItem.Name}", new[] { "general", "movement" }, "cleaning an item"),
							newEffectQueue.Peek().BaseCleanTime);
						actor.OutputHandler.Handle(
							new EmoteOutput(new Emote(newEffectQueue.Peek().EmoteBeginClean, actor, newItem)));
					}

					return;
				}

				effect[0] = effectQueue.Dequeue();
			}

			//Some cleaning effects require a specifically tagged tool
			var toolItem = effect[0].CleaningToolTag != null
				? actor.Body.HeldItems.First(x => x.Tags.Any(y => y.IsA(effect[0].CleaningToolTag)))
				: null;

			EmoteOutput output = null;
			if (effect[0].LiquidRequired != null)
			{
				//Some cleaning requires a liquid
				var potentialLiquidItems =
					actor.ContextualItems.SelectNotNull(y => y.GetItemType<ILiquidContainer>())
						 .Where(
							 x => x.LiquidMixture?.CountsAs(effect[0].LiquidRequired).Truth == true && x.IsOpen)
						 .ToList();
				var actualLiquidItems = new Dictionary<ILiquidContainer, double>();
				var actualLiquids = new Dictionary<LiquidMixture, double>();
				var targetAmount = effect[0].LiquidAmountConsumed;
				foreach (var lcon in potentialLiquidItems)
				{
					if (lcon.LiquidMixture.TotalVolume >= targetAmount)
					{
						actualLiquidItems[lcon] = targetAmount;
						var newLiquid = lcon.RemoveLiquidAmount(targetAmount, actor, "clean");
						actualLiquids[newLiquid] = targetAmount;
						break;
					}

					actualLiquidItems[lcon] = lcon.LiquidMixture.TotalVolume;
					actualLiquids[new LiquidMixture(lcon.LiquidMixture)] = lcon.LiquidMixture.TotalVolume;

					targetAmount -= lcon.LiquidMixture.TotalVolume;
					lcon.ReduceLiquidQuantity(lcon.LiquidMixture.TotalVolume, actor, "clean");
				}

				//Contaminate the cleaned item with the liquids used to clean it
				foreach (var liquidUsed in actualLiquids)
				{
					if (liquidUsed.Value <= 0.0)
					{
						continue;
					}

					var newContamEffect = gitem.EffectsOfType<ILiquidContaminationEffect>()
											   .FirstOrDefault(x => x.ContaminatingLiquid.CanMerge(liquidUsed.Key));
					if (newContamEffect != null)
					{
						newContamEffect.ContaminatingLiquid.AddLiquid(liquidUsed.Key);
						gitem.Reschedule(newContamEffect, LiquidContamination.EffectDuration(newContamEffect.ContaminatingLiquid));
					}
					else
					{
						gitem.AddEffect(new LiquidContamination(gitem, liquidUsed.Key),
							LiquidContamination.EffectDuration(liquidUsed.Key));
					}
				}

				var perceiverArgs = new List<IPerceivable> { actor, gitem, toolItem };
				perceiverArgs.AddRange(actualLiquidItems.Select(x => x.Key.Parent));
				var i = 3;
				if (effect[0].LiquidRequired != null)
				{
					output =
						new EmoteOutput(
							new Emote(
								string.Format(effect[0].EmoteFinishClean,
									$"{(toolItem != null ? " and" : "")} with {effect[0].LiquidRequired.Name.ToLowerInvariant().Colour(effect[0].LiquidRequired.DisplayColour)} from {actualLiquidItems.Select(x => $"${i++}").ListToString()}"),
								actor,
								perceiverArgs.ToArray()));
				}
				else
				{
					output =
						new EmoteOutput(
							new Emote(
								string.Format(effect[0].EmoteFinishClean,
									$"{(toolItem != null ? " and" : "")}"),
								actor,
								perceiverArgs.ToArray()));
				}

				foreach (var liquid in actualLiquids)
				{
					if (effect[0].CleanWithLiquid(liquid.Key, liquid.Value))
					{
						gitem.RemoveEffect(effect[0]);
						break;
					}
				}

				if (gitem.Effects.Contains(effect[0]) && CanBeCleaned(effect[0], gitem))
				{
					effectQueue.Enqueue(effect[0]);
				}
			}
			else
			{
				gitem.RemoveEffect(effect[0]);
				output =
					new EmoteOutput(new Emote(string.Format(effect[0].EmoteFinishClean, ""), actor, actor, gitem,
						toolItem));
			}

			actor.OutputHandler.Handle(output);

			if (effectQueue.Any())
			{
				actor.AddEffect(
					new SimpleCharacterAction(actor, CleanAction(gitem, actor, effectQueue, itemQueue),
						$"cleaning {gitem.Name}", new[] { "general", "movement" }, "cleaning an item"),
					effectQueue.Peek().BaseCleanTime);
				return;
			}

			actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ have|has finished cleaning $0.", actor, gitem)));

			if (itemQueue.Any())
			{
				var newItem = itemQueue.Dequeue();
				var newEffectQueue = GetCleanableEffectQueue(newItem, actor, false);
				while (!newEffectQueue.Any())
				{
					newItem = itemQueue.Any() ? itemQueue.Dequeue() : default;
					if (newItem == null)
					{
						actor.OutputHandler.Handle(
							new EmoteOutput(new Emote("@ have|has finished cleaning.", actor, gitem)));
						return;
					}

					newEffectQueue = GetCleanableEffectQueue(newItem, actor, false);
				}

				actor.AddEffect(
					new SimpleCharacterAction(actor, CleanAction(newItem, actor, newEffectQueue, itemQueue),
						$"cleaning {newItem.Name}", new[] { "general", "movement" }, "cleaning an item"),
					newEffectQueue.Peek().BaseCleanTime);
				actor.OutputHandler.Handle(
					new EmoteOutput(new Emote(newEffectQueue.Peek().EmoteBeginClean, actor, newItem)));
			}
			else
			{
				actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ have|has finished cleaning.", actor)));
			}
		};
	}

	private static Queue<ICleanableEffect> GetCleanableEffectQueue(IPerceivable target, ICharacter actor,
		bool echoFailure)
	{
		var originalCleanableEffects =
			target.EffectsOfType<ICleanableEffect>().Where(x => x.Applies(actor)).ToList();
		originalCleanableEffects =
			originalCleanableEffects.Where(x => x.LiquidRequired != null).ToList(); //Filter out non-cleanable liquids
		var cleanableEffects = originalCleanableEffects;
		if (!cleanableEffects.Any())
		{
			if (echoFailure)
			{
				actor.Send($"{target.HowSeen(actor, true)} does not need to be cleaned.");
			}

			return new Queue<ICleanableEffect>();
		}

		cleanableEffects =
			cleanableEffects.Where(
				x =>
					x.CleaningToolTag == null ||
					actor.Body.HeldItems.Any(y => y.Tags.Any(z => z.IsA(x.CleaningToolTag)))).ToList();

		if (!cleanableEffects.Any())
		{
			if (echoFailure)
			{
				actor.Send(
					$"You are missing the correct materials to clean {target.HowSeen(actor)}. You would require items with the {originalCleanableEffects.SelectNotNull(x => x.CleaningToolTag).Distinct().Select(x => x.Name.Colour(Telnet.Green)).ListToString()} tag.");
			}

			return new Queue<ICleanableEffect>();
		}

		bool IsSuitable(ICleanableEffect effect, IGameItem item)
		{
			var container = item.GetItemType<ILiquidContainer>();
			if (container == null)
			{
				return false;
			}

			if (container.LiquidMixture?.CountsAs(effect.LiquidRequired).Truth != true)
			{
				return false;
			}

			if (!container.IsOpen)
			{
				return false;
			}

			return true;
		}

		cleanableEffects =
			cleanableEffects.Where(
								x =>
									x.LiquidRequired == null ||
									actor.ContextualItems.Any(y => IsSuitable(x, y)))
							.ToList();

		if (!cleanableEffects.Any())
		{
			if (echoFailure)
			{
				actor.Send(
					$"You are missing the correct liquids to clean {target.HowSeen(actor)}. You would require the following liquids {originalCleanableEffects.SelectNotNull(x => x.LiquidRequired).Distinct().Select(x => x.Name.ToLowerInvariant().Colour(x.DisplayColour)).ListToString()}");
			}

			return new Queue<ICleanableEffect>();
		}

		return new Queue<ICleanableEffect>(cleanableEffects);
	}

	private static void CleanTarget(ICharacter actor, IPerceivable target)
	{
		var effectQueue = GetCleanableEffectQueue(target, actor, true);
		if (!effectQueue.Any())
		{
			return;
		}

		actor.AddEffect(
			new SimpleCharacterAction(actor, CleanAction(target, actor, effectQueue, new Queue<IPerceivable>()),
				$"cleaning {target.Name}", new[] { "general", "movement" }, "cleaning an item"),
			effectQueue.Peek().BaseCleanTime);
		actor.OutputHandler.Handle(new EmoteOutput(new Emote(effectQueue.Peek().EmoteBeginClean, actor, target)));
	}

	private static void CleanAll(ICharacter actor, bool onlyMine)
	{
		var localItems = actor.Location.LayerGameItems(actor.RoomLayer)
							  .Where(x => x.Effects.Any(y => y is ICleanableEffect)).ToList();
		var personalItems = actor.Body.ExternalItems.Where(x => x.Effects.Any(y => y is ICleanableEffect)).ToList();
		var potentialItems = onlyMine ? personalItems : personalItems.Concat(localItems).ToList();
		if (!potentialItems.Any())
		{
			actor.Send(onlyMine
				? "You don't have anything that needs cleaning."
				: "You neither have nor are in the vicinity of anything that needs cleaning.");
			return;
		}

		var cleanableItems = potentialItems.Where(x => GetCleanableEffectQueue(x, actor, false).Any()).ToList();
		if (!cleanableItems.Any())
		{
			actor.Send(onlyMine
				? "You are lacking the tools or solvents to clean any of the things on your person. Try cleaning individual items for a more specific reasoning."
				: "You are lacking the tools or solvents to clean any of the things which require cleaning on your person or in your vicinity. Try cleaning individual items for a more specific reasoning.");
			return;
		}

		if (cleanableItems.Count == 1)
		{
			CleanTarget(actor, cleanableItems.Single());
			return;
		}

		var effectQueue = GetCleanableEffectQueue(cleanableItems.First(), actor, false);
		var itemQueue = new Queue<IPerceivable>(cleanableItems.Skip(1));
		actor.AddEffect(
			new SimpleCharacterAction(actor, CleanAction(cleanableItems.First(), actor, effectQueue, itemQueue),
				$"cleaning {cleanableItems.First().Name}", new[] { "general", "movement" }, "cleaning an item"),
			effectQueue.Peek().BaseCleanTime);
		actor.OutputHandler.Handle(
			new EmoteOutput(new Emote(effectQueue.Peek().EmoteBeginClean, actor, cleanableItems.First())));
	}

	#endregion Clean SubCommands
}