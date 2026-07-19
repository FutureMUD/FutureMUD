
namespace MudSharp.Combat.Strategies;

public class CoveringFireStrategy : RangeBaseStrategy
{
    protected CoveringFireStrategy()
    {
    }

    public static CoveringFireStrategy Instance { get; } = new();

    #region Overrides of StrategyBase

    public override CombatStrategyMode Mode => CombatStrategyMode.CoveringFire;

    #endregion
}