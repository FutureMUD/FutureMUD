using MudSharp.Work.Foraging;

namespace MudSharp.Construction.Autobuilder.Rooms;

public record AutobuilderRoomInfo
{
    public AutobuilderRoomInfo()
    {
    }

    public AutobuilderRoomInfo(XElement root, IFuturemud gameworld)
    {
        DefaultTerrain = gameworld.Terrains.Get(long.Parse(root.Element("DefaultTerrain")?.Value ?? "0")) ??
                         gameworld.Terrains.FirstOrDefault(x => x.DefaultTerrain);
        RoomName = root.Element("RoomName")?.Value ?? "An Unnamed Room";
        RoomDescription = root.Element("RoomDescription")?.Value ?? "An undescribed room";
        OutdoorsType =
            (RoomOutdoorsType)int.Parse(root.Element("OutdoorsType")?.Value ??
                                        ((int)RoomOutdoorsType.Outdoors).ToString());
        AmbientLightFactor = double.Parse(root.Element("CellLightMultiplier")?.Value ?? "1.0");
        ForagableProfile =
            gameworld.ForagableProfiles.Get(long.Parse(root.Element("ForagableProfile")?.Value ?? "0"));
    }

    public string RoomName { get; init; }
    public string RoomDescription { get; init; }
    public RoomOutdoorsType OutdoorsType { get; init; }
    public double AmbientLightFactor { get; init; }
    public ITerrain DefaultTerrain { get; init; }
    public IForagableProfile ForagableProfile { get; init; }

    public XElement SaveToXml()
    {
        return new XElement("Terrain",
            new XElement("DefaultTerrain", DefaultTerrain?.Id ?? 0),
            new XElement("RoomName", new XCData(RoomName)),
            new XElement("RoomDescription", new XCData(RoomDescription)),
            new XElement("OutdoorsType", (int)OutdoorsType),
            new XElement("CellLightMultiplier", AmbientLightFactor),
            new XElement("ForagableProfile", ForagableProfile?.Id ?? 0)
        );
    }
}