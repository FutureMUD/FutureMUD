using MudSharp.Commands.Modules;
using MudSharp.Construction;
using Org.BouncyCastle.Asn1.Pkcs;

namespace MudSharp.RPG.AIStorytellers;

public class AIStorytellerSurveillanceStrategy : IAIStorytellerSurveillanceStrategy
{
    public AIStorytellerSurveillanceStrategy(IFuturemud gameworld, string surveillanceStrategyDefinition)
    {
        if (string.IsNullOrWhiteSpace(surveillanceStrategyDefinition))
        {
            return;
        }
        XElement root = XElement.Parse(surveillanceStrategyDefinition);
        foreach (XElement zoneElement in root.Element("Zones")?.Elements("Zone") ?? Enumerable.Empty<XElement>())
        {
            IZone zone = gameworld.Zones.Get(long.Parse(zoneElement.Value));
            if (zone != null)
            {
                Zones.Add(zone);
            }
        }
        foreach (XElement cellElement in root.Element("IncludedCells")?.Elements("Cell") ?? Enumerable.Empty<XElement>())
        {
            IRoom room = gameworld.Rooms.Get(long.Parse(cellElement.Value));
            if (room != null)
            {
                IncludedRooms.Add(room);
            }
        }
        foreach (XElement cellElement in root.Element("ExcludedCells")?.Elements("Cell") ?? Enumerable.Empty<XElement>())
        {
            IRoom room = gameworld.Rooms.Get(long.Parse(cellElement.Value));
            if (room != null)
            {
                ExcludedRooms.Add(room);
            }
        }
    }

    public List<IZone> Zones { get; } = new();
    public List<IRoom> ExcludedRooms { get; } = new();
    public List<IRoom> IncludedRooms { get; } = new();

    public IEnumerable<IRoom> GetRooms(IFuturemud gameworld)
    {
        List<IRoom> rooms = new();
        foreach (IZone zone in Zones)
        {
            rooms.AddRange(zone.Rooms);
        }
        rooms.AddRange(IncludedRooms);
        rooms.RemoveAll(x => ExcludedRooms.Contains(x));
        return rooms;
    }

    public string SaveDefinition()
    {
        return new XElement("Definition",
            new XElement("Zones",
                from zone in Zones
                select new XElement("Zone", zone.Id)
            ),
            new XElement("IncludedCells",
                from room in IncludedRooms
                select new XElement("Cell", room.Id)
            ),
            new XElement("ExcludedCells",
                from room in ExcludedRooms
                select new XElement("Cell", room.Id))
        )
        .ToString();
    }

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

        IZone zone = actor.Gameworld.Zones.GetByIdOrName(command.SafeRemainingArgument);
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

        IRoom room = RoomBuilderModule.LookupRoom(actor, command.SafeRemainingArgument);
        if (room is null)
        {
            actor.OutputHandler.Send($"There is no room identified by the text {command.SafeRemainingArgument.ColourCommand()}.");
            return false;
        }

        if (IncludedRooms.Contains(room))
        {
            IncludedRooms.Remove(room);
            actor.OutputHandler.Send($"Surveillance of the room {room.GetFriendlyReference(actor)} is no longer included.");
        }
        else
        {
            IncludedRooms.Add(room);
            actor.OutputHandler.Send($"Surveillance of the room {room.GetFriendlyReference(actor)} is now included.");
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

        IRoom room = RoomBuilderModule.LookupRoom(actor, command.SafeRemainingArgument);
        if (room is null)
        {
            actor.OutputHandler.Send($"There is no room identified by the text {command.SafeRemainingArgument.ColourCommand()}.");
            return false;
        }

        if (ExcludedRooms.Contains(room))
        {
            ExcludedRooms.Remove(room);
            actor.OutputHandler.Send($"Surveillance of the room {room.GetFriendlyReference(actor)} is no longer excluded.");
        }
        else
        {
            ExcludedRooms.Add(room);
            actor.OutputHandler.Send($"Surveillance of the room {room.GetFriendlyReference(actor)} is now excluded.");
        }

        return true;
    }

    public string Show(ICharacter actor)
    {
        StringBuilder sb = new();
        if (!Zones.Any() && !IncludedRooms.Any())
        {
            return "No surveillance of game world";
        }

        if (Zones.Any())
        {
            sb.AppendLine("All rooms in the following zones:");
            foreach (IZone zone in Zones)
            {
                sb.AppendLine($"\t{zone.Name.ColourName()} (#{zone.Id.ToStringN0Colour(actor)})");
            }
        }

        if (IncludedRooms.Any())
        {
            if (sb.Length > 0)
            {
                sb.AppendLine();
            }

            sb.AppendLine("The following specific rooms:");
            foreach (IRoom room in IncludedRooms)
            {
                sb.AppendLine($"\t{room.GetFriendlyReference(actor)}");
            }
        }

        if (ExcludedRooms.Any())
        {
            if (sb.Length > 0)
            {
                sb.AppendLine();
            }

            sb.AppendLine("Excluding the following specific rooms:");
            foreach (IRoom room in ExcludedRooms)
            {
                sb.AppendLine($"\t{room.GetFriendlyReference(actor)}");
            }
        }

        return sb.ToString();
    }
}
