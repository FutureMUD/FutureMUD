using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Framework;
using ExpressionEngine;
using MudSharp.Commands.Trees;
using MudSharp.Database;
using MudSharp.Framework.Revision;
using MudSharp.PerceptionEngine;

namespace MudSharp.Construction.Autobuilder.Rooms;

public class AutobuilderRoomRandomDescription : AutobuilderRoomBase
{
	public static void RegisterAutobuilderLoader()
	{
		AutobuilderFactory.RegisterLoader("room random description",
			(room, gameworld) => new AutobuilderRoomRandomDescription(room, gameworld));
		AutobuilderFactory.RegisterBuilderLoader("random",
			(gameworld, name) => new AutobuilderRoomRandomDescription(gameworld, name));
	}

	protected AutobuilderRoomRandomDescription(AutobuilderRoomRandomDescription rhs, string name) : base(rhs.Gameworld,
		name)
	{
		DefaultInfo = rhs.DefaultInfo;
		NumberOfRandomElements = new Expression(rhs.NumberOfRandomElements.OriginalExpression);
		AddToAllRoomDescriptions = rhs.AddToAllRoomDescriptions;
		ShowCommandByline = rhs.ShowCommandByline;
		ApplyAutobuilderTagsAsFrameworkTags = rhs.ApplyAutobuilderTagsAsFrameworkTags;
		foreach (var info in rhs.TerrainInfos)
		{
			TerrainInfos.Add(info.Key, (info.Value.Item1, new Expression(info.Value.Item2.OriginalExpression)));
		}

		foreach (var element in RandomDescriptionElements)
		{
			RandomDescriptionElements.Add(element.Clone());
		}

		using (new FMDB())
		{
			var dbitem = new AutobuilderRoomTemplate
			{
				Name = _name,
				TemplateType = "room random description",
				Definition = SaveToXml().ToString()
			};
			FMDB.Context.AutobuilderRoomTemplates.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public AutobuilderRoomRandomDescription(IFuturemud gameworld, string name) : base(gameworld, name)
	{
		ShowCommandByline = "A Random Room Template";
		DefaultInfo = new AutobuilderRoomInfo
		{
			DefaultTerrain = gameworld.Terrains.First(x => x.DefaultTerrain),
			CellName = "An Undescribed Location",
			CellDescription = "This location does not have any description",
			AmbientLightFactor = 1.0,
			OutdoorsType = gameworld.Terrains.First(x => x.DefaultTerrain).DefaultCellOutdoorsType
		};
		NumberOfRandomElements = new Expression("2+1d2");
		AddToAllRoomDescriptions = string.Empty;
		ApplyAutobuilderTagsAsFrameworkTags = true;

		using (new FMDB())
		{
			var dbitem = new AutobuilderRoomTemplate
			{
				Name = name,
				TemplateType = "room random description",
				Definition = SaveToXml().ToString()
			};
			FMDB.Context.AutobuilderRoomTemplates.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public AutobuilderRoomRandomDescription(AutobuilderRoomTemplate room, IFuturemud gameworld) : base(room, gameworld)
	{
	}

	public AutobuilderRoomInfo DefaultInfo { get; set; }
	public Expression NumberOfRandomElements { get; set; }
	public Dictionary<ITerrain, (AutobuilderRoomInfo, Expression)> TerrainInfos { get; } = new();
	public List<IAutobuilderRandomDescriptionElement> RandomDescriptionElements { get; } = new();
	public string AddToAllRoomDescriptions { get; protected set; }

	protected override void LoadFromXml(XElement root)
	{
		ShowCommandByline = root.Element("ShowCommandByline")?.Value ?? "A terrain-specific room without a byline.";
		AddToAllRoomDescriptions = root.Element("AddToAllRoomDescriptions")?.Value ?? string.Empty;

		foreach (var element in root.Elements("Terrain") ?? Enumerable.Empty<XElement>())
		{
			var terrain = long.TryParse(element.Attribute("type")?.Value ?? "0", out var value)
				? Gameworld.Terrains.Get(value)
				: Gameworld.Terrains.GetByName(element.Attribute("type")?.Value);
			if (terrain == null)
			{
				continue;
			}

			var roomInfo = new AutobuilderRoomInfo(element, Gameworld);
			TerrainInfos[terrain] =
				(roomInfo, new Expression(element.Element("NumberOfRandomElements")?.Value ?? "1"));
		}

		var defaultInfo = new AutobuilderRoomInfo(root, Gameworld);
		DefaultInfo = defaultInfo;
		NumberOfRandomElements = new Expression(root.Element("NumberOfRandomElements")?.Value ?? "1");

		foreach (var element in root.Element("Descriptions")?.Elements() ?? Enumerable.Empty<XElement>())
		{
			RandomDescriptionElements.Add(AutobuilderRoomDescriptionElementFactory.LoadElement(element, Gameworld));
		}

		base.LoadFromXml(root);
	}

	protected override XElement SaveToXml()
	{
		var result = new XElement("Template",
			new XElement("ApplyAutobuilderTagsAsFrameworkTags", ApplyAutobuilderTagsAsFrameworkTags),
			new XElement("ShowCommandByline", new XCData(ShowCommandByline)),
			new XElement("Default", DefaultInfo.SaveToXml(),
				new XElement("NumberOfRandomElements", new XCData(NumberOfRandomElements.OriginalExpression))),
			new XElement("Descriptions",
				from item in RandomDescriptionElements select item.SaveToXml()
			)
		);
		var terrainElement = new XElement("Terrains");
		foreach (var terrain in TerrainInfos)
		{
			var subElement = terrain.Value.Item1.SaveToXml();
			subElement.Add(new XElement("NumberOfRandomElements", new XCData(terrain.Value.Item2.OriginalExpression)));
			terrainElement.Add(subElement);
		}

		result.Add(terrainElement);
		return result;
	}

	#region Overrides of AutobuilderRoomBase

	public override IAutobuilderRoom Clone(string newName)
	{
		return new AutobuilderRoomRandomDescription(this, newName);
	}

	#endregion

	public override ICell CreateRoom(ICharacter builder, ITerrain specifiedTerrain, bool deferDescription,
		params string[] tags)
	{
		var room = new Room(builder, builder.CurrentOverlayPackage);
		var cell = room.Cells.First();
		var overlay = cell.GetOrCreateOverlay(builder.CurrentOverlayPackage);
		var (info, expression) = specifiedTerrain != null && TerrainInfos.ContainsKey(specifiedTerrain)
			? TerrainInfos[specifiedTerrain]
			: (DefaultInfo, NumberOfRandomElements);

		overlay.Terrain = specifiedTerrain ?? info.DefaultTerrain;
		overlay.OutdoorsType = overlay.Terrain.DefaultCellOutdoorsType;
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

		cell.ForagableProfile = info.ForagableProfile;
		ApplyTagsToCell(cell, tags);

		if (!deferDescription)
		{
			var texts = new List<(string Name, string Text)>();
			var random = Convert.ToInt32(expression.Evaluate());
			var validElements = RandomDescriptionElements
								.Where(x => x.Applies(specifiedTerrain ?? info.DefaultTerrain, tags)).ToList();
			var mandatoryElements = validElements.Where(x => x.MandatoryIfValid).OrderBy(x => x.MandatoryPosition)
												 .ToList();
			foreach (var mandatory in mandatoryElements)
			{
				validElements.Remove(mandatory);
				texts.Add(mandatory.TextForTags(specifiedTerrain ?? info.DefaultTerrain, tags));
				random--;
			}

			while (random > 0)
			{
				if (!validElements.Any())
				{
					break;
				}

				var element = validElements.GetWeightedRandom(x => x.Weight);
				validElements.Remove(element);
				texts.Add(element.TextForTags(specifiedTerrain ?? info.DefaultTerrain, tags));
				random--;
			}

			overlay.CellName = string
							   .Format(
								   texts.Where(x => !string.IsNullOrEmpty(x.Name)).GetRandomElement().Name ??
								   info.CellName,
								   info.CellName, (specifiedTerrain ?? info.DefaultTerrain).Name).TitleCase();
			var sb = new StringBuilder();
			if (!string.IsNullOrEmpty(info.CellDescription))
			{
				sb.Append(info.CellDescription);
			}

			foreach (var element in texts)
			{
				sb.Append(element.Text.LeadingSpaceIfNotEmpty().Fullstop());
			}

			if (!string.IsNullOrWhiteSpace(AddToAllRoomDescriptions))
			{
				sb.Append(AddToAllRoomDescriptions.LeadingSpaceIfNotEmpty().Fullstop());
			}

			overlay.CellDescription = sb.ToString();
		}

		return cell;
	}

	public override void RedescribeRoom(ICell cell, params string[] tags)
	{
		var overlay = (IEditableCellOverlay)cell.CurrentOverlay;
		var terrain = overlay.Terrain;
		var texts = new List<(string Name, string Text)>();
		var (info, expression) = terrain != null && TerrainInfos.ContainsKey(terrain)
			? TerrainInfos[terrain]
			: (DefaultInfo, NumberOfRandomElements);
		var random = Convert.ToInt32(expression.Evaluate());
		var validElements = RandomDescriptionElements
							.Where(x => x.Applies(terrain, tags)).ToList();
		while (random > 0)
		{
			if (!validElements.Any())
			{
				break;
			}

			var element = validElements.GetWeightedRandom(x => x.Weight);
			texts.Add(element.TextForTags(terrain, tags));
			validElements.Remove(element);
			random--;
		}

		overlay.CellName = string
						   .Format(
							   texts.Where(x => !string.IsNullOrEmpty(x.Name)).GetRandomElement().Name ?? info.CellName,
							   info.CellName, terrain?.Name ?? "Unknown").TitleCase();
		var sb = new StringBuilder();
		if (!string.IsNullOrEmpty(info.CellDescription))
		{
			sb.Append(info.CellDescription);
		}

		foreach (var element in texts)
		{
			sb.Append(element.Text.LeadingSpaceIfNotEmpty().Fullstop());
		}

		overlay.CellDescription = sb.ToString();
		ApplyTagsToCell(cell, tags);
	}

	protected override string SubtypeHelpText => @"
Commands pertaining to the Default Room (used if no override is specified):

	roomname <name> - the name of the default rooms
	description <text> - the description of the default rooms
	light <%> - the light multiplier of default rooms
	defaultterrain <terrain> - the fallback terrain used if none is specified
	fp <which> - the foragable profile of default rooms
	fp none - removes the foragable profile from default rooms
	expression <expression> - an expression for the number of random sentences generated for the default room
	outdoors|cave|indoors|shelter - sets the behaviour type of the default room

Commands pertaining to specific terrain overrides of the default info:

	terrain <which> - views detailed info about the terrain override
	terrain <which> remove - removes a terrain override
	terrain <which> roomname <name> - the name of the room
	terrain <which> description <text> - the description of the room
	terrain <which> light <%> - the light multiplier of room
	terrain <which> fp <which> - the foragable profile of room
	terrain <which> fp none - removes the foragable profile from room
	terrain <which> expression <expression> - an expression for the number of random sentences generated for the room
	terrain <which> outdoors|cave|indoors|shelter - sets the behaviour type of the room

Commands pertaining to random description elements that get added to the base descriptions:

	element add <type> - adds a new element
	element <#> - views detailed information about an element
	element <#> ... - edits the properties of an element. See individual type helps
	element <#> remove - removes a description element";

	public override string Show(ICharacter builder)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Autobuilder Room Template #{Id} ({Name})".Colour(Telnet.Cyan));
		sb.AppendLine($"");
		sb.AppendLine(
			"This template combines randomly selected sentences as appropriate for the terrain and applied tags, to generate a random paragraph of text and a random room name.");
		sb.AppendLine();
		sb.AppendLine($"Default Room Properties:");
		sb.AppendLine();
		sb.AppendLine($"Terrain: {DefaultInfo.DefaultTerrain.Name.ColourValue()}");
		sb.AppendLine($"Name: {DefaultInfo.CellName.ColourCommand()}");
		sb.AppendLine($"Text: {DefaultInfo.CellDescription.ColourCommand()}");
		sb.AppendLine($"Light Multiplier: {DefaultInfo.AmbientLightFactor.ToString("P3", builder).ColourValue()}");
		sb.AppendLine($"Behaviour: {DefaultInfo.OutdoorsType.Describe().ColourValue()}");
		sb.AppendLine(
			$"Foragable Profile: {DefaultInfo.ForagableProfile?.Name.ColourValue() ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine($"No. Sentences Expression: {NumberOfRandomElements.OriginalExpression.ColourCommand()}");
		sb.AppendLine();
		sb.AppendLine("Has specific overrides of default for the following terrain types:");
		sb.AppendLine(TerrainInfos.Select(x => x.Key.Name.ColourValue()).ListToString());
		sb.AppendLine();
		sb.AppendLine($"Description Elements:");
		sb.AppendLine();
		var i = 1;
		foreach (var item in RandomDescriptionElements)
		{
			if (item is AutobuilderRandomDescriptionGroup rg)
			{
				sb.AppendLine(
					$"\t{i++.ToString("N0", builder)}) Group of {rg.SubElements.Count().ToString("N0", builder).ColourValue()} elements with tags {rg.Tags.Select(x => x.ColourCommand()).ListToString()}");
				continue;
			}

			if (item is AutobuilderRoadRandomDescriptionElement)
			{
				sb.AppendLine(
					$"\t{i++.ToString("N0", builder)}) Road Element with tags {item.Tags.Select(x => x.ColourCommand()).ListToString()}");
				continue;
			}

			sb.AppendLine(
				$"\t{i++.ToString("N0", builder)}) Simple Element with tags {item.Tags.Select(x => x.ColourCommand()).ListToString()}");
		}

		return sb.ToString();
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
				return BuildingCommandDefaultTerrain(actor, command);
			case "foragable":
			case "fp":
			case "forageable":
			case "foragableprofile":
			case "forageableprofile":
				return BuildingCommandForagable(actor, command);
			case "expression":
			case "count":
			case "quantity":
			case "number":
			case "elements":
			case "descriptions":
				return BuildingCommandExpression(actor, command);
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

			case "item":
			case "element":
			case "elem":
				return BuildingCommandElement(actor, command);

			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	private bool BuildingCommandExpression(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must enter an expression for the number of description elements.");
			return false;
		}

		var expression = new Expression(command.SafeRemainingArgument);
		if (expression.HasErrors())
		{
			actor.OutputHandler.Send(expression.Error);
			return false;
		}

		NumberOfRandomElements = expression;
		Changed = true;
		actor.OutputHandler.Send(
			$"The default rooms will now use the expression {expression.OriginalExpression.ColourCommand()} for the number of sentences.");
		return true;
	}

	private bool BuildingCommandElement(ICharacter actor, StringStack command)
	{
		var text = command.PopSpeech();
		if (text.EqualTo("add"))
		{
			if (command.IsFinished)
			{
				actor.OutputHandler.Send(
					$"What type of description element do you want to add? The valid options are {"simple".ColourCommand()} and {"road".ColourCommand()}");
				return false;
			}

			switch (command.PopSpeech().ToLowerInvariant())
			{
				case "group":
					RandomDescriptionElements.Add(new AutobuilderRandomDescriptionGroup(Gameworld));
					actor.OutputHandler.Send(
						$"You add a new simple description element at position {RandomDescriptionElements.Count.ToString("N0", actor).ColourValue()}.");
					Changed = true;
					return true;
				case "simple":
					RandomDescriptionElements.Add(new AutobuilderRandomDescriptionElement(Gameworld));
					actor.OutputHandler.Send(
						$"You add a new simple description element at position {RandomDescriptionElements.Count.ToString("N0", actor).ColourValue()}.");
					Changed = true;
					return true;
				case "road":
					RandomDescriptionElements.Add(new AutobuilderRoadRandomDescriptionElement(Gameworld));
					actor.OutputHandler.Send(
						$"You add a new road description element at position {RandomDescriptionElements.Count.ToString("N0", actor).ColourValue()}.");
					Changed = true;
					return true;
				default:
					actor.OutputHandler.Send(
						$"That is not a valid type of description element. The valid options are {"simple".ColourCommand()}, {"group".ColourCommand()} and {"road".ColourCommand()}");
					return false;
			}
		}

		if (!int.TryParse(text, out var value) || value < 1 || value > RandomDescriptionElements.Count)
		{
			actor.OutputHandler.Send(
				$"That is not a valid index for a description element. You must enter a number between {1.ToString("N0", actor).ColourValue()} and {RandomDescriptionElements.Count.ToString("N0", actor).ColourValue()}.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(RandomDescriptionElements[value - 1].Show(actor));
			return false;
		}

		if (command.PeekSpeech().EqualTo("remove"))
		{
			RandomDescriptionElements.RemoveAt(value - 1);
			Changed = true;
			actor.OutputHandler.Send($"You remove the {value.ToOrdinal()} description element.");
			return true;
		}

		if (RandomDescriptionElements[value - 1].BuildingCommand(actor, command))
		{
			Changed = true;
			return true;
		}

		return false;
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
				sb.AppendLine($"No. of Random Elements: {info.Item2.OriginalExpression.ColourCommand()}");
				sb.AppendLine($"Room Name: {info.Item1.CellName.Colour(Telnet.Cyan)}");
				sb.AppendLine($"Behaviour: {info.Item1.OutdoorsType.Describe().ColourValue()}");
				sb.AppendLine($"Light Multiplier: {info.Item1.AmbientLightFactor.ToString("P3", actor).ColourValue()}");
				sb.AppendLine(
					$"Foragable Profile: {info.Item1.ForagableProfile?.Name.ColourValue() ?? "None".Colour(Telnet.Red)}");
				sb.AppendLine("Description:");
				sb.AppendLine();
				sb.AppendLine(info.Item1.CellDescription.Wrap(actor.InnerLineFormatLength, "\t"));
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
			case "expression":
			case "count":
			case "quantity":
			case "number":
			case "elements":
			case "descriptions":
				return BuildingCommandTerrainExpression(actor, terrain, command);
		}

		actor.OutputHandler.Send(@"You must enter one of the following sub-commands after the terrain type:

	name <name> - sets the name of the rooms
	description - sets the room description
	light <percentage> - sets the light multiplier
	outdoors|cave|windows|indoors|shelter - changes the outdoors behaviour of the room
	foragable <which> - sets the foragable profile for the terrain
	foragable none - removes a foragable profile for the terrain
	expression <number expression> - an expression for the number of random sentences generated
	remove - removes a template type for a terrain");
		return false;
	}

	private bool BuildingCommandTerrainExpression(ICharacter actor, ITerrain terrain, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must enter an expression for the number of description elements.");
			return false;
		}

		var expression = new Expression(command.SafeRemainingArgument);
		if (expression.HasErrors())
		{
			actor.OutputHandler.Send(expression.Error);
			return false;
		}

		TerrainInfos[terrain] = (TerrainInfos[terrain].Item1, expression);
		Changed = true;
		actor.OutputHandler.Send(
			$"The {terrain.Name.ColourValue()} terrain will now use the expression {expression.OriginalExpression.ColourCommand()} for the number of sentences.");
		return true;
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

			TerrainInfos[terrain] = (TerrainInfos[terrain].Item1 with { ForagableProfile = null },
				TerrainInfos[terrain].Item2);
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
			TerrainInfos[terrain] = (TerrainInfos[terrain].Item1 with { ForagableProfile = fp },
				TerrainInfos[terrain].Item2);
		}
		else
		{
			TerrainInfos[terrain] = (new AutobuilderRoomInfo
			{
				DefaultTerrain = terrain,
				CellName = "An Undescribed Room",
				CellDescription = "An undescribed room.",
				OutdoorsType = CellOutdoorsType.Outdoors,
				AmbientLightFactor = 1.0,
				ForagableProfile = fp
			}, new Expression("2+1d2"));
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
			TerrainInfos[terrain] = (TerrainInfos[terrain].Item1 with { OutdoorsType = outdoors },
				TerrainInfos[terrain].Item2);
		}
		else
		{
			TerrainInfos[terrain] = (new AutobuilderRoomInfo
			{
				DefaultTerrain = terrain,
				CellName = "An Undescribed Room",
				CellDescription = "An undescribed room.",
				OutdoorsType = outdoors,
				AmbientLightFactor = 1.0
			}, new Expression("2+1d2"));
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
			TerrainInfos[terrain] = (TerrainInfos[terrain].Item1 with { AmbientLightFactor = percentage },
				TerrainInfos[terrain].Item2);
		}
		else
		{
			TerrainInfos[terrain] = (new AutobuilderRoomInfo
			{
				DefaultTerrain = terrain,
				CellName = "An Undescribed Room",
				CellDescription = "An undescribed room.",
				OutdoorsType = terrain.DefaultCellOutdoorsType,
				AmbientLightFactor = percentage
			}, new Expression("2+1d2"));
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
									 TerrainInfos[terrain].Item1.CellDescription
														  .Wrap(actor.InnerLineFormatLength, "\t"));
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
			TerrainInfos[terrain] = (
				TerrainInfos[terrain].Item1 with { CellDescription = arg1.Trim().ProperSentences().Fullstop() },
				TerrainInfos[terrain].Item2);
		}
		else
		{
			TerrainInfos[terrain] = (new AutobuilderRoomInfo
			{
				DefaultTerrain = terrain,
				CellName = "An Undescribed Room",
				CellDescription = arg1.Trim().ProperSentences().Fullstop(),
				OutdoorsType = terrain.DefaultCellOutdoorsType,
				AmbientLightFactor = 1.0
			}, new Expression("2+1d2"));
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
			TerrainInfos[terrain] = (
				TerrainInfos[terrain].Item1 with { CellName = command.SafeRemainingArgument.TitleCase() },
				TerrainInfos[terrain].Item2);
		}
		else
		{
			TerrainInfos[terrain] = (new AutobuilderRoomInfo
			{
				DefaultTerrain = terrain,
				CellName = command.SafeRemainingArgument.TitleCase(),
				CellDescription = "An undescribed room.",
				OutdoorsType = terrain.DefaultCellOutdoorsType,
				AmbientLightFactor = 1.0
			}, new Expression("2+1d2"));
		}

		Changed = true;
		actor.OutputHandler.Send(
			$"The rooms of the {terrain.Name.ColourValue()} type will now use the room name {TerrainInfos[terrain].Item1.CellName.Colour(Telnet.Cyan)}.");
		return true;
	}
}