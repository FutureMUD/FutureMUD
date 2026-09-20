#nullable enable

using System.Collections.ObjectModel;
using System.Diagnostics;
using MudSharp.Construction;
using MudSharp.FutureProg;
using MudSharp.Work.Agriculture;

namespace MudSharp.Magic.Environment;

/// <summary>
/// One main-loop callback per world. Native invalidations coalesce; accepted samples apply until the
/// next real evaluation. All changed samples start prospectively, and inspections never accept samples.
/// </summary>
public sealed partial class EnvironmentalMagicCoordinator : IEnvironmentalMagicService
{
	private sealed class Registration(Cell cell, long profileId)
	{
		public Cell Cell { get; } = cell;
		public long ProfileId { get; } = profileId;
		public double DueAt;
		public double AuditAt;
		public double LastAudit;
		public double SampleAt;
		public double DirtyAt;
		public double RepairRate;
		public double RepairRemainder;
		public Dictionary<long, double>? ProductionRemainders;
		public long DefinitionRevision;
		public bool Production;
		public bool Maintenance;
		public bool Faulted;
		public EnvironmentalMagicDirtyReason Dirty;
		public LinkedListNode<Registration>? DirtyNode;
		public LinkedListNode<Registration>? AgeNode;
		public IReadOnlyList<EnvironmentalResourceSnapshot> Sample = Array.Empty<EnvironmentalResourceSnapshot>();
	}

	private readonly IFuturemud _world;
	private readonly TimeProvider _clock;
	private readonly long _epoch;
	private readonly IEnvironmentalMagicOperationStore _operations;
	private readonly Dictionary<long, Registration> _registered = new();
	private readonly Dictionary<long, LinkedListNode<Cell>> _cells = new();
	private readonly LinkedList<Cell> _cellOrder = new();
	private readonly Dictionary<long, IAgricultureField> _fields = new();
	private readonly Dictionary<long, IAgricultureField> _apiaryFields = new();
	private readonly Dictionary<long, long> _referenceGenerations = new();
	private readonly SortedSet<Registration> _due = new(Comparer<Registration>.Create((a, b) =>
		a.DueAt.CompareTo(b.DueAt) is var result && result != 0 ? result : a.Cell.Id.CompareTo(b.Cell.Id)));
	private readonly SortedSet<Registration> _audit = new(Comparer<Registration>.Create((a, b) =>
		a.AuditAt.CompareTo(b.AuditAt) is var result && result != 0 ? result : a.Cell.Id.CompareTo(b.Cell.Id)));
	private readonly LinkedList<Registration> _dirty = new();
	private readonly LinkedList<Registration> _auditAge = new();
	private readonly HashSet<long> _evaluating = new();
	private readonly HashSet<long> _recursive = new();
	private readonly Dictionary<long, double> _lastLoggedFault = new();
	private readonly Dictionary<long, double> _lastLoggedSlow = new();
	private LinkedListNode<Cell>? _discoveryCursor;
	private int _discoveryRemaining;
	private long _sourceGeneration;
	private bool _started;
	private bool _disposed;
	private bool _pumping;
	private int _lane;
	private int _productionCount;
	private int _maintenanceCount;
	private int _faultCount;
	private int _workingCount;
	private int _lastVisits;
	private int _lastEvaluations;
	private int _lastProgExecutions;
	private int _lastWrites;
	private double _lastPumpMilliseconds;
	private double _maximumPumpMilliseconds;
	private long _budgetLimitedPumps;
	private long _totalEvaluations;
	private long _totalWrites;
	private long _totalProgExecutions;
	private long _totalFaults;
	private long _totalSlowProgs;
	private string? _representativeError;
	private string? _slowProg;

	public EnvironmentalMagicCoordinator(IFuturemud world, TimeProvider? clock = null,
		EnvironmentalMagicOptions? options = null, IEnvironmentalMagicOperationStore? operations = null)
	{
		_world = world;
		_clock = clock ?? TimeProvider.System;
		_epoch = _clock.GetTimestamp();
		Options = options ?? new EnvironmentalMagicOptions();
		Options.Validate();
		_operations = operations ?? new DatabaseEnvironmentalMagicOperationStore();
	}

	public EnvironmentalMagicOptions Options { get; private set; }
	public DateTimeOffset UtcNow => _clock.GetUtcNow();
	private double Now => _clock.GetElapsedTime(_epoch).TotalSeconds;

	public void Initialise()
	{
		if (_started || _disposed) return;
		foreach (var field in _world.AgricultureFields)
		{
			_fields[field.Cell.Id] = field;
			RefreshPollinationCandidate(field);
		}
		foreach (var cell in _world.Cells) Register(cell);
		_world.HeartbeatManager.SecondHeartbeat += Pump;
		_started = true;
	}

	public void Configure(EnvironmentalMagicOptions options)
	{
		options.Validate();
		Options = options;
		BeginDiscovery();
	}

	private long? EffectiveProfileId(Cell cell) => cell.Id <= 0 || !ReferenceEquals(cell.Gameworld, _world)
		? null : cell.EnvironmentBindingMode switch
	{
		EnvironmentalMagicBindingMode.Inherit => cell.CurrentOverlay?.Terrain?.EnvironmentalMagicProfileId,
		EnvironmentalMagicBindingMode.Explicit => cell.EnvironmentalMagicProfileId,
		_ => null
	};

	private IEnvironmentalMagicProfile? Profile(long id)
	{
		var profile = _world.MagicResourceRegenerators.Get(id) as IEnvironmentalMagicProfile;
		if (profile is not null && _referenceGenerations.GetValueOrDefault(id, -1) != _sourceGeneration)
		{
			profile.RefreshReferences();
			_referenceGenerations[id] = _sourceGeneration;
		}
		return profile;
	}

	public void Register(ICell cell)
	{
		if (_disposed || cell is not Cell concrete || cell.Id <= 0 || !ReferenceEquals(cell.Gameworld, _world)) return;
		if (_cells.TryGetValue(cell.Id, out var previous) && !ReferenceEquals(previous.Value, concrete))
			Unregister(previous.Value);
		if (!_cells.ContainsKey(cell.Id)) _cells[cell.Id] = _cellOrder.AddLast(concrete);
		var id = EffectiveProfileId(concrete);
		if (_registered.TryGetValue(cell.Id, out var existing))
		{
			if (id == existing.ProfileId) return;
			Settle(existing, Now);
			SettlePressure(concrete, null);
			RemoveRegistration(existing);
		}
		if (!id.HasValue) return;
		var now = Now;
		var registration = new Registration(concrete, id.Value)
		{
			SampleAt = now,
			LastAudit = now,
			DueAt = now + Stagger(cell.Id, Options.ActiveCadenceSeconds),
			AuditAt = now + Stagger(cell.Id, Options.ReconciliationSeconds)
		};
		registration.AgeNode = _auditAge.AddLast(registration);
		_registered.Add(cell.Id, registration);
		_due.Add(registration);
		_audit.Add(registration);
	}

	private static double Stagger(long id, double interval) =>
		1.0 + (unchecked((ulong)id * 11400714819323198485UL) % 1000000UL) / 1000000.0 * Math.Max(0.0, interval - 1.0);

	public void Unregister(ICell cell)
	{
		if (!_cells.TryGetValue(cell.Id, out var indexed) || !ReferenceEquals(indexed.Value, cell)) return;
		if (_registered.Remove(cell.Id, out var registration)) RemoveRegistration(registration);
		if (_cells.Remove(cell.Id, out var node))
		{
			if (_discoveryCursor == node) _discoveryCursor = node.Next ?? _cellOrder.First;
			_cellOrder.Remove(node);
			if (_cellOrder.Count == 0) _discoveryCursor = null;
		}
		_fields.Remove(cell.Id);
	}

	private void RemoveRegistration(Registration registration)
	{
		_due.Remove(registration);
		_audit.Remove(registration);
		if (registration.DirtyNode is not null) _dirty.Remove(registration.DirtyNode);
		if (registration.AgeNode is not null) _auditAge.Remove(registration.AgeNode);
		if (registration.Production) _productionCount--;
		if (registration.Maintenance) _maintenanceCount--;
		if (registration.Faulted) _faultCount--;
		if (registration.Production || registration.Maintenance) _workingCount--;
		_registered.Remove(registration.Cell.Id);
	}

	public void SetBinding(ICell cell, EnvironmentalMagicBindingMode mode, long? profileId)
	{
		if (cell is not Cell concrete || cell.Id <= 0 || !ReferenceEquals(cell.Gameworld, _world) || !Enum.IsDefined(mode) ||
			mode == EnvironmentalMagicBindingMode.Explicit && !profileId.HasValue)
			throw new ArgumentException("A physical cell and a valid environmental binding are required.");
		if (_evaluating.Contains(cell.Id)) throw new InvalidOperationException("Environmental input progs must be read-only.");
		if (concrete.PendingEnvironmentalOperationId is { } pending)
			throw new InvalidOperationException($"Environmental operation {pending} must be confirmed before changing its binding.");
		if (mode != EnvironmentalMagicBindingMode.Explicit) profileId = null;
		if (concrete.EnvironmentBindingMode == mode && concrete.EnvironmentalMagicProfileId == profileId) return;
		if (_registered.TryGetValue(cell.Id, out var old)) Settle(old, Now);
		SettlePressure(concrete, null);
		concrete.SetEnvironmentBinding(mode, profileId);
		Register(concrete);
		if (_registered.TryGetValue(cell.Id, out var next)) Recheck(next, Now);
	}

	public void CellTerrainChanged(ICell cell)
	{
		if (_disposed) return;
		if (_registered.TryGetValue(cell.Id, out var old)) Settle(old, Now);
		Register(cell);
		if (_registered.TryGetValue(cell.Id, out var next)) Recheck(next, Now);
	}

	public void TerrainDefaultChanged(ITerrain terrain) => BeginDiscovery();
	public void BeforeProfileChange(IEnvironmentalMagicProfile profile) { }
	public void ProfileChanged(IEnvironmentalMagicProfile profile) => BeginDiscovery();
	public void SourceDefinitionChanged()
	{
		_sourceGeneration++;
		BeginDiscovery();
	}

	private void BeginDiscovery()
	{
		_discoveryCursor ??= _cellOrder.First;
		_discoveryRemaining = _cellOrder.Count;
	}

	public IAgricultureField? FieldFor(ICell cell) => ReferenceEquals(cell.Gameworld, _world)
		? _fields.GetValueOrDefault(cell.Id) : null;

	public IEnumerable<IAgricultureField> PollinationCandidates() => _apiaryFields.Values;

	public void RefreshPollinationCandidate(IAgricultureField field)
	{
		if (!ReferenceEquals(field.Cell.Gameworld, _world)) return;
		if (field.HasActiveApiary) _apiaryFields[field.Cell.Id] = field;
		else if (_apiaryFields.GetValueOrDefault(field.Cell.Id) == field) _apiaryFields.Remove(field.Cell.Id);
	}

	public void FieldChanged(IAgricultureField field, bool removed = false)
	{
		if (_disposed || !ReferenceEquals(field.Cell.Gameworld, _world)) return;
		if (removed)
		{
			if (_fields.GetValueOrDefault(field.Cell.Id) == field) _fields.Remove(field.Cell.Id);
			if (_apiaryFields.GetValueOrDefault(field.Cell.Id) == field) _apiaryFields.Remove(field.Cell.Id);
		}
		else
		{
			_fields[field.Cell.Id] = field;
			RefreshPollinationCandidate(field);
		}
		MarkDirty(field.Cell, EnvironmentalMagicDirtyReason.Agriculture);
	}

	public void MarkDirty(ICell cell, EnvironmentalMagicDirtyReason reason)
	{
		if (_evaluating.Contains(cell.Id)) { _recursive.Add(cell.Id); return; }
		if (_disposed || !_registered.TryGetValue(cell.Id, out var registration) || !ReferenceEquals(registration.Cell, cell)) return;
		if (reason is EnvironmentalMagicDirtyReason.Forage or EnvironmentalMagicDirtyReason.Agriculture)
		{
			var profile = _world.MagicResourceRegenerators.Get(registration.ProfileId) as IEnvironmentalMagicProfile;
			var kind = reason == EnvironmentalMagicDirtyReason.Forage ? EnvironmentalMagicInputKind.Forage : EnvironmentalMagicInputKind.Agriculture;
			if (profile is not null)
			{
				var manaDepends = profile.Inputs.Any(x =>
					(x.Kind == kind || x.Kind == EnvironmentalMagicInputKind.Prog) && profile.RequiredInputNames.Contains(x.Name));
				var organicDepends = profile.OrganicSources.Any(x => reason == EnvironmentalMagicDirtyReason.Forage
					? x.Kind == NativeOrganicSourceKind.Forage
					: x.Kind is NativeOrganicSourceKind.Crop or NativeOrganicSourceKind.Woodland or NativeOrganicSourceKind.Pasture) ||
					profile.OrganicPenalties.Any(penalty => penalty.RequiredInputNames.Any(name =>
						profile.Inputs.Any(input => input.Name.EqualTo(name) &&
							(input.Kind == kind || input.Kind == EnvironmentalMagicInputKind.Prog))));
				if (!manaDepends && !organicDepends) return;
			}
		}
		registration.Dirty |= reason;
		if (registration.DirtyNode is not null) return;
		registration.DirtyAt = Now;
		registration.DirtyNode = _dirty.AddLast(registration);
	}

	public void Pump()
	{
		if (_disposed || _pumping) return;
		_pumping = true;
		var watch = Stopwatch.StartNew();
		_lastVisits = _lastEvaluations = _lastProgExecutions = _lastWrites = 0;
		var outputWork = 0;
		try
		{
			var emptyLanes = 0;
			while (_lastVisits < Options.MaximumCellVisits && outputWork + 8 <= Options.MaximumOutputWork)
			{
				var now = Now;
				Registration? work = null;
				var discovered = false;
				var lane = _lane;
				_lane = (_lane + 1) % 4;
				switch (lane)
				{
					case 0 when _due.Min is { } due && due.DueAt <= now:
						work = due;
						break;
					case 1 when _dirty.First is { } dirty:
						work = dirty.Value;
						break;
					case 2 when _audit.Min is { } audit && audit.AuditAt <= now:
						work = audit;
						break;
					case 3 when _discoveryRemaining > 0 && _discoveryCursor is not null:
						var cell = _discoveryCursor.Value;
						_discoveryCursor = _discoveryCursor.Next ?? _cellOrder.First;
						_discoveryRemaining--;
						Register(cell);
						_registered.TryGetValue(cell.Id, out work);
						discovered = true;
						break;
				}
				if (work is null && !discovered)
				{
					if (++emptyLanes >= 4) break;
					continue;
				}
				emptyLanes = 0;
				_lastVisits++;
				if (work is not null)
				{
					try
					{
						outputWork += Math.Clamp(Profile(work.ProfileId)?.Outputs.Count ?? 1, 1, 8);
						Recheck(work, now, keepDeadline: lane != 0);
					}
					catch (Exception ex) { Fault(work, ex.Message, now); }
				}
				if (watch.Elapsed.TotalMilliseconds >= Options.SoftBudgetMilliseconds) break;
			}
			if (HasReadyWork(Now)) _budgetLimitedPumps++;
		}
		finally
		{
			_lastPumpMilliseconds = watch.Elapsed.TotalMilliseconds;
			_maximumPumpMilliseconds = Math.Max(_maximumPumpMilliseconds, _lastPumpMilliseconds);
			_pumping = false;
		}
	}

	private bool HasReadyWork(double now) => _dirty.Count > 0 || _discoveryRemaining > 0 ||
		_due.Min is { } due && due.DueAt <= now || _audit.Min is { } audit && audit.AuditAt <= now;

	public void Dispose()
	{
		if (_disposed) return;
		if (_started) _world.HeartbeatManager.SecondHeartbeat -= Pump;
		_disposed = true;
		_registered.Clear();
		_cells.Clear();
		_cellOrder.Clear();
		_fields.Clear();
		_apiaryFields.Clear();
		_referenceGenerations.Clear();
		_due.Clear();
		_audit.Clear();
		_dirty.Clear();
		_auditAge.Clear();
		_lastLoggedFault.Clear();
		_lastLoggedSlow.Clear();
		_lastLoggedOrganic.Clear();
		_discoveryCursor = null;
		_discoveryRemaining = _productionCount = _maintenanceCount = _faultCount = _workingCount = 0;
	}
}
