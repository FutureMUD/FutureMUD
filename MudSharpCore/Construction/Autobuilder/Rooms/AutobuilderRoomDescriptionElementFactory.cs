using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Framework;

namespace MudSharp.Construction.Autobuilder.Rooms;

public static class AutobuilderRoomDescriptionElementFactory
{
	public static IAutobuilderRandomDescriptionElement LoadElement(XElement root, IFuturemud gameworld)
	{
		switch (root.Attribute("type")?.Value ?? "")
		{
			case "road":
				return new AutobuilderRoadRandomDescriptionElement(root, gameworld);
			case "group":
				return new AutobuilderRandomDescriptionGroup(root, gameworld);
			default:
				return new AutobuilderRandomDescriptionElement(root, gameworld);
		}
	}
}