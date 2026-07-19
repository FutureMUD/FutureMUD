
namespace MudSharp.Combat.Strategies;

public class FullCoverStrategy : CoverSeekingRangedStrategy
{
    protected FullCoverStrategy()
    {
    }

    public static FullCoverStrategy Instance { get; } = new();
    public override CombatStrategyMode Mode => CombatStrategyMode.FullCover;
}