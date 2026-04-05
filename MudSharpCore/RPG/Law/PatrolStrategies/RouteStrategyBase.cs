using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.Movement;
using MudSharp.NPC;
using MudSharp.NPC.AI;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

            List<ICellExit> path = patrol.PatrolLeader
                             .PathBetween(patrol.NextMajorNode, 20,
                                 PathSearch.PathIncludeUnlockedDoors(patrol.PatrolLeader))
                             .ToList();
            // If we can't find a path, try to get closer at least
            if (path.Count == 0)
            {
                path = patrol.PatrolLeader.PathBetween(patrol.NextMajorNode, 50,
                    PathSearch.IgnorePresenceOfDoors).ToList();
                if (path.Count == 0)
                {
                    if (DateTime.UtcNow - patrol.LastArrivedTime > TimeSpan.FromMinutes(3))
                    {
                        // Abort patrol
                        patrol.AbortPatrol();
                        return;
                    }

                    return;
                }
            }

            FollowingPath fp = new(patrol.PatrolLeader, path);
            patrol.PatrolLeader.AddEffect(fp);
            fp.FollowPathAction();
            return;
        }
    }
}