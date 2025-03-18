using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

public class DoorguardCloseDoor : DelayedAction
{
	public DoorguardCloseDoor(IPerceivable owner, Action<IPerceivable> action, ICellExit exit) : base(
		owner, action, "closing a door")
	{
		Exit = exit;
	}

	public ICellExit Exit { get; }

	#region Overrides of Effect

	/// <inheritdoc />
	public override bool Applies(object target)
	{
		if (target is ICellExit ce)
		{
			return ce == Exit;
		}
		return base.Applies(target);
	}

	#endregion
}