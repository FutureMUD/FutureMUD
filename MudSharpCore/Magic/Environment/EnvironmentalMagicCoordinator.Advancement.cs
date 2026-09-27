#nullable enable

using MudSharp.Construction;

namespace MudSharp.Magic.Environment;

public sealed partial class EnvironmentalMagicCoordinator
{
	private sealed record AdvancePlan(Dictionary<IMagicResource, double> Balances, EnvironmentalMagicState State)
	{
		public Dictionary<long, double>? ProductionRemainders { get; set; }
		public double RepairRemainder { get; set; }
	}

	private AdvancePlan ProjectSettlement(Registration registration, double now)
	{
		var plan = new AdvancePlan(new(), registration.Cell.EnvironmentState)
		{
			ProductionRemainders = registration.ProductionRemainders is { Count: > 0 } remainders
				? new Dictionary<long, double>(remainders) : null,
			RepairRemainder = registration.RepairRemainder
		};
		var minutes = Math.Max(0.0, now - registration.SampleAt) / 60.0;
		if (minutes <= 0.0 || registration.Faulted) return plan;
		foreach (var output in registration.Sample)
		{
			if (!output.IsValid || output.Rate <= 0.0 || _world.MagicResources.Get(output.ResourceId) is not { } resource)
			{
				plan.ProductionRemainders?.Remove(output.ResourceId);
				continue;
			}
			var balance = registration.Cell.MagicResourceAmounts.GetValueOrDefault(resource);
			if (balance >= output.Maximum)
			{
				plan.Balances[resource] = output.Maximum;
				plan.ProductionRemainders?.Remove(output.ResourceId);
				continue;
			}
			var earned = output.Rate * minutes + (plan.ProductionRemainders?.GetValueOrDefault(output.ResourceId) ?? 0.0);
			var updated = earned > 0.0 ? Math.Min(output.Maximum, balance + earned) : balance;
			plan.Balances[resource] = updated;
			var remainder = updated >= output.Maximum ? 0.0 : earned - (updated - balance);
			if (remainder == 0.0) plan.ProductionRemainders?.Remove(output.ResourceId);
			else (plan.ProductionRemainders ??= new())[output.ResourceId] = remainder;
		}
		if (registration.RepairRate > 0.0 && plan.State.ScarDamage > 0.0)
		{
			var earned = registration.RepairRate * minutes + plan.RepairRemainder;
			var damage = earned > 0.0 ? Math.Max(0.0, plan.State.ScarDamage - earned) : plan.State.ScarDamage;
			plan.RepairRemainder = damage <= 0.0 ? 0.0 : earned - (plan.State.ScarDamage - damage);
			if (damage != plan.State.ScarDamage)
				plan = plan with { State = plan.State with { ScarDamage = damage, Revision = plan.State.Revision + 1 } };
		}
		else plan.RepairRemainder = 0.0;
		return plan;
	}

	private static void AcceptSettlementAccounting(Registration registration, AdvancePlan plan, double now)
	{
		// Residuals are bounded rounding errors from already earned online work, never elapsed time.
		// They may be negative after rounding up, so subsequent sub-ULP work cannot be over-awarded.
		registration.ProductionRemainders = plan.ProductionRemainders;
		registration.RepairRemainder = plan.RepairRemainder;
		registration.SampleAt = now;
	}

	private void ApplySettlement(Registration registration, AdvancePlan plan, double now, bool ensurePresent = false)
	{
		foreach (var balance in plan.Balances)
			if (registration.Cell.SetEnvironmentalResource(balance.Key, balance.Value, ensurePresent)) CountWrite();
		if (registration.Cell.SetEnvironmentState(plan.State)) CountWrite();
		AcceptSettlementAccounting(registration, plan, now);
	}

	private void Settle(Registration registration, double now)
	{
		ApplySettlement(registration, ProjectSettlement(registration, now), now);
	}

	private void Recheck(Registration registration, double now, bool keepDeadline = false)
	{
		if (_registered.GetValueOrDefault(registration.Cell.Id) != registration) return;
		if (EffectiveProfileId(registration.Cell) != registration.ProfileId)
		{
			Settle(registration, now);
			Register(registration.Cell);
			if (_registered.TryGetValue(registration.Cell.Id, out var replacement)) Recheck(replacement, now);
			return;
		}
		var snapshot = Inspect(registration.Cell);
		if (!snapshot.IsValid)
		{
			Fault(registration, string.Join("; ", snapshot.Errors), now);
			return;
		}
		var profile = Profile(registration.ProfileId)!;
		var plan = ProjectSettlement(registration, now);
		if (!TrySample(registration, snapshot, profile, plan, out var samples, out var error))
		{
			Fault(registration, error!, now);
			return;
		}
		ApplySettlement(registration, plan, now, true);
		SettlePressure(registration.Cell, profile);
		Accept(registration, samples, profile, now, keepDeadline);
	}

	private bool TrySample(Registration registration, EnvironmentalMagicSnapshot snapshot, IEnvironmentalMagicProfile profile,
		AdvancePlan plan, out IReadOnlyList<EnvironmentalResourceSnapshot> samples, out string? error)
	{
		var inputs = snapshot.Inputs.ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase);
		inputs["scardamage"] = plan.State.ScarDamage;
		var results = new List<EnvironmentalResourceSnapshot>(profile.Outputs.Count);
		error = null;
		foreach (var output in profile.Outputs)
		{
			var resource = output.Resource!;
			var balance = plan.Balances.GetValueOrDefault(resource, registration.Cell.MagicResourceAmounts.GetValueOrDefault(resource));
			var evaluation = profile.EvaluateOutput(output, inputs, balance);
			if (evaluation.IsValid && balance > evaluation.Maximum)
			{
				balance = evaluation.Maximum;
				evaluation = profile.EvaluateOutput(output, inputs, balance);
			}
			if (!evaluation.IsValid || !double.IsFinite(balance) || balance < 0.0)
			{
				error = evaluation.Error ?? "Invalid environmental balance or rate.";
				samples = Array.Empty<EnvironmentalResourceSnapshot>();
				return false;
			}
			results.Add(new(resource.Id, resource.Name, balance, true, evaluation.Maximum, evaluation.Rate, null));
			plan.Balances[resource] = balance;
		}
		samples = results.AsReadOnly();
		return true;
	}

	private void Accept(Registration registration, IReadOnlyList<EnvironmentalResourceSnapshot> samples,
		IEnvironmentalMagicProfile profile, double now, bool keepDeadline)
	{
		var production = samples.Any(x => x.Balance < x.Maximum && x.Rate > 0.0);
		var maintenance = registration.Cell.EnvironmentState.ScarDamage > 0.0 && profile.NaturalRepairPerMinute > 0.0;
		var wasActive = registration.Production || registration.Maintenance;
		SetStatus(registration, production, maintenance, false);
		registration.Sample = samples;
		registration.SampleAt = now;
		registration.RepairRate = profile.NaturalRepairPerMinute;
		if (!maintenance) registration.RepairRemainder = 0.0;
		if (registration.ProductionRemainders is { } remainders)
		{
			foreach (var id in remainders.Keys.ToArray())
				if (!samples.Any(x => x.ResourceId == id && x.IsValid && x.Rate > 0.0 && x.Balance < x.Maximum))
					remainders.Remove(id);
			if (remainders.Count == 0) registration.ProductionRemainders = null;
		}
		registration.DefinitionRevision = profile.Revision;
		ClearDirty(registration);
		var oldDeadline = registration.DueAt;
		_due.Remove(registration);
		if (production || maintenance)
		{
			registration.DueAt = keepDeadline && wasActive && oldDeadline > now
				? Math.Min(oldDeadline, now + Options.ActiveCadenceSeconds) : now + Options.ActiveCadenceSeconds;
			_due.Add(registration);
		}
		ScheduleAudit(registration, now, profile.IdleRecheckSeconds);
	}

	private void SetStatus(Registration registration, bool production, bool maintenance, bool faulted)
	{
		_workingCount += (production || maintenance ? 1 : 0) - (registration.Production || registration.Maintenance ? 1 : 0);
		_productionCount += (production ? 1 : 0) - (registration.Production ? 1 : 0);
		_maintenanceCount += (maintenance ? 1 : 0) - (registration.Maintenance ? 1 : 0);
		_faultCount += (faulted ? 1 : 0) - (registration.Faulted ? 1 : 0);
		registration.Production = production;
		registration.Maintenance = maintenance;
		registration.Faulted = faulted;
	}

	private void ClearDirty(Registration registration)
	{
		registration.Dirty = EnvironmentalMagicDirtyReason.None;
		if (registration.DirtyNode is null) return;
		_dirty.Remove(registration.DirtyNode);
		registration.DirtyNode = null;
	}

	private void ScheduleAudit(Registration registration, double now, double? idleSeconds)
	{
		_audit.Remove(registration);
		registration.LastAudit = now;
		var interval = Math.Max(1.0, Math.Min(Options.ReconciliationSeconds, idleSeconds ?? Options.ReconciliationSeconds));
		// Retain a stable phase even when a whole newly loaded population becomes dormant together.
		registration.AuditAt = Math.Floor(now / interval) * interval + Stagger(registration.Cell.Id, interval);
		if (registration.AuditAt <= now) registration.AuditAt += interval;
		_audit.Add(registration);
		if (registration.AgeNode is not null) _auditAge.Remove(registration.AgeNode);
		registration.AgeNode = _auditAge.AddLast(registration);
	}

	private void Fault(Registration registration, string error, double now)
	{
		_due.Remove(registration);
		ClearDirty(registration);
		registration.Sample = Array.Empty<EnvironmentalResourceSnapshot>();
		registration.SampleAt = now;
		registration.RepairRate = 0.0;
		registration.ProductionRemainders = null;
		registration.RepairRemainder = 0.0;
		SetStatus(registration, false, false, true);
		ScheduleAudit(registration, now, null);
		_representativeError = $"Profile #{registration.ProfileId}, cell #{registration.Cell.Id}: {error}";
		_totalFaults++;
		if (!_lastLoggedFault.TryGetValue(registration.ProfileId, out var last) || now - last >= 60.0)
		{
			_lastLoggedFault[registration.ProfileId] = now;
			_world.SystemMessage($"Environmental magic: {_representativeError}", true);
		}
	}

	public bool TryMutateResource(ICell cell, IMagicResource resource, EnvironmentalResourceMutation mutation,
		double amount, out bool success)
	{
		success = false;
		if (cell is not Cell concrete || EffectiveProfileId(concrete) is not { } id) return false;
		var profile = Profile(id);
		if (profile is not null && !profile.Outputs.Any(x => x.ResourceId == resource.Id)) return false;
		if (_disposed || !Enum.IsDefined(mutation) || !double.IsFinite(amount) || mutation == EnvironmentalResourceMutation.Debit && amount < 0.0 ||
			mutation == EnvironmentalResourceMutation.Set && amount < 0.0) return true;
		if (_evaluating.Contains(cell.Id)) { _recursive.Add(cell.Id); return true; }
		if (_ecologicalMutations.Contains(cell.Id)) return true;
		Register(cell); // Also discovers a newly inherited binding before the unconfigured fast path can escape.
		var registration = _registered[cell.Id];
		var snapshot = Inspect(cell);
		if (!snapshot.IsValid)
		{
			Fault(registration, string.Join("; ", snapshot.Errors), Now);
			return true;
		}
		var output = snapshot.Outputs.First(x => x.ResourceId == resource.Id);
		var recordedAvailable = Math.Min(output.Balance, output.Maximum);
		var insufficient = mutation == EnvironmentalResourceMutation.Debit && amount > recordedAvailable;
		var now = Now;
		var plan = ProjectSettlement(registration, now);
		var balance = Math.Min(plan.Balances.GetValueOrDefault(resource, cell.MagicResourceAmounts.GetValueOrDefault(resource)), output.Maximum);
		if (!insufficient)
		{
			balance = mutation switch
			{
				EnvironmentalResourceMutation.Add => Math.Clamp(balance + amount, 0.0, output.Maximum),
				EnvironmentalResourceMutation.Set => Math.Min(amount, output.Maximum),
				EnvironmentalResourceMutation.Debit => balance - amount,
				_ => balance
			};
			if (!double.IsFinite(balance)) return true;
			if (mutation == EnvironmentalResourceMutation.Set) plan.ProductionRemainders?.Remove(resource.Id);
		}
		plan.Balances[resource] = balance;
		if (!TrySample(registration, snapshot, profile!, plan, out var samples, out var error))
		{
			Fault(registration, error!, now);
			return true;
		}
		ApplySettlement(registration, plan, now, true);
		SettlePressure(concrete, profile);
		Accept(registration, samples, profile!, now, true);
		success = !insufficient;
		return true;
	}

	public bool TryDebit(ICell cell, IMagicResource resource, double amount, out string? error)
	{
		if (!TryMutateResource(cell, resource, EnvironmentalResourceMutation.Debit, amount, out var success))
		{
			error = "This cell/resource pair is not managed by an environmental profile.";
			return false;
		}
		error = success ? null : "The full recorded amount is unavailable, or the current environment is invalid.";
		return success;
	}

	public EnvironmentalMagicDiagnostics Diagnostics
	{
		get
		{
			var now = Now;
			var readyAt = now;
			if (_due.Min is { } due) readyAt = Math.Min(readyAt, due.DueAt);
			if (_dirty.First is { } dirty) readyAt = Math.Min(readyAt, dirty.Value.DirtyAt);
			if (_audit.Min is { } audit) readyAt = Math.Min(readyAt, audit.AuditAt);
			return new(_registered.Count, _productionCount, _maintenanceCount,
				_registered.Count - _workingCount - _faultCount,
				_dirty.Count, _faultCount, _due.Count, _audit.Count, _discoveryRemaining,
				_lastVisits, _lastEvaluations, _lastProgExecutions, _lastWrites,
				_lastPumpMilliseconds, _maximumPumpMilliseconds, Math.Max(0.0, now - readyAt),
				_auditAge.First is { } oldest ? Math.Max(0.0, now - oldest.Value.LastAudit) : 0.0,
				_budgetLimitedPumps, _totalEvaluations, _totalWrites, _totalProgExecutions, _totalFaults,
				_representativeError, _slowProg, _totalSlowProgs);
		}
	}

	public string DescribeDiagnostics()
	{
		var d = Diagnostics;
		return $"Environmental magic: {d.Configured:N0} configured; {d.ActiveProduction:N0} producing; {d.ActiveMaintenance:N0} maintaining; {d.Dormant:N0} dormant; {d.Dirty:N0} dirty; {d.Faulted:N0} faulted.\n" +
			$"Queues: {d.ProductionQueue:N0} production, {d.AuditQueue:N0} audit, {d.DiscoveryRemaining:N0} discovery remaining. Oldest ready {d.OldestReadySeconds:F2}s; oldest audit {d.OldestAuditSeconds:F2}s.\n" +
			$"Last pump: {d.LastCellVisits:N0} cells, {d.LastEvaluations:N0} evaluations, {d.LastInputProgExecutions:N0} progs, {d.LastWrites:N0} writes; {d.LastPumpMilliseconds:F3} ms (max {d.MaximumPumpMilliseconds:F3} ms); {d.BudgetLimitedPumps:N0} budget-limited pumps.\n" +
			$"Budgets: {Options.MaximumCellVisits:N0} cells / {Options.MaximumOutputWork:N0} outputs / {Options.SoftBudgetMilliseconds:F2} ms; active {Options.ActiveCadenceSeconds:F0}s, audit {Options.ReconciliationSeconds:F0}s.\n" +
			$"Last error: {d.RepresentativeError ?? "none"}\nSlow input progs: {d.TotalSlowInputProgs:N0}; latest: {d.SlowProg ?? "none"}";
	}
}
