#nullable enable

using MudSharp.Computers;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.GameItems.Components;

public static class SignalComponentUtilities
{
	public const string DefaultLocalSignalEndpointKey = "signal";
	public static string SignalGeneratorTag => "[signal generator]".Colour(Telnet.BoldCyan);
	public static string SignalConsumerTag => "[signal consumer]".Colour(Telnet.BoldMagenta);

	public static bool SignalsEqual(ComputerSignal lhs, ComputerSignal rhs)
	{
		return Math.Abs(lhs.Value - rhs.Value) < 0.0000001 &&
		       lhs.Duration == rhs.Duration &&
		       lhs.PulseInterval == rhs.PulseInterval;
	}

	public static string NormaliseSignalEndpointKey(string? endpointKey)
	{
		return string.IsNullOrWhiteSpace(endpointKey)
			? DefaultLocalSignalEndpointKey
			: endpointKey.Trim().ToLowerInvariant();
	}

	public static bool IsActiveSignal(double value, double threshold, bool activeWhenAboveThreshold)
	{
		return activeWhenAboveThreshold
			? value >= threshold
			: value < threshold;
	}

	public static bool TryResolveSignalComponentPrototype(IFuturemud gameworld, string componentIdentifier,
		out IGameItemComponentProto? prototype)
	{
		prototype = null;
		if (string.IsNullOrWhiteSpace(componentIdentifier))
		{
			return false;
		}

		prototype = long.TryParse(componentIdentifier, out var prototypeId)
			? gameworld.ItemComponentProtos.Get(prototypeId)
			: gameworld.ItemComponentProtos.GetByName(componentIdentifier);
		return prototype is not null;
	}

	public static string DescribeSignalComponent(IFuturemud gameworld, long sourceComponentId, string sourceComponentName)
	{
		return DescribeSignalComponent(gameworld, sourceComponentId, sourceComponentName,
			DefaultLocalSignalEndpointKey);
	}

	public static LocalSignalBinding CreateBinding(ISignalSourceComponent source, string? endpointKey = null)
	{
		var component = (IGameItemComponent)source;
		return new LocalSignalBinding(
			component.Parent.Id,
			component.Parent.Name,
			component.Id,
			component.Name,
			NormaliseSignalEndpointKey(endpointKey ?? source.EndpointKey));
	}

	public static string DescribeSignalComponent(LocalSignalBinding binding)
	{
		var itemDescription = string.IsNullOrWhiteSpace(binding.SourceItemName)
			? binding.SourceItemId > 0
				? $"#{binding.SourceItemId:N0}"
				: string.Empty
			: binding.SourceItemName;
		var componentDescription = string.IsNullOrWhiteSpace(binding.SourceComponentName)
			? binding.SourceComponentId > 0
				? $"#{binding.SourceComponentId:N0}"
				: "unknown"
			: binding.SourceComponentName;
		return string.IsNullOrWhiteSpace(itemDescription)
			? $"{componentDescription}:{NormaliseSignalEndpointKey(binding.SourceEndpointKey)}"
			: $"{itemDescription}@{componentDescription}:{NormaliseSignalEndpointKey(binding.SourceEndpointKey)}";
	}

	public static string DescribeSignalComponent(IFuturemud gameworld, long sourceComponentId, string sourceComponentName,
		string? sourceEndpointKey)
	{
		var endpointKey = NormaliseSignalEndpointKey(sourceEndpointKey);
		string? componentDescription = null;
		if (sourceComponentId > 0)
		{
			var prototype = gameworld.ItemComponentProtos.Get(sourceComponentId);
			if (prototype is not null)
			{
				componentDescription = prototype.Name;
			}
		}

		componentDescription ??= !string.IsNullOrWhiteSpace(sourceComponentName)
			? sourceComponentName
			: sourceComponentId > 0
				? $"#{sourceComponentId.ToString("N0")}"
				: "unknown";
		return $"{componentDescription}:{endpointKey}";
	}

	public static IGameItem ResolveSignalSearchAnchorItem(IGameItem item)
	{
		return item.GetItemType<IAutomationMountable>() is IAutomationMountable { MountHost: not null } mountable
			? mountable.MountHost.Parent
			: item;
	}

	public static IEnumerable<ICell> EnumerateSignalAccessibilityCells(IGameItem item)
	{
		var anchorItem = ResolveSignalSearchAnchorItem(item);
		return anchorItem.TrueLocations
			.OfType<ICell>()
			.Concat(item.TrueLocations.OfType<ICell>())
			.Distinct();
	}

	public static IEnumerable<IGameItem> EnumerateAccessibleSignalItems(IGameItem parent)
	{
		var rootItems = new List<IGameItem> { parent };
		var anchorItem = ResolveSignalSearchAnchorItem(parent);
		if (!ReferenceEquals(anchorItem, parent))
		{
			rootItems.Add(anchorItem);
		}

		rootItems.AddRange(parent.AttachedAndConnectedItems);
		if (!ReferenceEquals(anchorItem, parent))
		{
			rootItems.AddRange(anchorItem.AttachedAndConnectedItems);
		}

		var roomLayers = rootItems
			.Select(x => x.RoomLayer)
			.Distinct()
			.ToList();
		foreach (var cell in EnumerateSignalAccessibilityCells(parent))
		{
			foreach (var roomLayer in roomLayers)
			{
				rootItems.AddRange(cell.LayerGameItems(roomLayer));
			}
		}

		return rootItems.Distinct();
	}

	public static bool ItemsAreSignalAccessible(IGameItem origin, IGameItem target)
	{
		if (ReferenceEquals(origin, target))
		{
			return true;
		}

		if (origin.AttachedAndConnectedItems.Contains(target) || target.AttachedAndConnectedItems.Contains(origin))
		{
			return true;
		}

		var originCells = EnumerateSignalAccessibilityCells(origin).ToList();
		var targetCells = EnumerateSignalAccessibilityCells(target).ToList();
		return originCells.Intersect(targetCells).Any();
	}

	public static ISignalSourceComponent? FindSignalSource(IGameItem parent, LocalSignalBinding binding,
		IGameItemComponent? excludedComponent = null)
	{
		return FindSignalSource(parent, binding.SourceItemId, binding.SourceItemName, binding.SourceComponentId,
			binding.SourceComponentName, binding.SourceEndpointKey, excludedComponent);
	}

	public static ISignalSourceComponent? FindSignalSourceOnItem(IGameItem item, long sourceComponentId,
		string sourceComponentName, string endpointKey, IGameItemComponent? excludedComponent = null)
	{
		return item.GetItemTypes<ISignalSourceComponent>()
			.FirstOrDefault(x =>
				!ReferenceEquals(x, excludedComponent) &&
				x.EndpointKey.Equals(endpointKey, StringComparison.InvariantCultureIgnoreCase) &&
				(sourceComponentId > 0
					? ((IGameItemComponent)x).Id == sourceComponentId || x.LocalSignalSourceIdentifier == sourceComponentId
					: ((IGameItemComponent)x).Name.Equals(sourceComponentName, StringComparison.InvariantCultureIgnoreCase)));
	}

	public static ISignalSourceComponent? FindSignalSource(IGameItem parent, long sourceComponentId,
		string sourceComponentName, string? sourceEndpointKey = null,
		IGameItemComponent? excludedComponent = null)
	{
		return FindSignalSource(parent, 0L, string.Empty, sourceComponentId, sourceComponentName, sourceEndpointKey,
			excludedComponent);
	}

	public static ISignalSourceComponent? FindSignalSource(IGameItem parent, long sourceItemId, string sourceItemName,
		long sourceComponentId, string sourceComponentName, string? sourceEndpointKey = null,
		IGameItemComponent? excludedComponent = null)
	{
		if (sourceComponentId <= 0 && string.IsNullOrWhiteSpace(sourceComponentName))
		{
			return null;
		}

		var endpointKey = NormaliseSignalEndpointKey(sourceEndpointKey);
		if (sourceItemId > 0)
		{
			var item = parent.Gameworld.TryGetItem(sourceItemId, true);
			if (item is not null && ItemsAreSignalAccessible(parent, item))
			{
				var itemMatch = FindSignalSourceOnItem(item, sourceComponentId, sourceComponentName, endpointKey,
					excludedComponent);
				if (itemMatch is not null)
				{
					return itemMatch;
				}
			}
		}

		foreach (var item in EnumerateAccessibleSignalItems(parent).Distinct())
		{
			var match = FindSignalSourceOnItem(item, sourceComponentId, sourceComponentName, endpointKey,
				excludedComponent);
			if (match is not null)
			{
				return match;
			}
		}

		return null;
	}

	public static TTarget? FindSiblingComponent<TTarget>(IGameItem parent, string componentName,
		IGameItemComponent? excludedComponent = null)
		where TTarget : class, IGameItemComponent
	{
		if (string.IsNullOrWhiteSpace(componentName))
		{
			return null;
		}

		return parent.GetItemTypes<TTarget>()
			.FirstOrDefault(x =>
				!ReferenceEquals(x, excludedComponent) &&
				x.Name.Equals(componentName, StringComparison.InvariantCultureIgnoreCase));
	}
}
