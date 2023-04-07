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

public class SwimmingDragMovement : DragMovement
{
	public override double StaminaMultiplier => 2.0;

	public SwimmingDragMovement(ICharacter dragger, IEnumerable<ICharacter> helpers, IPerceivable target,
		Dragging effect, ICellExit exit, TimeSpan duration) : base(dragger, helpers, target, effect, exit, duration)
	{
		DragAddendum = " through the water";
		DragVerb1stPerson = "carry";
		DragVerb3rdPerson = "carries";
		DragVerbGerund = "carrying";
	}
}