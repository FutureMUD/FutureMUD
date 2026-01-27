using System;
using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Construction;
using System.Xml.Linq;
using System.Linq;
using System.Text;
using Org.BouncyCastle.Asn1.Pkcs;
using MudSharp.Commands.Modules;

namespace MudSharp.RPG.AIStorytellers;

public class AIStorytellerSurveillanceStrategy : IAIStorytellerSurveillanceStrategy
{
	public AIStorytellerSurveillanceStrategy(IFuturemud gameworld, string surveillanceStrategyDefinition)
	{
		if (string.IsNullOrWhiteSpace(surveillanceStrategyDefinition))
		{
			return;
		}
		var root = XElement.Parse(surveillanceStrategyDefinition);
		foreach (var zoneElement in root.Element("Zones")?.Elements("Zone") ?? Enumerable.Empty<XElement>())
		{
			var zone = gameworld.Zones.Get(long.Parse(zoneElement.Value));
			if (zone != null)
			{
				Zones.Add(zone);
			}
		}
		foreach (var cellElement in root.Element("IncludedCells")?.Elements("Cell") ?? Enumerable.Empty<XElement>())
		{
			var cell = gameworld.Cells.Get(long.Parse(cellElement.Value));
			if (cell != null)
			{
				IncludedCells.Add(cell);
			}
		}
		foreach (var cellElement in root.Element("ExcludedCells")?.Elements("Cell") ?? Enumerable.Empty<XElement>())
		{
			var cell = gameworld.Cells.Get(long.Parse(cellElement.Value));
			if (cell != null)
			{
				ExcludedCells.Add(cell);
			}
		}
	}

	public List<IZone> Zones { get; } = new();
	public List<ICell> ExcludedCells { get; } = new();
	public List<ICell> IncludedCells { get; } = new();

	public IEnumerable<ICell> GetCells(IFuturemud gameworld)
	{
		var cells = new List<ICell>();
		foreach (var zone in Zones)
		{
			cells.AddRange(zone.Cells);
		}
		cells.AddRange(IncludedCells);
		cells.RemoveAll(x => ExcludedCells.Contains(x));
		return cells;
	}

	public string SaveDefinition() => 
		new XElement("Definition",
			new XElement("Zones",
				from zone in Zones
				select new XElement("Zone", zone.Id)
			),
			new XElement("IncludedCells",
				from cell in IncludedCells
				select new XElement("Cell", cell.Id)
			),
			new XElement("ExcludedCells",
				from cell in ExcludedCells
				select new XElement("Cell", cell.Id))
		)
		.ToString();

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "zone":
				return BuildingCommandZone(actor, command);
			case "include":
				return BuildingCommandInclude(actor, command);
			case "exclude":
				return BuildingCommandExclude(actor, command);
			default:
				actor.OutputHandler.Send(@"You can use the following options:

	#3zone <zone id>#0 - toggles surveillance of all rooms in the given zone
	#3include <room id>#0 - toggles surveillance of the given specific room
	#3exclude <room id>#0 - toggles exclusion of a specific room".SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandZone(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which zone do you want to toggle surveillance of?");
			return false;
		}

		var zone = actor.Gameworld.Zones.GetByIdOrName(command.SafeRemainingArgument);
		if (zone is null)
		{
			actor.OutputHandler.Send($"There is no zone identified by the text {command.SafeRemainingArgument.ColourCommand()}.");
			return false;
		}

		if (Zones.Contains(zone))
		{
			Zones.Remove(zone);
			actor.OutputHandler.Send($"Surveillance of all rooms in the zone {zone.Name.ColourName()} is now disabled.");
		}
		else
		{
			Zones.Add(zone);
			actor.OutputHandler.Send($"Surveillance of all rooms in the zone {zone.Name.ColourName()} is now enabled.");
		}

		return true;
	}
	private bool BuildingCommandInclude(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which specific room do you want to toggle inclusion of?");
			return false;
		}

		var cell = RoomBuilderModule.LookupCell(actor, command.SafeRemainingArgument);
		if (cell is null)
		{
			actor.OutputHandler.Send($"There is no room identified by the text {command.SafeRemainingArgument.ColourCommand()}.");
			return false;
		}

		if (IncludedCells.Contains(cell))
		{
			IncludedCells.Remove(cell);
			actor.OutputHandler.Send($"Surveillance of the room {cell.GetFriendlyReference(actor)} is no longer included.");
		}
		else
		{
			IncludedCells.Add(cell);
			actor.OutputHandler.Send($"Surveillance of the room {cell.GetFriendlyReference(actor)} is now included.");
		}

		return true;
	}

	private bool BuildingCommandExclude(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which specific room do you want to toggle exclusion of?");
			return false;
		}

		var cell = RoomBuilderModule.LookupCell(actor, command.SafeRemainingArgument);
		if (cell is null)
		{
			actor.OutputHandler.Send($"There is no room identified by the text {command.SafeRemainingArgument.ColourCommand()}.");
			return false;
		}

		if (ExcludedCells.Contains(cell))
		{
			ExcludedCells.Remove(cell);
			actor.OutputHandler.Send($"Surveillance of the room {cell.GetFriendlyReference(actor)} is no longer excluded.");
		}
		else
		{
			ExcludedCells.Add(cell);
			actor.OutputHandler.Send($"Surveillance of the room {cell.GetFriendlyReference(actor)} is now excluded.");
		}

		return true;
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		if (!Zones.Any() && !IncludedCells.Any()) {
			return "No surveillance of game world";
		}

		if (Zones.Any())
		{
			sb.AppendLine("All rooms in the following zones:");
			foreach (var zone in Zones)
			{
				sb.AppendLine($"\t{zone.Name.ColourName()} (#{zone.Id.ToStringN0Colour(actor)})");
			}
		}

		if (IncludedCells.Any())
		{
			if (sb.Length > 0)
			{
				sb.AppendLine();
			}

			sb.AppendLine("The following specific rooms:");
			foreach (var cell in IncludedCells)
			{
				sb.AppendLine($"\t{cell.GetFriendlyReference(actor)}");
			}
		}

		if (ExcludedCells.Any())
		{
			if (sb.Length > 0)
			{
				sb.AppendLine();
			}

			sb.AppendLine("Excluding the following specific rooms:");
			foreach (var cell in ExcludedCells)
			{
				sb.AppendLine($"\t{cell.GetFriendlyReference(actor)}");
			}
		}

		return sb.ToString();
	}
}
