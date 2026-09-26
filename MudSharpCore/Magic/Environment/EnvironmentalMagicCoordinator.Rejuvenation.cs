#nullable enable

using MudSharp.Construction;
using MudSharp.FutureProg;

namespace MudSharp.Magic.Environment;

public sealed partial class EnvironmentalMagicCoordinator
{
	private sealed class TreatmentRegistration(ILandRejuvenationEffect effect, LandRejuvenationProgress progress,
		double now, double? ceiling)
	{
		public ILandRejuvenationEffect? Effect = effect;
		public Cell Cell = (Cell)effect.TreatmentCell;
		public LandRejuvenationProgress Progress = progress;
		public double At = now;
		public double DueAt = now + Math.Min(60.0, progress.RemainingSeconds);
		public double? Ceiling = ceiling;
		public bool Working;
	}

	private readonly Dictionary<long, TreatmentRegistration> _treatments = [];
	private readonly Dictionary<Guid, LandRejuvenationProgress> _treatmentRecords = [];
	private readonly Dictionary<long, Dictionary<Guid, LandRejuvenationProgress>> _treatmentRecordsByCell = [];
	private readonly HashSet<long> _treatmentCellsLoaded = [];
	private readonly Queue<ILandRejuvenationEffect> _loadedTreatments = [];
	private readonly HashSet<ILandRejuvenationEffect> _queuedTreatments = [];
	private readonly SortedSet<TreatmentRegistration> _treatmentDue = new(Comparer<TreatmentRegistration>.Create((a, b) =>
		a.DueAt.CompareTo(b.DueAt) is var result && result != 0 ? result : a.Cell.Id.CompareTo(b.Cell.Id)));
	public long TreatmentVisits { get; private set; }
	public int ActiveTreatmentCount => _treatmentDue.Count;

	public void RegisterLoadedTreatment(ILandRejuvenationEffect effect)
	{
		if (!_disposed && _queuedTreatments.Add(effect)) _loadedTreatments.Enqueue(effect);
	}

	private bool VisitTreatment(double now)
	{
		if (_loadedTreatments.TryDequeue(out var effect))
		{
			_queuedTreatments.Remove(effect);
			if (!ActivateTreatment(effect, null, out var error)) effect.TreatmentEnded(error ?? "Load refused.");
			return true;
		}
		if (_treatmentDue.Min is not { } due || due.DueAt > now) return false;
		AdvanceTreatment(due, now);
		return true;
	}

	private void TreatmentProfileChanged(IEnvironmentalMagicProfile profile)
	{
		foreach (var r in _treatments.Values.Where(x => x.Progress.ProfileId == profile.Id).ToArray())
		{
			var policy = InspectRepairPolicy(r.Cell);
			if (!policy.IsValid) CancelTreatment(r.Cell, r.Progress.Id, policy.Error ?? "Repair policy disabled.");
			else { r.Ceiling = policy.Ceiling; r.At = Now; }
		}
	}

	public LandRejuvenationPolicy InspectRepairPolicy(ICell cell)
	{
		if (_disposed || cell is not Cell concrete || !ReferenceEquals(cell.Gameworld, _world) || cell.Id <= 0)
			return new(null, null, "A physical cell in this gameworld is required.");
		var id = EffectiveProfileId(concrete);
		if (id is null || Profile(id.Value) is not { } profile)
			return new(id, null, "An effective environmental profile is required.");
		var state = concrete.EnvironmentState;
		if (state.SchemaVersion != 1 || !double.IsFinite(state.ScarDamage) || state.ScarDamage < 0.0 ||
			!double.IsFinite(PressureAt(state, UtcNow)))
			return new(id, profile.MagicalRepairLimitPerMinute, "The saved ecological scalar state is invalid.");
		if (profile.RepairValidationErrors.Count > 0)
			return new(id, profile.MagicalRepairLimitPerMinute, string.Join("; ", profile.RepairValidationErrors));
		var cap = profile.MagicalRepairLimitPerMinute;
		return new(id, cap, cap.HasValue && (!double.IsFinite(cap.Value) || cap.Value < 0.0)
			? "Invalid magical repair ceiling." : cap == 0.0 ? "This profile disables magical treatment." : null);
	}

	private void LoadTreatmentRecords(long cellId)
	{
		if (_treatmentCellsLoaded.Contains(cellId)) return;
		foreach (var row in _operations.TreatmentsFor(cellId)) CacheTreatment(row);
		_treatmentCellsLoaded.Add(cellId);
	}

	private void CacheTreatment(LandRejuvenationProgress progress)
	{
		_treatmentRecords[progress.Id] = progress;
		if (!_treatmentRecordsByCell.TryGetValue(progress.CellId, out var records))
			_treatmentRecordsByCell[progress.CellId] = records = [];
		records[progress.Id] = progress;
	}

	private IEnumerable<LandRejuvenationProgress> CellTreatmentRecords(long cellId) =>
		_treatmentRecordsByCell.TryGetValue(cellId, out var records) ? records.Values : [];

	public LandRejuvenationProgress? InspectTreatment(ICell cell, Guid treatmentId)
	{
		if (_disposed || !ReferenceEquals(cell.Gameworld, _world)) return null;
		LoadTreatmentRecords(cell.Id);
		return _treatmentRecords.GetValueOrDefault(treatmentId) is { } p && p.CellId == cell.Id ? p : null;
	}

	public IReadOnlyList<LandRejuvenationProgress> InspectTreatments(ICell cell)
	{
		if (_disposed || !ReferenceEquals(cell.Gameworld, _world)) return [];
		LoadTreatmentRecords(cell.Id);
		return CellTreatmentRecords(cell.Id).ToArray();
	}

	public bool CanInstallTreatment(ICell cell, out string? error)
	{
		var policy = InspectRepairPolicy(cell);
		error = policy.Error;
		if (!policy.IsValid) return false;
		if (_evaluating.Contains(cell.Id) || _ecologicalMutations.Contains(cell.Id))
		{
			error = "Treatment admission is unavailable during an ecological policy or mutation.";
			return false;
		}
		if (((Cell)cell).PendingEnvironmentalOperationId is { } pending)
		{
			error = $"Ecological operation {pending} remains unresolved.";
			return false;
		}
		try
		{
			LoadTreatmentRecords(cell.Id);
			if (_treatments.ContainsKey(cell.Id) || CellTreatmentRecords(cell.Id).Any(x => !x.IsTerminal || x.PendingRequest is not null))
				error = "This cell already has an active or unresolved rejuvenation treatment.";
			else if (((Cell)cell).EnvironmentState.ScarDamage <= 0.0) error = "There are no existing scars to treat.";
		}
		catch (Exception ex) { error = $"Treatment admission cannot read authoritative progress: {ex.Message}"; }
		return error is null;
	}

	public bool EvaluateRepairPolicy(ICell cell, ICharacter caster, IFutureProg prog, out string? error)
	{
		error = null;
		if (!_evaluating.Add(cell.Id)) { error = "Recursive repair policy evaluation is forbidden."; return false; }
		try
		{
			if (prog.StaticType != FutureProgStaticType.NotStatic || prog.ReturnType != ProgVariableTypes.Boolean ||
				prog.AcceptsAnyParameters || !prog.Parameters.SequenceEqual([ProgVariableTypes.Character, ProgVariableTypes.Location]) ||
				!string.IsNullOrEmpty(prog.CompileError))
				error = "Repair policies must be compiled NotStatic boolean (character, location) progs.";
			else if (prog.ExecuteBool(caster, cell) != true) error = "The repair policy declined this treatment.";
			if (_recursive.Remove(cell.Id)) error = "Repair policy attempted an environmental mutation or recursive read.";
		}
		catch (Exception ex) { error = $"Repair policy failed: {ex.Message}"; }
		finally { _evaluating.Remove(cell.Id); }
		return error is null;
	}

	private static string TreatmentAttribution(LandRejuvenationProgress p, long sequence) =>
		FormattableString.Invariant($"Land rejuvenation {p.Id}, spell #{p.SpellId}, step {sequence}");

	private static string? ValidateProgress(LandRejuvenationProgress p) =>
		p.Version != 1 || p.Id == Guid.Empty || p.ParentId == Guid.Empty || p.CellId <= 0 || p.SpellId <= 0 ||
		!Enum.IsDefined(p.Status) || p.Revision < 0 || p.Sequence < p.AcknowledgedSequence || p.AcknowledgedSequence < 0 ||
		!double.IsFinite(p.Rate) || p.Rate <= 0.0 || !double.IsFinite(p.InitialBudget) || p.InitialBudget <= 0.0 ||
		!double.IsFinite(p.RemainingBudget) || p.RemainingBudget < 0.0 || p.RemainingBudget > p.InitialBudget ||
		!double.IsFinite(p.TotalRepaired) || p.TotalRepaired < 0.0 || p.TotalRepaired > p.InitialBudget ||
		// Permit accumulated floating-point roundoff, never a second independently unspent budget.
		Math.Abs(p.InitialBudget - p.RemainingBudget - p.TotalRepaired) > Math.Max(double.Epsilon, p.InitialBudget * 1e-12) ||
		!double.IsFinite(p.RemainingSeconds) || p.RemainingSeconds < 0.0 || p.RemainingSeconds >= TimeSpan.MaxValue.TotalSeconds ||
		!double.IsFinite(p.EarnedWork) || p.EarnedWork < 0.0 || p.EarnedWork > p.RemainingBudget ||
		p.CasterId <= 0 || p.ActingInstanceId < 0 || p.ProfileId <= 0 || p.PlaneIds is null || p.PlaneIds.Any(x => x <= 0) ||
		p.Sequence - p.AcknowledgedSequence > 1 || p.LastOperationId == Guid.Empty ||
		(p.AcknowledgedSequence > 0) != p.LastOperationId.HasValue ||
		p.Status == LandRejuvenationStatus.Active && (p.PendingRequest is not null || p.Sequence != p.AcknowledgedSequence || p.CancellationRequested) ||
		p.Status == LandRejuvenationStatus.Pending && p.PendingRequest is null ||
		!double.IsFinite(p.Rate * (p.RemainingSeconds / 60.0)) ||
		p.PendingRequest is { } r && (r.OperationId == Guid.Empty || r.Damage != 0.0 || r.Pressure != 0.0 ||
			r.ActorId != p.CasterId || r.Attribution != TreatmentAttribution(p, p.Sequence) || r.OperationId == p.LastOperationId ||
			p.Status != LandRejuvenationStatus.Pending || p.Sequence != p.AcknowledgedSequence + 1 ||
			!double.IsFinite(r.Repair) || r.Repair <= 0.0 || r.Repair > Math.Min(p.EarnedWork, p.RemainingBudget))
			? "Malformed or unsupported treatment checkpoint; no work was registered." : null;

	public bool ActivateTreatment(ILandRejuvenationEffect effect, LandRejuvenationProgress? initial, out string? error)
	{
		error = null;
		if (_disposed || effect.TreatmentCell is not Cell cell || !ReferenceEquals(cell.Gameworld, _world))
		{ error = "Treatment requires an attached spell parent on a physical cell."; return false; }
		if (_treatments.TryGetValue(cell.Id, out var existing))
		{
			if (ReferenceEquals(existing.Effect, effect)) return true;
			error = existing.Progress.Id == effect.TreatmentId ? "Duplicate saved treatment identity ignored." : "Conflicting saved treatment disabled.";
			return false;
		}
		try
		{
			LoadTreatmentRecords(cell.Id);
			var progress = initial ?? _operations.FindTreatment(effect.TreatmentId);
			if (progress is null || (error = ValidateProgress(progress)) is not null ||
				progress.Id != effect.TreatmentId || progress.CellId != cell.Id || progress.ParentId != effect.ParentIdentity)
			{ error ??= "Missing or mismatched authoritative treatment identity."; return false; }
			CacheTreatment(progress);
			if (!effect.IsAttached)
			{
				error = "The loaded treatment has no valid attached source spell; it has been cancelled.";
				if (initial is null) CancelTreatment(cell, progress.Id, error);
				return false;
			}
			if (progress.IsTerminal || progress.CancellationRequested)
			{ error = "The saved treatment has ended; its XML cannot restore its budget."; return false; }
			if (CellTreatmentRecords(cell.Id).Any(x => x.Id != progress.Id && (!x.IsTerminal || x.PendingRequest is not null)))
			{ error = "Conflicting saved treatment checkpoints require staff review; no work was registered."; return false; }
			var policy = InspectRepairPolicy(cell);
			if (!policy.IsValid || policy.ProfileId != progress.ProfileId || !effect.CheckMaintenance(out error))
			{
				error ??= policy.Error ?? "The effective profile changed.";
				if (initial is null) SaveProgress(progress with { CancellationRequested = true,
					Status = progress.PendingRequest is null ? LandRejuvenationStatus.Cancelled : LandRejuvenationStatus.Pending, Diagnostic = error });
				return false;
			}
			if (initial is not null)
			{
				_operations.SaveTreatment(progress, null);
			}
			var registration = new TreatmentRegistration(effect, progress, Now, policy.Ceiling);
			_treatments.Add(cell.Id, registration);
			_treatmentDue.Add(registration);
			if (cell.EnvironmentState.ScarDamage == 0.0) CancelTreatment(cell, progress.Id, "No scars remain.");
			return true;
		}
		catch (Exception ex) { error = $"Treatment activation failed: {ex.Message}"; return false; }
	}

	private LandRejuvenationProgress SaveProgress(LandRejuvenationProgress progress)
	{
		var next = progress with { Revision = progress.Revision + 1 };
		_operations.SaveTreatment(next, progress.Revision);
		CacheTreatment(next);
		return next;
	}

	private static LandRejuvenationProgress ConfirmProgress(LandRejuvenationProgress prepared, double repair, double scar)
	{
		var budget = Math.Max(0.0, prepared.RemainingBudget - repair);
		return prepared with
		{
			Revision = prepared.Revision + 1, RemainingBudget = budget,
			TotalRepaired = Math.Min(prepared.InitialBudget, prepared.TotalRepaired + repair),
			EarnedWork = Math.Min(budget, Math.Max(0.0, prepared.EarnedWork - repair)),
			AcknowledgedSequence = prepared.Sequence, LastOperationId = prepared.PendingRequest!.OperationId,
			PendingRequest = null, Diagnostic = string.Empty,
			Status = scar == 0.0 || budget == 0.0 || prepared.RemainingSeconds == 0.0
				? LandRejuvenationStatus.Completed : LandRejuvenationStatus.Active
		};
	}

	private void QueueTreatment(TreatmentRegistration r)
	{
		_treatmentDue.Remove(r);
		if (r.Progress.IsTerminal && r.Progress.PendingRequest is null) { FinishTreatment(r); return; }
		if (r.Effect is null || r.Progress.CancellationRequested) return;
		r.DueAt = Now + Math.Min(60.0, Math.Max(1.0, r.Progress.RemainingSeconds));
		_treatmentDue.Add(r);
	}

	private void FinishTreatment(TreatmentRegistration r)
	{
		_treatmentDue.Remove(r);
		_treatments.Remove(r.Cell.Id);
		var effect = r.Effect;
		r.Effect = null;
		effect?.TreatmentEnded(r.Progress.Diagnostic);
	}

	public void CheckpointTreatment(ICell cell, Guid treatmentId)
	{
		if (_treatments.TryGetValue(cell.Id, out var r) && r.Progress.Id == treatmentId)
			AdvanceTreatment(r, Now, true);
	}

	private void AdvanceTreatment(TreatmentRegistration r, double now, bool checkpointOnly = false)
	{
		if (r.Working || _disposed) return;
		r.Working = true;
		_treatmentDue.Remove(r);
		if (!checkpointOnly) TreatmentVisits++;
		try
		{
			var p = r.Progress;
			if (p.PendingRequest is not null)
			{
				// Unknown steps earn nothing. A save may close their lifetime, but never confirms or retries them.
				if (checkpointOnly)
					r.Progress = SaveProgress(p with { RemainingSeconds = Math.Max(0.0, p.RemainingSeconds - Math.Max(0.0, now - r.At)) });
				else ReconcileTreatment(r.Cell, p.Id, true, out _);
				return;
			}
			if (r.Effect is null || !r.Effect.IsAttached || !r.Effect.CheckMaintenance(out var maintenance))
			{ CancelTreatment(r.Cell, p.Id, "Treatment attachment or maintenance failed."); return; }
			var policy = InspectRepairPolicy(r.Cell);
			if (!policy.IsValid || policy.ProfileId != p.ProfileId)
			{ CancelTreatment(r.Cell, p.Id, policy.Error ?? "The effective profile changed."); return; }
			if (r.Cell.PendingEnvironmentalOperationId is { } foreign)
			{
				r.Progress = SaveProgress(p with { RemainingSeconds = Math.Max(0.0, p.RemainingSeconds - Math.Max(0.0, now - r.At)),
					EarnedWork = 0.0, Diagnostic = $"Waiting for foreign ecological operation {foreign}; no elapsed credit." });
				r.At = now;
				return;
			}
			if (r.Cell.EnvironmentState.ScarDamage == 0.0)
			{ CancelTreatment(r.Cell, p.Id, "No scars remain."); return; }
			var seconds = Math.Min(p.RemainingSeconds, Math.Max(0.0, now - r.At));
			// A custom policy edit discovered late closes the unknown segment without inventing eligibility.
			var rate = policy.Ceiling != r.Ceiling ? 0.0 : Math.Min(p.Rate, r.Ceiling ?? p.Rate);
			r.Ceiling = policy.Ceiling;
			r.At = now;
			var earned = Math.Min(p.RemainingBudget, p.EarnedWork + Math.Min(p.RemainingBudget, rate * (seconds / 60.0)));
			var next = p with { RemainingSeconds = Math.Max(0.0, p.RemainingSeconds - seconds), EarnedWork = earned, Diagnostic = string.Empty };
			if (checkpointOnly)
			{
				if (seconds > 0.0) r.Progress = SaveProgress(next);
				return;
			}
			// Close natural recovery before preparing a magical step when it alone finishes the land.
			// There must be no zero-repair magical receipt or acknowledgement for this boundary.
			if (_registered.TryGetValue(r.Cell.Id, out var natural) && Inspect(r.Cell).IsValid &&
				ProjectSettlement(natural, now).State.ScarDamage == 0.0)
			{
				Settle(natural, now);
				r.Progress = SaveProgress(next with { EarnedWork = 0.0, Status = LandRejuvenationStatus.Completed,
					Diagnostic = "Natural recovery removed the final scars; no magical step was applied." });
				return;
			}
			var applied = ConservativeScarRepair.Calculate(r.Cell.EnvironmentState.ScarDamage, earned).Applied;
			if (applied == 0.0)
			{
				var possible = Math.Min(p.RemainingBudget, earned + Math.Min(p.RemainingBudget, Math.Min(p.Rate, r.Ceiling ?? p.Rate) * (next.RemainingSeconds / 60.0)));
				if (next.RemainingSeconds == 0.0 || ConservativeScarRepair.Calculate(r.Cell.EnvironmentState.ScarDamage, possible).Applied == 0.0)
					next = next with { Status = LandRejuvenationStatus.Completed, Diagnostic = "Remaining work cannot fund a representable decrement before expiry." };
				r.Progress = SaveProgress(next);
				return;
			}
			var request = new EnvironmentalMagicOperationRequest(Guid.NewGuid(), p.CasterId,
				TreatmentAttribution(p, p.Sequence + 1), Repair: Math.Min(earned, p.RemainingBudget));
			next = next with { Sequence = p.Sequence + 1, PendingRequest = request, Status = LandRejuvenationStatus.Pending };
			r.Progress = SaveProgress(next);
			CommitTreatmentStep(r);
		}
		catch (Exception ex)
		{
			r.Progress = r.Progress with { Diagnostic = $"Repair checkpoint requires confirmation: {ex.Message}" };
			CacheTreatment(r.Progress);
			_world.SystemMessage($"Land rejuvenation {r.Progress.Id}: {r.Progress.Diagnostic}", true);
			if (checkpointOnly) throw;
		}
		finally { r.At = now; r.Working = false; QueueTreatment(r); }
	}

	private bool CommitTreatmentStep(TreatmentRegistration r)
	{
		// A prepared request may outlive natural recovery while it awaits confirmation. End it
		// without a magical receipt; only the timed lane reaches this path after proving rollback.
		if (_registered.TryGetValue(r.Cell.Id, out var natural) && Inspect(r.Cell).IsValid &&
			ProjectSettlement(natural, Now).State.ScarDamage == 0.0)
		{
			Settle(natural, Now);
			r.Progress = SaveProgress(r.Progress with { PendingRequest = null, EarnedWork = 0.0,
				Status = LandRejuvenationStatus.Completed, Diagnostic = "Natural recovery removed the final scars; the prepared magical step was not applied." });
			return true;
		}
		var result = ApplyOperationCore(r.Cell, r.Progress.PendingRequest!, r.Progress);
		if (!result.Success)
		{
			r.Progress = r.Progress with { Diagnostic = result.Error ?? "Repair step remains unresolved." };
			CacheTreatment(r.Progress);
			return false;
		}
		r.Progress = _operations.FindTreatment(r.Progress.Id) ?? throw new InvalidOperationException("Committed repair checkpoint disappeared.");
		CacheTreatment(r.Progress);
		return true;
	}

	public bool ConfirmTreatment(ICell cell, Guid treatmentId, out string? error) => ReconcileTreatment(cell, treatmentId, false, out error);

	private bool ReconcileTreatment(ICell cell, Guid treatmentId, bool allowRetry, out string? error)
	{
		error = null;
		if (cell is not Cell concrete || !ReferenceEquals(cell.Gameworld, _world) || _evaluating.Contains(cell.Id) || _ecologicalMutations.Contains(cell.Id))
		{ error = "Confirmation requires a physical cell outside policy evaluation or ecological mutation."; return false; }
		_treatments.TryGetValue(cell.Id, out var r);
		if (r?.Progress.Id != treatmentId) r = null;
		var closeInterval = false;
		try
		{
			var stored = _operations.FindTreatment(treatmentId) ?? throw new InvalidOperationException("Authoritative treatment is missing.");
			if (stored.CellId != cell.Id || ValidateProgress(stored) is { }) throw new InvalidOperationException("Invalid or foreign treatment checkpoint.");
			var before = r?.Progress ?? _treatmentRecords.GetValueOrDefault(treatmentId) ?? stored;
			closeInterval = before.PendingRequest is not null || stored.PendingRequest is not null;
			if (r is null && !cell.Effects.OfType<ILandRejuvenationEffect>().Any(x => x.TreatmentId == treatmentId) && !stored.IsTerminal)
				before = before with { CancellationRequested = true, Diagnostic = "No attached treatment remains; staff confirmation closed its checkpoint." };
			if (before.PendingRequest is { } request)
			{
				var receipt = _operations.Find(request.OperationId);
				if (receipt is not null && (receipt.CellId != cell.Id || receipt.Request != request ||
					stored.AcknowledgedSequence < before.Sequence || stored.PendingRequest is not null))
					throw new InvalidOperationException("Receipt and authoritative checkpoint disagree; staff review required.");
				if (receipt is not null && concrete.PendingEnvironmentalOperationId == request.OperationId)
				{
					concrete.AdoptDurableEnvironment(request.OperationId, _operations.Load(concrete));
					ResetRecoveredEnvironment(concrete, false);
				}
				else if (receipt is null && concrete.PendingEnvironmentalOperationId == request.OperationId)
					concrete.CancelUncommittedEnvironment(request.OperationId);
			}
			else if (stored.LastOperationId is { } last && concrete.PendingEnvironmentalOperationId == last)
			{
				// Removal can already have adopted the committed budget while the cell still awaits acknowledgement.
				var receipt = _operations.Find(last);
				if (receipt is null || receipt.CellId != cell.Id || stored.PendingRequest is not null)
					throw new InvalidOperationException("The last acknowledged repair receipt is unavailable.");
				concrete.AdoptDurableEnvironment(last, _operations.Load(concrete));
				ResetRecoveredEnvironment(concrete, false);
			}
			if (closeInterval && r is not null && Now > r.At && stored.RemainingSeconds > 0.0)
				stored = SaveProgress(stored with { RemainingSeconds = Math.Max(0.0, stored.RemainingSeconds - (Now - r.At)) });
			if (before.CancellationRequested && !stored.CancellationRequested)
				stored = SaveProgress(stored with { CancellationRequested = true,
					Status = stored.PendingRequest is null ? LandRejuvenationStatus.Cancelled : LandRejuvenationStatus.Pending,
					Diagnostic = before.Diagnostic });
			if (stored.CancellationRequested && stored.PendingRequest is not null)
				stored = SaveProgress(stored with { PendingRequest = null, Status = LandRejuvenationStatus.Cancelled, EarnedWork = 0.0 });
			if (r is not null) r.Progress = stored;
			CacheTreatment(stored);
			if (stored.PendingRequest is not null && allowRetry && r is not null)
			{
				// Only the timed lane may retry an exact prepared request. Inspection/confirmation never repairs.
				if (r.Effect is null || !r.Effect.IsAttached || !r.Effect.CheckMaintenance(out error) ||
					!InspectRepairPolicy(cell).IsValid || InspectRepairPolicy(cell).ProfileId != stored.ProfileId)
				{
					CancelTreatment(cell, stored.Id, error ?? "Treatment maintenance or repair policy ended during confirmation.");
					return false;
				}
				if (!CommitTreatmentStep(r)) { error = r.Progress.Diagnostic; return false; }
			}
			return true;
		}
		catch (Exception ex) { error = ex.Message; return false; }
		finally { if (r is not null && closeInterval) { r.At = Now; if (!r.Working) QueueTreatment(r); } }
	}

	public void CancelTreatment(ICell cell, Guid treatmentId, string reason)
	{
		if (cell is not Cell || !ReferenceEquals(cell.Gameworld, _world) || treatmentId == Guid.Empty) return;
		if (!_treatments.TryGetValue(cell.Id, out var r) || r.Progress.Id != treatmentId)
		{
			try
			{
				var orphan = _operations.FindTreatment(treatmentId);
				if (orphan is not null && orphan.CellId == cell.Id && !orphan.IsTerminal)
					SaveProgress(orphan with { CancellationRequested = true, EarnedWork = 0.0,
						Status = orphan.PendingRequest is null ? LandRejuvenationStatus.Cancelled : LandRejuvenationStatus.Pending,
						Diagnostic = reason });
			}
			catch (Exception ex) { _world.SystemMessage($"Treatment {treatmentId} cancellation requires staff confirmation: {ex.Message}", true); }
			return;
		}
		_treatmentDue.Remove(r);
		var effect = r.Effect;
		r.Effect = null;
		try
		{
			var current = _operations.FindTreatment(treatmentId) ?? r.Progress;
			r.Progress = SaveProgress(current with { CancellationRequested = true,
				Status = current.PendingRequest is null ? LandRejuvenationStatus.Cancelled : LandRejuvenationStatus.Pending,
				Diagnostic = reason });
			if (r.Progress.PendingRequest is null) _treatments.Remove(cell.Id);
		}
		catch (Exception ex)
		{
			r.Progress = r.Progress with { CancellationRequested = true, Diagnostic = $"Cancellation pending: {reason}; {ex.Message}" };
			CacheTreatment(r.Progress);
		}
		effect?.TreatmentEnded(reason);
	}

	public void ExpireTreatment(ICell cell, Guid treatmentId)
	{
		if (!_treatments.TryGetValue(cell.Id, out var r) || r.Progress.Id != treatmentId) return;
		AdvanceTreatment(r, Now);
		if (_treatments.ContainsKey(cell.Id)) CancelTreatment(cell, treatmentId, "Spell lifetime expired.");
	}

	public void ScarStateChanged(ICell cell)
	{
		if (cell is Cell { EnvironmentState.ScarDamage: 0.0 } && _treatments.TryGetValue(cell.Id, out var r) && !r.Working)
			CancelTreatment(cell, r.Progress.Id, "No scars remain; this treatment has ended.");
	}

	private bool TryEndTreatmentsAtZeroBoundary(Cell cell, out string? error)
	{
		error = null;
		if (!_treatments.ContainsKey(cell.Id) && !cell.Effects.OfType<ILandRejuvenationEffect>().Any()) return true;
		try
		{
			LoadTreatmentRecords(cell.Id);
			foreach (var progress in CellTreatmentRecords(cell.Id).Where(x => !x.IsTerminal || x.PendingRequest is not null).ToArray())
			{
				CancelTreatment(cell, progress.Id, "Natural recovery reached zero scars; this treatment has ended.");
				var stored = _operations.FindTreatment(progress.Id);
				if (stored is { CancellationRequested: true } || stored is { IsTerminal: true, PendingRequest: null }) continue;
				error = $"Treatment {progress.Id} must have its termination durably confirmed before another ecological operation.";
				return false;
			}
			return true;
		}
		catch (Exception ex)
		{
			error = $"Treatment termination could not be confirmed at the zero-scar boundary: {ex.Message}";
			return false;
		}
	}
}
