#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.Magic;
using MudSharp.Magic.Lifecycle;
using MudSharp.NPC;

namespace MudSharp.Commands.Modules;

public partial class ImplementorModule
{
	private static void WithAllPCsLoaded(IFuturemud gameworld, Action maintenance)
	{
		var loadedPCs = new List<ICharacter>();
		Exception? maintenanceFailure = null;
		try
		{
			EnsureAllPCsAreLoaded(gameworld, loadedPCs);
			maintenance();
		}
		catch (Exception ex)
		{
			maintenanceFailure = ex;
			throw;
		}
		finally
		{
			try
			{
				CleanupAllPCsLoaded(gameworld, loadedPCs);
			}
			catch (Exception cleanupFailure) when (maintenanceFailure is not null)
			{
				throw new AggregateException("Maintenance and temporary PC cleanup both failed.", maintenanceFailure, cleanupFailure);
			}
		}
	}

	private static void CleanupDeadMaintenanceNpcs(IFuturemud gameworld)
	{
		using var isolated = FMDB.BeginIndependentScope();
		using var db = new FMDB();
		var candidates = FMDB.Context.Npcs.AsNoTracking()
			.Where(x => x.Character!.State == (int)CharacterState.Dead && !x.Character.IsArchived)
			.Select(x => new { x.CharacterId, x.Character!.BodyId }).ToArray();
		var corpses = gameworld.Items.Where(x => !x.Deleted).SelectNotNull(x => x!.GetItemType<ICorpse>()).ToArray();
		var parts = gameworld.Items.Where(x => !x.Deleted).SelectNotNull(x => x!.GetItemType<ISeveredBodypart>()).ToArray();
		var archived = 0; var retained = 0; var failed = 0;
		foreach (var candidate in candidates)
		{
			try
			{
				string diagnostic;
				if (corpses.Any(x => x.RepresentsFinalCharacterDeath && x.OriginalCharacter?.Id == candidate.CharacterId) ||
				    parts.Any(x => x.OriginalCharacterId == candidate.CharacterId))
				{
					diagnostic = "Physical remains still reference this canonical identity.";
				}
				else
				{
					var lifecycles = FMDB.Context.MagicSpellLifecycles.AsNoTracking().Include(x => x.Entities)
						.Where(x => x.Entities.Any(y => y.Kind == (int)SpellOwnedEntityKind.AutonomousCharacter &&
							y.EntityId == candidate.CharacterId)).ToArray().Select(SpellOwnedLifecycleStore.Read).ToArray();
					if (TryArchiveMaintenanceNpc(gameworld, candidate.CharacterId, candidate.BodyId, lifecycles,
						    RuntimeClock.UtcNow, out diagnostic))
					{
						archived++;
						continue;
					}
				}
				retained++;
				gameworld.SystemMessage($"Retained dead NPC #{candidate.CharacterId}: {diagnostic}", true);
			}
			catch (Exception ex)
			{
				// A post-commit runtime failure can already have archived the graph. Do not claim retention or success.
				failed++;
				gameworld.SystemMessage($"Archival attempt for dead NPC #{candidate.CharacterId} failed: {ex.Message}. Inspect its lifecycle before retrying.", true);
			}
		}
		gameworld.SystemMessage($"Archived {archived} eligible spell-owned dead NPCs; retained {retained} dead NPCs; failed {failed} archival attempts.", true);
	}

	private static bool TryArchiveMaintenanceNpc(IFuturemud gameworld, long characterId, long? bodyId,
		IReadOnlyList<SpellOwnedLifecycle> lifecycles, DateTime nowUtc, out string diagnostic)
	{
		if (lifecycles.Count != 1)
		{
			diagnostic = "No unique creation-proven spell lifecycle; retain the canonical identity, body and history.";
			return false;
		}
		var lifecycle = lifecycles[0];
		if (bodyId is not > 0 || !lifecycle.MayRemoveOwnedEntities ||
		    lifecycle.State is not (SpellLifecycleState.Retiring or SpellLifecycleState.RemainsPending) ||
		    lifecycle.DeathObservedUtc is null ||
		    lifecycle.Entities.Count(x => x.Kind == SpellOwnedEntityKind.AutonomousCharacter) != 1 ||
		    !lifecycle.Entities.Any(x => x.Kind == SpellOwnedEntityKind.AutonomousCharacter && x.Id == characterId) ||
		    lifecycle.Entities.Count(x => x.Kind == SpellOwnedEntityKind.Body) != 1 ||
		    !lifecycle.Entities.Any(x => x.Kind == SpellOwnedEntityKind.Body && x.Id == bodyId) ||
		    lifecycle.Entities.Any(x => x.Kind is SpellOwnedEntityKind.CharacterInstance or SpellOwnedEntityKind.Room or SpellOwnedEntityKind.Exit))
		{
			diagnostic = "The lifecycle does not prove eligible retiring NPC ownership and correlated death; retain the physical graph.";
			return false;
		}
		if (gameworld.CharacterArchives is not { } archives)
		{
			diagnostic = "The archive service is unavailable; retain the physical graph.";
			return false;
		}
		var npc = gameworld.TryGetCharacter(characterId, true);
		if (npc is not INPC || npc.Id != characterId || npc.Body?.Id != bodyId)
		{
			diagnostic = "The exact owned native NPC and body could not be resolved; retain the physical graph.";
			return false;
		}
		return archives.TryArchiveNpc(lifecycle.Origin.Id, lifecycle.Version, npc, nowUtc, out diagnostic);
	}
}
