#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.Movement;
using MudSharp.RPG.Checks;

namespace MudSharp.Vehicles;

/// <summary>Opt-in proficiency at the manual room-exit driving boundary, separate from navigation.</summary>
public static class VehiclePilot
{
	public const string EnabledSetting = "VehiclePilotEnabled";
	public const string TraitSetting = "VehiclePilotTraitId";
	public const string NoProgressMessage = "You cannot control the vehicle well enough to make progress.";
	private static readonly CardinalDirection[] Compass =
	[
		CardinalDirection.North, CardinalDirection.NorthEast, CardinalDirection.East, CardinalDirection.SouthEast,
		CardinalDirection.South, CardinalDirection.SouthWest, CardinalDirection.West, CardinalDirection.NorthWest
	];

	/// <summary>
	/// Source wagon proficiency is 1d60, followed by one nonlearning recovery roll on failure.
	/// Safe adjacent headings adapt its four-direction deviation to native eight-direction exits.
	/// The caller validates readiness first and revalidates the selected exit before committing movement.
	/// </summary>
	public static IRoomExit? Resolve(ICharacter actor, IRoomExit intended, IEnumerable<IRoomExit> exits,
		Func<IRoomExit, bool> canUse, Func<int, int, int>? roll = null, IPerceiver? movementPerceiver = null)
	{
		if (!actor.Gameworld.GetStaticBool(EnabledSetting)) return intended;
		using var learning = CheckLearningScope.EnterIfNeeded(actor);
		var definition = actor.Gameworld.Traits.Get(actor.Gameworld.GetStaticLong(TraitSetting));
		// Role ownership is deliberate: this hook never grants a missing proficiency through driving.
		if (definition is not ISkillDefinition || !actor.HasTrait(definition)) return null;
		var proficiency = actor.TraitValue(definition);
		if (!double.IsFinite(proficiency) || proficiency < 0.0) return null;
		roll ??= RandomUtilities.Random;
		if (roll(1, 60) <= proficiency) return CheckLearningScope.CanContinue(actor) ? intended : null;

		// One failed use, irrespective of occupants, tow train or subsequent recovery outcome.
		if (CheckLearningScope.CanContinue(actor))
			actor.GetTrait(definition)?.TraitUsed(actor, Outcome.Fail, Difficulty.Normal, TraitUseType.Practical, []);
		if (!CheckLearningScope.CanContinue(actor)) return null;
		if (roll(1, 60) <= proficiency) return null;

		var heading = Array.IndexOf(Compass, intended.OutboundDirection);
		if (heading < 0) return null;
		var direction = Compass[(heading + roll(-1, 1) + Compass.Length) % Compass.Length];
		var candidates = exits.Where(x => x.OutboundDirection == direction &&
			DirectionSenseMovement.IsSafeVisibleCompassExit(actor, intended.Origin, x, movementPerceiver) && canUse(x))
			.Distinct().Take(2).ToArray();
		return candidates.Length == 1 ? candidates[0] : null;
	}
}
