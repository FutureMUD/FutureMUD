using MudSharp.Construction.Boundary;
using MudSharp.Economy.Estates;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Movement;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.RPG.Law.PatrolStrategies;

public class InvestigationPatrolStrategy : PatrolStrategyBase
{
    public InvestigationPatrolStrategy(IFuturemud gameworld) : base(gameworld)
    {
    }

    public override string Name => "Investigation Patrol";

    public override void HandlePatrolTick(IPatrol patrol)
    {
        if (patrol.ActiveCorpseRecoveryReport == null)
        {
            patrol.CompletePatrol();
            return;
        }

        switch (patrol.PatrolPhase)
        {
            case PatrolPhase.Preperation:
                PatrolTickPreparationPhase(patrol);
                return;
            case PatrolPhase.Deployment:
                HandleDeployment(patrol);
                return;
            case PatrolPhase.Patrol:
                HandleRecovery(patrol);
                return;
            case PatrolPhase.Return:
                PatrolTickReturnPhase(patrol);
                return;
        }
    }

    protected override void PatrolTickPatrolPhase(IPatrol patrol)
    {
        HandleRecovery(patrol);
    }

    protected override void PatrolTickPreparationPhase(IPatrol patrol)
    {
        base.PatrolTickPreparationPhase(patrol);
        patrol.NextMajorNode = patrol.ActiveCorpseRecoveryReport?.SourceCell;
    }

    private void HandleDeployment(IPatrol patrol)
    {
        ICorpseRecoveryReport report = patrol.ActiveCorpseRecoveryReport;
        if (report == null)
        {
            patrol.CompletePatrol();
            return;
        }

        ICorpse corpse = report.Corpse?.GetItemType<ICorpse>();
        if (corpse == null || report.Corpse.Location == null || report.Corpse.Location != report.SourceCell)
        {
            report.MarkFailed();
            patrol.ActiveCorpseRecoveryReport = null;
            patrol.CompletePatrol();
            return;
        }

        if (patrol.PatrolLeader.Location == report.SourceCell)
        {
            patrol.PatrolPhase = PatrolPhase.Patrol;
            patrol.LastArrivedTime = DateTime.UtcNow;
            return;
        }

        if (patrol.PatrolLeader.CombinedEffectsOfType<FollowingPath>().Any())
        {
            return;
        }

        List<ICellExit> path = patrol.PatrolLeader.PathBetween(report.SourceCell, 50,
            PathSearch.PathIncludeUnlockableDoors(patrol.PatrolLeader)).ToList();
        if (!path.Any())
        {
            path = patrol.PatrolLeader.PathBetween(report.SourceCell, 50, PathSearch.IgnorePresenceOfDoors).ToList();
            if (!path.Any())
            {
                if (DateTime.UtcNow - patrol.LastArrivedTime > TimeSpan.FromMinutes(3))
                {
                    report.MarkFailed();
                    patrol.ActiveCorpseRecoveryReport = null;
                    patrol.CompletePatrol();
                }

                return;
            }
        }

        FollowingPath fp = new(patrol.PatrolLeader, path) { UseDoorguards = true, UseKeys = true, OpenDoors = true };
        patrol.PatrolLeader.AddEffect(fp);
        fp.FollowPathAction();
    }

    private void HandleRecovery(IPatrol patrol)
    {
        ICorpseRecoveryReport report = patrol.ActiveCorpseRecoveryReport;
        if (report == null)
        {
            patrol.CompletePatrol();
            return;
        }

        IGameItem corpseItem = report.Corpse;
        if (corpseItem?.GetItemType<ICorpse>() == null)
        {
            report.MarkFailed();
            patrol.ActiveCorpseRecoveryReport = null;
            patrol.CompletePatrol();
            return;
        }

        if (corpseItem.Location != report.SourceCell)
        {
            report.MarkFailed();
            patrol.ActiveCorpseRecoveryReport = null;
            patrol.CompletePatrol();
            return;
        }

        if (patrol.PatrolLeader.Location != report.SourceCell)
        {
            patrol.PatrolPhase = PatrolPhase.Deployment;
            return;
        }

        MorgueService.IntakeCorpse(report.EconomicZone, corpseItem);
        report.MarkCompleted();
        patrol.ActiveCorpseRecoveryReport = null;
        patrol.CompletePatrol();
    }
}
