
namespace MudSharp.Effects.Concrete;

public class WandererWaiting : DelayedAction
{
    public WandererWaiting(IPerceivable owner, Action<IPerceivable> action) : base(
        owner, action, "waiting for a chance to move")
    {
    }
}