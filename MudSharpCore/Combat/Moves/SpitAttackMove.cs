using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Combat.Moves;

public class SpitAttackMove : NaturalRangedAttackMoveBase
{
    public SpitAttackMove(ICharacter owner, INaturalAttack attack, ICharacter target) : base(owner, attack, target)
    {
    }

    private ISpitAttack SpitAttack => Attack.GetAttackType<ISpitAttack>();

    public override string Description => "Spitting a contaminating liquid";
    public override BuiltInCombatMoveType MoveType => BuiltInCombatMoveType.SpitNaturalAttack;
    protected override CheckType RangedCheck => CheckType.SpitNaturalAttack;

    protected override IEnumerable<IWound> ApplySuccessfulHit(IPerceiver target, CheckOutcome attackOutcome,
        OpposedOutcomeDegree degree, IBodypart bodypart)
    {
        double quantity = SpitAttack.MaximumQuantity * Math.Max(0.25, (attackOutcome.SuccessDegrees() + 1) / 3.0);
        LiquidMixture mixture = new(SpitAttack.Liquid, quantity, Gameworld);
        switch (target)
        {
            case ICharacter tch:
                tch.Body.ExposeToLiquid(mixture, bodypart, LiquidExposureDirection.FromOnTop);
                break;
            case IGameItem item:
                item.ExposeToLiquid(mixture, bodypart, LiquidExposureDirection.FromOnTop);
                break;
        }

        return Enumerable.Empty<IWound>();
    }

    protected override void HandleScatterImpact(RangedScatterResult scatter, CheckOutcome attackOutcome)
    {
        double quantity = SpitAttack.MaximumQuantity * 0.5;
        LiquidMixture mixture = new(SpitAttack.Liquid, quantity, Gameworld);
        PuddleGameItemComponentProto.TopUpOrCreateNewPuddle(mixture, scatter.Cell, scatter.RoomLayer, scatter.Cell);
    }
}
