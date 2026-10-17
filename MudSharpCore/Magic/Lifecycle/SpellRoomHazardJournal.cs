#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Database;

namespace MudSharp.Magic.Lifecycle;

public sealed class SpellRoomHazardJournal : ISpellRoomHazardJournal
{
	internal const string Family = "room-electrical-hazard";
	internal const string HeldPrefix = "Room hazard needs review; no further pulses:";
	private readonly SpellOwnedLifecycleStore _store = new();
	public SpellOwnedLifecycle Create(SpellLifecycleOrigin origin) => _store.CreateHazardJournal(origin);
	public SpellOwnedLifecycle? Find(Guid id) => _store.Find(id);
	public SpellOwnedLifecycle ClaimPulse(Guid id, long expectedVersion, DateTime nowUtc) => _store.ClaimHazardPulse(id, expectedVersion, nowUtc);
	public void Hold(Guid id, DateTime nowUtc, string diagnostic)
	{
		if (_store.Find(id) is { State: not SpellLifecycleState.Completed } life)
			_store.Hold(id, life.Version, HeldPrefix + " " + diagnostic[..Math.Min(diagnostic.Length, 1500)], nowUtc);
	}
	public void Retire(Guid id, SpellRetirementReason reason, DateTime nowUtc)
	{
		var life = _store.Find(id);
		if (life is null || life.Origin.Family != Family || life.State == SpellLifecycleState.Completed || life.Diagnostic.StartsWith(HeldPrefix, StringComparison.Ordinal)) return;
		if (life.State == SpellLifecycleState.Active) life = _store.BeginRetirement(id, life.Version, reason, nowUtc);
		_store.Complete(id, life.Version, nowUtc);
	}
	public int Reconcile(DateTime nowUtc, int limit = 100)
	{
		if (nowUtc.Kind != DateTimeKind.Utc || limit is < 1 or > 1000) throw new ArgumentException("Room hazard reconciliation requires bounded UTC input.");
		using var isolated = FMDB.BeginIndependentScope(); using var db = new FMDB();
		var ids = FMDB.Context.MagicSpellLifecycles.AsNoTracking().Where(x => x.Family == Family &&
			x.State != (int)SpellLifecycleState.Completed && x.DeadlineUtc <= nowUtc && !x.Diagnostic.StartsWith(HeldPrefix))
			.OrderBy(x => x.DeadlineUtc).ThenBy(x => x.Id).Take(limit).Select(x => x.Id).ToArray();
		foreach (var id in ids) Retire(id, SpellRetirementReason.Expiry, nowUtc);
		return ids.Length;
	}
}
