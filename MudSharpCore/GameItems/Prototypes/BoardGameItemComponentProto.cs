using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Community.Boards;
using MudSharp.FutureProg;

namespace MudSharp.GameItems.Prototypes;

public class BoardGameItemComponentProto : GameItemComponentProto
{
	public IBoard Board { get; private set; }
	public IFutureProg CanViewBoard { get; private set; }
	public IFutureProg CanPostToBoard { get; private set; }
	public string CantViewBoardEcho { get; private set; }
	public string CantPostToBoardEcho { get; private set; }
	public bool ShowAuthorName { get; private set; }
	public bool ShowAuthorShortDescription { get; private set; }
	public bool ShowAuthorDescription { get; private set; }
	public string StoredAuthorName { get; private set; }
	public string StoredAuthorShortDescription { get; private set; }
	public string StoredAuthorFullDescription { get; private set; }
	public override string TypeDescription => "Board";

	#region Constructors

	protected BoardGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator,
		"Board")
	{
		CanViewBoard = Gameworld.FutureProgs.Get(Gameworld.GetStaticLong("AlwaysTrueProg"));
		CanPostToBoard = Gameworld.FutureProgs.Get(Gameworld.GetStaticLong("AlwaysTrueProg"));
		CantViewBoardEcho = Gameworld.GetStaticString("DefaultCantViewBoardEcho");
		CantPostToBoardEcho = Gameworld.GetStaticString("DefaultCantPostToBoardEcho");
		;
	}

	protected BoardGameItemComponentProto(Models.GameItemComponentProto proto, IFuturemud gameworld) : base(proto,
		gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		Board = Gameworld.Boards.Get(long.Parse(root.Element("Board").Value));
		CanViewBoard = Gameworld.FutureProgs.Get(long.Parse(root.Element("CanViewBoard").Value));
		CanPostToBoard = Gameworld.FutureProgs.Get(long.Parse(root.Element("CanPostToBoard").Value));
		CantViewBoardEcho = root.Element("CantViewBoardEcho").Value;
		CantPostToBoardEcho = root.Element("CantPostToBoardEcho").Value;
		ShowAuthorName = bool.Parse(root.Element("ShowAuthorName")?.Value ?? "false");
		ShowAuthorDescription = bool.Parse(root.Element("ShowAuthorDescription")?.Value ?? "false");
		ShowAuthorShortDescription = bool.Parse(root.Element("ShowAuthorShortDescription")?.Value ?? "false");
		StoredAuthorName = root.Element("StoredAuthorName")?.Value;
		StoredAuthorShortDescription = root.Element("StoredAuthorShortDescription")?.Value;
		StoredAuthorFullDescription = root.Element("StoredAuthorFullDescription")?.Value;
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("Board", Board?.Id ?? 0),
			new XElement("CanViewBoard", CanViewBoard?.Id ?? 0),
			new XElement("CanPostToBoard", CanPostToBoard?.Id ?? 0),
			new XElement("CantViewBoardEcho", new XCData(CantViewBoardEcho)),
			new XElement("CantPostToBoardEcho", new XCData(CantPostToBoardEcho)),
			new XElement("ShowAuthorName", ShowAuthorName),
			new XElement("ShowAuthorDescription", ShowAuthorDescription),
			new XElement("ShowAuthorShortDescription", ShowAuthorShortDescription),
			new XElement("StoredAuthorName", new XCData(StoredAuthorName ?? string.Empty)),
			new XElement("StoredAuthorShortDescription", new XCData(StoredAuthorShortDescription ?? string.Empty)),
			new XElement("StoredAuthorFullDescription", new XCData(StoredAuthorFullDescription ?? string.Empty))
		).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new BoardGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(Models.GameItemComponent component, IGameItem parent)
	{
		return new BoardGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("Board".ToLowerInvariant(), true,
			(gameworld, account) => new BoardGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Board", (proto, gameworld) => new BoardGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"Board",
			$"Lets players view and post to a board when in room",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator, (proto, gameworld) => new BoardGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText = @"You can use the following options with this component:

	#3name <name>#0 - sets the name of the component
	#3desc <desc>#0 - sets the description of the component
	#3board <board>#0 - sets the board that this item is tied to
	#3view <prog>#0 - sets the prog that controls who can view the board
	#3cantview <echo>#0 - sets the echo when someone tries to read the board who can't
	#3post <prog>#0 - sets the prog that controls who can post to the board
	#3cantpost <echo>#0 - sets the echo when someone tries to post to the board who can't
	#3showname#0 - toggles showing the author's name
	#3showsdesc#0 - toggles showing the author's short desc
	#3showdesc#0 - toggles showing the author's desc";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "board":
				return BuildingCommandBoard(actor, command);
			case "view":
			case "viewprog":
			case "read":
			case "readprog":
				return BuildingCommandViewProg(actor, command);
			case "write":
			case "post":
			case "writeprog":
			case "postprog":
				return BuildingCommandPostProg(actor, command);
			case "cantreadecho":
			case "cantviewecho":
			case "cantread":
			case "cantview":
				return BuildingCommandCantViewEcho(actor, command);
			case "cantwriteecho":
			case "cantpostecho":
			case "cantwrite":
			case "cantpost":
				return BuildingCommandWriteEcho(actor, command);
			case "showname":
				return BuildingCommandShowName(actor);
			case "showdesc":
			case "showdescription":
				return BuildingCommandShowDescription(actor);
			case "showsdesc":
			case "showshortdescription":
				return BuildingCommandShowShortDescription(actor);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandShowShortDescription(ICharacter actor)
	{
		ShowAuthorShortDescription = !ShowAuthorShortDescription;
		Changed = true;
		actor.OutputHandler.Send(
			$"This board will {(ShowAuthorShortDescription ? "now" : "no longer")} show the post author's short description.");
		return true;
	}

	private bool BuildingCommandShowDescription(ICharacter actor)
	{
		ShowAuthorDescription = !ShowAuthorDescription;
		Changed = true;
		actor.OutputHandler.Send(
			$"This board will {(ShowAuthorDescription ? "now" : "no longer")} show the post author's description.");
		return true;
	}

	private bool BuildingCommandShowName(ICharacter actor)
	{
		ShowAuthorName = !ShowAuthorName;
		Changed = true;
		actor.OutputHandler.Send(
			$"This board will {(ShowAuthorName ? "now" : "no longer")} show the post author's name.");
		return true;
	}

	private bool BuildingCommandWriteEcho(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What echo do you want to give when someone tries to write to this board who can't?");
			return false;
		}

		CantPostToBoardEcho = command.SafeRemainingArgument.ProperSentences().Fullstop();
		Changed = true;
		actor.OutputHandler.Send(
			$"The echo when someone tries to post to the board but can't is now:\n{CantPostToBoardEcho.ColourCommand()}");
		return true;
	}

	private bool BuildingCommandCantViewEcho(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What echo do you want to give when someone tries to view this board who can't?");
			return false;
		}

		CantViewBoardEcho = command.SafeRemainingArgument.ProperSentences().Fullstop();
		Changed = true;
		actor.OutputHandler.Send(
			$"The echo when someone tries to view the board but can't is now:\n{CantViewBoardEcho.ColourCommand()}");
		return true;
	}

	private bool BuildingCommandPostProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which prog do you want to use to control who can write posts on this board?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Boolean, new List<FutureProgVariableTypes>
			{
				FutureProgVariableTypes.Character
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		CanPostToBoard = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This board will now use the {prog.MXPClickableFunctionName()} prog to determine who can write posts.");
		return true;
	}

	private bool BuildingCommandViewProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which prog do you want to use to control who can view the posts on this board?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Boolean, new List<FutureProgVariableTypes>
			{
				FutureProgVariableTypes.Character
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		CanViewBoard = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This board will now use the {prog.MXPClickableFunctionName()} prog to determine who can view the posts.");
		return true;
	}

	private bool BuildingCommandBoard(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which board did you want to set this item to point to? See the {"boards".MXPSend("boards")} command for a list.");
			return false;
		}

		var board = actor.Gameworld.Boards.GetByIdOrName(command.SafeRemainingArgument);
		if (board is null)
		{
			actor.OutputHandler.Send(
				$"There is no such board. See the {"boards".MXPSend("boards")} command for a list.");
			return false;
		}

		actor.OutputHandler.Send($"This item will now point to the {board.Name.ColourName()} board.");
		Board = board;
		Changed = true;
		return true;
	}

	#endregion

	#region Overrides of EditableItem

	public override bool CanSubmit()
	{
		if (Board is null)
		{
			return false;
		}

		return base.CanSubmit();
	}

	public override string WhyCannotSubmit()
	{
		if (Board is null)
		{
			return "You must first set a board for this item to be connected to.";
		}

		return base.WhyCannotSubmit();
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor, @"{0} (#{1:N0}r{2:N0}, {3})

This turns an item into a bulletin board for the {4} board.
It {9} show the author's name
It {11} show the author's short description at the time of posting
It {10} show the author's full description at the time of posting
Uses prog {5} to determine who can read it.
Can't read echo: {7}
Uses prog {6} to determine who can write to it.
Can't write echo: {8}",
			"Board Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			Board?.Name.ColourName() ?? "None".ColourError(),
			CanViewBoard.MXPClickableFunctionName(),
			CanPostToBoard.MXPClickableFunctionName(),
			CantViewBoardEcho.ColourCommand(),
			CantPostToBoardEcho.ColourCommand(),
			ShowAuthorName ? "will" : "will not",
			ShowAuthorDescription ? "will" : "will not",
			ShowAuthorShortDescription ? "will" : "will not"
		);
	}
}