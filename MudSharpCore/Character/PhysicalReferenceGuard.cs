#nullable enable

using System.Xml;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MudSharp.Computers;
using MudSharp.Database;
using MudSharp.Effects;
using MudSharp.Framework;
using Db = MudSharp.Models;

namespace MudSharp.Character;

/// <summary>Checks only relational identities and payloads with code-proven physical reference contracts.</summary>
public static class PhysicalReferenceGuard
{
	private const int RowLimit = 100000;
	private const int PayloadLimit = 1048576;
	public sealed record ComponentRow(long Id, long ItemId, string Type, string Definition);
	private sealed record Payload(long Id, string? Value);

	public static IQueryable<ComponentRow> RemainsComponents(FuturemudDatabaseContext context, long? excludingItemId = null) =>
		from component in context.GameItemComponents.AsNoTracking()
		join prototype in context.GameItemComponentProtos.AsNoTracking()
			on new { Id = component.GameItemComponentProtoId, Revision = component.GameItemComponentProtoRevision }
			equals new { prototype.Id, Revision = prototype.RevisionNumber }
		where (prototype.Type == "Corpse" || prototype.Type == "Bodypart") &&
			(excludingItemId == null || component.GameItemId != excludingItemId)
		select new ComponentRow(component.Id, component.GameItemId, prototype.Type, component.Definition);

	public static bool PersistedReferencesAreClear(FuturemudDatabaseContext context, PhysicalReferenceTargets targets,
		IReadOnlyCollection<long> removedCharacters, IReadOnlyCollection<long> removedBodies,
		IReadOnlyCollection<long> removedInstances, long? excludingItemId, out string diagnostic)
	{
		var characterIds = targets.Ids(PhysicalEntityKind.Character);
		var instanceIds = targets.Ids(PhysicalEntityKind.CharacterInstance);
		foreach (var kind in new[] { PhysicalEntityKind.Character, PhysicalEntityKind.Body, PhysicalEntityKind.CharacterInstance, PhysicalEntityKind.GameItem })
		{
			var ids = targets.Ids(kind);
			if (ids.Length == 0) continue;
			var type = kind.ToString();
			var item = context.GameItems.AsNoTracking().Where(x => excludingItemId == null || x.Id != excludingItemId)
				.FirstOrDefault(x => x.OwnerType == type && ids.Contains(x.OwnerId ?? 0) ||
					x.PositionTargetType == type && ids.Contains(x.PositionTargetId ?? 0));
			if (item is not null)
			{
				diagnostic = $"GameItem {item.Id}: {type} owner/position reference retains a physical entity.";
				return false;
			}
			var actor = context.Characters.AsNoTracking().Where(x => !removedCharacters.Contains(x.Id))
				.FirstOrDefault(x => x.PositionTargetType == type && ids.Contains(x.PositionTargetId ?? 0));
			if (actor is not null)
			{
				diagnostic = $"Character {actor.Id}: PositionTargetType/PositionTargetId retains {type}.";
				return false;
			}
			var instance = context.CharacterInstances.AsNoTracking().Where(x => !removedInstances.Contains(x.Id))
				.FirstOrDefault(x => x.PositionTargetType == type && ids.Contains(x.PositionTargetId ?? 0));
			if (instance is not null)
			{
				diagnostic = $"CharacterInstance {instance.Id}: PositionTargetType/PositionTargetId retains {type}.";
				return false;
			}
		}

		long? LegacyBody(long characterId) => context.Characters.AsNoTracking()
			.Where(x => x.Id == characterId && !x.IsArchived).Select(x => x.BodyId).SingleOrDefault();
		var itemIds = targets.Ids(PhysicalEntityKind.GameItem);
		if (itemIds.Length > 0 && context.Crimes.AsNoTracking().Any(x => x.ThirdPartyIItemType == "GameItem" && itemIds.Contains(x.ThirdPartyId ?? 0)))
		{
			diagnostic = "Crime.ThirdPartyIItemType/ThirdPartyId retains the item.";
			return false;
		}
		if ((targets.HasKind(PhysicalEntityKind.Body) || targets.HasKind(PhysicalEntityKind.Wound)) && !Check(RemainsComponents(context, excludingItemId), "GameItemComponent",
			x => x.Id, x => x.Definition, x => PhysicalReferenceCodecs.Component(x.Type, x.Definition, LegacyBody), targets, out diagnostic)) return false;
		if (!Check(context.Characters.AsNoTracking().Where(x => !removedCharacters.Contains(x.Id))
			.Select(x => new Payload(x.Id, x.EffectData)), "Character.EffectData", x => x.Id, x => x.Value,
			x => PhysicalReferenceCodecs.Effects(x.Value, targets: targets), targets, out diagnostic)) return false;
		if (!Check(context.Bodies.AsNoTracking().Where(x => !removedBodies.Contains(x.Id))
			.Select(x => new Payload(x.Id, x.EffectData)), "Body.EffectData", x => x.Id, x => x.Value,
			x => PhysicalReferenceCodecs.Effects(x.Value, targets: targets), targets, out diagnostic)) return false;
		if (!Check(context.CharacterInstances.AsNoTracking().Where(x => !removedInstances.Contains(x.Id))
			.Select(x => new Payload(x.Id, x.EffectData)), "CharacterInstance.EffectData", x => x.Id, x => x.Value,
			x => PhysicalReferenceCodecs.Effects(x.Value, instanceMetadata: true, targets: targets), targets, out diagnostic)) return false;
		if (!Check(context.GameItems.AsNoTracking().Where(x => excludingItemId == null || x.Id != excludingItemId)
			.Select(x => new Payload(x.Id, x.EffectData)), "GameItem.EffectData", x => x.Id, x => x.Value,
			x => PhysicalReferenceCodecs.Effects(x.Value, targets: targets), targets, out diagnostic)) return false;
		if (!Check(context.Rooms.AsNoTracking().Select(x => new Payload(x.Id, x.EffectData)), "Room.EffectData", x => x.Id, x => x.Value,
			x => PhysicalReferenceCodecs.Effects(x.Value, targets: targets), targets, out diagnostic)) return false;
		if (characterIds.Length > 0 || instanceIds.Length > 0)
		{
			if (!Check(context.GroupAis.AsNoTracking().Select(x => new Payload(x.Id, x.Definition)), "GroupAi.Definition", x => x.Id, x => x.Value,
				x => PhysicalReferenceCodecs.Group(x.Value!), targets, out diagnostic)) return false;
			if (!Check(context.ActiveRouteMotions.AsNoTracking().Select(x => new Payload(x.Id, x.StateData)), "ActiveRouteMotion.StateData", x => x.Id, x => x.Value,
				x => PhysicalReferenceCodecs.RouteMotion(x.Value), targets, out diagnostic)) return false;
		}
		if (characterIds.Length > 0)
		{
			if (!Check(context.CharacterComputerProgramProcesses.AsNoTracking().Where(x => x.WaitType == (int)ComputerProcessWaitType.UserInput && x.EndedAtUtc == null)
				.Select(x => new Payload(x.Id, x.WaitArgument)), "CharacterComputerProgramProcess.WaitArgument", x => x.Id, x => x.Value,
				x => PhysicalReferenceCodecs.UserInput(x.Value), targets, out diagnostic)) return false;
			if (!CharacterVariableTypes(context.VariableValues.Select(x => x.ValueTypeDefinition), out var valueTypes, out diagnostic)) return false;
			if (!Check(context.VariableValues.AsNoTracking().Where(x => valueTypes.Contains(x.ValueTypeDefinition)), "VariableValue.ValueDefinition", x => $"{x.ReferenceTypeDefinition}/{x.ReferenceId}/{x.ReferenceProperty}", x => x.ValueDefinition,
				x => PhysicalReferenceCodecs.Variable(x.ValueTypeDefinition, x.ValueDefinition), targets, out diagnostic)) return false;
			var defaults = from value in context.VariableDefaults.AsNoTracking()
				join definition in context.VariableDefinitions.AsNoTracking()
					on new { value.OwnerTypeDefinition, Name = value.Property }
					equals new { definition.OwnerTypeDefinition, Name = definition.Property }
				select new { value.OwnerTypeDefinition, value.Property, value.DefaultValue, definition.ContainedTypeDefinition };
			if (!CharacterVariableTypes(context.VariableDefinitions.Select(x => x.ContainedTypeDefinition), out var defaultTypes, out diagnostic)) return false;
			if (!Check(defaults.Where(x => defaultTypes.Contains(x.ContainedTypeDefinition)), "VariableDefault.DefaultValue", x => $"{x.OwnerTypeDefinition}/{x.Property}", x => x.DefaultValue,
				x => PhysicalReferenceCodecs.Variable(x.ContainedTypeDefinition, x.DefaultValue), targets, out diagnostic)) return false;
		}
		diagnostic = string.Empty;
		return true;
	}

	private static bool CharacterVariableTypes(IQueryable<string> definitions, out string[] types, out string diagnostic)
	{
		types = [];
		var declared = definitions.Distinct().Take(RowLimit + 1).ToArray();
		if (declared.Length > RowLimit)
		{
			diagnostic = "Variable type definitions exceed the bounded row limit.";
			return false;
		}
		try
		{
			types = declared.Where(PhysicalReferenceCodecs.IsCharacterVariableType).ToArray();
			diagnostic = string.Empty;
			return true;
		}
		catch (FormatException)
		{
			diagnostic = "Variable declared type is malformed; its reference contract cannot be determined.";
			return false;
		}
	}

	private static bool Check<T>(IQueryable<T> query, string source, Func<T, object> id, Func<T, string?> payload,
		Func<T, IEnumerable<PhysicalEntityReference>> references, PhysicalReferenceTargets targets, out string diagnostic)
	{
		var rows = query.Take(RowLimit + 1).ToArray();
		if (rows.Length > RowLimit)
		{
			diagnostic = $"{source}: the known reference channel exceeds the bounded row limit.";
			return false;
		}
		foreach (var row in rows)
		{
			try
			{
				if (payload(row)?.Length > PayloadLimit) throw new FormatException("Known reference payload exceeds its size limit.");
				foreach (var reference in references(row))
				{
					if (!targets.Includes(reference)) continue;
					diagnostic = $"{source} {id(row)}: {reference.Field} retains {reference.Kind} {reference.Id}.";
					return false;
				}
			}
			catch (Exception error) when (error is XmlException or FormatException or OverflowException or JsonException or InvalidOperationException or KeyNotFoundException)
			{
				diagnostic = $"{source} {id(row)}: malformed known reference contract ({error.Message}).";
				return false;
			}
		}
		diagnostic = string.Empty;
		return true;
	}

	public static bool HasLiveReference(IEnumerable<IEffect> effects, PhysicalReferenceTargets targets) =>
		effects.OfType<IPhysicalEntityReferenceProvider>().Any(x => x.PhysicalReferences.Any(targets.Includes));

	public static bool RuntimeReferencesAreClear(IFuturemud world, PhysicalReferenceTargets targets)
	{
		var actors = world.Actors.Concat(world.CachedActors).Concat(world.Characters)
			.SelectMany(x => x.Identity.Instances.OfType<ICharacter>()).DistinctBy(x => x.InstanceId);
		return !actors.Any(x => HasLiveReference(x.Effects, targets) || HasLiveReference(x.Body.Effects, targets)) &&
			!world.Bodies.Any(x => HasLiveReference(x.Effects, targets)) &&
			!world.Items.Any(x => !x.Deleted && HasLiveReference(x.Effects, targets)) &&
			!world.Rooms.Any(x => HasLiveReference(x.Effects, targets));
	}
}
