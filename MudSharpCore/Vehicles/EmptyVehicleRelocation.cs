#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Vehicles;

/// <summary>A prepared relocation of one empty vehicle, retaining its actual interior and cargo.</summary>
internal sealed class EmptyVehicleRelocation : ICurrencyTransferParticipant
{
	private readonly Vehicle _vehicle;
	private readonly GameItem _exterior;
	private readonly Room _origin;
	private readonly Room _destination;
	private readonly RoomLayer _originLayer;
	private readonly bool _unhitch;
	private readonly Func<bool>? _admission;
	private RoomLayer _targetLayer;
	private bool _prepared;
	private bool _committed;
	private IProximityChangeBatch? _proximity;
	private readonly VehicleHitchGraphService _graph = new();
	private readonly VehicleHitchService _hitches = new();

	private EmptyVehicleRelocation(Vehicle vehicle, GameItem exterior, Room origin, Room destination, bool unhitch, Func<bool>? admission)
	{
		_vehicle = vehicle; _exterior = exterior; _origin = origin; _destination = destination;
		_originLayer = vehicle.RoomLayer; _unhitch = unhitch; _admission = admission;
	}

	internal static EmptyVehicleRelocation? Create(IGameItem? exterior, IRoom? destination, bool unhitch, Func<bool>? admission = null)
	{
		return exterior is GameItem item && item.GetItemType<IVehicleExterior>()?.Vehicle is Vehicle vehicle &&
			vehicle.Location is Room origin && destination is Room target && !ReferenceEquals(origin, target) &&
			ReferenceEquals(item.Gameworld, target.Gameworld) && origin.RouteDefinition is null && target.RouteDefinition is null
			? new(vehicle, item, origin, target, unhitch, admission) : null;
	}

	private bool Current => _admission?.Invoke() != false && !_exterior.Deleted && !_exterior.Destroyed && !_vehicle.Destroyed &&
		ReferenceEquals(_vehicle.ExteriorItem, _exterior) && ReferenceEquals(_exterior.GetItemType<IVehicleExterior>()?.Vehicle, _vehicle) &&
		ReferenceEquals(_vehicle.Location, _origin) && ReferenceEquals(_exterior.DirectLocation, _origin) &&
		_exterior.ContainedIn is null && _exterior.InInventoryOf is null &&
		_vehicle.RoomLayer == _originLayer && _exterior.RoomLayer == _originLayer &&
		_vehicle.MovementState.MovementStatus == VehicleMovementStatus.Stationary &&
		_vehicle.LocationType == VehicleLocationType.Room &&
		_vehicle.ActiveJourney is null && !HasLoadedPassengers(_vehicle);

	public bool IsCurrent => _prepared && !_committed && Current &&
		!_graph.LinksInvolving(_vehicle.Gameworld, _exterior).Any();

	public bool Prepare()
	{
		if (_prepared) return IsCurrent;
		if (!Current || HasPersistedPassengers(_vehicle)) return false;
		var links = _graph.LinksInvolving(_vehicle.Gameworld, _exterior);
		// A second vehicle must be uncoupled explicitly; only draft characters are released here.
		if (links.Any(x => x.Source.Vehicle is not null && x.Target.Vehicle is not null)) return false;
		if (!_unhitch && links.Count > 0) return false;
		_proximity = _vehicle.Gameworld.ProximityEventService?.BeginChange(ProximityChangeCause.Movement, _exterior);
		_targetLayer = _destination.PrepareCurrencyInsertionLayer(_exterior);
		if (!Current) return false;
		if (_unhitch) UnhitchCharacters(_vehicle, _hitches);
		_prepared = true;
		return IsCurrent;
	}

	public void Commit()
	{
		_vehicle.CommitPreparedRelocation(_destination, _targetLayer);
		_committed = true;
	}

	public void Notify()
	{
		_vehicle.FinishPreparedRelocation(_origin, _destination);
		_proximity?.Complete();
	}
	public void Dispose() => _proximity?.Dispose();

	internal static bool HasLoadedPassengers(IVehicle vehicle) => vehicle.Occupancies.Any() || vehicle.Occupants.Any() ||
		vehicle.Compartments.Any(x => x.InteriorRoom?.Characters.Any() == true);

	internal static bool HasPersistedPassengers(IVehicle vehicle)
	{
		var rooms = vehicle.Compartments.Select(x => x.InteriorRoomId).OfType<long>().ToArray();
		if (rooms.Length == 0) return false;
		using var db = new FMDB();
		var instances = FMDB.Context.CharacterInstances.AsNoTracking()
			.Where(x => x.LocationId.HasValue && rooms.Contains(x.LocationId.Value) && !x.Character.IsArchived &&
				(x.State & (int)CharacterState.Dead) == 0)
			.Select(x => new { x.Id, x.CharacterId }).ToArray();
		var legacy = FMDB.Context.Characters.AsNoTracking().Where(x => rooms.Contains(x.Location) && !x.IsArchived &&
			(x.State & (int)CharacterState.Dead) == 0 && !x.CharacterInstances.Any(i => i.IsPrimary))
			.Select(x => x.Id).ToArray();
		var loaded = LoadedCharacters(vehicle.Gameworld);
		return PersistedPassengersRemain(rooms, instances.Select(x => (x.Id, x.CharacterId)), legacy, loaded);
	}

	private static IEnumerable<ICharacter> LoadedCharacters(IFuturemud world) => world.Actors.Concat(world.CachedActors)
		.SelectMany(x => x.Identity?.Instances.OfType<ICharacter>() ?? [x]).DistinctPhysicalInstances();

	internal static bool PersistedPassengersRemain(IReadOnlyCollection<long> rooms,
		IEnumerable<(long InstanceId, long CharacterId)> persisted, IEnumerable<long> legacy, IEnumerable<ICharacter> loaded)
	{
		var live = loaded.ToArray();
		return persisted.Any(row => live.FirstOrDefault(x => x.InstanceId == row.InstanceId) is not { } actor ||
			rooms.Contains(actor.Location?.Id ?? 0)) ||
			legacy.Any(id => live.FirstOrDefault(x => CharacterInstanceIdentityComparer.IdentityId(x) == id) is not { } actor ||
				rooms.Contains(actor.Location?.Id ?? 0));
	}

	internal static void UnhitchCharacters(IVehicle vehicle, VehicleHitchService service)
	{
		foreach (var link in service.LinksInvolving(vehicle.Gameworld, vehicle.ExteriorItem).ToArray())
		{
			if (link.SourceType == VehicleHitchEndpointType.Character || link.TargetType == VehicleHitchEndpointType.Character)
				service.DeletePersistentLink(vehicle.Gameworld, link.Id);
		}
		foreach (var actor in LoadedCharacters(vehicle.Gameworld).ToArray())
		{
			actor.RemoveAllEffects<Dragging>(x => ReferenceEquals(x.Target, vehicle.ExteriorItem), true);
			actor.RemoveAllEffects<CharacterHitch>(x => ReferenceEquals(x.Target, vehicle.ExteriorItem), true);
		}
	}
}
