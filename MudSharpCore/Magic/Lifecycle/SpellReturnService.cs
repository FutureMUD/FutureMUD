#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Framework.Scheduling;
using MudSharp.Magic.SpellEffects;
using MudSharp.NPC.AI;

namespace MudSharp.Magic.Lifecycle;

/// <summary>An empty-ownership lifecycle journals one return attempt; it never owns the existing caster or room.</summary>
public sealed class SpellReturnService : ISpellReturnService
{
	internal const string BlockedDiagnosticPrefix = "Delayed return needs review; no replay:";
	private readonly IFuturemud _world;
	private readonly ISpellOwnedLifecycleStore _store;
	private readonly Func<SpellLifecycleOrigin, SpellOwnedLifecycle> _create;
	private readonly Func<DateTime, int, IReadOnlyList<SpellOwnedLifecycle>> _pending;
	public SpellReturnService(IFuturemud world)
	{
		_world = world; var store = new SpellOwnedLifecycleStore(); _store = store;
		_create = store.CreateReturnJournal;
		_pending = ReadPending;
	}
	internal SpellReturnService(IFuturemud world, ISpellOwnedLifecycleStore store,
		Func<SpellLifecycleOrigin, SpellOwnedLifecycle> create,
		Func<DateTime, int, IReadOnlyList<SpellOwnedLifecycle>> pending)
	{ _world = world; _store = store; _create = create; _pending = pending; }
	private static IReadOnlyList<SpellOwnedLifecycle> ReadPending(DateTime now, int limit)
	{
		using var isolated = FMDB.BeginIndependentScope(); using var db = new FMDB();
		return FMDB.Context.MagicSpellLifecycles.AsNoTracking().Include(x => x.Entities)
			.Where(x => x.Family == SpellReturnAnchor.Family && x.State == (int)SpellLifecycleState.Active && x.DeadlineUtc <= now &&
				!x.Diagnostic.StartsWith(BlockedDiagnosticPrefix))
			.OrderBy(x => x.DeadlineUtc).ThenBy(x => x.Id).Take(limit).ToArray().Select(SpellOwnedLifecycleStore.Read).ToArray();
	}
	public SpellReturnBinding CreatePrepared(ICharacter caster, IMagicSpell spell, int grade, double secondsPerGrade,
		SpellReturnAnchor anchor, Func<bool> inputsCurrent)
	{
		if (!ReferenceEquals(caster.Gameworld, _world) || !ReferenceEquals(spell.Gameworld, _world) || grade is < 1 or > 7 ||
			!double.IsFinite(secondsPerGrade) || secondsPerGrade is <= 0 or > 86400 ||
			!Owns(anchor, caster) || caster.Body.Id != anchor.BodyId || caster.Location.Id != anchor.RoomId || caster.RoomLayer != anchor.Layer || !inputsCurrent())
			throw new InvalidOperationException("The exact delayed return caster, position or bounded delay changed.");
		SpellReturnAnchor.Parse(anchor.Save());
		var clock = RuntimeClock.UtcNow; var now = new DateTime(clock.Ticks - clock.Ticks % 10, DateTimeKind.Utc);
		var id = Guid.NewGuid(); var rawDeadline = now.AddSeconds(secondsPerGrade * grade);
		var deadline = new DateTime(rawDeadline.Ticks - rawDeadline.Ticks % 10, DateTimeKind.Utc);
		var origin = new SpellLifecycleOrigin(id, spell.Id, grade, anchor.CharacterId, SpellReturnAnchor.Family,
			SpellLifecycleMode.TemporaryCleanup, now, deadline, anchor.Save());
		var life = _create(origin);
		if (life.Origin != origin || life.State != SpellLifecycleState.Active || life.Entities.Count != 0 || !inputsCurrent())
			throw new InvalidOperationException("Delayed return creation needs review; never repeat a paid origin.");
		return new(id, anchor, deadline);
	}
	private static bool Owns(SpellReturnAnchor anchor, ICharacter actor) =>
		CharacterInstanceIdentityComparer.IdentityId(actor) == anchor.CharacterId && actor.InstanceId == anchor.InstanceId;
	private static bool Matches(SpellOwnedLifecycle life, SpellReturnBinding binding) =>
		life.Origin.Id == binding.Id && life.Origin.Family == SpellReturnAnchor.Family && life.Entities.Count == 0 &&
		life.Origin.CreatorId == binding.Anchor.CharacterId && life.Origin.DeadlineUtc == binding.DeadlineUtc &&
		life.Origin.Provenance == binding.Anchor.Save();
	private void CompleteCancellation(SpellOwnedLifecycle life, SpellRetirementReason reason, DateTime now)
	{
		if (life.State != SpellLifecycleState.Active || life.Diagnostic.StartsWith(BlockedDiagnosticPrefix, StringComparison.Ordinal)) return;
		life = _store.BeginRetirement(life.Origin.Id, life.Version, reason, now);
		_store.Complete(life.Origin.Id, life.Version, now);
	}
	public void Cancel(SpellReturnBinding binding, ICharacter actor, SpellRetirementReason reason)
	{
		if (!Owns(binding.Anchor, actor)) return;
		var life = _store.Find(binding.Id);
		if (life is null || !Matches(life, binding)) return;
		CompleteCancellation(life, reason, RuntimeClock.UtcNow);
	}
	public bool TryReturn(SpellReturnBinding binding, ICharacter actor, IMagicSpell spell, Func<bool> stillAttached)
	{
		var life = _store.Find(binding.Id);
		if (life is null || !Matches(life, binding) || life.State != SpellLifecycleState.Active ||
			life.Diagnostic.StartsWith(BlockedDiagnosticPrefix, StringComparison.Ordinal)) return true;
		if (!Owns(binding.Anchor, actor) || !ReferenceEquals(actor.Gameworld, _world) || !ReferenceEquals(spell.Gameworld, _world) || spell.Id != life.Origin.SpellId)
			return true;
		var now = RuntimeClock.UtcNow;
		if (now < binding.DeadlineUtc) return false;
		var anchor = binding.Anchor; var destination = _world.Rooms.Get(anchor.RoomId);
		if (actor.Body?.Id != anchor.BodyId || !CommandExecutionAuthority.IsCurrent(actor, false) || !stillAttached() ||
			destination is null || destination.CurrentOverlay.Id != anchor.OverlayId || destination.CurrentOverlay.Terrain.Id != anchor.TerrainId)
		{ CompleteCancellation(life, SpellRetirementReason.AnchorSevered, now); return true; }
		var source = actor.Location; var sourceLayer = actor.RoomLayer;
		SpellTravelSnapshot snapshot;
		try { snapshot = new(_world, [actor], [source, destination]); }
		catch (InvalidOperationException) { CompleteCancellation(life, SpellRetirementReason.AnchorSevered, now); return true; }
		if (!snapshot.AtCapturedPositions || SpellTravelEndpointPolicy.RelocationError(actor, actor, source, destination, spell, sourceLayer, anchor.Layer) is not null ||
			!snapshot.AtCapturedPositions || !stillAttached())
		{ CompleteCancellation(life, SpellRetirementReason.AnchorSevered, now); return true; }
		// A process that discovers Retiring never retries movement. Claim/commit uncertainty is therefore at-most-once.
		try
		{
			life = _store.BeginRetirement(binding.Id, life.Version, SpellRetirementReason.Expiry, now);
			bool Raw() => snapshot.Current && stillAttached();
			bool Current() => Raw() && SpellTravelEndpointPolicy.InFlightError(actor, actor, source, destination, spell, sourceLayer, anchor.Layer) is null && Raw();
			if (!Current()) throw new InvalidOperationException("The delayed return inputs changed after their durable attempt claim.");
			using var independent = CommandExecutionScope.EnterIndependent();
			using var displacement = CommandExecutionScope.EnterSoloDisplacement(actor, Current);
			if (!ReferenceEquals(source, destination) || sourceLayer != anchor.Layer)
				actor.Teleport(destination, anchor.Layer, false, true);
			if (!snapshot.Travellers.Single().At(destination, anchor.Layer) || !Current())
				throw new InvalidOperationException("The delayed return did not establish the exact saved body position and native membership.");
			_store.Complete(binding.Id, life.Version, now);
		}
		catch (Exception ex)
		{
			var current = _store.Find(binding.Id);
			if (current is { State: not SpellLifecycleState.Completed })
				_store.Hold(binding.Id, current.Version, BlockedDiagnosticPrefix + " " + ex.Message[..Math.Min(ex.Message.Length, 1500)], now);
		}
		return true;
	}
	public int Reconcile(DateTime nowUtc, int limit = 100)
	{
		if (nowUtc.Kind != DateTimeKind.Utc || limit is < 1 or > 1000) throw new ArgumentException("Delayed return reconciliation requires bounded UTC input.");
		var pending = _pending(nowUtc, limit);
		foreach (var life in pending)
		{
			if (life.Origin.Family != SpellReturnAnchor.Family || life.State != SpellLifecycleState.Active || life.Origin.DeadlineUtc > nowUtc ||
				life.Diagnostic.StartsWith(BlockedDiagnosticPrefix, StringComparison.Ordinal)) continue;
			var anchor = SpellReturnAnchor.Parse(life.Origin.Provenance);
			var actor = _world.Actors.FirstOrDefault(x => Owns(anchor, x) && x.Body.Id == anchor.BodyId);
			var child = actor?.Effects.OfType<SpellReturnTetherEffect>().SingleOrDefault(x => x.Binding.Id == life.Origin.Id);
			if (child is null) CompleteCancellation(life, SpellRetirementReason.Reboot, nowUtc);
			else child.CheckReturn();
		}
		return pending.Count;
	}
}
