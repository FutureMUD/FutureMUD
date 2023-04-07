using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

public class WandererWaiting : DelayedAction
{
	public WandererWaiting(IPerceivable owner, Action<IPerceivable> action) : base(
		owner, action, "waiting for a chance to move")
	{
	}
}