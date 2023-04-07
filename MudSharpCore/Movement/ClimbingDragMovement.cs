using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Movement;

public class ClimbingDragMovement : DragMovement
{
	public override double StaminaMultiplier => 1.5;

	public ClimbingDragMovement(ICharacter dragger, IEnumerable<ICharacter> helpers, IPerceivable target,
		Dragging effect, ICellExit exit, TimeSpan duration) : base(dragger, helpers, target, effect, exit, duration)
	{
		DragVerb1stPerson = "carry";
		DragVerb3rdPerson = "carries";
		DragVerbGerund = "carrying";
	}
}