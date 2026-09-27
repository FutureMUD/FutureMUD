#nullable enable

using MudSharp.Database;
using MudSharp.Magic.Environment;

namespace MudSharp.Construction;

public partial class Cell
{
	public EnvironmentalMagicBindingMode EnvironmentBindingMode { get; private set; }
	public long? EnvironmentalMagicProfileId { get; private set; }
	public EnvironmentalMagicState EnvironmentState { get; private set; } = EnvironmentalMagicState.Empty;
	public Guid? PendingEnvironmentalOperationId { get; private set; }
	internal long? ExpectedEnvironmentDatabaseRevision { get; private set; }
	private bool _environmentStateChanged;

	internal void LoadEnvironment(Models.Cell cell)
	{
		EnvironmentBindingMode = (EnvironmentalMagicBindingMode)cell.EnvironmentalMagicBindingMode;
		EnvironmentalMagicProfileId = cell.EnvironmentalMagicProfileId;
		var state = cell.EnvironmentalState;
		EnvironmentState = ReadEnvironmentState(state);
		ExpectedEnvironmentDatabaseRevision = state?.Revision;
		PendingEnvironmentalOperationId = null;
		_environmentStateChanged = false;
	}

	internal static EnvironmentalMagicState ReadEnvironmentState(Models.CellEnvironmentalState? state) =>
		state is null ? EnvironmentalMagicState.Empty : new EnvironmentalMagicState
		{
			SchemaVersion = state.SchemaVersion,
			Revision = state.Revision,
			ScarDamage = state.ScarDamage,
			LastDefileUtc = AsUtc(state.LastDefileUtc),
			RecentPressure = state.RecentPressure,
			PressureReferenceUtc = AsUtc(state.PressureReferenceUtc),
			PressureHalfLifeSeconds = state.PressureHalfLifeSeconds,
			PressureProfileId = state.PressureProfileId,
			PressureDecayAnchor = state.PressureDecayAnchor
		};

	private static DateTimeOffset? AsUtc(DateTime? value) => value.HasValue
		? new DateTimeOffset(System.DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)) : null;

	internal void SetEnvironmentBinding(EnvironmentalMagicBindingMode mode, long? profileId)
	{
		if (mode == EnvironmentBindingMode && profileId == EnvironmentalMagicProfileId) return;
		EnvironmentBindingMode = mode;
		EnvironmentalMagicProfileId = profileId;
		Changed = true;
	}

	internal bool SetEnvironmentState(EnvironmentalMagicState state)
	{
		if (PendingEnvironmentalOperationId.HasValue) return false;
		if (state == EnvironmentState) return false;
		EnvironmentState = state;
		_environmentStateChanged = true;
		Changed = true;
		if (state.ScarDamage == 0.0) Gameworld.EnvironmentalMagic?.ScarStateChanged(this);
		return true;
	}

	/// <summary>Coordinator-only balance boundary; no recursive cap query or dirty notification.</summary>
	internal bool SetEnvironmentalResource(Magic.IMagicResource resource, double amount, bool ensurePresent = false)
	{
		if (PendingEnvironmentalOperationId.HasValue) return false;
		if (!double.IsFinite(amount) || amount < 0.0)
			throw new ArgumentOutOfRangeException(nameof(amount));
		var exists = _magicResourceAmounts.ContainsKey(resource);
		if (exists && _magicResourceAmounts[resource] == amount || !exists && amount == 0.0 && !ensurePresent)
			return false;
		_magicResourceAmounts[resource] = amount;
		ResourcesChanged = true;
		return true;
	}

	internal void SaveEnvironment(Models.Cell cell)
	{
		cell.EnvironmentalMagicBindingMode = (int)EnvironmentBindingMode;
		cell.EnvironmentalMagicProfileId = EnvironmentalMagicProfileId;
		if (PendingEnvironmentalOperationId.HasValue) return;
		if (!_environmentStateChanged) return;
		if (cell.EnvironmentalState is null)
		{
			if (ExpectedEnvironmentDatabaseRevision.HasValue)
				throw new InvalidOperationException("The persisted environmental state disappeared; reload before saving it.");
			cell.EnvironmentalState = new Models.CellEnvironmentalState { CellId = Id, Cell = cell };
		}
		else
		{
			if (!ExpectedEnvironmentDatabaseRevision.HasValue)
				throw new InvalidOperationException("An unexpected persisted environmental state exists; reload before saving it.");
			// Compare against the cell's known persistence baseline, not whichever revision a fresh query returned.
			if (FMDB.Context is { } context)
				context.Entry(cell.EnvironmentalState).Property(x => x.Revision).OriginalValue =
					ExpectedEnvironmentDatabaseRevision.Value;
			else if (cell.EnvironmentalState.Revision != ExpectedEnvironmentDatabaseRevision.Value)
				throw new InvalidOperationException("The model's environmental revision changed; reload before saving it.");
		}
		CopyEnvironmentState(EnvironmentState, cell.EnvironmentalState);
		// SaveManager owns the enclosing commit. If it fails, a later explicit operation rejects a
		// database mismatch rather than silently overwriting it from this staged in-memory state.
		ExpectedEnvironmentDatabaseRevision = EnvironmentState.Revision;
		_environmentStateChanged = false;
	}

	internal void BeginEnvironmentalOperation(Guid operationId)
	{
		if (operationId == Guid.Empty) throw new ArgumentException("An operation identity is required.", nameof(operationId));
		if (PendingEnvironmentalOperationId is { } pending && pending != operationId)
			throw new InvalidOperationException($"Environmental operation {pending} must be resolved first.");
		PendingEnvironmentalOperationId = operationId;
	}

	internal void CancelUncommittedEnvironment(Guid operationId)
	{
		RequirePendingEnvironmentOperation(operationId);
		PendingEnvironmentalOperationId = null;
		// The failed candidate never became live state. Preserve and requeue any older deferred work.
		if (_environmentStateChanged || _resourcesChanged) Changed = true;
	}

	internal void AdoptCommittedEnvironment(Guid operationId, EnvironmentalMagicState state,
		IReadOnlyDictionary<Magic.IMagicResource, double>? resourceAmounts = null)
	{
		RequireMatchingEnvironmentOperation(operationId);
		var balances = resourceAmounts?.ToArray();
		EnvironmentState = state;
		ExpectedEnvironmentDatabaseRevision = state.Revision;
		_environmentStateChanged = false;
		if (balances is not null)
		{
			foreach (var balance in balances) _magicResourceAmounts[balance.Key] = balance.Value;
		}
		_resourcesChanged = false;
		PendingEnvironmentalOperationId = null;
	}

	internal void AdoptDurableEnvironment(Guid operationId, StoredEnvironmentalMagicState stored)
	{
		RequirePendingEnvironmentOperation(operationId);
		EnvironmentState = stored.State;
		ExpectedEnvironmentDatabaseRevision = stored.HasState ? stored.State.Revision : null;
		_environmentStateChanged = false;
		_magicResourceAmounts.Clear();
		_pendingMagicResourceAmounts.Clear();
		foreach (var balance in stored.ResourceAmounts)
		{
			if (Gameworld.MagicResources.Get(balance.Key) is { } resource) _magicResourceAmounts[resource] = balance.Value;
			else _pendingMagicResourceAmounts[balance.Key] = balance.Value;
		}
		_resourcesChanged = false;
		PendingEnvironmentalOperationId = null;
	}

	private void RequirePendingEnvironmentOperation(Guid operationId)
	{
		if (PendingEnvironmentalOperationId != operationId)
			throw new InvalidOperationException("Only the pending environmental operation may release its persistence freeze.");
	}

	private void RequireMatchingEnvironmentOperation(Guid operationId)
	{
		if (operationId == Guid.Empty || PendingEnvironmentalOperationId is { } pending && pending != operationId)
			throw new InvalidOperationException("The committed environmental operation does not match the pending identity.");
	}

	internal static void CopyEnvironmentState(EnvironmentalMagicState source, Models.CellEnvironmentalState target)
	{
		target.SchemaVersion = source.SchemaVersion;
		target.Revision = source.Revision;
		target.ScarDamage = source.ScarDamage;
		target.LastDefileUtc = source.LastDefileUtc?.UtcDateTime;
		target.RecentPressure = source.RecentPressure;
		target.PressureReferenceUtc = source.PressureReferenceUtc?.UtcDateTime;
		target.PressureHalfLifeSeconds = source.PressureHalfLifeSeconds;
		target.PressureProfileId = source.PressureProfileId;
		target.PressureDecayAnchor = source.PressureDecayAnchor;
	}
}
