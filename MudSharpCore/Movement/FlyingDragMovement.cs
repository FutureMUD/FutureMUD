using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Movement;

public class FlyingDragMovement : DragMovement
{
	public override double StaminaMultiplier => 1.5;

	public FlyingDragMovement(ICharacter dragger, IEnumerable<ICharacter> helpers, IPerceivable target, Dragging effect,
		ICellExit exit, TimeSpan duration) : base(dragger, helpers, target, effect, exit, duration)
	{
		DragAddendum = " through the air";
		DragVerb1stPerson = "carry";
		DragVerb3rdPerson = "carries";
		DragVerbGerund = "carrying";
	}

	public override bool IgnoreTerrainStamina => true;
}