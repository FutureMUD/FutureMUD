#nullable enable

using System.Globalization;
using System.Data;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Character;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.NPC.Templates;
using RuntimeNpc = MudSharp.NPC.NPC;

namespace MudSharp.Magic.Lifecycle;

/// <summary>Native construction and post-persistence death proof; no destructive retirement work.</summary>
public sealed partial class SpellOwnedNpcService(IFuturemud world) : ISpellOwnedNpcService
{
	private readonly SpellOwnedLifecycleStore _store = new();
	private long _lastInspectedNpcId;
	internal const string ActivationPendingDiagnostic = "Native NPC activation pending";

	public ICharacter Create(INPCTemplate template, SpatialLocation location, SpellLifecycleOrigin origin)
	{
		origin.Validate();
		using var authorization = FMDB.BeginIndependentScope(requireWrites: true);
		if (!ReferenceEquals(template.Gameworld, world) || !RouteSpatialService.Instance.TryValidateLocation(location, out var error))
			throw new ArgumentException("The native template or spawn location is invalid.");
		if (_store.Find(origin.Id) is not null) throw new InvalidOperationException("This native creation cannot be replayed.");
		if (NativeNpcCreationEligibility.TemplateError(template, world) is { } templateError)
			throw new InvalidOperationException(templateError);
		// Template preparation may evaluate authored selection predicates. It is outside the row factory;
		// the private actor has no controller, subscriptions, queued initialisation or world/cell presence.
		var characterTemplate = template.GetCharacterTemplate(location.Room);
		if (NativeNpcCreationEligibility.CharacterTemplateError(characterTemplate) is { } eligibilityError)
			throw new InvalidOperationException(eligibilityError);
		var npc = new RuntimeNpc(world, characterTemplate, template, deferInitialisation: true);
		npc.MoveTo(location, noSave: true);
		object? inserted = null;
		SpellOwnedLifecycle lifecycle;
		try
		{
			lifecycle = _store.Create(origin, creation =>
			{
				inserted = npc.DatabaseInsert();
				var model = (MudSharp.Models.Character)((Tuple<object, MudSharp.Models.Npc>)inserted).Item1;
				model.State = (int)CharacterState.Stasis;
				foreach (var primary in model.CharacterInstances.Where(x => x.IsPrimary)) primary.State = (int)CharacterState.Stasis;
				creation.Claim(SpellOwnedEntityKind.AutonomousCharacter, model);
				creation.Claim(SpellOwnedEntityKind.Body, model.Body);
			}, ActivationPendingDiagnostic);
		}
		catch
		{
			npc.ReleaseUnpublishedNativeNpc();
			throw;
		}
		try
		{
			npc.ActivateCommittedNativeNpc(characterTemplate, inserted!);
			CommitActivation(npc, lifecycle);
			return npc;
		}
		catch (Exception ex)
		{
			npc.ReleaseUnpublishedNativeNpc();
			RecordHold(lifecycle, ActivationPendingDiagnostic + ": activation failed; creation must not replay: " + ex.Message, RuntimeClock.UtcNow);
			throw;
		}
	}

	private static void CommitActivation(RuntimeNpc npc, SpellOwnedLifecycle lifecycle)
	{
		using var isolated = FMDB.BeginIndependentScope(requireWrites: true); using var db = new FMDB();
		using var transaction = FMDB.Context.Database.BeginTransaction(IsolationLevel.Serializable);
		var row = FMDB.Context.MagicSpellLifecycles.Include(x => x.Entities).Single(x => x.Id == lifecycle.Origin.Id);
		if (row.Version != lifecycle.Version || !row.Diagnostic.StartsWith(ActivationPendingDiagnostic, StringComparison.Ordinal) ||
			row.State != (int)SpellLifecycleState.Active)
			throw new InvalidOperationException("Native activation changed concurrently; the committed creation must remain held.");
		npc.Save(); npc.Body.Save();
		foreach (var trait in npc.CharacterTraits)
		{
			if (trait is TheoreticalSkill) { trait.Save(); continue; }
			// Creation preserves authored raw skills. Ordinary Skill.Save writes its dynamically
			// capped Value, which must not erase raw history during activation.
			var model = FMDB.Context.CharacterTraits.Find(npc.Id, trait.Definition.Id);
			if (model is null)
			{
				model = new() { CharacterId = npc.Id, TraitDefinitionId = trait.Definition.Id };
				FMDB.Context.CharacterTraits.Add(model);
			}
			model.Value = trait.RawValue;
			trait.Changed = false;
		}
		foreach (var trait in npc.Body.Traits.Where(x => x.Definition.OwnerScope == TraitOwnerScope.Body)) trait.Save();
		foreach (var knowledge in npc.CharacterKnowledges) knowledge.Save();
		row.Diagnostic = ""; row.Version = checked(row.Version + 1); row.UpdatedUtc = TransitionTime(lifecycle, RuntimeClock.UtcNow);
		if (lifecycle.Origin.Mode == SpellLifecycleMode.Permanent) row.State = (int)SpellLifecycleState.Completed;
		FMDB.Context.SaveChanges(); transaction.Commit();
		// Only this private graph was saved. Unrelated pending gameplay saves must never be flushed here.
		npc.Gameworld.SaveManager.Abort(npc); npc.Gameworld.SaveManager.Abort(npc.Body);
		foreach (var trait in npc.CharacterTraits.Concat(npc.Body.Traits)) npc.Gameworld.SaveManager.Abort(trait);
		foreach (var knowledge in npc.CharacterKnowledges) npc.Gameworld.SaveManager.Abort(knowledge);
	}

	internal static bool IsActivationPending(FuturemudDatabaseContext context, long characterId) =>
		context.MagicSpellLifecycles.Any(x => x.Diagnostic.StartsWith(ActivationPendingDiagnostic) &&
			x.Entities.Any(e => e.Kind == (int)SpellOwnedEntityKind.AutonomousCharacter && e.EntityId == characterId));

	public void ObserveNativeDeath(ICharacter character, IGameItem? remains)
	{
		var lifecycle = FindNpc(character.Id);
		if (lifecycle is null || lifecycle.State == SpellLifecycleState.Completed || lifecycle.DeathObservedUtc is not null) return;
		try
		{
			// Native item IDs flush pending insertion. ObserveDeath then independently verifies dead state
			// and the exact persisted body definition. Pre-death events never reach this seam.
			var remainsId = remains?.Id;
			RequireExactBody(lifecycle, character.Body.Id);
			_store.ObserveDeath(lifecycle.Origin.Id, lifecycle.Version, remainsId, TransitionTime(lifecycle, RuntimeClock.UtcNow));
		}
		catch (Exception ex)
		{
			RecordHold(lifecycle, "Persisted native death correlation needs retry: " + ex.Message, RuntimeClock.UtcNow);
		}
	}

	public int ReconcilePersistedDeaths(DateTime nowUtc, int limit = 100)
	{
		if (nowUtc.Kind != DateTimeKind.Utc || limit is < 1 or > 1000) throw new ArgumentException("A bounded UTC reconciliation is required.");
		long[] actors;
		using (var isolated = FMDB.BeginIndependentScope())
		using (var db = new FMDB())
		{
			actors = (from claim in FMDB.Context.MagicSpellOwnedEntities.AsNoTracking()
				join life in FMDB.Context.MagicSpellLifecycles.AsNoTracking() on claim.LifecycleId equals life.Id
				join actor in FMDB.Context.Characters.AsNoTracking() on claim.EntityId equals actor.Id
				where claim.Kind == (int)SpellOwnedEntityKind.AutonomousCharacter && claim.EntityId > _lastInspectedNpcId &&
					life.State != (int)SpellLifecycleState.Completed && life.DeathObservedUtc == null &&
					(actor.State & (int)CharacterState.Dead) != 0
				orderby claim.EntityId
				select claim.EntityId).Take(limit).ToArray();
		}
		if (actors.Length == 0) { _lastInspectedNpcId = 0; return 0; }
		foreach (var id in actors)
		{
			_lastInspectedNpcId = id;
			var lifecycle = FindNpc(id);
			if (lifecycle is null || lifecycle.DeathObservedUtc is not null || lifecycle.State == SpellLifecycleState.Completed) continue;
			try
			{
				long bodyId;
				long? remainsId;
				using (var isolated = FMDB.BeginIndependentScope())
				using (var db = new FMDB())
				{
					bodyId = FMDB.Context.Characters.Where(x => x.Id == id).Select(x => x.BodyId).Single()
						?? throw new InvalidOperationException("The dead identity has no physical body; it cannot be materialized to recover death proof.");
					RequireExactBody(lifecycle, bodyId);
					remainsId = FindPersistedRemains(bodyId);
				}
				_store.ObserveDeath(lifecycle.Origin.Id, lifecycle.Version, remainsId, TransitionTime(lifecycle, nowUtc));
			}
			catch (Exception ex) { RecordHold(lifecycle, "Persisted death reconciliation held: " + ex.Message, nowUtc); }
		}
		return actors.Length;
	}

	private SpellOwnedLifecycle? FindNpc(long characterId)
	{
		using var isolated = FMDB.BeginIndependentScope(); using var db = new FMDB();
		return FMDB.Context.MagicSpellLifecycles.AsNoTracking().Include(x => x.Entities)
			.SingleOrDefault(x => x.Entities.Any(e => e.Kind == (int)SpellOwnedEntityKind.AutonomousCharacter && e.EntityId == characterId))
			is { } row ? SpellOwnedLifecycleStore.Read(row) : null;
	}

	private static void RequireExactBody(SpellOwnedLifecycle lifecycle, long bodyId)
	{
		if (lifecycle.Entities.Count(x => x.Kind == SpellOwnedEntityKind.AutonomousCharacter) != 1 ||
			lifecycle.Entities.Count(x => x.Kind == SpellOwnedEntityKind.Body) != 1 ||
			lifecycle.Entities.Single(x => x.Kind == SpellOwnedEntityKind.Body).Id != bodyId)
			throw new InvalidOperationException("Native death must retain this creation's exact owned body; changed or borrowed bodies are held.");
	}

	private static long? FindPersistedRemains(long bodyId)
	{
		var token = bodyId.ToString(CultureInfo.InvariantCulture);
		// XML character references can encode any digit of a native body's exact ID. Include
		// those definitions in the bounded census rather than falsely proving remains absence.
		var candidates = FMDB.Context.GameItemComponents.AsNoTracking().Where(x => x.Definition.Contains(token) || x.Definition.Contains("&#"))
			.Select(x => new { x.GameItemId, x.Definition }).Take(257).ToArray();
		if (candidates.Length > 256) throw new InvalidOperationException("The remains reference census exceeds its bounded inspection limit.");
		var matches = new HashSet<long>();
		foreach (var candidate in candidates)
		{
			var xml = XElement.Parse(candidate.Definition); // Malformed candidates hold rather than proving absence.
			if ((long?)xml.Element("OriginalBody") == bodyId || (long?)xml.Element("OriginalBodyId") == bodyId)
				matches.Add(candidate.GameItemId);
		}
		return matches.Count switch
		{
			0 => null,
			1 => matches.Single(),
			_ => throw new InvalidOperationException("Multiple persisted remains reference this body; automatic death correlation is ambiguous.")
		};
	}

	private void RecordHold(SpellOwnedLifecycle lifecycle, string diagnostic, DateTime nowUtc)
	{
		var current = _store.Find(lifecycle.Origin.Id)!;
		if (current.DeathObservedUtc is not null || current.State == SpellLifecycleState.Completed) return;
		_store.Hold(current.Origin.Id, current.Version, diagnostic[..Math.Min(diagnostic.Length, 2048)], TransitionTime(current, nowUtc));
	}

	private static DateTime TransitionTime(SpellOwnedLifecycle life, DateTime nowUtc) => nowUtc < life.UpdatedUtc ? life.UpdatedUtc : nowUtc;
}
