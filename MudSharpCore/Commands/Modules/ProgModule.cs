using System;
using System.Linq;
using System.Text;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Editor;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using System.Diagnostics;
using MudSharp.Form.Shape;
using MudSharp.Construction;
using System.Collections.Generic;
using MudSharp.Accounts;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character.Heritage;
using MudSharp.Character.Name;
using MudSharp.CharacterCreation;
using MudSharp.Communication.Language;
using MudSharp.Community;
using MudSharp.GameItems;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Events.Hooks;
using MudSharp.FutureProg.Functions;
using MudSharp.FutureProg.Variables;
using MudSharp.Models;
using MudSharp.NPC.Templates;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Intervals;
using MudSharp.TimeAndDate.Time;
using System.Numerics;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MudSharp.Form.Material;

namespace MudSharp.Commands.Modules;

internal class ProgModule : Module<ICharacter>
{
	private ProgModule()
		: base("Prog")
	{
		IsNecessary = true;
	}

	public static ProgModule Instance { get; } = new();

	protected const string NewProgHelpText =
		@"The prog command is used to view, create and edit progs. Progs are used all over the code to provide customisable logic, and can also be used for 'scripting' systems that fill in gaps in functionality that the engine doesn't cover.

Progs in FutureMUD are strongly-typed and compiled - this means that the engine will check ahead of time whether a prog is valid. It also provides a substantial performance boost to the actual execution of the progs.

You can use the following commands to interact with progs:

	#3prog list#0 - this shows you all progs. It is the same as the #3show progs#0 command
	#3prog categories#0 - shows a list of categories that existing progs fall into
	#3prog categories <which>#0 - shows a list of all progs in the specified category
	#3prog show <which>#0 - shows you information about a prog. It is the same as the #3show prog <which>#0 command
	#3prog edit <which>#0 - begins to edit a prog
	#3prog edit close#0 - stops editing a prog
	#3prog edit new <name>#0 - creates and begins editing a new blank prog
	#3prog edit newevent <event> <name>#0 - creates and begins editing a new prog with parameters based on an event type
	#3prog compile#0 - forces all progs to recompile (if necessary for some reason)
	#3prog execute <prog> [<parameters>]#0 - manually executes a prog. You must be able to resolve the parameters.

The following commands are used to edit a prog:

	#3prog set name <name>#0 - sets a new name for this prog
	#3prog set category <category>#0 - changes the category of this prog
	#3prog set subcategory <category>#0 - changes the subcategory of this prog
	#3prog set comment#0 - drops you into an editor to enter an explanatory comment about this prog
	#3prog set anyparameters#0 - toggles this prog accepting any combination of parameters (and ignoring them)
	#3prog set return <type>#0 - changes the return type of this prog
	#3prog set static#0 - toggles this prog being static (same output regardless of input). Use with caution.
	#3prog set text#0 - drops you into an editor to set the content of the prog
	#3prog set append#0 - the same as above, except keeps existing text rather than clearing it.
	#3prog parameter add <name> <type>#0 - adds an input parameter to the prog
	#3prog parameter remove <name>#0 - deletes an input parameter from the prog
	#3prog paramater swap <par1> <par2>#0 - swaps the order of two parameters

There are also a few commands relating to prog help:

	#3prog help types#0 - shows all the variable types
	#3prog help type <type>#0 - shows detailed information about a variable type
	#3prog help functions#0 - shows a (very very long) list of all the in-built functions
	#3prog help function <which>#0 - shows detailed help about a particular function
	#3prog help collections#0 - shows a list of special collection functions
	#3prog help collection <which>#0 - shows help for a specific collection function
	#3prog help statements#0 - shows a list of the basic language syntax and statements

See also the closely related #6hook#0 and #6events#0 areas for some further supporting information.";


	[PlayerCommand("Prog", "prog")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("prog", NewProgHelpText, AutoHelp.HelpArgOrNoArg)]
	protected static void Prog(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());

		switch (ss.Pop().ToLowerInvariant())
		{
			case "edit":
				ProgEdit(actor, ss);
				break;
			case "clone":
				ProgClone(actor, ss);
				break;
			case "compile":
				ProgCompile(actor, ss);
				break;
			case "set":
				ProgSet(actor, ss);
				break;
			case "parameter":
				ProgParameter(actor, ss);
				break;
			case "execute":
				ProgExecute(actor, ss);
				break;
			case "delete":
				ProgDelete(actor, ss);
				break;
			case "list":
				ShowModule.Show_FutureProgs(actor, ss);
				break;
			case "show":
			case "view":
				ShowModule.Show_FutureProg(actor, ss);
				break;
			case "categories":
				ProgCategories(actor, ss);
				break;
			case "help":
				ProgHelp(actor, ss);
				break;
			default:
				actor.OutputHandler.Send(NewProgHelpText.SubstituteANSIColour());
				return;
		}
	}

	private static void ProgHelp(ICharacter actor, StringStack ss)
	{
		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "types":
				ProgHelpTypes(actor);
				return;
			case "type":
				ProgHelpType(actor, ss);
				return;
			case "statements":
				ProgHelpStatements(actor);
				return;
			case "statement":
				ProgHelpStatement(actor, ss);
				return;
			case "functions":
				ProgHelpFunctions(actor, ss);
				return;
			case "function":
				ProgHelpFunction(actor, ss);
				return;
			case "collections":
				ProgHelpCollections(actor, ss);
				return;
			case "collection":
				ProgHelpCollection(actor, ss);
				return;
		}

		actor.OutputHandler.Send(NewProgHelpText.SubstituteANSIColour());
	}

	public static string GetTextProgHelpStatement((string HelpText, string Related) help, string function, int linewidth, bool colour)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Prog Statement - {function.ToUpperInvariant()}".FluentColour(Telnet.Magenta, colour));
		sb.AppendLine();
		sb.AppendLine(help.HelpText.Wrap(linewidth).SubstituteANSIColour().StripANSIColour(!colour));
		if (!string.IsNullOrEmpty(help.Related))
		{
			sb.AppendLine();
			sb.AppendLine($"Related: {help.Related.FluentColour(Telnet.Cyan, colour)}");
		}
		return sb.ToString();
	}

	private static void ProgHelpStatement(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send($"Which statement would you like to see help for? See {"prog help statements".MXPSend("prog help statements")} for a list.");
			return;
		}

		if (!FutureProg.FutureProg.StatementHelpTexts.TryGetValue(ss.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send($"There is no such statement. See {"prog help statements".MXPSend("prog help statements")} for a list.");
			return;
		}

		actor.OutputHandler.Send(GetTextProgHelpStatement(value, ss.SafeRemainingArgument, actor.LineFormatLength, true));
	}

	public static string GetTextProgHelpCollection(CollectionExtensionFunctionCompilerInformation info, int linewidth, bool colour)
	{
		var sb = new StringBuilder();
		sb.AppendLine($@"#5Collection Extension Function - {info.FunctionName.ToUpperInvariant()}#0

Inner Function Type: {info.InnerFunctionReturnType.DescribeEnum().ColourName(colour)}
Return Type Info: {info.FunctionReturnInfo.ColourName(colour)}
");
		var line = info.FunctionHelp.Wrap(linewidth, "\t").FluentColourIncludingReset(Telnet.Yellow, colour)
					   .SubstituteANSIColour();
		sb.AppendLine(colour ? line : line.StripANSIColour());
		return sb.ToString();
	}

	private static void ProgHelpCollection(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send($"Which collection extension function would you like to see help for? See {"prog help collections".MXPSend("prog help collections")} for a list.");
			return;
		}

		var info = CollectionExtensionFunction.FunctionCompilerInformations.FirstOrDefault(x =>
			x.FunctionName.EqualTo(ss.SafeRemainingArgument));
		if (info is null)
		{
			actor.OutputHandler.Send($"There is no such collection extension type. See {"prog help collections".MXPSend("prog help collections")} for a list.");
			return;
		}

		actor.OutputHandler.Send(GetTextProgHelpCollection(info, actor.LineFormatLength, true));
	}

	public static string GetTextProgHelpCollections(int linewidth, bool unicode, bool colour)
	{
		var sb = new StringBuilder();
		sb.AppendLine("Help on Collection Functions".ColourName());
		sb.AppendLine();
		sb.AppendLine(@"These functions are accessed by doing something in the following form after a collection variable:

	#`156;220;254;@CollectionVariable#0.#`220;220;170;FunctionName#0(ItemVariableName#0, InnerFunction#0)#0

Where:
	
	#3CollectionVariable#0 is any variable or function returning a collection
	#3FunctionName#0 is the specific collection extension function you want to run (e.g. #`220;220;170;Any#0, #`220;220;170;Sum#0, etc)
	#3ItemVariableName#0 is a variable name that will be used inside the inner function to refer to each item in the collection
	#3InnerFunction#0 is a function (usually returning a Boolean or Number) that is run on each element in the collection

For example, if you had a #6Number Collection#0 called #3fibonacci#0 that contained the following items:

	#21#0, #21#0, #22#0, #23#0, #25#0, #28#0, #213#0

You could run:

	#`156;220;254;@fibonacci#0.#`220;220;170;Sum(#`156;220;254;number#0, #`156;220;254;@number#0)

And the result would be a number with the value of #233#0.".SubstituteANSIColour());
		sb.AppendLine();
		sb.AppendLine("Collection Functions:");
		sb.AppendLine(StringUtilities.GetTextTable(
			from item in CollectionExtensionFunction.FunctionCompilerInformations
			select new List<string>
			{
				item.FunctionName.ToUpperInvariant(),
				item.InnerFunctionReturnType.DescribeEnum(),
				item.FunctionReturnInfo
			},
			new List<string>
			{
				"Function",
				"Inner Function",
				"Return Type"
			},
			linewidth,
			colour,
			colour ? Telnet.BoldMagenta : null,
			1,
			unicode
		));

		return colour ? sb.ToString() : sb.ToString().StripANSIColour();
	}

	private static void ProgHelpCollections(ICharacter actor, StringStack ss)
	{
		actor.OutputHandler.Send(GetTextProgHelpCollections(actor.LineFormatLength, actor.Account.UseUnicode, true));
	}

	public static string GetTextProgHelpFunction(IEnumerable<FunctionCompilerInformation> functions, int linewidth, int innerwidth,
		bool unicode, bool colour)
	{
		var sb = new StringBuilder();
		foreach (var function in functions)
		{
			sb.AppendLine();
			sb.AppendLine(function.FunctionDisplayForm.StripANSIColour(!colour).GetLineWithTitle(
				linewidth,
				unicode, 
				colour ? Telnet.BoldMagenta : null, null));
			sb.AppendLine();
			sb.AppendLine(function.FunctionHelp.Wrap(innerwidth).FluentColour(Telnet.Yellow, colour));
			sb.AppendLine();
			for (var i = 0; i < function.Parameters.Count(); i++)
			{
				sb.Append("\t");
				sb.Append(function.Parameters.ElementAt(i).Describe().FluentColour(Telnet.Cyan, colour));
				sb.Append(" ");
				sb.Append(function.ParameterNames?.ElementAt(i) ?? $"var{i}");
				sb.Append(": ");
				sb.AppendLine(function.ParameterHelp?.ElementAt(i).FluentColour(Telnet.Yellow, colour) ??
							  "no help available".ColourError(colour));
			}

			sb.AppendLine();
		}

		return sb.ToString();
	}

	private static void ProgHelpFunction(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which function do you want to see help for?");
			return;
		}

		var which = ss.SafeRemainingArgument;
		var functions = FutureProg.FutureProg.GetFunctionCompilerInformations()
								  .Where(x => x.FunctionName.EqualTo(which))
								  .ToList();

		if (!functions.Any())
		{
			actor.OutputHandler.Send("There are no such functions. Please see PROG HELP FUNCTIONS for a list.");
			return;
		}

		actor.OutputHandler.Send(GetTextProgHelpFunction(functions, actor.LineFormatLength, actor.InnerLineFormatLength, actor.Account.UseUnicode, true));
	}

	public static string GetTextProgHelpFunctions(IEnumerable<FunctionCompilerInformation> infos, int linewidth, bool unicode, bool colour)
	{
		var sb = new StringBuilder();
		sb.AppendLine("There are the following built-in functions:");
		sb.AppendLine();
		foreach (var category in infos.GroupBy(x => x.Category))
		{
			sb.AppendLine();
			sb.AppendLine(category.Key.GetLineWithTitle(linewidth, unicode,
				colour ? Telnet.BoldGreen : null, colour ? Telnet.BoldYellow : null));
			sb.AppendLine();
			foreach (var function in category
									 .OrderBy(x => x.FunctionName)
									 .ThenBy(x => x.Parameters.Count())
									 .Select(x => x.FunctionDisplayForm))
			{
				sb.AppendLine($"\t{function}");
			}
		}

		return sb.ToString();
	}

	private static void ProgHelpFunctions(ICharacter actor, StringStack ss)
	{
		var infos = FutureProg.FutureProg.GetFunctionCompilerInformations().ToList();

		if (!ss.IsFinished)
		{
			var text = ss.SafeRemainingArgument;
			infos = infos.Where(x =>
				x.Category.EqualTo(text) ||
				x.Category.StartsWith(text, StringComparison.InvariantCultureIgnoreCase)).ToList();
		}

		actor.OutputHandler.Send(GetTextProgHelpFunctions(infos, actor.LineFormatLength, actor.Account.UseUnicode, true), nopage: true);
	}

	public static string GetTextProgHelpStatements(int linewidth, bool unicode, bool colour)
	{
		var sb = new StringBuilder();
		sb.AppendLine("Statement Help");
		sb.AppendLine();
		sb.AppendLine(@"Statements are lines within progs that perform certain critical tasks such as branching, returning, declaring variables and so on.

Statements are either line statements (the statement takes up one line in the code and does something) or block statements (the statement grabs subsequent lines in the code into a ""block"" and does something on the block.

A function (See PROG HELP FUNCTIONS) can also function as a statement on a line.");
		sb.AppendLine();
		sb.AppendLine($"There are the following statement types:");
		sb.AppendLine(StringUtilities.GetTextTable(
			from item in FutureProg.FutureProg.StatementHelpTexts
			select new List<string>
			{
				item.Key,
				item.Value.Related
			},
			new List<string>
			{
				"Statement",
				"Related Statements"
			},
			linewidth,
			colour,
			colour ? Telnet.BoldMagenta : null,
			1,
			unicode
		));

		return sb.ToString();
	}

	private static void ProgHelpStatements(ICharacter actor)
	{
		actor.OutputHandler.Send(GetTextProgHelpStatements(actor.LineFormatLength, actor.Account.UseUnicode, true));
	}

	public static string GetProgTypeHelpText(ProgVariableTypes type, int linewidth, bool unicode, bool colour)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Showing properties for the type {type.Describe().ColourName(colour)}:");
		var info = ProgVariable.DotReferenceCompileInfos.GetValueOrDefault(type, null);
		if (info == null)
		{
			return null;
		}

		sb.AppendLine(StringUtilities.GetTextTable(info.PropertyTypeMap.Keys.Select(x => new List<string>
			{
				x,
				info.PropertyTypeMap[x].Describe(),
				info.PropertyHelpInfo.GetValueOrDefault(x, string.Empty)
			}),
			new List<string>
			{
				"Property",
				"Return Type",
				"Help"
			}, 
			linewidth, 
			colour: colour ? Telnet.BoldMagenta : null,  
			truncatableColumnIndex: 2,
			unicodeTable: unicode));

		return sb.ToString();
	}

	private static void ProgHelpType(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What type do you want to see property help for?");
			return;
		}

		var which = ss.SafeRemainingArgument;
		var type = FutureProg.FutureProg.GetTypeByName(which);
		if (type == ProgVariableTypes.Error)
		{
			actor.OutputHandler.Send("There is no such type.");
			return;
		}

		var text = GetProgTypeHelpText(type, actor.LineFormatLength, actor.Account.UseUnicode, true);
		if (text is null)
		{
			actor.OutputHandler.Send($"The type {type.Describe().ColourName()} does not have any help.");
			return;
		}

		actor.OutputHandler.Send(text, nopage: true);
	}

	public static string GetTextProgHelpTypes(bool colour)
	{
		var sb = new StringBuilder();
		sb.AppendLine("There are the following types:");
		foreach (var type in ProgVariableTypes.Anything.GetAllFlags().OrderBy(x => x.DescribeEnum()))
		{
			if (type == ProgVariableTypes.Collection)
			{
				sb.AppendLine($"\t<OtherType> Collection".FluentColour(Telnet.VariableGreen, colour));
			}
			else if (type == ProgVariableTypes.CollectionDictionary)
			{
				sb.AppendLine($"\t<OtherType> CollectionDictionary".FluentColour(Telnet.VariableGreen, colour));
			}
			else if (type == ProgVariableTypes.Dictionary)
			{
				sb.AppendLine($"\t<OtherType> Dictionary".FluentColour(Telnet.VariableGreen, colour));
			}
			else
			{
				sb.AppendLine($"\t{type.DescribeEnum().FluentColour(Telnet.VariableGreen, colour)}");
			}
		}

		return sb.ToString();
	}

	private static void ProgHelpTypes(ICharacter actor)
	{
		actor.OutputHandler.Send(GetTextProgHelpTypes(true), nopage: true);
	}

	private static void ProgCategories(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"There are the following categories of progs: {actor.Gameworld.FutureProgs.Select(x => x.Category).Distinct().Select(x => x.ColourValue()).ListToString()}.");
			return;
		}

		var category = ss.PopSpeech();
		var progs = actor.Gameworld.FutureProgs
						 .Where(x => x.Category.StartsWith(category, StringComparison.InvariantCultureIgnoreCase))
						 .ToList();
		if (!progs.Any())
		{
			actor.OutputHandler.Send("There are no progs in that category.");
			return;
		}

		actor.OutputHandler.Send(
			$"There are the following subcategories in the {category.ColourValue()} category: {progs.Select(x => x.Subcategory).Distinct().Select(x => x.ColourValue()).ListToString()}.");
	}

	#region Prog Sub Commands

	public static string DescribeProgVariable(ICharacter actor, ProgVariableTypes returnType, object result)
	{
		if (result is null)
		{
			return "null";
		}

		switch (returnType)
		{
			case ProgVariableTypes.Boolean:
				return ((bool)result).ToColouredString();
			case ProgVariableTypes.Character:
				var ch = result as ICharacter;
				return ch?.HowSeen(actor) ?? "null".Colour(Telnet.Red);
			case ProgVariableTypes.Gender:
				return Gendering.Get((Gender)result).GenderClass(true).Colour(Telnet.Cyan);
			case ProgVariableTypes.Item:
				return (result as IGameItem)?.HowSeen(actor) ?? "null".Colour(Telnet.Red);
			case ProgVariableTypes.Location:
				if (result is ICell cell)
				{
					return cell.GetFriendlyReference(actor).ColourName();
				}
				else
				{
					return "a null location";
				}
			case ProgVariableTypes.Number:
				return ((decimal)result).ToString("N", actor).ColourValue();
			case ProgVariableTypes.Shard:
				if (result is IShard shard)
				{
					return $"{shard.Name.ColourName()} (#{shard.Id.ToString("N0", actor)})".ColourName();
				}
				else
				{
					return "a null shard";
				}
			case ProgVariableTypes.Text:
				return (string)result;
			case ProgVariableTypes.Zone:
				if (result is IZone zone)
				{
					return $"{zone.Name.ColourName()} (#{zone.Id.ToString("N0", actor)})".ColourName();
				}
				else
				{
					return "a null zone";
				}
			case ProgVariableTypes.TimeSpan:
				return ((TimeSpan)result).Describe(actor).ColourValue();
			case ProgVariableTypes.DateTime:
				return ((DateTime)result).ToString("G").ColourValue();
			case ProgVariableTypes.MudDateTime:
				return ((MudDateTime)result).ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue();
			case ProgVariableTypes.Toon:
				if (result is ICharacter)
				{
					goto case ProgVariableTypes.Character;
				}

				goto case ProgVariableTypes.Chargen;
			case ProgVariableTypes.Chargen:
				if (result is INPCTemplate npcTemplate)
				{
					return $" the NPC Template {npcTemplate.EditHeader().ColourCharacter()}";
				}

				var chargen = (IChargen)result;
				return $" chargen {$"{chargen.SelectedName?.GetName(NameStyle.FullName) ?? "Unnamed"} (#{chargen.Id.ToString("N0", actor)})".ColourCharacter()}.";
			case ProgVariableTypes.Race:
				return $"the {((IFrameworkItem)result).Name.ColourValue()} race";
			case ProgVariableTypes.Culture:
				return $"the {((IFrameworkItem)result).Name.ColourValue()} culture";
			case ProgVariableTypes.Trait:
				return $"the {((IFrameworkItem)result).Name.ColourValue()} trait";
			case ProgVariableTypes.Clan:
				return $"the {((IFrameworkItem)result).Name.ColourName()} clan";
			case ProgVariableTypes.ClanRank:
				var rank = (IRank)result;
				return $"the {rank.Name.ColourValue()} rank in the {rank.Clan.FullName.ColourName()} clan";
			case ProgVariableTypes.ClanAppointment:
				var appointment = (IAppointment)result;
				return $"the {appointment.Name.ColourValue()} appointment in the {appointment.Clan.FullName.ColourName()} clan";
			case ProgVariableTypes.ClanPaygrade:
				var paygrade = (IPaygrade)result;
				return $"the {paygrade.Name.ColourValue()} paygrade in the {paygrade.Clan.FullName.ColourName()} clan";
			case ProgVariableTypes.Currency:
				return $"the {((IFrameworkItem)result).Name.ColourValue()} currency";
			case ProgVariableTypes.Exit:
				break;
			case ProgVariableTypes.Language:
				return $"the {((IFrameworkItem)result).Name.ColourValue()} language";
			case ProgVariableTypes.Accent:
				var accent = (IAccent)result;
				return $"the {accent.Name.ColourValue()} accent for the {accent.Language.Name.ColourValue()} language";
			case ProgVariableTypes.Merit:
				return $"the {((IFrameworkItem)result).Name.ColourValue()} merit";
			case ProgVariableTypes.Calendar:
				return $"the {((IFrameworkItem)result).Name.ColourValue()} calendar";
			case ProgVariableTypes.Clock:
				return $"the {((IFrameworkItem)result).Name.ColourValue()} clock";
			case ProgVariableTypes.Knowledge:
				return $"the {((IFrameworkItem)result).Name.ColourValue()} knowledge";
			case ProgVariableTypes.Role:
				return $"the {((IFrameworkItem)result).Name.ColourValue()} chargen role";
			case ProgVariableTypes.Ethnicity:
				return $"the {((IFrameworkItem)result).Name.ColourValue()} ethnicity";
			case ProgVariableTypes.WeatherEvent:
				return $"the {((IFrameworkItem)result).Name.ColourValue()} weather event";
			case ProgVariableTypes.Shop:
				return $"the {((IFrameworkItem)result).Name.ColourValue()} shop";
			case ProgVariableTypes.Merchandise:
				return $"the {((IFrameworkItem)result).Name.ColourValue()} merchandise";
			case ProgVariableTypes.Outfit:
				break;
			case ProgVariableTypes.OutfitItem:
				break;
			case ProgVariableTypes.Project:
				return $"the {((IFrameworkItem)result).Name.ColourValue()} project";
			case ProgVariableTypes.OverlayPackage:
				return $"the {((IFrameworkItem)result).Name.ColourValue()} overlay package";
			case ProgVariableTypes.Terrain:
				return $"the {((IFrameworkItem)result).Name.ColourValue()} terrain";
			case ProgVariableTypes.Solid:
				return $"the {((IFrameworkItem)result).Name.ColourValue()} solid material";
			case ProgVariableTypes.Liquid:
				return $"the {((IFrameworkItem)result).Name.ColourValue()} liquid";
			case ProgVariableTypes.Gas:
				return $"the {((IFrameworkItem)result).Name.ColourValue()} gas";
			case ProgVariableTypes.MagicSpell:
				return $"the {((IFrameworkItem)result).Name.ColourValue()} spell";
			case ProgVariableTypes.MagicSchool:
				return $"the {((IFrameworkItem)result).Name.ColourValue()} magic school";
			case ProgVariableTypes.MagicCapability:
				return $"the {((IFrameworkItem)result).Name.ColourValue()} magic capability";
			case ProgVariableTypes.Bank:
				return $"the {((IFrameworkItem)result).Name.ColourValue()} bank";
			case ProgVariableTypes.BankAccount:
				return $"the {((IFrameworkItem)result).Name.ColourValue()} bank account";
			case ProgVariableTypes.BankAccountType:
				return $"the {((IFrameworkItem)result).Name.ColourValue()} bank account type";
			case ProgVariableTypes.LegalAuthority:
				return $"the {((IFrameworkItem)result).Name.ColourValue()} legal authority";

			case ProgVariableTypes.Law:
				return $"the {((IFrameworkItem)result).Name.ColourValue()} law";
			case ProgVariableTypes.Crime:
				return $"the #{((IFrameworkItem)result).Id.ToString("N0", actor).ColourValue()} crime";
			case ProgVariableTypes.Market:
				return $"the {((IFrameworkItem)result).Name.ColourValue()} market";
			case ProgVariableTypes.MarketCategory:
				return $"the {((IFrameworkItem)result).Name.ColourValue()} market category";
			case ProgVariableTypes.LiquidMixture:
				return $"the {((LiquidMixture)result).ColouredLiquidDescription} liquid mixture";
			case ProgVariableTypes.Script:
				return $"the {((IFrameworkItem)result).Name.ColourValue()} script";
			case ProgVariableTypes.Writing:
				var writing = (IWriting)result;
				return $"the writing {writing.DescribeInLook(actor)} (#{writing.Id.ToStringN0(actor)})";
			case ProgVariableTypes.Area:
				return $"the {((IFrameworkItem)result).Name.ColourValue()} area";
			case ProgVariableTypes.Perceivable:
				var perceivable = (IPerceivable)result;
				return perceivable.HowSeen(actor);
			case ProgVariableTypes.Perceiver:
				goto case ProgVariableTypes.Perceivable;
			case ProgVariableTypes.MagicResourceHaver:
				goto case ProgVariableTypes.Perceivable;
		}

		return "an undisplayable result";
	}

	private static void ProgExecute(ICharacter actor, StringStack ss)
	{
		var cmd = ss.PopSpeech();
		var prog = actor.Gameworld.FutureProgs.GetByIdOrName(cmd);

		if (prog is null)
		{
			actor.OutputHandler.Send("There is no such prog to execute.");
			return;
		}

		var parArray = new List<object>();
		for (var i =0; i < prog.NamedParameters.Count; i++)
		{
			var (arg, paramName) = prog.NamedParameters[i];
			cmd = ss.PopParentheses();
			if (string.IsNullOrEmpty(cmd))
			{
				cmd = ss.PopSpeech();
			}

			if (string.IsNullOrEmpty(cmd))
			{
				actor.OutputHandler.Send($"You must supply a value for the parameter at position {i.ToString("N0", actor).ColourValue()} - {arg.Describe().Colour(Telnet.VariableCyan)} {paramName.Colour(Telnet.VariableGreen)}.");
				return;
			}

			var (parResult, success) = GetArgument(arg, cmd, i, actor);
			if (!success)
			{
				return;
			}

			parArray.Add(parResult);
		}

		var sb = new StringBuilder();
		var text = $"Executing {prog.MXPClickableFunctionNameWithId()} with the following parameters:";
		sb.AppendLine(text);
		sb.AppendLine();
		for (var i =  0; i < parArray.Count; i++)
		{
			var parameter = parArray[i];
			var (parType, parName) = prog.NamedParameters[i];
			sb.AppendLine($"\t{parType.Describe().Colour(Telnet.VariableGreen)} {parName.Colour(Telnet.VariableCyan)}: {DescribeProgVariable(actor, parType, parameter)}");
		}
		sb.AppendLine();
		sb.AppendLine(new string('=', text.RawTextLength()));
		var sw = new Stopwatch();
		sw.Start();
		var result = prog.Execute(parArray.ToArray());
		sw.Stop();
		sb.AppendLine($"It returned {DescribeProgVariable(actor, prog.ReturnType, result)}.");
		sb.AppendLine($"It took approximately {(sw.ElapsedTicks * 100).ToString("N0", actor)} nanoseconds to execute.".Colour(Telnet.Yellow));
		actor.OutputHandler.Send(sb.ToString());
	}

	public static (object result, bool success) GetArgument(ProgVariableTypes type, string parText,
		int parNumber, ICharacter actor)
	{
		if (type.HasFlag(ProgVariableTypes.Collection))
		{
			var sss = new StringStack(parText);
			var collection = new List<object>();
			while (!sss.IsFinished)
			{
				var outcome = GetArgument(type ^ ProgVariableTypes.Collection, sss.PopSpeech(), parNumber, actor);
				if (!outcome.success)
				{
					return (null, false);
				}

				collection.Add(outcome.result);
			}

			return (collection, true);
		}

		if (ProgVariableTypes.ReferenceType.HasFlag(type) && parText.EqualTo("null"))
		{
			return (null, true);
		}

		var parameterArgument = parNumber > 0 ? $" at parameter {parNumber.ToString("N0", actor)}" : "";

		switch (type)
		{
			case ProgVariableTypes.Boolean:
				if (bool.TryParse(parText, out var bValue))
				{
					return (bValue, true);
				}

				actor.OutputHandler.Send($"That is not a valid boolean argument to use {parameterArgument}.");
				return (null, false);
			case ProgVariableTypes.Chargen:
				if (!long.TryParse(parText, out var id))
				{
					actor.OutputHandler.Send($"You must supply an ID number for the chargen you wish to use.");
					return (null, false);
				}

				using (new FMDB())
				{
					var dbitem = FMDB.Context.Chargens.Find(id);
					if (dbitem is null)
					{
						actor.OutputHandler.Send("There is no such chargen.");
						return (null, false);
					}

					return (new CharacterCreation.Chargen(dbitem, actor.Gameworld,dbitem.Account), true);
				}
			case ProgVariableTypes.Toon:
				if (parText[0] == '*')
				{
					parText = parText.Substring(1);
					goto case ProgVariableTypes.Chargen;
				}

				goto case ProgVariableTypes.Character;
			case ProgVariableTypes.Character:
				var targetActor = actor.TargetActor(parText);
				if (targetActor == null)
				{
					actor.OutputHandler.Send($"You do not see anybody like that here to target{parameterArgument}.");
					return (null, false);
				}

				return (targetActor, true);
			case ProgVariableTypes.Gender:
				switch (parText.ToLowerInvariant())
				{
					case "male":
						return (Gender.Male, true);
					case "female":
						return (Gender.Female, true);
					case "neuter":
						return (Gender.Neuter, true);
					case "non-binary":
					case "nb":
					case "nonbinary":
						return (Gender.NonBinary, true);
					case "indeterminate":
						return (Gender.Indeterminate, true);
					default:
						actor.OutputHandler.Send($"That is not a valid gender to use{parameterArgument}.");
						return (null, false);
				}
			case ProgVariableTypes.TimeSpan:
				if (!TimeSpan.TryParse(parText, out var tsValue))
				{
					actor.OutputHandler.Send($"That is not a valid timespan to use{parameterArgument}.");
					return (null, false);
				}

				return (tsValue, true);
			case ProgVariableTypes.DateTime:
				if (!DateTime.TryParse(parText, out var dtValue))
				{
					actor.OutputHandler.Send($"That is not a valid datetime to use{parameterArgument}.");
					return (null, false);
				}

				return (dtValue, true);
			case ProgVariableTypes.MudDateTime:
				if (!MudDateTime.TryParse(parText, actor.Gameworld, out var mdtValue))
				{
					actor.OutputHandler.Send($"That is not a valid mud datetime to use{parameterArgument}.");
					return (null, false);
				}

				return (mdtValue, true);
			case ProgVariableTypes.Item:
				var targetItem = actor.TargetItem(parText);
				if (targetItem == null)
				{
					actor.OutputHandler.Send($"You do not see any item like that to use{parameterArgument}.");
					return (null, false);
				}

				return (targetItem, true);
			case ProgVariableTypes.Location:
				if (parText.Equals("here", StringComparison.InvariantCultureIgnoreCase))
				{
					return (actor.Location, true);
				}

				if (long.TryParse(parText, out var iValue))
				{
					var targetCell = actor.Gameworld.Cells.Get(iValue);
					return (targetCell, true);
				}
				else
				{
					var targetCell = RoomBuilderModule.LookupCell(actor.Gameworld, parText);
					if (targetCell == null)
					{
						actor.OutputHandler.Send(
							$"You must specify the ID number of the location to use at parameter{parameterArgument}.");
						return (null, false);
					}

					return (targetCell, true);
				}

			case ProgVariableTypes.Number:
				if (decimal.TryParse(parText, out var dValue))
				{
					return (dValue, true);
				}

				actor.OutputHandler.Send($"That is not a valid number argument to use{parameterArgument}.");
				return (null, false);
			case ProgVariableTypes.Shard:
				if (long.TryParse(parText, out iValue))
				{
					var targetShard = actor.Gameworld.Shards.Get(iValue);
					return (targetShard, true);
				}

				actor.OutputHandler.Send($"You must specify the ID number of the shard to use{parameterArgument}.");
				return (null, false);
			case ProgVariableTypes.Text:
				return (parText, true);
			case ProgVariableTypes.Zone:
				if (long.TryParse(parText, out iValue))
				{
					var targetZone = actor.Gameworld.Zones.Get(iValue);
					return (targetZone, true);
				}

				actor.OutputHandler.Send($"You must specify the ID number of the zone to use{parameterArgument}.");
				return (null, false);
			case ProgVariableTypes.Clan:
				var targetClan = long.TryParse(parText, out iValue)
					? actor.Gameworld.Clans.Get(iValue)
					: actor.Gameworld.Clans.FirstOrDefault(
						  x => x.FullName.Equals(parText, StringComparison.InvariantCultureIgnoreCase)) ??
					  actor.Gameworld.Clans.FirstOrDefault(
						  x => x.Alias.Equals(parText, StringComparison.InvariantCultureIgnoreCase));
				if (targetClan == null)
				{
					actor.Send($"There is no such clan{parameterArgument}.");
					return (null, false);
				}

				return (targetClan, true);
			case ProgVariableTypes.ClanRank:
				var rank = actor.Gameworld.Clans.SelectMany(x => x.Ranks).GetByIdOrName(parText);
				if (rank is null)
				{
					actor.OutputHandler.Send($"There is no such rank{parameterArgument}.");
					return (null, false);
				}

				return (rank, true);
			case ProgVariableTypes.ClanPaygrade:
				var paygrade = actor.Gameworld.Clans.SelectMany(x => x.Paygrades).GetByIdOrName(parText);
				if (paygrade is null)
				{
					actor.OutputHandler.Send($"There is no such paygrade{parameterArgument}.");
					return (null, false);
				}

				return (paygrade, true);
			case ProgVariableTypes.ClanAppointment:
				var appointment = actor.Gameworld.Clans.SelectMany(x => x.Appointments).GetByIdOrName(parText);
				if (appointment is null)
				{
					actor.OutputHandler.Send($"There is no such appointment{parameterArgument}.");
					return (null, false);
				}

				return (appointment, true);
			case ProgVariableTypes.Currency:
				var targetCurrency = long.TryParse(parText, out iValue)
					? actor.Gameworld.Currencies.Get(iValue)
					: actor.Gameworld.Currencies.FirstOrDefault(
						x => x.Name.Equals(parText, StringComparison.InvariantCultureIgnoreCase));
				if (targetCurrency == null)
				{
					actor.Send($"There is no such currency{parameterArgument}.");
					return (null, false);
				}

				return (targetCurrency, true);
			case ProgVariableTypes.Language:
				var targetLanguages = long.TryParse(parText, out iValue)
					? actor.Gameworld.Languages.Get(iValue)
					: actor.Gameworld.Languages.GetByName(parText);
				if (targetLanguages == null)
				{
					actor.Send($"There is no such language{parameterArgument}.");
					return (null, false);
				}

				return (targetLanguages, true);
			case ProgVariableTypes.Script:
				var targetScripts = actor.Gameworld.Scripts.GetByIdOrName(parText);
				if (targetScripts is null)
				{
					actor.OutputHandler.Send($"There is no such script{parameterArgument}.");
					return (null, false);
				}

				return (targetScripts, true);
			case ProgVariableTypes.Accent:
				var targetAccent = long.TryParse(parText, out iValue)
					? actor.Gameworld.Accents.Get(iValue)
					: actor.Gameworld.Accents.GetByName(parText);
				if (targetAccent == null)
				{
					actor.Send($"There is no such accent{parameterArgument}.");
					return (null, false);
				}

				return (targetAccent, true);
			case ProgVariableTypes.Perceiver:
			case ProgVariableTypes.Perceivable:
				var targetPerceiver = actor.Target(parText);
				if (targetPerceiver == null)
				{
					actor.Send($"There is nothing or no-one like that for you to use{parameterArgument}.");
					return (null, false);
				}

				return (targetPerceiver, true);
			case ProgVariableTypes.Exit:
				var exit = actor.Location.GetExitKeyword(parText, actor);
				if (exit == null)
				{
					actor.OutputHandler.Send($"There is no such exit here{parameterArgument}.");
					return (null, false);
				}

				return (exit, true);
			case ProgVariableTypes.Trait:
				var trait = actor.Gameworld.Traits.GetByIdOrName(parText);
				if (trait is null)
				{
					actor.OutputHandler.Send($"There is no such trait{parameterArgument}.");
					return (null, false);
				}

				return (trait, true);
			case ProgVariableTypes.Race:
				var race = actor.Gameworld.Races.GetByIdOrName(parText);
				if (race is null)
				{
					actor.OutputHandler.Send($"There is no such race{parameterArgument}");
					return (null, false);
				}

				return (race, true);
			case ProgVariableTypes.Culture:
				var culture = actor.Gameworld.Cultures.GetByIdOrName(parText);
				if (culture is null)
				{
					actor.OutputHandler.Send($"There is no such culture{parameterArgument}");
					return (null, false);
				}

				return (culture, true);
			case ProgVariableTypes.Ethnicity:
				var ethnicity = actor.Gameworld.Ethnicities.GetByIdOrName(parText);
				if (ethnicity is null)
				{
					actor.OutputHandler.Send($"There is no such ethnicity{parameterArgument}");
					return (null, false);
				}

				return (ethnicity, true);
			case ProgVariableTypes.Merit:
				var merit = actor.Gameworld.Merits.GetByIdOrName(parText);
				if (merit is null)
				{
					actor.OutputHandler.Send($"There is no such merit{parameterArgument}");
					return (null, false);
				}

				return (merit, true);
			case ProgVariableTypes.Calendar:
				var calendar = actor.Gameworld.Calendars.GetByIdOrNames(parText);
				if (calendar is null)
				{
					actor.OutputHandler.Send($"There is no such calendar{parameterArgument}");
					return (null, false);
				}

				return (calendar, true);
			case ProgVariableTypes.Clock:
				var clock = actor.Gameworld.Clocks.GetByIdOrNames(parText);
				if (clock is null)
				{
					actor.OutputHandler.Send($"There is no such clock{parameterArgument}");
					return (null, false);
				}

				return (clock, true);
			case ProgVariableTypes.Knowledge:
				var knowledge = actor.Gameworld.Knowledges.GetByIdOrName(parText);
				if (knowledge is null)
				{
					actor.OutputHandler.Send($"There is no such knowledge{parameterArgument}");
					return (null, false);
				}

				return (knowledge, true);
			case ProgVariableTypes.Role:
				var role = actor.Gameworld.Roles.GetByIdOrName(parText);
				if (role is null)
				{
					actor.OutputHandler.Send($"There is no such role{parameterArgument}");
					return (null, false);
				}

				return (role, true);
			case ProgVariableTypes.Drug:
				var drug = actor.Gameworld.Drugs.GetByIdOrName(parText);
				if (drug is null)
				{
					actor.OutputHandler.Send($"There is no such drug{parameterArgument}");
					return (null, false);
				}

				return (drug, true);
			case ProgVariableTypes.WeatherEvent:
				var weather = actor.Gameworld.WeatherEvents.GetByIdOrName(parText);
				if (weather is null)
				{
					actor.OutputHandler.Send($"There is no such weather event{parameterArgument}");
					return (null, false);
				}

				return (weather, true);
			case ProgVariableTypes.Shop:
				var shop = actor.Gameworld.Shops.GetByIdOrName(parText);
				if (shop is null)
				{
					actor.OutputHandler.Send($"There is no such shop{parameterArgument}");
					return (null, false);
				}

				return (shop, true);
			case ProgVariableTypes.Merchandise:
				var merch = actor.Gameworld.Shops.SelectMany(x => x.Merchandises).GetByIdOrName(parText);
				if (merch is null)
				{
					actor.OutputHandler.Send($"There is no such merchandise{parameterArgument}");
					return (null, false);
				}

				return (merch, true);
			case ProgVariableTypes.OverlayPackage:
				var overlay = actor.Gameworld.CellOverlayPackages.GetByIdOrName(parText);
				if (overlay is null)
				{
					actor.OutputHandler.Send($"There is no such overlay package{parameterArgument}");
					return (null, false);
				}

				return (overlay, true);
			case ProgVariableTypes.Terrain:
				var terrain = actor.Gameworld.Terrains.GetByIdOrName(parText);
				if (terrain is null)
				{
					actor.OutputHandler.Send($"There is no such terrain{parameterArgument}");
					return (null, false);
				}

				return (terrain, true);
			case ProgVariableTypes.Solid:
				var material = actor.Gameworld.Materials.GetByIdOrName(parText);
				if (material is null)
				{
					actor.OutputHandler.Send($"There is no such material{parameterArgument}");
					return (null, false);
				}

				return (material, true);
			case ProgVariableTypes.Liquid:
				var liquid = actor.Gameworld.Liquids.GetByIdOrName(parText);
				if (liquid is null)
				{
					actor.OutputHandler.Send($"There is no such liquid{parameterArgument}");
					return (null, false);
				}

				return (liquid, true);
			case ProgVariableTypes.Gas:
				var gas = actor.Gameworld.Gases.GetByIdOrName(parText);
				if (gas is null)
				{
					actor.OutputHandler.Send($"There is no such gas{parameterArgument}");
					return (null, false);
				}

				return (gas, true);
			case ProgVariableTypes.LegalAuthority:
				var legal = actor.Gameworld.CellOverlayPackages.GetByIdOrName(parText);
				if (legal is null)
				{
					actor.OutputHandler.Send($"There is no such legal authority{parameterArgument}");
					return (null, false);
				}

				return (legal, true);
			case ProgVariableTypes.MagicCapability:
				var capability = actor.Gameworld.MagicCapabilities.GetByIdOrName(parText);
				if (capability is null)
				{
					actor.OutputHandler.Send($"There is no such magic capability{parameterArgument}");
					return (null, false);
				}

				return (capability, true);
			case ProgVariableTypes.MagicSchool:
				var school = actor.Gameworld.MagicSchools.GetByIdOrName(parText);
				if (school is null)
				{
					actor.OutputHandler.Send($"There is no such magic school{parameterArgument}");
					return (null, false);
				}

				return (school, true);
			case ProgVariableTypes.MagicSpell:
				var spell = actor.Gameworld.MagicSpells.GetByIdOrName(parText);
				if (spell is null)
				{
					actor.OutputHandler.Send($"There is no such magic spell{parameterArgument}");
					return (null, false);
				}

				return (spell, true);
			case ProgVariableTypes.Bank:
				var bank = actor.Gameworld.Banks.GetByIdOrName(parText);
				if (bank is null)
				{
					actor.OutputHandler.Send($"There is no such bank{parameterArgument}");
					return (null, false);
				}

				return (bank, true);
			case ProgVariableTypes.BankAccountType:
				var accountType = actor.Gameworld.BankAccountTypes.GetByIdOrName(parText);
				if (accountType is null)
				{
					actor.OutputHandler.Send($"There is no such bank account type{parameterArgument}");
					return (null, false);
				}

				return (accountType, true);
			case ProgVariableTypes.BankAccount:
				var (account, error) = Economy.Banking.Bank.FindBankAccount(parText, null, actor);
				if (account is null)
				{
					actor.OutputHandler.Send($"{error}{parameterArgument}");
					return (null, false);
				}

				return (account, true);
			case ProgVariableTypes.Project:
				var project = actor.Gameworld.ActiveProjects.GetByIdOrName(parText);
				if (project is null)
				{
					actor.OutputHandler.Send($"There is no such project{parameterArgument}");
					return (null, false);
				}

				return (project, true);
			case ProgVariableTypes.Law:
				var law = actor.Gameworld.Laws.GetByIdOrName(parText);
				if (law is null)
				{
					actor.OutputHandler.Send($"There is no such law{parameterArgument}");
					return (null, false);
				}

				return (law, true);
			case ProgVariableTypes.Market:
				var market = actor.Gameworld.Markets.GetByIdOrName(parText);
				if (market is null)
				{
					actor.OutputHandler.Send($"There is no such market{parameterArgument}");
					return (null, false);
				}

				return (market, true);
			case ProgVariableTypes.MarketCategory:
				var category = actor.Gameworld.MarketCategories.GetByIdOrName(parText);
				if (category is null)
				{
					actor.OutputHandler.Send($"There is no such market category{parameterArgument}");
					return (null, false);
				}

				return (category, true);
			case ProgVariableTypes.Crime:
				var crime = actor.Gameworld.Crimes.GetByIdOrName(parText);
				if (crime is null)
				{
					actor.OutputHandler.Send($"There is no such crime{parameterArgument}");
					return (null, false);
				}
				return (crime, true);
			case ProgVariableTypes.Area:
				var area = actor.Gameworld.Areas.GetByIdOrName(parText);
				if (area is null)
				{
					actor.OutputHandler.Send($"There is no such area{parameterArgument}");
					return (null, false);
				}

				return (area, true);
			case ProgVariableTypes.Writing:
				if (!long.TryParse(parText, out iValue))
				{
					actor.OutputHandler.Send($"The text is not a valid id{parameterArgument}");
					return (null, false);
				}

				var writing = actor.Gameworld.Writings.Get(iValue);
				if (writing is null)
				{
					actor.OutputHandler.Send($"There is no such writing{parameterArgument}");
					return (null, false);
				}

				return (writing, true);
			case ProgVariableTypes.Effect:
			case ProgVariableTypes.Outfit:
			case ProgVariableTypes.OutfitItem:
			case ProgVariableTypes.Material:
			default:
				actor.Send(
					$"The variable type {type.Describe().Colour(Telnet.VariableCyan)} is not yet supported in this command. Sorry.");
				return (null, false);
		}
	}

	private static void ProgSetText(ICharacter actor, IFutureProg prog, StringStack command, bool append)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"You are now editing the text of prog {prog.MXPClickableFunctionNameWithId()}.");
		if (!string.IsNullOrEmpty(prog.FunctionText))
		{
			sb.AppendLine();
			sb.AppendLine($"{(append ? "Appending to" : "Replacing")} the following function text:");
			sb.AppendLine();
			sb.AppendLine(prog.FunctionText);
			sb.AppendLine();
		}

		if (prog.ReturnType == ProgVariableTypes.Void)
		{
			sb.AppendLine("This prog does not have any return value.");
		}
		else
		{
			sb.AppendLineFormat("This prog returns {0}.", prog.ReturnType.Describe().A_An(colour: Telnet.VariableGreen));
		}

		if (prog.AcceptsAnyParameters)
		{
			sb.AppendLine(
				"This prog accepts any parameters passed to it, and so there are none accessible in the prog.");
		}
		else if (!prog.Parameters.Any())
		{
			sb.AppendLine("This prog does not have any parameters.");
		}
		else
		{
			sb.AppendLineFormat(actor, "This prog has the following parameters:\n\n{0}",
				prog.NamedParameters.Select(x => $"\t{x.Item2.Colour(Telnet.VariableCyan)} {"as".Colour(Telnet.KeywordBlue)} {x.Item1.Describe().Colour(Telnet.VariableGreen)}")
					.ListToString("\t", "\n", "", "\n")
			);
		}

		actor.Send(sb.ToString());
		actor.EditorMode((text, handler, paramlist) =>
			{
				prog.FunctionText = text;
				handler.Send(
					$"You change the function text of prog {prog.MXPClickableFunctionNameWithId()}.");
				if (prog.Compile())
				{
					handler.Send("It compiled successfully.".ColourBold(Telnet.Green));
				}
				else
				{
					handler.Send(
						$"It failed to compile, with the following error:\n\t{prog.CompileError}"
							.ColourBold(Telnet.Red));
				}

				prog.Changed = true;
			},
			(handler, paramlist) =>
			{
				handler.Send(
					$"You decide not to change the function text of prog {prog.MXPClickableFunctionNameWithId()}.");
			}, 1.0, null,
			EditorOptions.PermitEmpty);
	}

	private static void ProgEdit(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			var editing = actor.EffectsOfType<BuilderEditingEffect<IFutureProg>>().FirstOrDefault();
			if (editing == null)
			{
				actor.OutputHandler.Send("Which prog do you want to edit?");
				return;
			}

			ShowModule.Show_FutureProg(actor, new StringStack(editing.EditingItem.Id.ToString()));
			return;
		}

		if (command.Peek().Equals("new", StringComparison.InvariantCultureIgnoreCase))
		{
			ProgEditNew(actor, command);
			return;
		}

		if (command.Peek().Equals("newevent", StringComparison.InvariantCultureIgnoreCase))
		{
			ProgEditNewEvent(actor, command);
			return;
		}

		if (command.Peek().EqualTo("close"))
		{
			if (actor.CombinedEffectsOfType<BuilderEditingEffect<IFutureProg>>().Any())
			{
				actor.OutputHandler.Send(
					$"You are no longer editing the prog {actor.CombinedEffectsOfType<BuilderEditingEffect<IFutureProg>>().First().EditingItem.MXPClickableFunctionNameWithId()}.");
				actor.RemoveAllEffects<BuilderEditingEffect<IFutureProg>>();
				return;
			}

			actor.OutputHandler.Send("You are not editing a prog.");
			return;
		}

		var prog = long.TryParse(command.Pop(), out var value)
			? actor.Gameworld.FutureProgs.Get(value)
			: actor.Gameworld.FutureProgs.Get(command.Last).FirstOrDefault();

		if (prog == null)
		{
			actor.Send("There is no such prog for you to edit.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<IFutureProg>>();
		actor.AddEffect(new BuilderEditingEffect<IFutureProg>(actor) { EditingItem = prog });
		actor.OutputHandler.Send($"You are now editing prog {prog.MXPClickableFunctionNameWithId()}.");
	}

	private static void ProgClone(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which prog do you want to clone?");
			return;
		}

		var prog = long.TryParse(command.PopSpeech(), out var value)
			? actor.Gameworld.FutureProgs.Get(value)
			: actor.Gameworld.FutureProgs.GetByName(command.Last);
		if (prog == null)
		{
			actor.Send("There is no such prog for you to clone.");
			return;
		}

		var newName = $"{prog.FunctionName}clone";
		if (!command.IsFinished)
		{
			newName = command.Pop();
			if (
				actor.Gameworld.FutureProgs.Any(
					x => x.FunctionName.Equals(newName, StringComparison.InvariantCultureIgnoreCase)))
			{
				actor.Send("You cannot specify a function name that already exists!");
				return;
			}
		}

		using (new FMDB())
		{
			var dbitem = new Models.FutureProg();
			FMDB.Context.FutureProgs.Add(dbitem);
			dbitem.FunctionName = newName;
			dbitem.FunctionText = prog.FunctionText;
			dbitem.FunctionComment = prog.FunctionComment;
			dbitem.Category = prog.Category;
			dbitem.Subcategory = prog.Subcategory;
			dbitem.ReturnType = (long)prog.ReturnType;
			dbitem.Public = prog.Public;
			dbitem.AcceptsAnyParameters = prog.AcceptsAnyParameters;
			var i = 0;
			foreach (var parameter in prog.NamedParameters)
			{
				var dbparam = new FutureProgsParameter();
				dbitem.FutureProgsParameters.Add(dbparam);
				dbparam.FutureProg = dbitem;
				dbparam.ParameterIndex = i++;
				dbparam.ParameterName = parameter.Item2;
				dbparam.ParameterType = (long)parameter.Item1;
			}

			FMDB.Context.SaveChanges();
			var newProg = new FutureProg.FutureProg(dbitem, actor.Gameworld);
			newProg.Compile();
			actor.Gameworld.Add(newProg);
			actor.RemoveAllEffects<BuilderEditingEffect<IFutureProg>>();
			actor.AddEffect(new BuilderEditingEffect<IFutureProg>(actor) { EditingItem = newProg });
			actor.Send(
				$"You create a new prog {newProg.MXPClickableFunctionNameWithId()} which you are now editing, cloned from prog {prog.MXPClickableFunctionNameWithId()}.");
		}
	}

	private static void ProgEditNew(ICharacter actor, StringStack command)
	{
		command.Pop();
		var newName = "Unnamed";
		if (!command.IsFinished)
		{
			newName = command.Pop();
			if (
				actor.Gameworld.FutureProgs.Any(
					x => x.FunctionName.Equals(newName, StringComparison.InvariantCultureIgnoreCase)))
			{
				actor.Send("You cannot specify a function name that already exists!");
				return;
			}
		}

		using (new FMDB())
		{
			var dbitem = new Models.FutureProg();
			FMDB.Context.FutureProgs.Add(dbitem);
			dbitem.FunctionName = newName;
			dbitem.FunctionText = "";
			dbitem.FunctionComment = "Comment this function";
			dbitem.Category = "Uncategorised";
			dbitem.Subcategory = "New";
			dbitem.ReturnType = (long)ProgVariableTypes.Void;
			FMDB.Context.SaveChanges();
			var prog = new FutureProg.FutureProg(dbitem, actor.Gameworld);
			actor.Gameworld.Add(prog);
			actor.OutputHandler.Send(
				$"You create a new prog {prog.MXPClickableFunctionNameWithId()}, which you are now editing.");
			actor.RemoveAllEffects<BuilderEditingEffect<IFutureProg>>();
			actor.AddEffect(new BuilderEditingEffect<IFutureProg>(actor) { EditingItem = prog });
		}
	}

	private static void ProgEditNewEvent(ICharacter actor, StringStack command)
	{
		command.PopSpeech();
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which event do you want to use as a model for your new prog? See {"show events".MXPSend("show events")} for a list of available events.");
			return;
		}

		if (!command.PopSpeech().TryParseEnum<EventType>(out var type))
		{
			actor.OutputHandler.Send(
				$"There is no such event as {command.Last.ColourCommand()}. See {"show events".MXPSend("show events")} for a list of available events.");
			return;
		}

		var eventInfo = type.GetAttribute<EventInfoAttribute>();
		if (eventInfo is null)
		{
			actor.OutputHandler.Send(
				$"The {type.DescribeEnum().ColourName()} event has not been configured to work with progs yet. Sorry.");
			return;
		}

		var newName = "Unnamed";
		if (!command.IsFinished)
		{
			newName = command.Pop();
			if (
				actor.Gameworld.FutureProgs.Any(
					x => x.FunctionName.Equals(newName, StringComparison.InvariantCultureIgnoreCase)))
			{
				actor.Send("You cannot specify a function name that already exists!");
				return;
			}
		}

		using (new FMDB())
		{
			var dbitem = new Models.FutureProg();
			FMDB.Context.FutureProgs.Add(dbitem);
			dbitem.FunctionName = newName;
			dbitem.FunctionText = "";
			dbitem.FunctionComment = "Comment this function";
			dbitem.Category = "Uncategorised";
			dbitem.Subcategory = "New";
			dbitem.ReturnType = (long)ProgVariableTypes.Void;
			var targetCount = eventInfo.ProgTypes.Count();
			for (var i = 0; i < targetCount; i++)
			{
				dbitem.FutureProgsParameters.Add(new FutureProgsParameter
				{
					FutureProg = dbitem,
					ParameterName = eventInfo.Parameters.ElementAt(i).name,
					ParameterIndex = i,
					ParameterType = (long)eventInfo.ProgTypes.ElementAt(i)
				});
			}

			FMDB.Context.SaveChanges();
			var prog = new FutureProg.FutureProg(dbitem, actor.Gameworld);
			actor.Gameworld.Add(prog);
			actor.OutputHandler.Send(
				$"You create a new prog {prog.MXPClickableFunctionNameWithId()} based off the event {type.DescribeEnum().ColourName()}, which you are now editing.");
			actor.RemoveAllEffects<BuilderEditingEffect<IFutureProg>>();
			actor.AddEffect(new BuilderEditingEffect<IFutureProg>(actor) { EditingItem = prog });
		}
	}

	private static void ProgCompile(ICharacter actor, StringStack command)
	{
		actor.Send("Recompiling all progs...");
		foreach (var prog in actor.Gameworld.FutureProgs)
		{
			if (!prog.Compile())
			{
				actor.OutputHandler.Send(
					$"Prog {prog.MXPClickableFunctionNameWithId()} failed to compile: \n{prog.CompileError}"
						.ColourIncludingReset(Telnet.BoldRed));
			}
			prog.ColouriseFunctionText();
		}

		actor.Send("Done.");
	}

	private static void ProgDelete(ICharacter actor, StringStack command)
	{
		actor.Send("Coming soon...");
	}

	private static void ProgParameter(ICharacter actor, StringStack command)
	{
		var prog = actor.CombinedEffectsOfType<BuilderEditingEffect<IFutureProg>>().FirstOrDefault()?.EditingItem;
		if (prog == null)
		{
			actor.OutputHandler.Send("You are not editing a prog.");
			return;
		}

		switch (command.Pop().ToLowerInvariant())
		{
			case "add":
				ProgParameterAdd(actor, prog, command);
				break;
			case "delete":
			case "remove":
			case "rem":
				ProgParameterRemove(actor, prog, command);
				break;
			case "swap":
				ProgParameterSwap(actor, prog, command);
				break;
			default:
				actor.Send("Valid options are add, delete and swap.");
				return;
		}
	}

	private static void ProgParameterAdd(ICharacter actor, IFutureProg prog, StringStack command)
	{
		if (prog.AcceptsAnyParameters)
		{
			actor.Send("That prog accepts any parameters, and so cannot have them manually specified.");
			return;
		}

		if (command.IsFinished)
		{
			actor.Send("What name do you want to give your new parameter?");
			return;
		}

		var parameterName = command.Pop();
		if (prog.NamedParameters.Any(x => x.Item2.Equals(parameterName, StringComparison.InvariantCultureIgnoreCase)))
		{
			actor.OutputHandler.Send(
				$"Prog {prog.MXPClickableFunctionNameWithId()} already contains a parameter called {parameterName.ColourName()}.");
			return;
		}

		if (command.IsFinished)
		{
			actor.Send("Which variable type do you want your new parameter to be?");
			return;
		}

		var parameterType = command.SafeRemainingArgument;
		var type = FutureProg.FutureProg.GetTypeByName(parameterType);
		if (type == ProgVariableTypes.Void || type == ProgVariableTypes.Error)
		{
			actor.Send($"The text {parameterType.ColourCommand()} is not a valid variable type.");
			return;
		}

		prog.NamedParameters.Add(Tuple.Create(type, parameterName.ToLowerInvariant()));
		prog.Changed = true;
		actor.OutputHandler.Send(
			$"You add a parameter named {parameterName.Colour(Telnet.VariableCyan)} of type {type.Describe().Colour(Telnet.VariableGreen)} to prog {prog.MXPClickableFunctionNameWithId()}.");
		if (!prog.Compile())
		{
			actor.OutputHandler.Send($"Prog did not compile: \n{prog.CompileError}".ColourBold(Telnet.Red));
		}
	}

	private static void ProgParameterRemove(ICharacter actor, IFutureProg prog, StringStack command)
	{
		if (prog.AcceptsAnyParameters)
		{
			actor.Send("That prog accepts any parameters, and so cannot have them manually specified.");
			return;
		}

		if (command.IsFinished)
		{
			actor.Send("Which parameter do you want to remove?");
			return;
		}

		var parameterName = command.Pop();
		if (
			!prog.NamedParameters.Any(
				x => x.Item2.Equals(parameterName, StringComparison.InvariantCultureIgnoreCase)))
		{
			actor.Send("There is no such parameter for you to remove.");
			return;
		}

		prog.NamedParameters.RemoveAll(
			x => x.Item2.Equals(parameterName, StringComparison.InvariantCultureIgnoreCase));
		prog.Changed = true;
		actor.OutputHandler.Send(
			$"You remove parameter {parameterName.Colour(Telnet.VariableCyan)} from prog {prog.MXPClickableFunctionNameWithId()}.");
		if (!prog.Compile())
		{
			actor.Send("Prog did not compile: {0}".ColourBold(Telnet.Red), prog.CompileError);
		}
	}

	private static void ProgParameterSwap(ICharacter actor, IFutureProg prog, StringStack command)
	{
		if (prog.AcceptsAnyParameters)
		{
			actor.Send("That prog accepts any parameters, and so cannot have them manually specified.");
			return;
		}

		if (command.IsFinished)
		{
			actor.Send("What is the first paramater that you want to swap?");
			return;
		}

		var parameterName1 = command.Pop();
		if (
			!prog.NamedParameters.Any(
				x => x.Item2.Equals(parameterName1, StringComparison.InvariantCultureIgnoreCase)))
		{
			actor.Send("There is no such parameter for you to swap.");
			return;
		}

		if (command.IsFinished)
		{
			actor.Send("What is the second paramater that you want to swap?");
			return;
		}

		var parameterName2 = command.Pop();
		if (
			!prog.NamedParameters.Any(
				x => x.Item2.Equals(parameterName2, StringComparison.InvariantCultureIgnoreCase)))
		{
			actor.Send("There is no such parameter for you to swap.");
			return;
		}

		if (parameterName1.Equals(parameterName2, StringComparison.InvariantCultureIgnoreCase))
		{
			actor.Send("You cannot swap a parameter with itself.");
			return;
		}

		prog.NamedParameters.Swap(
			prog.NamedParameters.FindIndex(
				x => x.Item2.Equals(parameterName1, StringComparison.InvariantCultureIgnoreCase)),
			prog.NamedParameters.FindIndex(
				x => x.Item2.Equals(parameterName2, StringComparison.InvariantCultureIgnoreCase))
		);
		prog.Changed = true;
		actor.OutputHandler.Send(
			$"You swap the order of parameters {parameterName1.Colour(Telnet.VariableCyan)} and {parameterName2.Colour(Telnet.VariableCyan)} in prog {prog.MXPClickableFunctionNameWithId()}.");
		if (!prog.Compile())
		{
			actor.Send("Prog did not compile: {0}".ColourBold(Telnet.Red), prog.CompileError);
		}
	}

	private static void ProgSetName(ICharacter actor, StringStack command, IFutureProg prog)
	{
		if (command.IsFinished)
		{
			actor.Send("What name do you want to give this prog?");
			return;
		}

		var progName = command.Pop();
		if (
			actor.Gameworld.FutureProgs.Any(
				x => x.FunctionName.Equals(progName, StringComparison.InvariantCultureIgnoreCase)))
		{
			actor.Send("There is already a prog with that name. Names must be unique.");
			return;
		}

		actor.OutputHandler.Send(
			$"You rename prog {prog.MXPClickableFunctionNameWithId()} to {progName.ColourName()}.");
		prog.FunctionName = progName;
		prog.Changed = true;
	}

	private static void ProgSetCategory(ICharacter actor, StringStack command, IFutureProg prog)
	{
		if (command.IsFinished)
		{
			actor.Send("What category do you want to set for this prog?");
			return;
		}

		prog.Category = command.Pop().Proper();
		prog.Changed = true;
		actor.OutputHandler.Send(
			$"You set the category for prog {prog.MXPClickableFunctionNameWithId()} to {prog.Category}.");
	}

	private static void ProgSetSubcategory(ICharacter actor, StringStack command, IFutureProg prog)
	{
		if (command.IsFinished)
		{
			actor.Send("What subcategory do you want to set for this prog?");
			return;
		}

		prog.Subcategory = command.Pop().Proper();
		prog.Changed = true;
		actor.OutputHandler.Send(
			$"You set the subcategory for prog {prog.MXPClickableFunctionNameWithId()} to {prog.Subcategory.ColourValue()}.");
	}

	private static void ProgSetComment(ICharacter actor, StringStack command, IFutureProg prog)
	{
		actor.Send("Replacing:\n{0}\n\nEnter your comment for prog {1} in the editor below:",
			prog.FunctionComment, prog.MXPClickableFunctionNameWithId());
		actor.EditorMode((text, handler, paramlist) =>
			{
				prog.FunctionComment = text;
				prog.Changed = true;
				if (string.IsNullOrEmpty(prog.FunctionComment))
				{
					handler.Send($"You remove the Function Comment from prog {prog.MXPClickableFunctionNameWithId()}.");
				}
				else
				{
					handler.Send(
						$"You set the Function Comment for prog {prog.MXPClickableFunctionNameWithId()} to:\n\t{prog.FunctionComment.Fullstop()}");
				}
			},
			(handler, paramlist) =>
			{
				handler.Send(
					$"You decline to update the Function Comment for prog {prog.MXPClickableFunctionNameWithId()}.");
			}, 1.0, null, EditorOptions.PermitEmpty);
	}

	private static void ProgSetAnyParameters(ICharacter actor, StringStack command, IFutureProg prog)
	{
		if (prog.AcceptsAnyParameters)
		{
			prog.AcceptsAnyParameters = false;
			prog.Changed = true;
			actor.OutputHandler.Send(
				$"Prog {prog.MXPClickableFunctionNameWithId()} will no longer accept any parameters, but instead must specify them.");
			if (!prog.Compile())
			{
				actor.Send("Prog did not compile: {0}".ColourBold(Telnet.Red), prog.CompileError);
			}

			return;
		}

		prog.AcceptsAnyParameters = true;
		prog.NamedParameters.Clear();
		prog.Changed = true;
		actor.OutputHandler.Send(
			$"Prog {prog.MXPClickableFunctionNameWithId()} will now accept any parameters, and has had any existing parameters removed.");
		if (!prog.Compile())
		{
			actor.Send("Prog did not compile: {0}".ColourBold(Telnet.Red), prog.CompileError);
		}
	}

	private static void ProgSetReturns(ICharacter actor, StringStack command, IFutureProg prog)
	{
		if (command.IsFinished)
		{
			actor.Send("What type do you want this prog to return? Use void for no return type.");
			return;
		}

		if (command.SafeRemainingArgument.Equals("void", StringComparison.InvariantCultureIgnoreCase))
		{
			prog.ReturnType = ProgVariableTypes.Void;
			prog.StaticType = FutureProgStaticType.NotStatic;
			prog.Changed = true;
			actor.OutputHandler.Send($"Prog {prog.MXPClickableFunctionNameWithId()} no longer returns a type.");
			return;
		}

		var type = FutureProg.FutureProg.GetTypeByName(command.SafeRemainingArgument);
		if (type == ProgVariableTypes.Error)
		{
			actor.Send("That is not a valid variable type.");
			return;
		}

		prog.ReturnType = type;
		prog.Changed = true;
		actor.OutputHandler.Send(
			$"You change the return type of prog {prog.MXPClickableFunctionNameWithId()} to {type.Describe().Colour(Telnet.VariableGreen)}.");
		if (!prog.Compile())
		{
			actor.Send("Prog did not compile: {0}".ColourBold(Telnet.Red), prog.CompileError);
		}
	}

	private static void ProgSetStatic(ICharacter actor, IFutureProg prog)
	{
		if (prog.StaticType == FutureProgStaticType.FullyStatic)
		{
			prog.StaticType = FutureProgStaticType.NotStatic;
			actor.OutputHandler.Send("That prog is no longer static, and will fully evaluate every time it runs.");
			return;
		}

		if (prog.ReturnType == ProgVariableTypes.Void)
		{
			actor.OutputHandler.Send("Void-returning functions can't be static.");
			return;
		}

		prog.StaticType = FutureProgStaticType.FullyStatic;
		actor.OutputHandler.Send(
			"That prog is now static - it will not actually evaluate after the first time it runs but will instead store its return value to return quickly. Ensure you understand the ramifications of that action.");
	}

	private static void ProgSet(ICharacter actor, StringStack command)
	{
		var prog = actor.CombinedEffectsOfType<BuilderEditingEffect<IFutureProg>>().FirstOrDefault()?.EditingItem;
		if (prog == null)
		{
			actor.OutputHandler.Send("You are not currently editing a prog.");
			return;
		}

		switch (command.Pop())
		{
			case "name":
				ProgSetName(actor, command, prog);
				break;
			case "category":
				ProgSetCategory(actor, command, prog);
				break;
			case "subcategory":
				ProgSetSubcategory(actor, command, prog);
				break;
			case "comment":
				ProgSetComment(actor, command, prog);
				break;
			case "anyparameters":
			case "acceptsanyparameters":
			case "acceptsany":
				ProgSetAnyParameters(actor, command, prog);
				break;
			case "return":
			case "returns":
				ProgSetReturns(actor, command, prog);
				break;
			case "text":
			case "lines":
			case "function":
				ProgSetText(actor, prog, command, false);
				return;
			case "append":
				ProgSetText(actor, prog, command, true);
				return;
			case "static":
				ProgSetStatic(actor, prog);
				return;
			case "parameter":
				ProgParameter(actor, command);
				return;
			default:
				actor.OutputHandler.Send(NewProgHelpText.SubstituteANSIColour());
				return;
		}
	}

	#endregion Prog Sub Commands

	private const string RegisterHelp =
		@"The register command is used to view and edit the 'register variables' that are associated with any prog type.

Register variables are ways for you to extend what information is stored on existing data types. For example, while a character might already have fields like their name, age, location and a list of inventory items, you could add a new variable (a register variable) to characters to store a number that represented their reputation with a faction.

All things of the same type share the same register variables, but each individual thing gets to track its own values. So all rooms might have a 'GangOwner' variable but each room would have its own value for that.

The syntax for setting up register variables is as follows:

	#3register show <type>#0 - shows all registered variables for a type
	#3register remove <type> <variable name>#0 - removes a variable from a type. Causes irreversible data loss.
	#3register <type> <name> <variable type>#0 - creates a new variable for a type
	#3register default <type> <name> <value>#0 - sets the default value for a register variable if none is otherwise set

To use these variables in your progs, you can use the following two snippets:

	#6GetRegister(@thing, ""variable name"")#0 - retrieves a register variable
	#6SetRegister @thing ""variable name"" @value#0 - sets a register variable. Note that this is a statement not a function.

To see a list of types, use #3prog help types#0.
To see what register values a room, item or character has use the #3sniff#0 command.";

	[PlayerCommand("Register", "register")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("register", RegisterHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void Register(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.Send(@"The correct syntax is as follows:

	#3register show <type>#0 - shows all registered variables for a type
	#3register remove <type> <variable name>#0 - removes a variable from a type. Causes irreversible data loss.
	#3register <type> <name> <variable type>#0 - creates a new variable for a type
	#3register default <type> <name> <value>#0 - sets the default value for a register variable if none is otherwise set"
				.SubstituteANSIColour());
			return;
		}

		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "show":
			case "view":
				RegisterShow(actor, ss);
				return;
			case "remove":
				RegisterRemove(actor, ss);
				return;
			case "default":
				RegisterDefault(actor, ss);
				return;
		}

		var type = FutureProg.FutureProg.GetTypeByName(ss.Last);
		if (type == ProgVariableTypes.Error)
		{
			actor.Send("That is not a valid type.");
			return;
		}

		if (type.HasFlag(ProgVariableTypes.Collection) ||
			type.HasFlag(ProgVariableTypes.CollectionItem) ||
			type.HasFlag(ProgVariableTypes.Perceivable) ||
			type.HasFlag(ProgVariableTypes.Perceiver) ||
			type.HasFlag(ProgVariableTypes.MagicResourceHaver) ||
			type.HasFlag(ProgVariableTypes.CollectionDictionary) ||
			type.HasFlag(ProgVariableTypes.Dictionary) ||
			type.CompatibleWith(ProgVariableTypes.ValueType)
		   )
		{
			actor.Send("That type cannot have any declared variables.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("What name do you want to give to this variable?");
			return;
		}

		var variableName = ss.Pop().ToLowerInvariant();

		if (ss.IsFinished)
		{
			actor.Send("What type do you want to store this variable as?");
			return;
		}

		var varType = FutureProg.FutureProg.GetTypeByName(ss.SafeRemainingArgument);
		if (varType == ProgVariableTypes.Error)
		{
			actor.Send("That is not a valid variable type.");
			return;
		}

		if (actor.Gameworld.VariableRegister.RegisterVariable(type, varType, variableName))
		{
			actor.Send(
				$"You successfully register the variable {variableName.Colour(Telnet.Cyan)} as a {varType.Describe().Colour(Telnet.Cyan)} on the {type.Describe().Colour(Telnet.Yellow)} type.");
			var previousCompileCount = actor.Gameworld.FutureProgs.Count(x => !string.IsNullOrEmpty(x.CompileError));
			var newCount = 0;
			foreach (var prog in actor.Gameworld.FutureProgs)
			{
				if (!prog.Compile())
				{
					newCount += 1;
				}
			}

			actor.Send("{0:N0} prog{1} failed to compile, compared to {2:N0} prog{3} before this update.", newCount,
				newCount == 1 ? "" : "s", previousCompileCount, previousCompileCount == 1 ? "" : "s");
		}
		else
		{
			actor.Send("Unable to register that variable.");
		}
	}

	#region Register Sub Commands

	private static void RegisterShow(ICharacter actor, StringStack input)
	{
		if (input.IsFinished)
		{
			actor.Send("Show which type?");
			return;
		}

		var type = FutureProg.FutureProg.GetTypeByName(input.PopSpeech());
		if (type == ProgVariableTypes.Error)
		{
			actor.Send("There is no such type.");
			return;
		}

		var variables = actor.Gameworld.VariableRegister.AllVariables(type);
		var sb = new StringBuilder();
		sb.AppendLine($"Register Variables for Type {type.Describe().ColourName()}");
		sb.AppendLine($"");
		sb.AppendLine(StringUtilities.GetTextTable(
			from item in variables.OrderBy(x => x.Item1)
			select new List<string>
			{
				$"{item.Item2.Describe().ColourName()} {item.Item1.ColourCommand()}",
				FutureProg.FutureProg.VariableValueToText(
					actor.Gameworld.VariableRegister.GetDefaultValue(type, item.Item1), actor)
			},
			new List<string>
			{
				"Variable Type and Name",
				"Default Value"
			},
			actor,
			Telnet.Green
		));
		actor.Send(sb.ToString());
	}

	private static void RegisterDefault(ICharacter actor, StringStack input)
	{
		if (input.IsFinished)
		{
			actor.OutputHandler.Send("Which type do you want to set a default variable value for?");
			return;
		}

		var type = FutureProg.FutureProg.GetTypeByName(input.PopSpeech());
		if (type == ProgVariableTypes.Error)
		{
			actor.Send("There is no such type.");
			return;
		}

		if (type.HasFlag(ProgVariableTypes.Collection) ||
			type.HasFlag(ProgVariableTypes.CollectionItem) ||
			type.HasFlag(ProgVariableTypes.Perceivable) ||
			type.HasFlag(ProgVariableTypes.Perceiver) ||
			type.HasFlag(ProgVariableTypes.MagicResourceHaver) ||
			type.HasFlag(ProgVariableTypes.CollectionDictionary) ||
			type.HasFlag(ProgVariableTypes.Dictionary) ||
			type.CompatibleWith(ProgVariableTypes.ValueType)
		   )
		{
			actor.Send("That type cannot have any declared variables.");
			return;
		}

		if (input.IsFinished)
		{
			actor.Send("Which variable do you want to set a default for?");
			return;
		}

		var variableName = input.Pop().ToLowerInvariant();
		if (!actor.Gameworld.VariableRegister.IsRegistered(type, variableName))
		{
			actor.OutputHandler.Send(
				$"The {type.Describe().Colour(Telnet.Cyan)} type does not have a variable called {variableName.ColourCommand()}.");
			return;
		}

		if (input.IsFinished)
		{
			actor.OutputHandler.Send("What value do you want to set as the default value for that variable?");
			return;
		}

		var varType = actor.Gameworld.VariableRegister.GetType(type, variableName);
		var (item, success) = GetArgument(varType, input.SafeRemainingArgument, 0, actor);
		if (!success)
		{
			return;
		}

		var value = FutureProg.FutureProg.GetVariable(varType, item);
		actor.Gameworld.VariableRegister.SetDefaultValue(type, variableName, value);
		actor.OutputHandler.Send(
			$"The default value for the {variableName.ColourCommand()} variable on the {type.Describe().ColourName()} type is now {FutureProg.FutureProg.VariableValueToText(value, actor).ColourIncludingReset(Telnet.Green)}");
	}

	private static void RegisterRemove(ICharacter actor, StringStack ss)
	{
		var type = FutureProg.FutureProg.GetTypeByName(ss.Last);
		if (type == ProgVariableTypes.Error)
		{
			actor.Send("That is not a valid type.");
			return;
		}

		if (type.HasFlag(ProgVariableTypes.Collection) || type.HasFlag(ProgVariableTypes.CollectionItem) ||
			type.HasFlag(ProgVariableTypes.Perceivable) || type.HasFlag(ProgVariableTypes.Perceiver))
		{
			actor.Send("That type cannot have any declared variables.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("Which variable for that type do you want to remove?");
			return;
		}

		var variableName = ss.Pop().ToLowerInvariant();
		if (actor.Gameworld.VariableRegister.DeregisterVariable(type, variableName))
		{
			actor.Send(
				$"You successfully deregister the variable {variableName.Colour(Telnet.Cyan)} on the {type.Describe().Colour(Telnet.Yellow)} type.");
			var previousCompileCount = actor.Gameworld.FutureProgs.Count(x => !string.IsNullOrEmpty(x.CompileError));
			var newCount = 0;
			foreach (var prog in actor.Gameworld.FutureProgs)
			{
				if (!prog.Compile())
				{
					newCount += 1;
				}
			}

			actor.Send("{0:N0} prog{1} failed to compile, compared to {2:N0} prog{3} before this update.", newCount,
				newCount == 1 ? "" : "s", previousCompileCount, previousCompileCount == 1 ? "" : "s");
		}
		else
		{
			actor.Send("Unable to deregister that variable.");
		}
	}

	#endregion

	#region Hooks

	private const string HookHelpText =
		@"This command is used to create and install hooks. Hooks are ways of connecting events (see #6SHOW EVENTS#0) and progs (see #6PROG HELP#0).

Once you have a prog that matches the right call signature for an event, you create a matching hook. Then, you can install that hook on a character, item, room or the like to ensure that it always fires in response to the event.

Note - you may need to restart the game for some hook-related changes to take effect. Generally it's good practice to reboot after doing anything much which these.

The syntax for this command is as follows:
	
	#3hook list [<filters>]#0 - lists all of the hooks
	#3hook create <name> <prog> <event>#0 - creates a new hook for an event
	#3hook create <name> <prog> CommandInput|SelfCommandInput <command>#0 - creates a new hook for a command input
	#3hook category <hook> <category>#0 - sets a category for a hook for ease of searching
	#3hook rename <hook> <name>#0 - renames a hook
	#3hook prog <hook> <prog>#0 - changes or toggles the prog payload of a hook
	#3hook command <hook> <command>#0 - changes the command for a command triggered hook
	#3hook install <hook> <target>#0 - installs a hook on a target
	#3hook remove <hook> <target>#0 - removes a hook on a target
	#3hook defaults#0 - shows all default hooks
	#3hook adddefault <perceivable type> <hook> <filter prog>#0 - adds a default hook
	#3hook remdefault <perceivable type> <hook> <filter prog>#0 - removes a default hook

You can use the following filters with #3hook list#0:

	#6*<prog>#0 - filters by a prog that the hook calls
	#6<eventtype>#0 - filters by a hook that pertains to the event";

	[PlayerCommand("Hook", "hook")]
	[CommandPermission(PermissionLevel.SeniorAdmin)]
	[HelpInfo("hook", HookHelpText, AutoHelp.HelpArgOrNoArg)]
	protected static void Hook(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var cmd = ss.PopForSwitch();

		switch (cmd.ToLowerInvariant())
		{
			case "list":
				HookList(actor, ss);
				return;
			case "defaults":
				HookDefaults(actor, ss);
				return;
			case "adddefault":
				HookAddDefault(actor, ss);
				return;
			case "remdefault":
			case "removedefault":
				HookRemoveDefault(actor, ss);
				return;
			case "create":
				HookCreate(actor, ss);
				return;
			case "categorise":
			case "categorize":
			case "category":
				HookCategory(actor, ss);
				return;
			case "install":
				HookInstall(actor, ss);
				return;
			case "remove":
				HookRemove(actor, ss);
				return;
			case "rename":
				HookRename(actor, ss);
				return;
			case "prog":
				HookProg(actor, ss);
				return;
			case "command":
				HookCommand(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(HookHelpText.SubstituteANSIColour());
				return;
		}
	}

	private static void HookCommand(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which hook do you want to change the name of?");
			return;
		}

		var hook = actor.Gameworld.Hooks.GetByIdOrName(ss.PopSpeech());
		if (hook is null)
		{
			actor.OutputHandler.Send("There is no such hook.");
			return;
		}

		if (hook is not ICommandHook chook)
		{
			actor.OutputHandler.Send($"The {hook.Name.ColourName()} hook is not triggered by command input.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What command would you like this hook to trigger on?");
			return;
		}

		var command = ss.Pop();
		chook.CommandText = command;
		chook.Changed = true;
		actor.OutputHandler.Send($"The {hook.Name.ColourName()} hook will now execute from the {command.ColourCommand()} command.");
	}

	private static void HookProg(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which hook do you want to change the name of?");
			return;
		}

		var hook = actor.Gameworld.Hooks.GetByIdOrName(ss.PopSpeech());
		if (hook is null)
		{
			actor.OutputHandler.Send("There is no such hook.");
			return;
		}

		if (hook is not IExecuteProgHook phook)
		{
			actor.OutputHandler.Send($"The hook {hook.Name.ColourName()} is not a hook that executes a prog.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which prog do you want to set or toggle for that hook?");
			return;
		}

		var prog = actor.Gameworld.FutureProgs.GetByIdOrName(ss.SafeRemainingArgument);
		if (prog is null)
		{
			actor.OutputHandler.Send($"There is no prog identified by the text {ss.SafeRemainingArgument.ColourCommand()}.");
			return;
		}

		if (phook.FutureProgs.Contains(prog))
		{
			if (!phook.RemoveProg(prog))
			{
				actor.OutputHandler.Send($"The {prog.MXPClickableFunctionName()} prog cannot be removed from that hook.");
				return;
			}

			phook.Changed = true;
			actor.OutputHandler.Send($"The {hook.Name.ColourName()} hook no longer executes the {prog.MXPClickableFunctionName()} prog.");
			return;
		}

		var info = hook.Type.GetAttribute<EventInfoAttribute>();
		if (info?.ProgTypes.CompatibleWith(prog.Parameters) != true)
		{
			actor.OutputHandler.Send($"The {prog.MXPClickableFunctionName()} prog is not compatible with the {hook.Type.DescribeEnum(colour: Telnet.Cyan)} event.");
			return;
		}

		phook.AddProg(prog);
		phook.Changed = true;
		actor.OutputHandler.Send($"The {hook.Name.ColourName()} hook now executes the {prog.MXPClickableFunctionName()} prog.");
	}

	private static void HookRename(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which hook do you want to change the name of?");
			return;
		}

		var hook = actor.Gameworld.Hooks.GetByIdOrName(ss.PopSpeech());
		if (hook is null)
		{
			actor.OutputHandler.Send("There is no such hook.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send($"What new name do you want to set for the hook {hook.Name.ColourName()}?");
			return;
		}

		var name = ss.SafeRemainingArgument.TitleCase();
		if (actor.Gameworld.Hooks.Any(x => x.Name.EqualTo(name)))
		{
			actor.Send("There is already a hook with that name. Hook names must be unique.");
			return;
		}

		actor.OutputHandler.Send($"You rename the hook {hook.Name.ColourName()} to {name.ColourName()}.");
		hook.Name = name;
		hook.Changed = true;
	}

	private static void HookInstall(ICharacter actor, StringStack input)
	{
		var targetText = input.PopSpeech();
		if (string.IsNullOrEmpty(targetText))
		{
			actor.OutputHandler.Send(
				"What do you want to install your hook on?\nYou can specify an item or character by keyword, or use 'here' to target your room.");
			return;
		}

		var target = targetText == "here" ? actor.Location : actor.Target(targetText);
		if (target == null)
		{
			actor.OutputHandler.Send("That is not a valid target.");
			return;
		}

		targetText = input.PopSpeech();
		if (string.IsNullOrEmpty(targetText))
		{
			actor.OutputHandler.Send($"Which hook do you want to install on {target.HowSeen(actor)}?");
			return;
		}

		var hook = actor.Gameworld.Hooks.GetByIdOrName(targetText);

		if (hook is null)
		{
			actor.OutputHandler.Send("There is no such hook. See HOOK LIST.");
			return;
		}

		if (target.InstallHook(hook))
		{
			target.HooksChanged = true;
			actor.OutputHandler.Send(
				$"Successfully installed hook {hook.Name.Colour(Telnet.Cyan)} on {target.HowSeen(actor)}.");
		}
		else
		{
			actor.OutputHandler.Send($"{target.HowSeen(actor, true)} already has that hook installed.");
		}
	}

	protected static void HookCreate(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("What name do you want to give to this hook?");
			return;
		}

		var name = ss.PopSpeech().TitleCase();
		if (actor.Gameworld.Hooks.Any(x => x.Name.EqualTo(name)))
		{
			actor.Send("There is already a hook with that name. Hook names must be unique.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("What prog should be executed when this hook is triggered?");
			return;
		}

		var prog = actor.Gameworld.FutureProgs.GetByIdOrName(ss.PopSpeech());
		if (prog == null)
		{
			actor.Send("There is no prog like that which you can use in a hook.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send(
				"Which event should this hook be calibrated to? See SHOW EVENTS for a list. CommandInput and SelfCommandInput are the most common choices.");
			return;
		}

		if (!ss.PopSpeech().TryParseEnum<EventType>(out var whichEvent))
		{
			actor.Send(
				$"There is no event with that number. See {"show events".MXPSend("show events")} for a list of events.");
			return;
		}

		var info = whichEvent.GetAttribute<EventInfoAttribute>();
		if (info == null)
		{
			actor.Send("That event has not yet been set up to be used with hooks.");
			return;
		}

		if (!info.ProgTypes.CompatibleWith(prog.Parameters))
		{
			actor.Send("That prog does not have the right parameters to be used with that hook.");
			return;
		}

		switch (whichEvent)
		{
			case EventType.CommandInput:
			case EventType.SelfCommandInput:
				if (ss.IsFinished)
				{
					actor.Send(
						"This type of hook requires an additional argument to supply the command it intercepts.");
					return;
				}

				var newHook = new HookOnInput(name, ss.Pop(), actor.Gameworld, whichEvent, prog);
				actor.Send($"You create command hook #{newHook.Id.ToString("N0", actor)} \"{name.ColourName()}\".");
				return;
		}

		var hook = new FutureProgHook(name, actor.Gameworld, whichEvent, prog);
		actor.Send($"You create hook #{hook.Id.ToString("N0", actor)} \"{name.ColourName()}\".");
		return;
	}

	private static void HookCategory(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which hook do you want to change the category of?");
			return;
		}

		var hook = actor.Gameworld.Hooks.GetByIdOrName(ss.PopSpeech());
		if (hook is null)
		{
			actor.OutputHandler.Send("There is no such hook.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send($"What category do you want to set for the hook {hook.Name.ColourName()}?");
			return;
		}

		hook.Category = ss.SafeRemainingArgument.TitleCase();
		hook.Changed = true;
		actor.OutputHandler.Send(
			$"The hook {hook.Name.ColourName()} is now categorised as {hook.Category.ColourValue()}.");
	}

	private static void HookRemove(ICharacter actor, StringStack ss)
	{
		var targetText = ss.PopSpeech();
		if (string.IsNullOrEmpty(targetText))
		{
			actor.OutputHandler.Send(
				"What do you want to remove a hook from?\nYou can specify an item or character by keyword, or use 'here' to target your room.");
			return;
		}

		var target = targetText == "here" ? actor.Location : actor.TargetLocal(targetText);
		if (target == null)
		{
			actor.OutputHandler.Send("That is not a valid target.");
			return;
		}

		targetText = ss.PopSpeech();
		if (string.IsNullOrEmpty(targetText))
		{
			actor.OutputHandler.Send($"Which hook do you want to remove from {target.HowSeen(actor)}?");
			return;
		}

		var hook = actor.Gameworld.Hooks.GetByIdOrName(targetText);

		if (hook is null)
		{
			actor.OutputHandler.Send("There is no such hook. See HOOK LIST.");
			return;
		}

		if (target.RemoveHook(hook))
		{
			target.HooksChanged = true;
			actor.OutputHandler.Send(
				$"Successfully removed hook {hook.Name.Colour(Telnet.Cyan)} from {target.HowSeen(actor)}.");
		}
		else
		{
			actor.OutputHandler.Send($"{target.HowSeen(actor, true)} did not have that hook installed.");
		}
	}

	private static void HookAddDefault(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which type do you want to add a default hook for?\nThe valid options are {"room".ColourValue()}, {"character".ColourValue()} and {"item".ColourValue()}");
			return;
		}

		string type;
		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "room":
			case "cell":
			case "location":
				type = "Cell";
				break;
			case "character":
			case "ch":
				type = "Character";
				break;
			case "item":
			case "gameitem":
			case "object":
				type = "GameItem";
				break;
			default:
				actor.OutputHandler.Send(
					$"That is not a valid type to install a default hook on.\nThe valid options are {"room".ColourValue()}, {"character".ColourValue()} and {"item".ColourValue()}");
				return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which hook do you want to use for your default hook?");
			return;
		}

		var hook = actor.Gameworld.Hooks.GetByIdOrName(ss.PopSpeech());
		if (hook is null)
		{
			actor.OutputHandler.Send("There is no such hook.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which prog do you want to use to control whether the hook is installed on an instance of that type?");
			return;
		}

		var prog = new ProgLookupFromBuilderInput(actor.Gameworld, actor, ss.SafeRemainingArgument,
			ProgVariableTypes.Boolean, type switch
			{
				"Character" => new ProgVariableTypes[] { ProgVariableTypes.Toon },
				"GameItem" => new ProgVariableTypes[] { ProgVariableTypes.Item },
				"Cell" => new ProgVariableTypes[] { ProgVariableTypes.Location },
				_ => new ProgVariableTypes[] { ProgVariableTypes.Error }
			}).LookupProg();
		if (prog is null)
		{
			return;
		}

		if (actor.Gameworld.DefaultHooks.Any(x =>
				x.PerceivableType.EqualTo(type) && x.Hook == hook && x.EligibilityProg == prog))
		{
			actor.OutputHandler.Send("There is already a default hook with that combination of parameters.");
			return;
		}

		var defaultHook = new Events.Hooks.DefaultHook(type, prog, hook);
		actor.Gameworld.Add(defaultHook);
		actor.OutputHandler.Send(
			$"You add a default hook to the {type.ColourName()} type which executes the {hook.Name.ColourName()} hook based on the {prog.MXPClickableFunctionName()} filter prog.");
	}

	private static void HookRemoveDefault(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which type do you want to remove a default hook from?\nThe valid options are {"room".ColourValue()}, {"character".ColourValue()} and {"item".ColourValue()}");
			return;
		}

		string type;
		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "room":
			case "cell":
			case "location":
				type = "Cell";
				break;
			case "character":
			case "ch":
				type = "Character";
				break;
			case "item":
			case "gameitem":
			case "object":
				type = "GameItem";
				break;
			default:
				actor.OutputHandler.Send(
					$"That is not a valid type to remove a default hook from.\nThe valid options are {"room".ColourValue()}, {"character".ColourValue()} and {"item".ColourValue()}");
				return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which hook does the default hook you want to remove use?");
			return;
		}

		var hook = actor.Gameworld.Hooks.GetByIdOrName(ss.PopSpeech());
		if (hook is null)
		{
			actor.OutputHandler.Send("There is no such hook.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which prog does the default hook you want to remove use as a filter?");
			return;
		}

		var prog = new ProgLookupFromBuilderInput(actor.Gameworld, actor, ss.SafeRemainingArgument,
			ProgVariableTypes.Boolean, type switch
			{
				"Character" => new ProgVariableTypes[] { ProgVariableTypes.Toon },
				"GameItem" => new ProgVariableTypes[] { ProgVariableTypes.Item },
				"Cell" => new ProgVariableTypes[] { ProgVariableTypes.Location },
				_ => new ProgVariableTypes[] { ProgVariableTypes.Error }
			}).LookupProg();
		if (prog is null)
		{
			return;
		}

		var defaultHook = actor.Gameworld.DefaultHooks.FirstOrDefault(x =>
			x.PerceivableType.EqualTo(type) && x.Hook == hook && x.EligibilityProg == prog);
		if (defaultHook is null)
		{
			actor.OutputHandler.Send("There is no default hook with that combination of parameters.");
			return;
		}

		defaultHook.Delete();
		actor.Gameworld.Destroy(defaultHook);
		actor.OutputHandler.Send(
			$"You remove the default hook from the {type.ColourName()} type which executes the {hook.Name.ColourName()} hook based on the {prog.MXPClickableFunctionName()} filter prog.\n{"Warning: It is strongly recommended that you reboot IMMEDIATELY after running this command.".ColourError()}");
	}

	private static void HookDefaults(ICharacter actor, StringStack ss)
	{
		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from hook in actor.Gameworld.DefaultHooks
			select new List<string>
			{
				hook.Hook.Id.ToString("N0", actor),
				hook.Hook.Name,
				hook.PerceivableType,
				hook.EligibilityProg.MXPClickableFunctionName()
			},
			new List<string>
			{
				"Hook Id",
				"Hook Name",
				"Perceivable Type",
				"Eligibility Prog"
			},
			actor,
			Telnet.Green
		));
	}

	private static void HookList(ICharacter actor, StringStack ss)
	{
		var hooks = actor.Gameworld.Hooks.AsEnumerable();
		while (!ss.IsFinished)
		{
			var cmd = ss.PopSpeech();
			if (cmd[0] == '*')
			{
				var prog = actor.Gameworld.FutureProgs.GetByIdOrName(cmd.Substring(1));
				if (prog is null)
				{
					actor.OutputHandler.Send("There is no such prog to filter by.");
					return;
				}

				hooks = hooks.OfType<IHookWithProgs>().Where(x => x.FutureProgs.Contains(prog));
				continue;
			}

			if (!cmd.TryParseEnum<EventType>(out var type))
			{
				actor.OutputHandler.Send(
					$"That is not a valid event to filter by. See {"show events".MXPSend("show events")} for a list of events.");
				return;
			}

			hooks = hooks.Where(x => x.Type == type);
		}

		var sb = new StringBuilder();
		sb.AppendLine($"Hook List");
		sb.AppendLine();
		sb.AppendLine(StringUtilities.GetTextTable(
			from hook in hooks
			let chook = hook as ICommandHook
			select new List<string>
			{
				hook.Id.ToString("N0", actor),
				hook.Name,
				hook.Category,
				hook.Type.DescribeEnum(),
				chook?.CommandText ?? "",
				hook.InfoForHooklist
			},
			new List<string>
			{
				"Id",
				"Name",
				"Category",
				"Event",
				"Command",
				"Payload"
			},
			actor,
			Telnet.Green
		));
		actor.OutputHandler.Send(sb.ToString());
	}

	#endregion

	#region Schedules
	[PlayerCommand("Schedules", "schedules")]
	[CommandPermission(PermissionLevel.Admin)]
	protected static void Schedules(ICharacter actor, string input)
	{
		actor.Send(StringUtilities.GetTextTable(
			from schedule in actor.Gameworld.ProgSchedules
			select
				new[]
				{
					schedule.Id.ToString("N0", actor), 
					schedule.Prog.MXPClickableFunctionNameWithId(),
					schedule.Interval.Describe(schedule.NextReferenceTime.Calendar).ColourCommand(),
					schedule.NextReferenceTime.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()
				},
			new[] { "ID", "Prog", "Interval", "Next Time" },
			actor,
			colour: Telnet.Green
		));
	}

	private const string ScheduleHelp = @"The schedule command is used to create schedules that execute progs at a recurring interval of in-character time.

The syntax to use this command is as follows:

	#3schedule add <prog> ""every <interval>"" [<datetime>]#0 - adds a schedule
	#3schedule remove <id>#0 - removes a schedule

The interval will be text in one of the following formats:

	#6every <x> minutes#0
	#6every <x> hours#0
	#6every <x> days#0
	#6every <x> weekdays#0
	#6every <x> weeks#0
	#6every <x> months#0
	#6every <x> years#0

Note that these are in-game time periods, not real life time periods. You should take note of your MUD Time to Real Time Ratio.

The datetime argument will be in the following format:

	#6[timezone] day/month/year hour:minute:second [meridian]#0

The arguments #6timezone#0 and #6meridian#0 are optional. Note that #6month#0 must be the name of the month not the number (e.g. May not 5)

For example:
	
	#615/january/1972 11:32:00 am#0
	#65/martius/552 18:00:00#0

If no datetime argument is supplied with the #3schedule add#0 command, the current location's time and date will be used.";

	[PlayerCommand("Schedule", "schedule")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("schedule", ScheduleHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void Schedule(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "add":
				if (ss.IsFinished)
				{
					actor.Send("Which prog do you want to schedule?");
					return;
				}

				var prog = actor.Gameworld.FutureProgs.GetByIdOrName(ss.PopSpeech());
				if (prog == null)
				{
					actor.Send("There is no such prog for you to schedule.");
					return;
				}

				if (!prog.AcceptsAnyParameters && prog.Parameters.Any())
				{
					actor.Send("You can only schedule progs that accept any parameters or have no parameters.");
					return;
				}

				if (ss.IsFinished)
				{
					actor.Send("How often should this prog fire?");
					return;
				}

				var intervalText = ss.PopSpeech();
				if (!RecurringInterval.TryParse(intervalText, out var interval))
				{
					actor.OutputHandler.Send(
						$"That is not a valid interval.\n{"Use the following form: every <x> minutes|hours|days|weekdays|weeks|months|years <offset>".ColourCommand()}");
					return;
				}

				var dt = actor.Location.DateTime();
				if (!ss.IsFinished)
				{
					if (!MudDateTime.TryParse(ss.SafeRemainingArgument, actor.Location.Calendars.First(),
							actor.Location.Clocks.First(), actor, out dt, out var error))
					{
						actor.OutputHandler.Send($"That is not a valid date time.\n{error}");
						return;
					}
				}

				var newSchedule = new FutureProg.ProgSchedule(dt, interval, prog);
				actor.Gameworld.Add(newSchedule);
				actor.OutputHandler.Send($"You create schedule #{newSchedule.Id.ToString("N0", actor)} to execute {prog.MXPClickableFunctionName()} {interval.Describe(actor.Location.Calendars.First()).ColourCommand()} starting from {dt.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()}.");
				return;
			case "remove":
				if (ss.IsFinished)
				{
					actor.OutputHandler.Send("Which schedule would you like to remove?");
					return;
				}

				if (!long.TryParse(ss.SafeRemainingArgument, out var id))
				{
					actor.OutputHandler.Send("You must enter a valid ID number.");
					return;
				}

				var schedule = actor.Gameworld.ProgSchedules.Get(id);
				if (schedule is null)
				{
					actor.OutputHandler.Send("There is no such schedule.");
					return;
				}

				actor.OutputHandler.Send($"You delete prog schedule #{schedule.Id.ToString("N0", actor)}, which executed {schedule.Prog.MXPClickableFunctionName()} {schedule.Interval.Describe(schedule.NextReferenceTime.Calendar).ColourCommand()} (next would have been {schedule.NextReferenceTime.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()}).");
				schedule.Delete();
				return;
			default:
				actor.OutputHandler.Send(ScheduleHelp.SubstituteANSIColour());
				return;
		}
	}
	#endregion
}