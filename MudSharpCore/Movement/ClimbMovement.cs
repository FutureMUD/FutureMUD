using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Climate;
using MudSharp.Construction.Boundary;
using MudSharp.Framework.Scheduling;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Movement;

internal class ClimbMovement : SingleMovement
{
	public override bool IgnoreTerrainStamina => true;

	public ClimbMovement(GroupMovement movement, ICharacter mover) : base(movement, mover)
	{
	}

	public ClimbMovement(ICellExit exit, ICharacter mover, TimeSpan duration, IEmote playerEmote = null) : base(exit,
		mover, duration, playerEmote)
	{
	}

	protected override void HandleMovementEcho()
	{
		Mover.OutputHandler.Handle(
			new MixedEmoteOutput(
				new Emote(
					$"@ climb|climbs {Exit.InboundMovementSuffix}",
					Mover),
				flags: OutputFlags.SuppressObscured | OutputFlags.SuppressSource).Append(MoverEmote));
	}

	protected override MixedEmoteOutput GetMovementOutput => new(
		new Emote("@ begin|begins climbing " + Exit.OutboundMovementSuffix,
			Mover), flags: OutputFlags.SuppressObscured);

	public override void IntermediateStep()
	{
		if (Cancelled)
		{
			return;
		}

		var outcome = Mover.DoClimbMovementCheck(Exit.ClimbDifficulty);
		if (Cancelled)
		{
			return;
		}

		if (outcome)
		{
			base.IntermediateStep();
			return;
		}

		// TODO - climb speed
		if (TimeSpan.Zero.CompareTo(Duration) < 0)
		{
			Mover.Gameworld.Scheduler.AddSchedule(new Schedule(IntermediateStep, ScheduleType.Movement, Duration,
				"ClimbingMovement Intermediate Step"));
		}
		else
		{
			IntermediateStep();
		}
	}
}