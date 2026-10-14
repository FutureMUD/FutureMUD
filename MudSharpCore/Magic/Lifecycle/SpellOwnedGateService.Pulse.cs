#nullable enable

using System.Data;
using Microsoft.EntityFrameworkCore;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework.Scheduling;
using MudSharp.Health;

namespace MudSharp.Magic.Lifecycle;

public sealed partial class SpellOwnedGateService
{
	/// <summary>
	/// Claim each native pulse in the journal before invoking health callbacks. A crash may omit a
	/// pulse, but restart cannot replay harm already delivered by that claim.
	/// </summary>
	private SpellOwnedLifecycle PulseClosedRooms(SpellOwnedLifecycle life, GateTopology topology, SpellGateAnchor anchor)
	{
		var occupants = _world.Actors
			.Where(x => (x.Location?.Id == topology.South || x.Location?.Id == topology.North) &&
				!x.State.HasFlag(CharacterState.Dead))
			.Concat(new[] { _world.Rooms.Get(topology.South), _world.Rooms.Get(topology.North) }
				.Where(x => x is not null).SelectMany(x => x!.Characters))
			.Distinct(ReferenceEqualityComparer.Instance).OfType<ICharacter>()
			.Where(x => !x.State.HasFlag(CharacterState.Dead) &&
				(x.Location?.Id == topology.South || x.Location?.Id == topology.North) &&
				!x.Merits.Any(m => m.Id == anchor.UndeadMeritId && m.Applies(x)))
			.ToArray();
		if (occupants.Length == 0) return life;
		var now = RuntimeClock.UtcNow;
		using (var writes = FMDB.BeginIndependentScope(requireWrites: true))
		using (var db = new FMDB())
		using (var transaction = FMDB.Context.Database.BeginTransaction(IsolationLevel.Serializable))
		{
			var row = FMDB.Context.MagicSpellLifecycles.Single(x => x.Id == life.Origin.Id);
			if (row.Version != life.Version || row.State != (int)SpellLifecycleState.Retiring ||
				row.GateLinksClosedUtc is null || row.GateLastPulseUtc is null)
				throw new InvalidOperationException("Nilaz pulse authority changed or external links remain open.");
			if (now - row.GateLastPulseUtc.Value < TimeSpan.FromSeconds(anchor.SuffocationPulseSeconds)) return life;
			row.GateLastPulseUtc = now;
			row.UpdatedUtc = now < row.UpdatedUtc ? row.UpdatedUtc : now;
			row.Version++;
			FMDB.Context.SaveChanges(); transaction.Commit();
		}
		life = _store.Find(life.Origin.Id)!;
		foreach (var actor in occupants)
		{
			if (actor.State.HasFlag(CharacterState.Dead) ||
				(actor.Location?.Id != topology.South && actor.Location?.Id != topology.North) ||
				actor.Merits.Any(m => m.Id == anchor.UndeadMeritId && m.Applies(actor))) continue;
			var part = actor.Body.Bodyparts.FirstOrDefault();
			if (part is null) continue;
			actor.SufferDamage(new Damage
			{
				DamageType = DamageType.Hypoxia, DamageAmount = Random.Shared.Next(1, 26),
				StunAmount = Random.Shared.Next(1, 26), Bodypart = part, TargetBody = actor.Body
			}).ToArray();
		}
		_world.SaveManager.Flush();
		return life;
	}
}
