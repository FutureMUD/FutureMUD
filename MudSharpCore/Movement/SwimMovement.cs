using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework.Scheduling;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;

namespace MudSharp.Movement;

public class SwimMovement : SingleMovement
{
	public SwimMovement(GroupMovement movement, ICharacter mover) : base(movement, mover)
	{
	}

	public SwimMovement(ICellExit exit, ICharacter mover, TimeSpan duration, IEmote playerEmote = null) : base(exit,
		mover, duration, playerEmote)
	{
	}

	protected override void HandleMovementEcho()
	{
		if (Exit.Origin.Terrain(Mover).TerrainLayers.Any(x => x.IsUnderwater()) &&
		    Exit.Destination.Terrain(Mover).TerrainLayers.Any(x => x.IsUnderwater()))
		{
			Mover.OutputHandler.Handle(
				new MixedEmoteOutput(
					new Emote(
						$"@ swim|swims {Exit.InboundMovementSuffix}",
						Mover),
					flags: OutputFlags.SuppressObscured | OutputFlags.SuppressSource).Append(MoverEmote));
		}
		else
		{
			Mover.OutputHandler.Handle(
				new MixedEmoteOutput(
					new Emote(
						$"@ wade|wades {Exit.InboundMovementSuffix}",
						Mover),
					flags: OutputFlags.SuppressObscured | OutputFlags.SuppressSource).Append(MoverEmote));
		}
	}

	protected override MixedEmoteOutput GetMovementOutput
	{
		get
		{
			if (Exit.Origin.Terrain(Mover).TerrainLayers.Any(x => x.IsUnderwater()) &&
			    Exit.Destination.Terrain(Mover).TerrainLayers.Any(x => x.IsUnderwater()))
			{
				return new MixedEmoteOutput(new Emote("@ begin|begins swimming " + Exit.OutboundMovementSuffix, Mover),
					flags: OutputFlags.SuppressObscured);
			}

			return new MixedEmoteOutput(new Emote("@ begin|begins wading " + Exit.OutboundMovementSuffix, Mover),
				flags: OutputFlags.SuppressObscured);
		}
	}

	public override void IntermediateStep()
	{
		if (Cancelled)
		{
			return;
		}

		if (!Exit.Origin.Terrain(Mover).TerrainLayers.Any(x => x.IsUnderwater()))
		{
			Mover.SetPosition(PositionSwimming.Instance, PositionModifier.None, null, null);
			base.IntermediateStep();
			return;
		}

		if (!Mover.AffectedBy<IImmwalkEffect>())
		{
			var check = Mover.Gameworld.GetCheck(CheckType.SwimmingCheck);
			var allResults = check.CheckAgainstAllDifficulties(Mover, Difficulty.Normal, null);
			var primaryResult = allResults[Difficulty.Normal];

			if (primaryResult.IsAbjectFailure)
			{
				Mover.OutputHandler.Handle(new EmoteOutput(new Emote(
					"@ thrash|thrashes about uselessly instead of making any swimming progress.", Mover, Mover)));
				Cancel();
				return;
			}

			if (primaryResult.Outcome.IsFail() && allResults[Difficulty.Trivial].IsFail())
			{
				Mover.OutputHandler.Handle(new EmoteOutput(new Emote(
					"@ have|has trouble making any swimming progress and haven't|hasn't really gotten anywhere.", Mover,
					Mover)));
				if (TimeSpan.Zero.CompareTo(Duration) < 0)
				{
					Mover.Gameworld.Scheduler.AddSchedule(new Schedule(IntermediateStep, ScheduleType.Movement,
						Duration, "SwimMovement Intermediate Step"));
				}
				else
				{
					IntermediateStep();
				}

				return;
			}
		}

		base.IntermediateStep();
		if (!Exit.Destination.Terrain(Mover).TerrainLayers.Any(x => x.IsUnderwater()))
		{
			Mover.SetPosition(Mover.MostUprightMobilePosition(true) ?? PositionSprawled.Instance, PositionModifier.None,
				null, null);
		}
	}
}