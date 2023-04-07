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

	public static void RegisterLoader()
	{
		RegisterAIType("PathToLocation", (ai, gameworld) => new PathToLocationAI(ai, gameworld));
	}

	protected override IEnumerable<ICellExit> GetPath(ICharacter ch)
	{
		var location = (ICell)TargetLocationProg.Execute(ch);
		if (location == null || Equals(location, ch.Location))
		{
			return Enumerable.Empty<ICellExit>();
		}

		// First try to find a path to the primary target
		var path = ch.PathBetween(location, 12, GetSuitabilityFunction(ch));
		if (path.Any())
		{
			return path;
		}

		if (MoveEvenIfObstructionInWay)
		{
			path = ch.PathBetween(location, 12, GetSuitabilityFunction(ch, false));
			if (path.Any())
			{
				return path;
			}
		}

		// If we can't find a path to the primary target, check if there is a fallback target
		location = (ICell)FallbackLocationProg.Execute(ch);
		if (location == null || location == ch.Location)
		{
			return Enumerable.Empty<ICellExit>();
		}

		path = ch.PathBetween(location, 12, GetSuitabilityFunction(ch));
		if (path.Any())
		{
			return path;
		}

		if (MoveEvenIfObstructionInWay)
		{
			path = ch.PathBetween(location, 12, GetSuitabilityFunction(ch, false));
			if (path.Any())
			{
				return path;
			}
		}

		// If the fallback target can't  be reached, see if we can reach of any of the way points
		path = ch.PathBetween(((IList)WayPointsProg.Execute(ch)).OfType<ICell>().ToList(), 12,
			GetSuitabilityFunction(ch));
		if (path.Any())
		{
			return path;
		}

		return Enumerable.Empty<ICellExit>();
	}
}