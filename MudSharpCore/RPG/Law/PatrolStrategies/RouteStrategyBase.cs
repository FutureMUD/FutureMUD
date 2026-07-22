using MudSharp.Character.Name;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.GameItems;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.Movement;
using MudSharp.NPC;
using MudSharp.NPC.AI;

namespace MudSharp.RPG.Law.PatrolStrategies;

public abstract class RouteStrategyBase : PatrolStrategyBase
{
    protected RouteStrategyBase(IFuturemud gameworld) : base(gameworld)
    {
    }

    protected override void PatrolTickPatrolPhase(IPatrol patrol)
    {
        if (patrol.ActiveEnforcementTarget != null)
        {
            return;
        }

        if (patrol.PatrolLeader.Location == patrol.NextMajorNode)
        {
            patrol.LastMajorNode = patrol.NextMajorNode;
            patrol.LastArrivedTime = DateTime.UtcNow;
            if (patrol.NextMajorNode == patrol.PatrolRoute.PatrolNodes.Last())
            {
                patrol.NextMajorNode = patrol.PatrolRoute.PatrolNodes.First();
            }
            else
            {
                patrol.NextMajorNode = patrol.PatrolRoute.PatrolNodes.SkipWhile(x => x != patrol.LastMajorNode).Skip(1)
                                             .First();
            }
        }

        if (patrol.NextMajorNode is null &&
            DateTime.UtcNow - patrol.LastArrivedTime >= patrol.PatrolRoute.LingerTimeMajorNode)
        {
            patrol.CompletePatrol();
            return;
        }

        if (DateTime.UtcNow - patrol.LastArrivedTime >= patrol.PatrolRoute.LingerTimeMajorNode)
        {
            FollowingPath effect = patrol.PatrolLeader.CombinedEffectsOfType<FollowingPath>().FirstOrDefault();
            if (effect != null)
            {
                return;
            }

			if (TryBeginPatrolPath(
					patrol.PatrolLeader,
					patrol.NextMajorNode,
					20.0,
					PathSearch.PathIncludeUnlockableDoors(patrol.PatrolLeader)) ||
				TryBeginPatrolPath(
					patrol.PatrolLeader,
					patrol.NextMajorNode,
					50.0,
					PathSearch.IgnorePresenceOfDoors))
			{
				return;
			}

			if (DateTime.UtcNow - patrol.LastArrivedTime > TimeSpan.FromMinutes(3))
			{
				patrol.AbortPatrol();
			}

			return;
        }
    }
}
