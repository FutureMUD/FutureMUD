﻿using MudSharp.Character;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using MoreLinq.Extensions;
using MudSharp.Accounts;
using MudSharp.Character.Name;
using MudSharp.CharacterCreation;
using MudSharp.Commands.Helpers;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.RPG.Merits;

namespace MudSharp.Commands.Modules;

internal class ChargenModule : Module<ICharacter>
{
	private ChargenModule()
		: base("Chargen")
	{
		IsNecessary = true;
	}

	public static ChargenModule Instance { get; } = new();

	#region Chargen
	public const string ChargenHelpText =
		@"The chargen command is used to show and view character applications, as well as editing the character creation experience. Note that this command isn't the correct one for reviewing applications - instead see the #3APPLICATIONS#0 command for this functionality.

You can use the following options with this command:

	#3chargen list [<filters>]#0 - lists all of the character applications. See below for filtering options.
	#3chargen view <id>#0 - shows the 'submit' screen of an application you specify
	#3chargen overview#0 - shows the overall plan for character creation and screens
	#3chargen show <id|name>#0 - shows details about a storyboard
	#3chargen edit <id|name>#0 - opens a storyboard for editing
	#3chargen edit#0 - an alias for #3CHARGEN SHOW#0 for your currently edited storyboard
	#3chargen close#0 - closes the currently editing storyboard
	#3chargen set ...#0 - edits the property of a storyboard. See type help for more info.
	#3chargen types#0 - lists all of the types of chargen screens that can be set
	#3chargen typehelp <type>#0 - shows help for a screen storyboard type
	#3chargen changetype <stage> <new type>#0 - changes the type of screen for a particular stage

Filters for list:

	#Bspecial#0 - only show special applications
	#B!special#0 - don't show special applications
	#B*<accountname>#0 - search for submissions from the specified account
	#B%<accountname>#0 - search for submissions approved by the specified account
	#B+<keyword>#0 - names or short descriptions containing the keyword
	#B-<keyword>#0 - names or short descriptions NOT containing the keyword
	#BInProgress|Submitted|Approved#0 - show applications only in the specified state";

	[PlayerCommand("Chargen", "chargen")]
	[CommandPermission(PermissionLevel.SeniorAdmin)]
	[HelpInfo("chargen", ChargenHelpText, AutoHelp.HelpArgOrNoArg)]
	protected static void Chargen(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		switch (ss.PopSpeech())
		{
			case "list":
				ChargenList(actor, ss);
				return;
			case "show":
				ChargenShow(actor, ss);
				return;
			case "view":
				ChargenView(actor, ss);
				return;
			case "edit":
				ChargenEdit(actor, ss);
				return;
			case "close":
				ChargenClose(actor, ss);
				return;
			case "types":
				ChargenTypes(actor, ss);
				return;
			case "typehelp":
				ChargenTypeHelp(actor, ss);
				return;
			case "overview":
				ChargenOverview(actor, ss);
				return;
			case "reorder":
				ChargenReorder(actor, ss);
				return;
			case "set":
				ChargenSet(actor, ss);
				return;
			case "changetype":
				ChargenChangeType(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(ChargenHelpText.SubstituteANSIColour());
				return;
		}
	}

	private static void ChargenChangeType(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send($"Which stage do you want to change the screen type for?");
			return;
		}

		if (!ss.PopSpeech().TryParseEnum(out ChargenStage stage))
		{
			actor.OutputHandler.Send("That is not a valid character creation stage.");
			return;
		}

		var types = ChargenStoryboard.ChargenStageTypeInfos.Where(x => x.Stage == stage).ToList();
		if (types.Count == 1)
		{
			actor.OutputHandler.Send(
				$"There are no alternative screen types for the {stage.DescribeEnum().ColourName()} at this stage.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which alternative screen are you proposing to swap to?");
			return;
		}

		var typeName = ss.SafeRemainingArgument;
		var type = types.FirstOrDefault(x => x.Name.StartsWith(typeName, StringComparison.InvariantCultureIgnoreCase));
		if (type == default)
		{
			actor.OutputHandler.Send(
				$"There is no such chargen screen type for the {stage.DescribeEnum().ColourName()} stage. The valid options are:\n\n{types.Select(x => x.Name.ColourName()).ListToLines(true)}");
			return;
		}

		if (actor.Gameworld.ChargenStoryboard.StageScreenMap[stage].Name == type.Name)
		{
			actor.OutputHandler.Send(
				$"That is already the screen type for the {stage.DescribeEnum().ColourName()} stage.");
			return;
		}

		actor.OutputHandler.Send(
			$@"You are proposing to change the type of screen storyboard that manages the {stage.DescribeEnum().ColourName()} stage from {actor.Gameworld.ChargenStoryboard.StageScreenMap[stage].Name.ColourValue()} to {type.Name.ColourValue()}.

{"Warning: This may cause irreversible data loss where the screens have fundamentally different settings. The engine will attempt to match the data as closely as possible where it can, but few of the screens are perfectly aligned and you will need to rebuild them if you decide to switch back.".ColourError()}

{"It is strongly recommended that you have locked down character creation using WIZLOCK CHARGEN and also ensured that nobody is logged into character creation until you've finished rebuilding the screen.".ColourError()}

{Accept.StandardAcceptPhrasing}");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			DescriptionString = "Swapping out the chargen screen type of a stage",
			AcceptAction = text =>
			{
				if (actor.Gameworld.ChargenStoryboard.StageScreenMap[stage].Name == type.Name)
				{
					actor.OutputHandler.Send(
						$"That is already the screen type for the {stage.DescribeEnum().ColourName()} stage.");
					return;
				}

				actor.OutputHandler.Send(
					$@"You change the storyboard that manages the {stage.DescribeEnum().ColourName()} stage from {actor.Gameworld.ChargenStoryboard.StageScreenMap[stage].Name.ColourValue()} to {type.Name.ColourValue()}.

Note - this may have caused data loss and the screen will likely need immediate attention before it is usable.

You are now editing this new storyboard.");
				actor.Gameworld.ChargenStoryboard.SwapStoryboard(stage, type.Name);
				actor.RemoveAllEffects<BuilderEditingEffect<IChargenScreenStoryboard>>();
				actor.AddEffect(new BuilderEditingEffect<IChargenScreenStoryboard>(actor)
					{ EditingItem = actor.Gameworld.ChargenStoryboard.StageScreenMap[stage] });
			},
			RejectAction = text =>
			{
				actor.OutputHandler.Send("You decide not to change the character creation screen type.");
			},
			ExpireAction = () =>
			{
				actor.OutputHandler.Send("You decide not to change the character creation screen type.");
			}
		}), TimeSpan.FromSeconds(120));
	}

	private static void ChargenTypeHelp(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which type do you want to see type help for? The valid options are:\n\n{ChargenStoryboard.ChargenStageTypeInfos.OrderBy(x => x.Stage).Select(x => x.Name.ColourName()).ListToLines(true)}");
			return;
		}

		var typeName = ss.SafeRemainingArgument;
		var type = ChargenStoryboard.ChargenStageTypeInfos.FirstOrDefault(x =>
			x.Name.StartsWith(typeName, StringComparison.InvariantCultureIgnoreCase));
		if (type == default)
		{
			actor.OutputHandler.Send(
				$"There is no such chargen screen type. The valid options are:\n\n{ChargenStoryboard.ChargenStageTypeInfos.OrderBy(x => x.Stage).Select(x => x.Name.ColourName()).ListToLines(true)}");
			return;
		}

		actor.OutputHandler.Send($@"#6Chargen Screen Help for Screen Type ""{type.Name}""#0

Stage: {type.Stage.DescribeEnum().ColourValue()}
Description: {type.Description.ColourValue()}

{type.HelpText}".SubstituteANSIColour());
	}

	private static void ChargenShow(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IChargenScreenStoryboard>>()
			                  .FirstOrDefault();
			if (effect is null)
			{
				actor.OutputHandler.Send("Which storyboard would you like to show?");
				return;
			}

			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		var screen =
			actor.Gameworld.ChargenStoryboard.StageScreenMap.Values.GetByIdOrName(ss.SafeRemainingArgument);
		if (screen is null)
		{
			actor.OutputHandler.Send("There is no such storyboard for you to show.");
			return;
		}

		actor.OutputHandler.Send(screen.Show(actor));
	}

	private static void ChargenSet(ICharacter actor, StringStack ss)
	{
		var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IChargenScreenStoryboard>>()
		                  .FirstOrDefault();
		if (effect is null)
		{
			actor.OutputHandler.Send("You are not editing any storyboards.");
			return;
		}

		effect.EditingItem.BuildingCommand(actor, ss);
	}

	private static void ChargenReorder(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which storyboard would you like to change the order of?");
			return;
		}

		var screen =
			actor.Gameworld.ChargenStoryboard.StageScreenMap.Values.GetByIdOrName(ss.PopSpeech());
		if (screen is null)
		{
			actor.OutputHandler.Send("There is no such storyboard for you to reorder.");
			return;
		}

		if (screen.Stage.In(ChargenStage.Menu, ChargenStage.None, ChargenStage.ConfirmQuit, ChargenStage.Submit,
			    ChargenStage.Welcome))
		{
			actor.OutputHandler.Send("You cannot change the order of this storyboard type.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which other screen would you like to put this one after?");
			return;
		}

		var otherScreen =
			actor.Gameworld.ChargenStoryboard.StageScreenMap.Values.GetByIdOrName(ss.SafeRemainingArgument);
		if (otherScreen is null)
		{
			actor.OutputHandler.Send("There is no such storyboard for you to place the other one ahead of.");
			return;
		}

		if (otherScreen.Stage.In(ChargenStage.Menu, ChargenStage.None, ChargenStage.ConfirmQuit, ChargenStage.Submit))
		{
			actor.OutputHandler.Send("This stage cannot be the target of a reorder request.");
			return;
		}

		var isPrevious = true;
		foreach (var stage in actor.Gameworld.ChargenStoryboard.DefaultOrder)
		{
			if (stage == screen.Stage)
			{
				continue;
			}

			if (isPrevious && actor.Gameworld.ChargenStoryboard.StageDependencies[stage].Contains(screen.Stage))
			{
				actor.OutputHandler.Send(
					$"You cannot make that change because the {actor.Gameworld.ChargenStoryboard.StageScreenMap[stage].Name.ColourName()} has a dependency on the {screen.Name.ColourName()} screen, and this would put it after it.");
				return;
			}

			if (!isPrevious && actor.Gameworld.ChargenStoryboard.StageDependencies[screen.Stage].Contains(stage))
			{
				actor.OutputHandler.Send(
					$"You cannot make that change because the {screen.Name.ColourName()} has a dependency on the {actor.Gameworld.ChargenStoryboard.StageScreenMap[stage].Name.ColourName()} screen, and this would put it after it.");
				return;
			}

			if (stage == otherScreen.Stage)
			{
				isPrevious = false;
				continue;
			}
		}

		actor.Gameworld.ChargenStoryboard.ReorderStage(screen.Stage, otherScreen.Stage);
		actor.OutputHandler.Send(
			$"You change the order of the {screen.Name.ColourName()} stage to be after the {otherScreen.Name.ColourName()} stage.");
	}

	private static void ChargenOverview(ICharacter actor, StringStack ss)
	{
		var sb = new StringBuilder();
		sb.AppendLine("Chargen Overview".ColourName());
		sb.AppendLine(
			StringUtilities.GetTextTable(
				from storyboard in actor.Gameworld.ChargenStoryboard.StageScreenMap.Values
				orderby actor.Gameworld.ChargenStoryboard.OrderOf(storyboard.Stage)
				select new List<string>
				{
					storyboard.Id.ToString("N0", actor),
					storyboard.Name,
					storyboard.Stage.DescribeEnum(),
					actor.Gameworld.ChargenStoryboard.StageDependencies[storyboard.Stage]
					     .OrderBy(x => storyboard.Gameworld.ChargenStoryboard.OrderOf(x)).Select(x => x.DescribeEnum())
					     .ListToCommaSeparatedValues(", ")
				},
				new List<string>
				{
					"Id",
					"Type",
					"Stage",
					"Dependencies"
				},
				actor,
				Telnet.Green
			)
		);
		actor.OutputHandler.Send(sb.ToString());
	}

	private static void ChargenTypes(ICharacter actor, StringStack ss)
	{
		var sb = new StringBuilder();
		sb.AppendLine("There are the following storyboard types associated with each stage:");
		sb.AppendLine();
		sb.AppendLine(StringUtilities.GetTextTable(
			from item in ChargenStoryboard.ChargenStageTypeInfos.OrderBy(x => x.Stage)
			select new List<string>
			{
				item.Name,
				item.Stage.DescribeEnum(),
				item.Description
			},
			new List<string>
			{
				"Name",
				"Stage",
				"Description"
			},
			actor,
			Telnet.Cyan,
			2
		));
		actor.OutputHandler.Send(sb.ToString());
	}

	private static void ChargenClose(ICharacter actor, StringStack ss)
	{
		var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IChargenScreenStoryboard>>()
		                  .FirstOrDefault();
		if (effect is null)
		{
			actor.OutputHandler.Send("You are not editing any storyboards.");
			return;
		}

		actor.RemoveAllEffects(x => x is BuilderEditingEffect<IChargenScreenStoryboard>);
		actor.OutputHandler.Send($"You are no longer editing any storyboards.");
	}

	private static void ChargenEdit(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IChargenScreenStoryboard>>()
			                  .FirstOrDefault();
			if (effect is null)
			{
				actor.OutputHandler.Send("Which storyboard would you like to edit?");
				return;
			}

			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		var screen =
			actor.Gameworld.ChargenStoryboard.StageScreenMap.Values.GetByIdOrName(ss.SafeRemainingArgument);
		if (screen is null)
		{
			actor.OutputHandler.Send("There is no such storyboard for you to edit.");
			return;
		}

		actor.RemoveAllEffects(x => x is BuilderEditingEffect<IChargenScreenStoryboard>);
		actor.AddEffect(new BuilderEditingEffect<IChargenScreenStoryboard>(actor) { EditingItem = screen });
		actor.OutputHandler.Send($"You are now editing the storyboard {screen.Name.ColourName()}.");
	}

	private static void ChargenView(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which character application do you wish to review?");
			return;
		}

		if (!long.TryParse(ss.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send(
				"You must supply a valid ID number of the character application you wish to review.");
			return;
		}

		IChargen chargen = null;
		using (new FMDB())
		{
			var dbitem = FMDB.Context.Chargens.Find(value);
			if (dbitem is null)
			{
				actor.OutputHandler.Send("There is no such character application.");
				return;
			}

			chargen = new Chargen(dbitem, actor.Gameworld, dbitem.Account);
		}

		actor.OutputHandler.Send(chargen.DisplayForReview(actor.Account, actor.PermissionLevel));
	}

	private static void ChargenList(ICharacter actor, StringStack ss)
	{
		var sb = new StringBuilder();
		var filters = new List<string>();
		sb.AppendLine("Character Creation Records".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		List<IChargen> chargens;
		using (new FMDB())
		{
			chargens = FMDB.Context.Chargens
			               .Include(x => x.Account)
			               .ThenInclude(x => x.AccountsChargenResources)
			               .Select(x => new Chargen(x, actor.Gameworld, x.Account))
			               .ToList<IChargen>();
		}

		while (!ss.IsFinished)
		{
			var cmd = ss.PopSpeech();
			if (cmd.Length == 0)
			{
				continue;
			}

			if (cmd.EqualTo("special"))
			{
				chargens = chargens.Where(x => x.ApplicationType == ApplicationType.Special).ToList();
				filters.Add($"is a special application");
				continue;
			}

			if (cmd.EqualTo("!special"))
			{
				chargens = chargens.Where(x => x.ApplicationType != ApplicationType.Special).ToList();
				filters.Add($"is not a special application");
				continue;
			}

			if (cmd.TryParseEnum<ChargenState>(out var state))
			{
				chargens = chargens.Where(x => x.State == state).ToList();
				filters.Add($"in state {state.DescribeEnum().ColourName()}");
				continue;
			}

			if (cmd[0] == '%')
			{
				cmd = cmd.Substring(1);
				chargens = chargens
				           .Where(x => x.ApprovedBy?.Name.EqualTo(cmd) == true)
				           .ToList();
				filters.Add($"from account {cmd.Colour(Telnet.BoldYellow)}");
				continue;
			}

			if (cmd[0] == '*')
			{
				cmd = cmd.Substring(1);
				chargens = chargens
				           .Where(x =>
					           x.Account.Name.EqualTo(cmd)
				           )
				           .ToList();
				filters.Add($"from account {cmd.Colour(Telnet.BoldYellow)}");
				continue;
			}

			if (cmd[0] == '+')
			{
				cmd = cmd.Substring(1);
				chargens = chargens
				           .Where(x =>
					           x.Account.Name.Contains(cmd, StringComparison.InvariantCultureIgnoreCase) ||
					           x.SelectedName?.GetName(NameStyle.FullWithNickname)
					            .Contains(cmd, StringComparison.InvariantCultureIgnoreCase) == true ||
					           x.SelectedSdesc?.Contains(cmd, StringComparison.InvariantCultureIgnoreCase) == true
				           )
				           .ToList();
				filters.Add($"containing keyword {cmd.Colour(Telnet.BoldYellow)}");
				continue;
			}

			if (cmd[0] == '-')
			{
				cmd = cmd.Substring(1);
				chargens = chargens
				           .Where(x =>
					           !x.Account.Name.Contains(cmd, StringComparison.InvariantCultureIgnoreCase) &&
					           x.SelectedName?.GetName(NameStyle.FullWithNickname)
					            .Contains(cmd, StringComparison.InvariantCultureIgnoreCase) != true &&
					           x.SelectedSdesc?.Contains(cmd, StringComparison.InvariantCultureIgnoreCase) != true
				           )
				           .ToList();
				filters.Add($"not containing keyword {cmd.Colour(Telnet.BoldYellow)}");
				continue;
			}

			actor.OutputHandler.Send($"Couldn't figure out how to interpret filter argument {cmd.ColourCommand()}.");
			return;
		}

		if (filters.Any())
		{
			sb.AppendLine();
			sb.AppendLine("Filters:");
			foreach (var filter in filters)
			{
				sb.AppendLine($"\t{filter.ColourIncludingReset(Telnet.Yellow)}");
			}

			sb.AppendLine();
		}

		sb.AppendLine(
			StringUtilities.GetTextTable(
				from chargen in chargens
				select new List<string>
				{
					chargen.Id.ToString("N0", actor),
					chargen.Account.Name,
					chargen.SelectedName?.GetName(NameStyle.FullName) ?? "",
					chargen.SelectedRace?.Name ?? "",
					chargen.SelectedCulture?.Name ?? "",
					chargen.SelectedEthnicity?.Name ?? "",
					chargen.ApplicationCosts.Where(x => x.Value > 0)
					       .Select(x => $"{x.Value.ToString("N0", actor)} {x.Key.Alias}".ColourValue())
					       .ListToCommaSeparatedValues(", "),
					chargen.ApplicationType.DescribeEnum(),
					chargen.State.DescribeEnum()
				},
				new List<string>
				{
					"Id",
					"Account",
					"Name",
					"Race",
					"Culture",
					"Ethnicity",
					"Costs",
					"Type",
					"State"
				},
				actor,
				Telnet.Cyan
			)
		);
		sb.AppendLine($"Total Records: {chargens.Count.ToString("N0", actor).ColourValue()}");


		actor.OutputHandler.Send(sb.ToString());
	}
#endregion

	#region Intro Templates
	public const string IntroTemplateHelp = @"This command is used to edit #6Character Intro Templates#0, which are a series of echoes that are shown to characters when they log in for the first time after character creation.

Each character will be shown a maximum of one introduction, and they will be shown the first valid one with the highest priority (numerically).

You can use the following syntax with this command:

	#3introtemplate list#0 - lists all templates
	#3introtemplate show <id|name>#0 - shows a template
	#3introtemplate show#0 - an alias for showing your currently edited template
	#3introtemplate edit <which>#0 - begin editing a particular existing template
	#3introtemplate edit#0 - an alias for showing your currently edited template
	#3introtemplate close#0 - stop editing your current template
	#3introtemplate new <name>#0 - creates and begins editing a new intro template
	#3introtemplate clone <old> <newName>#0 - clones and begins editing a template
	#3introtemplate set name <name>#0 - changes the name of this template
	#3introtemplate set priority <##>#0 - sets the evaluation priority when deciding which to apply (higher number = higher priority)
	#3introtemplate set prog <prog>#0 - sets the prog that controls whether this is a valid template for a character
	#3introtemplate set echo add <seconds>#0 - drops you into an editor to write a new echo that lasts for the specified seconds amount
	#3introtemplate set echo add <seconds> <text>#0 - directly enters an echo without going into the editor
	#3introtemplate set echo remove <##>#0 - permanently deletes an echo
	#3introtemplate set echo text <##>#0 - drops you into an editor to edit a specific echo
	#3introtemplate set echo text <##> <text>#0 - directly overwrites an echo without going into the editor
	#3introtemplate set echo delay <##> <seconds>#0 - adjusts the delay on an echo
	#3introtemplate set echo swap <##> <##>#0 - swaps the order of two echoes";

	[PlayerCommand("IntroTemplate", "introtemplate")]
	[HelpInfo("IntroTemplate", IntroTemplateHelp, AutoHelp.HelpArgOrNoArg)]
	[CommandPermission(PermissionLevel.SeniorAdmin)]
	protected static void IntroTemplate(ICharacter actor, string command)
	{
		BaseBuilderModule.GenericBuildingCommand(actor, new StringStack(command.RemoveFirstWord()), EditableItemHelper.CharacterIntroTemplateHelper);
	}
	#endregion

	#region Merits
	public const string MeritHelpText = @"This command is used to create and edit merits (also known as flaws or quirks). These can be added to items and characters to give them various effects.

You can use the following commands to work with merits:

	#3merit list#0 - lists all merits
	#3merit show <id|name>#0 - shows a merit
	#3merit show#0 - an alias for showing your currently edited merit
	#3merit edit <which>#0 - begin editing a particular existing merit
	#3merit edit#0 - an alias for showing your currently edited merit
	#3merit close#0 - stop editing your current merit
	#3merit edit new <type> <name>#0 - creates and begins editing a new merit
	#3merit clone <old> <newName>#0 - clones and begins editing a merit
	#3merit types#0 - shows a list of merit types
	#3merit typehelp <type>#0 - shows the help text for a merit type
	#3merit set ...#0 - edits the properties of a merit. See the type help for more info.";

	[PlayerCommand("Merit", "merit")]
	[HelpInfo("merit", MeritHelpText, AutoHelp.HelpArgOrNoArg)]
	[CommandPermission(PermissionLevel.Admin)]
	protected static void Merit(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.PeekSpeech().EqualTo("types"))
		{
			MeritTypes(actor);
			return;
		}

		if (ss.PeekSpeech().EqualTo("typehelp"))
		{
			ss.PopSpeech();
			MeritTypeHelp(actor, ss);
			return;
		}

		BaseBuilderModule.GenericBuildingCommand(actor, new StringStack(command.RemoveFirstWord()), EditableItemHelper.MeritHelper);
	}

	private static void MeritTypeHelp(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which merit type do you want to view help for? See {"merit types".MXPSend("merit types")} for a complete list.");
			return;
		}

		var type = MeritFactory.TypeHelps.FirstOrDefault(x => x.Type.EqualTo(command.SafeRemainingArgument));
		if (string.IsNullOrEmpty(type.HelpText))
		{
			actor.OutputHandler.Send($"There is no merit type identified by the text {command.SafeRemainingArgument.ColourCommand()}. See {"merit types".MXPSend("merit types")} for a complete list.");
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine($"Merit Type Help - {type.Type}".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Blurb: {type.Blurb.ColourCommand()}");
		sb.AppendLine();
		sb.AppendLine(type.HelpText.SubstituteANSIColour());
		actor.OutputHandler.Send(sb.ToString());
	}

	private static void MeritTypes(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"There are the following merit types:");
		sb.AppendLine();
		sb.AppendLine(StringUtilities.GetTextTable(
			from item in MeritFactory.TypeHelps
			orderby item.Type
			select new List<string>
			{
				item.Type.TitleCase(),
				item.Blurb
			},
			new List<string>
			{
				"Type",
				"Blurb"
			},
			actor,
			Telnet.Cyan
		));
		actor.OutputHandler.Send(sb.ToString());
	}
	#endregion

	#region Chargen Resources

	public const string ChargenResourceHelp = @"The #3chargenresource#0 command is used to edit and create chargen resources, which are quantities that accrue or are awarded at an account level and would typically be used to unlock options during character creation (hence the name).

You can use the follow syntax with this command:

	#3cg list#0 - lists all chargen resources
	#3cg show <id|name>#0 - shows a chargen resource
	#3cg show#0 - an alias for showing your currently edited chargen resource
	#3cg edit <which>#0 - begin editing a particular existing chargen resource
	#3cg edit#0 - an alias for showing your currently edited chargen resource
	#3cg close#0 - stop editing your current chargen resource
	#3cg edit new <type> <name> <plural> <alias>#0 - creates and begins editing a new chargen resource
	#3cg set name <name>#0 - renames the resource
	#3cg set plural <name>#0 - sets the plural name
	#3cg set alias <alias>#0 - sets the alias
	#3cg set score#0 - toggles the resource showing in player scores
	#3cg set permission <level>#0 - sets the permission level required to award
	#3cg set permissiontime <level>#0 - sets the permission level required to skip the wait time
	#3cg set time <timespan>#0 - sets the time between awards
	#3cg set max <amount>#0 - sets the maximum amount that's awarded each award
	#3cg set control <prog>#0 - sets the prog that determines eligibility for automatic awards
	#3cg set resource <which>#0 - sets another resource that is used as a variable in the cap formula
	#3cg set formula <formula>#0 - sets the formula used to control the cap on amount";

	[PlayerCommand("ChargenResource", "chargenresource", "cr")]
	[HelpInfo("chargenresource", ChargenResourceHelp, AutoHelp.HelpArgOrNoArg)]
	[CommandPermission(PermissionLevel.SeniorAdmin)]
	protected static void ChargenResource(ICharacter actor, string command)
	{
		BaseBuilderModule.GenericBuildingCommand(actor, new StringStack(command.RemoveFirstWord()), EditableItemHelper.ChargenResourceHelper);
	}
	#endregion
}