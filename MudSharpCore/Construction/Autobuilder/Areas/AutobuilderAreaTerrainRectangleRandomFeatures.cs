using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Character;
using MudSharp.Commands.Trees;
using MudSharp.Construction.Autobuilder.Areas.Features;
using MudSharp.Construction.Boundary;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;

namespace MudSharp.Construction.Autobuilder.Areas;

public class AutobuilderAreaTerrainRectangleRandomFeatures : AutobuilderAreaTerrainRectangle
{
	public new static void RegisterAutobuilderLoader()
	{
		AutobuilderFactory.RegisterLoader("room by terrain random features",
			(room, gameworld) => new AutobuilderAreaTerrainRectangleRandomFeatures(room, gameworld));
		AutobuilderFactory.RegisterBuilderLoader("random features",
			(gameworld, name) => new AutobuilderAreaTerrainRectangleRandomFeatures(name, gameworld));
	}

	public override IAutobuilderArea Clone(string newName)
	{
		using (new FMDB())
		{
			var dbitem = new Models.AutobuilderAreaTemplate
			{
				Name = newName,
				Definition = SaveToXml().ToString(),
				TemplateType = "room by terrain random features"
			};
			FMDB.Context.AutobuilderAreaTemplates.Add(dbitem);
			FMDB.Context.SaveChanges();
			return new AutobuilderAreaTerrainRectangleRandomFeatures(dbitem, Gameworld);
		}
	}

	protected AutobuilderAreaTerrainRectangleRandomFeatures(string name, IFuturemud gameworld, string type = null) :
		base(name, gameworld, type ?? "room by terrain random features")
	{
	}

	protected AutobuilderAreaTerrainRectangleRandomFeatures(Models.AutobuilderAreaTemplate area, IFuturemud gameworld) :
		base(area, gameworld)
	{
	}

	protected override void LoadFromXml(XElement element)
	{
		base.LoadFromXml(element);
		var groupElement = element.Element("Groups");
		if (groupElement == null)
		{
			throw new ApplicationException(
				$"AutobuilderAreaTerrainRectangleRandomFeatures ID {Id} had no Groups element.");
		}

		try
		{
			foreach (var sub in groupElement.Elements())
			{
				switch (sub.Attribute("type")?.Value.ToLowerInvariant() ?? "none")
				{
					case "road":
						TerrainFeatureGroups.Add(new RoadFeatureGroup(sub, Gameworld));
						break;
					case "river":
						throw new NotImplementedException();
					case "uniform":
						TerrainFeatureGroups.Add(new UniformFeatureGroup(sub, Gameworld));
						break;
					case "none":
					default:
						TerrainFeatureGroups.Add(new SimpleFeatureGroup(sub, Gameworld));
						break;
				}
			}
		}
		catch (ApplicationException e)
		{
			throw new ApplicationException(
				$"AutobuilderAreaTerrainRectangleRandomFeatures ID {Id} encountered an application exception:\n\n{e.Message}");
		}
	}

	protected override XElement SaveToXml()
	{
		var @base = base.SaveToXml();
		var groups = new XElement("Groups");
		foreach (var item in TerrainFeatureGroups)
		{
			groups.Add(item.SaveToXml());
		}

		@base.Add(groups);
		return @base;
	}

	public List<TerrainFeatureGroup> TerrainFeatureGroups { get; } = new();

	public override IEnumerable<ICell> ExecuteTemplate(ICharacter builder, IEnumerable<object> arguments)
	{
		var package = builder.CurrentOverlayPackage;
		var argList = arguments.ToList();
		var height = (int)argList.ElementAt(0);
		var width = (int)argList.ElementAt(1);
		var roomTemplate = (IAutobuilderRoom)argList.ElementAt(2);
		var terrainArg = (ITerrain[])argList.ElementAt(3);

		var terrains = new ITerrain[width, height];
		var features = new List<string>[width, height];
		var lookup = new Dictionary<ICell, (int X, int Y)>();
		int x = 0, y = 0;
		foreach (var terrain in terrainArg)
		{
			terrains[x, y] = terrain;
			features[x, y] = new List<string>();
			if (++x == width)
			{
				x = 0;
				y++;
			}
		}

		builder.OutputHandler.PrioritySend("Initialising the cells and exits...");
		var cells = new ICell[width, height];
		for (var i = 0; i < width; i++)
		for (var j = 0; j < height; j++)
		{
			if (terrains[i, j] == null)
			{
				continue;
			}

			var cell = roomTemplate.CreateRoom(builder, terrains[i, j], true);
			cells[i, j] = cell;
			lookup[cell] = (i, j);

			// Setup the cell exits in the rectangle
			if (i > 0)
			{
				Exit exit;
				if (cells[i - 1, j] != null)
				{
					exit = new Exit(builder.Gameworld, cell, cells[i - 1, j], CardinalDirection.West,
						CardinalDirection.East, 1.0);
					cell.GetOrCreateOverlay(package).AddExit(exit);
					cells[i - 1, j].GetOrCreateOverlay(package).AddExit(exit);
				}

				if (j > 0 && ConnectCellsWithDiagonalExits)
				{
					if (cells[i - 1, j - 1] != null)
					{
						exit = new Exit(builder.Gameworld, cell, cells[i - 1, j - 1],
							CardinalDirection.NorthWest,
							CardinalDirection.SouthEast, 1.0);
						cells[i - 1, j - 1].GetOrCreateOverlay(package).AddExit(exit);
						cell.GetOrCreateOverlay(package).AddExit(exit);
					}

					if (i < width - 1)
					{
						if (cells[i + 1, j - 1] != null)
						{
							exit = new Exit(builder.Gameworld, cell, cells[i + 1, j - 1],
								CardinalDirection.NorthEast,
								CardinalDirection.SouthWest, 1.0);
							cell.GetOrCreateOverlay(package).AddExit(exit);
							cells[i + 1, j - 1].GetOrCreateOverlay(package).AddExit(exit);
						}
					}
				}
			}

			if (j > 0)
			{
				if (cells[i, j - 1] != null)
				{
					var exit = new Exit(builder.Gameworld, cell, cells[i, j - 1], CardinalDirection.North,
						CardinalDirection.South, 1.0);
					cell.GetOrCreateOverlay(package).AddExit(exit);
					cells[i, j - 1].GetOrCreateOverlay(package).AddExit(exit);
				}
			}
		}

		foreach (var cell in cells)
		{
			if (cell == null)
			{
				continue;
			}

			builder.Gameworld.ExitManager.UpdateCellOverlayExits(cell, cell.CurrentOverlay);
		}

		builder.OutputHandler.PrioritySend("Applying terrain feature groups...");
		foreach (var group in TerrainFeatureGroups)
		{
			group.ApplyTerrainFeatures(cells, features);
		}
#if DEBUG
		var count = 0;
		for (var i = 0; i < width; i++)
		for (var j = 0; j < height; j++)
		{
			count += features[i, j].Count;
		}

		builder.OutputHandler.Send($"Applied {count.ToString("N0", builder).ColourValue()} features in total.");
#endif

		builder.OutputHandler.PrioritySend("Describing the cells...");
		foreach (var cell in cells)
		{
			(x, y) = lookup[cell];
			roomTemplate.RedescribeRoom(cell, features[x, y].ToArray());
		}

		return cells.OfType<ICell>().ToList();
	}

	public override string Show(ICharacter builder)
	{
		var sb = new StringBuilder();
		sb.AppendLine(
			$"{$"Autobuilder Area Template #{Id} ({Name})".Colour(Telnet.Cyan)}\n\n");
		sb.AppendLine(
			$"This autobuilder template will return a rectangular area of cells with height, width, terrain and room template supplied by the builder. It {(ConnectCellsWithDiagonalExits ? "does" : "does not")} link diagonally between rooms."
				.Wrap(builder.InnerLineFormatLength));
		sb.AppendLine();
		sb.AppendLine(
			$"Additionally, it will sprinkle the generated area with random \"features\" which can be picked up by the room templates to guide the variable description that they created, and can also apply framework tags automatically if there is one with a matching name."
				.Wrap(builder.InnerLineFormatLength));
		sb.AppendLine();
		sb.AppendLine($"Feature Groups:");
		sb.AppendLine();
		foreach (var group in TerrainFeatureGroups)
		{
			sb.AppendLine($"\t{group.StringForParentShow(builder)}");
		}

		return sb.ToString();
	}

	public override string SubtypeHelpText => @"
	group add <type> <name> - adds a new feature group
	group remove <name> - removes a feature group
	group <name> <...> - edits the properties of a group (see individual group type helps)";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "group":
				return BuildingCommandGroup(actor, command);
		}

		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandGroup(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "add":
			case "new":
			case "create":
				return BuildingCommandGroupAdd(actor, command);
			case "delete":
			case "del":
			case "remove":
			case "rem":
				return BuildingCommandGroupRemove(actor, command);
		}

		var group = TerrainFeatureGroups.FirstOrDefault(x => x.Name.EqualTo(command.Last)) ??
		            TerrainFeatureGroups.FirstOrDefault(x => x.Name.StartsWith(command.Last));
		if (group == null)
		{
			actor.OutputHandler.Send(
				$"You must either use the keywords {"add".ColourCommand()} or {"remove".ColourCommand()}, or specify the name of a group you want to view or edit.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(group.Show(actor));
			return true;
		}

		return group.BuildingCommand(actor, this, command);
	}

	private bool BuildingCommandGroupRemove(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which terrain feature group do you want to remove?");
			return false;
		}

		var text = command.SafeRemainingArgument;
		var group = TerrainFeatureGroups.FirstOrDefault(x => x.Name.EqualTo(text)) ??
		            TerrainFeatureGroups.FirstOrDefault(x => x.Name.StartsWith(text));
		if (group == null)
		{
			actor.OutputHandler.Send($"There is no such terrain feature group.");
			return false;
		}

		actor.OutputHandler.Send(
			$"Are you sure you want to remove the terrain feature group called {group.Name.ColourName()}?\n{Accept.StandardAcceptPhrasing}");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			AcceptAction = msg =>
			{
				actor.OutputHandler.Send($"You delete the {group.Name.ColourName()} terrain feature group.");
				TerrainFeatureGroups.Remove(group);
				Changed = true;
			},
			RejectAction = msg =>
			{
				actor.OutputHandler.Send(
					$"You decide not to delete the {group.Name.ColourName()} terrain feature group.");
			},
			ExpireAction = () =>
			{
				actor.OutputHandler.Send(
					$"You decide not to delete the {group.Name.ColourName()} terrain feature group.");
			},
			DescriptionString = $"Deleting the {group.Name.ColourName()} terrain feature group",
			Keywords = new List<string> { "group", "delete" }
		}));
		return true;
	}

	private bool BuildingCommandGroupAdd(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What type of terrain feature group would you like to create? The valid types are {"simple".ColourName()}, {"uniform".ColourName()}, and {"road".ColourName()}.");
			return false;
		}

		var typeName = command.PopSpeech();
		switch (typeName.ToLowerInvariant())
		{
			case "simple":
			case "uniform":
			case "road":
				break;
			default:
				actor.OutputHandler.Send(
					$"That is not a valid terrain feature group type. The valid types are {"simple".ColourName()}, {"uniform".ColourName()}, and {"road".ColourName()}.");
				return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give this terrain feature group?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (TerrainFeatureGroups.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				"There is already a terrain feature group with that name. Names must be unique.");
			return false;
		}

		switch (typeName.ToLowerInvariant())
		{
			case "simple":
				TerrainFeatureGroups.Add(new SimpleFeatureGroup() { Name = name });
				actor.OutputHandler.Send($"You add a new simple feature group called {name.ColourName()}.");
				break;
			case "uniform":
				TerrainFeatureGroups.Add(new UniformFeatureGroup() { Name = name });
				actor.OutputHandler.Send($"You add a new uniform feature group called {name.ColourName()}.");
				break;
			case "road":
				TerrainFeatureGroups.Add(new RoadFeatureGroup() { Name = name });
				actor.OutputHandler.Send($"You add a new road feature group called {name.ColourName()}.");
				break;
		}

		Changed = true;
		return true;
	}
}