#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Framework.Save;
using PoolModel = MudSharp.Models.RegionalGatheringPool;
using ProfileModel = MudSharp.Models.RegionalGatheringProfile;

namespace MudSharp.Magic.Environment;

/// <summary>One indexed regional life service and one online hourly callback per world.</summary>
public sealed class RegionalGatheringCoordinator(IFuturemud world) : IRegionalGatheringService
{
	private readonly RegionalGatheringRegistry _registry = new();
	private readonly Dictionary<RegionalGatheringOwner, PoolSave> _saves = new();
	private bool _started;
	public IReadOnlyCollection<RegionalGatheringProfile> Profiles => _registry.Profiles;
	public IReadOnlyCollection<RegionalGatheringPool> Pools => _registry.Pools;

	public void Initialise()
	{
		if (_started) return;
		var profiles = new List<RegionalGatheringProfile>();
		PoolModel[] rows;
		using (new FMDB())
		{
			foreach (var row in FMDB.Context.RegionalGatheringProfiles.AsNoTracking())
			{
				try { profiles.Add(RegionalGatheringProfile.Parse(row.Id, row.Name, row.Revision, row.Definition)); }
				catch (Exception error) when (error is FormatException or System.Xml.XmlException or ArgumentException)
				{
					// Keep malformed definitions and their balances intact; resolution fails closed and reports the profile ID.
				}
			}
			rows = FMDB.Context.RegionalGatheringPools.AsNoTracking().ToArray();
		}
		var pools = rows.Select(row => new RegionalGatheringPool(
			new(Enum.Parse<RegionalGatheringOwnerKind>(row.OwnerKind), row.OwnerId), row.ProfileId,
			row.Priority, row.Revision, row.Stock, row.Capacity, row.Initialised) { Inherit = row.Inherit }).ToArray();
		foreach (var pool in pools) _saves.Add(pool.Owner, new PoolSave(this, world, pool.Owner, pool.Revision));
		_registry.Changed += Changed;
		_registry.Load(profiles, pools);
		foreach (var room in world.Rooms) Track(room);
		_registry.CompleteLoading();
		world.HeartbeatManager.HourHeartbeat += RecoverHour;
		_started = true;
	}

	private void Changed(RegionalGatheringOwner owner)
	{
		if (!_saves.TryGetValue(owner, out var save)) _saves.Add(owner, save = new(this, world, owner, 0));
		save.Changed = true;
	}
	private void RecoverHour() => _registry.RecoverHour();
	private void Track(IRoom room)
	{
		if (room.Id <= 0 || !ReferenceEquals(room.Gameworld, world) || room.Temporary || room.OwningZone is null) return;
		_registry.Track(new(room.Id, room.OwningZone.Id, room.OwningAreas.Select(x => x.Id).ToArray(),
			room.CurrentOverlay?.Terrain?.Id ?? 0, room is Room concrete && concrete.EnvironmentBindingMode == EnvironmentalMagicBindingMode.Disabled));
	}
	public void RoomChanged(IRoom room) { if (_started) Track(room); }
	public void Unregister(IRoom room) { if (_started) _registry.Untrack(room.Id); }
	public RegionalGatheringSnapshot? Inspect(IRoom room) => ReferenceEquals(room.Gameworld, world) ? _registry.Inspect(room.Id) : null;
	public string? UnavailableReason(IRoom room) => !ReferenceEquals(room.Gameworld, world)
		? "Regional gathering requires a physical room in this gameworld." : _registry.UnavailableReason(room.Id);
	public bool TryPlan(IRoom room, double units, out RegionalGatheringDebit plan)
	{
		plan = null!;
		return ReferenceEquals(room.Gameworld, world) && _registry.TryPlan(room.Id, units, out plan);
	}
	public bool TryApply(IRoom room, RegionalGatheringDebit plan) => ReferenceEquals(room.Gameworld, world) && room.Id == plan.RoomId && _registry.TryApply(plan);
	public void StageSave(RegionalGatheringOwner owner)
	{
		_saves[owner].PrepareSave();
		_saves[owner].Save();
	}
	public void RecoverSaveFailure(RegionalGatheringOwner owner) => _saves[owner].RecoverFromSaveFailure();
	public void SetBinding(RegionalGatheringOwner owner, long? profileId, int priority, bool inherit = false)
	{
		if (!_started || owner.Kind == RegionalGatheringOwnerKind.Zone && world.Zones.Get(owner.Id) is null ||
			owner.Kind == RegionalGatheringOwnerKind.Area && world.Areas.Get(owner.Id) is null)
			throw new ArgumentException("Select an existing physical zone or area.");
		_registry.SetBinding(owner, profileId, priority, inherit);
	}
	public RegionalGatheringProfile CreateProfile(string name, double contribution, double recovery)
	{
		name = name.Trim();
		var desired = new RegionalGatheringProfile(1, name, 1, contribution, recovery, new Dictionary<long, double>());
		if (name.Length is < 1 or > 200 || !desired.IsValid) throw new ArgumentException("A name and finite non-negative contribution/recovery are required.");
		using (new FMDB())
		{
			var row = new ProfileModel { Name = name, Revision = 1, Definition = desired.Definition() };
			FMDB.Context.RegionalGatheringProfiles.Add(row);
			FMDB.Context.SaveChanges();
			desired = desired with { Id = row.Id };
		}
		_registry.SetProfile(desired);
		return desired;
	}
	public void SetProfile(RegionalGatheringProfile profile)
	{
		if (!profile.IsValid || string.IsNullOrWhiteSpace(profile.Name) || profile.Name.Length > 200)
			throw new ArgumentException("Invalid regional gathering profile.");
		using (new FMDB())
		{
			var row = FMDB.Context.RegionalGatheringProfiles.Find(profile.Id) ?? throw new ArgumentException("Missing regional profile.");
			if (row.Revision != profile.Revision - 1) throw new DbUpdateConcurrencyException("The regional profile was edited elsewhere; reload it.");
			row.Name = profile.Name;
			row.Revision = profile.Revision;
			row.Definition = profile.Definition();
			FMDB.Context.SaveChanges();
		}
		_registry.SetProfile(profile);
	}
	public void Dispose()
	{
		if (_started) world.HeartbeatManager.HourHeartbeat -= RecoverHour;
		_registry.Changed -= Changed;
		_started = false;
	}

	private sealed class PoolSave : SaveableItem, IRecoverableSaveFailure
	{
		private readonly RegionalGatheringCoordinator _coordinator;
		private readonly RegionalGatheringOwner _owner;
		private long _persistedRevision;
		private long _beforeAttempt;
		private bool _staged;
		public PoolSave(RegionalGatheringCoordinator coordinator, IFuturemud gameworld, RegionalGatheringOwner owner, long persistedRevision)
		{
			_coordinator = coordinator;
			_owner = owner;
			Gameworld = gameworld;
			_id = owner.Id;
			_name = owner.ParticipantKey;
			_persistedRevision = persistedRevision;
		}
		public override string FrameworkItemType => "RegionalGatheringPool";
		public void PrepareSave() => _staged = false;
		public override void Save()
		{
			_staged = false;
			_beforeAttempt = _persistedRevision;
			var pool = _coordinator._registry.Pool(_owner)!;
			var row = FMDB.Context.RegionalGatheringPools.Find(_owner.Kind.ToString(), _owner.Id);
			if (row is null)
			{
				if (_persistedRevision != 0) throw new DbUpdateConcurrencyException("The regional pool disappeared; no replacement/refill was created.");
				row = new PoolModel { OwnerKind = _owner.Kind.ToString(), OwnerId = _owner.Id };
				FMDB.Context.RegionalGatheringPools.Add(row);
			}
			else if (row.Revision != _persistedRevision)
				throw new DbUpdateConcurrencyException("The regional pool changed outside this world; retained for review.");
			_beforeAttempt = _persistedRevision;
			row.ProfileId = pool.ProfileId;
			row.Priority = pool.Priority;
			row.Revision = pool.Revision;
			row.Stock = pool.Stock;
			row.Capacity = pool.Capacity;
			row.Initialised = pool.Initialised;
			row.Inherit = pool.Inherit;
			_persistedRevision = pool.Revision;
			_staged = true;
			Changed = false;
		}
		public void RecoverFromSaveFailure()
		{
			if (_staged) _persistedRevision = _beforeAttempt;
			_staged = false;
			Changed = true;
		}
	}
}
