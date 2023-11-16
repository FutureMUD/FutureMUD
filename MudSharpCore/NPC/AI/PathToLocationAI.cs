using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.NPC.AI.Strategies;

namespace MudSharp.NPC.AI;

public class PathToLocationAI : PathingAIWithProgTargetsBase
{
	protected PathToLocationAI(ArtificialIntelligence ai, IFuturemud gameworld) : base(ai, gameworld)
	{
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("PathingEnabledProg", PathingEnabledProg?.Id ?? 0L),
			new XElement("OnStartToPathProg", OnStartToPathProg?.Id ?? 0L),
			new XElement("TargetLocationProg", TargetLocationProg?.Id ?? 0L),
			new XElement("FallbackLocationProg", FallbackLocationProg?.Id ?? 0L),
			new XElement("WayPointsProg", WayPointsProg.Id),
			new XElement("OpenDoors", OpenDoors),
			new XElement("UseKeys", UseKeys),
			new XElement("SmashLockedDoors", SmashLockedDoors),
			new XElement("CloseDoorsBehind", CloseDoorsBehind),
			new XElement("UseDoorguards", UseDoorguards),
			new XElement("MoveEvenIfObstructionInWay", MoveEvenIfObstructionInWay)
		).ToString();
	}

	public static void RegisterLoader()
	{
		RegisterAIType("PathToLocation", (ai, gameworld) => new PathToLocationAI(ai, gameworld));
	}

	protected override (ICell Target, IEnumerable<ICellExit>) GetPath(ICharacter ch)
	{
		var location = (ICell)TargetLocationProg.Execute(ch);
		if (location == null || Equals(location, ch.Location))
		{
			return (null, Enumerable.Empty<ICellExit>());
		}

		// First try to find a path to the primary target
		var path = ch.PathBetween(location, 12, GetSuitabilityFunction(ch));
		if (path.Any())
		{
			return (location, path);
		}

		if (MoveEvenIfObstructionInWay)
		{
			path = ch.PathBetween(location, 12, GetSuitabilityFunction(ch, false));
			if (path.Any())
			{
				return (location, path);
			}
		}

		// If we can't find a path to the primary target, check if there is a fallback target
		location = (ICell)FallbackLocationProg.Execute(ch);
		if (location == null || location == ch.Location)
		{
			return (null, Enumerable.Empty<ICellExit>());
		}

		path = ch.PathBetween(location, 12, GetSuitabilityFunction(ch));
		if (path.Any())
		{
			return (location, path);
		}

		if (MoveEvenIfObstructionInWay)
		{
			path = ch.PathBetween(location, 12, GetSuitabilityFunction(ch, false));
			if (path.Any())
			{
				return (location, path);
			}
		}

		// If the fallback target can't  be reached, see if we can reach of any of the way points
		path = ch.PathBetween(((IList)WayPointsProg.Execute(ch)).OfType<ICell>().ToList(), 12,
			GetSuitabilityFunction(ch));
		if (path.Any())
		{
			return (location, path);
		}

		return (null, Enumerable.Empty<ICellExit>());
	}
}