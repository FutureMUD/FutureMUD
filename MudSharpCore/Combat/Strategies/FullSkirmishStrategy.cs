
namespace MudSharp.Combat.Strategies;

public class FullSkirmishStrategy : SkirmishStrategy
{
    protected FullSkirmishStrategy()
    {
    }

    public new static FullSkirmishStrategy Instance { get; } = new();
    public override CombatStrategyMode Mode => CombatStrategyMode.FullSkirmish;
}