using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

public class DoorguardCloseDoor : DelayedAction
{
	public DoorguardCloseDoor(IPerceivable owner, Action<IPerceivable> action) : base(
		owner, action, "closing a door")
	{
	}
}