#nullable enable

using MudSharp.Computers;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using System;
using System.Linq;

namespace MudSharp.GameItems.Components;

public static class SignalComponentUtilities
{
	public const string DefaultLocalSignalEndpointKey = "signal";

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
		return new LocalSignalBinding(
			((IGameItemComponent)source).Id,
			((IGameItemComponent)source).Name,
			NormaliseSignalEndpointKey(endpointKey ?? source.EndpointKey));
	}

	public static string DescribeSignalComponent(LocalSignalBinding binding)
	{
		return $"{(string.IsNullOrWhiteSpace(binding.SourceComponentName) ? $"#{binding.SourceComponentId.ToString("N0")}" : binding.SourceComponentName)}:{NormaliseSignalEndpointKey(binding.SourceEndpointKey)}";
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

	public static ISignalSourceComponent? FindSignalSource(IGameItem parent, long sourceComponentId,
		string sourceComponentName, string? sourceEndpointKey = null,
		IGameItemComponent? excludedComponent = null)
	{
		if (sourceComponentId <= 0 && string.IsNullOrWhiteSpace(sourceComponentName))
		{
			return null;
		}

		var endpointKey = NormaliseSignalEndpointKey(sourceEndpointKey);
		return parent.GetItemTypes<ISignalSourceComponent>()
			.FirstOrDefault(x =>
				!ReferenceEquals(x, excludedComponent) &&
				x.EndpointKey.Equals(endpointKey, StringComparison.InvariantCultureIgnoreCase) &&
				(sourceComponentId > 0
					? ((IGameItemComponent)x).Id == sourceComponentId || x.LocalSignalSourceIdentifier == sourceComponentId
					: ((IGameItemComponent)x).Name.Equals(sourceComponentName, StringComparison.InvariantCultureIgnoreCase)));
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
