#nullable enable annotations

using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.GameItems;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.Models;
using MudSharp.NPC.AI.Strategies;
using System.Collections;

namespace MudSharp.NPC.AI;

public class PathToLocationAI : PathingAIWithProgTargetsBase
{
    protected PathToLocationAI(ArtificialIntelligence ai, IFuturemud gameworld) : base(ai, gameworld)
    {
    }

    private PathToLocationAI()
    {

    }

    private PathToLocationAI(IFuturemud gameworld, string name) : base(gameworld, name, "PathToLocation")
    {
        PathingEnabledProg = Gameworld.AlwaysFalseProg;
        DatabaseInitialise();
    }

    protected override string SaveToXml()
    {
        return new XElement("Definition",
            new XElement("PathingEnabledProg", PathingEnabledProg?.Id ?? 0L),
            new XElement("OnStartToPathProg", OnStartToPathProg?.Id ?? 0L),
            new XElement("TargetLocationProg", TargetLocationProg?.Id ?? 0L),
            new XElement("FallbackLocationProg", FallbackLocationProg?.Id ?? 0L),
            new XElement("WayPointsProg", WayPointsProg?.Id ?? 0L),
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
        RegisterAIBuilderInformation("pathtolocation", (gameworld, name) => new PathToLocationAI(gameworld, name), new PathToLocationAI().HelpText);
    }

    protected override (ICell? Target, IEnumerable<ICellExit>) GetPath(ICharacter ch)
    {
        ICell location = TargetLocationProg?.Execute<ICell>(ch);
        if (location == null || Equals(location, ch.Location))
        {
            return (null, Enumerable.Empty<ICellExit>());
        }

        // First try to find a path to the primary target
        IEnumerable<ICellExit> path = ch.PathBetween(location, 12, GetSuitabilityFunction(ch));
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
        FallbackLocationProg?.Execute<ICell>(ch);
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
        if (WayPointsProg is not null)
        {
            path = ch.PathBetween((WayPointsProg.ExecuteCollection<ICell>(ch)).ToList(), 12,
                GetSuitabilityFunction(ch));
            if (path.Any())
            {
                return (location, path);
            }
        }

        return (null, Enumerable.Empty<ICellExit>());
    }

	protected override (ICell? Target, ISpatialPath? Path) GetSpatialPath(ICharacter ch)
	{
		var primary = TargetLocationProg?.Execute<ICell>(ch);
		if (TryFindHybridCandidate(ch, primary, out var primaryPath))
		{
			return (primary, primaryPath);
		}

		var fallback = FallbackLocationProg?.Execute<ICell>(ch);
		if (TryFindHybridCandidate(ch, fallback, out var fallbackPath))
		{
			return (fallback, fallbackPath);
		}

		if (WayPointsProg is null)
		{
			return (null, null);
		}

		var waypointPaths = WayPointsProg.ExecuteCollection<ICell>(ch)
			.Where(x => x is not null)
			.Select(x => (Target: x, Found: TryFindHybridCandidate(ch, x, out var path), Path: path))
			.Where(x => x.Found && x.Path is not null)
			.OrderBy(x => x.Path!.RoomEquivalentCost)
			.ToList();
		return waypointPaths.Count == 0
			? (null, null)
			: (waypointPaths[0].Target, waypointPaths[0].Path);
	}

	private bool TryFindHybridCandidate(ICharacter ch, ICell? target, out ISpatialPath? path)
	{
		path = null;
		if (target is null)
		{
			return false;
		}

		if (TryFindSpatialPath(ch, target, 12.0, GetSuitabilityFunction(ch), out path) &&
			path is not null && path.Steps.Count > 0 && RequiresSpatialFollowing(path))
		{
			return true;
		}

		if (!MoveEvenIfObstructionInWay)
		{
			path = null;
			return false;
		}

		return TryFindSpatialPath(ch, target, 12.0, GetSuitabilityFunction(ch, false), out path) &&
		       path is not null && path.Steps.Count > 0 && RequiresSpatialFollowing(path);
	}
}
