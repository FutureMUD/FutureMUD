using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

public class DoorguardOpenDoor : DelayedAction
{
	public DoorguardOpenDoor(IPerceivable owner, Action<IPerceivable> action) : base(
		owner, action, "opening a door")
	{
	}
}