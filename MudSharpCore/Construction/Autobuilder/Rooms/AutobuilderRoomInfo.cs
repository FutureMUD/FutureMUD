using System.Linq;
using System.Xml.Linq;
using MudSharp.Framework;
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
		CellName = root.Element("RoomName")?.Value ?? "An Unnamed Room";
		CellDescription = root.Element("RoomDescription")?.Value ?? "An undescribed room";
		OutdoorsType =
			(CellOutdoorsType)int.Parse(root.Element("OutdoorsType")?.Value ??
			                            ((int)CellOutdoorsType.Outdoors).ToString());
		AmbientLightFactor = double.Parse(root.Element("CellLightMultiplier")?.Value ?? "1.0");
		ForagableProfile =
			gameworld.ForagableProfiles.Get(long.Parse(root.Element("ForagableProfile")?.Value ?? "0"));
	}

	public string CellName { get; init; }
	public string CellDescription { get; init; }
	public CellOutdoorsType OutdoorsType { get; init; }
	public double AmbientLightFactor { get; init; }
	public ITerrain DefaultTerrain { get; init; }
	public IForagableProfile ForagableProfile { get; init; }

	public XElement SaveToXml()
	{
		return new XElement("Terrain",
			new XElement("DefaultTerrain", DefaultTerrain?.Id ?? 0),
			new XElement("RoomName", new XCData(CellName)),
			new XElement("RoomDescription", new XCData(CellDescription)),
			new XElement("OutdoorsType", (int)OutdoorsType),
			new XElement("CellLightMultiplier", AmbientLightFactor),
			new XElement("ForagableProfile", ForagableProfile?.Id ?? 0)
		);
	}
}