#nullable enable

using System.Runtime.CompilerServices;
using System.Threading;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects;
using MudSharp.GameItems;
using MudSharp.Movement;
using MudSharp.NPC.AI;
using MudSharp.Planes;

namespace MudSharp.Vehicles;

/// <summary>Actual contact for an admitted vehicle/train crossing, before any member leaves its origin.</summary>
internal static class VehicleExitBarrierTraversal
{
	private sealed class ContactState
	{
		internal HashSet<IGameItem> Exteriors { get; } = new(ReferenceEqualityComparer.Instance);
		internal HashSet<ICharacter> Characters { get; } = new(CharacterPhysicalInstanceEqualityComparer.Instance);
	}

	private static readonly ConditionalWeakTable<IMovement, ContactState> MovementContacts = new();
	private static readonly AsyncLocal<HashSet<IVehicle>?> ActiveVehicles = new();
	internal const string BarrierReason = "A magical barrier blocks the vehicle's passage.";

	/// <summary>
	/// Returns true when the crossing must stop. Call only after normal movement readiness succeeds.
	/// External characters exclude vehicle occupants; their exposure is selected separately below.
	/// The optional consumed-character set is shared with ordinary dragging movement's contact ledger.
	/// </summary>
	internal static bool Contact(VehicleHitchGraphMovePlan plan, IRoomExit exit,
		IEnumerable<ICharacter> externalCharacters, IMovement? movement, out string reason,
		ISet<ICharacter>? consumedCharacters = null, Func<bool>? continuation = null)
	{
		reason = string.Empty;
		var barriers = exit.Exit.Effects.OfType<IExitBarrierTraversalEffect>().Where(x => x.IsActive).ToArray();
		if (barriers.Length == 0) return false;

		var vehicles = plan.Vehicles.DistinctBy(x => x.Id).ToArray();
		var occupants = vehicles.SelectMany(x => x.Occupants)
			.ToHashSet(CharacterPhysicalInstanceEqualityComparer.Instance);
		var characters = externalCharacters.Where(x => !occupants.Contains(x))
			.Concat(ExposedOccupants(plan))
			.Where(x => ReferenceEquals(x.Location, exit.Origin))
			.Distinct(CharacterPhysicalInstanceEqualityComparer.Instance)
			.ToArray();
		var exteriors = vehicles.Select(x => x.ExteriorItem).Where(x => x is not null)
			.Distinct<IGameItem>(ReferenceEqualityComparer.Instance).ToArray();
		var contactingCharacters = characters.Where(x => !Exempt(x) && barriers.Any(b => b.OccupiesLayerAndPlane(x))).ToArray();
		var contactingItems = exteriors.Where(x => barriers.OfType<IExitBarrierItemTraversalEffect>()
			.Any(b => b.OccupiesLayerAndPlane(x))).ToArray();
		if (contactingCharacters.Length == 0 && contactingItems.Length == 0) return false;

		reason = BarrierReason;
		if (exit.Exit.Door?.IsOpen == false)
		{
			reason = "The door through that exit is closed.";
			return true;
		}
		var active = ActiveVehicles.Value;
		if (active is not null && vehicles.Any(active.Contains)) return true;
		var state = movement is null ? null : MovementContacts.GetValue(movement, _ => new ContactState());
		// Independent dragged trains may share one IMovement. Consume recipients, not the whole movement.
		contactingItems = contactingItems.Where(x => state?.Exteriors.Add(x) ?? true).ToArray();
		contactingCharacters = contactingCharacters.Where(x => state?.Characters.Add(x) ?? true).ToArray();
		if (contactingItems.Length == 0 && contactingCharacters.Length == 0) return true;

		var frame = new Crossing(plan, exit, movement, continuation);
		if (!frame.IsCurrent) return true;
		// Admission is checked for the whole external/contact cohort before the first injury.
		foreach (var character in characters)
		{
			var admission = ExitBarrierTraversal.CanCrossForContact(character, exit);
			if (admission.Success && frame.IsCurrent) continue;
			if (admission.FailureOutput is not null) character.OutputHandler.Handle(admission.FailureOutput);
			return true;
		}

		ActiveVehicles.Value = active is null
			? new HashSet<IVehicle>(vehicles, ReferenceEqualityComparer.Instance)
			: new HashSet<IVehicle>(active.Concat(vehicles), ReferenceEqualityComparer.Instance);
		try
		{
			foreach (var character in contactingCharacters)
			{
				if (!frame.Continue()) break;
				if (consumedCharacters?.ContainsPhysicalInstance(character) == true) continue;
				consumedCharacters?.Add(character);
				ExitBarrierTraversal.ContactForVehicle(character, exit, frame.Continue);
			}

			foreach (var item in contactingItems)
			{
				if (!frame.Continue()) break;
				var layer = item.RoomLayer;
				var plane = item.GetPlanarPresence().Definition.SaveToXml().ToString(SaveOptions.DisableFormatting);
				var effects = item.Effects.ToArray();
				bool CurrentItem() => frame.Continue() && !item.Deleted && !item.Destroyed &&
					ReferenceEquals(item.Location, exit.Origin) && item.RoomLayer == layer &&
					effects.SequenceEqual(item.Effects, ReferenceEqualityComparer.Instance) &&
					item.GetPlanarPresence().Definition.SaveToXml().ToString(SaveOptions.DisableFormatting) == plane;
				foreach (var barrier in barriers.OfType<IExitBarrierItemTraversalEffect>())
				{
					if (!CurrentItem()) break;
					if (!barrier.IsActive || !barrier.OccupiesLayerAndPlane(item)) continue;
					barrier.Contact(item, exit, () => CurrentItem() && barrier.IsActive && barrier.OccupiesLayerAndPlane(item));
				}
			}
		}
		catch (InvalidOperationException)
		{
			// This admitted attempt is consumed; changed physical state cannot turn it into a crossing.
		}
		finally { ActiveVehicles.Value = active; }
		return true;
	}

	private static bool Exempt(ICharacter character) => character.AffectedBy<IImmwalkEffect>() ||
		character.RidingMount?.AffectedBy<IImmwalkEffect>() == true;

	/// <summary>
	/// Only explicitly exposed occupants physically at the exterior location cross its exit.
	/// RoomScale occupants in hosted rooms remain sheltered regardless of this prototype setting.
	/// </summary>
	internal static IEnumerable<ICharacter> ExposedOccupants(VehicleHitchGraphMovePlan plan) => plan.Vehicles
		.Where(x => x.Prototype.ExposesOccupantsToExitBarriers)
		.SelectMany(vehicle => vehicle.Occupants.Where(character =>
			ReferenceEquals(character.Location, vehicle.Location) && character.RoomLayer == vehicle.RoomLayer &&
			!vehicle.Compartments.Any(x => ReferenceEquals(x.InteriorRoom, character.Location))))
		.Distinct(CharacterPhysicalInstanceEqualityComparer.Instance);

	private sealed class Crossing
	{
		private readonly VehicleHitchGraphMovePlan _plan;
		private readonly IRoomExit _exit;
		private readonly IExit _boundary;
		private readonly IRoom _origin;
		private readonly IRoom _destination;
		private readonly IMovement? _movement;
		private readonly Func<bool>? _continuation;
		private readonly IEffect[] _effects;
		private readonly IDoor? _door;
		private readonly bool? _doorOpen;
		private readonly (IVehicle Vehicle, IGameItem Item, RoomLayer Layer, ICharacter[] Occupants)[] _vehicles;

		internal Crossing(VehicleHitchGraphMovePlan plan, IRoomExit exit, IMovement? movement, Func<bool>? continuation)
		{
			_plan = plan; _exit = exit; _boundary = exit.Exit; _origin = exit.Origin; _destination = exit.Destination;
			_movement = movement; _continuation = continuation; _effects = _boundary.Effects.ToArray();
			_door = _boundary.Door; _doorOpen = _door?.IsOpen;
			_vehicles = plan.Vehicles.Select(x => (x, x.ExteriorItem, x.RoomLayer, x.Occupants.ToArray())).ToArray();
		}

		internal bool IsCurrent => (_movement is null || !_movement.Cancelled &&
			_movement.Phase == MovementPhase.OriginalRoom && ReferenceEquals(_movement.Exit, _exit)) &&
			(_continuation?.Invoke() ?? true) && ReferenceEquals(_exit.Exit, _boundary) &&
			ReferenceEquals(_exit.Origin, _origin) && ReferenceEquals(_exit.Destination, _destination) &&
			ReferenceEquals(_boundary.Gameworld, _origin.Gameworld) && ReferenceEquals(_destination.Gameworld, _origin.Gameworld) &&
			ReferenceEquals(_boundary.RoomExitFor(_origin), _exit) && ReferenceEquals(_exit.Opposite?.Exit, _boundary) &&
			ReferenceEquals(_exit.Opposite?.Origin, _destination) && ReferenceEquals(_exit.Opposite?.Destination, _origin) &&
			ReferenceEquals(_boundary.Door, _door) && _door?.IsOpen == _doorOpen &&
			_effects.SequenceEqual(_boundary.Effects, ReferenceEqualityComparer.Instance) &&
			_vehicles.All(x => x.Item is not null && ReferenceEquals(x.Vehicle.ExteriorItem, x.Item) &&
				ReferenceEquals(x.Vehicle.Gameworld, _origin.Gameworld) && ReferenceEquals(x.Item.Gameworld, _origin.Gameworld) &&
				ReferenceEquals(x.Vehicle.Location, _origin) && x.Vehicle.RoomLayer == x.Layer &&
				ReferenceEquals(x.Item.Location, _origin) && x.Item.RoomLayer == x.Layer && !x.Item.Deleted &&
				x.Item.InInventoryOf is null && x.Item.ContainedIn is null &&
				x.Occupants.SequenceEqual(x.Vehicle.Occupants, CharacterPhysicalInstanceEqualityComparer.Instance)) &&
			_plan.Links.All(x => !x.IsDisabled && !x.IsManuallyDisabled);

		internal bool Continue() => IsCurrent && CommandExecutionScope.TryContinue() && IsCurrent;
	}
}
