using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;

namespace MudSharp.Construction.Autobuilder.Rooms;

public abstract class AutobuilderRoomBase : SaveableItem, IAutobuilderRoom
{
	protected bool ApplyAutobuilderTagsAsFrameworkTags { get; set; }

	protected AutobuilderRoomBase(IFuturemud gameworld, string name)
	{
		Gameworld = gameworld;
		_name = name;
	}

	protected AutobuilderRoomBase(Models.AutobuilderRoomTemplate room, IFuturemud gameworld)
	{
		_id = room.Id;
		_name = room.Name;
		Gameworld = gameworld;
		LoadFromXml(XElement.Parse(room.Definition));
	}

	protected virtual void LoadFromXml(XElement root)
	{
		ApplyAutobuilderTagsAsFrameworkTags =
			bool.Parse(root.Element("ApplyAutobuilderTagsAsFrameworkTags")?.Value ?? "false");
	}

	protected abstract XElement SaveToXml();

	protected void ApplyTagsToCell(ICell cell, string[] tags)
	{
		if (!ApplyAutobuilderTagsAsFrameworkTags)
		{
			return;
		}

		foreach (var tag in tags)
		{
			var fwTag = Gameworld.Tags.FirstOrDefault(x => x.Name.EqualTo(tag));
			if (fwTag != null)
			{
				cell.AddTag(fwTag);
			}
		}
	}

	#region Overrides of Item

	public sealed override string FrameworkItemType => "AutobuilderRoom";

	#endregion

	#region Implementation of IAutobuilderRoom

	public abstract IAutobuilderRoom Clone(string newName);

	public abstract ICell CreateRoom(ICharacter builder, ITerrain specifiedTerrain, bool deferDescription,
		params string[] tags);

	public virtual void RedescribeRoom(ICell cell, params string[] tags)
	{
		ApplyTagsToCell(cell, tags);
	}

	protected string BuildingHelpText => $@"You can use the following options with this command:

    name <name> - renames this room template
    summary <text> - a summary of this template for LIST and SHOW
    applytags - toggles whether room tags are applied as framework tags{SubtypeHelpText}";

	protected abstract string SubtypeHelpText { get; }

	public virtual bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant().CollapseString())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "applytags":
				return BuildingCommandTags(actor, command);
			case "byline":
			case "summary":
				return BuildingCommandByline(actor, command);
			default:
				actor.OutputHandler.Send(BuildingHelpText);
				return false;
		}
	}

	private bool BuildingCommandByline(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What summary do you want to add to this room template?");
			return false;
		}

		ShowCommandByline = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send(
			$"The summary for this autobuilder room template is now {ShowCommandByline.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandTags(ICharacter actor, StringStack command)
	{
		ApplyAutobuilderTagsAsFrameworkTags = !ApplyAutobuilderTagsAsFrameworkTags;
		Changed = true;
		actor.OutputHandler.Send(
			$"This room builder template will {(ApplyAutobuilderTagsAsFrameworkTags ? "now" : "no longer")} attempt to match room builder tags with identically-named framework tags and apply them to the created room.");
		return true;
	}

	protected bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What new name do you want to give to this room builder template?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Gameworld.AutobuilderRooms.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				"There is already a room builder template with that name. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You rename the room builder template {_name.ColourName()} to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.AutobuilderRoomTemplates.Find(Id);
		dbitem.Name = Name;
		dbitem.Definition = SaveToXml().ToString();
		Changed = false;
	}

	public abstract string Show(ICharacter builder);
	public string ShowCommandByline { get; protected set; }

	#endregion
}