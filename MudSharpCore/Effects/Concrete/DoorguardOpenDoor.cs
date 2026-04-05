using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Effects.Concrete;

public class DoorguardOpenDoor : DelayedAction
{
    public DoorguardOpenDoor(IPerceivable owner, Action<IPerceivable> action) : base(
        owner, action, "opening a door")
    {
    }
}