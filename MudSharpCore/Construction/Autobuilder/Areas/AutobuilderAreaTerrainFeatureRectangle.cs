using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Database;

namespace MudSharp.Construction.Autobuilder.Areas;

public class AutobuilderAreaTerrainFeatureRectangle : AutobuilderAreaTerrainRectangle
{
	public new static void RegisterAutobuilderLoader()
	{
		AutobuilderFactory.RegisterLoader("terrain feature rectangle",
			(area, gameworld) => new AutobuilderAreaTerrainFeatureRectangle(area, gameworld));
		AutobuilderFactory.RegisterBuilderLoader("terrain feature rectangle",
			(gameworld, name) => new AutobuilderAreaTerrainFeatureRectangle(name, gameworld));
	}

	protected AutobuilderAreaTerrainFeatureRectangle(string name, IFuturemud gameworld, string type = null) : base(name,
		gameworld, type ?? "terrain feature rectangle")
	{
	}

	protected AutobuilderAreaTerrainFeatureRectangle(Models.AutobuilderAreaTemplate area, IFuturemud gameworld) : base(
		area, gameworld)
	{
	}

	protected override void SetupParameters()
	{
		base.SetupParameters();
		_parameters.Add(new AutobuilderCustomParameter
		{
			Gameworld = Gameworld,
			IsOptional = false,
			ParameterName = "featuresmask",
			MissingErrorMessage =
				"You must enter a mask of features for each location, separated by vertical lines (|) within the location and commas between the locations, starting from the top left corner of the rectangle and proceeding right and down.",
			TypeName = "feature mask",
			IsValidArgumentFunction = (arg, game, args) =>
			{
				var height = (int)args[0];
				var width = (int)args[1];
				var split = arg.Split(',');
				if (split.Length != height * width)
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
					return "The feature mask must exactly match the size of the grid.";
				}

				return "";
			},
			GetArgumentFunction = (arg, game) => { return arg.Split(',').Select(x => x.Split('|')).ToArray(); }
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
		var featureArg = (string[][])argList.ElementAt(4);

		var terrains = new ITerrain[width, height];
		var features = new string[width, height][];
		int x = 0, y = 0;
		for (var i = 0; i < terrainArg.Length; i++)
		{
			terrains[x, y] = terrainArg[i];
			features[x, y] = featureArg[i];
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

			var cell = roomTemplate.CreateRoom(builder, terrains[i, j], false, features[i, j]);
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
			$"{$"Autobuilder Area Template #{Id} ({Name})".Colour(Telnet.Cyan)}\n\n{$"This autobuilder template will return a rectangular area of cells with height, width, terrain, room features and room template supplied by the builder. It also requires the builder to specify a matching mask of tags to be applied to the generated rooms. This template {(ConnectCellsWithDiagonalExits ? "does" : "does not")} connect rooms diagonally.".Wrap(builder.InnerLineFormatLength)}";
	}

	public override IAutobuilderArea Clone(string newName)
	{
		using (new FMDB())
		{
			var dbitem = new Models.AutobuilderAreaTemplate
			{
				Name = newName,
				Definition = SaveToXml().ToString(),
				TemplateType = "terrain feature rectangle"
			};
			FMDB.Context.AutobuilderAreaTemplates.Add(dbitem);
			FMDB.Context.SaveChanges();
			return new AutobuilderAreaTerrainFeatureRectangle(dbitem, Gameworld);
		}
	}
}