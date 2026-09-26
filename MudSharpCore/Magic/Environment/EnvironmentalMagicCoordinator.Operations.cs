#nullable enable

using MudSharp.Construction;

namespace MudSharp.Magic.Environment;

public sealed partial class EnvironmentalMagicCoordinator
{
	public const double MaximumRecentPressure = 1.0e12;

	public EnvironmentalMagicOperationResult ApplyOperation(ICell cell, EnvironmentalMagicOperationRequest request)
		=> ApplyOperationCore(cell, request, null);

	private readonly HashSet<long> _ecologicalMutations = [];

	private void ResetRecoveredEnvironment(Cell cell, bool notifyScarChange = true)
	{
		Register(cell);
		if (_registered.TryGetValue(cell.Id, out var recovered))
		{
			recovered.SampleAt = Now;
			recovered.Sample = Array.Empty<EnvironmentalResourceSnapshot>();
			recovered.RepairRate = 0.0;
			Recheck(recovered, Now);
		}
		if (notifyScarChange) ScarStateChanged(cell);
	}

	private EnvironmentalMagicOperationResult ApplyOperationCore(ICell cell, EnvironmentalMagicOperationRequest request,
		LandRejuvenationProgress? treatment)
	{
		EnvironmentalMagicOperationResult Fail(string error) => new(request.OperationId, false, false, 0.0, 0.0, 0.0, error);
		if (_disposed || cell is not Cell concrete || cell.Id <= 0 || !ReferenceEquals(cell.Gameworld, _world) || request.OperationId == Guid.Empty ||
			string.IsNullOrWhiteSpace(request.Attribution) || request.Attribution.Length > 500 ||
			!double.IsFinite(request.Damage) || request.Damage < 0.0 || !double.IsFinite(request.Pressure) || request.Pressure < 0.0 ||
			!double.IsFinite(request.Repair) || request.Repair < 0.0 || request.Repair > 0.0 && (request.Damage > 0.0 || request.Pressure > 0.0))
			return Fail("A physical cell, unique operation ID, attribution and finite non-negative damage/pressure or repair are required.");
		if (_evaluating.Contains(cell.Id)) { _recursive.Add(cell.Id); return Fail("Environmental input progs must be read-only."); }
		if (concrete.PendingEnvironmentalOperationId is { } pending && pending != request.OperationId)
			return Fail($"Operation {pending} must be confirmed before another environmental operation can be applied.");
		if (!_ecologicalMutations.Add(cell.Id)) return Fail("This cell already has an ecological mutation in progress.");
		try
		{
			var previous = _operations.Find(request.OperationId);
			if (previous is not null)
			{
				if (previous.CellId != cell.Id || previous.Request != request)
					return Fail("That operation ID has already been used with a different request.");
				if (concrete.PendingEnvironmentalOperationId == request.OperationId)
				{
					concrete.AdoptDurableEnvironment(request.OperationId, _operations.Load(concrete));
					ResetRecoveredEnvironment(concrete);
				}
				return previous.Result with { Replayed = true };
			}
			if (concrete.PendingEnvironmentalOperationId == request.OperationId)
				concrete.CancelUncommittedEnvironment(request.OperationId);
			Register(cell);
			_registered.TryGetValue(cell.Id, out var registration);
			var now = Now;
			var utcNow = UtcNow;
			var plan = registration is null || !Inspect(cell, utcNow).IsValid
				? new AdvancePlan(new(), concrete.EnvironmentState) : ProjectSettlement(registration, now);
			var state = plan.State;
			if (state.SchemaVersion != 1 || !double.IsFinite(state.ScarDamage) || state.ScarDamage < 0.0)
				return Fail("The environmental state must be repaired before this operation can be applied.");
			var pressure = PressureAt(state, utcNow);
			if (!double.IsFinite(pressure)) return Fail("The saved pressure state is invalid.");
			var damage = state.ScarDamage + request.Damage;
			if (!double.IsFinite(damage)) return Fail("The damage total is not finite.");
			if (request.Damage > 0.0 && damage <= state.ScarDamage)
				return Fail("The requested damage cannot increase the stored ecological scar at its current magnitude.");
			var (remainingScar, repaired) = ConservativeScarRepair.Calculate(damage, request.Repair);
			if (request.Repair > 0.0 && repaired == 0.0 && (damage > 0.0 || treatment is not null))
				return Fail("The earned repair cannot yet make a representable scar decrement.");
			var addedPressure = Math.Min(request.Pressure, Math.Max(0.0, MaximumRecentPressure - pressure));
			var profile = EffectiveProfileId(concrete) is { } id ? Profile(id) : null;
			var updated = state with { ScarDamage = remainingScar, Revision = state.Revision + 1 };
			if (request.Damage > 0.0 || request.Pressure > 0.0)
			{
				updated = PressureAnchor(updated, profile, pressure + addedPressure, utcNow) with { LastDefileUtc = utcNow };
			}
			// Natural settlement may reach zero before this operation adds new damage.
			// Persist the old treatment's termination first so a restart cannot revive it against that damage.
			if (treatment is null && state.ScarDamage == 0.0 && !TryEndTreatmentsAtZeroBoundary(concrete, out var terminationError))
				return Fail(terminationError!);
			var result = new EnvironmentalMagicOperationResult(request.OperationId, true, false,
				damage - state.ScarDamage, addedPressure, repaired, null);
			if (treatment is null) _operations.Commit(concrete, request, result, updated, utcNow, plan.Balances);
			else _operations.CommitRepair(concrete, request, result, updated, utcNow, plan.Balances,
				ConfirmProgress(treatment, repaired, remainingScar), treatment.Revision);
			concrete.AdoptCommittedEnvironment(request.OperationId, updated, plan.Balances);
			if (treatment is null) ScarStateChanged(cell);
			CountWrite();
			if (registration is not null)
			{
				AcceptSettlementAccounting(registration, plan, now);
				// The old sample closes at this explicit operation boundary. Repaired capacity begins now.
				Recheck(registration, now, true);
			}
			return result;
		}
		catch (Exception ex)
		{
			var error = $"Operation {request.OperationId} could not be confirmed: {ex.Message}. Retry only with the same ID; no automatic replay was attempted.";
			if (_registered.TryGetValue(cell.Id, out var registration)) Fault(registration, error, Now);
			return Fail(error);
		}
		finally { _ecologicalMutations.Remove(cell.Id); }
	}
}
