
namespace MudSharp.Effects.Concrete;

public class DoorguardOpenDoor : DelayedAction
{
    public DoorguardOpenDoor(IPerceivable owner, Action<IPerceivable> action) : base(
        owner, action, "opening a door")
    {
    }
}