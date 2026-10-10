#nullable enable

using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp.Magic.Lifecycle;

public sealed partial class SpellOwnedCreation
{
	private Models.CharacterInstance? _borrowedCorpseInstance;
	private readonly Dictionary<object, (EntityState State, Dictionary<string, object?> Values)> _borrowedChanges = new(ReferenceEqualityComparer.Instance);

	/// <summary>Only this adapter may suspend a dead body holder and remove the exact source room link.</summary>
	internal void ClaimBorrowedCorpseAnimation(Models.CharacterInstance instance, long corpseId, long cellId)
	{
		if (_borrowedCorpseInstance is not null || instance.IsPrimary ||
			instance.InstanceKind != (int)CharacterInstanceKind.AnimatedCorpse ||
			instance.ControlPolicy != (int)CharacterInstanceControlPolicy.ScriptOnly ||
			instance.PersistencePolicy != (int)CharacterInstancePersistencePolicy.DespawnOnReboot ||
			instance.LocationId != cellId || Context.Entry(instance).State != EntityState.Added ||
			!CharacterInstanceMetadata.TryGetAnimatedCorpseMetadata(instance.EffectData, out var metadata) ||
			metadata.CorpseItemId != corpseId || metadata.OriginalCharacterId != instance.CharacterId ||
			metadata.OriginalBodyId != instance.BodyId || metadata.AnchorCharacterId != _creatorId)
			throw new InvalidOperationException("Corpse borrowing requires the exact new native secondary instance.");
		var error = PersistedCorpseError(Context, corpseId, instance.CharacterId, instance.BodyId, cellId);
		if (error is not null) throw new InvalidOperationException(error);
		foreach (var existing in Context.CharacterInstances.Where(x => x.BodyId == instance.BodyId && x.IsEmbodied))
		{
			existing.IsEmbodied = false;
			existing.IsControllable = false;
			CaptureBorrowedChange(existing);
		}
		var link = Context.RoomsGameItems.Single(x => x.GameItemId == corpseId && x.RoomId == cellId);
		Context.RoomsGameItems.Remove(link);
		CaptureBorrowedChange(link);
		_borrowedCorpseInstance = instance;
		Claim(SpellOwnedEntityKind.CharacterInstance, instance);
		CaptureBorrowedChange(instance);
	}

	internal static string? PersistedCorpseError(FuturemudDatabaseContext context, long corpseId, long ownerId, long bodyId, long cellId)
	{
		if (BorrowedBodyOwnershipError(context, ownerId, bodyId) is { } ownershipError) return ownershipError;
		if (SpellOwnedCorpseAnimationService.HasUnfinishedBorrow(context, corpseId))
			return "That saved corpse is already borrowed by an unfinished animation.";
		var corpse = context.GameItems.AsNoTracking().SingleOrDefault(x => x.Id == corpseId);
		if (corpse is null || corpse.ContainerId is not null || corpse.RoutePosition is not null ||
			context.BodiesGameItems.Any(x => x.GameItemId == corpseId) ||
			context.RoomsGameItems.Count(x => x.GameItemId == corpseId) != 1 ||
			!context.RoomsGameItems.Any(x => x.GameItemId == corpseId && x.RoomId == cellId))
			return "The saved corpse must be directly in its current room, outside a route or inventory.";
		if (!context.Characters.Any(x => x.Id == ownerId && !x.IsArchived) || !context.Bodies.Any(x => x.Id == bodyId) ||
			!PhysicalReferenceGuard.RemainsComponents(context).Where(x => x.ItemId == corpseId && x.Type == "Corpse").Select(x => x.Definition)
				.AsEnumerable().Any(x => IsExactCorpseDefinition(x, ownerId, bodyId)))
			return "The saved corpse must retain its exact original character and body.";
		if (!((CharacterState)context.Characters.Where(x => x.Id == ownerId).Select(x => x.State).Single()).IsDead())
			return "The saved canonical corpse owner must remain dead.";
		if (context.CharacterInstances.AsNoTracking().Where(x => x.BodyId == bodyId && x.IsEmbodied)
			.AsEnumerable().Any(x => !((CharacterState)x.State).IsDead() && !((CharacterState)x.State).HasFlag(CharacterState.Stasis)))
			return "That corpse body already has a live embodied instance.";
		return null;
	}

	internal static string? BorrowedBodyOwnershipError(FuturemudDatabaseContext context, long ownerId, long bodyId)
	{
		var owner = context.Characters.AsNoTracking().SingleOrDefault(x => x.Id == ownerId && !x.IsArchived && x.BodyId == bodyId);
		if (owner is null || !((CharacterState)owner.State).IsDead() ||
			context.Characters.Any(x => x.BodyId == bodyId && x.Id != ownerId) ||
			context.CharacterInstances.Any(x => x.BodyId == bodyId && x.CharacterId != ownerId) ||
			context.CharacterBodies.Any(x => x.BodyId == bodyId && x.CharacterId != ownerId) ||
			context.CharacterBodySources.Any(x => x.BodyId == bodyId && x.CharacterId != ownerId) ||
			context.CharacterBodyRetirements.Any(x => x.BodyId == bodyId && x.CharacterId != ownerId))
			return "The borrowed final corpse body must belong exclusively to its dead original canonical character.";
		return null;
	}

	internal static bool IsExactCorpseDefinition(string definition, long ownerId, long bodyId)
	{
		try
		{
			var root = XElement.Parse(definition);
			return (long?)root.Element("OriginalCharacter") == ownerId && (long?)root.Element("OriginalBody") == bodyId;
		}
		catch (Exception ex) when (ex is System.Xml.XmlException or FormatException or OverflowException) { return false; }
	}

	private void CaptureBorrowedChange(object row)
	{
		Context.ChangeTracker.DetectChanges();
		var entry = Context.Entry(row);
		_borrowedChanges.Add(row, (entry.State, entry.Properties.ToDictionary(x => x.Metadata.Name, x => x.CurrentValue)));
	}

	private bool IsExactBorrowedChange(EntityEntry entry) => _borrowedChanges.TryGetValue(entry.Entity, out var allowed) &&
		entry.State == allowed.State && entry.Properties.All(x => Equals(x.CurrentValue, allowed.Values[x.Metadata.Name]));
}
