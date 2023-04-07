using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

public class WaitingForScavenge : DelayedAction
{
	public WaitingForScavenge(IPerceivable owner, Action<IPerceivable> action) : base(owner, action,
		"waiting to scavenge again")
	{
	}
}