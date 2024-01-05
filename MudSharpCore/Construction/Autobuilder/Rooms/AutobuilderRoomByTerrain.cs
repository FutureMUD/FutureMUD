using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using ExpressionEngine;
using MoreLinq.Extensions;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.PerceptionEngine;
using Newtonsoft.Json.Serialization;

namespace MudSharp.Construction.Autobuilder.Rooms;

public class AutobuilderRoomByTerrain : AutobuilderRoomBase
{
	public static void RegisterAutobuilderLoader()
	{
		AutobuilderFactory.RegisterLoader("room by terrain",
			(room, gameworld) => new AutobuilderRoomByTerrain(room, gameworld));
		AutobuilderFactory.RegisterBuilderLoader("terrain",
			(gameworld, name) => new AutobuilderRoomByTerrain(gameworld, name));
	}

	public AutobuilderRoomInfo DefaultInfo { get; set; }
	public Dictionary<ITerrain, AutobuilderRoomInfo> TerrainInfos { get; } = new();

	public AutobuilderRoomByTerrain(AutobuilderRoomByTerrain rhs, string newName) : base(rhs.Gameworld, newName)
	{
		DefaultInfo = rhs.DefaultInfo;
		ShowCommandByline = rhs.ShowCommandByline;
		ApplyAutobuilderTagsAsFrameworkTags = rhs.ApplyAutobuilderTagsAsFrameworkTags;
		foreach (var info in rhs.TerrainInfos)
		{
			TerrainInfos.Add(info.Key, info.Value);
		}

		using (new FMDB())
		{
			var dbitem = new AutobuilderRoomTemplate
			{
				Name = _name,
				TemplateType = "room by terrain",
				Definition = SaveToXml().ToString()
			};
			FMDB.Context.AutobuilderRoomTemplates.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	protected AutobuilderRoomByTerrain(IFuturemud gameworld, string name) : base(gameworld, name)
	{
		ShowCommandByline = "A Terrain-Specific Room Template";
		DefaultInfo = new AutobuilderRoomInfo
		{
			DefaultTerrain = gameworld.Terrains.First(x => x.DefaultTerrain),
			CellName = "An Undescribed Location",
			CellDescription = "This location does not have any description",
			AmbientLightFactor = 1.0,
			OutdoorsType = gameworld.Terrains.First(x => x.DefaultTerrain).DefaultCellOutdoorsType
		};
		ApplyAutobuilderTagsAsFrameworkTags = true;

		using (new FMDB())
		{
			var dbitem = new AutobuilderRoomTemplate
			{
				Name = name,
				TemplateType = "room by terrain",
				Definition = SaveToXml().ToString()
			};
			FMDB.Context.AutobuilderRoomTemplates.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public AutobuilderRoomByTerrain(AutobuilderRoomTemplate room, IFuturemud gameworld) : base(room, gameworld)
	{
	}

	public override IAutobuilderRoom Clone(string newName)
	{
		return new AutobuilderRoomByTerrain(this, newName);
	}

	#region Overrides of AutobuilderRoomBase

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		ShowCommandByline = root.Element("ShowCommandByline")?.Value ?? "A terrain-specific room without a byline.";

		DefaultInfo = new AutobuilderRoomInfo(root.Element("Default")!.Element("Terrain"), Gameworld);

		foreach (var element in root.Element("Terrains")!.Elements("Terrain"))
		{
			var roomInfo = new AutobuilderRoomInfo(element, Gameworld);
			TerrainInfos[roomInfo.DefaultTerrain] = roomInfo;
		}
	}

	protected override XElement SaveToXml()
	{
		return new XElement("Template",
			new XElement("ApplyAutobuilderTagsAsFrameworkTags", ApplyAutobuilderTagsAsFrameworkTags),
			new XElement("ShowCommandByline", new XCData(ShowCommandByline)),
			new XElement("Default", DefaultInfo.SaveToXml()),
			new XElement("Terrains",
				from terrain in TerrainInfos
				select terrain.Value.SaveToXml()
			)
		);
	}

	private readonly Regex _terrainInfoRegex = new("\\$terrain", RegexOptions.IgnoreCase);

	public override ICell CreateRoom(ICharacter builder, ITerrain specifiedTerrain, bool deferDescription,
		params string[] tags)
	{
		var room = new Room(builder, builder.CurrentOverlayPackage);
		var cell = room.Cells.First();
		var overlay = cell.GetOrCreateOverlay(builder.CurrentOverlayPackage);
		var info = specifiedTerrain != null && TerrainInfos.ContainsKey(specifiedTerrain)
			? TerrainInfos[specifiedTerrain]
			: DefaultInfo;
		overlay.CellName = _terrainInfoRegex.Replace(info.CellName, match => info.DefaultTerrain.Name);
		overlay.CellDescription = _terrainInfoRegex.Replace(info.CellDescription, match => info.DefaultTerrain.Name);
		overlay.AmbientLightFactor = info.AmbientLightFactor;
		overlay.OutdoorsType = info.OutdoorsType;
		overlay.Terrain = specifiedTerrain ?? info.DefaultTerrain;
		cell.ForagableProfile = info.ForagableProfile;
		ApplyTagsToCell(cell, tags);
		return cell;
	}

	protected override string SubtypeHelpText => @" roomname <name> - the default name of the generated room
    description - edits the default description of the generated room
    light <percentage> - sets a default light multiplier for the generated room
    defaultterrain <terrain> - sets the default terrain used if none is supplied
    outdoors|cave|indoors|shelter - sets the default outdoor behaviour type
    fp <which> - sets the foragable profile for the default room
    fp none - removes the foragable profile from the default room
    terrain <terrain> - shows any custom information set up for a particular terrain
    terrain <terrain> name <name> - sets a name for a terrain type
    terrain <terrain> description - sets the description for a terrain type
    terrain <terrain> light <percentage> - sets the light multiplier for a terrain type
    terrain <terrain> outdoors|cave|indoors|shelter - sets the terrain type's outdoor behaviour type
    terrain <terrain> fp <which> - sets the foragable profile for the terrain
    terrain <terrain> fp none - removes a foragable profile for the terrain
    terrain <terrain> remove - removes the terrain-specific overrides";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "roomname":
			case "cellname":
				return BuildingCommandRoomName(actor, command);
			case "desc":
			case "description":
				return BuildingCommandDescription(actor, command);
			case "light":
				return BuildingCommandLight(actor, command);
			case "defaultterrain":
				return BuildingCommandDefaultTerrain(actor, command);
			case "foragable":
			case "fp":
			case "forageable":
			case "foragableprofile":
			case "forageableprofile":
				return BuildingCommandForagable(actor, command);
			case "outdoor":
			case "outdoors":
			case "outside":
				return BuildingCommandOutdoorType(actor, CellOutdoorsType.Outdoors);
			case "cave":
			case "nolight":
				return BuildingCommandOutdoorType(actor, CellOutdoorsType.IndoorsNoLight);
			case "windows":
				return BuildingCommandOutdoorType(actor, CellOutdoorsType.IndoorsWithWindows);
			case "indoors":
			case "indoor":
			case "inside":
				return BuildingCommandOutdoorType(actor, CellOutdoorsType.Indoors);
			case "shelter":
			case "climate":
			case "sheltered":
			case "exposed":
				return BuildingCommandOutdoorType(actor, CellOutdoorsType.IndoorsClimateExposed);
			case "terrain":
				return BuildingCommandTerrain(actor, command);
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	private bool BuildingCommandForagable(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"You must either specify a foragable profile to apply, or use the keyword 'none' to clear an existing one.");
			return false;
		}

		if (command.PeekSpeech().EqualToAny("none", "clear", "remove"))
		{
			DefaultInfo = DefaultInfo with { ForagableProfile = null };
			Changed = true;
			actor.OutputHandler.Send($"The default rooms generated will no longer have a foragable profile override.");
			return true;
		}

		var fp = Gameworld.ForagableProfiles.GetByIdOrName(command.SafeRemainingArgument);
		if (fp == null)
		{
			actor.OutputHandler.Send("There is no such foragable profile.");
			return false;
		}

		if (fp.Status != RevisionStatus.Current)
		{
			actor.OutputHandler.Send(
				$"The {fp.Name.ColourName()} foragable profile is {fp.Status.DescribeColour()}, and cannot be used.");
			return false;
		}

		DefaultInfo = DefaultInfo with { ForagableProfile = fp };
		Changed = true;
		actor.OutputHandler.Send(
			$"The default rooms generated with this template will use the {fp.Name.ColourName()} foragable profile.");
		return true;
	}

	private bool BuildingCommandTerrain(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which terrain type do you want to edit the template values for?");
			return false;
		}

		var terrain = Gameworld.Terrains.GetByIdOrName(command.PopSpeech());
		if (terrain == null)
		{
			actor.OutputHandler.Send("There is no such terrain type.");
			return false;
		}

		if (command.IsFinished)
		{
			if (TerrainInfos.ContainsKey(terrain))
			{
				var info = TerrainInfos[terrain];
				var sb = new StringBuilder();
				sb.AppendLine($"Info for the {terrain.Name.ColourValue()} terrain:");
				sb.AppendLine($"Room Name: {info.CellName.Colour(Telnet.Cyan)}");
				sb.AppendLine($"Behaviour: {info.OutdoorsType.Describe().ColourValue()}");
				sb.AppendLine($"Light Multiplier: {info.AmbientLightFactor.ToString("P3", actor).ColourValue()}");
				sb.AppendLine(
					$"Foragable Profile: {info.ForagableProfile?.Name.ColourValue() ?? "None".Colour(Telnet.Red)}");
				sb.AppendLine("Description:");
				sb.AppendLine();
				sb.AppendLine(info.CellDescription.Wrap(actor.InnerLineFormatLength, "\t"));
				actor.OutputHandler.Send(sb.ToString());
				return true;
			}
		}

		switch (command.PopForSwitch())
		{
			case "roomname":
			case "cellname":
			case "name":
				return BuildingCommandTerrainRoomName(actor, terrain, command);
			case "desc":
			case "description":
				return BuildingCommandTerrainDescription(actor, terrain, command);
			case "light":
				return BuildingCommandTerrainLight(actor, terrain, command);
			case "outdoor":
			case "outdoors":
			case "outside":
				return BuildingCommandTerrainOutdoorType(actor, terrain, CellOutdoorsType.Outdoors);
			case "cave":
			case "nolight":
				return BuildingCommandTerrainOutdoorType(actor, terrain, CellOutdoorsType.IndoorsNoLight);
			case "windows":
				return BuildingCommandTerrainOutdoorType(actor, terrain, CellOutdoorsType.IndoorsWithWindows);
			case "indoors":
			case "indoor":
			case "inside":
				return BuildingCommandTerrainOutdoorType(actor, terrain, CellOutdoorsType.Indoors);
			case "shelter":
			case "climate":
			case "sheltered":
			case "exposed":
				return BuildingCommandTerrainOutdoorType(actor, terrain, CellOutdoorsType.IndoorsClimateExposed);
			case "remove":
			case "rem":
			case "delete":
			case "del":
			case "none":
				return BuildingCommandTerrainDelete(actor, terrain);
			case "foragable":
			case "fp":
			case "forageable":
			case "foragableprofile":
			case "forageableprofile":
				return BuildingCommandTerrainForagable(actor, terrain, command);
		}

		actor.OutputHandler.Send(@"You must enter one of the following sub-commands after the terrain type:

    name <name> - sets the name of the rooms
    description - sets the room description
    light <percentage> - sets the light multiplier
    outdoors|cave|windows|indoors|shelter - changes the outdoors behaviour of the room
    foragable <which> - sets the foragable profile for the terrain
    foragable none - removes a foragable profile for the terrain
    remove - removes a template type for a terrain");
		return false;
	}

	private bool BuildingCommandTerrainForagable(ICharacter actor, ITerrain terrain, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"You must either specify a foragable profile to apply, or use the keyword 'none' to clear an existing one.");
			return false;
		}

		if (command.PeekSpeech().EqualToAny("none", "clear", "remove"))
		{
			if (!TerrainInfos.ContainsKey(terrain))
			{
				actor.OutputHandler.Send(
					"There is no information set up for the terrain type so there is nothing to remove.");
				return false;
			}

			TerrainInfos[terrain] = TerrainInfos[terrain] with { ForagableProfile = null };
			Changed = true;
			actor.OutputHandler.Send(
				$"The rooms generated with the {terrain.Name.ColourValue()} terrain type will no longer have a foragable profile override.");
			return true;
		}

		var fp = Gameworld.ForagableProfiles.GetByIdOrName(command.SafeRemainingArgument);
		if (fp == null)
		{
			actor.OutputHandler.Send("There is no such foragable profile.");
			return false;
		}

		if (fp.Status != RevisionStatus.Current)
		{
			actor.OutputHandler.Send(
				$"The {fp.Name.ColourName()} foragable profile is {fp.Status.DescribeColour()}, and cannot be used.");
			return false;
		}

		if (TerrainInfos.ContainsKey(terrain))
		{
			TerrainInfos[terrain] = TerrainInfos[terrain] with { ForagableProfile = fp };
		}
		else
		{
			TerrainInfos[terrain] = new AutobuilderRoomInfo
			{
				DefaultTerrain = terrain,
				CellName = "An Undescribed Room",
				CellDescription = "An undescribed room.",
				OutdoorsType = CellOutdoorsType.Outdoors,
				AmbientLightFactor = 1.0,
				ForagableProfile = fp
			};
		}

		Changed = true;
		actor.OutputHandler.Send(
			$"The {terrain.Name.ColourValue()} terrain rooms generated with this template will use the {fp.Name.ColourName()} foragable profile.");
		return true;
	}

	private bool BuildingCommandTerrainDelete(ICharacter actor, ITerrain terrain)
	{
		TerrainInfos.Remove(terrain);
		Changed = true;
		actor.OutputHandler.Send(
			$"You remove any room template information for the {terrain.Name.ColourValue()} terrain.");
		return true;
	}

	private bool BuildingCommandOutdoorType(ICharacter actor, CellOutdoorsType outdoors)
	{
		DefaultInfo = DefaultInfo with { OutdoorsType = outdoors };
		Changed = true;
		actor.OutputHandler.Send(
			$"The default rooms generated with this template will now have a type of {outdoors.DescribeEnum(true).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandDefaultTerrain(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What terrain should be used as the default fallback terrain if one is not specified?");
			return false;
		}

		var terrain = Gameworld.Terrains.GetByIdOrName(command.SafeRemainingArgument);
		if (terrain == null)
		{
			actor.OutputHandler.Send("That is not a valid terrain.");
			return false;
		}

		DefaultInfo = DefaultInfo with { DefaultTerrain = terrain, OutdoorsType = terrain.DefaultCellOutdoorsType };
		Changed = true;
		actor.OutputHandler.Send(
			$"When no terrain is specified, this room template will now use the {terrain.Name.ColourValue()} terrain as a fallback.");
		return true;
	}

	private bool BuildingCommandLight(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What default light multiplier should be applied to rooms generated with this template?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParsePercentage(out var percentage))
		{
			actor.OutputHandler.Send("That is not a valid percentage for the light multiplier.");
			return false;
		}

		DefaultInfo = DefaultInfo with { AmbientLightFactor = percentage };
		Changed = true;
		actor.OutputHandler.Send(
			$"The default light multiplier for rooms will now be {percentage.ToString("P2", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandDescription(ICharacter actor, StringStack command)
	{
		if (!string.IsNullOrEmpty(DefaultInfo.CellDescription))
		{
			actor.OutputHandler.Send("Replacing:\n" +
			                         DefaultInfo.CellDescription.Wrap(actor.InnerLineFormatLength, "\t"));
		}

		actor.OutputHandler.Send(
			"Enter the description in the editor below. Use $terrain to substitute the terrain name.");
		actor.EditorMode(PostDefaultDescription, CancelDefaultDescription, 1.0);
		return true;
	}

	private void CancelDefaultDescription(IOutputHandler arg1, object[] arg2)
	{
		arg1.Send("You decide not to change the default room description.");
	}

	private void PostDefaultDescription(string arg1, IOutputHandler arg2, object[] arg3)
	{
		DefaultInfo = DefaultInfo with { CellDescription = arg1.Trim().ProperSentences().Fullstop() };
		Changed = true;
		arg2.Send("You change the default room description.");
	}

	private bool BuildingCommandRoomName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What name do you want to give to the default (fallback) rooms created with this room template? You can use $terrain to substitute the terrain name.");
			return false;
		}

		DefaultInfo = DefaultInfo with { CellName = command.SafeRemainingArgument.TitleCase() };
		Changed = true;
		actor.OutputHandler.Send(
			$"The default rooms will now use the room name {DefaultInfo.CellName.Colour(Telnet.Cyan)}.");
		return true;
	}

	private bool BuildingCommandTerrainOutdoorType(ICharacter actor, ITerrain terrain, CellOutdoorsType outdoors)
	{
		if (TerrainInfos.ContainsKey(terrain))
		{
			TerrainInfos[terrain] = TerrainInfos[terrain] with { OutdoorsType = outdoors };
		}
		else
		{
			TerrainInfos[terrain] = new AutobuilderRoomInfo
			{
				DefaultTerrain = terrain,
				CellName = "An Undescribed Room",
				CellDescription = "An undescribed room.",
				OutdoorsType = outdoors,
				AmbientLightFactor = 1.0
			};
		}

		Changed = true;
		actor.OutputHandler.Send(
			$"The {terrain.Name.ColourValue()} terrain rooms generated with this template will now have a type of {outdoors.DescribeEnum(true).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandTerrainLight(ICharacter actor, ITerrain terrain, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What light multiplier should be applied to {terrain.Name.ColourValue()} terrain rooms generated with this template?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParsePercentage(out var percentage))
		{
			actor.OutputHandler.Send("That is not a valid percentage for the light multiplier.");
			return false;
		}

		if (TerrainInfos.ContainsKey(terrain))
		{
			TerrainInfos[terrain] = TerrainInfos[terrain] with { AmbientLightFactor = percentage };
		}
		else
		{
			TerrainInfos[terrain] = new AutobuilderRoomInfo
			{
				DefaultTerrain = terrain,
				CellName = "An Undescribed Room",
				CellDescription = "An undescribed room.",
				OutdoorsType = terrain.DefaultCellOutdoorsType,
				AmbientLightFactor = percentage
			};
		}

		Changed = true;
		actor.OutputHandler.Send(
			$"The light multiplier for {terrain.Name.ColourValue()} terrain rooms will now be {percentage.ToString("P2", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandTerrainDescription(ICharacter actor, ITerrain terrain, StringStack command)
	{
		if (TerrainInfos.ContainsKey(terrain))
		{
			actor.OutputHandler.Send("Replacing:\n" +
			                         TerrainInfos[terrain].CellDescription.Wrap(actor.InnerLineFormatLength, "\t"));
		}

		actor.OutputHandler.Send(
			"Enter the description in the editor below. Use $terrain to substitute the terrain name.");
		actor.EditorMode(PostDefaultTerrainDescription, CancelDefaultTerrainDescription, 1.0);
		return true;
	}

	private void CancelDefaultTerrainDescription(IOutputHandler arg1, object[] arg2)
	{
		var terrain = (ITerrain)arg2[0];
		arg1.Send($"You decide not to change the room description for the {terrain.Name.ColourValue()} terrain.");
	}

	private void PostDefaultTerrainDescription(string arg1, IOutputHandler arg2, object[] arg3)
	{
		var terrain = (ITerrain)arg3[0];
		if (TerrainInfos.ContainsKey(terrain))
		{
			TerrainInfos[terrain] = TerrainInfos[terrain] with
			{
				CellDescription = arg1.Trim().ProperSentences().Fullstop()
			};
		}
		else
		{
			TerrainInfos[terrain] = new AutobuilderRoomInfo
			{
				DefaultTerrain = terrain,
				CellName = "An Undescribed Room",
				CellDescription = arg1.Trim().ProperSentences().Fullstop(),
				OutdoorsType = terrain.DefaultCellOutdoorsType,
				AmbientLightFactor = 1.0
			};
		}

		Changed = true;
		arg2.Send($"You change the room description for the {terrain.Name.ColourValue()}.");
	}

	private bool BuildingCommandTerrainRoomName(ICharacter actor, ITerrain terrain, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What name do you want to give to rooms created with the {terrain.Name.ColourValue()} terrain for this room template?");
			return false;
		}

		if (TerrainInfos.ContainsKey(terrain))
		{
			TerrainInfos[terrain] = TerrainInfos[terrain] with { CellName = command.SafeRemainingArgument.TitleCase() };
		}
		else
		{
			TerrainInfos[terrain] = new AutobuilderRoomInfo
			{
				DefaultTerrain = terrain,
				CellName = command.SafeRemainingArgument.TitleCase(),
				CellDescription = "An undescribed room.",
				OutdoorsType = terrain.DefaultCellOutdoorsType,
				AmbientLightFactor = 1.0
			};
		}

		Changed = true;
		actor.OutputHandler.Send(
			$"The rooms of the {terrain.Name.ColourValue()} type will now use the room name {TerrainInfos[terrain].CellName.Colour(Telnet.Cyan)}.");
		return true;
	}

	public override string Show(ICharacter builder)
	{
		return
			$"{$"Autobuilder Room Template #{Id.ToString("N0", builder)} ({Name})".Colour(Telnet.Cyan)}\n\nThis template will create the same cell for each terrain. This particular template has specific cell information set up for the terrain types {TerrainInfos.Select(x => x.Key.Name.Colour(Telnet.Green)).ListToString()}.";
	}

	#endregion
}