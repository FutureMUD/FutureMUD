#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Movement;

namespace MudSharp.Vehicles;

public sealed record VehicleMotiveCohort(
	IReadOnlyCollection<ICharacter> Roots,
	IReadOnlyCollection<ICharacter> Characters,
	IReadOnlyCollection<ILocateable> Locateables,
	IReadOnlyCollection<ICharacter> StaminaMovers);

public sealed class VehicleMotiveCohortService
{
	private readonly RouteSpatialService _spatialService = RouteSpatialService.Instance;

	public bool TryBuild(IReadOnlyCollection<ICharacter> roots, SpatialLocation origin,
		out VehicleMotiveCohort cohort, out string reason)
	{
		if (!roots.Any())
		{
			cohort = default!;
			reason = "That vehicle has no external motive character or mount.";
			return false;
		}

		var characters = new HashSet<ICharacter>(CharacterPhysicalInstanceEqualityComparer.Instance);
		var targets = new HashSet<ILocateable>(ReferenceEqualityComparer.Instance);
		var nonStamina = new HashSet<ICharacter>(CharacterPhysicalInstanceEqualityComparer.Instance);
		var queue = new Queue<ICharacter>();
		var immediate = RouteSpatialConfiguration.FromGameworld(roots.First().Gameworld).ImmediateDistanceMetres;
		var nearby = _spatialService.GetPerceivablesWithin(origin, immediate).OfType<ICharacter>().ToList();

		void Add(ICharacter? character)
		{
			if (character is not null && characters.Add(character))
			{
				queue.Enqueue(character);
			}
		}

		foreach (var root in roots)
		{
			Add(root);
			if (root.Party?.Leader is not { } leader || !leader.SamePhysicalInstance(root))
			{
				continue;
			}

			foreach (var member in root.Party.ActiveCharacterMembers)
			{
				Add(member);
			}
		}

		while (queue.Count > 0)
		{
			var character = queue.Dequeue();
			foreach (var follower in nearby.Where(x => x.Following is ICharacter followed &&
			                                           followed.SamePhysicalInstance(character)))
			{
				Add(follower);
			}

			if (character.RidingMount is not null)
			{
				Add(character.RidingMount);
				nonStamina.Add(character);
			}

			foreach (var rider in character.Riders)
			{
				Add(rider);
				nonStamina.Add(rider);
			}

			foreach (var dragging in character.EffectsOfType<Dragging>())
			{
				foreach (var dragger in dragging.CharacterDraggers)
				{
					Add(dragger);
				}

				foreach (var helper in dragging.Helpers)
				{
					Add(helper);
				}

				if (dragging.Target is ICharacter targetCharacter)
				{
					Add(targetCharacter);
					nonStamina.Add(targetCharacter);
				}
				else if (dragging.Target is ILocateable target)
				{
					targets.Add(target);
				}
			}
		}

		foreach (var character in characters)
		{
			if (character.Movement is not null)
			{
				cohort = default!;
				reason = $"{character.HowSeen(roots.First(), true)} is already moving.";
				return false;
			}

			if (nonStamina.Contains(character) || character.RidingMount is not null)
			{
				continue;
			}

			var canMove = character.CanMove(CanMoveFlags.None);
			if (!canMove.Result)
			{
				cohort = default!;
				reason = $"{character.HowSeen(roots.First(), true)} cannot move: {canMove.ErrorMessage}";
				return false;
			}
		}

		var locateables = characters.Cast<ILocateable>()
			.Concat(targets)
			.Distinct<ILocateable>(ReferenceEqualityComparer.Instance)
			.ToList();
		foreach (var locateable in locateables)
		{
			var location = _spatialService.GetEffectiveLocation(locateable);
			var closeEnough = ReferenceEquals(location.Cell, origin.Cell) && location.Layer == origin.Layer &&
			                  (origin.Cell.RouteDefinition is null ||
			                   location.RoutePositionMetres.HasValue && origin.RoutePositionMetres.HasValue &&
			                   Math.Abs(location.RoutePositionMetres.Value - origin.RoutePositionMetres.Value) <=
			                   immediate);
			if (!closeEnough)
			{
				cohort = default!;
				reason = $"{locateable.Name} is not close enough to move with the vehicle's motive cohort.";
				return false;
			}
		}

		cohort = new VehicleMotiveCohort(
			roots.Distinct(CharacterPhysicalInstanceEqualityComparer.Instance).ToList(),
			characters.ToList(),
			locateables,
			characters.Where(x => !nonStamina.Contains(x) && x.RidingMount is null).ToList());
		reason = string.Empty;
		return true;
	}

	public static bool CanTraverseExit(VehicleMotiveCohort cohort, VehicleHitchGraphMovePlan movePlan,
		ICellExit exit, ICharacter voyeur, out string reason)
	{
		var vehicleOccupants = movePlan.Vehicles
			.SelectMany(x => x.Occupants)
			.ToHashSet(CharacterPhysicalInstanceEqualityComparer.Instance);
		foreach (var character in cohort.Characters)
		{
			if (vehicleOccupants.Contains(character) ||
			    character.RidingMount is not null &&
			    cohort.Characters.ContainsPhysicalInstance(character.RidingMount))
			{
				continue;
			}

			if (exit.MovementTransition(character).TransitionType == CellMovementTransition.NoViableTransition)
			{
				reason = $"{character.HowSeen(voyeur, true)} cannot use that exit.";
				return false;
			}

			var canMove = character.CanMove(exit, CanMoveFlags.None);
			if (!canMove.Result)
			{
				reason =
					$"{character.HowSeen(voyeur, true)} cannot move through that exit: {canMove.ErrorMessage}";
				return false;
			}
		}

		reason = string.Empty;
		return true;
	}

	public static void MoveAcrossExit(VehicleMotiveCohort cohort, VehicleHitchGraphMovePlan movePlan,
		ICellExit exit, IMovement movement)
	{
		var vehicleOccupants = movePlan.Vehicles
			.SelectMany(x => x.Occupants)
			.ToHashSet(CharacterPhysicalInstanceEqualityComparer.Instance);
		var moved = new HashSet<ICharacter>(CharacterPhysicalInstanceEqualityComparer.Instance);
		foreach (var character in cohort.StaminaMovers.Where(x =>
			         !vehicleOccupants.Contains(x) && x.RidingMount is null))
		{
			character.ExecuteMove(movement);
			moved.Add(character);
			foreach (var rider in character.Riders)
			{
				moved.Add(rider);
			}
		}

		foreach (var character in cohort.Characters.Where(x =>
			         !vehicleOccupants.Contains(x) && !moved.Contains(x)))
		{
			if (character.RidingMount is not null && moved.Contains(character.RidingMount))
			{
				continue;
			}

			var targetLayer = exit.MovementTransition(character).TargetLayer;
			exit.Origin.Leave(character);
			character.RoomLayer = targetLayer;
			character.Moved(movement);
			exit.Destination.Enter(character, exit, roomLayer: targetLayer);
		}
	}

	public static void MoveExtraItemsAcrossExit(VehicleMotiveCohort cohort, VehicleHitchGraphMovePlan movePlan,
		ICellExit exit, RoomLayer targetLayer)
	{
		var graphItems = movePlan.Vehicles
			.Select(x => x.ExteriorItem)
			.Concat(movePlan.HitchItems)
			.Where(x => x is not null)
			.Cast<IGameItem>()
			.ToHashSet(ReferenceEqualityComparer.Instance);
		foreach (var item in cohort.Locateables
			         .OfType<IGameItem>()
			         .Where(x => !graphItems.Contains(x) &&
			                     ReferenceEquals(x.Location, exit.Origin) &&
			                     x.InInventoryOf is null && x.ContainedIn is null)
			         .DistinctBy(x => x.Id))
		{
			exit.Origin.Extract(item);
			item.RoomLayer = targetLayer;
			exit.Destination.Insert(item, true);
			item.ForceMove();
		}
	}
}
