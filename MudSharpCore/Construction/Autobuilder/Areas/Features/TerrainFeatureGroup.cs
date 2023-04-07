using System.Collections.Generic;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.Construction.Autobuilder.Areas.Features;

public abstract class TerrainFeatureGroup
{
	public string Name { get; set; }
	public abstract void ApplyTerrainFeatures(ICell[,] cellMap, List<string>[,] featureMap);
	public abstract string Show(ICharacter builder);

	public abstract bool BuildingCommand(ICharacter actor, AutobuilderAreaTerrainRectangleRandomFeatures parent,
		StringStack command);

	public abstract XElement SaveToXml();
	public abstract List<Feature> Features { get; }
	public abstract string StringForParentShow(ICharacter builder);
}