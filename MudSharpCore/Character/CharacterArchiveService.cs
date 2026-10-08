#nullable enable

using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MudSharp.Accounts;
using MudSharp.Character.Name;
using MudSharp.Database;
using MudSharp.Magic;
using MudSharp.Magic.Lifecycle;
using Db = MudSharp.Models;

namespace MudSharp.Character;

/// <summary>Conservative, transactional compaction of a proven dead spell-created NPC.</summary>
public sealed class CharacterArchiveService : ICharacterArchiveService
{
	private const int RowLimit = 100000;
	private static readonly MethodInfo SetMethod = typeof(DbContext).GetMethods()
		.Single(x => x.Name == nameof(DbContext.Set) && x.IsGenericMethod && x.GetParameters().Length == 0);
	private static readonly HashSet<string> CharacterStateRows = new(StringComparer.Ordinal)
	{
		"CharacterBody", "CharacterBodySource", "CharacterTrait", "CharacterKnowledge", "CharacterAccent",
		"CharactersChargenRoles", "CharactersLanguages", "CharactersMagicResources", "CharactersScripts",
		"CharactersSignedLanguage", "CharacterSignedLanguageVariety", "PerceiverMerit", "HooksPerceivable",
		"Dub", "DreamsAlreadyDreamt", "DreamsCharacters", "CharacterBodyRetirement"
	};
	private static readonly HashSet<string> BodyStateRows = new(StringComparer.Ordinal)
	{
		"BodyDrugDose", "BodyDrugExposure", "BodiesSeveredParts", "Characteristic", "Trait",
		"HooksPerceivable", "PerceiverMerit", "CharacterBody", "CharacterBodySource", "CharacterBodyRetirement",
		"Wound"
	};

	public ArchivedCharacterIdentity? Find(long characterId)
	{
		if (characterId <= 0) return null;
		using var isolated = FMDB.BeginIndependentScope();
		using var db = new FMDB();
		var row = (from archive in FMDB.Context.CharacterArchives.AsNoTracking()
			join identity in FMDB.Context.Characters.AsNoTracking() on archive.CharacterId equals identity.Id
			where archive.CharacterId == characterId
			select new { Archive = archive, identity.NameInfo }).SingleOrDefault();
		return row is null ? null : Read(row.Archive) with { NameInfo = row.NameInfo };
	}

	public bool TryArchiveNpc(Guid lifecycleId, long expectedVersion, ICharacter character, DateTime nowUtc,
		out string diagnostic)
	{
		var result = ArchiveNpc(lifecycleId, expectedVersion, character, nowUtc);
		diagnostic = result.Diagnostic;
		return result.Success;
	}

	private static (bool Success, string Diagnostic) ArchiveNpc(Guid lifecycleId, long expectedVersion,
		ICharacter character, DateTime nowUtc)
	{
		string diagnostic;
		if (FMDB.WritesAreSuppressed || character is not MudSharp.NPC.NPC runtime || character.IsPlayerCharacter ||
		    character.Id <= 0 || character.Body?.Id is not > 0 || nowUtc.Kind != DateTimeKind.Utc)
		{
			diagnostic = "Compaction requires a writable scope and a native NPC identity with an exact persisted body.";
			return (false, diagnostic);
		}
		using (var isolated = FMDB.BeginIndependentScope(requireWrites: true))
		using (var db = new FMDB())
		using (var transaction = FMDB.Context.Database.BeginTransaction(IsolationLevel.Serializable))
		{
			var context = FMDB.Context;
			var row = context.MagicSpellLifecycles.Include(x => x.Entities).SingleOrDefault(x => x.Id == lifecycleId);
			if (row is null || row.Version != expectedVersion)
			{
				diagnostic = "The lifecycle is missing or changed; reload before compaction.";
				return (false, diagnostic);
			}
			var lifecycle = SpellOwnedLifecycleStore.Read(row);
			SpellLifecycleTransitions.ValidateUtc(nowUtc, lifecycle);
			var archive = context.CharacterArchives.SingleOrDefault(x => x.CharacterId == character.Id);
			var bodyId = character.Body.Id;
			var claims = lifecycle.Entities;
			if (!lifecycle.MayRemoveOwnedEntities ||
			    claims.Count(x => x.Kind == SpellOwnedEntityKind.AutonomousCharacter) != 1 ||
			    claims.Count(x => x.Kind == SpellOwnedEntityKind.AutonomousCharacter && x.Id == character.Id) != 1 ||
			    claims.Count(x => x.Kind == SpellOwnedEntityKind.Body && x.Id == bodyId) != 1 ||
			    claims.Any(x => x.Kind is SpellOwnedEntityKind.CharacterInstance or SpellOwnedEntityKind.Room or SpellOwnedEntityKind.Exit ||
				    x.Kind == SpellOwnedEntityKind.Body && x.Id != bodyId) ||
			    lifecycle.State is not (SpellLifecycleState.Retiring or SpellLifecycleState.RemainsPending or SpellLifecycleState.Completed))
			{
				return (false, "Only an exact retiring autonomous NPC and its creation-proven body can be compacted.");
			}
			if (archive is not null)
			{
				if (archive.LifecycleId != lifecycleId || archive.OriginalBodyId != bodyId ||
				    !context.Characters.Any(x => x.Id == character.Id && x.IsArchived && x.BodyId == null) ||
				    context.Bodies.Any(x => x.Id == bodyId) || context.Npcs.Any(x => x.CharacterId == character.Id) ||
				    context.CharacterInstances.Any(x => x.CharacterId == character.Id) ||
				    context.CharacterBodies.Any(x => x.CharacterId == character.Id) ||
				    context.CharacterBodySources.Any(x => x.CharacterId == character.Id) || lifecycle.DeathObservedUtc is null)
				{
					return Hold("The existing archive does not match this lifecycle's compacted graph.");
				}
				transaction.Commit();
			}
			else
			{
				if (lifecycle.State == SpellLifecycleState.Completed)
					return Hold("Completed ownership has no matching archive; do not infer compaction authority.");
				var identity = context.Characters.SingleOrDefault(x => x.Id == character.Id);
				var body = context.Bodies.SingleOrDefault(x => x.Id == bodyId);
				if (identity is null || body is null || identity.IsArchived || identity.AccountId is not null ||
				    identity.IsAdminAvatar || identity.BodyId != bodyId || identity.DeathTime is null ||
				    !((CharacterState)identity.State).HasFlag(CharacterState.Dead) ||
				    !character.State.HasFlag(CharacterState.Dead) || lifecycle.DeathObservedUtc is null ||
				    !context.Npcs.Any(x => x.CharacterId == character.Id))
				{
					return Hold("Persisted native death and an accountless NPC graph must be correlated before compaction.");
				}
				if (HasRuntimeDependants(character))
					return Hold("A runtime group, guard, movement, mount, combat or position relationship retains the NPC.");
				if (context.Crimes.Any(x => x.CriminalId == character.Id && !x.IsFinalised))
					return Hold("Unresolved enforcement still requires a physical criminal; preserve the NPC graph.");
				if (character.Body.AllItems.Any() || character.Bodies.Any(x => x.Id != bodyId) ||
				    character.Effects.Any() || character.Body.Effects.Any() ||
				    character.Gameworld.Characters.Any(x => x.Id != character.Id && x.Body?.Id == bodyId) ||
				    !NpcArchiveReferencePolicy.IsEmptyEffects(identity.EffectData) ||
				    !NpcArchiveReferencePolicy.IsEmptyEffects(body.EffectData) ||
				    claims.Any(x => x.Kind == SpellOwnedEntityKind.GameItem && context.GameItems.Any(y => y.Id == x.Id)) ||
				    lifecycle.RemainsItemId is { } remains && context.GameItems.Any(x => x.Id == remains))
				{
					return Hold("Physical possessions, effects, other forms, live bodies or dependent owned entities remain.");
				}
				if (context.CharacterInstances.Any(x => x.CharacterId == character.Id &&
					    (!x.IsPrimary || x.BodyId != bodyId || !((CharacterState)x.State).HasFlag(CharacterState.Dead))) ||
				    context.CharacterBodies.Any(x => x.CharacterId == character.Id && x.BodyId != bodyId) ||
				    context.CharacterBodySources.Any(x => x.CharacterId == character.Id && x.BodyId != bodyId) ||
				    context.GameItems.Any(x => x.OwnerId == bodyId && x.OwnerType == "Body" ||
					    x.OwnerId == character.Id && x.OwnerType == "Character" ||
					    x.PositionTargetId == bodyId && x.PositionTargetType == "Body" ||
					    x.PositionTargetId == character.Id && x.PositionTargetType == "Character") ||
				    context.Wounds.Any(x => x.BodyId == bodyId && x.LodgedItemId != null))
				{
					return Hold("A live instance, borrowed form, polymorphic item reference or lodged possession remains.");
				}
				var wounds = context.Wounds.AsNoTracking().Where(x => x.BodyId == bodyId).Take(257).ToArray();
				var woundHistory = JsonSerializer.Serialize(wounds.Select(x => new
				{
					x.Id, x.BodypartProtoId, x.DamageType, x.OriginalDamage, x.CurrentDamage, x.CurrentPain,
					x.CurrentShock, x.CurrentStun, x.Internal, x.ActorOriginId, x.ToolOriginId, x.RealTimeOfWound,
					x.WoundType, x.ExtraInformation
				}));
				var displayName = character.PersonalName.GetName(NameStyle.FullWithNickname);
				var shortDescription = body.ShortDescription ?? string.Empty;
				var fullDescription = body.FullDescription ?? string.Empty;
				if (wounds.Length > 256 || woundHistory.Length > 65535 || displayName.Length > 1024 ||
				    shortDescription.Length > 4096 || fullDescription.Length > 65535 ||
				    (identity.NameInfo?.Length ?? 0) > 65535 || (identity.IntroductionMessage?.Length ?? 0) > 4096 ||
				    (identity.CustomAlertEmote?.Length ?? 0) > 4096 || (identity.CustomDistantAlertEmote?.Length ?? 0) > 4096 ||
				    (identity.LongTermPlan?.Length ?? 0) > 4096 || (identity.ShortTermPlan?.Length ?? 0) > 4096)
				{
					return Hold("Historical attribution exceeds this bounded archive format; retain the graph for explicit handling.");
				}
				var compactRows = new HashSet<object>(ReferenceEqualityComparer.Instance);
				foreach (var principal in new[] { (typeof(Db.Character), character.Id), (typeof(Db.Body), bodyId) })
				{
					foreach (var foreignKey in context.Model.FindEntityType(principal.Item1)!.GetReferencingForeignKeys())
					{
						if (foreignKey.Properties.Count != 1 || foreignKey.PrincipalKey.Properties.Single().Name != "Id")
							return Hold("An unclassified composite/alternate identity relation requires explicit archival handling.");
						var matches = Rows(context, foreignKey.DeclaringEntityType, foreignKey.Properties.Single(), principal.Item2);
						if (matches.Count == 0) continue;
						if (matches.Count > RowLimit) return Hold("A dependent graph exceeds the bounded compaction batch.");
						var type = foreignKey.DeclaringEntityType.ClrType.Name;
						var property = foreignKey.Properties.Single().Name;
						if (principal.Item1 == typeof(Db.Body) && matches.Any(x =>
						        x is Db.CharacterBody form && form.CharacterId != character.Id ||
						        x is Db.CharacterBodySource source && source.CharacterId != character.Id ||
						        x is Db.CharacterBodyRetirement retirement && retirement.CharacterId != character.Id))
							return Hold($"Foreign ownership in {type}.{property} retains the claimed body.");
						if (principal.Item1 == typeof(Db.Body) && type == "Character" && property == "BodyId" &&
						    matches.All(x => ((Db.Character)x).Id == character.Id)) continue;
						if (principal.Item1 == typeof(Db.Character) &&
						    (type is "Crime" or "CharacterLog" ||
						     type == "Wound" && property == "ActorOriginId" ||
						     type == "Writing" && property is "AuthorId" or "TrueAuthorId" ||
						     type == "Drawing" && property == "AuthorId")) continue;
						var removable = principal.Item1 == typeof(Db.Character)
							? CharacterStateRows.Contains(type) && property == "CharacterId" || type == "Npc" && property == "CharacterId" ||
							  type == "CharacterInstance" && property == "CharacterId"
							: BodyStateRows.Contains(type) && property == "BodyId" || type == "CharacterInstance" && property == "BodyId" &&
							  matches.All(x => ((Db.CharacterInstance)x).CharacterId == character.Id);
						if (!removable) return Hold($"Retained or unclassified relation {type}.{property} requires explicit handling.");
						foreach (var match in matches) compactRows.Add(match);
					}
				}
				var npcRows = compactRows.OfType<Db.Npc>().ToArray();
				foreach (var npcRow in npcRows)
				{
					foreach (var foreignKey in context.Model.FindEntityType(typeof(Db.Npc))!.GetReferencingForeignKeys())
					{
						if (foreignKey.Properties.Count != 1 || foreignKey.DeclaringEntityType.ClrType != typeof(Db.NpcsArtificialIntelligences))
							return Hold("An unclassified NPC relationship prevents release of its AI graph.");
						var aiRows = Rows(context, foreignKey.DeclaringEntityType, foreignKey.Properties.Single(), npcRow.Id);
						if (aiRows.Count > RowLimit) return Hold("The NPC AI assignment graph exceeds the bounded compaction batch.");
						foreach (var match in aiRows)
							compactRows.Add(match);
					}
				}
				var instanceIds = compactRows.OfType<Db.CharacterInstance>().Select(x => x.Id).ToArray();
				if (context.CharacterInstances.Any(x => x.AnchorInstanceId != null && instanceIds.Contains(x.AnchorInstanceId.Value)))
					return Hold("A dependent anchored instance prevents identity compaction.");
				if (context.GameItems.Any(x => x.OwnerType == "CharacterInstance" && instanceIds.Contains(x.OwnerId ?? 0) ||
					    x.PositionTargetType == "CharacterInstance" && instanceIds.Contains(x.PositionTargetId ?? 0)))
					return Hold("A polymorphic item reference retains a physical character instance.");
				if (wounds.Any(x => context.Infections.Any(y => y.WoundId == x.Id)))
					return Hold("An infection graph requires explicit historical handling before compaction.");
				if (!SerializedReferencesAreClear(context, character.Id, bodyId,
					    instanceIds.Concat(wounds.Select(x => x.Id)).ToArray(), out var referenceDiagnostic))
					return Hold(referenceDiagnostic);
				if (!UnmappedReferencesAreClear(context, character.Id, bodyId,
					    instanceIds.Concat(wounds.Select(x => x.Id)).ToArray(), compactRows, out referenceDiagnostic))
					return Hold(referenceDiagnostic);

				context.CharacterArchives.Add(new Db.CharacterArchive
				{
					CharacterId = character.Id, OriginalBodyId = bodyId, LifecycleId = lifecycleId,
					ArchivedUtc = nowUtc, DisplayName = displayName, ShortDescription = shortDescription,
					FullDescription = fullDescription, WoundHistory = woundHistory,
					Provenance = lifecycle.Origin.Provenance
				});
				identity.IsArchived = true;
				identity.BodyId = null;
				identity.Body = null;
				identity.Status = (int)CharacterStatus.Retired;
				identity.NeedsModel = "NoNeeds";
				identity.Outfits = null;
				identity.PositionTargetId = null;
				identity.PositionTargetType = string.Empty;
				identity.PositionEmote = string.Empty;
				identity.RoutePosition = null;
				context.RemoveRange(compactRows);
				context.Bodies.Remove(body);
				context.SaveChanges();
				transaction.Commit();
			}

			(bool Success, string Diagnostic) Hold(string reason)
			{
				if (lifecycle.State != SpellLifecycleState.Completed && row.Diagnostic != reason)
				{
					row.Diagnostic = reason;
					row.Version = checked(row.Version + 1);
					row.UpdatedUtc = nowUtc;
					context.SaveChanges();
				}
				transaction.Commit();
				return (false, reason);
			}
		}
		runtime.ReleaseArchivedRuntime();
		return (true, string.Empty);
	}

	private static List<object> Rows(FuturemudDatabaseContext context, IEntityType type, IProperty property, long id)
	{
		var query = (IQueryable)SetMethod.MakeGenericMethod(type.ClrType).Invoke(context, null)!;
		var parameter = Expression.Parameter(type.ClrType, "row");
		var value = Expression.Call(typeof(EF), nameof(EF.Property), [property.ClrType], parameter, Expression.Constant(property.Name));
		var predicate = Expression.Lambda(Expression.Equal(value, Expression.Convert(Expression.Constant(id), value.Type)), parameter);
		var where = Expression.Call(typeof(Queryable), nameof(Queryable.Where), [type.ClrType], query.Expression, Expression.Quote(predicate));
		var take = Expression.Call(typeof(Queryable), nameof(Queryable.Take), [type.ClrType], where, Expression.Constant(RowLimit + 1));
		return query.Provider.CreateQuery(take).Cast<object>().ToList();
	}

	private static bool SerializedReferencesAreClear(FuturemudDatabaseContext context, long characterId, long bodyId,
		long[] additionalPhysicalIds, out string diagnostic)
	{
		foreach (var type in context.Model.GetEntityTypes())
		{
			foreach (var property in type.GetProperties().Where(x => x.ClrType == typeof(string) &&
				         (x.Name.Contains("Definition", StringComparison.Ordinal) || x.Name is "EffectData" or "Data" or "Value" or
					         "StateData" or "StateJson" or "ResultJson" or "WaitArgument" or "Tattoos" or
					         "ExtraInformation" or "ProcedureParameters" or "OperationalPayload" or "CommandArguments" or
					         "StrategyData" or "LandDetailJson")))
			{
				// Writing content is historical narrative, not a serialized runtime actor or physical reference.
				if (type.ClrType == typeof(Db.Writing) && property.Name == "Definition") continue;
				var query = (IQueryable)SetMethod.MakeGenericMethod(type.ClrType).Invoke(context, null)!;
				var parameter = Expression.Parameter(type.ClrType, "row");
				var select = Expression.Call(typeof(Queryable), nameof(Queryable.Select), [type.ClrType, typeof(string)],
					query.Expression, Expression.Quote(Expression.Lambda(Expression.Property(parameter, property.Name), parameter)));
				var values = query.Provider.CreateQuery<string?>(select).Take(RowLimit + 1).ToArray();
				if (values.Length > RowLimit || values.Any(x => x?.Length > 1048576 ||
				    NpcArchiveReferencePolicy.HasReferenceOrUncertainty(x, characterId, bodyId, additionalPhysicalIds)))
				{
					diagnostic = $"Unresolved serialized reference or scan limit in {type.ClrType.Name}.{property.Name}; retain the physical graph.";
					return false;
				}
			}
		}
		diagnostic = string.Empty;
		return true;
	}

	internal static bool HasRuntimeDependants(ICharacter character)
	{
		var world = character.Gameworld;
		return character.Combat is not null || character.Movement is not null || character.Party is not null ||
		       character.CharacterController is not null and not MudSharp.NPC.NPCController ||
		       character is Character actor && actor.Instances.Any(x => !ReferenceEquals(x, character)) ||
		       character.Following is not null || character.RidingMount is not null || character.Riders.Any() ||
		       character.PositionTarget is not null || character.TargetedBy.Any() || character.SeenTargets.Any() ||
		       world.Connections.Any(x => x.ControlPuppet?.Actor?.Id == character.Id) ||
		       world.Actors.Concat(world.CachedActors).Concat(world.Characters).Concat(world.NPCs)
			       .Any(x => x.Id == character.Id && !ReferenceEquals(x, character)) ||
		       world.Bodies.Any(x => x.Id == character.Body.Id && !ReferenceEquals(x, character.Body)) ||
		       world.GroupAIs.Any(x => x.GroupMembers.Any(y => y.Id == character.Id) ||
			       x.GroupRoles.Keys.Any(y => y.Id == character.Id)) ||
		       world.CachedBodyguards.Any(x => x.Key == character.Id && x.Value.Count != 0 ||
			       x.Value.Any(y => y.Id == character.Id)) ||
		       world.Actors.Concat(world.CachedActors).Any(x => x.Id != character.Id &&
			       (x.Following?.Id == character.Id || x.RidingMount?.Id == character.Id ||
			        x.SeenTargets.Any(y => y.Id == character.Id && y.FrameworkItemType == character.FrameworkItemType) ||
			        x.CombatTarget is ICharacter target && target.Id == character.Id ||
			        x.PositionTarget is ICharacter position && position.Id == character.Id));
	}

	private static bool UnmappedReferencesAreClear(FuturemudDatabaseContext context, long characterId, long bodyId,
		long[] additionalPhysicalIds, HashSet<object> compactRows, out string diagnostic)
	{
		foreach (var type in context.Model.GetEntityTypes())
		{
			// These are immutable canonical attribution/ownership receipts, never physical actor loaders.
			if (type.ClrType == typeof(Db.CharacterArchive) || type.ClrType == typeof(Db.MagicSpellLifecycle) ||
			    type.ClrType == typeof(Db.MagicSpellOwnedEntity)) continue;
			foreach (var property in type.GetProperties().Where(x =>
				         (x.ClrType == typeof(long) || x.ClrType == typeof(long?)) &&
				         x.Name.EndsWith("Id", StringComparison.Ordinal) && !x.GetContainingKeys().Any() &&
				         !x.GetContainingForeignKeys().Any()))
			{
				var matches = new[] { characterId, bodyId }.Concat(additionalPhysicalIds).Where(x => x > 0)
					.SelectMany(id => Rows(context, type, property, id)).ToArray();
				if (matches.Length == 0) continue;
				// These uniqueness keys derive from the already-audited primary instance, not an independent actor reference.
				if (type.ClrType == typeof(Db.CharacterInstance) && property.Name is "EmbodiedBodyId" or "PrimaryCharacterId" &&
				    property.GetComputedColumnSql() is not null && matches.All(compactRows.Contains)) continue;
				diagnostic = $"Unclassified scalar reference in {type.ClrType.Name}.{property.Name}; retain the physical graph.";
				return false;
			}
		}
		diagnostic = string.Empty;
		return true;
	}

	private static ArchivedCharacterIdentity Read(Db.CharacterArchive archive) =>
		new(archive.CharacterId, archive.OriginalBodyId, archive.LifecycleId,
			DateTime.SpecifyKind(archive.ArchivedUtc, DateTimeKind.Utc), archive.DisplayName,
			archive.ShortDescription, archive.FullDescription);
}
