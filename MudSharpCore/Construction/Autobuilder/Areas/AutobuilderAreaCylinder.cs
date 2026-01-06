using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Database;
using MudSharp.Framework;

namespace MudSharp.Construction.Autobuilder.Areas;

public class AutobuilderAreaCylinder : AutobuilderAreaBase
{
	public static void RegisterAutobuilderLoader()
	{
		AutobuilderFactory.RegisterLoader("cylinder",
			(area, gameworld) => new AutobuilderAreaCylinder(area, gameworld));
		AutobuilderFactory.RegisterBuilderLoader("cylinder",
			(gameworld, name) => new AutobuilderAreaCylinder(name, gameworld));
	}

	public bool ConnectCellsWithDiagonalExits { get; private set; }

	public bool ConnectRingsAlongLengthOfCylinder { get; private set; }

	protected AutobuilderAreaCylinder(string name, IFuturemud gameworld, string type = null) : base(name, gameworld,
		type ?? "cylinder")
	{
	}

	protected AutobuilderAreaCylinder(Models.AutobuilderAreaTemplate area, IFuturemud gameworld) : base(area,
		gameworld)
	{
	}

	protected override void LoadFromXml(XElement element)
	{
		ShowCommandByLine = element.Element("ShowCommandByLine")?.Value ??
							"An undescribed autobuilder area template";
		ConnectCellsWithDiagonalExits = bool.Parse(element.Element("ConnectCellsWithDiagonalExits")?.Value ?? "false");
		ConnectRingsAlongLengthOfCylinder = bool.Parse(element.Element("ConnectRingsAlongLengthOfCylinder")?.Value ?? "false");
	}

	protected override XElement SaveToXml()
	{
		return new XElement("Template",
			new XElement("ShowCommandByLine", new XCData(ShowCommandByLine)),
			new XElement("ConnectCellsWithDiagonalExits", ConnectCellsWithDiagonalExits),
			new XElement("ConnectRingsAlongLengthOfCylinder", ConnectRingsAlongLengthOfCylinder)
		);
	}

	public override string SubtypeHelpText => @"
	#3diagonals#0 - toggles diagonals being linked up
	#3link#0 - toggles rings along the cylinder being linked up";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "diagonals":
				ConnectCellsWithDiagonalExits = !ConnectCellsWithDiagonalExits;
				actor.OutputHandler.Send($"This autobuilder area template will {(ConnectCellsWithDiagonalExits ? "now" : "no longer")} connect cells with diagonal exits.");
				return true;
			case "link":
				ConnectRingsAlongLengthOfCylinder = !ConnectRingsAlongLengthOfCylinder;
				actor.OutputHandler.Send($"This autobuilder area template will {(ConnectRingsAlongLengthOfCylinder ? "now" : "no longer")} connect cells along the length of the cylinder.");
				return true;
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}

	}

	protected override void SetupParameters()
	{
		_parameters.Add(
			new AutobuilderIntegerParameter(
				"circumfrence", "You must specify a circumfrence for the ring that you want to build", false)
			{
				MinimumValue = 4,
				MaximumValue = 100
			});
		_parameters.Add(
			new AutobuilderIntegerParameter(
				"length", "You must specify a length for the cylinder of rings that you want to build", false)
			{
				MinimumValue = 1,
				MaximumValue = 100
			});
		_parameters.Add(
		   new AutobuilderDirectionParameter(
			   "direction", "You must specify a direction for the cylinder to be oriented in (rings will be at 90/270 degrees to this)", false)
		   {
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
		var circumfrence = (int)argList.ElementAt(0);
		var length = (int)argList.ElementAt(1);
		var conedirection = (CardinalDirection)argList.ElementAt(2);
		var clockwisedirection = conedirection.Rotate90Clockwise();
		var counterclockwisedirection = conedirection.Rotate90CounterClockwise();
		var roomTemplate = (IAutobuilderRoom)argList.ElementAt(3);
		var terrain = argList.ElementAtOrDefault(4) as Terrain;

		CardinalDirection CombineDirections(CardinalDirection primary, CardinalDirection secondary)
		{
			var northness = primary.Northness() + secondary.Northness();
			var southness = primary.Southness() + secondary.Southness();
			var eastness = primary.Eastness() + secondary.Eastness();
			var westness = primary.Westness() + secondary.Westness();
			var upness = primary.Upness() + secondary.Upness();
			var downness = primary.Downness() + secondary.Downness();
			var north = northness - southness;
			var east = eastness - westness;
			var up = upness - downness;

			if (up > 0)
			{
				return CardinalDirection.Up;
			}

			if (up < 0)
			{
				return CardinalDirection.Down;
			}

			if (north > 0 && east > 0)
			{
				return CardinalDirection.NorthEast;
			}

			if (north > 0 && east < 0)
			{
				return CardinalDirection.NorthWest;
			}

			if (north < 0 && east > 0)
			{
				return CardinalDirection.SouthEast;
			}

			if (north < 0 && east < 0)
			{
				return CardinalDirection.SouthWest;
			}

			if (north > 0)
			{
				return CardinalDirection.North;
			}

			if (north < 0)
			{
				return CardinalDirection.South;
			}

			if (east > 0)
			{
				return CardinalDirection.East;
			}

			if (east < 0)
			{
				return CardinalDirection.West;
			}

			return CardinalDirection.Unknown;
		}

		var coneOpposite = conedirection.Opposite();
		var clockwiseDiagonalFromPrevious = CombineDirections(coneOpposite, clockwisedirection);
		var counterClockwiseDiagonalFromPrevious = CombineDirections(coneOpposite, counterclockwisedirection);

		var cells = new ICell[length, circumfrence];
		for (var i = 0; i < length; i++)
		{
			for (var j = 0; j < circumfrence; j++)
			{
				var cell = roomTemplate.CreateRoom(builder, terrain, false);
				cells[i, j] = cell;

				if (j > 0)
				{
					var exit = new Exit(builder.Gameworld, cell, cells[i, j - 1], counterclockwisedirection,
						clockwisedirection, 1.0);
					cell.GetOrCreateOverlay(package).AddExit(exit);
					cells[i, j - 1].GetOrCreateOverlay(package).AddExit(exit);
				}

				if (i > 0 && (j == 0 || ConnectRingsAlongLengthOfCylinder))
				{
					var exit = new Exit(builder.Gameworld, cell, cells[i - 1, j], coneOpposite, conedirection, 1.0);
					cell.GetOrCreateOverlay(package).AddExit(exit);
					cells[i - 1, j].GetOrCreateOverlay(package).AddExit(exit);
				}

				if (i > 0 && ConnectRingsAlongLengthOfCylinder && ConnectCellsWithDiagonalExits)
				{
					var clockwiseIndex = (j + 1) % circumfrence;
					var counterClockwiseIndex = (j - 1 + circumfrence) % circumfrence;

					var exit = new Exit(builder.Gameworld, cell, cells[i - 1, clockwiseIndex],
						clockwiseDiagonalFromPrevious, clockwiseDiagonalFromPrevious.Opposite(), 1.0);
					cell.GetOrCreateOverlay(package).AddExit(exit);
					cells[i - 1, clockwiseIndex].GetOrCreateOverlay(package).AddExit(exit);

					exit = new Exit(builder.Gameworld, cell, cells[i - 1, counterClockwiseIndex],
						counterClockwiseDiagonalFromPrevious, counterClockwiseDiagonalFromPrevious.Opposite(), 1.0);
					cell.GetOrCreateOverlay(package).AddExit(exit);
					cells[i - 1, counterClockwiseIndex].GetOrCreateOverlay(package).AddExit(exit);
				}
			}

			var ringExit = new Exit(builder.Gameworld, cells[i, 0], cells[i, circumfrence - 1],
				counterclockwisedirection, clockwisedirection, 1.0);
			cells[i, 0].GetOrCreateOverlay(package).AddExit(ringExit);
			cells[i, circumfrence - 1].GetOrCreateOverlay(package).AddExit(ringExit);
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
			$@"{$"Autobuilder Area Template #{Id} ({Name})".Colour(Telnet.Cyan)}

This autobuilder template will return a cylinder area of linked cells (like the inside of a ring world) with circumfrence, length, orientation, terrain and room template supplied by the builder.

It {(ConnectRingsAlongLengthOfCylinder ? "will".Colour(Telnet.Green) : "will not".Colour(Telnet.Red))} connect the rooms on the ring along the length.
It {(ConnectCellsWithDiagonalExits ? "will".Colour(Telnet.Green) : "will not".Colour(Telnet.Red))} connect the rooms in diagonals.";
	}

	public override IAutobuilderArea Clone(string newName)
	{
		using (new FMDB())
		{
			var dbitem = new Models.AutobuilderAreaTemplate
			{
				Name = newName,
				Definition = SaveToXml().ToString(),
				TemplateType = "cylinder"
			};
			FMDB.Context.AutobuilderAreaTemplates.Add(dbitem);
			FMDB.Context.SaveChanges();
			return new AutobuilderAreaCylinder(dbitem, Gameworld);
		}
	}
}
