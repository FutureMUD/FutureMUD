#nullable enable

using System;
using System.Collections.Generic;
using System.Collections.Frozen;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Construction;

namespace MudSharp.Magic.Environment;

public enum RegionalGatheringOwnerKind { Zone, Area }

public readonly record struct RegionalGatheringOwner(RegionalGatheringOwnerKind Kind, long Id)
{
	public string ParticipantKey => $"region:{Kind.ToString().ToLowerInvariant()}:{Id}";
}

/// <summary>Reusable ecological life units, not personal mana or a room's forage profile.</summary>
public sealed record RegionalGatheringProfile(long Id, string Name, long Revision,
	double DefaultContribution, double RecoveryPerHour, IReadOnlyDictionary<long, double> TerrainContributions)
{
	public double Contribution(long terrainId) => TerrainContributions.TryGetValue(terrainId, out var value) ? value : DefaultContribution;
	public bool IsValid => Id > 0 && Revision > 0 && double.IsFinite(DefaultContribution) && DefaultContribution >= 0 &&
		double.IsFinite(RecoveryPerHour) && RecoveryPerHour >= 0 && TerrainContributions.All(x => x.Key > 0 && double.IsFinite(x.Value) && x.Value >= 0);
	public string Definition() => new XElement("RegionalGathering", new XAttribute("version", 1),
		new XAttribute("contribution", DefaultContribution.ToString("R", CultureInfo.InvariantCulture)),
		new XAttribute("recovery", RecoveryPerHour.ToString("R", CultureInfo.InvariantCulture)),
		TerrainContributions.OrderBy(x => x.Key).Select(x => new XElement("Terrain", new XAttribute("id", x.Key),
			new XAttribute("life", x.Value.ToString("R", CultureInfo.InvariantCulture))))).ToString();
	public static RegionalGatheringProfile Parse(long id, string name, long revision, string definition)
	{
		var root = XElement.Parse(definition);
		if (root.Name != "RegionalGathering" || (int?)root.Attribute("version") != 1) throw new FormatException("Unsupported regional gathering profile.");
		var profile = new RegionalGatheringProfile(id, name, revision, (double)root.Attribute("contribution")!,
			(double)root.Attribute("recovery")!, root.Elements("Terrain").ToDictionary(x => (long)x.Attribute("id")!, x => (double)x.Attribute("life")!));
		if (!profile.IsValid) throw new FormatException("Regional contributions and recovery must be finite and non-negative.");
		return profile;
	}
}

public sealed record RegionalGatheringPool(RegionalGatheringOwner Owner, long? ProfileId, int Priority,
	long Revision, double Stock, double Capacity, bool Initialised)
{
	public bool Inherit { get; init; }
}
public sealed record RegionalGatheringRoom(long Id, long ZoneId, IReadOnlyList<long> Areas, long TerrainId, bool Disabled = false);
public sealed record RegionalGatheringSnapshot(RegionalGatheringOwner Owner, long ProfileId, long ProfileRevision,
	long Revision, double Stock, double Capacity, int RoomCount);
public sealed record RegionalGatheringDebit(long RoomId, RegionalGatheringSnapshot Source, double Units);

/// <summary>
/// Indexed, simulation-thread ecological ownership. Every room contributes to at most one pool.
/// Pure inspection never regenerates stock. Recovery is called once per online real-hour heartbeat.
/// </summary>
public sealed class RegionalGatheringRegistry
{
	private readonly Dictionary<long, RegionalGatheringProfile> _profiles = new();
	private readonly Dictionary<RegionalGatheringOwner, RegionalGatheringPool> _pools = new();
	private readonly Dictionary<long, RegionalGatheringRoom> _rooms = new();
	private readonly Dictionary<long, (RegionalGatheringOwner Owner, double Contribution)> _assigned = new();
	private readonly Dictionary<RegionalGatheringOwner, HashSet<long>> _members = new();
	private readonly Dictionary<RegionalGatheringOwner, HashSet<long>> _candidates = new();
	private readonly Dictionary<RegionalGatheringOwner, double> _capacities = new();
	private readonly HashSet<RegionalGatheringOwner> _deferred = new();
	private readonly HashSet<RegionalGatheringOwner> _recovering = new();
	private bool _loading = true;
	private bool _batching;
	public event Action<RegionalGatheringOwner>? Changed;
	public IReadOnlyCollection<RegionalGatheringProfile> Profiles => _profiles.Values;
	public IReadOnlyCollection<RegionalGatheringPool> Pools => _pools.Values;
	public RegionalGatheringPool? Pool(RegionalGatheringOwner owner) => _pools.GetValueOrDefault(owner);
	public RegionalGatheringProfile? Profile(long id) => _profiles.GetValueOrDefault(id);

	public void Load(IEnumerable<RegionalGatheringProfile> profiles, IEnumerable<RegionalGatheringPool> pools)
	{
		if (_rooms.Count != 0 || !_loading) throw new InvalidOperationException("Load regional state before indexing rooms.");
		foreach (var profile in profiles) SetProfile(profile);
		foreach (var pool in pools)
		{
			if (pool.Owner.Id <= 0 || !Enum.IsDefined(pool.Owner.Kind) || pool.Revision < 1 ||
				!double.IsFinite(pool.Stock) || pool.Stock < 0 || !double.IsFinite(pool.Capacity) || pool.Capacity < 0)
				throw new FormatException("Invalid saved regional ecological state; repair it explicitly.");
			_pools.Add(pool.Owner, pool);
		}
	}

	public void CompleteLoading()
	{
		_loading = false;
		foreach (var owner in _pools.Keys.ToArray()) Recalculate(owner);
	}

	public void SetProfile(RegionalGatheringProfile profile)
	{
		if (!profile.IsValid) throw new ArgumentException("Invalid regional gathering profile.");
		if (!_loading && _profiles.TryGetValue(profile.Id, out var old) && profile.Revision <= old.Revision)
			throw new ArgumentException("Profile edits require a newer revision.");
		_profiles[profile.Id] = profile with { TerrainContributions = profile.TerrainContributions.ToFrozenDictionary() };
		if (_loading) return;
		_batching = true;
		foreach (var owner in _pools.Values.Where(x => x.ProfileId == profile.Id).Select(x => x.Owner).ToArray())
		{
			foreach (var id in Candidates(owner).ToArray()) Reassign(id);
			Recalculate(owner, true);
		}
		FinishBatch();
	}

	public void SetBinding(RegionalGatheringOwner owner, long? profileId, int priority, bool inherit = false)
	{
		if (_loading || owner.Id <= 0 || !Enum.IsDefined(owner.Kind) || profileId.HasValue && !_profiles.ContainsKey(profileId.Value))
			throw new ArgumentException("A loaded native region and valid profile are required.");
		var old = Pool(owner);
		_pools[owner] = (old is null ? new(owner, profileId, priority, 1, 0, 0, false)
			: old with { ProfileId = profileId, Priority = priority, Revision = old.Revision + 1 }) with { Inherit = inherit };
		_batching = true;
		foreach (var id in Candidates(owner).ToArray()) Reassign(id);
		Recalculate(owner, true);
		FinishBatch();
	}
	private void FinishBatch()
	{
		_batching = false;
		foreach (var owner in _deferred.ToArray()) Recalculate(owner, true);
		_deferred.Clear();
	}

	public void Track(RegionalGatheringRoom room)
	{
		if (room.Id <= 0 || room.ZoneId <= 0) return;
		room = room with { Areas = Array.AsReadOnly(room.Areas.Distinct().ToArray()) };
		if (_rooms.TryGetValue(room.Id, out var old)) RemoveCandidate(old);
		_rooms[room.Id] = room;
		foreach (var owner in Owners(room)) Candidates(owner).Add(room.Id);
		Reassign(room.Id);
	}

	public void Untrack(long roomId)
	{
		if (!_rooms.Remove(roomId, out var room)) return;
		RemoveCandidate(room);
		if (_assigned.Remove(roomId, out var previous))
		{
			Members(previous.Owner).Remove(roomId);
			_capacities[previous.Owner] = Math.Max(0, _capacities.GetValueOrDefault(previous.Owner) - previous.Contribution);
			Recalculate(previous.Owner);
		}
	}

	private void RemoveCandidate(RegionalGatheringRoom room)
	{
		foreach (var owner in Owners(room)) Candidates(owner).Remove(room.Id);
	}
	private static IEnumerable<RegionalGatheringOwner> Owners(RegionalGatheringRoom room) =>
		room.Areas.Select(x => new RegionalGatheringOwner(RegionalGatheringOwnerKind.Area, x))
			.Append(new(RegionalGatheringOwnerKind.Zone, room.ZoneId));
	private HashSet<long> Candidates(RegionalGatheringOwner owner) => GetSet(_candidates, owner);
	private HashSet<long> Members(RegionalGatheringOwner owner) => GetSet(_members, owner);
	private static HashSet<long> GetSet(Dictionary<RegionalGatheringOwner, HashSet<long>> map, RegionalGatheringOwner owner)
	{
		if (!map.TryGetValue(owner, out var set)) map.Add(owner, set = new());
		return set;
	}
	private RegionalGatheringOwner? Resolve(RegionalGatheringRoom room)
	{
		if (room.Disabled) return null;
		return room.Areas.Select(x => new RegionalGatheringOwner(RegionalGatheringOwnerKind.Area, x))
			.Where(x => _pools.TryGetValue(x, out var pool) && !pool.Inherit).OrderByDescending(x => _pools[x].Priority).ThenBy(x => x.Id)
			.Cast<RegionalGatheringOwner?>().FirstOrDefault()
			?? (_pools.TryGetValue(new(RegionalGatheringOwnerKind.Zone, room.ZoneId), out var zone) && !zone.Inherit ? new(RegionalGatheringOwnerKind.Zone, room.ZoneId) : null);
	}
	private void Reassign(long roomId)
	{
		var room = _rooms[roomId];
		var selected = Resolve(room);
		var next = selected.HasValue && _pools[selected.Value].ProfileId is { } profileId && _profiles.TryGetValue(profileId, out var profile)
			? ((RegionalGatheringOwner Owner, double Contribution)?)(selected.Value, profile.Contribution(room.TerrainId)) : null;
		var hadPrevious = _assigned.Remove(roomId, out var previous);
		if (hadPrevious)
		{
			Members(previous.Owner).Remove(roomId);
			_capacities[previous.Owner] = Math.Max(0, _capacities.GetValueOrDefault(previous.Owner) - previous.Contribution);
		}
		if (next.HasValue)
		{
			_assigned.Add(roomId, next.Value);
			Members(next.Value.Owner).Add(roomId);
			_capacities[next.Value.Owner] = _capacities.GetValueOrDefault(next.Value.Owner) + next.Value.Contribution;
		}
		if (!_loading && hadPrevious && (!next.HasValue || previous.Owner != next.Value.Owner)) Recalculate(previous.Owner);
		if (!_loading && next.HasValue) Recalculate(next.Value.Owner, hadPrevious && previous != next.Value);
	}
	private void Recalculate(RegionalGatheringOwner owner, bool force = false)
	{
		if (_loading || !_pools.TryGetValue(owner, out var pool)) return;
		if (pool.ProfileId is { } missing && !_profiles.ContainsKey(missing)) return;
		if (_batching) { _deferred.Add(owner); return; }
		var capacity = _capacities.GetValueOrDefault(owner);
		if (!double.IsFinite(capacity)) throw new InvalidOperationException("Regional ecological capacity overflow.");
		var stock = !pool.ProfileId.HasValue || pool.Inherit ? pool.Stock : !pool.Initialised ? capacity : Math.Min(pool.Stock, capacity);
		var updated = pool with { Capacity = capacity, Stock = stock, Initialised = pool.Initialised || pool.ProfileId.HasValue };
		if (force || updated != pool)
		{
			_pools[owner] = updated with { Revision = pool.Revision + 1 };
			Changed?.Invoke(owner);
		}
		RefreshRecovery(owner);
	}
	private void RefreshRecovery(RegionalGatheringOwner owner)
	{
		var pool = _pools[owner];
		if (!pool.Inherit && pool.ProfileId is { } id && _profiles.TryGetValue(id, out var profile) && profile.RecoveryPerHour > 0 && pool.Stock < pool.Capacity)
			_recovering.Add(owner);
		else _recovering.Remove(owner);
	}
	public RegionalGatheringSnapshot? Inspect(long roomId)
	{
		if (!_assigned.TryGetValue(roomId, out var assignment)) return null;
		var pool = _pools[assignment.Owner];
		var profile = _profiles[pool.ProfileId!.Value];
		return new(pool.Owner, profile.Id, profile.Revision, pool.Revision, pool.Stock, pool.Capacity, Members(pool.Owner).Count);
	}
	public string? UnavailableReason(long roomId)
	{
		if (!_rooms.TryGetValue(roomId, out var room)) return "The physical room is not indexed for regional gathering.";
		if (room.Disabled) return "This room explicitly disables environmental gathering.";
		var owner = Resolve(room);
		return owner.HasValue && _pools[owner.Value].ProfileId is { } id && !_profiles.ContainsKey(id)
			? $"Regional gathering profile #{id} is missing or invalid; its stock is retained for repair." : null;
	}
	public bool TryPlan(long roomId, double units, out RegionalGatheringDebit plan)
	{
		plan = null!;
		var snapshot = Inspect(roomId);
		if (snapshot is null || !double.IsFinite(units) || units <= 0 || units > snapshot.Stock) return false;
		plan = new(roomId, snapshot, units);
		return true;
	}
	public bool TryApply(RegionalGatheringDebit plan)
	{
		if (!TryPlan(plan.RoomId, plan.Units, out var current) || current != plan) return false;
		var pool = _pools[plan.Source.Owner];
		_pools[pool.Owner] = pool with { Stock = pool.Stock - plan.Units, Revision = pool.Revision + 1 };
		RefreshRecovery(pool.Owner);
		Changed?.Invoke(pool.Owner);
		return true;
	}
	public void RecoverHour()
	{
		foreach (var owner in _recovering.ToArray())
		{
			var pool = _pools[owner];
			var profile = _profiles[pool.ProfileId!.Value];
			var stock = Math.Min(pool.Capacity, AddRecovery(pool.Stock, profile.RecoveryPerHour));
			if (!double.IsFinite(stock)) throw new InvalidOperationException("Regional ecological recovery overflow.");
			_pools[owner] = pool with { Stock = stock, Revision = pool.Revision + 1 };
			RefreshRecovery(owner);
			Changed?.Invoke(owner);
		}
	}
	private static double AddRecovery(double stock, double recovery)
	{
		// Ordinary fractional rates must reach a usable point: ten .1-hour increments are one life.
		// Preserve the full double range for profiles whose values cannot round-trip through decimal.
		try
		{
			var exactStock = (decimal)stock;
			var exactRecovery = (decimal)recovery;
			if ((double)exactStock == stock && (double)exactRecovery == recovery)
				return (double)(exactStock + exactRecovery);
		}
		catch (OverflowException) { }
		return stock + recovery;
	}
}

public interface IRegionalGatheringService : IDisposable
{
	IReadOnlyCollection<RegionalGatheringProfile> Profiles { get; }
	IReadOnlyCollection<RegionalGatheringPool> Pools { get; }
	RegionalGatheringSnapshot? Inspect(IRoom room);
	string? UnavailableReason(IRoom room);
	bool TryPlan(IRoom room, double units, out RegionalGatheringDebit plan);
	bool TryApply(IRoom room, RegionalGatheringDebit plan);
	void StageSave(RegionalGatheringOwner owner);
	void RecoverSaveFailure(RegionalGatheringOwner owner);
	void SetBinding(RegionalGatheringOwner owner, long? profileId, int priority, bool inherit = false);
	RegionalGatheringProfile CreateProfile(string name, double contribution, double recovery);
	void SetProfile(RegionalGatheringProfile profile);
	void RoomChanged(IRoom room);
	void Unregister(IRoom room);
	void Initialise();
}
