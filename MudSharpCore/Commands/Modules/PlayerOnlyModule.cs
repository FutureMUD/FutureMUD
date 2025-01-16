using System;
using System.Linq;
using System.Text;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System.Collections.Generic;
using MudSharp.Accounts;
using MudSharp.Character.Name;
using MudSharp.TimeAndDate;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using MudSharp.Community.Boards;

namespace MudSharp.Commands.Modules;

internal class PlayerOnlyModule : Module<ICharacter>
{
	private PlayerOnlyModule()
		: base("Player Only")
	{
		IsNecessary = true;
	}

	public static PlayerOnlyModule Instance { get; } = new();

	private const string JournalHelp =
		@"The journal command allows you to make in-character notes about the goings-on of your character. You can retrieve these journal entries at a later date, perhaps to remind you of the details of events your character might remember. Admins can also view your journal entries, which they may use to create tailored plot content for you.

The syntax for this command is as follows:

  #3journal#0 - shows you a list of all your journal entries
  #3journal read <##>#0 - reads a journal entry for your current character
  #3journal read <character> <##>#0 - reads a journal entry for a past character of yours
  #3journal history#0 - shows you all your characters who have journal entries
  #3journal history <character>#0 - shows you all the journal entries for a particular character of yours
  #3journal write <title>#0 - drops you into an editor to write a journal entry";

	private static Regex JournalReadRegex = new("(?<name>\"?\\w+\"?) (?<index>\\d+)");

	[PlayerCommand("Journal", "journal")]
	[HelpInfo("journal", JournalHelp, AutoHelp.HelpArg)]
	[CustomModuleName("Game")]
	protected static void Journal(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		using (new FMDB())
		{
			var account = FMDB.Context.Accounts.Find(actor.Account.Id);
			if (account == null)
			{
				actor.OutputHandler.Send("You are not allowed to have or keep a journal.");
				return;
			}

			if (ss.IsFinished)
			{
				// Show journal entries for current character
				var notes =
					FMDB.Context.AccountNotes
					    .Where(x => x.AccountId == account.Id && x.CharacterId == actor.Id && x.IsJournalEntry)
					    .OrderByDescending(x => x.TimeStamp)
					    .ThenBy(x => x.Id)
					    .ToList();
				var index = notes.Count;

				if (notes.Count == 0)
				{
					actor.OutputHandler.Send("You haven't made any entries in your journal.");
					return;
				}

				actor.OutputHandler.Send($"The Journal of {actor.PersonalName.GetName(NameStyle.FullName)}:\n\n" +
				                         StringUtilities.GetTextTable(
					                         from note in notes
					                         select
						                         new[]
						                         {
							                         index--.ToString("N0", actor),
							                         note.Subject.Proper(),
							                         new MudDateTime(note.InGameTimeStamp, actor.Gameworld).Date
								                         .Display(TimeAndDate.Date.CalendarDisplayMode.Short)
						                         },
					                         new[] { "Id", "Subject", "Date" },
					                         actor.Account.LineFormatLength,
					                         colour: Telnet.Green,
					                         truncatableColumnIndex: 1,
					                         unicodeTable: actor.Account.UseUnicode
				                         )
				);
				return;
			}

			switch (ss.PopSpeech().ToLowerInvariant())
			{
				case "read":
					if (ss.IsFinished)
					{
						actor.OutputHandler.Send(
							"You must either specify a numbered journal entry to read, or the name of a past character of yours whose journal you want to read.");
						return;
					}

					var character = actor.Id;
					var characterName = actor.PersonalName.GetName(NameStyle.FullName);
					int index;
					if (JournalReadRegex.IsMatch(ss.SafeRemainingArgument))
					{
						var match = JournalReadRegex.Match(ss.SafeRemainingArgument);
						var name = match.Groups["name"].Value;
						if (!int.TryParse(match.Groups["index"].Value, out index))
						{
							actor.OutputHandler.Send(
								"You must specify a valid number of the journal entry you'd like to read for that character.");
							return;
						}

						var pastCharacters = FMDB.Context.Characters
						                         .Where(x => x.AccountId == account.Id)
						                         .Distinct()
						                         .AsEnumerable()
						                         .Select(x => (Character: x,
							                         Name: new PersonalName(
									                         XElement.Parse(x.NameInfo).Element("PersonalName")
									                                 .Element("Name"),
									                         actor.Gameworld)
								                         .GetName(NameStyle.FullName)))
						                         .ToList();

						var couldBe =
							pastCharacters.FirstOrDefault(x => x.Name.EqualTo(name)).Character ??
							pastCharacters.FirstOrDefault(x =>
								x.Name.StartsWith(name, StringComparison.InvariantCultureIgnoreCase)).Character ??
							pastCharacters.FirstOrDefault(x =>
								x.Name.Contains(name, StringComparison.InvariantCultureIgnoreCase)).Character;

						if (couldBe == null)
						{
							actor.OutputHandler.Send("You have never had a character with a name like that.");
							return;
						}

						character = couldBe.Id;
						characterName =
							new PersonalName(XElement.Parse(couldBe.NameInfo).Element("PersonalName").Element("Name"),
								actor.Gameworld).GetName(NameStyle.FullName);
					}
					else
					{
						if (!int.TryParse(ss.SafeRemainingArgument, out index))
						{
							actor.OutputHandler.Send(
								"You must specify a valid number of the journal entry you'd like to read.");
							return;
						}
					}

					var notes = FMDB.Context.AccountNotes
					                .Where(x => x.AccountId == account.Id && x.CharacterId == character &&
					                            x.IsJournalEntry)
					                .OrderBy(x => x.TimeStamp)
					                .ThenBy(x => x.Id)
					                .ToList();

					if (!notes.Any())
					{
						if (character != actor.Id)
						{
							actor.OutputHandler.Send(
								$"{characterName.ColourName()} does not have any journal entries to read.");
						}
						else
						{
							actor.OutputHandler.Send("You do not have any journal entries to read.");
						}

						return;
					}

					if (index < 1 || index > notes.Count)
					{
						actor.OutputHandler.Send(
							$"You must enter an index number between {1.ToString("N0", actor).ColourValue()} and {notes.Count.ToString("N0", actor).ColourValue()}.");
						return;
					}

					var note = notes[index - 1];
					var sb = new StringBuilder();
					sb.AppendLine(
						$"The Journal of {characterName.ColourName()}, Entry #{index.ToString("N0", actor)}.");
					sb.AppendLine(
						$"Dated: {new MudDateTime(note.InGameTimeStamp, actor.Gameworld).Date.Display(TimeAndDate.Date.CalendarDisplayMode.Short).Colour(Telnet.Green)}");
					sb.AppendLine();
					sb.AppendLine(note.Subject.GetLineWithTitle(actor.InnerLineFormatLength, actor.Account.UseUnicode,
						Telnet.Yellow, Telnet.BoldWhite));
					sb.AppendLine();
					sb.AppendLine(note.Text.Wrap(actor.InnerLineFormatLength));
					actor.OutputHandler.Send(sb.ToString());
					break;
				case "write":

					if (ss.IsFinished)
					{
						actor.OutputHandler.Send("What title do you want your new journal entry to have?");
						return;
					}

					var title = ss.SafeRemainingArgument.TitleCase();
					var time = actor.Location.DateTime().GetDateTimeString();
					actor.OutputHandler.Send("Write your journal entry in the text editor below.");
					actor.EditorMode(JournalWritePost, JournalWriteCancel, 1.0,
						suppliedArguments: new object[] { account.Id, title, actor.Account.Id, time, actor.Id });
					break;
				case "history":
					if (ss.IsFinished)
					{
						var whoHasNotes = FMDB.Context.AccountNotes
						                      .Where(x => x.AccountId == account.Id && x.IsJournalEntry)
						                      .OrderByDescending(x => x.TimeStamp)
						                      .Select(x => x.Character)
						                      .Distinct()
						                      .AsEnumerable()
						                      .Select(x =>
							                      new PersonalName(
								                      XElement.Parse(x.NameInfo).Element("PersonalName")
								                              .Element("Name"),
								                      actor.Gameworld).GetName(NameStyle.FullName))
						                      .ToList();
						if (whoHasNotes.Count == 0)
						{
							actor.OutputHandler.Send(
								"You do not have any characters who have written in their journal.");
							return;
						}

						actor.OutputHandler.Send(
							$"You have journal notes on the following characters:\n{whoHasNotes.ListToLines(true)}");
						return;
					}

					var targetName = ss.SafeRemainingArgument;
					var formerCharacters = FMDB.Context.Characters
					                           .Where(x => x.AccountId == account.Id)
					                           .Distinct()
					                           .AsEnumerable()
					                           .Select(x => (Character: x,
						                           Name: new PersonalName(
							                           XElement.Parse(x.NameInfo).Element("PersonalName")
							                                   .Element("Name"),
							                           actor.Gameworld).GetName(NameStyle.FullName)))
					                           .ToList();

					var lookupCharacter =
						formerCharacters.FirstOrDefault(x => x.Name.EqualTo(targetName)).Character ??
						formerCharacters.FirstOrDefault(x =>
							x.Name.StartsWith(targetName, StringComparison.InvariantCultureIgnoreCase)).Character ??
						formerCharacters.FirstOrDefault(x =>
							x.Name.Contains(targetName, StringComparison.InvariantCultureIgnoreCase)).Character;

					if (lookupCharacter == null)
					{
						actor.OutputHandler.Send("You have never had a character with a name like that.");
						return;
					}

					character = lookupCharacter.Id;
					characterName =
						new PersonalName(
							XElement.Parse(lookupCharacter.NameInfo).Element("PersonalName").Element("Name"),
							actor.Gameworld).GetName(NameStyle.FullName);

					notes = FMDB.Context.AccountNotes
					            .Where(x => x.AccountId == account.Id && x.CharacterId == character && x.IsJournalEntry)
					            .OrderByDescending(x => x.TimeStamp)
					            .ThenBy(x => x.Id)
					            .ToList();

					if (notes.Count == 0)
					{
						actor.OutputHandler.Send($"{characterName.ColourName()} did not have any journal entries.");
						return;
					}

					index = notes.Count;
					actor.OutputHandler.Send($"The Journal of {characterName.ColourName()}:\n\n" +
					                         StringUtilities.GetTextTable(
						                         from theNote in notes
						                         select
							                         new[]
							                         {
								                         index--.ToString("N0", actor),
								                         theNote.Subject.Proper(),
								                         new MudDateTime(theNote.InGameTimeStamp, actor.Gameworld).Date
									                         .Display(TimeAndDate.Date.CalendarDisplayMode.Short)
							                         },
						                         new[] { "Id", "Subject", "Date" },
						                         actor.Account.LineFormatLength,
						                         colour: Telnet.Green,
						                         truncatableColumnIndex: 1,
						                         unicodeTable: actor.Account.UseUnicode
					                         )
					);
					return;
				default:
					actor.OutputHandler.Send(JournalHelp);
					return;
			}
		}
	}

	private static void JournalWritePost(string message, IOutputHandler handler, object[] arguments)
	{
		var title = (string)arguments[1];
		using (new FMDB())
		{
			var note = new Models.AccountNote
			{
				Text = message,
				AccountId = (long)arguments[0],
				AuthorId = (long)arguments[2],
				Subject = title,
				TimeStamp = DateTime.UtcNow,
				IsJournalEntry = true,
				InGameTimeStamp = (string)arguments[3],
				CharacterId = (long)arguments[4]
			};
			FMDB.Context.AccountNotes.Add(note);
			FMDB.Context.SaveChanges();
			handler.Send($"\nYou finish posting your journal entry entitled \"{title.ColourCommand()}\".");
		}
	}

	private static void JournalWriteCancel(IOutputHandler handler, object[] arguments)
	{
		handler.Send("You decide not to write in your journal.");
	}

	[PlayerCommand("Typo", "typo")]
	[NoCombatCommand]
	[NoMovementCommand]
	[CustomModuleName("Game")]
	[HelpInfo("typo",
		@"This command is used to report a typo in the world building, whether it be rooms, items, NPCs, echoes or whatever you have noticed that is off. Simply type #6typo#0 and you will be dropped into an editor where you can describe the typo that you saw. This command will record your current location so there is no need to include that information.",
		AutoHelp.HelpArg)]
	protected static void Typo(ICharacter actor, string command)
	{
		actor.OutputHandler.Send("Describe the typo that you have observed in the text editor below.");
		actor.EditorMode(TypoWritePost, TypoCancelPost, 1.0, suppliedArguments: new object[] { actor });
	}

	private static void TypoWritePost(string message, IOutputHandler handler, object[] arguments)
	{
		var actor = (ICharacter)arguments[0];
		var board = actor.Gameworld.Boards.Get(actor.Gameworld.GetStaticLong("TyposBoardId"));
		if (board is null)
		{
			using (new FMDB())
			{
				var dbBoard = new Models.Board
				{
					Name = "Typos",
					ShowOnLogin = true
				};
				FMDB.Context.Boards.Add(dbBoard);
				FMDB.Context.SaveChanges();
				board = new Board(dbBoard, actor.Gameworld);
				actor.Gameworld.Add(board);
				var dbitem = FMDB.Context.StaticConfigurations.Find("TyposBoardId");
				if (dbitem == null)
				{
					FMDB.Context.StaticConfigurations.Add(new Models.StaticConfiguration
						{ SettingName = "TyposBoardId", Definition = dbBoard.Id.ToString("F0") });
				}
				else
				{
					dbitem.Definition = dbBoard.Id.ToString("F0");
				}

				FMDB.Context.SaveChanges();
				actor.Gameworld.UpdateStaticConfiguration("TyposBoardId", dbBoard.Id.ToString("F0"));
			}
		}

		var sb = new StringBuilder();
		sb.AppendLine($"==Typo Report==".ColourBold(Telnet.White));
		sb.AppendLine(
			$"Location: {actor.Location.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreLayers)} ({actor.Location.Id.ToString("N0", actor).ColourValue()})");
		sb.AppendLine();
		sb.AppendLine(message);
		board.MakeNewPost(actor, "Typo Report", sb.ToString());
		handler.Send($"\n\nYour typo submission has been recorded. Thank you for helping to improve the game.");
	}

	private static void TypoCancelPost(IOutputHandler handler, object[] arguments)
	{
		handler.Send("You decide not to report a typo.");
	}

	[PlayerCommand("Roles", "roles")]
	[CustomModuleName("Game")]
	protected static void Roles(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		var sb = new StringBuilder();
		if (!actor.IsAdministrator() || ss.IsFinished)
		{
			sb.AppendLine("You took the following roles at character creation:");
			foreach (var role in actor.Roles.OrderBy(x => x.RoleType))
			{
				sb.AppendLine($"\t[{role.RoleType}] {role.Name.TitleCase()}");
			}

			actor.Send(sb.ToString());
			return;
		}

		var target = actor.TargetActor(ss.Pop());
		if (target == null)
		{
			actor.Send("You don't see anyone like that.");
			return;
		}

		sb.AppendLine($"{target.HowSeen(actor, true)} took the following roles at character creation:");
		foreach (var role in target.Roles.OrderBy(x => x.RoleType))
		{
			sb.AppendLine($"\t[{role.RoleType}] {role.Name.TitleCase()}");
		}

		actor.Send(sb.ToString());
	}

	const string AliasHelpText = @"The #3alias#0 command is used to create an alternative alias that you can go by, which you can subsequently use in other sorts of commands that require you to use a name.

The syntax is as follows:

	#3alias add <name>#0 - creates a new alias
	#3alias add culture <name>#0 - creates a new cultural alias (if you have an ethnic naming culture)
	#3alias select <name>#0 - start going by one of your aliases
	#3alias clear#0 - stop going by your alias and go by your real name instead
	#3alias remove <name>#0 - removes one of your aliases

See also the #3names#0 command and the #3introduce#0 command.";

	[PlayerCommand("Alias", "alias")]
	[CommandPermission(PermissionLevel.NPC)]
	[RequiredCharacterState(CharacterState.Conscious)]
	[CustomModuleName("Game")]
	[HelpInfo("alias", AliasHelpText, AutoHelp.HelpArgOrNoArg)]
	protected static void Alias(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		switch (ss.Pop().ToLowerInvariant())
		{
			case "add":
				AliasAdd(actor, ss);
				break;
			case "remove":
				AliasRemove(actor, ss);
				break;
			case "select":
				AliasSelect(actor, ss);
				break;
			case "clear":
				AliasClear(actor, ss);
				break;
			case "":
				Names(actor, "names");
				break;
			default:
				actor.OutputHandler.Send(AliasHelpText.SubstituteANSIColour());
				return;
		}
	}

	[PlayerCommand("Names", "names")]
	[RequiredCharacterState(CharacterState.Conscious)]
	[CustomModuleName("Game")]
	[HelpInfo("names", @"The #3names#0 command is used to view your real name and any aliases. See also the #3alias#0 command and the #3introduce#0 command.", AutoHelp.HelpArg)]
	protected static void Names(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		var sb = new StringBuilder();
		if (!actor.IsAdministrator() || ss.IsFinished)
		{
			sb.Append(
				$"Your name is {actor.PersonalName.GetName(NameStyle.FullName).Colour(Telnet.Cyan)}{(!actor.PersonalName.GetName(NameStyle.SimpleFull).Equals(actor.PersonalName.GetName(NameStyle.FullWithNickname), StringComparison.InvariantCultureIgnoreCase) ? ", a.k.a. " + actor.PersonalName.GetName(NameStyle.FullWithNickname).Colour(Telnet.Cyan) : "")}. ");
			if (!actor.Aliases.Any())
			{
				sb.AppendLine("You do not have any aliases.");
			}
			else
			{
				sb.AppendLine("You have the following aliases:\n");
				foreach (var item in actor.Aliases)
				{
					sb.AppendLine(
						$"\t{item.GetName(NameStyle.FullName).Colour(Telnet.Cyan)}{(!item.GetName(NameStyle.SimpleFull).Equals(item.GetName(NameStyle.FullWithNickname), StringComparison.InvariantCultureIgnoreCase) ? ", a.k.a. " + item.GetName(NameStyle.FullWithNickname).Colour(Telnet.Cyan) : "")}");
				}
			}
		}
		else
		{
			var targetText = ss.PopSpeech();
			var target = actor.TargetActor(targetText) ?? actor.Gameworld.Characters.GetByName(targetText);
			if (target == null)
			{
				actor.OutputHandler.Send("There is no such person for whom you can retrieve their names.");
				return;
			}

			sb.Append(
				$"{target.HowSeen(actor, true, DescriptionType.Possessive)} name is {target.PersonalName.GetName(NameStyle.FullName).Colour(Telnet.Cyan)}{(!target.PersonalName.GetName(NameStyle.SimpleFull).Equals(target.PersonalName.GetName(NameStyle.FullWithNickname), StringComparison.InvariantCultureIgnoreCase) ? ", a.k.a. " + target.PersonalName.GetName(NameStyle.FullWithNickname).Colour(Telnet.Cyan) : "")}. ");
			if (!target.Aliases.Any())
			{
				sb.AppendLine(
					$"{target.ApparentGender(actor).Subjective(true)} {(target.IsSelf(actor) ? "do" : "does")} not have any aliases.");
			}
			else
			{
				sb.AppendLine(
					$"{target.ApparentGender(actor).Subjective(true)} {(target.IsSelf(actor) ? "have" : "has")} the following aliases:");
				foreach (var item in target.Aliases)
				{
					sb.AppendLine(
						$"\t{item.GetName(NameStyle.FullName).Colour(Telnet.Cyan)}{(!item.GetName(NameStyle.SimpleFull).Equals(item.GetName(NameStyle.FullWithNickname), StringComparison.InvariantCultureIgnoreCase) ? ", a.k.a. " + item.GetName(NameStyle.FullWithNickname).Colour(Telnet.Cyan) : "")}");
				}
			}
		}

		actor.OutputHandler.Send(sb.ToString(), false);
	}

	[PlayerCommand("Introduce", "introduce")]
	[RequiredCharacterState(CharacterState.Conscious)]
	[HelpInfo("introduce",
		@"The introduce command allows you to let others know your name, and also creates a ""dub"" (a keyword alias) for you with the people to whom you introduce yourself.

You can introduce yourself or others. To introduce others, you need to have been introduced to them yourself, or have set up a dub for them (see HELP DUB) and added a name for them (see HELP DUBNAME). You will always introduce yourself by your current name (the name which you have NAME SELECT <which>).

The syntax is #3INTRODUCE ME#0 or #3INTRODUCE <person>#0. You can append a bracketed emote to this command.",
		AutoHelp.HelpArgOrNoArg)]
	[CustomModuleName("Game")]
	protected static void Introduce(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		var target = actor.TargetActorOrCorpse(ss.PopSafe());
		if (target == null)
		{
			actor.OutputHandler.Send("You don't see anyone like that.");
			return;
		}

		PlayerEmote emote = null;
		var text = ss.PopParentheses();
		if (!string.IsNullOrWhiteSpace(text))
		{
			emote = new PlayerEmote(text, actor);
			if (!emote.Valid)
			{
				actor.Send(emote.ErrorMessage);
				return;
			}
		}

		string targetName, firstName;
		if (target == actor)
		{
			targetName = actor.CurrentName.GetName(NameStyle.FullName);
			firstName = actor.CurrentName.GetName(NameStyle.GivenOnly).ToLowerInvariant();
		}
		else
		{
			var actorDub =
				actor.Dubs.FirstOrDefault(x => x.TargetId == target.Id && x.TargetType == target.FrameworkItemType);
			if (actorDub == null)
			{
				actor.OutputHandler.Send("You can only introduce people that you have dubbed and given a name.");
				return;
			}

			if (string.IsNullOrEmpty(actorDub.IntroducedName))
			{
				actor.OutputHandler.Send(
					$"Although you have a dub for {target.HowSeen(actor)}, you do not have a name set. They must either introduce themselves to you first or you can use DUBNAME <target> <name> to set it manually.");
				return;
			}

			targetName = actorDub.IntroducedName;
			firstName = new StringStack(targetName).Pop().ToLowerInvariant();
		}

		actor.OutputHandler.Handle(
			new MixedEmoteOutput(
				new Emote($"@ introduce|introduces $1=0 as {targetName.ColourName()}.", actor, actor, target),
				flags: OutputFlags.PurelyAudible).Append(emote));
		using (new FMDB())
		{
			var addedDubs = new List<(ICharacter owner, MudSharp.Models.Dub newDub)>();
			foreach (var tch in actor.Location.LayerCharacters(actor.RoomLayer))
			{
				if (tch == actor || !tch.CanHear(actor))
				{
					continue;
				}

				var dub = tch.Dubs.FirstOrDefault(x =>
					x.TargetId == target.Id && x.TargetType == target.FrameworkItemType);
				if (dub != null)
				{
					if (string.IsNullOrEmpty(dub.IntroducedName))
					{
						dub.IntroducedName = targetName;
					}

					continue;
				}

				var dbdub = new Models.Dub();
				FMDB.Context.Dubs.Add(dbdub);
				dbdub.CharacterId = tch.Id;
				dbdub.LastDescription = target.HowSeen(tch, colour: false);
				dbdub.LastUsage = DateTime.UtcNow;
				dbdub.Keywords = firstName;
				dbdub.TargetId = target.Id;
				dbdub.TargetType = target.FrameworkItemType;
				dbdub.IntroducedName = targetName;
				addedDubs.Add((tch, dbdub));
			}

			FMDB.Context.SaveChanges();
			foreach (var (tch, dub) in addedDubs)
			{
				tch.Dubs.Add(new Dub(dub, tch, actor.Gameworld));
			}
		}
	}

	[PlayerCommand("Social", "social")]
	[CustomModuleName("Game")]
	protected static void Social(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which social do you want to view?");
			return;
		}

		var socialText = ss.Pop();
		var social = actor.CommandTree.Commands.Socials.FirstOrDefault(x => x.Applies(actor, socialText, false));
		if (social == null)
		{
			actor.OutputHandler.Send("There is no such social for you to view.");
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine($"{social.Name.ToUpper()}".Colour(Telnet.Cyan));
		sb.AppendLine();
		sb.AppendLine($"No Target Echo: {social.NoTargetEcho}");
		sb.AppendLine($"One Target Echo: {social.OneTargetEcho}");
		if (!string.IsNullOrEmpty(social.MultiTargetEcho))
		{
			sb.AppendLine($"Multi Target Echo: {social.MultiTargetEcho}");
		}

		if (!string.IsNullOrEmpty(social.DirectionTargetEcho))
		{
			sb.AppendLine($"Direction Echo: {social.DirectionTargetEcho}");
		}

		if (actor.IsAdministrator())
		{
			sb.AppendLine(
				$"Applicability Prog: {(social.ApplicabilityProg != null ? string.Format(actor, "{0} (#{1:N0})", social.ApplicabilityProg.FunctionName.FluentTagMXP("send", $"href='show futureprog {social.ApplicabilityProg.Id}'"), social.ApplicabilityProg.Id) : "None")}");
		}

		actor.OutputHandler.Send(sb.ToString(), false);
	}

	[PlayerCommand("Socials", "socials")]
	[CustomModuleName("Game")]
	protected static void Socials(ICharacter actor, string command)
	{
		if (!actor.CommandTree.Commands.Socials.Any())
		{
			actor.OutputHandler.Send("You do not know any socials.");
			return;
		}

		actor.OutputHandler.Send("You know the following socials:");
		actor.OutputHandler.Send(
			StringUtilities.GetTextTable(
				from social in actor.CommandTree.Commands.Socials
				where
					social.ApplicabilityProg?.ExecuteBool(actor) ?? true
				orderby social.Name
				select new[]
				{
					social.Name,
					string.IsNullOrEmpty(social.NoTargetEcho) ? "No" : "Yes",
					string.IsNullOrEmpty(social.OneTargetEcho) ? "No" : "Yes",
					string.IsNullOrEmpty(social.MultiTargetEcho) ? "No" : "Yes",
					string.IsNullOrEmpty(social.DirectionTargetEcho) ? "No" : "Yes"
				},
				new[] { "Name", "No Target", "One Target", "Multi Target", "Direction" },
				actor.Account.LineFormatLength,
				colour: Telnet.Green
			)
		);
	}

	[PlayerCommand("Save", "save")]
	[CustomModuleName("Game")]
	protected static void Save(ICharacter actor, string command)
	{
		actor.Send("Saving your character...");
		actor.Send("Done.");
	}

	#region Alias Sub-Commands

	private static void AliasAdd(ICharacter actor, StringStack command)
	{
		var overrideEthnicNameCulture = false;
		if (command.PeekSpeech().EqualTo("culture"))
		{
			overrideEthnicNameCulture = true;
			command.PopSpeech();
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What other name do you want to go by?");
			return;
		}

		var limit = Character.Character.MaximumNumberOfAliases(actor);
		if (limit > 0 && actor.Aliases.Count >= limit)
		{
			actor.OutputHandler.Send($"You cannot add any more aliases, as you may have no more than {limit.ToStringN0Colour(actor)}.");
			return;
		}

		var nameText = command.SafeRemainingArgument.Trim();
		var nc =
			overrideEthnicNameCulture ? 
				actor.Culture.NameCultureForGender(actor.Gender.Enum) : 
				actor.NameCultureForGender(actor.Gender.Enum);

		if (nc is null)
		{
			actor.OutputHandler.Send("You don't have a valid naming culture for that option.");
			return;
		}

		var newName = nc.GetPersonalName(nameText, true);
		if (newName == null)
		{
			var exampleRN = actor.Gameworld.RandomNameProfiles.Where(x => x.Culture == nc && x.IsCompatibleGender(actor.Gender.Enum) && x.IsReady).GetRandomElement();
			var sb = new StringBuilder();
			sb.AppendLine($"That is not a valid name for your name culture ({nc.Name.ColourValue()}).");
			if (exampleRN is not null)
			{
				sb.AppendLine($"Some examples of valid names for that culture are as follows:\n");
				for (var i = 0; i < 4; i++)
				{
					sb.AppendLine($"\t{exampleRN.GetRandomPersonalName(true).GetName(NameStyle.FullName).ColourName()}");
				}
			}
			actor.OutputHandler.Send(sb.ToString());
			return;
		}

		var newNameText = newName.GetName(NameStyle.FullName);
		if (
			(actor.PersonalName.GetName(NameStyle.FullName)
			     .Equals(newNameText, StringComparison.InvariantCultureIgnoreCase) && actor.PersonalName.Culture == nc) ||
			actor.Aliases.Any(
				x => x.Culture == nc && x.GetName(NameStyle.FullName).Equals(newNameText, StringComparison.InvariantCultureIgnoreCase)))
		{
			actor.OutputHandler.Send("Your alias is too similiar to your real name or one of your existing aliases.");
			return;
		}

		actor.Aliases.Add(newName);
		actor.NamesChanged = true;

		actor.OutputHandler.Send($"You will now go by the alias of {newName.GetName(NameStyle.FullName).Colour(Telnet.Green)}{(!newName.GetName(NameStyle.SimpleFull).Equals(newName.GetName(NameStyle.FullWithNickname), StringComparison.InvariantCultureIgnoreCase) ? ", a.k.a. " + newName.GetName(NameStyle.FullWithNickname).Colour(Telnet.Green) : "")}.");
	}

	private static void AliasRemove(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which alias do you want to remove?");
			return;
		}

		var nameText = command.SafeRemainingArgument;
		var alias = actor.Aliases.FirstOrDefault(x => x.GetName(NameStyle.FullName).EqualTo(nameText));
		if (alias is null)
		{
			actor.OutputHandler.Send($"You don't have any alias like that. See the {"names".MXPSend()} command.");
			return;
		}

		actor.Aliases.Remove(alias);
		if (actor.CurrentName == alias)
		{
			actor.CurrentName = actor.PersonalName;
		}

		actor.NamesChanged = true;
		actor.OutputHandler.Send($"You will no longer go by the alias {alias.GetName(NameStyle.FullName).ColourName()}.");
	}

	private static void AliasSelect(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which of your aliases do you wish to select as the one you are currently using?");
			return;
		}

		var aliasText = command.SafeRemainingArgument;
		var alias = actor.Aliases.GetName(aliasText);
		if (alias == null)
		{
			actor.OutputHandler.Send("You do not have any such alias.");
			return;
		}

		actor.CurrentName = alias;
		actor.NamesChanged = true;
		actor.OutputHandler.Send(
			$"You will now go by the alias {alias.GetName(NameStyle.FullName).Colour(Telnet.Green)}{(!alias.GetName(NameStyle.SimpleFull).Equals(alias.GetName(NameStyle.FullWithNickname), StringComparison.InvariantCultureIgnoreCase) ? ", a.k.a. " + alias.GetName(NameStyle.FullWithNickname).Colour(Telnet.Green) : "")}.");
	}

	private static void AliasClear(ICharacter actor, StringStack command)
	{
		if (actor.CurrentName == actor.PersonalName)
		{
			actor.OutputHandler.Send("You are not currently going by an alias.");
			return;
		}

		actor.CurrentName = actor.PersonalName;
		actor.NamesChanged = true;
		actor.OutputHandler.Send("You will now go by your real name.");
	}

	#endregion Alias Sub-Commands
}