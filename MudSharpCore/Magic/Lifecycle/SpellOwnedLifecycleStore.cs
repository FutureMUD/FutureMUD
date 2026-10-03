#nullable enable

using System.Data;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;
using MudSharp.Character;
using System.Xml.Linq;

namespace MudSharp.Magic.Lifecycle;

/// <summary>Immediate optimistic journal writes, independent of deferred gameplay saves.</summary>
public sealed class SpellOwnedLifecycleStore : ISpellOwnedLifecycleStore
{
	public SpellOwnedLifecycle Create(SpellLifecycleOrigin origin, Action<SpellOwnedCreation> create, string initialDiagnostic = "")
	{
		origin.Validate();
		if (initialDiagnostic is null || initialDiagnostic.Length > 2048) throw new ArgumentException("Invalid creation diagnostic.");
		using var isolated = FMDB.BeginIndependentScope(requireWrites: true);
		using var db = new FMDB();
		using var transaction = FMDB.Context.Database.BeginTransaction(IsolationLevel.Serializable);
		if (FMDB.Context.MagicSpellLifecycles.Any(x => x.Id == origin.Id))
		{
			throw new InvalidOperationException("This lifecycle creation key has already been used; creation cannot be replayed.");
		}
		if (!FMDB.Context.Characters.Any(x => x.Id == origin.CreatorId) ||
		    !FMDB.Context.MagicSpells.Any(x => x.Id == origin.SpellId))
		{
			throw new InvalidOperationException("Creation needs a persisted canonical creator and source spell.");
		}
		var creation = new SpellOwnedCreation(FMDB.Context, origin.CreatorId);
		create(creation);
		creation.ValidateBeforeSave();
		FMDB.Context.SaveChanges();
		var claims = creation.SavedClaims();
		if (claims.Any(x => x.Id <= 0) || claims.DistinctBy(x => (x.Kind, x.Id)).Count() != claims.Count)
		{
			throw new InvalidOperationException("Created entity IDs are missing or duplicated.");
		}
		if (origin.Mode == SpellLifecycleMode.DeathOnExpiry &&
		    !claims.Any(x => x.Kind is SpellOwnedEntityKind.AutonomousCharacter or SpellOwnedEntityKind.CharacterInstance))
		{
			throw new InvalidOperationException("Death-on-expiry requires a created actor.");
		}
		var row = new MagicSpellLifecycle
		{
			Id = origin.Id, SpellId = origin.SpellId, Grade = origin.Grade, CreatorId = origin.CreatorId,
			Family = origin.Family, Mode = (int)origin.Mode, CreatedUtc = origin.CreatedUtc,
			DeadlineUtc = origin.DeadlineUtc, Provenance = origin.Provenance,
			State = (int)SpellLifecycleState.Active, UpdatedUtc = origin.CreatedUtc, Version = 1, Diagnostic = initialDiagnostic
		};
		foreach (var claim in claims)
		{
			row.Entities.Add(new() { Kind = (int)claim.Kind, EntityId = claim.Id, Role = (int)claim.Role, Lifecycle = row });
		}
		FMDB.Context.MagicSpellLifecycles.Add(row);
		FMDB.Context.SaveChanges();
		transaction.Commit();
		return Read(row);
	}

	public SpellOwnedLifecycle? Find(Guid id)
	{
		using var isolated = FMDB.BeginIndependentScope();
		using var db = new FMDB();
		return FMDB.Context.MagicSpellLifecycles.AsNoTracking().Include(x => x.Entities)
			.SingleOrDefault(x => x.Id == id) is { } row ? Read(row) : null;
	}

	public IReadOnlyList<SpellOwnedLifecycle> Pending(DateTime nowUtc, int limit = 100)
	{
		if (nowUtc.Kind != DateTimeKind.Utc || limit is < 1 or > 1000) throw new ArgumentException("Invalid pending query bound or UTC time.");
		using var isolated = FMDB.BeginIndependentScope();
		using var db = new FMDB();
		return FMDB.Context.MagicSpellLifecycles.AsNoTracking().Include(x => x.Entities)
			.Where(x => x.State == (int)SpellLifecycleState.Retiring || x.State == (int)SpellLifecycleState.RemainsPending ||
				x.State == (int)SpellLifecycleState.Active && x.DeadlineUtc <= nowUtc)
			.OrderBy(x => x.UpdatedUtc).ThenBy(x => x.Id).Take(limit).ToArray().Select(Read).ToArray();
	}

	public SpellOwnedLifecycle BeginRetirement(Guid id, long expectedVersion, SpellRetirementReason reason, DateTime nowUtc) =>
		Update(id, expectedVersion, nowUtc, (row, current) =>
		{
			var next = SpellLifecycleTransitions.BeginRetirement(current, reason, nowUtc);
			if (next == current.State) return false;
			row.State = (int)next; row.Reason = (int)reason; return true;
		});

	public SpellOwnedLifecycle ObserveDeath(Guid id, long expectedVersion, long? remainsItemId, DateTime nowUtc) =>
		Update(id, expectedVersion, nowUtc, (row, current) =>
		{
			if (!current.Entities.Any(x => x.Kind is SpellOwnedEntityKind.AutonomousCharacter or SpellOwnedEntityKind.CharacterInstance) ||
			    remainsItemId is <= 0) throw new InvalidOperationException("Death correlation requires a created actor and a valid optional remains ID.");
			if (current.DeathObservedUtc is not null)
			{
				if (current.RemainsItemId != remainsItemId) throw new InvalidOperationException("Recorded native remains cannot be replaced.");
				return false;
			}
			if (current.State == SpellLifecycleState.Completed) throw new InvalidOperationException("A completed lifecycle cannot be reopened by death.");
			var actor = current.Entities.Single(x => x.Kind is SpellOwnedEntityKind.AutonomousCharacter or SpellOwnedEntityKind.CharacterInstance);
			var bodyId = PersistedDeadActorBody(actor);
			if (remainsItemId is { } remains && !FMDB.Context.GameItemComponents.AsNoTracking()
				.Where(x => x.GameItemId == remains).Select(x => x.Definition).AsEnumerable()
				.Any(x => IsRemainsForBody(x, bodyId)))
			{
				throw new InvalidOperationException("Native remains must retain the exact created actor's body.");
			}
			row.DeathObservedUtc = nowUtc; row.RemainsItemId = remainsItemId;
			row.Reason ??= (int)SpellRetirementReason.EarlyDeath;
			row.State = (int)SpellLifecycleState.RemainsPending; return true;
		});

	public SpellOwnedLifecycle Hold(Guid id, long expectedVersion, string diagnostic, DateTime nowUtc) =>
		Update(id, expectedVersion, nowUtc, (row, current) =>
		{
			if (string.IsNullOrWhiteSpace(diagnostic) || diagnostic.Length > 2048) throw new ArgumentException("A bounded diagnostic is required.");
			if (current.State == SpellLifecycleState.Completed) return false;
			if (row.Diagnostic == diagnostic) return false;
			row.Diagnostic = diagnostic; return true;
		});

	public SpellOwnedLifecycle Complete(Guid id, long expectedVersion, DateTime nowUtc) =>
		Update(id, expectedVersion, nowUtc, (row, current) =>
		{
			if (current.State == SpellLifecycleState.Completed) return false;
			if (current.RequiresNativeDeath) throw new InvalidOperationException("Required native death has not been correlated; retirement must remain pending.");
			if (current.Origin.Mode != SpellLifecycleMode.Permanent &&
			    (current.State == SpellLifecycleState.Active || current.Entities.Any(x => EntityExists(x, current)) ||
			     current.RemainsItemId is { } remains && FMDB.Context.GameItems.Any(x => x.Id == remains)))
			{
				throw new InvalidOperationException("Retirement is incomplete: owned rows or dependent remains still exist.");
			}
			row.State = (int)SpellLifecycleState.Completed; row.Diagnostic = ""; return true;
		});

	private static SpellOwnedLifecycle Update(Guid id, long expectedVersion, DateTime nowUtc,
		Func<MagicSpellLifecycle, SpellOwnedLifecycle, bool> change)
	{
		using var isolated = FMDB.BeginIndependentScope(requireWrites: true);
		using var db = new FMDB();
		using var transaction = FMDB.Context.Database.BeginTransaction(IsolationLevel.Serializable);
		var row = FMDB.Context.MagicSpellLifecycles.Include(x => x.Entities).Single(x => x.Id == id);
		var current = Read(row);
		if (current.Version != expectedVersion) throw new InvalidOperationException("Lifecycle changed concurrently; reload before acting.");
		SpellLifecycleTransitions.ValidateUtc(nowUtc, current);
		if (change(row, current))
		{
			row.Version = checked(row.Version + 1); row.UpdatedUtc = nowUtc;
			FMDB.Context.SaveChanges();
		}
		transaction.Commit();
		return Read(row);
	}

	private static bool EntityExists(SpellOwnedEntity entity, SpellOwnedLifecycle lifecycle) => entity.Kind switch
	{
		SpellOwnedEntityKind.GameItem => FMDB.Context.GameItems.Any(x => x.Id == entity.Id),
		SpellOwnedEntityKind.AutonomousCharacter => !HasCompactedIdentity(entity.Id, lifecycle),
		SpellOwnedEntityKind.CharacterInstance => FMDB.Context.CharacterInstances.Any(x => x.Id == entity.Id),
		SpellOwnedEntityKind.Body => FMDB.Context.Bodies.Any(x => x.Id == entity.Id),
		SpellOwnedEntityKind.Cell => FMDB.Context.Cells.Any(x => x.Id == entity.Id),
		SpellOwnedEntityKind.Exit => FMDB.Context.Exits.Any(x => x.Id == entity.Id),
		_ => throw new InvalidOperationException("Unknown owned entity kind; retirement is blocked.")
	};

	private static bool HasCompactedIdentity(long characterId, SpellOwnedLifecycle lifecycle)
	{
		if (lifecycle.DeathObservedUtc is null ||
		    lifecycle.Entities.Count(x => x.Kind == SpellOwnedEntityKind.AutonomousCharacter) != 1 ||
		    lifecycle.Entities.Count(x => x.Kind == SpellOwnedEntityKind.Body) != 1) return false;
		var bodyId = lifecycle.Entities.Single(x => x.Kind == SpellOwnedEntityKind.Body).Id;
		return FMDB.Context.Characters.Any(x => x.Id == characterId && x.IsArchived && x.BodyId == null) &&
		       FMDB.Context.CharacterArchives.Any(x => x.CharacterId == characterId &&
			       x.LifecycleId == lifecycle.Origin.Id && x.OriginalBodyId == bodyId) &&
		       !FMDB.Context.Bodies.Any(x => x.Id == bodyId) &&
		       !FMDB.Context.Npcs.Any(x => x.CharacterId == characterId) &&
		       !FMDB.Context.CharacterInstances.Any(x => x.CharacterId == characterId) &&
		       !FMDB.Context.CharacterBodies.Any(x => x.CharacterId == characterId) &&
		       !FMDB.Context.CharacterBodySources.Any(x => x.CharacterId == characterId);
	}

	private static long PersistedDeadActorBody(SpellOwnedEntity actor)
	{
		var state = actor.Kind == SpellOwnedEntityKind.AutonomousCharacter
			? FMDB.Context.Characters.Where(x => x.Id == actor.Id).Select(x => new { x.BodyId, x.State }).SingleOrDefault()
			: FMDB.Context.CharacterInstances.Where(x => x.Id == actor.Id).Select(x => new { BodyId = (long?)x.BodyId, x.State }).SingleOrDefault();
		if (state is null || !((CharacterState)state.State).HasFlag(CharacterState.Dead))
		{
			throw new InvalidOperationException("Native death must be persisted before observation; absence alone is not death proof.");
		}
		return state.BodyId ?? throw new InvalidOperationException("Archived identities have no native physical body.");
	}

	private static bool IsRemainsForBody(string definition, long bodyId)
	{
		try
		{
			var xml = XElement.Parse(definition);
			return (long?)xml.Element("OriginalBody") == bodyId || (long?)xml.Element("OriginalBodyId") == bodyId;
		}
		catch (Exception ex) when (ex is System.Xml.XmlException or FormatException or OverflowException)
		{
			return false;
		}
	}

	private static DateTime Utc(DateTime value) => DateTime.SpecifyKind(value, DateTimeKind.Utc);
	internal static SpellOwnedLifecycle Read(MagicSpellLifecycle row)
	{
		var origin = new SpellLifecycleOrigin(row.Id, row.SpellId, row.Grade, row.CreatorId, row.Family,
			(SpellLifecycleMode)row.Mode, Utc(row.CreatedUtc), row.DeadlineUtc is { } deadline ? Utc(deadline) : null, row.Provenance);
		origin.Validate();
		var claims = Array.AsReadOnly(row.Entities.Select(x => new SpellOwnedEntity((SpellOwnedEntityKind)x.Kind,
			x.EntityId, (SpellOwnedEntityRole)x.Role)).OrderBy(x => x.Kind).ThenBy(x => x.Id).ToArray());
		if (!Enum.IsDefined((SpellLifecycleState)row.State) || row.Reason is { } reason && !Enum.IsDefined((SpellRetirementReason)reason) ||
		    row.Version <= 0 || claims.Count is 0 or > 256 || Utc(row.UpdatedUtc) < origin.CreatedUtc ||
		    row.DeathObservedUtc is { } observed && Utc(observed) < origin.CreatedUtc || row.RemainsItemId is <= 0 ||
		    row.RemainsItemId is not null && row.DeathObservedUtc is null ||
		    (SpellLifecycleState)row.State == SpellLifecycleState.RemainsPending && row.DeathObservedUtc is null ||
		    (SpellLifecycleState)row.State == SpellLifecycleState.Retiring && row.Reason is null ||
		    (SpellLifecycleState)row.State == SpellLifecycleState.Completed && origin.Mode == SpellLifecycleMode.DeathOnExpiry && row.DeathObservedUtc is null ||
		    claims.Count(x => x.Kind is SpellOwnedEntityKind.AutonomousCharacter or SpellOwnedEntityKind.CharacterInstance) > 1 ||
		    claims.Any(x => !Enum.IsDefined(x.Kind) || !Enum.IsDefined(x.Role) || x.Id <= 0 ||
			    x.Role == SpellOwnedEntityRole.GeneratedPossession && x.Kind != SpellOwnedEntityKind.GameItem))
			throw new InvalidOperationException($"Invalid lifecycle journal {row.Id}; retirement is blocked.");
		return new(origin, claims, (SpellLifecycleState)row.State, (SpellRetirementReason?)row.Reason,
			row.DeathObservedUtc is { } death ? Utc(death) : null, row.RemainsItemId, Utc(row.UpdatedUtc), row.Version, row.Diagnostic);
	}
}
