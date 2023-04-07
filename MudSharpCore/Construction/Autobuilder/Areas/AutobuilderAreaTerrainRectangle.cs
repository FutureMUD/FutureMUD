using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Database;
using MudSharp.Framework;

namespace MudSharp.Construction.Autobuilder.Areas;

public class AutobuilderAreaTerrainRectangle : AutobuilderAreaBase
{
	public static void RegisterAutobuilderLoader()
	{
		AutobuilderFactory.RegisterLoader("terrain rectangle",
			(area, gameworld) => new AutobuilderAreaTerrainRectangle(area, gameworld));
		AutobuilderFactory.RegisterBuilderLoader("terrain rectangle",
			(gameworld, name) => new AutobuilderAreaTerrainRectangle(name, gameworld));
	}

	public override IAutobuilderArea Clone(string newName)
	{
		using (new FMDB())
		{
			var dbitem = new Models.AutobuilderAreaTemplate
			{
				Name = newName,
				Definition = SaveToXml().ToString(),
				TemplateType = "terrain rectangle"
			};
			FMDB.Context.AutobuilderAreaTemplates.Add(dbitem);
			FMDB.Context.SaveChanges();
			return new AutobuilderAreaTerrainRectangle(dbitem, Gameworld);
		}
	}

	public bool ConnectCellsWithDiagonalExits { get; protected set; }

	protected AutobuilderAreaTerrainRectangle(string name, IFuturemud gameworld, string type = null) : base(name,
		gameworld, type ?? "terrain rectangle")
	{
	}

	protected AutobuilderAreaTerrainRectangle(Models.AutobuilderAreaTemplate area, IFuturemud gameworld) : base(area,
		gameworld)
	{
	}

	protected override void LoadFromXml(XElement element)
	{
		ConnectCellsWithDiagonalExits = bool.Parse(element.Attribute("connect_diagonals")?.Value ?? "false");
		ShowCommandByLine = element.Element("ShowCommandByLine")?.Value ??
		                    "An undescribed autobuilder area template";
	}

	protected override XElement SaveToXml()
	{
		return new XElement("Template",
			new XAttribute("connect_diagonals", ConnectCellsWithDiagonalExits),
			new XElement("ShowCommandByLine", new XCData(ShowCommandByLine))
		);
	}

	protected override void SetupParameters()
	{
		_parameters.Add(
			new AutobuilderIntegerParameter(
				"height", "You must specify a height for the rectangle that you want to build", false)
			{
				MinimumValue = 1,
				MaximumValue = 50
			});
		_parameters.Add(
			new AutobuilderIntegerParameter(
				"width", "You must specify a width for the rectangle that you want to build", false)
			{
				MinimumValue = 1,
				MaximumValue = 50
			});
		_parameters.Add(new AutobuilderRoomTemplateParameter("room template",
			"You must specify a valid autobuilder room template to use.",
			false, Gameworld));
		_parameters.Add(new AutobuilderCustomParameter
		{
			Gameworld = Gameworld,
			IsOptional = false,
			ParameterName = "mask",
			MissingErrorMessage =
				"You must enter a mask of terrain ids, separated by commas, starting from the top left corner of the rectangle and proceeding right and down.",
			TypeName = "terrain mask",
			IsValidArgumentFunction = (arg, game, args) =>
			{
				var height = (int)args[0];
				var width = (int)args[1];
				var split = arg.Split(',');
				if (split.Length != height * width)
				{
					return false;
				}

				if (split.Any(x => !x.IsInteger()))
				{
					return false;
				}

				if (split.Any(x => x != "0" && !game.Terrains.Has(long.Parse(x))))
				{
					return false;
				}

				return true;
			},
			WhyIsNotValidArgumentFunction = (arg, game, args) =>
			{
				var height = (int)args[0];
				var width = (int)args[1];
				var split = arg.Split(',');
				if (split.Length != height * width)
				{
					return "The terrain mask must exactly match the size of the grid.";
				}

				if (split.Any(x => !x.IsInteger()))
				{
					return "There are parts of the terrain mask that are not valid integers.";
				}

				if (split.Any(x => x != "0" && !game.Terrains.Has(long.Parse(x))))
				{
					return "There are parts of the terrain mask that don't translate to valid terrain types.";
				}

				return "";
			},
			GetArgumentFunction = (arg, game) =>
			{
				return arg.Split(',').Select(x => game.Terrains.Get(long.Parse(x))).ToArray();
			}
		});
	}

	public override IEnumerable<ICell> ExecuteTemplate(ICharacter builder, IEnumerable<object> arguments)
	{
		var package = builder.CurrentOverlayPackage;
		var argList = arguments.ToList();
		var height = (int)argList.ElementAt(0);
		var width = (int)argList.ElementAt(1);
		var roomTemplate = (IAutobuilderRoom)argList.ElementAt(2);
		var terrainArg = (ITerrain[])argList.ElementAt(3);

		var terrains = new ITerrain[width, height];
		int x = 0, y = 0;
		foreach (var terrain in terrainArg)
		{
			terrains[x, y] = terrain;
			if (++x == width)
			{
				x = 0;
				y++;
			}
		}

		var cells = new ICell[width, height];
		for (var i = 0; i < width; i++)
		for (var j = 0; j < height; j++)
		{
			if (terrains[i, j] == null)
			{
				continue;
			}

			var cell = roomTemplate.CreateRoom(builder, terrains[i, j], false);
			cells[i, j] = cell;

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

		return cells.OfType<ICell>().ToList();
	}

	public override string Show(ICharacter builder)
	{
		return
			$"{$"Autobuilder Area Template #{Id} ({Name})".Colour(Telnet.Cyan)}\n\n{$"This autobuilder template will return a rectangular area of cells with height, width, terrain and room template supplied by the builder. It {(ConnectCellsWithDiagonalExits ? "does" : "does not")} link diagonally between rooms.".Wrap(builder.InnerLineFormatLength)}";
	}

	public override string SubtypeHelpText => @"
	diagonals - toggles whether diagonals will be linked";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant().CollapseString())
		{
			case "diagonal":
			case "diagonals":
				return BuildingCommandDiagonals(actor);
		}

		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandDiagonals(ICharacter actor)
	{
		ConnectCellsWithDiagonalExits = !ConnectCellsWithDiagonalExits;
		Changed = true;
		actor.OutputHandler.Send(
			$"This template will {(ConnectCellsWithDiagonalExits ? "now" : "no longer")} connect rooms diagonally.");
		return true;
	}
}