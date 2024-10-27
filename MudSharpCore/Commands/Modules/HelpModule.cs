using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Editor;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Help;
using MudSharp.PerceptionEngine;

namespace MudSharp.Commands.Modules;

internal class HelpModule : Module<ICharacter>
{
	private HelpModule()
		: base("Help")
	{
		IsNecessary = true;
	}

	public static HelpModule Instance { get; } = new();

	private static void HEditNewPost(string text, IOutputHandler handler, object[] parameters)
	{
		var character = (ICharacter)parameters[0];
		using (new FMDB())
		{
			var dbhelpfile = new Models.Helpfile
			{
				Name = (string)parameters[1],
				Category = (string)parameters[2],
				Subcategory = (string)parameters[3]
			};
			dbhelpfile.TagLine = string.IsNullOrWhiteSpace((string)parameters[4])
				? "Tagline for helpfile " + dbhelpfile.Name
				: (string)parameters[4];
			dbhelpfile.Keywords = dbhelpfile.Name;
			dbhelpfile.LastEditedBy = character.Account.Name.Proper();
			dbhelpfile.LastEditedDate = DateTime.UtcNow;
			dbhelpfile.PublicText = text.SubstituteANSIColour();
			FMDB.Context.Helpfiles.Add(dbhelpfile);
			FMDB.Context.SaveChanges();
			character.Gameworld.Add(new Helpfile(dbhelpfile, character.Gameworld));
			handler.Send(
				$"You create helpfile {dbhelpfile.Name.TitleCase().Colour(Telnet.Green)} (ID {dbhelpfile.Id})");
		}
	}

	private static void HEditNewCancel(IOutputHandler handler, object[] parameters)
	{
		handler.Send("You decide not to create any helpfile after all.");
	}

	private static void HEditNew(ICharacter actor, StringStack command)
	{
		if (command.CountRemainingArguments() < 3)
		{
			actor.Send(StringUtilities.HMark + "Syntax: hedit new <name> <category> <subcategory> [<tag line>]");
			return;
		}

		var nameText = command.PopSpeech();
		var categoryText = command.PopSpeech();
		var subcategoryText = command.PopSpeech();

		if (actor.Gameworld.Helpfiles.Any(x => x.Name.Equals(nameText, StringComparison.InvariantCultureIgnoreCase)))
		{
			actor.Send("There is already a helpfile with that name. Helpfile names must be unique.");
			return;
		}

		actor.Send(
			"You are creating the helpfile {0} in Category {1}, Subcategory {2}. Enter your helpfile text in the editor below:",
			nameText.Proper().Colour(Telnet.Green), categoryText.TitleCase().Colour(Telnet.Green),
			subcategoryText.TitleCase().Colour(Telnet.Green));
		actor.EditorMode(HEditNewPost, HEditNewCancel, 1.0, null, EditorOptions.None,
			new object[] { actor, nameText, categoryText, subcategoryText, command.SafeRemainingArgument });
	}

	private static void HEditDelete(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which helpfile do you want to delete?");
			return;
		}

		var helpfile = GetHelpfile(actor, command, true);
		if (helpfile == null)
		{
			actor.OutputHandler.Send("There is no such helpfile.");
			return;
		}

		actor.OutputHandler.Send($"Are you sure you want to delete the {helpfile.Name.ColourName()} ({helpfile.Category.ColourValue()}/{helpfile.Subcategory.ColourValue()}) help file? This cannot be undone.\n{Accept.StandardAcceptPhrasing}");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			DescriptionString = $"Deleting the {helpfile.Name.ColourName()} ({helpfile.Category.ColourValue()}/{helpfile.Subcategory.ColourValue()}) help file",
			AcceptAction = text =>
			{
				helpfile.Delete();
				actor.Gameworld.Destroy(helpfile);
				actor.OutputHandler.Send($"You delete the {helpfile.Name.ColourName()} ({helpfile.Category.ColourValue()}/{helpfile.Subcategory.ColourValue()}) help file.");
			},
			RejectAction = text =>
			{
				actor.OutputHandler.Send($"You decide not to delete the {helpfile.Name.ColourName()} ({helpfile.Category.ColourValue()}/{helpfile.Subcategory.ColourValue()}) help file.");
			},
			ExpireAction = () =>
			{
				actor.OutputHandler.Send($"You decide not to delete the {helpfile.Name.ColourName()} ({helpfile.Category.ColourValue()}/{helpfile.Subcategory.ColourValue()}) help file.");
			},
			Keywords = new List<string>{ "delete", "help", "file", helpfile.Name}
		}), TimeSpan.FromSeconds(120));
	}

	private static void HEditName(ICharacter actor, StringStack command)
	{
		if (command.CountRemainingArguments() < 2)
		{
			actor.Send(StringUtilities.HMark + "Syntax: hedit name <helpfile> <newname>");
			return;
		}

		var helpfile = GetHelpfile(actor, command, false);
		if (helpfile == null)
		{
			actor.Send("There is no such helpfile for you to edit.");
			return;
		}

		var name = command.SafeRemainingArgument.Trim();
		name = name.Replace("\"", ""); //Don't let quote marks into file names under any circumstances. It gets awkward.

		if (actor.Gameworld.Helpfiles.Any(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)))
		{
			actor.Send("There is already a helpfile with that name. Helpfile names must be unique.");
			return;
		}

		actor.Send("You rename the {0} helpfile to {1}.", helpfile.Name.Proper().Colour(Telnet.Green),
			name.Proper().Colour(Telnet.Green));
		helpfile.GetEditableHelpfile.SetName(name);
		helpfile.GetEditableHelpfile.LastEditedBy = actor.Account.Name;
		helpfile.GetEditableHelpfile.LastEditedDate = DateTime.UtcNow;
	}

	private static void HEditKeywords(ICharacter actor, StringStack command)
	{
		if (command.CountRemainingArguments() < 2)
		{
			actor.Send(StringUtilities.HMark + "Syntax: hedit keywords <helpfile> <keywords>");
			return;
		}

		var helpfile = GetHelpfile(actor, command);
		if (helpfile == null)
		{
			actor.Send("There is no such helpfile for you to edit.");
			return;
		}

		helpfile.GetEditableHelpfile.Keywords.Clear();
		helpfile.GetEditableHelpfile.Keywords.AddRange(
			command.SafeRemainingArgument.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
			       .Select(x => x.ToLowerInvariant())
			       .Distinct());
		helpfile.GetEditableHelpfile.LastEditedBy = actor.Account.Name;
		helpfile.GetEditableHelpfile.LastEditedDate = DateTime.UtcNow;
		actor.Send("You change the keywords of helpfile {0} to {1}.", helpfile.Name.Proper().Colour(Telnet.Green),
			helpfile.Keywords.Select(x => x.Colour(Telnet.Green)).ListToString());
	}

	private static void HEditTagline(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which help file do you want to edit the tagline for?");
			return;
		}

		var helpfile = GetHelpfile(actor, command);
		if (helpfile == null)
		{
			actor.Send("There is no such helpfile for you to edit.");
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to set the tagline to for this new helpfile?");
			return;
		}

		helpfile.GetEditableHelpfile.TagLine = command.SafeRemainingArgument.NormaliseSpacing().ProperSentences();
		helpfile.GetEditableHelpfile.LastEditedBy = actor.Account.Name;
		helpfile.GetEditableHelpfile.LastEditedDate = DateTime.UtcNow;
		actor.Send("You change the tagline of helpfile {0} to \"{1}\".", helpfile.Name.Proper().Colour(Telnet.Green),
			helpfile.TagLine);
	}

	private static void HEditCategory(ICharacter actor, StringStack command)
	{
		if (command.CountRemainingArguments() < 2)
		{
			actor.Send(StringUtilities.HMark + "Syntax: hedit category <helpfile> <category>");
			return;
		}

		var helpfile = GetHelpfile(actor, command);
		if (helpfile == null)
		{
			actor.Send("There is no such helpfile for you to edit.");
			return;
		}

		helpfile.GetEditableHelpfile.Category = command.SafeRemainingArgument.Trim();
		helpfile.GetEditableHelpfile.LastEditedBy = actor.Account.Name;
		helpfile.GetEditableHelpfile.LastEditedDate = DateTime.UtcNow;
		actor.Send("You change the category of helpfile {0} to {1}.", helpfile.Name.Proper().Colour(Telnet.Green),
			helpfile.Category.Proper().Colour(Telnet.Green));
	}

	private static void HEditSubcategory(ICharacter actor, StringStack command)
	{
		if (command.CountRemainingArguments() < 2)
		{
			actor.Send(StringUtilities.HMark + "Syntax: hedit subcategory <helpfile> <subcategory>");
			return;
		}

		var helpfile = GetHelpfile(actor, command);
		if (helpfile == null)
		{
			actor.Send("There is no such helpfile for you to edit.");
			return;
		}

		helpfile.GetEditableHelpfile.Subcategory = command.SafeRemainingArgument.Trim();
		helpfile.GetEditableHelpfile.LastEditedBy = actor.Account.Name;
		helpfile.GetEditableHelpfile.LastEditedDate = DateTime.UtcNow;
		actor.Send("You change the subcategory of helpfile {0} to {1}.", helpfile.Name.Proper().Colour(Telnet.Green),
			helpfile.Subcategory.Proper().Colour(Telnet.Green));
	}

	private static void HEditProg(ICharacter actor, StringStack command)
	{
		if (command.CountRemainingArguments() < 2)
		{
			actor.Send(StringUtilities.HMark + "Syntax: hedit prog <helpfile> <prog ID/name>");
			actor.Send("           hedit prog <helpfile> clear");
			return;
		}

		var helpfile = GetHelpfile(actor, command);
		if (helpfile == null)
		{
			actor.Send("There is no such helpfile for you to edit.");
			return;
		}

		if (command.Pop().Equals("clear", StringComparison.InvariantCultureIgnoreCase))
		{
			if (helpfile.Rule == null)
			{
				actor.Send("Helpfile {0} does not have an availability prog to clear.",
					helpfile.Name.Proper().Colour(Telnet.Green));
				return;
			}

			helpfile.GetEditableHelpfile.Rule = null;
			actor.Send("You clear the availability rule from helpfile {0}. It is now fully public.",
				helpfile.Name.Proper().Colour(Telnet.Green));
			return;
		}

		var prog = long.TryParse(command.Last, out var value)
			? actor.Gameworld.FutureProgs.Get(value)
			: actor.Gameworld.FutureProgs.FirstOrDefault(
				x => x.FunctionName.Equals(command.Last, StringComparison.InvariantCultureIgnoreCase));
		if (prog == null)
		{
			actor.Send("There is no such prog.");
			return;
		}

		if (!prog.MatchesParameters(new[] { FutureProgVariableTypes.Toon }))
		{
			actor.Send("You may only use progs that take a single toon parameter.");
			return;
		}

		if (prog.ReturnType != FutureProgVariableTypes.Boolean)
		{
			actor.Send("You may only use progs that return a boolean.");
			return;
		}

		if (!string.IsNullOrEmpty(prog.CompileError))
		{
			actor.Send("That prog failed to compile and is thus not available.");
			return;
		}

		helpfile.GetEditableHelpfile.Rule = prog;
		helpfile.GetEditableHelpfile.LastEditedBy = actor.Account.Name;
		helpfile.GetEditableHelpfile.LastEditedDate = DateTime.UtcNow;
		actor.Send("You set the availability rule for helpfile {0} to {1}.",
			helpfile.Name.Proper().Colour(Telnet.Green),
			$"{prog.FunctionName.Colour(Telnet.Cyan)} (#{prog.Id})".FluentTagMXP("send",
				$"href='show futureprog {prog.Id}'"));
	}

	private static void HEditTextPost(string text, IOutputHandler handler, params object[] parameters)
	{
		var actor = (ICharacter)parameters[0];
		var helpfile = ((IHelpfile)parameters[1]).GetEditableHelpfile;
		helpfile.PublicText = text.SubstituteANSIColour();
		helpfile.LastEditedBy = actor.Account.Name;
		helpfile.LastEditedDate = DateTime.UtcNow;
		handler.Send(
			$"You change the text of helpfile {helpfile.Name.Proper().Colour(Telnet.Green)} to:\n\n{helpfile.PublicText.Wrap(80, "\t").NoWrap()}");
	}

	private static void HEditTextCancel(IOutputHandler handler, params object[] parameters)
	{
		var helpfile = (IHelpfile)parameters[1];
		handler.Send($"You decline to edit the text of helpfile {helpfile.Name.Proper().Colour(Telnet.Green)}.");
	}

	private static void HEditText(ICharacter actor, StringStack command)
	{
		if (command.CountRemainingArguments() < 1)
		{
			actor.Send(StringUtilities.HMark + "Syntax: hedit text <helpfile>");
			return;
		}

		var helpfile = GetHelpfile(actor, command, true);
		if (helpfile == null)
		{
			actor.Send("There is no such helpfile for you to edit.");
			return;
		}

		actor.Send("Replacing:\n\n{0}\nPlease enter the new text in the editor below.",
			helpfile.PublicText.Wrap(80, "\t").NoWrap());
		actor.EditorMode(HEditTextPost, HEditTextCancel, 1.0, null, EditorOptions.None,
			new object[] { actor, helpfile });
	}

	private static void HEditExtraTextAddPost(string text, IOutputHandler handler, object[] parameters)
	{
		var actor = (ICharacter)parameters[0];
		var helpfile = ((IHelpfile)parameters[1]).GetEditableHelpfile;
		var prog = (IFutureProg)parameters[2];
		helpfile.AdditionalTexts.Add(Tuple.Create(prog, text));
		helpfile.LastEditedBy = actor.Account.Name;
		helpfile.LastEditedDate = DateTime.UtcNow;
		handler.Send(
			$"You add an extra text to helpfile {helpfile.Name.Proper().Colour(Telnet.Green)} with prog {$"{prog.Name.Colour(Telnet.Cyan)} (#{prog.Id})".FluentTagMXP("send", $"href='show futureprog {prog.Id}'")} to:\n\n{text.Wrap(80, "\t").NoWrap()}");
	}

	private static void HEditExtraTextAddCancel(IOutputHandler handler, object[] parameters)
	{
		handler.Send("You decide not to add any new extra text to the helpfile.");
	}

	private static void HEditExtraTextAdd(ICharacter actor, StringStack command)
	{
		if (command.CountRemainingArguments() < 2)
		{
			actor.Send(StringUtilities.HMark + "Syntax: hedit extra add <helpfile> <prog id/name>");
			return;
		}

		var helpfile = GetHelpfile(actor, command);
		if (helpfile == null)
		{
			actor.Send("There is no such helpfile for you to edit.");
			return;
		}

		var prog = long.TryParse(command.Pop(), out var value)
			? actor.Gameworld.FutureProgs.Get(value)
			: actor.Gameworld.FutureProgs.FirstOrDefault(
				x => x.FunctionName.Equals(command.Last, StringComparison.InvariantCultureIgnoreCase));
		if (prog == null)
		{
			actor.Send("There is no such prog.");
			return;
		}

		if (!prog.MatchesParameters(new[] { FutureProgVariableTypes.Character }))
		{
			actor.Send("You may only use progs that take a single character parameter.");
			return;
		}

		if (prog.ReturnType != FutureProgVariableTypes.Boolean)
		{
			actor.Send("You may only use progs that return a boolean.");
			return;
		}

		if (!string.IsNullOrEmpty(prog.CompileError))
		{
			actor.Send("That prog failed to compile and is thus not available.");
			return;
		}

		actor.Send("Enter the extra text for this helpfile in the editor below.");
		actor.EditorMode(HEditExtraTextAddPost, HEditExtraTextAddCancel, 1.0, null, EditorOptions.None,
			new object[] { actor, helpfile, prog });
	}

	private static void HEditExtraTextRemove(ICharacter actor, StringStack command)
	{
		if (command.CountRemainingArguments() < 2)
		{
			actor.Send(StringUtilities.HMark + "Syntax: hedit extra remove <helpfile> <extra index>");
			return;
		}

		var helpfile = GetHelpfile(actor, command);
		if (helpfile == null)
		{
			actor.Send("There is no such helpfile for you to edit.");
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which extra text did you want to remove?");
			return;
		}

		if (!int.TryParse(command.PopSpeech(), out var index) || index < 1 || index > helpfile.AdditionalTexts.Count())
		{
			actor.OutputHandler.Send(
				$"You must enter a valid number between {1.ToString("N0", actor).ColourValue()} and {helpfile.AdditionalTexts.Count().ToString("N0", actor).ColourValue()}.");
			return;
		}

		actor.OutputHandler.Send(
			$"Are you sure you want to delete the {index.ToOrdinal().ColourValue()} extra text (below)? This cannot be undone.\n\n{helpfile.AdditionalTexts.ElementAt(index - 1).Item2.Wrap(actor.InnerLineFormatLength, "\t")}\n\n{Accept.StandardAcceptPhrasing}");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			DescriptionString = $"Deleting the {index.ToOrdinal().ColourValue()} extra text of help file {helpfile.Name.ColourName()}",
			AcceptAction = text =>
			{
				helpfile.DeleteExtraText(index - 1);
				actor.OutputHandler.Send("You delete the extra text.");
			},
			RejectAction = text =>
			{
				actor.OutputHandler.Send("You decide not to delete the helpfile's extra text.");
			},
			ExpireAction = () =>
			{
				actor.OutputHandler.Send("You decide not to delete the helpfile's extra text.");
			},
			Keywords = new List<string>{"delete", "help", "extra"}
		}), TimeSpan.FromSeconds(120));
	}

	private static void HEditExtraTextEditPost(string text, IOutputHandler handler, object[] parameters)
	{
		text = text.SubstituteANSIColour();
		var actor = (ICharacter)parameters[0];
		var helpfile = ((IHelpfile)parameters[1]).GetEditableHelpfile;
		var additionalText = (Tuple<IFutureProg, string>)parameters[2];
		var index = helpfile.AdditionalTexts.IndexOf(additionalText);
		helpfile.AdditionalTexts.Insert(index, Tuple.Create(additionalText.Item1, text));
		helpfile.AdditionalTexts.Remove(additionalText);
		helpfile.LastEditedBy = actor.Account.Name;
		helpfile.LastEditedDate = DateTime.UtcNow;
		handler.Send(string.Format(actor, "You change extra text #{0:N0} for helpfile {1} to:\n\n{2}", index + 1,
			helpfile.Name.Proper().Colour(Telnet.Green),
			text.Wrap(80, "\t").NoWrap()
		));
	}

	private static void HEditExtraTextEditCancel(IOutputHandler handler, object[] parameters)
	{
		handler.Send("You decide not to change the extra text.");
	}

	private static void HEditExtraTextEdit(ICharacter actor, StringStack command)
	{
		if (command.CountRemainingArguments() < 2)
		{
			actor.Send(StringUtilities.HMark + "Syntax: hedit extra text <helpfile> <extraindex>");
			actor.Send("           Enter 'hedit show <helpfile> to find <extraindex>");
			return;
		}

		var helpfile = GetHelpfile(actor, command);
		if (helpfile == null)
		{
			actor.Send("There is no such helpfile for you to edit.");
			return;
		}

		if (!int.TryParse(command.Pop(), out var value))
		{
			actor.Send("You must enter a valid number for the extra text to edit.");
			return;
		}

		var additionalText = helpfile.AdditionalTexts.ElementAtOrDefault(value - 1);
		if (additionalText == null)
		{
			actor.Send("There is no such numbered additional text.");
			return;
		}

		actor.Send("Enter the extra text for this helpfile in the editor below.");
		actor.Send("Replacing:\n{0}", additionalText.Item2.Wrap(80, "\t").NoWrap());
		actor.EditorMode(HEditExtraTextEditPost, HEditExtraTextEditCancel, 1.0, null, EditorOptions.None,
			new object[] { actor, helpfile, additionalText });
	}

	private static void HEditExtraTextProg(ICharacter actor, StringStack command)
	{
		if (command.CountRemainingArguments() < 3)
		{
			actor.Send(StringUtilities.HMark + "Syntax: hedit extra prog <helpfile> <extraindex> <prog id/name>");
			actor.Send("           Enter 'hedit show <helpfile> to find <extraindex>");
			return;
		}

		var helpfile = GetHelpfile(actor, command);
		if (helpfile == null)
		{
			actor.Send("There is no such helpfile for you to edit.");
			return;
		}

		if (!int.TryParse(command.Pop(), out var value))
		{
			actor.Send("You must enter a valid number for the extra text to edit.");
			return;
		}

		var additionalText = helpfile.AdditionalTexts.ElementAtOrDefault(value - 1);
		if (additionalText == null)
		{
			actor.Send("There is no such numbered additional text.");
			return;
		}

		var prog = long.TryParse(command.Pop(), out var progid)
			? actor.Gameworld.FutureProgs.Get(progid)
			: actor.Gameworld.FutureProgs.FirstOrDefault(
				x => x.FunctionName.Equals(command.Last, StringComparison.InvariantCultureIgnoreCase));
		if (prog == null)
		{
			actor.Send("There is no such prog.");
			return;
		}

		if (!prog.MatchesParameters(new[] { FutureProgVariableTypes.Character }))
		{
			actor.Send("You may only use progs that take a single character parameter.");
			return;
		}

		if (prog.ReturnType != FutureProgVariableTypes.Boolean)
		{
			actor.Send("You may only use progs that return a boolean.");
			return;
		}

		if (!string.IsNullOrEmpty(prog.CompileError))
		{
			actor.Send("That prog failed to compile and is thus not available.");
			return;
		}

		var ehelp = helpfile.GetEditableHelpfile;
		var index = ehelp.AdditionalTexts.IndexOf(additionalText);
		ehelp.AdditionalTexts.Insert(index, Tuple.Create(prog, additionalText.Item2));
		ehelp.AdditionalTexts.Remove(additionalText);
		ehelp.LastEditedBy = actor.Account.Name.Proper();
		ehelp.LastEditedDate = DateTime.UtcNow;
		actor.Send("You edit additional text {0:N0} of helpfile {1} to use prog {2}.", index + 1,
			helpfile.Name.Proper().Colour(Telnet.Green),
			$"{prog.FunctionName.Colour(Telnet.Cyan)} (#{prog.Id})".FluentTagMXP("send",
				$"href='show futureprog {prog.Id}'")
		);
	}

	private static void HEditExtraText(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(StringUtilities.HMark + "Subcommands of 'hedit extra' include: add, remove, move, text, prog");
			return;
		}

		switch (command.Pop().ToLowerInvariant())
		{
			case "add":
			case "new":
				HEditExtraTextAdd(actor, command);
				break;
			case "remove":
			case "delete":
				HEditExtraTextRemove(actor, command);
				break;
			case "text":
				HEditExtraTextEdit(actor, command);
				break;
			case "prog":
				HEditExtraTextProg(actor, command);
				break;
			default:
				actor.Send(
					StringUtilities.HMark + "Subcommands of 'hedit extra' include: add, remove, text, prog");
				return;
		}
	}

	private static void HEditView(ICharacter actor, StringStack command)
	{
		if (command.CountRemainingArguments() < 1)
		{
			actor.Send(StringUtilities.HMark + "Syntax: hedit show <helpfile>");
			return;
		}

		var helpfile = GetHelpfile(actor, command, true);
		if (helpfile == null)
		{
			actor.Send("There is no such helpfile to show you.");
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine(
			string.Format(actor, "Helpfile {0} (#{1:N0})", helpfile.Name.Proper(), helpfile.Id).Colour(Telnet.Cyan));
		sb.AppendLine();
		sb.Append(new[]
		{
			$"Category: {helpfile.Category.Proper().Colour(Telnet.Green)}",
			$"Subcategory: {helpfile.Subcategory.Proper().Colour(Telnet.Green)}"
		}.ArrangeStringsOntoLines(2, (uint)actor.Account.LineFormatLength));
		sb.AppendLine(
			$"Keywords: {(helpfile.Keywords.Any() ? helpfile.Keywords.Select(x => x.Colour(Telnet.Green)).ListToString() : "none".Colour(Telnet.Red))}");
		sb.AppendLine($"Tagline: {helpfile.TagLine}");
		sb.AppendLine(
			$"Prog: {(helpfile.Rule == null ? "none".Colour(Telnet.Red) : $"{helpfile.Rule.FunctionName.Colour(Telnet.Cyan)} (#{helpfile.Rule.Id})".FluentTagMXP("send", $"href='show futureprog {helpfile.Rule.Id}'"))}");
		sb.AppendLine();
		sb.AppendLine("Text:");
		sb.AppendLine();
		sb.AppendLine(helpfile.PublicText);
		var count = 1;
		foreach (var additional in helpfile.AdditionalTexts)
		{
			sb.AppendLine();
			sb.AppendLine(string.Format("Additional Text {1:N0} - Prog {0}",
				$"{additional.Item1.FunctionName.Colour(Telnet.Cyan)} (#{additional.Item1.Id})".FluentTagMXP("send",
					$"href='show futureprog {additional.Item1.Id}'"), count++));
			sb.AppendLine();
			sb.AppendLine(additional.Item2);
		}

		sb.AppendLine();
		sb.AppendLine(
			$"Last edited by {helpfile.LastEditedBy.Proper().Colour(Telnet.Green)} on {helpfile.LastEditedDate.GetLocalDateString(actor).Colour(Telnet.Green)}");
		actor.Send(sb.ToString());
	}

	// - useFullArgument - If true, will use the entirety of 'command' to search. If not, will attempt to parse
	//                     help file name either from between quote marks or as a single word.
	private static IHelpfile GetHelpfile(ICharacter actor, StringStack command, bool useFullArgument = false)
	{
		var helpName = useFullArgument ? command.SafeRemainingArgument : command.PopSpeech();
		var helpfile = actor.Gameworld.Helpfiles.GetByIdOrName(helpName);
		if (helpfile != null && !helpfile.CanView(actor))
		{
			return null;
		}

		return helpfile;
	}

	public const string HEditHelpText = @"This command allows you to create and edit help files.

The syntax for this is as follows:

	#3hedit list#0 - lists all helpfiles
	#3hedit show <helpfile>#0 - shows builder information about a helpfile
	#3hedit new <name> <category> <subcategory> [<tagline>]#0 - creates a new helpfile
	#3hedit delete <which>#0 - deletes a helpfile
	#3hedit name <helpfile> <newname>#0 - renames a helpfile
	#3hedit keywords <keywords separated by spaces>#0 - sets keywords for the helpfile for searching
	#3hedit tagline <helpile> <tagline>#0 - sets the tagline for a helpfile
	#3hedit category <category>#0 - sets the category of a helpfile
	#3hedit subcategory <subcategory#0 - sets the subcategory of a helpfile
	#3hedit prog <helpfile> <prog>#0 - sets a prog that controls if someone can see the helpfile
	#3hedit prog <helpfile> clear#0 - clears a prog from a helpfile (instead always visible)
	#3hedit text <helpfile#0 - drops you into an editor to edit the helpfile text
	#3hedit extra add <helpfile> <prog>#0 - drops you into an editor to add a new extra text
	#3hedit extra remove <helpfile> <##>#0 - removes the specified extra text
	#3hedit extra text <helpfile> <##>#0 - drops you into an editor to edit the text of an extra text
	#3hedit extra prog <##> <prog>#0 - changes the prog associated with an extra text";

	[PlayerCommand("Hedit", "hedit")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	[HelpInfo("hedit", HEditHelpText, AutoHelp.HelpArgOrNoArg)]
	protected static void HEdit(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());

		switch (ss.PopForSwitch())
		{
			case "list":
				HEditList(actor, ss);
				return;
			case "new":
			case "create":
				HEditNew(actor, ss);
				break;
			case "delete":
				HEditDelete(actor, ss);
				break;
			case "name":
				HEditName(actor, ss);
				break;
			case "keywords":
				HEditKeywords(actor, ss);
				break;
			case "tagline":
				HEditTagline(actor, ss);
				break;
			case "category":
				HEditCategory(actor, ss);
				break;
			case "subcategory":
				HEditSubcategory(actor, ss);
				break;
			case "prog":
				HEditProg(actor, ss);
				break;
			case "text":
				HEditText(actor, ss);
				break;
			case "extra":
				HEditExtraText(actor, ss);
				break;
			case "view":
			case "show":
				HEditView(actor, ss);
				break;
			default:
				actor.OutputHandler.Send(HEditHelpText.SubstituteANSIColour());
				return;
		}
	}

	private static void HEditList(ICharacter actor, StringStack ss)
	{
		var sb = new StringBuilder();
		var helpfiles = actor.Gameworld.Helpfiles.ToList();
		sb.AppendLine(StringUtilities.GetTextTable(
			from help in helpfiles
			select new List<string>
			{
				help.Name.TitleCase(),
				help.Category.TitleCase(),
				help.Subcategory.TitleCase(),
				help.Rule?.MXPClickableFunctionName() ?? "",
				help.TagLine
			},
			new List<string>
			{
				"Name",
				"Category",
				"Subcategory",
				"Prog",
				"Tagline"
			},
			actor,
			Telnet.Orange));
		actor.OutputHandler.Send(sb.ToString());
	}

	//Finds and echoes an exact match help file. Returns false if it was unable to find the exact match.
	// - actor is the character trying to read the help file
	// - helpFileName is the name of the helpfile to search
	// - requirePermission defaults to false under the assumption that permission to access any aspect of the 
	//   command calling into this function was already handled at the command level. This prevents situations
	//   where the actor has permission to use the command but maybe not the helpfile which would result in a
	//   blank response.
	public static bool HelpExactMatch(ICharacter actor, string helpFileName, bool requirePermission = false)
	{
		var exactMatch = actor.Gameworld.Helpfiles.FirstOrDefault(x =>
			(x.CanView(actor) || requirePermission == false) && x.Name.EqualTo(helpFileName));

		if (exactMatch == null)
		{
			return false;
		}

		var helpFilesMatched = new[] { exactMatch }.ToList();

		actor.OutputHandler.Send(helpFilesMatched.First().DisplayHelpFile(actor));

		return true;
	}

	[PlayerCommand("Help", "help")]
	protected static void Help(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		var allHelpfiles = actor.Gameworld.Helpfiles.Where(x => x.CanView(actor)).ToList();
		var builtInHelp = actor.Gameworld.RetrieveAppropriateCommandTree(actor).Commands.CommandHelpInfos.ToList();
		var categories = allHelpfiles
		                 .Select(x => x.Category.TitleCase())
		                 .Concat(["Commands"])
		                 .Distinct()
		                 .OrderBy(x => x)
		                 .ToList();
		if (ss.IsFinished)
		{
			// TODO - make this something the users could configure
			actor.OutputHandler.Send(string.Format(
				$@"Welcome to the Help System for {actor.Gameworld.Name.Colour(Telnet.Cyan)}

The help system contains information on a variety of topics that may be useful to you as you play the game. You can view a help file by using the {"help <topic>".Colour(Telnet.Yellow)} command, for example, {"help get".Colour(Telnet.Yellow)}, which would show the help file for the {"get".Colour(Telnet.Yellow)} command.

You can also type {"help on <category>".Colour(Telnet.Yellow)} to see a list of all help files within that category.

See {"help help".Colour(Telnet.Yellow)} to learn conventions common across help files.

The following is a list of all of the categories that exist for you to search:

{categories.ArrangeStringsOntoLines()}"

				));
			return;
		}

		var cmd = ss.PeekSpeech();

		//handle the 'help on <category>' syntax.
		if (string.Equals(cmd.ToLowerInvariant(), "on", StringComparison.InvariantCultureIgnoreCase))
		{
			ss.PopSpeech(); //Pop the 'on'
			cmd = ss.PopSpeech();
			var helpfiles =
				allHelpfiles
					.Where(x => x.Category.StartsWith(cmd, StringComparison.InvariantCultureIgnoreCase))
					.ToList();
			if (!cmd.EqualTo("commands") && !helpfiles.Any())
			{
				actor.OutputHandler.Send($"There are no help files in the {cmd.TitleCase().Colour(Telnet.Cyan)} category.");
				return;
			}

			var helpOutput = helpfiles
			                 .Select(x => (Name: x.Name, TagLine: x.TagLine, SubCategory: x.Subcategory, Keywords: x.Keywords.ListToString(separator: " ", conjunction: "")))
			                 .ToList();
			if (cmd.EqualTo("commands"))
			{
				helpOutput.AddRange(builtInHelp.Select(x => (Name: x.HelpName, $"Automatically generated helpfile for command", "Built-In", x.HelpName)));
			}

			helpOutput.Sort((x1,x2) => string.Compare(x1.Name, x2.Name, StringComparison.InvariantCultureIgnoreCase));
			actor.OutputHandler.Send($"There are the following help files in the {cmd.TitleCase().Colour(Telnet.Cyan)} category:\n\n{StringUtilities.GetTextTable(
				helpfiles.Select(
					x =>
						new[]
						{
							x.Name.TitleCase(),
							x.Subcategory.TitleCase(),
							x.TagLine.Proper(),
							x.Keywords.ListToString(separator: " ", conjunction: "")
						}),
				new[] { "Help File", "Subcategory", "Synopsis", "Keywords" }, actor)}"
			);
			return;
		}

		//Next handle 'help <exact match>' or 'help "<exact match>"'
		if (HelpExactMatch(actor, ss.RemainingArgument, true))
			//User was shown the exact matching helpfile.
		{
			return;
		}

		string helpfileName, categoryName;

		cmd = ss.PopSpeech();

		if (ss.IsFinished)
		{
			//This supports the 'help <file>' or 'help "<file>"' syntax.
			helpfileName = cmd;
			categoryName = string.Empty;
		}
		else
		{
			//This supports the 'help <category> <file>' or 'help "<category>" "<file>"' or 'help "<category>" <file> syntax.
			categoryName = cmd;
			helpfileName = ss.SafeRemainingArgument;
		}

		var exactMatch = actor.Gameworld.Helpfiles.FirstOrDefault(x => x.CanView(actor) &&
		                                                               (string.IsNullOrEmpty(categoryName) ||
		                                                                x.Category.StartsWith(categoryName,
			                                                                StringComparison
				                                                                .InvariantCultureIgnoreCase)) &&
		                                                               x.Name.EqualTo(helpfileName));

		var helpfilesMatched = exactMatch != null
			? new[] { exactMatch }.ToList()
			: actor.Gameworld.Helpfiles.Where(
				x => x.CanView(actor) &&
				     (string.IsNullOrEmpty(categoryName) ||
				      x.Category.StartsWith(categoryName, StringComparison.InvariantCultureIgnoreCase)) &&
				     x.Name.StartsWith(helpfileName, StringComparison.InvariantCultureIgnoreCase)
			).ToList();

		if (!helpfilesMatched.Any())
		{
			helpfilesMatched =
				actor.Gameworld.Helpfiles.Where(
					     x => x.Keywords.Any(y =>
						     y.StartsWith(helpfileName, StringComparison.Ordinal) && x.CanView(actor)))
				     .ToList();
			if (!helpfilesMatched.Any())
			{
				actor.OutputHandler.Send("No help files matched your query.");
				return;
			}
		}

		if (helpfilesMatched.Count > 1)
		{
			actor.OutputHandler.Send("Your query matched multiple help files:\n\n" +
			                         (from hf in helpfilesMatched
			                          select hf.Category + " - " + hf.Subcategory + " - " + hf.Name).ListToString
				                         (separator: "\n", conjunction: ""));
			return;
		}

		actor.OutputHandler.Send(helpfilesMatched.First().DisplayHelpFile(actor));
	}
}