
namespace MudSharp.Effects.Concrete;

public class WaitingForScavenge : DelayedAction
{
    public WaitingForScavenge(IPerceivable owner, Action<IPerceivable> action) : base(owner, action,
        "waiting to scavenge again")
    {
    }
}