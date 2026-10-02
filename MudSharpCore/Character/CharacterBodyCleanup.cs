#nullable enable

using MudSharp.Body;
using MudSharp.Database;
using MudSharp.GameItems;
using System.Data;
using Microsoft.EntityFrameworkCore;
using MudSharp.Magic;
using MudSharp.Effects;

namespace MudSharp.Character;

public partial class Character
{
	private bool HasPhysicalReferenceToRetiredBody(long bodyId, IGameItem? excludingReference)
	{
		return Gameworld.Items
		                .Where(x => !ReferenceEquals(x, excludingReference))
		                .Where(x => !x.Deleted)
		                .Select(x => x.GetItemType<IButcherable>())
		                .Where(x => x is not null)
		                .Any(x => x!.OriginalBodyId == bodyId);
	}

	private bool HasLiveRuntimeReferenceToRetiredBody(IBody body)
	{
		if (body.Id == 0 || CurrentBody.Id == body.Id)
		{
			return true;
		}

		if (_forms.Any(x => x.Body.Id == body.Id) ||
		    _formSources.Any(x => x.Body.Id == body.Id))
		{
			return true;
		}

		return Gameworld.Characters
			.Any(x => x.Identity.Instances.Any(instance => instance.Body.Id == body.Id) ||
				x.EffectsOfType<IBodyBackupEffect>().Any(effect => effect.BackupBodyId == body.Id) ||
				HasSavedRuntimeBodyReference(x.Effects, body.Id) || HasSavedRuntimeBodyReference(x.Body.Effects, body.Id)) ||
			Gameworld.Items.Where(x => !x.Deleted)
				.Any(x => HasSavedRuntimeBodyReference(x.Effects, body.Id));
	}

	private static bool HasSavedRuntimeBodyReference(IEnumerable<IEffect> effects, long bodyId) =>
		effects.Where(x => x.SavingEffect).Any(x => RetiredBodyReferencePolicy.HasReferenceOrUncertainty(
			x.SaveToXml(new Dictionary<IEffect, TimeSpan>()).ToString(), bodyId));

	private bool DeleteRetiredBodyDatabaseState(IBody body, long? excludingItemId)
	{
		using var isolated = FMDB.BeginIndependentScope(requireWrites: true);
		using (new FMDB())
		using (var transaction = FMDB.Context.Database.BeginTransaction(IsolationLevel.Serializable))
		{
			var dbCharacter = FMDB.Context.Characters.Find(Id);
			var dbBody = FMDB.Context.Bodies.Find(body.Id);
			if (dbCharacter is null || dbBody is null || CurrentBody.Id <= 0 ||
			    !FMDB.Context.Bodies.Any(x => x.Id == CurrentBody.Id))
			{
				return false;
			}

			if (CurrentBody.Id == body.Id ||
			    FMDB.Context.Characters.Any(x => x.Id != Id && x.BodyId == body.Id) ||
			    FMDB.Context.CharacterInstances.Any(x => x.BodyId == body.Id) ||
			    FMDB.Context.BodiesGameItems.Any(x => x.BodyId == body.Id) ||
			    FMDB.Context.BodiesImplants.Any(x => x.BodyId == body.Id) ||
			    FMDB.Context.BodiesProsthetics.Any(x => x.BodyId == body.Id) ||
			    FMDB.Context.GameItems.Any(x => x.OwnerType == "Body" && x.OwnerId == body.Id ||
				    x.PositionTargetType == "Body" && x.PositionTargetId == body.Id) ||
			    FMDB.Context.Wounds.Any(x => x.BodyId == body.Id && x.LodgedItemId != null))
			{
				return false;
			}

			var formRows = FMDB.Context.CharacterBodies
			                      .Where(x => x.BodyId == body.Id)
			                      .ToList();
			if (formRows.Any(x => x.CharacterId != Id))
			{
				return false;
			}

			var sourceRows = FMDB.Context.CharacterBodySources
			                        .Where(x => x.BodyId == body.Id)
			                        .ToList();
			if (sourceRows.Any(x => x.CharacterId != Id))
			{
				return false;
			}

			// Existing forms, durable ordinary retirement or a creation-proven retiring lifecycle must account for the body.
			// Body.Actor alone is insufficient: secondary retirement can reassign a borrowed body to its owner.
			if (formRows.Count == 0 && sourceRows.Count == 0 &&
			    !FMDB.Context.CharacterBodyRetirements.Any(x => x.BodyId == body.Id && x.CharacterId == Id) &&
			    !FMDB.Context.MagicSpellOwnedEntities.Any(x => x.Kind == (int)SpellOwnedEntityKind.Body &&
				    x.EntityId == body.Id && x.Lifecycle.CreatorId == Id &&
				    x.Lifecycle.Mode != (int)SpellLifecycleMode.Permanent &&
				    (x.Lifecycle.State == (int)SpellLifecycleState.Retiring ||
				     x.Lifecycle.State == (int)SpellLifecycleState.RemainsPending)))
			{
				return false;
			}

			if (HasPersistedReferenceToRetiredBody(body.Id, excludingItemId)) return false;
			if (dbCharacter.BodyId == body.Id) dbCharacter.BodyId = CurrentBody.Id;

			FMDB.Context.CharacterBodies.RemoveRange(formRows);
			FMDB.Context.CharacterBodySources.RemoveRange(sourceRows);
			FMDB.Context.CharacterBodyRetirements.RemoveRange(FMDB.Context.CharacterBodyRetirements.Where(x => x.BodyId == body.Id));

			FMDB.Context.Bodies.Remove(dbBody);
			FMDB.Context.SaveChanges();
			transaction.Commit();
			return true;
		}
	}

	private bool HasPersistedReferenceToRetiredBody(long bodyId, long? excludingItemId)
	{
		return FMDB.Context.GameItemComponents.AsNoTracking()
			.Where(x => excludingItemId == null || x.GameItemId != excludingItemId)
			.Select(x => x.Definition).AsEnumerable()
			.Any(x => RetiredBodyReferencePolicy.HasReferenceOrUncertainty(x, bodyId)) ||
			FMDB.Context.Characters.AsNoTracking().Select(x => x.EffectData).AsEnumerable()
				.Any(x => RetiredBodyReferencePolicy.HasReferenceOrUncertainty(x, bodyId)) ||
			FMDB.Context.CharacterInstances.AsNoTracking().Select(x => x.EffectData).AsEnumerable()
				.Any(x => RetiredBodyReferencePolicy.HasReferenceOrUncertainty(x, bodyId)) ||
			FMDB.Context.Bodies.AsNoTracking().Where(x => x.Id != bodyId).Select(x => x.EffectData).AsEnumerable()
				.Any(x => RetiredBodyReferencePolicy.HasReferenceOrUncertainty(x, bodyId)) ||
			FMDB.Context.GameItems.AsNoTracking().Where(x => excludingItemId == null || x.Id != excludingItemId)
				.Select(x => x.EffectData).AsEnumerable()
				.Any(x => RetiredBodyReferencePolicy.HasReferenceOrUncertainty(x, bodyId));
	}

	public bool TryCleanupRetiredBody(IBody body, IGameItem? excludingReference = null)
	{
		if (FMDB.WritesAreSuppressed || body is null || body.AllItems.Any() ||
		    HasLiveRuntimeReferenceToRetiredBody(body) ||
		    HasPhysicalReferenceToRetiredBody(body.Id, excludingReference))
		{
			return false;
		}

		var excludingItemId = excludingReference?.Id > 0 &&
			excludingReference.GetItemType<IButcherable>()?.OriginalBodyId == body.Id ? excludingReference.Id : (long?)null;
		if (!DeleteRetiredBodyDatabaseState(body, excludingItemId))
		{
			return false;
		}

		Gameworld.SaveManager.Abort(body);
		_pendingBodyRetirements?.Remove(body.Id);
		Gameworld.EffectScheduler.Destroy(body);
		Gameworld.Scheduler.Destroy(body);
		Gameworld.Destroy(body);
		return true;
	}
}
