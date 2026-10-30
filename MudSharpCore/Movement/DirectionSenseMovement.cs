#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.RPG.Checks;

namespace MudSharp.Movement;

/// <summary>One orientation sequence at the manual command boundary, never at follower dispatch.</summary>
public static class DirectionSenseMovement
{
	public const string NoProgressMessage = "You lose your bearings and make no progress.";

	public static bool IsCompassDirection(CardinalDirection direction) =>
		direction is >= CardinalDirection.North and <= CardinalDirection.NorthWest;

	public static bool IsActive(ICharacter actor, IRoom origin)
	{
		if (actor.EffectsOfType<IImmwalkEffect>().Any()) return false;
		if (!actor.Gameworld.GetStaticBool("DirectionSenseEnabled"))
		{
			return false;
		}

		var prog = actor.Gameworld.FutureProgs.Get(actor.Gameworld.GetStaticLong("DirectionSenseObscurationProg"));
		if (prog is not null && prog.ReturnType == ProgVariableTypes.Boolean &&
		    prog.MatchesParameters([ProgVariableTypes.Character, ProgVariableTypes.Location]))
		{
			return prog.ExecuteBool(actor, origin) == true;
		}

		// Use the existing race/darksight-aware light model, not terrain Spot difficulty.
		var minimumSightDifficulty = ReadDifficulty(actor, "DirectionSenseMinimumSightDifficulty") ?? Difficulty.VeryHard;
		if (actor.Body.VisionPercentage <= 0.0 || !actor.Body.CanSee(origin) ||
		    actor.IlluminationSightDifficulty(origin) >= minimumSightDifficulty)
		{
			return true;
		}

		var exposure = origin.OutdoorsType(actor);
		var weather = origin.CurrentWeather(actor);
		if (weather is null) return false;
		if (exposure == RoomOutdoorsType.Outdoors && Enum.IsDefined(weather.Precipitation) &&
		    actor.Gameworld.GetStaticInt($"MinimumSightDifficulty{weather.Precipitation}") >= (int)minimumSightDifficulty)
		{
			return true;
		}

		var maximumMultiplier = actor.Gameworld.GetStaticDouble("DirectionSenseMaximumObscuringWeatherLightMultiplier");
		if (!double.IsFinite(maximumMultiplier) || maximumMultiplier < 0.0) maximumMultiplier = 0.3;
		return exposure is RoomOutdoorsType.Outdoors or RoomOutdoorsType.IndoorsClimateExposed &&
		       weather.Hazards?.AtmosphereGasId > 0 && weather.ObscuresViewOfSky &&
		       double.IsFinite(weather.LightLevelMultiplier) && weather.LightLevelMultiplier <= maximumMultiplier;
	}

	/// <summary>
	/// Null means no progress. The caller validates the intended movement first, then revalidates
	/// the selected exit at dispatch. Eligibility must be silent and must not spend resources or roll.
	/// </summary>
	public static IRoomExit? Resolve(ICharacter actor, IRoomExit intended, IEnumerable<IRoomExit> exits,
		Func<IRoomExit, bool> canUse, IPerceiver? movementPerceiver = null)
	{
		var initial = actor.Gameworld.GetCheck(CheckType.DirectionSenseCheck);
		var recovery = actor.Gameworld.GetCheck(CheckType.DirectionSenseRecoveryCheck);
		// Do not use the world's fallback check, branch a missing skill, or learn from recovery.
		if (!HasRequiredCheckPolicy(initial, recovery))
		{
			return null;
		}

		var firstDifficulty = ReadDifficulty(actor, "DirectionSenseDifficulty");
		var recoveryDifficulty = ReadDifficulty(actor, "DirectionSenseRecoveryDifficulty");
		if (firstDifficulty is null || recoveryDifficulty is null)
		{
			return null;
		}

		if (initial.Check(actor, firstDifficulty.Value).Outcome.IsPass())
		{
			return intended;
		}

		if (recovery.Check(actor, recoveryDifficulty.Value).Outcome.IsPass())
		{
			return null;
		}

		return exits.Where(x => !ReferenceEquals(x, intended) &&
		                        x.OutboundDirection != intended.OutboundDirection &&
		                        IsSafeVisibleCompassExit(actor, intended.Origin, x, movementPerceiver) && canUse(x))
			.Distinct()
			.ToArray()
			.GetRandomElement();
	}

	public static bool IsSafeVisibleCompassExit(ICharacter actor, IRoom origin, IRoomExit exit,
		IPerceiver? movementPerceiver = null) =>
		exit.Origin == origin && exit.Destination is not null && IsCompassDirection(exit.OutboundDirection) &&
		exit is not INonCardinalRoomExit && !exit.IsFallExit &&
		exit.MovementTransition(movementPerceiver ?? actor).TransitionType is not (RoomMovementTransition.FallExit or RoomMovementTransition.NoViableTransition) &&
		actor.Body.CanSee(origin, exit);

	public static void NoProgress(ICharacter actor)
	{
		actor.QueuedMoveCommands.Clear();
		actor.OutputHandler.Send(NoProgressMessage);
		var seconds = actor.Gameworld.GetStaticDouble("DirectionSenseNoProgressDelaySeconds");
		if (double.IsFinite(seconds) && seconds > 0.0)
		{
			actor.AddEffect(new CommandDelay(actor, new[] { "move", "drive" }, "You are still getting your bearings."),
				TimeSpan.FromSeconds(seconds));
		}
	}

	private static Difficulty? ReadDifficulty(ICharacter actor, string setting) =>
		Enum.TryParse<Difficulty>(actor.Gameworld.GetStaticConfiguration(setting), true, out var difficulty) &&
		Enum.IsDefined(difficulty) ? difficulty : null;

	private static bool HasRequiredCheckPolicy(ICheck initial, ICheck recovery)
	{
		if (initial?.Type != CheckType.DirectionSenseCheck || recovery?.Type != CheckType.DirectionSenseRecoveryCheck ||
		    !initial.ImproveTraits || recovery.ImproveTraits || initial.CanTraitBranchIfMissing ||
		    recovery.CanTraitBranchIfMissing || initial.FailIfTraitMissing != FailIfTraitMissingType.FailIfAnyMissing ||
		    recovery.FailIfTraitMissing != FailIfTraitMissingType.FailIfAnyMissing)
		{
			return false;
		}

		var first = initial.TargetNumberExpression.Parameters.Values.ToArray();
		var second = recovery.TargetNumberExpression.Parameters.Values.ToArray();
		return first.Length == 1 && second.Length == 1 && first[0].Trait.Id == second[0].Trait.Id &&
		       first[0].CanImprove && !second[0].CanImprove && !first[0].CanBranch && !second[0].CanBranch;
	}
}
