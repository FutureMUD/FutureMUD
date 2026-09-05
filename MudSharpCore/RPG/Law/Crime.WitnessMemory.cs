#nullable enable

namespace MudSharp.RPG.Law;

public partial class Crime
{
	private readonly List<CrimeWitnessMemory> _witnessMemories = [];
	private readonly HashSet<CrimeWitnessMemory> _scheduledWitnesses = [];
	public IReadOnlyList<CrimeWitnessMemory> WitnessMemories => _witnessMemories;

	private void InitialiseWitnessMemories(string? xml)
	{
		if (!string.IsNullOrWhiteSpace(xml))
		{
			_witnessMemories.AddRange(XElement.Parse(xml).Elements("Witness").Select(CrimeWitnessMemory.Load));
		}
		foreach (var id in _witnessIds.Distinct())
		{
			if (_witnessMemories.Any(x => x.Kind == CrimeWitnessSourceKind.Character && x.SourceId == id)) continue;
			_witnessMemories.Add(new CrimeWitnessMemory { Kind = CrimeWitnessSourceKind.Character, SourceId = id, LocationId = CrimeLocation?.Id });
		}
		foreach (var memory in _witnessMemories.Where(x => x.ReportDueUtc.HasValue && !x.ReportDelivered)) ScheduleReport(memory);
	}

	private string SaveWitnessMemories() => new XElement("Witnesses", _witnessMemories.Select(x => x.Save())).ToString();

	public bool CanWitnessRecall(long identityId) => _witnessMemories.Any(x =>
		x.Kind == CrimeWitnessSourceKind.Character && x.SourceId == identityId && x.CanRecall(RuntimeClock.UtcNow));

	public void QueueVirtualReport(long profileId, bool identityKnown, double reliability, TimeSpan delay, bool willReport = true)
	{
		if (_witnessMemories.Any(x => x.Kind == CrimeWitnessSourceKind.Virtual && x.SourceId == profileId)) return;
		var memory = new CrimeWitnessMemory
		{
			Kind = CrimeWitnessSourceKind.Virtual, SourceId = profileId, LocationId = CrimeLocation?.Id,
			IdentityKnown = identityKnown, Reliability = Math.Clamp(reliability, 0, 1),
			ReportDueUtc = willReport ? RuntimeClock.UtcNow.Add(delay) : null
		};
		_witnessMemories.Add(memory);
		Changed = true;
		if (!willReport) return;
		if (delay <= TimeSpan.Zero) DeliverReport(memory);
		else ScheduleReport(memory);
	}

	public void ForgetWitness(CrimeWitnessMemory memory, ICharacter actor, TimeSpan duration, bool permanent)
	{
		if (!_witnessMemories.Contains(memory)) throw new ArgumentException("Witness does not belong to this crime.", nameof(memory));
		memory.Forget(RuntimeClock.UtcNow, duration, permanent, CharacterInstanceIdentityComparer.IdentityId(actor));
		Changed = true;
	}

	internal void CancelPendingWitnessReports()
	{
		foreach (var memory in _witnessMemories.Where(x => x.ReportDueUtc.HasValue))
		{
			memory.ReportDueUtc = null;
			Changed = true;
		}
	}

	public void RestoreWitness(CrimeWitnessMemory memory, ICharacter actor)
	{
		if (!_witnessMemories.Contains(memory)) throw new ArgumentException("Witness does not belong to this crime.", nameof(memory));
		memory.Restore(RuntimeClock.UtcNow, CharacterInstanceIdentityComparer.IdentityId(actor));
		Changed = true;
		// An existing callback may be waiting until the old suppression expiry.
		// Deliver now when overdue; that callback becomes a harmless no-op.
		if (memory.ReportDueUtc <= RuntimeClock.UtcNow) DeliverReport(memory);
		ScheduleReport(memory);
	}

	private void ScheduleReport(CrimeWitnessMemory memory)
	{
		if (memory.ReportDelivered || memory.ReportDueUtc is null || memory.PermanentlyForgotten ||
		    !_scheduledWitnesses.Add(memory)) return;
		var due = memory.ReportDueUtc.Value;
		if (memory.SuppressedUntilUtc is { } until && until > due) due = until;
		Gameworld.Scheduler.AddSchedule(new Schedule(() =>
		{
			_scheduledWitnesses.Remove(memory);
			DeliverReport(memory);
		}, ScheduleType.System, due > RuntimeClock.UtcNow ? due - RuntimeClock.UtcNow : TimeSpan.Zero,
			"Pending virtual witness report"));
	}

	private void DeliverReport(CrimeWitnessMemory memory)
	{
		if (memory.ReportDelivered || memory.ReportDueUtc is null) return;
		if (HasBeenFinalised)
		{
			memory.ReportDueUtc = null;
			Changed = true;
			return;
		}
		if (!memory.CanRecall(RuntimeClock.UtcNow))
		{
			ScheduleReport(memory);
			return;
		}
		LegalAuthority.ReportVirtualCrime(this, memory);
		memory.ReportDelivered = true;
		memory.ReportDueUtc = null;
		Changed = true;
	}
}
