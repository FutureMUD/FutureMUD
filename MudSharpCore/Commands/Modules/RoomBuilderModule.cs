using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using MoreLinq.Extensions;
using MudSharp.Accounts;
using MudSharp.Celestial;
using MudSharp.Character;
using MudSharp.Commands.Helpers;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Database;
using MudSharp.Editor;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Units;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;
using MudSharp.Models;
using MudSharp.OpenAI;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using ProgSchedule = MudSharp.FutureProg.ProgSchedule;

namespace MudSharp.Commands.Modules;

internal class RoomBuilderModule : Module<ICharacter>
{
	private RoomBuilderModule()
		: base("Room Builder")
	{
		IsNecessary = true;
	}

	public static RoomBuilderModule Instance { get; } = new();

	public static List<ICell> BuiltCells { get; } = new();

#nullable enable
	public static ICell? LookupCell(IFuturemud gameworld, string cellText)
	{
		if (string.IsNullOrEmpty(cellText))
		{
			return null;
		}

		if (cellText[0] == '@' && cellText.Length > 1 && int.TryParse(cellText[1..], out var index) &&
			BuiltCells.Count >= index && index > 0)
		{
			return BuiltCells[Index.FromEnd(index)];
		}

		return gameworld.Cells.GetByIdOrName(cellText);
	}
#nullable restore

	private const string CellHelpText = @"
#5Introduction#0

This command is used primarily to build cells (also known as ""rooms"", or ""locations""). Almost all uses of this command require you to be editing a #2cell overlay package#0. If you are not familiar with these, you should read the section on them below before proceeding with this command.

#5Cell Overlay Packages#0

A cell overlay package is used to allow building to take place on the ""live"" game server, and also permit review, roll-back and multiple versions of room building to exist. All cell-based building begins with a cell overlay package. 

While you have a package open, any changes you make to the location are made to an ""overlay"" of the cell that's stored in the package. It's not until you're done building, the package has been submitted, reviewed and swapped in as the ""current"" package that any of the building appears to anyone but you.

The key process is as follows: #6Open a Cell Overlay Package#3 -> #6Do your building#3 -> #6Submit the package#3 -> #6Someone approves the package#3 -> #6The package is swapped in and becomes live#0.

Where possible you should prefer to revise existing cell overlay packages instead of creating new ones, as the latest approved version of the package will be used for any swap commands or progs. This is especially true if you have event-based special building where you have multiple versions of a room.

#5Cell Commands#0

The following commands do not require you to have adopted an overlay package:

	#3cell show#0 - shows builder-specific info about the location you are in
	#3cell overlay <id> [<revnum>]#0 - temporarily adopt a specific package so you can view the world as if it were live
	#3cell overlay clear#0 - clears your current override for seeing cell packages
	#3cell exit list#0 - lists all exits for the current cell (including in other overlays)
	#3cell exit hide <exit> <prog>#0 - hides a cell exit with a specified prog controlling who can see it
	#3cell exit unhide <exit>#0 - unhides a cell exit
	#3cell set register <varname> <value>#0 - sets the specified prog variable for the current cell to the specified value
	#3cell set register delete <varname>#0 - resets the specified prog variable to its default value for the current cell
	#3cell landmark [<prog>] [<sphere>]#0 - sets your current location as a landmark (with optional applicability prog and sphere)
	#3cell landmark#0 - toggles being a landmark off
	#3cell meeting [<prog>] [<sphere>]#0 - sets your current location as a meeting place landmark (with optional applicability prog and sphere)
	#3cell meeting#0 - toggles being a meeting place off
	#3cell landmarktext#0 - shows all extra texts for a landmark cell
	#3cell landmarktext add <prog>#0 - drops into an editor to create a new extra text
	#3cell landmarktext prog <##> <prog>#0 - replaces the prog of a specific extra text
	#3cell landmarktext text <##>#0 - drops into an editor to replace an extra text
	#3cell landmarktext swap <##> <##>#0 - swaps the order of two extra texts
	#3cell landmarktext delete <##>#0 - deletes a landmark text

These are the commands used to work with overlay packages:

	#3cell package list [all|by <who> | mine]#0 - lists all cell packages (optionally filtered)
	#3cell package new ""name of your package""#0 - creates a new package with the specified name
	#3cell package open <id>|""name of your package"">#0 - opens an existing unapproved package for further editing
	#3cell package rename <name>#0 - renames your open cell package to something else
	#3cell package revise <id>|""name of your package"">#0 - creates a new revision of an existing package
	#3cell package close#0 - closes the package you are currently editing
	#3cell package show <id>|""name"">#0 - views an existing package
	#3cell package submit#0 - submits the package for review by an appropriate reviewer
	#3cell package review list#0 - shows all packages ready for review
	#3cell package review all#0 - reviews all submitted packages at once
	#3cell package review <id>#0 - reviews a specific package
	#3cell package history <id>#0 - shows the building/review history of a particular cell package
	#3cell package swap <id|""name of your package"">#0 - swaps the package into the affected rooms and makes it live

These commands all require you to have an open overlay package:

	#3cell new#0 - creates a new cell and transports you to it
	#3cell dig <direction#0 - creates a new cell and a two-way exit in the specified direction, and transports you to it
	#3cell ndig <template> <outboundkeyword> <inboundkeyword> ""<outbounddescription>"" ""<inbounddescription>""#0 - creates a new cell with a non cardinal exit, and transports you to it
	#3cell exit add <id>#0 - adds an existing exit from another overlay for this cell to this overlay
	#3cell exit remove <id>#0 - removes an exit from this overlay
	#3cell exit size <id|direction> <size>#0 - sets the maximum size of creatures that can use the exit
	#3cell exit upright <id|direction> <size>#0 - sets the maximum size of creatures that can use the exit in a standing position
	#3cell exit reset <id|direction>#0 - turns a climb/fall exit into a regular exit
	#3cell exit fall <id|direction>#0 - turns an up/down exit into a fall exit or toggles it off
	#3cell exit climb <id|direction> <difficulty>#0 - turns an exit into a climb exit with a specified difficulty
	#3cell exit climb <id|direction>#0 - toggles a climb exit off
	#3cell exit block <id|direction> <layer>#0 - blocks an exit from appearing in a specified layer
	#3cell exit unblock <id|direction> <layer>#0 - removes a block on an exit from appearing in a specified layer
	#3cell link <direction> <cellid**>#0 - creates a new exit in the specified direction to the specified cell
	#3cell nlink <template> <cellid**> <outboundkeyword> <inboundkeyword> ""<outbounddescription>"" ""<inbounddescription>""#0 - creates a non-cardinal exit using a template to a cell
	#3cell set name <name>#0 - sets the name of the cell
	#3cell set desc#0 - drops you into an editor to edit the cell description
	#3cell set terrain <id|name>#0 - sets the terrain of this cell
	#3cell set hearing <id|name>#0 - sets the hearing/noise profile for this cell
	#3cell set lightmultiplier <multiplier>#0 - sets the multiplier for natural light (e.g. from shade etc)
	#3cell set lightlevel <lux>#0 - sets the added light for the location to the specified lux level
	#3cell set type outdoors|indoors|cave|windows|exposed#0 - sets the cell exposure type
	#3cell set door <exit id|direction> clear#0 - clears the exit from accepting doors
	#3cell set door <exit id|direction> <size>#0 - sets the exit to accept doors of the specified size
	#3cell set forage clear#0 - clears an existing forage profile
	#3cell set forage <id|name>#0 - sets the forage profile to the specified profile
	#3cell set atmosphere liquid|gas <id|name>#0 - sets the atmosphere to the specified
	#3cell set atmosphere none#0 - sets the location to have no atmosphere
	#3cell set safequit#0 - toggles whether the current room is a safe quit room

#6** Note: You can use the alternate syntax @n instead of the room ID for this.

For example, @1 is the most recently created new room, @2 is the 2nd most recently created room etc.
This only works for rooms created since the last reboot.
This command is useful when you write-up a bunch of room creation commands in a text file to paste into the MUD at once, so you can refer to the rooms that you create rather than having to presuppose what the room ID will be.#0

#5Working with the Autobuilder#0

Autobuilder templates allow you to automatically build areas based on some set parameters. They can vary from simply ways to easily kick off pre-linked areas (like large rectangles of rooms with exits between them) from full-blown randomly generated descriptions and properties. Autobuilder templates are created by your MUD's senior designers and each one may have different usage guidelines.

To see a list of all autobuilder templates, you simply use the #3SHOW AUTOAREAS#0 command.

In order to use an autobuilder template, you must first be editing a cell overlay package, and then use the following command:

	#3cell new <template id|name> ...#0

Each template has its own required arguments, which you will see if you simply type the command above with no further text.

There is also a universal optional argument which must come first in the form of #3prog=someprog#0. This allows you to specify a prog that accepts either a single location or a collection of locations as an argument, and the template will execute that prog on the generated cells. You may specify this option multiple times but they must always be the first arguments in the list, before any template specific ones.";

	[PlayerCommand("Cell", "cell", "room")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("cell", CellHelpText, AutoHelp.HelpArgOrNoArg)]
	protected static void Cell(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		switch (ss.Pop().ToLowerInvariant())
		{
			case "new":
			case "create":
				CellNew(actor, ss);
				break;
			case "dig":
				CellDig(actor, ss);
				break;
			case "ndig":
				CellNDig(actor, ss);
				break;
			case "show":
				CellShow(actor, ss);
				break;
			case "set":
			case "edit":
				CellSet(actor, ss);
				break;
			case "exit":
				CellExit(actor, ss);
				break;
			case "overlay":
				CellOverlay(actor, ss);
				break;
			case "package":
				CellPackage(actor, ss);
				break;
			case "link":
				CellEditLink(actor, ss);
				break;
			case "nlink":
				CellEditNlink(actor, ss);
				break;
			case "delete":
				CellDelete(actor, ss);
				break;
			case "meetingplace":
			case "meeting":
				CellMeetingPlace(actor, ss);
				break;
			case "landmark":
				CellLandmark(actor, ss);
				break;
			case "landmarktext":
				CellLandmarkText(actor, ss);
				break;
			default:
				actor.OutputHandler.Send(
					$"That is not a valid option to use with the {"cell".Colour(Telnet.Yellow)} command. See {"CELL HELP".FluentTagMXP("send", "href='cell help' hint='display cell help'")} for more info.");
				return;
		}
	}

	private static void CellLandmarkText(ICharacter actor, StringStack ss)
	{
		var cell = actor.Location;
		var existing = cell.EffectsOfType<LandmarkEffect>().FirstOrDefault();
		if (existing is null)
		{
			actor.OutputHandler.Send("You current location is not a landmark.");
			return;
		}

		var sb = new StringBuilder();
		if (ss.IsFinished)
		{
			if (existing.LandmarkDescriptionTexts.Count == 0)
			{
				actor.OutputHandler.Send("This landmark does not have any extra texts.");
				return;
			}

			sb.AppendLine($"Extra Texts for this landmark:");
			var i = 1;
			foreach (var text in existing.LandmarkDescriptionTexts)
			{
				sb.AppendLine();
				sb.AppendLine($"#{i++.ToStringN0(actor)} - Prog: {text.Prog.MXPClickableFunctionName()}");
				sb.AppendLine();
				sb.AppendLine(text.Text.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength));
			}
			actor.OutputHandler.Send(sb.ToString());
			return;
		}

		void PostAction(string text, IOutputHandler handler, object[] args)
		{
			if (args[0] is int index)
			{
				if (existing.LandmarkDescriptionTexts.Count <= index)
				{
					handler.Send("Unfortunately some of the texts have been deleted while you were editing.");
					return;
				}

				existing.LandmarkDescriptionTexts[index] = (existing.LandmarkDescriptionTexts[index].Prog, text);
				existing.Changed = true;
				handler.Send($"You change the text of the {(index + 1).ToOrdinal()} extra text for this landmark to:\n\n{text.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength)}");
			}
			else if (args[0] is IFutureProg prog)
			{
				existing.LandmarkDescriptionTexts.Add((prog, text));
				existing.Changed = true;
				handler.Send($"You add a new extra text for this landmark with the prog {prog.MXPClickableFunctionName()} and text as follows:\n\n{text.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength)}");
			}
			else
			{
				throw new ApplicationException("Unsupported post action in CELL LANDMARKTEXT");
			}
		}


		switch (ss.PopForSwitch())
		{
			case "add":
			case "new":
				if (ss.IsFinished)
				{
					actor.OutputHandler.Send("Which prog do you want to set for that extra text?");
					return;
				}

				var prog = new ProgLookupFromBuilderInput(actor, ss.SafeRemainingArgument, ProgVariableTypes.Boolean, [ProgVariableTypes.Character]).LookupProg();
				if (prog is null)
				{
					return;
				}

				actor.OutputHandler.Send("Enter your text below:");
				actor.EditorMode(postAction: PostAction, cancelAction: (handler, objects) => handler.Send("You decide not to create a new text."), suppliedArguments: [prog]);
				return;
			case "edit":
			case "text":
				if (ss.IsFinished)
				{
					actor.OutputHandler.Send("Which number extra text do you want to edit?");
					return;
				}

				if (!int.TryParse(ss.PopSpeech(), actor, out var value) || value < 1 || value > existing.LandmarkDescriptionTexts.Count)
				{
					actor.OutputHandler.Send($"The text {ss.Last.ColourCommand()} is not a valid number between {1.ToStringN0Colour(actor)} and {existing.LandmarkDescriptionTexts.Count.ToStringN0Colour(actor)}.");
					return;
				}

				actor.OutputHandler.Send($@"Replacing:

{existing.LandmarkDescriptionTexts[value - 1].Text.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength, "\t")}

Enter your text below:");
				actor.EditorMode(postAction: PostAction, cancelAction: (handler, objects) => handler.Send("You decide not to edit the text."), suppliedArguments: [value-1]);
				return;
			case "prog":
				if (ss.IsFinished)
				{
					actor.OutputHandler.Send("Which number extra text do you want to change the prog for?");
					return;
				}

				if (!int.TryParse(ss.PopSpeech(), actor, out value) || value < 1 || value > existing.LandmarkDescriptionTexts.Count)
				{
					actor.OutputHandler.Send($"The text {ss.Last.ColourCommand()} is not a valid number between {1.ToStringN0Colour(actor)} and {existing.LandmarkDescriptionTexts.Count.ToStringN0Colour(actor)}.");
					return;
				}

				if (ss.IsFinished)
				{
					actor.OutputHandler.Send("Which prog do you want to set for that extra text?");
					return;
				}

				prog = new ProgLookupFromBuilderInput(actor, ss.SafeRemainingArgument, ProgVariableTypes.Boolean, [ProgVariableTypes.Character]).LookupProg();
				if (prog is null)
				{
					return;
				}

				existing.LandmarkDescriptionTexts[value - 1] = (prog, existing.LandmarkDescriptionTexts[value - 1].Text);
				existing.Changed = true;
				actor.OutputHandler.Send($"The {value.ToOrdinal()} extra description for this landmark now uses the prog {prog.MXPClickableFunctionName()}.");
				return;
			case "swap":
				if (ss.IsFinished)
				{
					actor.OutputHandler.Send("What is the first number of the extra text do you want to remove?");
					return;
				}

				if (!int.TryParse(ss.PopSpeech(), actor, out var value1) || value1 < 1 || value1 > existing.LandmarkDescriptionTexts.Count)
				{
					actor.OutputHandler.Send($"The text {ss.Last.ColourCommand()} is not a valid number between {1.ToStringN0Colour(actor)} and {existing.LandmarkDescriptionTexts.Count.ToStringN0Colour(actor)}.");
					return;
				}

				if (ss.IsFinished)
				{
					actor.OutputHandler.Send("What is the second number of the extra text do you want to remove?");
					return;
				}

				if (!int.TryParse(ss.PopSpeech(), actor, out var value2) || value2 < 1 || value2 > existing.LandmarkDescriptionTexts.Count)
				{
					actor.OutputHandler.Send($"The text {ss.Last.ColourCommand()} is not a valid number between {1.ToStringN0Colour(actor)} and {existing.LandmarkDescriptionTexts.Count.ToStringN0Colour(actor)}.");
					return;
				}

				if (value1 == value2)
				{
					actor.OutputHandler.Send("You can't swap something with itself.");
					return;
				}

				existing.LandmarkDescriptionTexts.Swap(value1, value2);
				existing.Changed = true;
				actor.OutputHandler.Send($"You swap the ordering of the {value1.ToOrdinal()} and {value2.ToOrdinal()} extra texts for this landmark.");
				return;
			case "delete":
			case "del":
			case "remove":
			case "rem":
				if (ss.IsFinished)
				{
					actor.OutputHandler.Send("Which number extra text do you want to remove?");
					return;
				}

				if (!int.TryParse(ss.PopSpeech(), actor, out value) || value < 1 || value > existing.LandmarkDescriptionTexts.Count)
				{
					actor.OutputHandler.Send($"The text {ss.Last.ColourCommand()} is not a valid number between {1.ToStringN0Colour(actor)} and {existing.LandmarkDescriptionTexts.Count.ToStringN0Colour(actor)}.");
					return;
				}

				var old = existing.LandmarkDescriptionTexts.ElementAt(value - 1);
				existing.LandmarkDescriptionTexts.Remove(old);
				existing.Changed = true;
				actor.OutputHandler.Send($"You remove the {value.ToOrdinal()} landmark description, which was:\n\n{old.Text.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength)}");
				return;
			default:
				actor.OutputHandler.Send("The valid subcommands are #3add#0, #3remove#0, #3swap#0, #3edit#0, and #3prog#0.".SubstituteANSIColour());
				return;
		}
	}

	private static void CellMeetingPlace(ICharacter actor, StringStack ss)
	{
		var cell = actor.Location;
		var existing = cell.EffectsOfType<LandmarkEffect>().FirstOrDefault();
		var sphere = string.Empty;
		var prog = default(IFutureProg);
		if (ss.IsFinished)
		{
			if (existing is not null)
			{
				if (existing.IsMeetingPlace)
				{
					cell.RemoveEffect(existing);
					actor.OutputHandler.Send($"Your current location ({cell.GetFriendlyReference(actor)}) is no longer considered a meeting place.");
					return;
				}

				existing.IsMeetingPlace = true;
				existing.Changed = true;
				actor.OutputHandler.Send($"Your current location ({cell.GetFriendlyReference(actor)}) is no longer only a landmark, but is now also a meeting place.");
				return;
			}
		}
		else
		{
			prog = new ProgLookupFromBuilderInput(
				actor, 
				ss.PopSpeech(), 
				ProgVariableTypes.Boolean, 
				[
					[ProgVariableTypes.Location],
					[ProgVariableTypes.Location, ProgVariableTypes.Character]
				]
			).LookupProg();
			if (prog is null)
			{
				return;
			}

			if (!ss.IsFinished)
			{
				sphere = ss.SafeRemainingArgument.TitleCase();
			}
		}

		if (existing is not null)
		{
			existing.IsMeetingPlace = true;
			existing.Sphere = sphere;
			existing.ApplicabilityProg = prog;
			existing.Changed = true;
		}
		else
		{
			existing = new LandmarkEffect(cell, true, sphere, prog);
			cell.AddEffect(existing);
		}

		actor.OutputHandler.Send($"Your current location ({cell.GetFriendlyReference(actor)}) is now a meeting place landmark{(string.IsNullOrEmpty(sphere) ? "" : $" in the {sphere.ColourName()} sphere")} {(prog is not null ? $"that applies when the prog {prog.MXPClickableFunctionName()} is true" : "that always applies")}.");
	}

	private static void CellLandmark(ICharacter actor, StringStack ss)
	{
		var cell = actor.Location;
		var existing = cell.EffectsOfType<LandmarkEffect>().FirstOrDefault();
		var sphere = string.Empty;
		var prog = default(IFutureProg);
		if (ss.IsFinished)
		{
			if (existing is not null)
			{
				if (!existing.IsMeetingPlace)
				{
					cell.RemoveEffect(existing);
					actor.OutputHandler.Send($"Your current location ({cell.GetFriendlyReference(actor)}) is no longer considered a landmark.");
					return;
				}

				existing.IsMeetingPlace = false;
				existing.Changed = true;
				actor.OutputHandler.Send($"Your current location ({cell.GetFriendlyReference(actor)}) is now only a landmark, instead of also being a meeting place.");
				return;
			}
		}
		else
		{
			prog = new ProgLookupFromBuilderInput(
				actor,
				ss.PopSpeech(),
				ProgVariableTypes.Boolean,
				[
					[ProgVariableTypes.Location],
					[ProgVariableTypes.Location, ProgVariableTypes.Character]
				]
			).LookupProg();
			if (prog is null)
			{
				return;
			}

			if (!ss.IsFinished)
			{
				sphere = ss.SafeRemainingArgument.TitleCase();
			}
		}

		if (existing is not null)
		{
			existing.IsMeetingPlace = false;
			existing.Sphere = sphere;
			existing.ApplicabilityProg = prog;
			existing.Changed = true;
		}
		else
		{
			existing = new LandmarkEffect(cell, false, sphere, prog);
			cell.AddEffect(existing);
		}

		actor.OutputHandler.Send($"Your current location ({cell.GetFriendlyReference(actor)}) is now a landmark{(string.IsNullOrEmpty(sphere) ? "" : $" in the {sphere.ColourName()} sphere")} {(prog is not null ? $"that applies when the prog {prog.MXPClickableFunctionName()} is true" : "that always applies")}.");
	}

	private static void CellDelete(ICharacter actor, StringStack ss)
	{
		if (!actor.IsAdministrator(PermissionLevel.HighAdmin))
		{
			actor.OutputHandler.Send("This command has some serious potential side-effects and should only be done by the highest admin authorities.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send($@"Are you sure that you want to delete the cell that you're currently in? #1This action is irreversible and can have major unanticipated effects#0, including:

	1) All items and characters in the room (including those not logged in) will be moved to another room.
	2) There are potentially parts of the code that reference this room indirectly that may not be updated

{Accept.StandardAcceptPhrasing}".SubstituteANSIColour());

			var location = actor.Location;

			actor.AddEffect(new Accept(actor, new GenericProposal {	
				AcceptAction = text => {
					var result = location.ProposeDelete();
					if (!result.Truth)
					{
						var sb = new StringBuilder();
						sb.AppendLine($"Couldn't delete {location.GetFriendlyReference(actor)} because of the following issues:");
						sb.AppendLine();
						var count = 1;
						foreach (var issue in result.Errors)
						{
							sb.AppendLine($"\t{count++.ToString("N0", actor)}) {issue}");
						}
						actor.OutputHandler.Send(sb.ToString());
						return;
					}

					var fallback = location.Zone.Rooms.FirstOrDefault(x => x != location.Room)?.Cells.First() ??
					location.Shard.Rooms.FirstOrDefault(x => x != location.Room)?.Cells.First() ??
					actor.Gameworld.Rooms.FirstOrDefault(x => x != location.Room)?.Cells.First();
					if (fallback is null)
					{
						actor.OutputHandler.Send("You can't delete that location because there would be no fallback. There must always be a fallback room.");
						return;
					}

					actor.OutputHandler.Send($"You delete {location.GetFriendlyReference(actor)}.");
					location.Destroy(fallback);
				},
				RejectAction = text => {
					actor.OutputHandler.Send("You decide not to delete the room.");
				},
				ExpireAction = () => {
					actor.OutputHandler.Send("You decide not to delete the room.");
				},
				DescriptionString = "Deleting a room",
				Keywords = new List<string> { "delete", "room", "cell", "location" }
			}), TimeSpan.FromSeconds(120));
			return;
		}

		// TODO
		actor.OutputHandler.Send("Mass deletion will come later.");
	}

	#region CellNew

	private static void CellNew(ICharacter actor, StringStack input)
	{
		if (actor.CurrentOverlayPackage == null)
		{
			actor.OutputHandler.Send("You have not adopted a cell overlay package.");
			return;
		}

		if (actor.CurrentOverlayPackage.Status != RevisionStatus.UnderDesign)
		{
			actor.OutputHandler.Send(
				"Only a Cell Overlay Package that is in the Under Design status can be used to create new Cells.");
			return;
		}

		if (!input.IsFinished)
		{
			CellNewTemplate(actor, input);
			return;
		}

		var newRoom = new Construction.Room(actor, actor.CurrentOverlayPackage);
		actor.Send("You create a new cell with ID #{0}.", newRoom.Cells.First().Id);
		actor.TransferTo(newRoom.Cells.First(), RoomLayer.GroundLevel);
		BuiltCells.Add(newRoom.Cells.First());
	}

	private static void CellDig(ICharacter actor, StringStack input)
	{
		if (actor.CurrentOverlayPackage == null)
		{
			actor.OutputHandler.Send("You have not adopted a cell overlay package.");
			return;
		}

		if (actor.CurrentOverlayPackage.Status != RevisionStatus.UnderDesign)
		{
			actor.OutputHandler.Send(
				"Only a Cell Overlay Package that is in the Under Design status can be used to create new Cells.");
			return;
		}

		if (input.IsFinished)
		{
			actor.OutputHandler.Send("Towards which direction do you want to make a new cell?");
			return;
		}

		if (!Constants.CardinalDirectionStringToDirection.TryGetValue(input.PopSpeech(), out var direction))
		{
			actor.OutputHandler.Send("That is not a valid direction.");
			return;
		}

		var overlay = actor.Location.GetOrCreateOverlay(actor.CurrentOverlayPackage);
		if (
			actor.Gameworld.ExitManager.GetExitsFor(actor.Location, overlay)
				 .Any(x => x.OutboundDirection == direction))
		{
			actor.OutputHandler.Send("This Cell Overlay already contains an exit in that direction.");
			return;
		}

		var newRoom = new Construction.Room(actor, actor.CurrentOverlayPackage);
		var cell = newRoom.Cells.First();
		BuiltCells.Add(cell);
		var otherOverlay = cell.GetOrCreateOverlay(actor.CurrentOverlayPackage);
		var oppositeDirection = direction.Opposite();
		var newExit =
			new Construction.Boundary.Exit(actor.Gameworld, actor.Location, cell, direction, oppositeDirection, 1.0);
		overlay.AddExit(newExit);
		otherOverlay.AddExit(newExit);
		actor.Gameworld.ExitManager.UpdateCellOverlayExits(actor.Location, overlay);
		actor.Gameworld.ExitManager.UpdateCellOverlayExits(cell, otherOverlay);

		actor.Send($"You create a new cell to the {direction.Describe().ColourValue()} with ID #{cell.Id.ToString("N0", actor)}.", cell.Id);
		actor.TransferTo(cell, RoomLayer.GroundLevel);
	}

	private static void CellNDig(ICharacter actor, StringStack input)
	{
		if (actor.CurrentOverlayPackage == null)
		{
			actor.OutputHandler.Send("You have not adopted a cell overlay package.");
			return;
		}

		if (actor.CurrentOverlayPackage.Status != RevisionStatus.UnderDesign)
		{
			actor.OutputHandler.Send(
				"Only a Cell Overlay Package that is in the Under Design status can be used to create new Cells.");
			return;
		}

		var match = CellEditNDigRegex.Match(input.RemainingArgument);
		if (!match.Success)
		{
			actor.OutputHandler.Send("You must supply an argument in this form: " +
									 "cell ndig <template> <outbound keyword> <inbound keyword> \"<outbound name>\" \"<inbound name>\""
										 .Colour(Telnet.Yellow));
			return;
		}

		var template = actor.Gameworld.NonCardinalExitTemplates.GetByIdOrName(match.Groups["template"].Value);
		if (template == null)
		{
			actor.OutputHandler.Send("That is not a valid Non Cardinal Exit Template.");
			return;
		}

		var overlay = actor.Location.GetOrCreateOverlay(actor.CurrentOverlayPackage);
		var newRoom = new Construction.Room(actor, actor.CurrentOverlayPackage);
		var cell = newRoom.Cells.First();
		BuiltCells.Add(cell);
		var otherOverlay = cell.GetOrCreateOverlay(actor.CurrentOverlayPackage);
		var newExit = new Construction.Boundary.Exit(actor.Gameworld, actor.Location, cell, 1.0, template,
			match.Groups["outboundkey"].Value,
			match.Groups["inboundkey"].Value, match.Groups["outboundname"].Value, match.Groups["inboundname"].Value);
		overlay.AddExit(newExit);
		otherOverlay.AddExit(newExit);
		actor.Gameworld.ExitManager.UpdateCellOverlayExits(actor.Location, overlay);
		actor.Gameworld.ExitManager.UpdateCellOverlayExits(cell, otherOverlay);

		actor.Send($"You create a new cell from the non-cardinal template {template.Name.Proper().Colour(Telnet.Cyan)} with ID #{cell.Id.ToString("N0", actor)}.", cell.Id);
		actor.TransferTo(cell, RoomLayer.GroundLevel);
	}

	private static Regex CellNewTemplateOptionsRegex { get; } = new(@"(?<option>[a-zA-Z]+)=(?<value>\w+)\b");

	private static void CellNewTemplate(ICharacter actor, StringStack input)
	{
		var template = long.TryParse(input.PopSpeech(), out var value)
			? actor.Gameworld.AutobuilderAreas.Get(value)
			: actor.Gameworld.AutobuilderAreas.GetByName(input.Last);
		if (template == null)
		{
			actor.Send("There is no such autobuilder template for an area which you can use. See SHOW AUTOAREAS.");
			return;
		}

		var afterExecutionProgs = new List<IFutureProg>();
		while (!input.IsFinished && CellNewTemplateOptionsRegex.IsMatch(input.PeekSpeech()))
		{
			var match = CellNewTemplateOptionsRegex.Match(input.PopSpeech());
			switch (match.Groups["option"].Value.ToLowerInvariant())
			{
				case "prog":
					var prog = long.TryParse(match.Groups["value"].Value, out value)
						? actor.Gameworld.FutureProgs.Get(value)
						: actor.Gameworld.FutureProgs.GetByName(match.Groups["value"].Value);
					;
					if (prog == null)
					{
						actor.OutputHandler.Send(
							$"There is no such prog as \"{match.Groups["value"].Value}\" for you to use.");
						return;
					}

					if (!prog.MatchesParameters(new ProgVariableTypes[]
							{ ProgVariableTypes.Location | ProgVariableTypes.Collection }) &&
						!prog.MatchesParameters(new ProgVariableTypes[] { ProgVariableTypes.Location }))
					{
						actor.OutputHandler.Send(
							$"Any prog specified as an option must accept only a single location or a collection of locations as a parameter. The {prog.MXPClickableFunctionName()} prog does not.");
						return;
					}

					afterExecutionProgs.Add(prog);
					continue;
				default:
					actor.OutputHandler.Send(
						$"\"{match.Groups["option"].Value.ToLowerInvariant()}\" is not a recognised option for use with cell area templates.");
					return;
			}
		}

		var (success, error, args) = template.TryArguments(actor, input);
		if (!success)
		{
			actor.Send(error);
			return;
		}

		actor.OutputHandler.PrioritySend(
			$"Launching the {template.Name.Colour(Telnet.Cyan)} autobuilder area template...");
		var results = template.ExecuteTemplate(actor, args).ToList();
		actor.OutputHandler.PrioritySend($"Generated {results.Count} rooms.");
		foreach (var prog in afterExecutionProgs)
		{
			actor.OutputHandler.PrioritySend(
				$"Executing the prog {prog.MXPClickableFunctionNameWithId()} on the results.");
			if (prog.MatchesParameters(new[] { ProgVariableTypes.Location }))
			{
				foreach (var cell in results)
				{
					prog.Execute(cell);
				}
			}
			else
			{
				prog.Execute(results);
			}
		}

		actor.OutputHandler.PrioritySend("All done. Transferring you to the base room.");
		actor.TransferTo(results.FirstOrDefault(x => x != null), RoomLayer.GroundLevel);
		BuiltCells.AddRange(results.AsEnumerable());
	}

	#endregion

	#region CellShow

	private static void CellShow(ICharacter actor, StringStack input)
	{
		var cell = actor.Location;
		var sb = new StringBuilder();
		sb.AppendLine(string.Format(actor, "Showing Cell ID {0:N0}", cell.Id).Colour(Telnet.Cyan));
		sb.Append(new[]
		{
			$"Cell ID: {cell.Id.ToString("N0", actor).Colour(Telnet.Green)}",
			$"Room ID: {cell.Room.Id.ToString("N0", actor).Colour(Telnet.Green)}"
		}.ArrangeStringsOntoLines(2, (uint)actor.Account.LineFormatLength));
		sb.Append(new[]
		{
			$"Zone: {cell.Zone.Name.TitleCase().Colour(Telnet.Green)} (#{cell.Zone.Id:N0})",
			$"Shard: {cell.Shard.Name.TitleCase().Colour(Telnet.Green)} (#{cell.Shard.Id:N0})"
		}.ArrangeStringsOntoLines(2, (uint)actor.Account.LineFormatLength));
		sb.AppendLine($"Current Name: {cell.CurrentOverlay.CellName}");
		sb.AppendLine(
			$"Current Description:\n\n{cell.CurrentOverlay.CellDescription.Wrap(actor.InnerLineFormatLength, "\t")}");
		sb.Append(new[]
		{
			$"Terrain: {cell.CurrentOverlay.Terrain.Name.TitleCase().Colour(Telnet.Green)}",
			$"Outdoors: {cell.CurrentOverlay.OutdoorsType.Describe().Colour(Telnet.Green)}"
		}.ArrangeStringsOntoLines(2, (uint)actor.Account.LineFormatLength));
		sb.Append(new[]
		{
			$"Ambient Light Factor: {cell.CurrentOverlay.AmbientLightFactor.ToString("N5", actor).Colour(Telnet.Green)}",
			$"Added Light: {cell.CurrentOverlay.AddedLight.ToString("N5", actor).Colour(Telnet.Green)} lux"
		}.ArrangeStringsOntoLines(2, (uint)actor.Account.LineFormatLength));
		sb.AppendLine(
			$"Noise Profile: {(cell.CurrentOverlay.HearingProfile == null ? "None".Colour(Telnet.Red) : $"{cell.CurrentOverlay.HearingProfile.Name.TitleCase().Colour(Telnet.Green)} (#{cell.CurrentOverlay.HearingProfile.Id:N0})")}");
		sb.AppendLine("Overlays:\n");
		sb.Append(
			StringUtilities.GetTextTable(
				from overlay in cell.Overlays
				orderby overlay.Package.BuilderDate descending
				select new[]
				{
					overlay.Package.Id.ToString("N0", actor),
					overlay.Package.RevisionNumber.ToString("N0", actor),
					overlay.Package.Name.TitleCase(),
					overlay.Package.Status.Describe(),
					cell.CurrentOverlay == overlay ? "Yes" : "No"
				},
				new[] { "ID#", "Rev#", "Package", "Status", "Current" },
				actor.Account.LineFormatLength,
				colour: Telnet.Green
			)
		);
		actor.SendNoNewLine(sb.ToString());
	}

	#endregion

	#region CellOverlay

	private static void CellOverlay(ICharacter actor, StringStack input)
	{
		if (input.IsFinished)
		{
			actor.OutputHandler.Send(
				"You must specify a Cell Overlay Package to adopt, or use \"clear\" to return to default.");
			return;
		}

		var cmd = input.PopSpeech().ToLowerInvariant();

		if (cmd == "clear")
		{
			actor.CurrentOverlayPackage = null;
			actor.OutputHandler.Send("You will now only see the current cell overlay for each location.");
			return;
		}

		ICellOverlayPackage package = null;
		if (long.TryParse(cmd, out var value))
		{
			cmd = input.Pop();
			if (!string.IsNullOrEmpty(cmd) && int.TryParse(cmd, out var revnum))
			{
				package = actor.Gameworld.CellOverlayPackages.Get(value, revnum);
			}
			else
			{
				package = actor.Gameworld.CellOverlayPackages.Get(value);
			}
		}
		else
		{
			var revcmd = input.Pop();
			if (!string.IsNullOrEmpty(cmd) && int.TryParse(revcmd, out var revnum))
			{
				package = actor.Gameworld.CellOverlayPackages.GetByName(cmd, revnum);
			}
			else
			{
				package = actor.Gameworld.CellOverlayPackages.GetByName(cmd);
			}
		}

		if (package == null)
		{
			actor.OutputHandler.Send("There is no such package for you to adopt.");
			return;
		}

		actor.CurrentOverlayPackage = package;
		actor.Send(
			"You adopt the \"{0}\" (ID#{1} Rev#{2}) Cell Overlay Package, and will see it by default where it applies until you clear it.",
			package.Name, package.Id, package.RevisionNumber);
	}

	#endregion

	[PlayerCommand("Zones", "zones")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	protected static void Zones(ICharacter actor, string command)
	{
		actor.OutputHandler.Send(
			StringUtilities.GetTextTable(
				from zone in actor.Gameworld.Zones
				select
					new[]
					{
						zone.Id.ToString(), zone.Name,
						zone.Clocks.FirstOrDefault()?
							.DisplayTime(zone.Time(zone.Clocks.First()),
								TimeDisplayTypes.Immortal) ?? "",
						zone.Calendars.FirstOrDefault()?
							.DisplayDate(zone.Date(zone.Calendars.First()),
								CalendarDisplayMode.Short) ?? "",
						zone.Rooms.Count().ToString("N0", actor),
						zone.Shard.Name
					},
				new[] { "ID#", "Name", "Local Time", "Local Date", "# Rooms", "Shard" },
				actor.Account.LineFormatLength, colour: Telnet.Green, unicodeTable: actor.Account.UseUnicode
			)
		);
	}

	[PlayerCommand("Rooms", "rooms")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("rooms",
		@"This command is used to show a list of rooms. There are various options that you can use to filter the list of rooms that you want to see, and these can be added after the ROOMS command. You can add as many filters as you like.

Possible filter options include:

	#3<zone id|name>#0 - shows only rooms belonging to a particular zone
	#3+keyword#0 - shows only rooms whose name or description contains a keyword
	#3-keyword#0 - shows only rooms whose name and description do not contain a keyword", AutoHelp.HelpArgOrNoArg)]
	protected static void Rooms(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());

		// Filters
		var rooms = actor.Gameworld.Cells.AsEnumerable();
		var filterDescs = new List<string>();
		while (!ss.IsFinished)
		{
			var arg = ss.PopSpeech();
			var arg1 = arg.Substring(1);
			IZone zone;
			switch (arg[0])
			{
				case '+':
					if (arg1.Length < 1)
					{
						actor.OutputHandler.Send($"The text {arg.ColourCommand()} is not a valid filter.");
						return;
					}

					rooms = rooms.Where(x =>
						x.HowSeen(actor, colour: false).Contains(arg1, StringComparison.InvariantCultureIgnoreCase) ||
						x.HowSeen(actor, colour: false, type: DescriptionType.Full)
						 .Contains(arg1, StringComparison.InvariantCultureIgnoreCase)
					);
					filterDescs.Add($"containing keyword {arg1.ColourCommand()}");
					continue;
				case '-':
					if (arg1.Length < 1)
					{
						actor.OutputHandler.Send($"The text {arg.ColourCommand()} is not a valid filter.");
						return;
					}

					rooms = rooms.Where(x =>
						!x.HowSeen(actor, colour: false).Contains(arg1, StringComparison.InvariantCultureIgnoreCase) &&
						!x.HowSeen(actor, colour: false, type: DescriptionType.Full)
						  .Contains(arg1, StringComparison.InvariantCultureIgnoreCase)
					);
					filterDescs.Add($"excluding keyword {arg1.ColourCommand()}");
					continue;
				default:
					zone = long.TryParse(arg, out var value)
						? actor.Gameworld.Zones.Get(value)
						: actor.Gameworld.Zones.FirstOrDefault(
							x => x.Name.Equals(ss.Last, StringComparison.InvariantCultureIgnoreCase));
					if (zone == null)
					{
						actor.OutputHandler.Send($"There is no such zone as {arg.ColourCommand()}.");
						return;
					}

					rooms = rooms.Where(x => x.Zone.Id == zone.Id);
					filterDescs.Add($"in Zone {zone.Name.ColourName()}");

					continue;
			}
		}

		rooms = rooms.ToArray();
		var sb = new StringBuilder();

		sb.AppendLine($"Rooms{(filterDescs.Any() ? $" {filterDescs.ListToString()}" : "")}:");
		sb.AppendLine(
			StringUtilities.GetTextTable(
				from room in rooms
				select
					new[]
					{
						room.Id.ToString("N0", actor), room.HowSeen(actor, colour: false),
						room.CurrentOverlay.Package.Name, room.CurrentOverlay.Terrain.Name,
						room.CurrentOverlay.OutdoorsType.Describe()
					},
				new[] { "ID", "Name", "Overlay", "Terrain", "Outdoors" },
				actor.Account.LineFormatLength,
				colour: Telnet.Green,
				truncatableColumnIndex: 4
			)
		);
		sb.AppendLine($"Total Rooms: {rooms.Count().ToString("N0", actor).ColourValue()}");
		actor.OutputHandler.Send(sb.ToString());
	}

	[PlayerCommand("Rezone", "rezone")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("rezone",
		"This command is used to change a room into a different zone. The usage is REZONE <new zone>. You must use this command from inside the room you want to rezone.",
		AutoHelp.HelpArgOrNoArg)]
	protected static void Rezone(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var zone = long.TryParse(ss.SafeRemainingArgument, out var value)
			? actor.Gameworld.Zones.Get(value)
			: actor.Gameworld.Zones.GetByName(ss.SafeRemainingArgument);
		if (zone == null)
		{
			actor.Send("There is no such zone. See ZONES for a list of zones.");
			return;
		}

		actor.Location.Room.SetNewZone(zone);
		actor.Send(
			$"{actor.Location.HowSeen(actor, true)} is now in zone {zone.Name.Colour(Telnet.Cyan)} (#{zone.Id})");
	}

	private const string ZoneHelpText =
		@"This command is used to create and edit zones, which are distinct geographical areas within a specific shard. For example, all rooms in a zone share the same timezone, geographic coordinates and usually weather as well.

You can use the following subcommands:

	#3zone new <name> <shard> <timezones...>#0 - creates a new zone (note: must be editing a #3CELL PACKAGE#0)
	#3zone show <which>#0 - shows detailed information about a zone
	#3zone set <which> name <new name>#0 - renames a zone
	#3zone set <which> latitude <degrees>#0 - sets the latitude of a zone
	#3zone set <which> longitude <degrees>#0 - sets the longitude of a zone
	#3zone set <which> elevation <amount>#0 - sets the elevation of the zone above mean sea level
	#3zone set <which> light <##>#0 - sets the ambient light in lux from the zone at all times
	#3zone set <which> timezone <clock> <tz>#0 - sets a timezone for a particular clock
	#3zone set <which> fp <which>#0 - sets the foragable profile for the zone
	#3zone set <which> fp clear#0 - clears the foragable profile for the zone
	#3zone set <which> weather <wc>#0 - sets a weather controller for the zone
	#3zone set <which> weather none#0 - clears the weather controller for the zone

See also the #3ZONES#0 command to see a list of commands, and the #3ROOMS <zone>#0 command to see a list of rooms within a zone.
See the #3CELL#0 command for more information about #3CELL PACKAGES#0.";

	[PlayerCommand("Zone", "zone")]
	[CommandPermission(PermissionLevel.SeniorAdmin)]
	[HelpInfo("zone", ZoneHelpText, AutoHelp.HelpArgOrNoArg)]
	protected static void Zone(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());

		switch (ss.Pop().ToLowerInvariant())
		{
			case "list":
				Zones(actor, input);
				return;
			case "create":
			case "new":
				ZoneCreate(actor, ss);
				break;
			case "set":
			case "edit":
				ZoneEdit(actor, ss);
				break;
			case "show":
			case "view":
				ZoneShow(actor, ss);
				break;
			default:
				actor.OutputHandler.Send(ZoneHelpText.SubstituteANSIColour());
				return;
		}
	}


	#region Zone Sub-Commands

	protected static void ZoneEditName(ICharacter actor, IEditableZone zone, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to this zone?");
			return;
		}

		var nameText = command.SafeRemainingArgument;
		if (actor.Gameworld.Zones.Any(x => x.Name.Equals(nameText, StringComparison.InvariantCultureIgnoreCase)))
		{
			actor.OutputHandler.Send("There is already a zone with that name. Zone names must be unique.");
			return;
		}

		actor.OutputHandler.Send(
			$"You change the name of zone #{zone.Id:N0} from {zone.Name.TitleCase().Colour(Telnet.Green)} to {nameText.TitleCase().Colour(Telnet.Green)}.");
		zone.SetName(nameText);
	}

	protected static void ZoneEditElevation(ICharacter actor, IEditableZone zone, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What elevation above sea level do you want to set for this zone?");
			return;
		}

		var amountText = command.SafeRemainingArgument;
		var heightInBaseUnits = actor.Gameworld.UnitManager.GetBaseUnits(amountText, UnitType.Length, out var success);
		if (!success)
		{
			actor.OutputHandler.Send("That is not a valid elevation.");
			return;
		}

		zone.Geography = new GeographicCoordinate(zone.Geography.Latitude, zone.Geography.Longitude,
			heightInBaseUnits * actor.Gameworld.UnitManager.BaseHeightToMetres, zone.Shard.SphericalRadiusMetres);
		actor.OutputHandler.Send(
			$"You change the elevation of zone {zone.Name.TitleCase().Colour(Telnet.Green)} (#{zone.Id:N0}) to {actor.Gameworld.UnitManager.Describe(heightInBaseUnits, UnitType.Length, actor).Colour(Telnet.Green)} above sea level.");
	}

	protected static void ZoneEditLatitude(ICharacter actor, IEditableZone zone, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What latitude would you like to set for this zone?");
			return;
		}

		if (!double.TryParse(command.Pop(), out var latitude))
		{
			actor.OutputHandler.Send("You must enter a number of degrees of latitude for this zone.");
			return;
		}

		if (latitude > 90 || latitude < -90)
		{
			actor.OutputHandler.Send("Latitudes must be between 90 and -90 degrees.");
			return;
		}

		zone.Geography = new GeographicCoordinate(latitude.DegreesToRadians(), zone.Geography.Longitude,
			zone.Geography.Elevation, zone.Shard.SphericalRadiusMetres);
		actor.OutputHandler.Send(string.Format(actor, "You change the latitude of zone {0} (#{1:N0}) to {2:N6} {3}.",
			zone.Name.TitleCase().Colour(Telnet.Green),
			zone.Id,
			Math.Abs(latitude),
			latitude > 0 ? "North" : "South"
		));
	}

	protected static void ZoneEditLongitude(ICharacter actor, IEditableZone zone, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What longitude would you like to set for this zone?");
			return;
		}

		if (!double.TryParse(command.Pop(), out var longitude))
		{
			actor.OutputHandler.Send("You must enter a number of degrees of longitude for this zone.");
			return;
		}

		if (longitude > 180 || longitude < -180)
		{
			actor.OutputHandler.Send("Longitudes must be between 180 and -180 degrees.");
			return;
		}

		zone.Geography = new GeographicCoordinate(zone.Geography.Latitude, longitude.DegreesToRadians(),
			zone.Geography.Elevation, zone.Shard.SphericalRadiusMetres);
		actor.OutputHandler.Send(string.Format(actor, "You change the latitude of zone {0} (#{1:N0}) to {2:N6} {3}.",
			zone.Name.TitleCase().Colour(Telnet.Green),
			zone.Id,
			Math.Abs(longitude),
			longitude > 0 ? "East" : "West"
		));
	}

	protected static void ZoneEditAmbientLight(ICharacter actor, IEditableZone zone, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What amount of ambient light pollution would you like to set for this zone?");
			return;
		}

		if (!double.TryParse(command.Pop(), out var level))
		{
			actor.OutputHandler.Send(
				"You must enter an amount of light in lux for this zone's ambient light pollution.");
			return;
		}

		if (level < 0)
		{
			actor.OutputHandler.Send("Zones cannot have negative ambient light.");
			return;
		}

		zone.AmbientLightPollution = level;
		actor.OutputHandler.Send(string.Format(actor,
			"You change the ambient light pollution of zone {0} (#{1:N0}) to {2:N5} lux.",
			zone.Name.TitleCase().Colour(Telnet.Green),
			zone.Id,
			level
		));
	}

	protected static void ZoneEditTimezone(ICharacter actor, IEditableZone zone, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which clock do you want to set a timezone for?");
			return;
		}

		var clockText = command.PopSpeech();
		if (command.IsFinished)
		{
			actor.Send("Which timezone do you want to set for that clock?");
			return;
		}

		var timezoneText = command.PopSpeech();

		var clock = long.TryParse(clockText, out var value)
			? actor.Gameworld.Clocks.Get(value)
			: actor.Gameworld.Clocks.FirstOrDefault(
				  x => x.Alias.Equals(clockText, StringComparison.InvariantCultureIgnoreCase)) ??
			  actor.Gameworld.Clocks.FirstOrDefault(
				  x => x.Description.Equals(clockText, StringComparison.InvariantCultureIgnoreCase));
		if (clock == null)
		{
			actor.Send("There is no such clock.");
			return;
		}

		var timeZone = clock.Timezones.GetByIdOrNames(timezoneText);
		if (timeZone == null)
		{
			actor.Send("There is no such timezone for the {0} clock.", clock.Alias);
			return;
		}

		zone.TimeZones[clock] = timeZone;
		zone.Changed = true;
		actor.Send("You set the timezone for {0} with the {1} clock to be {2}.",
			zone.Name.Colour(Telnet.Green),
			clock.Alias.Colour(Telnet.Green),
			timeZone.Description.Colour(Telnet.Green)
		);
	}

	protected static void ZoneEdit(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which zone do you want to edit?");
			return;
		}

		var izone = long.TryParse(command.PopSpeech(), out var value)
			? actor.Gameworld.Zones.Get(value)
			: actor.Gameworld.Zones.FirstOrDefault(
				x => x.Name.Equals(command.Last, StringComparison.InvariantCultureIgnoreCase));
		if (izone == null)
		{
			actor.OutputHandler.Send("There is no such zone.");
			return;
		}

		var zone = izone.GetEditableZone;
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What about that zone do you want to edit?");
			return;
		}

		switch (command.PopForSwitch())
		{
			case "name":
				ZoneEditName(actor, zone, command);
				break;
			case "lat":
			case "latitude":
				ZoneEditLatitude(actor, zone, command);
				break;
			case "long":
			case "longitude":
				ZoneEditLongitude(actor, zone, command);
				break;
			case "elevation":
				ZoneEditElevation(actor, zone, command);
				break;
			case "light":
			case "ambientlight":
				ZoneEditAmbientLight(actor, zone, command);
				break;
			case "timezone":
				ZoneEditTimezone(actor, zone, command);
				break;
			case "forage":
			case "forageprofile":
			case "foragableprofile":
			case "profile":
			case "fp":
				ZoneEditForageProfile(actor, zone, command);
				break;
			case "weather":
				ZoneEditWeather(actor, zone, command);
				return;
			default:
				actor.OutputHandler.Send("What about that zone do you want to edit?");
				return;
		}
	}

	private static void ZoneEditWeather(ICharacter actor, IEditableZone zone, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which weather controller do you want to set for this zone?");
			return;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			zone.Weather = null;
			actor.OutputHandler.Send($"The {zone.Name.ColourName()} zone no longer has any weather.");
			return;
		}

		var wc = actor.Gameworld.WeatherControllers.GetByIdOrName(command.SafeRemainingArgument);
		if (wc is null)
		{
			actor.OutputHandler.Send($"There is no such weather controller identified by the text {command.SafeRemainingArgument.ColourCommand()}.");
			return;
		}

		zone.Weather = wc;
		actor.OutputHandler.Send($"The {zone.Name.ColourName()} zone now uses the {wc.Name.ColourValue()} weather controller.");
	}

	private static void ZoneEditForageProfile(ICharacter actor, IEditableZone zone, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Do you want to clear the profile, or set one for this zone?");
			return;
		}

		if (command.Peek().Equals("clear", StringComparison.InvariantCultureIgnoreCase))
		{
			if (zone.ForagableProfile == null)
			{
				actor.Send("That zone does not have a foragable profile to clear.");
				return;
			}

			zone.ForagableProfile = null;
			actor.Send("You clear the foragable profile from zone {0} ({1}).", zone.Name, zone.Id);
			return;
		}

		var profile = long.TryParse(command.PopSpeech(), out var value)
			? actor.Gameworld.ForagableProfiles.Get(value)
			: actor.Gameworld.ForagableProfiles.GetByName(command.Last, true);

		if (profile == null)
		{
			actor.Send("There is no such foragable profile for you to assign to this zone.");
			return;
		}

		if (profile.Status != RevisionStatus.Current)
		{
			actor.Send("You may only assign approved foragable profiles to zones.");
			return;
		}

		zone.ForagableProfile = profile;
		actor.Send("You set the foragable profile for zone {0} ({1:N0}) to be {2} ({3:N0}).", zone.Name, zone.Id,
			profile.Name, profile.Id);
	}

	protected static void ZoneCreate(ICharacter actor, StringStack command)
	{
		if (actor.CurrentOverlayPackage == null ||
			actor.CurrentOverlayPackage.Status != RevisionStatus.UnderDesign)
		{
			actor.OutputHandler.Send(
				"You must be editing an Under Design status cell overlay package to create a new zone.");
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to your new zone?");
			return;
		}

		var zoneName = command.PopSpeech().TitleCase();

		if (actor.Gameworld.Zones.Any(x => x.Name.Equals(zoneName, StringComparison.InvariantCultureIgnoreCase)))
		{
			actor.OutputHandler.Send("There is already a zone with that name. Zone names must be unique.");
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("In which shard will this new zone reside?");
			return;
		}

		var shard = long.TryParse(command.PopSpeech(), out var value)
			? actor.Gameworld.Shards.Get(value)
			: actor.Gameworld.Shards.FirstOrDefault(
				x => x.Name.Equals(command.Last, StringComparison.InvariantCultureIgnoreCase));
		if (shard == null)
		{
			actor.OutputHandler.Send("There is no such shard.");
			return;
		}

		if (!shard.Clocks.Any())
		{
			actor.OutputHandler.Send(
				$"The {shard.Name.ColourName()} shard does not have any clocks associated with it. You must first give it at least one clock before you can create any zones.");
			return;
		}

		var timezones = new Dictionary<IClock, IMudTimeZone>();
		foreach (var clock in shard.Clocks)
		{
			if (command.IsFinished)
			{
				actor.Send("You must specify a timezone for the {0} clock.",
					clock.Alias.TitleCase().Colour(Telnet.Green));
				return;
			}

			var timezoneText = command.PopSpeech();
			var timezone = clock.Timezones.GetByIdOrNames(timezoneText);
			if (timezone == null)
			{
				actor.Send("There is no such timezone for the {0} clock.",
					clock.Alias.TitleCase().Colour(Telnet.Green));
				return;
			}

			timezones.Add(clock, timezone);
		}

#if DEBUG
#else
			try {
#endif
		using (new FMDB())
		{
			var dbzone = new Models.Zone();
			FMDB.Context.Zones.Add(dbzone);

			dbzone.Name = zoneName.TitleCase();
			dbzone.ShardId = shard.Id;
			dbzone.Latitude = 0;
			dbzone.Longitude = 0;
			dbzone.Elevation = 0;
			dbzone.AmbientLightPollution = 0;
			FMDB.Context.SaveChanges();

			foreach (var timezone in timezones)
			{
				var dbtz = new ZonesTimezones();
				FMDB.Context.ZonesTimezones.Add(dbtz);
				dbtz.Zone = dbzone;
				dbtz.TimezoneId = timezone.Value.Id;
				dbtz.ClockId = timezone.Key.Id;
			}

			FMDB.Context.SaveChanges();


			var dbroom = new Models.Room();
			FMDB.Context.Rooms.Add(dbroom);

			dbroom.Zone = dbzone;
			dbroom.X = 0;
			dbroom.Y = 0;
			dbroom.Z = 0;

			FMDB.Context.SaveChanges();

			var dbcell = new Models.Cell
			{
				EffectData = "<Effects/>"
			};
			FMDB.Context.Cells.Add(dbcell);

			dbcell.Room = dbroom;
			dbzone.DefaultCell = dbcell;

			FMDB.Context.SaveChanges();

			var dboverlay = new Models.CellOverlay();
			FMDB.Context.CellOverlays.Add(dboverlay);

			dboverlay.Cell = dbcell;
			dboverlay.CellOverlayPackageId = actor.CurrentOverlayPackage.Id;
			dboverlay.CellOverlayPackageRevisionNumber = actor.CurrentOverlayPackage.RevisionNumber;
			dboverlay.AddedLight = 0;
			dboverlay.AmbientLightFactor = 1.0;
			dboverlay.CellDescription =
				"This is a newly built location that has not yet been described. It should not be approved for use in game.";
			dboverlay.CellName = "An Unnamed Location";
			dboverlay.Name = actor.CurrentOverlayPackage.Name;
			dboverlay.Terrain = FMDB.Context.Terrains.First(x => x.DefaultTerrain);
			dboverlay.OutdoorsType = (int)CellOutdoorsType.Outdoors;
			dbcell.CurrentOverlay = dboverlay;
			dboverlay.AtmosphereId = actor.Gameworld.GetStaticLong("DefaultAtmosphereId");
			dboverlay.AtmosphereType = actor.Gameworld.GetStaticConfiguration("DefaultAtmosphereType");

			FMDB.Context.SaveChanges();

			var newZone = new Construction.Zone(dbzone, actor.Gameworld);
			actor.Gameworld.Add(newZone);
			var newRoom = new Construction.Room(dbroom, newZone);
			actor.Gameworld.Add(newRoom);
			var newCell = new Construction.Cell(dbcell, newRoom);
			actor.Gameworld.Add(newCell);
			newZone.PostLoadSetup();

			actor.Send("You create zone {0} (#{1:N0}), with default cell #{2:N0}.",
				zoneName.Colour(Telnet.Green), newZone.Id, newCell.Id);
		}
#if DEBUG
#else
			}
			catch (Exception e) {
				Console.WriteLine("Exception in Zone Creation: " + e.Message);
			}
#endif
	}

	private static void ZoneShow(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which zone do you want to view?");
			return;
		}

		var zone = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.Zones.Get(value)
			: actor.Gameworld.Zones.GetByName(ss.Last);
		if (zone == null)
		{
			actor.OutputHandler.Send("There is no such zone.");
			return;
		}

		actor.OutputHandler.Send(zone.ShowToBuilder(actor));
	}

	#endregion



	public const string TimeZoneHelp = @"This command is used to create and edit in-game timezones for a particular clock. Timezones are relative to a standard default timezone for that clock - a real world example would be UTC.

The syntax for this command is as follows:

	#3timezone list [<clock>]#0 - lists all of the time zones
	#3timezone create <clock> <alias> ""<name>"" <hoursoffset> [<minutesoffset>]#0 - create a new timezone
	#3timezone edit <clock> <alias> ""<name>"" <hoursoffset> [<minutesoffset>]#0 - edit an existing timezone";

	[PlayerCommand("Timezone", "timezone")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("timezone", TimeZoneHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void Timezone(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());

		switch (ss.PopForSwitch())
		{
			case "list":
				TimeZoneList(actor, ss);
				return;
			case "create":
			case "new":
				TimeZoneCreate(actor, ss);
				return;
			case "edit":
			case "update":
				TimeZoneEdit(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(TimeZoneHelp.SubstituteANSIColour());
				return;
		}
	}

	private static void TimeZoneList(ICharacter actor, StringStack ss)
	{
		ShowModule.Show_Timezones(actor, ss);
	}

	private static void TimeZoneEdit(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("Which clock do you want to edit a timezone for?");
			return;
		}

		var clock = actor.Gameworld.Clocks.GetByIdOrNames(ss.PopSpeech());
		if (clock is null)
		{
			actor.Send("There is no such clock.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("What is the alias of the timezone you want to edit (e.g. #6GMT#0)?".SubstituteANSIColour());
			return;
		}

		var alias = ss.PopSpeech();
		if (!clock.Timezones.Any(x => x.Alias.Equals(alias, StringComparison.InvariantCultureIgnoreCase)))
		{
			actor.Send($"There is no timezone for the {clock.Name.ColourName()} clock with that alias.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("What name do you want to set for the timezone (e.g. #6\"Pacific Standard Time\"#0)?".SubstituteANSIColour());
			return;
		}

		var name = ss.PopSpeech().TitleCase();

		if (ss.IsFinished)
		{
			actor.Send("How many hours offset from the base should this timezone be?");
			return;
		}

		if (!int.TryParse(ss.PopSpeech(), out var hoursoffset))
		{
			actor.Send("You must enter a number of hours for this timezone to be offset.");
			return;
		}

		var minutesoffset = 0;
		if (!ss.IsFinished)
		{
			if (!int.TryParse(ss.SafeRemainingArgument, out minutesoffset))
			{
				actor.Send(
					"You must enter a number of minutes for this timezone to be offset, or enter nothing at all.");
				return;
			}
		}

		var timezone = clock.Timezones.GetByIdOrNames(alias)!;
		var eTimezone = (IEditableMudTimeZone)timezone;
		eTimezone.Description = name;
		eTimezone.OffsetHours = hoursoffset;
		eTimezone.OffsetMinutes = minutesoffset;
		actor.Send("You set the timezone {0} from the {1} clock to be called ({2}) and have offset {3}.",
			timezone.Alias.ColourValue(),
			clock.Name.ColourName(),
			timezone.Description.ColourName(),
			new TimeSpan(0, hoursoffset, minutesoffset, 0, 0).Describe().ColourValue()
		);
	}

	private static void TimeZoneCreate(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("Which clock do you want to create a timezone for?");
			return;
		}

		var clock = actor.Gameworld.Clocks.GetByIdOrNames(ss.PopSpeech());
		if (clock is null)
		{
			actor.Send("There is no such clock.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("What alias do you want to give to your timezone (e.g. #6GMT#0)?".SubstituteANSIColour());
			return;
		}

		var alias = ss.PopSpeech();
		if (clock.Timezones.Any(x => x.Alias.Equals(alias, StringComparison.InvariantCultureIgnoreCase)))
		{
			actor.Send("There is already a timezone with that alias for that clock. The alias must be unique.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("What name do you want to give to your timezone (e.g. #6\"Pacific Standard Time\"#0)?".SubstituteANSIColour());
			return;
		}

		var name = ss.PopSpeech().TitleCase();

		if (ss.IsFinished)
		{
			actor.Send("How many hours offset from the base should this timezone be?");
			return;
		}

		if (!int.TryParse(ss.PopSpeech(), out var hoursoffset))
		{
			actor.Send("You must enter a number of hours for this timezone to be offset.");
			return;
		}

		var minutesoffset = 0;
		if (!ss.IsFinished)
		{
			if (!int.TryParse(ss.SafeRemainingArgument, out minutesoffset))
			{
				actor.Send(
					"You must enter a number of minutes for this timezone to be offset, or enter nothing at all.");
				return;
			}
		}

		var timezone = new MudTimeZone(clock, hoursoffset, minutesoffset, name, alias);
		clock.AddTimezone(timezone);
		actor.Send("You create timezone #{0:N0} - {1} ({2}), offset {3}.",
			timezone.Id,
			timezone.Description.ColourName(),
			timezone.Alias.ColourValue(),
			new TimeSpan(0, hoursoffset, minutesoffset, 0, 0).Describe().ColourValue()
		);
	}

	private const string ShardHelpText =
		@"The shard command is used to create and edit shards, which can be thought of as planets, planes/realms or similar. The key things about shards is that they know what celestial objects and clocks they have.

You can use the following subcommands:

	#3shard list#0 - lists all of the shards
	#3shard new <name> <sky template>#0 - creates a new shard with the specified sky template
	#3shard set <shard> name <newname>#0 - renames a shard
	#3shard set <shard> clocks <clock1> ... [<clockn>]#0 - sets which clocks apply in this shard
	#3shard set <shard> calendars <calendar1> ... [<calendarn>]#0 - sets which calendars apply in this shard
	#3shard set <shard> celestials <celestial1> ... [<celestialn>]#0 - sets which celestials this shard has
	#3shard set <shard> sky <which>#0 - sets the sky template
	#3shard set <shard> lux <##>#0 - sets the base illumination for the whole shard";

	[PlayerCommand("Shard", "shard")]
	[CommandPermission(PermissionLevel.HighAdmin)]
	[HelpInfo("shard", ShardHelpText, AutoHelp.HelpArgOrNoArg)]
	protected static void Shard(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "list":
				Shards(actor, input);
				return;
			case "create":
			case "new":
				ShardCreate(actor, ss);
				break;
			case "set":
			case "edit":
				ShardEdit(actor, ss);
				break;
			default:
				actor.OutputHandler.Send(ShardHelpText.SubstituteANSIColour());
				return;
		}
	}

	[PlayerCommand("Shards", "shards")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	protected static void Shards(ICharacter actor, string input)
	{
		actor.OutputHandler.Send(
			StringUtilities.GetTextTable(
				from shard in actor.Gameworld.Shards
				select
					new[]
					{
						shard.Id.ToString("N0", actor),
						shard.Name,
						shard.Calendars.Select(x => x.ShortName.TitleCase()).ListToString(),
						shard.Clocks.Select(x => x.Alias.TitleCase()).ListToString(),
						shard.Celestials.Select(x => x.Name.TitleCase()).ListToString(),
						shard.GetEditableShard.SkyDescriptionTemplate.Name.TitleCase(),
						shard.MinimumTerrestrialLux.ToString("N5", actor)
					},
				new[] { "ID#", "Name", "Calendars", "Clocks", "Celestials", "Sky", "Minimum Lux" },
				actor.Account.LineFormatLength, colour: Telnet.Green, unicodeTable: actor.Account.UseUnicode
			)
		);
	}

	#region CellSet

	private static void CellSet(ICharacter actor, StringStack input)
	{
		if (input.Peek().EqualTo("exit"))
		{
			input.Pop();
			CellExit(actor, input);
			return;
		}

		if (input.Peek().Equals("register", StringComparison.InvariantCultureIgnoreCase))
		{
			input.Pop();
			CellSetRegister(actor, input);
			return;
		}

		if (actor.CurrentOverlayPackage == null)
		{
			actor.OutputHandler.Send(
				"You do not have a Cell Overlay Package prepared, and so cannot edit any Cells.");
			return;
		}

		if (actor.CurrentOverlayPackage.Status != RevisionStatus.UnderDesign)
		{
			actor.OutputHandler.Send(
				"Only a Cell Overlay Package that is in the Under Design status can be used to edit Cells.");
			return;
		}

		switch (input.PopSpeech().ToLowerInvariant())
		{
			case "link":
				CellEditLink(actor, input);
				break;
			case "nlink":
				CellEditNlink(actor, input);
				break;
			case "name":
				CellEditName(actor, input);
				break;
			case "desc":
			case "description":
				CellEditDescription(actor, input);
				break;
			case "suggestdesc":
			case "suggestdescription":
				CellEditSuggestDescription(actor, input);
				break;
			case "terrain":
				CellEditTerrain(actor, input);
				break;
			case "hearing":
			case "hearing profile":
				CellEditHearingProfile(actor, input);
				break;
			case "multiplier":
			case "lightmultiplier":
			case "light multiplier":
				CellEditLightMultiplier(actor, input);
				break;
			case "light":
			case "level":
			case "light level":
			case "lightlevel":
				CellEditLightLevel(actor, input);
				break;
			case "outdoors":
			case "type":
				CellEditOutdoors(actor, input);
				break;
			case "door":
				CellSetDoor(actor, input);
				break;
			case "forage":
			case "forage profile":
			case "forageprofile":
			case "foragable profile":
			case "foragableprofile":
			case "profile":
				CellSetForageProfile(actor, input);
				break;
			case "atmosphere":
				CellSetAtmosphere(actor, input);
				break;
			case "safequit":
			case "quit":
			case "safe":
			case "safe quit":
			case "safe_quit":
				CellSetSafeQuit(actor, input);
				break;
			default:
				actor.OutputHandler.Send("That is not a valid option for the " + "cell edit".Colour(Telnet.Cyan) +
										 " command.");
				return;
		}
	}

	private static void CellEditSuggestDescription(ICharacter actor, StringStack command)
	{
		var overlay = actor.Location.GetOrCreateOverlay(actor.CurrentOverlayPackage);
		var sb = new StringBuilder();
		sb.AppendLine(Futuremud.Games.First().GetStaticString("GPT_RoomSuggestionPrompt"));
		sb.AppendLine();
		sb.AppendLine($"The room that you are describing has a title of \"{overlay.CellName}\". It has a terrain type of {overlay.Terrain.Name}.");
		sb.AppendLine();
		if (actor.Location.ExitsFor(actor, true).Any())
		{
			sb.AppendLine($"There are other rooms adjacent to this one, which are as follows:");
			sb.AppendLine();
			foreach (var item in actor.Location.ExitsFor(actor, true))
			{
				if (item.OutboundDirection == CardinalDirection.Unknown)
				{
					sb.AppendLine($"Accessible from here is {item.Destination.GetOverlayFor(actor).CellName}");
				}
				else
				{
					sb.AppendLine($"To {item.OutboundDirectionDescription} is {item.Destination.GetOverlayFor(actor).CellName}");
				}
			}
		}

		sb.AppendLine();
		sb.AppendLine("You may refer to these other locations if it adds interesting detail, but do not spend more than a sentence or two on it, and none unless it's to add an interesting detail.");
		sb.AppendLine();
		sb.AppendLine($"The engine uses a few special markup formats that you can employ if you choose, which help it dynamically parse for the situation at the time like current weather, literacy of the viewer etc. They are listed below.");
		sb.AppendLine();
		sb.AppendLine(@$"Written Text: writing{{language,script,style=...,colour=...,minskill}}{{text if you understand}}{{text if you cant}}
{(actor.Location.Shop is not null ? "Name of the Shop at this location: @shop\n" : "")}Weather/Light/Time: environment{{conditions=text}}{{optional more conditions up to 8 times=text}}{{fallback if none triggered}}

Conditions for 'environment' include:

Times: day, night, morning, afternoon, dusk, dawn, notnight
Light: pitch black, almost completely dark, extremely dark, very dark, dark, dim, soft, normal, bright, very bright, extremely bright
Light: Add a > or < to light levels to check higher than (inclusive) or lower than (exclusive)
Seasons: the dry season, the cold season, the rainy season, the windy season
Precipitation: parched, dry, humid, lightrain, rain, heavyrain, torrentialrain, lightsnow, snow, heavysnow, blizzard, sleet
Precipitation: Add a * to the front of the above to check highest recent precipitation (e.g. *rain to see if it's rained recently)
Precipitation: Add a > or < to precipitation to check higher than (inclusive) or lower than (exclusive) e.g. >lightrain, <heavyrain

Note: reverse any condition with a ! (e.g. !dawn, !snow, !*rain, !summer)");
		sb.AppendLine();

		if (!command.IsFinished)
		{
			sb.AppendLine();
			sb.AppendLine(command.SafeRemainingArgument);
		}

		void GPTCallback(string text)
		{
			var descriptions = text.Split('#', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
			var sb = new StringBuilder();
			sb.AppendLine($"Your GPT Model has made the following suggestions for descriptions for room {overlay.CellName.ColourRoom()}:");
			var i = 1;
			foreach (var desc in descriptions)
			{
				sb.AppendLine();
				sb.AppendLine($"Suggestion {i++.ToString("N0", actor)}".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
				sb.AppendLine();
				sb.AppendLine(desc.Wrap(actor.InnerLineFormatLength, "\t"));
			}

			sb.AppendLine();
			sb.AppendLine($"You can apply one of these by using the syntax {"accept desc <n>".Colour(Telnet.Cyan)}, such as {"accept desc 1".Colour(Telnet.Cyan)}.");
			actor.OutputHandler.Send(sb.ToString());
			actor.AddEffect(new Accept(actor, new GenericProposal
			{
				DescriptionString = "Applying a GPT Description suggestion",
				AcceptAction = text =>
				{
					if (!int.TryParse(text, out var value) || value < 1 || value > descriptions.Length)
					{
						actor.OutputHandler.Send("You did not specify a valid description. If you still want to use the descriptions, you'll have to copy them in manually.");
						return;
					}

					overlay.CellDescription = descriptions[value - 1];
					overlay.Changed = true;
					actor.OutputHandler.Send($"You set the description of the room {overlay.CellName} to the {value.ToOrdinal()} suggestion.");
				},
				RejectAction = text => { actor.OutputHandler.Send("You decide not to use any of the suggestions."); },
				ExpireAction = () => { actor.OutputHandler.Send("You decide not to use any of the suggestions."); },
				Keywords = new List<string> { "description", "suggestion" }
			}), TimeSpan.FromSeconds(120));
		}

		switch (actor.Gameworld.GetStaticConfiguration("DescSuggestionAIModel").ToLowerInvariant())
		{
			case "openai":
				var descModel = Futuremud.Games.First().GetStaticConfiguration("GPT_DescSuggestion_Model");
				if (!OpenAIHandler.MakeGPTRequest(sb.ToString(), actor.Gameworld.GetStaticString("GPT_RoomSuggestionFinalWord"), GPTCallback, descModel))
				{
					actor.OutputHandler.Send("Your GPT Model is not set up correctly, so you cannot get any suggestions.");
					return;
				}
				break;
			case "anthropic":
				var anthropicModel = Futuremud.Games.First().GetStaticConfiguration("AnthropicDefaultModel");
				if (!OpenAIHandler.MakeAnthropicRequest(sb.ToString(), actor.Gameworld.GetStaticString("GPT_RoomSuggestionFinalWord"), GPTCallback, anthropicModel))
				{
					actor.OutputHandler.Send("Your Anthropic Model is not set up correctly, so you cannot get any suggestions.");
					return;
				}
				break;
			default:
				actor.OutputHandler.Send($"The value of the #3DescSuggestionAIModel#0 static configuration must be either #3openai#0 or #3anthropic#0.");
				return;
		}
		
		actor.OutputHandler.Send("You send your request off to the GPT Model.");
	}

	private static void CellSetSafeQuit(ICharacter actor, StringStack input)
	{
		var overlay = actor.Location.GetOrCreateOverlay(actor.CurrentOverlayPackage);
		overlay.SafeQuit = !overlay.SafeQuit;
		actor.OutputHandler.Send($"This location is {(overlay.SafeQuit ? "now" : "no longer")} a safe quit room.");
	}

	private static void CellSetAtmosphere(ICharacter actor, StringStack input)
	{
		if (input.IsFinished)
		{
			actor.Send("Do you want to CLEAR the current atmosphere, or set it to a specific type?");
			return;
		}

		var isGas = false;
		switch (input.PopSpeech().ToLowerInvariant())
		{
			case "gas":
				isGas = true;
				break;
			case "liquid":
				break;
			case "clear":
			case "none":
			case "remove":
			case "rem":
				CellSetAtmosphereNone(actor);
				return;
			default:
				actor.Send(
					"You must specify either NONE to clear the atmosphere, or GAS or LIQUID to say what type of atmosphere it is.");
				return;
		}

		if (input.IsFinished)
		{
			actor.Send($"Which {(isGas ? "gas" : "liquid")} do you want to set as the atmosphere?");
			return;
		}

		IFluid fluid;
		var text = input.SafeRemainingArgument;
		if (isGas)
		{
			fluid = actor.Gameworld.Gases.GetByIdOrName(text);
		}
		else
		{
			fluid = actor.Gameworld.Liquids.GetByIdOrName(text);
		}

		if (fluid == null)
		{
			actor.Send($"There is no such {(isGas ? "gas" : "liquid")}.");
			return;
		}

		var overlay = actor.Location.GetOrCreateOverlay(actor.CurrentOverlayPackage);
		overlay.Atmosphere = fluid;
		actor.OutputHandler.Send($"This location now has {fluid.Name.Colour(fluid.DisplayColour)} as an atmosphere.");
	}

	private static void CellSetAtmosphereNone(ICharacter actor)
	{
		var overlay = actor.Location.GetOrCreateOverlay(actor.CurrentOverlayPackage);
		overlay.Atmosphere = null;
		actor.Send("This cell will no longer have any atmosphere.");
	}

	private static void CellSetForageProfile(ICharacter actor, StringStack input)
	{
		if (input.IsFinished)
		{
			actor.Send("Do you want to clear the profile, or set one for this cell?");
			return;
		}

		if (input.Peek().Equals("clear", StringComparison.InvariantCultureIgnoreCase))
		{
			if (actor.Location.ForagableProfile == null)
			{
				actor.Send("Your location does not have a foragable profile to clear.");
				return;
			}

			actor.Location.ForagableProfile = null;
			actor.Send("You clear the foragable profile from this cell.");
			return;
		}

		var profile = actor.Gameworld.ForagableProfiles.GetByIdOrName(input.SafeRemainingArgument);
		if (profile == null)
		{
			actor.Send("There is no such foragable profile for you to assign to this cell.");
			return;
		}

		if (profile.Status != RevisionStatus.Current)
		{
			actor.Send("You may only assign approved foragable profiles to cells.");
			return;
		}

		actor.Location.ForagableProfile = profile;
		actor.Send("You set the foragable profile for this cell to be {0} ({1:N0}).", profile.Name, profile.Id);
	}

	private static void CellSetRegister(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("You can either delete an existing variable, or set a value for one.");
			return;
		}

		if (command.Peek().Equals("delete", StringComparison.InvariantCultureIgnoreCase))
		{
			CellSetRegisterDelete(actor, command);
			return;
		}

		var variableName = command.Pop();
		var variableType = actor.Gameworld.VariableRegister.GetType(ProgVariableTypes.Location, variableName);
		if (variableType == ProgVariableTypes.Error)
		{
			actor.OutputHandler.Send($"There is no cell variable called {variableName.ColourName()} - you will need to register it first.");
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What value do you want to set for this variable?\nNote: To delete the variable, use {"cell set register delete <variable>".Colour(Telnet.Yellow)} instead");
			return;
		}

		var (result, success) = ProgModule.GetArgument(variableType, command.SafeRemainingArgument, 1, actor);
		if (!success)
		{
			return;
		}

		actor.Gameworld.VariableRegister.SetValue(actor.Location, variableName, FutureProg.FutureProg.GetVariable(variableType, result));
		actor.OutputHandler.Send($"You set the register value {variableName.Colour(Telnet.Cyan)} for this cell to {ProgModule.DescribeProgVariable(actor, variableType, result)}.");
	}

	private static void CellSetRegisterDelete(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which register value do you want to delete for this cell?");
			return;
		}

		var whichVariable = command.Pop().ToLowerInvariant();
		var type = actor.Gameworld.VariableRegister.GetType(ProgVariableTypes.Location, whichVariable);
		if (type == ProgVariableTypes.Error)
		{
			actor.Send("This cell does not have a register value of {0}.", whichVariable);
			return;
		}

		actor.Gameworld.VariableRegister.ResetValue(actor.Location, whichVariable);
		actor.Send(
			"You reset the register value for your current location for variable {0}. It will now use the system-wide default.",
			whichVariable);
	}

	private static void CellSetDoor(ICharacter actor, StringStack input)
	{
		if (input.IsFinished)
		{
			actor.OutputHandler.Send("Which exit do you want to edit the door information for?");
			return;
		}

		var currentOverlayExits = actor.Location.ExitsFor(actor);
		var exit = long.TryParse(input.Pop(), out var id)
			? currentOverlayExits.FirstOrDefault(x => x.Exit.Id == id)
			: currentOverlayExits.GetFromItemListByKeyword(input.Last, actor);
		if (exit == null)
		{
			actor.OutputHandler.Send("There is no such exit for you to edit.");
			return;
		}

		if (input.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to do to that exit with regards to doors?");
			return;
		}

		IExit newExit;
		IEditableCellOverlay overlay;
		var text = input.PopSpeech();
		if (text.Equals("clear", StringComparison.InvariantCultureIgnoreCase))
		{
			if (!exit.Exit.AcceptsDoor)
			{
				actor.OutputHandler.Send("That exit already does not accept doors.");
				return;
			}

			overlay = actor.Location.GetOrCreateOverlay(actor.CurrentOverlayPackage);
			var otherOverlay = exit.Destination.GetOrCreateOverlay(actor.CurrentOverlayPackage);

			var existingNonDoorExit =
				actor.Gameworld.ExitManager.GetAllExits(actor.Location)
					 .FirstOrDefault(
						 x =>
							 x.Origin == exit.Origin && x.Destination == exit.Destination &&
							 x.InboundDirection == exit.InboundDirection &&
							 x.OutboundDirection == exit.OutboundDirection && !x.Exit.AcceptsDoor);
			if (existingNonDoorExit != null)
			{
				newExit = existingNonDoorExit.Exit;
				actor.Send("That exit will no longer accept doors. Swapped in existing exit with ID {0:N0}.",
					newExit.Id);
			}
			else
			{
				newExit = exit.Exit.Clone();
				newExit.AcceptsDoor = false;
				foreach (var block in newExit.BlockedLayers.ToList())
				{
					newExit.RemoveBlockedLayer(block);
				}

				actor.Send("That exit will no longer accept doors. New ID is {0:N0}.", newExit.Id);
			}

			overlay.RemoveExit(exit.Exit);
			otherOverlay.RemoveExit(exit.Exit);
			overlay.AddExit(newExit);
			otherOverlay.AddExit(newExit);
			actor.Gameworld.ExitManager.UpdateCellOverlayExits(actor.Location, overlay);
			actor.Gameworld.ExitManager.UpdateCellOverlayExits(exit.Destination, otherOverlay);
		}
		else
		{
			var sizes = Enum.GetValues(typeof(SizeCategory)).OfType<SizeCategory>();
			if (!sizes.Any(x => x.Describe().Equals(text, StringComparison.InvariantCultureIgnoreCase)))
			{
				actor.OutputHandler.Send("That is not a valid size for the door.");
				return;
			}

			var target =
				sizes.FirstOrDefault(x => x.Describe().Equals(text, StringComparison.InvariantCultureIgnoreCase));
			overlay = actor.Location.GetOrCreateOverlay(actor.CurrentOverlayPackage);
			var otherOverlay = exit.Destination.GetOrCreateOverlay(actor.CurrentOverlayPackage);

			if (!actor.Location.Overlays.Except(overlay).Any(x => x.ExitIDs.Contains(exit.Exit.Id)))
			{
				newExit = exit.Exit;
				actor.Send("That exit will now accept doors of size {0}.", target.Describe().Colour(Telnet.Green));
			}
			else
			{
				newExit = exit.Exit.Clone();
				overlay.RemoveExit(exit.Exit);
				overlay.AddExit(newExit);
				otherOverlay.RemoveExit(exit.Exit);
				otherOverlay.AddExit(newExit);
				actor.Gameworld.ExitManager.UpdateCellOverlayExits(actor.Location, overlay);
				actor.Gameworld.ExitManager.UpdateCellOverlayExits(exit.Destination, otherOverlay);
				actor.Send("That exit will now accept doors of size {0}. New ID is {1:N0}.",
					target.Describe().Colour(Telnet.Green), newExit.Id);
			}

			newExit.AcceptsDoor = true;
			newExit.DoorSize = target;
			var newExitLayers = newExit.CellExitFor(actor.Location).WhichLayersExitAppears().ToList();
			if (newExitLayers.Count > 1)
			{
				foreach (var layer in newExitLayers)
				{
					if (layer == actor.RoomLayer)
					{
						continue;
					}

					newExit.AddBlockedLayer(layer);
				}

				if (!newExit.CellExitFor(actor.Location).WhichLayersExitAppears().Any())
				{
					newExit.RemoveBlockedLayer(newExitLayers.FirstMin(x => Math.Abs(x.LayerHeight())));
				}
			}
		}

		newExit.Changed = true;
	}

	private static void CellEditLightMultiplier(ICharacter actor, StringStack input)
	{
		if (input.IsFinished)
		{
			actor.OutputHandler.Send("What multiplier of natural light levels do you want this cell to have?");
			return;
		}

		if (!double.TryParse(input.Pop(), out var value))
		{
			actor.OutputHandler.Send(
				"You must enter a valid number to use as a multiplier for natural light levels.");
			return;
		}

		if (value < 0)
		{
			actor.OutputHandler.Send("The multipler cannot be negative.");
			return;
		}

		var overlay = actor.Location.GetOrCreateOverlay(actor.CurrentOverlayPackage);
		overlay.AmbientLightFactor = value;
		actor.Send("You set the Ambient Light Multipler for this cell to {0:N3}.", value);
	}

	private static void CellEditLightLevel(ICharacter actor, StringStack input)
	{
		if (input.IsFinished)
		{
			actor.OutputHandler.Send("What level of ambient light do you want this cell to have?");
			return;
		}

		if (!double.TryParse(input.Pop(), out var value))
		{
			actor.OutputHandler.Send("You must enter a valid number to use as the ambient light level.");
			return;
		}

		if (value < 0)
		{
			actor.OutputHandler.Send("The ambient light cannot be negative.");
			return;
		}

		var overlay = actor.Location.GetOrCreateOverlay(actor.CurrentOverlayPackage);
		overlay.AddedLight = value;
		actor.Send("You set the Ambient Light Level for this cell to {0:N3} lux.", value);
	}

	private static void CellEditOutdoors(ICharacter actor, StringStack input)
	{
		if (input.IsFinished)
		{
			actor.OutputHandler.Send("Do you want to set this as an Outdoors, Indoors, Exposed, Windows or Cave area?");
			return;
		}

		CellOutdoorsType type;
		var text = input.SafeRemainingArgument.ToLowerInvariant();
		switch (text)
		{
			case "outdoor":
			case "outdoors":
			case "outside":
				type = CellOutdoorsType.Outdoors;
				break;
			case "indoors":
			case "indoor":
			case "inside":
				type = CellOutdoorsType.Indoors;
				break;
			case "windows":
				type = CellOutdoorsType.IndoorsWithWindows;
				break;
			case "cave":
			case "nolight":
			case "no light":
				type = CellOutdoorsType.IndoorsNoLight;
				break;
			case "shelter":
			case "climate":
			case "sheltered":
			case "exposed":
				type = CellOutdoorsType.IndoorsClimateExposed;
				break;
			default:
				actor.OutputHandler.Send("Valid options are Outdoors, Indoors, Exposed, Cave or Windows.");
				return;
		}

		var overlay = actor.Location.GetOrCreateOverlay(actor.CurrentOverlayPackage);
		overlay.OutdoorsType = type;
		actor.Send("This location is now {0}.", type.Describe().Colour(Telnet.Green));
		if (type == CellOutdoorsType.IndoorsNoLight)
		{
			actor.Send("The light level multiplier has been set to {0:N1}".Colour(Telnet.Yellow), 0);
			overlay.AmbientLightFactor = 0.0;
		}
		else if (type != CellOutdoorsType.Outdoors)
		{
			actor.Send(
				"Don't forget to set a multiplier for the light levels in this room. Default has been set to 0.25 multiplier from natural."
					.Colour(Telnet.Yellow));
			overlay.AmbientLightFactor = 0.25;
		}
		else
		{
			actor.Send("The light level multiplier has been reset to {0:N1}".Colour(Telnet.Yellow), 1.0);
			overlay.AmbientLightFactor = 1.0;
		}
	}

	private static void CellEditTerrain(ICharacter actor, StringStack input)
	{
		if (input.IsFinished)
		{
			actor.OutputHandler.Send("Which terrain do you want to set for this cell?");
			return;
		}

		var terrainText = input.SafeRemainingArgument;
		var terrain = actor.Gameworld.Terrains.GetByIdOrName(terrainText);
		if (terrain == null)
		{
			actor.OutputHandler.Send("There is no such terrain type.");
			return;
		}

		var overlay = actor.Location.GetOrCreateOverlay(actor.CurrentOverlayPackage);
		overlay.Terrain = terrain;
		overlay.OutdoorsType = terrain.DefaultCellOutdoorsType;
		switch (overlay.OutdoorsType)
		{
			case CellOutdoorsType.Indoors:
				overlay.AmbientLightFactor = 0.25;
				break;
			case CellOutdoorsType.IndoorsWithWindows:
				overlay.AmbientLightFactor = 0.35;
				break;
			case CellOutdoorsType.Outdoors:
				overlay.AmbientLightFactor = 1.0;
				break;
			case CellOutdoorsType.IndoorsNoLight:
				overlay.AmbientLightFactor = 0.0;
				break;
			case CellOutdoorsType.IndoorsClimateExposed:
				overlay.AmbientLightFactor = 0.9;
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		actor.OutputHandler.Send("You set the Terrain for this cell to \"" +
								 terrain.Name.TitleCase().Colour(Telnet.Green) + "\"");
	}

	private static void CellEditHearingProfile(ICharacter actor, StringStack input)
	{
		if (input.IsFinished)
		{
			actor.OutputHandler.Send("Which terrain do you want to set for this cell?");
			return;
		}

		var profileText = input.SafeRemainingArgument;
		var profile = actor.Gameworld.HearingProfiles.GetByIdOrName(profileText);
		if (profile == null)
		{
			actor.OutputHandler.Send("There is no such hearing profile.");
			return;
		}

		var overlay = actor.Location.GetOrCreateOverlay(actor.CurrentOverlayPackage);
		overlay.HearingProfile = profile;
		actor.OutputHandler.Send("You set the Hearing Profile for this cell to \"" +
								 profile.Name.TitleCase().Colour(Telnet.Green) + "\"");
	}

	private static void CellExit(ICharacter actor, StringStack input)
	{
		var overlay = actor.Location.Overlays.FirstOrDefault(x => x.Package == actor.CurrentOverlayPackage);

		if (input.IsFinished || input.Peek().ToLowerInvariant() == "list")
		{
			var exits = actor.Gameworld.ExitManager.GetAllExits(actor.Location);
			actor.OutputHandler.Send($@"This Cell has the following exits{(overlay != null
				? $" ({"Note: Exits for your current Overlay appear with a [x] after them".Colour(Telnet.Red)})"
				: "")}:

{(from exit in exits
							 select exit.BuilderInformationString(actor)
									+
									(overlay != null && overlay.ExitIDs.Contains(exit.Exit.Id)
										? "[x]"
										: "")
				).ListToString(separator: "\n", conjunction: "")}"
			);
			return;
		}

		if (input.Peek().EqualTo("hide"))
		{
			input.Pop();
			CellExitHide(actor, input);
			return;
		}

		if (input.Peek().EqualTo("unhide"))
		{
			input.Pop();
			CellExitUnhide(actor, input);
			return;
		}

		if (actor.CurrentOverlayPackage == null)
		{
			actor.OutputHandler.Send(
				"You do not have a Cell Overlay Package prepared, and so cannot edit any Cells.");
			return;
		}

		if (actor.CurrentOverlayPackage.Status != RevisionStatus.UnderDesign)
		{
			actor.OutputHandler.Send(
				"Only a Cell Overlay Package that is in the Under Design status can be used to edit Cells.");
			return;
		}

		if (input.Peek().EqualTo("size"))
		{
			input.Pop();
			CellExitSize(actor, input, false);
			return;
		}

		if (input.Peek().EqualTo("upright"))
		{
			input.Pop();
			CellExitSize(actor, input, true);
			return;
		}

		if (input.Peek().EqualTo("fall"))
		{
			input.Pop();
			CellExitFall(actor, input);
			return;
		}

		if (input.Peek().EqualTo("climb"))
		{
			input.Pop();
			CellExitClimb(actor, input);
			return;
		}

		if (input.Peek().EqualTo("reset"))
		{
			input.Pop();
			CellExitReset(actor, input);
			return;
		}

		if (input.Peek().EqualTo("block"))
		{
			input.Pop();
			CellExitBlock(actor, input);
			return;
		}

		if (input.Peek().EqualTo("unblock"))
		{
			input.Pop();
			CellExitUnblock(actor, input);
			return;
		}

		var add = false;
		switch (input.Pop().ToLowerInvariant())
		{
			case "add":
				add = true;
				break;
			case "remove":
			case "rem":
			case "delete":
			case "del":
				break;
			default:
				actor.OutputHandler.Send(@"The valid options for this command are as follows:

	#3add <id>#0 - adds an existing exit from another overlay for this cell to this overlay
	#3remove <id>#0 - removes an exit from this overlay
	#3size <id|direction> <size>#0 - sets the maximum size of creatures that can use the exit
	#3upright <id|direction> <size>#0 - sets the maximum size of creatures that can use the exit in a standing position
	#3reset <id|direction>#0 - turns a climb/fall exit into a regular exit
	#3fall <id|direction>#0 - turns an up/down exit into a fall exit or toggles it off
	#3climb <id|direction> <difficulty>#0 - turns an exit into a climb exit with a specified difficulty
	#3climb <id|direction>#0 - toggles a climb exit off
	#3block <id|direction> <layer>#0 - blocks an exit from appearing in a specified layer
	#3unblock <id|direction> <layer>#0 - removes a block on an exit from appearing in a specified layer".SubstituteANSIColour());
				return;
		}

		if (!long.TryParse(input.Pop(), out var value))
		{
			actor.OutputHandler.Send($"You must enter the ID of the exit you want to {(add ? "add" : "remove")}.");
			return;
		}

		var texit = actor.Gameworld.ExitManager.GetExitByID(value);
		if (texit == null)
		{
			actor.OutputHandler.Send("There is no such exit.");
			return;
		}

		if (texit.CellExitFor(actor.Location) == null)
		{
			actor.OutputHandler.Send("That exit is not a valid choice for this location.");
			return;
		}

		var editableOverlay = actor.Location.GetOrCreateOverlay(actor.CurrentOverlayPackage);
		if (add)
		{
			if (editableOverlay.ExitIDs.Contains(texit.Id))
			{
				actor.OutputHandler.Send("The current Cell Overlay already contains that exit.");
				return;
			}

			editableOverlay.AddExit(texit);
			// We also have to add this exit to an overlay at its destination.
			var otherOverlay =
				texit.CellExitFor(actor.Location).Destination.GetOrCreateOverlay(actor.CurrentOverlayPackage);
			otherOverlay.AddExit(texit);

			actor.OutputHandler.Send("You add the Exit with ID " + texit.Id + " to this Location's Cell Overlay.");
		}
		else
		{
			if (!editableOverlay.ExitIDs.Contains(texit.Id))
			{
				actor.OutputHandler.Send("The current Cell Overlay does not contain that exit.");
				return;
			}

			editableOverlay.RemoveExit(texit);
			var otherOverlay =
				texit.CellExitFor(actor.Location).Destination.GetOrCreateOverlay(actor.CurrentOverlayPackage);
			otherOverlay.RemoveExit(texit);

			actor.OutputHandler.Send("You remove the Exit with ID " + texit.Id +
									 " from this Location's Cell Overlay.");
		}
	}

	private static void CellExitHide(ICharacter actor, StringStack input)
	{
		if (input.IsFinished)
		{
			actor.OutputHandler.Send("Which exit would you like to hide?");
			return;
		}

		var currentOverlayExits = actor.Location.ExitsFor(actor, true);
		var exit = long.TryParse(input.Pop(), out var id)
			? currentOverlayExits.FirstOrDefault(x => x.Exit.Id == id)
			: currentOverlayExits.GetFromItemListByKeyword(input.Last, actor);
		if (exit == null)
		{
			actor.OutputHandler.Send("There is no such exit for you to edit.");
			return;
		}

		if (actor.Location.EffectsOfType<IExitHiddenEffect>().Any(x => x.Exit == exit.Exit))
		{
			actor.OutputHandler.Send(
				$"The exit to {exit.OutboundDirectionDescription.ColourValue()} is already hidden.");
			return;
		}

		if (input.IsFinished)
		{
			actor.OutputHandler.Send("Which prog do you want to use to control whether someone can see the exit?");
			return;
		}

		var prog = actor.Gameworld.FutureProgs.GetByIdOrName(input.SafeRemainingArgument);
		if (prog == null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return;
		}

		if (!prog.ReturnType.CompatibleWith(ProgVariableTypes.Boolean))
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that returns a boolean value, whereas {prog.MXPClickableFunctionName()} returns {prog.ReturnType.Describe().ColourName()}.");
			return;
		}

		if (!prog.MatchesParameters(new List<ProgVariableTypes>
				{ ProgVariableTypes.Location, ProgVariableTypes.Character }))
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that can accept a room and a character parameter, whereas {prog.MXPClickableFunctionName()} does not.");
			return;
		}

		actor.Location.AddEffect(new ExitHidden(actor.Location, exit.Exit, prog));
		actor.OutputHandler.Send(
			$"The exit to {exit.OutboundDirectionDescription.ColourValue()} is now hidden with the {prog.MXPClickableFunctionName()} prog as a filter.");
	}

	private static void CellExitUnhide(ICharacter actor, StringStack input)
	{
		if (input.IsFinished)
		{
			actor.OutputHandler.Send("Which exit would you like to unhide?");
			return;
		}

		var currentOverlayExits = actor.Location.ExitsFor(actor, true);
		var exit = long.TryParse(input.Pop(), out var id)
			? currentOverlayExits.FirstOrDefault(x => x.Exit.Id == id)
			: currentOverlayExits.GetFromItemListByKeyword(input.Last, actor);
		if (exit == null)
		{
			actor.OutputHandler.Send("There is no such exit for you to edit.");
			return;
		}

		if (actor.Location.EffectsOfType<IExitHiddenEffect>().All(x => x.Exit != exit.Exit))
		{
			actor.OutputHandler.Send($"The exit to {exit.OutboundDirectionDescription.ColourValue()} is not hidden.");
			return;
		}

		actor.Location.RemoveAllEffects<IExitHiddenEffect>(x => x.Exit == exit.Exit);
		actor.OutputHandler.Send($"The exit to {exit.OutboundDirectionDescription.ColourValue()} is no longer hidden.");
	}

	private static void CellExitUnblock(ICharacter actor, StringStack input)
	{
		if (input.IsFinished)
		{
			actor.OutputHandler.Send("You must specify which exit you want to unblock a layer for.");
			return;
		}

		var currentOverlayExits = actor.Location.ExitsFor(actor, true);
		var exit = long.TryParse(input.Pop(), out var id)
			? currentOverlayExits.FirstOrDefault(x => x.Exit.Id == id)
			: currentOverlayExits.GetFromItemListByKeyword(input.Last, actor);
		if (exit == null)
		{
			actor.OutputHandler.Send("There is no such exit for you to edit.");
			return;
		}

		if (input.IsFinished)
		{
			actor.OutputHandler.Send("Which room layer do you want to unblock?");
			return;
		}

		if (!input.PopSpeech().TryParseEnum<RoomLayer>(out var layer))
		{
			actor.OutputHandler.Send(
				$"There is no such room layer. The valid room layers are {Enum.GetNames(typeof(RoomLayer)).Select(x => x.ColourValue()).ListToString()}.");
			return;
		}

		var (newExit, newId) = GetOrCopyCellExit(exit, actor.CurrentOverlayPackage);
		newExit.RemoveBlockedLayer(layer);
		actor.OutputHandler.Send(
			$"You unblock layer {layer.DescribeEnum().ColourValue()} from that exit.{(newId != 0 ? $" New ID is {newId:N0}" : "")}");
	}

	private static void CellExitBlock(ICharacter actor, StringStack input)
	{
		if (input.IsFinished)
		{
			actor.OutputHandler.Send("You must specify which exit you want to block a layer for.");
			return;
		}

		var currentOverlayExits = actor.Location.ExitsFor(actor, true);
		var exit = long.TryParse(input.Pop(), out var id)
			? currentOverlayExits.FirstOrDefault(x => x.Exit.Id == id)
			: currentOverlayExits.GetFromItemListByKeyword(input.Last, actor);
		if (exit == null)
		{
			actor.OutputHandler.Send("There is no such exit for you to edit.");
			return;
		}

		if (input.IsFinished)
		{
			actor.OutputHandler.Send("Which room layer do you want to block?");
			return;
		}

		if (!input.PopSpeech().TryParseEnum<RoomLayer>(out var layer))
		{
			actor.OutputHandler.Send(
				$"There is no such room layer. The valid room layers are {Enum.GetNames(typeof(RoomLayer)).Select(x => x.ColourValue()).ListToString()}.");
			return;
		}

		var (newExit, newId) = GetOrCopyCellExit(exit, actor.CurrentOverlayPackage);
		newExit.AddBlockedLayer(layer);
		actor.OutputHandler.Send(
			$"You block layer {layer.DescribeEnum().ColourValue()} from that exit.{(newId != 0 ? $" New ID is {newId:N0}" : "")}");
	}

	private static void CellExitReset(ICharacter actor, StringStack input)
	{
		if (input.IsFinished)
		{
			actor.OutputHandler.Send(
				"You must specify which exit you want to turn change into a non-fall, non-climb exit.");
			return;
		}

		var currentOverlayExits = actor.Location.ExitsFor(actor, true);
		var exit = long.TryParse(input.Pop(), out var id)
			? currentOverlayExits.FirstOrDefault(x => x.Exit.Id == id)
			: currentOverlayExits.GetFromItemListByKeyword(input.Last, actor);
		if (exit == null)
		{
			actor.OutputHandler.Send("There is no such exit for you to edit.");
			return;
		}

		if (!exit.IsClimbExit && !exit.IsFallExit && !exit.Opposite.IsFallExit)
		{
			actor.OutputHandler.Send("That exit is already neither a fall exit nor a climb exit.");
			return;
		}

		var (newExit, newId) = GetOrCopyCellExit(exit, actor.CurrentOverlayPackage);
		newExit.FallCell = null;
		newExit.ClimbDifficulty = Difficulty.Normal;
		newExit.IsClimbExit = false;
		newExit.Changed = true;
		actor.OutputHandler.Send(
			$"That exit is no longer a fall exit or a climb exit.{(newId != 0 ? $" New ID is {newId:N0}" : "")}");
	}

	private static (IExit Exit, long NewId) GetOrCopyCellExit(ICellExit exit, ICellOverlayPackage package)
	{
		var overlay = exit.Origin.GetOrCreateOverlay(package);
		var otherOverlay = exit.Destination.GetOrCreateOverlay(package);
		if (!exit.Origin.Overlays.Except(overlay).Any(x => x.ExitIDs.Contains(exit.Exit.Id)))
		{
			return (exit.Exit, 0);
		}
		else
		{
			var newExit = exit.Exit.Clone();
			overlay.RemoveExit(exit.Exit);
			overlay.AddExit(newExit);
			otherOverlay.RemoveExit(exit.Exit);
			otherOverlay.AddExit(newExit);
			package.Gameworld.ExitManager.UpdateCellOverlayExits(exit.Origin, overlay);
			package.Gameworld.ExitManager.UpdateCellOverlayExits(exit.Destination, otherOverlay);
			return (newExit, newExit.Id);
		}
	}

	private static void CellExitFall(ICharacter actor, StringStack input)
	{
		if (input.IsFinished)
		{
			actor.OutputHandler.Send(
				"You must specify which exit you want to turn into (or turn back from) being a fall exit.");
			return;
		}

		var currentOverlayExits = actor.Location.ExitsFor(actor, true);
		var exit = long.TryParse(input.Pop(), out var id)
			? currentOverlayExits.FirstOrDefault(x => x.Exit.Id == id)
			: currentOverlayExits.GetFromItemListByKeyword(input.Last, actor);
		if (exit == null)
		{
			actor.OutputHandler.Send("There is no such exit for you to edit.");
			return;
		}

		if (!exit.OutboundDirection.In(CardinalDirection.Down, CardinalDirection.Up))
		{
			actor.OutputHandler.Send("Only up and down direction exits can be made into fall exits.");
			return;
		}

		if (exit.IsFallExit || exit.IsFlyExit)
		{
			var (newExit, newId) = GetOrCopyCellExit(exit, actor.CurrentOverlayPackage);
			newExit.FallCell = null;
			newExit.Changed = true;
			actor.OutputHandler.Send(
				$"That exit is no longer a fall exit.{(newId != 0 ? $" New ID is {newId:N0}" : "")}");
			return;
		}

		var (editedExit, editedId) = GetOrCopyCellExit(exit, actor.CurrentOverlayPackage);
		editedExit.FallCell = exit.OutboundDirection == CardinalDirection.Up ? exit.Origin : exit.Destination;
		editedExit.Changed = true;
		actor.OutputHandler.Send(
			$"This exit is now a fall exit {(exit.OutboundDirection == CardinalDirection.Up ? "towards" : "away from")} this cell.{(editedId != 0 ? $" New ID {editedId:N0}" : "")}");
	}

	private static void CellExitClimb(ICharacter actor, StringStack input)
	{
		if (input.IsFinished)
		{
			actor.OutputHandler.Send(
				"You must specify which exit you want to turn into (or turn back from) being a climb exit.");
			return;
		}

		var currentOverlayExits = actor.Location.ExitsFor(actor, true);
		var exit = long.TryParse(input.Pop(), out var id)
			? currentOverlayExits.FirstOrDefault(x => x.Exit.Id == id)
			: currentOverlayExits.GetFromItemListByKeyword(input.Last, actor);
		if (exit == null)
		{
			actor.OutputHandler.Send("There is no such exit for you to edit.");
			return;
		}

		if (input.IsFinished)
		{
			if (exit.IsClimbExit)
			{
				var (newExit, newId) = GetOrCopyCellExit(exit, actor.CurrentOverlayPackage);
				newExit.IsClimbExit = false;
				newExit.ClimbDifficulty = Difficulty.Normal;
				newExit.Changed = true;
				actor.OutputHandler.Send(
					$"That exit is no longer a climb exit.{(newId != 0 ? $" New ID is {newId:N0}" : "")}");
				return;
			}

			actor.OutputHandler.Send("What difficulty do you want to make that climb exit?");
			return;
		}

		if (!CheckExtensions.GetDifficulty(input.SafeRemainingArgument, out var difficulty))
		{
			actor.OutputHandler.Send("That is not a valid difficulty.");
			return;
		}

		var (editedExit, editedID) = GetOrCopyCellExit(exit, actor.CurrentOverlayPackage);
		editedExit.IsClimbExit = true;
		editedExit.ClimbDifficulty = difficulty;
		editedExit.Changed = true;
		actor.OutputHandler.Send(
			$"That exit is now a climb exit with a difficulty of {difficulty.Describe().Colour(Telnet.Green)}.{(editedID != 0 ? $" New ID is {editedID:N0}" : "")}");
	}

	private static void CellExitSize(ICharacter actor, StringStack input, bool upright)
	{
		if (input.IsFinished)
		{
			actor.OutputHandler.Send("Which exit do you want to edit the size information for?");
			return;
		}

		var currentOverlayExits = actor.Location.ExitsFor(actor, true);
		var exit = long.TryParse(input.Pop(), out var id)
			? currentOverlayExits.FirstOrDefault(x => x.Exit.Id == id)
			: currentOverlayExits.GetFromItemListByKeyword(input.Last, actor);
		if (exit == null)
		{
			actor.OutputHandler.Send("There is no such exit for you to edit.");
			return;
		}

		if (input.IsFinished)
		{
			actor.Send(
				$"What maximum size{(upright ? " while upright" : "")} do you want this exit to be? Use {"show sizes".Colour(Telnet.Yellow)} to see a list of sizes.");
			return;
		}

		var text = input.PopSpeech();
		var sizes = Enum.GetValues(typeof(SizeCategory)).OfType<SizeCategory>();
		if (!sizes.Any(x => x.Describe().Equals(text, StringComparison.InvariantCultureIgnoreCase)))
		{
			actor.OutputHandler.Send("That is not a valid size for the exit.");
			return;
		}

		var target = sizes.FirstOrDefault(x => x.Describe().Equals(text, StringComparison.InvariantCultureIgnoreCase));

		var (newExit, newId) = GetOrCopyCellExit(exit, actor.CurrentOverlayPackage);
		actor.OutputHandler.Send(
			$"That exit will now only allow creatures up to size {target.Describe().Colour(Telnet.Green)} to fit through{(upright ? " while upright" : "")}{(newId != 0 ? $". New ID is {newId:N0}" : "")}.");

		if (!upright)
		{
			newExit.MaximumSizeToEnter = target;
			if (newExit.MaximumSizeToEnterUpright > newExit.MaximumSizeToEnter)
			{
				newExit.MaximumSizeToEnterUpright = newExit.MaximumSizeToEnter;
			}
		}
		else
		{
			newExit.MaximumSizeToEnterUpright = target;
			if (newExit.MaximumSizeToEnter < newExit.MaximumSizeToEnterUpright)
			{
				newExit.MaximumSizeToEnter = newExit.MaximumSizeToEnterUpright;
			}
		}

		newExit.Changed = true;
	}

	private static void CellEditLink(ICharacter actor, StringStack input)
	{
		if (actor.CurrentOverlayPackage == null)
		{
			actor.OutputHandler.Send("You must first open a Cell Overlay Package to use this command.");
			return;
		}

		if (!actor.CurrentOverlayPackage.Status.In(RevisionStatus.UnderDesign, RevisionStatus.PendingRevision))
		{
			actor.OutputHandler.Send("You current Cell Overlay Package is not Under Design or Pending Revision.");
			return;
		}

		if (input.IsFinished)
		{
			actor.OutputHandler.Send("Which direction would you like to create a two-way exit towards?");
			return;
		}

		if (!Constants.CardinalDirectionStringToDirection.TryGetValue(input.PopSpeech(), out var direction))
		{
			actor.OutputHandler.Send("That is not a valid direction.");
			return;
		}

		if (input.IsFinished)
		{
			actor.OutputHandler.Send("Which room would you like to create a two-way link to?");
			return;
		}

		var cell = LookupCell(actor.Gameworld, input.SafeRemainingArgument);
		if (cell == null)
		{
			actor.OutputHandler.Send("There is no such room for you to link to.");
			return;
		}

		if (cell == actor.Location)
		{
			actor.OutputHandler.Send("You cannot link a room to itself.");
			return;
		}

		var overlay = actor.Location.GetOrCreateOverlay(actor.CurrentOverlayPackage);
		if (
			actor.Gameworld.ExitManager.GetExitsFor(actor.Location, overlay)
				 .Any(x => x.OutboundDirection == direction))
		{
			actor.OutputHandler.Send("This Cell Overlay already contains an exit in that direction.");
			return;
		}

		if (actor.Gameworld.ExitManager.GetExitsFor(actor.Location, overlay).Any(x => x.Destination == cell))
		{
			actor.OutputHandler.Send("This Cell Overlay already contains an exit to that location.");
			return;
		}

		var otherOverlay = cell.GetOrCreateOverlay(actor.CurrentOverlayPackage);
		var oppositeDirection = direction.Opposite();
		if (
			actor.Gameworld.ExitManager.GetExitsFor(cell, otherOverlay)
				 .Any(x => x.OutboundDirection == oppositeDirection))
		{
			actor.OutputHandler.Send("The target cell already has an exit in the opposite direction.");
			return;
		}

		var newExit =
			new Construction.Boundary.Exit(actor.Gameworld, actor.Location, cell, direction, oppositeDirection, 1.0);
		overlay.AddExit(newExit);
		otherOverlay.AddExit(newExit);
		actor.Gameworld.ExitManager.UpdateCellOverlayExits(actor.Location, overlay);
		actor.Gameworld.ExitManager.UpdateCellOverlayExits(cell, otherOverlay);
		actor.OutputHandler.Send("You create a two-way exit to the " + direction.Describe() + " to cell \"" +
								 cell.HowSeen(actor) + "\"");
	}

	private static readonly Regex CellEditNLinkRegex =
		new("((?:\\d+)|(?:\\w+)) (\\S+) (\\w+) (\\w+) \"([^\"]+)\" \"([^\"]+)\"", RegexOptions.IgnoreCase);

	private static readonly Regex CellEditNDigRegex =
		new("(?<template>(?:\\d+)|(?:\\w+)) (?<outboundkey>\\w+) (?<inboundkey>\\w+) \"(?<outboundname>[^\"]+)\" \"(?<inboundname>[^\"]+)\"", RegexOptions.IgnoreCase);

	private static void CellEditNlink(ICharacter actor, StringStack input)
	{
		var match = CellEditNLinkRegex.Match(input.RemainingArgument);
		if (!match.Success)
		{
			actor.OutputHandler.Send("You must supply an argument in this form: " +
									 "cell nlink <template> <cellid> <outbound keyword> <inbound keyword> \"<outbound description>\" \"<inbound description>\""
										 .Colour(Telnet.Yellow));
			return;
		}

		var template = long.TryParse(match.Groups[1].Value, out var value)
			? actor.Gameworld.NonCardinalExitTemplates.Get(value)
			: actor.Gameworld.NonCardinalExitTemplates.FirstOrDefault(
				x => x.Name.StartsWith(match.Groups[1].Value, StringComparison.InvariantCultureIgnoreCase));
		if (template == null)
		{
			actor.OutputHandler.Send("That is not a valid Non Cardinal Exit Template.");
			return;
		}

		var cell = LookupCell(actor.Gameworld, match.Groups[2].Value);
		if (cell == null)
		{
			actor.OutputHandler.Send("There is no such Cell for you to link to.");
			return;
		}

		if (cell == actor.Location)
		{
			actor.OutputHandler.Send("You cannot link a cell to itself.");
			return;
		}

		if (actor.CurrentOverlayPackage == null)
		{
			actor.OutputHandler.Send("You must first open a Cell Overlay Package to use this command.");
			return;
		}

		if (!actor.CurrentOverlayPackage.Status.In(RevisionStatus.UnderDesign, RevisionStatus.PendingRevision))
		{
			actor.OutputHandler.Send("You current Cell Overlay Package is not Under Design or Pending Revision.");
			return;
		}

		var overlay = actor.Location.GetOrCreateOverlay(actor.CurrentOverlayPackage);
		if (actor.Gameworld.ExitManager.GetExitsFor(actor.Location, overlay).Any(x => x.Destination == cell))
		{
			actor.OutputHandler.Send("This Cell Overlay already contains an exit to that location.");
			return;
		}

		var otherOverlay = cell.GetOrCreateOverlay(actor.CurrentOverlayPackage);
		var newExit = new Construction.Boundary.Exit(actor.Gameworld, actor.Location, cell, 1.0, template,
			match.Groups[3].Value,
			match.Groups[4].Value, match.Groups[5].Value, match.Groups[6].Value);
		overlay.AddExit(newExit);
		otherOverlay.AddExit(newExit);
		actor.Gameworld.ExitManager.UpdateCellOverlayExits(actor.Location, overlay);
		actor.Gameworld.ExitManager.UpdateCellOverlayExits(cell, otherOverlay);
		actor.OutputHandler.Send(
			$"You create a two-way exit to cell \"{cell.HowSeen(actor)}\" from the template {template.Name.Proper().Colour(Telnet.Cyan)}");
	}

	private static void CellEditName(ICharacter actor, StringStack input)
	{
		if (input.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to set for this room?");
			return;
		}

		var overlay = actor.Location.GetOrCreateOverlay(actor.CurrentOverlayPackage);
		overlay.CellName = input.SafeRemainingArgument.TitleCase();
		actor.OutputHandler.Send($"You set the room name to {overlay.CellName.ColourName()}");
	}

	private static void DoCellDescPost(string description, IOutputHandler handler, object[] arguments)
	{
		var actor = (ICharacter)arguments[0];
		var overlay = actor.Location.GetOrCreateOverlay(actor.CurrentOverlayPackage);
		overlay.CellDescription = description;
		handler.Send("You set the Cell Description to:\n\n" +
					 overlay.CellDescription.Wrap(actor.InnerLineFormatLength));
	}

	private static void DoCellDescCancel(IOutputHandler handler, object[] arguments)
	{
		handler.Send("You decide not to change the description.");
	}

	private static void CellEditDescription(ICharacter actor, StringStack input)
	{
		var sb = new StringBuilder();
		sb.AppendLine("Replacing:\n\n");
		sb.AppendLine(actor.Location.GetOverlayFor(actor).CellDescription.Wrap(actor.InnerLineFormatLength, "\t"));
		sb.AppendLine();
		sb.AppendLine("Enter the description in the editor below.");
		sb.AppendLine();
		sb.AppendLine("You can use the following markup options in this description:");
		sb.AppendLine();
		sb.AppendLine(
			$"{"Check Skills/Attributes".ColourName()}: {"check{trait,minvalue}{text if the trait is >= value}{text if not}".ColourCommand()}");
		sb.AppendLine(
			$"{"Written Text".ColourName()}: {"writing{language,script,style=...,colour=...,minskill}{text if you understand}{text if you cant}".ColourCommand()}");
		if (actor.Location.Shop is not null)
		{
			sb.AppendLine($"{"Shop Name".ColourName()}: {"@shop".ColourCommand()}");
		}

		sb.AppendLine(
			$"{"Weather/Light/Time".ColourName()}: {"environment{conditions=text}{optional more conds up to 8 times=text}{fallback}".ColourCommand()}");
		sb.AppendLine();
		sb.AppendLine("Conditions for 'environment' include:");
		sb.AppendLine();
		sb.AppendLine(
			$"{"Times".ColourName()}: {"day, night, morning, afternoon, dusk, dawn, notnight".ColourCommand()}");
		sb.AppendLine(
			$"{"Light".ColourName()}: {$"{actor.Gameworld.LightModel.LightDescriptions.Select(x => x.ToLowerInvariant()).ListToCommaSeparatedValues(", ")}".ColourCommand()}");
		sb.AppendLine(
			$"{"Light".ColourName()}: {"Add a > or < to light levels to check higher than (inclusive) or lower than (exclusive)".ColourCommand()}");
		if (actor.Location.WeatherController is not null)
		{
			sb.AppendLine(
				$"{"Seasons".ColourName()}: {$"{actor.Location.WeatherController.RegionalClimate.Seasons.Select(x => x.Name.ToLowerInvariant()).ListToCommaSeparatedValues(", ")}".ColourCommand()}");
			sb.AppendLine(
				$"{"Precipitation".ColourName()}: {"parched, dry, humid, lightrain, rain, heavyrain, torrentialrain, lightsnow, snow, heavysnow, blizzard, sleet".ColourCommand()}");
			sb.AppendLine(
				$"{"Precipitation".ColourName()}: {"Add a * to the front of the above to check highest recent precipitation (e.g. *rain to see if it's rained recently)".ColourCommand()}");
			sb.AppendLine(
				$"{"Precipitation".ColourName()}: {"Add a > or < to precipitation to check higher than (inclusive) or lower than (exclusive) e.g. >lightrain, <heavyrain".ColourCommand()}");
		}

		sb.AppendLine();
		sb.AppendLine($"Note: reverse any condition with a ! (e.g. !dawn, !snow, !*rain, !summer)".ColourError());
		actor.OutputHandler.Send(sb.ToString());
		actor.EditorMode(DoCellDescPost, DoCellDescCancel, 1.0, null, EditorOptions.None, new object[] { actor });
	}

	#endregion

	#region CellPackage

	private static void CellPackage(ICharacter actor, StringStack input)
	{
		if (input.IsFinished)
		{
			if (actor.CurrentOverlayPackage == null)
			{
				actor.OutputHandler.Send("You have not adopted a cell overlay package.");
				return;
			}

			actor.OutputHandler.Send(actor.CurrentOverlayPackage.Show(actor));
			return;
		}

		switch (input.PopSpeech().CollapseString().ToLowerInvariant())
		{
			case "show":
				CellPackageShow(actor, input);
				break;
			case "swap":
				CellPackageSwap(actor, input);
				break;
			case "new":
			case "create":
				CellPackageNew(actor, input);
				break;
			case "rename":
				CellPackageRename(actor, input);
				break;
			case "open":
				CellPackageOpen(actor, input);
				break;
			case "close":
				CellPackageClose(actor, input);
				break;
			case "revise":
				CellPackageRevise(actor, input);
				break;
			case "submit":
				CellPackageSubmit(actor, input);
				break;
			case "review":
				CellPackageReview(actor, input);
				break;
			case "delete":
				CellPackageDelete(actor, input);
				break;
			case "obsolete":
				CellPackageObsolete(actor, input);
				break;
			case "list":
				CellPackageList(actor, input);
				break;
			default:
				actor.OutputHandler.Send(@"These are the commands used to work with overlay packages:

#3cell package list [all|by <who> | mine]#0 - lists all cell packages (optionally filtered)
#3cell package new ""name of your package""#0 - creates a new package with the specified name
#3cell package open <id>|""name of your package""#0 - opens an existing unapproved package for further editing
#3cell package rename <name>#0 - renames your open cell package to something else
#3cell package revise <id>|""name of your package""#0 - creates a new revision of an existing package
#3cell package close#0 - closes the package you are currently editing
#3cell package show <id>|""name""#0 - views an existing package
#3cell package submit#0 - submits the package for review by an appropriate reviewer
#3cell package review list#0 - shows all packages ready for review
#3cell package review all#0 - reviews all submitted packages at once
#3cell package review <id>#0 - reviews a specific package
#3cell package history <id>#0 - shows the building/review history of a particular cell package"
					.SubstituteANSIColour());
				return;
		}
	}

	private static void CellPackageRename(ICharacter actor, StringStack input)
	{
		if (actor.CurrentOverlayPackage == null)
		{
			actor.OutputHandler.Send(
				"You do not have a Cell Overlay Package open. You must first open a cell package.");
			return;
		}

		if (input.IsFinished)
		{
			actor.OutputHandler.Send("What new name do you want to give to your open cell package?");
			return;
		}

		var name = input.SafeRemainingArgument.TitleCase();
		if (actor.Gameworld.CellOverlayPackages.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				"There is already a cell overlay package with that name. Names must be unique.");
			return;
		}

		var oldname = actor.CurrentOverlayPackage.Name;
		var packages = actor.Gameworld.CellOverlayPackages
							.Where(x => x.Id == actor.CurrentOverlayPackage.Id)
							.ToList();
		foreach (var package in packages)
		{
			package.SetName(name);
		}

		actor.OutputHandler.Send($"You rename the cell overlay package {oldname.ColourName()} to {name.ColourName()}.");
	}

	private static void CellPackageList(ICharacter actor, StringStack input)
	{
		var packages = actor.Gameworld.CellOverlayPackages.GetAllApprovedOrMostRecent(false).AsEnumerable();

		while (!input.IsFinished)
		{
			var cmd = input.Pop().ToLowerInvariant();
			switch (cmd)
			{
				case "all":
					packages = actor.Gameworld.CellOverlayPackages.AsEnumerable();
					break;
				case "by":
					cmd = input.Pop().ToLowerInvariant();
					if (cmd.Length == 0)
					{
						actor.OutputHandler.Send("List Cell Overlay Packages for Review by whom?");
						return;
					}

					using (new FMDB())
					{
						var dbaccount = FMDB.Context.Accounts.FirstOrDefault(x => x.Name == cmd);
						if (dbaccount == null)
						{
							actor.OutputHandler.Send("There is no such account.");
							return;
						}

						packages = packages.Where(x => x.BuilderAccountID == dbaccount.Id);
						break;
					}
				case "mine":
					packages = packages.Where(x => x.BuilderAccountID == actor.Account.Id);
					break;
				default:
					actor.OutputHandler.Send(
						"That is not a valid option for Listing Cell Overlay Packages for Review.");
					return;
			}
		}

		// Order the list
		packages = packages.OrderBy(x => x.Id).ThenBy(x => x.RevisionNumber);

		// Display Output for List
		using (new FMDB())
		{
			actor.OutputHandler.Send(
				StringUtilities.GetTextTable(
					from package in packages
					select
						new[]
						{
							package.Id.ToString(),
							package.RevisionNumber.ToString(),
							package.Name.Proper(),
							actor.Gameworld.Cells.Count(x => x.Overlays.Any(y => y.Package == package)).ToString(),
							FMDB.Context.Accounts.Find(package.BuilderAccountID).Name,
							package.BuilderComment,
							package.Status.Describe()
						},
					new[] { "ID#", "Rev#", "Name", "#Cells", "Builder", "Comment", "Status" },
					actor.Account.LineFormatLength,
					colour: Telnet.Green,
					truncatableColumnIndex: 5,
					unicodeTable: actor.Account.UseUnicode
				)
			);
		}
	}

	private static void CellPackageOpen(ICharacter actor, StringStack input)
	{
		if (actor.CurrentOverlayPackage != null)
		{
			actor.OutputHandler.Send("You must first close your current Cell Overlay Package.");
			return;
		}

		var package = actor.Gameworld.CellOverlayPackages.BestRevisableMatch(actor, input);
		if (package == null)
		{
			actor.OutputHandler.Send("There is no such Cell Overlay Package to open.");
			return;
		}

		actor.CurrentOverlayPackage = package;
		actor.Send("You open {0}.", package.EditHeader().Colour(Telnet.Green));
	}

	private static void CellPackageClose(ICharacter actor, StringStack input)
	{
		if (actor.CurrentOverlayPackage == null)
		{
			actor.OutputHandler.Send("You do not have a Cell Overlay Package to close.");
			return;
		}

		actor.Send("You close {0}.", actor.CurrentOverlayPackage.EditHeader().Colour(Telnet.Green));
		actor.CurrentOverlayPackage = null;
	}

	private static void CellPackageSwap(ICharacter actor, StringStack input)
	{
		var package = long.TryParse(input.PopSpeech(), out var value)
			? actor.Gameworld.CellOverlayPackages.Get(value)
			: actor.Gameworld.CellOverlayPackages.GetByName(input.Last, true);

		if (package == null)
		{
			actor.OutputHandler.Send("There is no such Cell Overlay Package to swap in.");
			return;
		}

		if (package.Status != RevisionStatus.Current)
		{
			actor.OutputHandler.Send("You can only swap in approved Cell Overlay Packages.");
			return;
		}

		var cells = actor.Gameworld.Cells.Where(x => x.Overlays.Any(y => y.Package == package)).ToList();
		foreach (var cell in cells)
		{
			cell.SetCurrentOverlay(package);
			actor.Gameworld.ExitManager.UpdateCellOverlayExits(cell, cell.CurrentOverlay);
		}

		actor.OutputHandler.Send(
			$"Swapped Cell Overlay Package \"{package.Name.TitleCase()}\" into {cells.Count.ToString().Colour(Telnet.Green)} cells.");
	}

	private static void CellPackageShow(ICharacter actor, StringStack input)
	{
		ICellOverlayPackage package = null;
		package = input.IsFinished
			? actor.CurrentOverlayPackage
			: actor.Gameworld.CellOverlayPackages.BestRevisableMatch(actor, input);

		if (package == null)
		{
			actor.OutputHandler.Send("There is no such Cell Overlay Package to show you.");
			return;
		}

		actor.OutputHandler.Send(package.Show(actor));
	}

	private static void CellPackageNew(ICharacter actor, StringStack input)
	{
		if (input.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to your new Cell Overlay Package?");
			return;
		}

		var name = input.SafeRemainingArgument.TitleCase();
		var lowerName = name.ToLowerInvariant();
		if (actor.Gameworld.CellOverlayPackages.Any(x => x.Name.ToLowerInvariant() == lowerName))
		{
			actor.OutputHandler.Send(
				"There is already a Cell Overlay Package with that name. The name must be unique.");
			return;
		}

		ICellOverlayPackage package = new Construction.CellOverlayPackage(actor.Gameworld, actor.Account, name);
		actor.Gameworld.Add(package);
		actor.CurrentOverlayPackage = package;
		actor.OutputHandler.Send(
			$"You create Cell Overlay Package #{package.Id.ToString("N0", actor)}, called {package.Name.ColourName()}, which is now your open overlay package.");
	}

	private static void CellPackageRevise(ICharacter actor, StringStack input)
	{
		if (input.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a Cell Overlay Package to revise.");
			return;
		}

		var cmd = input.PopSpeech().ToLowerInvariant();

		ICellOverlayPackage package = null;
		package = long.TryParse(cmd, out var value)
			? actor.Gameworld.CellOverlayPackages.Get(value)
			: actor.Gameworld.CellOverlayPackages.GetByName(cmd);

		if (package == null)
		{
			actor.OutputHandler.Send("There is no such package for you to revise.");
			return;
		}

		if (package.Status != RevisionStatus.Current && package.Status != RevisionStatus.Rejected)
		{
			actor.OutputHandler.Send(
				"You may only revise cell overlay packages that have a status of Current or Rejected.");
			return;
		}

		if (actor.Gameworld.CellOverlayPackages.Where(x => x.Id == package.Id).Any(x =>
				x.RevisionNumber > package.RevisionNumber &&
				x.Status.In(RevisionStatus.UnderDesign, RevisionStatus.PendingRevision)))
		{
			actor.OutputHandler.Send(
				"There is already another cell overlay package under design or pending review for that ID. You should either OPEN that under design one for editing or REVIEW it to make it the current package, or DELETE it if it's no longer required.");
			return;
		}

		var newPackage = (ICellOverlayPackage)package.CreateNewRevision(actor);
		actor.CurrentOverlayPackage = newPackage;
		actor.Send(
			"You create a new revision ({2}) of Cell Overlay Package #{0} \"{1}\", which is now your open overlay package.",
			newPackage.Id.ToString("N0", actor),
			newPackage.Name.ColourName(), newPackage.RevisionNumber.ToString("N0", actor).ColourValue());
	}

	private static void CellPackageSubmit(ICharacter actor, StringStack input)
	{
		if (actor.CurrentOverlayPackage == null)
		{
			actor.OutputHandler.Send("You do not have a Cell Overlay Package to submit for review.");
			return;
		}

		if (actor.CurrentOverlayPackage.Status != RevisionStatus.UnderDesign)
		{
			actor.OutputHandler.Send("Your Cell Overlay Package is not in the " +
									 "Under Design".Colour(Telnet.Yellow) + " status.");
			return;
		}

		actor.CurrentOverlayPackage.ChangeStatus(RevisionStatus.PendingRevision, input.SafeRemainingArgument,
			actor.Account);
		actor.OutputHandler.Send("You submit the Cell Overlay Package \"" + actor.CurrentOverlayPackage.Name +
								 "\" for review.");
		actor.CurrentOverlayPackage = null;
	}

	private static void CellPackageReview(ICharacter actor, StringStack input)
	{
		var cmd = input.PopSpeech().ToLowerInvariant();
		if (cmd.Length == 0)
		{
			CellPackageReviewList(actor, input);
			return;
		}

		switch (cmd)
		{
			case "list":
				CellPackageReviewList(actor, input);
				break;
			case "all":
				CellPackageReviewAll(actor, input);
				break;
			case "history":
				CellPackageReviewHistory(actor, input);
				break;
			default:
				CellPackageReviewDefault(actor, input);
				break;
		}
	}

	private static void CellPackageReviewHistory(ICharacter actor, StringStack input)
	{
		var protos = actor.Gameworld.CellOverlayPackages.GetAllByIdOrName(input.SafeRemainingArgument);
		if (!protos.Any())
		{
			actor.OutputHandler.Send(
				$"There is no cell overlay package that is identified by {input.SafeRemainingArgument.ColourCommand()}.");
			return;
		}

		// Display Output for List
		using (new FMDB())
		{
			actor.OutputHandler.Send(
				StringUtilities.GetTextTable(
					from proto in protos.OrderBy(x => x.RevisionNumber)
					select new[]
					{
						proto.Id.ToString(), proto.RevisionNumber.ToString(),
						FMDB.Context.Accounts.Find(proto.BuilderAccountID).Name, proto.BuilderComment,
						proto.BuilderDate.GetLocalDateString(actor),
						FMDB.Context.Accounts.Any(x => x.Id == proto.ReviewerAccountID)
							? FMDB.Context.Accounts.Find(proto.ReviewerAccountID).Name
							: "",
						proto.ReviewerComment,
						proto.ReviewerDate.HasValue
							? proto.ReviewerDate.Value.GetLocalDateString(actor)
							: "",
						proto.Status.Describe()
					},
					new[]
					{
						"ID#", "Rev#", "Builder", "Comment", "Build Date", "Reviewer", "Comment",
						"Review Date",
						"Status"
					}, actor.Account.LineFormatLength, colour: Telnet.Green,
					unicodeTable: actor.Account.UseUnicode
				)
			);
		}
	}

	private static void CellPackageReviewList(ICharacter actor, StringStack input)
	{
		var packages = actor.Gameworld.CellOverlayPackages.Where(x => x.Status == RevisionStatus.PendingRevision);

		while (!input.IsFinished)
		{
			var cmd = input.Pop().ToLowerInvariant();
			switch (cmd)
			{
				case "by":
					cmd = input.Pop().ToLowerInvariant();
					if (cmd.Length == 0)
					{
						actor.OutputHandler.Send("List Cell Overlay Packages for Review by whom?");
						return;
					}

					using (new FMDB())
					{
						var dbaccount = FMDB.Context.Accounts.FirstOrDefault(x => x.Name == cmd);
						if (dbaccount == null)
						{
							actor.OutputHandler.Send("There is no such account.");
							return;
						}

						packages = packages.Where(x => x.BuilderAccountID == dbaccount.Id);
						break;
					}
				case "mine":
					packages = packages.Where(x => x.BuilderAccountID == actor.Account.Id);
					break;
				default:
					actor.OutputHandler.Send(
						"That is not a valid option for Listing Cell Overlay Packages for Review.");
					return;
			}
		}

		packages = packages.OrderBy(x => x.Id);

		// Display Output for List
		using (new FMDB())
		{
			actor.OutputHandler.Send(
				StringUtilities.GetTextTable(
					from package in packages
					select
						new[]
						{
							package.Id.ToString(), package.RevisionNumber.ToString(), package.Name.Proper(),
							actor.Gameworld.Cells.Count(x => x.Overlays.Any(y => y.Package == package)).ToString(),
							FMDB.Context.Accounts.Find(package.BuilderAccountID).Name, package.BuilderComment
						},
					new[] { "ID#", "Rev#", "Name", "#Cells", "Builder", "Comment" }, actor.Account.LineFormatLength,
					colour: Telnet.Green,
					unicodeTable: actor.Account.UseUnicode,
					truncatableColumnIndex: 5
				)
			);
		}
	}

	private static void CellPackageReviewAll(ICharacter actor, StringStack input)
	{
		var packages = actor.Gameworld.CellOverlayPackages.Where(x => x.Status == RevisionStatus.PendingRevision);

		while (!input.IsFinished)
		{
			var cmd = input.Pop().ToLowerInvariant();
			switch (cmd)
			{
				case "by":
					cmd = input.Pop().ToLowerInvariant();
					if (cmd.Length == 0)
					{
						actor.OutputHandler.Send("List Cell Overlay Packages for Review by whom?");
						return;
					}

					using (new FMDB())
					{
						var dbaccount = FMDB.Context.Accounts.FirstOrDefault(x => x.Name == cmd);
						if (dbaccount == null)
						{
							actor.OutputHandler.Send("There is no such account.");
							return;
						}

						packages = packages.Where(x => x.BuilderAccountID == dbaccount.Id);
						break;
					}
				case "mine":
					packages = packages.Where(x => x.BuilderAccountID == actor.Account.Id);
					break;
				default:
					actor.OutputHandler.Send(
						"That is not a valid option for Listing Cell Overlay Packages for Review.");
					return;
			}
		}

		if (!packages.Any())
		{
			actor.OutputHandler.Send("There are no Cell Overlay Packages to review.");
			return;
		}

		actor.OutputHandler.Send(
			("You are reviewing " + packages.Count() + " Cell Overlay Package" + (packages.Count() == 1 ? "" : "s"))
			.Colour(Telnet.Red) + "\n\nTo approve these Cell Overlay Package" +
			(packages.Count() == 1 ? "" : "s") + ",  type " + "accept edit <your comments>".Colour(Telnet.Yellow) +
			" or " + "decline edit <your comments>".Colour(Telnet.Yellow) +
			" to reject.\nIf you do not wish to approve or decline, your request will time out in 120 seconds.");
		actor.AddEffect(
			new Accept(actor, new EditableItemReviewProposal<ICellOverlayPackage>(actor, packages.ToList())),
			TimeSpan.FromSeconds(120));
	}

	private static void CellPackageReviewDefault(ICharacter actor, StringStack input)
	{
		if (!long.TryParse(input.Last, out var value))
		{
			actor.OutputHandler.Send("Which Cell Overlay Package do you wish to review?");
			return;
		}

		ICellOverlayPackage package = null;

		var cmd = input.Pop();
		if (cmd.Length == 0)
		{
			package =
				actor.Gameworld.CellOverlayPackages.FirstOrDefault(
					x => x.Id == value && x.Status == RevisionStatus.PendingRevision);
		}
		else
		{
			if (!int.TryParse(cmd, out var revnum))
			{
				actor.OutputHandler.Send("Which specific revision do you wish to review?");
				return;
			}

			package = actor.Gameworld.CellOverlayPackages.Get(value, revnum);
			if (package != null && package.Status != RevisionStatus.PendingRevision)
			{
				package = null;
			}
		}

		if (package == null)
		{
			actor.OutputHandler.Send("There is no such Cell Overlay Package for you to review.");
			return;
		}

		actor.OutputHandler.Send(("You are reviewing " + package.EditHeader()).Colour(Telnet.Red) + "\n\n" +
								 package.Show(actor) + "\n\nTo approve this Cell Overlay Package, type " +
								 "accept edit <your comments>".Colour(Telnet.Yellow) + " or " +
								 "decline edit <your comments>".Colour(Telnet.Yellow) +
								 " to reject.\nIf you do not wish to approve or decline, your request will time out in 120 seconds.");
		actor.AddEffect(
			new Accept(actor,
				new EditableItemReviewProposal<ICellOverlayPackage>(actor, new List<ICellOverlayPackage> { package })),
			TimeSpan.FromSeconds(120));
	}

	private static void CellPackageDelete(ICharacter actor, StringStack input)
	{
		if (actor.CurrentOverlayPackage == null)
		{
			actor.OutputHandler.Send("You are not currently editing any Cell Overlay Packages.");
			return;
		}

		if (actor.CurrentOverlayPackage.Status != RevisionStatus.UnderDesign)
		{
			actor.OutputHandler.Send("That Cell Overlay Package is not currently under design.");
			return;
		}

		using (new FMDB())
		{
			actor.Gameworld.SaveManager.Flush();
			// We must first delete all of the CellOverlays linked to this CellOverlayPackage, because the Overlays have a foreign key constraint of the Package ID.
			var dboverlayproto = FMDB.Context.CellOverlays
									 .Where(x => x.CellOverlayPackageId == actor.CurrentOverlayPackage.Id)
									 .Where(x => x.CellOverlayPackageRevisionNumber ==
												 actor.CurrentOverlayPackage.RevisionNumber)
									 .ToList();

			foreach (var overlay in dboverlayproto)
			{
				var cell = actor.Gameworld.Cells.Get(overlay.CellId);
				FMDB.Context.CellOverlays.Remove(overlay);
				FMDB.Context.CellOverlaysExits.RemoveRange(overlay.CellOverlaysExits.AsEnumerable());
				cell.RemoveOverlay(overlay.Id);
			}

			var dbpackageproto = FMDB.Context.CellOverlayPackages
									 .Find(actor.CurrentOverlayPackage.Id, actor.CurrentOverlayPackage.RevisionNumber);
			if (dbpackageproto != null)
			{
				FMDB.Context.CellOverlayPackages.Remove(dbpackageproto);
				FMDB.Context.SaveChanges();
			}
		}

		actor.OutputHandler.Send("You delete " + actor.CurrentOverlayPackage.EditHeader() + ".");
		actor.Gameworld.Destroy(actor.CurrentOverlayPackage);
		actor.CurrentOverlayPackage = null;
	}

	private static void CellPackageObsolete(ICharacter actor, StringStack input)
	{
		if (actor.CurrentOverlayPackage == null)
		{
			actor.OutputHandler.Send("You are not currently editing any Cell Overlay Packages.");
			return;
		}

		if (actor.CurrentOverlayPackage.Status != RevisionStatus.Current)
		{
			actor.OutputHandler.Send("You are not editing the most current revision of this Cell Overlay Package.");
			return;
		}

		actor.CurrentOverlayPackage.ChangeStatus(RevisionStatus.Obsolete, input.SafeRemainingArgument, actor.Account);
		actor.OutputHandler.Send("You mark " + actor.CurrentOverlayPackage.EditHeader() +
								 " as an obsolete prototype.");
		actor.CurrentOverlayPackage = null;
	}

	#endregion

	#region Shard Sub-Commands

	private static void ShardCreate(ICharacter actor, StringStack input)
	{
		if (input.IsFinished)
		{
			actor.Send("What name do you want to give to this new shard?");
			return;
		}

		var nameText = input.PopSpeech();
		if (input.IsFinished)
		{
			actor.Send("Which Sky Description Template do you want to use for this new shard?");
			return;
		}

		var template = long.TryParse(input.PopSpeech(), out var value)
			? actor.Gameworld.SkyDescriptionTemplates.Get(value)
			: actor.Gameworld.SkyDescriptionTemplates.FirstOrDefault(
				x => x.Name.Equals(input.Last, StringComparison.InvariantCultureIgnoreCase));
		if (template == null)
		{
			actor.Send("There is no such Sky Description Template.");
			return;
		}

		var newShard = new Construction.Shard(actor.Gameworld, template, nameText);
		actor.Gameworld.Add(newShard);
		actor.Send("You create shard #{0:N0}, called {1}.", newShard.Id, newShard.Name.Colour(Telnet.Green));
	}

	#region ShardEdit Sub-Commands

	private static void ShardEditName(ICharacter actor, StringStack input, IShard shard)
	{
		if (input.IsFinished)
		{
			actor.Send("What name do you want to give to this shard?");
			return;
		}

		shard.GetEditableShard.SetName(input.SafeRemainingArgument.TitleCase());
		shard.Changed = true;
		actor.Send("You change the name of shard #{0:N0} to {1}.", shard.Id, shard.Name.Colour(Telnet.Green));
	}

	private static void ShardEditClocks(ICharacter actor, StringStack input, IShard shard)
	{
		if (input.IsFinished)
		{
			actor.Send("Which clocks should this shard have? (enter the IDs of the clocks separated by spaces)");
			return;
		}

		var clocks = new List<IClock>();
		while (!input.IsFinished)
		{
			var clock = long.TryParse(input.PopSpeech(), out var value)
				? actor.Gameworld.Clocks.Get(value)
				: actor.Gameworld.Clocks.FirstOrDefault(
					  x => x.Name.Equals(input.Last, StringComparison.InvariantCultureIgnoreCase)) ??
				  actor.Gameworld.Clocks.FirstOrDefault(
					  x => x.Alias.Equals(input.Last, StringComparison.InvariantCultureIgnoreCase));
			if (clock == null)
			{
				actor.Send("There is no clock identified by {0}.", input.Last);
				return;
			}

			if (!clocks.Contains(clock))
			{
				clocks.Add(clock);
			}
		}

		shard.GetEditableShard.Clocks.Clear();
		shard.GetEditableShard.Clocks.AddRange(clocks);
		shard.Changed = true;
		actor.Send(
			"The shard now has the following clocks:\n{0}Note: Don't forget to set up timezones for the new clocks in any zones in this shard.",
			clocks.Select(x => "\t" + x.Name.TitleCase().ColourValue())
				  .ListToString(separator: "\n", twoItemJoiner: "\n", conjunction: ""));
	}

	private static void ShardEditCalendars(ICharacter actor, StringStack input, IShard shard)
	{
		if (input.IsFinished)
		{
			actor.Send(
				"Which calendars should this shard have? (enter the IDs of the calendars separated by spaces)");
			return;
		}

		var calendars = new List<ICalendar>();
		while (!input.IsFinished)
		{
			var calendar = long.TryParse(input.PopSpeech(), out var value)
				? actor.Gameworld.Calendars.Get(value)
				: actor.Gameworld.Calendars.FirstOrDefault(
					  x => x.ShortName.Equals(input.Last, StringComparison.InvariantCultureIgnoreCase)) ??
				  actor.Gameworld.Calendars.FirstOrDefault(
					  x => x.Alias.Equals(input.Last, StringComparison.InvariantCultureIgnoreCase));
			if (calendar == null)
			{
				actor.Send("There is no calendar identified by {0}.", input.Last);
				return;
			}

			if (!calendars.Contains(calendar))
			{
				calendars.Add(calendar);
			}
		}

		shard.GetEditableShard.Calendars.Clear();
		shard.GetEditableShard.Calendars.AddRange(calendars);
		shard.Changed = true;
		actor.Send(
			"The shard now has the following calendars:\n{0}",
			calendars.Select(x => "\t" + x.ShortName.TitleCase().ColourValue())
					 .ListToString(separator: "\n", twoItemJoiner: "\n", conjunction: ""));
	}

	private static void ShardEditCelestials(ICharacter actor, StringStack input, IShard shard)
	{
		if (input.IsFinished)
		{
			actor.Send(
				"Which celestials should this shard have? (enter the IDs of the celestials separated by spaces)");
			return;
		}

		var celestials = new List<ICelestialObject>();
		while (!input.IsFinished)
		{
			var celestial = long.TryParse(input.PopSpeech(), out var value)
				? actor.Gameworld.CelestialObjects.Get(value)
				: actor.Gameworld.CelestialObjects.FirstOrDefault(
					x => x.Name.Equals(input.Last, StringComparison.InvariantCultureIgnoreCase));
			if (celestial == null)
			{
				actor.Send("There is no celestial identified by {0}.", input.Last);
				return;
			}

			if (!celestials.Contains(celestial))
			{
				celestials.Add(celestial);
			}
		}

		foreach (var zone in shard.Zones)
		{
			zone.DeregisterCelestials();
		}

		shard.GetEditableShard.Celestials.Clear();
		shard.GetEditableShard.Celestials.AddRange(celestials);
		foreach (var zone in shard.Zones)
		{
			zone.InitialiseCelestials();
		}

		shard.Changed = true;
		actor.Send("The shard now has the following celestials:\n{0}",
			celestials.Select(x => "\t" + x.Name.TitleCase())
					  .ListToString(separator: "\n", twoItemJoiner: "\n", conjunction: ""));
	}

	private static void ShardEditLux(ICharacter actor, StringStack input, IShard shard)
	{
		if (input.IsFinished)
		{
			actor.Send("What lux level should this shard provide by default at all times?");
			return;
		}

		if (!double.TryParse(input.Pop(), out var value))
		{
			actor.Send("You must enter a valid number of lux.");
			return;
		}

		shard.GetEditableShard.MinimumTerrestrialLux = value;
		shard.Changed = true;
		actor.Send("This shard now has the minimum terrestrial lux of {0}.", shard.MinimumTerrestrialLux);
	}

	private static void ShardEditSky(ICharacter actor, StringStack input, IShard shard)
	{
		if (input.IsFinished)
		{
			actor.Send("Which Sky Description Template do you want to use for this shard?");
			return;
		}

		var template = long.TryParse(input.PopSpeech(), out var value)
			? actor.Gameworld.SkyDescriptionTemplates.Get(value)
			: actor.Gameworld.SkyDescriptionTemplates.FirstOrDefault(
				x => x.Name.Equals(input.Last, StringComparison.InvariantCultureIgnoreCase));
		if (template == null)
		{
			actor.Send("There is no such Sky Description Template for you to use.");
			return;
		}

		shard.GetEditableShard.SkyDescriptionTemplate = template;
		shard.Changed = true;
		actor.Send("This shard now uses the {0} Sky Description Template.", template.Name.Colour(Telnet.Green));
	}

	#endregion

	private static void ShardEdit(ICharacter actor, StringStack input)
	{
		if (input.IsFinished)
		{
			actor.Send("Which shard do you want to edit?");
			return;
		}

		var shard = long.TryParse(input.PopSpeech(), out var value)
			? actor.Gameworld.Shards.Get(value)
			: actor.Gameworld.Shards.FirstOrDefault(
				x => x.Name.Equals(input.Last, StringComparison.InvariantCultureIgnoreCase));
		if (shard == null)
		{
			actor.Send("There is no such shard.");
			return;
		}

		if (input.IsFinished)
		{
			actor.Send("What about that shard do you want to edit?");
			return;
		}

		switch (input.Pop().ToLowerInvariant())
		{
			case "name":
				ShardEditName(actor, input, shard);
				break;
			case "clocks":
				ShardEditClocks(actor, input, shard);
				break;
			case "calendars":
				ShardEditCalendars(actor, input, shard);
				break;
			case "celestials":
				ShardEditCelestials(actor, input, shard);
				break;
			case "lux":
				ShardEditLux(actor, input, shard);
				break;
			case "sky":
				ShardEditSky(actor, input, shard);
				break;
			default:
				actor.Send("That is not a valid option for editing shards.");
				return;
		}
	}

	#endregion

	#region Areas
	private static void AreaList(ICharacter actor, string target)
	{
		IEnumerable<IArea> areas;
		if (string.IsNullOrWhiteSpace(target))
		{
			areas = actor.Gameworld.Areas;
			actor.OutputHandler.Send("Showing all Areas:");
		}
		else
		{
			var zone = actor.Gameworld.Zones.FirstOrDefault(x =>
				x.Name.EqualTo(target) || x.Name.StartsWith(target, StringComparison.OrdinalIgnoreCase));
			if (zone == null)
			{
				actor.OutputHandler.Send("There is no such zone to filter areas by.");
				return;
			}

			areas = actor.Gameworld.Areas.Where(x => x.Zones.Contains(zone)).ToList();
			actor.OutputHandler.Send(
				$"Showing Areas with Rooms in Zone #{zone.Id.ToString("N0", actor)} ({zone.Name.Colour(Telnet.Cyan)}):");
		}

		actor.OutputHandler.Send(
			StringUtilities.GetTextTable(from area in areas
										 select new[]
										 {
											 area.Id.ToString("N0", actor),
											 area.Name,
											 area.Weather?.RegionalClimate.Name.Colour(Telnet.Cyan) ?? "Default",
											 area.Zones.Select(x => x.Name)
												 .ListToString(conjunction: "", twoItemJoiner: ", "),
											 area.Cells.Count().ToString("N0", actor)
										 },
				new[] { "Id", "Name", "Weather", "Zones", "Cells" },
				actor.LineFormatLength,
				colour: Telnet.Green,
				unicodeTable: actor.Account.UseUnicode)
		);
	}

	public const string AreaHelpText = @"This command is used to create, view and edit #6Areas#0, which are a way to group together rooms with similar proximity or purpose.

Areas are independent of the location hierarchy; so for example, they are not a stage in-between rooms and zones but rather separate; areas can split across multiple zones. Rooms can even belong to more than one area at a time.

Areas are useful for organising echoes and logic in progs (law enforcement for example).

The syntax for working with areas is as follows:

	#3area list [<zone>]#0 - lists all areas (optionally containing rooms in a zone)
	#3area show <id|name>#0 - shows details about an area
	#3area edit <id|name>#0 - begins editing an area
	#3area edit#0 - an alias for #3area edit <id>#0 for the area you're editing
	#3area close#0 - stops editing an area
	#3area new <name>#0 - creates a new area and begins editing it
	#3area add#0 - adds the room you're in to your currently editing area
	#3area remove#0 - removes the room you're in from your currently editing area
	#3area weather <id|name>#0 - sets a weather controller for the area that overrides zone
	#3area weather clear#0 - clears a weather controller override";

	[PlayerCommand("Area", "area")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	[HelpInfo("area", AreaHelpText, AutoHelp.HelpArgOrNoArg)]
	protected static void Area(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());

		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "list":
				AreaList(actor, ss.SafeRemainingArgument);
				return;
			case "new":
			case "create":
				AreaCreate(actor, ss);
				return;
			case "view":
			case "show":
				AreaView(actor, ss);
				return;
			case "add":
				AreaAdd(actor, ss);
				return;
			case "remove":
				AreaRemove(actor, ss);
				return;
			case "rename":
				AreaRename(actor, ss);
				return;
			case "weather":
				AreaWeather(actor, ss);
				return;
			case "close":
				AreaClose(actor, ss);
				return;
			case "edit":
				AreaEdit(actor, ss);
				return;
		}

		actor.OutputHandler.Send(AreaHelpText.SubstituteANSIColour());
	}

	private static void AreaClose(ICharacter actor, StringStack ss)
	{
		actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<IArea>>());
		actor.OutputHandler.Send("You are no longer editing any areas.");
	}

	private static void AreaWeather(ICharacter actor, StringStack ss)
	{
		var area = actor.EffectsOfType<BuilderEditingEffect<IArea>>().FirstOrDefault()?.EditingItem;
		if (area == null)
		{
			actor.OutputHandler.Send("You must first EDIT an area before you can configure its weather.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				"Do you want to set an overriding weather controller for the area, or do you want to CLEAR an existing one?");
			return;
		}

		var editArea = (IEditableArea)area;
		if (ss.Peek().EqualToAny("clear", "none", "remove", "delete"))
		{
			editArea.Weather = null;
			actor.OutputHandler.Send(
				"That area will now have no overriding weather controller; its weather will be whatever the zones say it should be.");
			return;
		}

		var controller = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.WeatherControllers.Get(value)
			: actor.Gameworld.WeatherControllers.GetByName(ss.Last);
		if (controller == null)
		{
			actor.OutputHandler.Send("There is no such weather controller.");
			return;
		}

		editArea.Weather = controller;
		actor.OutputHandler.Send(
			$"This area will now override the zone weather with the {controller.Name.Colour(Telnet.BoldCyan)} weather controller.");
	}

	private static void AreaRename(ICharacter actor, StringStack ss)
	{
		var area = actor.EffectsOfType<BuilderEditingEffect<IArea>>().FirstOrDefault()?.EditingItem;
		if (area == null)
		{
			actor.OutputHandler.Send("You must first EDIT an area before you can rename it.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What new name do you want to give to the area?");
			return;
		}

		var name = ss.SafeRemainingArgument.TitleCase();
		if (actor.Gameworld.Areas.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				"You cannot give the area that name because it is already assigned to an area, and area names must be unique.");
			return;
		}

		var editArea = (IEditableArea)area;
		editArea.SetName(name);
		actor.OutputHandler.Send($"That area is now known as {area.Name.Colour(Telnet.Cyan)}.");
	}

	private static void AreaRemove(ICharacter actor, StringStack ss)
	{
		var area = actor.EffectsOfType<BuilderEditingEffect<IArea>>().FirstOrDefault()?.EditingItem;
		if (area == null)
		{
			actor.OutputHandler.Send("You must first EDIT an area before you can remove anything to it.");
			return;
		}

		if (!area.Rooms.Contains(actor.Location.Room))
		{
			actor.OutputHandler.Send("You currently location is not currently considered a part of that area.");
			return;
		}

		var editArea = (IEditableArea)area;
		editArea.Remove(actor.Location.Room);
		if (actor.Location.Room.Cells.Count() > 1)
		{
			actor.OutputHandler.Send(
				$"You remove your current location and associated locations in the same room from the {area.Name.Colour(Telnet.Cyan)} area.");
		}
		else
		{
			actor.OutputHandler.Send(
				$"You remove your current location from the {area.Name.Colour(Telnet.Cyan)} area.");
		}
	}

	private static void AreaAdd(ICharacter actor, StringStack ss)
	{
		var area = actor.EffectsOfType<BuilderEditingEffect<IArea>>().FirstOrDefault()?.EditingItem;
		if (area == null)
		{
			actor.OutputHandler.Send("You must first EDIT an area before you can add anything to it.");
			return;
		}

		if (area.Rooms.Contains(actor.Location.Room))
		{
			actor.OutputHandler.Send("You currently location is already considered a part of that area.");
			return;
		}

		var editArea = (IEditableArea)area;
		editArea.Add(actor.Location.Room);
		if (actor.Location.Room.Cells.Count() > 1)
		{
			actor.OutputHandler.Send(
				$"You add your current location and associated locations in the same room to the {area.Name.Colour(Telnet.Cyan)} area.");
		}
		else
		{
			actor.OutputHandler.Send($"You add your current location to the {area.Name.Colour(Telnet.Cyan)} area.");
		}
	}

	private static void AreaView(ICharacter actor, StringStack ss)
	{
		var area = actor.EffectsOfType<BuilderEditingEffect<IArea>>().FirstOrDefault()?.EditingItem;
		if (area == null)
		{
			actor.OutputHandler.Send("You must first EDIT an area before you can view it.");
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine($"Area #{area.Id.ToString("N0", actor)}: {area.Name.Colour(Telnet.Cyan)}");
		sb.AppendLine(
			$"Weather Controller: {(area.Weather == null ? "Default".Colour(Telnet.Magenta) : $"{area.Weather.Name} (#{area.Weather.Id})")}");
		sb.AppendLine("Cells in Area:");
		foreach (var cell in area.Cells)
		{
			sb.AppendLine($"\t#{cell.Id.ToString("N0", actor)}: {cell.HowSeen(actor)}");
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	private static void AreaCreate(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to your new area?");
			return;
		}

		var name = ss.SafeRemainingArgument.TitleCase();
		if (actor.Gameworld.Areas.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("That name is already taken by an existing area. Area names must be unique.");
			return;
		}

		var area = new Area(actor.Location.Room, name);
		actor.OutputHandler.Send(
			$"You create the new area {area.Name.Colour(Telnet.Cyan)}, with ID #{area.Id.ToString("N0", actor)}. You are now editing this area.");
		actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<IArea>>());
		actor.AddEffect(new BuilderEditingEffect<IArea>(actor) { EditingItem = area });
	}

	private static void AreaEdit(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			if (actor.EffectsOfType<BuilderEditingEffect<IArea>>().Any())
			{
				AreaView(actor, ss);
				return;
			}

			actor.OutputHandler.Send("Which area would you like to edit?");
			return;
		}

		var area = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.Areas.Get(value)
			: actor.Gameworld.Areas.GetByName(ss.Last) ??
			  actor.Gameworld.Areas.FirstOrDefault(x => x.Name.StartsWith(ss.Last, StringComparison.OrdinalIgnoreCase));

		if (area == null)
		{
			actor.OutputHandler.Send("There is no such area to edit.");
			return;
		}

		actor.OutputHandler.Send(
			$"You are now editing area #{area.Id.ToString("N0", actor)} ({area.Name.Colour(Telnet.Cyan)}).");
		actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<IArea>>());
		actor.AddEffect(new BuilderEditingEffect<IArea>(actor) { EditingItem = area });
	}

	#endregion

	#region Autorooms
	public const string AutoRoomHelpText =
		@"This command is used to work with and edit room templates for the autobuilder.

The core syntax is as follows:

	#3autoroom list#0 - shows all room templates
	#3autoroom edit new <name> <school>#0 - creates a new room template
	#3autoroom clone <old> <new>#0 - clones an existing room template
	#3autoroom edit <which>#0 - begins editing a room template
	#3autoroom close#0 - closes an editing room template
	#3autoroom show <which>#0 - shows builder information about a room
	#3autoroom show#0 - shows builder information about the currently edited room
	#3autoroom edit#0 - an alias for room template show (with no args)
	#3autoroom set <...>#0 - edits the properties of a room template";

	[PlayerCommand("AutoRoom", "autoroom")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("autoroom", AutoRoomHelpText, AutoHelp.HelpArgOrNoArg)]
	protected static void AutoRoom(ICharacter actor, string command)
	{
		BuilderModule.GenericBuildingCommand(actor, new StringStack(command.RemoveFirstWord()),
			EditableItemHelper.RoomBuilder);
	}

	public const string AutoAreaHelpText =
		@"This command is used to work with and edit area templates for the autobuilder.

The core syntax is as follows:

	#3autoarea list#0 - shows all area templates
	#3autoarea edit new <name> <school>#0 - creates a new area template
	#3autoarea clone <old> <new>#0 - clones an existing area template
	#3autoarea edit <which>#0 - begins editing an area template
	#3autoarea close#0 - closes an editing area template
	#3autoarea show#0 <which> - shows builder information about a area
	#3autoarea show#0 - shows builder information about the currently edited area
	#3autoarea edit#0 - an alias for area template show (with no args)
	#3autoarea set <...>#0 - edits the properties of an area template";

	[PlayerCommand("AutoArea", "autoarea")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("autoarea", AutoAreaHelpText, AutoHelp.HelpArgOrNoArg)]
	protected static void AutoArea(ICharacter actor, string command)
	{
		BuilderModule.GenericBuildingCommand(actor, new StringStack(command.RemoveFirstWord()),
			EditableItemHelper.AreaBuilder);
	}

	#endregion
}