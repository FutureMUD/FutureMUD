#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.GameItems;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using MudSharp.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private GameItemProto EnsureVehicleProjectionItem(
		VehicleProjectionItemSeedSpec spec,
		GameItemComponentProto projectionComponent)
	{
		var components = spec.Components
			.Append(projectionComponent.Name)
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToArray();
		var projectedSpec = spec with { Components = components };
		var item = CreateItem(
			projectedSpec.StableReference,
			projectedSpec.Noun,
			projectedSpec.ShortDescription,
			projectedSpec.LongDescription,
			projectedSpec.FullDescription,
			projectedSpec.Size,
			projectedSpec.Quality,
			projectedSpec.WeightInGrams,
			projectedSpec.Cost,
			projectedSpec.Skinnable,
			projectedSpec.HiddenFromPlayers,
			projectedSpec.Material,
			projectedSpec.Tags,
			projectedSpec.Components,
			null,
			null,
			null,
			null,
			projectedSpec.BuilderNotes,
			allowLegacyShortDescriptionMatch: false);
		if (item is null)
		{
			throw new InvalidOperationException($"Unable to seed vehicle projection item {projectedSpec.StableReference}.");
		}

		ApplyVehicleItemDefinition(item, projectedSpec);
		EnsureVehicleItemComponent(item, projectionComponent);
		return item;
	}

	private void ApplyVehicleItemDefinition(GameItemProto item, VehicleProjectionItemSeedSpec spec)
	{
		if (!_materials.TryGetValue(spec.Material, out var material))
		{
			throw new InvalidOperationException($"Vehicle item {spec.StableReference} references missing material {spec.Material}.");
		}

		item.Name = spec.Noun.ToLowerInvariant();
		item.UniqueName = spec.StableReference;
		item.BuilderNotes = BuildReworkItemBuilderNotes(spec.StableReference, spec.Tags, spec.BuilderNotes);
		item.Keywords = string.Join(" ", Regex.Matches($"{spec.Noun} {spec.ShortDescription}", "[A-Za-z0-9'-]+")
			.Cast<Match>()
			.Select(x => x.Value.ToLowerInvariant())
			.Where(x => x is not ("a" or "an" or "the"))
			.Distinct(StringComparer.OrdinalIgnoreCase));
		item.MaterialId = material.Id;
		item.Size = (int)spec.Size;
		item.Weight = spec.WeightInGrams;
		item.LongDescription = spec.LongDescription ?? DefaultVehicleLongDescription(spec.ShortDescription);
		item.ShortDescription = spec.ShortDescription;
		item.FullDescription = spec.FullDescription;
		item.BaseItemQuality = (int)spec.Quality;
		item.PermitPlayerSkins = spec.Skinnable;
		item.CostInBaseCurrency = spec.Cost;
		item.IsHiddenFromPlayers = spec.HiddenFromPlayers;
		item.ReadOnly = false;
		item.HighPriority = false;
		item.MorphGameItemProtoId = null;
		item.MorphTimeSeconds = 0;
		item.MorphEmote = string.Empty;
		item.OnDestroyedGameItemProtoId = null;
		item.PreserveRegisterVariables = false;
		item.PlanarData = string.Empty;

		var desiredTags = new List<Tag>();
		foreach (var tagPath in spec.Tags)
		{
			if (!_tagsByFullPath.TryGetValue(tagPath, out var tag))
			{
				throw new InvalidOperationException($"Vehicle item {spec.StableReference} references missing tag {tagPath}.");
			}

			desiredTags.Add(tag);
		}

		var desiredComponents = new List<GameItemComponentProto>();
		foreach (var componentName in spec.Components)
		{
			if (!_components.TryGetValue(componentName, out var component))
			{
				throw new InvalidOperationException(
					$"Vehicle item {spec.StableReference} references missing component {componentName}.");
			}

			desiredComponents.Add(component);
		}

		ReconcileVehicleItemTags(item, desiredTags);
		ReconcileVehicleItemComponents(item, desiredComponents);
	}

	private void ReconcileVehicleItemTags(GameItemProto item, IReadOnlyCollection<Tag> desiredTags)
	{
		var desiredIds = desiredTags.Select(x => x.Id).ToHashSet();
		var joins = item.GameItemProtosTags
			.Concat(_context!.GameItemProtosTags.Local
				.Where(x => x.GameItemProtoId == item.Id && x.GameItemProtoRevisionNumber == item.RevisionNumber))
			.Concat(_context.GameItemProtosTags
				.Where(x => x.GameItemProtoId == item.Id && x.GameItemProtoRevisionNumber == item.RevisionNumber)
				.AsEnumerable())
			.Distinct()
			.ToList();
		foreach (var group in joins.GroupBy(x => x.TagId))
		{
			var rows = group.ToArray();
			if (!desiredIds.Contains(group.Key))
			{
				_context.GameItemProtosTags.RemoveRange(rows);
				continue;
			}

			if (rows.Length > 1)
			{
				_context.GameItemProtosTags.RemoveRange(rows.Skip(1));
			}
		}

		foreach (var tag in desiredTags)
		{
			EnsureVehicleItemTag(item, tag);
		}
	}

	private void ReconcileVehicleItemComponents(
		GameItemProto item,
		IReadOnlyCollection<GameItemComponentProto> desiredComponents)
	{
		var desiredKeys = desiredComponents
			.Select(x => (x.Id, x.RevisionNumber))
			.ToHashSet();
		var joins = item.GameItemProtosGameItemComponentProtos
			.Concat(_context!.GameItemProtosGameItemComponentProtos.Local
				.Where(x => x.GameItemProtoId == item.Id && x.GameItemProtoRevision == item.RevisionNumber))
			.Concat(_context.GameItemProtosGameItemComponentProtos
				.Where(x => x.GameItemProtoId == item.Id && x.GameItemProtoRevision == item.RevisionNumber)
				.AsEnumerable())
			.Distinct()
			.ToList();
		foreach (var group in joins.GroupBy(x => (x.GameItemComponentProtoId, x.GameItemComponentRevision)))
		{
			var rows = group.ToArray();
			if (!desiredKeys.Contains(group.Key))
			{
				_context.GameItemProtosGameItemComponentProtos.RemoveRange(rows);
				continue;
			}

			if (rows.Length > 1)
			{
				_context.GameItemProtosGameItemComponentProtos.RemoveRange(rows.Skip(1));
			}
		}

		foreach (var component in desiredComponents)
		{
			EnsureVehicleItemComponent(item, component);
		}
	}

	private void EnsureVehicleItemTag(GameItemProto item, Tag tag)
	{
		var local = item.GameItemProtosTags.Any(x => x.TagId == tag.Id) ||
		            _context!.GameItemProtosTags.Local.Any(x =>
			            x.GameItemProtoId == item.Id &&
			            x.GameItemProtoRevisionNumber == item.RevisionNumber &&
			            x.TagId == tag.Id);
		var persisted = local || _context.GameItemProtosTags.Any(x =>
			x.GameItemProtoId == item.Id &&
			x.GameItemProtoRevisionNumber == item.RevisionNumber &&
			x.TagId == tag.Id);
		if (persisted)
		{
			return;
		}

		_context.GameItemProtosTags.Add(new GameItemProtosTags
		{
			GameItemProtoId = item.Id,
			GameItemProtoRevisionNumber = item.RevisionNumber,
			TagId = tag.Id
		});
	}

	private void EnsureVehicleItemComponent(GameItemProto item, GameItemComponentProto component)
	{
		var local = item.GameItemProtosGameItemComponentProtos.Any(x =>
			            x.GameItemComponentProtoId == component.Id &&
			            x.GameItemComponentRevision == component.RevisionNumber) ||
		            _context!.GameItemProtosGameItemComponentProtos.Local.Any(x =>
			            x.GameItemProtoId == item.Id &&
			            x.GameItemProtoRevision == item.RevisionNumber &&
			            x.GameItemComponentProtoId == component.Id &&
			            x.GameItemComponentRevision == component.RevisionNumber);
		var persisted = local || _context.GameItemProtosGameItemComponentProtos.Any(x =>
			x.GameItemProtoId == item.Id &&
			x.GameItemProtoRevision == item.RevisionNumber &&
			x.GameItemComponentProtoId == component.Id &&
			x.GameItemComponentRevision == component.RevisionNumber);
		if (persisted)
		{
			return;
		}

		if (component.Type.Equals(VehicleExteriorComponentType, StringComparison.OrdinalIgnoreCase) ||
		    component.Type.Equals(VehicleAccessComponentType, StringComparison.OrdinalIgnoreCase) ||
		    component.Type.Equals(VehicleCargoComponentType, StringComparison.OrdinalIgnoreCase))
		{
			var conflicting = _context.GameItemProtosGameItemComponentProtos
				.Include(x => x.GameItemComponent)
				.AsEnumerable()
				.FirstOrDefault(x =>
					x.GameItemProtoId == item.Id &&
					x.GameItemProtoRevision == item.RevisionNumber &&
					x.GameItemComponent.Type.Equals(component.Type, StringComparison.OrdinalIgnoreCase) &&
					x.GameItemComponentProtoId != component.Id);
			if (conflicting is not null)
			{
				throw new InvalidOperationException(
					$"Vehicle item {item.UniqueName} is already linked to another {component.Type} component.");
			}
		}

		_context.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
		{
			GameItemProtoId = item.Id,
			GameItemProtoRevision = item.RevisionNumber,
			GameItemComponentProtoId = component.Id,
			GameItemComponentRevision = component.RevisionNumber
		});
	}

	private GameItemComponentProto EnsureVehicleComponentPrototype(
		string name,
		string type,
		string description,
		string definition)
	{
		if (_components.TryGetValue(name, out var cached))
		{
			if (!cached.Type.Equals(type, StringComparison.OrdinalIgnoreCase))
			{
				throw new InvalidOperationException(
					$"Component name {name} is already used by type {cached.Type}, not required vehicle type {type}.");
			}

			cached.Description = description;
			cached.Definition = definition;
			return cached;
		}

		var existing = _context!.GameItemComponentProtos
			.Include(x => x.EditableItem)
			.AsEnumerable()
			.Where(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
			.OrderByDescending(x => x.EditableItem.RevisionStatus == 4)
			.ThenByDescending(x => x.RevisionNumber)
			.FirstOrDefault();
		if (existing is not null)
		{
			if (!existing.Type.Equals(type, StringComparison.OrdinalIgnoreCase))
			{
				throw new InvalidOperationException(
					$"Component name {name} is already used by type {existing.Type}, not required vehicle type {type}.");
			}

			existing.Description = description;
			existing.Definition = definition;
			_components[name] = existing;
			return existing;
		}

		var component = new GameItemComponentProto
		{
			Id = NextVehicleComponentPrototypeId(),
			RevisionNumber = 0,
			EditableItem = NewCurrentVehicleEditableItem($"Vehicle seeder component: {name}"),
			Type = type,
			Name = name,
			Description = description,
			Definition = definition
		};
		_context.GameItemComponentProtos.Add(component);
		_components[name] = component;
		return component;
	}

}
