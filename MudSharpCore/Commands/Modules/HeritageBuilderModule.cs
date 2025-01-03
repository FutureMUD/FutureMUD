using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Character.Name;
using MudSharp.Commands.Helpers;
using MudSharp.Communication.Language;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Knowledge;

namespace MudSharp.Commands.Modules;

internal class HeritageBuilderModule : BaseBuilderModule
{
	private HeritageBuilderModule()
		: base("HeritageBuilder")
	{
		IsNecessary = true;
	}

	public new static HeritageBuilderModule Instance { get; } = new();

	#region Languages

	private const string LanguageCommandHelp =
		"This command allows you to view detailed information about a language that you speak. The syntax is LANGUAGE <name>, and can be used on any language that you know.";

	private const string AdminLanguageCommandHelp =
		@"This command allows you to create and edit languages. You can use the following options with this command:

	#3language show <which>#0 - shows you a language
	#3language edit new <name> <trait>#0 - creates a new language with the specified name and linked trait
	#3language edit <which>#0 - begins editing a language
	#3language close#0 - stops editing a language
	#3language set ...#0 - sets the properties of a language you're editing";

	[PlayerCommand("Language", "language")]
	[HelpInfo("language", LanguageCommandHelp, AutoHelp.HelpArgOrNoArg, AdminLanguageCommandHelp)]
	[CommandPermission(PermissionLevel.Guest)]
	protected static void Language(ICharacter actor, string command)
	{
		if (!actor.IsAdministrator())
		{
			PlayerLanguage(actor, command);
			return;
		}

		var ss = new StringStack(command.RemoveFirstWord());
		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "edit":
				LanguageEdit(actor, ss);
				return;
			case "set":
				LanguageSet(actor, ss);
				return;
			case "close":
				LanguageClose(actor, ss);
				return;
			case "view":
			case "show":
				LanguageView(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(AdminLanguageCommandHelp.SubstituteANSIColour());
				return;
		}
	}

	protected static void PlayerLanguage(ICharacter actor, string input)
	{
		var which = input.RemoveFirstWord();
		var languages = actor.IsAdministrator() ? actor.Gameworld.Languages.ToList() : actor.Languages.ToList();
		var language = languages.FirstOrDefault(x => x.Name.EqualTo(which)) ??
					   languages.FirstOrDefault(x =>
						   x.Name.StartsWith(which, StringComparison.InvariantCultureIgnoreCase)) ??
					   languages.FirstOrDefault(
						   x => x.Name.Contains(which, StringComparison.InvariantCultureIgnoreCase));

		if (language == null)
		{
			actor.OutputHandler.Send("You do not know any such language.");
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine($"Information for the {language.Name.Proper().Colour(Telnet.Green)} language.");
		sb.AppendLine($"Trait: {language.LinkedTrait.Name.Colour(Telnet.Green)}");
		sb.AppendLine($"Unknown Description: {language.UnknownLanguageSpokenDescription.Colour(Telnet.Yellow)}");
		sb.AppendLine();
		sb.AppendLine("Normally written with one of the following scripts:");
		var scripts = actor.Gameworld.Scripts.Where(x => x.DesignedLanguages.Contains(language)).ToList();
		if (!scripts.Any())
		{
			sb.AppendLine("\tNone that you are aware of.");
		}
		else
		{
			foreach (var script in scripts)
			{
				sb.AppendLine($"\t{script.KnownScriptDescription}");
			}
		}

		sb.AppendLine();
		sb.AppendLine("Offers mutual intelligibility with the following languages:");
		var mutuals = actor.Gameworld.Languages.Where(x => language.MutualIntelligability(x) != Difficulty.Impossible)
						   .ToList();
		if (!mutuals.Any())
		{
			sb.AppendLine("\tNone that you are aware of.");
		}
		else
		{
			foreach (var mutual in mutuals)
			{
				sb.AppendLine(
					$"\t{mutual.Name.Proper()} @ {language.MutualIntelligability(mutual).Describe().Colour(Telnet.Green)}");
			}
		}

		actor.OutputHandler.Send(sb.ToString());
	}


	private static void LanguageClose(ICharacter actor, StringStack ss)
	{
		var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<ILanguage>>().FirstOrDefault();
		if (editing == null)
		{
			actor.OutputHandler.Send("You are not editing any languages.");
			return;
		}

		actor.RemoveEffect(editing);
		actor.OutputHandler.Send(
			$"You are no longer editing the {editing.EditingItem.Name.ColourName()} language.");
	}

	private static void LanguageView(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<ILanguage>>().FirstOrDefault();
			if (editing == null)
			{
				actor.OutputHandler.Send("You are not editing any languages.");
				return;
			}

			actor.OutputHandler.Send(editing.EditingItem.Show(actor));
			return;
		}

		var language = long.TryParse(ss.SafeRemainingArgument, out var value)
			? actor.Gameworld.Languages.Get(value)
			: actor.Gameworld.Languages.GetByName(ss.SafeRemainingArgument);
		if (language == null)
		{
			actor.OutputHandler.Send("There is no such language.");
			return;
		}

		actor.OutputHandler.Send(language.Show(actor));
	}

	private static void LanguageSet(ICharacter actor, StringStack ss)
	{
		var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<ILanguage>>().FirstOrDefault();
		if (editing == null)
		{
			actor.OutputHandler.Send(
				"You are not editing any languages. Please specify a language that you want to edit.");
			return;
		}

		editing.EditingItem.BuildingCommand(actor, ss);
	}

	private static void LanguageEdit(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<ILanguage>>().FirstOrDefault();
			if (editing == null)
			{
				actor.OutputHandler.Send(
					"You are not editing any languages. Please specify a language that you want to edit.");
				return;
			}

			actor.OutputHandler.Send(editing.EditingItem.Show(actor));
			return;
		}

		if (ss.PeekSpeech().EqualTo("new"))
		{
			ss.PopSpeech();
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("What name do you want to give to your new language?");
				return;
			}

			var name = ss.PopSpeech();
			if (actor.Gameworld.Languages.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send("There is already a language with that name. Names must be unique.");
				return;
			}

			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("Which trait do you want to use as the linked trait for this language?");
				return;
			}

			var trait = long.TryParse(ss.SafeRemainingArgument, out var tvalue)
				? actor.Gameworld.Traits.Get(tvalue)
				: actor.Gameworld.Traits.GetByName(ss.SafeRemainingArgument);
			if (trait == null)
			{
				actor.OutputHandler.Send("There is no such trait.");
				return;
			}

			var newLanguage = new Language(actor.Gameworld, trait, name);
			actor.Gameworld.Add(newLanguage);
			actor.RemoveAllEffects<BuilderEditingEffect<ILanguage>>();
			actor.AddEffect(new BuilderEditingEffect<ILanguage>(actor) { EditingItem = newLanguage });
			actor.OutputHandler.Send(
				$"You create a new language called {newLanguage.Name.ColourName()} with Id #{newLanguage.Id.ToString("N0", actor)}, which you are now editing.");
			return;
		}

		var language = long.TryParse(ss.SafeRemainingArgument, out var value)
			? actor.Gameworld.Languages.Get(value)
			: actor.Gameworld.Languages.GetByName(ss.SafeRemainingArgument);
		if (language == null)
		{
			actor.OutputHandler.Send("There is no such language.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<ILanguage>>();
		actor.AddEffect(new BuilderEditingEffect<ILanguage>(actor) { EditingItem = language });
		actor.OutputHandler.Send($"You are now editing the {language.Name.ColourName()} language.");
	}

	#endregion

	#region Accents

	private const string AccentPlayerHelp =
		"The accent command allows you to view detailed information about a particular accent that you speak. The syntax is ACCENT <which accent>, or ACCENT <language>.<which accent> if you have accents that have the same name in multiple languages.";

	private const string AccentAdminHelp =
		@"This command allows you to create and edit accents. You can use the following options with this command:

	#3accent show <which>#0 - shows you an accent
	#3accent edit new <name> <language>#0 - creates a new accent with the specified name for a language
	#3accent edit <which>#0 - begins editing an accent
	#3accent close#0 - stops editing an accent
	#3accent set ...#0 - sets the properties of an accent you're editing";

	[PlayerCommand("Accent", "accent")]
	[HelpInfo("accent", AccentPlayerHelp, AutoHelp.HelpArgOrNoArg, AccentAdminHelp)]
	[CommandPermission(PermissionLevel.Guest)]
	protected static void Accent(ICharacter actor, string input)
	{
		if (!actor.IsAdministrator())
		{
			PlayerAccent(actor, input);
			return;
		}

		var ss = new StringStack(input.RemoveFirstWord());
		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "edit":
				AccentEdit(actor, ss);
				return;
			case "close":
				AccentClose(actor);
				return;
			case "set":
				AccentSet(actor, ss);
				return;
			case "view":
			case "show":
				AccentShow(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(AccentAdminHelp.SubstituteANSIColour());
				return;
		}
	}

	private static void AccentEdit(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IAccent>>().FirstOrDefault();
			if (editing == null)
			{
				actor.OutputHandler.Send(
					"You are not editing any accents. Please specify an accent that you want to edit.");
				return;
			}

			actor.OutputHandler.Send(editing.EditingItem.Show(actor));
			return;
		}

		if (ss.PeekSpeech().EqualTo("new"))
		{
			ss.PopSpeech();
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("What name do you want to give to your new accent?");
				return;
			}

			var name = ss.PopSpeech();

			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("What language do you want to create an accent for?");
				return;
			}

			var language = long.TryParse(ss.PopSpeech(), out var value)
				? actor.Gameworld.Languages.Get(value)
				: actor.Gameworld.Languages.GetByName(ss.Last);
			if (language == null)
			{
				actor.OutputHandler.Send("There is no such language.");
				return;
			}

			if (actor.Gameworld.Accents.Any(x => x.Name.EqualTo(name) && x.Language == language))
			{
				actor.OutputHandler.Send(
					"There is already an accent with that name for that language. Names must be unique.");
				return;
			}

			var newAccent = new Accent(actor.Gameworld, language, name);
			actor.Gameworld.Add(newAccent);
			actor.RemoveAllEffects<BuilderEditingEffect<IAccent>>();
			actor.AddEffect(new BuilderEditingEffect<IAccent>(actor) { EditingItem = newAccent });
			actor.OutputHandler.Send(
				$"You create a new accent called {newAccent.Name.ColourName()} with Id #{newAccent.Id.ToString("N0", actor)} for the {language.Name.ColourName()} language, which you are now editing.");
			return;
		}

		var accent = long.TryParse(ss.SafeRemainingArgument, out var evalue)
			? actor.Gameworld.Accents.Get(evalue)
			: actor.Gameworld.Accents.GetByName(ss.SafeRemainingArgument);
		if (accent == null)
		{
			actor.OutputHandler.Send("There is no such accent.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<IAccent>>();
		actor.AddEffect(new BuilderEditingEffect<IAccent>(actor) { EditingItem = accent });
		actor.OutputHandler.Send($"You are now editing the {accent.Name.ColourName()} accent.");
	}

	private static void AccentClose(ICharacter actor)
	{
		var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IAccent>>().FirstOrDefault();
		if (editing == null)
		{
			actor.OutputHandler.Send("You are not editing any accents.");
			return;
		}

		actor.RemoveEffect(editing);
		actor.OutputHandler.Send(
			$"You are no longer editing the {editing.EditingItem.Name.ColourName()} accent.");
	}

	private static void AccentSet(ICharacter actor, StringStack ss)
	{
		var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IAccent>>().FirstOrDefault();
		if (editing == null)
		{
			actor.OutputHandler.Send(
				"You are not editing any accents. Please specify an accent that you want to edit.");
			return;
		}

		editing.EditingItem.BuildingCommand(actor, ss);
	}

	private static void AccentShow(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IAccent>>().FirstOrDefault();
			if (editing == null)
			{
				actor.OutputHandler.Send("You are not editing any accents.");
				return;
			}

			actor.OutputHandler.Send(editing.EditingItem.Show(actor));
			return;
		}

		var accent = long.TryParse(ss.SafeRemainingArgument, out var value)
			? actor.Gameworld.Accents.Get(value)
			: actor.Gameworld.Accents.GetByName(ss.SafeRemainingArgument);
		if (accent == null)
		{
			actor.OutputHandler.Send("There is no such accent.");
			return;
		}

		actor.OutputHandler.Send(accent.Show(actor));
	}

	protected static void PlayerAccent(ICharacter actor, string input)
	{
		var which = input.RemoveFirstWord();
		var language = default(ILanguage);
		if (which.Contains('.'))
		{
			var split = which.Split(new[] { '.' }, 2);
			var lang = split[0];
			var languages = actor.IsAdministrator() ? actor.Gameworld.Languages.ToList() : actor.Languages.ToList();
			language = languages.FirstOrDefault(x => x.Name.EqualTo(lang)) ??
					   languages.FirstOrDefault(x =>
						   x.Name.StartsWith(lang, StringComparison.InvariantCultureIgnoreCase)) ??
					   languages.FirstOrDefault(x =>
						   x.Name.Contains(lang, StringComparison.InvariantCultureIgnoreCase));

			if (language == null)
			{
				actor.OutputHandler.Send("You do not know any such language.");
				return;
			}

			which = split[1];
		}

		var accents = actor.IsAdministrator() ? actor.Gameworld.Accents.ToList() : actor.Accents.ToList();
		if (language != null)
		{
			accents = accents.Where(x => x.Language == language).ToList();
		}

		var accent = accents.FirstOrDefault(x => x.Name.EqualTo(which)) ??
					 accents.FirstOrDefault(x =>
						 x.Name.StartsWith(which, StringComparison.InvariantCultureIgnoreCase)) ??
					 accents.FirstOrDefault(x => x.Name.Contains(which, StringComparison.InvariantCultureIgnoreCase));
		if (accent == null)
		{
			actor.OutputHandler.Send(
				$"You do not know any such accent{(language != null ? $" in the {language.Name.Proper().Colour(Telnet.Green)} language" : "")}.");
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine(
			$"Information about the {accent.Name.TitleCase().Colour(Telnet.Green)} accent for the {accent.Language.Name.Proper().Colour(Telnet.Green)} language.");
		sb.AppendLine($"Group: {accent.Group.TitleCase().Colour(Telnet.Green)}");
		sb.AppendLine($"Difficulty for listener if not known: {accent.Difficulty.Describe().Colour(Telnet.Green)}");
		sb.AppendLine(
			$"Difficulty for those who know an accent from the same group: {Utilities.StageDown(accent.Difficulty, 1).Describe().Colour(Telnet.Green)}");
		sb.AppendLine(
			$"Difficulty for those who know 3+ accents from the same group: {Utilities.StageDown(accent.Difficulty, 2).Describe().Colour(Telnet.Green)}");
		sb.AppendLine($"Known Suffix: {accent.AccentSuffix.Colour(Telnet.Yellow)}");
		sb.AppendLine($"Unknown Suffix: {accent.VagueSuffix.Colour(Telnet.Yellow)}");
		sb.AppendLine();
		sb.AppendLine($"Accent description:");
		sb.AppendLine();
		sb.AppendLine(accent.Description.Wrap(actor.InnerLineFormatLength));
		actor.OutputHandler.Send(sb.ToString());
	}

	#endregion

	#region Scripts

	private const string ScriptCommandPlayerHelp =
		"This command allows you to set which script and language you use when you write, as well as view information about scripts. For the former usage, the syntax is SCRIPT <script> <language>. For the latter, it is SCRIPT SHOW <script>.";

	private const string ScriptCommandAdminHelp =
		@"This command allows you to create and edit scripts. You can use the following options with this command:

	#3script show <which>#0 - shows you a script
	#3script edit new <name> <knowledge>#0 - creates a new script with the specified name and linked knowledge
	#3script edit <which>#0 - begins editing a script
	#3script close#0 - stops editing a script
	#3script set ...#0 - sets the properties of a script you're editing";

	[PlayerCommand("Script", "script")]
	[HelpInfo("script", ScriptCommandPlayerHelp, AutoHelp.HelpArgOrNoArg, ScriptCommandAdminHelp)]
	[CommandPermission(PermissionLevel.Guest)]
	protected static void Script(ICharacter actor, string command)
	{
		if (!actor.IsAdministrator())
		{
			PlayerScript(actor, command);
			return;
		}

		var ss = new StringStack(command.RemoveFirstWord());
		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "edit":
				ScriptEdit(actor, ss);
				return;
			case "set":
				ScriptSet(actor, ss);
				return;
			case "show":
			case "view":
				ScriptShow(actor, ss);
				return;
			case "close":
				ScriptClose(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(ScriptCommandAdminHelp.SubstituteANSIColour());
				return;
		}
	}

	private static void ScriptClose(ICharacter actor, StringStack ss)
	{
		var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IScript>>().FirstOrDefault();
		if (effect == null)
		{
			actor.OutputHandler.Send("You are not editing any scripts.");
			return;
		}

		actor.RemoveEffect(effect);
		actor.OutputHandler.Send($"You are no longer editing the {effect.EditingItem.Name.ColourName()} script.");
	}

	private static void ScriptShow(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IScript>>().FirstOrDefault();
			if (effect == null)
			{
				actor.OutputHandler.Send("Which script do you want to show?");
				return;
			}

			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		var script = long.TryParse(ss.SafeRemainingArgument, out var value)
			? actor.Gameworld.Scripts.Get(value)
			: actor.Gameworld.Scripts.GetByName(ss.SafeRemainingArgument);
		if (script == null)
		{
			actor.OutputHandler.Send("There is no such script.");
			return;
		}

		actor.OutputHandler.Send(script.Show(actor));
	}

	private static void ScriptSet(ICharacter actor, StringStack ss)
	{
		var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IScript>>().FirstOrDefault();
		if (effect == null)
		{
			actor.OutputHandler.Send("You are not editing any scripts.");
			return;
		}

		effect.EditingItem.BuildingCommand(actor, ss);
	}

	private static void ScriptEdit(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IScript>>().FirstOrDefault();
			if (effect == null)
			{
				actor.OutputHandler.Send("Which script do you want to edit?");
				return;
			}

			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		var cmd = ss.PopSpeech();
		if (cmd.EqualTo("new"))
		{
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("Which name do you want to give to your new script?");
				return;
			}

			var name = ss.PopSpeech().ToLowerInvariant().TitleCase();
			if (actor.Gameworld.Scripts.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send("There is already a script with that name. Names must be unique.");
				return;
			}

			if (ss.IsFinished)
			{
				actor.OutputHandler.Send(
					"Which knowledge do you want to use to control access to this script? You must build this knowledge first and supply it at the time of script creation.");
				return;
			}

			var knowledge = long.TryParse(ss.SafeRemainingArgument, out var value)
				? actor.Gameworld.Knowledges.Get(value)
				: actor.Gameworld.Knowledges.GetByName(ss.SafeRemainingArgument);
			if (knowledge == null)
			{
				actor.OutputHandler.Send("There is no such knowledge.");
				return;
			}

			var newScript = new Script(actor.Gameworld, name, knowledge);
			actor.Gameworld.Add(newScript);
			actor.RemoveAllEffects<BuilderEditingEffect<IScript>>();
			actor.AddEffect(new BuilderEditingEffect<IScript>(actor) { EditingItem = newScript });
			actor.OutputHandler.Send(
				$"You create a new script with Id #{newScript.Id.ToString("N0", actor)} called {name.ColourName()} linked with knowledge {knowledge.Name.ColourName()}.");
			return;
		}

		var script = long.TryParse(cmd, out var lvalue)
			? actor.Gameworld.Scripts.Get(lvalue)
			: actor.Gameworld.Scripts.GetByName(cmd);
		if (script == null)
		{
			actor.OutputHandler.Send("There is no such script.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<IScript>>();
		actor.AddEffect(new BuilderEditingEffect<IScript>(actor) { EditingItem = script });
		actor.OutputHandler.Send($"You are now editing the script called {script.Name.ColourName()}.");
	}


	protected static void PlayerScript(ICharacter actor, string command)
	{
		if (!actor.IsLiterate)
		{
			actor.Send("You're illiterate. You don't know anything about fancy squiggly lines.");
			return;
		}

		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.Send(actor.CurrentScript != null
				? $"When you write, you use the {actor.CurrentScript.Name.Colour(Telnet.Green)} script and the {actor.CurrentWritingLanguage.Name.Colour(Telnet.Green)} language."
				: $"You don't have a current script set.");
			return;
		}

		var text = ss.PopSpeech();
		IScript script = null;
		var showMode = false;
		if (text.EqualToAny("view", "show", "info"))
		{
			if (ss.IsFinished)
			{
				actor.Send("Which script do you want to show information about?");
				return;
			}

			text = ss.PopSpeech();
			showMode = true;
		}

		if (!actor.IsAdministrator())
		{
			script = actor.Scripts.FirstOrDefault(
				x => x.Name.StartsWith(text, StringComparison.InvariantCultureIgnoreCase));
		}
		else
		{
			script = actor.Gameworld.Scripts.FirstOrDefault(
				x => x.Name.StartsWith(text, StringComparison.InvariantCultureIgnoreCase));
		}

		if (script == null)
		{
			actor.Send("You don't know any script like that.");
			return;
		}

		if (showMode)
		{
			var sb = new StringBuilder();
			sb.AppendLine($"Information about the script {script.Name.TitleCase().Colour(Telnet.Green)}.");
			sb.AppendLine($"Linked Knowledge: {script.ScriptKnowledge.Name.Colour(Telnet.Green)}");
			sb.AppendLine($"Known Description: {script.KnownScriptDescription.Colour(Telnet.Yellow)}");
			sb.AppendLine($"Unknown Description: {script.UnknownScriptDescription.Colour(Telnet.Yellow)}");
			sb.AppendLine(
				$"Document Length Modifier: {script.DocumentLengthModifier.ToString("N2", actor).Colour(Telnet.Green)}");
			sb.AppendLine($"Ink Use Modifier: {script.InkUseModifier.ToString("N2", actor).Colour(Telnet.Green)}");
			actor.OutputHandler.Send(sb.ToString());
			return;
		}

		if (ss.IsFinished && actor.CurrentWritingLanguage == null)
		{
			actor.Send("You must also specify a language to use when writing.");
			return;
		}

		var language = actor.CurrentWritingLanguage;
		if (!ss.IsFinished)
		{
			text = ss.PopSpeech();
			language = actor.IsAdministrator()
				? actor.Gameworld.Languages.FirstOrDefault(x =>
					x.Name.StartsWith(text, StringComparison.InvariantCultureIgnoreCase))
				: actor.Languages.FirstOrDefault(x =>
					x.Name.StartsWith(text, StringComparison.InvariantCultureIgnoreCase));
			if (language == null)
			{
				actor.Send("You don't know any language like that.");
				return;
			}
		}

		actor.CurrentScript = script;
		actor.CurrentWritingLanguage = language;
		actor.Send(
			$"You will now use the {script.Name.Colour(Telnet.Green)} script and the {language.Name.Colour(Telnet.Green)} language when you write.");
	}

	#endregion

	#region Knowledges

	private const string KnowledgeCommandHelp =
		"This command allows you to view detailed information about a knowledge that you have. The syntax is KNOWLEDGE <name>, and can be used on any knowledge that you know.";

	private const string AdminKnowledgeCommandHelp =
		@"This command allows you to create and edit knowledges. You can use the following options with this command:

	#3knowledge show <which>#0 - shows you a knowledge
	#3knowledge edit new <name>#0 - creates a new knowledge with the specified name
	#3knowledge edit <which>#0 - begins editing a language
	#3knowledge close#0 - stops editing a knowledge
	#3knowledge set name <name>#0 - renames the knowledge
	#3knowledge set desc <desc>#0 - gives a new brief description of the knowledge
	#3knowledge set ldesc <desc>#0 - sets the long description of a knowledge
	#3knowledge set type <type>#0 - sets the knowledge type / category
	#3knowledge set subtype <type>#0 - sets the knowledge subtype / subcategory
	#3knowledge set sessions <##>#0 - sets the number of #3teach#0 sessions before someone learns the knowledge
	#3knowledge set learnable LearnableAtSkillUp|LearnableAtChargen|LearnableFromTeacher#0 - toggles a learn type
	#3knowledge set learnprog <prog>#0 - sets a prog that controls if someone can learn the knowledge
	#3knowledge set learndifficulty <difficulty>#0 - sets the difficulty of the learn checks
	#3knowledge set teachdifficulty <difficulty>#0 - sets the difficulty of the teach checks
	#3knowledge set chargenprog <prog>#0 - sets the prog that controls if it can be taken at chargen
	#3knowledge set resource <which> <##>#0 - sets the chargne cost of this knowledge (use 0 to remove cost)";

	[PlayerCommand("Knowledge", "knowledge")]
	[HelpInfo("knowledge", KnowledgeCommandHelp, AutoHelp.HelpArgOrNoArg, AdminKnowledgeCommandHelp)]
	[CommandPermission(PermissionLevel.Guest)]
	protected static void Knowledge(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (!actor.IsAdministrator())
		{
			PlayerKnowledge(actor, ss);
			return;
		}

		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "edit":
				KnowledgeEdit(actor, ss);
				return;
			case "show":
				KnowledgeShow(actor, ss);
				return;
			case "close":
				KnowledgeClose(actor, ss);
				return;
			case "set":
				KnowledgeSet(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(AdminKnowledgeCommandHelp.SubstituteANSIColour());
				return;
		}
	}

	private static void KnowledgeSet(ICharacter actor, StringStack ss)
	{
		var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IKnowledge>>().FirstOrDefault();
		if (editing == null)
		{
			actor.OutputHandler.Send("You are not editing any knowledges.");
			return;
		}

		editing.EditingItem.BuildingCommand(actor, ss);
	}

	private static void KnowledgeEdit(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IKnowledge>>().FirstOrDefault();
			if (editing == null)
			{
				actor.OutputHandler.Send("Which knowledge do you want to edit?");
				return;
			}

			actor.OutputHandler.Send(editing.EditingItem.Show(actor));
			return;
		}

		var cmd = ss.PopSpeech();
		if (cmd.EqualTo("new"))
		{
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("What name do you want to give to the new knowledge?");
				return;
			}

			var name = ss.SafeRemainingArgument;
			if (actor.Gameworld.Knowledges.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send("There is already a knowledge with that name. Names must be unique.");
				return;
			}

			var newKnowledge = new Knowledge(name);
			actor.Gameworld.Add(newKnowledge);
			actor.RemoveAllEffects<BuilderEditingEffect<IKnowledge>>();
			actor.AddEffect(new BuilderEditingEffect<IKnowledge>(actor) { EditingItem = newKnowledge });
			actor.OutputHandler.Send(
				$"You create a new knowledge called {name.ColourName()}, which you are now editing.");
			return;
		}

		var knowledge = long.TryParse(cmd, out var value)
			? actor.Gameworld.Knowledges.Get(value)
			: actor.Gameworld.Knowledges.GetByName(cmd);
		if (knowledge == null)
		{
			actor.OutputHandler.Send("There is no such knowledge.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<IKnowledge>>();
		actor.AddEffect(new BuilderEditingEffect<IKnowledge>(actor) { EditingItem = knowledge });
		actor.OutputHandler.Send($"You are now editing {knowledge.Name.ColourName()}.");
	}

	private static void KnowledgeShow(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IKnowledge>>().FirstOrDefault();
			if (editing == null)
			{
				actor.OutputHandler.Send("You are not editing any knowledges.");
				return;
			}

			actor.OutputHandler.Send(editing.EditingItem.Show(actor));
			return;
		}

		var knowledge = long.TryParse(ss.SafeRemainingArgument, out var value)
			? actor.Gameworld.Knowledges.Get(value)
			: actor.Gameworld.Knowledges.GetByName(ss.SafeRemainingArgument);
		if (knowledge == null)
		{
			actor.OutputHandler.Send("There is no such knowledge.");
			return;
		}

		actor.OutputHandler.Send(knowledge.Show(actor));
	}

	private static void KnowledgeClose(ICharacter actor, StringStack ss)
	{
		var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IKnowledge>>().FirstOrDefault();
		if (editing == null)
		{
			actor.OutputHandler.Send("You are not editing any knowledges.");
			return;
		}

		actor.RemoveEffect(editing);
		actor.OutputHandler.Send($"You are no longer editing the {editing.EditingItem.Name.ColourName()} knowledge.");
	}

	private static void PlayerKnowledge(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which of your knowledges would you like to view more information on?");
			return;
		}

		var target = ss.SafeRemainingArgument;
		var knowledge = actor.CharacterKnowledges.FirstOrDefault(x => x.Knowledge.Name.EqualTo(target)) ??
						actor.CharacterKnowledges.FirstOrDefault(x => x.Knowledge.Description.EqualTo(target)) ??
						actor.CharacterKnowledges.FirstOrDefault(x => x.Name.StartsWith(target, StringComparison.InvariantCultureIgnoreCase));
		if (knowledge == null)
		{
			actor.OutputHandler.Send("You don't know of any knowledge like that.");
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine($"Information about the {knowledge.Knowledge.Name.ColourName()} Knowledge:");
		sb.AppendLine($"Description: {knowledge.Knowledge.Description.ColourCommand()}");
		sb.AppendLine($"Long Description:");
		sb.AppendLine();
		sb.AppendLine(knowledge.Knowledge.LongDescription.Wrap(actor.InnerLineFormatLength));
		sb.AppendLine();
		sb.AppendLine($"Teach Difficulty: {knowledge.Knowledge.TeachDifficulty.Describe().ColourValue()}");
		sb.AppendLine($"Learn Difficulty: {knowledge.Knowledge.LearnDifficulty.Describe().ColourValue()}");
		sb.AppendLine(
			$"Learner Sessions Required: {knowledge.Knowledge.LearnerSessionsRequired.ToString("N0", actor).ColourValue()}");
		actor.OutputHandler.Send(sb.ToString());
	}

	#endregion

	#region Random Names

	private const string RandomNameHelp =
		@"This command allows you to view, create and edit random name profiles, which are used to generate random names for variable NPCs or storyteller use.

For the closely related command to generate your own names on the fly, see RANDOMNAMES.

The syntax for this command is as follows:

	#3randomname list#0 - lists all of the random name profiles
	#3randomname list <culture>#0 - lists all of the random name profiles for a specific culture
	#3randomname edit <which>#0 - begins editing a random name profile
	#3randomname edit new <culture> <name> <gender>#0 - generates a new random name profile
	#3randomname clone <old> <new>#0 - clones an existing random name profile to a new one
	#3randomname close#0 - stops editing a random name profile
	#3randomname show <which>#0 - views information about a random name profile
	#3randomname show#0 - views information about your currently editing random name profile
	#3randomname set ...#0 - edits the properties of a random name culture";

	[PlayerCommand("RandomName", "randomname", "rn")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("randomname", RandomNameHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void RandomName(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		switch (ss.PopSpeech())
		{
			case "list":
				RandomNameList(actor, ss);
				return;
			case "edit":
				RandomNameEdit(actor, ss);
				return;
			case "close":
				RandomNameClose(actor, ss);
				return;
			case "set":
				RandomNameSet(actor, ss);
				return;
			case "show":
			case "view":
				RandomNameShow(actor, ss);
				return;
			case "clone":
				RandomNameClone(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(RandomNameHelp.SubstituteANSIColour());
				return;
		}
	}

	private static void RandomNameList(ICharacter actor, StringStack ss)
	{
		var profiles = actor.Gameworld.RandomNameProfiles.ToList();
		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from item in profiles
			select new List<string>
			{
				item.Id.ToString("N0", actor),
				item.Name,
				item.Gender.DescribeEnum(),
				item.Culture.Name,
				item.RandomNames
					.Select(x =>
						$"{x.Value.Count.ToString("N0", actor)} {x.Key.DescribeEnum().Pluralise()}".ColourValue())
					.ListToCommaSeparatedValues(", "),
				item.UseForChargenSuggestionsProg?.MXPClickableFunctionName() ?? ""
			},
			new List<string>
			{
				"Id",
				"Name",
				"Gender",
				"Name Culture",
				"No. Elements",
				"Chargen Suggestions"
			},
			actor.LineFormatLength,
			colour: Telnet.Cyan,
			unicodeTable: actor.Account.UseUnicode
		));
	}

	private static void RandomNameEdit(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IRandomNameProfile>>().FirstOrDefault();
			if (effect == null)
			{
				actor.OutputHandler.Send("Which random name profile would you like to edit?");
				return;
			}

			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		if (ss.PeekSpeech().EqualTo("new"))
		{
			ss.PopSpeech();
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("Which name culture would you like to create a random name profile for?");
				return;
			}

			var culture = long.TryParse(ss.PopSpeech(), out var profileid)
				? actor.Gameworld.NameCultures.Get(profileid)
				: actor.Gameworld.NameCultures.GetByName(ss.Last);
			if (culture == null)
			{
				actor.OutputHandler.Send("There is no such name culture.");
				return;
			}

			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("What name do you want to give to your new random name profile?");
				return;
			}

			var name = ss.PopSpeech().TitleCase();
			if (actor.Gameworld.RandomNameProfiles.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					$"There is already a random name profile for the {culture.Name.ColourName()} name culture with that name. Names must be unique.");
				return;
			}

			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("What gender should this random name profile generate names for?");
				return;
			}

			if (!ss.SafeRemainingArgument.TryParseEnum<Gender>(out var gender))
			{
				actor.OutputHandler.Send(
					$"That is not a valid gender. The valid genders are {Enum.GetNames<Gender>().Select(x => x.ColourValue()).ListToString()}.");
				return;
			}

			var newProfile = new RandomNameProfile(culture, gender, name);
			actor.Gameworld.Add(newProfile);
			actor.OutputHandler.Send(
				$"You create a new random name profile called {name.ColourName()}, which you are now editing.");
			actor.RemoveAllEffects<BuilderEditingEffect<IRandomNameProfile>>();
			actor.AddEffect(new BuilderEditingEffect<IRandomNameProfile>(actor) { EditingItem = newProfile });
			return;
		}

		var randomProfile = long.TryParse(ss.SafeRemainingArgument, out var value)
			? actor.Gameworld.RandomNameProfiles.Get(value)
			: actor.Gameworld.RandomNameProfiles.GetByName(ss.SafeRemainingArgument);
		if (randomProfile == null)
		{
			actor.OutputHandler.Send("There is no such name culture.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<IRandomNameProfile>>();
		actor.AddEffect(new BuilderEditingEffect<IRandomNameProfile>(actor) { EditingItem = randomProfile });
		actor.OutputHandler.Send($"You are now editing the name culture {randomProfile.Name.ColourName()}.");
	}

	private static void RandomNameClose(ICharacter actor, StringStack ss)
	{
		var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IRandomNameProfile>>().FirstOrDefault();
		if (effect == null)
		{
			actor.OutputHandler.Send("You are not editing any random name profiles.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<IRandomNameProfile>>();
		actor.OutputHandler.Send("You are no longer editing any random name profiles.");
	}

	private static void RandomNameShow(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IRandomNameProfile>>().FirstOrDefault();
			if (effect == null)
			{
				actor.OutputHandler.Send("Which random name profile would you like to view?");
				return;
			}

			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		var randomProfile = long.TryParse(ss.SafeRemainingArgument, out var value)
			? actor.Gameworld.RandomNameProfiles.Get(value)
			: actor.Gameworld.RandomNameProfiles.GetByName(ss.SafeRemainingArgument);
		if (randomProfile == null)
		{
			actor.OutputHandler.Send("There is no such random name profile.");
			return;
		}

		actor.OutputHandler.Send(randomProfile.Show(actor));
	}

	private static void RandomNameSet(ICharacter actor, StringStack ss)
	{
		var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IRandomNameProfile>>().FirstOrDefault();
		if (effect == null)
		{
			actor.OutputHandler.Send("You are not editing any random name profiles.");
			return;
		}

		effect.EditingItem.BuildingCommand(actor, ss);
	}

	private static void RandomNameClone(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which random name profile do you want to clone?");
			return;
		}

		var profile = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.RandomNameProfiles.Get(value)
			: actor.Gameworld.RandomNameProfiles.GetByName(ss.Last);
		if (profile == null)
		{
			actor.OutputHandler.Send("There is no such random name profile.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to the new random name profile?");
			return;
		}

		var name = ss.SafeRemainingArgument.TitleCase();
		if (actor.Gameworld.RandomNameProfiles.Any(x => x.Culture == profile.Culture && x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				"There is already a random name profile for that name culture with that name. Names must be unique per name culture.");
			return;
		}

		var clone = new RandomNameProfile((RandomNameProfile)profile, name);
		actor.Gameworld.Add(clone);
		actor.OutputHandler.Send(
			$"You clone the {profile.Name.ColourName()} random name profile, creating a new one called {name.ColourName()}, which you are now editing.");
		actor.RemoveAllEffects<BuilderEditingEffect<IRandomNameProfile>>();
		actor.AddEffect(new BuilderEditingEffect<IRandomNameProfile>(actor) { EditingItem = clone });
	}

	#endregion

	#region Name Cultures

	private const string NameCultureHelp =
		@"Naming cultures are used by cultures to determine what valid names an individual can have. They contain rules about the number and type of names, as well as rules for how they are combined in different circumstances.

The correct syntax for this command is as follows:

	#3nameculture list#0 - lists all of the name cultures
	#3nameculture edit#0 <which> - begins editing a name culture
	#3nameculture edit new <name>#0 - begins editing a new name culture
	#3nameculture clone <old> <new>#0 - clones an existing name culture to a new one
	#3nameculture close#0 - stops editing a name culture
	#3nameculture show <which>#0 - views information about a name culture
	#3nameculture show#0 - views information about your currently editing name culture
	#3nameculture set name <name>#0 - renames the naming culture	
	#3nameculture set regex <regex>#0 - sets the regex for the naming culture
	#3nameculture set pattern <which> <pattern>#0 - sets the naming pattern for a particular context
	#3nameculture set element add <type>#0 - adds a new naming element of the specified type
	#3nameculture set element remove <type>#0 - removes a naming element of the specified type
	#3nameculture set element <type> name <name>#0 - renames an element
	#3nameculture set element <type> min <min>#0 - sets the minimum number of picks for an element
	#3nameculture set element <type> max <max>#0 - sets the maximum number of picks for an element
	#3nameculture set element <type> blurb#0 - drops into an editor for the chargen blurb

#6Note 1 - Regex Rules#0

For the regular expression, each of the name elements should be captured as a group, with the name being the string representation of the name element type (not the name). So for example a BirthName element type should be captured in a group like (?<birthname>...)

#6Note 2 - Patterns#0

In the pattern you use the text #3$ElementType#0 to refer to each of the elements. For example, #3$BirthName#0 for a Birth Name element.

You can also use a pattern in the form #3?ElementType[true][false]#0 to show the true or false text if the name has one or more of the specified element. 
For example, #3?Nickname[ a.k.a ""$Nickname""][]#0";

	[PlayerCommand("NameCulture", "nameculture", "nc")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("nameculture", NameCultureHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void NameCulture(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		switch (ss.PopSpeech())
		{
			case "list":
				NameCultureList(actor, ss);
				return;
			case "edit":
				NameCultureEdit(actor, ss);
				return;
			case "close":
				NameCultureClose(actor, ss);
				return;
			case "set":
				NameCultureSet(actor, ss);
				return;
			case "show":
			case "view":
				NameCultureShow(actor, ss);
				return;
			case "clone":
				NameCultureClone(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(NameCultureHelp.SubstituteANSIColour());
				return;
		}
	}

	private static void NameCultureClone(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which name culture do you want to clone?");
			return;
		}

		var nameCulture = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.NameCultures.Get(value)
			: actor.Gameworld.NameCultures.GetByName(ss.Last);
		if (nameCulture == null)
		{
			actor.OutputHandler.Send("There is no such name culture.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to the new name culture?");
			return;
		}

		var name = ss.SafeRemainingArgument.TitleCase();
		if (actor.Gameworld.NameCultures.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already a name culture with that name. Names must be unique.");
			return;
		}

		var clone = new NameCulture(nameCulture, name);
		actor.Gameworld.Add(clone);
		actor.OutputHandler.Send(
			$"You clone the {nameCulture.Name.ColourName()} name culture, creating a new one called {name.ColourName()}, which you are now editing.");
		actor.RemoveAllEffects<BuilderEditingEffect<INameCulture>>();
		actor.AddEffect(new BuilderEditingEffect<INameCulture>(actor) { EditingItem = clone });
	}

	private static void NameCultureShow(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<INameCulture>>().FirstOrDefault();
			if (effect == null)
			{
				actor.OutputHandler.Send("Which name culture would you like to view?");
				return;
			}

			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		var nameCulture = long.TryParse(ss.SafeRemainingArgument, out var value)
			? actor.Gameworld.NameCultures.Get(value)
			: actor.Gameworld.NameCultures.GetByName(ss.SafeRemainingArgument);
		if (nameCulture == null)
		{
			actor.OutputHandler.Send("There is no such name culture.");
			return;
		}

		actor.OutputHandler.Send(nameCulture.Show(actor));
	}

	private static void NameCultureSet(ICharacter actor, StringStack ss)
	{
		var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<INameCulture>>().FirstOrDefault();
		if (effect == null)
		{
			actor.OutputHandler.Send("You are not editing any name cultures.");
			return;
		}

		effect.EditingItem.BuildingCommand(actor, ss);
	}

	private static void NameCultureClose(ICharacter actor, StringStack ss)
	{
		var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<INameCulture>>().FirstOrDefault();
		if (effect == null)
		{
			actor.OutputHandler.Send("You are not editing any name cultures.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<INameCulture>>();
		actor.OutputHandler.Send("You are no longer editing any name cultures.");
	}

	private static void NameCultureEdit(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<INameCulture>>().FirstOrDefault();
			if (effect == null)
			{
				actor.OutputHandler.Send("Which name culture would you like to edit?");
				return;
			}

			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		if (ss.PeekSpeech().EqualTo("new"))
		{
			ss.PopSpeech();
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("What name do you want to give to your new name culture?");
				return;
			}

			var name = ss.SafeRemainingArgument.TitleCase();
			if (actor.Gameworld.NameCultures.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send("There is already a name culture with that name. Names must be unique.");
				return;
			}

			var newName = new NameCulture(actor.Gameworld, name);
			actor.Gameworld.Add(newName);
			actor.OutputHandler.Send(
				$"You create a new name culture called {name.ColourName()}, which you are now editing.");
			actor.RemoveAllEffects<BuilderEditingEffect<INameCulture>>();
			actor.AddEffect(new BuilderEditingEffect<INameCulture>(actor) { EditingItem = newName });
		}

		var nameCulture = long.TryParse(ss.SafeRemainingArgument, out var value)
			? actor.Gameworld.NameCultures.Get(value)
			: actor.Gameworld.NameCultures.GetByName(ss.SafeRemainingArgument);
		if (nameCulture == null)
		{
			actor.OutputHandler.Send("There is no such name culture.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<INameCulture>>();
		actor.AddEffect(new BuilderEditingEffect<INameCulture>(actor) { EditingItem = nameCulture });
		actor.OutputHandler.Send($"You are now editing the name culture {nameCulture.Name.ColourName()}.");
	}

	private static void NameCultureList(ICharacter actor, StringStack ss)
	{
		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from culture in actor.Gameworld.NameCultures
			select new List<string>
			{
				culture.Id.ToString("N0", actor),
				culture.Name,
				actor.Gameworld.Cultures
					 .Where(x =>
						 x.NameCultureForGender(Gender.Male) == culture ||
						 x.NameCultureForGender(Gender.Female) == culture ||
						 x.NameCultureForGender(Gender.NonBinary) == culture ||
						 x.NameCultureForGender(Gender.Neuter) == culture ||
						 x.NameCultureForGender(Gender.Indeterminate) == culture
					 )
					 .Select(x => x.Name)
					 .ListToString(),
				actor.Gameworld.Ethnicities
				     .Where(x =>
					     x.NameCultureForGender(Gender.Male) == culture ||
					     x.NameCultureForGender(Gender.Female) == culture ||
					     x.NameCultureForGender(Gender.NonBinary) == culture ||
					     x.NameCultureForGender(Gender.Neuter) == culture ||
					     x.NameCultureForGender(Gender.Indeterminate) == culture
				     )
				     .Select(x => x.Name)
				     .ListToString(),
				actor.Gameworld.RandomNameProfiles.Count(x => x.Culture == culture).ToStringN0(actor)
			},
			new List<string>
			{
				"Id",
				"Name",
				"Cultures",
				"Ethnicities",
				"# Profiles"
			},
			actor.LineFormatLength,
			colour: Telnet.Green,
			unicodeTable: actor.Account.UseUnicode,
			truncatableColumnIndex: 2
		));
	}

	#endregion

	#region Cultures

	private const string CultureHelp =
		@"Cultures reflect the way in which a character was raised - the society (or lack therefor) in which they grew up.

The correct syntax for this command is as follows:

	#3culture list#0 - lists all of the cultures
	#3culture edit <which>#0 - begins editing a culture
	#3culture edit new <name>#0 - begins editing a new culture
	#3culture clone <old> <new>#0 - clones an existing culture to a new one
	#3culture close#0 - stops editing a culture
	#3culture show <which>#0 - views information about a culture
	#3culture show#0 - views information about your currently editing culture
	#3culture set name <name>#0 - renames this culture
	#3culture set calendar <which>#0 - changes the calender used by this culture
	#3culture set nameculture <which> [all|male|female|neuter|nb|indeterminate]#0 - changes the name culture used by this culture
	#3culture set male|female|neuter|indeterminate <word>#0 - changes the gendered words for someone of this culture
	#3culture set desc#0 - drops you into an editor for the culture description
	#3culture set availability <prog>#0 - sets a prog that controls appearance in character creation
	#3culture set tempfloor <amount>#0 - sets the tolerable temperature floor modifier
	#3culture set tempceiling <amount>#0 - sets the tolerable temperature ceiling modifier
	#3culture set advice <which>#0 - toggles a chargen advice applying to this culture
	#3culture set cost <resource> <amount>#0 - sets a cost for character creation
	#3culture set require <resource> <amount>#0 - sets a non-cost requirement for character creation
	#3culture set cost <resource> clear#0 - clears a resource cost for character creation
	#3culture set skill <prog>#0 - sets the prog that controls skill starting values";

	[PlayerCommand("Culture", "culture")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	protected static void Culture(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		switch (ss.PopSpeech())
		{
			case "list":
				CultureList(actor, ss);
				return;
			case "edit":
				CultureEdit(actor, ss);
				return;
			case "close":
				CultureClose(actor, ss);
				return;
			case "set":
				CultureSet(actor, ss);
				return;
			case "show":
			case "view":
				CultureShow(actor, ss);
				return;
			case "clone":
				CultureClone(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(CultureHelp.SubstituteANSIColour());
				return;
		}
	}

	private static void CultureClone(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which culture do you want to clone?");
			return;
		}

		var culture = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.Cultures.Get(value)
			: actor.Gameworld.Cultures.GetByName(ss.Last);
		if (culture == null)
		{
			actor.OutputHandler.Send("There is no such culture.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to the new culture?");
			return;
		}

		var name = ss.SafeRemainingArgument.TitleCase();
		if (actor.Gameworld.Cultures.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already a culture with that name. Names must be unique.");
			return;
		}

		var clone = new Culture(culture, name);
		actor.Gameworld.Add(clone);
		actor.OutputHandler.Send(
			$"You clone the {culture.Name.ColourName()} culture, creating a new one called {name.ColourName()}, which you are now editing.");
		actor.RemoveAllEffects<BuilderEditingEffect<ICulture>>();
		actor.AddEffect(new BuilderEditingEffect<ICulture>(actor) { EditingItem = clone });
	}

	private static void CultureShow(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<ICulture>>().FirstOrDefault();
			if (effect == null)
			{
				actor.OutputHandler.Send("Which culture would you like to view?");
				return;
			}

			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		var culture = long.TryParse(ss.SafeRemainingArgument, out var value)
			? actor.Gameworld.Cultures.Get(value)
			: actor.Gameworld.Cultures.GetByName(ss.SafeRemainingArgument);
		if (culture == null)
		{
			actor.OutputHandler.Send("There is no such culture.");
			return;
		}

		actor.OutputHandler.Send(culture.Show(actor));
	}

	private static void CultureSet(ICharacter actor, StringStack ss)
	{
		var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<ICulture>>().FirstOrDefault();
		if (effect == null)
		{
			actor.OutputHandler.Send("You are not editing any cultures.");
			return;
		}

		effect.EditingItem.BuildingCommand(actor, ss);
	}

	private static void CultureClose(ICharacter actor, StringStack ss)
	{
		var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<ICulture>>().FirstOrDefault();
		if (effect == null)
		{
			actor.OutputHandler.Send("You are not editing any cultures.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<ICulture>>();
		actor.OutputHandler.Send("You are no longer editing any cultures.");
	}

	private static void CultureEdit(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<ICulture>>().FirstOrDefault();
			if (effect == null)
			{
				actor.OutputHandler.Send("Which culture would you like to edit?");
				return;
			}

			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		if (ss.PeekSpeech().EqualTo("new"))
		{
			ss.PopSpeech();
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("What name do you want to give to your new culture?");
				return;
			}

			var name = ss.SafeRemainingArgument.TitleCase();
			if (actor.Gameworld.Cultures.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send("There is already a culture with that name. Names must be unique.");
				return;
			}

			var newCulture = new Culture(actor.Gameworld, name);
			actor.Gameworld.Add(newCulture);
			actor.OutputHandler.Send(
				$"You create a new culture called {name.ColourName()}, which you are now editing.");
			actor.RemoveAllEffects<BuilderEditingEffect<ICulture>>();
			actor.AddEffect(new BuilderEditingEffect<ICulture>(actor) { EditingItem = newCulture });
			return;
		}

		var culture = long.TryParse(ss.SafeRemainingArgument, out var value)
			? actor.Gameworld.Cultures.Get(value)
			: actor.Gameworld.Cultures.GetByName(ss.SafeRemainingArgument);
		if (culture == null)
		{
			actor.OutputHandler.Send("There is no such culture.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<ICulture>>();
		actor.AddEffect(new BuilderEditingEffect<ICulture>(actor) { EditingItem = culture });
		actor.OutputHandler.Send($"You are now editing the culture {culture.Name.ColourName()}.");
	}

	private static void CultureList(ICharacter actor, StringStack ss)
	{
		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from culture in actor.Gameworld.Cultures
			select new List<string>
			{
				culture.Id.ToString("N0", actor),
				culture.Name,
				new[]
				{
					culture.NameCultureForGender(Gender.Male),
					culture.NameCultureForGender(Gender.Female),
					culture.NameCultureForGender(Gender.Neuter),
					culture.NameCultureForGender(Gender.NonBinary),
					culture.NameCultureForGender(Gender.Indeterminate)
				}.Distinct().Select(x => x.Name.TitleCase()).ListToCommaSeparatedValues(", "),
				culture.PrimaryCalendar.ShortName,
				culture.SkillStartingValueProg.MXPClickableFunctionName(),
				culture.AvailabilityProg?.MXPClickableFunctionName() ?? "None"
			},
			new List<string>
			{
				"Id",
				"Name",
				"Name Culture",
				"Calendar",
				"Starting Skills",
				"Availability"
			},
			actor.LineFormatLength,
			colour: Telnet.Green,
			unicodeTable: actor.Account.UseUnicode,
			truncatableColumnIndex: 2
		));
	}

	#endregion

	#region Ethnicities

	private const string EthnicityHelp =
		@"Ethnicities represent some variation of a race, and control things like the normal range of character variables (skin colour, hair colour, eye colour etc) and a few other properties. Ethnicities can also be used to represent breeds with animal races.

See the closely related command for building races.

The correct syntax for this command is as follows:

	#3ethnicity list#0 - lists all of the ethnicities
	#3ethnicity list <race>#0 - lists all the ethnicities for a particular race
	#3ethnicity edit <which>#0 - begins editing a ethnicity
	#3ethnicity edit new <name> <race>#0 - begins editing a new ethnicity
	#3ethnicity clone <old> <new>#0 - clones an existing ethnicity to a new one
	#3ethnicity close#0 - stops editing a ethnicity
	#3ethnicity show <which>#0 - views information about a ethnicity
	#3ethnicity show#0 - views information about your currently editing ethnicity
	#3ethnicity set name <name>#0 - renames this ethnicity
	#3ethnicity set group <group>#0 - sets an ethnic group
	#3ethnicity set group clear#0 - clears an ethnic group
	#3ethnicity set subgroup <group>#0 - sets an ethnic subgroup
	#3ethnicity set subgroup clear#0 - clears an ethnic subgroup
	#3ethnicity set desc#0 - drops you into an editor for the ethnicity description
	#3ethnicity set nameculture <which> [all|male|female|neuter|nb|indeterminate]#0 - changes the name culture used by this ethnicity
	#3ethnicity set nameculture none [all|male|female|neuter|nb|indeterminate]#0 - resets the name to not override its culture
	#3ethnicity set availability <prog>#0 - sets a prog that controls appearance in character creation
	#3ethnicity set bloodmodel <model>#0 - sets the population blood model for this ethnicity
	#3ethnicity set tempfloor <amount>#0 - sets the tolerable temperature floor modifier
	#3ethnicity set tempceiling <amount>#0 - sets the tolerable temperature ceiling modifier
	#3ethnicity set characteristic <which> <profile>#0 - sets the characteristic profile for this ethnicity
	#3ethnicity set advice <which>#0 - toggles a chargen advice applying to this ethnicity
	#3ethnicity set cost <resource> <amount>#0 - sets a cost for character creation
	#3ethnicity set require <resource> <amount>#0 - sets a non-cost requirement for character creation
	#3ethnicity set cost <resource> clear#0 - clears a resource cost for character creation";

	[PlayerCommand("Ethnicity", "ethnicity")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	protected static void Ethnicity(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		switch (ss.PopSpeech())
		{
			case "list":
				EthnicityList(actor, ss);
				return;
			case "edit":
				EthnicityEdit(actor, ss);
				return;
			case "close":
				EthnicityClose(actor, ss);
				return;
			case "set":
				EthnicitySet(actor, ss);
				return;
			case "show":
			case "view":
				EthnicityShow(actor, ss);
				return;
			case "clone":
				EthnicityClone(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(EthnicityHelp.SubstituteANSIColour());
				return;
		}
	}

	private static void EthnicityClone(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which ethnicity do you want to clone?");
			return;
		}

		var ethnicity = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.Ethnicities.Get(value)
			: actor.Gameworld.Ethnicities.GetByName(ss.Last);
		if (ethnicity == null)
		{
			actor.OutputHandler.Send("There is no such ethnicity.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to the new ethnicity?");
			return;
		}

		var name = ss.SafeRemainingArgument.TitleCase();
		if (actor.Gameworld.Ethnicities.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already an ethnicity with that name. Names must be unique.");
			return;
		}

		var clone = new Ethnicity(ethnicity, name);
		actor.Gameworld.Add(clone);
		actor.OutputHandler.Send(
			$"You clone the {ethnicity.Name.ColourName()} ethnicity, creating a new one called {name.ColourName()}, which you are now editing.");
		actor.RemoveAllEffects<BuilderEditingEffect<IEthnicity>>();
		actor.AddEffect(new BuilderEditingEffect<IEthnicity>(actor) { EditingItem = clone });
	}

	private static void EthnicityShow(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IEthnicity>>().FirstOrDefault();
			if (effect == null)
			{
				actor.OutputHandler.Send("Which ethnicity would you like to view?");
				return;
			}

			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		var ethnicity = long.TryParse(ss.SafeRemainingArgument, out var value)
			? actor.Gameworld.Ethnicities.Get(value)
			: actor.Gameworld.Ethnicities.GetByName(ss.SafeRemainingArgument);
		if (ethnicity == null)
		{
			actor.OutputHandler.Send("There is no such ethnicity.");
			return;
		}

		actor.OutputHandler.Send(ethnicity.Show(actor));
	}

	private static void EthnicitySet(ICharacter actor, StringStack ss)
	{
		var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IEthnicity>>().FirstOrDefault();
		if (effect == null)
		{
			actor.OutputHandler.Send("You are not editing any ethnicities.");
			return;
		}

		effect.EditingItem.BuildingCommand(actor, ss);
	}

	private static void EthnicityClose(ICharacter actor, StringStack ss)
	{
		var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IEthnicity>>().FirstOrDefault();
		if (effect == null)
		{
			actor.OutputHandler.Send("You are not editing any ethnicities.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<IEthnicity>>();
		actor.OutputHandler.Send("You are no longer editing any ethnicities.");
	}

	private static void EthnicityEdit(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IEthnicity>>().FirstOrDefault();
			if (effect == null)
			{
				actor.OutputHandler.Send("Which ethnicity would you like to edit?");
				return;
			}

			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		if (ss.PeekSpeech().EqualTo("new"))
		{
			ss.PopSpeech();
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("What name do you want to give to your new ethnicity?");
				return;
			}

			var name = ss.PopSpeech().TitleCase();

			if (ss.IsFinished)
			{
				actor.OutputHandler.Send($"You must specify a race for this ethnicity to apply to.");
				return;
			}

			var race = long.TryParse(ss.SafeRemainingArgument, out var raceid)
				? actor.Gameworld.Races.Get(raceid)
				: actor.Gameworld.Races.GetByName(ss.SafeRemainingArgument);
			if (race == null)
			{
				actor.OutputHandler.Send("There is no such race.");
				return;
			}


			if (actor.Gameworld.Ethnicities.Any(x => x.ParentRace == race && x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					$"There is already an ethnicity for the {race.Name.ColourName()} race with that name. Names must be unique.");
				return;
			}

			var newEthnicity = new Ethnicity(actor.Gameworld, race, name);
			actor.Gameworld.Add(newEthnicity);
			actor.OutputHandler.Send(
				$"You create a new ethnicity called {name.ColourName()}, which you are now editing.");
			actor.RemoveAllEffects<BuilderEditingEffect<IEthnicity>>();
			actor.AddEffect(new BuilderEditingEffect<IEthnicity>(actor) { EditingItem = newEthnicity });
			return;
		}

		var ethnicity = long.TryParse(ss.SafeRemainingArgument, out var value)
			? actor.Gameworld.Ethnicities.Get(value)
			: actor.Gameworld.Ethnicities.GetByName(ss.SafeRemainingArgument);
		if (ethnicity == null)
		{
			actor.OutputHandler.Send("There is no such ethnicity.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<IEthnicity>>();
		actor.AddEffect(new BuilderEditingEffect<IEthnicity>(actor) { EditingItem = ethnicity });
		actor.OutputHandler.Send($"You are now editing the ethnicity {ethnicity.Name.ColourName()}.");
	}

	private static void EthnicityList(ICharacter actor, StringStack ss)
	{
		var ethnicities = actor.Gameworld.Ethnicities.ToList();
		if (!ss.IsFinished)
		{
			var race = long.TryParse(ss.SafeRemainingArgument, out var raceid)
				? actor.Gameworld.Races.Get(raceid)
				: actor.Gameworld.Races.GetByName(ss.SafeRemainingArgument);
			if (race == null)
			{
				actor.OutputHandler.Send("There is no such race.");
				return;
			}

			ethnicities = ethnicities.Where(x => race.SameRace(x.ParentRace)).ToList();
		}

		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from ethnicity in ethnicities
			select new List<string>
			{
				ethnicity.Id.ToString("N0", actor),
				ethnicity.Name,
				ethnicity.ParentRace?.Name ?? "",
				ethnicity.EthnicGroup ?? "",
				ethnicity.EthnicSubgroup ?? "",
				ethnicity.PopulationBloodModel?.Name ?? "",
				ethnicity.AvailabilityProg?.MXPClickableFunctionName() ?? "None",
				ethnicity.NameCultures.Distinct().Where(x => x is not null).Select(x => x.Name).ListToString()
			},
			new List<string>
			{
				"Id",
				"Name",
				"Race",
				"Group",
				"Subgroup",
				"Blood Model",
				"Availability",
				"Name Cultures"
			},
			actor.LineFormatLength,
			colour: Telnet.Green,
			unicodeTable: actor.Account.UseUnicode,
			truncatableColumnIndex: 2
		));
	}

	#endregion

	#region Races

	public const string RaceHelp =
		@"The race command is used to view, edit and create races. All characters have a race, and the race represents something approximating a species (although you can make a more broad paradigm and use ethnicity to control species).

Races are typically very tightly coupled with ethnicities, so you should also see the helpfile for ethnicities.

The syntax to use this command is as follows:

	#3race list#0 - lists all of the races
	#3race edit <which>#0 - begins editing a race
	#3race edit new <name> <bodyproto> [<parent race>]#0 - begins editing a new race
	#3race clone <old> <new>#0 - clones an existing race to a new one
	#3race close#0 - stops editing a race
	#3race show <which>#0 - views information about a race
	#3race show#0 - views information about your currently editing race

Additionally, there are numerous properties that can be set:

	#6Core Properties#0

	#3race set name <name>#0 - renames the race
	#3race set desc#0 - drops you into an editor to describe the race
	#3race set parent <race>#0 - sets a parent race for this race
	#3race set parent none#0 - clears a parent race from this race
	#3race set body <template>#0 - changes the body template of the race
	#3race set parthealth <%>#0 - sets a multiplier for bodypart HPs
	#3race set partsize <##>#0 - sets a number of steps bigger/smaller for bodyparts

	#6Chargen Properties#0

	#3race set chargen <prog>#0 - sets a prog that controls chargen availability
	#3race set advice <which>#0 - toggles a chargen advice applying to this race
	#3race set cost <resource> <amount>#0 - sets a cost for character creation
	#3race set require <resource> <amount>#0 - sets a non-cost requirement for character creation
	#3race set cost <resource> clear#0 - clears a resource cost for character creation	

	#6Combat Properties#0

	#3race set armour <type>#0 - sets the natural armour type for this race
	#3race set armour none#0 - clears the natural armour type
	#3race set armourquality <quality>#0 - sets the quality of the natural armour
	#3race set armourmaterial <material>#0 - sets the default material for the race's natural armour
	#3race set canattack#0 - toggles the race being able to use attacks
	#3race set candefend#0 - toggles the race being able to dodge/parry/block
	#3race set canuseweapons#0 - toggles the race being able to use weapons (if it has wielding parts)

	#6Physical Properties#0

	#3race set age <category> <minimum>#0 - sets the minimum age for a specified age category
	#3race set variable all <characteristic>#0 - adds or sets a specified characteristic for all genders
	#3race set variable male <characteristic>#0 - adds or sets a specified characteristic for males only
	#3race set variable female <characteristic>#0 - adds or sets a specified characteristic for females only
	#3race set variable remove <characteristic>#0 - removes a characteristic from this race
	#3race set variable promote <characteristic>#0 - pushes a characteristic up to the parent race
	#3race set variable demote <characteristic>#0 - pushes a characteristic down to all child races (and remove from this)
	#3race set attribute <which>#0 - toggles this race having the specified attribute
	#3race set attribute promote <which>#0 - pushes this attribute up to the parent race
	#3race set attribute demote <which>#0 - pushes this attribute down to all child races (and remove from this)
	#3race set roll <dice>#0 - the dice roll expression (#6xdy+z#0) for attributes for this race
	#3race set cap <number>#0 - the total cap on the sum of attributes for this race
	#3race set bonusprog <which>#0 - sets the prog that controls attribute bonuses
	#3race set corpse <model>#0 - changes the corpse model of the race
	#3race set health <model>#0 - changes the health mode of the race
	#3race set perception <%>#0 - sets the light-percetion multiplier of the race (higher is better)
	#3race set genders <list of genders>#0 - sets the allowable genders for this race
	#3race set butcher <profile>#0 - sets a butchery profile for this race
	#3race set butcher none#0 - clears a butchery profile from this race
	#3race set breathing nonbreather|simple|lung|gill|blowhole#0 - sets the breathing model
	#3race set breathingrate <volume per minute>#0 - sets the volume of breathing per minute
	#3race set holdbreath <seconds expression>#0 - sets the formula for breathe-holding length
	#3race set sweat <liquid>#0 - sets the race's sweat liquid
	#3race set sweat none#0 - disables sweating for this race
	#3race set sweatrate <volume per minute>#0 - sets the volume of sweating per minute
	#3race set blood <liquid>#0 - sets the race's blood liquid
	#3race set blood none#0 - disables bleeding for this race
	#3race set bloodmodel <model>#0 - sets the blood antigen typing model for this race
	#3race set bloodmodel none#0 - clears a blood antigen typing model from this race
	#3race set tempfloor <temperature>#0 - sets the base minimum tolerable temperature for this race
	#3race set tempceiling <temperature>#0 - sets the base maximum tolerable temperature for this race

	#6Eating Properties#0

	#3race set caneatcorpses#0 - toggles the race being able to eat corpses directly (without butchering)
	#3race set biteweight <weight>#0 - sets the amount of corpse weight eaten per bite
	#3race set material add <material>#0 - adds a material definition for corpse-eating
	#3race set material remove <material>#0 - removes a material as eligible for corpse-eating
	#3race set material alcohol|thirst|hunger|water|calories <amount>#0 - sets the per-kg nutrition for this material
	#3race set optinediblematerial#0 - toggles whether the race can only eat materials from the pre-defined list
	#3race set emotecorpse <emote>#0 - sets the emote for eating corpses. $0 is eater, $1 is corpse.
	#3race set yield#0 - tba";

	[PlayerCommand("Race", "race")]
	[HelpInfo("Race", RaceHelp, AutoHelp.HelpArgOrNoArg)]
	[CommandPermission(PermissionLevel.Admin)]
	protected static void Race(ICharacter actor, string input)
	{
		GenericBuildingCommand(actor, new StringStack(input.RemoveFirstWord()), EditableItemHelper.RaceHelper);
	}

	#endregion
}