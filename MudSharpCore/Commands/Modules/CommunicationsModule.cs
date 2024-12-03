using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Communication.Language;
using MudSharp.Community.Boards;
using MudSharp.Database;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;

namespace MudSharp.Commands.Modules;

internal class CommunicationsModule : Module<ICharacter>
{
	private CommunicationsModule()
		: base("Communications")
	{
		IsNecessary = true;
	}

	public static CommunicationsModule Instance { get; } = new();

	public override int CommandsDisplayOrder => 6;

	[PlayerCommand("Think", "think")]
	[RequiredCharacterState(CharacterState.SleepingOrBetter)]
	protected static void Think(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());

		var emote = new PlayerEmote(ss.PopParentheses(), actor);
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("What is it that you want to think?");
			return;
		}

		if (ss.RemainingArgument.Length > actor.Gameworld.GetStaticInt("MaximumSayLength"))
		{
			actor.Send("You could not possibly think so much at once.");
			return;
		}

		var thinkText = ss.RemainingArgument.Sanitise().ProperSentences().NormaliseSpacing().Fullstop();
		actor.Send(
			$"You think{(!string.IsNullOrWhiteSpace(emote.RawText) ? $", {emote.ParseFor(actor)}, " : "")}\n\t\"{thinkText}\"");
		foreach (
			var character in
			actor.Gameworld.Characters.Where(
				x => x != actor && x.EffectsOfType<ITelepathyEffect>().Any(y => y.Applies(actor) && y.ShowThinks))
		)
		{
			var effects =
				character.EffectsOfType<ITelepathyEffect>().Where(x => x.Applies(actor) && x.ShowThinks).ToList();
			var showDesc = effects.Any(x => x.ShowDescription(actor));
			var showName = effects.Any(x => x.ShowName(actor));
			var showEmote = effects.Any(x => x.ShowThinkEmote(actor));

			character.Send(
				$"{(showDesc ? actor.HowSeen(character, true) : "Someone")} {(showName ? $"({actor.PersonalName.GetName(NameStyle.SimpleFull)}) " : "")}thinks{(showEmote && !string.IsNullOrWhiteSpace(emote.RawText) ? $", {emote.ParseFor(character)}, " : ",")}\n\t\"{thinkText}\"");
		}
	}

	[PlayerCommand("Feel", "feel")]
	[RequiredCharacterState(CharacterState.SleepingOrBetter)]
	protected static void Feel(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());

		var emote = new PlayerEmote(ss.PopParentheses(), actor);
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("What is it that you want to feel?");
			return;
		}

		if (ss.RemainingArgument.Length > actor.Gameworld.GetStaticInt("MaximumSayLength"))
		{
			actor.Send("You could not possibly feel so much at once.");
			return;
		}

		var feelEmote = new PlayerEmote(ss.RemainingArgument, actor);
		if (!feelEmote.Valid)
		{
			actor.OutputHandler.Send(feelEmote.ErrorMessage);
		}

		actor.OutputHandler.Send(
			$"You feel{(!string.IsNullOrWhiteSpace(emote.RawText) ? $", {emote.ParseFor(actor)}, " : " ")}{feelEmote.ParseFor(actor)}");
		foreach (
			var character in
			actor.Gameworld.Characters.Where(
				x => x != actor && x.EffectsOfType<ITelepathyEffect>().Any(y => y.Applies(actor) && y.ShowFeels)))
		{
			var effects =
				character.EffectsOfType<ITelepathyEffect>().Where(x => x.Applies(actor) && x.ShowFeels).ToList();
			var showDesc = effects.Any(x => x.ShowDescription(actor));
			var showName = effects.Any(x => x.ShowName(actor));
			var showEmote = effects.Any(x => x.ShowThinkEmote(actor));

			character.OutputHandler.Send(
				$"{(showDesc ? actor.HowSeen(character, true) : "Someone")} {(showName ? $"({actor.PersonalName.GetName(NameStyle.SimpleFull)}) " : "")}feels{(showEmote && !string.IsNullOrWhiteSpace(emote.RawText) ? $", {emote.ParseFor(character)}, " : " ")}{feelEmote.ParseFor(character)}");
		}
	}

	[PlayerCommand("Languages", "languages")]
	[HelpInfo("languages",
		"The languages command allows you to view which languages you know and how well you know them. The syntax is simply LANGUAGES.",
		AutoHelp.HelpArg)]
	protected static void Languages(ICharacter actor, string input)
	{
		var sb = new StringBuilder();
		sb.AppendLine("You know the following languages:");
		sb.AppendLine();
		foreach (var lang in actor.Languages)
		{
			sb.AppendLine(lang.Name.Proper() + " @ " + actor.Body.GetTraitDecorated(lang.LinkedTrait));
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	[PlayerCommand("PreferAccent", "preferaccent")]
	protected static void PreferAccent(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished || ss.Peek().EqualToAny("help", "?"))
		{
			actor.Send(
				$"This command allows you to set a preferred accent for a particular language. The syntax is {"preferaccent <accent> [<language>]".Colour(Telnet.Yellow)}. You only need to specify the language if you have two accents with the same name for different languages.");
			return;
		}

		var accentText = ss.PopSpeech();
		var languageText = ss.PopSpeech();

		var accent = actor.Accents.FirstOrDefault(x => x.Name.EqualTo(accentText) &&
																									 (string.IsNullOrEmpty(languageText) ||
																										x.Language.Name.EqualTo(languageText)));
		if (accent == null)
		{
			actor.Send("You have no accent like that to set as a preferred accent.");
			return;
		}

		actor.Send(
			$"You will now prefer to use the {accent.Name.Colour(Telnet.Green)} accent when speaking in the {accent.Language.Name.Colour(Telnet.Green)} language.");
		actor.SetPreferredAccent(accent);
	}

	[PlayerCommand("Accents", "accents")]
	protected static void Accents(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var cmd = ss.PopSpeech().ToLowerInvariant();
		IEnumerable<IAccent> accents;
		if (string.IsNullOrEmpty(cmd))
		{
			accents = actor.Body.Accents;
		}
		else
		{
			var lang = actor.Body.Languages.FirstOrDefault(x =>
				x.Name.ToLowerInvariant().StartsWith(cmd, StringComparison.Ordinal));
			if (lang == null)
			{
				actor.OutputHandler.Send("You do not know such a language for which to list accents.");
				return;
			}

			accents = actor.Body.Accents.Where(x => lang.Accents.Contains(x));
		}

		var sb = new StringBuilder();
		sb.AppendLine("You know the following accents:");
		sb.AppendLine();
		foreach (var language in accents.GroupBy(x => x.Language))
		{
			var preferred = actor.PreferredAccent(language.Key);
			sb.AppendLine(language.Key.Name.Proper().Colour(Telnet.Yellow));
			foreach (var accent in language.OrderBy(x => actor.Body.AccentDifficulty(x, false)).ThenBy(x => x.Name))
			{
				sb.AppendLine(
					$"{$"{accent.Name.Proper()} \"{accent.AccentSuffix}\"".FluentColour(Telnet.BoldWhite, preferred == accent)} {actor.Body.AccentDifficulty(accent, false).Describe().Colour(Telnet.Green).Parentheses()}{(preferred == accent ? " [Preferred]" : "")}");
			}

			sb.AppendLine();
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	[PlayerCommand("Speak", "speak")]
	[HelpInfo("speak", @"The #3speak#0 command allows you to select which language and which accent or dialect you will use when you use commands that make you speak. You can also use it to view what language and accent you are currently using.

The syntax is as follows:

	#3speak#0 - see what accents you are currently speaking
	#3speak <language>#0 - start speaking a language with your preferred or otherwise best known accent
	#3speak <language> <accent>#0 - start speaking a language with a specific accent", AutoHelp.HelpArg)]
	protected static void Speak(ICharacter actor, string input)
	{
		if (!actor.Languages.Any())
		{
			actor.OutputHandler.Send("You don't know how to speak any languages.");
			return;
		}

		var ss = new StringStack(input.RemoveFirstWord());
		var lang = ss.PopSpeech();
		if (lang.Length == 0)
		{
			if (actor.CurrentLanguage is null)
			{
				actor.OutputHandler.Send("You are not currently speaking any languages.");
				return;
			}

			if (actor.CurrentAccent is null)
			{
				actor.CurrentAccent = actor.Accents.Where(x => x.Language == actor.CurrentLanguage).FirstMin(x => actor.AccentDifficulty(x, false));
				if (actor.CurrentAccent is null)
				{
					actor.LearnAccent(actor.CurrentLanguage.DefaultLearnerAccent, Difficulty.Automatic);
					actor.CurrentAccent = actor.CurrentLanguage.DefaultLearnerAccent;
				}
			}
			actor.OutputHandler.Send($"You are currently speaking {actor.CurrentLanguage.Name.Proper().ColourValue()} {actor.CurrentAccent.AccentSuffix}.");
			return;
		}

		var language =
			actor.Languages.FirstOrDefault(
				x => x.Name.StartsWith(lang, StringComparison.InvariantCultureIgnoreCase));
		if (language == null)
		{
			actor.OutputHandler.Send("You do not know that language.");
			return;
		}

		IAccent accent = null;
		var taccent = ss.PopSpeech();
		if (!string.IsNullOrEmpty(taccent))
		{
			accent =
				actor.Accents.Where(x => language.Accents.Contains(x))
						 .FirstOrDefault(x => x.Name.StartsWith(taccent, StringComparison.InvariantCultureIgnoreCase));
		}
		else
		{
			accent = actor.PreferredAccent(language) ?? actor.Accents.Where(x => language.Accents.Contains(x))
																											 .FirstMin(x => actor.AccentDifficulty(x, false));
		}

		if (accent == null)
		{
			actor.OutputHandler.Send($"You do not know that accent of {language.Name.Proper().ColourValue()}.");
			return;
		}

		if (actor.AccentDifficulty(accent, false) > Difficulty.Easy && !actor.IsAdministrator() &&
				actor.Accents.Count(x => x.Language == language) > 1)
		{
			actor.OutputHandler.Send($"You do not have sufficient command over {accent.Description.ColourValue()} to speak it.");
			return;
		}

		actor.CurrentLanguage = language;
		actor.CurrentAccent = accent;
		actor.OutputHandler.Send($"You will now speak in {language.Name.Proper().ColourValue()} {accent.AccentSuffix}.");
	}

	[PlayerCommand("Semote", "semote")]
	protected static void Semote(ICharacter actor, string input)
	{
		if (string.IsNullOrWhiteSpace(input.RemoveFirstWord()))
		{
			actor.OutputHandler.Send("What do you want to silently emote?");
			return;
		}

		actor.Body.Emote(input.RemoveFirstWord(), CharacterState.Able.HasFlag(actor.State),
			OutputFlags.SuppressObscured);
	}

	[PlayerCommand("Hemote", "hemote")]
	protected static void Hemote(ICharacter actor, string input)
	{
		var message = input.RemoveFirstWord();
		if (string.IsNullOrWhiteSpace(message))
		{
			actor.OutputHandler.Send("What do you want to hidden emote?");
			return;
		}

		if (message.Length > actor.Gameworld.GetStaticInt("MaximumEmoteLength"))
		{
			actor.Send($"That is far too much to emote at any one time. Keep it under {actor.Gameworld.GetStaticInt("MaximumEmoteLength").ToString("N0", actor).ColourValue()} characters.");
			return;
		}

		actor.Body.Emote(message, CharacterState.Conscious.HasFlag(actor.State),
			OutputFlags.NoticeCheckRequired);
	}

	[PlayerCommand("Emote", "emote", "em", "emo", "emot", ",")]
	[HelpInfo("emote", @"The #3emote#0 command is used to send an in-character echo to others in your room describing your character's actions. It is one of the main roleplaying tools that you will use and so it is worth understanding what you can do with it.

Many other parts of the engine also accept an emote, such as the communication commands like #3say#0. The emotes in those commands use the same markup syntax as described in this command.

The syntax for using the emote command is #3emote <message>#0. Note that if you do not use the @ token at some point in your emote, it will be prepended to the beginning of your emote automatically. For example, #3emote ducks down#0 is equivalent to #3emote @ ducks down#0.

You can use the following tokens in your emotes:

	#6~target#0 will substitute the short description of the target person or ""you"" to them
	#6~keyword's#0 - targets a person and displays their short description + 's or ""your"" to them
	#6~#keyword#0 - targets a person and displays ""he/she/it/they"" or ""you""
	#6~!keyword#0 - targets a person and displays ""him/her/it/they"" or ""you""
	#6~!keyword#0 - targets a person and displays ""his/her/its/their"" or ""your""
	#6~keyword|one|two#0 - displays #3one#0 to the target or #3two#0 to others, use for grammar
	#6@#0 - equivalent to using ~ and targeting yourself, however it cannot be modified with ! or #.
	#6*target#0 will substitute the short description of the target item

You can also include speech in your emotes by putting the text in double quotes (#6""#0). This will use whatever language and accent you have set using the #3speak#0 command.

Some examples are below:

	#3emote brushes the hair out of her face with a soft sigh#0
	#3emote stares angrily at ~brute, his fury palpable#0
	#3emote Without so much as a single word, @ turns around and leaves the room#0

If you care about getting grammatically correct echoes to yourself (for log purposes etc), consider the difference between the below two examples:

	#3emote smacks his hand with *hammer and says ""Ouch!""
	#3emote ~me|smack|smacks ~!me hand with *hammer and ~me|say|says ""Ouch!""", AutoHelp.HelpArgOrNoArg)]
	protected static void Emote(ICharacter actor, string input)
	{
		var message = input.RemoveFirstWord();
		if (string.IsNullOrWhiteSpace(message))
		{
			actor.OutputHandler.Send("What do you want to emote?");
			return;
		}

		if (message.Length > actor.Gameworld.GetStaticInt("MaximumEmoteLength"))
		{
			actor.Send($"That is far too much to emote at any one time. Keep it under {actor.Gameworld.GetStaticInt("MaximumEmoteLength").ToString("N0", actor).ColourValue()} characters.");
			return;
		}

		actor.Body.Emote(message, CharacterState.Conscious.HasFlag(actor.State));
	}

	[PlayerCommand("Say", "say", ".")]
	[RequiredCharacterState(CharacterState.Conscious)]
	protected static void Say(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());

		var emote = new PlayerEmote(ss.PopParentheses(), actor);

		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to say?");
			return;
		}

		var message = ss.RemainingArgument;
		if (message.Length > actor.Gameworld.GetStaticInt("MaximumSayLength"))
		{
			actor.Send($"That is far too much to say at any one time. Keep it under {actor.Gameworld.GetStaticInt("MaximumSayLength").ToString("N0", actor).ColourValue()} characters.");
			return;
		}

		actor.Body.Say(null, message, emote);
	}

	[PlayerCommand("Sing", "sing")]
	[RequiredCharacterState(CharacterState.Conscious)]
	protected static void Sing(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());

		var emote = new PlayerEmote(ss.PopParentheses(), actor);

		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to sing?");
			return;
		}

		var message = ss.RemainingArgument;
		if (message.Length > actor.Gameworld.GetStaticInt("MaximumSayLength"))
		{
			actor.Send($"That is far too much to sing at any one time. Keep it under {actor.Gameworld.GetStaticInt("MaximumSayLength").ToString("N0", actor).ColourValue()} characters.");
			return;
		}

		var splitMessage =
			message.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
						 .Select(x => x.Trim())
						 .Where(x => !string.IsNullOrWhiteSpace(x))
						 .ToList();
		if (!splitMessage.Any())
		{
			actor.OutputHandler.Send("What do you want to sing?");
			return;
		}

		message = splitMessage.Select(x => x.ProperSentences())
													.ListToString(separator: "\n ", conjunction: "", twoItemJoiner: "\n ");

		actor.Body.Sing(null, message, emote);
	}

	[PlayerCommand("SingTo", "singto")]
	[RequiredCharacterState(CharacterState.Conscious)]
	protected static void SingTo(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var target = ss.Pop();
		var ptarget = actor.Target(target);
		if (ptarget == null)
		{
			actor.OutputHandler.Send("You cannot see them to sing to.");
			return;
		}

		var emote = new PlayerEmote(ss.PopParentheses(), actor);

		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to sing?");
			return;
		}

		var message = ss.RemainingArgument;
		if (message.Length > actor.Gameworld.GetStaticInt("MaximumSayLength"))
		{
			actor.Send($"That is far too much to sing at any one time. Keep it under {actor.Gameworld.GetStaticInt("MaximumSayLength").ToString("N0", actor).ColourValue()} characters.");
			return;
		}

		var splitMessage =
			message.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
						 .Select(x => x.Trim())
						 .Where(x => !string.IsNullOrWhiteSpace(x))
						 .ToList();
		if (!splitMessage.Any())
		{
			actor.OutputHandler.Send("What do you want to sing?");
			return;
		}

		message = splitMessage.Select(x => x.ProperSentences())
													.ListToString(separator: "\n ", conjunction: "", twoItemJoiner: "\n ");
		actor.Body.Sing(ptarget, message, emote);
	}

	[PlayerCommand("Talk", "talk", "talkto")]
	[DisplayOptions(CommandDisplayOptions.DisplayCommandWords)]
	[RequiredCharacterState(CharacterState.SleepingOrBetter)]
	protected static void Talk(ICharacter actor, string input)
	{
		var ss = new StringStack(input);
		var argument = ss.Pop();

		IPerceivable target = null;
		if (argument.Equals("talkto", StringComparison.InvariantCultureIgnoreCase))
		{
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("To whom do you want to talk?");
				return;
			}

			target = actor.Target(ss.Pop());
			if (target == null)
			{
				actor.OutputHandler.Send("You do not see anyone or anything like that to talk to.");
				return;
			}
		}

		var emote = new PlayerEmote(ss.PopParentheses(), actor);

		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to say?");
			return;
		}

		var message = ss.RemainingArgument;
		if (message.Length > actor.Gameworld.GetStaticInt("MaximumSayLength"))
		{
			actor.Send($"That is far too much to say at any one time. Keep it under {actor.Gameworld.GetStaticInt("MaximumSayLength").ToString("N0", actor).ColourValue()} characters.");
			return;
		}

		actor.Body.Talk(target, message, emote);
	}

	[PlayerCommand("Transmit", "transmit", "transmitwith")]
	[DisplayOptions(CommandDisplayOptions.DisplayCommandWords)]
	[RequiredCharacterState(CharacterState.Able)]
	protected static void Transmit(ICharacter actor, string input)
	{
		var ss = new StringStack(input);
		var argument = ss.Pop();

		IGameItem target = null;
		if (argument.Equals("transmitwith", StringComparison.InvariantCultureIgnoreCase))
		{
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("What do you want to transmit with?");
				return;
			}

			target = actor.TargetItem(ss.Pop());
			if (target == null)
			{
				actor.OutputHandler.Send("You do not see anything like that to transmit with.");
				return;
			}

			if (!(target.GetItemType<ITransmit>()?.ManualTransmit ?? true))
			{
				actor.OutputHandler.Send("That is not something that you can transmit with.");
				return;
			}

			var (truth, error) = actor.CanManipulateItem(target);
			if (!truth)
			{
				actor.OutputHandler.Send(error);
				return;
			}
		}

		var emote = new PlayerEmote(ss.PopParentheses(), actor);

		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to say?");
			return;
		}

		var message = ss.RemainingArgument;
		if (message.Length > actor.Gameworld.GetStaticInt("MaximumSayLength"))
		{
			actor.Send($"That is far too much to say at any one time. Keep it under {actor.Gameworld.GetStaticInt("MaximumSayLength").ToString("N0", actor).ColourValue()} characters.");
			return;
		}

		actor.Body.Transmit(target, message, emote);
	}

	[PlayerCommand("Tell", "tell", "sayto")]
	[DisplayOptions(CommandDisplayOptions.DisplayCommandWords)]
	[RequiredCharacterState(CharacterState.Conscious)]
	protected static void Tell(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var target = ss.Pop();

		var emote = new PlayerEmote(ss.PopParentheses(), actor);

		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to tell them?");
			return;
		}

		var message = ss.RemainingArgument;
		if (message.Length > actor.Gameworld.GetStaticInt("MaximumSayLength"))
		{
			actor.Send($"That is far too much to say at any one time. Keep it under {actor.Gameworld.GetStaticInt("MaximumSayLength").ToString("N0", actor).ColourValue()} characters.");
			return;
		}

		var ptarget = actor.Target(target);
		if (ptarget == null)
		{
			actor.OutputHandler.Send("You cannot see them to tell.");
			return;
		}

		actor.Body.Say(ptarget, message, emote);
	}

	[PlayerCommand("Shout", "shout", "shoutat")]
	[DisplayOptions(CommandDisplayOptions.DisplayCommandWords)]
	[RequiredCharacterState(CharacterState.Conscious)]
	protected static void Shout(ICharacter actor, string input)
	{
		var ss = new StringStack(input);
		var argument = ss.Pop();

		IPerceivable target = null;
		if (argument.Equals("shoutat", StringComparison.InvariantCultureIgnoreCase))
		{
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("At whom do you want to shout?");
				return;
			}

			target = actor.Target(ss.Pop());
			if (target == null)
			{
				actor.OutputHandler.Send("You do not see anyone or anything like that to shout at.");
				return;
			}
		}

		var emote = new PlayerEmote(ss.PopParentheses(), actor);

		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to shout?");
			return;
		}

		var message = ss.RemainingArgument;
		if (message.Length > actor.Gameworld.GetStaticInt("MaximumSayLength"))
		{
			actor.Send($"That is far too much to say at any one time. Keep it under {actor.Gameworld.GetStaticInt("MaximumSayLength").ToString("N0", actor).ColourValue()} characters.");
			return;
		}

		actor.Body.Shout(target, message, emote);
	}

	[PlayerCommand("LoudSay", "loudsay", "loudtell")]
	[DisplayOptions(CommandDisplayOptions.DisplayCommandWords)]
	[RequiredCharacterState(CharacterState.Conscious)]
	protected static void LoudSay(ICharacter actor, string input)
	{
		var ss = new StringStack(input);
		var argument = ss.Pop();

		IPerceivable target = null;
		if (argument.Equals("loudtell", StringComparison.InvariantCultureIgnoreCase))
		{
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("Who do you want to tell loudly?");
				return;
			}

			target = actor.Target(ss.Pop());
			if (target == null)
			{
				actor.OutputHandler.Send("You do not see anyone or anything like that to tell loudly.");
				return;
			}
		}

		var emote = new PlayerEmote(ss.PopParentheses(), actor);

		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to say loudly?");
			return;
		}

		var message = ss.RemainingArgument;
		if (message.Length > actor.Gameworld.GetStaticInt("MaximumSayLength"))
		{
			actor.Send($"That is far too much to say at any one time. Keep it under {actor.Gameworld.GetStaticInt("MaximumSayLength").ToString("N0", actor).ColourValue()} characters.");
			return;
		}

		actor.Body.LoudSay(target, message, emote);
	}

	[PlayerCommand("Yell", "yell", "yellat")]
	[DisplayOptions(CommandDisplayOptions.DisplayCommandWords)]
	[RequiredCharacterState(CharacterState.Conscious)]
	protected static void Yell(ICharacter actor, string input)
	{
		var ss = new StringStack(input);
		var argument = ss.Pop();

		IPerceivable target = null;
		if (argument.Equals("yellat", StringComparison.InvariantCultureIgnoreCase))
		{
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("At whom do you want to yell?");
				return;
			}

			target = actor.Target(ss.Pop());
			if (target == null)
			{
				actor.OutputHandler.Send("You do not see anyone or anything like that to yell at.");
				return;
			}
		}

		var emote = new PlayerEmote(ss.PopParentheses(), actor);

		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to yell?");
			return;
		}

		var message = ss.RemainingArgument;
		if (message.Length > actor.Gameworld.GetStaticInt("MaximumSayLength"))
		{
			actor.Send($"That is far too much to say at any one time. Keep it under {actor.Gameworld.GetStaticInt("MaximumSayLength").ToString("N0", actor).ColourValue()} characters.");
			return;
		}

		actor.Body.Yell(target, message, emote);
	}

	[PlayerCommand("Whisper", "whisper", "whisperto")]
	[DisplayOptions(CommandDisplayOptions.DisplayCommandWords)]
	[RequiredCharacterState(CharacterState.SleepingOrBetter)]
	protected static void Whisper(ICharacter actor, string input)
	{
		var ss = new StringStack(input);
		var argument = ss.Pop();

		IPerceivable target = null;
		if (argument.Equals("whisperto", StringComparison.InvariantCultureIgnoreCase))
		{
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("To whom do you want to whisper?");
				return;
			}

			target = actor.Target(ss.Pop());
			if (target == null)
			{
				actor.OutputHandler.Send("You do not see anyone or anything like that to whisper to.");
				return;
			}
		}

		var emote = new PlayerEmote(ss.PopParentheses(), actor);

		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to say?");
			return;
		}

		var message = ss.RemainingArgument;
		if (message.Length > actor.Gameworld.GetStaticInt("MaximumSayLength"))
		{
			actor.Send($"That is far too much to say at any one time. Keep it under {actor.Gameworld.GetStaticInt("MaximumSayLength").ToString("N0", actor).ColourValue()} characters.");
			return;
		}

		actor.Body.Whisper(target, message, emote);
	}

	[PlayerCommand("OOC", "ooc")]
	[HelpInfo("ooc",
		@"This command is used to communicate in an out of character manner with players in the same room as you. Any usage of this command is considered to be you, the player, communicating with other people and not your character. This command should be reserved for helping new players and urgent clarifications of mistakes only. Overuse of this command is very poor form.

The syntax is simply #3OOC <your message>#0 to send a message to everyone in the room.", AutoHelp.HelpArg)]
	protected static void OOC(ICharacter actor, string input)
	{
		var message = input.RemoveFirstWord();
		if (string.IsNullOrEmpty(message))
		{
			actor.OutputHandler.Send("What is it that you wish to say out of character?");
			return;
		}

		if (message.Length > 350)
		{
			actor.Send("That is far too much to say at any one time. Keep it under 350 characters.");
			return;
		}

		foreach (var ch in actor.Location.LayerCharacters(actor.RoomLayer))
		{
			ch.OutputHandler.Send(
				$"{(ch == actor ? "You say" : $"{actor.HowSeen(ch, true)} says")} out of character, \"{message.ProperSentences().NormaliseSpacing().Trim().Fullstop()}\"");
		}
	}

	private const string BoardHelpPlayer =
		@"The board command allows you to interact with discussion boards that are in your room. In order to see the list of posts on a board, simply LOOK at it.

You can use the following syntax with this command:

	#3board read <##>#0 - reads the specified board post
	#3board write <title>#0 - drops you into an editor to make a post
	#3board delete <##>#0 - deletes a board post you authored";

	private const string BoardHelpAdmin =
		@"The board command can be used either to interact with a discussion board item in room or to view boards, including ones that have no in-game item presence. See the BOARDS command for a list of board.

#6Note: Consider POSSESSing an appropriate NPC when making a post to an in-room board#0.

You can use the following syntax with this command:

	#3board posts <board>#0 - view all posts on a specified board
	#3board read <board> <##>#0 - views a particular board post
	#3board read <##>#0 - reads the specified board post from an in-room board
	#3board write <title>#0 - drops you into an editor to make a post to an in-room board
	#3board delete <##>#0 - deletes a board post
	#3board view <##>#0 - views a post for a virtual board
	#3board create <name> <calendar>#0 - creates a new board";

	[PlayerCommand("Board", "board")]
	[RequiredCharacterState(CharacterState.Conscious)]
	[NoMovementCommand]
	[NoHideCommand]
	[HelpInfo("board", BoardHelpPlayer, AutoHelp.HelpArgOrNoArg, BoardHelpAdmin)]
	protected static void Board(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		switch (ss.Pop().ToLowerInvariant())
		{
			case "read":
				BoardRead(actor, ss);
				return;
			case "write":
				BoardWrite(actor, ss);
				return;
			case "delete":
			case "del":
			case "remove":
			case "rem":
				BoardDelete(actor, ss);
				return;
			case "list":
			case "posts":
				if (!actor.IsAdministrator())
				{
					goto default;
				}

				BoardList(actor, ss);
				break;
			case "view":
				if (!actor.IsAdministrator())
				{
					goto default;
				}

				BoardView(actor, ss);
				break;
			case "create":
			case "new":
				if (!actor.IsAdministrator())
				{
					goto default;
				}

				BoardCreate(actor, ss);
				break;
			default:
				actor.Send((actor.IsAdministrator() ? BoardHelpAdmin : BoardHelpPlayer).SubstituteANSIColour());
				return;
		}
	}

	private static void BoardDelete(ICharacter actor, StringStack input)
	{
		var board = actor.Location.LayerGameItems(actor.RoomLayer)
		                 .SelectNotNull(x => x.GetItemType<IBoardItem>()).FirstOrDefault();
		if (board is null)
		{
			actor.OutputHandler.Send("There is not a discussion board present in your location.");
			return;
		}

		if (board.CanViewBoard.Execute<bool?>(actor) == false)
		{
			actor.OutputHandler.Send(board.CantViewBoardEcho.SubstituteANSIColour());
			return;
		}

		if (input.IsFinished)
		{
			actor.OutputHandler.Send("What number post on the board would you like to delete?");
			return;
		}

		if (!int.TryParse(input.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send("That is not a valid number.");
			return;
		}

		var post = board.Board.Posts.ElementAtOrDefault(value - 1);
		if (post is null)
		{
			actor.OutputHandler.Send("There is no such post on that board.");
			return;
		}

		if (!actor.IsAdministrator() && post.AuthorId != actor.Id)
		{
			actor.OutputHandler.Send($"The post {post.Title.ColourName()} (#{post.Id.ToStringN0(actor)}) was not authored by you and so you cannot delete it.");
			return;
		}

		board.Board.DeletePost(post);
		actor.OutputHandler.Send($"You delete the post {post.Title.ColourName()} (#{post.Id.ToStringN0(actor)}).");
	}

	private static void BoardCreate(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to your new board?");
			return;
		}

		var name = ss.PopSpeech().TitleCase();
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which calendar do you want to use for displaying in-game dates for posts on this board?");
			return;
		}

		var calendar = actor.Gameworld.Calendars.GetByIdOrNames(ss.PopSpeech());
		if (calendar is null)
		{
			actor.OutputHandler.Send($"There is no calendar identified by the text \"{ss.Last.ColourCommand()}\".");
			return;
		}

		using (new FMDB())
		{
			var dbBoard = new Models.Board
			{
				Name = name,
				ShowOnLogin = true,
				CalendarId = calendar.Id,
			};
			FMDB.Context.Boards.Add(dbBoard);
			FMDB.Context.SaveChanges();
			var board = new Board(dbBoard, actor.Gameworld);
			actor.Gameworld.Add(board);
			actor.OutputHandler.Send($"You create a new board with ID #{dbBoard.Id.ToString("N0", actor)} called {name.ColourName()} and controlled by the {calendar.Name.ColourName()} calendar.");
		}
	}

	private static void BoardRead(ICharacter actor, StringStack input)
	{
		var board = actor.Location.LayerGameItems(actor.RoomLayer)
										 .SelectNotNull(x => x.GetItemType<IBoardItem>()).FirstOrDefault();
		if (board is null)
		{
			actor.OutputHandler.Send("There is not a discussion board present in your location.");
			return;
		}

		if (board.CanViewBoard.Execute<bool?>(actor) == false)
		{
			actor.OutputHandler.Send(board.CantViewBoardEcho.SubstituteANSIColour());
			return;
		}

		if (input.IsFinished)
		{
			actor.OutputHandler.Send("What number post on the board would you like to read?");
			return;
		}

		if (!int.TryParse(input.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send("That is not a valid number.");
			return;
		}

		var post = board.Board.Posts.ElementAtOrDefault(value - 1);
		if (post is null)
		{
			actor.OutputHandler.Send("There is no such post on that board.");
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine(post.Title.ColourName());
		if (post.InGameDateTime is not null)
		{
			sb.AppendLine(
				$"Posted: {post.InGameDateTime.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()}");
		}

		if (actor.IsAdministrator())
		{
			sb.AppendLine($"Real Time: {post.PostTime.GetLocalDateString(actor, true).ColourValue()}");
			sb.AppendLine($"Author: {post.AuthorName.ColourName()}");
			if (!string.IsNullOrEmpty(post.AuthorShortDescription))
			{
				sb.AppendLine($"SDesc: {post.AuthorShortDescription.ColourCharacter()}");
			}
		}
		else
		{
			if (board.ShowAuthorName)
			{
				sb.AppendLine($"Author: {post.AuthorName.ColourName()}");
			}

			if (board.ShowAuthorShortDescription && !string.IsNullOrEmpty(post.AuthorShortDescription))
			{
				sb.AppendLine($"Author: {post.AuthorShortDescription.ColourCharacter()}");
			}
		}

		sb.AppendLine();
		sb.AppendLine(post.Text.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength));
		if (board.ShowAuthorDescription && !string.IsNullOrEmpty(post.AuthorFullDescription))
		{
			sb.AppendLine();
			sb.AppendLine("Author Description:");
			sb.AppendLine();
			sb.AppendLine(post.AuthorFullDescription.Wrap(actor.InnerLineFormatLength, "\t"));
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	private static void BoardWrite(ICharacter actor, StringStack input)
	{
		var board = actor.Location.LayerGameItems(actor.RoomLayer)
										 .SelectNotNull(x => x.GetItemType<IBoardItem>()).FirstOrDefault();
		if (board is null)
		{
			actor.OutputHandler.Send("There is not a discussion board present in your location.");
			return;
		}

		if (board.CanPostToBoard.Execute<bool?>(actor) == false)
		{
			actor.OutputHandler.Send(board.CantPostToBoardEcho.SubstituteANSIColour());
			return;
		}

		if (input.IsFinished)
		{
			actor.OutputHandler.Send("What title do you want to give to your board post?");
			return;
		}

		if (input.SafeRemainingArgument.Length > 200)
		{
			actor.OutputHandler.Send("Post titles must be under 200 characters in length.");
			return;
		}

		actor.OutputHandler.Send("Enter the post in the editor below.");
		actor.EditorMode(PostAction, CancelAction, 1.0,
			suppliedArguments: new object[] { actor, input.SafeRemainingArgument, board });
	}

	private static void CancelAction(IOutputHandler handler, object[] args)
	{
		handler.Send("You decide not to make a board post.");
	}

	private static void PostAction(string text, IOutputHandler handler, object[] args)
	{
		if (text.Length > 5000)
		{
			handler.Send("Board posts must be under 5000 characters in length.");
			return;
		}

		var actor = (ICharacter)args[0];
		var title = ((string)args[1]).TitleCase();
		var board = (IBoardItem)args[2];

		board.Board.MakeNewPost(actor, title, text);
		handler.Send($"You successfully make your post {title.ColourName()}.");
	}

	private static void BoardView(ICharacter actor, StringStack input)
	{
		if (input.IsFinished)
		{
			actor.Send("Which post do you want to view?");
			return;
		}

		if (!long.TryParse(input.SafeRemainingArgument, out var value))
		{
			actor.Send("That is not a valid ID for a post to view.");
			return;
		}

		var post = actor.Gameworld.Boards.SelectMany(x => x.Posts).FirstOrDefault(x => x.Id == value);
		if (post == null)
		{
			actor.Send("There is no post like that to view.");
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine(
			$"Post #{post.Id.ToString("N0", actor)} by {post.AuthorName?.Colour(Telnet.Magenta) ?? "System".Colour(Telnet.BoldYellow)} on {actor.Gameworld.Boards.First(x => x.Posts.Contains(post)).Name.Colour(Telnet.Green)}");
		sb.AppendLine($"Title: {post.Title.Colour(Telnet.Cyan)}");
		sb.AppendLine($"When (OOC): {post.PostTime.GetLocalDate(actor).ToString("G", actor).ColourValue()}");
		sb.AppendLine(
			$"When (IC): {post.InGameDateTime?.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Immortal).ColourValue() ?? "Unspecified".ColourError()}");
		sb.AppendLine();
		sb.AppendLine(post.Text.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength));
		actor.Send(sb.ToString());
	}

	private static void BoardList(ICharacter actor, StringStack input)
	{
		if (input.IsFinished)
		{
			actor.Send("Which board do you want to list posts for?");
			return;
		}

		var board = actor.Gameworld.Boards.GetByName(input.PopSpeech());
		if (board == null)
		{
			actor.Send("There is no board like that.");
			return;
		}

		if (!board.Posts.Any())
		{
			actor.Send($"The {board.Name.Colour(Telnet.Green)} board has no posts.");
			return;
		}

		var offset = 0U;
		if (!input.IsFinished && !uint.TryParse(input.SafeRemainingArgument, out offset))
		{
			actor.Send("That is not a valid offset for this board.");
			return;
		}

		if (offset >= board.Posts.Count())
		{
			actor.Send("There are not that many posts on that board.");
			return;
		}

		var offsetPosts = board.Posts.Reverse().Skip((int)offset).ToList();
		var posts = offsetPosts.Take(actor.Account.PageLength - 3).ToList();
		var sb = new StringBuilder();
		sb.AppendLine($"The {board.Name.Colour(Telnet.Green)} board has the following posts:");
		foreach (var post in posts)
		{
			var leadin =
				$"#{post.Id} - {post.AuthorName?.Colour(Telnet.BoldGreen) ?? "System".Colour(Telnet.BoldYellow)}: ";
			sb.AppendLine(post.Title.Length + leadin.RawTextLength() <= actor.LineFormatLength
				? $"{leadin}{post.Title}"
				: $"{leadin}{post.Title.RawTextSubstring(0, actor.Account.LineFormatLength - leadin.RawTextLength() - 3)}...");
		}

		if (offsetPosts.Count != posts.Count)
		{
			sb.AppendLine(
				$"Use the command {$"show {board.Name.ToLowerInvariant()} {offset + posts.Count:N0}".Colour(Telnet.Yellow)} to view the next page of posts.");
		}

		actor.Send(sb.ToString());
	}

	[PlayerCommand("Boards", "boards")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	[HelpInfo("boards", @"The #3boards#0 command is used to review which boards exist in the game, which you can interact with and edit with the related #3board#0 command. The syntax is simply #3boards#0.", AutoHelp.HelpArg)]
	protected static void Boards(ICharacter actor, string input)
	{
		var sb = new StringBuilder("There are the following boards available:\n");
		foreach (var board in actor.Gameworld.Boards)
		{
			sb.AppendLine($"\t{board.Name} - {board.Posts.Count():N0} posts");
		}

		actor.Send(sb.ToString());
	}
}