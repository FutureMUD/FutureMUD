using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModelPlane = MudSharp.Models.Plane;

namespace MudSharp.Planes;

public class Plane : SavableKeywordedItem, IPlane
{
	private readonly List<string> _aliases = new();
	private string _description;
	private string _roomDescriptionAddendum;
	private string _roomNameFormat;
	private int _displayOrder;
	private bool _isDefault;

	public Plane(ModelPlane plane, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = plane.Id;
		_name = plane.Name;
		_description = plane.Description;
		_roomDescriptionAddendum = plane.RoomDescriptionAddendum;
		_roomNameFormat = plane.RoomNameFormat;
		_displayOrder = plane.DisplayOrder;
		_isDefault = plane.IsDefault;
		_aliases.AddRange((plane.Alias ?? string.Empty).Split(' ', System.StringSplitOptions.RemoveEmptyEntries));
		ResetKeywords();
	}

	public Plane(IFuturemud gameworld, string name)
	{
		Gameworld = gameworld;
		_name = name.TitleCase();
		_description = "An undescribed metaphysical plane.";
		_displayOrder = (gameworld.Planes.Any() ? gameworld.Planes.Max(x => x.DisplayOrder) : 0) + 1;
		_isDefault = !gameworld.Planes.Any();
		_aliases.AddRange(GetKeywordsFromSDesc(_name));

		using (new FMDB())
		{
			var dbitem = new ModelPlane
			{
				Name = _name,
				Alias = _aliases.ListToString(separator: " ", conjunction: ""),
				Description = _description,
				RoomDescriptionAddendum = _roomDescriptionAddendum,
				RoomNameFormat = _roomNameFormat,
				DisplayOrder = _displayOrder,
				IsDefault = _isDefault
			};
			FMDB.Context.Planes.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}

		ResetKeywords();
	}

	public override string FrameworkItemType => "Plane";
	public IEnumerable<string> Aliases => _aliases;
	public IEnumerable<string> Names => _aliases.Append(Name);
	public string Description => _description;
	public string RoomDescriptionAddendum => _roomDescriptionAddendum;
	public string RoomNameFormat => _roomNameFormat;
	public int DisplayOrder => _displayOrder;
	public bool IsDefault => _isDefault;

	public override void Save()
	{
		using (new FMDB())
		{
			var dbitem = FMDB.Context.Planes.Find(Id);
			dbitem.Name = Name;
			dbitem.Alias = _aliases.ListToString(separator: " ", conjunction: "");
			dbitem.Description = _description;
			dbitem.RoomDescriptionAddendum = _roomDescriptionAddendum;
			dbitem.RoomNameFormat = _roomNameFormat;
			dbitem.DisplayOrder = _displayOrder;
			dbitem.IsDefault = _isDefault;
			FMDB.Context.SaveChanges();
		}

		Changed = false;
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Plane #{Id.ToString("N0", actor)} - {Name.ColourName()}");
		sb.AppendLine($"Default: {IsDefault.ToColouredString()}");
		sb.AppendLine($"Display Order: {DisplayOrder.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Aliases: {Aliases.Select(x => x.ColourCommand()).DefaultIfEmpty("None".ColourError()).ListToString()}");
		sb.AppendLine($"Room Name Format: {(string.IsNullOrWhiteSpace(RoomNameFormat) ? "None".ColourError() : RoomNameFormat.ColourCommand())}");
		sb.AppendLine();
		sb.AppendLine("Description:");
		sb.AppendLine();
		sb.AppendLine(Description.Wrap(actor.InnerLineFormatLength, "\t"));
		sb.AppendLine();
		sb.AppendLine("Room Description Addendum:");
		sb.AppendLine();
		sb.AppendLine(string.IsNullOrWhiteSpace(RoomDescriptionAddendum)
			? "\tNone".ColourError()
			: RoomDescriptionAddendum.Wrap(actor.InnerLineFormatLength, "\t"));
		return sb.ToString();
	}

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "alias":
			case "aliases":
			case "keyword":
			case "keywords":
				return BuildingCommandAliases(actor, command);
			case "description":
			case "desc":
				return BuildingCommandDescription(actor, command);
			case "addendum":
			case "roomdesc":
			case "roomdescription":
			case "roomdescriptionaddendum":
				return BuildingCommandRoomDescriptionAddendum(actor, command);
			case "roomname":
			case "roomnameformat":
			case "nameformat":
			case "format":
				return BuildingCommandRoomNameFormat(actor, command);
			case "order":
			case "display":
			case "displayorder":
				return BuildingCommandDisplayOrder(actor, command);
			case "default":
				return BuildingCommandDefault(actor);
		}

		actor.OutputHandler.Send(@"You can use the following options with this command:

	#3name <name>#0 - renames this plane
	#3aliases <alias list>#0 - sets the aliases for this plane
	#3description <description>#0 - sets the builder description
	#3addendum <text|none>#0 - sets or clears room description addendum text
	#3roomname <format|none>#0 - sets or clears the room name format, e.g. #3Astral Plane {0}#0
	#3order <number>#0 - sets the display order
	#3default#0 - makes this the default plane".SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give this plane?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Gameworld.Planes.Any(x => x != this && x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send($"There is already a plane called {name.ColourName()}.");
			return false;
		}

		_name = name;
		ResetKeywords();
		Changed = true;
		actor.OutputHandler.Send($"You rename this plane to {Name.ColourName()}.");
		return true;
	}

	private bool BuildingCommandAliases(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which aliases should this plane have?");
			return false;
		}

		_aliases.Clear();
		_aliases.AddRange(command.SafeRemainingArgument.Split(' ', System.StringSplitOptions.RemoveEmptyEntries));
		ResetKeywords();
		Changed = true;
		actor.OutputHandler.Send($"This plane now has the aliases {Aliases.Select(x => x.ColourCommand()).ListToString()}.");
		return true;
	}

	private bool BuildingCommandDescription(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What description should this plane have?");
			return false;
		}

		_description = command.SafeRemainingArgument.ProperSentences().Fullstop();
		Changed = true;
		actor.OutputHandler.Send($"You set the description of {Name.ColourName()}.");
		return true;
	}

	private bool BuildingCommandRoomDescriptionAddendum(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What room description addendum should this plane show? Use none to clear it.");
			return false;
		}

		var text = command.SafeRemainingArgument;
		if (text.EqualToAny("none", "clear", "remove", "delete", "blank"))
		{
			_roomDescriptionAddendum = null;
			Changed = true;
			actor.OutputHandler.Send($"{Name.ColourName()} will no longer add text to room descriptions.");
			return true;
		}

		_roomDescriptionAddendum = text.Fullstop();
		Changed = true;
		actor.OutputHandler.Send($"{Name.ColourName()} will now add that text to room descriptions.");
		return true;
	}

	private bool BuildingCommandRoomNameFormat(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What room name format should this plane use? Use none to clear it. The format must include {0}.");
			return false;
		}

		var format = command.SafeRemainingArgument;
		if (format.EqualToAny("none", "clear", "remove", "delete", "blank"))
		{
			_roomNameFormat = null;
			Changed = true;
			actor.OutputHandler.Send($"{Name.ColourName()} will no longer alter room names.");
			return true;
		}

		if (!format.Contains("{0}", StringComparison.Ordinal))
		{
			actor.OutputHandler.Send("Room name formats must include {0}, which is replaced with the normal room name.");
			return false;
		}

		string example;
		try
		{
			example = string.Format(format, "A Test Room");
		}
		catch (FormatException)
		{
			actor.OutputHandler.Send("That is not a valid string format. Use {0} where the normal room name should appear.");
			return false;
		}

		_roomNameFormat = format;
		Changed = true;
		actor.OutputHandler.Send($"{Name.ColourName()} will now show room names like {example.ColourRoom()}.");
		return true;
	}

	private bool BuildingCommandDisplayOrder(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !int.TryParse(command.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send("You must enter a valid display order.");
			return false;
		}

		_displayOrder = value;
		Changed = true;
		actor.OutputHandler.Send($"This plane now has a display order of {DisplayOrder.ToString("N0", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandDefault(ICharacter actor)
	{
		if (IsDefault)
		{
			actor.OutputHandler.Send($"{Name.ColourName()} is already the default plane.");
			return false;
		}

		foreach (var plane in Gameworld.Planes.OfType<Plane>())
		{
			plane._isDefault = plane == this;
			plane.Changed = true;
		}

		actor.OutputHandler.Send($"{Name.ColourName()} is now the default plane.");
		return true;
	}

	private void ResetKeywords()
	{
		_keywords = new Lazy<List<string>>(() =>
			GetKeywordsFromSDesc(Name)
				.Concat(_aliases)
				.Distinct(StringComparer.InvariantCultureIgnoreCase)
				.ToList());
	}
}
