using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Database;
using MudSharp.Framework;

namespace MudSharp.Construction.Autobuilder.Areas;

public class AutobuilderAreaRectangle : AutobuilderAreaBase
{
	public static void RegisterAutobuilderLoader()
	{
		AutobuilderFactory.RegisterLoader("rectangle",
			(area, gameworld) => new AutobuilderAreaRectangle(area, gameworld));
		AutobuilderFactory.RegisterBuilderLoader("rectangle",
			(gameworld, name) => new AutobuilderAreaRectangle(name, gameworld));
	}

	public virtual bool ConnectCellsWithDiagonalExits => false;

	protected AutobuilderAreaRectangle(string name, IFuturemud gameworld, string type = null) : base(name, gameworld,
		type ?? "rectangle")
	{
	}

	protected AutobuilderAreaRectangle(Models.AutobuilderAreaTemplate area, IFuturemud gameworld) : base(area,
		gameworld)
	{
	}

	protected override void LoadFromXml(XElement element)
	{
		ShowCommandByLine = element.Element("ShowCommandByLine")?.Value ??
		                    "An undescribed autobuilder area template";
	}

	protected override XElement SaveToXml()
	{
		return new XElement("Template",
			new XElement("ShowCommandByLine", new XCData(ShowCommandByLine))
		);
	}

	public override string SubtypeHelpText => "";

	protected override void SetupParameters()
	{
		_parameters.Add(
			new AutobuilderIntegerParameter(
				"height", "You must specify a height for the rectangle that you want to build", false)
			{
				MinimumValue = 1,
				MaximumValue = 100
			});
		_parameters.Add(
			new AutobuilderIntegerParameter(
				"width", "You must specify a width for the rectangle that you want to build", false)
			{
				MinimumValue = 1,
				MaximumValue = 100
			});
		_parameters.Add(new AutobuilderRoomTemplateParameter("room template",
			"You must specify a valid autobuilder room template to use.",
			false, Gameworld));
		_parameters.Add(new AutobuilderTerrainParameter("terrain type", "", true, Gameworld));
	}

	public override IEnumerable<ICell> ExecuteTemplate(ICharacter builder, IEnumerable<object> arguments)
	{
		var package = builder.CurrentOverlayPackage;
		var argList = arguments.ToList();
		var height = (int)argList.ElementAt(0);
		var width = (int)argList.ElementAt(1);
		var roomTemplate = (IAutobuilderRoom)argList.ElementAt(2);
		var terrain = argList.ElementAtOrDefault(3) as Terrain;

		var cells = new ICell[width, height];
		for (var i = 0; i < width; i++)
		for (var j = 0; j < height; j++)
		{
			var cell = roomTemplate.CreateRoom(builder, terrain, false);
			cells[i, j] = cell;

			// Setup the cell exits in the rectangle
			if (i > 0)
			{
				var exit = new Exit(builder.Gameworld, cell, cells[i - 1, j], CardinalDirection.West,
					CardinalDirection.East, 1.0);
				cell.GetOrCreateOverlay(package).AddExit(exit);
				cells[i - 1, j].GetOrCreateOverlay(package).AddExit(exit);

				if (j > 0 && ConnectCellsWithDiagonalExits)
				{
					exit = new Exit(builder.Gameworld, cell, cells[i - 1, j - 1], CardinalDirection.SouthWest,
						CardinalDirection.NorthEast, 1.0);
					cells[i - 1, j - 1].GetOrCreateOverlay(package).AddExit(exit);
					cell.GetOrCreateOverlay(package).AddExit(exit);
					if (i < width - 1)
					{
						exit = new Exit(builder.Gameworld, cell, cells[i + 1, j - 1], CardinalDirection.SouthEast,
							CardinalDirection.NorthWest, 1.0);
						cell.GetOrCreateOverlay(package).AddExit(exit);
						cells[i + 1, j - 1].GetOrCreateOverlay(package).AddExit(exit);
					}
				}
			}

			if (j > 0)
			{
				var exit = new Exit(builder.Gameworld, cell, cells[i, j - 1], CardinalDirection.South,
					CardinalDirection.North, 1.0);
				cell.GetOrCreateOverlay(package).AddExit(exit);
				cells[i, j - 1].GetOrCreateOverlay(package).AddExit(exit);
			}
		}

		foreach (var cell in cells)
		{
			builder.Gameworld.ExitManager.UpdateCellOverlayExits(cell, cell.CurrentOverlay);
		}

		return cells.Cast<ICell>().ToList();
	}

	public override string Show(ICharacter builder)
	{
		return
			$"{$"Autobuilder Area Template #{Id} ({Name})".Colour(Telnet.Cyan)}\n\nThis autobuilder template will return a rectangular area of linked cells with height, width, terrain and room template supplied by the builder.";
	}

	public override IAutobuilderArea Clone(string newName)
	{
		using (new FMDB())
		{
			var dbitem = new Models.AutobuilderAreaTemplate
			{
				Name = newName,
				Definition = SaveToXml().ToString(),
				TemplateType = "rectangle"
			};
			FMDB.Context.AutobuilderAreaTemplates.Add(dbitem);
			FMDB.Context.SaveChanges();
			return new AutobuilderAreaRectangle(dbitem, Gameworld);
		}
	}
}