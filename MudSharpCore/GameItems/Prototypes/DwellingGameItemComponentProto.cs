using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Commands.Trees;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Prototypes;

public class DwellingGameItemComponentProto : GameItemComponentProto
{
	private long _templateEntryCellId;
	private ICell _templateEntryCell;

	public ICell TemplateEntryCell
	{
		get
		{
			if (_templateEntryCellId != 0)
			{
				_templateEntryCell = Gameworld.Cells.Get(_templateEntryCellId);
				_templateEntryCellId = 0;
			}

			return _templateEntryCell;
		}
		protected set => _templateEntryCell = value;
	}

	private long _doorProtoId;
	private IGameItemProto _doorProto;

	public IGameItemProto DoorProto
	{
		get
		{
			if (_doorProtoId != 0)
			{
				_doorProto = Gameworld.ItemProtos.Get(_doorProtoId);
				_doorProtoId = 0;
			}

			return _doorProto;
		}
		protected set => _doorProto = value;
	}

	public SizeCategory DoorSize { get; protected set; }
	public string EntranceKeyword { get; protected set; }
	public string EntranceDescription { get; protected set; }
	public override string TypeDescription => "Dwelling";

	public override bool WarnBeforePurge => true;

	#region Constructors

	protected DwellingGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator,
		"Dwelling")
	{
		DoorSize = SizeCategory.Large;
		EntranceKeyword = "building";
		EntranceDescription = "the building";
	}

	protected DwellingGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) : base(
		proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		_doorProtoId = long.Parse(root.Element("DoorProto")?.Value ?? "0");
		_templateEntryCellId = long.Parse(root.Element("TemplateEntryCell")?.Value ?? "0");
		DoorSize = (SizeCategory)int.Parse(root.Element("DoorSize")?.Value ?? "0");
		EntranceKeyword = root.Element("EntranceKeyword")?.Value ?? "building";
		EntranceDescription = root.Element("EntranceDescription")?.Value ?? "the building";
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("TemplateEntryCell", TemplateEntryCell?.Id ?? 0L),
			new XElement("DoorProto", DoorProto?.Id ?? 0L),
			new XElement("DoorSize", (int)DoorSize),
			new XElement("EntranceKeyword", new XCData(EntranceKeyword)),
			new XElement("EntranceDescription", new XCData(EntranceDescription))
		).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new DwellingGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new DwellingGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("Dwelling".ToLowerInvariant(), true,
			(gameworld, account) => new DwellingGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Dwelling",
			(proto, gameworld) => new DwellingGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"Dwelling",
			$"Makes an item that when put in the room creates an enterable area.",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator, (proto, gameworld) => new DwellingGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	#region Overrides of GameItemComponentProto

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tentry <id> - the cell that is in the template that will be used for the entry\n\tkeyword <keyword> - the keyword that will be used for the external entrance\n\tdoor <item>|none - sets the item template that loads up as a door by default\n\tsize <item size> - sets the size of the door that fits in the entrance.";

	public override string ShowBuildingHelp => BuildingHelpText;

	#endregion

	#region Overrides of EditableItem

	public override bool CanSubmit()
	{
		if (TemplateEntryCell == null)
		{
			return false;
		}

		return base.CanSubmit();
	}

	public override string WhyCannotSubmit()
	{
		if (TemplateEntryCell == null)
		{
			return "You must set a cell as the entry cell for the template before you can submit.";
		}

		return base.WhyCannotSubmit();
	}

	#endregion

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "door":
				return BuildingCommandDoor(actor, command);
			case "entry":
				return BuildingCommandEntry(actor, command);
			case "size":
				return BuildingCommandSize(actor, command);
			case "keyword":
			case "key":
				return BuildingCommandKeyword(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandKeyword(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which keyword would you like the exit to the outside to use?");
			return false;
		}

		EntranceKeyword = command.Pop().ToLowerInvariant();
		Changed = true;
		actor.Send($"This dwelling will now generate an external exit with the keyword '{EntranceKeyword}'.");
		return true;
	}

	private bool BuildingCommandSize(ICharacter actor, StringStack command)
	{
		if (DoorProto != null)
		{
			actor.Send("You cannot change the size of the doorframe while you have a door prototype set.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.Send(
				$"What size doors should be permitted to be installed in this door? See {"show itemsizes".Colour(Telnet.Yellow)} for a correct list.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<SizeCategory>(out var size))
		{
			actor.OutputHandler.Send(
				$"That is not a valid item size. See {"show itemsizes".Colour(Telnet.Yellow)} for a correct list.");
			return false;
		}

		DoorSize = size;
		Changed = true;
		actor.Send($"The entranceway for this dwelling will now accept doors of size {DoorSize.Describe()}.");
		return true;
	}

	private bool BuildingCommandDoor(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				"You must specify a game item template which is a door, or use \"none\" to set it to not have a door.");
			return false;
		}

		if (command.Peek().EqualTo("none"))
		{
			DoorProto = null;
			Changed = true;
			actor.Send(
				$"This dwelling will now load up without a door. It still permits doors of size {DoorSize.Describe()} to be installed.");
			return true;
		}

		var proto = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.ItemProtos.Get(value)
			: Gameworld.ItemProtos.GetByName(command.Last);
		if (proto == null)
		{
			actor.Send("There is no such game item template to use for the door.");
			return false;
		}

		if (proto.Status != RevisionStatus.Current)
		{
			actor.Send(
				$"The prototype {proto.Name.Colour(Telnet.Cyan)} ({proto.Id}r{proto.RevisionNumber}) is not approved for use.");
			return false;
		}

		if (!proto.IsItemType<DoorGameItemComponentProto>())
		{
			actor.Send(
				$"The prototype {proto.Name.Colour(Telnet.Cyan)} ({proto.Id}r{proto.RevisionNumber}) is not a door.");
			return false;
		}

		DoorProto = proto;
		DoorSize = DoorProto.Size;
		Changed = true;
		actor.Send(
			$"This dwelling will now use the prototype {proto.Name.Colour(Telnet.Cyan)} ({proto.Id}r{proto.RevisionNumber}) for its doors.");
		return true;
	}

	private bool BuildingCommandEntry(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("You must specify a cell to act as the entryway for the template area for your dwelling.");
			return false;
		}

		if (!long.TryParse(command.Pop(), out var value))
		{
			actor.Send("You must specify a valid ID number for the cell you want to set as the entryway.");
			return false;
		}

		var cell = Gameworld.Cells.Get(value);
		if (cell == null)
		{
			actor.Send("There is no such cell to use as the entryway.");
			return false;
		}

		Changed = true;
		TemplateEntryCell = cell;
		actor.Send(
			$"You set the entryway cell for this dwelling to cell #{cell.Id} ({cell.CurrentOverlay.CellName}).\nIt is connected to {GetTemplateCells.Count() - 1:N0} rooms.");
		return true;
	}

	#endregion

	public IEnumerable<ICell> GetTemplateCells
	{
		get
		{
			return (TemplateEntryCell?.CellsInVicinity(10U,
				exit => true,
				cell => !cell.Temporary
			) ?? Enumerable.Empty<ICell>()).ToList();
		}
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nThis item is a dwelling; an item that creates an internal set of rooms when it is loaded, and destroys them when it is destroyed. It uses cell {4} as its template, which is connected to {5:N0} other rooms. {6}. It uses the keyword {7} for its external exit.",
			"Dwelling Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			TemplateEntryCell != null
				? $"#{TemplateEntryCell.Id:N0} ({TemplateEntryCell.CurrentOverlay.CellName})".Colour(Telnet.Cyan)
				: "not yet set".Colour(Telnet.Red),
			GetTemplateCells.Count(),
			DoorProto != null
				? $"It uses item prototype {DoorProto.Id} ({DoorProto.Name}) for its external door"
				: $"It does not load with a door, but permits doors of size {DoorSize.Describe()} to be installed",
			EntranceKeyword ?? "none"
		);
	}
}