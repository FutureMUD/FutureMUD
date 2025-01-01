using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Construction.Autobuilder.Areas;
using MudSharp.Database;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.PerceptionEngine;
using MudSharp.Work.Foraging;

namespace MudSharp.Construction.Autobuilder.Rooms;

public class AutobuilderRoomSimple : AutobuilderRoomBase
{
	public static void RegisterAutobuilderLoader()
	{
		AutobuilderFactory.RegisterLoader("simple", (room, gameworld) => new AutobuilderRoomSimple(room, gameworld));
		AutobuilderFactory.RegisterBuilderLoader("simple",
			(gameworld, name) => new AutobuilderRoomSimple(gameworld, name));
	}

	public string CellName { get; protected set; }
	public string CellDescription { get; protected set; }
	public CellOutdoorsType OutdoorsType { get; protected set; }
	public double AmbientLightFactor { get; protected set; }
	public ITerrain DefaultTerrain { get; protected set; }
	public IForagableProfile ForagableProfile { get; protected set; }

	public AutobuilderRoomSimple(IFuturemud gameworld, string name) : base(gameworld, name)
	{
		ShowCommandByline = "A Terrain-Specific Room Template";
		DefaultTerrain = gameworld.Terrains.First(x => x.DefaultTerrain);
		CellName = "An Undescribed Location";
		CellDescription = "This location does not have any description";
		AmbientLightFactor = 1.0;
		OutdoorsType = DefaultTerrain.DefaultCellOutdoorsType;
		ApplyAutobuilderTagsAsFrameworkTags = true;

		using (new FMDB())
		{
			var dbitem = new AutobuilderRoomTemplate
			{
				Name = name,
				TemplateType = "simple",
				Definition = SaveToXml().ToString()
			};
			FMDB.Context.AutobuilderRoomTemplates.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public AutobuilderRoomSimple(AutobuilderRoomSimple rhs, string newName) : base(rhs.Gameworld, newName)
	{
		Gameworld = rhs.Gameworld;
		CellName = rhs.CellName;
		CellDescription = rhs.CellDescription;
		OutdoorsType = rhs.OutdoorsType;
		AmbientLightFactor = rhs.AmbientLightFactor;
		DefaultTerrain = rhs.DefaultTerrain;
		ForagableProfile = rhs.ForagableProfile;
		ShowCommandByline = rhs.ShowCommandByline;
		ApplyAutobuilderTagsAsFrameworkTags = rhs.ApplyAutobuilderTagsAsFrameworkTags;

		using (new FMDB())
		{
			var dbitem = new AutobuilderRoomTemplate
			{
				Name = _name,
				TemplateType = "simple",
				Definition = SaveToXml().ToString()
			};
			FMDB.Context.AutobuilderRoomTemplates.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	protected AutobuilderRoomSimple(AutobuilderRoomTemplate room, IFuturemud gameworld) : base(room, gameworld)
	{
	}

	#region Overrides of AutobuilderRoomBase

	protected override void LoadFromXml(XElement root)
	{
		CellName = root.Element("RoomName")?.Value ?? "An Unnamed Room";
		CellDescription = root.Element("RoomDescription")?.Value ?? "An undescribed room";
		OutdoorsType =
			(CellOutdoorsType)int.Parse(root.Element("OutdoorsType")?.Value ??
										((int)CellOutdoorsType.Outdoors).ToString());
		AmbientLightFactor = double.Parse(root.Element("CellLightMultiplier")?.Value ?? "1.0");
		DefaultTerrain = Gameworld.Terrains.Get(long.Parse(root.Element("DefaultTerrain")?.Value ?? "0")) ??
						 Gameworld.Terrains.FirstOrDefault(x => x.DefaultTerrain);
		ForagableProfile =
			Gameworld.ForagableProfiles.Get(long.Parse(root.Element("ForagableProfile")?.Value ?? "0"));
		ShowCommandByline = root.Element("ShowCommandByline")?.Value ?? "A simple room without a byline.";
		base.LoadFromXml(root);
	}

	protected override XElement SaveToXml()
	{
		return new XElement("Template",
			new XElement("DefaultTerrain", DefaultTerrain?.Id ?? 0),
			new XElement("RoomName", new XCData(CellName)),
			new XElement("RoomDescription", new XCData(CellDescription)),
			new XElement("OutdoorsType", (int)OutdoorsType),
			new XElement("CellLightMultiplier", AmbientLightFactor),
			new XElement("ForagableProfile", ForagableProfile?.Id ?? 0),
			new XElement("ApplyAutobuilderTagsAsFrameworkTags", ApplyAutobuilderTagsAsFrameworkTags),
			new XElement("ShowCommandByline", new XCData(ShowCommandByline))
		);
	}

	public override IAutobuilderRoom Clone(string newName)
	{
		throw new NotImplementedException();
	}

	public override ICell CreateRoom(ICharacter builder, ITerrain specifiedTerrain, bool deferDescription,
		params string[] tags)
	{
		var room = new Room(builder, builder.CurrentOverlayPackage);
		var cell = room.Cells.First();
		var overlay = cell.GetOrCreateOverlay(builder.CurrentOverlayPackage);
		overlay.CellName = CellName;
		overlay.CellDescription = CellDescription;
		overlay.AmbientLightFactor = AmbientLightFactor;
		overlay.OutdoorsType = OutdoorsType;
		overlay.Terrain = specifiedTerrain ?? DefaultTerrain;
		cell.ForagableProfile = ForagableProfile;
		ApplyTagsToCell(cell, tags);
		return cell;
	}

	protected override string SubtypeHelpText => @" #3roomname <name>#0 - the name of the generated room
	#3description#0 - edits the description of the generated room
	#3light <percentage>#0 - sets a light multiplier for the generated room
	#3defaultterrain <terrain>#0 - sets the terrain used if none is supplied
	#3fp <which>#0 - sets the foragable profile
	#3fp none#0 - removes a foragable profile
	#3outdoors|cave|indoors|shelter#0 - sets the outdoor behaviour type";

	public override string Show(ICharacter builder)
	{
		return
			$"{$"Autobuilder Area Template #{Id} ({Name})".Colour(Telnet.Cyan)}\n\nThis template will create the same cell every time. This particular template creates cells with terrain type {DefaultTerrain.Name.Colour(Telnet.Green)}, outdoors type {OutdoorsType.Describe().Colour(Telnet.Green)} and Ambient Light Factor {AmbientLightFactor:N3}. {(ForagableProfile == null ? "It does not have a foragable profile set." : $"It will use the {ForagableProfile.Name.Colour(Telnet.Green)} foragable profile.")}\n\nCell Name: {CellName}\nCell Description:\n\n{CellDescription.Wrap(builder.InnerLineFormatLength, "\t")}";
	}

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
			case "terrain":
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
			ForagableProfile = null;
			Changed = true;
			actor.OutputHandler.Send($"The rooms generated will no longer have a foragable profile override.");
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

		ForagableProfile = fp;
		Changed = true;
		actor.OutputHandler.Send(
			$"The rooms generated with this template will use the {fp.Name.ColourName()} foragable profile.");
		return true;
	}

	private bool BuildingCommandOutdoorType(ICharacter actor, CellOutdoorsType outdoors)
	{
		OutdoorsType = outdoors;
		Changed = true;
		actor.OutputHandler.Send(
			$"The rooms generated with this template will now have a type of {outdoors.DescribeEnum(true).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandDefaultTerrain(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What terrain should be used as the terrain if one is not specified?");
			return false;
		}

		var terrain = Gameworld.Terrains.GetByIdOrName(command.SafeRemainingArgument);
		if (terrain == null)
		{
			actor.OutputHandler.Send("That is not a valid terrain.");
			return false;
		}

		DefaultTerrain = terrain;
		OutdoorsType = terrain.DefaultCellOutdoorsType;
		Changed = true;
		actor.OutputHandler.Send(
			$"When no terrain is specified, this room template will now use the {terrain.Name.ColourValue()} terrain as a fallback.");
		return true;
	}

	private bool BuildingCommandLight(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What light multiplier should be applied to rooms generated with this template?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParsePercentage(out var percentage))
		{
			actor.OutputHandler.Send("That is not a valid percentage for the light multiplier.");
			return false;
		}

		AmbientLightFactor = percentage;
		Changed = true;
		actor.OutputHandler.Send(
			$"The light multiplier for rooms will now be {percentage.ToString("P2", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandDescription(ICharacter actor, StringStack command)
	{
		if (!string.IsNullOrEmpty(CellDescription))
		{
			actor.OutputHandler.Send("Replacing:\n" + CellDescription.Wrap(actor.InnerLineFormatLength, "\t"));
		}

		actor.OutputHandler.Send(
			"Enter the description in the editor below. Use $terrain to substitute the terrain name.");
		actor.EditorMode(PostDefaultDescription, CancelDefaultDescription, 1.0);
		return true;
	}

	private void CancelDefaultDescription(IOutputHandler arg1, object[] arg2)
	{
		arg1.Send("You decide not to change the room description.");
	}

	private void PostDefaultDescription(string arg1, IOutputHandler arg2, object[] arg3)
	{
		CellDescription = arg1.Trim().ProperSentences().Fullstop();
		Changed = true;
		arg2.Send("You change the room description.");
	}

	private bool BuildingCommandRoomName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What name do you want to give to the default (fallback) rooms created with this room template? You can use $terrain to substitute the terrain name.");
			return false;
		}

		CellName = command.SafeRemainingArgument.TitleCase();
		Changed = true;
		actor.OutputHandler.Send($"The default rooms will now use the room name {CellName.Colour(Telnet.Cyan)}.");
		return true;
	}

	#endregion
}